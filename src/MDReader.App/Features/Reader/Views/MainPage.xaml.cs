using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using MDReader.App.Features.Reader.ViewModels;
using MDReader.Core.Documents;
using Windows.System;
using Windows.UI;

namespace MDReader.App.Features.Reader.Views;

/// <summary>
/// The main content page displaying open document tabs, formatting toolbar,
/// the markdown editor, Antigravity offline WebView2 renderer, and comments drawer.
/// </summary>
public sealed partial class MainPage : Page
{
    private ReaderViewModel? _viewModel;
    private readonly List<int> _findMatches = new();
    private int _currentFindIndex = -1;
    private bool _isWebViewInitialized;

    public MainPage()
    {
        InitializeComponent();
        Loaded += async (s, e) => await InitializeWebViewAsync();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is ReaderViewModel readerViewModel)
        {
            _viewModel = readerViewModel;
            DataContext = readerViewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            UpdateViewModeColumns();
            ApplyTheme();
            UpdateWebViewContent();
        }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        if (_viewModel is not null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private async Task InitializeWebViewAsync()
    {
        if (_isWebViewInitialized) return;

        try
        {
            await AntigravityViewer.EnsureCoreWebView2Async();
            var assetsFolder = Path.Combine(AppContext.BaseDirectory, "Assets", "Renderer");
            AntigravityViewer.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "appassets.local",
                assetsFolder,
                CoreWebView2HostResourceAccessKind.Allow);

            AntigravityViewer.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
            AntigravityViewer.NavigationCompleted += (s, e) =>
            {
                _isWebViewInitialized = true;
                UpdateWebViewContent();
                ApplyTheme();
                ApplyZoom();
            };

            AntigravityViewer.Source = new Uri("https://appassets.local/viewer.html");
        }
        catch
        {
            // Gracefully handle environments without WebView2 runtime
        }
    }

    private void UpdateWebViewContent()
    {
        if (!_isWebViewInitialized || AntigravityViewer.CoreWebView2 == null)
            return;

        try
        {
            var markdown = _viewModel?.DocumentContent ?? string.Empty;
            var json = JsonSerializer.Serialize(new
            {
                type = "setContent",
                markdown = markdown
            });
            AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
        }
        catch { }
    }

