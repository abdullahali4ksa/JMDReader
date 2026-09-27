// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Contracts;

/// <summary>
/// Request to execute a command through the application command gateway.
/// Provider-neutral: does not reference any specific AI provider.
/// </summary>
public sealed class CommandRequest
{
    /// <summary>Gets or sets the command name to execute (e.g., "OpenDocument").</summary>
    public string Command { get; init; } = string.Empty;

    /// <summary>Gets or sets the parameters for the command.</summary>
    public Dictionary<string, object?> Parameters { get; init; } = new();

    /// <summary>Gets or sets a unique identifier for correlating requests with responses.</summary>
    public string? RequestId { get; init; }
}
