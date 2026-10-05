// MIT License - see LICENSE file in the repository root.

using System.Text;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using MDReader.Core.Markdown;

namespace MDReader.Core.Markdown;

/// <summary>
/// Converts a Markdig syntax tree into the platform-neutral
/// <see cref="MarkdownElement"/> model. This is the single place that knows
/// about Markdig's API; the rest of Core and all of App depend only on the
/// element model, keeping the platform boundary clean.
/// </summary>
internal sealed class MarkdownAstBuilder
{
    /// <summary>
    /// Builds the default Markdig pipeline (with GitHub-flavored extensions:
    /// tables, task lists, strikethrough, etc.) and parses the source text into
    /// a platform-neutral document element tree.
    /// </summary>
    /// <param name="sourceText">The raw Markdown source text.</param>
    /// <returns>A parsed document element tree.</returns>
    public static MarkdownDocumentElement Build(string sourceText)
    {
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        var markdigDoc = Markdig.Markdown.Parse(sourceText, pipeline);
        return BuildDocument(markdigDoc);
    }

    /// <summary>
    /// Converts a Markdig syntax tree into the element model.
    /// </summary>
    public static MarkdownDocumentElement BuildDocument(Markdig.Syntax.MarkdownDocument markdigDoc)
    {
        var blocks = new List<MarkdownBlockElement>();
        foreach (var block in markdigDoc)
        {
            var converted = ConvertBlock(block);
            if (converted is not null)
                blocks.Add(converted);
        }

        return new MarkdownDocumentElement(blocks);
    }

    /// <summary>
    /// Converts a single Markdig block, returning null for unsupported/inert
    /// node types so the caller can degrade safely.
    /// </summary>
    private static MarkdownBlockElement? ConvertBlock(Block? block)
    {
        if (block is null)
            return null;

        switch (block)
        {
            case HeadingBlock h:
                return new MarkdownHeadingElement(h.Level, ConvertInlines(h.Inline));

            case ParagraphBlock p:
                return new MarkdownParagraphElement(ConvertInlines(p.Inline));

            case FencedCodeBlock fcb:
                return new MarkdownCodeBlockElement(fcb.Lines.ToString(), fcb.Info, isFenced: true);

            case CodeBlock ccb: // indented code block (base type, not fenced)
                return new MarkdownCodeBlockElement(ccb.Lines.ToString(), language: null, isFenced: false);

            case ListBlock lb:
                return ConvertList(lb);

            case QuoteBlock qb:
                return new MarkdownBlockquoteElement(ConvertChildBlocks(qb));

            case ThematicBreakBlock:
                return new MarkdownThematicBreakElement();

            case HtmlBlock hb:
                // Raw HTML is inert — never executed by the renderer. The raw
                // text (from the inherited LeafBlock.Lines StringLineGroup) is
                // retained for diagnostics only; the App layer renders it as
                // plain text or omits it, never as live markup.
                return new MarkdownHtmlElement(hb.Lines.ToString() ?? string.Empty);

            default:
                // Tables are resolved by type name so Core does not depend on
                // the Markdig.Extensions.Tables assembly at compile time.
                if (block.GetType().FullName is "Markdig.Extensions.Tables.Table")
                    return ConvertTable(block);

                // Any other block type degrades safely (skipped).
                return null;
        }
    }

    /// <summary>
    /// Returns the direct block children of a container block by filtering the
    /// recursive <see cref="MarkdownObjectExtensions.Descendants"/> sequence to
    /// those whose <see cref="Block.Parent"/> is the container itself.
    /// </summary>
    private static List<MarkdownBlockElement> ConvertChildBlocks(ContainerBlock container)
    {
        var result = new List<MarkdownBlockElement>();
        foreach (var descendant in container.Descendants())
        {
            if (descendant is Block child && child.Parent == container && child != container)
            {
                var converted = ConvertBlock(child);
                if (converted is not null)
                    result.Add(converted);
            }
        }
        return result;
    }

    private static MarkdownListElement ConvertList(ListBlock lb)
    {
        var items = new List<MarkdownListItemElement>();
        foreach (var descendant in lb.Descendants())
        {
            if (descendant is ListItemBlock lib && lib.Parent == lb)
            {
                items.Add(new MarkdownListItemElement(ConvertChildBlocks(lib)));
            }
        }

        // ListBlock.OrderedStart is a string in Markdig 0.40.0 (e.g. "1"); parse
        // it to an int, defaulting to 1 for ordered lists.
        int start = 1;
        if (lb.IsOrdered && int.TryParse(lb.OrderedStart, out var parsed) && parsed >= 1)
            start = parsed;

        return new MarkdownListElement(lb.IsOrdered, start, items);
    }

