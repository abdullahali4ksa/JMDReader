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
using MDReader.Core.Markdown;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
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
    private readonly List<(int Index, int Length)> _findMatches = new();
    private int _currentFindIndex = -1;
    private bool _isWebViewInitialized;
    private readonly DispatcherTimer _renderDebounceTimer = new() { Interval = TimeSpan.FromMilliseconds(180) };

    public MainPage()
    {
        InitializeComponent();
        _renderDebounceTimer.Tick += (s, e) =>
        {
            _renderDebounceTimer.Stop();
            ExecuteUpdateWebViewContent();
        };
        Loaded += async (s, e) =>
        {
            try
            {
                await InitializeWebViewAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MainPage] Loaded InitializeWebViewAsync failed: {ex}");
            }
        };
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
            UpdateWebViewContent(immediate: true);
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
            var localFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userDataFolder = Path.Combine(localFolder, "JMDReader", "WebView2Data");
            Directory.CreateDirectory(userDataFolder);

            var environment = await CoreWebView2Environment.CreateWithOptionsAsync(null, userDataFolder, null);
            await AntigravityViewer.EnsureCoreWebView2Async(environment);

            var assetsFolder = Path.Combine(AppContext.BaseDirectory, "Assets", "Renderer");
            AntigravityViewer.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "appassets.local",
                assetsFolder,
                CoreWebView2HostResourceAccessKind.Allow);

            AntigravityViewer.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
            AntigravityViewer.NavigationCompleted += (s, e) =>
            {
                _isWebViewInitialized = true;
                UpdateWebViewContent(immediate: true);
                ApplyTheme();
                ApplyZoom();
            };

            AntigravityViewer.Source = new Uri("https://appassets.local/viewer.html");
        }
        catch (Exception ex)
        {
            // Gracefully handle environments without WebView2 runtime or sandbox restrictions
            Debug.WriteLine($"[MainPage] WebView2 initialization error: {ex}");
        }
    }

    private void UpdateWebViewContent(bool immediate = false)
    {
        if (!_isWebViewInitialized || AntigravityViewer.CoreWebView2 == null)
            return;

        if (immediate)
        {
            _renderDebounceTimer.Stop();
            ExecuteUpdateWebViewContent();
        }
        else
        {
            // Debounce rapid typing to avoid UI freezing on large documents (P20-T001)
            _renderDebounceTimer.Stop();
            _renderDebounceTimer.Start();
        }
    }

    private void ExecuteUpdateWebViewContent()
    {
        if (!_isWebViewInitialized || AntigravityViewer.CoreWebView2 == null)
            return;

        try
        {
            var content = _viewModel?.DocumentContent ?? string.Empty;
            var currentPath = _viewModel?.SelectedTab?.FilePath ?? string.Empty;
            var isTxt = currentPath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase);
            var isJson = currentPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
                         currentPath.EndsWith(".jsonc", StringComparison.OrdinalIgnoreCase) ||
                         currentPath.EndsWith(".jsonl", StringComparison.OrdinalIgnoreCase);

            var targetUri = isJson
                ? new Uri("https://appassets.local/json_viewer.html")
                : new Uri("https://appassets.local/viewer.html");

            if (AntigravityViewer.Source != targetUri)
            {
                AntigravityViewer.Source = targetUri;
                return;
            }

            var json = JsonSerializer.Serialize(new
            {
                type = "setContent",
                content = content,
                markdown = content,
                json = content,
                isText = isTxt
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

        // Synchronize page and window requested theme
        var pageElementTheme = (theme == "Dark") ? ElementTheme.Dark : ElementTheme.Light;
        this.RequestedTheme = pageElementTheme;
        if (App.MainWindow?.Content is FrameworkElement rootElem)
        {
            rootElem.RequestedTheme = pageElementTheme;
        }

        // Synchronize editor colors and visual states with the selected theme
        if (MarkdownEditorTextBox != null)
        {
            Color bgColor, fgColor, borderColor;
            ElementTheme editorTheme;

            switch (theme)
            {
                case "Light":
                    editorTheme = ElementTheme.Light;
                    bgColor = Color.FromArgb(255, 255, 255, 255);
                    fgColor = Color.FromArgb(255, 31, 35, 40);
                    borderColor = Color.FromArgb(255, 208, 215, 222);
                    break;
                case "Sepia":
                    editorTheme = ElementTheme.Light;
                    bgColor = Color.FromArgb(255, 251, 240, 217);
                    fgColor = Color.FromArgb(255, 56, 52, 40);
                    borderColor = Color.FromArgb(255, 220, 208, 184);
                    break;
                case "Dark":
                default:
                    editorTheme = ElementTheme.Dark;
                    bgColor = Color.FromArgb(255, 13, 17, 23);
                    fgColor = Color.FromArgb(255, 230, 237, 243);
                    borderColor = Color.FromArgb(255, 48, 54, 61);
                    break;
            }

            MarkdownEditorTextBox.RequestedTheme = editorTheme;
            var bgBrush = new SolidColorBrush(bgColor);
            var fgBrush = new SolidColorBrush(fgColor);
            var borderBrush = new SolidColorBrush(borderColor);

            MarkdownEditorTextBox.Background = bgBrush;
            MarkdownEditorTextBox.Foreground = fgBrush;
            MarkdownEditorTextBox.BorderBrush = borderBrush;

            // Override WinUI TextBox VisualState template brushes so Focused / PointerOver never reverts to black
            MarkdownEditorTextBox.Resources["TextControlBackground"] = bgBrush;
            MarkdownEditorTextBox.Resources["TextControlBackgroundPointerOver"] = bgBrush;
            MarkdownEditorTextBox.Resources["TextControlBackgroundFocused"] = bgBrush;
            MarkdownEditorTextBox.Resources["TextControlForeground"] = fgBrush;
            MarkdownEditorTextBox.Resources["TextControlForegroundPointerOver"] = fgBrush;
            MarkdownEditorTextBox.Resources["TextControlForegroundFocused"] = fgBrush;
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
                speedLevel = _viewModel.ScrollSpeed
            });
            AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
        }
        catch { }
    }

    private void ScrollSpeedCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_viewModel == null || ScrollSpeedCombo == null) return;

        if (ScrollSpeedCombo.SelectedItem is ComboBoxItem item &&
            double.TryParse(item.Tag?.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var speed))
        {
            _viewModel.ScrollSpeed = speed;
        }
        else if (ScrollSpeedCombo.SelectedItem is string str)
        {
            ParseAndApplyCustomSpeed(str);
        }
    }

    private void ScrollSpeedCombo_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
    {
        ParseAndApplyCustomSpeed(args.Text);
        args.Handled = true;
    }

    private void ParseAndApplyCustomSpeed(string? rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText) || _viewModel == null) return;
        var clean = rawText.Replace("x", "", StringComparison.OrdinalIgnoreCase).Trim();
        if (double.TryParse(clean, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var customSpeed) && customSpeed > 0)
        {
            _viewModel.ScrollSpeed = customSpeed;
            if (ScrollSpeedCombo != null && ScrollSpeedCombo.Text != $"{customSpeed}x")
            {
                ScrollSpeedCombo.Text = $"{customSpeed}x";
            }
        }
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
            if (root.TryGetProperty("type", out var typeProp))
            {
                var type = typeProp.GetString();
                if (type == "textSelected" && root.TryGetProperty("text", out var textProp))
                {
                    var selectedText = textProp.GetString() ?? string.Empty;
                    if (_viewModel != null && !string.IsNullOrWhiteSpace(selectedText))
                    {
                        _viewModel.SelectedQuoteText = selectedText;
                        if (!_viewModel.IsCommentsPaneOpen)
                        {
                            _viewModel.IsCommentsPaneOpen = true;
                        }
                    }
                }
            }
        }
        catch { }
    }

    private async void ExportPdf_Click(object sender, RoutedEventArgs e)
    {
        if (!_isWebViewInitialized || AntigravityViewer.CoreWebView2 == null)
            return;

        try
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("PDF Document", new List<string> { ".pdf" });
            picker.SuggestedFileName = Path.GetFileNameWithoutExtension(_viewModel?.CurrentDocumentPath ?? "Document") + ".pdf";

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var printSettings = AntigravityViewer.CoreWebView2.Environment.CreatePrintSettings();
                printSettings.ShouldPrintBackgrounds = true;
                printSettings.HeaderTitle = _viewModel?.CurrentDocumentPathDisplay ?? "MDReader";
                var success = await AntigravityViewer.CoreWebView2.PrintToPdfAsync(file.Path, printSettings);
                if (success)
                {
                    Process.Start(new ProcessStartInfo(file.Path) { UseShellExecute = true });
                }
            }
        }
        catch { }
    }

    private async void ExportHtml_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("Standalone HTML Document", new List<string> { ".html" });
            picker.SuggestedFileName = Path.GetFileNameWithoutExtension(_viewModel?.CurrentDocumentPath ?? "Document") + ".html";

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                var assetsFolder = Path.Combine(AppContext.BaseDirectory, "Assets", "Renderer");
                var viewerHtmlPath = Path.Combine(assetsFolder, "viewer.html");
                var template = File.Exists(viewerHtmlPath) ? await File.ReadAllTextAsync(viewerHtmlPath) : "<html><body><div id=\"content\"></div></body></html>";

                var mdContent = _viewModel?.DocumentContent ?? string.Empty;
                var safeMdJson = JsonSerializer.Serialize(mdContent);
                var currentTheme = (_viewModel?.CurrentTheme ?? "dark").ToLowerInvariant();

                // 1. Set the active theme
                var exportedHtml = template.Replace("<body data-theme=\"dark\">", $"<body data-theme=\"{currentTheme}\">");

                // 2. Set document title
                var docName = Path.GetFileNameWithoutExtension(_viewModel?.CurrentDocumentPath ?? "Document");
                var escapedTitle = System.Net.WebUtility.HtmlEncode(docName);
                exportedHtml = exportedHtml.Replace("<title>MDReader Antigravity Viewer</title>", $"<title>{escapedTitle} — MDReader</title>");

                // 3. Inject self-contained bootstrap script right before </body> to render offline without WebView host
                var bootstrapScript = $@"
    <script>
        (function() {{
            function renderDocument() {{
                try {{
                    const md = {safeMdJson};
                    const contentEl = document.getElementById('content');
                    if (contentEl && typeof parseAntigravityMarkdown === 'function') {{
                        contentEl.innerHTML = parseAntigravityMarkdown(md);
                        if (typeof setupHeadingObserver === 'function') {{
                            setupHeadingObserver();
                        }}
                    }}
                }} catch (err) {{
                    console.error('Failed to render exported markdown:', err);
                }}
            }}

            if (document.readyState === 'loading') {{
                document.addEventListener('DOMContentLoaded', renderDocument);
            }} else {{
                renderDocument();
            }}
        }})();
    </script>
