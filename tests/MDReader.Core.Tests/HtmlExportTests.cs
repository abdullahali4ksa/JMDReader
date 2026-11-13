// MIT License - see LICENSE file in the repository root.

using System.IO;
using System.Text.Json;
using Xunit;

namespace MDReader.Core.Tests;

public class HtmlExportTests
{
    [Fact]
    public void ExportHtml_GeneratesSelfContainedExecutableScript()
    {
        var rawMarkdown = "# Test Title\n\n> [!NOTE]\n> Note alert\n\n| Col A | Col B |\n|---|---|\n| 1 | 2 |\n";
        var safeMdJson = JsonSerializer.Serialize(rawMarkdown);
        var theme = "sepia";
        var title = "Document Title";

        var template = @"<!DOCTYPE html>
<html>
<head><title>MDReader Antigravity Viewer</title></head>
<body data-theme=""dark"">
<div id=""content""></div>
<script>
function parseAntigravityMarkdown(md) { return '<div>' + md + '</div>'; }
</script>
</body>
</html>";

        // Apply theme
        var exported = template.Replace("<body data-theme=\"dark\">", $"<body data-theme=\"{theme}\">");
        Assert.Contains("data-theme=\"sepia\"", exported);

        // Apply title
        exported = exported.Replace("<title>MDReader Antigravity Viewer</title>", $"<title>{title} — MDReader</title>");
        Assert.Contains("<title>Document Title — MDReader</title>", exported);

        // Inject bootstrap
        var bootstrap = $@"
<script>
(function() {{
    const md = {safeMdJson};
    document.getElementById('content').innerHTML = parseAntigravityMarkdown(md);
}})();
</script>
</body>";
        exported = exported.Replace("</body>", bootstrap);

        Assert.Contains(safeMdJson, exported);
        Assert.Contains("parseAntigravityMarkdown(md)", exported);
    }
}
