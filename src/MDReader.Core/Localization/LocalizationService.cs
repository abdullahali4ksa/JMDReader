// MIT License - see LICENSE file in the repository root.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MDReader.Core.Localization;

/// <summary>
/// Provides pure multi-language localization for JMDReader supporting the top 10 global languages:
/// Arabic (العربية), English, Spanish (Español), Hindi (हिन्दी), Chinese (中文 简体),
/// French (Français), German (Deutsch), Russian (Русский), Japanese (日本語), Portuguese (Português).
/// Follows pure i18n rules without compound slash labels (no "File / ملف").
/// </summary>
public sealed class LocalizationService : INotifyPropertyChanged
{
    private static readonly Lazy<LocalizationService> _instance = new(() => new LocalizationService());
    public static LocalizationService Instance => _instance.Value;

    public event PropertyChangedEventHandler? PropertyChanged;

    private string _currentLanguage = "ar"; // Default Arabic as requested

    public static readonly IReadOnlyList<(string Code, string Name, string NativeName)> SupportedLanguages = new[]
    {
        ("ar", "Arabic", "العربية"),
        ("en", "English", "English"),
        ("es", "Spanish", "Español"),
        ("hi", "Hindi", "हिन्दी"),
        ("zh", "Chinese", "中文 (简体)"),
        ("fr", "French", "Français"),
        ("de", "German", "Deutsch"),
        ("ru", "Russian", "Русский"),
        ("ja", "Japanese", "日本語"),
        ("pt", "Portuguese", "Português"),
    };

    public string CurrentLanguage
    {
        get => _currentLanguage;
        set => SetLanguage(value);
    }

    public bool IsRtl => _currentLanguage == "ar";

    public LocalizationService()
    {
    }

    public void SetLanguage(string? langCode)
    {
        if (string.IsNullOrWhiteSpace(langCode))
            langCode = "ar";

        langCode = langCode.ToLowerInvariant().Trim();
        if (!_translations.ContainsKey(langCode))
            langCode = "en"; // Fallback to English

        if (_currentLanguage != langCode)
        {
            _currentLanguage = langCode;
            OnPropertyChanged(string.Empty); // Refresh all bound properties
        }
    }

