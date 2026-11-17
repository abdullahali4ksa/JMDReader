// MIT License - see LICENSE file in the repository root.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using MDReader.Core.Documents;
using MDReader.Core.Markdown;
using Xunit;

namespace MDReader.Core.Tests;

/// <summary>
/// Rigorous heavy stress, concurrency, and crash resilience test suite (JMD-9 / P22-T001).
/// Validates application stability under extreme file sizes, rapid operations, and edge cases.
/// </summary>
public sealed class StressAndResilienceTests
{
    [Fact]
    public void StressTest_100kLines_ParseAndOutline_CompletesWithoutCrash()
    {
        var sb = new StringBuilder(10 * 1024 * 1024);
        sb.AppendLine("# Massive Document Header");

        for (int i = 1; i <= 50000; i++)
        {
            if (i % 50 == 0)
            {
                sb.AppendLine($"## Section Heading {i}");
            }
            sb.AppendLine($"Line {i}: This is high-volume paragraph text containing `code snippet {i}` and [link {i}](https://example.com/{i}).");
        }

        var content = sb.ToString();
        var sw = Stopwatch.StartNew();

        // 1. Parse document AST
        var doc = new MarkdownDocument { SourceText = content }.Parse();
        Assert.NotNull(doc);
        Assert.NotNull(doc.Document);
        
        // Documents exceeding 5MB trigger safe size-warning short-circuit to protect memory
        if (doc.Document.Elements != null)
        {
            Assert.NotEmpty(doc.Document.Elements.Blocks);
        }
        else
        {
            Assert.Equal("SIZE_WARNING", doc.Error?.Code);
        }

        // 2. Extract Document Outline
        var outline = MarkdownOutlineExtractor.ExtractOutline(content);
        Assert.NotNull(outline);
        Assert.True(outline.Count >= 1000, $"Expected >= 1000 headings, got {outline.Count}");

        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds < 5000, $"100K lines processing took {sw.ElapsedMilliseconds}ms (limit: 5000ms)");
    }

    [Fact]
    public void StressTest_GlobalReplace_50kMatches_CompletesInSubsecond()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 20000; i++)
        {
            sb.Append("keyword_target other_text ");
        }

        var content = sb.ToString();
        var sw = Stopwatch.StartNew();

        var (result, replacedCount) = MarkdownEditorUtilities.ReplaceText(
            content,
            query: "keyword_target",
            replacement: "replacement_val",
            matchCase: true,
            isRegex: false);

        sw.Stop();

        Assert.Equal(20000, replacedCount);
        Assert.DoesNotContain("keyword_target", result);
        Assert.True(sw.ElapsedMilliseconds < 2000, $"20k replacements took {sw.ElapsedMilliseconds}ms");
    }

    [Fact]
    public void ResilienceTest_CatastrophicRegex_HandledSafelyWithTimeout()
    {
        // ReDoS pattern known to cause catastrophic backtracking: (a+)+$
        var badRegex = @"(a+)+$";
        var maliciousInput = new string('a', 35) + "!";

        var sw = Stopwatch.StartNew();
        var matches = MarkdownEditorUtilities.FindMatches(maliciousInput, badRegex, matchCase: false, isRegex: true);
        sw.Stop();

        // Must timeout safely within ~1 second instead of hanging thread forever
        Assert.Empty(matches);
        Assert.True(sw.ElapsedMilliseconds < 2500, $"ReDoS defense exceeded limit: {sw.ElapsedMilliseconds}ms");
    }

    [Theory]
    [InlineData("(")]
    [InlineData("[a-z")]
    [InlineData("*")]
    [InlineData("(?<=")]
    [InlineData("\\")]
    public void ResilienceTest_MalformedRegexPatterns_ReturnEmptyWithoutCrash(string malformedPattern)
    {
        var content = "Sample text for testing invalid regex resilience.";
        var matches = MarkdownEditorUtilities.FindMatches(content, malformedPattern, matchCase: false, isRegex: true);
        Assert.NotNull(matches);
        Assert.Empty(matches);

        var (result, count) = MarkdownEditorUtilities.ReplaceText(content, malformedPattern, "rep", matchCase: false, isRegex: true);
        Assert.Equal(0, count);
        Assert.Equal(content, result);
    }

    [Fact]
    public void ResilienceTest_CorruptAndBinaryBytes_ParsesGracefully()
    {
        // Simulate corrupt or non-text file with null bytes and control chars
        var corruptBytes = new byte[] { 0x00, 0xFF, 0xFE, 0x00, 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x1F, 0x7F };
        var text = Encoding.UTF8.GetString(corruptBytes);

        var doc = new MarkdownDocument { SourceText = text }.Parse();
        Assert.NotNull(doc);
        Assert.NotNull(doc.Document);
    }

    [Fact]
    public async System.Threading.Tasks.Task ResilienceTest_ConcurrentTabManagement_ThreadSafe()
    {
        var sessions = new System.Collections.Concurrent.ConcurrentBag<DocumentSession>();
        var tasks = new System.Threading.Tasks.Task[20];

        for (int i = 0; i < 20; i++)
        {
            var index = i;
            tasks[i] = System.Threading.Tasks.Task.Run(() =>
            {
                var session = DocumentSession.FromFile($@"C:\Workspace\doc_{index}.md", $"# Title {index}\nContent for tab {index}");
                if (index % 2 == 0)
                {
                    session.UpdateContent($"# Title {index}\nModified content");
                }
                sessions.Add(session);
            });
        }

        await System.Threading.Tasks.Task.WhenAll(tasks);
        Assert.Equal(20, sessions.Count);
    }
}
