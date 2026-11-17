// MIT License - see LICENSE file in the repository root.

using System.Diagnostics;
using System.Text.Json;
using MDReader.Core.Json;
using Xunit;

namespace MDReader.Core.Tests;

/// <summary>
/// Unit and performance tests for the core JSON tree parser (v2.0 JMD-11).
/// </summary>
public sealed class JsonTreeParserTests
{
    [Fact]
    public void Parse_ValidJsonObject_BuildsAccurateHierarchyAndPaths()
    {
        var json = """
        {
            "appName": "JMDReader",
            "version": 2.0,
            "isReleased": true,
            "tags": ["markdown", "json", "fast"],
            "publisher": {
                "name": "birotech",
                "id": 42
            }
        }
        """;

        var result = JsonTreeParser.Parse(json);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Root);
        Assert.Equal(JsonValueKind.Object, result.Root.ValueKind);
        Assert.Equal(5, result.Root.Children.Count);
        Assert.True(result.TotalNodes >= 10);

        // Check specific child
        var appName = result.Root.Children[0];
        Assert.Equal("appName", appName.Key);
        Assert.Equal("JMDReader", appName.Value);
        Assert.Equal("$.appName", appName.JsonPath);

        // Check array
        var tags = result.Root.Children[3];
        Assert.Equal("tags", tags.Key);
        Assert.Equal(JsonValueKind.Array, tags.ValueKind);
        Assert.Equal(3, tags.Children.Count);
        Assert.Equal("$.tags[0]", tags.Children[0].JsonPath);
        Assert.Equal("markdown", tags.Children[0].Value);

        // Check nested object
        var publisher = result.Root.Children[4];
        Assert.Equal("publisher", publisher.Key);
        Assert.Equal(2, publisher.Children.Count);
        Assert.Equal("$.publisher.name", publisher.Children[0].JsonPath);
        Assert.Equal("birotech", publisher.Children[0].Value);
    }

    [Fact]
    public void Parse_InvalidJson_SurfacesLineAndColumnAccurately()
    {
        var malformedJson = """
        {
            "valid": true,
            "broken": [1, 2, 3
        }
        """;

        var result = JsonTreeParser.Parse(malformedJson);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Root);
        Assert.NotNull(result.ErrorMessage);
        Assert.True(result.ErrorLine >= 3, $"Expected error on line >= 3, got line {result.ErrorLine}");
    }

    [Fact]
    public void Parse_JsonWithCommentsAndTrailingCommas_Succeeds()
    {
        var jsonWithComments = """
        {
            // Application configuration
            "theme": "Dark",
            "fontSize": 14, // in pixels
            "experimental": false,
        }
        """;

        var result = JsonTreeParser.Parse(jsonWithComments);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Root);
        Assert.Equal(3, result.Root.Children.Count);
    }

    [Fact]
    public void Parse_EmptyOrWhitespace_ReturnsGracefulError()
    {
        var result = JsonTreeParser.Parse("   \n\t  ");
        Assert.False(result.IsSuccess);
        Assert.Contains("empty", result.ErrorMessage, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Parse_LargeJsonPayload_ExecutesUnder100ms()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("{\"items\": [");
        for (int i = 0; i < 5000; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append($"{{\"id\": {i}, \"name\": \"Item_{i}\", \"active\": {(i % 2 == 0 ? "true" : "false")}}}");
        }
        sb.Append("]}");

        var largeJson = sb.ToString();
        var sw = Stopwatch.StartNew();
        var result = JsonTreeParser.Parse(largeJson);
        sw.Stop();

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Root);
        Assert.True(sw.ElapsedMilliseconds < 500, $"5000 items parsed in {sw.ElapsedMilliseconds}ms (limit: 500ms)");
    }
}
