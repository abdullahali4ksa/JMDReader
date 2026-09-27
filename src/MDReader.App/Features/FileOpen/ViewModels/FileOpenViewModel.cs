// MIT License - see LICENSE file in the repository root.

namespace MDReader.App.Features.FileOpen.ViewModels;

/// <summary>
/// ViewModel for the file open dialog (browsing, filtering, selecting .md files).
/// This is a placeholder — will be implemented in Phase 1.
/// </summary>
/// <remarks>
/// The FileOpen feature controls how users select Markdown files to open.
/// It should communicate with the Reader feature through defined contracts,
/// not by directly manipulating Reader XAML.
/// </remarks>
public sealed class FileOpenViewModel
{
    /// <summary>Gets or sets the selected file path.</summary>
    public string? SelectedFilePath { get; set; }
}
