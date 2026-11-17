using System;
using System.IO;
using MDReader.Core.Analytics;
using MDReader.Core.Drafts;
using MDReader.Core.Recent;
using MDReader.Core.Session;
using Xunit;

namespace MDReader.Core.Tests;

public class UXFeaturesTests
{
    [Fact]
    public void RecentFileManager_AddAndPin_SortsCorrectly()
    {
        var manager = new RecentFileManager { MaxCount = 5 };
        manager.AddRecentFile("C:\\file1.md");
        manager.AddRecentFile("C:\\file2.md");
        manager.AddRecentFile("C:\\file3.md");

        // Pin file 1
        var pinned = manager.TogglePin("C:\\file1.md");
        Assert.True(pinned);

        var list = manager.GetRecentFiles();
        Assert.Equal(3, list.Count);
        // Pinned file should be first
        Assert.Equal("C:\\file1.md", list[0].Path);
        Assert.True(list[0].IsPinned);
    }

    [Fact]
    public void RecentFileManager_Persistence_RoundTripsJson()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"recent_test_{Guid.NewGuid():N}.json");
        try
        {
            var manager = new RecentFileManager();
            manager.AddRecentFile("D:\\test\\doc.md", isPinned: true);
            manager.AddRecentFile("D:\\test\\data.json");
            manager.SaveToFile(tempFile);

            var loaded = new RecentFileManager();
            loaded.LoadFromFile(tempFile);

            var items = loaded.GetRecentFiles();
            Assert.Equal(2, items.Count);
            Assert.Contains(items, x => x.Path.EndsWith("doc.md") && x.IsPinned);
            Assert.Contains(items, x => x.Path.EndsWith("data.json") && !x.IsPinned);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void SessionManager_SaveAndLoad_RestoresTabs()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"session_test_{Guid.NewGuid():N}.json");
        try
        {
            var state = new SessionState
            {
                OpenDocumentPaths = new() { "C:\\doc1.md", "C:\\doc2.json" },
                ActiveDocumentPath = "C:\\doc2.json",
                SelectedTabIndex = 1
            };

            SessionManager.SaveSession(state, tempFile);
            var loaded = SessionManager.LoadSession(tempFile);

            Assert.NotNull(loaded);
            Assert.Equal(2, loaded.OpenDocumentPaths.Count);
            Assert.Equal("C:\\doc2.json", loaded.ActiveDocumentPath);
            Assert.Equal(1, loaded.SelectedTabIndex);
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }

    [Fact]
    public void DraftManager_SaveListAndDelete_WorksCleanly()
    {
        var tempDir = Path.Combine(Path.GetTempPath(), $"drafts_test_{Guid.NewGuid():N}");
        try
        {
            var draft = new DraftModel
            {
                DraftId = "doc_draft_1",
                OriginalFilePath = "C:\\projects\\my_post.md",
                DocumentTitle = "my_post.md",
                Content = "# Draft in progress\nUnsaved text here."
            };

            DraftManager.SaveDraft(tempDir, draft);

            var drafts = DraftManager.GetAllDrafts(tempDir);
            Assert.Single(drafts);
            Assert.Equal("my_post.md", drafts[0].DocumentTitle);
            Assert.Equal("# Draft in progress\nUnsaved text here.", drafts[0].Content);

            DraftManager.DeleteDraft(tempDir, "doc_draft_1");
            var remaining = DraftManager.GetAllDrafts(tempDir);
            Assert.Empty(remaining);
        }
        finally
        {
            if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void ReadingAnalyticsCalculator_ComputesCorrectMetrics()
    {
        var arabicEnglishText = @"# عنوان المقال

هذا نص تجريبي باللغة العربية لاختبار دقة حساب عدد الكلمات والأحرف والوقت التقديري للقراءة الصامتة والإلقاء الصوتي.

Here is a second paragraph in English to verify bilingual word counting, line counting, and speech pacing.";

        var stats = ReadingAnalyticsCalculator.Calculate(arabicEnglishText);

        Assert.True(stats.WordCount > 25);
        Assert.True(stats.CharacterCountWithSpaces > 150);
        Assert.True(stats.CharacterCountWithoutSpaces > 120);
        Assert.True(stats.LineCount >= 4);
        Assert.True(stats.ParagraphCount >= 2);
        Assert.True(stats.SilentReadingMinutes > 0);
        Assert.True(stats.SpeakingMinutes > stats.SilentReadingMinutes); // Speaking is slower than silent reading
        Assert.NotEmpty(stats.ReadingTimeDisplay);
        Assert.NotEmpty(stats.SpeakingTimeDisplay);
    }
}
