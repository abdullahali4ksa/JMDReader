using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace MDReader.Core.Drafts;

/// <summary>
/// Manages automatic draft caching, background recovery saving, and cleanup.
/// </summary>
public sealed class DraftManager
{
    public static void SaveDraft(string draftsDirectory, DraftModel draft)
    {
        if (string.IsNullOrWhiteSpace(draftsDirectory) || draft == null) return;

        try
        {
            if (!Directory.Exists(draftsDirectory))
                Directory.CreateDirectory(draftsDirectory);

            var filePath = Path.Combine(draftsDirectory, $"{draft.DraftId}.draft.json");
            var json = JsonSerializer.Serialize(draft, new JsonSerializerOptions { WriteIndented = true });
            var tmp = filePath + ".tmp";
            File.WriteAllText(tmp, json);

            if (File.Exists(filePath))
                File.Delete(filePath);

            File.Move(tmp, filePath);
        }
        catch { }
    }

    public static IReadOnlyList<DraftModel> GetAllDrafts(string draftsDirectory)
    {
        var result = new List<DraftModel>();
        if (string.IsNullOrWhiteSpace(draftsDirectory) || !Directory.Exists(draftsDirectory))
            return result;

        try
        {
            var files = Directory.GetFiles(draftsDirectory, "*.draft.json");
            foreach (var file in files)
            {
                try
                {
                    using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using var reader = new StreamReader(stream);
                    var draft = JsonSerializer.Deserialize<DraftModel>(reader.ReadToEnd());
                    if (draft != null)
                        result.Add(draft);
                }
                catch { }
            }
        }
        catch { }

        return result;
    }

    public static void DeleteDraft(string draftsDirectory, string draftId)
    {
        if (string.IsNullOrWhiteSpace(draftsDirectory) || string.IsNullOrWhiteSpace(draftId)) return;

        try
        {
            var filePath = Path.Combine(draftsDirectory, $"{draftId}.draft.json");
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch { }
    }

    public static void ClearAllDrafts(string draftsDirectory)
    {
        if (string.IsNullOrWhiteSpace(draftsDirectory) || !Directory.Exists(draftsDirectory)) return;

        try
        {
            var files = Directory.GetFiles(draftsDirectory, "*.draft.json");
            foreach (var file in files)
            {
                try { File.Delete(file); } catch { }
            }
        }
        catch { }
    }
}
