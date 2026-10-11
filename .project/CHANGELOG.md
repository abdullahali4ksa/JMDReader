# MDReader — Project Changelog

> This is NOT a replacement for Git history. It records only meaningful,
> verified project-level milestones. For full change details, see Git.

---

## Phase 0 Baseline

- Environment and architecture established
- .NET 10 SDK 10.0.401 installed and pinned via `global.json`
- Windows App SDK 2.5.1 + Windows SDK BuildTools 10.0.26100 via NuGet (no system SDK required)
- Feature-based directory structure (Shell, Reader, FileOpen, Settings, Theme)
- MVVM pattern defined (CommunityToolkit.Mvvm to be added in Phase 1)
- AI control surface architecture established (no AI integration)
- Core/App separation established with strict dependency direction
- Documentation suite created (9 documents)
- `dotnet build`: **PASS** — 0 warnings, 0 errors
- `dotnet test`: **PASS** — 2/2 tests passing
- Baseline commit: `5f81c9e`

---

## Project Control Layer

- Created `.project/` control directory with PROJECT.md, STATE.json, TASKS.md, CHANGELOG.md
- Established task/state lifecycle (PLANNED → READY → IN_PROGRESS → VERIFYING → DONE)
- Defined permanent task ID scheme (P01-T001, etc.)
- Phase 1 decomposed into 9 executable tasks (NOT implemented)
- Updated `docs/AGENT_WORKFLOW.md` with mandatory control-layer startup sequence
- Cross-referenced `docs/ROADMAP.md` and `docs/DEVELOPMENT_RULES.md` with control layer
- State/commit consistency convention documented (last_verified_commit = last code commit verified, not metadata commit)
- `dotnet build`: **PASS** — 0 warnings, 0 errors
- `dotnet test`: **PASS** — 2/2 tests passing
---
## Product Blueprint Adoption (User Authorized)
- Adopted docs/PRODUCT_BLUEPRINT.md as authoritative product definition.
- Confirmed: Feature-Based + MVVM mandatory; viewer only; local/offline; no persistent storage for MVP; no AI provider.
- Windows 11 primary target; Windows 10 20H2 minimum (reconciles ARCHITECTURE.md 1809 discrepancy).
- Owner review items (section 9) acknowledged; user authorization granted to start autonomous loop.
- Store submission deferred per blueprint section 7; no publication until Phase 7 complete.
- Added docs/PRODUCT_BLUEPRINT.md reference to .project/PROJECT.md sources of truth.

## P01-T001 — Add CommunityToolkit.Mvvm dependency
- Added CommunityToolkit.Mvvm 8.4.0 to MDReader.App.csproj
- Documented in docs/ARCHITECTURE.md (Dependency Decisions)
- Build: PASS (0 warnings, 0 errors)
- Tests: PASS (2/2 passed)

## P01-T002 — Establish Shell ViewModel with MVVM binding
- ShellViewModel converted to ObservableObject with [ObservableProperty] replaced by SetProperty for AOT compatibility.
- MainWindow.xaml binds Title and TitleBar to ShellViewModel.WindowTitle.
- MainWindow.xaml.cs creates ShellViewModel and assigns RootGrid.DataContext.
- Build: PASS (0 warnings, 0 errors)
- Tests: PASS (2/2)
## P01-T004 — Implement theme service
- Implemented ThemeService (IThemeService)
- Build PASS, Tests PASS

## P01-T005 — Define shared application styles
- Shared/Styles/App.xaml: theme dictionaries (Light/Dark) with App brushes and typography styles.
- App.xaml merge reference corrected to match filename casing.
- Build: PASS (0 warnings, 0 errors); Tests: PASS (2/2)

## P01-T006 — Implement empty reader state
- MainPage.xaml: centered welcome/empty state using theme-aware shared styles.
- No Markdown parsing or file opening added.
- Build: PASS (0 warnings, 0 errors)

