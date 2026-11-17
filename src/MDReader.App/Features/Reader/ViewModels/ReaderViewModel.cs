// MIT License - see LICENSE file in the repository root.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MDReader.Core.Analytics;
using MDReader.Core.Documents;
using MDReader.Core.Drafts;
using MDReader.Core.Localization;
using MDReader.Core.Markdown;
using MDReader.Core.Recent;
using MDReader.Core.Session;
using MDReader.App.Features.Settings.Services;

namespace MDReader.App.Features.Reader.ViewModels;

public enum ReaderViewMode { Split, Editor, Preview }

/// <summary>
/// ViewModel for the document reader — manages multi-tab document sessions,
/// content display, editor view modes, theming, auto-scrolling, and comments.
/// </summary>
public sealed partial class ReaderViewModel : ObservableObject
{
    private string? _currentDocumentPath;
    private string? _documentContent;
    private string? _errorMessage;
    private MarkdownDocumentElement? _elements;
    private bool _isLoading;
    private double _zoomRatio = 1.0;
    private DocumentTabViewModel? _selectedTab;
    private ReaderViewMode _viewMode = ReaderViewMode.Preview;
    private string _currentTheme = "Light";
    private bool _isAutoScrolling;
    private double _scrollSpeed = 2.0;
    private bool _isCommentsPaneOpen;
    private string _newCommentText = string.Empty;
    private string _selectedQuoteText = string.Empty;
    private bool _isOutlinePaneOpen;
    private bool _isPathBarVisible = true;
    private DocumentOutlineItem? _activeOutlineItem;
    private DocumentAnalytics _analytics;
    private bool _isZenMode;

    /// <summary>Collection of active document tabs.</summary>
    public ObservableCollection<DocumentTabViewModel> Tabs { get; } = new();

    /// <summary>Collection of in-document comments.</summary>
    public ObservableCollection<DocumentComment> Comments { get; } = new();

    /// <summary>Collection of headings for the table of contents / outline.</summary>
    public ObservableCollection<DocumentOutlineItem> OutlineItems { get; } = new();

    public bool IsOutlinePaneOpen
    {
        get => _isOutlinePaneOpen;
        set => SetProperty(ref _isOutlinePaneOpen, value);
    }

    public DocumentOutlineItem? ActiveOutlineItem
    {
        get => _activeOutlineItem;
        set => SetProperty(ref _activeOutlineItem, value);
    }

    public DocumentAnalytics Analytics
    {
        get => _analytics;
        private set
        {
            if (SetProperty(ref _analytics, value))
            {
                OnPropertyChanged(nameof(AnalyticsSummary));
                OnPropertyChanged(nameof(WordCountDisplay));
                OnPropertyChanged(nameof(ReadingTimeDisplay));
                OnPropertyChanged(nameof(LineCountDisplay));
            }
        }
    }

    /// <summary>Access to the global multi-language localization service.</summary>
    public LocalizationService Strings => LocalizationService.Instance;

