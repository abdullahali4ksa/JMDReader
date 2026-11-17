using System;
using System.IO;
using System.Text.Json;

namespace MDReader.Core.Session;

/// <summary>
/// Handles atomic persistence and retrieval of application document sessions.
/// </summary>
public sealed class SessionManager
{
    public static void SaveSession(SessionState state, string filePath)
    {
        if (state == null || string.IsNullOrWhiteSpace(filePath)) return;

        try
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            var tmp = filePath + ".tmp";
            File.WriteAllText(tmp, json);

            if (File.Exists(filePath))
                File.Delete(filePath);

            File.Move(tmp, filePath);
        }
        catch { }
    }

    public static SessionState? LoadSession(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            return null;

        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            return JsonSerializer.Deserialize<SessionState>(json);
        }
        catch
        {
            return null;
        }
    }

    public static void ClearSession(string filePath)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath))
                File.Delete(filePath);
        }
        catch { }
    }
}
