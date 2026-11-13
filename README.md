# MDReader 🚀

<div align="center">

![MDReader Logo](src/MDReader.App/Assets/Square150x150Logo.scale-200.png)

### **قارئ ومحرر ماركداون العصري لنظام ويندوز | Modern Antigravity-Grade Markdown Reader & Editor for Windows**

[![Build Status](https://img.shields.io/badge/Build-0%20Errors%20%7C%200%20Warnings-brightgreen.svg)](https://github.com/MDReader/MDReader)
[![Tests](https://img.shields.io/badge/Tests-77%2F77%20Passed-success.svg)](https://github.com/MDReader/MDReader)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011%20x64-blue.svg)](https://github.com/MDReader/MDReader)
[![Architecture](https://img.shields.io/badge/Architecture-WinUI%203%20%2B%20Offline%20WebView2-orange.svg)](https://github.com/MDReader/MDReader)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

[**تحميل برنامج التثبيت (Setup Wizard)**](https://github.com/MDReader/MDReader/releases) · [**دليل الاستخدام بالعربية**](#-المميزات-الرئيسية) · [**English Guide**](#-english-features--overview) · [**اختصارات لوحة المفاتيح**](#-اختصارات-لوحة-المفاتيح-keyboard-shortcuts)

</div>

---

## 🌟 المميزات الرئيسية (Arabic Highlights)

تم تطوير **MDReader** وفق المعمارية الهجينة الحديثة (**Native WinUI 3 Shell + Offline WebView2 Engine**)، ليقدم تجربة قراءة وتحرير تحاكي بيئة **Google Antigravity** و**GitHub Flavored Markdown** بكفاءة استثنائية وبدون أي اتصال بالإنترنت (100% Offline):

1. **أوضاع العرض الثلاثية المتقدمة:**
   - **وضع الانقسام (◫ Split):** محرر سريع على اليسار ومعاينة حية متزامنة على اليمين.
   - **وضع التحرير الكامل (✏️ Edit):** شاشة تحرير كاملة للتركيز على كتابة وصياغة المستند.
   - **وضع القراءة الخالي من التشتيت (📖 Read):** إخفاء تلقائي لأزرار التنسيق وتوسيع الصفحة لملء 100% من الشاشة بهوامش مريحة ومحاذاة مركزية لكتب المعرفة.

2. **ثلاثة ثيمات قراءة متكيفة:**
   - **☀️ النهاري (Light):** أبيض نقي مع حدود ناعمة مريحة في الغرف المضاءة.
   - **🌙 الليلي (Dark):** خلفية داكنة تحافظ على البطارية وتريح العين ليلاً.
   - **📜 الدافئ (Sepia):** ثيم الورق القديم المائل للبيج الهادئ (`#FBF0D9`) المعتمد في Apple Books وKindle لساعات قراءة طويلة دون إجهاد.

3. **فهرس العناوين التفاعلي (Table of Contents / Outline):**
   - استخراج شجري فوري لكافة العناوين (`H1` إلى `H6`).
   - إبراز العنوان المقروء حالياً أثناء التمرير في الزمن الحقيقي (**Active Heading Spy**).
   - انتقال فوري وسلس بمجرد النقر على أي قسم.

4. **وضع التركيز التام (Zen Mode - F11):**
   - بضغطة زر **F11** يختفي شريط العنوان، وشريط القوائم، وشريط التبويبات بالكامل لتحصل على تجربة قراءة نقية 100%، مع زر عائم للرجوع أو بالضغط على `Escape`.

5. **شريط حالة وإحصاءات القراءة (Reading Analytics):**
   - عداد حي للكلمات، الأسطر، والحروف، مع حساب ذكي للوقت التقديري لإنهاء قراءة المقال (مثلاً: `وقت القراءة: ~4 دقائق`).

6. **نظام الملاحظات والتعليقات الذكي (Comments Drawer):**
   - درج جانبي يفتح بنقرة زر `💬 Comments`.
   - عند تظليل أي نص أو جملة في العارض، يتم التقاطها تلقائياً كاقتباس مع إمكانية إضافة تعليقك وحفظه بالتاريخ والوقت.

7. **التمرير التلقائي الذكي (Auto-Scroll):**
   - قراءة المقالات الطويلة دون لمس الفأرة، مع مؤقت ثوانٍ تفاعلي لتحديد سرعة النزول التلقائي.

8. **دعم متكامل لكافة معايير Antigravity Markdown:**
   - **صناديق التنبيه (GitHub Alerts):** `> [!NOTE]` و `> [!TIP]` و `> [!IMPORTANT]` و `> [!WARNING]` و `> [!CAUTION]`.
   - **المعادلات الرياضية (LaTeX Math):** معادلات مضمنة `$E=mc^2$` وكتل معادلات `$$`.
   - **المخططات الهيكلية (Mermaid Diagrams):** عروض بصرية للرسوم البيانية وتدفق العمليات.
   - **الجداول الاحترافية (GFM & ASCII Tables):** دعم الجداول مع التظليل وتنسيق الأعمدة.
   - **دعم كامل للغة العربية (RTL):** كشف تلقائي للاتجاه وتنسيق الخط العربي الأنيق.

9. **تصدير وطباعة فائقة الدقة (Export to PDF & Standalone HTML):**
   - تصدير فوري إلى ملف **PDF** بجودة فيكتور عالية مع الاحتفاظ بكافة الألوان والمعادلات.
   - تصدير كصفحة ويب **HTML مستقلة** تعمل بدون إنترنت وبكامل التنسيقات.

---

## 💻 English Features & Overview

- **Hybrid Modern Architecture**: WinUI 3 native desktop shell coupled with local offline WebView2 rendering engine.
- **Tri-Mode Workflow**: Instant switching between `Split View (◫)`, `Editor Only (✏️)`, and `Full Reader (📖)`.
- **Tri-Theme Engine**: Pure Light, High-Contrast Dark, and Apple Books / Kindle style Warm Sepia.
- **Dynamic Table of Contents**: Real-time heading extraction with IntersectionObserver active section tracking and instant jump.
- **Zen / Focus Mode (F11)**: Immersive distraction-free reading hiding all chrome, menus, and borders.
- **Document Analytics**: Live word counter, line count, and estimated reading time.
- **Smart Comments Drawer**: Interactive side panel capturing highlighted quotes and timestamped annotations.
- **Hands-Free Auto-Scroll**: Configurable interval scrolling for hands-free reading.
- **Antigravity Markdown Standard**: GitHub callout alerts, LaTeX math formulas, Mermaid diagrams, task lists, and syntax-highlighted code blocks with copy buttons.
- **1-Click Export**: Publication-grade vector PDF printing and offline self-contained HTML export.
- **100% Offline & Private**: Zero cloud dependencies, zero telemetry, zero data egress.

---

## ⌨️ اختصارات لوحة المفاتيح (Keyboard Shortcuts)

| الاختصار | الوظيفة بالعربية | English Action |
|:---|:---|:---|
| `Ctrl + O` | فتح ملف ماركداون جديد | Open Markdown File |
| `Ctrl + S` | حفظ التعديلات على القرص | Save Document |
| `Ctrl + Shift + S` | حفظ الملف باسم جديد | Save As... |
| `Ctrl + N` | فتح تبويب مستند جديد فارغ | New Document Tab |
| `Ctrl + W` | إغلاق التبويب الحالي | Close Active Tab |
| `F11` | تفعيل/إلغاء وضع التركيز التام | Toggle Zen Focus Mode |
| `Escape` | الخروج من وضع التركيز التام | Exit Focus Mode |
| `Ctrl + B` | تبديل شريط مستكشف المجلدات | Toggle Workspace Explorer |
| `Ctrl + F` | البحث داخل المستند | Find Text |
| `Ctrl + +` / `Ctrl + -` | تكبير / تصغير حجم الخط | Zoom In / Zoom Out |
| `Ctrl + 0` | إعادة ضبط حجم العرض (100%) | Reset Zoom to 100% |

---

## 📦 التثبيت والتشغيل (Installation & Setup)

### 1. معالج التثبيت القياسي لويندوز (Recommended Setup Wizard)
حمّل ملف التثبيت المعتمد:
- **`dist/installer/MDReader-Setup-x64.exe`** (حجم الملف: 52 ميجابايت)
- يتضمن تثبيت بنقرات `Next -> Next -> Install`، وضع اختصار على سطح المكتب وقائمة ابدأ، وربط ملفات `.md` و `.markdown` تلقائياً مع خيار "Open with MDReader" عند النقر بزر الفأرة الأيمن.

### 2. حزمة تطبيقات ويندوز الموقعة (Windows App Package - MSIX)
- **`dist/installer/MDReader-x64.msix`** (حجم الملف: 31.8 ميجابايت)
- تثبيت مباشر بضغطة زر عبر **Windows App Installer** لبيئات الشركات والمستخدمين المحترفين.

### 3. عبر مدير الحزم Winget (Terminal)
```powershell
winget install MDReader
```

---

## 🛠️ البناء والتطوير من الكود المصدري (Developer Build)

### المتطلبات الأساسية (Prerequisites):
- **Windows 10/11 x64**
- **.NET SDK 10.0+**
- **Inno Setup 6** (لإنشاء معالج التثبيت)

```powershell
# 1. استنساخ المستودع
git clone https://github.com/MDReader/MDReader.git
cd MDReader

# 2. استعادة الحزم والبناء
dotnet restore
dotnet build

# 3. تشغيل كافة الاختبارات المؤتمتة (77 اختبار ناجح)
dotnet test

# 4. بناء نسخة الإصدار المستقلة وتوقيعها رقمياً
powershell -ExecutionPolicy Bypass -File scripts/build-release.ps1

# 5. بناء معالج التثبيت Setup Wizard وحزمة MSIX
powershell -ExecutionPolicy Bypass -File scripts/build-installer.ps1
powershell -ExecutionPolicy Bypass -File scripts/build-msix.ps1
```

---

## 📁 هيكلية المشروع (Project Architecture)

```
MDReader/
├── src/
│   ├── MDReader.App/                  # واجهة WinUI 3 ومحرك العرض Offline WebView2
│   │   ├── Assets/                    # الأيقونات الحديثة ومحرك viewer.html
│   │   ├── Features/
│   │   │   ├── Reader/                # شاشات العرض والتحرير، التعليقات، الفهرس
│   │   │   └── Shell/                 # شريط العنوان والنافذة والقوائم
│   │   └── Package.appxmanifest       # تعريف حزمة MSIX وارتباطات الملفات
│   └── MDReader.Core/                 # محرك الماركداون، الفهرسة، والإحصاءات الحسابية
├── tests/
│   └── MDReader.Core.Tests/           # 77 اختبار وحدة تغطي الفهرسة، الماركداون، والأمان
├── packaging/
│   ├── inno/setup.iss                 # سكريبت معالج التثبيت القياسي Inno Setup
│   └── certs/MDReaderDev.pfx          # شهادة التوقيع الرقمي Authenticode SHA-256
├── scripts/
│   ├── build-release.ps1              # سكربت النشر الذاتي win-x64
│   ├── sign-binaries.ps1              # سكربت التوقيع الرقمي الآلي
│   ├── build-installer.ps1            # بناء معالج التثبيت MDReader-Setup-x64.exe
│   └── build-msix.ps1                 # بناء وتوقيع حزمة MSIX
└── dist/
    └── installer/                     # الملفات الجاهزة للتوزيع والتنزيل
        ├── MDReader-Setup-x64.exe     # معالج التثبيت (52.6 MB)
        └── MDReader-x64.msix          # حزمة التطبيق (31.9 MB)
```

---

## 📄 الترخيص (License)
هذا المشروع مرخص تحت رخصة **MIT**. لمزيد من التفاصيل راجع ملف [LICENSE](LICENSE).
