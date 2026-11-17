// MIT License - see LICENSE file in the repository root.

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using MDReader.Core.Documents;
using MDReader.Core.Markdown;

namespace MDReader.App.Features.Reader.ViewModels;

/// <summary>
/// Represents a single open document tab, encapsulating its session,
/// dirty state, header with dirty indicator, and parsed AST elements.
/// </summary>
public sealed partial class DocumentTabViewModel : ObservableObject
{
    private readonly DocumentSession _session;
    private MarkdownDocumentElement? _elements;
    private bool _isPinned;

    public DocumentSession Session => _session;

    public string Id => _session.Id;

    public bool IsPinned
    {
        get => _isPinned;
        set
        {
            if (SetProperty(ref _isPinned, value))
            {
                OnPropertyChanged(nameof(Header));
                OnPropertyChanged(nameof(CanClose));
                OnPropertyChanged(nameof(TabGlyph));
                OnPropertyChanged(nameof(PinMenuText));
            }
        }
    }

    public bool CanClose => !IsPinned;

    public string TabGlyph => IsPinned ? "\uE840" : (IsJsonDocument ? "\uE943" : "\uE8A5");

    public string PinMenuText => IsPinned ? "📌 Unpin Tab / إلغاء تثبيت التاب" : "📌 Pin Tab / تثبيت في المفضلة";

    public string Header => (IsPinned ? "📌 " : "") + _session.Title + (_session.IsDirty ? " *" : "");

    public string? FilePath => _session.FilePath;

    public string Title => _session.Title;

    public string Content
    {
        get => _session.CurrentContent;
        set
        {
            if (_session.CurrentContent != value)
            {
                _session.UpdateContent(value);
                OnPropertyChanged(nameof(Content));
                OnPropertyChanged(nameof(IsDirty));
                OnPropertyChanged(nameof(Header));
                UpdateParsedElements();
            }
        }
    }

    public bool IsDirty => _session.IsDirty;

    public bool IsJsonDocument =>
        !string.IsNullOrEmpty(Title) &&
        (Title.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
         Title.EndsWith(".jsonc", StringComparison.OrdinalIgnoreCase) ||
         Title.EndsWith(".jsonl", StringComparison.OrdinalIgnoreCase));

    public bool IsMarkdownDocument => !IsJsonDocument;

    public MarkdownDocumentElement? Elements
    {
        get => _elements;
        private set => SetProperty(ref _elements, value);
    }

    public DocumentTabViewModel(DocumentSession session)
    {
        _session = session;
        UpdateParsedElements();
    }

    public void UpdateParsedElements()
    {
        if (string.IsNullOrEmpty(_session.CurrentContent))
        {
            Elements = null;
            return;
        }

        try
        {
            var parsed = new MarkdownDocument { SourceText = _session.CurrentContent }.Parse();
            Elements = parsed.Document.Elements;
        }
        catch
        {
            Elements = null;
        }
    }

    public void MarkSaved(string filePath, DateTimeOffset savedAt)
    {
        _session.MarkSaved(filePath, savedAt);
        OnPropertyChanged(nameof(FilePath));
        OnPropertyChanged(nameof(Title));
        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(Header));
    }
}
