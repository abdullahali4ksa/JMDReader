// MIT License - see LICENSE file in the repository root.

using System.Text;
using System.Collections.Generic;
using System.Linq;
using MDReader.Core.Markdown;
using Xunit;

namespace MDReader.Core.Tests;

/// <summary>
/// Tests for the Markdown parser (<see cref="MarkdownDocument.Parse"/>), which
/// converts Markdig's AST into the platform-neutral element model.
/// </summary>
public class MarkdownAstTests
{
    /// <summary>Parses source text into its top-level block elements.</summary>
    private static IReadOnlyList<MarkdownBlockElement> ParseBlocks(string source) =>
        Parse(source).Blocks;

    private static MarkdownDocumentElement Parse(string source) =>
        new MarkdownDocument { SourceText = source }.Parse().Document.Elements!;

    /// <summary>Flattens a sequence of inline elements into plain text.</summary>
    private static string ToText(IReadOnlyList<MarkdownInlineElement> inlines)
    {
        var sb = new StringBuilder();
        foreach (var inline in inlines)
            AppendInlineText(inline, sb);
        return sb.ToString();
    }

    private static void AppendInlineText(MarkdownInlineElement inline, StringBuilder sb)
    {
        switch (inline)
        {
            case MarkdownLiteralInline l:
                sb.Append(l.Text);
                break;
            case MarkdownCodeInline c:
                sb.Append(c.Code);
                break;
            case MarkdownEmphasisInline e:
                foreach (var child in e.Inlines)
                    AppendInlineText(child, sb);
                break;
            case MarkdownLinkInline l:
                foreach (var child in l.Inlines)
                    AppendInlineText(child, sb);
                break;
            case MarkdownImageInline i:
                sb.Append(i.AltText);
                break;
            default:
                sb.Append(inline.ToString());
                break;
        }
    }

    // ------------------------------------------------------------------ Headings

    [Theory]
    [InlineData("# H1", 1)]
    [InlineData("## H2", 2)]
    [InlineData("###### H6", 6)]
    public void Parse_Heading_ReturnsCorrectLevel(string source, int level)
    {
        var doc = Parse(source);
        var heading = Assert.IsType<MarkdownHeadingElement>(Assert.Single(doc.Blocks));
        Assert.Equal(level, heading.Level);
        Assert.Equal("H" + level, ToText(heading.Inlines));
    }

    // ------------------------------------------------------------------ Paragraphs

    [Fact]
    public void Parse_ParagraphWithRichInlines_ReturnsAllInlineTypes()
    {
        var doc = Parse("A **bold** *italic* `code` [link](https://x.io) ~~strike~~.");
        var para = Assert.IsType<MarkdownParagraphElement>(Assert.Single(doc.Blocks));

        Assert.Equal("A bold italic code link strike.", ToText(para.Inlines));

        var types = para.Inlines.Select(i => i.GetType().Name).ToArray();
        Assert.Contains("MarkdownLiteralInline", types);
        Assert.Contains("MarkdownEmphasisInline", types);   // bold + italic + strikethrough
        Assert.Contains("MarkdownCodeInline", types);
        Assert.Contains("MarkdownLinkInline", types);
    }

    [Fact]
    public void Parse_BoldEmphasis_HasBoldType()
    {
        var doc = Parse("**bold**");
        var para = Assert.IsType<MarkdownParagraphElement>(Assert.Single(doc.Blocks));
        var bold = Assert.IsType<MarkdownEmphasisInline>(Assert.Single(para.Inlines));
        Assert.Equal(MarkdownEmphasisType.Bold, bold.EmphasisType);
        Assert.Equal("bold", ToText(bold.Inlines));
    }

    [Fact]
    public void Parse_ItalicEmphasis_HasItalicType()
    {
        var doc = Parse("*italic*");
        var para = Assert.IsType<MarkdownParagraphElement>(Assert.Single(doc.Blocks));
        var italic = Assert.IsType<MarkdownEmphasisInline>(Assert.Single(para.Inlines));
        Assert.Equal(MarkdownEmphasisType.Italic, italic.EmphasisType);
    }

