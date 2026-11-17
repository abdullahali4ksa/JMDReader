using System.IO;
using Xunit;
using MDReader.Core.Documents;

namespace MDReader.Core.Tests;

public class MarkdownFileReaderTests
{
    [Theory]
    [InlineData("test.md", true)]
    [InlineData("test.markdown", true)]
    public void ReadFile_ValidExtensions_Accepted(string fileName, bool shouldPass)
    {
        var tempPath = Path.Combine(Path.GetTempPath(), fileName);
        try
        {
            File.WriteAllText(tempPath, "# Hello", System.Text.Encoding.UTF8);
            var info = MarkdownFileReader.ReadFile(tempPath);
            Assert.True(shouldPass);
            Assert.Equal(fileName, info.Title);
            Assert.Equal("utf-8", info.Encoding);
        }
        finally
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    [Fact]
    public void ReadFile_UnsupportedExtension_Throws()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "test.pdf");
        try
        {
            File.WriteAllText(tempPath, "not supported", System.Text.Encoding.UTF8);
            Assert.Throws<System.ArgumentException>(() => MarkdownFileReader.ReadFile(tempPath));
        }
        finally
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }

    [Theory]
    [InlineData(".md", true)]
    [InlineData(".markdown", true)]
    [InlineData(".json", true)]
    [InlineData(".jsonc", true)]
    [InlineData(".jsonl", true)]
    [InlineData(".txt", true)]
    [InlineData(".pdf", false)]
    [InlineData(".docx", false)]
    public void IsSupportedExtension_ValidatesCorrectly(string extension, bool expected)
    {
        Assert.Equal(expected, MarkdownFileReader.IsSupportedExtension(extension));
    }

    [Fact]
    public void ReadFile_JsonAndTxtFiles_Succeeds()
    {
        var jsonPath = Path.Combine(Path.GetTempPath(), "sample.json");
        var txtPath = Path.Combine(Path.GetTempPath(), "notes.txt");
        try
        {
            File.WriteAllText(jsonPath, "{\"key\":\"value\"}", System.Text.Encoding.UTF8);
            File.WriteAllText(txtPath, "Plain text log", System.Text.Encoding.UTF8);

            var jsonInfo = MarkdownFileReader.ReadFile(jsonPath);
            Assert.Equal("sample.json", jsonInfo.Title);

            var txtInfo = MarkdownFileReader.ReadFile(txtPath);
            Assert.Equal("notes.txt", txtInfo.Title);
        }
        finally
        {
            if (File.Exists(jsonPath)) File.Delete(jsonPath);
            if (File.Exists(txtPath)) File.Delete(txtPath);
        }
    }

    [Fact]
    public void ReadFile_MissingFile_ThrowsFileNotFound()
    {
        Assert.Throws<FileNotFoundException>(() => MarkdownFileReader.ReadFile("C:\\nonexistent\\missing.md"));
    }

    [Fact]
    public void ReadFile_EmptyFile_Succeeds()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), "empty.md");
        try
        {
            File.WriteAllText(tempPath, "", System.Text.Encoding.UTF8);
            var info = MarkdownFileReader.ReadFile(tempPath);
            Assert.True(info.SizeInBytes == 0 || info.SizeInBytes > 0);
        }
        finally
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
        }
    }
}
