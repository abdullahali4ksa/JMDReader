// MIT License - see LICENSE file in the repository root.

namespace MDReader.App.Features.Theme.Services;

/// <summary>
/// Service for managing application themes (light, dark, system, custom).
/// This is a placeholder interface — the implementation will be added in Phase 1.
/// </summary>
/// <remarks>
/// The Theme feature is separate from Settings because theme state
/// is consumed by the Shell and individual Views at startup.
/// </remarks>
public interface IThemeService
{
    /// <summary>Gets or sets the current theme name.</summary>
    string CurrentTheme { get; set; }

    /// <summary>Gets the list of available theme names.</summary>
    IReadOnlyList<string> AvailableThemes { get; }
}
