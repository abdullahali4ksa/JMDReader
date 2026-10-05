// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Markdown;

/// <summary>
/// A literal run of text (plain text, no formatting).
/// </summary>
public sealed class MarkdownLiteralInline : MarkdownInlineElement
{
    /// <summary>Gets the literal text.</summary>
    public string Text { get; }

    /// <summary>Creates a literal inline.</summary>
    public MarkdownLiteralInline(string text) => Text = text;
}

/// <summary>
/// Emphasized text (bold, italic, or strikethrough) wrapping child inlines.
/// </summary>
public sealed class MarkdownEmphasisInline : MarkdownInlineElement
{
    /// <summary>Gets the kind of emphasis applied.</summary>
    public MarkdownEmphasisType EmphasisType { get; }

    /// <summary>Gets the inline content inside the emphasis.</summary>
    public IReadOnlyList<MarkdownInlineElement> Inlines { get; }

    /// <summary>Creates an emphasis inline.</summary>
    public MarkdownEmphasisInline(MarkdownEmphasisType emphasisType, IReadOnlyList<MarkdownInlineElement> inlines)
    {
        EmphasisType = emphasisType;
        Inlines = inlines;
    }
}

/// <summary>
/// Inline code (e.g. <c>`code`</c>).
/// </summary>
public sealed class MarkdownCodeInline : MarkdownInlineElement
{
    /// <summary>Gets the code text.</summary>
    public string Code { get; }

    /// <summary>Creates a code inline.</summary>
    public MarkdownCodeInline(string code) => Code = code;
}

/// <summary>
/// A hyperlink (e.g. <c>[text](url)</c>). The link text is retained as inlines;
/// the App layer renders it as a clickable link via the OS handler.
/// </summary>
public sealed class MarkdownLinkInline : MarkdownInlineElement
{
    /// <summary>Gets the link target URL.</summary>
    public string Url { get; }

    /// <summary>Gets the link text content.</summary>
    public IReadOnlyList<MarkdownInlineElement> Inlines { get; }

    /// <summary>Gets the optional link title, or null.</summary>
    public string? Title { get; }

    /// <summary>Creates a link inline.</summary>
    public MarkdownLinkInline(string url, IReadOnlyList<MarkdownInlineElement> inlines, string? title = null)
    {
        Url = url;
        Inlines = inlines;
        Title = title;
    }
}

/// <summary>
/// <summary>A GFM task list checkbox (<c>[x]</c> or <c>[ ]</c>).</summary>
public sealed class MarkdownTaskInline : MarkdownInlineElement
{
    /// <summary>Gets whether the task is checked.</summary>
    public bool IsChecked { get; }

    /// <summary>Creates a task list checkbox inline.</summary>
    public MarkdownTaskInline(bool isChecked) => IsChecked = isChecked;
}

/// <summary>An inline image (e.g. <c>![alt](src)</c>). Local image resolution and rendering</summary>
/// is handled in Phase 3; for MVP the renderer degrades safely to a placeholder.
/// </summary>
public sealed class MarkdownImageInline : MarkdownInlineElement
{
    /// <summary>Gets the image source URL (relative or absolute).</summary>
    public string Url { get; }

    /// <summary>Gets the alternative text.</summary>
    public string AltText { get; }

    /// <summary>Creates an image inline.</summary>
    public MarkdownImageInline(string url, string altText)
    {
        Url = url;
        AltText = altText;
    }
}
