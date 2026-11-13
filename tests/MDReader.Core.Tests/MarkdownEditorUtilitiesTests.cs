using System.IO;
using MDReader.Core.Markdown;
using Xunit;

namespace MDReader.Core.Tests;

public class MarkdownEditorUtilitiesTests
{
    [Theory]
    [InlineData("photo.png", true)]
    [InlineData("image.jpg", true)]
    [InlineData("banner.JPEG", true)]
    [InlineData("chart.svg", true)]
    [InlineData("anim.gif", true)]
    [InlineData("doc.md", false)]
    [InlineData("file.txt", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsImageFile_CorrectlyIdentifiesImages(string? input, bool expected)
    {
        var result = MarkdownEditorUtilities.IsImageFile(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GenerateMarkdownImageSnippet_CreatesStandardMarkdownTag()
    {
        var snippet = MarkdownEditorUtilities.GenerateMarkdownImageSnippet("Architecture Diagram", "images/arch.png");
        Assert.Equal("![Architecture Diagram](images/arch.png)", snippet);

        var defaultSnippet = MarkdownEditorUtilities.GenerateMarkdownImageSnippet("", "images\\logo.png");
        Assert.Equal("![image](images/logo.png)", defaultSnippet);
    }

    [Fact]
    public void FindMatches_CaseInsensitiveAndSensitive()
    {
        var content = "The quick brown fox jumps over the lazy dog. The fox is quick.";

        // Case-insensitive
        var matches = MarkdownEditorUtilities.FindMatches(content, "the", matchCase: false, isRegex: false);
        Assert.Equal(3, matches.Count);

        // Case-sensitive
        var sensitiveMatches = MarkdownEditorUtilities.FindMatches(content, "The", matchCase: true, isRegex: false);
        Assert.Equal(2, sensitiveMatches.Count);
    }

    [Fact]
    public void FindMatches_WithRegex()
    {
        var content = "Question Q1: pass, Question Q24: review, Question Q183: verified.";
        var matches = MarkdownEditorUtilities.FindMatches(content, @"Q\d+", matchCase: false, isRegex: true);

        Assert.Equal(3, matches.Count);
        Assert.Equal("Q1", content.Substring(matches[0].Index, matches[0].Length));
        Assert.Equal("Q24", content.Substring(matches[1].Index, matches[1].Length));
        Assert.Equal("Q183", content.Substring(matches[2].Index, matches[2].Length));
    }

    [Fact]
    public void ReplaceText_SingleOccurrence()
    {
        var content = "Hello world! Hello everyone!";
        var (result, count) = MarkdownEditorUtilities.ReplaceText(
            content,
            query: "Hello",
            replacement: "Greetings",
            matchCase: true,
            isRegex: false,
            targetIndex: 13,
            targetLength: 5);

        Assert.Equal(1, count);
        Assert.Equal("Hello world! Greetings everyone!", result);
    }

    [Fact]
    public void ReplaceText_AllOccurrences_PlainAndRegex()
    {
        var content = "Apple, banana, apple, orange, APPLE.";
        
        // Plain case-insensitive replace all
        var (resPlain, countPlain) = MarkdownEditorUtilities.ReplaceText(content, "apple", "peach", matchCase: false, isRegex: false);
        Assert.Equal(3, countPlain);
        Assert.Equal("peach, banana, peach, orange, peach.", resPlain);

        // Regex replace all
        var doc = "item-10, item-20, item-300";
        var (resRegex, countRegex) = MarkdownEditorUtilities.ReplaceText(doc, @"item-(\d+)", "product_$1", matchCase: false, isRegex: true);
        Assert.Equal(3, countRegex);
        Assert.Equal("product_10, product_20, product_300", resRegex);
    }
}