    [Fact]
    public void Parse_Strikethrough_HasStrikethroughType()
    {
        var doc = Parse("~~strike~~");
        var para = Assert.IsType<MarkdownParagraphElement>(Assert.Single(doc.Blocks));
        var strike = Assert.IsType<MarkdownEmphasisInline>(Assert.Single(para.Inlines));
        Assert.Equal(MarkdownEmphasisType.Strikethrough, strike.EmphasisType);
    }

    [Fact]
    public void Parse_LinkInline_HasUrlAndText()
    {
        var doc = Parse("[click here](https://example.com/page)");
        var para = Assert.IsType<MarkdownParagraphElement>(Assert.Single(doc.Blocks));
        var link = Assert.IsType<MarkdownLinkInline>(Assert.Single(para.Inlines));
        Assert.Equal("https://example.com/page", link.Url);
        Assert.Equal("click here", ToText(link.Inlines));
    }

    [Fact]
    public void Parse_ImageInline_HasUrlAndAltText()
    {
        var doc = Parse("![alt text](image.png)");
        var para = Assert.IsType<MarkdownParagraphElement>(Assert.Single(doc.Blocks));
        var image = Assert.IsType<MarkdownImageInline>(Assert.Single(para.Inlines));
        Assert.Equal("image.png", image.Url);
        Assert.Equal("alt text", image.AltText);
    }

    [Fact]
    public void Parse_InlineCode_HasCodeText()
    {
        var doc = Parse("Use `let x` now.");
        var para = Assert.IsType<MarkdownParagraphElement>(Assert.Single(doc.Blocks));
        var code = para.Inlines.OfType<MarkdownCodeInline>().Single();
        Assert.Equal("let x", code.Code);
    }

    // ------------------------------------------------------------------ Lists

    [Fact]
    public void Parse_UnorderedList_ReturnsItems()
    {
        var doc = Parse("- one\n- two\n- three\n");
        var list = Assert.IsType<MarkdownListElement>(Assert.Single(doc.Blocks));
        Assert.False(list.IsOrdered);
        Assert.Equal(3, list.Items.Count);
        Assert.Equal("one", GetListItemText(list.Items[0]));
        Assert.Equal("two", GetListItemText(list.Items[1]));
        Assert.Equal("three", GetListItemText(list.Items[2]));
    }

    [Fact]
    public void Parse_OrderedLists_HaveStartAndOrder()
    {
        var doc = Parse("1. first\n2. second\n");
        var list = Assert.IsType<MarkdownListElement>(Assert.Single(doc.Blocks));
        Assert.True(list.IsOrdered);
        Assert.Equal(1, list.Start);
        Assert.Equal("first", GetListItemText(list.Items[0]));
        Assert.Equal("second", GetListItemText(list.Items[1]));
    }

    [Fact]
    public void Parse_OrderedList_CustomStart_PreservesStartNumber()
    {
        var doc = Parse("5. five\n6. six\n");
        var list = Assert.IsType<MarkdownListElement>(Assert.Single(doc.Blocks));
        Assert.True(list.IsOrdered);
        Assert.Equal(5, list.Start);
    }

    [Fact]
    public void Parse_NestedList_ProducesNestedListElement()
    {
        var doc = Parse("- outer\n  - inner\n");
        var list = Assert.IsType<MarkdownListElement>(Assert.Single(doc.Blocks));
        Assert.Single(list.Items);
        var outer = GetListItemText(list.Items[0]);
        Assert.Equal("outer", outer);

        // The list item's blocks should contain a nested list.
        var nested = list.Items[0].Blocks.OfType<MarkdownListElement>().Single();
        Assert.Equal("inner", GetListItemText(nested.Items[0]));
    }

    private static string GetListItemText(MarkdownListItemElement item)
    {
        var para = item.Blocks.OfType<MarkdownParagraphElement>().Single();
        return ToText(para.Inlines);
    }

    // ------------------------------------------------------------------ Code blocks

    [Fact]
    public void Parse_FencedCodeBlock_ExtractsLanguageAndCode()
    {
        var doc = Parse("```csharp\nvar x = 1;\nConsole.WriteLine(x);\n```\n");
        var code = Assert.IsType<MarkdownCodeBlockElement>(Assert.Single(doc.Blocks));
        Assert.True(code.IsFenced);
        Assert.Equal("csharp", code.Language);
        Assert.Contains("var x = 1;", code.Code);
        Assert.Contains("Console.WriteLine(x);", code.Code);
    }

