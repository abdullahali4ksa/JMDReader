// MIT License - see LICENSE file in the repository root.

using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MDReader.Core.Json;

/// <summary>
/// High-performance DOM parser and syntax linter for JSON documents (v2.0 JMD-11).
/// Converts raw JSON into an interactive <see cref="JsonTreeNode"/> hierarchy using System.Text.Json.
/// </summary>
public static class JsonTreeParser
{
    private static readonly JsonDocumentOptions DefaultOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        MaxDepth = 128
    };

    /// <summary>
    /// Parses a JSON text string into an interactive tree model with syntax linting and JSONPath tracking.
    /// </summary>
    /// <param name="jsonText">Raw JSON or JSONC string content.</param>
    /// <returns>A typed <see cref="JsonParseResult"/> containing the root tree or line-accurate syntax error.</returns>
    public static JsonParseResult Parse(string? jsonText)
    {
        if (string.IsNullOrWhiteSpace(jsonText))
        {
            return new JsonParseResult
            {
                IsSuccess = false,
                ErrorMessage = "Document is empty or contains only whitespace.",
                ErrorLine = 1,
                ErrorBytePosition = 0
            };
        }

        try
        {
            using var document = JsonDocument.Parse(jsonText, DefaultOptions);
            int nodeCounter = 0;
            var root = BuildNode(string.Empty, document.RootElement, "$", ref nodeCounter);

            return new JsonParseResult
            {
                IsSuccess = true,
                Root = root,
                TotalNodes = nodeCounter
            };
        }
        catch (JsonException jex)
        {
            return new JsonParseResult
            {
                IsSuccess = false,
                ErrorMessage = jex.Message,
                ErrorLine = jex.LineNumber ?? 1,
                ErrorBytePosition = jex.BytePositionInLine ?? 0
            };
        }
        catch (Exception ex)
        {
            return new JsonParseResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message,
                ErrorLine = 1,
                ErrorBytePosition = 0
            };
        }
    }

    private static JsonTreeNode BuildNode(string key, JsonElement element, string currentPath, ref int nodeCounter)
    {
        nodeCounter++;
        var node = new JsonTreeNode
        {
            Key = key,
            ValueKind = element.ValueKind,
            JsonPath = currentPath
        };

        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                node.TypeName = "object";
                var objChildren = new List<JsonTreeNode>();
                foreach (var prop in element.EnumerateObject())
                {
                    var childPath = string.IsNullOrEmpty(key) && currentPath == "$"
                        ? $"$.{prop.Name}"
                        : $"{currentPath}.{prop.Name}";
                    objChildren.Add(BuildNode(prop.Name, prop.Value, childPath, ref nodeCounter));
                }
                node.Children = objChildren;
                node.Summary = $"{{ {objChildren.Count} {(objChildren.Count == 1 ? "key" : "keys")} }}";
                break;

            case JsonValueKind.Array:
                node.TypeName = "array";
                var arrChildren = new List<JsonTreeNode>();
                int index = 0;
                foreach (var item in element.EnumerateArray())
                {
                    var childPath = $"{currentPath}[{index}]";
                    arrChildren.Add(BuildNode($"[{index}]", item, childPath, ref nodeCounter));
                    index++;
                }
                node.Children = arrChildren;
                node.Summary = $"[ {arrChildren.Count} {(arrChildren.Count == 1 ? "item" : "items")} ]";
                break;

            case JsonValueKind.String:
                node.TypeName = "string";
                node.Value = element.GetString();
                node.Summary = $"\"{node.Value}\"";
                break;

            case JsonValueKind.Number:
                node.TypeName = "number";
                node.Value = element.GetRawText();
                node.Summary = node.Value;
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                node.TypeName = "boolean";
                node.Value = element.GetBoolean() ? "true" : "false";
                node.Summary = node.Value;
                break;

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
            default:
                node.TypeName = "null";
                node.Value = "null";
                node.Summary = "null";
                break;
        }

        return node;
    }
}
