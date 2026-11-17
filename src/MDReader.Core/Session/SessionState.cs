using System;
using System.Collections.Generic;

namespace MDReader.Core.Session;

/// <summary>
/// Represents the saved session state of open document tabs.
/// </summary>
public sealed class SessionState
{
    public List<string> OpenDocumentPaths { get; set; } = new();
    public string? ActiveDocumentPath { get; set; }
    public int SelectedTabIndex { get; set; }
    public DateTime LastSavedUtc { get; set; } = DateTime.UtcNow;
}
