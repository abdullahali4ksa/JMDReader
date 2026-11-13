# MDReader Product Blueprint

**Status:** Accepted and authorized by the project owner (Original: 2026-10-05; Evolution Amendment: 2026-11-13). This blueprint is the authoritative product plan; implementation is authorized via the Autonomous Developer Loop, task by task, within the file scopes and acceptance criteria defined in `.project/TASKS.md`.
**Authority:** This blueprint expands `docs/PRODUCT_SPEC.md` and must remain consistent with `docs/ARCHITECTURE.md`, `docs/UI_DESIGN.md`, `docs/FEATURE_MAP.md`, `docs/ROADMAP.md`, `docs/DECISIONS.md`, `.project/PROJECT.md`, and `.project/STATE.json`.
**Product:** MDReader — an Antigravity-grade Windows Markdown Reader & Editor powered by a modern hybrid architecture (WinUI 3 Shell + Offline WebView2 Engine).

## 1. Product definition

MDReader opens, edits, and renders Markdown documents with the fidelity and rich visual capabilities of Google Antigravity in a fast, native Windows desktop application. It supports reading, live editing, and previewing with GitHub alerts, Mermaid diagrams, KaTeX math formulas, syntax-highlighted code blocks with copy actions, multi-document tabs, and workspace folder browsing. It starts quickly, behaves predictably with local files, supports Arabic and English (RTL/LTR), operates 100% offline, and avoids accounts, cloud services, telemetry, and external network dependencies.

### Intended users

- Knowledge workers, developers, and writers seeking an Antigravity-like rich Markdown viewing and editing experience on Windows.
- Developers reviewing and editing project documentation, READMEs, diagrams, and code snippets.
- Students and researchers reading and authoring technical notes with math formulas ($E=mc^2$) and Mermaid workflows.
- Arabic/English readers and authors who require seamless bidirectional text editing and rendering.

Windows 10 20H2 or later is the minimum product target; Windows 11 is the primary design target.

## 2. Product principles and evolution boundaries

1. **Reader and Editor:** Safe, atomic editing and saving of Markdown files (`Ctrl+S`, Save As, dirty state indicator `*`). Never corrupt user files; use atomic writes (`.tmp` -> replace).
2. **Local and 100% offline:** All rendering (including Markdown-it, Mermaid, KaTeX, and syntax highlighters) runs strictly from local offline assets bundled in the application. Zero network calls or CDN requests.
3. **Explicit access:** Access only files and folders explicitly opened or dropped by the user; no background telemetry or broad disk crawling.
4. **Hybrid Architecture (Option A):** WinUI 3 hosts the native Windows shell, window chrome, `TabView`, workspace folder tree, file I/O, and hotkeys. A local, sandboxed `WebView2` hosts the Antigravity-grade Markdown preview and editor components.
5. **Core/App separation:** Business logic, atomic file I/O, and document sessions reside in `MDReader.Core`. UI presentation, tabs, and WebView2 hosting reside in `MDReader.App`.
6. **Arabic & Bi-directional fidelity:** Native per-paragraph and per-block RTL/LTR detection for Arabic and English text, maintaining LTR for code blocks, URLs, and math formulas.

## 3. Functional scope

### 3.1 File types and opening

- MVP input: UTF-8 `.md` and `.markdown` local files, including Arabic and English text. Invalid byte sequences, empty files, missing files, locked files, and access-denied cases produce a clear recoverable message.
- Open through a Windows file picker and command-line file path (`mdreader.exe <path>`). Drag-and-drop is in the planned UX phase.
- A selected Markdown file is the unit of reading. Opening another replaces the current document; no tabs or multi-document editing.
- Folder selection and folder browsing are deferred. The app does not crawl directories. Relative Markdown links may open another explicit local Markdown file only after confirmation/selection rules are defined in the relevant task.
- Relative images are supported only from within the Markdown document's directory tree; path traversal and symlinks escaping that tree are rejected.

### 3.2 Preview and Markdown rendering

- Parse with Markdig in `MDReader.Core`; render the supported AST in WinUI in `MDReader.App`.
- MVP: headings, paragraphs, emphasis, ordered/unordered lists, fenced and indented code blocks, inline code, blockquotes, horizontal rules, and tables. Scrollable document view; long code lines can scroll horizontally.
- Phase 3 quality scope: GFM tables, strikethrough, task lists, relative local images, external links, internal heading anchors, file-relative Markdown links, and common-language code highlighting if an acceptable dependency is approved and recorded.
- Raw HTML nodes are displayed as inert text or omitted consistently; never execute HTML or scripts and do not use WebView for document rendering.
- Internal anchors navigate within the current document. External `http`/`https` links open in the system browser after a user click. Unsupported schemes are not launched.
- Per-paragraph RTL detection: Arabic/Hebrew paragraphs use `FlowDirection.RightToLeft`; English/code paragraphs use `FlowDirection.LeftToRight`. LTR preserved for code blocks, headings, table cells, URLs, and inline code.
- Visible zoom ratio indicator (e.g., 100%) displayed in the application. Keyboard zoom actions (`Ctrl++` for +10%, `Ctrl+-` for -10%, `Ctrl+0` for reset) are discoverable. No persistent preference stored (current-run only, per Blueprint §3.7).
- Rendering is read-only. No source editing, export, print-to-PDF, or clipboard enhancement is committed for MVP.

### 3.3 Navigation, search, and multiple items

- MVP navigation is vertical scrolling and internal heading anchors. No outline pane or folder tree in MVP.
- In-document find is planned for UX refinement (`Ctrl+F`, next/previous match, clear query); search across folders or all files is out of scope.
- Recent-file list is session-only and bounded (proposed maximum 10); it clears on exit. Reopening still requires the path to exist and access to succeed.
- No tabs, split views, workspace/project browser, multi-file batch open, or folder index.

