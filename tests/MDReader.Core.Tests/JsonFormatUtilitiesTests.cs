// MIT License - see LICENSE file in the repository root.

using System.Linq;
using MDReader.Core.Json;
using Xunit;

namespace MDReader.Core.Tests;

public class JsonFormatUtilitiesTests
{
    [Fact]
    public void Beautify_FormatsMinifiedJson_PreservingArabicUtf8()
    {
        string raw = "{\"title\":\"قارئ ماركداون\",\"version\":\"2.0\",\"active\":true}";
        string formatted = JsonFormatUtilities.Beautify(raw, 2);

        Assert.Contains("\n", formatted);
        Assert.Contains("  \"title\": \"قارئ ماركداون\"", formatted);
        Assert.DoesNotContain("\\u", formatted); // Must be real UTF-8 Arabic letters, not \u escaped
    }

    [Fact]
    public void Minify_CompressesIndentedJsonToSingleLine()
    {
        string indented = """
        {
          "name": "MDReader",
          "features": [
            "Markdown",
            "JSON"
          ]
        }
        """;

        string minified = JsonFormatUtilities.Minify(indented);

        Assert.DoesNotContain("\n", minified);
        Assert.DoesNotContain("\r", minified);
        Assert.Contains("{\"name\":\"MDReader\",\"features\":[\"Markdown\",\"JSON\"]}", minified);
    }

    [Fact]
    public void ConvertJsonToMarkdownTable_ValidArrayOfObjects_GeneratesGfmTable()
    {
        string json = """
        [
          { "id": 1, "name": "Item A", "price": 10.5 },
          { "id": 2, "name": "Item B | Note", "price": 25.0 }
        ]
        """;

        string table = JsonFormatUtilities.ConvertJsonToMarkdownTable(json);

        Assert.Contains("| id | name | price |", table);
        Assert.Contains("|---|---|---|", table);
        Assert.Contains("| 1 | Item A | 10.5 |", table);
        Assert.Contains("Item B \\| Note", table); // Pipe must be escaped
    }

    [Fact]
    public void ConvertJsonToMarkdownTable_SingleObject_GeneratesSingleRowTable()
    {
        string json = "{\"author\":\"birotech\",\"year\":2026}";
        string table = JsonFormatUtilities.ConvertJsonToMarkdownTable(json);

        Assert.Contains("| author | year |", table);
        Assert.Contains("| birotech | 2026 |", table);
    }

    [Fact]
    public void JsonPathEvaluator_DirectPropertiesAndBracketAccess()
    {
        string json = """
        {
          "publisher": {
            "name": "birotech",
            "details": {
              "city": "Riyadh"
            }
          }
        }
        """;

        var match1 = JsonPathEvaluator.Evaluate(json, "$.publisher.name");
        Assert.Single(match1);
        Assert.Equal("birotech", match1[0].Value);

        var match2 = JsonPathEvaluator.Evaluate(json, "$['publisher']['details']['city']");
        Assert.Single(match2);
        Assert.Equal("Riyadh", match2[0].Value);
    }

    [Fact]
    public void JsonPathEvaluator_ArrayIndicesAndWildcards()
    {
        string json = """
        {
          "users": [
            { "id": 101, "role": "Admin" },
            { "id": 102, "role": "Editor" },
            { "id": 103, "role": "Viewer" }
          ]
        }
        """;

        var first = JsonPathEvaluator.Evaluate(json, "$.users[0].role");
        Assert.Single(first);
        Assert.Equal("Admin", first[0].Value);

        var last = JsonPathEvaluator.Evaluate(json, "$.users[-1].id");
        Assert.Single(last);
        Assert.Equal("103", last[0].Value);

        var allRoles = JsonPathEvaluator.Evaluate(json, "$.users[*].role");
        Assert.Equal(3, allRoles.Count);
        Assert.Equal("Admin", allRoles[0].Value);
        Assert.Equal("Editor", allRoles[1].Value);
        Assert.Equal("Viewer", allRoles[2].Value);
    }

    [Fact]
    public void JsonPathEvaluator_RecursiveDescent_FindsNestedKeys()
    {
        string json = """
        {
          "store": {
            "book": {
              "id": "b1",
              "title": "Clean Code",
              "author": "Robert C. Martin"
            },
            "magazine": {
              "id": "m1",
              "title": "Tech Monthly"
            }
          }
        }
        """;

        var titles = JsonPathEvaluator.Evaluate(json, "$..title");
        Assert.Equal(2, titles.Count);
        Assert.Contains(titles, t => t.Value == "Clean Code");
        Assert.Contains(titles, t => t.Value == "Tech Monthly");
    }

    [Fact]
    public void JsonPathEvaluator_InvalidOrEmpty_ReturnsEmptySafely()
    {
        var res1 = JsonPathEvaluator.Evaluate("", "$.test");
        Assert.Empty(res1);

        var res2 = JsonPathEvaluator.Evaluate("{}", "");
        Assert.Empty(res2);

        var res3 = JsonPathEvaluator.Evaluate("{ invalid json }", "$.key");
        Assert.Empty(res3);
    }
}
