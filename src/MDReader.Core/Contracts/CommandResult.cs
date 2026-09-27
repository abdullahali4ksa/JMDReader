// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Contracts;

/// <summary>
/// Result of a command execution through the application command gateway.
/// Provider-neutral: does not reference any specific AI provider.
/// </summary>
public sealed class CommandResult
{
    /// <summary>Gets a value indicating whether the command executed successfully.</summary>
    public bool Success { get; init; }

    /// <summary>Gets the result data (provider-neutral format, typically JSON or plain text).</summary>
    public string? Data { get; init; }

    /// <summary>Gets the error message if the command failed, or null if it succeeded.</summary>
    public string? Error { get; init; }

    /// <summary>Gets the request ID from the corresponding CommandRequest, for correlation.</summary>
    public string? RequestId { get; init; }
}
