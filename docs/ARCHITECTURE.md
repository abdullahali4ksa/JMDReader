# MDReader — Architecture

> **Phase 0 — Environment and Architecture**
> This document is the single source of truth for the MDReader project's technology choices, project boundaries, dependency direction, and design decisions. All future agents must consult this document before making architectural changes.

---

## 1. Technology Choices

| Concern              | Choice                              | Rationale |
|----------------------|-------------------------------------|-----------|
| Platform             | Windows 11 (min: Windows 10 1809)   | Windows-native first; broad compatibility through Windows App SDK |
| Language             | C#                                  | First-party language for WinUI 3; excellent .NET ecosystem support |
| Runtime              | .NET 10 (LTS)                       | Latest LTS release as of September 2026; supported until November 2028; compatible with Windows App SDK 2.x |
| UI Framework         | WinUI 3                             | Native, modern, Windows-first UI framework; not cross-platform or web-based |
| App Model            | Windows App SDK 2.x                 | Provides WinUI 3 and modern Windows APIs independently of Windows OS version; supports back to Windows 10 1809 |
| IDE                  | Visual Studio Code                  | Lightweight, fast, cross-platform-capable; preferred per project direction |
| Build System         | .NET CLI (`dotnet build`)           | Command-line driven; integrates with VS Code |
| Packaging            | MSIX                                | Native Windows packaging; Microsoft Store ready |
| Version Control      | Git                                 | Standard for source control |
| Test Framework       | xUnit + Microsoft.NET.Test.Sdk      | Lightweight, widely used with .NET; integrates with `dotnet test` |

### Version Matrix (Selected)

| Component         | Version       | Source / Channel | Status |
|-------------------|---------------|------------------|--------|
| .NET SDK          | 10.0.401      | dotnet-install.ps1 (user-local) | LTS    |
| Windows App SDK   | 2.5.1 (NuGet) | NuGet            | Stable |
| Windows SDK       | 10.0.26100.9169 (BuildTools NuGet) | NuGet | Optional — BuildTools NuGet provides headers/libs |
| VS Code           | 1.138.0       | Installed        | Current |
| C# Extension      | Latest        | VS Code Marketplace | Current |
| Git               | 2.53.0        | Pre-installed     | Current |

---

## 2. Project Boundaries

```
MDReader/
├── src/
│   ├── MDReader.App/              — WinUI 3 UI, feature-based structure, AI control surface
│   │   ├── Features/              — Feature-based organization
│   │   │   ├── Shell/             — App.xaml, MainWindow, ShellViewModel
│   │   │   ├── Reader/            — MainPage, ReaderViewModel
│   │   │   ├── FileOpen/          — FileOpenViewModel
│   │   │   ├── Settings/          — SettingsViewModel
│   │   │   └── Theme/             — IThemeService
│   │   ├── AI/                    — AI control surface (architectural only, no integration)
│   │   │   ├── Contracts/         — IAICommandHandler, CommandMetadata
│   │   │   ├── Commands/          — AICommandSet
│   │   │   ├── Gateway/           — AIGateway (placeholder)
│   │   │   └── Security/          — AISecurityPolicy
│   │   ├── Shared/                — Styles, Converters, Helpers, Controls
│   │   ├── Package.appxmanifest
│   │   ├── app.manifest
│   │   └── MDReader.App.csproj
│   └── MDReader.Core/             — Markdown logic, document models, contracts
│       ├── Markdown/              — MarkdownDocument model
│       ├── Documents/             — DocumentInfo, document metadata
│       └── Contracts/             — ICommandGateway, CommandRequest, CommandResult
├── tests/
│   ├── MDReader.Core.Tests/       — Automated tests for Core (2/2 passing)
│   └── MDReader.App.Tests/        — UI tests (planned for Phase 4)
├── docs/                          — Full documentation suite (9 documents)
├── .vscode/                       — VS Code config (settings, tasks, launch)
├── global.json                    — SDK version pinning (10.0.401)
├── README.md                      — Project overview
├── .gitignore
└── MDReader.sln                   — Solution file
```

### Responsibilities

**MDReader.App** (`src/MDReader.App/`)
- WinUI 3 application host
- Window management (creation, sizing, title bar)
- File picker integration (Windows.Storage.Pickers)
- Markdown rendering to XAML (consuming Core models)
- Command handling (Open File, Exit, etc.)
- Light/Dark theme handling (delegated to Windows system theme)
- Application lifecycle (startup, shutdown, suspend)

Must **not** contain:
- Markdown parsing logic (that belongs in Core)
- Database access
- Cloud/network code (out of scope for MVP)

**MDReader.Core** (`src/MDReader.Core/`)
- Markdown document model (AST-like representation)
- Markdown parsing logic (language-agnostic)
- File I/O utilities for reading .md files
- UTF-8 encoding handling
- Error handling for invalid/missing/unreadable files
- Business logic for document state

