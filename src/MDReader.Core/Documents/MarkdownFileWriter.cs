// MIT License - see LICENSE file in the repository root.

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MDReader.Core.Documents;

/// <summary>
/// Result of a file save operation.
/// </summary>
public sealed class SaveResult
{
    public bool IsSuccess { get; init; }
    public string? ErrorMessage { get; init; }
    public string FilePath { get; init; } = string.Empty;
    public long BytesWritten { get; init; }
    public DateTimeOffset SavedAt { get; init; }

    public static SaveResult Success(string filePath, long bytesWritten, DateTimeOffset savedAt) =>
        new() { IsSuccess = true, FilePath = filePath, BytesWritten = bytesWritten, SavedAt = savedAt };

    public static SaveResult Failure(string filePath, string errorMessage) =>
        new() { IsSuccess = false, FilePath = filePath, ErrorMessage = errorMessage };
}

/// <summary>
/// Provides safe, atomic file writing for Markdown documents.
/// Uses a temporary file staging mechanism and atomic rename/replace to prevent corruption.
/// </summary>
public static class MarkdownFileWriter
{
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    /// <summary>
    /// Writes content to a Markdown file atomically.
    /// </summary>
    /// <param name="filePath">Target destination path.</param>
    /// <param name="content">Markdown text content.</param>
    /// <param name="createBackup">Whether to create a backup (.bak) of the previous version if it exists.</param>
    /// <returns>A SaveResult indicating success or failure.</returns>
    public static SaveResult WriteFile(string filePath, string content, bool createBackup = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return SaveResult.Failure(filePath ?? string.Empty, "File path cannot be empty.");

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (extension != ".md" && extension != ".markdown")
            return SaveResult.Failure(filePath, $"Unsupported file extension '{extension}'. Only .md and .markdown are allowed.");

        string? tempFilePath = null;
        try
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var dir = string.IsNullOrEmpty(directory) ? "." : directory;
            tempFilePath = Path.Combine(dir, $"{Path.GetFileName(filePath)}.tmp.{Guid.NewGuid():N}");

            // Write content to temporary file first
            File.WriteAllText(tempFilePath, content ?? string.Empty, Utf8NoBom);

            var tempInfo = new FileInfo(tempFilePath);
            var bytesWritten = tempInfo.Length;

            if (File.Exists(filePath))
            {
                if (createBackup)
                {
                    var backupPath = filePath + ".bak";
                    File.Copy(filePath, backupPath, overwrite: true);
                }

                // Atomic replace on Windows
                File.Move(tempFilePath, filePath, overwrite: true);
            }
            else
            {
                File.Move(tempFilePath, filePath);
            }

            tempFilePath = null; // Successfully moved
            return SaveResult.Success(filePath, bytesWritten, DateTimeOffset.UtcNow);
        }
        catch (Exception ex)
        {
            return SaveResult.Failure(filePath, ex.Message);
        }
        finally
        {
            // Clean up temporary file if it was left behind due to an error
            if (tempFilePath != null && File.Exists(tempFilePath))
            {
                try { File.Delete(tempFilePath); } catch { /* best effort cleanup */ }
            }
        }
    }

    /// <summary>
    /// Asynchronously writes content to a Markdown file atomically.
    /// </summary>
    public static async Task<SaveResult> WriteFileAsync(string filePath, string content, bool createBackup = false, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => WriteFile(filePath, content, createBackup), cancellationToken);
    }
}
