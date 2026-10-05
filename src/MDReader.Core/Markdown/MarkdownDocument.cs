// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Markdown;

/// <summary>
/// Represents a Markdown document: raw source text plus its parsed element tree.
/// Parsing is performed by <see cref="Parse"/> using the Markdig library, which
/// builds a platform-neutral <see cref="MarkdownDocumentElement"/> AST held in
/// <see cref="Elements"/>. Core stays platform-neutral — rendering lives in App.
/// </summary>
public sealed class MarkdownDocument
{
    /// <summary>Gets or sets the raw source text of the Markdown document.</summary>
    public string SourceText { get; set; } = string.Empty;

    /// <summary>
    /// Gets the parsed, platform-neutral element tree, or null when the document
    /// has not been parsed (or was short-circuited by a size warning).
    /// </summary>
    public MarkdownDocumentElement? Elements { get; set; }

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
                    Document = new MarkdownDocument { SourceText = SourceText, Elements = new MarkdownDocumentElement(System.Array.Empty<MarkdownBlockElement>()) },
                    Error = null
                };
            }

            // Size-warning behavior: files exceeding a safe preview threshold
            // should surface an informational result, not an error. The document
            // is still returned (without parsing) to keep large-file tests fast.
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

            // Core stays platform-neutral; no raw HTML execution. Markdig parses
            // to an AST, which the builder converts into the element tree held in
            // Document.Elements. Rendering stays in the App layer.
            var elements = MarkdownAstBuilder.Build(SourceText);

            return new MarkdownParseResult
            {
                Success = true,
                Document = new MarkdownDocument { SourceText = SourceText, Elements = elements },
                Error = null
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