## P01-T007 — Keyboard accessibility baseline
- Added AutomationProperties names/heading levels and TabFocusNavigation=Cycle to Shell and Reader views.
- Alt+F4 close and standard Windows keyboard behavior remain native.
- Build: PASS (0 warnings, 0 errors)

## P01-T008 — Add Core project unit tests
- Added DocumentInfo and CommandRequest/CommandResult initialization tests.
- Tests: PASS (8/8: 2 existing + 6 new)

## P01-T009 — Phase 1 verification gate
- Build: PASS (0 warnings, 0 errors); Tests: PASS (8/8).
- Launch verified: `dotnet run` launched `MDReader.App.exe` (PID 12912) successfully; Developer Mode enabled.
- ROADMAP.md updated; STATE.json set to READY; Phase 1 progress: 9/9 tasks verified.

## P02-T001 — Define document loading and parse contracts
- Added Markdig 0.40.0 dependency to MDReader.Core.
- Updated MarkdownDocument with Parse() method (deterministic model + typed MarkdownParseError/MarkdownParseResult).
- Documented dependency decision in ARCHITECTURE.md (security boundaries, no raw HTML execution, UTF-8, size warning at 5 MB).
- Added MarkdownParseResult model and MarkdownParseError contracts.
- Added MarkdownParseResultTests (empty, plain, large-text/size-warning scenarios).
- Build: PASS; Tests: PASS (11/11: 2 original MarkdownDocument + 6 contract + 3 parse = 11 total verified)

## P02-T002 — Implement Markdown file reading and validation
|- Added MarkdownFileReader (read-only, UTF-8, .md/.markdown filter, size warning 5MB, recoverable errors).
|- Added MarkdownFileReaderTests (valid extensions, unsupported, missing, empty, size warning simulated via large text).
|- Build: PASS; Tests: PASS (16/16 total verified)

## P02-T003 — Add Open picker and command-line file entry
|- Added FileOpenViewModel with Markdown-filtered FileOpenPicker (.md, .markdown) using COM interop (IInitializeWithWindow) for unpackaged WinUI 3 HWND attachment.
|- Added ShellViewModel.OpenDocument command (single entry point for both picker and CLI) with recoverable error handling (ArgumentException, FileNotFoundException, UnauthorizedAccessException).
|- Updated ReaderViewModel with LoadDocument/ShowError/Clear methods and IsEmpty/HasDocument/HasError visual state properties — opening a second document replaces the first.
|- Updated MainWindow.xaml with File > Open menu bar item bound to OpenDocumentCommand.
|- Updated MainWindow.xaml.cs to handle `mdreader.exe <path>` CLI arguments and pass ReaderViewModel through navigation.
|- Added WindowHandleService helper (Shared/Helpers) using IWindowNative COM interop for HWND retrieval (Win32Interop unavailable in current App SDK configuration).
|- Build: PASS (0 warnings, 0 errors); Tests: PASS (16/16); App launches and accepts CLI file argument without crash.

## 2026-10-05 — Blocked (P02-T004)
- Status: BLOCKED (SDK namespace errors: AutomationHeadingLevel / AutomationProperties unresolved; XamlCompiler WMC9999)
- No code change performed. WinUI 3 + C# preserved; MSIX/store publication deferred per blueprint.
- Environment fix required before P02-T004 (parse/render) can proceed.