    [Fact]
    public void Parse_IndentedCodeBlock_IsNotFenced()
    {
        var doc = Parse("    indented code here\n");
        var code = Assert.IsType<MarkdownCodeBlockElement>(Assert.Single(doc.Blocks));
        Assert.False(code.IsFenced);
        Assert.Equal("indented code here", code.Code.Trim());
        Assert.Null(code.Language);
    }

    // ------------------------------------------------------------------ Blockquotes

    [Fact]
    public void Parse_Blockquote_WrapsBlocks()
    {
        var doc = Parse("> This is a quote.\n");
        var quote = Assert.IsType<MarkdownBlockquoteElement>(Assert.Single(doc.Blocks));
        var para = Assert.IsType<MarkdownParagraphElement>(Assert.Single(quote.Blocks));
        Assert.Equal("This is a quote.", ToText(para.Inlines));
    }

    // ------------------------------------------------------------------ Thematic break

    [Fact]
    public void Parse_ThematicBreak_ReturnsThematicBreakElement()
    {
        var doc = Parse("Paragraph\n\n---\n\nAfter\n");
        var blocks = doc.Blocks.ToArray();
        Assert.IsType<MarkdownParagraphElement>(blocks[0]);
        Assert.IsType<MarkdownThematicBreakElement>(blocks[1]);
        Assert.IsType<MarkdownParagraphElement>(blocks[2]);
    }

    // ------------------------------------------------------------------ Tables

    [Fact]
    public void Parse_Table_ExtractsHeadersAndRows()
    {
        var doc = Parse("| Name | Age |\n|------|-----|\n| Ann | 30 |\n| Bob | 25 |\n");
        var table = Assert.IsType<MarkdownTableElement>(Assert.Single(doc.Blocks));
        Assert.Equal(new[] { "Name", "Age" }, table.Headers);
        Assert.Equal(2, table.Rows.Count);
        Assert.Equal(new[] { "Ann", "30" }, table.Rows[0]);
        Assert.Equal(new[] { "Bob", "25" }, table.Rows[1]);
    }

    // ------------------------------------------------------------------ HTML (inert)

    [Fact]
    public void Parse_HtmlBlock_IsInertElementWithRawText()
    {
        var doc = Parse("<div>raw html</div>\n");
        var html = Assert.IsType<MarkdownHtmlElement>(Assert.Single(doc.Blocks));
        Assert.Equal("<div>raw html</div>", html.Html.Trim());
    }

    [Fact]
    public void Parse_HtmlInline_DegradesToLiteralText()
    {
        var doc = Parse("Before <b>inline html</b> after.\n");
        var para = Assert.IsType<MarkdownParagraphElement>(Assert.Single(doc.Blocks));
        // Inline HTML should not execute; it degrades to literal text.
        Assert.Contains("inline html", ToText(para.Inlines));
    }

    // ------------------------------------------------------------------ Empty / whitespace

    [Fact]
    public void Parse_EmptyText_ReturnsEmptyDocument()
    {
        var doc = Parse("");
        Assert.Empty(doc.Blocks);
    }

    [Fact]
    public void Parse_WhitespaceOnly_ReturnsEmptyDocument()
    {
        var doc = Parse("   \n\n  \n");
        Assert.Empty(doc.Blocks);
    }

    [Fact]
    public void Parse_EmptyText_SetsElements()
    {
        var result = new MarkdownDocument { SourceText = "" }.Parse();
        Assert.True(result.Success);
        Assert.NotNull(result.Document.Elements);
        Assert.Empty(result.Document.Elements!.Blocks);
        Assert.Null(result.Error);
    }

    // ------------------------------------------------------------------ Result/error model

    [Fact]
    public void Parse_InvalidMarkdown_ReturnsNoThrow()
    {
        // Markdig is lenient; malformed input should never throw out of Parse.
        var md = "*unterminated emphasis [link](\n# Heading\n\n```\nunclosed code";
        var result = new MarkdownDocument { SourceText = md }.Parse();
        Assert.True(result.Success);
        Assert.NotNull(result.Document.Elements);
    }
}
