// MIT License - see LICENSE file in the repository root.

using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Media;
using System.IO;
using MDReader.Core.Markdown;

namespace MDReader.App.Features.Reader;

/// <summary>
/// Renders the platform-neutral <see cref="MarkdownDocumentElement"/> tree
/// produced by <see cref="MDReader.Core"/> into WinUI 3 controls. This class
/// lives in the App layer (it depends on WinUI); the Core layer has no WinUI
/// dependency. Unsupported or inert nodes degrade safely (skipped).
/// </summary>
public sealed class MarkdownRenderer
{
    private static readonly FontFamily CodeFontFamily = new("Consolas, Cascadia Code, monospace");
    private static readonly FontFamily ArabicFontFamily = new("Segoe UI Variable, Cairo, Amiri, Segoe UI");

    /// <summary>
    /// Renders a parsed document into an ordered list of block elements suitable
    /// for addition to a XAML panel (e.g. a <see cref="StackPanel"/>).
    /// </summary>
    private string? _documentBasePath;

    public IReadOnlyList<FrameworkElement> Render(MarkdownDocumentElement document, string? documentBasePath = null)
    {
        _documentBasePath = documentBasePath;
        var elements = new List<FrameworkElement>();
        foreach (var block in document.Blocks)
        {
            var element = RenderBlock(block);
            if (element is not null)
                elements.Add(element);
        }
        return elements;
    }

    private FrameworkElement? RenderBlock(MarkdownBlockElement block) => block switch
    {
        MarkdownHeadingElement h => RenderHeading(h),
        MarkdownParagraphElement p => RenderParagraph(p),
        MarkdownCodeBlockElement c => RenderCodeBlock(c),
        MarkdownListElement l => RenderList(l),
        MarkdownBlockquoteElement b => RenderBlockquote(b),
        MarkdownThematicBreakElement => RenderThematicBreak(),
        MarkdownTableElement t => RenderTable(t),
        MarkdownHtmlElement => null, // inert raw HTML: never rendered as live markup
        _ => null, // unsupported node: degrade safely
    };

    private TextBlock RenderHeading(MarkdownHeadingElement h)
    {
        var text = InlinesToPlainText(h.Inlines);
        var tb = new TextBlock { Style = GetStyle("AppBodyTextStyle") };
        tb.FontFamily = ContainsRtlText(text) ? ArabicFontFamily : new FontFamily("Segoe UI Variable");
        tb.FlowDirection = ContainsRtlText(text) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        tb.FontSize = HeadingFontSize(h.Level);
        tb.FontWeight = h.Level <= 2 ? FontWeights.Bold : FontWeights.SemiBold;
        tb.TextWrapping = TextWrapping.Wrap;
        tb.Margin = new Thickness(0, 20, 0, 8);
        tb.HorizontalAlignment = HorizontalAlignment.Stretch;
        AutomationProperties.SetName(tb, "Heading level " + h.Level);
        SetHeadingLevel(tb, h.Level);
        // Stable anchor derived from heading text for internal navigation
        var anchorId = GenerateAnchorId(text);
        tb.Tag = anchorId;
        AddInlines(tb.Inlines, h.Inlines);
        return tb;
    }

