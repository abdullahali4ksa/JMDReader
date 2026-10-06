// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Markdown;

/// <summary>
/// Pure, platform-neutral link classification used by the renderer to enforce
/// the safe-link policy (Phase 3). Kept in Core so the rules are unit-testable
/// without a WinUI host.
/// </summary>
public static class MarkdownLinkPolicy
{
    /// <summary>
    /// Determines whether a link target is an explicit external HTTP(S) URL that
    /// may be launched through the OS handler.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="System.UriKind.Absolute"/> deliberately: a relative target
    /// (an internal anchor such as <c>#section</c>, or a file-relative path such as
    /// <c>./other.md</c>) is not an external URL, and reading <c>Uri.Scheme</c> on a
    /// relative <see cref="System.Uri"/> throws <see cref="System.InvalidOperationException"/>.
    /// Restricting to absolute URIs avoids that failure entirely.
    /// </remarks>
    public static bool IsExternalHttpUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return System.Uri.TryCreate(url, System.UriKind.Absolute, out var uri)
            && (uri.Scheme == System.Uri.UriSchemeHttp || uri.Scheme == System.Uri.UriSchemeHttps)
            && uri.Host.Length > 0;
    }
}
