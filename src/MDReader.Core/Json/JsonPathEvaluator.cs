// MIT License - see LICENSE file in the repository root.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MDReader.Core.Json;

/// <summary>
/// High-performance JSONPath query evaluator (v2.0 JMD-12).
/// Supports dot-notation, bracket-notation, array wildcards, array indices, and recursive descent (..).
/// </summary>
public static class JsonPathEvaluator
{
    private static readonly JsonDocumentOptions Options = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>
    /// Evaluates a JSONPath expression against a raw JSON string.
    /// </summary>
    /// <param name="json">Raw JSON text.</param>
    /// <param name="jsonPath">JSONPath expression (e.g. "$.store.book[*].author" or "$..title").</param>
    /// <returns>A list of matching nodes with their resolved JSONPaths and values.</returns>
    public static IReadOnlyList<JsonPathMatch> Evaluate(string? json, string? jsonPath)
    {
        if (string.IsNullOrWhiteSpace(json) || string.IsNullOrWhiteSpace(jsonPath))
            return Array.Empty<JsonPathMatch>();

        try
        {
            using var doc = JsonDocument.Parse(json, Options);
            return Evaluate(doc.RootElement, jsonPath);
        }
        catch
        {
            return Array.Empty<JsonPathMatch>();
        }
    }

    /// <summary>
    /// Evaluates a JSONPath expression against an existing JsonElement root.
    /// </summary>
    public static IReadOnlyList<JsonPathMatch> Evaluate(JsonElement root, string? jsonPath)
    {
        if (string.IsNullOrWhiteSpace(jsonPath))
            return Array.Empty<JsonPathMatch>();

        string normalizedPath = jsonPath.Trim();

        // Check if root-only query
        if (normalizedPath == "$" || normalizedPath == "$." || normalizedPath == "")
        {
            return new[]
            {
                new JsonPathMatch
                {
                    Path = "$",
                    Value = FormatElementValue(root),
                    ValueKind = root.ValueKind,
                    Element = root.Clone()
                }
            };
        }

        // Check for recursive descent: e.g. "$..property" or "..property"
        if (normalizedPath.StartsWith("$..") || normalizedPath.StartsWith(".."))
        {
            string searchKey = normalizedPath.StartsWith("$..") 
                ? normalizedPath.Substring(3) 
                : normalizedPath.Substring(2);

            var recursiveMatches = new List<JsonPathMatch>();
            SearchRecursive(root, "$", searchKey, recursiveMatches);
            return recursiveMatches;
        }

        // Parse path segments
        var segments = TokenizePath(normalizedPath);
        var currentItems = new List<(string Path, JsonElement Element)> { ("$", root) };

        foreach (var segment in segments)
        {
            var nextItems = new List<(string Path, JsonElement Element)>();

            foreach (var current in currentItems)
            {
                ResolveSegment(current.Path, current.Element, segment, nextItems);
            }

            currentItems = nextItems;
            if (currentItems.Count == 0) break;
        }

        var results = new List<JsonPathMatch>(currentItems.Count);
        foreach (var item in currentItems)
        {
            results.Add(new JsonPathMatch
            {
                Path = item.Path,
                Value = FormatElementValue(item.Element),
                ValueKind = item.Element.ValueKind,
                Element = item.Element.Clone()
            });
        }

        return results;
    }

    private static List<string> TokenizePath(string path)
    {
        var tokens = new List<string>();
        string clean = path;
        if (clean.StartsWith("$.")) clean = clean.Substring(2);
        else if (clean.StartsWith("$")) clean = clean.Substring(1);

        int i = 0;
        while (i < clean.Length)
        {
            if (clean[i] == '.')
            {
                i++;
                continue;
            }

            if (clean[i] == '[')
            {
                int closeBracket = clean.IndexOf(']', i);
                if (closeBracket == -1) break;
                string inside = clean.Substring(i + 1, closeBracket - i - 1).Trim('\'', '"', ' ');
                tokens.Add($"[{inside}]");
                i = closeBracket + 1;
            }
            else
            {
                int nextDot = clean.IndexOf('.', i);
                int nextBracket = clean.IndexOf('[', i);
                int end;

                if (nextDot == -1 && nextBracket == -1) end = clean.Length;
                else if (nextDot == -1) end = nextBracket;
                else if (nextBracket == -1) end = nextDot;
                else end = Math.Min(nextDot, nextBracket);

                string token = clean.Substring(i, end - i).Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    tokens.Add(token);
                }
                i = end;
            }
        }

        return tokens;
    }

    private static void ResolveSegment(string currentPath, JsonElement element, string segment, List<(string Path, JsonElement Element)> output)
    {
        // Wildcard array or object property: [*] or *
        if (segment == "[*]" || segment == "*")
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                int idx = 0;
                foreach (var child in element.EnumerateArray())
                {
                    output.Add(($"{currentPath}[{idx}]", child));
                    idx++;
                }
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in element.EnumerateObject())
                {
                    output.Add(($"{currentPath}.{prop.Name}", prop.Value));
                }
            }
            return;
        }

        // Index accessor: [0] or [-1]
        if (segment.StartsWith("[") && segment.EndsWith("]"))
        {
            string inner = segment.Substring(1, segment.Length - 2);
            if (int.TryParse(inner, out int index))
            {
                if (element.ValueKind == JsonValueKind.Array)
                {
                    int length = element.GetArrayLength();
                    int actualIndex = index >= 0 ? index : length + index;
                    if (actualIndex >= 0 && actualIndex < length)
                    {
                        output.Add(($"{currentPath}[{actualIndex}]", element[actualIndex]));
                    }
                }
                return;
            }
            else
            {
                // Property via bracket: ['propertyName']
                segment = inner;
            }
        }

        // Direct property accessor
        if (element.ValueKind == JsonValueKind.Object)
        {
            if (element.TryGetProperty(segment, out var child))
            {
                string newPath = currentPath == "$" ? $"$.{segment}" : $"{currentPath}.{segment}";
                output.Add((newPath, child));
            }
        }
    }

    private static void SearchRecursive(JsonElement element, string currentPath, string targetKey, List<JsonPathMatch> matches)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                string propPath = currentPath == "$" ? $"$.{prop.Name}" : $"{currentPath}.{prop.Name}";
                if (string.Equals(prop.Name, targetKey, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(new JsonPathMatch
                    {
                        Path = propPath,
                        Value = FormatElementValue(prop.Value),
                        ValueKind = prop.Value.ValueKind,
                        Element = prop.Value.Clone()
                    });
                }

                // Recurse into children
                SearchRecursive(prop.Value, propPath, targetKey, matches);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            int idx = 0;
            foreach (var item in element.EnumerateArray())
            {
                string itemPath = $"{currentPath}[{idx}]";
                SearchRecursive(item, itemPath, targetKey, matches);
                idx++;
            }
        }
    }

    private static string FormatElementValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "null",
            JsonValueKind.Array => $"[{element.GetArrayLength()} items]",
            JsonValueKind.Object => "{object}",
            _ => element.GetRawText()
        };
    }
}
