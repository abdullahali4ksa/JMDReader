using System.Collections.Generic;
using System.Linq;
using MDReader.Core.Recent;

namespace MDReader.App.Features.Settings.Services;

/// <summary>
/// Service providing access to recent files with pinning and persistent storage.
/// </summary>
public sealed class RecentFilesService
{
    private readonly RecentFileManager _manager;

    public RecentFilesService()
    {
        _manager = UserPreferencesService.Instance.RecentFiles;
    }

    public int MaxCount => _manager.MaxCount;

    public IReadOnlyList<string> RecentPaths => _manager.GetRecentFiles().Select(x => x.Path).ToList();

    public IReadOnlyList<RecentFileInfo> RecentItems => _manager.GetRecentFiles();

    public void Add(string path, bool isPinned = false)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        _manager.AddRecentFile(path, isPinned);
        UserPreferencesService.Instance.Save();
    }

    public bool TogglePin(string path)
    {
        var result = _manager.TogglePin(path);
        UserPreferencesService.Instance.Save();
        return result;
    }

    public void Remove(string path)
    {
        _manager.RemoveRecentFile(path);
        UserPreferencesService.Instance.Save();
    }

    public void Clear()
    {
        _manager.ClearAll();
        UserPreferencesService.Instance.Save();
    }
}
