// MIT License - see LICENSE file in the repository root.

namespace MDReader.App.AI.Contracts;

/// <summary>
/// Metadata describing a single AI command: its name, human-readable description,
/// and the parameters it accepts.
/// </summary>
public sealed record CommandMetadata(string Name, string Description, string[] Parameters);
