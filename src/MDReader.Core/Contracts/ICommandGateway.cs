// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Contracts;

/// <summary>
/// Provider-neutral contract for processing application commands.
/// This interface allows different command invokers (UI, AI, CLI, IPC)
/// to interact with the application through a single, safe layer.
/// </summary>
/// <remarks>
/// The application command layer must remain independent of any specific
/// AI provider. Adapters may connect Hermes, MCP, CLI, Local Agent,
/// IPC, or Named Pipes — all through this same contract.
/// </remarks>
public interface ICommandGateway
{
    /// <summary>
    /// Executes a command and returns the result.
    /// </summary>
    /// <param name="request">The command request containing the command name, parameters, and request ID.</param>
    /// <returns>The command result with success status, data, or error message.</returns>
    CommandResult Execute(CommandRequest request);
}
