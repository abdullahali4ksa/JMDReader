// MIT License - see LICENSE file in the repository root.

using System.Collections.Generic;
using System.Text.Json;

namespace MDReader.Core.Json;

/// <summary>
/// Represents a single node in a hierarchical JSON document tree (v2.0 JMD-11).
/// Supports objects, arrays, and primitives with color-coded token metadata and JSONPath tracking.
/// </summary>
public sealed class JsonTreeNode
{
    /// <summary>Gets or sets the property key name (or array index e.g. "[0]"). Empty for root.</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Gets or sets the formatted string representation of primitive values, or null for containers.</summary>
    public string? Value { get; set; }

    /// <summary>Gets or sets the JSON value kind (Object, Array, String, Number, True, False, Null).</summary>
    public JsonValueKind ValueKind { get; set; }

    /// <summary>Gets or sets the human-readable type identifier: "object", "array", "string", "number", "boolean", "null".</summary>
    public string TypeName { get; set; } = string.Empty;

    /// <summary>Gets or sets the summary badge text e.g. "{ 4 keys }" or "[ 12 items ]".</summary>
    public string Summary { get; set; } = string.Empty;

    /// <summary>Gets or sets the child nodes when this node is an Object or Array.</summary>
    public List<JsonTreeNode> Children { get; set; } = new();

    /// <summary>Gets or sets whether this node is expanded in the hierarchical tree view.</summary>
    public bool IsExpanded { get; set; } = true;

    /// <summary>Gets or sets the standard JSONPath expression pointing to this node, e.g. "$.store.books[0].title".</summary>
    public string JsonPath { get; set; } = "$";

    /// <summary>Gets whether this node has children (is a non-empty container).</summary>
    public bool HasChildren => Children.Count > 0;
}
