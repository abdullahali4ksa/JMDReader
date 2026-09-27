// MIT License - see LICENSE file in the repository root.

namespace MDReader.App.Features.Settings.ViewModels;

/// <summary>
/// ViewModel for the settings page (theme, language, preferences).
/// This is a placeholder — will be implemented in Phase 1.
/// </summary>
/// <remarks>
/// The Settings feature controls application preferences that persist
/// across sessions using app settings or a local configuration file.
/// </remarks>
public sealed class SettingsViewModel
{
    /// <summary>Gets or sets the current theme name.</summary>
    public string Theme { get; set; } = "System";
}
