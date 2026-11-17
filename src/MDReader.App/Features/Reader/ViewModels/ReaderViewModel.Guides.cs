// MIT License - see LICENSE file in the repository root.

using CommunityToolkit.Mvvm.Input;
using MDReader.Core.Documents;

namespace MDReader.App.Features.Reader.ViewModels;

public sealed partial class ReaderViewModel
{
    /// <summary>
    /// Indicates whether the active document is a JSON or JSONC/JSONL document.
    /// </summary>
    public bool IsCurrentDocumentJson => SelectedTab?.IsJsonDocument ?? false;

    /// <summary>
    /// Indicates whether the active document is a Markdown document.
    /// </summary>
    public bool IsCurrentDocumentMarkdown => !IsCurrentDocumentJson;

    /// <summary>
    /// Controls visibility of Markdown text formatting buttons (B, I, H, Code, etc.).
    /// </summary>
    public bool IsMarkdownFormattingVisible => IsEditorVisible && IsCurrentDocumentMarkdown;

    [RelayCommand]
    public void BeautifyJson()
    {
        if (SelectedTab == null || string.IsNullOrWhiteSpace(DocumentContent)) return;
        var formatted = MDReader.Core.Json.JsonFormatUtilities.Beautify(DocumentContent, 2);
        DocumentContent = formatted;
        ShowError("✓ Beautified JSON (2 Spaces)");
    }

    [RelayCommand]
    public void MinifyJson()
    {
        if (SelectedTab == null || string.IsNullOrWhiteSpace(DocumentContent)) return;
        var min = MDReader.Core.Json.JsonFormatUtilities.Minify(DocumentContent);
        DocumentContent = min;
        ShowError("✓ Minified JSON (Single Line)");
    }

