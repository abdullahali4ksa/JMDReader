// MIT License - see LICENSE file in the repository root.

namespace MDReader.App.AI.Security;

/// <summary>
/// Defines the security policy for AI access to the application.
/// AI must NOT automatically have arbitrary filesystem, process, or network access.
/// Only explicit allowlisted application commands are permitted.
/// </summary>
/// <remarks>
/// See docs/AI_INTERFACE.md for the full security model and trust boundary documentation.
/// </remarks>
public static class AISecurityPolicy
{
    /// <summary>
    /// Commands that AI agents are explicitly allowed to execute.
    /// Only these commands may be processed by the AI gateway.
    /// </summary>
    public static readonly IReadOnlySet<string> AllowedCommands = new HashSet<string>
    {
        "OpenDocument",
        "CloseDocument",
        "GetCurrentDocument",
        "GetDocumentMetadata",
        "ChangeTheme",
        "NavigateTo",
        "GetApplicationState",
        "ListSupportedCommands"
    };

    /// <summary>
    /// Commands that are explicitly blocked from AI access.
    /// These operations must never be reachable through the AI gateway.
    /// </summary>
    public static readonly IReadOnlySet<string> BlockedCommands = new HashSet<string>
    {
        "ExecutePowerShell",
        "DeleteFile",
        "RunExecutable",
        "ReadCredentials",
        "WriteArbitraryFile",
        "ModifyRegistry",
        "NetworkControl"
    };
}
