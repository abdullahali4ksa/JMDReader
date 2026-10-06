# MDReader — Demo Document

مرحبًا! هذا ملف تجريبي لاختبار قارئ Markdown.

## 1. تنسيق النص (Text formatting)

هذا **نص عريض**، وهذا *نص مائل*، وهذا ~~نص مشطوب~~، وهذا `كود داخلي`.

## 2. العناوين والقوائم (Headings & Lists)

### قائمة نقطية
- العنصر الأول
- العنصر الثاني
  - عنصر متداخل
- العنصر الثالث

### قائمة مرقّمة
1. الخطوة الأولى
2. الخطوة الثانية
3. الخطوة الثالثة

### قائمة مهام (GFM Task List)
- [x] مهمة مكتملة
- [ ] مهمة قيد الانتظار
- [x] مهمة أخرى مكتملة

## 3. الاقتباس (Blockquote)

> القراءة غذاء العقل.
> Knowledge is power.

## 4. كتلة الكود (Code Block)

```csharp
public sealed class ShellViewModel : ObservableObject
{
    private string _windowTitle = "MDReader";
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value);
    }
}
```

## 5. الجدول (Table)

| الميزة | الحالة | الملاحظات |
|---|---|---|
| فتح الملفات | ✅ | منتقي ملفات + سطر الأوامر |
| عرض Markdown | ✅ | Markdig |
| دعم العربية | ✅ | RTL تلقائي |
| الصور المحلية | ✅ | آمنة داخل مجلد المستند |
| الروابط | ✅ | HTTP(S) فقط |

## 6. الروابط (Links)

- رابط خارجي: [Nous Research](https://nousresearch.com)
- رابط داخلي: [اذهب إلى القسم 1](#1-تنسيق-النص-text-formatting)

## 7. نص مختلط (Mixed direction)

This paragraph is English and should read left-to-right.
هذه فقرة عربية ويجب أن تُقرأ من اليمين إلى اليسار.
Mixed: كلمة English داخل جملة عربية.

---

*نهاية الملف التجريبي.*
