// MIT License - see LICENSE file in the repository root.

using System.Linq;
using MDReader.Core.Markdown;
using Xunit;

namespace MDReader.Core.Tests;

public class MarkdownOutlineTests
{
    [Fact]
    public void ExtractOutline_StandardHeadings_ExtractsCorrectLevelsAndIds()
    {
        string md = @"
# Project Overview
Some introduction text here.

## Architecture
Details on architecture.

### Sub Component
More details.

```csharp
# This is a comment inside code block, should be ignored
int x = 10;
```

## Architecture
Duplicate heading.
";

        var items = MarkdownOutlineExtractor.ExtractOutline(md);

        Assert.Equal(4, items.Count);

        Assert.Equal("Project Overview", items[0].Title);
        Assert.Equal(1, items[0].Level);
        Assert.Equal("project-overview", items[0].Id);

        Assert.Equal("Architecture", items[1].Title);
        Assert.Equal(2, items[1].Level);
        Assert.Equal("architecture", items[1].Id);

        Assert.Equal("Sub Component", items[2].Title);
        Assert.Equal(3, items[2].Level);
        Assert.Equal("sub-component", items[2].Id);

        Assert.Equal("Architecture", items[3].Title);
        Assert.Equal(2, items[3].Level);
        Assert.Equal("architecture-2", items[3].Id); // duplicate handled with suffix
    }

    [Fact]
    public void ExtractOutline_ArabicHeadings_GeneratesValidItems()
    {
        string md = @"
# نظرة عامة على المشروع
محتوى باللغة العربية.

## خطة العمل
التفاصيل.
";

        var items = MarkdownOutlineExtractor.ExtractOutline(md);

        Assert.Equal(2, items.Count);
        Assert.Equal("نظرة عامة على المشروع", items[0].Title);
        Assert.Equal(1, items[0].Level);
        Assert.Equal("خطة العمل", items[1].Title);
        Assert.Equal(2, items[1].Level);
    }

    [Fact]
    public void ComputeAnalytics_CalculatesWordsLinesAndReadingTime()
    {
        string md = "First line with some words.\nSecond line with more content and explanations.";

        var stats = MarkdownOutlineExtractor.ComputeAnalytics(md);

        Assert.True(stats.WordCount > 5);
        Assert.Equal(2, stats.LineCount);
        Assert.Equal(1, stats.EstimatedReadingMinutes);
        Assert.Contains("words", stats.FormattedSummary);
    }
}
