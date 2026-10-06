// MIT License - see LICENSE file in the repository root.

using MDReader.Core.Markdown;
using Xunit;

namespace MDReader.Core.Tests;

/// <summary>
/// Tests for the safe-link policy. These lock in the regression fix where
/// reading Uri.Scheme on a relative target (internal anchor or file-relative
/// path) threw InvalidOperationException and crashed the renderer.
/// </summary>
public sealed class MarkdownLinkPolicyTests
{
    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com/path?q=1")]
    [InlineData("HTTPS://EXAMPLE.COM")]
    public void AbsoluteHttpUrls_AreExternal(string url)
        => Assert.True(MarkdownLinkPolicy.IsExternalHttpUrl(url));

    [Theory]
    [InlineData("#section")]              // internal anchor
    [InlineData("#1-تنسيق-النص")]         // Arabic anchor
    [InlineData("./other.md")]            // file-relative path
    [InlineData("../parent.md")]          // parent-relative path
    [InlineData("other.md")]              // bare relative path
    [InlineData("mailto:a@b.com")]        // unsupported scheme
    [InlineData("ftp://host/file")]       // unsupported scheme
    [InlineData("file:///C:/x.md")]       // local file scheme
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RelativeOrUnsupportedTargets_AreNotExternal(string? url)
        => Assert.False(MarkdownLinkPolicy.IsExternalHttpUrl(url));

    [Fact]
    public void RelativeAnchor_DoesNotThrow()
    {
        // Regression: this previously threw InvalidOperationException
        // ("This operation is not supported for a relative URI").
        var exception = Record.Exception(() => MarkdownLinkPolicy.IsExternalHttpUrl("#anchor"));
        Assert.Null(exception);
    }
}