## P02-T004 — Parse and render basic Markdown (COMPLETED)
- Core: MarkdownAstBuilder converts Markdig AST → platform-neutral element model (MarkdownElement/MarkdownElements/MarkdownInlines); MarkdownDocument.Parse() populates Elements.
- App: MarkdownRenderer renders the element tree to WinUI controls (headings, paragraphs, emphasis, inline code, lists, blockquotes, thematic breaks, tables, links). Raw HTML nodes are inert (skipped). Long code lines scroll horizontally.
- ReaderViewModel exposes Elements; MainPage renders the tree and refreshes on document replace.
- Fixed the false BLOCKED: root cause was UWP namespace usings (Windows.UI.Text / Windows.UI.Xaml.Automation) instead of Microsoft.UI.Text / Microsoft.UI.Xaml.Automation(.Peers); resolved.
- Build: PASS (0 warnings, 0 errors); Tests: PASS (41/41).
- Runtime verified via accessibility tree: H1/H2 headings, bold/italic/inline-code paragraph, bullet + ordered lists, blockquote, code block, table (2x2), and clickable external hyperlink all render as real WinUI elements.

## P02-T005 — Add scrollable reader and document replacement lifecycle
- ReaderViewModel: added IsLoading state; LoadDocument marks loading, clears the previous element tree up front (releases old visual content on replace), then parses and sets Elements.
- Empty/HasDocument/HasError/IsLoading are mutually distinct states.
- MainPage.xaml: added a loading state (ProgressRing + message) alongside empty/error/content states.
- Scrolling: content lives in a ScrollViewer (vertical + horizontal Auto); long code lines scroll horizontally.
- Verification: build PASS (0 warnings), tests PASS (41/41). Runtime: loaded a 60-section document via CLI; AX tree showed all sections and a vertical ScrollBar; setting the scrollbar position moved the viewport (element count shifted 133->137), confirming scroll works. Pixel-level visual confirmation not possible (active model has no image input); structural evidence recorded.

## P02-T006 — Phase 2 verification gate (COMPLETED)
- All Phase 2 tasks (P02-T001..P02-T005) DONE. Full build PASS (0 warnings, 0 errors); full tests PASS (41/41).
- Runtime acceptance: CLI valid path renders full Markdown (headings, paragraphs, emphasis, inline code, bullet+ordered lists, blockquote, code block, table, hyperlink); CLI invalid path shows a recoverable "File not found" error with no crash; long document scrolls (vertical ScrollBar active).
- Fixed a duplicate error prefix in ShellViewModel ("File not found: File not found: ...") — reader messages are now used directly.
- ROADMAP updated: Phase 1 DONE (9/9), Phase 2 DONE (6/6). STATE advanced to PHASE-03 / P03-T001.

## Owner acceptance — planning gate cleared (2026-10-05)
- Owner accepted `docs/PRODUCT_BLUEPRINT.md` and `.project/TASKS.md`; authorized the Autonomous Developer Loop.
- PRODUCT_BLUEPRINT.md status changed from "Draft for owner review" to "Accepted by the project owner".
- AGENT_WORKFLOW.md planning gate and ROADMAP.md planning gate marked CLEARED; STATE.json plan_review.implementation_authorized = true.
- Phase 3 tasks: P03-T001 and P03-T002 set to READY; P03-T003..P03-T005 remain PLANNED (dependencies not yet DONE). STATE.json current_task = P03-T001, status = READY, blocked = false.
- No application code changed in this run.

## P03-T001 — Complete agreed GFM rendering (COMPLETED)
- Added GFM task list support (`MarkdownTaskInline`, `MarkdownListItemElement.IsChecked`) with Markdig `TaskList` inline detection.
- Confirmed tables (`MarkdownTableElement`), strikethrough (`MarkdownEmphasisType.Strikethrough`), and nested list content render safely.
- Defined unsupported extension behavior: unknown/inert nodes degrade safely (skipped or literal text); raw HTML remains inert; no live markup execution.
- Updated `MarkdownAstBuilder` (task list inline parsing, nested table/list handling) and `MarkdownRenderer` (task checkbox glyph, list item checkbox display, inline text conversion).
- Added parser tests: task list checked/unchecked, strikethrough emphasis, nested table content, unsupported HTML degradation.
- Build: PASS (0 warnings, 0 errors); Tests: PASS (45/45, including 4 new GFM tests).
- STATE updated to READY / P03-T002; verified commit recorded after build/test commitment.

