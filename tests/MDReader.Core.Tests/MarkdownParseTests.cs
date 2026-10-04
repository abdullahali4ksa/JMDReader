using Xunit;
using MDReader.Core.Markdown;

namespace MDReader.Core.Tests;

public class MarkdownParseResultTests
{
    [Fact]
    public void Parse_EmptyText_ReturnsSuccess()
    {
        var doc = new MarkdownDocument { SourceText = "" };
        var result = doc.Parse();

        Assert.True(result.Success);
        Assert.NotNull(result.Document);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Parse_PlainText_ReturnsSuccess()
    {
        var doc = new MarkdownDocument { SourceText = "Hello world" };
        var result = doc.Parse();

        Assert.True(result.Success);
        Assert.Equal("Hello world", result.Document.SourceText);
    }

    [Fact]
    public void Parse_LargeText_ReturnsSizeWarning()
    {
        // Create a string above 5 MB threshold (approx 6 MB of text)
        var largeText = new System.Text.StringBuilder();
        for (int i = 0; i < 130000; i++)
        {
            largeText.Append("This is a very large markdown line with many characters to exceed the warning threshold of five megabytes. ");
        }

        var doc = new MarkdownDocument { SourceText = largeText.ToString() };
        var result = doc.Parse();

        Assert.True(result.Success);
        Assert.NotNull(result.Error);
        Assert.Equal("SIZE_WARNING", result.Error.Code);
        Assert.True(result.Error.IsRecoverable);
    }
}
