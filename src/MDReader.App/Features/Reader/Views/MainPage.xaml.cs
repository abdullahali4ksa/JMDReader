using System.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using MDReader.App.Features.Reader.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDReader.App.Features.Reader.Views;

/// <summary>
/// The main content page displayed inside the application window.
/// Receives the ReaderViewModel via navigation parameters, sets it as the
/// page's DataContext for MVVM binding, and renders the parsed Markdown
/// element tree into WinUI controls via <see cref="MarkdownRenderer"/>.
/// </summary>
public sealed partial class MainPage : Page
{
    private readonly MarkdownRenderer _renderer = new();
    private ReaderViewModel? _viewModel;

    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the page is navigated to. Sets the ReaderViewModel
    /// passed from MainWindow as this page's DataContext and subscribes to
    /// its property changes so the rendered content refreshes on load/replace.
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is ReaderViewModel readerViewModel)
        {
            _viewModel = readerViewModel;
            DataContext = readerViewModel;
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            RenderDocument();
        }
    }

    /// <summary>Detaches the handler when the page is unloaded.</summary>
    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        base.OnNavigatedFrom(e);
        if (_viewModel is not null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ReaderViewModel.Elements) ||
            e.PropertyName == nameof(ReaderViewModel.HasDocument))
        {
            RenderDocument();
        }
    }

    /// <summary>
    /// Rebuilds the visual tree from the current element model. Clearing the
    /// panel first releases the previous document's controls when a new
    /// document replaces the current one.
    /// </summary>
    private void RenderDocument()
    {
        RenderedDocumentPanel.Children.Clear();

        var elements = _viewModel?.Elements;
        if (elements is null)
            return;

        var basePath = _viewModel?.CurrentDocumentPath;
        foreach (var element in _renderer.Render(elements, basePath))
            RenderedDocumentPanel.Children.Add(element);
    }
}