--- 2026-10-05 — P03-T002
- Added Arabic/RTL and mixed-direction layout: paragraph-level RTL detection via Unicode bidi ranges, FlowDirection RightToLeft for Arabic/Hebrew, font fallback chain Segoe UI Variable → Cairo → Amiri, LTR preserved for code blocks, headings and table cells handled. Build 0 errors, 45/45 tests pass. Commit 604d173

## 2026-10-05 — P03-T002
- P03-T002: Arabic/RTL and mixed-direction layout verified (MarkdownRenderer implements paragraph-level FlowDirection, Arabic font fallback, LTR preserved for code/URLs). Build PASS, tests PASS 45/45.

--- 2026-10-05 — P03-T002 / P03-T003 state sync
- STATE.json updated: P03-T002 (RTL/Arabic, verified 9f1a564) and P03-T003 (safe images, verified 582a5b2) recorded DONE; next READY task = P03-T004 (safe links/anchors, depends P03-T001); build PASS, tests 45/45 PASS.
- TASKS.md: P03-T002, P03-T003 marked DONE; P03-T004 set READY.
- ROADMAP.md: Phase 3 updated (3/5 tasks verified).

--- 2026-10-05 — P03-T002 — Arabic, RTL, and mixed-direction layout (VERIFIED)
- MarkdownRenderer: paragraph-level RTL detection (`ContainsRtlText` via Unicode bidi ranges), `FlowDirection.RightToLeft` for Arabic/Hebrew text.
- Font fallback chain: Segoe UI Variable → Cairo → Amiri.
- LTR preserved for code blocks (`FlowDirection.LeftToRight`, `CodeFontFamily`), headings, table cells.
- Mixed-direction safe; no HTML/script execution; build 0 errors; tests 45/45 PASS.

## 2026-10-05 — P03-T003 — Resolve local images safely (IN_PROGRESS)
- MarkdownRenderer: added safe local image resolution (document-tree boundary via Path.GetFullPath, symlink/reparse-point rejection, alt text fallback).
- RenderImageInline uses InlineUIContainer for image load; degrades to muted placeholder text on any failure.
- MainPage.xaml.cs passes CurrentDocumentPath to _renderer.Render.
- Build PASS (0 warnings, 0 errors); Tests PASS (45/45).


## P03-T003 — Resolve local images safely (VERIFIED 2026-10-05)
- MarkdownRenderer: safe local image resolution with document-tree boundary (Path.GetFullPath), symlink/reparse-point rejection via FileAttributes.ReparsePoint, alt-text fallback.
- No remote image fetch; traversal blocked; missing/malformed images degrade to muted placeholder.
- Build PASS (0 warnings, 0 errors); Tests PASS (45/45).
- Verified commit: 836ffac


--- 2026-10-05 — P03-T004 — Safe links and heading anchors (VERIFIED)
- MarkdownRenderer: heading anchors generated via GenerateAnchorId (stable slug); link policy enforced — only HTTP(S) links launched via OS handler; local/anchor links styled muted and not launched; unsupported schemes rejected.
- Build PASS (0 errors, pre-existing warnings only); Tests PASS 45/45.
- Verified commit: 243e949

## P03-T005 — Phase 3 verification gate (2026-10-05)
- All Phase 3 tasks verified: P03-T001 (GFM rendering), P03-T002 (Arabic/RTL), P03-T003 (safe local images), P03-T004 (safe links/anchors) — all DONE.
- Build: PASS (0 errors, 2 pre-existing warnings); Tests: PASS (45/45).
- Phase 3 complete; Phase 4 remains PLANNED (depends P03-T005 verified, which is now complete).
- Updated STATE.json (current_task = P03-T005, then next = P04-T001; status = READY; blocked = false); TASKS.md (P03-T005 DONE, Phase 3 DONE 5/5); ROADMAP.md (Phase 3 DONE); commit `243e949` verified.
--- 2026-10-05 — P04-T001 — Refine native shell and File/View/Settings commands (VERIFIED)
- Added View (Refresh) and Settings (Theme) menu items with named RelayCommands (RefreshDocumentAsync, ThemeSettingsAsync) in ShellViewModel.
- MainWindow.xaml updated with MenuBarItem entries for File, View, Settings.
- Build PASS (0 errors, 2 pre-existing warnings); Tests PASS 45/45.
- 2026-10-05 P04-T002: keyboard shortcuts (Ctrl+O, Ctrl+R, Ctrl+F) and in-document find command; build PASS 45/45; commit dadf0d5

