using Microsoft.UI.Xaml;
using MDReader.App.Features.Shell.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDReader.App.Features.Shell.Views;

/// <summary>
/// The application window. This hosts a Frame that displays pages. Add your
/// UI and logic to MainPage.xaml / MainPage.xaml.cs instead of here so you
/// can use Page features such as navigation events and the Loaded lifecycle.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Set MVVM DataContext on root grid
        RootGrid.DataContext = new ShellViewModel();

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

        // Navigate the root frame to the main page on startup.
        RootFrame.Navigate(typeof(MDReader.App.Features.Reader.Views.MainPage));
    }
}
