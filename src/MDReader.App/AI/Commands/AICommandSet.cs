// MIT License - see LICENSE file in the repository root.

using MDReader.App.AI.Contracts;

namespace MDReader.App.AI.Commands;

/// <summary>
/// Registry of available AI commands for the application.
/// This defines the command vocabulary that the AI control surface exposes.
/// </summary>
/// <remarks>
/// The command set is the bridge between the AI interface (docs/AI_INTERFACE.md)
/// and the application's internal command layer (ICommandGateway).
/// Each command maps to a method on ICommandGateway.
/// </remarks>
public static class AICommandSet
{
    /// <summary>
    /// Gets the list of all supported AI command names.
    /// </summary>
    public static IReadOnlyList<string> GetAllCommands() => new[]
    {
        "OpenDocument",
        "CloseDocument",
        "GetCurrentDocument",
        "GetDocumentMetadata",
        "ChangeTheme",
        "NavigateTo",
        "GetApplicationState",
        "ListSupportedCommands"
    };

    /// <summary>
    /// Gets metadata for a specific command by name.
    /// Returns null if the command is not recognized.
    /// </summary>
    public static CommandMetadata? GetCommandMetadata(string command) =>
        command switch
        {
            "OpenDocument" => new("OpenDocument",
                "Opens a Markdown document from the specified file path.",
                new[] { "path" }),
            "CloseDocument" => new("CloseDocument",
                "Closes the currently open document.",
                Array.Empty<string>()),
            "GetCurrentDocument" => new("GetCurrentDocument",
                "Returns the content of the currently open document.",
                Array.Empty<string>()),
            "GetDocumentMetadata" => new("GetDocumentMetadata",
                "Returns metadata about the currently open document.",
                Array.Empty<string>()),
            "ChangeTheme" => new("ChangeTheme",
                "Changes the application theme to the specified value.",
                new[] { "theme" }),
            "NavigateTo" => new("NavigateTo",
                "Navigates to a specific application feature/screen.",
                new[] { "feature" }),
            "GetApplicationState" => new("GetApplicationState",
                "Returns the current application state (open document, theme, etc.).",
                Array.Empty<string>()),
            "ListSupportedCommands" => new("ListSupportedCommands",
                "Lists all commands supported by the AI interface.",
                Array.Empty<string>()),
            _ => null
        };
}
