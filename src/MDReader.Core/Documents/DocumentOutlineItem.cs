// MIT License - see LICENSE file in the repository root.

using System;

namespace MDReader.Core.Documents;

/// <summary>
/// Represents a section heading in the document outline / table of contents.
/// </summary>
public sealed class DocumentOutlineItem
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public int Level { get; init; } = 1;
    public int LineNumber { get; init; } = 0;

    /// <summary>Margin offset for visual hierarchical indentation in UI.</summary>
    public int IndentMargin => Math.Max(0, (Level - 1) * 14);

    public string DisplayTitle => Level > 1 ? new string(' ', (Level - 1) * 3) + "• " + Title : Title;

    public bool IsTopLevel => Level <= 2;

    public override string ToString() => $"H{Level}: {Title} (#{Id})";
}
