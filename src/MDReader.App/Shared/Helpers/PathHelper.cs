// MIT License - see LICENSE file in the repository root.

using System.Linq;

namespace MDReader.App.Shared.Helpers;

/// <summary>
/// Utility methods for file path validation and normalization.
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// Validates that a file path is safe — no directory traversal,
    /// no control characters, no excessively long paths.
    /// </summary>
    public static bool IsPathSafe(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        if (path.Length > 260)
            return false;

        if (path.Contains(".."))
            return false;

        if (path.Any(c => c < 32))
            return false;

        return true;
    }
}
