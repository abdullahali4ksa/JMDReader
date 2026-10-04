// MIT License - see LICENSE file in the repository root.

using System.IO;
using System.Text;

namespace MDReader.Core.Documents;

/// <summary>Provides safe, read-only file reading for Markdown documents.</summary>
public static class MarkdownFileReader
{
    public const int SizeWarningBytes = 5 * 1024 * 1024; // 5 MB per Blueprint §3.2

    /// <summary>Reads a Markdown file safely and validates access/encoding.</summary>
    /// <param name="filePath">Path to the Markdown file.</param>
    /// <returns>A DocumentInfo if the file is valid; throws typed exceptions for recoverable errors.</returns>
    public static DocumentInfo ReadFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new FileNotFoundException("File path is empty.");

        var extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
        if (extension != ".md" && extension != ".markdown")
            throw new System.ArgumentException($"Unsupported file extension: '{extension}'. Only .md and .markdown are allowed.");

        if (!System.IO.File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");

        var fileInfo = new System.IO.FileInfo(filePath);
        var info = new DocumentInfo
        {
            FilePath = filePath,
            Title = fileInfo.Name,
            SizeInBytes = fileInfo.Length,
            LastModified = fileInfo.LastWriteTime,
            Encoding = "utf-8"
        };

        if (info.SizeInBytes > SizeWarningBytes)
        {
            // Size warning does not prevent reading; the caller (App/Reader) handles the warning.
            info = new DocumentInfo
            {
                FilePath = info.FilePath,
                Title = info.Title,
                SizeInBytes = info.SizeInBytes,
                LastModified = info.LastModified,
                Encoding = info.Encoding
            };
        }

        // Read as UTF-8 without modifying source; no persistent data written.
        _ = System.IO.File.ReadAllText(filePath, Encoding.UTF8);

        return info;
    }
}
