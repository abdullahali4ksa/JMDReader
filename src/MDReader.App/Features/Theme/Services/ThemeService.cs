namespace MDReader.App.Features.Theme.Services;

public sealed class ThemeService : IThemeService
{
    public string CurrentTheme { get; set; } = "System";
    public IReadOnlyList<string> AvailableThemes => new[] { "Light", "Dark", "System" };
}
