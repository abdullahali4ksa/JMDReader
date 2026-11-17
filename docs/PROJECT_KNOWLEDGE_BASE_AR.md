# 📚 دليل وسجل المعرفة الشامل لمشروع JMDReader
## توثيق الخطوات، الحلول الفنية، ومتطلبات النشر على متجر مايكروسوفت

---

### 1. 📌 بطاقة تعريف المشروع (Identity Card)
* **اسم التطبيق (App Name):** `JMDReader`
* **معرف المتجر (Store ID):** `9PC12HBZG0CX`
* **المطور والناشر (Publisher):** `birotech` (المهندس عبد الله علي)
* **معرف الناشر المعتمد (Publisher ID):** `CN=4B6200B3-DE4E-4F39-A6EA-B5DC049294B2`
* **اسم عائلة الحزمة (Package Family Name - PFN):** `birotech.JMDReader_4spa8h579ffz6`
* **رابط المستودع على GitHub:** [https://github.com/abdullahali4ksa/JMDReader](https://github.com/abdullahali4ksa/JMDReader)
* **الإصدار الحالي (Release):** `v1.0.1` (مرفق به مثبت `JMDReader-Setup-x64.exe` وحزمة `MSIX`)
* **سياسة الخصوصية:** [https://raw.githubusercontent.com/abdullahali4ksa/JMDReader/main/PRIVACY.md](https://raw.githubusercontent.com/abdullahali4ksa/JMDReader/main/PRIVACY.md)

---

### 2. 🔍 ما هي صفحة "Submission Options" وما وظيفتها؟
رابط الصفحة: `https://partner.microsoft.com/en-us/dashboard/products/9PC12HBZG0CX/submissions/1152921505702400334/options`

هذه الصفحة تسمى **خيارات التقديم (Submission Options)** وهي المرحلة التنظيمية الأخيرة قبل فحص مايكروسوفت، وتحتوي على 3 أقسام جوهرية:

1. **خيارات موعد ونشر التطبيق (Publishing hold options):**
   * الخيار الافتراضي والموصى به: **Publish this submission as soon as it passes certification** (نشر فوري بمجرد اجتياز الفحص دون تأخير).
   * الخيار البديل: تعليق النشر اليدوي حتى تاريخ محدد أو حتى ضغط زر النشر يدوياً.
2. **صلاحيات التشغيل المقيدة (Restricted Capabilities - `runFullTrust`):**
   * يكتشف متجر مايكروسوفت تلقائياً وجود صلاحية `runFullTrust` في ملف `Package.appxmanifest`.
   * **المبرر المقبول رسمياً:** التطبيق مبني بنظام Windows App SDK / WinUI 3 الأصلي لسطح المكتب، ويحتاج الوصول الكامل لتسجيل ارتباط امتداد `.md` في نظام ويندوز، وفتح وحفظ وتعديل ملفات الماركداون المحلية التي يختارها المستخدم، وتصدير مستندات HTML محلية، والعمل 100% أوفلاين دون اتصال خارجي.
3. **ملاحظات لفاحصي مايكروسوفت (Notes for certification):**
   * توجيهات واضحة لفريق مراجعة التطبيقات لدى مايكروسوفت توضح:
     - التطبيق يعمل بالكامل دون الحاجة لأي حساب أو تسجيل دخول أو إنترنت.
     - خطوات الاختبار السريع (فتح ملف .md، تجربة محرر النصوص المنقسم، تشغيل الملقن التلقائي Teleprompter، تصدير ملف HTML، تبديل اللغات والسمات الداكنة/الفاتحة).
     - التأكيد على خصوصية التطبيق الكاملة (Zero Telemetry).

---

### 3. 💡 الدروس الفنية المستفادة والحلول المطبقة (Technical Lessons Learned)

#### أ) التوافق الصارم مع هوية الناشر في MSIX (Identity Matching)
* **المشكلة:** عند رفع الحزمة الأولى ظهر خطأ:
  `Invalid package publisher name: CN=birotech, O=birotech, C=SA (expected: CN=4B6200B3-DE4E-4F39-A6EA-B5DC049294B2)`
  `Invalid package family name: birotech.JMDReader_srn3v9f82cnar (expected: birotech.JMDReader_4spa8h579ffz6)`
* **السبب:** مايكروسوفت تشتق تجزئة اسم العائلة (PFN hash) مباشرة من نص الـ Publisher.
* **الحل:** تم توليد شهادة رقمية مطابقة تماماً لـ `CN=4B6200B3-DE4E-4F39-A6EA-B5DC049294B2` (بصمة: `4D6109C9BF7EF236605BAE96A681A0814A8E406A`) وتحديث سكريبت البناء `scripts/build-msix.ps1` وإعادة التوقيع. تم قبول الحزمة فوراً بنجاح 100%.

#### ب) ضوابط الهروب في مثبت Inno Setup
* في ملف `setup.iss`، تسبب المعرف `{B9A5F148-...}` في خطأ مترجم Inno Setup رقم 2؛ لأن أقواس `{` يجب مضاعفتها للهروب (`{{B9A5...}`). تم إصلاحها وأصبح المثبت يعمل بصمت تام عبر خيارات: `/VERYSILENT /SUPPRESSMSGBOXES /NORESTART`.

#### ج) دعم اللغات واتجاه النصوص (RTL Support)
* تم تجهيز صفحة المتجر باللغتين:
  - **الإنجليزية (United States - ID: 4)**: نصوص وميزات وملاحظات إصدار احترافية.
  - **العربية (Saudi Arabia - ID: 8)**: صياغة دقيقة تدعم اتجاه اليمين لليسار مع إبراز توافق WinUI 3 والخصوصية التامة.
* **شرط مايكروسوفت لصور العرض:** تشترط مايكروسوفت وجود لقطة شاشة واحدة على الأقل لسطح المكتب (Desktop screenshot بدقة 1366x768 أو 1920x1080) في كل لغة لتتحول حالة القائمة من Incomplete إلى Complete.

---

### 4. 🗂️ المهام المسجلة في Microsoft To Do وقاعدة البيانات المحلية
تم الوصول إلى قاعدة بيانات التطبيق المحلية في نظام ويندوز:
`C:\Users\ZAACO\AppData\Local\Packages\Microsoft.Todos_8wekyb3d8bbwe\LocalState\AccountsRoot\15732d62f2cc49fa869a5b2dca8bf2ab\todosqlite.db`

وتم تسجيل 4 مهام استراتيجية عالية الأهمية داخل مجلد **`💡 أفكار - تطبيقات`**:
1. `🌐 [JMDReader] إضافة جوجل كروم (Chrome Extension - Manifest V3)` - المهمة رقم 375
2. `📄 [JMDReader] إضافة مايكروسوفت أوفيس (Office 365 / Word Add-in)` - المهمة رقم 376
3. `🐧 [JMDReader] تطبيق أوبونتو ولينكس (Ubuntu Snap / Flathub)` - المهمة رقم 377
4. `📱 [JMDReader] تطبيق أندرويد (Android Tablet & Mobile App)` - المهمة رقم 378

---

### 5. 🚀 توصيات المنصات لتسريع إطلاق المنتجات البرمجية
* **Linear (الخيار المجاني الأقوى لإدارة وتتبع المشاريع):**
  - خطة مجانية ممتازة وغير محدودة لأعضاء الفريق الصغير.
  - نظام دورات (Cycles) وإدارة تذاكر برمجية وربط مباشر وتلقائي مع GitHub.
* **Supabase (قاعدة البيانات السحابية والباك إند الفوري):**
  - خطة مجانية سخية توفر قاعدة بيانات PostgreSQL كاملة، نظام توثيق مستخدمين (Auth)، وتخزين ملفات لأي فكرة تطبيق جديدة في دقائق.
