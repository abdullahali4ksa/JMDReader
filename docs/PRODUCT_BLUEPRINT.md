# MDReader Product Blueprint

**Status:** Accepted by the project owner (2026-10-05). This blueprint is the authoritative product plan; implementation is authorized via the Autonomous Developer Loop, task by task, within the file scopes and acceptance criteria defined in `.project/TASKS.md`.
**Authority:** This blueprint expands `docs/PRODUCT_SPEC.md` and must remain consistent with `docs/ARCHITECTURE.md`, `docs/UI_DESIGN.md`, `docs/FEATURE_MAP.md`, `docs/ROADMAP.md`, `docs/DECISIONS.md`, `.project/PROJECT.md`, and `.project/STATE.json`.
**Product:** MDReader — a focused, offline Windows Markdown reader.

## 1. Product definition

MDReader opens Markdown documents and presents them as readable formatted content in a small, native Windows desktop application. It is for reading and previewing documents, not authoring them. It should start quickly, behave predictably with local files, support Arabic and English, and avoid accounts, cloud services, telemetry, and network dependencies.

### Intended users

- People who receive or keep Markdown notes and want to read them without opening a code editor.
- Developers and technical writers reviewing README, project notes, and documentation.
- Students and knowledge workers reading local study notes.
- Arabic/English readers who need correct RTL, LTR, and mixed-direction text.

Windows 10 20H2 or later is the minimum product target as stated in `PRODUCT_SPEC.md`; Windows 11 is the primary design target. (The older Windows 10 1809 statement in `ARCHITECTURE.md` must be reconciled before implementation.)

## 2. Product principles and hard boundaries

1. Viewer only: never edit, save, or overwrite document content.
2. Local and offline: opening and rendering files must not require a network connection.
3. Explicit access: access only files/folders selected or dropped by the user; no broad disk scan.
4. Core/App separation and feature-based MVVM remain mandatory.
5. Keep the first release small. No database, accounts, cloud sync, telemetry, plugins, or AI provider.
6. Raw embedded HTML is not rendered. Links and images follow the safe subset in the architecture document.
7. Current product spec prohibits persistent local product data beyond temporary cache. Recent items therefore exist only in the current process; session restore and persisted preferences are deferred until a documented decision changes that constraint.
8. Do not begin implementation until the owner reviews this blueprint and the backlog and explicitly starts the autonomous development loop.

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
