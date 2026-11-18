using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using MDReader.App.Features.Reader.ViewModels;
using MDReader.App.Features.Reader.Views;
using MDReader.App.Features.Settings.Services;
using MDReader.App.Features.Shell.ViewModels;
using MDReader.App.Shared.Helpers;
using MDReader.Core.Recent;
using MDReader.Core.Session;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDReader.App.Features.Shell.Views;

/// <summary>
/// The application window. This hosts a Frame that displays pages.
/// On launch it wires up the ShellViewModel (with the ReaderViewModel)
/// and handles command-line file arguments.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly ShellViewModel _shellViewModel;

    public MainWindow()
    {
        InitializeComponent();

        // Create the ReaderViewModel that MainPage will display
        var readerViewModel = new ReaderViewModel();

        // Create the ShellViewModel, coordinating Shell with the Reader
        _shellViewModel = new ShellViewModel(readerViewModel);

        // Provide the window handle so the FileOpenPicker can attach to this window
        _shellViewModel.WindowHandle = WindowHandleService.GetWindowHandle(this);

        // Set MVVM DataContext on root grid (ShellViewModel for menu/commands)
        RootGrid.DataContext = _shellViewModel;

        RegisterKeyboardAccelerators(readerViewModel);

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        // Load and set the application icon reliably for window and taskbar
        try
        {
            var baseDir = AppContext.BaseDirectory;
            var iconPath = Path.Combine(baseDir, "Assets", "AppIcon.ico");
            if (!File.Exists(iconPath))
            {
                // Fallback for development / dotnet run unpackaged from project root
                iconPath = Path.Combine(Directory.GetCurrentDirectory(), "src", "MDReader.App", "Assets", "AppIcon.ico");
            }
            if (File.Exists(iconPath))
            {
                AppWindow.SetIcon(Path.GetFullPath(iconPath));
            }
        }
        catch { }

        // Set initial window size and center
        var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(AppWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
        AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));
        var centerPosition = new Windows.Graphics.PointInt32(
            (displayArea.WorkArea.Width - 1200) / 2,
            (displayArea.WorkArea.Height - 800) / 2);
        AppWindow.Move(centerPosition);

        // Navigate the root frame to the main page, passing the ReaderViewModel
        RootFrame.Navigate(typeof(MainPage), readerViewModel);

        // Handle command-line file argument (mdreader.exe <path>) or restore session.
        var cliArgs = System.Environment.GetCommandLineArgs();
        if (cliArgs.Length > 1 && !string.IsNullOrWhiteSpace(cliArgs[1]))
        {
            var cliPath = cliArgs[1];
            DispatcherQueue.TryEnqueue(() => _shellViewModel.OpenDocument(cliPath));
        }
        else
        {
            DispatcherQueue.TryEnqueue(RestoreSessionTabs);
        }

        Closed += (s, e) =>
        {
            try
            {
                _shellViewModel.Reader.SaveSessionState();
            }
            catch { }
        };

        try
        {
            AutoSaveToggleItem.IsChecked = UserPreferencesService.Instance.AutoSaveDrafts;
            RestoreSessionToggleItem.IsChecked = UserPreferencesService.Instance.RestoreSession;
            TypewriterToggleItem.IsChecked = UserPreferencesService.Instance.TypewriterMode;

            // Subscribe to language changes for dynamic instant UI translation across 10 languages
            MDReader.Core.Localization.LocalizationService.Instance.PropertyChanged += (s, e) =>
            {
                DispatcherQueue.TryEnqueue(ApplyLanguage);
            };
            ApplyLanguage();
        }
        catch { }
    }

    private void RegisterKeyboardAccelerators(ReaderViewModel readerViewModel)
    {
        RegisterKeyboardAccelerator(VirtualKey.N, VirtualKeyModifiers.Control,
            () => _shellViewModel.NewDocumentCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.O, VirtualKeyModifiers.Control,
            () => _shellViewModel.OpenDocumentCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.O, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift,
            () => _shellViewModel.OpenFolderCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.S, VirtualKeyModifiers.Control,
            () => _shellViewModel.SaveDocumentCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.W, VirtualKeyModifiers.Control,
            () => _shellViewModel.CloseCurrentTabCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.B, VirtualKeyModifiers.Control,
            () => _shellViewModel.ToggleWorkspacePaneCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.P, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift,
            async () => await ShowCommandPaletteAsync());
        RegisterKeyboardAccelerator(VirtualKey.F2, VirtualKeyModifiers.None,
            async () => await ShowCommandPaletteAsync());
        RegisterKeyboardAccelerator(VirtualKey.F, VirtualKeyModifiers.Control,
            () => (RootFrame.Content as MainPage)?.ShowFind());
        RegisterKeyboardAccelerator(VirtualKey.H, VirtualKeyModifiers.Control,
            () => (RootFrame.Content as MainPage)?.ShowReplace());
        RegisterKeyboardAccelerator(VirtualKey.R, VirtualKeyModifiers.Control,
            () => _shellViewModel.RefreshDocumentCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.Number0, VirtualKeyModifiers.Control,
            () => readerViewModel.ResetZoomCommand.Execute(null));
        RegisterKeyboardAccelerator((VirtualKey)0xBB, VirtualKeyModifiers.Control | VirtualKeyModifiers.Shift,
            () => readerViewModel.ZoomInCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.Add, VirtualKeyModifiers.Control,
            () => readerViewModel.ZoomInCommand.Execute(null));
        RegisterKeyboardAccelerator((VirtualKey)0xBD, VirtualKeyModifiers.Control,
            () => readerViewModel.ZoomOutCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.Subtract, VirtualKeyModifiers.Control,
            () => readerViewModel.ZoomOutCommand.Execute(null));
        RegisterKeyboardAccelerator(VirtualKey.F1, VirtualKeyModifiers.None,
            async () => await ShowKeyboardShortcutsDialogAsync());
    }

    private void WorkspaceTreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
    {
        if (args.InvokedItem is MDReader.App.Features.Shell.ViewModels.FolderNodeViewModel node)
        {
            _shellViewModel.OnNodeSelected(node);
        }
    }

    private void RegisterKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers modifiers, Action execute)
    {
        var accelerator = new KeyboardAccelerator { Key = key, Modifiers = modifiers };
        accelerator.Invoked += (_, args) =>
        {
            execute();
            args.Handled = true;
        };
        RootGrid.KeyboardAccelerators.Add(accelerator);
    }
    private void Find_Click(object sender, RoutedEventArgs e) => (RootFrame.Content as MainPage)?.ShowFind();
    private void Replace_Click(object sender, RoutedEventArgs e) => (RootFrame.Content as MainPage)?.ShowReplace();

    private void MarkdownGuide_Click(object sender, RoutedEventArgs e)
    {
        try { _shellViewModel.Reader.OpenMarkdownGuide(); }
        catch { }
    }

    private void JsonGuide_Click(object sender, RoutedEventArgs e)
    {
        try { _shellViewModel.Reader.OpenJsonGuide(); }
        catch { }
    }

    private async void About_Click(object sender, RoutedEventArgs e)
    {
        try { await ShowAboutDialogAsync(); }
        catch { }
    }

    private async System.Threading.Tasks.Task ShowAboutDialogAsync()
    {
        if (Content?.XamlRoot == null) return;

        var stack = new StackPanel { Spacing = 10, MaxWidth = 460 };

        var title = new TextBlock
        {
            Text = "JMDReader v2.0 (birotech)",
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            FontSize = 18
        };
        stack.Children.Add(title);

        var subtitle = new TextBlock
        {
            Text = "Modern Markdown, JSON & Text Workspace for Windows 11",
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 13,
            Opacity = 0.85
        };
        stack.Children.Add(subtitle);

        var desc = new TextBlock
        {
            Text = "قارئ ومحرر ماركداون متقدم ومستعرض JSON الشجري الملون.\n\nمعايير الأمان الرقمي:\n• يعمل بدون اتصال بالإنترنت بنسبة 100% (100% Offline).\n• خالي تماماً من جمع البيانات أو التتبع (Zero Telemetry).\n• أداء أصيل وسلس على معالجات x64 و Arm64 الحديثة.",
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12,
            LineHeight = 20
        };
        stack.Children.Add(desc);

        var copyright = new TextBlock
        {
            Text = "Developed by birotech (Abdullah Ali Khaled Moqbel)\nLicensed under the MIT License.",
            FontSize = 11,
            Opacity = 0.65,
            Margin = new Thickness(0, 10, 0, 0)
        };
        stack.Children.Add(copyright);

        var dialog = new ContentDialog
        {
            Title = "About JMDReader / عن البرنامج",
            Content = stack,
            CloseButtonText = "Close / إغلاق",
            XamlRoot = Content.XamlRoot
        };

        await dialog.ShowAsync();
    }

    private async void KeyboardShortcuts_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await ShowKeyboardShortcutsDialogAsync();
        }
        catch { }
    }

    private async System.Threading.Tasks.Task ShowKeyboardShortcutsDialogAsync()
    {
        if (Content?.XamlRoot == null) return;

        var stack = new StackPanel { Spacing = 8, MaxWidth = 460 };
        var intro = new TextBlock
        {
            Text = "MDReader Antigravity Keyboard Shortcuts / اختصارات لوحة المفاتيح",
            FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
            FontSize = 14,
            Margin = new Thickness(0, 0, 0, 8)
        };
        stack.Children.Add(intro);

        var shortcuts = new (string Key, string Desc)[]
        {
            ("Ctrl + N", "New Document / مستند جديد"),
            ("Ctrl + O", "Open Markdown File / فتح ملف"),
            ("Ctrl + Shift + O", "Open Folder Workspace / فتح مجلد مساحة عمل"),
            ("Ctrl + S", "Save Document / حفظ"),
            ("Ctrl + Shift + S", "Save As / حفظ باسم"),
            ("Ctrl + W", "Close Current Tab / إغلاق التبويب"),
            ("Ctrl + F", "Find & Replace / بحث واستبدال متقدم"),
            ("Ctrl + B", "Toggle Workspace Sidebar / الشريط الجانبي للمجلد"),
            ("F11", "Zen Focus Mode / وضع التركيز التام"),
            ("Ctrl + P", "Export to PDF / تصدير وطباعة PDF"),
            ("Ctrl + E", "Export to Standalone HTML / تصدير صفحة ويب كاملة"),
            ("Ctrl + +/-", "Zoom In / Out / تكبير وتصغير العرض"),
            ("Ctrl + 0", "Reset Zoom / إعادة ضبط الزووم 100%"),
            ("F1", "Keyboard Shortcuts / دليل الاختصارات")
        };

        foreach (var (k, d) in shortcuts)
        {
            var row = new Grid { Margin = new Thickness(0, 2, 0, 2) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(140) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var keyBadge = new Border
            {
                Background = (Brush)Application.Current.Resources["AppSubtleBackgroundBrush"],
                BorderBrush = (Brush)Application.Current.Resources["AppBorderBrush"],
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(6, 2, 6, 2),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            keyBadge.Child = new TextBlock
            {
                Text = k,
                FontFamily = new FontFamily("Cascadia Code, Consolas"),
                FontSize = 11,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            };
            Grid.SetColumn(keyBadge, 0);

            var descText = new TextBlock
            {
                Text = d,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 0, 0)
            };
            Grid.SetColumn(descText, 1);

            row.Children.Add(keyBadge);
            row.Children.Add(descText);
            stack.Children.Add(row);
        }

        var scroll = new ScrollViewer { Content = stack, MaxHeight = 440 };

        var dialog = new ContentDialog
        {
            Title = "⌨️ Keyboard Shortcuts",
            Content = scroll,
            CloseButtonText = "Close",
            XamlRoot = Content.XamlRoot
        };

        await dialog.ShowAsync();
    }

    private void RootGrid_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        e.DragUIOverride.Caption = "Open Markdown file";
        e.DragUIOverride.IsCaptionVisible = true;
        e.DragUIOverride.IsContentVisible = true;
    }

    private async void RootGrid_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.AvailableFormats.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
        {
            try
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0];
                    if (file.IsOfType(Windows.Storage.StorageItemTypes.File) && MDReader.Core.Documents.MarkdownFileReader.IsSupportedExtension(file.Name))
                    {
                        _shellViewModel.OpenDocument(file.Path);
                    }
                }
            }
            catch
            {
                // Ignore drop failures
            }
        }
    }

    private async void CommandPalette_Click(object sender, RoutedEventArgs e)
    {
        await ShowCommandPaletteAsync();
    }

    private void AutoSaveToggle_Click(object sender, RoutedEventArgs e)
    {
        UserPreferencesService.Instance.AutoSaveDrafts = AutoSaveToggleItem.IsChecked;
        UserPreferencesService.Instance.Save();
        _shellViewModel.Reader.IsAutoSaveDraftsEnabled = AutoSaveToggleItem.IsChecked;
    }

    private void RestoreSessionToggle_Click(object sender, RoutedEventArgs e)
    {
        UserPreferencesService.Instance.RestoreSession = RestoreSessionToggleItem.IsChecked;
        UserPreferencesService.Instance.Save();
        _shellViewModel.Reader.IsRestoreSessionEnabled = RestoreSessionToggleItem.IsChecked;
    }

    private void TypewriterToggle_Click(object sender, RoutedEventArgs e)
    {
        UserPreferencesService.Instance.TypewriterMode = TypewriterToggleItem.IsChecked;
        UserPreferencesService.Instance.Save();
        _shellViewModel.Reader.IsTypewriterMode = TypewriterToggleItem.IsChecked;
    }

    private void ToggleTypewriter_Click(object sender, RoutedEventArgs e)
    {
        _shellViewModel.Reader.ToggleTypewriterMode();
        TypewriterToggleItem.IsChecked = _shellViewModel.Reader.IsTypewriterMode;
    }

    public void ApplyLanguage()
    {
        var s = MDReader.Core.Localization.LocalizationService.Instance;

        // File Menu
        if (FileMenuBarItem != null) FileMenuBarItem.Title = s.MenuFile;
        if (MenuNewItem != null) MenuNewItem.Text = $"📄 {s.MenuNew}";
        if (MenuOpenItem != null) MenuOpenItem.Text = $"📂 {s.MenuOpenFile}";
        if (MenuOpenFolderItem != null) MenuOpenFolderItem.Text = $"📁 {s.MenuOpenFolder}";
        if (RecentFilesMenuSubItem != null) RecentFilesMenuSubItem.Text = $"🕒 {s.MenuRecentFiles}";
        if (MenuSaveItem != null) MenuSaveItem.Text = $"💾 {s.MenuSave}";
        if (MenuSaveAsItem != null) MenuSaveAsItem.Text = $"💾 {s.MenuSaveAs}";
        if (MenuCloseTabItem != null) MenuCloseTabItem.Text = $"❌ {s.MenuCloseTab}";

        // View Menu
        if (ViewMenuBarItem != null) ViewMenuBarItem.Title = s.MenuView;
        if (MenuCommandPaletteItem != null) MenuCommandPaletteItem.Text = $"⚡ {s.MenuCommandPalette}";
        if (MenuToggleSidebarItem != null) MenuToggleSidebarItem.Text = $"📑 {s.MenuToggleSidebar}";
        if (MenuToggleTypewriterItem != null) MenuToggleTypewriterItem.Text = $"⌨️ {s.MenuTypewriter}";
        if (MenuRefreshItem != null) MenuRefreshItem.Text = $"🔄 {s.MenuRefresh}";
        if (MenuFindItem != null) MenuFindItem.Text = $"🔍 {s.MenuFind}";
        if (MenuReplaceItem != null) MenuReplaceItem.Text = $"✏️ {s.MenuReplace}";

        // Settings Menu
        if (SettingsMenuBarItem != null) SettingsMenuBarItem.Title = s.MenuSettings;
        if (MenuThemeSubItem != null) MenuThemeSubItem.Text = $"🎨 {s.MenuTheme}";
        if (MenuThemeSystemItem != null) MenuThemeSystemItem.Text = $"💻 {s.MenuThemeSystem}";
        if (MenuThemeLightItem != null) MenuThemeLightItem.Text = $"☀️ {s.MenuThemeLight}";
        if (MenuThemeDarkItem != null) MenuThemeDarkItem.Text = $"🌙 {s.MenuThemeDark}";
        if (MenuThemeDarkSlateItem != null) MenuThemeDarkSlateItem.Text = $"🌑 {s.MenuThemeDarkSlate}";
        if (MenuThemeSepiaItem != null) MenuThemeSepiaItem.Text = $"📜 {s.MenuThemeSepia}";
        if (MenuPreferencesSubItem != null) MenuPreferencesSubItem.Text = $"⚙️ {s.MenuPreferences}";
        if (AutoSaveToggleItem != null) AutoSaveToggleItem.Text = $"💾 {s.MenuAutoSave}";
        if (RestoreSessionToggleItem != null) RestoreSessionToggleItem.Text = $"🔄 {s.MenuRestoreSession}";
        if (TypewriterToggleItem != null) TypewriterToggleItem.Text = $"⌨️ {s.MenuTypewriter}";
        if (MenuLanguageSubItem != null) MenuLanguageSubItem.Text = $"🌐 {s.MenuLanguage}";

        // Help Menu
        if (HelpMenuBarItem != null) HelpMenuBarItem.Title = s.MenuHelp;
        if (MenuMarkdownGuideItem != null) MenuMarkdownGuideItem.Text = $"📖 {s.MenuMarkdownGuide}";
        if (MenuJsonGuideItem != null) MenuJsonGuideItem.Text = $"🌳 {s.MenuJsonGuide}";
        if (MenuShortcutsItem != null) MenuShortcutsItem.Text = $"⌨️ {s.MenuShortcuts}";
        if (MenuAboutItem != null) MenuAboutItem.Text = $"ℹ️ {s.MenuAbout}";

        PopulateRecentFilesMenu();
    }

    public void PopulateRecentFilesMenu()
    {
        if (RecentFilesMenuSubItem == null) return;
        RecentFilesMenuSubItem.Items.Clear();

        var s = MDReader.Core.Localization.LocalizationService.Instance;
        var recentFiles = UserPreferencesService.Instance.RecentFiles.GetRecentFiles();
        if (recentFiles.Count == 0)
        {
            var emptyItem = new MenuFlyoutItem { Text = s.NoRecentFiles, IsEnabled = false };
            RecentFilesMenuSubItem.Items.Add(emptyItem);
            return;
        }

        foreach (var file in recentFiles)
        {
            var icon = file.IsPinned ? "📌 " : (file.TypeGlyph + " ");
            var item = new MenuFlyoutItem
            {
                Text = $"{icon}{file.Title}  ({file.DisplayTime})",
                Tag = file.Path
            };
            ToolTipService.SetToolTip(item, file.Path);
            item.Click += (sSender, eArgs) =>
            {
                if (sSender is MenuFlyoutItem mfi && mfi.Tag is string path && File.Exists(path))
                {
                    _shellViewModel.OpenDocument(path);
                    PopulateRecentFilesMenu();
                }
            };
            RecentFilesMenuSubItem.Items.Add(item);
        }

        RecentFilesMenuSubItem.Items.Add(new MenuFlyoutSeparator());
        var clearItem = new MenuFlyoutItem { Text = $"🗑️ {s.ClearHistory}" };
        clearItem.Click += (sSender, eArgs) =>
        {
            UserPreferencesService.Instance.RecentFiles.ClearAll();
            PopulateRecentFilesMenu();
            _shellViewModel.Reader.RefreshRecentFiles();
        };
        RecentFilesMenuSubItem.Items.Add(clearItem);
    }

    private void RestoreSessionTabs()
    {
        if (!UserPreferencesService.Instance.RestoreSession) return;
        try
        {
            var state = SessionManager.LoadSession(UserPreferencesService.Instance.SessionFilePath);
            if (state != null && state.OpenDocumentPaths != null && state.OpenDocumentPaths.Count > 0)
            {
                foreach (var path in state.OpenDocumentPaths)
                {
                    if (File.Exists(path))
                    {
                        _shellViewModel.OpenDocument(path);
                    }
                }
            }
        }
        catch { }
    }

    private sealed class CommandPaletteItem
    {
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Shortcut { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public Action Execute { get; set; } = () => { };
    }

    public async Task ShowCommandPaletteAsync()
    {
        try
        {
            var allCommands = new List<CommandPaletteItem>
            {
                new() { Title = "New Document / مستند جديد", Category = "File", Shortcut = "Ctrl+N", Icon = "📄", Execute = () => _shellViewModel.NewDocument() },
                new() { Title = "Open File... / فتح ملف", Category = "File", Shortcut = "Ctrl+O", Icon = "📂", Execute = () => _shellViewModel.OpenDocumentCommand.Execute(null) },
                new() { Title = "Open Folder... / فتح مجلد", Category = "File", Shortcut = "Ctrl+Shift+O", Icon = "📁", Execute = () => _shellViewModel.OpenFolderCommand.Execute(null) },
                new() { Title = "Save / حفظ", Category = "File", Shortcut = "Ctrl+S", Icon = "💾", Execute = () => _shellViewModel.SaveDocumentCommand.Execute(null) },
                new() { Title = "Save As... / حفظ باسم", Category = "File", Shortcut = "Ctrl+Shift+S", Icon = "💾", Execute = () => _shellViewModel.SaveDocumentAsCommand.Execute(null) },
                new() { Title = "Export to PDF / تصدير PDF", Category = "Export", Shortcut = "Ctrl+P", Icon = "🖨️", Execute = () => (RootFrame.Content as MainPage)?.ExportPdf() },
                new() { Title = "Export Standalone HTML / تصدير كصفحة ويب", Category = "Export", Shortcut = "", Icon = "🌐", Execute = () => (RootFrame.Content as MainPage)?.ExportHtml() },
                new() { Title = "Find in Document / بحث", Category = "Edit", Shortcut = "Ctrl+F", Icon = "🔍", Execute = () => (RootFrame.Content as MainPage)?.ShowFind() },
                new() { Title = "Replace in Document / استبدال", Category = "Edit", Shortcut = "Ctrl+H", Icon = "🔁", Execute = () => (RootFrame.Content as MainPage)?.ShowReplace() },
                new() { Title = "Toggle Split View / وضع العرض المنقسم", Category = "View", Shortcut = "Ctrl+E", Icon = "◫", Execute = () => _shellViewModel.Reader.SwitchViewMode("Split") },
                new() { Title = "Toggle Fullscreen Focus / وضع التركيز", Category = "View", Shortcut = "F11", Icon = "🖥️", Execute = () => _shellViewModel.Reader.ToggleZenModeCommand.Execute(null) },
                new() { Title = "Toggle Typewriter Mode / وضع الآلة الكاتبة", Category = "View", Shortcut = "", Icon = "⌨️", Execute = () => ToggleTypewriter_Click(this, null!) },
                new() { Title = "Theme: System / المظهر حسب النظام", Category = "Theme", Shortcut = "", Icon = "💻", Execute = () => _shellViewModel.SetTheme("System") },
                new() { Title = "Theme: Light / المظهر الفاتح", Category = "Theme", Shortcut = "", Icon = "☀️", Execute = () => _shellViewModel.SetTheme("Light") },
                new() { Title = "Theme: Dark / المظهر الداكن", Category = "Theme", Shortcut = "", Icon = "🌙", Execute = () => _shellViewModel.SetTheme("Dark") },
                new() { Title = "Theme: Dark Slate / رمادي فاحم", Category = "Theme", Shortcut = "", Icon = "🌑", Execute = () => _shellViewModel.SetTheme("DarkSlate") },
                new() { Title = "Theme: Sepia / مظهر الورق", Category = "Theme", Shortcut = "", Icon = "📜", Execute = () => _shellViewModel.SetTheme("Sepia") },
                new() { Title = "JSON: Beautify & Format / تجميل الكود", Category = "JSON", Shortcut = "", Icon = "✨", Execute = () => _shellViewModel.Reader.BeautifyJsonCommand.Execute(null) },
                new() { Title = "JSON: Minify / ضغط الكود", Category = "JSON", Shortcut = "", Icon = "⚡", Execute = () => _shellViewModel.Reader.MinifyJsonCommand.Execute(null) },
                new() { Title = "JSON: Convert to Markdown Table / تحويل لجدول", Category = "JSON", Shortcut = "", Icon = "📊", Execute = () => _shellViewModel.Reader.ConvertJsonToTableCommand.Execute(null) },
                new() { Title = "Document Reading Analytics / إحصائيات القراءة", Category = "Tools", Shortcut = "", Icon = "📊", Execute = () => (RootFrame.Content as MainPage)?.ShowReadingStats() },
                new() { Title = "Interactive Markdown Guide / دليل الماركداون", Category = "Help", Shortcut = "", Icon = "📖", Execute = () => MarkdownGuide_Click(this, null!) },
                new() { Title = "Interactive JSON Guide / دليل الجيسون", Category = "Help", Shortcut = "", Icon = "🌳", Execute = () => JsonGuide_Click(this, null!) },
                new() { Title = "Keyboard Shortcuts Cheat Sheet / الاختصارات", Category = "Help", Shortcut = "F1", Icon = "⌨️", Execute = () => KeyboardShortcuts_Click(this, null!) },
                new() { Title = "About JMDReader / عن البرنامج", Category = "Help", Shortcut = "", Icon = "ℹ️", Execute = () => About_Click(this, null!) }
            };

            foreach (var tab in _shellViewModel.Reader.Tabs)
            {
                var targetTab = tab;
                allCommands.Insert(0, new CommandPaletteItem
                {
                    Title = $"Switch to Tab: {targetTab.Title}",
                    Category = "Open Tabs",
                    Shortcut = targetTab.IsDirty ? "● Unsaved" : "",
                    Icon = targetTab.IsJsonDocument ? "🌳" : "📝",
                    Execute = () => _shellViewModel.Reader.SelectedTab = targetTab
                });
            }

            var dialog = new ContentDialog
            {
                Title = "⚡ Command Palette / لوحة الأوامر السريعة",
                CloseButtonText = "Cancel / إلغاء",
                XamlRoot = this.Content.XamlRoot
            };

            var sp = new StackPanel { Spacing = 10, MinWidth = 460, MaxHeight = 420 };
            var searchBox = new TextBox
            {
                PlaceholderText = "Type a command or tab name... / ابحث عن أمر...",
                Margin = new Thickness(0, 0, 0, 4)
            };

            var listView = new ListView
            {
                SelectionMode = ListViewSelectionMode.Single,
                IsItemClickEnabled = true,
                MaxHeight = 320
            };

            void UpdateList(string query)
            {
                var filtered = string.IsNullOrWhiteSpace(query)
                    ? allCommands
                    : allCommands.Where(c => c.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                             c.Category.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                             c.Shortcut.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

                listView.ItemsSource = filtered;
                if (filtered.Count > 0) listView.SelectedIndex = 0;
            }

            UpdateList("");
            searchBox.TextChanged += (s, e) => UpdateList(searchBox.Text);

            listView.ItemTemplate = (DataTemplate)Microsoft.UI.Xaml.Markup.XamlReader.Load(@"
                <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <Grid Padding=""6,4"" ColumnSpacing=""10"">
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width=""Auto"" />
                            <ColumnDefinition Width=""*"" />
                            <ColumnDefinition Width=""Auto"" />
                        </Grid.ColumnDefinitions>
                        <TextBlock Text=""{Binding Icon}"" FontSize=""14"" VerticalAlignment=""Center"" />
                        <StackPanel Grid.Column=""1"" VerticalAlignment=""Center"">
                            <TextBlock Text=""{Binding Title}"" FontSize=""12"" FontWeight=""SemiBold"" />
                            <TextBlock Text=""{Binding Category}"" FontSize=""10"" Opacity=""0.6"" />
                        </StackPanel>
                        <TextBlock Grid.Column=""2"" Text=""{Binding Shortcut}"" FontSize=""11"" Opacity=""0.7"" VerticalAlignment=""Center"" />
                    </Grid>
                </DataTemplate>");

            searchBox.KeyDown += (s, e) =>
            {
                if (e.Key == VirtualKey.Enter)
                {
                    if (listView.SelectedItem is CommandPaletteItem selected)
                    {
                        dialog.Hide();
                        selected.Execute();
                    }
                }
                else if (e.Key == VirtualKey.Down)
                {
                    if (listView.SelectedIndex < (listView.ItemsSource as List<CommandPaletteItem>)?.Count - 1)
                        listView.SelectedIndex++;
                }
                else if (e.Key == VirtualKey.Up)
                {
                    if (listView.SelectedIndex > 0)
                        listView.SelectedIndex--;
                }
            };

            listView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is CommandPaletteItem selected)
                {
                    dialog.Hide();
                    selected.Execute();
                }
            };

            sp.Children.Add(searchBox);
            sp.Children.Add(listView);
            dialog.Content = sp;

            dialog.Opened += (s, e) => searchBox.Focus(FocusState.Programmatic);
            await dialog.ShowAsync();
        }
        catch { }
    }
}