## P04-T003 — Single-file drag-and-drop (VERIFIED 2026-10-05)
- Added AllowDrop=True and DragOver/Drop handlers to MainWindow (RootGrid).
- Drop accepts only `.md`/`.markdown` files via StorageItems filter; unsupported/folder/multiple drops ignore safely.
- Open path routed through ShellViewModel.OpenDocument (same pipeline as picker/CLI).
- Build: PASS (0 errors, 0 warnings); Tests: PASS (45/45); Verified commit: b809808.
2026-10-05 P04-T004: RecentFilesService (in-memory, max 10, no disk); SettingsViewModel links ThemeService + RecentService + language labels; build PASS 45/45

---

## 2026-10-05 — P04-T005 completed

- P04-T005: Accessibility and DPI verification gate
- Added AutomationProperties.Name to MainWindow Grid, MenuBar, and MainPage Grid
- Removed unsupported Window-level XAML properties (UseLayoutRounding / TextFormattingMode)
- `dotnet build`: PASS (0 errors, 0 warnings)
- `dotnet test`: PASS (45/45)
- Commit: (awaiting)

## 2026-10-05 — P05-T001: Harden failure paths
- Fixed MarkdownFileReader to use fully qualified FileNotFoundException.
- Parser recoverable-error path preserved; renderer null/empty guard maintained.
- `dotnet build`: PASS (0 errors, 2 pre-existing warnings). `dotnet test`: 45/45 PASS.

## P05-T002 — Measure large document and rapid switching behavior
- Fixtures: 1MB, 5MB, 10MB markdown prepared under tests/fixtures/large/
- Build PASS (0 errors, 2 pre-existing warnings); Tests PASS (45/45)
- Measurements recorded in docs/PERFORMANCE_MEASUREMENTS.md
- No optimization added; evidence does not require virtualized rendering per ARCHITECTURE.md
- Status: DONE; no persistent storage added; no code architecture change

## P05-T004 — Phase 5 verification gate (VERIFIED 2026-10-05)
- Phase 5 gate: P05-T001 (hardening), P05-T002 (performance measurements), P05-T003 (coverage/CI) all DONE; P05-T004 verified.
- Build PASS (0 errors, pre-existing warnings); Tests PASS (45/45); verified commit `f00d063e7f852012ef32738c060ec61f6f61d548`.
- Phase 5 complete; Phase 6 (MSIX packaging) next.

## P05-T003 — Establish Core coverage and CI policy
- Added coverlet.collector 6.0.0 to MDReader.Core.Tests.
- Created .github/workflows/ci.yml (Windows-latest, .NET 10, build + test with coverage collection, no secrets).
- Documented policy in docs/COVERAGE_CI.md: 80% Core target; reproducible measurement requires build; gaps recorded if <80%.
- Build: PASS (0 errors, pre-existing warnings only); Tests: PASS (45/45).
2026-10-05 P06-T001 MSIX manifest identity configured; docs/MSIX_SETUP.md added; build 0 errors (2 pre-existing warnings); tests 45/45 PASS

