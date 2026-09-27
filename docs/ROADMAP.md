# MDReader — Roadmap

> A phased delivery plan for the MDReader Markdown reader application. Each phase is self-contained, testable, and builds on the previous one. **Phase 0 is the current phase.**

---

## Phase 0 — Environment and Architecture

**Status:** Complete

**Goals:**
- Audit the Windows development environment
- Install all required tools (verify no redundant installs)
- Document architecture and design decisions
- Create the project skeleton (solution, projects, docs, gitignore)
- Verify the build compiles

**Deliverables:**
- [x] Environment audit report
- [x] `.gitignore` created
- [x] `docs/DEVELOPMENT_RULES.md` created
- [x] `docs/ARCHITECTURE.md` created
- [x] `docs/ROADMAP.md` created (this file)
- [x] `README.md` created
- [x] Feature-based directory structure (Features/{Shell,Reader,FileOpen,Settings,Theme})
- [x] Source files moved to feature-based locations with updated namespaces
- [x] App.xaml/App.xaml.cs moved to Features/Shell/
- [x] MainWindow.xaml/xaml.cs moved to Features/Shell/Views/
- [x] MainPage.xaml/xaml.cs moved to Features/Reader/Views/
- [x] MarkdownDocument.cs moved to Core/Markdown/
- [x] AI control surface architecture (AI/Contracts, AI/Commands, AI/Gateway, AI/Security)
- [x] Shared components (Shared/Styles, Shared/Converters, Shared/Helpers)
- [x] ViewModels for Shell, Reader, FileOpen, Settings
- [x] Core contracts (Contracts/ICommandGateway, Documents/DocumentInfo)
- [x] Documentation suite created (9 docs: PRODUCT_SPEC, ARCHITECTURE, FEATURE_MAP, UI_DESIGN, AI_INTERFACE, DEVELOPMENT_RULES, DECISIONS, ROADMAP, AGENT_WORKFLOW)
- [x] Project skeleton (App + Core + Tests projects, solution file)
- [x] `dotnet build` succeeds (0 warnings, 0 errors)
- [x] `dotnet test` succeeds (2/2 tests passing)
- [x] Package.appxmanifest updated (Identity Name, DisplayName, GUIDs)
- [x] `.gitignore` created and verified
- [x] Git repository initialized

**Stop condition:** Report complete. **Do NOT** implement Markdown reading or UI features.

---

## Phase 1 — Minimal Window

**Status:** Pending Approval

**Goals:**
- Create a minimal WinUI 3 window that launches and displays
- Follow Windows app silhouette (title bar, minimize/maximize/close)
- Respect system light/dark theme
- Handle basic window sizing and positioning
- Graceful shutdown

**Deliverables:**
- [ ] WinUI 3 app project builds and runs via `dotnet run`
- [ ] Empty window appears with correct dimensions
- [ ] Window respects system theme
- [ ] Window is DPI-aware
- [ ] Basic keyboard accessibility (Tab navigation)
- [ ] Unit tests for Core project initialization

---

## Phase 2 — Open and Read Markdown File

**Goals:**
- Open a `.md` file via Windows File Picker
- Read file contents as UTF-8
- Parse Markdown to an intermediate document model
- Render basic Markdown elements (headings, paragraphs, lists, code blocks)
- Display rendered content in a scrollable view
- Handle invalid/missing/unreadable files with graceful error messages
- Open another file (replaces current document)

**Deliverables:**
- [ ] File picker opens and filters for `.md` files
- [ ] UTF-8 file reading with proper encoding
- [ ] Markdown parsing via `Markdig` (NuGet dependency documented)
- [ ] Basic Markdown rendering (headings, paragraphs, bold, italic, lists, code blocks)
- [ ] ScrollViewer for content navigation
- [ ] Error handling for missing/locked/invalid files
- [ ] Unit tests for Markdown parsing and document model

---

## Phase 3 — Markdown Rendering Quality

**Goals:**
- Full GitHub Flavored Markdown support
- Images referenced by relative paths
- Links (external → browser, internal → document navigation)
- Syntax highlighting for code blocks
- RTL text support (Arabic, Hebrew)
- Horizontal rule, blockquotes, tables
- Inline elements (inline code, links, image alt text)

**Deliverables:**
- [ ] GFM tables, strikethrough, task lists
- [ ] Image loading from relative paths (same directory as .md file)
- [ ] External link handling (launch in default browser)
- [ ] Syntax highlighting for common languages
- [ ] RTL paragraph support
- [ ] Unit tests for complex Markdown constructs

---

## Phase 4 — Windows UX Refinement

**Goals:**
- Native Windows-app title bar (custom title bar with window controls)
- Context menu for file operations
- Keyboard shortcuts (Ctrl+O for Open, Ctrl+R for Reload, Ctrl+F for Find)
- Print support
- Accessibility audit (Narrator, keyboard navigation, high-contrast)
- High-DPI testing at multiple scaling factors

**Deliverables:**
- [ ] Custom title bar following Windows app silhouette
- [ ] Keyboard shortcuts
- [ ] Print dialog
- [ ] Accessibility passes Narrator verification
- [ ] Tested at 100%, 150%, 200%, 250% DPI scaling

---

## Phase 5 — Reliability and Testing

**Goals:**
- Error boundary around all file I/O and rendering
- Performance testing with large files (>5MB)
- Memory profiling
- Unit test coverage ≥ 80% for Core
- Load testing for rapid document switching

**Deliverables:**
- [ ] Unit test coverage report
- [ ] Large file handling tested (≥ 10MB)
- [ ] Startup time < 1 second
- [ ] Memory usage profiling
- [ ] Automated test in CI (GitHub Actions or similar)

---

## Phase 6 — MSIX Packaging

**Goals:**
- Package the application as MSIX
- Code-signing certificate (self-signed for dev)
- Application manifest with correct capabilities
- Installer/uninstaller tested
- Update mechanism not included (MSIX handles updates via Store or sideloading)

**Deliverables:**
- [ ] MSIX package builds successfully
- [ ] `makeappx.exe` and `signtool.exe` used from Windows SDK
- [ ] App installs and uninstalls cleanly
- [ ] App runs from installed MSIX package

---

## Phase 7 — Microsoft Store Readiness

**Goals:**
- Verify Store certification compliance
- Remove any blocked APIs
- App manifest updated for Store submission
- Privacy policy and EULA prepared
- Store listing assets (icons, screenshots, description)

**Deliverables:**
- [ ] Store certification checklist passed
- [ ] App package ready for Store submission
- [ ] README updated with Store install instructions

---

## Out of Scope (Never in MVP)

- Markdown editing
- Cloud synchronization
- Accounts/login
- AI provider integration (LLM, OpenAI, OpenRouter, MCP)
- Database
- Plugins
- Collaboration
- Browser automation
- Complex settings
- Telemetry
- Custom updater

*These can only be proposed later, after MVP.*
