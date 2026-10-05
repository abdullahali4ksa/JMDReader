// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Markdown;

/// <summary>
/// Kind of inline emphasis, derived from the Markdig delimiter.
/// </summary>
public enum MarkdownEmphasisType
{
    /// <summary>Bold emphasis (e.g. <c>**text**</c> or <c>__text__</c>).</summary>
    Bold,

    /// <summary>Italic emphasis (e.g. <c>*text*</c> or <c>_text_</c>).</summary>
    Italic,

    /// <summary>Strikethrough (e.g. <c>~~text~~</c>).</summary>
    Strikethrough,
}

/// <summary>
/// Root of the platform-neutral Markdown AST. Contains the ordered list of
/// top-level block elements parsed from the source text.
/// </summary>
public sealed class MarkdownDocumentElement : MarkdownBlockElement
{
    /// <summary>Gets the ordered list of top-level block elements.</summary>
    public IReadOnlyList<MarkdownBlockElement> Blocks { get; }

    /// <summary>Creates a document element from the given block list.</summary>
    public MarkdownDocumentElement(IReadOnlyList<MarkdownBlockElement> blocks) => Blocks = blocks;
}

/// <summary>
/// A heading block (e.g. <c># Heading</c>). <see cref="Level"/> is 1–6.
/// </summary>
public sealed class MarkdownHeadingElement : MarkdownBlockElement
{
    /// <summary>Gets the heading level (1 through 6).</summary>
    public int Level { get; }

    /// <summary>Gets the inline content of the heading.</summary>
    public IReadOnlyList<MarkdownInlineElement> Inlines { get; }

    /// <summary>Creates a heading element.</summary>
    public MarkdownHeadingElement(int level, IReadOnlyList<MarkdownInlineElement> inlines)
    {
        Level = level;
        Inlines = inlines;
    }
}

/// <summary>
/// A paragraph block of inline content.
/// </summary>
public sealed class MarkdownParagraphElement : MarkdownBlockElement
{
    /// <summary>Gets the inline content of the paragraph.</summary>
    public IReadOnlyList<MarkdownInlineElement> Inlines { get; }

    /// <summary>Creates a paragraph element.</summary>
    public MarkdownParagraphElement(IReadOnlyList<MarkdownInlineElement> inlines) => Inlines = inlines;
}

/// <summary>
/// A fenced or indented code block. The raw code text is available in
/// <see cref="Code"/>.
/// </summary>
public sealed class MarkdownCodeBlockElement : MarkdownBlockElement
{
    /// <summary>Gets the raw code text.</summary>
    public string Code { get; }

    /// <summary>Gets the optional language identifier (fenced blocks only), or null.</summary>
    public string? Language { get; }

    /// <summary>Gets a value indicating whether this is a fenced code block (true) or indented (false).</summary>
    public bool IsFenced { get; }

    /// <summary>Creates a code-block element.</summary>
    public MarkdownCodeBlockElement(string code, string? language = null, bool isFenced = true)
    {
        Code = code;
        Language = language;
        IsFenced = isFenced;
    }
}

/// <summary>
/// An ordered or unordered list. Each item may itself contain blocks
/// (including nested lists).
/// </summary>
public sealed class MarkdownListElement : MarkdownBlockElement
{
    /// <summary>Gets a value indicating whether the list is ordered (true) or unordered (false).</summary>
    public bool IsOrdered { get; }

    /// <summary>Gets the starting number for ordered lists (ignored for unordered lists).</summary>
    public int Start { get; }

    /// <summary>Gets the list items in order.</summary>
    public IReadOnlyList<MarkdownListItemElement> Items { get; }

    /// <summary>Creates a list element.</summary>
    public MarkdownListElement(bool isOrdered, int start, IReadOnlyList<MarkdownListItemElement> items)
    {
        IsOrdered = isOrdered;
        Start = start;
        Items = items;
    }
}

/// <summary>
/// A single item in a <see cref="MarkdownListElement"/>.
/// </summary>
public sealed class MarkdownListItemElement : MarkdownBlockElement
{
    /// <summary>Gets the blocks contained in this list item.</summary>
    public IReadOnlyList<MarkdownBlockElement> Blocks { get; }

    /// <summary>Creates a list-item element.</summary>
    public MarkdownListItemElement(IReadOnlyList<MarkdownBlockElement> blocks) => Blocks = blocks;
}

/// <summary>
/// A blockquote (`<code>&gt;</code>`) containing one or more block elements.
/// </summary>
public sealed class MarkdownBlockquoteElement : MarkdownBlockElement
{
    /// <summary>Gets the blocks quoted inside the blockquote.</summary>
    public IReadOnlyList<MarkdownBlockElement> Blocks { get; }

    /// <summary>Creates a blockquote element.</summary>
    public MarkdownBlockquoteElement(IReadOnlyList<MarkdownBlockElement> blocks) => Blocks = blocks;
}

/// <summary>
/// A thematic break (horizontal rule, e.g. <c>---</c>, <c>***</c>, <c>___</c>).
/// </summary>
public sealed class MarkdownThematicBreakElement : MarkdownBlockElement
{
}

/// <summary>
/// A table with a header row and zero or more body rows.
/// </summary>
public sealed class MarkdownTableElement : MarkdownBlockElement
{
    /// <summary>Gets the header cell texts.</summary>
    public IReadOnlyList<string> Headers { get; }

    /// <summary>Gets the body rows, each a list of cell texts.</summary>
    public IReadOnlyList<IReadOnlyList<string>> Rows { get; }

    /// <summary>Creates a table element.</summary>
    public MarkdownTableElement(IReadOnlyList<string> headers, IReadOnlyList<IReadOnlyList<string>> rows)
    {
        Headers = headers;
        Rows = rows;
    }
}

/// <summary>
/// Raw HTML. Treated as inert: the App layer must never execute or render it
/// as live markup (no WebView). The raw HTML text is retained for diagnostics
/// but is not rendered as interactive content.
/// </summary>
public sealed class MarkdownHtmlElement : MarkdownBlockElement
{
    /// <summary>Gets the raw HTML text (untouched, inert).</summary>
    public string Html { get; }

    /// <summary>Creates an HTML element.</summary>
    public MarkdownHtmlElement(string html) => Html = html;
}