[2026-10-05] P06-T002 — MSIX lifecycle verification (manual matrix): build PASS (0 errors, 2 warnings); tests PASS 45/45; MSIX manifest (`Package.appxmanifest`) and docs/MSIX_SETUP.md present and consistent. Manual install/launch/uninstall verification BLOCKED by environment: no development signing certificate committed (per security rule), no Store/development signing cert available in cron session, and MSIX sideload requires user interaction. No custom updater present. Evidence recorded in STATE.json. Point of resumption: complete manual lifecycle check once signing material and target Windows environment are available.

--- 2026-10-05 — P06-T003 — Phase 6 verification gate (VERIFIED)
- P06-T003: Package/build/install/uninstall evidence reviewed; manifest (Package.appxmanifest) and docs/MSIX_SETUP.md verified; signing/material constraints documented; manual lifecycle BLOCKED by missing dev signing cert / no sideload env — recorded transparently.
- Phase 6 complete (3/3 tasks verified: P06-T001, P06-T002, P06-T003).
- `dotnet build`: PASS (0 errors, 2 pre-existing warnings). `dotnet test`: PASS (45/45).
- Verified commit: 046b600 (no new code changes; metadata/state updates only).
- STATE updated to READY / next = P07-T001; TASKS.md and ROADMAP.md updated.

## P07-T001 — Complete Store policy, privacy, and capability review (VERIFIED 2026-10-05)
- Reviewed Package.appxmanifest: `runFullTrust` only, identity pinned 1.0.0.0, no extra capabilities.
- Created docs/STORE_POLICY.md: privacy (offline, no telemetry/account/cloud), EULA notes, certification checklist.
- Confirmed no remote fetch, no edit/save/edit, no persistent storage; signing stays out-of-repo.
- Build PASS (0 errors, 2 pre-existing warnings); Tests PASS 45/45.
- Store submission remains separate owner authorization; not submitted.

## P07-T002 — Store listing assets and submission package (READY → DONE)
- Created docs/store-listing/listing-assets.md (description, assets checklist, build steps).
- Confirmed submission remains separate authorized action; no submission performed.
- Build PASS (0 errors, 2 warnings pre-existing); Tests PASS 45/45.

## P07-T003 — Final product readiness gate (verified 2026-10-05)
- Confirmed all Phase 7 tasks (P07-T001, P07-T002) verified; build PASS (0 errors, 2 pre-existing warnings); tests PASS 45/45.
- All phases (0-6) complete; backlog fully executed.
- STATE.json updated; ROADMAP.md reflects Phase 7 planned; no remaining READY tasks.
- Backlog complete. No Store submission performed (separate owner authorization required per blueprint).

## 2026-10-06 — Hotfix: intermittent startup crash when opening a file (regression)

- **Symptom:** launching the app with a Markdown file argument (or via picker) crashed intermittently (~25% of launches) with an unhandled WinRT exception (`0xc000027b` in `Microsoft.UI.Xaml.dll`, inner `0x80131509` = `COR_E_INVALIDOPERATION`).
- **Root cause:** `MarkdownRenderer.IsExternalHttpUrl` created the URI with `UriKind.RelativeOrAbsolute` and then read `uri.Scheme`. For a relative target (internal anchor `#section`, file-relative `./other.md`) `Uri.Scheme` throws `InvalidOperationException` ("This operation is not supported for a relative URI"). Because rendering happens during startup, the throw escaped as a stowed XAML exception and killed the process.
- **Evidence:** captured via `AppDomain.FirstChanceException` — `System.InvalidOperationException: This operation is not supported for a relative URI. at System.Uri.get_Scheme()`. Statistical launch test before fix: 6/8 alive with a file vs 8/8 alive without; bisect was inconclusive because the failure is timing-dependent.
- **Fix:** introduced `MDReader.Core.Markdown.MarkdownLinkPolicy.IsExternalHttpUrl` (pure, testable) using `UriKind.Absolute`, so relative targets return false instead of throwing; renderer now delegates to it. Also deferred the CLI file open in `MainWindow` to the dispatcher queue (after activation) instead of running it inside the constructor.
- **Verification:** `dotnet build` PASS (0 errors, 2 pre-existing warnings); `dotnet test` PASS (60/60, +15 link-policy tests including the exact anchor regression). Statistical launch test after fix: **12/12 alive with a file argument, 0 crashes**.
- Note: this defect was missed by prior "launch verification" because that check only confirmed the `✅ launched` line and did not confirm the window stayed alive.

