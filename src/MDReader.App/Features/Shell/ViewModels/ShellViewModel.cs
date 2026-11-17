// MIT License - see LICENSE file in the repository root.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MDReader.App.Features.FileOpen.ViewModels;
using MDReader.App.Features.Reader.ViewModels;
using MDReader.Core.Documents;
using MDReader.Core.Localization;

namespace MDReader.App.Features.Shell.ViewModels;

/// <summary>
/// ViewModel for the application shell (window management, navigation, app menu, workspace folder tree).
/// </summary>
public sealed partial class ShellViewModel : ObservableObject
{
    private string _windowTitle = "JMDReader";
    private bool _isWorkspacePaneOpen;
    private readonly ReaderViewModel _readerViewModel;
    private readonly FileOpenViewModel _fileOpenViewModel;

    /// <summary>Access to the global multi-language localization service.</summary>
    public LocalizationService Strings => LocalizationService.Instance;

    /// <summary>Gets the ReaderViewModel instance.</summary>
    public ReaderViewModel Reader => _readerViewModel;

    /// <summary>Gets or sets the window title displayed in the title bar.</summary>
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value);
    }

    /// <summary>Gets or sets whether the workspace folder sidebar pane is open.</summary>
    public bool IsWorkspacePaneOpen
    {
        get => _isWorkspacePaneOpen;
        set => SetProperty(ref _isWorkspacePaneOpen, value);
    }

    /// <summary>Tree nodes for the currently loaded workspace folder.</summary>
    public ObservableCollection<FolderNodeViewModel> WorkspaceNodes { get; } = new();

    /// <summary>
    /// Gets or sets the HWND for the application window.
    /// </summary>
    public System.IntPtr WindowHandle { get; set; }

    public ShellViewModel() : this(new ReaderViewModel())
    {
    }

    public ShellViewModel(ReaderViewModel readerViewModel)
    {
        _readerViewModel = readerViewModel;
        _fileOpenViewModel = new FileOpenViewModel();

        _readerViewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ReaderViewModel.SelectedTab) ||
                e.PropertyName == nameof(ReaderViewModel.CurrentDocumentPath))
            {
                UpdateTitle();
            }
        };

        LocalizationService.Instance.PropertyChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(Strings));
            UpdateTitle();
        };
    }

    [RelayCommand]
    public void SetLanguage(string langCode)
    {
        LocalizationService.Instance.SetLanguage(langCode);
        try
        {
            MDReader.App.Features.Settings.Services.UserPreferencesService.Instance.PreferredLanguage = langCode;
            MDReader.App.Features.Settings.Services.UserPreferencesService.Instance.Save();
        }
        catch { }
        OnPropertyChanged(nameof(Strings));
    }

    private void UpdateTitle()
    {
        if (_readerViewModel.SelectedTab != null)
        {
            WindowTitle = $"{_readerViewModel.SelectedTab.Header} — JMDReader";
        }
        else
        {
            WindowTitle = "JMDReader";
        }
    }

    [RelayCommand]
    public void ToggleWorkspacePane()
    {
        IsWorkspacePaneOpen = !IsWorkspacePaneOpen;
    }

    [RelayCommand]
    public async Task OpenFolderAsync()
    {
        try
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            folderPicker.FileTypeFilter.Add("*");

            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, WindowHandle);

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                LoadWorkspaceDirectory(folder.Path);
            }
        }
        catch (System.Exception ex)
        {
            _readerViewModel.ShowError($"Could not open workspace: {ex.Message}");
        }
    }

    public void LoadWorkspaceDirectory(string path)
    {
        try
        {
            WorkspaceNodes.Clear();
            var root = FolderNodeViewModel.FromDirectory(path);
            WorkspaceNodes.Add(root);
            IsWorkspacePaneOpen = true;
        }
        catch (System.Exception ex)
        {
            _readerViewModel.ShowError($"Could not load workspace folder: {ex.Message}");
        }
    }

    public void OnNodeSelected(FolderNodeViewModel? node)
    {
        if (node != null && !node.IsDirectory && !string.IsNullOrEmpty(node.FullPath))
        {
            OpenDocument(node.FullPath);
        }
    }

    [RelayCommand]
    public void NewDocument()
    {
        _readerViewModel.CreateNewDocument();
        UpdateTitle();
    }

    [RelayCommand]
    public async Task OpenDocumentAsync()
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
    public async Task SaveDocumentAsync()
    {
        if (_readerViewModel.SelectedTab == null)
            return;

        if (string.IsNullOrEmpty(_readerViewModel.SelectedTab.FilePath))
        {
            await SaveDocumentAsAsync();
        }
        else
        {
            _readerViewModel.SaveCurrentDocument();
            UpdateTitle();
        }
    }

    [RelayCommand]
    public async Task SaveDocumentAsAsync()
    {
        if (_readerViewModel.SelectedTab == null)
            return;

        try
        {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Markdown Document", new List<string>() { ".md", ".markdown" });
            savePicker.FileTypeChoices.Add("JSON Document", new List<string>() { ".json", ".jsonc", ".jsonl" });
            savePicker.FileTypeChoices.Add("Plain Text Document", new List<string>() { ".txt" });
            savePicker.SuggestedFileName = _readerViewModel.SelectedTab.Title.Replace(" *", "");

            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, WindowHandle);

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                _readerViewModel.SaveCurrentDocument(file.Path);
                UpdateTitle();
            }
        }
        catch (System.Exception ex)
        {
            _readerViewModel.ShowError($"Could not save file: {ex.Message}");
        }
    }

    [RelayCommand]
    public void CloseCurrentTab()
    {
        if (_readerViewModel.SelectedTab != null)
        {
            _readerViewModel.CloseTab(_readerViewModel.SelectedTab);
            UpdateTitle();
        }
    }

    [RelayCommand]
    private Task RefreshDocumentAsync()
    {
        var currentPath = _readerViewModel.CurrentDocumentPath;
        if (!string.IsNullOrWhiteSpace(currentPath))
            OpenDocument(currentPath);

        return Task.CompletedTask;
    }

    [RelayCommand]
    public void SetTheme(string theme)
    {
        _readerViewModel.SetTheme(theme);
    }

    [RelayCommand]
    private Task ThemeSettingsAsync()
    {
        _readerViewModel.CycleTheme();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Opens a Markdown document from the given file path into a tab.
    /// </summary>
    public void OpenDocument(string filePath)
    {
        try
        {
            var info = MarkdownFileReader.ReadFile(filePath);
            var content = File.ReadAllText(filePath, Encoding.UTF8);
            _readerViewModel.LoadDocument(info, content);
            UpdateTitle();

            if (App.MainWindow is MDReader.App.Features.Shell.Views.MainWindow mw)
            {
                mw.PopulateRecentFilesMenu();
            }
        }
        catch (System.Exception ex)
        {
            _readerViewModel.ShowError($"Could not open file: {ex.Message}");
        }
    }
}
