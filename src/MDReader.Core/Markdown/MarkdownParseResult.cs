// MIT License
namespace MDReader.Core.Markdown;

/// <summary>Typed parsing error for Markdown document loading/reading.</summary>
public sealed class MarkdownParseError
{
    public string Code { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public bool IsRecoverable { get; init; } = true;
}

/// <summary>Result of parsing a Markdown file. Contains the parsed document model.</summary>
public sealed class MarkdownParseResult
{
    public MarkdownDocument Document { get; init; } = new();
    public bool Success { get; init; }
    public MarkdownParseError? Error { get; init; }
}
