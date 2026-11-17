using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MDReader.Core.Recent;

/// <summary>
/// Manages recent document history, pinning, and atomic JSON persistence.
/// </summary>
public sealed class RecentFileManager
{
    private readonly List<RecentFileInfo> _items = new();
    private readonly object _syncLock = new();

    public int MaxCount { get; set; } = 30;

    public void Add(string path, bool isPinned = false) => AddRecentFile(path, isPinned);

    public IReadOnlyList<RecentFileInfo> GetRecentFiles()
    {
        lock (_syncLock)
        {
            // Pinned items first (ordered by date desc), then unpinned items (ordered by date desc)
            return _items
                .OrderByDescending(x => x.IsPinned)
                .ThenByDescending(x => x.LastOpenedUtc)
                .Take(MaxCount)
                .ToList();
        }
    }

    public void AddRecentFile(string path, bool isPinned = false)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        lock (_syncLock)
        {
            var normalized = Path.GetFullPath(path);
            var existing = _items.FirstOrDefault(x => string.Equals(x.Path, normalized, StringComparison.OrdinalIgnoreCase));

            long size = 0;
            try
            {
                if (File.Exists(normalized))
                    size = new FileInfo(normalized).Length;
            }
            catch { }

            if (existing != null)
            {
                existing.LastOpenedUtc = DateTime.UtcNow;
                existing.FileSizeBytes = size;
                if (isPinned) existing.IsPinned = true;
            }
            else
            {
                _items.Insert(0, new RecentFileInfo
                {
                    Path = normalized,
                    Title = Path.GetFileName(normalized),
                    LastOpenedUtc = DateTime.UtcNow,
                    IsPinned = isPinned,
                    FileSizeBytes = size
                });
            }

            // Trim unpinned items beyond MaxCount
            TrimExcess();
        }
    }

    public bool TogglePin(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;

        lock (_syncLock)
        {
            var normalized = Path.GetFullPath(path);
            var existing = _items.FirstOrDefault(x => string.Equals(x.Path, normalized, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.IsPinned = !existing.IsPinned;
                return existing.IsPinned;
            }
            return false;
        }
    }

    public void RemoveRecentFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        lock (_syncLock)
        {
            var normalized = Path.GetFullPath(path);
            _items.RemoveAll(x => string.Equals(x.Path, normalized, StringComparison.OrdinalIgnoreCase));
        }
    }

    public void ClearAll()
    {
        lock (_syncLock)
        {
            _items.Clear();
        }
    }

    private void TrimExcess()
    {
        var pinned = _items.Where(x => x.IsPinned).ToList();
        var unpinned = _items.Where(x => !x.IsPinned).OrderByDescending(x => x.LastOpenedUtc).Take(MaxCount).ToList();

        _items.Clear();
        _items.AddRange(pinned);
        _items.AddRange(unpinned);
    }

    public string ExportJson()
    {
        lock (_syncLock)
        {
            return JsonSerializer.Serialize(_items, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public void ImportJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return;

        try
        {
            var list = JsonSerializer.Deserialize<List<RecentFileInfo>>(json);
            if (list != null)
            {
                lock (_syncLock)
                {
                    _items.Clear();
                    _items.AddRange(list);
                    TrimExcess();
                }
            }
        }
        catch { }
    }

    public void SaveToFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) return;

        try
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = ExportJson();
            var tmpFile = filePath + ".tmp";
            File.WriteAllText(tmpFile, json);

            if (File.Exists(filePath))
                File.Delete(filePath);

            File.Move(tmpFile, filePath);
        }
        catch { }
    }

    public void LoadFromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath)) return;

        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            ImportJson(json);
        }
        catch { }
    }
}
