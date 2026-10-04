using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MDReader.App.Features.Reader.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDReader.App.Features.Reader.Views;

/// <summary>
/// The main content page displayed inside the application window.
/// Receives the ReaderViewModel via navigation parameters and sets it
/// as the page's DataContext for MVVM binding.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the page is navigated to. Sets the ReaderViewModel
    /// passed from MainWindow as this page's DataContext.
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is ReaderViewModel readerViewModel)
        {
            DataContext = readerViewModel;
        }
    }
}
