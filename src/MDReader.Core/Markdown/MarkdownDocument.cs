// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Markdown;

/// <summary>
/// Placeholder type to ensure the Core project compiles and has a single,
/// well-defined entry point. Real models and services will be added in
/// subsequent phases (Phase 2 — Markdown rendering).
/// </summary>
public sealed class MarkdownDocument
{
    /// <summary>Gets or sets the raw source text of the Markdown document.</summary>
    public string SourceText { get; set; } = string.Empty;

    /// <summary>Parses the source text using Markdig and returns a result model.</summary>
    /// <returns>A parse result containing either the document or a typed error.</returns>
    public MarkdownParseResult Parse()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(SourceText))
            {
                return new MarkdownParseResult
                {
                    Success = true,
                    Document = new MarkdownDocument { SourceText = SourceText },
                    Error = null
                };
            }

            // Size-warning behavior: files exceeding a safe preview threshold
            // should surface an informational result, not an error.
            const int sizeWarningBytes = 5 * 1024 * 1024; // 5 MB
            long estimatedBytes = System.Text.Encoding.UTF8.GetByteCount(SourceText);
            if (estimatedBytes > sizeWarningBytes)
            {
                return new MarkdownParseResult
                {
                    Success = true,
                    Document = new MarkdownDocument { SourceText = SourceText },
                    Error = new MarkdownParseError
                    {
                        Code = "SIZE_WARNING",
                        Message = $"Document exceeds {sizeWarningBytes} bytes; performance may degrade.",
                        IsRecoverable = true
                    }
                };
            }

            // Core stays platform-neutral; no raw HTML execution.
            // Markdig parses to AST but rendering stays in App layer.
            return new MarkdownParseResult
            {
                Success = true,
                Document = new MarkdownDocument { SourceText = SourceText }
            };
        }
        catch (System.Exception ex)
        {
            return new MarkdownParseResult
            {
                Success = false,
                Error = new MarkdownParseError
                {
                    Code = "PARSE_ERROR",
                    Message = ex.Message,
                    IsRecoverable = false
                }
            };
        }
    }
}
