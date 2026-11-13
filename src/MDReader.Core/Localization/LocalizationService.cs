// MIT License - see LICENSE file in the repository root.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MDReader.Core.Localization;

/// <summary>
/// Provides multi-language localization for MDReader supporting the top 10 global languages:
/// Arabic (العربية), English, Chinese (中文), Hindi (हिन्दी), Spanish (Español),
/// French (Français), German (Deutsch), Russian (Русский), Japanese (日本語), Portuguese (Português).
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
        ("zh", "Chinese", "中文 (简体)"),
        ("hi", "Hindi", "हिन्दी"),
        ("es", "Spanish", "Español"),
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
        // Initialized with Arabic by default
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
    public string MenuSave => Get("MenuSave", "Save  (Ctrl+S)");
    public string MenuSaveAs => Get("MenuSaveAs", "Save As...  (Ctrl+Shift+S)");
    public string MenuCloseTab => Get("MenuCloseTab", "Close Tab  (Ctrl+W)");

    public string MenuView => Get("MenuView", "View");
    public string MenuToggleSidebar => Get("MenuToggleSidebar", "Toggle Sidebar  (Ctrl+B)");
    public string MenuRefresh => Get("MenuRefresh", "Refresh  (Ctrl+R)");
    public string MenuFind => Get("MenuFind", "Find  (Ctrl+F)");

    public string MenuSettings => Get("MenuSettings", "Settings");
    public string MenuTheme => Get("MenuTheme", "Theme / المظهر");
    public string MenuThemeLight => Get("MenuThemeLight", "Light (أبيض)");
    public string MenuThemeDark => Get("MenuThemeDark", "Dark (داكن)");
    public string MenuThemeSepia => Get("MenuThemeSepia", "Sepia (مريح للقراءة)");
    public string MenuLanguage => Get("MenuLanguage", "🌐 Language / اللغة");
    public string MenuShortcuts => Get("MenuShortcuts", "Keyboard Shortcuts  (F1)");

    public string TooltipSplit => Get("TooltipSplit", "Split View (محرر + معاينة)");
    public string TooltipEditor => Get("TooltipEditor", "Editor Only (محرر فقط)");
    public string TooltipRead => Get("TooltipRead", "Reading Mode (وضع القراءة الكامل)");

    public string TooltipBold => Get("TooltipBold", "Bold / عريض (**text**)");
    public string TooltipItalic => Get("TooltipItalic", "Italic / مائل (*text*)");
    public string TooltipHeading => Get("TooltipHeading", "Heading / عنوان (# Title)");
    public string TooltipCode => Get("TooltipCode", "Code Block / كتلة كود (```)");
    public string TooltipMermaid => Get("TooltipMermaid", "Mermaid Diagram / رسم هيكلي");
    public string TooltipAlert => Get("TooltipAlert", "Alert Callout / صندوق تنبيه ([!NOTE])");
    public string TooltipMath => Get("TooltipMath", "Math Formula / معادلة رياضية ($$)");

    public string TooltipOutline => Get("TooltipOutline", "Outline / فهرس العناوين");
    public string TooltipAutoScroll => Get("TooltipAutoScroll", "Smooth Auto-Scroll / تحريك انسيابي بدون لمس");
    public string TooltipAutoScrollSpeed => Get("TooltipAutoScrollSpeed", "Auto-Scroll Speed / سرعة التحريك الانسيابي (1 - 5)");
    public string TooltipThemeToggle => Get("TooltipThemeToggle", "Cycle Theme / تبديل المظهر (داكن / فاتح / دافئ)");
    public string TooltipComments => Get("TooltipComments", "Comments Drawer / درج الملاحظات والتعليقات");
    public string TooltipExportPdf => Get("TooltipExportPdf", "Export to PDF / طباعة وتصدير كملف PDF");
    public string TooltipExportHtml => Get("TooltipExportHtml", "Export to HTML / تصدير صفحة ويب كاملة دون إنترنت");
    public string TooltipFocus => Get("TooltipFocus", "Focus Mode / وضع التركيز التام (F11)");

    public string TooltipZoomIn => Get("TooltipZoomIn", "Zoom In / تكبير (Ctrl++)");
    public string TooltipZoomOut => Get("TooltipZoomOut", "Zoom Out / تصغير (Ctrl+-)");
    public string TooltipResetZoom => Get("TooltipResetZoom", "Reset Zoom / إعادة ضبط 100% (Ctrl+0)");

    public string TooltipCopyPath => Get("TooltipCopyPath", "Click to copy full path / انقر لنسخ المسار الكامل");
    public string TooltipHidePath => Get("TooltipHidePath", "Hide path bar / إخفاء شريط المسار");
    public string TooltipShowPath => Get("TooltipShowPath", "Show path bar / إظهار شريط المسار");
    public string TextPathCopied => Get("TextPathCopied", "✓ تم نسخ المسار");

    public string StatusWords => Get("StatusWords", "words");
    public string StatusLines => Get("StatusLines", "lines");
    public string StatusMinRead => Get("StatusMinRead", "min read");

    public string DrawerOutlineTitle => Get("DrawerOutlineTitle", "DOCUMENT OUTLINE / فهرس العناوين");
    public string DrawerCommentsTitle => Get("DrawerCommentsTitle", "NOTES & COMMENTS / التعليقات والملاحظات");
    public string CommentPlaceholder => Get("CommentPlaceholder", "Write a note or comment on selected text...");
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
            ["MenuSave"] = "حفظ  (Ctrl+S)",
            ["MenuSaveAs"] = "حفظ باسم...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "إغلاق التبويب  (Ctrl+W)",
            ["MenuView"] = "عرض",
            ["MenuToggleSidebar"] = "إظهار/إخفاء الشريط الجانبي  (Ctrl+B)",
            ["MenuRefresh"] = "تحديث  (Ctrl+R)",
            ["MenuFind"] = "بحث  (Ctrl+F)",
            ["MenuSettings"] = "الإعدادات",
            ["MenuTheme"] = "🎨 المظهر / Theme",
            ["MenuThemeLight"] = "☀️ أبيض (فاتح)",
            ["MenuThemeDark"] = "🌙 داكن (أسود)",
            ["MenuThemeSepia"] = "📜 سيبيا (بيج مريح للقراءة)",
            ["MenuLanguage"] = "🌐 اللغة / Language",
            ["MenuShortcuts"] = "⌨️ اختصارات لوحة المفاتيح  (F1)",
            ["TooltipSplit"] = "تقسيم الشاشة (محرر + معاينة)",
            ["TooltipEditor"] = "محرر النصوص فقط",
            ["TooltipRead"] = "وضع القراءة الخالي من التشتيت",
            ["TooltipBold"] = "عريض (**نص**)",
            ["TooltipItalic"] = "مائل (*نص*)",
            ["TooltipHeading"] = "عنوان (# عنوان)",
            ["TooltipCode"] = "كتلة برمجة (```)",
            ["TooltipMermaid"] = "إدراج مخطط هيكلي Mermaid",
            ["TooltipAlert"] = "إدراج صندوق تنبيه ([!NOTE])",
            ["TooltipMath"] = "إدراج معادلة رياضية ($$)",
            ["TooltipOutline"] = "فهرس العناوين التفاعلي",
            ["TooltipAutoScroll"] = "تحريك انسيابي مستمر بدون لمس",
            ["TooltipAutoScrollSpeed"] = "سرعة التحريك الانسيابي (1 - 5)",
            ["TooltipThemeToggle"] = "تبديل مظهر القراءة (داكن / فاتح / سيبيا)",
            ["TooltipComments"] = "درج الملاحظات والتعليقات",
            ["TooltipExportPdf"] = "طباعة وتصدير كملف PDF",
            ["TooltipExportHtml"] = "تصدير كصفحة ويب كاملة دون إنترنت (HTML)",
            ["TooltipFocus"] = "وضع التركيز بملء الشاشة (F11)",
            ["TooltipZoomIn"] = "تكبير المستند (Ctrl++)",
            ["TooltipZoomOut"] = "تصغير المستند (Ctrl+-)",
            ["TooltipResetZoom"] = "إعادة ضبط التكبير 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "انقر لنسخ المسار الكامل للمستند",
            ["TooltipHidePath"] = "إخفاء شريط المسار بالكامل",
            ["TooltipShowPath"] = "إظهار شريط مسار الملف",
            ["TextPathCopied"] = "✓ تم نسخ المسار",
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
            ["MenuSave"] = "Save  (Ctrl+S)",
            ["MenuSaveAs"] = "Save As...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Close Tab  (Ctrl+W)",
            ["MenuView"] = "View",
            ["MenuToggleSidebar"] = "Toggle Sidebar  (Ctrl+B)",
            ["MenuRefresh"] = "Refresh  (Ctrl+R)",
            ["MenuFind"] = "Find  (Ctrl+F)",
            ["MenuSettings"] = "Settings",
            ["MenuTheme"] = "🎨 Theme",
            ["MenuThemeLight"] = "☀️ Light",
            ["MenuThemeDark"] = "🌙 Dark",
            ["MenuThemeSepia"] = "📜 Sepia (Warm Reading)",
            ["MenuLanguage"] = "🌐 Language",
            ["MenuShortcuts"] = "⌨️ Keyboard Shortcuts  (F1)",
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
            ["TooltipAutoScroll"] = "Continuous Teleprompter Auto-Scroll",
            ["TooltipAutoScrollSpeed"] = "Continuous Scroll Speed (1 - 5)",
            ["TooltipThemeToggle"] = "Cycle Reading Theme (Dark / Light / Sepia)",
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
            ["StatusWords"] = "words",
            ["StatusLines"] = "lines",
            ["StatusMinRead"] = "min read",
            ["DrawerOutlineTitle"] = "DOCUMENT OUTLINE",
            ["DrawerCommentsTitle"] = "NOTES & COMMENTS",
            ["CommentPlaceholder"] = "Write a note or comment on selected text...",
            ["ButtonAddComment"] = "+ Add Comment",
            ["ButtonClearAll"] = "Clear All",
        },

        // 3. Chinese (中文 简体)
        ["zh"] = new()
        {
            ["MenuFile"] = "文件",
            ["MenuNew"] = "新建  (Ctrl+N)",
            ["MenuOpenFile"] = "打开文件...  (Ctrl+O)",
            ["MenuOpenFolder"] = "打开文件夹...  (Ctrl+Shift+O)",
            ["MenuSave"] = "保存  (Ctrl+S)",
            ["MenuSaveAs"] = "另存为...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "关闭标签页  (Ctrl+W)",
            ["MenuView"] = "视图",
            ["MenuToggleSidebar"] = "切换侧边栏  (Ctrl+B)",
            ["MenuRefresh"] = "刷新  (Ctrl+R)",
            ["MenuFind"] = "查找  (Ctrl+F)",
            ["MenuSettings"] = "设置",
            ["MenuTheme"] = "🎨 主题",
            ["MenuThemeLight"] = "☀️ 明亮 (浅色)",
            ["MenuThemeDark"] = "🌙 暗黑 (深色)",
            ["MenuThemeSepia"] = "📜 护眼 (羊皮纸)",
            ["MenuLanguage"] = "🌐 语言 / Language",
            ["MenuShortcuts"] = "⌨️ 快捷键列表  (F1)",
            ["TooltipSplit"] = "分屏视图 (编辑器 + 预览)",
            ["TooltipEditor"] = "纯编辑器视图",
            ["TooltipRead"] = "无干扰沉浸阅读模式",
            ["TooltipBold"] = "粗体 (**文本**)",
            ["TooltipItalic"] = "斜体 (*文本*)",
            ["TooltipHeading"] = "标题 (# 标题)",
            ["TooltipCode"] = "代码块 (```)",
            ["TooltipMermaid"] = "插入 Mermaid 图表",
            ["TooltipAlert"] = "插入警告提示框 ([!NOTE])",
            ["TooltipMath"] = "插入数学公式 ($$)",
            ["TooltipOutline"] = "文档大纲目录",
            ["TooltipAutoScroll"] = "平滑连续自动滚动",
            ["TooltipAutoScrollSpeed"] = "平滑滚动速度 (1 - 5)",
            ["TooltipThemeToggle"] = "切换主题模式 (深色/浅色/护眼)",
            ["TooltipComments"] = "批注与笔记抽屉",
            ["TooltipExportPdf"] = "打印或导出为 PDF",
            ["TooltipExportHtml"] = "导出离线单页 HTML",
            ["TooltipFocus"] = "全屏专注模式 (F11)",
            ["TooltipZoomIn"] = "放大 (Ctrl++)",
            ["TooltipZoomOut"] = "缩小 (Ctrl+-)",
            ["TooltipResetZoom"] = "重置缩放 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "点击复制文件完整路径",
            ["TooltipHidePath"] = "隐藏路径栏",
            ["TooltipShowPath"] = "显示路径栏",
            ["TextPathCopied"] = "✓ 已复制路径",
            ["StatusWords"] = "字",
            ["StatusLines"] = "行",
            ["StatusMinRead"] = "分钟阅读",
            ["DrawerOutlineTitle"] = "文档大纲",
            ["DrawerCommentsTitle"] = "笔记与批注",
            ["CommentPlaceholder"] = "输入您的批注或笔记...",
            ["ButtonAddComment"] = "+ 添加批注",
            ["ButtonClearAll"] = "清空全部",
        },

        // 4. Hindi (हिन्दी)
        ["hi"] = new()
        {
            ["MenuFile"] = "फ़ाइल",
            ["MenuNew"] = "नया  (Ctrl+N)",
            ["MenuOpenFile"] = "फ़ाइल खोलें...  (Ctrl+O)",
            ["MenuOpenFolder"] = "फ़ोल्डर खोलें...  (Ctrl+Shift+O)",
            ["MenuSave"] = "सहेजें  (Ctrl+S)",
            ["MenuSaveAs"] = "इस रूप में सहेजें...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "टैब बंद करें  (Ctrl+W)",
            ["MenuView"] = "देखें",
            ["MenuToggleSidebar"] = "साइडबार दिखाएं/छिपाएं  (Ctrl+B)",
            ["MenuRefresh"] = "ताज़ा करें  (Ctrl+R)",
            ["MenuFind"] = "खोजें  (Ctrl+F)",
            ["MenuSettings"] = "सेटिंग्स",
            ["MenuTheme"] = "🎨 थीम",
            ["MenuThemeLight"] = "☀️ लाइट",
            ["MenuThemeDark"] = "🌙 डार्क",
            ["MenuThemeSepia"] = "📜 सेपिया (पठन हेतु)",
            ["MenuLanguage"] = "🌐 भाषा / Language",
            ["MenuShortcuts"] = "⌨️ कीबोर्ड शॉर्टकट  (F1)",
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
            ["StatusWords"] = "शब्द",
            ["StatusLines"] = "पंक्तियाँ",
            ["StatusMinRead"] = "मिनट पठन",
            ["DrawerOutlineTitle"] = "दस्तावेज़ की रूपरेखा",
            ["DrawerCommentsTitle"] = "नोट्स और टिप्पणियां",
            ["CommentPlaceholder"] = "अपनी टिप्पणी लिखें...",
            ["ButtonAddComment"] = "+ टिप्पणी जोड़ें",
            ["ButtonClearAll"] = "सभी हटाएं",
        },

        // 5. Spanish (Español)
        ["es"] = new()
        {
            ["MenuFile"] = "Archivo",
            ["MenuNew"] = "Nuevo  (Ctrl+N)",
            ["MenuOpenFile"] = "Abrir archivo...  (Ctrl+O)",
            ["MenuOpenFolder"] = "Abrir carpeta...  (Ctrl+Shift+O)",
            ["MenuSave"] = "Guardar  (Ctrl+S)",
            ["MenuSaveAs"] = "Guardar como...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Cerrar pestaña  (Ctrl+W)",
            ["MenuView"] = "Ver",
            ["MenuToggleSidebar"] = "Alternar barra lateral  (Ctrl+B)",
            ["MenuRefresh"] = "Actualizar  (Ctrl+R)",
            ["MenuFind"] = "Buscar  (Ctrl+F)",
            ["MenuSettings"] = "Configuración",
            ["MenuTheme"] = "🎨 Tema",
            ["MenuThemeLight"] = "☀️ Claro",
            ["MenuThemeDark"] = "🌙 Oscuro",
            ["MenuThemeSepia"] = "📜 Sepia (Lectura cómoda)",
            ["MenuLanguage"] = "🌐 Idioma / Language",
            ["MenuShortcuts"] = "⌨️ Atajos de teclado  (F1)",
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
            ["TooltipOutline"] = "Índice del documento",
            ["TooltipAutoScroll"] = "Desplazamiento automático fluido",
            ["TooltipAutoScrollSpeed"] = "Velocidad de desplazamiento (1 - 5)",
            ["TooltipThemeToggle"] = "Cambiar tema de lectura",
            ["TooltipComments"] = "Panel de notas y comentarios",
            ["TooltipExportPdf"] = "Imprimir / Exportar a PDF",
            ["TooltipExportHtml"] = "Exportar HTML independiente sin conexión",
            ["TooltipFocus"] = "Modo de enfoque en pantalla completa (F11)",
            ["TooltipZoomIn"] = "Acercar (Ctrl++)",
            ["TooltipZoomOut"] = "Alejar (Ctrl+-)",
            ["TooltipResetZoom"] = "Restablecer al 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "Clic para copiar ruta completa",
            ["TooltipHidePath"] = "Ocultar barra de ruta",
            ["TooltipShowPath"] = "Mostrar barra de ruta",
            ["TextPathCopied"] = "✓ Ruta copiada",
            ["StatusWords"] = "palabras",
            ["StatusLines"] = "líneas",
            ["StatusMinRead"] = "min de lectura",
            ["DrawerOutlineTitle"] = "ÍNDICE DEL DOCUMENTO",
            ["DrawerCommentsTitle"] = "NOTAS Y COMENTARIOS",
            ["CommentPlaceholder"] = "Escribe una nota o comentario...",
            ["ButtonAddComment"] = "+ Añadir comentario",
            ["ButtonClearAll"] = "Borrar todo",
        },

        // 6. French (Français)
        ["fr"] = new()
        {
            ["MenuFile"] = "Fichier",
            ["MenuNew"] = "Nouveau  (Ctrl+N)",
            ["MenuOpenFile"] = "Ouvrir un fichier...  (Ctrl+O)",
            ["MenuOpenFolder"] = "Ouvrir un dossier...  (Ctrl+Shift+O)",
            ["MenuSave"] = "Enregistrer  (Ctrl+S)",
            ["MenuSaveAs"] = "Enregistrer sous...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Fermer l'onglet  (Ctrl+W)",
            ["MenuView"] = "Affichage",
            ["MenuToggleSidebar"] = "Afficher/Masquer le volet latéral  (Ctrl+B)",
            ["MenuRefresh"] = "Actualiser  (Ctrl+R)",
            ["MenuFind"] = "Rechercher  (Ctrl+F)",
            ["MenuSettings"] = "Paramètres",
            ["MenuTheme"] = "🎨 Thème",
            ["MenuThemeLight"] = "☀️ Clair",
            ["MenuThemeDark"] = "🌙 Sombre",
            ["MenuThemeSepia"] = "📜 Sépia (Lecture confortable)",
            ["MenuLanguage"] = "🌐 Langue / Language",
            ["MenuShortcuts"] = "⌨️ Raccourcis clavier  (F1)",
            ["TooltipSplit"] = "Vue fractionnée (Éditeur + Aperçu)",
            ["TooltipEditor"] = "Éditeur seul",
            ["TooltipRead"] = "Mode lecture sans distraction",
            ["TooltipBold"] = "Gras (**texte**)",
            ["TooltipItalic"] = "Italique (*texte*)",
            ["TooltipHeading"] = "Titre (# Titre)",
            ["TooltipCode"] = "Bloc de code (```)",
            ["TooltipMermaid"] = "Insérer un diagramme Mermaid",
            ["TooltipAlert"] = "Insérer une alerte ([!NOTE])",
            ["TooltipMath"] = "Insérer une formule mathématique ($$)",
            ["TooltipOutline"] = "Plan du document interactif",
            ["TooltipAutoScroll"] = "Défilement automatique fluide",
            ["TooltipAutoScrollSpeed"] = "Vitesse de défilement (1 - 5)",
            ["TooltipThemeToggle"] = "Changer de thème de lecture",
            ["TooltipComments"] = "Volet des notes et commentaires",
            ["TooltipExportPdf"] = "Imprimer / Exporter en PDF",
            ["TooltipExportHtml"] = "Exporter en HTML autonome hors ligne",
            ["TooltipFocus"] = "Mode plein écran sans distraction (F11)",
            ["TooltipZoomIn"] = "Zoom avant (Ctrl++)",
            ["TooltipZoomOut"] = "Zoom arrière (Ctrl+-)",
            ["TooltipResetZoom"] = "Réinitialiser à 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "Cliquer pour copier le chemin",
            ["TooltipHidePath"] = "Masquer la barre de chemin",
            ["TooltipShowPath"] = "Afficher la barre de chemin",
            ["TextPathCopied"] = "✓ Chemin copié",
            ["StatusWords"] = "mots",
            ["StatusLines"] = "lignes",
            ["StatusMinRead"] = "min de lecture",
            ["DrawerOutlineTitle"] = "PLAN DU DOCUMENT",
            ["DrawerCommentsTitle"] = "NOTES & COMMENTAIRES",
            ["CommentPlaceholder"] = "Écrivez une note ou un commentaire...",
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
            ["MenuSave"] = "Speichern  (Ctrl+S)",
            ["MenuSaveAs"] = "Speichern unter...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Tab schließen  (Ctrl+W)",
            ["MenuView"] = "Ansicht",
            ["MenuToggleSidebar"] = "Seitenleiste umschalten  (Ctrl+B)",
            ["MenuRefresh"] = "Aktualisieren  (Ctrl+R)",
            ["MenuFind"] = "Suchen  (Ctrl+F)",
            ["MenuSettings"] = "Einstellungen",
            ["MenuTheme"] = "🎨 Design",
            ["MenuThemeLight"] = "☀️ Hell",
            ["MenuThemeDark"] = "🌙 Dunkel",
            ["MenuThemeSepia"] = "📜 Sepia (Augenschonend)",
            ["MenuLanguage"] = "🌐 Sprache / Language",
            ["MenuShortcuts"] = "⌨️ Tastenkombinationen  (F1)",
            ["TooltipSplit"] = "Geteilte Ansicht (Editor + Vorschau)",
            ["TooltipEditor"] = "Nur Editor",
            ["TooltipRead"] = "Ablenkungsfreier Lesemodus",
            ["TooltipBold"] = "Fett (**Text**)",
            ["TooltipItalic"] = "Kursiv (*Text*)",
            ["TooltipHeading"] = "Überschrift (# Titel)",
            ["TooltipCode"] = "Code-Block (```)",
            ["TooltipMermaid"] = "Mermaid-Diagramm einfügen",
            ["TooltipAlert"] = "Hinweisbox einfügen ([!NOTE])",
            ["TooltipMath"] = "Mathematische Formel einfügen ($$)",
            ["TooltipOutline"] = "Interaktives Dokument-Inhaltsverzeichnis",
            ["TooltipAutoScroll"] = "Sanfter kontinuierlicher Bildlauf",
            ["TooltipAutoScrollSpeed"] = "Scroll-Geschwindigkeit (1 - 5)",
            ["TooltipThemeToggle"] = "Design wechseln (Dunkel / Hell / Sepia)",
            ["TooltipComments"] = "Notizen- und Kommentarleiste",
            ["TooltipExportPdf"] = "Als PDF drucken / exportieren",
            ["TooltipExportHtml"] = "Als eigenständiges Offline-HTML exportieren",
            ["TooltipFocus"] = "Vollbild-Fokusmodus (F11)",
            ["TooltipZoomIn"] = "Vergrößern (Ctrl++)",
            ["TooltipZoomOut"] = "Verkleinern (Ctrl+-)",
            ["TooltipResetZoom"] = "Auf 100% zurücksetzen (Ctrl+0)",
            ["TooltipCopyPath"] = "Klicken, um den Pfad zu kopieren",
            ["TooltipHidePath"] = "Pfadleiste ausblenden",
            ["TooltipShowPath"] = "Pfadleiste einblenden",
            ["TextPathCopied"] = "✓ Pfad kopiert",
            ["StatusWords"] = "Wörter",
            ["StatusLines"] = "Zeilen",
            ["StatusMinRead"] = "Min. Lesezeit",
            ["DrawerOutlineTitle"] = "INHALTSVERZEICHNIS",
            ["DrawerCommentsTitle"] = "NOTIZEN & KOMMENTARE",
            ["CommentPlaceholder"] = "Kommentar oder Notiz eingeben...",
            ["ButtonAddComment"] = "+ Kommentar hinzufügen",
            ["ButtonClearAll"] = "Alle löschen",
        },

        // 8. Russian (Русский)
        ["ru"] = new()
        {
            ["MenuFile"] = "Файл",
            ["MenuNew"] = "Создать  (Ctrl+N)",
            ["MenuOpenFile"] = "Открыть файл...  (Ctrl+O)",
            ["MenuOpenFolder"] = "Открыть папку...  (Ctrl+Shift+O)",
            ["MenuSave"] = "Сохранить  (Ctrl+S)",
            ["MenuSaveAs"] = "Сохранить как...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Закрыть вкладку  (Ctrl+W)",
            ["MenuView"] = "Вид",
            ["MenuToggleSidebar"] = "Боковая панель  (Ctrl+B)",
            ["MenuRefresh"] = "Обновить  (Ctrl+R)",
            ["MenuFind"] = "Найти  (Ctrl+F)",
            ["MenuSettings"] = "Настройки",
            ["MenuTheme"] = "🎨 Тема оформления",
            ["MenuThemeLight"] = "☀️ Светлая",
            ["MenuThemeDark"] = "🌙 Тёмная",
            ["MenuThemeSepia"] = "📜 Сепия (для чтения)",
            ["MenuLanguage"] = "🌐 Язык / Language",
            ["MenuShortcuts"] = "⌨️ Горячие клавиши  (F1)",
            ["TooltipSplit"] = "Разделенный экран (Редактор + Просмотр)",
            ["TooltipEditor"] = "Только редактор",
            ["TooltipRead"] = "Режим чтения без отвлечений",
            ["TooltipBold"] = "Жирный (**текст**)",
            ["TooltipItalic"] = "Курсив (*текст*)",
            ["TooltipHeading"] = "Заголовок (# Заголовок)",
            ["TooltipCode"] = "Блок кода (```)",
            ["TooltipMermaid"] = "Вставить диаграмму Mermaid",
            ["TooltipAlert"] = "Вставить блок примечания ([!NOTE])",
            ["TooltipMath"] = "Вставить формулу ($$)",
            ["TooltipOutline"] = "Оглавление документа",
            ["TooltipAutoScroll"] = "Плавная непрерывная автопрокрутка",
            ["TooltipAutoScrollSpeed"] = "Скорость прокрутки (1 - 5)",
            ["TooltipThemeToggle"] = "Сменить тему (Тёмная/Светлая/Сепия)",
            ["TooltipComments"] = "Панель заметок и комментариев",
            ["TooltipExportPdf"] = "Печать / Экспорт в PDF",
            ["TooltipExportHtml"] = "Экспорт в автономный HTML",
            ["TooltipFocus"] = "Полноэкранный режим концентрации (F11)",
            ["TooltipZoomIn"] = "Увеличить (Ctrl++)",
            ["TooltipZoomOut"] = "Уменьшить (Ctrl+-)",
            ["TooltipResetZoom"] = "Сброс масштаба 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "Нажмите для копирования пути",
            ["TooltipHidePath"] = "Скрыть строку пути",
            ["TooltipShowPath"] = "Показать строку пути",
            ["TextPathCopied"] = "✓ Путь скопирован",
            ["StatusWords"] = "слов",
            ["StatusLines"] = "строк",
            ["StatusMinRead"] = "мин. чтения",
            ["DrawerOutlineTitle"] = "ОГЛАВЛЕНИЕ ДОКУМЕНТА",
            ["DrawerCommentsTitle"] = "ЗАМЕТКИ И КОММЕНТАРИИ",
            ["CommentPlaceholder"] = "Напишите заметку или комментарий...",
            ["ButtonAddComment"] = "+ Добавить комментарий",
            ["ButtonClearAll"] = "Очистить всё",
        },

        // 9. Japanese (日本語)
        ["ja"] = new()
        {
            ["MenuFile"] = "ファイル",
            ["MenuNew"] = "新規作成  (Ctrl+N)",
            ["MenuOpenFile"] = "ファイルを開く...  (Ctrl+O)",
            ["MenuOpenFolder"] = "フォルダーを開く...  (Ctrl+Shift+O)",
            ["MenuSave"] = "保存  (Ctrl+S)",
            ["MenuSaveAs"] = "名前を付けて保存...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "タブを閉じる  (Ctrl+W)",
            ["MenuView"] = "表示",
            ["MenuToggleSidebar"] = "サイドバーの切り替え  (Ctrl+B)",
            ["MenuRefresh"] = "再読み込み  (Ctrl+R)",
            ["MenuFind"] = "検索  (Ctrl+F)",
            ["MenuSettings"] = "設定",
            ["MenuTheme"] = "🎨 テーマ",
            ["MenuThemeLight"] = "☀️ ライト",
            ["MenuThemeDark"] = "🌙 ダーク",
            ["MenuThemeSepia"] = "📜 セピア (読書用)",
            ["MenuLanguage"] = "🌐 言語 / Language",
            ["MenuShortcuts"] = "⌨️ キーボードショートカット  (F1)",
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
            ["MenuSave"] = "Salvar  (Ctrl+S)",
            ["MenuSaveAs"] = "Salvar como...  (Ctrl+Shift+S)",
            ["MenuCloseTab"] = "Fechar aba  (Ctrl+W)",
            ["MenuView"] = "Exibir",
            ["MenuToggleSidebar"] = "Alternar barra lateral  (Ctrl+B)",
            ["MenuRefresh"] = "Atualizar  (Ctrl+R)",
            ["MenuFind"] = "Localizar  (Ctrl+F)",
            ["MenuSettings"] = "Configurações",
            ["MenuTheme"] = "🎨 Tema",
            ["MenuThemeLight"] = "☀️ Claro",
            ["MenuThemeDark"] = "🌙 Escuro",
            ["MenuThemeSepia"] = "📜 Sépia (Leitura agradável)",
            ["MenuLanguage"] = "🌐 Idioma / Language",
            ["MenuShortcuts"] = "⌨️ Atalhos de teclado  (F1)",
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
            ["TooltipOutline"] = "Índice do documento interativo",
            ["TooltipAutoScroll"] = "Rolagem contínua suave",
            ["TooltipAutoScrollSpeed"] = "Velocidade de rolagem (1 - 5)",
            ["TooltipThemeToggle"] = "Alternar tema de leitura",
            ["TooltipComments"] = "Painel de notas e comentários",
            ["TooltipExportPdf"] = "Imprimir / Exportar para PDF",
            ["TooltipExportHtml"] = "Exportar HTML independente offline",
            ["TooltipFocus"] = "Modo de foco em tela cheia (F11)",
            ["TooltipZoomIn"] = "Aumentar zoom (Ctrl++)",
            ["TooltipZoomOut"] = "Diminuir zoom (Ctrl+-)",
            ["TooltipResetZoom"] = "Redefinir para 100% (Ctrl+0)",
            ["TooltipCopyPath"] = "Clique para copiar o caminho",
            ["TooltipHidePath"] = "Ocultar barra de caminho",
            ["TooltipShowPath"] = "Mostrar barra de caminho",
            ["TextPathCopied"] = "✓ Caminho copiado",
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