    [RelayCommand]
    public void ConvertJsonToTable()
    {
        if (SelectedTab == null || string.IsNullOrWhiteSpace(DocumentContent)) return;
        var table = MDReader.Core.Json.JsonFormatUtilities.ConvertJsonToMarkdownTable(DocumentContent);

        try
        {
            var dp = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dp.SetText(table);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dp);
            ShowError("✓ Markdown Table copied to clipboard!");
        }
        catch { }
    }

    [RelayCommand]
    public void OpenMarkdownGuide()
    {
        var guideContent = GetMarkdownGuideContent();
        var session = DocumentSession.CreateNew("Markdown-Guide.md", guideContent);
        var tab = new DocumentTabViewModel(session);
        Tabs.Add(tab);
        SelectedTab = tab;
    }

    [RelayCommand]
    public void OpenJsonGuide()
    {
        var guideContent = GetJsonGuideContent();
        var session = DocumentSession.CreateNew("JSON-Guide.json", guideContent);
        var tab = new DocumentTabViewModel(session);
        Tabs.Add(tab);
        SelectedTab = tab;
    }

    private static string GetMarkdownGuideContent() => """
    # 📖 دليل صياغة وكتابة الماركداون (Markdown Syntax Guide)

    مرحباً بك في **JMDReader**! هذا المستند تفاعلي حي، يمكنك تعديله في جزء المحرر ومشاهدة النتيجة فوراً في جزء القراءة.

    ---

    ## 1. العناوين (Headings)
    # عنوان رئيسي (H1)
    ## عنوان فرعي (H2)
    ### عنوان قسم (H3)
    #### عنوان فرعي (H4)
    ##### عنوان تفصيلي (H5)
    ###### أصغر عنوان (H6)

    ---

    ## 2. تنسيقات النصوص (Text Formatting)
    - **نص عريض (Bold):** `**نص عريض**`
    - *نص مائل (Italic):* `*نص مائل*`
    - ***عريض ومائل معاً:*** `***عريض ومائل***`
    - ~~نص مشطوب (Strikethrough):~~ `~~نص مشطوب~~`
    - `كود مضمن (Inline Code):` `` `كود مضمن` ``

    ---

    ## 3. القوائم والمهام (Lists & Task Checkboxes)
    ### قائمة نقطية:
    * فكرة أولى
    * فكرة ثانية
      * فكرة متداخلة

    ### قائمة مرقمة:
    1. الخطوة الأولى
    2. الخطوة الثانية
    3. الخطوة الثالثة

    ### قائمة مهام تفاعلية (Task Lists):
    - [x] تثبيت برنامج JMDReader
    - [x] تجربة وضع القراءة المريح
    - [ ] تجربة مستعرض JSON الشجري

    ---

    ## 4. الجداول (GFM Tables)
    | الميزة / Feature | الوصف / Description | الاختصار / Shortcut |
    |---|---|---|
    | فتح ملف | فتح ملفات .md و .json و .txt | Ctrl + O |
    | حفظ التعديلات | حفظ المستند بأمان ذري | Ctrl + S |
    | البحث والاستبدال | تلوين فوري في النص والشجرة | Ctrl + F / Ctrl + H |
    | وضع التركيز (Zen) | إخفاء القوائم وتكبير مساحة القراءة | F11 |

    ---

    ## 5. كتل الأكواد البرمجية (Code Blocks)
    ```csharp
    // كود C# مع تلوين نحوي وزر نسخ بنقرة واحدة
    public void SayHello()
    {
        Console.WriteLine("Hello from JMDReader!");
    }
    ```

    ```python
    # كود بايثون
    def calculate_fibonacci(n):
        a, b = 0, 1
        for _ in range(n):
            yield a
            a, b = b, a + b
    ```

    ---

    ## 6. المعادلات الرياضية (LaTeX / KaTeX Math)
    معادلة في نفس السطر: $E = mc^2$

    معادلة مخصصة في كتلة منفصلة:
    $$
    f(x) = \int_{-\infty}^{\infty} \hat{f}(\xi)\,e^{2 \pi i \xi x}\,d\xi
    $$

    ---

    ## 7. تنبيهات واقتباسات GitHub (Alerts & Blockquotes)
    > [!NOTE]
    > هذا تنبيه للملاحظات الهامة والمعلومات الأساسية.

    > [!TIP]
    > نصيحة: يمكنك سحب أي تاب وإفلاته في المتصفح أو أي دردشة لنقل الملف فوراً.

    > [!WARNING]
    > تحذير: لا تقم بمشاركة مفاتيحك السرية داخل ملفات المستندات العامة.

    ---

    ## 8. الرسوم البيانية التفاعلية (Mermaid Diagrams)
    ```mermaid
    flowchart LR
        A[ملف Markdown أو JSON] --> B(محرك JMDReader)
        B --> C{نوع العرض}
        C -->|Markdown| D[عرض القراءة الغني]
        C -->|JSON| E[مستعرض شجري ملون]
    ```
    """;

    private static string GetJsonGuideContent() => """
    {
      "title": "دليل وفحص صياغة JSON (Interactive JSON Guide)",
      "application": "JMDReader v2.0",
      "publisher": {
        "name": "birotech",
        "author": "Abdullah Ali Khaled Moqbel",
        "license": "MIT",
        "offline": true
      },
      "supported_formats": [
        ".json (Standard JSON)",
        ".jsonc (JSON with Comments)",
        ".jsonl (JSON Lines)",
        ".md (Markdown)",
        ".txt (Plain Text)"
      ],
      "data_types_demonstration": {
        "string_example": "نص عربي UTF-8 نقي بدون ترميز uXXXX",
        "integer_number": 2026,
        "float_number": 99.95,
        "boolean_true": true,
        "boolean_false": false,
        "null_value": null
      },
      "jsonpath_query_cheatsheet": {
        "root": "$ (يعيد الكائن الجذري بالكامل)",
        "property_dot": "$.publisher.name (استخراج الخاصية بالنقطة)",
        "property_bracket": "$['publisher']['name'] (استخراج الخاصية بالأقواس)",
        "array_index": "$.supported_formats[0] (أول عنصر في المصفوفة)",
        "array_last": "$.supported_formats[-1] (آخر عنصر في المصفوفة)",
        "array_wildcard": "$.supported_formats[*] (جميع عناصر المصفوفة)",
        "recursive_search": "$..license (بحث شامل عن خاصية license أينما وجدت في الشجرة)"
      },
      "productivity_features": [
        {
          "name": "✨ Beautify",
          "description": "تنسيق الكود بمسافتين مع سطر جديد لكل خاصية"
        },
        {
          "name": "⚡ Minify",
          "description": "ضغط الكود وإزالة كافة الفراغات والأسطر لتقليل الحجم"
        },
        {
          "name": "📊 To Table",
          "description": "تحويل مصفوفات الكائنات إلى جدول ماركداون متناسق بنقرة واحدة"
        },
        {
          "name": "💾 Export HTML",
          "description": "تصدير صفحة ويب مستقلة تفاعلية تعمل أوفلاين مع حفظ الثيم والشجرة"
        }
      ]
    }
    """;
}
