using System;
using System.IO;
using System.Text.Json;
using MDReader.Core.Recent;

namespace MDReader.App.Features.Settings.Services;

/// <summary>
/// Manages user application preferences and recent files persistence.
/// </summary>
public sealed class UserPreferencesService
{
    private static readonly Lazy<UserPreferencesService> _instance = new(() => new UserPreferencesService());
    public static UserPreferencesService Instance => _instance.Value;

    private readonly string _storageDir;
    private readonly string _prefsFilePath;
    private readonly string _recentFilePath;

    public RecentFileManager RecentFiles { get; } = new();

    public bool AutoSaveDrafts { get; set; } = true;
    public bool RestoreSession { get; set; } = true;
    public bool TypewriterMode { get; set; } = false;
    public string PreferredTheme { get; set; } = "System";

    public string DraftsDirectory => Path.Combine(_storageDir, "drafts");
    public string SessionFilePath => Path.Combine(_storageDir, "last_session.json");

    private UserPreferencesService()
    {
        _storageDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MDReader");

        _prefsFilePath = Path.Combine(_storageDir, "preferences.json");
        _recentFilePath = Path.Combine(_storageDir, "recent_history.json");

        EnsureDirectories();
        Load();
    }

    private void EnsureDirectories()
    {
        try
        {
            if (!Directory.Exists(_storageDir))
                Directory.CreateDirectory(_storageDir);
            if (!Directory.Exists(DraftsDirectory))
                Directory.CreateDirectory(DraftsDirectory);
        }
        catch { }
    }

    public void Load()
    {
        try
        {
            RecentFiles.LoadFromFile(_recentFilePath);

            if (File.Exists(_prefsFilePath))
            {
                using var stream = new FileStream(_prefsFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                var json = reader.ReadToEnd();
                var model = JsonSerializer.Deserialize<PreferencesModel>(json);
                if (model != null)
                {
                    AutoSaveDrafts = model.AutoSaveDrafts;
                    RestoreSession = model.RestoreSession;
                    TypewriterMode = model.TypewriterMode;
                    if (!string.IsNullOrWhiteSpace(model.PreferredTheme))
                        PreferredTheme = model.PreferredTheme;
                }
            }
        }
        catch { }
    }

    public void Save()
    {
        try
        {
            EnsureDirectories();
            RecentFiles.SaveToFile(_recentFilePath);

            var model = new PreferencesModel
            {
                AutoSaveDrafts = AutoSaveDrafts,
                RestoreSession = RestoreSession,
                TypewriterMode = TypewriterMode,
                PreferredTheme = PreferredTheme
            };

            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
            var tmp = _prefsFilePath + ".tmp";
            File.WriteAllText(tmp, json);

            if (File.Exists(_prefsFilePath))
                File.Delete(_prefsFilePath);

            File.Move(tmp, _prefsFilePath);
        }
        catch { }
    }

    private sealed class PreferencesModel
    {
        public bool AutoSaveDrafts { get; set; } = true;
        public bool RestoreSession { get; set; } = true;
        public bool TypewriterMode { get; set; } = false;
        public string PreferredTheme { get; set; } = "System";
    }
}
