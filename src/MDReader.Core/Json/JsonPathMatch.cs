// MIT License - see LICENSE file in the repository root.

using System.Text.Json;

namespace MDReader.Core.Json;

/// <summary>
/// Represents a single matching node found by a JSONPath query (v2.0 JMD-12).
/// </summary>
public sealed class JsonPathMatch
{
    /// <summary>
    /// The normalized JSONPath pointing to this exact element (e.g. "$.store.books[0].title").
    /// </summary>
    public string Path { get; init; } = "$";

    /// <summary>
    /// The raw string or text representation of the matched element.
    /// </summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>
    /// The JSON value kind (Object, Array, String, Number, True, False, Null).
    /// </summary>
    public JsonValueKind ValueKind { get; init; } = JsonValueKind.Undefined;

    /// <summary>
    /// The underlying System.Text.Json element.
    /// </summary>
    public JsonElement Element { get; init; }

    public override string ToString() => $"{Path} => {Value}";
}
