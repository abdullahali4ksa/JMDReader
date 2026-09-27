// MIT License - see LICENSE file in the repository root.

namespace MDReader.App.Features.Shell.ViewModels;

/// <summary>
/// ViewModel for the application shell (window management, navigation, app menu).
/// This is a placeholder — the MVVM pattern will be fully implemented
/// when the CommunityToolkit.Mvvm dependency is added in Phase 1.
/// </summary>
/// <remarks>
/// The Shell feature controls the overall application window structure:
/// title bar, window state, navigation frame, and app menu.
/// </remarks>
public sealed class ShellViewModel
{
    /// <summary>Gets or sets the window title.</summary>
    public string WindowTitle { get; set; } = "MDReader";
}