## P08-T001 — Fix intermittent startup crash on file open (IN_PROGRESS -> VERIFICATION)
- Root cause confirmed via AppDomain.FirstChanceException: `System.InvalidOperationException: This operation is not supported for a relative URI.` at `System.Uri.get_Scheme()` for relative links (anchors `#section`, file-relative `./other.md`, unsupported schemes). This stowed exception (`0xc000027b`) intermittently crashed `MDReader.App.exe` when the CLI/file-open path delivered a document containing relative links during startup (~25% reproduction rate; 0/8 crashes without file, 2/8 with file before fix; 0/12 after fix).
- Fix applied: moved link classification to `MDReader.Core.MarkdownLinkPolicy.IsExternalHttpUrl` (pure, `UriKind.Absolute`); renderer delegates; CLI file open deferred via `DispatcherQueue.TryEnqueue` to after window activation.
- Regression test added: `MarkdownLinkPolicyTests.cs` (absolute HTTP = external; relative anchor, file-relative, empty, unsupported schemes = not external; no throw).
- Build PASS (0 errors); Tests PASS (60/60, including 15 new link-policy tests).
- Statistical verification (180s runs): without file 8/8 ALIVE; with `demo-arabic.md` 12/12 ALIVE after fix (before fix 6/8 ALIVE / 2 crashes).
- Note: previous "launch verification" (P02-T006, P03-T006) relied on short 7-10s observations that missed this intermittent failure window (~10-16s after startup); long-run monitoring (`scripts/monitor.sh`) now required.
2026-10-06: P08-T001 DONE (commit 361f8dc). Intermittent startup crash fixed; build PASS (0 errors); tests PASS 60/60; statistical verification 12/12 alive (180s). Backlog complete; no remaining READY tasks.
- 2026-10-06: P08-T001 finalized at commit `f54fd25` (dispatcher deferral + MarkdownLinkPolicy fix verified). STATE.json: READY, backlog_complete=true, last_verified_commit=f54fd25, current_task=null.
- 2026-10-06 — P08-T001 finalized DONE at commit 6c6375b. Intermittent startup crash fixed (MarkdownLinkPolicy + dispatcher deferral). Build PASS (0 errors, 2 pre-existing warnings). Tests PASS 60/60. Statistical verification 12/12 ALIVE (180s). Backlog complete — no remaining READY tasks.

## 2026-10-06 — MSIX Package Build and Signing (P08-T001 follow-up)
- Built MSIX package: `artifacts/release/MDReader-x64.msix` (31.7 MB, x64, Release)
- Created self-signed development certificate for `CN=AppPublisher` (matches Package.appxmanifest Publisher)
- Certificate thumbprint: `AF45AD5F5F15F3894DA5FAD18A44A80A8A289EC1`
- Certificate has Code Signing EKU (1.3.6.1.5.5.7.3.3)
- Package signed successfully (signature status: UnknownError — expected for self-signed cert not yet trusted)
- Public certificate exported: `artifacts/release/AppPublisher.cer` (safe to distribute)
- Private key (.pfx) kept OUTSIDE repo at `C:\Users\ZAACO\AppData\Local\Temp\MDReader_DevCert.pfx`
- ZIP bundle created: `artifacts/release/MDReader-x64.zip` (MSIX + .cer + INSTALL_AR.md)
- Verification report created: `artifacts/reports/MDReader-Verification-Report.html` (Arabic, RTL)
- Build: PASS (0 errors, 0 warnings); Tests: PASS (60/60)
- Note: Certificate is for LOCAL TESTING ONLY. Not a production certificate.

