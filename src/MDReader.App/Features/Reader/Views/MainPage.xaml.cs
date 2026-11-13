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
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Web.WebView2.Core;
using MDReader.App.Features.Reader.ViewModels;
using MDReader.Core.Documents;
using Windows.System;

namespace MDReader.App.Features.Reader.Views;

/// <summary>
/// The main content page displaying open document tabs, formatting toolbar,
/// the markdown editor, and the Antigravity offline WebView2 renderer.
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

    private void OnWebMessageReceived(CoreWebView2 sender, CoreWebView2WebMessageReceivedEventArgs args)
    {
        try
        {
            using var doc = JsonDocument.Parse(args.WebMessageAsJson);
            var root = doc.RootElement;
            if (root.TryGetProperty("type", out var typeProp) && typeProp.GetString() == "linkClick")
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
        if (e.PropertyName == nameof(ReaderViewModel.DocumentContent) ||
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