Must **not** depend on:
- WinUI 3 (`Microsoft.UI.Xaml`)
- Windows App SDK (`Microsoft.WindowsAppSDK`)
- Any Windows-specific API
- Any NuGet package that depends on Windows-specific APIs

**MDReader.Core.Tests** (`tests/MDReader.Core.Tests/`)
- Unit tests for all Core logic
- Mock-based testing for file I/O and parsing
- No UI integration tests at this layer

### 2.1 Feature-Based Architecture

The application is organized by **feature** (responsibility or unit of user value),
not by layer (Views/ViewModels/Models/). Each feature has its own directory tree
under `Features/` containing only the files relevant to that feature.

### 2.2 MVVM Pattern

The application follows the MVVM pattern: View (XAML, no business logic), ViewModel
(presentation logic, state, commands), Model (data and domain logic).
MVVM framework `CommunityToolkit.Mvvm` will be added in Phase 1.

### 2.3 AI Control Surface

The `AI/` directory is the architectural control surface for future AI agent
integration. It contains contracts, command registry, security policy, and a
placeholder gateway. No AI provider is integrated. See `docs/AI_INTERFACE.md`.

---

## 3. Dependency Direction

```
MDReader.App  ───depends on───>  MDReader.Core

MDReader.Core.Tests  ───depends on───>  MDReader.Core
```

- `MDReader.App` depends on `MDReader.Core` (and on WinUI 3 / Windows App SDK NuGet packages).
- `MDReader.Core` depends on **nothing** Windows-specific. It may reference standard .NET libraries and at most one Markdown parsing library (to be decided and documented per DEVELOPMENT_RULES.md §3).
- `MDReader.Core.Tests` depends on `MDReader.Core` and test framework packages only.

---

## 4. Dependency Decisions

> All NuGet dependencies must be documented here. No dependency is added without an entry below.

### Existing Dependencies

| Package | Version | Purpose | License | Why built-in is insufficient |
|---------|---------|---------|---------|------------------------------|
|`Microsoft.WindowsAppSDK`| 2.5.1 | Provides WinUI 3, Windows App SDK runtime API bindings | Microsoft MIT | .NET does not ship UI frameworks; WinUI 3 is required for the Windows-native UI |
|`Microsoft.Windows.SDK.BuildTools`| 10.0.26100.9169 | Windows SDK headers, libraries, and build tools (makeappx, signtool) | MS-RL | Replaces the need for a system-level Windows SDK installation during build |
||`Microsoft.Windows.SDK.BuildTools.WinApp`| 0.7.0 | Windows App SDK-specific build tools (MSIX packaging, debug identity) | MS-RL | Hooks `dotnet run` for packaged WinUI apps |
||`CommunityToolkit.Mvvm`| 8.4.0 | MVVM framework (ObservableObject, ObservableProperty, RelayCommand) for Shell and feature ViewModels | MIT (CommunityToolkit) | .NET does not include a standard MVVM framework; required for feature-based MVVM architecture defined in ARCHITECTURE.md §2.2 |

### Pending Decision: Markdown Parser

**Status:** Decision documented, dependency not yet added (Phase 0 only).

**Options evaluated:**
1. **Built-in .NET (`System.Text.Markdown`)** — If available in .NET 10, this would be the zero-dependency choice. As of .NET 8/9, there is no official `System.Text.Markdown` for parsing to an AST. System.Text.Json is available but that is for JSON, not Markdown. **Insufficient.**
2. **`Markdig`** (NuGet: `Markdig`) — The de facto standard .NET Markdown parser. Mature, MIT-licensed, actively maintained, produces a syntax tree, supports GitHub Flavored Markdown extensions. **Recommended.**
3. **Custom parser** — Would be large, error-prone, and difficult to maintain. **Not recommended.**

**Decision:** Use `Markdig` in `MDReader.Core` for parsing Markdown to a syntax tree. Rendering to XAML will be handled in `MDReader.App` by walking the syntax tree and creating appropriate WinUI 3 controls (`TextBlock`, `Image`, etc.). This keeps parsing logic in Core (testable, Windows-independent) and rendering in App (Windows-specific). The `Markdig` NuGet package will be added in Phase 2 (Markdown rendering), not Phase 0.

**Rationale for Markdig over built-in:** .NET does not ship a Markdown parsing library in the runtime or BCL. Markdig is the standard, well-maintained, MIT-licensed option with a permissive license compatible with MSIX packaging and Microsoft Store distribution.

---

## 5. Windows Architecture

### Application Model
- **Model:** Unpackaged desktop app using Windows App SDK 2.x
- The app runs as a standalone Win32 process with a WinUI 3 window
- **Packaged vs Unpackaged:** Phase 0 targets **unpackaged** development (for `dotnet run` debugging via VS Code). MSIX packaging is addressed in Phase 6.
- The `Microsoft.Windows.SDK.BuildTools.WinApp` NuGet package (included via the WinUI template) handles debug identity for `dotnet run` when Developer Mode is enabled.

