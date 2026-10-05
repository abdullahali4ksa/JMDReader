// MIT License - see LICENSE file in the repository root.

using CommunityToolkit.Mvvm.ComponentModel;
using MDReader.Core.Documents;
using MDReader.Core.Markdown;

namespace MDReader.App.Features.Reader.ViewModels;

/// <summary>
/// ViewModel for the document reader — manages document content display,
/// error messaging, and empty-state logic. Acts as the data context for
/// MainPage.xaml, exposing properties that drive the empty, content, and
/// error visual states.</summary>
public sealed partial class ReaderViewModel : ObservableObject
{
    private string? _currentDocumentPath;
    private string? _documentContent;
    private string? _errorMessage;
    private MarkdownDocumentElement? _elements;
    private bool _isLoading;

    /// <summary>Gets or sets the file path of the currently loaded document.</summary>
    public string? CurrentDocumentPath
    {
        get => _currentDocumentPath;
        set
        {
            if (SetProperty(ref _currentDocumentPath, value))
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasDocument));
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    /// <summary>Gets or sets the raw Markdown text of the loaded document.</summary>
    public string? DocumentContent
    {
        get => _documentContent;
        set => SetProperty(ref _documentContent, value);
    }

    /// <summary>
    /// Gets the parsed, platform-neutral element tree for the loaded document,
    /// or null when no document is loaded. The View renders this tree using
    /// <c>MarkdownRenderer</c>.
    /// </summary>
    public MarkdownDocumentElement? Elements
    {
        get => _elements;
        private set => SetProperty(ref _elements, value);
    }

    /// <summary>Gets or sets the error message to display, or null when no error.</summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasDocument));
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    /// <summary>True when no document is loaded and no error is shown.</summary>
    public bool IsEmpty => _errorMessage is null && !_isLoading && string.IsNullOrEmpty(_currentDocumentPath);

    /// <summary>True when a document is loaded and ready for rendering.</summary>
    public bool HasDocument => _errorMessage is null && !_isLoading && !string.IsNullOrEmpty(_currentDocumentPath);

    /// <summary>True when there is an active error to display.</summary>
    public bool HasError => _errorMessage is not null;

    /// <summary>True while a document is being loaded (between Open and render).</summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (SetProperty(ref _isLoading, value))
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasDocument));
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    /// <summary>
    /// Loads a document into the reader, clearing any previous state.
    /// </summary>
    /// <param name="info">Document metadata from the file reader.</param>
    /// <param name="content">The raw Markdown text content.</param>
    public void LoadDocument(DocumentInfo info, string content)
    {
        // Mark loading and release the previous document's tree up front so a
        // replacement never keeps the old visual content alive.
        IsLoading = true;
        Elements = null;

        try
        {
            CurrentDocumentPath = info.FilePath;
            DocumentContent = content;

            // Parse the Markdown into the platform-neutral element tree. Parse
            // failures (or size warnings) never throw: the document is still
            // shown, degrading to whatever could be parsed.
            var parsed = new MarkdownDocument { SourceText = content }.Parse();
            Elements = parsed.Document.Elements;

            ErrorMessage = null;
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Shows an error message, clearing any loaded document.
    /// </summary>
    /// <param name="message">A human-readable error message.</param>
    public void ShowError(string message)
    {
        CurrentDocumentPath = null;
        DocumentContent = null;
        Elements = null;
        ErrorMessage = message;
    }

    /// <summary>
    /// <summary>Find text within the current document.</summary>
    public string FindText { get; set; } = string.Empty;

    /// <summary>Clear all document and error state, returning to the empty welcome state.</summary>
    /// </summary>
    public void Clear()
    {
        CurrentDocumentPath = null;
        DocumentContent = null;
        Elements = null;
        ErrorMessage = null;
    }
}