</body>";
                exportedHtml = exportedHtml.Replace("</body>", bootstrapScript);

                await File.WriteAllTextAsync(file.Path, exportedHtml, Encoding.UTF8);
                Process.Start(new ProcessStartInfo(file.Path) { UseShellExecute = true });
            }
        }
        catch { }
    }

    public void ShowFind()
    {
        FindBar.Visibility = Visibility.Visible;
        FindTextBox.Focus(FocusState.Programmatic);
        FindTextBox.SelectAll();
        if (!string.IsNullOrWhiteSpace(FindTextBox.Text))
        {
            RunFindQuery();
        }
    }

    public void ShowReplace()
    {
        FindBar.Visibility = Visibility.Visible;
        ReplaceTextBox.Focus(FocusState.Programmatic);
        ReplaceTextBox.SelectAll();
        if (!string.IsNullOrWhiteSpace(FindTextBox.Text))
        {
            RunFindQuery();
        }
    }

    private void FindButton_Click(object sender, RoutedEventArgs e)
    {
        if (FindBar.Visibility == Visibility.Visible)
        {
            CloseFind();
        }
        else
        {
            ShowFind();
        }
    }

    private void CloseFind()
    {
        FindBar.Visibility = Visibility.Collapsed;
        if (_isWebViewInitialized && AntigravityViewer?.CoreWebView2 != null)
        {
            try
            {
                var json = JsonSerializer.Serialize(new { type = "clearSearch" });
                AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
            }
            catch { }
        }
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

    private void DocumentTabs_TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
    {
        try
        {
            if (args.Item is DocumentTabViewModel tab)
            {
                var content = tab.Content ?? string.Empty;
                var path = tab.FilePath;

                args.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Link;

                // 1. Text payload: Enables dragging directly into chat text boxes, comments, or web inputs
                if (!string.IsNullOrEmpty(content))
                {
                    args.Data.SetText(content);
                }

                // 2. Storage items payload: Enables dragging files into Windows Explorer, web upload zones, and chat attachments
                args.Data.SetDataProvider(StandardDataFormats.StorageItems, async request =>
                {
                    var deferral = request.GetDeferral();
                    try
                    {
                        StorageFile file;
                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
                        {
                            file = await StorageFile.GetFileFromPathAsync(path);
                        }
                        else
                        {
                            var tempFolder = Path.Combine(Path.GetTempPath(), "MDReader_DragDrop");
                            Directory.CreateDirectory(tempFolder);

                            var safeTitle = string.IsNullOrWhiteSpace(tab.Title) ? "Untitled.md" : tab.Title.Replace(" *", "");
                            if (!safeTitle.EndsWith(".md", StringComparison.OrdinalIgnoreCase) &&
                                !safeTitle.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                                !safeTitle.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                            {
                                safeTitle += ".md";
                            }

                            var tempPath = Path.Combine(tempFolder, safeTitle);
                            await File.WriteAllTextAsync(tempPath, content, Encoding.UTF8);
                            file = await StorageFile.GetFileFromPathAsync(tempPath);
                        }

                        request.SetData(new[] { file });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[TabDragStarting] DataProvider error: {ex}");
                    }
                    finally
                    {
                        deferral.Complete();
                    }
                });
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainPage] TabDragStarting error: {ex}");
        }
    }

    private void FindOption_Changed(object sender, RoutedEventArgs e)
    {
        RunFindQuery();
    }

    private void FindTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        RunFindQuery();
    }

    private void RunFindQuery()
    {
        _findMatches.Clear();
        _currentFindIndex = -1;
        var query = FindTextBox.Text;
        var content = _viewModel?.DocumentContent ?? string.Empty;
        if (string.IsNullOrWhiteSpace(query))
        {
            FindStatusText.Text = string.Empty;
            FindPreviewText.Text = string.Empty;
            if (_isWebViewInitialized && AntigravityViewer?.CoreWebView2 != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(new { type = "clearSearch" });
                    AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
                }
                catch { }
            }
            return;
        }

        bool matchCase = MatchCaseToggle?.IsChecked == true;
        bool isRegex = RegexToggle?.IsChecked == true;

        var matches = MarkdownEditorUtilities.FindMatches(content, query, matchCase, isRegex);
        _findMatches.AddRange(matches);

        if (_findMatches.Count == 0)
        {
            FindStatusText.Text = "No matches";
            FindPreviewText.Text = string.Empty;
            if (_isWebViewInitialized && AntigravityViewer?.CoreWebView2 != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(new { type = "clearSearch" });
                    AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
                }
                catch { }
            }
            return;
        }

        _currentFindIndex = 0;
        UpdateFindResult();
    }

    private void FindTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        try
        {
            if (e.Key == VirtualKey.Enter)
            {
                var shiftState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift);
                if ((shiftState & Windows.UI.Core.CoreVirtualKeyStates.Down) == Windows.UI.Core.CoreVirtualKeyStates.Down)
                    FindPrevious();
                else
                    FindNext();
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                CloseFind();
                e.Handled = true;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[FindTextBox_KeyDown] Error: {ex}");
        }
    }

    private void ReplaceTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            Replace_Click(sender, e);
            e.Handled = true;
        }
    }

    private void Replace_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel == null || _findMatches.Count == 0 || _currentFindIndex < 0 || _currentFindIndex >= _findMatches.Count)
            return;

        var match = _findMatches[_currentFindIndex];
        var content = _viewModel.DocumentContent ?? string.Empty;
        var query = FindTextBox.Text;
        var replacement = ReplaceTextBox.Text ?? string.Empty;
        bool matchCase = MatchCaseToggle?.IsChecked == true;
        bool isRegex = RegexToggle?.IsChecked == true;

        var (newContent, count) = MarkdownEditorUtilities.ReplaceText(content, query, replacement, matchCase, isRegex, match.Index, match.Length);
        if (count > 0)
        {
            _viewModel.DocumentContent = newContent;
            ReplaceStatusText.Text = "✓ Replaced match";
            RunFindQuery();
        }
    }

    private void ReplaceAll_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel == null) return;
        var content = _viewModel.DocumentContent ?? string.Empty;
        var query = FindTextBox.Text;
        var replacement = ReplaceTextBox.Text ?? string.Empty;
        if (string.IsNullOrEmpty(query)) return;

        bool matchCase = MatchCaseToggle?.IsChecked == true;
        bool isRegex = RegexToggle?.IsChecked == true;

        var (newContent, count) = MarkdownEditorUtilities.ReplaceText(content, query, replacement, matchCase, isRegex);
        if (count > 0)
        {
            _viewModel.DocumentContent = newContent;
            ReplaceStatusText.Text = $"✓ Replaced {count} matches";
            RunFindQuery();
        }
        else
        {
            ReplaceStatusText.Text = "0 matches found";
        }
    }

    private void FindPrevious_Click(object sender, RoutedEventArgs e) => FindPrevious();
    private void FindNext_Click(object sender, RoutedEventArgs e) => FindNext();
    private void CloseFind_Click(object sender, RoutedEventArgs e) => CloseFind();

    private void CopyDocument_Click(object sender, RoutedEventArgs e)
    {
        var content = _viewModel?.DocumentContent ?? string.Empty;
        if (string.IsNullOrEmpty(content)) return;

        try
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(content);
            Clipboard.SetContent(dataPackage);

            if (PathCopiedBadge != null)
            {
                PathCopiedBadge.Text = "✓ Document copied / تم نسخ النص";
                PathCopiedBadge.Visibility = Visibility.Visible;
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    PathCopiedBadge.Visibility = Visibility.Collapsed;
                    PathCopiedBadge.Text = "✓ تم نسخ المسار";
                };
                timer.Start();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainPage] CopyDocument error: {ex}");
        }
    }

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

    private void CopyPathButton_Click(object sender, RoutedEventArgs e)
    {
        var path = _viewModel?.CurrentDocumentPath;
        if (string.IsNullOrWhiteSpace(path))
        {
            path = _viewModel?.CurrentDocumentPathDisplay ?? "Untitled Document";
        }

        try
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(path);
            Clipboard.SetContent(dataPackage);

            if (PathCopiedBadge != null)
            {
                PathCopiedBadge.Visibility = Visibility.Visible;
                var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    PathCopiedBadge.Visibility = Visibility.Collapsed;
                };
                timer.Start();
            }
        }
        catch { }
    }

    private void MarkdownEditorTextBox_DragOver(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }
    }

    private async void MarkdownEditorTextBox_Drop(object sender, DragEventArgs e)
    {
        try
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                foreach (var item in items)
                {
                    if (item is StorageFile file && MarkdownEditorUtilities.IsImageFile(file.FileType))
                    {
                        await InsertImageFileAsync(file);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainPage] MarkdownEditorTextBox_Drop error: {ex}");
        }
    }

    private async void MarkdownEditorTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        try
        {
            var ctrlState = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control);
            var isCtrlDown = (ctrlState & Windows.UI.Core.CoreVirtualKeyStates.Down) == Windows.UI.Core.CoreVirtualKeyStates.Down;

            if (e.Key == VirtualKey.V && isCtrlDown)
            {
                var clipboard = Clipboard.GetContent();
                if (clipboard.Contains(StandardDataFormats.Bitmap))
                {
                    e.Handled = true;
                    await InsertClipboardBitmapAsync(clipboard);
                }
                else if (clipboard.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await clipboard.GetStorageItemsAsync();
                    foreach (var item in items)
                    {
                        if (item is StorageFile file && MarkdownEditorUtilities.IsImageFile(file.FileType))
                        {
                            e.Handled = true;
                            await InsertImageFileAsync(file);
                            return;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MarkdownEditorTextBox_KeyDown] Error: {ex}");
        }
    }

    private async Task InsertImageFileAsync(StorageFile file)
    {
        if (_viewModel == null) return;
        try
        {
            var docDir = !string.IsNullOrWhiteSpace(_viewModel.CurrentDocumentPath)
                ? Path.GetDirectoryName(_viewModel.CurrentDocumentPath)
                : null;

            var fileName = Path.GetFileName(file.Path);
            var (targetDir, targetFilePath, relativePath) = MarkdownEditorUtilities.GetImageDestination(docDir, fileName);

            if (!string.Equals(file.Path, targetFilePath, StringComparison.OrdinalIgnoreCase))
            {
                await Task.Run(() => File.Copy(file.Path, targetFilePath, overwrite: true));
            }

            var snippet = MarkdownEditorUtilities.GenerateMarkdownImageSnippet(Path.GetFileNameWithoutExtension(fileName), relativePath);
            InsertSnippetAtCaret(snippet);
        }
        catch { }
    }

    private async Task InsertClipboardBitmapAsync(DataPackageView clipboard)
    {
        if (_viewModel == null) return;
        try
        {
            var bitmapStream = await clipboard.GetBitmapAsync();
            var docDir = !string.IsNullOrWhiteSpace(_viewModel.CurrentDocumentPath)
                ? Path.GetDirectoryName(_viewModel.CurrentDocumentPath)
                : null;

            var fileName = $"pasted_image_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            var (targetDir, targetFilePath, relativePath) = MarkdownEditorUtilities.GetImageDestination(docDir, fileName);

            using (var stream = await bitmapStream.OpenReadAsync())
            using (var netStream = stream.AsStreamForRead())
            using (var fileStream = File.Create(targetFilePath))
            {
                await netStream.CopyToAsync(fileStream);
            }

            var snippet = MarkdownEditorUtilities.GenerateMarkdownImageSnippet("Pasted Image", relativePath);
            InsertSnippetAtCaret(snippet);
        }
        catch { }
    }

    private void InsertSnippetAtCaret(string snippet)
    {
        if (MarkdownEditorTextBox == null || _viewModel == null) return;

        var text = _viewModel.DocumentContent ?? string.Empty;
        var start = MarkdownEditorTextBox.SelectionStart;
        var length = MarkdownEditorTextBox.SelectionLength;

        var sb = new StringBuilder(text.Length + snippet.Length);
        sb.Append(text.Substring(0, start));
        sb.Append(snippet);
        sb.Append(text.Substring(start + length));

        _viewModel.DocumentContent = sb.ToString();
        MarkdownEditorTextBox.SelectionStart = start + snippet.Length;
        MarkdownEditorTextBox.SelectionLength = 0;
        MarkdownEditorTextBox.Focus(FocusState.Programmatic);
    }

    private void ExitZenMode_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.IsZenMode = false;
        }
    }

    private void MainPage_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Escape && _viewModel != null && _viewModel.IsZenMode)
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
        if (_findMatches.Count == 0 || _currentFindIndex < 0 || _currentFindIndex >= _findMatches.Count) return;
        var content = _viewModel?.DocumentContent ?? string.Empty;
        var match = _findMatches[_currentFindIndex];
        var previewStart = Math.Max(0, match.Index - 32);
        var previewLength = Math.Min(content.Length - previewStart, match.Length + 64);
        FindStatusText.Text = $"{_currentFindIndex + 1} of {_findMatches.Count}";
        FindPreviewText.Text = content.Substring(previewStart, previewLength).Replace('\r', ' ').Replace('\n', ' ');

        if (MarkdownEditorTextBox != null && match.Index + match.Length <= content.Length)
        {
            MarkdownEditorTextBox.Select(match.Index, match.Length);
        }

        if (_isWebViewInitialized && AntigravityViewer?.CoreWebView2 != null)
        {
            try
            {
                var query = FindTextBox.Text;
                var json = JsonSerializer.Serialize(new
                {
                    type = "highlightSearch",
                    query = query,
                    activeIndex = _currentFindIndex
                });
                AntigravityViewer.CoreWebView2.PostWebMessageAsJson(json);
            }
            catch { }
        }
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
                 e.PropertyName == nameof(ReaderViewModel.ScrollSpeed) ||
                 e.PropertyName == nameof(ReaderViewModel.ScrollSpeedSeconds))
        {
            ApplyAutoScroll();
        }
        else if (e.PropertyName == nameof(ReaderViewModel.SelectedTab) ||
                 e.PropertyName == nameof(ReaderViewModel.HasDocument))
        {
            UpdateWebViewContent(immediate: true);
        }
        else if (e.PropertyName == nameof(ReaderViewModel.DocumentContent))
        {
            UpdateWebViewContent(immediate: false);
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