    public string Get(string key, string? fallback = null)
    {
        if (_translations.TryGetValue(_currentLanguage, out var dict) && dict.TryGetValue(key, out var val))
            return val;

        if (_translations.TryGetValue("en", out var enDict) && enDict.TryGetValue(key, out var enVal))
            return enVal;

        return fallback ?? key;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Strongly-typed accessors for WinUI XAML data-binding
    public string MenuFile => Get("MenuFile", "File");
    public string MenuNew => Get("MenuNew", "New  (Ctrl+N)");
    public string MenuOpenFile => Get("MenuOpenFile", "Open File...  (Ctrl+O)");
    public string MenuOpenFolder => Get("MenuOpenFolder", "Open Folder...  (Ctrl+Shift+O)");
    public string MenuRecentFiles => Get("MenuRecentFiles", "Recent Files");
    public string MenuSave => Get("MenuSave", "Save  (Ctrl+S)");
    public string MenuSaveAs => Get("MenuSaveAs", "Save As...  (Ctrl+Shift+S)");
    public string MenuCloseTab => Get("MenuCloseTab", "Close Tab  (Ctrl+W)");

    public string MenuView => Get("MenuView", "View");
    public string MenuCommandPalette => Get("MenuCommandPalette", "Command Palette...  (Ctrl+Shift+P / F2)");
    public string MenuToggleSidebar => Get("MenuToggleSidebar", "Toggle Sidebar  (Ctrl+B)");
    public string MenuTypewriter => Get("MenuTypewriter", "Typewriter Mode");
    public string MenuRefresh => Get("MenuRefresh", "Refresh  (Ctrl+R)");
    public string MenuFind => Get("MenuFind", "Find in File...  (Ctrl+F)");
    public string MenuReplace => Get("MenuReplace", "Replace...  (Ctrl+H)");

    public string MenuSettings => Get("MenuSettings", "Settings");
    public string MenuTheme => Get("MenuTheme", "Theme");
    public string MenuThemeSystem => Get("MenuThemeSystem", "System");
    public string MenuThemeLight => Get("MenuThemeLight", "Light");
    public string MenuThemeDark => Get("MenuThemeDark", "Dark");
    public string MenuThemeDarkSlate => Get("MenuThemeDarkSlate", "Dark Slate");
    public string MenuThemeSepia => Get("MenuThemeSepia", "Sepia");
    public string MenuPreferences => Get("MenuPreferences", "Preferences & Automation");
    public string MenuAutoSave => Get("MenuAutoSave", "Auto-Save Drafts");
    public string MenuRestoreSession => Get("MenuRestoreSession", "Restore Last Session Tabs");
    public string MenuLanguage => Get("MenuLanguage", "Language");

    public string MenuHelp => Get("MenuHelp", "Help");
    public string MenuMarkdownGuide => Get("MenuMarkdownGuide", "Markdown Guide");
    public string MenuJsonGuide => Get("MenuJsonGuide", "JSON & JSONPath Guide");
    public string MenuShortcuts => Get("MenuShortcuts", "Keyboard Shortcuts  (F1)");
    public string MenuAbout => Get("MenuAbout", "About JMDReader");

    public string TabPin => Get("TabPin", "Pin Tab");
    public string TabUnpin => Get("TabUnpin", "Unpin Tab");
    public string TabFavorite => Get("TabFavorite", "Add to Favorites");
    public string TabClose => Get("TabClose", "Close Tab  (Ctrl+W)");
    public string TabCloseRight => Get("TabCloseRight", "Close Tabs to the Right");
    public string TabCloseOthers => Get("TabCloseOthers", "Close Other Tabs");
    public string TabCloseAll => Get("TabCloseAll", "Close All Tabs");

    public string JsonExpandAll => Get("JsonExpandAll", "Expand All");
    public string JsonCollapseAll => Get("JsonCollapseAll", "Collapse All");
    public string JsonFormat => Get("JsonFormat", "Format");
    public string JsonMinify => Get("JsonMinify", "Minify");
    public string JsonTable => Get("JsonTable", "Table");
    public string JsonExport => Get("JsonExport", "Export");
    public string JsonCopy => Get("JsonCopy", "Copy");

    public string TooltipSplit => Get("TooltipSplit", "Split View");
    public string TooltipEditor => Get("TooltipEditor", "Editor Only");
    public string TooltipRead => Get("TooltipRead", "Reading Mode");

    public string TooltipBold => Get("TooltipBold", "Bold (**text**)");
    public string TooltipItalic => Get("TooltipItalic", "Italic (*text*)");
    public string TooltipHeading => Get("TooltipHeading", "Heading (# Title)");
    public string TooltipCode => Get("TooltipCode", "Code Block (```)");
    public string TooltipMermaid => Get("TooltipMermaid", "Mermaid Diagram");
    public string TooltipAlert => Get("TooltipAlert", "Alert Callout ([!NOTE])");
    public string TooltipMath => Get("TooltipMath", "Math Formula ($$)");

    public string TooltipOutline => Get("TooltipOutline", "Outline");
    public string TooltipAutoScroll => Get("TooltipAutoScroll", "Auto-Scroll");
    public string TooltipAutoScrollSpeed => Get("TooltipAutoScrollSpeed", "Auto-Scroll Speed (1 - 5)");
    public string TooltipThemeToggle => Get("TooltipThemeToggle", "Cycle Theme");
    public string TooltipComments => Get("TooltipComments", "Comments");
    public string TooltipExportPdf => Get("TooltipExportPdf", "Export to PDF");
    public string TooltipExportHtml => Get("TooltipExportHtml", "Export to HTML");
    public string TooltipFocus => Get("TooltipFocus", "Focus Mode (F11)");

    public string TooltipZoomIn => Get("TooltipZoomIn", "Zoom In (Ctrl++)");
    public string TooltipZoomOut => Get("TooltipZoomOut", "Zoom Out (Ctrl+-)");
    public string TooltipResetZoom => Get("TooltipResetZoom", "Reset Zoom (Ctrl+0)");

    public string TooltipCopyPath => Get("TooltipCopyPath", "Copy file path");
    public string TooltipHidePath => Get("TooltipHidePath", "Hide path bar");
    public string TooltipShowPath => Get("TooltipShowPath", "Show path bar");
    public string TextPathCopied => Get("TextPathCopied", "✓ Path Copied");

    public string SearchPlaceholder => Get("SearchPlaceholder", "Quick search or JSONPath (Ctrl+F)...");
    public string NoRecentFiles => Get("NoRecentFiles", "No recent files");
    public string ClearHistory => Get("ClearHistory", "Clear History");
    public string DashboardFavoritesTitle => Get("DashboardFavoritesTitle", "FAVORITES & RECENT");
    public string CopyAll => Get("CopyAll", "Copy All Text  (Ctrl+C)");

    public string StatusWords => Get("StatusWords", "words");
    public string StatusLines => Get("StatusLines", "lines");
    public string StatusMinRead => Get("StatusMinRead", "min read");

    public string DrawerOutlineTitle => Get("DrawerOutlineTitle", "DOCUMENT OUTLINE");
    public string DrawerCommentsTitle => Get("DrawerCommentsTitle", "NOTES & COMMENTS");
    public string CommentPlaceholder => Get("CommentPlaceholder", "Write a note or comment...");
    public string ButtonAddComment => Get("ButtonAddComment", "+ Add Comment");
    public string ButtonClearAll => Get("ButtonClearAll", "Clear All");

    private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        // 1. Arabic (العربية - Default)
        ["ar"] = new()
        {
            ["MenuFile"] = "ملف",
            ["MenuNew"] = "جديد  (Ctrl+N)",
            ["MenuOpenFile"] = "فتح ملف...  (Ctrl+O)",
            ["MenuOpenFolder"] = "فتح مجلد...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "الملفات المشاهدة مؤخراً",
            ["MenuSave"] = "حفظ  (Ctrl+S)",
            ["MenuSaveAs"] = "حفظ باسم...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "إغلاق التاب  (Ctrl+W)",
            ["MenuView"] = "عرض",
            ["MenuCommandPalette"] = "لوحة الأوامر...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "الشريط الجانبي  (Ctrl+B)",
            ["MenuTypewriter"] = "وضع الآلة الكاتبة",
            ["MenuRefresh"] = "تحديث  (Ctrl+R)",
            ["MenuFind"] = "بحث في الملف...  (Ctrl+F)",
            ["MenuReplace"] = "استبدال...  (Ctrl+H)",
            ["MenuSettings"] = "الإعدادات",
            ["MenuTheme"] = "المظهر",
            ["MenuThemeSystem"] = "حسب النظام",
            ["MenuThemeLight"] = "فاتح (أبيض)",
            ["MenuThemeDark"] = "داكن (أسود)",
            ["MenuThemeDarkSlate"] = "رمادي غامق فاحم",
            ["MenuThemeSepia"] = "مريح للقراءة (بيج)",
            ["MenuPreferences"] = "الخيارات والأتمتة",
            ["MenuAutoSave"] = "حفظ المسودات تلقائياً",
            ["MenuRestoreSession"] = "استئناف التابات السابقة",
            ["MenuLanguage"] = "اللغة",
            ["MenuHelp"] = "مساعدة",
            ["MenuMarkdownGuide"] = "دليل الماركداون",
            ["MenuJsonGuide"] = "دليل الجيسون ومسار JSONPath",
            ["MenuShortcuts"] = "اختصارات لوحة المفاتيح  (F1)",
            ["MenuAbout"] = "عن البرنامج",
            ["TabPin"] = "تثبيت التاب في المفضلة",
            ["TabUnpin"] = "إلغاء تثبيت التاب",
            ["TabFavorite"] = "إضافة للمفضلة الدائمة",
            ["TabClose"] = "إغلاق التاب  (Ctrl+W)",
            ["TabCloseRight"] = "إغلاق التابات لليمين",
            ["TabCloseOthers"] = "إغلاق التابات الأخرى",
            ["TabCloseAll"] = "إغلاق جميع التابات",
            ["JsonExpandAll"] = "توسيع الكل",
            ["JsonCollapseAll"] = "طي الكل",
            ["JsonFormat"] = "تنسيق الكود",
            ["JsonMinify"] = "ضغط الكود",
            ["JsonTable"] = "تحويل لجدول",
            ["JsonExport"] = "تصدير HTML",
            ["JsonCopy"] = "نسخ الخام",
            ["TooltipSplit"] = "تقسيم الشاشة (محرر + معاينة)",
            ["TooltipEditor"] = "محرر النصوص فقط",
            ["TooltipRead"] = "وضع القراءة الخالي من التشتيت",
            ["TooltipBold"] = "خط عريض (**نص**)",
            ["TooltipItalic"] = "خط مائل (*نص*)",
            ["TooltipHeading"] = "عنوان رئيسي (# عنوان)",
            ["TooltipCode"] = "كتلة كود برمجية (```)",
            ["TooltipMermaid"] = "إدراج رسم بياني Mermaid",
            ["TooltipAlert"] = "إدراج صندوق تنبيه ([!NOTE])",
            ["TooltipMath"] = "إدراج معادلة رياضية ($$)",
            ["TooltipOutline"] = "فهرس العناوين",
            ["TooltipAutoScroll"] = "تحريك انسيابي مستمر بدون لمس",
            ["TooltipAutoScrollSpeed"] = "سرعة التحريك الانسيابي (1 - 5)",
            ["TooltipThemeToggle"] = "تبديل المظهر (داكن / فاتح / سيبيا)",
            ["TooltipComments"] = "درج الملاحظات والتعليقات",
            ["TooltipExportPdf"] = "طباعة وتصدير كملف PDF",
            ["TooltipExportHtml"] = "تصدير كصفحة ويب كاملة دون إنترنت (HTML)",
            ["TooltipFocus"] = "وضع التركيز بملء الشاشة (F11)",
            ["TooltipZoomIn"] = "تكبير المستند (Ctrl++)",
            ["TooltipZoomOut"] = "تصغير المستند (Ctrl+-)",
            ["TooltipResetZoom"] = "إعادة ضبط التكبير 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "انقر لنسخ المسار الكامل",
            ["TooltipHidePath"] = "إخفاء شريط المسار",
            ["TooltipShowPath"] = "إظهار شريط المسار",
            ["TextPathCopied"] = "✓ تم نسخ المسار",
            ["SearchPlaceholder"] = "بحث فوري أو مسار JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "لا توجد ملفات حديثة",
            ["ClearHistory"] = "تفريغ السجل",
            ["DashboardFavoritesTitle"] = "المفضلة والملفات المشاهدة مؤخراً",
            ["CopyAll"] = "نسخ النص كاملاً  (Ctrl+C)",
            ["StatusWords"] = "كلمة",
            ["StatusLines"] = "سطر",
            ["StatusMinRead"] = "دقيقة قراءة",
            ["DrawerOutlineTitle"] = "فهرس العناوين والمحتويات",
            ["DrawerCommentsTitle"] = "الملاحظات والتعليقات",
            ["CommentPlaceholder"] = "اكتب ملاحظتك أو تعليقك هنا...",
            ["ButtonAddComment"] = "+ إضافة تعليق",
            ["ButtonClearAll"] = "مسح الكل",
        },

        // 2. English
        ["en"] = new()
        {
            ["MenuFile"] = "File",
            ["MenuNew"] = "New  (Ctrl+N)",
            ["MenuOpenFile"] = "Open File...  (Ctrl+O)",
            ["MenuOpenFolder"] = "Open Folder...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "Recent Files",
            ["MenuSave"] = "Save  (Ctrl+S)",
            ["MenuSaveAs"] = "Save As...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Close Tab  (Ctrl+W)",
            ["MenuView"] = "View",
            ["MenuCommandPalette"] = "Command Palette...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "Toggle Sidebar  (Ctrl+B)",
            ["MenuTypewriter"] = "Typewriter Mode",
            ["MenuRefresh"] = "Refresh  (Ctrl+R)",
            ["MenuFind"] = "Find in File...  (Ctrl+F)",
            ["MenuReplace"] = "Replace...  (Ctrl+H)",
            ["MenuSettings"] = "Settings",
            ["MenuTheme"] = "Theme",
            ["MenuThemeSystem"] = "System",
            ["MenuThemeLight"] = "Light",
            ["MenuThemeDark"] = "Dark",
            ["MenuThemeDarkSlate"] = "Dark Slate",
            ["MenuThemeSepia"] = "Sepia",
            ["MenuPreferences"] = "Preferences & Automation",
            ["MenuAutoSave"] = "Auto-Save Drafts",
            ["MenuRestoreSession"] = "Restore Last Session Tabs",
            ["MenuLanguage"] = "Language",
            ["MenuHelp"] = "Help",
            ["MenuMarkdownGuide"] = "Markdown Guide",
            ["MenuJsonGuide"] = "JSON & JSONPath Guide",
            ["MenuShortcuts"] = "Keyboard Shortcuts  (F1)",
            ["MenuAbout"] = "About JMDReader",
            ["TabPin"] = "Pin to Favorites",
            ["TabUnpin"] = "Unpin Tab",
            ["TabFavorite"] = "Add to Favorites",
            ["TabClose"] = "Close Tab  (Ctrl+W)",
            ["TabCloseRight"] = "Close Tabs to the Right",
            ["TabCloseOthers"] = "Close Other Tabs",
            ["TabCloseAll"] = "Close All Tabs",
            ["JsonExpandAll"] = "Expand All",
            ["JsonCollapseAll"] = "Collapse All",
            ["JsonFormat"] = "Format",
            ["JsonMinify"] = "Minify",
            ["JsonTable"] = "Table",
            ["JsonExport"] = "Export HTML",
            ["JsonCopy"] = "Copy Raw",
            ["TooltipSplit"] = "Split View (Editor + Preview)",
            ["TooltipEditor"] = "Editor Only",
            ["TooltipRead"] = "Distraction-Free Reading Mode",
            ["TooltipBold"] = "Bold (**text**)",
            ["TooltipItalic"] = "Italic (*text*)",
            ["TooltipHeading"] = "Heading (# Title)",
            ["TooltipCode"] = "Code Block (```)",
            ["TooltipMermaid"] = "Insert Mermaid Diagram",
            ["TooltipAlert"] = "Insert Alert Callout ([!NOTE])",
            ["TooltipMath"] = "Insert Math Formula ($$)",
            ["TooltipOutline"] = "Interactive Document Outline",
            ["TooltipAutoScroll"] = "Smooth Teleprompter Auto-Scroll",
            ["TooltipAutoScrollSpeed"] = "Continuous Scroll Speed (1 - 5)",
            ["TooltipThemeToggle"] = "Cycle Theme (Dark / Light / Sepia)",
            ["TooltipComments"] = "Notes & Comments Drawer",
            ["TooltipExportPdf"] = "Print / Export to PDF",
            ["TooltipExportHtml"] = "Export Standalone Offline HTML",
            ["TooltipFocus"] = "Zen Focus Fullscreen Mode (F11)",
            ["TooltipZoomIn"] = "Zoom In (Ctrl++)",
            ["TooltipZoomOut"] = "Zoom Out (Ctrl+-)",
            ["TooltipResetZoom"] = "Reset Zoom to 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "Click to copy document path",
            ["TooltipHidePath"] = "Hide document path bar",
            ["TooltipShowPath"] = "Show document path bar",
            ["TextPathCopied"] = "✓ Path Copied",
            ["SearchPlaceholder"] = "Quick search or JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "No recent files",
            ["ClearHistory"] = "Clear History",
            ["DashboardFavoritesTitle"] = "FAVORITES & RECENT",
            ["CopyAll"] = "Copy All Text  (Ctrl+C)",
            ["StatusWords"] = "words",
            ["StatusLines"] = "lines",
            ["StatusMinRead"] = "min read",
            ["DrawerOutlineTitle"] = "DOCUMENT OUTLINE",
            ["DrawerCommentsTitle"] = "NOTES & COMMENTS",
            ["CommentPlaceholder"] = "Write a note or comment on selected text...",
            ["ButtonAddComment"] = "+ Add Comment",
            ["ButtonClearAll"] = "Clear All",
        },

        // 3. Spanish (Español)
        ["es"] = new()
        {
            ["MenuFile"] = "Archivo",
            ["MenuNew"] = "Nuevo  (Ctrl+N)",
            ["MenuOpenFile"] = "Abrir archivo...  (Ctrl+O)",
            ["MenuOpenFolder"] = "Abrir carpeta...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "Archivos recientes",
            ["MenuSave"] = "Guardar  (Ctrl+S)",
            ["MenuSaveAs"] = "Guardar como...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Cerrar pestaña  (Ctrl+W)",
            ["MenuView"] = "Ver",
            ["MenuCommandPalette"] = "Paleta de comandos...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "Barra lateral  (Ctrl+B)",
            ["MenuTypewriter"] = "Modo máquina de escribir",
            ["MenuRefresh"] = "Actualizar  (Ctrl+R)",
            ["MenuFind"] = "Buscar en archivo...  (Ctrl+F)",
            ["MenuReplace"] = "Reemplazar...  (Ctrl+H)",
            ["MenuSettings"] = "Configuración",
            ["MenuTheme"] = "Tema",
            ["MenuThemeSystem"] = "Del sistema",
            ["MenuThemeLight"] = "Claro",
            ["MenuThemeDark"] = "Oscuro",
            ["MenuThemeDarkSlate"] = "Gris oscuro",
            ["MenuThemeSepia"] = "Sepia",
            ["MenuPreferences"] = "Preferencias y automatización",
            ["MenuAutoSave"] = "Guardar borradores automáticamente",
            ["MenuRestoreSession"] = "Restaurar pestañas de la sesión anterior",
            ["MenuLanguage"] = "Idioma",
            ["MenuHelp"] = "Ayuda",
            ["MenuMarkdownGuide"] = "Guía de Markdown",
            ["MenuJsonGuide"] = "Guía de JSON y JSONPath",
            ["MenuShortcuts"] = "Atajos de teclado  (F1)",
            ["MenuAbout"] = "Acerca de JMDReader",
            ["TabPin"] = "Fijar en favoritos",
            ["TabUnpin"] = "Desfijar pestaña",
            ["TabFavorite"] = "Agregar a favoritos",
            ["TabClose"] = "Cerrar pestaña  (Ctrl+W)",
            ["TabCloseRight"] = "Cerrar pestañas a la derecha",
            ["TabCloseOthers"] = "Cerrar otras pestañas",
            ["TabCloseAll"] = "Cerrar todas las pestañas",
            ["JsonExpandAll"] = "Expandir todo",
            ["JsonCollapseAll"] = "Contraer todo",
            ["JsonFormat"] = "Formatear",
            ["JsonMinify"] = "Comprimir",
            ["JsonTable"] = "Tabla",
            ["JsonExport"] = "Exportar HTML",
            ["JsonCopy"] = "Copiar sin formato",
            ["TooltipSplit"] = "Vista dividida (Editor + Vista previa)",
            ["TooltipEditor"] = "Solo editor",
            ["TooltipRead"] = "Modo de lectura sin distracciones",
            ["TooltipBold"] = "Negrita (**texto**)",
            ["TooltipItalic"] = "Cursiva (*texto*)",
            ["TooltipHeading"] = "Encabezado (# Título)",
            ["TooltipCode"] = "Bloque de código (```)",
            ["TooltipMermaid"] = "Insertar diagrama Mermaid",
            ["TooltipAlert"] = "Insertar alerta ([!NOTE])",
            ["TooltipMath"] = "Insertar fórmula matemática ($$)",
            ["TooltipOutline"] = "Índice de contenido",
            ["TooltipAutoScroll"] = "Desplazamiento automático suave",
            ["TooltipAutoScrollSpeed"] = "Velocidad de desplazamiento (1 - 5)",
            ["TooltipThemeToggle"] = "Cambiar tema",
            ["TooltipComments"] = "Notas y comentarios",
            ["TooltipExportPdf"] = "Exportar a PDF",
            ["TooltipExportHtml"] = "Exportar HTML sin conexión",
            ["TooltipFocus"] = "Modo concentración (F11)",
            ["TooltipZoomIn"] = "Acercar (Ctrl++)",
            ["TooltipZoomOut"] = "Alejar (Ctrl+-)",
            ["TooltipResetZoom"] = "Restablecer zoom (Ctrl+0)",
            ["TooltipCopyPath"] = "Copiar ruta del archivo",
            ["TooltipHidePath"] = "Ocultar barra de ruta",
            ["TooltipShowPath"] = "Mostrar barra de ruta",
            ["TextPathCopied"] = "✓ Ruta copiada",
            ["SearchPlaceholder"] = "Búsqueda rápida o JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "No hay archivos recientes",
            ["ClearHistory"] = "Borrar historial",
            ["DashboardFavoritesTitle"] = "FAVORITOS Y RECIENTES",
            ["CopyAll"] = "Copiar todo el texto  (Ctrl+C)",
            ["StatusWords"] = "palabras",
            ["StatusLines"] = "líneas",
            ["StatusMinRead"] = "min de lectura",
            ["DrawerOutlineTitle"] = "ÍNDICE DE CONTENIDO",
            ["DrawerCommentsTitle"] = "NOTAS Y COMENTARIOS",
            ["CommentPlaceholder"] = "Escriba una nota o comentario...",
            ["ButtonAddComment"] = "+ Agregar comentario",
            ["ButtonClearAll"] = "Borrar todo",
        },

        // 4. Hindi (हिन्दी)
        ["hi"] = new()
        {
            ["MenuFile"] = "फ़ाइल",
            ["MenuNew"] = "नया  (Ctrl+N)",
            ["MenuOpenFile"] = "फ़ाइल खोलें...  (Ctrl+O)",
            ["MenuOpenFolder"] = "फ़ोल्डर खोलें...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "हाल की फ़ाइलें",
            ["MenuSave"] = "सहेजें  (Ctrl+S)",
            ["MenuSaveAs"] = "इस रूप में सहेजें...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "टैब बंद करें  (Ctrl+W)",
            ["MenuView"] = "देखें",
            ["MenuCommandPalette"] = "कमांड पैलेट...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "साइडबार दिखाएं/छिपाएं  (Ctrl+B)",
            ["MenuTypewriter"] = "टाइपराइटर मोड",
            ["MenuRefresh"] = "ताज़ा करें  (Ctrl+R)",
            ["MenuFind"] = "फ़ाइल में खोजें...  (Ctrl+F)",
            ["MenuReplace"] = "बदलें...  (Ctrl+H)",
            ["MenuSettings"] = "सेटिंग्स",
            ["MenuTheme"] = "थीम",
            ["MenuThemeSystem"] = "सिस्टम अनुसार",
            ["MenuThemeLight"] = "लाइट",
            ["MenuThemeDark"] = "डार्क",
            ["MenuThemeDarkSlate"] = "गहरा स्लेट",
            ["MenuThemeSepia"] = "सेपिया (पठन हेतु)",
            ["MenuPreferences"] = "प्राथमिकताएं और स्वचालन",
            ["MenuAutoSave"] = "ड्राफ्ट स्वतः सहेजें",
            ["MenuRestoreSession"] = "पिछले सत्र के टैब पुनर्स्थापित करें",
            ["MenuLanguage"] = "भाषा",
            ["MenuHelp"] = "सहायता",
            ["MenuMarkdownGuide"] = "मार्कडाउन गाइड",
            ["MenuJsonGuide"] = "JSON और JSONPath गाइड",
            ["MenuShortcuts"] = "कीबोर्ड शॉर्टकट  (F1)",
            ["MenuAbout"] = "JMDReader के बारे में",
            ["TabPin"] = "पसंदीदा में पिन करें",
            ["TabUnpin"] = "टैब अनपिन करें",
            ["TabFavorite"] = "पसंदीदा में जोड़ें",
            ["TabClose"] = "टैब बंद करें  (Ctrl+W)",
            ["TabCloseRight"] = "दाईं ओर के टैब बंद करें",
            ["TabCloseOthers"] = "अन्य टैब बंद करें",
            ["TabCloseAll"] = "सभी टैब बंद करें",
            ["JsonExpandAll"] = "सभी विस्तृत करें",
            ["JsonCollapseAll"] = "सभी संकुचित करें",
            ["JsonFormat"] = "प्रारूपित करें",
            ["JsonMinify"] = "छोटा करें",
            ["JsonTable"] = "तालिका",
            ["JsonExport"] = "HTML निर्यात करें",
            ["JsonCopy"] = "कॉपी करें",
            ["TooltipSplit"] = "विभाजित दृश्य (संपादक + पूर्वावलोकन)",
            ["TooltipEditor"] = "केवल संपादक",
            ["TooltipRead"] = "व्याकुलता-मुक्त पठन मोड",
            ["TooltipBold"] = "बोल्ड (**पाठ**)",
            ["TooltipItalic"] = "इटैलिक (*पाठ*)",
            ["TooltipHeading"] = "शीर्षक (# शीर्षक)",
            ["TooltipCode"] = "कोड ब्लॉक (```)",
            ["TooltipMermaid"] = "मर्मेड आरेख जोड़ें",
            ["TooltipAlert"] = "अलर्ट जोड़ें ([!NOTE])",
            ["TooltipMath"] = "गणितीय सूत्र जोड़ें ($$)",
            ["TooltipOutline"] = "दस्तावेज़ की रूपरेखा",
            ["TooltipAutoScroll"] = "सहज निरंतर स्वतः स्क्रॉल",
            ["TooltipAutoScrollSpeed"] = "स्क्रॉल गति (1 - 5)",
            ["TooltipThemeToggle"] = "थीम बदलें",
            ["TooltipComments"] = "नोट्स और टिप्पणियां",
            ["TooltipExportPdf"] = "PDF के रूप में निर्यात करें",
            ["TooltipExportHtml"] = "ऑफ़लाइन HTML निर्यात करें",
            ["TooltipFocus"] = "फ़ोकस मोड (F11)",
            ["TooltipZoomIn"] = "ज़ूम इन (Ctrl++)",
            ["TooltipZoomOut"] = "ज़ूम आउट (Ctrl+-)",
            ["TooltipResetZoom"] = "100% पर रीसेट करें (Ctrl+0)",
            ["TooltipCopyPath"] = "पथ कॉपी करने के लिए क्लिक करें",
            ["TooltipHidePath"] = "पथ पट्टी छिपाएं",
            ["TooltipShowPath"] = "पथ पट्टी दिखाएं",
            ["TextPathCopied"] = "✓ पथ कॉपी किया गया",
            ["SearchPlaceholder"] = "त्वरित खोज या JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "कोई हालिया फ़ाइल नहीं",
            ["ClearHistory"] = "इतिहास साफ़ करें",
            ["DashboardFavoritesTitle"] = "पसंदीदा और हालिया फ़ाइलें",
            ["CopyAll"] = "पूरा पाठ कॉपी करें  (Ctrl+C)",
            ["StatusWords"] = "शब्द",
            ["StatusLines"] = "पंक्तियाँ",
            ["StatusMinRead"] = "मिनट पठन",
            ["DrawerOutlineTitle"] = "दस्तावेज़ की रूपरेखा",
            ["DrawerCommentsTitle"] = "नोट्स और टिप्पणियां",
            ["CommentPlaceholder"] = "अपनी टिप्पणी लिखें...",
            ["ButtonAddComment"] = "+ टिप्पणी जोड़ें",
            ["ButtonClearAll"] = "सभी हटाएं",
        },

        // 5. Chinese (中文 简体)
        ["zh"] = new()
        {
            ["MenuFile"] = "文件",
            ["MenuNew"] = "新建  (Ctrl+N)",
            ["MenuOpenFile"] = "打开文件...  (Ctrl+O)",
            ["MenuOpenFolder"] = "打开文件夹...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "最近打开的文件",
            ["MenuSave"] = "保存  (Ctrl+S)",
            ["MenuSaveAs"] = "另存为...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "关闭标签页  (Ctrl+W)",
            ["MenuView"] = "视图",
            ["MenuCommandPalette"] = "命令面板...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "切换侧边栏  (Ctrl+B)",
            ["MenuTypewriter"] = "打字机模式",
            ["MenuRefresh"] = "刷新  (Ctrl+R)",
            ["MenuFind"] = "在文件中查找...  (Ctrl+F)",
            ["MenuReplace"] = "替换...  (Ctrl+H)",
            ["MenuSettings"] = "设置",
            ["MenuTheme"] = "主题",
            ["MenuThemeSystem"] = "跟随系统",
            ["MenuThemeLight"] = "浅色",
            ["MenuThemeDark"] = "深色",
            ["MenuThemeDarkSlate"] = "暗石板灰",
            ["MenuThemeSepia"] = "护眼羊皮纸",
            ["MenuPreferences"] = "首选项与自动化",
            ["MenuAutoSave"] = "自动保存草稿",
            ["MenuRestoreSession"] = "恢复上次会话标签页",
            ["MenuLanguage"] = "语言",
            ["MenuHelp"] = "帮助",
            ["MenuMarkdownGuide"] = "Markdown 语法指南",
            ["MenuJsonGuide"] = "JSON 与 JSONPath 指南",
            ["MenuShortcuts"] = "快捷键列表  (F1)",
            ["MenuAbout"] = "关于 JMDReader",
            ["TabPin"] = "固定到收藏夹",
            ["TabUnpin"] = "取消固定标签页",
            ["TabFavorite"] = "添加到收藏夹",
            ["TabClose"] = "关闭标签页  (Ctrl+W)",
            ["TabCloseRight"] = "关闭右侧所有标签页",
            ["TabCloseOthers"] = "关闭其他标签页",
            ["TabCloseAll"] = "关闭所有标签页",
            ["JsonExpandAll"] = "全部展开",
            ["JsonCollapseAll"] = "全部折叠",
            ["JsonFormat"] = "美化格式",
            ["JsonMinify"] = "压缩单行",
            ["JsonTable"] = "转为表格",
            ["JsonExport"] = "导出离线网页",
            ["JsonCopy"] = "复制原始内容",
            ["TooltipSplit"] = "分屏视图 (编辑器 + 预览)",
            ["TooltipEditor"] = "仅编辑器",
            ["TooltipRead"] = "全屏专注阅读模式",
            ["TooltipBold"] = "粗体 (**文本**)",
            ["TooltipItalic"] = "斜体 (*文本*)",
            ["TooltipHeading"] = "标题 (# 标题)",
            ["TooltipCode"] = "代码块 (```)",
            ["TooltipMermaid"] = "插入 Mermaid 图表",
            ["TooltipAlert"] = "插入提示框 ([!NOTE])",
            ["TooltipMath"] = "插入数学公式 ($$)",
            ["TooltipOutline"] = "大纲目录",
            ["TooltipAutoScroll"] = "平滑连续自动滚动",
            ["TooltipAutoScrollSpeed"] = "滚动速度 (1 - 5)",
            ["TooltipThemeToggle"] = "切换阅读主题",
            ["TooltipComments"] = "便签与评论",
            ["TooltipExportPdf"] = "导出为 PDF 文档",
            ["TooltipExportHtml"] = "导出离线 HTML 网页",
            ["TooltipFocus"] = "禅模式全屏 (F11)",
            ["TooltipZoomIn"] = "放大 (Ctrl++)",
            ["TooltipZoomOut"] = "缩小 (Ctrl+-)",
            ["TooltipResetZoom"] = "重置缩放 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "点击复制文件路径",
            ["TooltipHidePath"] = "隐藏路径栏",
            ["TooltipShowPath"] = "显示路径栏",
            ["TextPathCopied"] = "✓ 路径已复制",
            ["SearchPlaceholder"] = "快速搜索或 JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "暂无最近文件",
            ["ClearHistory"] = "清空历史记录",
            ["DashboardFavoritesTitle"] = "收藏夹与最近文件",
            ["CopyAll"] = "复制全文内容  (Ctrl+C)",
            ["StatusWords"] = "字",
            ["StatusLines"] = "行",
            ["StatusMinRead"] = "分钟阅读",
            ["DrawerOutlineTitle"] = "文档大纲目录",
            ["DrawerCommentsTitle"] = "便签与评论记录",
            ["CommentPlaceholder"] = "在此添加便签或注释...",
            ["ButtonAddComment"] = "+ 添加评论",
            ["ButtonClearAll"] = "清空全部",
        },

        // 6. French (Français)
        ["fr"] = new()
        {
            ["MenuFile"] = "Fichier",
            ["MenuNew"] = "Nouveau  (Ctrl+N)",
            ["MenuOpenFile"] = "Ouvrir un fichier...  (Ctrl+O)",
            ["MenuOpenFolder"] = "Ouvrir un dossier...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "Fichiers récents",
            ["MenuSave"] = "Enregistrer  (Ctrl+S)",
            ["MenuSaveAs"] = "Enregistrer sous...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Fermer l'onglet  (Ctrl+W)",
            ["MenuView"] = "Affichage",
            ["MenuCommandPalette"] = "Palette de commandes...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "Volet latéral  (Ctrl+B)",
            ["MenuTypewriter"] = "Mode machine à écrire",
            ["MenuRefresh"] = "Actualiser  (Ctrl+R)",
            ["MenuFind"] = "Rechercher dans le fichier...  (Ctrl+F)",
            ["MenuReplace"] = "Remplacer...  (Ctrl+H)",
            ["MenuSettings"] = "Paramètres",
            ["MenuTheme"] = "Thème",
            ["MenuThemeSystem"] = "Système",
            ["MenuThemeLight"] = "Clair",
            ["MenuThemeDark"] = "Sombre",
            ["MenuThemeDarkSlate"] = "Ardoise foncée",
            ["MenuThemeSepia"] = "Sépia",
            ["MenuPreferences"] = "Préférences et automatisation",
            ["MenuAutoSave"] = "Enregistrement automatique des brouillons",
            ["MenuRestoreSession"] = "Restaurer les onglets de la session précédente",
            ["MenuLanguage"] = "Langue",
            ["MenuHelp"] = "Aide",
            ["MenuMarkdownGuide"] = "Guide Markdown",
            ["MenuJsonGuide"] = "Guide JSON et JSONPath",
            ["MenuShortcuts"] = "Raccourcis clavier  (F1)",
            ["MenuAbout"] = "À propos de JMDReader",
            ["TabPin"] = "Épingler aux favoris",
            ["TabUnpin"] = "Désépingler l'onglet",
            ["TabFavorite"] = "Ajouter aux favoris",
            ["TabClose"] = "Fermer l'onglet  (Ctrl+W)",
            ["TabCloseRight"] = "Fermer les onglets à droite",
            ["TabCloseOthers"] = "Fermer les autres onglets",
            ["TabCloseAll"] = "Fermer tous les onglets",
            ["JsonExpandAll"] = "Tout développer",
            ["JsonCollapseAll"] = "Tout réduire",
            ["JsonFormat"] = "Formater",
            ["JsonMinify"] = "Compresser",
            ["JsonTable"] = "Tableau",
            ["JsonExport"] = "Exporter HTML",
            ["JsonCopy"] = "Copier brut",
            ["TooltipSplit"] = "Vue fractionnée (Éditeur + Aperçu)",
            ["TooltipEditor"] = "Éditeur uniquement",
            ["TooltipRead"] = "Mode lecture sans distraction",
            ["TooltipBold"] = "Gras (**texte**)",
            ["TooltipItalic"] = "Italique (*texte*)",
            ["TooltipHeading"] = "Titre (# Titre)",
            ["TooltipCode"] = "Bloc de code (```)",
            ["TooltipMermaid"] = "Insérer un schéma Mermaid",
            ["TooltipAlert"] = "Insérer un avertissement ([!NOTE])",
            ["TooltipMath"] = "Insérer une formule ($$)",
            ["TooltipOutline"] = "Plan du document",
            ["TooltipAutoScroll"] = "Défilement automatique fluide",
            ["TooltipAutoScrollSpeed"] = "Vitesse de défilement (1 - 5)",
            ["TooltipThemeToggle"] = "Changer de thème",
            ["TooltipComments"] = "Notes et commentaires",
            ["TooltipExportPdf"] = "Exporter en PDF",
            ["TooltipExportHtml"] = "Exporter en HTML hors ligne",
            ["TooltipFocus"] = "Mode plein écran zen (F11)",
            ["TooltipZoomIn"] = "Zoom avant (Ctrl++)",
            ["TooltipZoomOut"] = "Zoom arrière (Ctrl+-)",
            ["TooltipResetZoom"] = "Réinitialiser le zoom (Ctrl+0)",
            ["TooltipCopyPath"] = "Copier le chemin du fichier",
            ["TooltipHidePath"] = "Masquer la barre de chemin",
            ["TooltipShowPath"] = "Afficher la barre de chemin",
            ["TextPathCopied"] = "✓ Chemin copié",
            ["SearchPlaceholder"] = "Recherche rapide ou JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "Aucun fichier récent",
            ["ClearHistory"] = "Effacer l'historique",
            ["DashboardFavoritesTitle"] = "FAVORIS ET FICHIERS RÉCENTS",
            ["CopyAll"] = "Copier tout le texte  (Ctrl+C)",
            ["StatusWords"] = "mots",
            ["StatusLines"] = "lignes",
            ["StatusMinRead"] = "min de lecture",
            ["DrawerOutlineTitle"] = "PLAN DU DOCUMENT",
            ["DrawerCommentsTitle"] = "NOTES ET COMMENTAIRES",
            ["CommentPlaceholder"] = "Rédiger une note ou un commentaire...",
            ["ButtonAddComment"] = "+ Ajouter un commentaire",
            ["ButtonClearAll"] = "Tout effacer",
        },

        // 7. German (Deutsch)
        ["de"] = new()
        {
            ["MenuFile"] = "Datei",
            ["MenuNew"] = "Neu  (Ctrl+N)",
            ["MenuOpenFile"] = "Datei öffnen...  (Ctrl+O)",
            ["MenuOpenFolder"] = "Ordner öffnen...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "Zuletzt verwendete Dateien",
            ["MenuSave"] = "Speichern  (Ctrl+S)",
            ["MenuSaveAs"] = "Speichern unter...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Tab schließen  (Ctrl+W)",
            ["MenuView"] = "Ansicht",
            ["MenuCommandPalette"] = "Befehlspalette...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "Seitenleiste umschalten  (Ctrl+B)",
            ["MenuTypewriter"] = "Schreibmaschinen-Modus",
            ["MenuRefresh"] = "Aktualisieren  (Ctrl+R)",
            ["MenuFind"] = "In Datei suchen...  (Ctrl+F)",
            ["MenuReplace"] = "Ersetzen...  (Ctrl+H)",
            ["MenuSettings"] = "Einstellungen",
            ["MenuTheme"] = "Design",
            ["MenuThemeSystem"] = "System",
            ["MenuThemeLight"] = "Hell",
            ["MenuThemeDark"] = "Dunkel",
            ["MenuThemeDarkSlate"] = "Dunkler Schiefer",
            ["MenuThemeSepia"] = "Sepia",
            ["MenuPreferences"] = "Einstellungen & Automatisierung",
            ["MenuAutoSave"] = "Entwürfe automatisch speichern",
            ["MenuRestoreSession"] = "Tabs der letzten Sitzung wiederherstellen",
            ["MenuLanguage"] = "Sprache",
            ["MenuHelp"] = "Hilfe",
            ["MenuMarkdownGuide"] = "Markdown-Anleitung",
            ["MenuJsonGuide"] = "JSON- & JSONPath-Anleitung",
            ["MenuShortcuts"] = "Tastenkombinationen  (F1)",
            ["MenuAbout"] = "Über JMDReader",
            ["TabPin"] = "Zu Favoriten anheften",
            ["TabUnpin"] = "Tab lösen",
            ["TabFavorite"] = "Zu Favoriten hinzufügen",
            ["TabClose"] = "Tab schließen  (Ctrl+W)",
            ["TabCloseRight"] = "Rechte Tabs schließen",
            ["TabCloseOthers"] = "Andere Tabs schließen",
            ["TabCloseAll"] = "Alle Tabs schließen",
            ["JsonExpandAll"] = "Alle erweitern",
            ["JsonCollapseAll"] = "Alle einklappen",
            ["JsonFormat"] = "Formatieren",
            ["JsonMinify"] = "Komprimieren",
            ["JsonTable"] = "Tabelle",
            ["JsonExport"] = "HTML exportieren",
            ["JsonCopy"] = "Rohdaten kopieren",
            ["TooltipSplit"] = "Geteilte Ansicht (Editor + Vorschau)",
            ["TooltipEditor"] = "Nur Editor",
            ["TooltipRead"] = "Ablenkungsfreier Lesemodus",
            ["TooltipBold"] = "Fett (**Text**)",
            ["TooltipItalic"] = "Kursiv (*Text*)",
            ["TooltipHeading"] = "Überschrift (# Titel)",
            ["TooltipCode"] = "Codeblock (```)",
            ["TooltipMermaid"] = "Mermaid-Diagramm einfügen",
            ["TooltipAlert"] = "Hinweisfeld einfügen ([!NOTE])",
            ["TooltipMath"] = "Formel einfügen ($$)",
            ["TooltipOutline"] = "Dokumentgliederung",
            ["TooltipAutoScroll"] = "Sanfter automatischer Bildlauf",
            ["TooltipAutoScrollSpeed"] = "Scrollgeschwindigkeit (1 - 5)",
            ["TooltipThemeToggle"] = "Design wechseln",
            ["TooltipComments"] = "Notizen und Kommentare",
            ["TooltipExportPdf"] = "Als PDF exportieren",
            ["TooltipExportHtml"] = "Als Offline-HTML exportieren",
            ["TooltipFocus"] = "Vollbild-Fokusmodus (F11)",
            ["TooltipZoomIn"] = "Vergrößern (Ctrl++)",
            ["TooltipZoomOut"] = "Verkleinern (Ctrl+-)",
            ["TooltipResetZoom"] = "Zoom zurücksetzen (Ctrl+0)",
            ["TooltipCopyPath"] = "Dateipfad kopieren",
            ["TooltipHidePath"] = "Pfadleiste ausblenden",
            ["TooltipShowPath"] = "Pfadleiste einblenden",
            ["TextPathCopied"] = "✓ Pfad kopiert",
            ["SearchPlaceholder"] = "Schnellsuche oder JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "Keine aktuellen Dateien",
            ["ClearHistory"] = "Verlauf löschen",
            ["DashboardFavoritesTitle"] = "FAVORITEN UND ZULETZT VERWENDET",
            ["CopyAll"] = "Gesamten Text kopieren  (Ctrl+C)",
            ["StatusWords"] = "Wörter",
            ["StatusLines"] = "Zeilen",
            ["StatusMinRead"] = "Min. Lesezeit",
            ["DrawerOutlineTitle"] = "DOKUMENTGLIEDERUNG",
            ["DrawerCommentsTitle"] = "NOTIZEN UND KOMMENTARE",
            ["CommentPlaceholder"] = "Notiz oder Kommentar eingeben...",
            ["ButtonAddComment"] = "+ Kommentar hinzufügen",
            ["ButtonClearAll"] = "Alles löschen",
        },

        // 8. Russian (Русский)
        ["ru"] = new()
        {
            ["MenuFile"] = "Файл",
            ["MenuNew"] = "Создать  (Ctrl+N)",
            ["MenuOpenFile"] = "Открыть файл...  (Ctrl+O)",
            ["MenuOpenFolder"] = "Открыть папку...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "Недавние файлы",
            ["MenuSave"] = "Сохранить  (Ctrl+S)",
            ["MenuSaveAs"] = "Сохранить как...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Закрыть вкладку  (Ctrl+W)",
            ["MenuView"] = "Вид",
            ["MenuCommandPalette"] = "Палитра команд...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "Боковая панель  (Ctrl+B)",
            ["MenuTypewriter"] = "Режим пишущей машинки",
            ["MenuRefresh"] = "Обновить  (Ctrl+R)",
            ["MenuFind"] = "Найти в файле...  (Ctrl+F)",
            ["MenuReplace"] = "Заменить...  (Ctrl+H)",
            ["MenuSettings"] = "Настройки",
            ["MenuTheme"] = "Тема",
            ["MenuThemeSystem"] = "Системная",
            ["MenuThemeLight"] = "Светлая",
            ["MenuThemeDark"] = "Темная",
            ["MenuThemeDarkSlate"] = "Темный сланец",
            ["MenuThemeSepia"] = "Сепия",
            ["MenuPreferences"] = "Параметры и автоматизация",
            ["MenuAutoSave"] = "Автосохранение черновиков",
            ["MenuRestoreSession"] = "Восстанавливать вкладки сессии",
            ["MenuLanguage"] = "Язык",
            ["MenuHelp"] = "Справка",
            ["MenuMarkdownGuide"] = "Руководство по Markdown",
            ["MenuJsonGuide"] = "Руководство по JSON и JSONPath",
            ["MenuShortcuts"] = "Горячие клавиши  (F1)",
            ["MenuAbout"] = "О программе JMDReader",
            ["TabPin"] = "Закрепить в избранном",
            ["TabUnpin"] = "Открепить вкладку",
            ["TabFavorite"] = "Добавить в избранное",
            ["TabClose"] = "Закрыть вкладку  (Ctrl+W)",
            ["TabCloseRight"] = "Закрыть вкладки справа",
            ["TabCloseOthers"] = "Закрыть другие вкладки",
            ["TabCloseAll"] = "Закрыть все вкладки",
            ["JsonExpandAll"] = "Развернуть все",
            ["JsonCollapseAll"] = "Свернуть все",
            ["JsonFormat"] = "Форматировать",
            ["JsonMinify"] = "Сжать",
            ["JsonTable"] = "Таблица",
            ["JsonExport"] = "Экспорт в HTML",
            ["JsonCopy"] = "Копировать исходный",
            ["TooltipSplit"] = "Разделенный вид (Редактор + Просмотр)",
            ["TooltipEditor"] = "Только редактор",
            ["TooltipRead"] = "Режим чтения без отвлечений",
            ["TooltipBold"] = "Жирный (**текст**)",
            ["TooltipItalic"] = "Курсив (*текст*)",
            ["TooltipHeading"] = "Заголовок (# Заголовок)",
            ["TooltipCode"] = "Блок кода (```)",
            ["TooltipMermaid"] = "Вставить диаграмму Mermaid",
            ["TooltipAlert"] = "Вставить примечание ([!NOTE])",
            ["TooltipMath"] = "Вставить формулу ($$)",
            ["TooltipOutline"] = "Оглавление документа",
            ["TooltipAutoScroll"] = "Плавная автопрокрутка",
            ["TooltipAutoScrollSpeed"] = "Скорость прокрутки (1 - 5)",
            ["TooltipThemeToggle"] = "Переключить тему",
            ["TooltipComments"] = "Заметки и комментарии",
            ["TooltipExportPdf"] = "Экспорт в PDF",
            ["TooltipExportHtml"] = "Экспорт в автономный HTML",
            ["TooltipFocus"] = "Полноэкранный режим (F11)",
            ["TooltipZoomIn"] = "Увеличить (Ctrl++)",
            ["TooltipZoomOut"] = "Уменьшить (Ctrl+-)",
            ["TooltipResetZoom"] = "Сброс масштаба 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "Скопировать путь к файлу",
            ["TooltipHidePath"] = "Скрыть строку пути",
            ["TooltipShowPath"] = "Показать строку пути",
            ["TextPathCopied"] = "✓ Путь скопирован",
            ["SearchPlaceholder"] = "Быстрый поиск или JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "Нет недавних файлов",
            ["ClearHistory"] = "Очистить историю",
            ["DashboardFavoritesTitle"] = "ИЗБРАННОЕ И НЕДАВНИЕ ФАЙЛЫ",
            ["CopyAll"] = "Копировать весь текст  (Ctrl+C)",
            ["StatusWords"] = "слов",
            ["StatusLines"] = "строк",
            ["StatusMinRead"] = "мин чтения",
            ["DrawerOutlineTitle"] = "ОГЛАВЛЕНИЕ ДОКУМЕНТА",
            ["DrawerCommentsTitle"] = "ЗАМЕТКИ И КОММЕНТАРИИ",
            ["CommentPlaceholder"] = "Напишите заметку или комментарий...",
            ["ButtonAddComment"] = "+ Добавить комментарий",
            ["ButtonClearAll"] = "Очистить все",
        },

        // 9. Japanese (日本語)
        ["ja"] = new()
        {
            ["MenuFile"] = "ファイル",
            ["MenuNew"] = "新規作成  (Ctrl+N)",
            ["MenuOpenFile"] = "ファイルを開く...  (Ctrl+O)",
            ["MenuOpenFolder"] = "フォルダーを開く...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "最近開いたファイル",
            ["MenuSave"] = "保存  (Ctrl+S)",
            ["MenuSaveAs"] = "名前を付けて保存...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "タブを閉じる  (Ctrl+W)",
            ["MenuView"] = "表示",
            ["MenuCommandPalette"] = "コマンドパレット...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "サイドバーの切り替え  (Ctrl+B)",
            ["MenuTypewriter"] = "タイプライターモード",
            ["MenuRefresh"] = "再読み込み  (Ctrl+R)",
            ["MenuFind"] = "ファイル内を検索...  (Ctrl+F)",
            ["MenuReplace"] = "置換...  (Ctrl+H)",
            ["MenuSettings"] = "設定",
            ["MenuTheme"] = "テーマ",
            ["MenuThemeSystem"] = "システム",
            ["MenuThemeLight"] = "ライト",
            ["MenuThemeDark"] = "ダーク",
            ["MenuThemeDarkSlate"] = "ダークスレート",
            ["MenuThemeSepia"] = "セピア",
            ["MenuPreferences"] = "環境設定と自動化",
            ["MenuAutoSave"] = "下書きの自動保存",
            ["MenuRestoreSession"] = "前回のタブを復元",
            ["MenuLanguage"] = "言語",
            ["MenuHelp"] = "ヘルプ",
            ["MenuMarkdownGuide"] = "Markdown ガイド",
            ["MenuJsonGuide"] = "JSON & JSONPath ガイド",
            ["MenuShortcuts"] = "キーボードショートカット  (F1)",
            ["MenuAbout"] = "JMDReader について",
            ["TabPin"] = "お気に入りに固定",
            ["TabUnpin"] = "固定を解除",
            ["TabFavorite"] = "お気に入りに追加",
            ["TabClose"] = "タブを閉じる  (Ctrl+W)",
            ["TabCloseRight"] = "右側のタブを閉じる",
            ["TabCloseOthers"] = "他のタブを閉じる",
            ["TabCloseAll"] = "すべてのタブを閉じる",
            ["JsonExpandAll"] = "すべて展開",
            ["JsonCollapseAll"] = "すべて折りたたむ",
            ["JsonFormat"] = "整形",
            ["JsonMinify"] = "圧縮",
            ["JsonTable"] = "表に変換",
            ["JsonExport"] = "HTML エクスポート",
            ["JsonCopy"] = "生データをコピー",
            ["TooltipSplit"] = "分割ビュー (エディター + プレビュー)",
            ["TooltipEditor"] = "エディターのみ",
            ["TooltipRead"] = "集中読書モード",
            ["TooltipBold"] = "太字 (**テキスト**)",
            ["TooltipItalic"] = "斜体 (*テキスト*)",
            ["TooltipHeading"] = "見出し (# タイトル)",
            ["TooltipCode"] = "コードブロック (```)",
            ["TooltipMermaid"] = "Mermaid ダイアグラムを挿入",
            ["TooltipAlert"] = "アラート吹き出しを挿入 ([!NOTE])",
            ["TooltipMath"] = "数式を挿入 ($$)",
            ["TooltipOutline"] = "目次アウトライン",
            ["TooltipAutoScroll"] = "滑らかな連続自動スクロール",
            ["TooltipAutoScrollSpeed"] = "スクロール速度 (1 - 5)",
            ["TooltipThemeToggle"] = "テーマを切り替え",
            ["TooltipComments"] = "ノートとコメントドロワー",
            ["TooltipExportPdf"] = "印刷 / PDF にエクスポート",
            ["TooltipExportHtml"] = "単一のオフライン HTML にエクスポート",
            ["TooltipFocus"] = "全画面集中モード (F11)",
            ["TooltipZoomIn"] = "拡大 (Ctrl++)",
            ["TooltipZoomOut"] = "縮小 (Ctrl+-)",
            ["TooltipResetZoom"] = "100% にリセット (Ctrl+0)",
            ["TooltipCopyPath"] = "クリックしてパスをコピー",
            ["TooltipHidePath"] = "パスバーを隠す",
            ["TooltipShowPath"] = "パスバーを表示",
            ["TextPathCopied"] = "✓ パスをコピーしました",
            ["SearchPlaceholder"] = "クイック検索または JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "最近のファイルはありません",
            ["ClearHistory"] = "履歴を消去",
            ["DashboardFavoritesTitle"] = "お気に入りと最近のファイル",
            ["CopyAll"] = "全文をコピー  (Ctrl+C)",
            ["StatusWords"] = "単語",
            ["StatusLines"] = "行",
            ["StatusMinRead"] = "分で読了",
            ["DrawerOutlineTitle"] = "ドキュメントの目次",
            ["DrawerCommentsTitle"] = "ノートとコメント",
            ["CommentPlaceholder"] = "コメントやメモを入力...",
            ["ButtonAddComment"] = "+ コメントを追加",
            ["ButtonClearAll"] = "すべて消去",
        },

        // 10. Portuguese (Português)
        ["pt"] = new()
        {
            ["MenuFile"] = "Arquivo",
            ["MenuNew"] = "Novo  (Ctrl+N)",
            ["MenuOpenFile"] = "Abrir arquivo...  (Ctrl+O)",
            ["MenuOpenFolder"] = "Abrir pasta...  (Ctrl+Shift+O)",
            ["MenuRecentFiles"] = "Arquivos recentes",
            ["MenuSave"] = "Salvar  (Ctrl+S)",
            ["MenuSaveAs"] = "Salvar como...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Fechar aba  (Ctrl+W)",
            ["MenuView"] = "Exibir",
            ["MenuCommandPalette"] = "Paleta de comandos...  (Ctrl+Shift+P / F2)",
            ["MenuToggleSidebar"] = "Alternar barra lateral  (Ctrl+B)",
            ["MenuTypewriter"] = "Modo máquina de escrever",
            ["MenuRefresh"] = "Atualizar  (Ctrl+R)",
            ["MenuFind"] = "Localizar no arquivo...  (Ctrl+F)",
            ["MenuReplace"] = "Substituir...  (Ctrl+H)",
            ["MenuSettings"] = "Configurações",
            ["MenuTheme"] = "Tema",
            ["MenuThemeSystem"] = "Sistema",
            ["MenuThemeLight"] = "Claro",
            ["MenuThemeDark"] = "Escuro",
            ["MenuThemeDarkSlate"] = "Ardósia escura",
            ["MenuThemeSepia"] = "Sépia",
            ["MenuPreferences"] = "Preferências e automação",
            ["MenuAutoSave"] = "Salvar rascunhos automaticamente",
            ["MenuRestoreSession"] = "Restaurar abas da sessão anterior",
            ["MenuLanguage"] = "Idioma",
            ["MenuHelp"] = "Ajuda",
            ["MenuMarkdownGuide"] = "Guia do Markdown",
            ["MenuJsonGuide"] = "Guia do JSON e JSONPath",
            ["MenuShortcuts"] = "Atalhos de teclado  (F1)",
            ["MenuAbout"] = "Sobre o JMDReader",
            ["TabPin"] = "Fixar nos favoritos",
            ["TabUnpin"] = "Desafixar aba",
            ["TabFavorite"] = "Adicionar aos favoritos",
            ["TabClose"] = "Fechar aba  (Ctrl+W)",
            ["TabCloseRight"] = "Fechar abas à direita",
            ["TabCloseOthers"] = "Fechar outras abas",
            ["TabCloseAll"] = "Fechar todas as abas",
            ["JsonExpandAll"] = "Expandir tudo",
            ["JsonCollapseAll"] = "Recolher tudo",
            ["JsonFormat"] = "Formatar",
            ["JsonMinify"] = "Compactar",
            ["JsonTable"] = "Tabela",
            ["JsonExport"] = "Exportar HTML",
            ["JsonCopy"] = "Copiar bruto",
            ["TooltipSplit"] = "Visualização dividida (Editor + Prévia)",
            ["TooltipEditor"] = "Apenas editor",
            ["TooltipRead"] = "Modo de leitura sem distrações",
            ["TooltipBold"] = "Negrito (**texto**)",
            ["TooltipItalic"] = "Itálico (*texto*)",
            ["TooltipHeading"] = "Título (# Título)",
            ["TooltipCode"] = "Bloco de código (```)",
            ["TooltipMermaid"] = "Inserir diagrama Mermaid",
            ["TooltipAlert"] = "Inserir alerta ([!NOTE])",
            ["TooltipMath"] = "Inserir fórmula matemática ($$)",
            ["TooltipOutline"] = "Índice do documento",
            ["TooltipAutoScroll"] = "Rolagem contínua suave",
            ["TooltipAutoScrollSpeed"] = "Velocidade de rolagem (1 - 5)",
            ["TooltipThemeToggle"] = "Alternar tema",
            ["TooltipComments"] = "Notas e comentários",
            ["TooltipExportPdf"] = "Imprimir / Exportar para PDF",
            ["TooltipExportHtml"] = "Exportar HTML independente offline",
            ["TooltipFocus"] = "Modo de foco em tela cheia (F11)",
            ["TooltipZoomIn"] = "Aumentar zoom (Ctrl++)",
            ["TooltipZoomOut"] = "Diminuir zoom (Ctrl+-)",
            ["TooltipResetZoom"] = "Redefinir zoom 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "Copiar caminho do arquivo",
            ["TooltipHidePath"] = "Ocultar barra de caminho",
            ["TooltipShowPath"] = "Mostrar barra de caminho",
            ["TextPathCopied"] = "✓ Caminho copiado",
            ["SearchPlaceholder"] = "Busca rápida ou JSONPath (Ctrl+F)...",
            ["NoRecentFiles"] = "Nenhum arquivo recente",
            ["ClearHistory"] = "Limpar histórico",
            ["DashboardFavoritesTitle"] = "FAVORITOS E ARQUIVOS RECENTES",
            ["CopyAll"] = "Copiar texto completo  (Ctrl+C)",
            ["StatusWords"] = "palavras",
            ["StatusLines"] = "linhas",
            ["StatusMinRead"] = "min de leitura",
            ["DrawerOutlineTitle"] = "ÍNDICE DO DOCUMENTO",
            ["DrawerCommentsTitle"] = "NOTAS E COMENTÁRIOS",
            ["CommentPlaceholder"] = "Escreva uma nota ou comentário...",
            ["ButtonAddComment"] = "+ Adicionar comentário",
            ["ButtonClearAll"] = "Limpar tudo",
        }
    };
}
