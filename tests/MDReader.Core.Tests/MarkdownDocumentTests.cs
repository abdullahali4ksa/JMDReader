// MIT License - see LICENSE file in the repository root.

using Xunit;
using MDReader.Core.Markdown;

namespace MDReader.Core.Tests;

public class MarkdownDocumentTests
{
    [Fact]
    public void MarkdownDocument_HasDefaultEmptySourceText()
    {
        // Arrange & Act
        var doc = new MarkdownDocument();

        // Assert
        Assert.Equal(string.Empty, doc.SourceText);
    }

    [Fact]
    public void MarkdownDocument_SourceTextCanBeSet()
    {
        // Arrange
        var doc = new MarkdownDocument();

        // Act
        doc.SourceText = "# Hello World";

        // Assert
        Assert.Equal("# Hello World", doc.SourceText);
    }
}
