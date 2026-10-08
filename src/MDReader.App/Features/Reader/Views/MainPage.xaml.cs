using System.ComponentModel;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MDReader.App.Features.Reader.ViewModels;
using Windows.UI.Core;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our app templates, see: http://aka.ms/winui-project-info.

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
        KeyDown += OnKeyDown;
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

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (_viewModel is null)
            return;

        var ctrl = InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control)
            .HasFlag(VirtualKeyStates.Down);

        if (!ctrl)
            return;

        switch (e.Key)
        {
            case VirtualKey.Add:
                _viewModel.ZoomInCommand.Execute(null);
                e.Handled = true;
                break;
            case VirtualKey.Subtract:
                _viewModel.ZoomOutCommand.Execute(null);
                e.Handled = true;
                break;
            case VirtualKey.Number0:
                _viewModel.ResetZoomCommand.Execute(null);
                e.Handled = true;
                break;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ReaderViewModel.Elements) ||
            e.PropertyName == nameof(ReaderViewModel.HasDocument))
        {
            RenderDocument();
        }
        else if (e.PropertyName == nameof(ReaderViewModel.ZoomRatio))
        {
            ApplyZoom();
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

        ApplyZoom();
    }

    /// <summary>
    /// Applies the current zoom ratio to the rendered document panel via a
    /// ScaleTransform. The zoom is applied as a render transform so it does
    /// not affect layout or cause infinite measure/arrange cycles.
    /// </summary>
    private void ApplyZoom()
    {
        if (_viewModel is null)
            return;

        var scale = _viewModel.ZoomRatio;
        RenderedDocumentPanel.RenderTransform = new ScaleTransform
        {
            ScaleX = scale,
            ScaleY = scale,
        };
    }
}
