using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MDReader.Core.Analytics;

/// <summary>
/// High-performance calculator for document analytics, speech timing, and reading grade.
/// </summary>
public static class ReadingAnalyticsCalculator
{
    private static readonly Regex WordRegex = new(@"[\p{L}\p{N}]+", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
    private static readonly Regex ParagraphRegex = new(@"(?:\r?\n\s*){2,}", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    public static ReadingAnalytics Calculate(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return new ReadingAnalytics();
        }

        int charWithSpaces = content.Length;
        int charWithoutSpaces = content.Count(c => !char.IsWhiteSpace(c));

        var lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        int lineCount = lines.Length;

        int wordCount = 0;
        try
        {
            wordCount = WordRegex.Matches(content).Count;
        }
        catch (RegexMatchTimeoutException)
        {
            // Fallback to simple split
            wordCount = content.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        int paragraphCount = 1;
        try
        {
            var pMatches = ParagraphRegex.Matches(content.Trim());
            paragraphCount = pMatches.Count + 1;
        }
        catch
        {
            paragraphCount = lines.Count(l => !string.IsNullOrWhiteSpace(l));
        }

        // Standard reading speed: 200 words per minute
        double silentMinutes = wordCount / 200.0;
        // Standard speaking / presentation speed: 130 words per minute
        double speakingMinutes = wordCount / 130.0;

        // Readability estimate
        string level = "Easy / سهل";
        if (wordCount > 0)
        {
            double avgWordLength = (double)charWithoutSpaces / wordCount;
            if (avgWordLength > 6.5) level = "Technical & Advanced / تخصصي ومتقدم";
            else if (avgWordLength > 5.0) level = "Standard / متوسط";
        }

        return new ReadingAnalytics
        {
            WordCount = wordCount,
            CharacterCountWithSpaces = charWithSpaces,
            CharacterCountWithoutSpaces = charWithoutSpaces,
            LineCount = lineCount,
            ParagraphCount = Math.Max(1, paragraphCount),
            SilentReadingMinutes = silentMinutes,
            SpeakingMinutes = speakingMinutes,
            ReadabilityLevel = level
        };
    }
}
