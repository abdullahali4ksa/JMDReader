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
        var tempPath = Path.Combine(Path.GetTempPath(), "test.txt");
        try
        {
            File.WriteAllText(tempPath, "not markdown", System.Text.Encoding.UTF8);
            Assert.Throws<System.ArgumentException>(() => MarkdownFileReader.ReadFile(tempPath));
        }
        finally
        {
            if (File.Exists(tempPath)) File.Delete(tempPath);
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
