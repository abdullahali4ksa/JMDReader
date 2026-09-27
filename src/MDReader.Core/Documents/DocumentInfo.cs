// MIT License - see LICENSE file in the repository root.

namespace MDReader.Core.Documents;

/// <summary>
/// Metadata about a document — file path, size, last modified time, etc.
/// Does not include the document content itself.
/// </summary>
/// <remarks>
/// This type lives in Core (not in the App layer) so that document metadata
/// can be used by any consumer without depending on WinUI or Windows App SDK.
/// </remarks>
public sealed class DocumentInfo
{
    /// <summary>Gets or sets the file path of the document.</summary>
    public string FilePath { get; init; } = string.Empty;

    /// <summary>Gets or sets the display title of the document (usually the file name).</summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>Gets or sets the file size in bytes.</summary>
    public long SizeInBytes { get; init; }

    /// <summary>Gets or sets the last modified time of the file.</summary>
    public DateTimeOffset LastModified { get; init; }

    /// <summary>Gets or sets the detected text encoding (e.g., "utf-8").</summary>
    public string Encoding { get; init; } = "utf-8";
}
