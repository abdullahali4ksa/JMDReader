// MIT License - see LICENSE file in the repository root.

using System;
using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MDReader.App.Features.Shell.ViewModels;

/// <summary>
/// Represents a node in the workspace folder TreeView (either a folder or a Markdown file).
/// </summary>
public sealed class FolderNodeViewModel : ObservableObject
{
    public string Name { get; init; } = string.Empty;
    public string FullPath { get; init; } = string.Empty;
    public bool IsDirectory { get; init; }

    /// <summary>FontIcon Glyph: Folder or Document icon from Segoe Fluent Icons.</summary>
    public string Glyph => IsDirectory ? "\uED25" : "\uE8A5";

    public ObservableCollection<FolderNodeViewModel> Children { get; } = new();

    public static FolderNodeViewModel FromDirectory(string dirPath)
    {
        var dirInfo = new DirectoryInfo(dirPath);
        var root = new FolderNodeViewModel
        {
            Name = dirInfo.Name,
            FullPath = dirInfo.FullName,
            IsDirectory = true
        };

        PopulateChildren(root, dirInfo);
        return root;
    }

    private static void PopulateChildren(FolderNodeViewModel parentNode, DirectoryInfo dirInfo)
    {
        try
        {
            foreach (var subDir in dirInfo.GetDirectories())
            {
                if (subDir.Name.StartsWith(".") || (subDir.Attributes & FileAttributes.Hidden) != 0)
                    continue;

                var childDirNode = new FolderNodeViewModel
                {
                    Name = subDir.Name,
                    FullPath = subDir.FullName,
                    IsDirectory = true
                };

                PopulateChildren(childDirNode, subDir);

                // Add directory if it has markdown files or valid subdirectories
                if (childDirNode.Children.Count > 0)
                    parentNode.Children.Add(childDirNode);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                var ext = file.Extension.ToLowerInvariant();
                if (ext == ".md" || ext == ".markdown")
                {
                    parentNode.Children.Add(new FolderNodeViewModel
                    {
                        Name = file.Name,
                        FullPath = file.FullName,
                        IsDirectory = false
                    });
                }
            }
        }
        catch
        {
            // Graceful handling of locked or unauthorized folders
        }
    }
}
