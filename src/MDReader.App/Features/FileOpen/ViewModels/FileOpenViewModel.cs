// MIT License - see LICENSE file in the repository root.

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace MDReader.App.Features.FileOpen.ViewModels;

/// <summary>
/// COM interface required to attach a window handle to a <see cref="FileOpenPicker"/>
/// in unpackaged WinUI 3 desktop applications.
/// </summary>
[ComImport]
[Guid("781deb8d-0efc-4b60-a7ee-0c4d7d4b7d50")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IInitializeWithWindow
{
    void SetWindow(IntPtr hwnd);
}

/// <summary>
/// ViewModel for the file open dialog — handles file picking with
/// Markdown-specific filtering (.md, .markdown) and path exposure.
/// Communicates the selected path to the Shell for document loading.
/// </summary>
public sealed partial class FileOpenViewModel : ObservableObject
{
    private string? _selectedFilePath;

    /// <summary>Gets or sets the most recently selected file path.</summary>
    public string? SelectedFilePath
    {
        get => _selectedFilePath;
        set => SetProperty(ref _selectedFilePath, value);
    }

    /// <summary>
    /// Shows a file open picker filtered to Markdown files (.md, .markdown)
    /// and returns the selected file path.
    /// </summary>
    /// <param name="hwnd">Window handle for the picker (required for unpackaged apps).</param>
    /// <returns>The selected file path, or null if the user cancels or an error occurs.</returns>
    public async Task<string?> PickMarkdownFileAsync(IntPtr hwnd)
    {
        try
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".md");
            picker.FileTypeFilter.Add(".markdown");
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Attach the window handle — required for unpackaged WinUI 3 apps.
            // Uses COM interop (Marshal + QueryInterface) because the Windows App
            // SDK configuration in this project does not expose `.As<T>()` on
            // the FileOpenPicker projection.
            AttachWindowHandle(picker, hwnd);

            var file = await picker.PickSingleFileAsync();
            if (file is null)
                return null; // user cancelled

            // For files selected from the local file system, Path is available
            SelectedFilePath = file.Path;
            return SelectedFilePath;
        }
        catch (System.UnauthorizedAccessException)
        {
            // File in a protected location — return null, caller shows error
            return null;
        }
        catch (System.Exception)
        {
            // Any other picker error — return null, caller shows error
            return null;
        }
    }

    /// <summary>
    /// Attaches a Win32 window handle to a FileOpenPicker via COM interop,
    /// required for unpackaged WinUI 3 applications.
    /// </summary>
    private static void AttachWindowHandle(FileOpenPicker picker, IntPtr hwnd)
    {
        var iUnknown = Marshal.GetIUnknownForObject(picker);
        try
        {
            var iid = typeof(IInitializeWithWindow).GUID;
            Marshal.QueryInterface(iUnknown, in iid, out var interfacePtr);
            try
            {
                if (interfacePtr != IntPtr.Zero)
                {
                    var initializeWithWindow = (IInitializeWithWindow)Marshal.GetObjectForIUnknown(interfacePtr);
                    initializeWithWindow.SetWindow(hwnd);
                }
            }
            finally
            {
                if (interfacePtr != IntPtr.Zero)
                    Marshal.Release(interfacePtr);
            }
        }
        finally
        {
            Marshal.Release(iUnknown);
        }
    }
}
