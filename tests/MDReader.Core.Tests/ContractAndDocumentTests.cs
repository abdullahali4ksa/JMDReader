// MIT License - see LICENSE file in the repository root.

using Xunit;
using MDReader.Core.Documents;
using MDReader.Core.Contracts;

namespace MDReader.Core.Tests;

public class DocumentInfoTests
{
    [Fact]
    public void DocumentInfo_HasSafeDefaults()
    {
        var info = new DocumentInfo();

        Assert.Equal(string.Empty, info.FilePath);
        Assert.Equal(string.Empty, info.Title);
        Assert.Equal(0L, info.SizeInBytes);
        Assert.Equal("utf-8", info.Encoding);
    }

    [Fact]
    public void DocumentInfo_CanBeInitialized()
    {
        var modified = new DateTimeOffset(2026, 10, 4, 12, 0, 0, TimeSpan.Zero);

        var info = new DocumentInfo
        {
            FilePath = @"C:\docs\readme.md",
            Title = "readme.md",
            SizeInBytes = 1024,
            LastModified = modified,
            Encoding = "utf-8"
        };

        Assert.Equal(@"C:\docs\readme.md", info.FilePath);
        Assert.Equal("readme.md", info.Title);
        Assert.Equal(1024L, info.SizeInBytes);
        Assert.Equal(modified, info.LastModified);
    }
}

public class CommandContractTests
{
    [Fact]
    public void CommandRequest_HasSafeDefaults()
    {
        var request = new CommandRequest();

        Assert.Equal(string.Empty, request.Command);
        Assert.NotNull(request.Parameters);
        Assert.Empty(request.Parameters);
        Assert.Null(request.RequestId);
    }

    [Fact]
    public void CommandRequest_CanCarryCommandAndParameters()
    {
        var request = new CommandRequest
        {
            Command = "OpenDocument",
            Parameters = { ["path"] = @"C:\docs\readme.md" },
            RequestId = "req-1"
        };

        Assert.Equal("OpenDocument", request.Command);
        Assert.Equal(@"C:\docs\readme.md", request.Parameters["path"]);
        Assert.Equal("req-1", request.RequestId);
    }

    [Fact]
    public void CommandResult_RepresentsSuccess()
    {
        var result = new CommandResult
        {
            Success = true,
            Data = "{\"ok\":true}",
            RequestId = "req-1"
        };

        Assert.True(result.Success);
        Assert.Equal("{\"ok\":true}", result.Data);
        Assert.Null(result.Error);
        Assert.Equal("req-1", result.RequestId);
    }

    [Fact]
    public void CommandResult_RepresentsFailure()
    {
        var result = new CommandResult
        {
            Success = false,
            Error = "File not found"
        };

        Assert.False(result.Success);
        Assert.Null(result.Data);
        Assert.Equal("File not found", result.Error);
    }
}