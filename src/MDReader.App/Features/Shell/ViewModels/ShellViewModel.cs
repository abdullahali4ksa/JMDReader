// MIT License - see LICENSE file in the repository root.

using CommunityToolkit.Mvvm.ComponentModel;

namespace MDReader.App.Features.Shell.ViewModels;

/// <summary>
/// ViewModel for the application shell (window management, navigation, app menu).
/// Uses CommunityToolkit.Mvvm ObservableObject for MVVM binding.
/// </summary>
public sealed partial class ShellViewModel : ObservableObject
{
    private string _windowTitle = "MDReader";

    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value);
    }
}
