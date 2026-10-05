// MIT License - see LICENSE file in the repository root.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MDReader.App.Features.FileOpen.ViewModels;
using MDReader.App.Features.Reader.ViewModels;
using MDReader.Core.Documents;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MDReader.App.Features.Shell.ViewModels;

/// <summary>
/// ViewModel for the application shell (window management, navigation, app menu).
/// Coordinates between the FileOpen picker, the Core document reader, and the
/// Reader display ViewModel. Uses CommunityToolkit.Mvvm ObservableObject for
/// MVVM binding and RelayCommand for commanding.</summary>
public sealed partial class ShellViewModel : ObservableObject
{
    private string _windowTitle = "MDReader";
    private readonly ReaderViewModel _readerViewModel;
    private readonly FileOpenViewModel _fileOpenViewModel;

    /// <summary>Gets or sets the window title displayed in the title bar.</summary>
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value);
    }

    /// <summary>
    /// Gets or sets the HWND for the application window.
    /// Required for file picker initialization in unpackaged apps.
    /// </summary>
    public System.IntPtr WindowHandle { get; set; }

    /// <summary>
    /// Default constructor — creates internal child ViewModels for design-time use.
    /// At runtime, prefer the constructor that accepts a ReaderViewModel so the
    /// Shell and Reader share the same instance.
    /// </summary>
    public ShellViewModel()
    {
        _readerViewModel = new ReaderViewModel();
        _fileOpenViewModel = new FileOpenViewModel();
    }

    /// <summary>
    /// Creates a ShellViewModel that coordinates with the provided ReaderViewModel.
    /// </summary>
    /// <param name="readerViewModel">The ReaderViewModel that displays loaded documents.</param>
    public ShellViewModel(ReaderViewModel readerViewModel)
    {
        _readerViewModel = readerViewModel;
        _fileOpenViewModel = new FileOpenViewModel();
    }

    /// <summary>
    /// Opens the file picker and loads the selected Markdown document.
    /// Routed through the same OpenDocument pipeline as CLI paths.
    /// </summary>
    [RelayCommand]
    private async Task OpenDocumentAsync()
    {
        try
        {
            var filePath = await _fileOpenViewModel.PickMarkdownFileAsync(WindowHandle);
            if (!string.IsNullOrEmpty(filePath))
            {
                OpenDocument(filePath);
            }
        }
        catch (System.Exception ex)
        {
            _readerViewModel.ShowError($"Could not open file: {ex.Message}");
        }
    }

    [RelayCommand]
    private Task FindDocumentAsync()
    {
        // Named action: open find for current document.
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task RefreshDocumentAsync()
    {
        // Named action: refresh/reload current document view.
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ThemeSettingsAsync()
    {
        // Named action: open theme settings.
        return Task.CompletedTask;
    }

    /// <summary>
    /// Opens a Markdown document from the given file path.
    /// This is the single entry point for both picker and CLI file opening.
    /// Opening a second document replaces the first.
    /// </summary>
    /// <param name="filePath">The path to the Markdown file.</param>
    public void OpenDocument(string filePath)
    {
        try
        {
            // Validate and read metadata via the Core reader
            var info = MarkdownFileReader.ReadFile(filePath);

            // Read the file content (UTF-8, read-only, no modification)
            var content = File.ReadAllText(filePath, Encoding.UTF8);

            _readerViewModel.LoadDocument(info, content);
        }
        catch (System.ArgumentException ex)
        {
            // Reader messages are already user-facing (e.g. unsupported extension).
            _readerViewModel.ShowError(ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            _readerViewModel.ShowError(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _readerViewModel.ShowError($"Access denied: {ex.Message}");
        }
        catch (System.Exception ex)
        {
            _readerViewModel.ShowError($"Could not open file: {ex.Message}");
        }
    }
}

