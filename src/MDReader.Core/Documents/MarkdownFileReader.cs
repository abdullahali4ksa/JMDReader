// MIT License - see LICENSE file in the repository root.

using System.IO;
using System.Text;

namespace MDReader.Core.Documents;

/// <summary>Provides safe, read-only file reading for Markdown, JSON, and Text documents.</summary>
public static class MarkdownFileReader
{
    public const int SizeWarningBytes = 5 * 1024 * 1024; // 5 MB per Blueprint §3.2

    /// <summary>
    /// File extensions supported by MDReader across Markdown, JSON, and plain text formats.
    /// </summary>
    public static readonly string[] SupportedExtensions = [".md", ".markdown", ".json", ".jsonc", ".jsonl", ".txt"];

    /// <summary>
    /// Checks whether the given file path or extension is supported by MDReader.
    /// </summary>
    public static bool IsSupportedExtension(string? filePathOrExt)
    {
        if (string.IsNullOrWhiteSpace(filePathOrExt)) return false;
        var ext = filePathOrExt.Contains('.') 
            ? System.IO.Path.GetExtension(filePathOrExt).ToLowerInvariant() 
            : ("." + filePathOrExt.ToLowerInvariant());
        return System.Array.Exists(SupportedExtensions, s => s == ext);
    }

    /// <summary>Reads a document file safely and validates access/encoding.</summary>
    /// <param name="filePath">Path to the document file.</param>
    /// <returns>A DocumentInfo if the file is valid; throws typed exceptions for recoverable errors.</returns>
    public static DocumentInfo ReadFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new System.IO.FileNotFoundException("File path is empty.");

        var extension = System.IO.Path.GetExtension(filePath).ToLowerInvariant();
        if (!IsSupportedExtension(extension))
            throw new System.ArgumentException($"Unsupported file extension: '{extension}'. Supported extensions: .md, .markdown, .json, .jsonc, .jsonl, .txt");

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