    private void ApplyTheme()
    {
        if (_viewModel == null) return;
        var theme = _viewModel.CurrentTheme;

        if (_isWebViewInitialized && AntigravityViewer.CoreWebView2 != null)
        {
            try
            {
                var json = JsonSerializer.Serialize(new { type = "setTheme", theme = theme });
                AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
            }
            catch { }
        }

        // Synchronize editor colors with the selected reading theme
        if (MarkdownEditorTextBox != null)
        {
            switch (theme)
            {
                case "Light":
                    MarkdownEditorTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    MarkdownEditorTextBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 31, 35, 40));
                    MarkdownEditorTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 208, 215, 222));
                    break;
                case "Sepia":
                    MarkdownEditorTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 251, 240, 217));
                    MarkdownEditorTextBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 56, 52, 40));
                    MarkdownEditorTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 220, 208, 184));
                    break;
                case "Dark":
                default:
                    MarkdownEditorTextBox.Background = new SolidColorBrush(Color.FromArgb(255, 13, 17, 23));
                    MarkdownEditorTextBox.Foreground = new SolidColorBrush(Color.FromArgb(255, 230, 237, 243));
                    MarkdownEditorTextBox.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 48, 54, 61));
                    break;
            }
        }
    }

    private void ApplyAutoScroll()
    {
        if (!_isWebViewInitialized || AntigravityViewer.CoreWebView2 == null || _viewModel == null)
            return;

        try
        {
            var json = JsonSerializer.Serialize(new
            {
                type = "autoScroll",
                enabled = _viewModel.IsAutoScrolling,
                intervalSeconds = _viewModel.ScrollSpeedSeconds
            });
            AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
        }
        catch { }
    }

    private void DeleteComment_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is DocumentComment comment)
        {
            _viewModel?.DeleteComment(comment);
        }
    }

    private void OnWebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        try
        {
            using var doc = JsonDocument.Parse(args.WebMessageAsJson);
            var root = doc.RootElement.Clone();
            DispatcherQueue.TryEnqueue(() =>
            {
                if (root.TryGetProperty("type", out var typeProp))
                {
                    var type = typeProp.GetString();
                    if (type == "linkClick")
                    {
                        var url = root.GetProperty("url").GetString();
                        if (!string.IsNullOrEmpty(url))
                        {
                            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                                url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                            {
                                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                            }
                            else if (url.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                                     url.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase))
                            {
                                var currentDir = Path.GetDirectoryName(_viewModel?.CurrentDocumentPath);
                                var target = Path.GetFullPath(Path.Combine(currentDir ?? ".", url));
                                if (File.Exists(target))
                                {
                                    var info = MarkdownFileReader.ReadFile(target);
                                    var content = File.ReadAllText(target, Encoding.UTF8);
                                    _viewModel?.LoadDocument(info, content);
                                }
                            }
                        }
                    }
                    else if (type == "autoScrollEnded")
                    {
                        if (_viewModel != null) _viewModel.IsAutoScrolling = false;
                    }
                    else if (type == "textSelected")
                    {
                        if (_viewModel != null && root.TryGetProperty("text", out var textProp))
                        {
                            _viewModel.SelectedQuoteText = textProp.GetString() ?? string.Empty;
                        }
                    }
                    else if (type == "activeHeadingChanged")
                    {
                        if (_viewModel != null && root.TryGetProperty("id", out var idProp))
                        {
                            var id = idProp.GetString();
                            var match = System.Linq.Enumerable.FirstOrDefault(_viewModel.OutlineItems, o => o.Id == id);
                            if (match != null)
                            {
                                _viewModel.ActiveOutlineItem = match;
                            }
                        }
                    }
                }
            });
        }
        catch { }
    }

    public void ShowFind()
    {
        FindBar.Visibility = Visibility.Visible;
        FindTextBox.Focus(FocusState.Programmatic);
        FindTextBox.SelectAll();
    }

    private void DocumentTabs_AddTabButtonClick(TabView sender, object args)
    {
        _viewModel?.CreateNewDocument();
    }

    private void DocumentTabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        if (args.Item is DocumentTabViewModel tab)
        {
            _viewModel?.CloseTab(tab);
        }
    }

    private void ScrollSpeedCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel == null || ScrollSpeedCombo.SelectedItem is not ComboBoxItem item)
            return;

        if (int.TryParse(item.Tag?.ToString(), out int speed))
        {
            _viewModel.ScrollSpeedSeconds = speed;
        }
    }

    private void FindTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        _findMatches.Clear();
        _currentFindIndex = -1;
        var query = FindTextBox.Text;
        var content = _viewModel?.DocumentContent ?? string.Empty;
        if (string.IsNullOrWhiteSpace(query))
        {
            FindStatusText.Text = string.Empty;
            FindPreviewText.Text = string.Empty;
            return;
        }

        var position = 0;
        while ((position = content.IndexOf(query, position, StringComparison.CurrentCultureIgnoreCase)) >= 0)
        {
            _findMatches.Add(position);
            position += query.Length;
        }

        if (_findMatches.Count == 0)
        {
            FindStatusText.Text = "No matches";
            FindPreviewText.Text = string.Empty;
            return;
        }

        _currentFindIndex = 0;
        UpdateFindResult();
    }

    private void FindTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            if (InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(VirtualKeyStates.Down))
                FindPrevious();
            else
                FindNext();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.Escape)
        {
            FindBar.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }
    }

    private void FindPrevious_Click(object sender, RoutedEventArgs e) => FindPrevious();
    private void FindNext_Click(object sender, RoutedEventArgs e) => FindNext();
    private void CloseFind_Click(object sender, RoutedEventArgs e) => FindBar.Visibility = Visibility.Collapsed;

    private void OutlineListView_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is DocumentOutlineItem item && !string.IsNullOrEmpty(item.Id))
        {
            if (_isWebViewInitialized && AntigravityViewer.CoreWebView2 != null)
            {
                var json = JsonSerializer.Serialize(new
                {
                    type = "scrollToHeading",
                    id = item.Id
                });
                AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
            }
        }
    }

    private async void ExportPdf_Click(object sender, RoutedEventArgs e)
    {
        await ExportToPdfAsync();
    }

    public async Task ExportToPdfAsync()
    {
        if (!_isWebViewInitialized || AntigravityViewer.CoreWebView2 == null)
            return;

        try
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("PDF Document", new List<string> { ".pdf" });
            picker.SuggestedFileName = (_viewModel?.SelectedTab?.Title ?? "Document").Replace(" *", "") + ".pdf";

            var hwnd = Process.GetCurrentProcess().MainWindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var settings = AntigravityViewer.CoreWebView2.Environment.CreatePrintSettings();
                settings.ShouldPrintBackgrounds = true;
                settings.ShouldPrintHeaderAndFooter = false;
                await AntigravityViewer.CoreWebView2.PrintToPdfAsync(file.Path, settings);
            }
        }
        catch { }
    }

    private async void ExportHtml_Click(object sender, RoutedEventArgs e)
    {
        await ExportToHtmlAsync();
    }

    public async Task ExportToHtmlAsync()
    {
        try
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("HTML Webpage", new List<string> { ".html", ".htm" });
            picker.SuggestedFileName = (_viewModel?.SelectedTab?.Title ?? "Document").Replace(" *", "") + ".html";

            var hwnd = Process.GetCurrentProcess().MainWindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var viewerPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Renderer", "viewer.html");
                var template = File.Exists(viewerPath) ? File.ReadAllText(viewerPath) : string.Empty;
                var md = _viewModel?.DocumentContent ?? string.Empty;
                var jsonEscapedMd = JsonSerializer.Serialize(md);
                var standalone = template.Replace("parseAntigravityMarkdown(data.markdown || \"\")", $"parseAntigravityMarkdown({jsonEscapedMd})");
                standalone = standalone.Replace("document.body.setAttribute('data-theme', t);", $"document.body.setAttribute('data-theme', '{(_viewModel?.CurrentTheme ?? "dark").ToLower()}');");
                await File.WriteAllTextAsync(file.Path, standalone, Encoding.UTF8);
            }
        }
        catch { }
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == VirtualKey.F11)
        {
            _viewModel?.ToggleZenMode();
            e.Handled = true;
        }
        else if (e.Key == VirtualKey.Escape && (_viewModel?.IsZenMode ?? false))
        {
            _viewModel.IsZenMode = false;
            e.Handled = true;
        }
    }

    private void FindPrevious()
    {
        if (_findMatches.Count == 0) return;
        _currentFindIndex = (_currentFindIndex - 1 + _findMatches.Count) % _findMatches.Count;
        UpdateFindResult();
    }

    private void FindNext()
    {
        if (_findMatches.Count == 0) return;
        _currentFindIndex = (_currentFindIndex + 1) % _findMatches.Count;
        UpdateFindResult();
    }

    private void UpdateFindResult()
    {
        var content = _viewModel?.DocumentContent ?? string.Empty;
        var start = _findMatches[_currentFindIndex];
        var queryLength = FindTextBox.Text.Length;
        var previewStart = Math.Max(0, start - 32);
        var previewLength = Math.Min(content.Length - previewStart, queryLength + 64);
        FindStatusText.Text = $"{_currentFindIndex + 1} of {_findMatches.Count}";
        FindPreviewText.Text = content.Substring(previewStart, previewLength).Replace('\r', ' ').Replace('\n', ' ');
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ReaderViewModel.ViewMode))
        {
            UpdateViewModeColumns();
        }
        else if (e.PropertyName == nameof(ReaderViewModel.CurrentTheme))
        {
            ApplyTheme();
        }
        else if (e.PropertyName == nameof(ReaderViewModel.IsAutoScrolling) ||
                 e.PropertyName == nameof(ReaderViewModel.ScrollSpeedSeconds))
        {
            ApplyAutoScroll();
        }
        else if (e.PropertyName == nameof(ReaderViewModel.DocumentContent) ||
                 e.PropertyName == nameof(ReaderViewModel.SelectedTab) ||
                 e.PropertyName == nameof(ReaderViewModel.HasDocument))
        {
            UpdateWebViewContent();
        }
        else if (e.PropertyName == nameof(ReaderViewModel.ZoomRatio))
        {
            ApplyZoom();
        }
    }

    private void UpdateViewModeColumns()
    {
        if (_viewModel == null || EditorCol == null || PreviewCol == null || DividerCol == null)
            return;

        switch (_viewModel.ViewMode)
        {
            case ReaderViewMode.Editor:
                EditorCol.Width = new GridLength(1, GridUnitType.Star);
                DividerCol.Width = new GridLength(0);
                PreviewCol.Width = new GridLength(0);
                break;
            case ReaderViewMode.Preview:
                EditorCol.Width = new GridLength(0);
                DividerCol.Width = new GridLength(0);
                PreviewCol.Width = new GridLength(1, GridUnitType.Star);
                break;
            case ReaderViewMode.Split:
            default:
                EditorCol.Width = new GridLength(1, GridUnitType.Star);
                DividerCol.Width = GridLength.Auto;
                PreviewCol.Width = new GridLength(1, GridUnitType.Star);
                break;
        }
    }

    private void ApplyZoom()
    {
        if (_viewModel is null || !_isWebViewInitialized || AntigravityViewer.CoreWebView2 == null)
            return;

        try
        {
            var json = JsonSerializer.Serialize(new
            {
                type = "setZoom",
                zoom = _viewModel.ZoomRatio
            });
            AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
        }
        catch { }
    }
}
