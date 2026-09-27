// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Markdown;

/// <summary>
/// Placeholder type to ensure the Core project compiles and has a single,
/// well-defined entry point. Real models and services will be added in
/// subsequent phases (Phase 2 — Markdown rendering).
/// </summary>
public sealed class MarkdownDocument
{
    /// <summary>
    /// Gets or sets the raw source text of the Markdown document.
    /// </summary>
    public string SourceText { get; set; } = string.Empty;
}
