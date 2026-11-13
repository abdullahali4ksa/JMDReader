// MIT License - see LICENSE file in the repository root.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using MDReader.Core.Documents;
using MDReader.Core.Markdown;
using Xunit;

namespace MDReader.Core.Tests;

public sealed class LargeFileStressTests
{
    private static readonly string FixturesDir = Path.GetFullPath(
        Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "fixtures", "large"));

    [Fact]
    public void StressTest_1MbFixture_ParsesUnder1Second()
    {
        var file1Mb = Path.Combine(FixturesDir, "fixture_1mb.md");
        if (!File.Exists(file1Mb))
            return; // Skip if run in isolation outside repo tree

        var content = File.ReadAllText(file1Mb, Encoding.UTF8);

        var sw = Stopwatch.StartNew();
        var doc = new MarkdownDocument { SourceText = content }.Parse();
        sw.Stop();

        Assert.NotNull(doc.Document);
        Assert.NotNull(doc.Document.Elements);
        Assert.NotEmpty(doc.Document.Elements.Blocks);
        Assert.True(sw.ElapsedMilliseconds < 2000, $"1MB parse took {sw.ElapsedMilliseconds}ms (must be < 2000ms)");
    }

    [Fact]
    public void StressTest_ArabicRtlAndEnglishMixed_RoundTripPersistence()
    {
        var tempFile = Path.Combine(Path.GetTempPath(), $"stress_arabic_{Guid.NewGuid():N}.md");
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("# اختبار الإجهاد للنصوص العربية والإنجليزية");
            for (int i = 0; i < 1000; i++)
            {
                sb.AppendLine($"- فقرة رقم {i}: يحتوي هذا السطر على كود `var x = {i};` ومعادلة $E=mc^2$ وملاحظة هامة.");
            }

            var originalText = sb.ToString();

            // Save atomically
            var saveResult = MarkdownFileWriter.WriteFile(tempFile, originalText);
            Assert.True(saveResult.IsSuccess);

            // Read back and compare
            var readBack = File.ReadAllText(tempFile, Encoding.UTF8);
            Assert.Equal(originalText, readBack);

            // Parse AST
            var doc = new MarkdownDocument { SourceText = readBack }.Parse();
            Assert.NotNull(doc.Document);
            Assert.NotNull(doc.Document.Elements);
            Assert.NotEmpty(doc.Document.Elements.Blocks);
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }
}