### 3.4 Themes, settings, shortcuts, and input

- Follow Windows light/dark mode by default; provide a Light/Dark/System choice for the current run. Persistent theme preference is deferred under the no-persistent-storage constraint.
- Settings are limited to theme and language presentation (English/Arabic where UI localization is available). No broad preferences page, accounts, sync, or complex configuration.
- Keyboard baseline: Tab/focus accessibility and Alt+F4 in Phase 1; `Ctrl+O` Open, `Ctrl+R` Reload, `Ctrl+F` Find, Escape clear/close search in Phase 4. Shortcuts must be discoverable and not conflict with Windows conventions.
- Drag-and-drop accepts a single supported Markdown file in the UX phase. Unsupported file drops show a concise message. Folder drops are deferred.
- Touch and mouse-wheel scrolling are supported; no custom gesture system.

### 3.5 Recent files and state

- Current-session recent items only; no persistent recent list, last-document restore, scroll-position persistence, or window-state persistence in MVP.
- No database. No new persistent storage mechanism may be added without updating the spec and an accepted architecture decision.

## 4. Non-functional requirements

- Offline operation: all core features work without network access. External links are the only user-initiated network handoff; the app itself does not fetch remote content.
- Startup: target a visible window within 1 second on the reference development machine; measure in the reliability phase rather than treating an unmeasured target as achieved.
- Performance: typical documents under 1 MB should open and become readable without a noticeable pause. Files over 5 MB show a warning; 10 MB or larger are included in reliability testing. Virtualization/streaming is considered only if measurements show need.
- Memory: avoid unnecessary copies and release prior render trees when replacing a document. No unsupported hard memory guarantee.
- Accessibility: logical keyboard order, visible focus, screen-reader labels for images and controls, high contrast, and DPI checks at 100%, 150%, 200%, and 250%.
- Localization/direction: English and Arabic UI targets; per-paragraph RTL handling for Arabic/Hebrew and mixed-script text using Unicode bidi behavior. Do not mirror LTR code indiscriminately.
- Resilience: file and parser errors are surfaced without crashing; reload handles files modified or removed since opening.

## 5. Security and privacy

- Never execute Markdown HTML, scripts, macros, embedded commands, or custom URI handlers.
- Read only user-selected/dropped files and approved in-tree local image references. Validate normalized paths and reject directory escape, traversal, and symlink escape.
- Launch only explicitly clicked `http`/`https` URLs through the OS. Do not silently prefetch URLs or remote images.
- Do not add network clients, AI keys, telemetry, crash analytics, or account data.
- Use least-privilege package capabilities; no broad filesystem capability unless a reviewed decision and Store requirement justify it.
- Treat file contents as untrusted input; bound resource use, handle parse failures, and test malformed/large documents.

## 6. AI Control Surface (future boundary only)

The provider-neutral contracts, command registry, gateway placeholder, and security policy may define a future boundary. MVP has no AI assistant, provider integration, LLM, network transport, API keys, autonomous file modification, or AI-generated content. Any future enablement requires a separate owner-approved product change and security review. The current autonomous developer reads repository instructions/backlog; it does not control the running app through an AI integration.

## 7. Packaging and distribution

- Develop unpackaged through Phase 0–5; keep code MSIX-compatible.
- Phase 6 creates and validates an MSIX, install/uninstall, identity, signing workflow, and clean launch from the package. Signing secrets/certificates are not committed.
- Phase 7 checks Microsoft Store policies/certification, capabilities, privacy disclosures, EULA/privacy policy needs, and listing assets. Store submission/publication is a separate owner action and is not implied by readiness tasks.
- No custom updater; Store/MSIX distribution owns updates.

## 8. MVP acceptance boundary

MVP means: app launches; user opens one `.md`/`.markdown` file through picker, drag/drop, or CLI; readable basic Markdown renders in a scrollable view; Arabic/English and RTL/LTR content is legible; light/dark/system themes work; internal anchors and safe local images/links behave as specified; errors are recoverable; app is offline-first, accessible at baseline, and packaged as MSIX with Store readiness documented. Phase mapping in the roadmap orders these outcomes; a phase is not done until its task-level verification passes.

### Explicitly deferred

- Folder browser, directory crawling, multi-tab/multi-document workspace, folder-wide search.
- Persistent recent files, session restoration, saved scroll/window state, and saved preferences pending a product/architecture decision.
- Print support, syntax highlighting beyond a dependency-approved scope, extensive customization, plugins, collaboration/comments.
- Cloud sync, accounts, database, telemetry, custom updater, remote image retrieval.
- Any AI provider or in-app AI feature.
- Store submission/publishing itself.

### Never build under this product definition

Markdown editing or saving source; account/login and cloud synchronization; database-backed product features; telemetry/analytics; plugin marketplace; collaboration/comments; embedded browser automation; execution of embedded HTML/scripts or Markdown commands; built-in AI chatbot/provider integration. Reconsideration requires explicit owner approval and updated product/architecture records.

## 9. Conflicts and owner review items

Before implementation begins, the owner should confirm: (a) Windows 10 minimum, resolving 1809 vs 20H2; (b) whether user settings/recent files may persist, resolving the Product Spec vs UI Design discrepancy; (c) whether MVP includes images and external/file-relative links or keeps them in Phase 3; (d) whether print is excluded from MVP; and (e) whether the proposed in-memory recent list limit of 10 is useful. Until accepted, the current source-of-truth constraints above govern and ambiguous enhancements stay deferred.
