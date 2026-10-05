using CommunityToolkit.Mvvm.ComponentModel;
using MDReader.App.Features.Settings.Services;
using MDReader.App.Features.Theme.Services;

namespace MDReader.App.Features.Settings.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly IThemeService _themeService;
    private readonly RecentFilesService _recentService;

    public SettingsViewModel(IThemeService themeService, RecentFilesService recentService)
    {
        _themeService = themeService;
        _recentService = recentService;
    }

    public string Theme
    {
        get => _themeService.CurrentTheme;
        set
        {
            if (_themeService.CurrentTheme != value)
            {
                _themeService.CurrentTheme = value;
                OnPropertyChanged();
            }
        }
    }

    public IReadOnlyList<string> RecentFiles => _recentService.RecentPaths;

    public string[] ThemeOptions => new[] { "System", "Light", "Dark" };
    public string[] LanguageLabels => new[] { "English", "العربية" };
}
