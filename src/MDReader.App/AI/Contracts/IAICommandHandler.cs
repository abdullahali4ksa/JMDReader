// MIT License - see LICENSE file in the repository root.

using MDReader.Core.Contracts;

namespace MDReader.App.AI.Contracts;

/// <summary>
/// Handles AI-specific command processing through the application command gateway.
/// Implemented by the AI gateway adapter (see AI/Gateway/).
/// </summary>
/// <remarks>
/// This interface is the AI-facing adapter. It receives requests from an AI agent
/// (via MCP, CLI, IPC, or another controlled transport) and delegates to
/// ICommandGateway. It is NOT an LLM integration — no network calls, no API keys.
/// </remarks>
public interface IAICommandHandler
{
    /// <summary>
    /// Processes an AI command request and returns the result.
    /// </summary>
    CommandResult Process(CommandRequest request);

    /// <summary>
    /// Gets the list of command names supported by this handler.
    /// </summary>
    IReadOnlyList<string> GetSupportedCommands();
}