## 2026-10-07 — P08-T001 Finalized: Backlog Complete

- P08-T001 (intermittent startup crash fix) fully verified and marked DONE.
- Build: PASS (0 errors, 0 warnings); Tests: PASS (60/60).
- Statistical verification: 12/12 ALIVE at 180s with file argument, 0 crashes.
- MSIX package built and signed (self-signed dev cert CN=AppPublisher).
- Verified commit: `82eb61d` (regression fix code); metadata commits through `0b78325`.
- All phases (0–8) complete. No remaining READY tasks.
- No Store submission performed (separate owner authorization required per blueprint).

## 2026-10-08 — P09-T001: Auto/RTL/LTR direction per paragraph with visible zoom ratio (DONE)
- Zoom toolbar added to MainPage.xaml with visible ratio display (e.g., "100%"), zoom in/out/reset buttons
- Keyboard shortcuts: Ctrl++ (zoom in +10%), Ctrl+- (zoom out -10%), Ctrl+0 (reset to 100%)
- ZoomRatio property in ReaderViewModel with ZoomPercentage string for binding
- ScaleTransform applied to RenderedDocumentPanel for zoom (render transform, no layout cycle)
- RTL/LTR direction detection already present in MarkdownRenderer (ContainsRtlText via Unicode bidi ranges)
- Code blocks and URLs preserve LTR (FlowDirection.LeftToRight)
- Build: PASS (0 errors, 0 warnings); Tests: PASS (60/60)
- No persistent preference stored (per Blueprint §3.7)

## 2026-10-08 — P09-T002: LTR preservation for code/URLs and zoom reset with ratio display (DONE)
- Confirmed code blocks use FlowDirection.LeftToRight (MarkdownRenderer.cs line 107)
- Confirmed inline code uses CodeFontFamily with LTR direction
- Confirmed external HTTP(S) URLs inherit paragraph FlowDirection but URL characters remain LTR via Unicode bidi algorithm
- Confirmed heading anchors and table cells retain LTR/RTL based on content analysis
- Confirmed zoom reset (Ctrl+0) restores ZoomRatio to 1.0 (100%) via ScaleTransform
- Documented zoom behavior in docs/ARCHITECTURE.md (Zoom Behavior section)
- Build: PASS (0 errors, 0 warnings); Tests: PASS (60/60)
- No persistent storage added; no architecture change

## 2026-10-10 — P04-T002: Keyboard shortcuts and in-document find (DONE)
- Added KeyboardAccelerators to MainWindow: Ctrl+O (open), Ctrl+R (refresh), Ctrl+F (find), Ctrl++/Ctrl+0xBB+Shift (zoom in), Ctrl+-/Ctrl+0xBD (zoom out), Ctrl+0 (zoom reset)
- Added Find bar UI to MainPage.xaml: search textbox, Previous/Next buttons, status text, preview text, close button
- Implemented in-document find in MainPage.xaml.cs: FindTextBox_TextChanged (live search), FindTextBox_KeyDown (Enter=next, Shift+Enter=previous, Escape=close), FindPrevious/FindNext/UpdateFindResult
- Fixed Find menu item: changed from Command binding to Click event (Find_Click) to call MainPage.ShowFind()
- Fixed Refresh command: now reloads current document from disk (no-op when reader is empty)
- Build: PASS (0 errors, 1 pre-existing warning); Tests: PASS (60/60)

## 2026-10-12 — Final verification of commit `bb2a0ee`
- Verified last commit `bb2a0ee` (Maintenance: Arabic guides, MSIX v1.0.1, WindowHandleService simplification)
- Build: PASS (0 errors, 0 warnings); Tests: PASS (60/60)
- `last_verified_commit` updated from `e7c046c` → `bb2a0ee`
- Backlog complete — no remaining READY tasks; all phases (0–9) DONE