    public ReaderViewModel()
    {
        _currentTheme = UserPreferencesService.Instance.PreferredTheme;
        LocalizationService.Instance.PropertyChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(Strings));
            OnPropertyChanged(nameof(AnalyticsSummary));
            OnPropertyChanged(nameof(WordCountDisplay));
            OnPropertyChanged(nameof(ReadingTimeDisplay));
            OnPropertyChanged(nameof(LineCountDisplay));
        };
    }

    public string AnalyticsSummary => _analytics.FormattedSummary;
    public string WordCountDisplay => $"{_analytics.WordCount:N0} {Strings.StatusWords}";
    public string ReadingTimeDisplay => $"⏱ ~{_analytics.EstimatedReadingMinutes} {Strings.StatusMinRead}";
    public string LineCountDisplay => $"{_analytics.LineCount:N0} {Strings.StatusLines}";

    public bool IsZenMode
    {
        get => _isZenMode;
        set
        {
            if (SetProperty(ref _isZenMode, value))
            {
                OnPropertyChanged(nameof(IsNormalUiVisible));
                OnPropertyChanged(nameof(ZenModeButtonText));
            }
        }
    }

    public bool IsNormalUiVisible => !_isZenMode;
    public string ZenModeButtonText => _isZenMode ? "🗗 Exit Focus" : "🗖 Focus (F11)";

    [RelayCommand]
    public void ToggleOutlinePane()
    {
        IsOutlinePaneOpen = !IsOutlinePaneOpen;
    }

    [RelayCommand]
    public void ToggleZenMode()
    {
        IsZenMode = !IsZenMode;
    }

    public void UpdateOutlineAndAnalytics()
    {
        OutlineItems.Clear();
        var items = MarkdownOutlineExtractor.ExtractOutline(DocumentContent);
        foreach (var item in items)
        {
            OutlineItems.Add(item);
        }

        Analytics = MarkdownOutlineExtractor.ComputeAnalytics(DocumentContent);
    }

    /// <summary>Gets or sets the current view mode (Split, Editor, Preview).</summary>
    public ReaderViewMode ViewMode
    {
        get => _viewMode;
        set
        {
            if (SetProperty(ref _viewMode, value))
            {
                OnPropertyChanged(nameof(IsEditorVisible));
                OnPropertyChanged(nameof(IsPreviewVisible));
                OnPropertyChanged(nameof(IsSplitMode));
                OnPropertyChanged(nameof(IsMarkdownFormattingVisible));
            }
        }
    }

    public bool IsEditorVisible => _viewMode == ReaderViewMode.Split || _viewMode == ReaderViewMode.Editor;
    public bool IsPreviewVisible => _viewMode == ReaderViewMode.Split || _viewMode == ReaderViewMode.Preview;
    public bool IsSplitMode => _viewMode == ReaderViewMode.Split;

    /// <summary>Gets or sets the current theme (Light, Dark, Sepia, System).</summary>
    public string CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (SetProperty(ref _currentTheme, value))
            {
                OnPropertyChanged(nameof(ThemeGlyph));
            }
        }
    }

    public string ThemeGlyph => CurrentTheme switch
    {
        "System" => "💻",
        "Light" => "☀️",
        "Dark" => "🌙",
        "DarkSlate" => "🌑",
        "Sepia" => "📜",
        _ => "💻"
    };

    [RelayCommand]
    public void SetTheme(string theme)
    {
        CurrentTheme = theme;
        UserPreferencesService.Instance.PreferredTheme = theme;
        UserPreferencesService.Instance.Save();
    }

    [RelayCommand]
    public void CycleTheme()
    {
        CurrentTheme = CurrentTheme switch
        {
            "System" => "Light",
            "Light" => "Dark",
            "Dark" => "DarkSlate",
            "DarkSlate" => "Sepia",
            _ => "System"
        };
        UserPreferencesService.Instance.PreferredTheme = CurrentTheme;
        UserPreferencesService.Instance.Save();
    }

    /// <summary>Gets or sets whether Typewriter focus scrolling mode is enabled.</summary>
    public bool IsTypewriterMode
    {
        get => UserPreferencesService.Instance.TypewriterMode;
        set
        {
            if (UserPreferencesService.Instance.TypewriterMode != value)
            {
                UserPreferencesService.Instance.TypewriterMode = value;
                UserPreferencesService.Instance.Save();
                OnPropertyChanged();
                OnPropertyChanged(nameof(TypewriterModeGlyph));
            }
        }
    }

    public string TypewriterModeGlyph => IsTypewriterMode ? "⌨️ [ON]" : "⌨️ [OFF]";

    [RelayCommand]
    public void ToggleTypewriterMode() => IsTypewriterMode = !IsTypewriterMode;

    /// <summary>Gets or sets whether auto-save for drafts is enabled.</summary>
    public bool IsAutoSaveDraftsEnabled
    {
        get => UserPreferencesService.Instance.AutoSaveDrafts;
        set
        {
            if (UserPreferencesService.Instance.AutoSaveDrafts != value)
            {
                UserPreferencesService.Instance.AutoSaveDrafts = value;
                UserPreferencesService.Instance.Save();
                OnPropertyChanged();
            }
        }
    }

    /// <summary>Gets or sets whether restoring session tabs on launch is enabled.</summary>
    public bool IsRestoreSessionEnabled
    {
        get => UserPreferencesService.Instance.RestoreSession;
        set
        {
            if (UserPreferencesService.Instance.RestoreSession != value)
            {
                UserPreferencesService.Instance.RestoreSession = value;
                UserPreferencesService.Instance.Save();
                OnPropertyChanged();
            }
        }
    }

    /// <summary>Gets recent files for quick dashboard and menu display.</summary>
    public IReadOnlyList<RecentFileInfo> RecentFiles => UserPreferencesService.Instance.RecentFiles.GetRecentFiles();

    public void RefreshRecentFiles()
    {
        OnPropertyChanged(nameof(RecentFiles));
    }

    public int SelectedTabIndex => SelectedTab != null ? Tabs.IndexOf(SelectedTab) : -1;

    public void TriggerDraftAutoSave()
    {
        if (!IsAutoSaveDraftsEnabled || SelectedTab == null || !SelectedTab.IsDirty) return;
        try
        {
            var draft = new DraftModel
            {
                DraftId = SelectedTab.Id,
                OriginalFilePath = SelectedTab.FilePath,
                DocumentTitle = SelectedTab.Title,
                Content = SelectedTab.Content,
                SavedAtUtc = DateTime.UtcNow
            };
            DraftManager.SaveDraft(UserPreferencesService.Instance.DraftsDirectory, draft);
        }
        catch { }
    }

    public void ClearCurrentDraft()
    {
        if (SelectedTab == null) return;
        try
        {
            DraftManager.DeleteDraft(UserPreferencesService.Instance.DraftsDirectory, SelectedTab.Id);
        }
        catch { }
    }

    /// <summary>Gets or sets whether auto-scroll is actively running.</summary>
    public bool IsAutoScrolling
    {
        get => _isAutoScrolling;
        set
        {
            if (SetProperty(ref _isAutoScrolling, value))
            {
                OnPropertyChanged(nameof(AutoScrollButtonText));
                OnPropertyChanged(nameof(AutoScrollGlyph));
            }
        }
    }

    /// <summary>Auto-scroll speed multiplier (0.25, 0.5, 1.0, 1.5, 2.0, or custom value).</summary>
    public double ScrollSpeed
    {
        get => _scrollSpeed;
        set
        {
            if (SetProperty(ref _scrollSpeed, Math.Max(0.05, value)))
            {
                OnPropertyChanged(nameof(ScrollSpeedDisplay));
            }
        }
    }

    public string ScrollSpeedDisplay => $"{_scrollSpeed}x";

    /// <summary>Legacy integer seconds accessor for backward compatibility.</summary>
    public int ScrollSpeedSeconds
    {
        get => (int)Math.Max(1, Math.Round(_scrollSpeed));
        set => ScrollSpeed = value;
    }

    public string AutoScrollButtonText => IsAutoScrolling ? "⏸ Pause" : "▶ Auto-Scroll";
    public string AutoScrollGlyph => IsAutoScrolling ? "⏸" : "▶";

    [RelayCommand]
    public void ToggleAutoScroll()
    {
        IsAutoScrolling = !IsAutoScrolling;
    }

    /// <summary>Gets or sets whether the comments drawer is open.</summary>
    public bool IsCommentsPaneOpen
    {
        get => _isCommentsPaneOpen;
        set => SetProperty(ref _isCommentsPaneOpen, value);
    }

    public string NewCommentText
    {
        get => _newCommentText;
        set => SetProperty(ref _newCommentText, value);
    }

    public string SelectedQuoteText
    {
        get => _selectedQuoteText;
        set
        {
            if (SetProperty(ref _selectedQuoteText, value))
            {
                OnPropertyChanged(nameof(HasSelectedQuote));
            }
        }
    }

    public bool HasSelectedQuote => !string.IsNullOrWhiteSpace(_selectedQuoteText);

    [RelayCommand]
    public void ClearQuote()
    {
        SelectedQuoteText = string.Empty;
    }

    [RelayCommand]
    public void ToggleCommentsPane()
    {
        IsCommentsPaneOpen = !IsCommentsPaneOpen;
    }

    /// <summary>Gets or sets whether the informational file path bar is visible.</summary>
    public bool IsPathBarVisible
    {
        get => _isPathBarVisible;
        set => SetProperty(ref _isPathBarVisible, value);
    }

    [RelayCommand]
    public void TogglePathBarVisibility()
    {
        IsPathBarVisible = !IsPathBarVisible;
    }

    [RelayCommand]
    public void AddComment()
    {
        if (string.IsNullOrWhiteSpace(NewCommentText)) return;

        Comments.Add(new DocumentComment
        {
            CommentText = NewCommentText.Trim(),
            SelectedText = SelectedQuoteText.Trim()
        });

        NewCommentText = string.Empty;
        SelectedQuoteText = string.Empty;
    }

    [RelayCommand]
    public void DeleteComment(DocumentComment comment)
    {
        if (comment != null)
            Comments.Remove(comment);
    }

    /// <summary>Gets or sets the active document tab.</summary>
    public DocumentTabViewModel? SelectedTab
    {
        get => _selectedTab;
        set
        {
            if (SetProperty(ref _selectedTab, value))
            {
                if (_selectedTab != null)
                {
                    _currentDocumentPath = _selectedTab.FilePath;
                    _documentContent = _selectedTab.Content;
                    _elements = _selectedTab.Elements;
                    _errorMessage = null;
                }
                else
                {
                    _currentDocumentPath = null;
                    _documentContent = null;
                    _elements = null;
                }

                OnPropertyChanged(nameof(CurrentDocumentPath));
                OnPropertyChanged(nameof(CurrentDocumentPathDisplay));
                OnPropertyChanged(nameof(DocumentContent));
                OnPropertyChanged(nameof(Elements));
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasDocument));
                OnPropertyChanged(nameof(HasError));
                OnPropertyChanged(nameof(IsCurrentDocumentJson));
                OnPropertyChanged(nameof(IsCurrentDocumentMarkdown));
                OnPropertyChanged(nameof(IsMarkdownFormattingVisible));
                UpdateOutlineAndAnalytics();
            }
        }
    }

    /// <summary>Gets or sets the file path of the currently active document.</summary>
    public string? CurrentDocumentPath
    {
        get => _currentDocumentPath;
        set
        {
            if (SetProperty(ref _currentDocumentPath, value))
            {
                OnPropertyChanged(nameof(CurrentDocumentPathDisplay));
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasDocument));
                OnPropertyChanged(nameof(HasError));
                OnPropertyChanged(nameof(IsCurrentDocumentJson));
                OnPropertyChanged(nameof(IsCurrentDocumentMarkdown));
                OnPropertyChanged(nameof(IsMarkdownFormattingVisible));
                UpdateOutlineAndAnalytics();
            }
        }
    }

    /// <summary>Gets the display-friendly file path or default title for informational binding.</summary>
    public string CurrentDocumentPathDisplay =>
        string.IsNullOrWhiteSpace(_currentDocumentPath) ? "Untitled Document" : _currentDocumentPath;

    /// <summary>Gets or sets the raw Markdown text of the active document.</summary>
    public string? DocumentContent
    {
        get => _documentContent;
        set
        {
            if (SetProperty(ref _documentContent, value))
            {
                if (_selectedTab != null && _selectedTab.Content != value)
                {
                    _selectedTab.Content = value ?? string.Empty;
                    _elements = _selectedTab.Elements;
                    OnPropertyChanged(nameof(Elements));
                }
                UpdateOutlineAndAnalytics();
            }
        }
    }

    /// <summary>
    /// Gets the parsed element tree for the active document.
    /// </summary>
    public MarkdownDocumentElement? Elements
    {
        get => _elements;
        private set => SetProperty(ref _elements, value);
    }

    /// <summary>Gets or sets the error message to display.</summary>
    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasDocument));
                OnPropertyChanged(nameof(HasError));
                OnPropertyChanged(nameof(IsCurrentDocumentJson));
                OnPropertyChanged(nameof(IsCurrentDocumentMarkdown));
                OnPropertyChanged(nameof(IsMarkdownFormattingVisible));
                UpdateOutlineAndAnalytics();
            }
        }
    }

    /// <summary>True when no document is loaded or active.</summary>
    public bool IsEmpty => _errorMessage is null && !_isLoading && (Tabs.Count == 0 || (_selectedTab is null && string.IsNullOrEmpty(_currentDocumentPath) && string.IsNullOrEmpty(_documentContent)));

    /// <summary>True when a document is loaded and ready for rendering or editing.</summary>
    public bool HasDocument => _errorMessage is null && !_isLoading && (_selectedTab is not null || !string.IsNullOrEmpty(_currentDocumentPath) || !string.IsNullOrEmpty(_documentContent));

    /// <summary>True when there is an active error to display.</summary>
    public bool HasError => _errorMessage is not null;

    /// <summary>True while a document is being loaded.</summary>
    public bool IsLoading
    {
        get => _isLoading;
        private set
        {
            if (SetProperty(ref _isLoading, value))
            {
                OnPropertyChanged(nameof(IsEmpty));
                OnPropertyChanged(nameof(HasDocument));
                OnPropertyChanged(nameof(HasError));
                OnPropertyChanged(nameof(IsCurrentDocumentJson));
                OnPropertyChanged(nameof(IsCurrentDocumentMarkdown));
                OnPropertyChanged(nameof(IsMarkdownFormattingVisible));
                UpdateOutlineAndAnalytics();
            }
        }
    }

    /// <summary>Gets the current zoom ratio (1.0 = 100%).</summary>
    public double ZoomRatio
    {
        get => _zoomRatio;
        private set
        {
            if (SetProperty(ref _zoomRatio, value))
                OnPropertyChanged(nameof(ZoomPercentage));
        }
    }

    /// <summary>Gets the zoom percentage string for display (e.g., "100%").</summary>
    public string ZoomPercentage => $"{(int)(_zoomRatio * 100)}%";

    [RelayCommand]
    public void SwitchViewMode(string mode)
    {
        if (Enum.TryParse<ReaderViewMode>(mode, true, out var m))
        {
            ViewMode = m;
        }
    }

    [RelayCommand]
    public void InsertMarkdownSyntax(string syntax)
    {
        if (_selectedTab == null) return;
        var insertion = syntax switch
        {
            "bold" => "**Bold Text**",
            "italic" => "*Italic Text*",
            "heading" => "\n\n## New Heading\n\n",
            "code" => "\n\n```csharp\n// Code here\n```\n\n",
            "mermaid" => "\n\n```mermaid\nflowchart TD\n    A[Start] --> B[Process]\n    B --> C[End]\n```\n\n",
            "alert" => "\n\n> [!NOTE]\n> Notification message.\n\n",
            "table" => "\n\n| Part | Topic | Question Range | Total |\n| :--- | :--- | :--- | :--- |\n| Part A | IT Components | Q1 - Q10 | 9 |\n\n",
            "math" => " $E=mc^2$ ",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(insertion))
        {
            DocumentContent = (DocumentContent ?? string.Empty) + insertion;
        }
    }

    /// <summary>Zoom in by 10% (Ctrl++).</summary>
    [RelayCommand]
    private void ZoomIn()
    {
        ZoomRatio = Math.Min(_zoomRatio + 0.1, 3.0);
    }

    /// <summary>Zoom out by 10% (Ctrl+-).</summary>
    [RelayCommand]
    private void ZoomOut()
    {
        ZoomRatio = Math.Max(_zoomRatio - 0.1, 0.5);
    }

    /// <summary>Reset zoom to 100% (Ctrl+0).</summary>
    [RelayCommand]
    private void ResetZoom()
    {
        ZoomRatio = 1.0;
    }

    /// <summary>
    /// Loads a document into a tab (opening a new tab or selecting the existing one).
    /// </summary>
    public void LoadDocument(DocumentInfo info, string content)
    {
        IsLoading = true;
        try
        {
            var existing = Tabs.FirstOrDefault(t => string.Equals(t.FilePath, info.FilePath, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Content = content;
                SelectedTab = existing;
                ErrorMessage = null;
                return;
            }

            var session = DocumentSession.FromFile(info.FilePath, content);
            var tab = new DocumentTabViewModel(session);

            if (Tabs.Count == 1 && string.IsNullOrEmpty(Tabs[0].FilePath) && !Tabs[0].IsDirty && string.IsNullOrEmpty(Tabs[0].Content))
            {
                Tabs[0] = tab;
            }
            else
            {
                Tabs.Add(tab);
            }

            SelectedTab = tab;
            ErrorMessage = null;

            if (!string.IsNullOrEmpty(info.FilePath))
            {
                UserPreferencesService.Instance.RecentFiles.Add(info.FilePath);
                RefreshRecentFiles();
                SaveSessionState();
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void SaveSessionState()
    {
        if (!IsRestoreSessionEnabled) return;
        try
        {
            var openPaths = Tabs
                .Where(t => !string.IsNullOrEmpty(t.FilePath) && System.IO.File.Exists(t.FilePath))
                .Select(t => t.FilePath!)
                .ToList();

            var state = new SessionState
            {
                OpenDocumentPaths = openPaths,
                ActiveDocumentPath = SelectedTab?.FilePath,
                SelectedTabIndex = SelectedTabIndex,
                LastSavedUtc = DateTime.UtcNow
            };
            SessionManager.SaveSession(state, UserPreferencesService.Instance.SessionFilePath);
        }
        catch { }
    }

    /// <summary>
    /// Creates a new draft untitled document in a new tab.
    /// </summary>
    [RelayCommand]
    public void CreateNewDocument()
    {
        var untitledNumber = Tabs.Count + 1;
        var session = DocumentSession.CreateNew($"Untitled-{untitledNumber}.md");
        var tab = new DocumentTabViewModel(session);
        Tabs.Add(tab);
        SelectedTab = tab;
        ErrorMessage = null;
    }

    /// <summary>
    /// Closes the specified tab.
    /// </summary>
    public void CloseTab(DocumentTabViewModel tab)
    {
        var index = Tabs.IndexOf(tab);
        Tabs.Remove(tab);

        if (SelectedTab == tab)
        {
            if (Tabs.Count > 0)
            {
                var nextIndex = Math.Min(index, Tabs.Count - 1);
                SelectedTab = Tabs[nextIndex];
            }
            else
            {
                SelectedTab = null;
                Clear();
            }
        }

        SaveSessionState();
    }

    /// <summary>
    /// Saves the currently active document.
    /// </summary>
    public bool SaveCurrentDocument(string? targetPath = null)
    {
        if (_selectedTab == null)
            return false;

        var savePath = targetPath ?? _selectedTab.FilePath;
        if (string.IsNullOrWhiteSpace(savePath))
            return false;

        var result = MarkdownFileWriter.WriteFile(savePath, _selectedTab.Content);
        if (result.IsSuccess)
        {
            _selectedTab.MarkSaved(savePath, result.SavedAt);
            CurrentDocumentPath = savePath;
            ClearCurrentDraft();
            UserPreferencesService.Instance.RecentFiles.Add(savePath);
            RefreshRecentFiles();
            SaveSessionState();
            return true;
        }

        ShowError($"Failed to save file: {result.ErrorMessage}");
        return false;
    }

    /// <summary>
    /// Shows an error message.
    /// </summary>
    public void ShowError(string message)
    {
        ErrorMessage = message;
    }

    /// <summary>Find text within the current document.</summary>
    public string FindText { get; set; } = string.Empty;

    /// <summary>Clear all document and error state.</summary>
    public void Clear()
    {
        _currentDocumentPath = null;
        _documentContent = null;
        _elements = null;
        _errorMessage = null;
        OnPropertyChanged(nameof(CurrentDocumentPath));
        OnPropertyChanged(nameof(DocumentContent));
        OnPropertyChanged(nameof(Elements));
        OnPropertyChanged(nameof(IsEmpty));
        OnPropertyChanged(nameof(HasDocument));
        OnPropertyChanged(nameof(HasError));
    }
}
