using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;

using MDReader.App.Features.Shell.ViewModels;
using MDReader.App.Features.Shell.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MDReader.App;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static Window? MainWindow { get; private set; }
    private Window? _window;
    
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();

        UnhandledException += (sender, e) =>
        {
            e.Handled = true;
            LogDiagnosticCrash("CRASH_XAML", e.Message, e.Exception);
        };

        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            e.SetObserved();
            LogDiagnosticCrash("UNOBSERVED_TASK", e.Exception?.Message ?? "Unobserved task fault", e.Exception);
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            LogDiagnosticCrash("APPDOMAIN_UNHANDLED", e.ExceptionObject?.ToString() ?? "AppDomain fault", e.ExceptionObject as Exception);
        };
    }

    private static void LogDiagnosticCrash(string category, string message, Exception? ex)
    {
        try
        {
            var logPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MDReader", "crash.log");
            var dir = System.IO.Path.GetDirectoryName(logPath);
            if (!string.IsNullOrEmpty(dir)) System.IO.Directory.CreateDirectory(dir);
            System.IO.File.AppendAllText(logPath, $"[{category}] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}\n{ex}\n{ex?.StackTrace}\nInner: {ex?.InnerException}\n---\n");
        }
        catch { }
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        try
        {
            _window = new MainWindow();
            MainWindow = _window;
            _window.Activate();
        }
        catch (Exception ex)
        {
            try
            {
                var logPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MDReader", "launch_error.log");
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath)!);
                System.IO.File.WriteAllText(logPath, $"[LAUNCH ERROR] {DateTime.Now}: {ex.Message}\n{ex}\n{ex.StackTrace}\nInner: {ex.InnerException}");
            }
            catch { }
            throw;
        }
    }
}
