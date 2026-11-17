using System;
using System.IO;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;
using MDReader.App.Features.Reader.ViewModels;
using MDReader.App.Features.Reader.Views;
using MDReader.App.Features.Shell.ViewModels;
using MDReader.App.Shared.Helpers;
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

        // Handle command-line file argument (mdreader.exe <path>).
        // Deferred until the window is activated: building the rendered visual
        // tree inside the constructor (before Activate) races with WinUI
        // activation and can intermittently crash the process. Enqueuing the
        // open on the dispatcher runs it after activation, on the UI thread.
        var cliArgs = System.Environment.GetCommandLineArgs();
        if (cliArgs.Length > 1 && !string.IsNullOrWhiteSpace(cliArgs[1]))
        {
            var cliPath = cliArgs[1];
            DispatcherQueue.TryEnqueue(() => _shellViewModel.OpenDocument(cliPath));
        }
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
}