### Runtime Dependencies
- The Windows App SDK runtime is already present on the machine (multiple versions installed as AppX packages): versions 1.1 through 1.8 and 2.x.
- The app will declare its Windows App SDK version requirement via the NuGet package reference. At runtime, the Windows App SDK framework package (already installed) provides the implementation.
- .NET 10 runtime will be installed as part of the .NET 10 SDK installation.

### File Access
- The app opens `.md` files via the Windows Storage File Picker (`Windows.Storage.Pickers`).
- File reading uses `System.IO` and `System.Text` (UTF-8) — no special permissions needed for user-selected files.
- No broad filesystem access is required. The app requests access only to files the user explicitly selects.

---

## 6. Critical Design Points

### Markdown Parser/Rendering Strategy
- **Parser:** `Markdig` (NuGet package, to be added in Phase 2). Parses Markdown to an AST.
- **Renderer:** In `MDReader.App`, walk the Markdig AST and create WinUI 3 controls dynamically:
  - Headings → `TextBlock` with appropriate style/size
  - Paragraphs → `TextBlock` in a `StackPanel` or `ContentPresenter`
  - Code blocks → `Border` + `TextBlock` (with monospace font)
  - Lists → `TextBlock` with bullets, or `ListView`/`StackPanel` with margin
  - Inline code → `Span` with monospace font and background
  - Images → `BitmapImage` in an `Image` control
  - Links → `Hyperlink` in a `TextBlock`
- This approach keeps parsing in Core (testable) and rendering in App (Windows-specific).

### Local File Permissions/Access
- Files opened via the Windows File Picker grant the app a `StorageFile` with read access.
- For unpackaged apps, `System.IO.File.ReadAllBytes` works on user-selected paths without special permissions (the user has already granted access by selecting the file).
- **Risk:** Long paths (> 260 chars). Mitigation: use `\\?\` prefix or `System.IO.Path.GetFullPath` with long-path awareness. .NET 10 supports long paths by default on Windows.
- **Risk:** Files that disappear or are locked after selection. Mitigation: try/catch around `File.ReadAllBytes` and show a graceful error.

### UTF-8 and Arabic Text
- .NET 10 reads text files as UTF-8 by default when using `File.ReadAllText(path, Encoding.UTF8)`.
- Arabic text and other RTL scripts are handled by .NET's `System.Text` encoding correctly.
- WinUI 3 `TextBlock` natively supports bidirectional text via `FlowDirection` property.

### RTL Content
- Markdown files containing RTL text (Arabic, Hebrew) will be rendered with proper bidirectional support.
- WinUI 3 `TextBlock` and `RichTextBlock` support `FlowDirection="RightToLeft"` for entire paragraphs.
- Mixed LTR/RTL content in the same paragraph is handled by the Unicode bidirectional algorithm in DirectWrite (used by WinUI 3).
- **Decision:** Set `FlowDirection` per-paragraph based on content analysis, or rely on the system default. This will be refined in Phase 2.

### Large Markdown Files
- **Risk:** Very large files (e.g., 10MB+) could consume excessive memory when fully parsed and rendered.
- **Decision:** Load and parse the entire file into memory (Markdown is typically small). For Phase 5 (Reliability), consider virtualized rendering if performance testing shows issues.
- For the MVP, files > 5MB will be loaded with a warning to the user. Files that cause `OutOfMemoryException` will be caught and reported gracefully.

### Images Referenced by Relative Paths
- Markdown files may reference images via relative paths (e.g., `![alt](./image.png)`).
- **Decision:** Resolve relative paths relative to the Markdown file's directory. Load images as `BitmapImage` with `BitmapImage.UriSource` set to the absolute file path.
- **Security:** Only load images from the same directory tree as the Markdown file. Do not follow symbolic links that escape the file's directory.

### Links
- Internal links (`#heading`) — navigate to the heading within the document.
- External links (`https://...`) — open in the default browser via `Windows.System.Launcher.LaunchUriAsync`.
- File-relative links (other .md files) — open via the file picker mechanism (Phase 2 or later).

### Code Blocks
- Fenced code blocks (```) and indented code blocks will be rendered in a `Border` with a monospace `FontFamily`.
- Language identifiers (e.g., ```csharp) will be used for optional syntax highlighting in Phase 2.
- Code blocks should support horizontal scrolling for long lines (horizontal `ScrollViewer`).

