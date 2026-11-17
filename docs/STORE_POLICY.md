# Microsoft Store Policy, Submission Requirements & Development Standards

> **Authoritative Policy Document**  
> **App**: JMDReader  
> **Publisher**: birotech  
> **Store Product ID**: `9PC12HBZG0CX`  
> **Submission ID**: `1152921505702400334`  
> **Package Family Name (PFN)**: `birotech.JMDReader_4spa8h579ffz6`  
> **Publisher ID**: `CN=4B6200B3-DE4E-4F39-A6EA-B5DC049294B2`

---

## 1. Mandatory Dual Task Registration (Linear.app & TASKS.md)

All future developments, features, bug fixes, and version releases (including v1.0.2, v2.0, and onward) **must adhere to the following workflow**:
1. **Task Registration Before Code**:
   - Every development initiative must be registered as a task in `.project/TASKS.md`.
   - Every task must also be created and tracked in **Linear.app** under the workspace `https://linear.app/jmdreader` (Team: `JMDReader`, Project: `JMDReader birotech`).
2. **Lifecycle States**:
   - `Backlog` / `Todo` -> `In Progress` -> `Done` / `Completed`.
   - Issue IDs (e.g. `JMD-5`, `JMD-6`, `JMD-7`, `JMD-8`) must be cited in git commit messages, PRs, and `.project/CHANGELOG.md`.
3. **No Unregistered Work**:
   - No feature or modification shall be introduced without an associated issue in Linear and entry in `TASKS.md`.

---

## 2. Microsoft Store Identity & Packaging Specifications

All Store packages submitted to Microsoft Partner Center must satisfy these criteria:

| Parameter | Required Value | Notes |
|---|---|---|
| **Identity Name** | `birotech.JMDReader` | Exact match with Partner Center reservation |
| **Publisher** | `CN=4B6200B3-DE4E-4F39-A6EA-B5DC049294B2` | Verified Microsoft Partner Publisher ID |
| **PublisherDisplayName** | `birotech` | Registered organization name |
| **Package Family Name** | `birotech.JMDReader_4spa8h579ffz6` | Generated PFN |
| **Version Syntax** | `Major.Minor.Build.Revision` | e.g. `1.0.2.0`, `2.0.0.0` |
| **Target Architectures** | `x64` AND `arm64` | Native compilation for both Intel/AMD and Arm64 |
| **Package Format** | `.msix` or `.msixupload` | Modern containerized Windows package |

---

## 3. Technical Requirements & Sandbox Hardening

### 3.1 WebView2 LocalAppData Isolation (Anti-Crash Guarantee)
- In packaged MSIX apps, `Package.Current.InstalledLocation` points to `C:\Program Files\WindowsApps\...` which is **strictly read-only**.
- Calling `CoreWebView2.EnsureCoreWebView2Async()` without specifying a user data directory causes WebView2 to attempt creating cache folders in the application directory, resulting in fatal unhandled crashes (`0x8000FFFF E_UNEXPECTED` / `0xc000027b`).
- **Policy Rule**: Always initialize WebView2 using `CoreWebView2Environment.CreateWithOptionsAsync` pointing explicitly to:
  ```csharp
  var userDataFolder = Path.Combine(Environment.GetFolderPath(SpecialFolder.LocalApplicationData), "JMDReader", "WebView2Data");
  ```

### 3.2 Multi-Architecture Binary Delivery
- To support modern Copilot+ PC devices (Snapdragon X Elite/Plus) and standard PCs:
  - Both `MDReader.App_<version>_x64.msix` and `MDReader.App_<version>_arm64.msix` must be generated for every Store submission.
  - Scripts must support `-Architecture all` (`scripts/build-msix.ps1`).

### 3.3 Theme Resilience & XAML Dictionaries
- All XAML themes in `Shared/Styles/App.xaml` must declare a `<ResourceDictionary x:Key="Default">` fallback dictionary in addition to `Light` and `Dark`.

---

## 4. Privacy, Telemetry, and Capabilities

### 4.1 Zero Telemetry Guarantee
- **100% Offline**: The app never connects to external servers, tracks user analytics, or transmits document content.
- **Local File System Only**: Access to files occurs strictly through explicit user interaction (file dialogs, drag & drop, MRU list).

### 4.2 Capability Declaration
- Declared Capability: `runFullTrust` (Restricted Capability).
- Justification for Store Submission:
  - Required for WinUI 3 desktop process hosting, high-performance Markdown parsing, local PDF rendering (`PrintToPdfAsync`), and file association handling (`.md`, `.markdown`).
- No extraneous capabilities (e.g. `internetClient`, `location`, `microphone`, `webcam`) are permitted.

---

## 5. Certification & Verification Protocol

Before submitting any package to Microsoft Partner Center:
1. `dotnet test` must pass 100% with zero failures.
2. Both `x64` and `arm64` MSIX packages must be generated and signed with the developer certificate.
3. Package SHA256 hashes must be verified and recorded.
4. Linear tasks for the target release must be updated to **Done**.
