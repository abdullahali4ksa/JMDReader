using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MDReader.Core.Markdown;

/// <summary>
/// Provides pure utility functions for Markdown editing, search and replace (including Regex),
/// and clipboard/drag-and-drop image snippet generation.
/// </summary>
public static class MarkdownEditorUtilities
{
    private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp", ".bmp", ".ico"
    };

    /// <summary>
    /// Checks whether the given file path or extension represents a supported image format.
    /// </summary>
    public static bool IsImageFile(string? pathOrExtension)
    {
        if (string.IsNullOrWhiteSpace(pathOrExtension)) return false;
        var ext = Path.GetExtension(pathOrExtension);
        if (string.IsNullOrEmpty(ext)) ext = pathOrExtension;
        if (!ext.StartsWith('.')) ext = "." + ext;
        return SupportedImageExtensions.Contains(ext);
    }

    /// <summary>
    /// Generates a standard Markdown image tag: ![AltText](Path)
    /// </summary>
    public static string GenerateMarkdownImageSnippet(string altText, string imagePath)
    {
        var cleanAlt = string.IsNullOrWhiteSpace(altText) ? "image" : altText.Trim();
        var normalizedPath = imagePath.Replace('\\', '/');
        return $"![{cleanAlt}]({normalizedPath})";
    }

    /// <summary>
    /// Resolves the destination file path for a dropped or pasted image.
    /// If documentDirectory is provided, puts it in an 'images' subfolder;
    /// otherwise uses the system local app data fallback.
    /// </summary>
    public static (string TargetDirectory, string TargetFilePath, string MarkdownRelativePath) GetImageDestination(
        string? documentDirectory,
        string fileName)
    {
        string baseDir;
        string relativeDir;

        if (!string.IsNullOrWhiteSpace(documentDirectory) && Directory.Exists(documentDirectory))
        {
            baseDir = Path.Combine(documentDirectory, "images");
            relativeDir = "images";
        }
        else
        {
            baseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MDReader", "images");
            relativeDir = "images";
        }

        Directory.CreateDirectory(baseDir);
        var targetFilePath = Path.Combine(baseDir, fileName);
        var markdownRelativePath = $"{relativeDir}/{fileName}".Replace('\\', '/');

        return (baseDir, targetFilePath, markdownRelativePath);
    }

    /// <summary>
    /// Finds all match indices and lengths in the target content according to case and regex options.
    /// </summary>
    public static List<(int Index, int Length)> FindMatches(
        string content,
        string query,
        bool matchCase = false,
        bool isRegex = false)
    {
        var results = new List<(int Index, int Length)>();
        if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(query))
            return results;

        if (isRegex)
        {
            try
            {
                var options = matchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                var matches = Regex.Matches(content, query, options, TimeSpan.FromSeconds(1));
                foreach (Match m in matches)
                {
                    if (m.Success && m.Length > 0)
                    {
                        results.Add((m.Index, m.Length));
                    }
                }
            }
            catch (Exception ex) when (ex is ArgumentException or RegexMatchTimeoutException)
            {
                // Invalid regex pattern or catastrophic backtracking timeout: return empty results safely
            }
        }
        else
        {
            var comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            var position = 0;
            while ((position = content.IndexOf(query, position, comparison)) >= 0)
            {
                results.Add((position, query.Length));
                position += Math.Max(1, query.Length);
            }
        }

        return results;
    }

    /// <summary>
    /// Replaces occurrences of query with replacement in the content.
    /// If targetIndex is -1, replaces ALL occurrences.
    /// If targetIndex >= 0, replaces only the specific occurrence starting at targetIndex with targetLength.
    /// </summary>
    public static (string Result, int ReplacedCount) ReplaceText(
        string content,
        string query,
        string replacement,
        bool matchCase = false,
        bool isRegex = false,
        int targetIndex = -1,
        int targetLength = -1)
    {
        if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(query))
            return (content, 0);

        replacement ??= string.Empty;

        // Single specific replacement
        if (targetIndex >= 0 && targetLength > 0 && targetIndex + targetLength <= content.Length)
        {
            var sb = new System.Text.StringBuilder(content.Length - targetLength + replacement.Length);
            sb.Append(content.Substring(0, targetIndex));
            sb.Append(replacement);
            sb.Append(content.Substring(targetIndex + targetLength));
            return (sb.ToString(), 1);
        }

        // Global replacement
        if (isRegex)
        {
            try
            {
                var options = matchCase ? RegexOptions.None : RegexOptions.IgnoreCase;
                var timeout = TimeSpan.FromSeconds(1);
                var count = Regex.Matches(content, query, options, timeout).Count;
                var updated = Regex.Replace(content, query, replacement, options, timeout);
                return (updated, count);
            }
            catch (Exception ex) when (ex is ArgumentException or RegexMatchTimeoutException)
            {
                return (content, 0);
            }
        }
        else
        {
            var matches = FindMatches(content, query, matchCase, isRegex: false);
            if (matches.Count == 0) return (content, 0);

            var sb = new System.Text.StringBuilder();
            var lastPos = 0;
            foreach (var match in matches)
            {
                sb.Append(content.Substring(lastPos, match.Index - lastPos));
                sb.Append(replacement);
                lastPos = match.Index + match.Length;
            }
            sb.Append(content.Substring(lastPos));
            return (sb.ToString(), matches.Count);
        }
    }
}
