// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Json;

/// <summary>
/// Encapsulates the outcome of parsing and linting a JSON document stream (v2.0 JMD-11).
/// </summary>
public sealed class JsonParseResult
{
    /// <summary>Gets or sets whether the document was parsed successfully without syntax errors.</summary>
    public bool IsSuccess { get; set; }

    /// <summary>Gets or sets the root node of the parsed hierarchical tree.</summary>
    public JsonTreeNode? Root { get; set; }

    /// <summary>Gets or sets the human-readable syntax or I/O error message if parsing failed.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Gets or sets the 1-based line number where the syntax error occurred, or -1 if valid.</summary>
    public long ErrorLine { get; set; } = -1;

    /// <summary>Gets or sets the 0-based character/byte position on the error line, or -1 if valid.</summary>
    public long ErrorBytePosition { get; set; } = -1;

    /// <summary>Gets or sets the total number of nodes (keys, items, values) parsed into the tree.</summary>
    public int TotalNodes { get; set; }
}
