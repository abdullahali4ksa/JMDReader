// MIT License - see LICENSE file in the repository root.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MDReader.Core.Json;

/// <summary>
/// High-performance utilities for formatting, minifying, and converting JSON documents (v2.0 JMD-12).
/// Supports offline Beautify, Minify, and conversion of JSON arrays to GitHub-flavored Markdown tables.
/// </summary>
public static class JsonFormatUtilities
{
    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        MaxDepth = 128
    };

    /// <summary>
    /// Formats a raw or minified JSON string into pretty-printed, indented JSON.
    /// Preserves raw UTF-8 characters (e.g. Arabic, international scripts, emojis) without \uXXXX escaping.
    /// </summary>
    /// <param name="json">Raw JSON text.</param>
    /// <param name="indentSpaces">Indentation size in spaces (default: 2).</param>
    /// <returns>Indented JSON string, or original input if parsing fails.</returns>
    public static string Beautify(string? json, int indentSpaces = 2)
    {
        if (string.IsNullOrWhiteSpace(json))
            return string.Empty;

        try
        {
            using var document = JsonDocument.Parse(json, DocumentOptions);
            using var memoryStream = new MemoryStream();
            
            var writerOptions = new JsonWriterOptions
            {
                Indented = true,
                IndentSize = Math.Clamp(indentSpaces, 1, 8),
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            using (var writer = new Utf8JsonWriter(memoryStream, writerOptions))
            {
                document.WriteTo(writer);
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
        catch
        {
            // If syntax is invalid, return original text safely
            return json;
        }
    }

    /// <summary>
    /// Compresses a JSON string into a minimal single-line string with all superfluous whitespace removed.
    /// </summary>
    /// <param name="json">Raw or formatted JSON text.</param>
    /// <returns>Minified JSON string, or original input if parsing fails.</returns>
    public static string Minify(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return string.Empty;

        try
        {
            using var document = JsonDocument.Parse(json, DocumentOptions);
            using var memoryStream = new MemoryStream();

            var writerOptions = new JsonWriterOptions
            {
                Indented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            using (var writer = new Utf8JsonWriter(memoryStream, writerOptions))
            {
                document.WriteTo(writer);
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
        catch
        {
            return json;
        }
    }

    /// <summary>
    /// Converts a JSON array of objects into a GitHub-flavored Markdown (GFM) table.
    /// Example: [{"id": 1, "name": "Document"}, {"id": 2, "name": "Report"}]
    /// Returns:
    /// | id | name |
    /// |---|---|
    /// | 1 | Document |
    /// | 2 | Report |
    /// </summary>
    /// <param name="json">JSON array string.</param>
    /// <returns>Markdown table string, or error notice if JSON is not an array of objects.</returns>
    public static string ConvertJsonToMarkdownTable(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return string.Empty;

        try
        {
            using var document = JsonDocument.Parse(json, DocumentOptions);
            var root = document.RootElement;

            // If root is an object, wrap in single-item array logic
            List<JsonElement> rows = new();
            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in root.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        rows.Add(item);
                    }
                }
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                rows.Add(root);
            }
            else
            {
                return "<!-- Notice: Input JSON must be an array of objects to generate a Markdown table. -->";
            }

            if (rows.Count == 0)
            {
                return "<!-- Notice: No object rows found in JSON array. -->";
            }

            // Gather all distinct property headers in order of appearance
            var headers = new List<string>();
            var headerSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var row in rows)
            {
                foreach (var prop in row.EnumerateObject())
                {
                    if (headerSet.Add(prop.Name))
                    {
                        headers.Add(prop.Name);
                    }
                }
            }

            if (headers.Count == 0)
            {
                return "<!-- Notice: Objects contain no properties. -->";
            }

            var sb = new StringBuilder();

            // Header row
            sb.Append("| ");
            for (int i = 0; i < headers.Count; i++)
            {
                sb.Append(EscapeMarkdownCell(headers[i]));
                sb.Append(i == headers.Count - 1 ? " |" : " | ");
            }
            sb.AppendLine();

            // Separator row
            sb.Append("|");
            for (int i = 0; i < headers.Count; i++)
            {
                sb.Append("---|");
            }
            sb.AppendLine();

            // Data rows
            foreach (var row in rows)
            {
                sb.Append("| ");
                for (int i = 0; i < headers.Count; i++)
                {
                    var header = headers[i];
                    string cellValue = string.Empty;

                    if (row.TryGetProperty(header, out var propValue))
                    {
                        cellValue = FormatCellValue(propValue);
                    }

                    sb.Append(EscapeMarkdownCell(cellValue));
                    sb.Append(i == headers.Count - 1 ? " |" : " | ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            return $"<!-- Error converting JSON to Markdown Table: {ex.Message} -->";
        }
    }

    private static string FormatCellValue(JsonElement element)
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

    private static string EscapeMarkdownCell(string value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        // Escape pipes and replace newlines with spaces so table formatting doesn't break
        return value
            .Replace("|", "\\|")
            .Replace("\r\n", " ")
            .Replace("\n", " ")
            .Replace("\r", " ");
    }
}
