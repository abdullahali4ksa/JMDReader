// MIT License - see LICENSE file in the repository root.

namespace MDReader.App.Features.Reader.ViewModels;

/// <summary>
/// ViewModel for the document reader (Markdown rendering, scrolling, navigation).
/// This is a placeholder — Reader functionality will be implemented in Phase 2.
/// </summary>
/// <remarks>
/// The Reader feature is responsible for rendering Markdown content
/// using the Markdig library and displaying it in a WebView2 or
/// custom text rendering control.
/// </remarks>
public sealed class ReaderViewModel
{
    /// <summary>Gets or sets the path of the currently loaded document.</summary>
    public string? CurrentDocumentPath { get; set; }
}
