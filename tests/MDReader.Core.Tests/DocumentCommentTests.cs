// MIT License - see LICENSE file in the repository root.

using System;
using MDReader.Core.Documents;
using Xunit;

namespace MDReader.Core.Tests;

public class DocumentCommentTests
{
    [Fact]
    public void DocumentComment_DefaultValues_AreInitialized()
    {
        var comment = new DocumentComment();

        Assert.False(string.IsNullOrWhiteSpace(comment.Id));
        Assert.Empty(comment.SelectedText);
        Assert.Empty(comment.CommentText);
        Assert.False(comment.HasQuote);
        Assert.False(string.IsNullOrWhiteSpace(comment.CreatedAtDisplay));
    }

    [Fact]
    public void DocumentComment_WithQuoteAndText_HasQuoteIsTrue()
    {
        var comment = new DocumentComment
        {
            SelectedText = "A critical finding in the report",
            CommentText = "We need to verify this against NIST standards."
        };

        Assert.True(comment.HasQuote);
        Assert.Equal("A critical finding in the report", comment.SelectedText);
        Assert.Equal("We need to verify this against NIST standards.", comment.CommentText);
        Assert.Contains(DateTimeOffset.Now.ToString("yyyy-MM-dd"), comment.CreatedAtDisplay);
    }

    [Fact]
    public void DocumentComment_WhitespaceQuote_HasQuoteIsFalse()
    {
        var comment = new DocumentComment
        {
            SelectedText = "   ",
            CommentText = "Some note"
        };

        Assert.False(comment.HasQuote);
    }
}