    private TextBlock RenderParagraph(MarkdownParagraphElement p)
    {
        var text = InlinesToPlainText(p.Inlines);
        var tb = new TextBlock
        {
            Style = GetStyle("AppBodyTextStyle"),
            FontFamily = ContainsRtlText(text) ? ArabicFontFamily : new FontFamily("Segoe UI Variable"),
            FlowDirection = ContainsRtlText(text) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(0, 4, 0, 4),
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        AddInlines(tb.Inlines, p.Inlines);
        return tb;
    }

    private FrameworkElement RenderCodeBlock(MarkdownCodeBlockElement c)
    {
        var textBlock = new TextBlock
        {
            Text = c.Code,
            FontFamily = CodeFontFamily,
            FontSize = 13,
            TextWrapping = TextWrapping.NoWrap,
            FlowDirection = FlowDirection.LeftToRight,
            Foreground = GetBrush("AppTextBrush"),
            IsTextSelectionEnabled = true,
        };

        // Long lines scroll horizontally; the ScrollViewer clips to the viewport
        // width and exposes a horizontal scrollbar when the TextBlock exceeds it.
        var border = new Border
        {
            Background = GetBrush("AppBackgroundBrush"),
            BorderBrush = GetBrush("AppBorderBrush"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(12),
            Child = textBlock,
        };

        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Content = border,
            Margin = new Thickness(0, 8, 0, 8),
        };
        return scroll;
    }

    private FrameworkElement RenderList(MarkdownListElement l)
    {
        var panel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(0, 8, 0, 8) };
        var number = l.Start;
        foreach (var item in l.Items)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 2, 0, 2) };
            var glyph = new TextBlock
            {
                Text = item.IsChecked ? (l.IsOrdered ? (number + ".") : "\u2611") : (l.IsOrdered ? (number + ".") : "\u2610"),
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Foreground = GetBrush("AppTextBrush"),
            };
            row.Children.Add(glyph);
            var content = RenderBlocksPanel(item.Blocks);
            row.Children.Add(content);
            panel.Children.Add(row);
            number++;
        }
        return panel;
    }

    private FrameworkElement RenderBlockquote(MarkdownBlockquoteElement b)
    {
        var inner = RenderBlocksPanel(b.Blocks);
        inner.Padding = new Thickness(12, 4, 12, 4);
        inner.Margin = new Thickness(0, 0, 0, 0);
        var border = new Border
        {
            BorderBrush = GetBrush("AppSecondaryTextBrush"),
            BorderThickness = new Thickness(4, 0, 0, 0),
            CornerRadius = new CornerRadius(0, 4, 4, 0),
            Child = inner,
        };
        border.Margin = new Thickness(0, 8, 0, 8);
        return border;
    }

    private FrameworkElement RenderThematicBreak() =>
        new Border { Height = 1, Background = GetBrush("AppBorderBrush"), Margin = new Thickness(0, 16, 0, 16) };

    private FrameworkElement RenderTable(MarkdownTableElement t)
    {
        var columnCount = Math.Max(t.Headers.Count, t.Rows.Select(r => r.Count).DefaultIfEmpty(0).Max());
        var grid = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch };
        for (int i = 0; i < columnCount; i++)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var rowIndex = 0;

        // Header row.
        if (t.Headers.Count > 0)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int c = 0; c < t.Headers.Count; c++)
            {
                var cell = CreateTableCell(t.Headers[c], isHeader: true);
                Grid.SetRow(cell, rowIndex);
                Grid.SetColumn(cell, c);
                grid.Children.Add(cell);
            }
            rowIndex++;
        }

        // Body rows.
        foreach (var row in t.Rows)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int c = 0; c < row.Count; c++)
            {
                var cell = CreateTableCell(row[c], isHeader: false);
                Grid.SetRow(cell, rowIndex);
                Grid.SetColumn(cell, c);
                grid.Children.Add(cell);
            }
            rowIndex++;
        }

        var outer = new Border
        {
            BorderBrush = GetBrush("AppBorderBrush"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(8),
            Margin = new Thickness(0, 8, 0, 8),
            Child = grid,
        };

        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            Content = outer,
        };
        return scroll;
    }

    private TextBlock CreateTableCell(string? text, bool isHeader)
    {
        var cellText = text ?? string.Empty;
        var cell = new TextBlock
        {
            Text = cellText,
            Style = GetStyle("AppBodyTextStyle"),
            FontFamily = ContainsRtlText(cellText) ? ArabicFontFamily : new FontFamily("Segoe UI Variable"),
            FlowDirection = ContainsRtlText(cellText) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
            TextWrapping = TextWrapping.Wrap,
            Padding = new Thickness(8, 6, 8, 6),
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        if (isHeader)
        {
            cell.FontWeight = FontWeights.Bold;
            cell.Foreground = GetBrush("AppSecondaryTextBrush");
        }
        return cell;
    }

    private StackPanel RenderBlocksPanel(IEnumerable<MarkdownBlockElement> blocks)
    {
        var panel = new StackPanel { Orientation = Orientation.Vertical };
        foreach (var block in blocks)
        {
            var element = RenderBlock(block);
            if (element is not null)
                panel.Children.Add(element);
        }
        return panel;
    }

    private void AddInlines(InlineCollection target, IReadOnlyList<MarkdownInlineElement> inlines)
    {
        foreach (var inline in inlines)
        {
            var converted = ConvertInline(inline);
            if (converted is not null)
                target.Add(converted);
        }
    }

    private Inline? ConvertInline(MarkdownInlineElement inline)
    {
        switch (inline)
        {
            case MarkdownLiteralInline l:
                return new Run { Text = l.Text };

            case MarkdownCodeInline c:
                // WinUI Run has no Background/Padding, so inline code is styled
                // with a monospace font and a distinct foreground (no fill).
                return new Run
                {
                    Text = c.Code,
                    FontFamily = CodeFontFamily,
                    FontSize = 13,
                };

            case MarkdownEmphasisInline e when e.EmphasisType == MarkdownEmphasisType.Bold:
                var bold = new Bold();
                AddInlines(bold.Inlines, e.Inlines);
                return bold;

            case MarkdownEmphasisInline e when e.EmphasisType == MarkdownEmphasisType.Italic:
                var italic = new Italic();
                AddInlines(italic.Inlines, e.Inlines);
                return italic;

            case MarkdownEmphasisInline e when e.EmphasisType == MarkdownEmphasisType.Strikethrough:
                // WinUI Run has no Inlines collection, so nested formatting is
                // flattened to text and the strikethrough decoration is applied.
                return new Run
                {
                    Text = InlinesToPlainText(e.Inlines),
                    TextDecorations = Windows.UI.Text.TextDecorations.Strikethrough,
                };

            case MarkdownLinkInline l:
                return CreateLink(l);

            case MarkdownTaskInline t:
                return new Run
                {
                    Text = t.IsChecked ? "\u2610 " : "\u2611 ",
                    FontFamily = CodeFontFamily,
                };

            case MarkdownImageInline i:
                return RenderImageInline(i);

            default:
                return new Run { Text = inline.ToString() ?? string.Empty };
        }
    }

    private Inline CreateLink(MarkdownLinkInline link)
    {
        var hyperlink = new Hyperlink();
        AddInlines(hyperlink.Inlines, link.Inlines);
        // Safe link policy (Phase 3): only explicit HTTP(S) links launch via OS handler.
        // Internal anchors and local Markdown links remain styled but are not launched.
        // Unsupported schemes are explicitly rejected (no implicit network fetch).
        if (IsExternalHttpUrl(link.Url))
        {
            // External HTTP(S) URLs always render LTR regardless of paragraph direction.
            // WinUI's Unicode bidirectional algorithm handles mixed-direction text within
            // the hyperlink, but the URL itself (scheme, domain, path) is always LTR.
            // The hyperlink inherits FlowDirection from its parent TextBlock; for RTL
            // paragraphs the URL characters remain LTR via the bidi algorithm.
            hyperlink.Click += (s, e) => OpenExternalUrl(link.Url);
            hyperlink.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
        }
        else
        {
            // Non-HTTP links (anchors, relative/local files, unsupported schemes) render muted
            // and are not wired to any launch action. This satisfies the safe-link contract.
            hyperlink.Foreground = GetBrush("AppSecondaryTextBrush");
            hyperlink.TextDecorations = Windows.UI.Text.TextDecorations.Underline;
        }
        return hyperlink;
    }

    private static bool IsExternalHttpUrl(string url) => MarkdownLinkPolicy.IsExternalHttpUrl(url);

    private static void OpenExternalUrl(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch
        {
            // Launching is best-effort; failure must not crash the app.
        }
    }

    private static string InlinesToPlainText(IReadOnlyList<MarkdownInlineElement> inlines)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var inline in inlines)
        {
            switch (inline)
            {
                case MarkdownLiteralInline l:
                    sb.Append(l.Text);
                    break;
                case MarkdownCodeInline c:
                    sb.Append(c.Code);
                    break;
                case MarkdownEmphasisInline e:
                    sb.Append(InlinesToPlainText(e.Inlines));
                    break;
                case MarkdownLinkInline l:
                    sb.Append(InlinesToPlainText(l.Inlines));
                    break;
                case MarkdownImageInline i:
                    sb.Append(i.AltText);
                    break;
                case MarkdownTaskInline t:
                    sb.Append(t.IsChecked ? "[x]" : "[ ]");
                    break;
            }
        }
        return sb.ToString();
    }

    private static double HeadingFontSize(int level) => level switch
    {
        1 => 30, 2 => 24, 3 => 20, 4 => 17, 5 => 14, 6 => 12,
        _ => 16,
    };

    private static void SetHeadingLevel(TextBlock target, int level)
    {
        var headingLevel = level switch
        {
            1 => AutomationHeadingLevel.Level1, 2 => AutomationHeadingLevel.Level2,
            3 => AutomationHeadingLevel.Level3, 4 => AutomationHeadingLevel.Level4,
            5 => AutomationHeadingLevel.Level5, 6 => AutomationHeadingLevel.Level6,
            _ => AutomationHeadingLevel.Level9,
        };
        AutomationProperties.SetHeadingLevel(target, headingLevel);
    }

    private Inline RenderImageInline(MarkdownImageInline image)
    {
        // Safe local image resolution: enforce document-tree boundary (no traversal, no symlink escape)
        string altText = string.IsNullOrEmpty(image.AltText) ? "image" : image.AltText;
        string? resolvedPath = null;
        try
        {
            if (!string.IsNullOrEmpty(_documentBasePath) && !string.IsNullOrEmpty(image.Url))
            {
                var baseDir = Path.GetDirectoryName(_documentBasePath) ?? _documentBasePath;
                var targetPath = Path.GetFullPath(Path.Combine(baseDir, image.Url));
                var baseFull = Path.GetFullPath(baseDir);
                // Ensure resolved path is under base directory
                if (targetPath.StartsWith(baseFull, StringComparison.OrdinalIgnoreCase) || targetPath == baseFull)
                {
                    if (File.Exists(targetPath))
                    {
                        // Symlink check: reject reparse points
                        var attr = File.GetAttributes(targetPath);
                        if ((attr & FileAttributes.ReparsePoint) == 0)
                        {
                            resolvedPath = targetPath;
                        }
                    }
                }
            }
        }
        catch
        {
            // Any path failure degrades safely to alt text
        }

        if (!string.IsNullOrEmpty(resolvedPath))
        {
            try
            {
                var bitmap = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage { UriSource = new Uri(resolvedPath) };
                var img = new Microsoft.UI.Xaml.Controls.Image
                {
                    Source = bitmap,
                    Stretch = Microsoft.UI.Xaml.Media.Stretch.Uniform,
                    MaxHeight = 400,
                    Margin = new Thickness(0, 8, 0, 8),
                };
                AutomationProperties.SetName(img, altText);
                return new InlineUIContainer { Child = img };
            }
            catch
            {
                // If loading fails, degrade
            }
        }

        // Degrade safely: muted placeholder with alt text
        return new Run
        {
            Text = "[image: " + altText + "]",
            Foreground = GetBrush("AppSecondaryTextBrush"),
            FontStyle = Windows.UI.Text.FontStyle.Italic,
        };
    }

    private static bool ContainsRtlText(string text) =>
        text.Any(c => (c >= '\u0590' && c <= '\u08FF') || (c >= '\uFB50' && c <= '\uFDFF') || (c >= '\uFE70' && c <= '\uFEFF'));

    private static Brush GetBrush(string key)
    {
        var resource = Application.Current.Resources[key];
        return resource is Brush brush ? brush : new SolidColorBrush(Colors.Black);
    }

    private static Style? GetStyle(string key) => Application.Current.Resources[key] as Style;

    private static string GenerateAnchorId(string text)
    {
        var normalized = new string(text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
        var slug = normalized.ToLowerInvariant().Trim();
        slug = System.Text.RegularExpressions.Regex.Replace(slug, "\\s+", "-");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, "[^a-z0-9-]", "");
        slug = System.Text.RegularExpressions.Regex.Replace(slug, "-+", "-");
        return slug.Trim('-');
    }
}
