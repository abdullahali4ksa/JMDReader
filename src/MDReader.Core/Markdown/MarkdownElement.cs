// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Markdown;

/// <summary>
/// Base type for every element in the platform-neutral Markdown AST produced by
/// <see cref="MarkdownDocument.Parse"/>. The App layer walks this tree to render
/// WinUI 3 controls; the Core layer never references WinUI.
/// </summary>
public abstract class MarkdownElement
{
}

/// <summary>
/// Base type for block-level elements (headings, paragraphs, code blocks, lists, etc.).
/// A block may contain other blocks (container blocks) or inline content.
/// </summary>
public abstract class MarkdownBlockElement : MarkdownElement
{
}

/// <summary>
/// Base type for inline-level elements (text runs, emphasis, code, links) which
/// appear inside block-level containers such as headings and paragraphs.
/// </summary>
public abstract class MarkdownInlineElement : MarkdownElement
{
}
