using System;

namespace MDReader.Core.Recent;

/// <summary>
/// Represents a recent document entry with timestamp, pinning status, and metadata.
/// </summary>
public sealed class RecentFileInfo
{
    public string Path { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime LastOpenedUtc { get; set; } = DateTime.UtcNow;
    public bool IsPinned { get; set; }
    public long FileSizeBytes { get; set; }

    public string Extension => System.IO.Path.GetExtension(Path).ToLowerInvariant();
    public string PinGlyph => IsPinned ? "📌" : "📄";
    public string TypeGlyph => Extension == ".json" ? "🌳" : (Extension == ".txt" ? "📄" : "📝");

    public string DisplayTime
    {
        get
        {
            var diff = DateTime.UtcNow - LastOpenedUtc;
            if (diff.TotalMinutes < 1) return "Just now / الآن";
            if (diff.TotalHours < 1) return $"{(int)diff.TotalMinutes}m ago";
            if (diff.TotalDays < 1) return $"{(int)diff.TotalHours}h ago";
            if (diff.TotalDays < 7) return $"{(int)diff.TotalDays}d ago";
            return LastOpenedUtc.ToLocalTime().ToString("yyyy-MM-dd");
        }
    }
}
