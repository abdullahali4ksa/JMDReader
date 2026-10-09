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
[Guid("3E68D4BD-7135-4D10-8018-9FB6D9F33FA1")]
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
        if (hwnd == IntPtr.Zero)
            throw new InvalidOperationException("The MDReader window handle is unavailable; the file picker cannot be attached to the app window.");

        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".md");
        picker.FileTypeFilter.Add(".markdown");
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        AttachWindowHandle(picker, hwnd);
        var file = await picker.PickSingleFileAsync();
        if (file is null)
            return null;

        SelectedFilePath = file.Path;
        return SelectedFilePath;
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