### Security Implications of Embedded HTML
- **Markdig** by default does NOT render raw HTML in the AST (it stores it as `HtmlBlock` and `HtmlInline` nodes). The renderer will **skip** HTML nodes by default.
- **Decision:** Do NOT render embedded HTML. Treat it as plain text or skip it entirely. This prevents XSS-like attacks and security vulnerabilities.
- Future consideration: If HTML rendering is needed, use a sandboxed WebView2 with script disabled.

### Application Startup Performance
- The app should launch and show a window within 1 second for typical use.
- **Decision:** Minimize startup work. Load the file content lazily — show an empty window first, then load the file in the background if a file path is passed as a command-line argument.
- The Windows App SDK's `Microsoft.Windows.SDK.BuildTools.WinApp` handles debug identity setup efficiently.

### Memory Usage
- The entire Markdown file content + parsed AST + rendered XAML elements will reside in memory.
- For typical Markdown files (< 1MB), memory usage should be well under 50MB.
- **Decision:** No caching strategy for Phase 0. Consider LRU caching of rendered documents if multiple files are open (future enhancement).

### Packaged vs Unpackaged WinUI Deployment
- **Phase 0-5:** Unpackaged deployment for development. Use `dotnet run` with Developer Mode enabled.
- **Phase 6:** MSIX packaging for distribution. The project will be updated to be compatible with MSIX packaging.
- **Decision:** Start unpackaged, ensure all code paths are MSIX-compatible from the start.

### MSIX Packaging
- MSIX packaging requires:
  - Windows SDK 10.0.26100+ (for makeappx.exe and signtool.exe)
  - Application manifest (AppxManifest.xml) with correct capabilities
  - Package identity
  - Code signing certificate (for distribution; self-signed for development)
- The MSIX Packaging Tool (already installed) can create packages.
- **Decision:** Defer MSIX packaging to Phase 6. Ensure the project structure is MSIX-ready from Phase 0.

### Microsoft Store Readiness
- The app must not use APIs blocked by Store certification:
  - No `LoadLibrary` for arbitrary DLLs
  - No `HKLM` writes
  - No `SeDebugPrivilege`
  - No processes launched with elevated privileges
- WinUI 3 + Windows App SDK apps are generally Store-compatible.
- **Decision:** Document Store considerations now; implement in Phase 7.

### Accessibility
- WinUI 3 controls are accessible by default (support Narrator, keyboard navigation).
- **Decision:** Ensure all rendered content has appropriate accessibility properties:
  - `AutomationProperties.Name` for images
  - Proper heading levels (`HeadingLevel` property in TextBlock)
  - Keyboard-focusable elements
- Test with Narrator and keyboard-only navigation in Phase 4.

### High-DPI / Scaling
- WinUI 3 uses Device-Independent Pixels (DIPs) and scales automatically based on the system DPI setting.
- `BitmapImage` images scale based on DPI.
- **Decision:** Use Vector icons where possible. For raster images, ensure they are provided at 1x, 1.5x, and 2x scales, or use `BitmapImage.DecodePixelType`.

### Windows Light/Dark Theme Behavior
- WinUI 3 `Application.RequestedTheme` can be set to `Light` or `Dark`.
- The app should follow the system theme by default.
- **Decision:** Use `ApplicationTheme.Target` set to follow system. The Windows App SDK provides `Microsoft.UI.System.ThemeSettings` for runtime theme detection and change notification.

---

## 7. Future Extension Points

These are planned for future phases and are **not** implemented in Phase 0:

| Extension Point | Planned For | Notes |
|-----------------|-------------|-------|
| Markdown editing | Phase 2+ | Tabbed interface, dual-pane editor |
| Live preview | Phase 3 | Real-time rendering as user types |
| Syntax highlighting | Phase 2 | Using ColorCode or similar library |
| Custom themes | Phase 4 | Beyond system light/dark |
| Settings UI | Phase 1 | Minimal settings page |
| File explorer sidebar | Phase 3 | Navigate folder tree |
| Search | Phase 3 | Search within document |
| Export to HTML/PDF | Phase 4 | Export rendered document |
| Plugin system | Future | Out of scope for Phase 0-7 |

---

## 8. Build and Run

### Prerequisites (verified in Phase 0)
- .NET 10 SDK 10.0.401 (installed to `C:\Users\ZAACO\.dotnet` via `dotnet-install.ps1`)
- Windows SDK is optional — `Microsoft.Windows.SDK.BuildTools` NuGet package (10.0.26100.9169) provides headers and build tools
- Windows App SDK 2.5.1 (via NuGet, auto-restored)
- VS Code 1.138+ with C# extension (`ms-dotnettools.csharp`)
- Developer Mode (optional — required only for `dotnet run` debugging; build/test work without it)

### Commands
```bash
# Build the solution
dotnet build

# Run the app (from src/MDReader.App)
dotnet run --project src/MDReader.App

# Run tests
dotnet test

# Restore packages
dotnet restore
```

---

*Generated as part of Phase 0 — Environment and Architecture (September 2026).*
