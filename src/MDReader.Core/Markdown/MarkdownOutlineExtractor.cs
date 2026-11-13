// MIT License - see LICENSE file in the repository root.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MDReader.Core.Documents;

namespace MDReader.Core.Markdown;

/// <summary>
/// Extracts document outline / table of contents headings and reading statistics.
/// </summary>
public static class MarkdownOutlineExtractor
{
    private static readonly Regex HeadingRegex = new(@"^(#{1,6})\s+(.+)$", RegexOptions.Compiled);
    private static readonly Regex SlugCleanRegex = new(@"[^\w\s-]", RegexOptions.Compiled);
    private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex WordSplitRegex = new(@"[\s\p{P}]+", RegexOptions.Compiled);

    /// <summary>
    /// Extracts all headings (H1-H6) from markdown text into an ordered list of outline items.
    /// </summary>
    public static IReadOnlyList<DocumentOutlineItem> ExtractOutline(string? markdown)
    {
        var items = new List<DocumentOutlineItem>();
        if (string.IsNullOrWhiteSpace(markdown))
            return items;

        var seenIds = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var lines = markdown.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        bool inCodeBlock = false;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.Trim();

            // Track code blocks so code comments like `# Comment` are not treated as headings
            if (trimmed.StartsWith("```"))
            {
                inCodeBlock = !inCodeBlock;
                continue;
            }

            if (inCodeBlock)
                continue;

            var match = HeadingRegex.Match(trimmed);
            if (match.Success)
            {
                int level = match.Groups[1].Length;
                string rawTitle = match.Groups[2].Value.Trim();

                // Strip basic inline formatting like **bold** or `code` for outline display
                string cleanTitle = rawTitle.Replace("**", "").Replace("*", "").Replace("`", "").Trim();
                if (string.IsNullOrEmpty(cleanTitle))
                    continue;

                // Generate anchor slug matching viewer.html
                string baseSlug = SlugCleanRegex.Replace(cleanTitle.ToLowerInvariant(), "").Trim();
                baseSlug = WhitespaceRegex.Replace(baseSlug, "-");
                if (string.IsNullOrEmpty(baseSlug))
                    baseSlug = $"section-{i + 1}";

                string uniqueId = baseSlug;
                if (seenIds.TryGetValue(baseSlug, out int count))
                {
                    count++;
                    seenIds[baseSlug] = count;
                    uniqueId = $"{baseSlug}-{count}";
                }
                else
                {
                    seenIds[baseSlug] = 1;
                }

                items.Add(new DocumentOutlineItem
                {
                    Id = uniqueId,
                    Title = cleanTitle,
                    Level = level,
                    LineNumber = i + 1
                });
            }
        }

        return items;
    }

    /// <summary>
    /// Computes word count, character count, line count, and estimated reading time.
    /// </summary>
    public static DocumentAnalytics ComputeAnalytics(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return new DocumentAnalytics(0, 0, 0, 0);

        int characterCount = markdown.Length;
        var lines = markdown.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        int lineCount = lines.Length;

        // Word count supporting Arabic, English, and digits
        var tokens = WordSplitRegex.Split(markdown);
        int wordCount = 0;
        foreach (var token in tokens)
        {
            if (!string.IsNullOrWhiteSpace(token))
                wordCount++;
        }

        // Standard reading speed: 200 words per minute
        int readingMinutes = Math.Max(1, (int)Math.Ceiling(wordCount / 200.0));

        return new DocumentAnalytics(wordCount, characterCount, lineCount, readingMinutes);
    }
}

/// <summary>
/// Document reading metrics (word count, character count, lines, reading time).
/// </summary>
public readonly record struct DocumentAnalytics(
    int WordCount,
    int CharacterCount,
    int LineCount,
    int EstimatedReadingMinutes
)
{
    public string FormattedSummary => $"{WordCount:N0} words • ~{EstimatedReadingMinutes} min read";
}
