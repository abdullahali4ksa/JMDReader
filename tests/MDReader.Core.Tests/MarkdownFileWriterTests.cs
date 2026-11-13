// MIT License - see LICENSE file in the repository root.

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MDReader.Core.Documents;
using Xunit;

namespace MDReader.Core.Tests;

public sealed class MarkdownFileWriterTests : IDisposable
{
    private readonly string _tempDirectory;

    public MarkdownFileWriterTests()
    {
        _tempDirectory = Path.Combine(Path.GetTempPath(), "MDReader_Tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            try { Directory.Delete(_tempDirectory, recursive: true); } catch { }
        }
    }

    [Fact]
    public void WriteFile_NewFile_WritesContentSuccessfully()
    {
        var filePath = Path.Combine(_tempDirectory, "test_new.md");
        var content = "# Hello World\n\nThis is a test markdown file.";

        var result = MarkdownFileWriter.WriteFile(filePath, content);

        Assert.True(result.IsSuccess);
        Assert.True(File.Exists(filePath));
        var readBack = File.ReadAllText(filePath, Encoding.UTF8);
        Assert.Equal(content, readBack);
    }

    [Fact]
    public void WriteFile_OverwriteExistingFile_UpdatesContentAtomically()
    {
        var filePath = Path.Combine(_tempDirectory, "test_overwrite.md");
        File.WriteAllText(filePath, "Initial Content", Encoding.UTF8);

        var newContent = "Updated Content with Atomic Overwrite";
        var result = MarkdownFileWriter.WriteFile(filePath, newContent, createBackup: true);

        Assert.True(result.IsSuccess);
        var readBack = File.ReadAllText(filePath, Encoding.UTF8);
        Assert.Equal(newContent, readBack);

        // Backup file check
        Assert.True(File.Exists(filePath + ".bak"));
        Assert.Equal("Initial Content", File.ReadAllText(filePath + ".bak", Encoding.UTF8));
    }

    [Fact]
    public void WriteFile_ArabicAndRtlContent_PreservesUtf8Correctly()
    {
        var filePath = Path.Combine(_tempDirectory, "arabic_document.md");
        var arabicText = "# تقرير مشروع قارئ ومحرر الماركداون\n\n- دعم كامل للغة العربية والاتجاه من اليمين لليسار.\n- ميزات Google Antigravity المتقدمة.";

        var result = MarkdownFileWriter.WriteFile(filePath, arabicText);

        Assert.True(result.IsSuccess);
        var readBack = File.ReadAllText(filePath, Encoding.UTF8);
        Assert.Equal(arabicText, readBack);
    }

    [Fact]
    public void WriteFile_UnsupportedExtension_ReturnsFailure()
    {
        var filePath = Path.Combine(_tempDirectory, "document.txt");
        var result = MarkdownFileWriter.WriteFile(filePath, "test");

        Assert.False(result.IsSuccess);
        Assert.Contains("Unsupported file extension", result.ErrorMessage);
    }

    [Fact]
    public void WriteFile_EmptyPath_ReturnsFailure()
    {
        var result = MarkdownFileWriter.WriteFile("", "test");
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void DocumentSession_DirtyTracking_WorksCorrectly()
    {
        var session = DocumentSession.CreateNew("Notes.md", "# Initial Notes");

        // Fresh session is not dirty
        Assert.False(session.IsDirty);
        Assert.Equal("Notes.md", session.Title);

        // Content changed -> Dirty
        session.UpdateContent("# Initial Notes\n\nAdded paragraph");
        Assert.True(session.IsDirty);

        // Content restored to original -> Not dirty
        session.UpdateContent("# Initial Notes");
        Assert.False(session.IsDirty);

        // Save session -> Not dirty
        session.UpdateContent("# Final Notes");
        Assert.True(session.IsDirty);
        session.MarkSaved("C:\\Notes.md", DateTimeOffset.UtcNow);
        Assert.False(session.IsDirty);
        Assert.Equal("Notes.md", session.Title);
        Assert.Equal("C:\\Notes.md", session.FilePath);
    }
}