    private static MarkdownTableElement ConvertTable(Block table)
    {
        // Table rows are the direct block children of the Table.
        var rows = GetDirectChildBlocks(table);
        var headers = new List<string>();
        var bodyRows = new List<List<string>>();
        var isHeader = true;

        foreach (var row in rows)
        {
            var cellTexts = new List<string>();
            // Each TableRow's direct children are TableCell blocks.
            foreach (var cell in GetDirectChildBlocks(row))
            {
                cellTexts.Add(BlockToPlainText(cell));
            }

            if (isHeader)
            {
                headers = cellTexts;
                isHeader = false;
            }
            else
            {
                bodyRows.Add(cellTexts);
            }
        }

        return new MarkdownTableElement(headers, bodyRows);
    }

    /// <summary>
    /// Returns the direct block children of a container block.
    /// </summary>
    private static List<Block> GetDirectChildBlocks(Block container)
    {
        var result = new List<Block>();
        foreach (var descendant in container.Descendants())
        {
            if (descendant is Block child && child.Parent == container && child != container)
                result.Add(child);
        }
        return result;
    }

    /// <summary>
    /// Extracts plain text from a block (used for table cells which hold a
    /// paragraph of inline text).
    /// </summary>
    private static string BlockToPlainText(Block block)
    {
        if (block is ParagraphBlock p)
            return InlineToPlainText(p.Inline);

        // Fallback: any other block — flatten its descendants' inline text.
        var sb = new StringBuilder();
        foreach (var inline in block.Descendants().OfType<Inline>())
            sb.Append(InlineToPlainText(inline));
        return sb.ToString();
    }

    /// <summary>
    /// Converts a Markdig inline tree into the platform-neutral inline model.
    /// </summary>
    private static IReadOnlyList<MarkdownInlineElement> ConvertInlines(ContainerInline? container)
    {
        var result = new List<MarkdownInlineElement>();
        if (container is null)
            return result;

        foreach (var child in container)
        {
            ConvertInline(child, result);
        }

        return result;
    }

    /// <summary>
    /// Converts a single Markdig inline into one or more element inlines.
    /// </summary>
    private static void ConvertInline(Inline inline, List<MarkdownInlineElement> result)
    {
        switch (inline)
        {
            case CodeInline ci:
                result.Add(new MarkdownCodeInline(ci.Content ?? string.Empty));
                break;

            case EmphasisInline ei:
                var emphasisType = EmphasisTypeFromDelimiter(ei);
                result.Add(new MarkdownEmphasisInline(emphasisType, ConvertInlines(ei)));
                break;

            case LinkInline li when li.IsImage:
                result.Add(new MarkdownImageInline(li.Url ?? string.Empty, InlineToPlainText(li)));
                break;

            case LinkInline li:
                result.Add(new MarkdownLinkInline(li.Url ?? string.Empty, ConvertInlines(li), li.Title));
                break;

            case LiteralInline:
                // LiteralInline exposes its text via ToString() (no public Content
                // property exists in Markdig 0.40.0); verified by API probing.
                result.Add(new MarkdownLiteralInline(inline.ToString() ?? string.Empty));
                break;

            case ContainerInline ci:
                // Generic inline container — recurse into its children.
                foreach (var child in ci)
                    ConvertInline(child, result);
                break;

            default:
                // Unknown inline — degrade to a literal run of its text.
                result.Add(new MarkdownLiteralInline(inline.ToString() ?? string.Empty));
                break;
        }
    }

    /// <summary>
    /// Maps a Markdig <see cref="EmphasisInline"/> to the element emphasis type
    /// using the delimiter character and count (DelimiterCount==2 ⇒ bold,
    /// DelimiterCount==1 ⇒ italic, '~' ⇒ strikethrough).
    /// </summary>
    private static MarkdownEmphasisType EmphasisTypeFromDelimiter(EmphasisInline ei)
    {
        return ei.DelimiterChar switch
        {
            '~' => MarkdownEmphasisType.Strikethrough,
            '*' or '_' when ei.DelimiterCount >= 2 => MarkdownEmphasisType.Bold,
            '*' or '_' => MarkdownEmphasisType.Italic,
            _ => MarkdownEmphasisType.Italic
        };
    }

    /// <summary>
    /// Extracts plain text from an inline (used for image alt text and table cells).
    /// </summary>
    private static string InlineToPlainText(Inline? inline)
    {
        if (inline is null)
            return string.Empty;

        if (inline is CodeInline ci)
            return ci.Content ?? string.Empty;

        if (inline is ContainerInline container)
        {
            var sb = new StringBuilder();
            foreach (var child in container)
                sb.Append(InlineToPlainText(child));
            return sb.ToString();
        }

        return inline.ToString() ?? string.Empty;
    }
}
