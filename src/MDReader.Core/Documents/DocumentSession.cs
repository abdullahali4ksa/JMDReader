// MIT License - see LICENSE file in the repository root.

using System;
using System.IO;

namespace MDReader.Core.Documents;

/// <summary>
/// Represents an active document session in memory, suitable for tabs, editing,
/// and tracking dirty/unsaved modifications.
/// </summary>
public sealed class DocumentSession
{
    public string Id { get; } = Guid.NewGuid().ToString("N");
    public string? FilePath { get; private set; }
    public string Title { get; private set; }
    public string OriginalContent { get; private set; }
    public string CurrentContent { get; private set; }
    public bool IsDirty => !string.Equals(OriginalContent, CurrentContent, StringComparison.Ordinal);
    public DateTimeOffset? LastSavedAt { get; private set; }

    public DocumentSession(string? filePath, string title, string initialContent)
    {
        FilePath = filePath;
        Title = string.IsNullOrWhiteSpace(title) ? "Untitled.md" : title;
        OriginalContent = initialContent ?? string.Empty;
        CurrentContent = OriginalContent;
    }

    /// <summary>
    /// Creates a session loaded from an existing file.
    /// </summary>
    public static DocumentSession FromFile(string filePath, string content)
    {
        var title = Path.GetFileName(filePath);
        return new DocumentSession(filePath, title, content);
    }

    /// <summary>
    /// Creates a new draft document session.
    /// </summary>
    public static DocumentSession CreateNew(string title = "Untitled.md", string initialContent = "")
    {
        return new DocumentSession(null, title, initialContent);
    }

    /// <summary>
    /// Updates the live in-memory content, altering the IsDirty flag accordingly.
    /// </summary>
    public void UpdateContent(string newContent)
    {
        CurrentContent = newContent ?? string.Empty;
    }

    /// <summary>
    /// Marks the session as saved to a specific file path.
    /// </summary>
    public void MarkSaved(string filePath, DateTimeOffset savedAt)
    {
        FilePath = filePath;
        Title = Path.GetFileName(filePath);
        OriginalContent = CurrentContent;
        LastSavedAt = savedAt;
    }
}
