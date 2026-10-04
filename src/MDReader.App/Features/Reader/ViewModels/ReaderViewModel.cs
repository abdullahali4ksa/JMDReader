// MIT License - see LICENSE file in the repository root.

using CommunityToolkit.Mvvm.ComponentModel;
using MDReader.Core.Documents;

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
    public bool IsEmpty => _errorMessage is null && string.IsNullOrEmpty(_currentDocumentPath);

    /// <summary>True when a document is loaded and ready for rendering.</summary>
    public bool HasDocument => _errorMessage is null && !string.IsNullOrEmpty(_currentDocumentPath);

    /// <summary>True when there is an active error to display.</summary>
    public bool HasError => _errorMessage is not null;

    /// <summary>
    /// Loads a document into the reader, clearing any previous state.
    /// </summary>
    /// <param name="info">Document metadata from the file reader.</param>
    /// <param name="content">The raw Markdown text content.</param>
    public void LoadDocument(DocumentInfo info, string content)
    {
        CurrentDocumentPath = info.FilePath;
        DocumentContent = content;
        ErrorMessage = null;
    }

    /// <summary>
    /// Shows an error message, clearing any loaded document.
    /// </summary>
    /// <param name="message">A human-readable error message.</param>
    public void ShowError(string message)
    {
        CurrentDocumentPath = null;
        DocumentContent = null;
        ErrorMessage = message;
    }

    /// <summary>
    /// Clears all document and error state, returning to the empty welcome state.
    /// </summary>
    public void Clear()
    {
        CurrentDocumentPath = null;
        DocumentContent = null;
        ErrorMessage = null;
    }
}
