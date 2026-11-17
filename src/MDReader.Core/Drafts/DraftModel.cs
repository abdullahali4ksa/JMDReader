using System;

namespace MDReader.Core.Drafts;

/// <summary>
/// Represents an uncommitted document draft saved for crash recovery.
/// </summary>
public sealed class DraftModel
{
    public string DraftId { get; set; } = Guid.NewGuid().ToString("N");
    public string? OriginalFilePath { get; set; }
    public string DocumentTitle { get; set; } = "Untitled";
    public string Content { get; set; } = string.Empty;
    public DateTime SavedAtUtc { get; set; } = DateTime.UtcNow;
}
