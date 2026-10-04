// MIT License - see LICENSE file in the repository root.

using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;

namespace MDReader.App.Shared.Helpers;

/// <summary>
/// Provides Win32 window handle (HWND) retrieval for WinUI 3 windows
/// via COM interop with IWindowNative, without depending on the
/// Win32Interop class which is not available in all Windows App SDK
/// NuGet configurations.
/// </summary>
public static class WindowHandleService
{
    [ComImport]
    [Guid("EECDBF6C-3D3F-4B85-975E-3545DE1B44BE")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IWindowNative
    {
        IntPtr WindowHandle { get; }
    }

    /// <summary>
    /// Gets the HWND (window handle) for the specified WinUI 3 Window.
    /// </summary>
    /// <param name="window">The WinUI 3 Window to get the handle for.</param>
    /// <returns>The HWND as an IntPtr, or IntPtr.Zero on failure.</returns>
    public static IntPtr GetWindowHandle(Window window)
    {
        var iUnknown = Marshal.GetIUnknownForObject(window);
        try
        {
            var iid = typeof(IWindowNative).GUID;
            Marshal.QueryInterface(iUnknown, in iid, out var interfacePtr);
            try
            {
                if (interfacePtr == IntPtr.Zero)
                    return IntPtr.Zero;
                var native = (IWindowNative)Marshal.GetObjectForIUnknown(interfacePtr);
                return native.WindowHandle;
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
