// MIT License - see LICENSE file in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MDReader.Core.Documents;
using Xunit;

namespace MDReader.Core.Tests;

public sealed class DocumentTabTests : IDisposable
{
    private readonly string _tempDir;

    public DocumentTabTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "MDReader_TabTests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            try { Directory.Delete(_tempDir, recursive: true); } catch { }
        }
    }

    [Fact]
    public void MultiSession_SessionsRemainIsolated()
    {
        var session1 = DocumentSession.FromFile("doc1.md", "# Document 1");
        var session2 = DocumentSession.FromFile("doc2.md", "# Document 2");

        Assert.Equal("doc1.md", session1.Title);
        Assert.Equal("doc2.md", session2.Title);
        Assert.False(session1.IsDirty);
        Assert.False(session2.IsDirty);

        // Modify session 1 only
        session1.UpdateContent("# Document 1 Modified");
        Assert.True(session1.IsDirty);
        Assert.False(session2.IsDirty);
        Assert.Equal("# Document 2", session2.CurrentContent);
    }

    [Fact]
    public void MultiSession_SaveOneSession_PreservesOtherDirtyStates()
    {
        var file1 = Path.Combine(_tempDir, "tab1.md");
        var file2 = Path.Combine(_tempDir, "tab2.md");

        File.WriteAllText(file1, "# Original 1", Encoding.UTF8);
        File.WriteAllText(file2, "# Original 2", Encoding.UTF8);

        var session1 = DocumentSession.FromFile(file1, "# Original 1");
        var session2 = DocumentSession.FromFile(file2, "# Original 2");

        session1.UpdateContent("# Modified 1");
        session2.UpdateContent("# Modified 2");

        Assert.True(session1.IsDirty);
        Assert.True(session2.IsDirty);

        // Save session 1
        var saveResult = MarkdownFileWriter.WriteFile(file1, session1.CurrentContent);
        Assert.True(saveResult.IsSuccess);
        session1.MarkSaved(file1, saveResult.SavedAt);

        Assert.False(session1.IsDirty);
        Assert.True(session2.IsDirty); // session 2 remains dirty until explicitly saved
    }

    [Fact]
    public void MultiSession_DraftSessions_HaveDistinctIdsAndTitles()
    {
        var draft1 = DocumentSession.CreateNew("Untitled-1.md");
        var draft2 = DocumentSession.CreateNew("Untitled-2.md");

        Assert.NotEqual(draft1.Id, draft2.Id);
        Assert.Equal("Untitled-1.md", draft1.Title);
        Assert.Equal("Untitled-2.md", draft2.Title);
        Assert.Null(draft1.FilePath);
        Assert.Null(draft2.FilePath);
    }
}
