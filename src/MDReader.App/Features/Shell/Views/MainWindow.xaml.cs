using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MDReader.App.Features.Reader.ViewModels;
using MDReader.App.Features.Reader.Views;
using MDReader.App.Features.Shell.ViewModels;
using MDReader.App.Shared.Helpers;

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

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");

        // Set initial window size and center
        var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(AppWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Primary);
        AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));
        var centerPosition = new Windows.Graphics.PointInt32(
            (displayArea.WorkArea.Width - 1200) / 2,
            (displayArea.WorkArea.Height - 800) / 2);
        AppWindow.Move(centerPosition);

        // Navigate the root frame to the main page, passing the ReaderViewModel
        RootFrame.Navigate(typeof(MainPage), readerViewModel);

        // Handle command-line file argument (mdreader.exe <path>)
        var cliArgs = System.Environment.GetCommandLineArgs();
        if (cliArgs.Length > 1 && !string.IsNullOrWhiteSpace(cliArgs[1]))
        {
            _shellViewModel.OpenDocument(cliArgs[1]);
        }
    }
}
