// MIT License - see LICENSE file in the repository root.

using System;

namespace MDReader.Core.Documents;

/// <summary>
/// Represents an in-document user comment or annotation.
/// </summary>
public sealed class DocumentComment
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string SelectedText { get; init; } = string.Empty;
    public string CommentText { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.Now;

    public string CreatedAtDisplay => CreatedAt.ToString("yyyy-MM-dd HH:mm");
    public bool HasQuote => !string.IsNullOrWhiteSpace(SelectedText);
}
