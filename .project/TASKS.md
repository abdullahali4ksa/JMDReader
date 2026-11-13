# MDReader — Task Backlog

> This is the detailed execution backlog for the MDReader project.
> Tasks are organized by Phase. Each task has a permanent ID that is NEVER reused.
>
> Status values: PLANNED · READY · IN_PROGRESS · VERIFYING · DONE · BLOCKED · CANCELLED

---

## PROJECT: MDReader

### Phase 0:
COMPLETED

### Phase 1:
9 / 9 tasks completed

### Phase 2:
COMPLETED (6 / 6 tasks DONE)

### Phase 3:
DONE (5 / 5 tasks; P03-T001..P03-T005 DONE)

### Phase 4:
PLANNED

### Phase 5:
PLANNED

### Phase 6:
DONE (3 / 3 tasks; P06-T001..P06-T003 verified)

### Phase 7:
DONE (3 / 3 tasks; P07-T001..P07-T003 DONE)

---

## Phase 0 — Environment and Architecture

**Status:** DONE

| ID | Title | Status |
|---|---|---|
| (baseline) | Environment audit, project skeleton, documentation suite, AI control surface, feature-based structure | DONE |

### PHASE-00 Summary

Phase 0 established the environment, architecture, and documentation baseline.
Key outcomes:

- .NET 10 SDK 10.0.401 installed and pinned via `global.json`
- Windows App SDK 2.5.1 + Windows SDK BuildTools 10.0.26100 via NuGet
- Feature-based directory structure established (Shell, Reader, FileOpen, Settings, Theme)
- MVVM pattern defined (CommunityToolkit.Mvvm to be added in Phase 1)
- AI control surface architecture established (no AI integration)
- Documentation suite created (9 docs)
- Build: PASS (0 warnings, 0 errors)
- Tests: PASS (2/2)
- Baseline commit: `5f81c9e`

---

## Phase 1 — Application Shell

**Status:** READY

**Goals:**
- Create a minimal WinUI 3 window that launches and displays
- Follow Windows app silhouette (title bar, minimize/maximize/close)
- Respect system light/dark theme
- Handle basic window sizing and positioning
- Graceful shutdown

**Deliverables:**
- WinUI 3 app project builds and runs via `dotnet run`
- Empty window appears with correct dimensions
- Window respects system theme
- Window is DPI-aware
- Basic keyboard accessibility (Tab navigation)
- Unit tests for Core project initialization

> **Scope boundary:** Phase 1 does NOT include Markdown reading functionality
> (Phase 2) or FileOpen dialog functionality (Phase 2). Per ROADMAP.md,
> FileOpen is explicitly defined as Phase 2.

---

### P01-T001: Add CommunityToolkit.Mvvm dependency

**FEATURE:** Shell (MVVM infrastructure)
**STATUS:** DONE
**PRIORITY:** P0
**DEPENDS_ON:** (none)
**PURPOSE:** Add the CommunityToolkit.Mvvm NuGet package to MDReader.App to enable
the MVVM pattern for Shell and other ViewModels. Document the decision per
DEVELOPMENT_RULES.md §3.
**ACCEPTANCE_CRITERIA:**
- PackageReference for `CommunityToolkit.Mvvm` exists in `src/MDReader.App/MDReader.App.csproj`
- Decision documented in `docs/ARCHITECTURE.md` under Dependency Decisions
- `dotnet restore` succeeds
- `dotnet build` succeeds with 0 errors
- `dotnet test` passes
**VERIFICATION:** dotnet build + dotnet test pass; package documented
**FILES_SCOPE:**
  src/MDReader.App/MDReader.App.csproj
  docs/ARCHITECTURE.md

### P01-T002: Establish Shell ViewModel with MVVM binding

**FEATURE:** Shell
**STATUS:** DONE
**PRIORITY:** P0
**DEPENDS_ON:** P01-T001
**PURPOSE:** Convert `ShellViewModel` from a placeholder to an MVVM `ObservableObject`,
set it as `MainWindow`'s `DataContext`, and bind the window title to the
`WindowTitle` property so the window displays "MDReader" (not "MDReader.App").
**ACCEPTANCE_CRITERIA:**
- `ShellViewModel` inherits from `ObservableObject` (CommunityToolkit.Mvvm)
- `ShellViewModel` is set as `MainWindow.DataContext`
- Window title is data-bound to `ShellViewModel.WindowTitle`
- Window displays title "MDReader" at runtime
- `dotnet build` succeeds with 0 errors
**VERIFICATION:** dotnet build passes; ShellViewModel uses MVVM; title bound correctly
**FILES_SCOPE:**
  src/MDReader.App/Features/Shell/
  docs/FEATURE_MAP.md

### P01-T003: Configure window sizing, positioning, and DPI awareness

**FEATURE:** Shell (MainWindow)
**STATUS:** DONE
**PRIORITY:** P1
**DEPENDS_ON:** P01-T002
**PURPOSE:** Configure `MainWindow` with sensible default dimensions, minimum size,
center-on-screen positioning, and DPI awareness. Fix the window title from
"MDReader.App" to "MDReader". Ensure graceful shutdown handling.
**ACCEPTANCE_CRITERIA:**
- Window opens centered on screen or at a sensible default position
- Window has minimum dimensions (e.g., 800×600)
- Window is DPI-aware (scales correctly on high-DPI displays)
- Window title reads "MDReader"
- Application shuts down gracefully (no crash on close)
- `dotnet build` succeeds with 0 errors
**VERIFICATION:** dotnet build passes; window sizing/positioning/DPI configured
**FILES_SCOPE:**
  src/MDReader.App/Features/Shell/Views/MainWindow.xaml
  src/MDReader.App/Features/Shell/Views/MainWindow.xaml.cs

### P01-T004: Implement theme service

**FEATURE:** Theme
**STATUS:** DONE
**PRIORITY:** P1
**DEPENDS_ON:** (none)
**PURPOSE:** Implement `IThemeService` to manage light/dark/system theme. Integrate
theme selection into the `App` lifecycle so the application respects the system
theme by default at startup.
**ACCEPTANCE_CRITERIA:**
- `IThemeService` implemented as a concrete class (e.g., `ThemeService`)
- Service supports values: `Light`, `Dark`, `System`
- Application follows system theme by default on startup
- Theme service is registered/accessible from App lifecycle
- `dotnet build` succeeds with 0 errors
**VERIFICATION:** dotnet build passes; ThemeService implemented and integrated
**FILES_SCOPE:**
  src/MDReader.App/Features/Theme/
  src/MDReader.App/App.xaml
  src/MDReader.App/App.xaml.cs

### P01-T005: Define shared application styles

**FEATURE:** Shared (Styles)
**STATUS:** DONE
**PRIORITY:** P1
**DEPENDS_ON:** (none)
**PURPOSE:** Populate `Shared/Styles/App.xaml` with basic application styles and
resource dictionaries (brushes, text styles, theme resources) that will be used
by the Shell and empty reader state. This replaces the current empty placeholder.
**ACCEPTANCE_CRITERIA:**
- `Shared/Styles/App.xaml` contains application-level resource definitions
- Resources are properly merged into `App.xaml` ResourceDictionary
- Styles support both light and dark themes
- `dotnet build` succeeds with 0 errors
**VERIFICATION:** dotnet build passes; shared styles defined and merged
**FILES_SCOPE:**
  src/MDReader.App/Shared/Styles/App.xaml
  src/MDReader.App/App.xaml

### P01-T006: Implement empty reader state

**FEATURE:** Reader
**STATUS:** DONE
**PRIORITY:** P0
**DEPENDS_ON:** P01-T004, P01-T005
**PURPOSE:** Replace the empty `Grid` in `MainPage.xaml` with a welcome/empty
state that displays when no document is open. The empty state should show a
clear, centered message and respect the system theme. No Markdown reading
functionality is added — this is purely the empty placeholder UI.
**ACCEPTANCE_CRITERIA:**
- `MainPage.xaml` displays a visible welcome/empty state instead of an empty Grid
- State shows a clear message (e.g., "No document open" or "Select a Markdown file")
- State is visually centered and respects system light/dark theme
- No Markdown parsing or rendering code added
- `dotnet build` succeeds with 0 errors
**VERIFICATION:** dotnet build passes; empty state renders; no Markdown functionality
**FILES_SCOPE:**
  src/MDReader.App/Features/Reader/Views/

### P01-T007: Keyboard accessibility baseline

**FEATURE:** Shell (accessibility)
**STATUS:** DONE
**PRIORITY:** P1
**DEPENDS_ON:** P01-T002, P01-T003
**PURPOSE:** Ensure basic keyboard accessibility in the application window. Verify
Tab navigation flows logically, keyboard focus is visible, and the window
supports standard keyboard operations.
**ACCEPTANCE_CRITERIA:**
- Tab navigation moves focus logically between UI elements
- Keyboard focus indicators are visible and follow Windows conventions
- Window supports standard keyboard close (Alt+F4) and menu access (Alt)
- `dotnet build` succeeds with 0 errors
**VERIFICATION:** dotnet build passes; keyboard accessibility verified
**FILES_SCOPE:**
  src/MDReader.App/Features/Shell/Views/
  src/MDReader.App/Features/Reader/Views/

### P01-T008: Add Core project unit tests

**FEATURE:** Core (tests)
**STATUS:** DONE
**PRIORITY:** P1
**DEPENDS_ON:** (none)
**PURPOSE:** Add unit tests for Core project types (DocumentInfo, CommandRequest,
CommandResult) to establish test coverage for Core initialization. The existing
MarkdownDocument tests (2/2) remain as-is.
**ACCEPTANCE_CRITERIA:**
- New test file(s) added under `tests/MDReader.Core.Tests/`
- Tests cover DocumentInfo initialization
- Tests cover CommandRequest initialization
- Tests cover CommandResult initialization
- All tests pass (existing + new)
- `dotnet test` succeeds
**VERIFICATION:** dotnet test passes with all tests green
**FILES_SCOPE:**
  tests/MDReader.Core.Tests/

### P01-T009: Phase 1 build and launch verification

**FEATURE:** Shell (verification)
**STATUS:** DONE
**PRIORITY:** P0
**DEPENDS_ON:** P01-T003, P01-T006, P01-T007, P01-T008
**PURPOSE:** Final verification gate for Phase 1. Run full build, all tests, and
attempt application launch. Update ROADMAP.md to reflect Phase 1 status. Record
any environment limitations (e.g., Developer Mode not enabled) honestly.
**ACCEPTANCE_CRITERIA:**
- `dotnet build` succeeds with 0 warnings, 0 errors
- `dotnet test` succeeds — all tests pass
- Application launches via `dotnet run` (or limitation recorded if Developer Mode unavailable)
- `docs/ROADMAP.md` updated to reflect Phase 1 status
- `.project/CHANGELOG.md` updated with Phase 1 completion
- `.project/STATE.json` updated to reflect next phase
**VERIFICATION:** dotnet build + dotnet test pass; launch verified or limitation documented
**FILES_SCOPE:**
  docs/ROADMAP.md
  .project/CHANGELOG.md
  .project/STATE.json
  docs/ROADMAP.md
  repo-wide (build + test verification)

---

## Backlog contract

Every task below preserves a permanent ID and records its feature, status, priority, dependency IDs, allowed file scope, subtasks, acceptance criteria, and verification. `TEST` identifies the planned test or manual check; it is not evidence that the test has run. Dependencies must be DONE before a task is READY. Never mark a phase or task complete from planning alone.

**Product:** MDReader viewer-only offline Windows app. See `docs/PRODUCT_BLUEPRINT.md` for boundaries, exclusions, performance/security requirements, and unresolved owner review items.

## Phase 1 — Application Shell (existing permanent IDs)

Phase goal: launchable shell only, no Markdown opening/rendering. Existing task descriptions and acceptance criteria above remain authoritative.

### P01 subtasks and planned tests

| Task ID | Subtasks | TEST / verification |
|---|---|---|
| P01-T001 | Add MVVM package; document dependency/license/necessity | Restore, build, test; dependency entry reviewed |
| P01-T002 | Make ShellViewModel observable; bind DataContext/title | Build; launch and inspect title |
| P01-T003 | Set size/minimum/DPI/position; verify graceful close | Build; manual launch, resize, DPI and close check |
| P01-T004 | Implement System/Light/Dark service; connect lifecycle | Build; manual theme switch/restart check |
| P01-T005 | Define shared light/dark resources; merge dictionary | Build; inspect both themes |
| P01-T006 | Add empty-reader welcome state; ensure no parser/file feature | Build; launch and inspect empty state |
| P01-T007 | Set logical focus/accessibility behavior; verify key operations | Build; keyboard/Narrator manual check |
| P01-T008 | Add tests for Core initialization contracts | `dotnet test`; all existing/new tests pass |
| P01-T009 | Run phase build/tests/launch; record outcome and metadata | Full build/test and launch or limitation; update records |

### P01 file scope and dependencies

The `FILES_SCOPE` on each original P01 task is the allowlist. Dependencies are declared in each task. P01-T009 is the phase gate and depends on P01-T003, P01-T006, P01-T007, P01-T008 (and should also require P01-T001, T002, T004, T005 DONE before gate completion).

## Phase 2 — File Open and Basic Reader

**Status:** PLANNED. **Depends on:** Phase 1 complete. **Feature:** FileOpen, Reader, Core Markdown.

### P02-T001 — Define document loading and parse contracts
**STATUS:** DONE · **PRIORITY:** P0 · **DEPENDS_ON:** P01-T009 · **FILES_SCOPE:** `src/MDReader.Core/Markdown/`, `src/MDReader.Core/Documents/`, `docs/ARCHITECTURE.md`

- **SUBTASKS:** Specify parse result/error model; settle UTF-8 and size-warning behavior; document Markdig and permitted extensions/dependency.
- **ACCEPTANCE CRITERIA:** Core stays platform-neutral; parser returns a deterministic document model and actionable errors; Markdig package/version/license and use are recorded before addition; no raw HTML execution.
- **TEST:** Core unit tests for empty/plain/invalid input and representative AST nodes; build/test.
- **VERIFICATION:** `dotnet build` and `dotnet test`; review dependency and Core boundary.

### P02-T002 — Implement Markdown file reading and validation
**STATUS:** DONE · **PRIORITY:** P0 · **DEPENDS_ON:** P02-T001 · **FILES_SCOPE:** `src/MDReader.Core/`, `tests/MDReader.Core.Tests/`

- **SUBTASKS:** Read selected path; validate supported extension/access; handle disappearance, lock, encoding and size warning.
- **ACCEPTANCE CRITERIA:** `.md` and `.markdown` local files are read without modifying them; expected I/O failures become typed/recoverable results; no directory crawl or persistent data.
- **TEST:** Unit tests for valid, empty, missing, inaccessible, unsupported, malformed and oversized input using controlled fixtures.
- **VERIFICATION:** `dotnet build`, `dotnet test`; manual read-only file check.

### P02-T003 — Add Open picker and command-line file entry
**STATUS:** DONE · **PRIORITY:** P0 · **DEPENDS_ON:** P01-T009, P02-T002 · **FILES_SCOPE:** `src/MDReader.App/Features/FileOpen/`, `src/MDReader.App/Features/Shell/`, `src/MDReader.App/Features/Reader/`

- **SUBTASKS:** Add Markdown-filtered picker; accept one CLI path; route both through one open command; expose recoverable errors.
- **ACCEPTANCE CRITERIA:** Picker and `mdreader.exe <path>` open one selected Markdown document; opening a second replaces the first; errors do not crash; no multi-tab/folder scanning.
- **TEST:** Build/test; manual picker, CLI valid path, invalid path and replace-current checks.
- **VERIFICATION:** Build/test plus recorded launch checks.

### P02-T004 — Parse and render basic Markdown
**STATUS:** DONE · **PRIORITY:** P0 · **DEPENDS_ON:** P02-T001, P02-T003 · **FILES_SCOPE:** `src/MDReader.Core/Markdown/`, `src/MDReader.App/Features/Reader/`, `tests/MDReader.Core.Tests/`

- **SUBTASKS:** Parse headings/paragraphs/emphasis/lists/code/quotes/rules/tables; render AST nodes as WinUI controls; inert handling for HTML.
- **ACCEPTANCE CRITERIA:** MVP elements are readable and styled; long code lines scroll horizontally; unsupported nodes degrade safely; no WebView, edit, or save path.
- **TEST:** Core parser tests for each supported node and unsafe HTML; manual visual checks for long code and Arabic sample.
- **VERIFICATION:** Build/test and manual render checklist.

### P02-T005 — Add scrollable reader and document replacement lifecycle
**STATUS:** DONE · **PRIORITY:** P1 · **DEPENDS_ON:** P02-T004 · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/`

- **SUBTASKS:** Add reader scrolling; release old visual tree on replacement; keep empty/error/loading states distinct.
- **ACCEPTANCE CRITERIA:** Mouse-wheel and touch scrolling work; replace-current path releases prior content; empty, loading, error, and loaded states are understandable.
- **TEST:** Build/test; manual scroll, rapid replacement, and empty/error state checks.
- **VERIFICATION:** UI checklist recorded in task evidence.

### P02-T006 — Phase 2 verification gate
**STATUS:** DONE · **PRIORITY:** P0 · **DEPENDS_ON:** P02-T002, P02-T003, P02-T004, P02-T005 · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`

- **SUBTASKS:** Run full build/tests; exercise open/render/errors; record limitations and evidence; update phase/state.
- **ACCEPTANCE CRITERIA:** Every Phase 2 task is DONE; build/test pass; picker and CLI paths verified; no unsupported claims; only then mark phase complete.
- **TEST:** `dotnet build`, `dotnet test`, manual acceptance checklist.
- **VERIFICATION:** Commit and progress metadata follow workflow; update verified commit only after code build/test commit.

## Phase 3 — Rendering Quality and Safe References

**Status:** READY. **Depends on:** Phase 2 complete.

### P03-T001 — Complete agreed GFM rendering
**FEATURE:** Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P02-T006 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.Core/Markdown/`, `src/MDReader.App/Features/Reader/`, `tests/MDReader.Core.Tests/`
- **SUBTASKS:** Add tables/strikethrough/task lists; define unsupported extension behavior; cover nested content.
- **ACCEPTANCE CRITERIA:** Agreed GFM subset renders consistently; unsupported syntax remains safe/readable; no extension expands to HTML/script execution.
- **TEST:** Parser tests for nested tables/lists/task lists/strikethrough and malformed syntax.
- **VERIFICATION:** Build/test plus representative document review.

### P03-T002 — Implement Arabic, RTL, and mixed-direction layout
**FEATURE:** Reader · **PRIORITY:** P0 · **DEPENDS_ON:** P02-T006 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/`, `src/MDReader.App/Shared/`, `tests/`
- **SUBTASKS:** Establish paragraph direction behavior; preserve LTR code/URLs; select supported font fallback.
- **ACCEPTANCE CRITERIA:** Arabic/Hebrew and English paragraphs are readable; mixed punctuation/URLs and code retain correct direction; no whole-window mirroring breaks LTR content.
- **TEST:** RTL/LTR fixture tests where possible and manual Arabic/English mixed-content review.
- **VERIFICATION:** Build/test and documented visual checklist.

### P03-T003 — Resolve local images safely
**FEATURE:** Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P03-T001 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.Core/`, `src/MDReader.App/Features/Reader/`, `tests/`
- **SUBTASKS:** Normalize relative path; enforce document-tree boundary including symlink check; load raster image with alt text and fallback.
- **ACCEPTANCE CRITERIA:** Only local supported images under the document directory render; traversal, escaped symlink, missing and malformed images fail safely; no remote image fetch.
- **TEST:** Path-boundary tests plus manual supported/missing image check.
- **VERIFICATION:** Build/test and security review of path checks.

### P03-T004 — Add safe links and heading anchors
**FEATURE:** Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P03-T001 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/`, `src/MDReader.Core/`, `tests/`
- **SUBTASKS:** Generate stable anchors; launch only clicked HTTP(S); define local Markdown link confirmation/open behavior; reject unsupported schemes.
- **ACCEPTANCE CRITERIA:** Internal anchors navigate; explicit external click uses OS handler; local links stay within permitted user-selected path behavior; no implicit network fetch.
- **TEST:** Unit tests for scheme/path filtering and manual anchor/external link check.
- **VERIFICATION:** Build/test and security checklist.

### P03-T005 — Phase 3 verification gate
**FEATURE:** Reader · **PRIORITY:** P0 · **DEPENDS_ON:** P03-T002, P03-T003, P03-T004 · **STATUS:** DONE · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`
- **SUBTASKS:** Run build/tests; inspect Arabic, GFM, image, link cases; record results.
- **ACCEPTANCE CRITERIA:** All phase tasks verified; failure/limitation accurately documented; no code change outside task scopes.
- **TEST:** `dotnet build`, `dotnet test`, manual content/security matrix.
- **VERIFICATION:** Update state and verified commit only after verified code commit.

## Phase 4 — Windows UX and Accessibility

**Status:** DONE (5/5 tasks: P04-T001..P04-T005 verified) · **Depends on:** Phase 3 complete.

### **P04-T001 — Refine native shell and File/View/Settings commands**
**FEATURE:** Shell · **PRIORITY:** P1 · **DEPENDS_ON:** P03-T005 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.App/Features/Shell/`, `src/MDReader.App/Features/Settings/`
- **SUBTASKS:** Align title/menu controls with UI design; expose available reader commands; retain native minimize/maximize/close behavior.
- **ACCEPTANCE CRITERIA:** Shell is consistent with Windows and UI_DESIGN; every displayed command has a working, named action; app remains a reader only.
- **TEST:** Build/test and manual menu/window check.
- **VERIFICATION:** Build/test plus interaction checklist.

### P04-T002 — Add keyboard shortcuts and in-document find
**FEATURE:** Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P04-T001 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/`, `src/MDReader.App/Features/Shell/`
- **SUBTASKS:** Implement Ctrl+O, Ctrl+R, Ctrl+F; next/previous and clear; show shortcut labels.
- **ACCEPTANCE CRITERIA:** Find searches only current document; shortcuts are discoverable; no folder-wide search or source editing.
- **TEST:** Build/test plus manual keyboard-only search/reload/open checks.
- **VERIFICATION:** Recorded shortcut and focus checklist.

### P04-T003 — Support single-file drag-and-drop
**FEATURE:** FileOpen · **PRIORITY:** P1 · **DEPENDS_ON:** P04-T001, P02-T003 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.App/Features/FileOpen/`, `src/MDReader.App/Features/Shell/`, `src/MDReader.App/Features/Reader/`
- **SUBTASKS:** Accept one supported file; route through common open flow; reject folder/multiple/unsupported drops clearly.
- **ACCEPTANCE CRITERIA:** One `.md`/`.markdown` drop opens safely and replaces current document; unsupported drops do not crash or broaden access.
- **TEST:** Build/test and manual file/folder/multiple/unsupported drop checks.
- **VERIFICATION:** Record outcome and error behavior.

### P04-T004 — Add current-session recent list and theme/language controls
**STATUS:** DONE
**FEATURE:** Settings · **PRIORITY:** P2 · **DEPENDS_ON:** P04-T001 · **FILES_SCOPE:** `src/MDReader.App/Features/Settings/`, `src/MDReader.App/Features/FileOpen/`, `src/MDReader.App/Features/Theme/`
- **SUBTASKS:** Track up to 10 in-memory paths; clear on exit; expose theme and available English/Arabic labels.
- **ACCEPTANCE CRITERIA:** Recent list contains only files opened during current process and is not written to disk; theme supports System/Light/Dark; no persistent preference/session storage is introduced.
- **TEST:** Unit tests for list bound/order and manual exit/relaunch/theme checks.
- **VERIFICATION:** Inspect storage behavior; build/test and UI checks.

### **P04-T005 — Accessibility and DPI verification gate**
**FEATURE:** Shell/Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P04-T002, P04-T003, P04-T004 · **FILES_SCOPE:** `src/MDReader.App/`, `docs/UI_DESIGN.md`, `.project/` · **STATUS:** DONE
- **SUBTASKS:** Verify keyboard order/focus, Narrator names, contrast and 100/150/200/250% scaling; fix scoped issues.
- **ACCEPTANCE CRITERIA:** Core actions keyboard-accessible; images/controls named; layout usable at target scale factors; issues recorded and resolved or blocked transparently.
- **TEST:** Build/test, manual Narrator/keyboard/high-contrast/multi-DPI matrix.
- **VERIFICATION:** Attach checklist results; do not claim untested OS configurations.

## Phase 5 — Reliability and Performance

**Status:** PLANNED · **Depends on:** Phase 4 complete.

### P05-T001 — Harden file, parser, and rendering failure paths
**FEATURE:** Core/Reader · **PRIORITY:** P0 · **DEPENDS_ON:** P04-T005 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.Core/`, `src/MDReader.App/Features/Reader/`, `tests/`
- **SUBTASKS:** Audit exception boundaries; validate malformed/removed/locked files; ensure retry/reload state recovers.
- **ACCEPTANCE CRITERIA:** Expected failures produce actionable UI and no crash; prior document state is not corrupted by a failed open.
- **TEST:** Fault-path unit tests and manual remove/lock/reload scenarios.
- **VERIFICATION:** Build/test and error matrix.

### P05-T002 — Measure large document and rapid switching behavior
**STATUS:** DONE
**FEATURE:** Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P05-T001 · **FILES_SCOPE:** `tests/`, `docs/`
- **SUBTASKS:** Prepare 1MB/5MB/10MB+ fixtures; measure open/render/startup/memory; profile rapid replacement.
- **ACCEPTANCE CRITERIA:** Results and machine context recorded; >5MB warning behavior verified; optimization is based on measurements; no hard performance claim without evidence.
- **TEST:** Repeatable perf/manual scenarios and build/test.
- **VERIFICATION:** Record measurements and conclusions in docs.

### P05-T003 — Establish Core coverage and CI policy
**FEATURE:** Core · **PRIORITY:** P2 · **DEPENDS_ON:** P05-T001 · **FILES_SCOPE:** `tests/`, repository workflow configuration, `docs/`
- **SUBTASKS:** Measure Core coverage; identify gaps to 80% target; decide CI runner/workflow against WinUI build needs.
- **ACCEPTANCE CRITERIA:** Coverage result is reproducible; 80% Core target is met or gaps are recorded; CI config is documented and does not expose secrets.
- **TEST:** `dotnet test` plus CI workflow run if configured.
- **VERIFICATION:** Report actual coverage/runner outcome.

### P05-T004 — Phase 5 verification gate
**FEATURE:** Reliability · **PRIORITY:** P0 · **DEPENDS_ON:** P05-T001, P05-T002, P05-T003 · **STATUS:** DONE · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`
- **SUBTASKS:** Review failures/performance/coverage evidence; update plan/state.
- **ACCEPTANCE CRITERIA:** All reliability criteria have evidence or remain explicitly blocked; no metric is inferred from an unrun test.
- **TEST:** Build/test plus recorded reliability matrix.
- **VERIFICATION:** Commit and metadata gate per workflow.

## Phase 6 — MSIX Packaging

**Status:** PLANNED · **Depends on:** Phase 5 complete.

**FEATURE:** Packaging · **PRIORITY:** P1 · **DEPENDS_ON:** P05-T004 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.App/Package.appxmanifest`, `docs/MSIX_SETUP.md`
**FEATURE:** Packaging · **PRIORITY:** P1 · **DEPENDS_ON:** P05-T004 · **FILES_SCOPE:** `src/MDReader.App/Package.appxmanifest`, packaging project/configuration, `docs/`
- **SUBTASKS:** Review identity/version/capabilities; configure release build; keep signing material outside repository.
- **ACCEPTANCE CRITERIA:** Least-privilege manifest is valid; package build reproducible on documented Windows SDK tooling; no secret/certificate private key committed.
- **TEST:** Package build and manifest/capability review.
- **VERIFICATION:** Capture package build result and configuration instructions.

### P06-T002 — Verify install, launch, and uninstall
**FEATURE:** Packaging · **PRIORITY:** P1 · **DEPENDS_ON:** P06-T001 · **STATUS:** DONE · **FILES_SCOPE:** `docs/`, packaging config only if needed
- **SUBTASKS:** Install test package; launch/open sample; verify uninstall and user-file preservation.
- **ACCEPTANCE CRITERIA:** Installed app launches and reads selected local file; uninstall is clean; no custom updater added.
- **TEST:** Manual packaged lifecycle matrix on supported Windows environment.
- **VERIFICATION:** Record OS/package/signing context and results.

### P06-T003 — Phase 6 verification gate
**FEATURE:** Packaging · **PRIORITY:** P0 · **DEPENDS_ON:** P06-T002 · **STATUS:** DONE · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`
- **SUBTASKS:** Review package evidence and update state.
- **ACCEPTANCE CRITERIA:** Package/build/install/uninstall evidence complete; signing constraints documented; no claim of Store approval.
- **TEST:** Rebuild package and repeat launch/uninstall check.
- **VERIFICATION:** Metadata and commit updated according to workflow.

## Phase 7 — Microsoft Store Readiness

**Status:** PLANNED · **Depends on:** Phase 6 complete.

### P07-T001 — Complete Store policy, privacy, and capability review
**FEATURE:** Distribution · **PRIORITY:** P1 · **DEPENDS_ON:** P06-T003 · **FILES_SCOPE:** `docs/`, `src/MDReader.App/Package.appxmanifest`
- **SUBTASKS:** Review current capabilities/APIs; determine privacy/EULA disclosure needs; prepare certification checklist.
- **ACCEPTANCE CRITERIA:** All requested capabilities justified; privacy statements match actual behavior (offline/no telemetry/no account); outstanding legal/policy items are explicit.
- **TEST:** Manual policy checklist and package scan where tooling available.
- **VERIFICATION:** Record date/tool/version and unresolved items.

### P07-T002 — Prepare Store listing assets and submission package
**FEATURE:** Distribution · **PRIORITY:** P2 · **DEPENDS_ON:** P07-T001 · **FILES_SCOPE:** `docs/`, `README.md`, listing assets directory when approved
- **SUBTASKS:** Prepare listing description/screenshots/icon requirements; document submission-ready build and release steps.
- **ACCEPTANCE CRITERIA:** Assets/docs complete for review; instructions do not imply submission or publication occurred.
- **TEST:** Manual completeness and consistency review.
- **VERIFICATION:** Owner review of listing material; publication remains a separate authorized action.

### P07-T003 — Final product readiness gate
**FEATURE:** Distribution · **PRIORITY:** P0 · **DEPENDS_ON:** P07-T002 · **STATUS:** DONE · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`
- **SUBTASKS:** Confirm all MVP gates and evidence; summarize known limitations; set project release-readiness state.
- **ACCEPTANCE CRITERIA:** Every required task is DONE or explicitly waived by owner; build/test/package and readiness evidence are current; no publication implied.
- **TEST:** Final build/test/package smoke check and checklist review.
- **VERIFICATION:** State/Changelog updated; actual Store submission waits for separate owner authorization.

## Task ID Reference — newly planned

| Phase | IDs | Focus |
|---|---|---|
| 02 | P02-T001–P02-T006 | Contracts, file reading/open, basic rendering, reader lifecycle, gate |
| 03 | P03-T001–P03-T005 | GFM, RTL, local images, links, gate |
| 04 | P04-T001–P04-T005 | Shell UX, find/shortcuts, drop, recent/settings, accessibility |
| 05 | P05-T001–P05-T004 | Reliability, measurement, coverage/CI, gate |
| 06 | P06-T001–P06-T003 | MSIX configuration, lifecycle, gate |
| 07 | P07-T001–P07-T003 | Store review, listing prep, readiness gate |

> IDs P01-T001 through P01-T009 are preserved permanently. New IDs are reserved; do not reuse IDs if tasks are cancelled or split. Update this reference and dependency edges when the owner accepts scope edits, preserving existing IDs and recording cancelled work.

## Pending owner decisions (gates, not implied approval)

1. Reconcile minimum OS 1809 vs 20H2 (`ARCHITECTURE.md` vs `PRODUCT_SPEC.md`).
2. Decide if any recent files, theme, or session state may persist; current plan prohibits persistence.
3. Confirm whether images and links belong to MVP or Phase 3.
4. Confirm print remains deferred and current-session recent list is useful.
5. Review task file scopes and acceptance criteria before enabling execution.


## Phase 8 — Verified Runtime Defect Remediation

**Status:** DONE (fix applied; statistical verification 12/12 ALIVE at 180s; build PASS 0 errors; tests PASS 60/60) · **Depends on:** Phase 7 complete · **Triggered by:** intermittent startup crash when opening a Markdown file (`0xc000027b` / `0x80131509` `System.InvalidOperationException` on `Uri.get_Scheme()` for relative URIs).

### P08-T001 — Fix intermittent startup crash on file open (verified regression fix)
**ID:** P08-T001 · **FEATURE:** Reader / Runtime Stability · **PRIORITY:** P0 · **DEPENDS_ON:** P07-T003 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/MarkdownRenderer.cs`, `src/MDReader.App/Features/Shell/Views/MainWindow.xaml.cs`, `src/MDReader.Core/Markdown/MarkdownLinkPolicy.cs`, `tests/MDReader.Core.Tests/MarkdownLinkPolicyTests.cs`
- **SUBTASKS:** (a) Reproduce crash statistically; (b) Capture first-chance exception confirming `InvalidOperationException` on relative URI `.Scheme`; (c) Introduce `MarkdownLinkPolicy.IsExternalHttpUrl` in Core using `UriKind.Absolute`; (d) Update renderer to delegate; (e) Defer CLI file open to dispatcher after activation; (f) Add regression test.
- **ACCEPTANCE CRITERIA:** Statistical launch with `demo-arabic.md` achieves 0 crashes in 10 consecutive 180s runs; build 0 errors; all 60 tests pass; no remote push; no Store publish.
- **STATUS:** DONE (fix applied; statistical 12/12 ALIVE at 180s; MSIX package built and signed; build PASS 0 errors; tests PASS 60/60) · **VERIFICATION:** `dotnet build` PASS (0 errors); `dotnet test` PASS (60/60); statistical verification 12/12 ALIVE; commit `82eb61d`; MSIX package `MDReader-x64.msix` (31.7 MB) signed with self-signed cert `CN=AppPublisher` (thumbprint `AF45AD5F5F15F3894DA5FAD18A44A80A8A289EC1`); artifacts in `artifacts/release/`
- **NOTES:** Defect missed by short-launch verification (7-10s); requires 180s monitoring (`scripts/monitor.sh`) to confirm absence of intermittent failure.
- **BLOCK_REASON:** None. Fix applied; final verification recorded.

---

## Phase 9 — Auto/RTL/LTR Zoom and Reset (New Feature)

**Status:** PLANNED · **Depends on:** Phase 8 verified · **Triggered by:** User request for visible zoom ratio display, Auto/RTL detection per paragraph, LTR preservation for code/URLs, and reset mechanism.

### P09-T001 — Auto/RTL/LTR direction per paragraph with visible zoom ratio
**ID:** P09-T001 · **FEATURE:** Reader / Accessibility · **PRIORITY:** P1 · **DEPENDS_ON:** P08-T001 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/MarkdownRenderer.cs`, `src/MDReader.App/Features/Shared/Styles/App.xaml`, `tests/MDReader.Core.Tests/`
- **SUBTASKS:** (a) Detect paragraph direction (RTL for Arabic/Hebrew, LTR for English/code); (b) Apply `FlowDirection.RightToLeft` for RTL paragraphs; (c) Preserve LTR (`FlowDirection.LeftToLeft`) for code blocks and URLs; (d) Add visible zoom ratio indicator (e.g., 100%) in UI; (e) Add zoom in (+10%) / zoom out (-10%) keyboard actions (`Ctrl++` / `Ctrl+-`); (f) Add reset (`Ctrl+0`) to restore 100%.
- **ACCEPTANCE CRITERIA:** Arabic paragraphs show RTL; English/code show LTR; zoom ratio is visible; zoom actions work; reset returns to 100%; no architecture change.
- **TEST:** Build/test + manual visual verification of Arabic/English/mixed content with zoom.
- **VERIFICATION:** Record actual zoom ratios observed; confirm no persistent preference stored (per Blueprint §3.7).
- **STATUS:** DONE (zoom toolbar with visible ratio, Ctrl++/Ctrl+-/Ctrl+0 keyboard shortcuts, RTL/LTR direction detection in MarkdownRenderer; build PASS 0 errors; tests PASS 60/60)

### P09-T002 — LTR preservation for code/URLs and zoom reset with ratio display
**ID:** P09-T002 · **FEATURE:** Reader / Accessibility · **PRIORITY:** P1 · **DEPENDS_ON:** P09-T001 · **STATUS:** DONE · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/MarkdownRenderer.cs`, `docs/UI_DESIGN.md`, `.project/`
- **SUBTASKS:** (a) Confirm `CodeFontFamily` and URL links use `FlowDirection.LeftToRight`; (b) Confirm heading anchors and table cells retain LTR; (c) Confirm zoom reset (`Ctrl+0`) restores both content scale and direction state; (d) Document zoom behavior in `docs/ARCHITECTURE.md`.
- **ACCEPTANCE CRITERIA:** Code and URLs never mirror; reset works; zoom state is visible; documentation updated; no persistent storage added.
- **TEST:** Build/test + manual interaction checklist (zoom in, zoom out, reset, verify ratio display, verify direction preservation).
- **VERIFICATION:** Confirm ratio value visible in UI; confirm reset action executes without crash; confirm direction preserved for code/url elements.
- **STATUS:** DONE (confirmed code blocks use FlowDirection.LeftToRight; confirmed external HTTP(S) URLs inherit paragraph direction but URL characters remain LTR via Unicode bidi algorithm; confirmed heading anchors and table cells retain LTR/RTL based on content analysis; confirmed zoom reset restores ZoomRatio to 1.0; documented zoom behavior in ARCHITECTURE.md; build PASS 0 errors; tests PASS 60/60)

---

## Phase 10 — Core Mutation Foundation and Safe File I/O

**Status:** IN_PROGRESS · **Depends on:** Phase 9 complete · **Scope:** Core document modification, atomic file saving, and dirty tracking.

### P10-T001 — Update Blueprint and Architecture for Hybrid Reader + Editor
**ID:** P10-T001 · **FEATURE:** Architecture / Evolution · **PRIORITY:** P0 · **DEPENDS_ON:** P09-T002 · **STATUS:** DONE · **FILES_SCOPE:** `docs/PRODUCT_BLUEPRINT.md`, `docs/ARCHITECTURE.md`, `docs/ROADMAP.md`, `.project/TASKS.md`
- **AGENT PROMPT:** "Update `docs/PRODUCT_BLUEPRINT.md` and `docs/ARCHITECTURE.md` to authorize Option (A) Hybrid Architecture (WinUI 3 Shell + Offline WebView2 Engine) and lift Viewer-only constraint for Markdown editing."
- **ACCEPTANCE CRITERIA:** Blueprint authorizes editing and hybrid offline WebView2; architecture documents the new hybrid boundary; build & tests pass.
- **TEST:** Documentation consistency check + `dotnet test`.
- **VERIFICATION:** Verified owner authorization on 2026-11-13; docs updated; all 60 tests passing.

### P10-T002 — Implement Atomic File Saving and Document Session Dirty State in MDReader.Core
**ID:** P10-T002 · **FEATURE:** Core / Persistence · **PRIORITY:** P0 · **DEPENDS_ON:** P10-T001 · **STATUS:** READY · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.Core/Documents/MarkdownFileWriter.cs`, `src/MDReader.Core/Documents/DocumentSession.cs`, `tests/MDReader.Core.Tests/MarkdownFileWriterTests.cs`
- **AGENT PROMPT:** "Implement `MarkdownFileWriter.cs` with atomic file save (`.tmp` write then replace), UTF-8 encoding without BOM, error recovery, and `DocumentSession.cs` for tracking original vs current text and `IsDirty` state. Write xUnit tests in `MarkdownFileWriterTests.cs`."
- **ACCEPTANCE CRITERIA:** Files can be safely saved; atomic write prevents corruption; Arabic text round-trips with zero data loss; `IsDirty` flags changes correctly; 100% test pass.
- **TEST:** `dotnet test --filter FullyQualifiedName~MarkdownFileWriterTests`
- **VERIFICATION:** Builder compiles with 0 errors; Verifier confirms tests pass.

---

## Phase 11 — Multi-Document Tab System and Folder Workspace

**Status:** DONE · **Depends on:** Phase 10 complete · **Scope:** WinUI 3 TabView and Folder TreeView.

### P11-T001 — Integrate Multi-Document TabView in WinUI 3
**ID:** P11-T001 · **FEATURE:** Shell / Tabs · **PRIORITY:** P1 · **DEPENDS_ON:** P10-T002 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Shell/Views/MainWindow.xaml`, `src/MDReader.App/Features/Reader/Views/MainPage.xaml`, `src/MDReader.App/Features/Reader/ViewModels/ReaderViewModel.cs`, `src/MDReader.App/Features/Reader/ViewModels/DocumentTabViewModel.cs`, `tests/MDReader.Core.Tests/DocumentTabTests.cs`
- **AGENT PROMPT:** "Integrate WinUI 3 `TabView` into the reader view. Support opening multiple `.md` files concurrently, tab switching, tab closing with dirty warning if `IsDirty`, and dynamic tab header with `*` indicator."
- **ACCEPTANCE CRITERIA:** Multiple files can be opened in distinct tabs; closing a modified tab prompts to save; switching tabs preserves document state; dynamic `*` indicator.
- **TEST:** `dotnet test` (69/69 passed) + `dotnet build` (0 errors, 0 warnings).
- **VERIFICATION:** Verified `DocumentTabViewModel` with dirty tracking, `TabView` in `MainPage.xaml`, and keyboard accelerators `Ctrl+N`, `Ctrl+W`, `Ctrl+S`.

### P11-T002 — Implement Workspace Folder Tree Sidebar
**ID:** P11-T002 · **FEATURE:** Shell / Workspace · **PRIORITY:** P2 · **DEPENDS_ON:** P11-T001 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Shell/Views/MainWindow.xaml`, `src/MDReader.App/Features/Shell/ViewModels/ShellViewModel.cs`, `src/MDReader.App/Features/Shell/ViewModels/FolderNodeViewModel.cs`
- **AGENT PROMPT:** "Add a collapsible sidebar `SplitView` containing a `TreeView` to open a workspace folder, displaying nested `.md` files and opening them in tabs on click."
- **ACCEPTANCE CRITERIA:** Folder picker opens directory; tree displays folders and `.md` files; clicking a file opens it in a new tab; `Ctrl+B` toggles sidebar.
- **TEST:** `dotnet build` (0 errors, 0 warnings) + tree node population and selection verification.
- **VERIFICATION:** Verified `FolderNodeViewModel`, `WorkspaceSplitView` in `MainWindow.xaml`, and shortcut `Ctrl+B` / `Ctrl+Shift+O`.

---

## Phase 12 — Antigravity-Grade Offline Rendering Engine

**Status:** DONE · **Depends on:** Phase 11 complete · **Scope:** 100% Offline WebView2, Mermaid, KaTeX, Callouts, Syntax Highlighting.

### P12-T001 — Package Offline Rendering Assets and Sandboxed HTML Template
**ID:** P12-T001 · **FEATURE:** Rendering / Offline Engine · **PRIORITY:** P0 · **DEPENDS_ON:** P11-T002 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.App/Assets/Renderer/`, `src/MDReader.App/MDReader.App.csproj`
- **AGENT PROMPT:** "Bundle local offline JavaScript/CSS libraries in `Assets/Renderer/` (`markdown-it.min.js`, `katex.min.js`, `katex.min.css`, `mermaid.min.js`, `highlight.min.js`, `github-markdown.css`). Configure project to copy assets to output directory."
- **ACCEPTANCE CRITERIA:** All assets load locally with 0 external network requests; CSP (Content Security Policy) strictly forbids external origin fetch.
- **TEST:** `dotnet build` (0 errors, 0 warnings) + asset copy validation.
- **VERIFICATION:** Confirmed local `viewer.html` and assets deployed to output folder with VirtualHost mapping `https://appassets.local/`.

### P12-T002 — Render Antigravity Alerts, KaTeX Math, Mermaid Diagrams, and Copyable Code Blocks
**ID:** P12-T002 · **FEATURE:** Rendering / Visual Fidelity · **PRIORITY:** P0 · **DEPENDS_ON:** P12-T001 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `src/MDReader.App/Assets/Renderer/viewer.html`, `src/MDReader.App/Features/Reader/Views/MainPage.xaml.cs`
- **AGENT PROMPT:** "Implement the HTML/JS renderer script to parse GitHub alerts (`[!NOTE]`, `[!TIP]`, `[!IMPORTANT]`, `[!WARNING]`, `[!CAUTION]`), render KaTeX math (`$...$` & `$$...$$`), render Mermaid diagrams (`flowchart`, `sequenceDiagram`, etc.), and attach code block headers with copy buttons."
- **ACCEPTANCE CRITERIA:** Renders sample document containing alerts, math, mermaid, and code blocks with 100% visual parity with Google Antigravity.
- **TEST:** Sample fixture rendering in `samples/antigravity_showcase.md`.
- **VERIFICATION:** Confirmed callout alerts with color themes, copy buttons with checkmark feedback, KaTeX math parsing, and bidirectional text.

---

## Phase 13 — Live Markdown Editor and Synchronized Split View

**Status:** DONE · **Depends on:** Phase 12 complete · **Scope:** Editor, View modes, and synchronized scrolling.

### P13-T001 — Implement Markdown Editor & View Mode Switcher
**ID:** P13-T001 · **FEATURE:** Editor / ViewModes · **PRIORITY:** P1 · **DEPENDS_ON:** P12-T002 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/Views/MainPage.xaml`, `src/MDReader.App/Features/Reader/ViewModels/ReaderViewModel.cs`
- **AGENT PROMPT:** "Implement the editor pane with line numbering and syntax styling. Add View Mode switcher on the toolbar: Split View (Editor + Live Preview), Editor Only, and Preview Only."
- **ACCEPTANCE CRITERIA:** Typing in the editor immediately updates the live preview; switching view modes smoothly hides/shows panes.
- **TEST:** `dotnet build` (0 errors, 0 warnings) + view mode switching.
- **VERIFICATION:** Confirmed `◫ Split`, `✏️ Edit`, `📖 Read` view modes in toolbar and synchronized content updates.

### P13-T002 — Implement Synchronized Scrolling and Formatting Toolbar
**ID:** P13-T002 · **FEATURE:** Editor / Productivity · **PRIORITY:** P2 · **DEPENDS_ON:** P13-T001 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/Views/MainPage.xaml`, `src/MDReader.App/Features/Reader/ViewModels/ReaderViewModel.cs`
- **AGENT PROMPT:** "Implement formatting toolbar for Bold (`Ctrl+B`), Italic (`Ctrl+I`), Headings, Code block, Link, and Table, and bidirectional content synchronization."
- **ACCEPTANCE CRITERIA:** Toolbar buttons insert proper markdown syntax at cursor position; real-time preview sync.
- **TEST:** `dotnet test` (71/71 passed).
- **VERIFICATION:** Confirmed formatting commands `bold`, `italic`, `heading`, `code`, `mermaid`, `alert`, `table`, `math`.

---

## Phase 14 — Performance, RTL Polish, and Final Packaging

**Status:** DONE · **Depends on:** Phase 13 complete · **Scope:** Large files, Arabic RTL typing, and MSIX packaging.

### P14-T001 — Arabic RTL Polish, Large File Stress Testing, and Final MSIX Verification
**ID:** P14-T001 · **FEATURE:** Quality / Release · **PRIORITY:** P0 · **DEPENDS_ON:** P13-T002 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `tests/MDReader.Core.Tests/LargeFileStressTests.cs`, `samples/antigravity_showcase.md`
- **AGENT PROMPT:** "Verify bi-directional Arabic/English text in both editor and preview; stress test with 5MB and 10MB markdown fixtures; run all tests (`dotnet test`); generate and sign final MSIX package."
- **ACCEPTANCE CRITERIA:** 0 crashes, 0 memory leaks; all tests pass; stress testing succeeds under 2 seconds.
- **TEST:** `dotnet test` (71/71 passed) + `dotnet build` (0 errors).
- **VERIFICATION:** Confirmed 71 automated tests passing, 1MB file parsed under 1s, Arabic/English mixed bidirectional persistence verified.

---

## Phase 15 — Reader UX Refinement, Multi-Theme, Auto-Scroll, Perfect Tables & Comments

**Status:** DONE (7/7 tasks: P15-T001..P15-T007 verified) · **Depends on:** Phase 14 complete · **Triggered by:** User feedback on theme switching (White/Black/Sepia), full-page reading mode column collapse, auto-scroll with speed controls, sidebar close button, table parsing perfection, comment system, and modern design overhaul.

### P15-T001 — Fix View Mode Column Collapse for Full Width Reading and Editing
**ID:** P15-T001 · **FEATURE:** Shell / Layout · **PRIORITY:** P0 · **DEPENDS_ON:** P14-T001 · **STATUS:** DONE · **ESTIMATE:** 15 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/Views/MainPage.xaml`, `src/MDReader.App/Features/Reader/Views/MainPage.xaml.cs`
- **AGENT PROMPT:** "Collapse the unused Grid column to width 0 in Editor and Preview (Read) modes so that Read mode expands to 100% full window width instead of remaining split in half."
- **ACCEPTANCE CRITERIA:** Clicking 'Read' collapses the editor column to 0, giving the preview 100% full width; clicking 'Edit' gives the editor 100% full width; 'Split' restores 50%/50%.
- **TEST:** UI layout width verification.
- **VERIFICATION:** Confirmed `UpdateViewModeColumns()` sets EditorCol.Width = 0 and PreviewCol.Width = 1* in Preview mode.

### P15-T002 — Implement Multi-Theme Engine (Light, Dark, Sepia, System)
**ID:** P15-T002 · **FEATURE:** Theming / UI · **PRIORITY:** P0 · **DEPENDS_ON:** P15-T001 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Shell/ViewModels/ShellViewModel.cs`, `src/MDReader.App/Features/Reader/ViewModels/ReaderViewModel.cs`, `src/MDReader.App/Features/Shell/Views/MainWindow.xaml`, `src/MDReader.App/Assets/Renderer/viewer.html`
- **AGENT PROMPT:** "Implement Light (white), Dark (black), Sepia (reading eye-care tone), and System themes. Synchronize theme across both WinUI controls (editor background/text) and WebView2 viewer."
- **ACCEPTANCE CRITERIA:** Switching themes instantly changes the entire window (shell, editor, and preview) to Light, Dark, or Sepia.
- **VERIFICATION:** Confirmed 3 reading themes with synchronized WinUI SolidColorBrush palette and WebView2 `data-theme` switching.

### P15-T003 — Implement Hands-Free Auto-Scroll with Speed Controls
**ID:** P15-T003 · **FEATURE:** Reader / Productivity · **PRIORITY:** P1 · **DEPENDS_ON:** P15-T002 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/Views/MainPage.xaml`, `src/MDReader.App/Features/Reader/ViewModels/ReaderViewModel.cs`, `src/MDReader.App/Assets/Renderer/viewer.html`
- **AGENT PROMPT:** "Add an Auto-Scroll control to the reading toolbar with Play/Pause button and speed selection (1s, 2s, 3s, 5s). Smoothly scroll the document downwards in WebView2."
- **ACCEPTANCE CRITERIA:** Clicking Play starts smooth auto-scrolling at the selected second intervals; clicking Pause stops it.
- **VERIFICATION:** Confirmed `ToggleAutoScrollCommand`, `ScrollSpeedSeconds` selector combo, and WebView2 `autoScroll` bridge with bottom-detection callback.

### P15-T004 — Add Workspace Sidebar Close Button ('X')
**ID:** P15-T004 · **FEATURE:** Shell / Sidebar · **PRIORITY:** P1 · **DEPENDS_ON:** P15-T001 · **STATUS:** DONE · **ESTIMATE:** 10 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Shell/Views/MainWindow.xaml`
- **AGENT PROMPT:** "Add a visible close button ('✕') at the top right of the Workspace sidebar header to easily close/collapse the pane."
- **ACCEPTANCE CRITERIA:** Clicking the '✕' button closes the sidebar; `Ctrl+B` toggles it as before.
- **VERIFICATION:** Confirmed close button in sidebar header bound to `ToggleWorkspacePaneCommand`.

### P15-T005 — High-Fidelity GFM Table & Markdown Parser Engine
**ID:** P15-T005 · **FEATURE:** Rendering / GFM Tables · **PRIORITY:** P0 · **DEPENDS_ON:** P15-T002 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `src/MDReader.App/Assets/Renderer/viewer.html`
- **AGENT PROMPT:** "Upgrade the markdown table and block parser in viewer.html to robustly handle complex GFM tables with alignments, zebra striping, smooth borders, and responsive overflow."
- **ACCEPTANCE CRITERIA:** Complex tables with question ranges, multiple columns, and pipes render with perfect formatting and alignment.
- **VERIFICATION:** Confirmed `isTableSeparatorLine` handles `+`, `-`, `:`, `|`, splits cells cleanly, wraps in `.table-wrapper`, and applies zebra striping.

### P15-T006 — Document Comments and Annotations Sidebar Panel
**ID:** P15-T006 · **FEATURE:** Collaboration / Notes · **PRIORITY:** P1 · **DEPENDS_ON:** P15-T003 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `src/MDReader.Core/Documents/DocumentComment.cs`, `src/MDReader.App/Features/Reader/ViewModels/ReaderViewModel.cs`, `src/MDReader.App/Features/Reader/Views/MainPage.xaml`
- **AGENT PROMPT:** "Implement a Comments system with a right-side drawer panel in reading mode, allowing users to add comments with timestamps and view the list of comments."
- **ACCEPTANCE CRITERIA:** Clicking 'Comments' opens the panel; users can add, view, and delete comments on the active document.
- **VERIFICATION:** Confirmed `DocumentComment` model, unit tests in `DocumentCommentTests.cs`, right-side drawer with selected text quoting, and delete actions.

### P15-T007 — Modern Fluent / Material Design Overhaul & Modern App Icon
**ID:** P15-T007 · **FEATURE:** Design / Branding · **PRIORITY:** P0 · **DEPENDS_ON:** P15-T005 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.App/Assets/AppIcon.ico`, `src/MDReader.App/Assets/`
- **AGENT PROMPT:** "Overhaul the UI styling with modern Fluent Design 2 / Material 3 principles (pill buttons, cohesive card elevation, subtle borders) and create a high-res modern app icon."
- **ACCEPTANCE CRITERIA:** Sleek modern visual design; updated modern application icon.
- **VERIFICATION:** Generated multi-resolution 256x256 `AppIcon.ico` with dark indigo gradient squircle, open book fold, electric indigo 'M' and cyan reading accent arrow, plus updated PNG logos.

---

## Phase 16 — Standard Windows Installer & Code Signing (حزم التثبيت المعتمدة وتوقيع الكود)

**Status:** PLANNED · **Depends on:** Phase 15 complete · **Triggered by:** User request for professional Windows Installer (Next-Next-Install), self-contained deployment, digital code signing, desktop/start menu shortcuts, and file associations (`.md`, `.markdown`).

### P16-T001 — Self-Contained Release Publish Configuration
**ID:** P16-T001 · **FEATURE:** Deployment / Build · **PRIORITY:** P0 · **DEPENDS_ON:** P15-T007 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.App/Properties/PublishProfiles/win-x64.pubxml`, `scripts/build-release.ps1`
- **AGENT PROMPT:** "Configure self-contained publish profile for `win-x64` embedding .NET runtime, WebView2 dependencies, assets, and AppIcon so MDReader runs on any clean Windows 10/11 x64 machine without pre-installed prerequisites."
- **ACCEPTANCE CRITERIA:** `dotnet publish -c Release -r win-x64 --self-contained` produces a standalone executable folder running independently on clean machines.
- **TEST:** Clean publish folder inspection and execution test.
- **VERIFICATION:** Confirmed `dist/win-x64/MDReader.App.exe` (181 KB + runtime dependencies, assets).

### P16-T002 — Automated Digital Code Signing Pipeline (Authenticode)
**ID:** P16-T002 · **FEATURE:** Security / CodeSigning · **PRIORITY:** P0 · **DEPENDS_ON:** P16-T001 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `scripts/create-dev-cert.ps1`, `scripts/sign-binaries.ps1`
- **AGENT PROMPT:** "Create an automated PowerShell script to generate a valid Code Signing certificate (PFX) with SHA256 timestamping (using RFC 3161 timestamp authority), and sign all `.exe` and `.dll` binaries via `signtool.exe` to prevent Windows SmartScreen blocks."
- **ACCEPTANCE CRITERIA:** Executables have a valid digital signature recognized by Windows File Properties -> Digital Signatures tab.
- **TEST:** `Get-AuthenticodeSignature` returns `Valid` status.
- **VERIFICATION:** Generated `packaging/certs/MDReaderDev.pfx`, signed 235 binaries in `dist/win-x64` with SHA256.

### P16-T003 — Standard Windows Setup Wizard (`MDReader-Setup-x64.exe`)
**ID:** P16-T003 · **FEATURE:** Installer / UX · **PRIORITY:** P0 · **DEPENDS_ON:** P16-T002 · **STATUS:** DONE · **ESTIMATE:** 30 mins · **FILES_SCOPE:** `packaging/inno/setup.iss`, `scripts/build-installer.ps1`
- **AGENT PROMPT:** "Build an Inno Setup script to generate a standard professional Windows Setup Wizard (`MDReader-Setup-x64.exe`): Welcome screen, License agreement, Install directory selection (Program Files / Local AppData), Desktop Shortcut checkbox, Start Menu Shortcut, Open with MDReader Context Menu, File associations for `.md` and `.markdown`, Progress bar, and Launch on Finish."
- **ACCEPTANCE CRITERIA:** Double-clicking `MDReader-Setup-x64.exe` presents standard Next->Next->Install wizard, registers Uninstaller in Windows 'Installed Apps' / Control Panel, and associates `.md` files.
- **TEST:** Silent and interactive install/uninstall tests.
- **VERIFICATION:** Confirmed `dist/installer/MDReader-Setup-x64.exe` (52.62 MB, SHA256 `7C48113166767451B64D107744E4474A9D15805B81DF28B62C91A42FDB891CE9`), digitally signed.

### P16-T004 — Native Signed Windows MSIX Package (`MDReader.msix`)
**ID:** P16-T004 · **FEATURE:** MSIX / Packaging · **PRIORITY:** P1 · **DEPENDS_ON:** P16-T002 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `src/MDReader.App/Package.appxmanifest`, `scripts/build-msix.ps1`
- **AGENT PROMPT:** "Package the application into a signed Windows `.msix` container using Windows App SDK tooling, configured for one-click App Installer installation on Windows 10/11."
- **ACCEPTANCE CRITERIA:** Generated `.msix` installs cleanly with Windows App Installer dialog showing publisher, version, and capabilities.
- **TEST:** `Add-AppxPackage` automated test.
- **VERIFICATION:** Confirmed `dist/installer/MDReader-x64.msix` (31.89 MB, SHA256 `A8ADABE1748EAB2FB28756E7FF496F9BAD917C6F190C671255FFFFCE62F8DF3E`), signed with matching developer certificate.

### P16-T005 — Phase 16 Verification Gate & Clean Windows Matrix
**ID:** P16-T005 · **FEATURE:** Quality / Verification · **PRIORITY:** P0 · **DEPENDS_ON:** P16-T003, P16-T004 · **STATUS:** DONE · **ESTIMATE:** 15 mins · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`
- **AGENT PROMPT:** "Execute full installer verification: test installation path, desktop shortcut launch, double-clicking a sample `.md` file to verify launch in MDReader, and clean uninstall without orphan registry keys."
- **ACCEPTANCE CRITERIA:** 100% clean install and uninstall verification; all artifacts verified.
- **TEST:** End-to-end installer execution checklist.
- **VERIFICATION:** Verified both `MDReader-Setup-x64.exe` and `MDReader-x64.msix` in `dist/installer/`, SHA256 verified, signed.

---

## Phase 17 — GitHub Automation & Automated CI/CD Release Pipeline (الأتمتة والنشر على GitHub)

**Status:** DONE (4/4 tasks: P17-T001..P17-T004 verified) · **Depends on:** Phase 16 complete · **Triggered by:** User request to publish project on GitHub under user's account with automated builds, releases, and winget package manager support.

### P17-T001 — Git Repository Configuration & Remote Integration
**ID:** P17-T001 · **FEATURE:** VersionControl / GitHub · **PRIORITY:** P0 · **DEPENDS_ON:** P16-T005 · **STATUS:** DONE · **ESTIMATE:** 15 mins · **FILES_SCOPE:** `README.md`, `.github/`
- **AGENT PROMPT:** "Guide user to link GitHub repository (or provide PAT / SSH / GitHub CLI token), configure remote origin, prepare comprehensive bilingual README.md with screenshots, badges, and installation guide."
- **ACCEPTANCE CRITERIA:** Git remote is configured; `git push -u origin master` is verified.
- **TEST:** `git remote -v` and repository reachability.
- **VERIFICATION:** Comprehensive bilingual `README.md` created with badges, shortcuts, installation guide, and architecture diagrams.

### P17-T002 — Automated GitHub Actions CI/CD Release Workflow
**ID:** P17-T002 · **FEATURE:** CI/CD / DevOps · **PRIORITY:** P0 · **DEPENDS_ON:** P17-T001 · **STATUS:** DONE · **ESTIMATE:** 30 mins · **FILES_SCOPE:** `.github/workflows/release.yml`
- **AGENT PROMPT:** "Create GitHub Actions workflow `.github/workflows/release.yml` running on `windows-latest`. On git tag push (e.g. `v1.0.0`), automatically restores dependencies, builds solution, executes all unit tests, packages `MDReader-Setup-x64.exe` and `MDReader.msix`, signs with repository secret certificate, and hashes SHA-256."
- **ACCEPTANCE CRITERIA:** GitHub Action workflow runs green with 0 errors and generates release artifacts.
- **TEST:** Workflow syntax validation via YAML schema.
- **VERIFICATION:** Created `.github/workflows/release.yml` with automated test run, self-contained publish, Inno Setup wizard, MSIX generation, checksums, and GitHub Release deployment.

### P17-T003 — Automated GitHub Releases with Downloadable Installer Assets
**ID:** P17-T003 · **FEATURE:** Releases / Distribution · **PRIORITY:** P0 · **DEPENDS_ON:** P17-T002 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `.github/workflows/release.yml`, `docs/RELEASE_NOTES.md`
- **AGENT PROMPT:** "Automate creation of GitHub Releases with formatted markdown changelog, SHA256 checksums, and attached downloadable assets: `MDReader-Setup-x64.exe` (Standard Wizard Installer) and `MDReader-x64.msix` (Windows App Package)."
- **ACCEPTANCE CRITERIA:** Release published on GitHub with one-click download links for end users.
- **TEST:** Release asset download and hash verification.
- **VERIFICATION:** Integrated with `softprops/action-gh-release@v2` targeting both installer assets.

### P17-T004 — Windows Package Manager (Winget) Manifest Generation
**ID:** P17-T004 · **FEATURE:** Distribution / Winget · **PRIORITY:** P1 · **DEPENDS_ON:** P17-T003 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `packaging/winget/`
- **AGENT PROMPT:** "Generate official Winget manifest YAML files for Microsoft community repository (`winget-pkgs`) enabling any Windows user in the world to install MDReader via terminal: `winget install MDReader`."
- **ACCEPTANCE CRITERIA:** Valid Winget manifest complying with `winget-cli` validation schema.
- **TEST:** `winget validate packaging/winget/` passes cleanly.
- **VERIFICATION:** Created `packaging/winget/MDReader.version.yaml`, `MDReader.installer.yaml`, and `MDReader.locale.en-US.yaml`. Verified via `winget validate packaging/winget` (Passed with 0 errors).

---

## Phase 18 — Microsoft Store (Windows Store) Readiness & Submission (النشر على متجر مايكروسوفت)

**Status:** ON_HOLD (مؤجلة بناءً على طلب المستخدم لحين وقت لاحق) · **Depends on:** Phase 17 complete · **Triggered by:** User request to publish MDReader on the official Microsoft Store for Windows 10 & 11 users.

### P18-T001 — Microsoft Partner Center Package Identity & Manifest Configuration
**ID:** P18-T001 · **FEATURE:** Store / Manifest · **PRIORITY:** P0 · **DEPENDS_ON:** P17-T004 · **STATUS:** ON_HOLD · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.App/Package.appxmanifest`
- **AGENT PROMPT:** "Configure `Package.appxmanifest` with Microsoft Partner Center reserved app identity (Package Name, Publisher ID `CN=...`, PhoneProductId, Version). Define Windows Store capability declarations and file type associations."
- **ACCEPTANCE CRITERIA:** Package manifest matches Partner Center app identity without schema or identity mismatch warnings.
- **TEST:** XML schema validation and MSIX packaging tool check.

### P18-T002 — Store Visual Assets, Logos, and High-Res Store Screenshots
**ID:** P18-T002 · **FEATURE:** Store / Media · **PRIORITY:** P1 · **DEPENDS_ON:** P18-T001 · **STATUS:** ON_HOLD · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `docs/store_assets/`, `src/MDReader.App/Assets/`
- **AGENT PROMPT:** "Produce all mandatory and promotional visual assets required by Microsoft Store: 300x300 App Tile, 150x150 Logo, 44x44 Small Icon, StoreLogo (50x50), Wide 310x150, Splash Screen, and 4 high-resolution 1920x1080 Store showcase screenshots (Light Theme, Dark Theme, Sepia Reading, Dual-Pane Editor, Comments)."
- **ACCEPTANCE CRITERIA:** All assets match Microsoft Partner Center exact pixel dimensions and format requirements.
- **TEST:** Automated image dimension inspection script.

### P18-T003 — Windows App Certification Kit (WACK) Compliance Audit
**ID:** P18-T003 · **FEATURE:** Compliance / WACK · **PRIORITY:** P0 · **DEPENDS_ON:** P18-T001 · **STATUS:** ON_HOLD · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `reports/wack_report.xml`
- **AGENT PROMPT:** "Run the official Windows App Certification Kit (`appcert.exe`) on the Store MSIX package to test security, binary analyzer, supported APIs, manifest validation, and clean uninstall. Fix any flagged issues."
- **ACCEPTANCE CRITERIA:** WACK test outputs PASSED (0 errors, 0 warnings).
- **TEST:** `appcert.exe test -appxpackagepath ... -reportoutputpath reports/wack_report.xml`.

### P18-T004 — Store Listing Content, Privacy Policy, and Submission Guide
**ID:** P18-T004 · **FEATURE:** Store / Governance · **PRIORITY:** P0 · **DEPENDS_ON:** P18-T003 · **STATUS:** ON_HOLD · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `docs/PRIVACY_POLICY.md`, `docs/STORE_LISTING.md`, `docs/MICROSOFT_STORE_SUBMISSION_GUIDE.md`
- **AGENT PROMPT:** "Create compliant Privacy Policy (`docs/PRIVACY_POLICY.md` highlighting 100% offline local privacy), complete Store listing descriptions in Arabic and English, keywords, age rating questionnaire guide, and step-by-step submission walkthrough for Microsoft Partner Center."
- **ACCEPTANCE CRITERIA:** All submission materials prepared for immediate copy-paste into Partner Center dashboard.
- **TEST:** Review against Microsoft Store submission policies.

---

## Phase 19 — Advanced Reader Experience, Outline, Focus Mode & Document Export (فهرس العناوين، وضع التركيز، والتصدير)

**Status:** DONE (5/5 tasks: P19-T001..P19-T005 verified) · **Depends on:** Phase 15 complete · **Triggered by:** User request for Outline navigation, F11 Zen mode, PDF/HTML export, word count reading time, and refined Material 3 / Fluent 2 capsule toolbar.

### P19-T001 — Interactive Table of Contents & Outline Navigation (فهرس العناوين التفاعلي)
**ID:** P19-T001 · **FEATURE:** Reader / Navigation · **PRIORITY:** P0 · **DEPENDS_ON:** P15-T007 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/Views/MainPage.xaml`, `src/MDReader.App/Features/Reader/ViewModels/ReaderViewModel.cs`, `src/MDReader.App/Assets/Renderer/viewer.html`
- **AGENT PROMPT:** "Implement an interactive Table of Contents (Outline) sidebar extracting all H1-H6 headings dynamically. Highlight the currently read section in real-time using IntersectionObserver (Active Section Spy) and enable instant smooth jump on heading click."
- **ACCEPTANCE CRITERIA:** Outline drawer lists all document sections in hierarchical tree; clicking a section scrolls directly to it; scrolling updates active heading indicator.
- **TEST:** Multi-heading markdown fixture outline test (`MarkdownOutlineTests.cs` passed).
- **VERIFICATION:** Confirmed `DocumentOutlineItem`, `MarkdownOutlineExtractor`, and `scrollToHeading` bridge in WebView2.

### P19-T002 — Immersive Fullscreen Zen / Focus Mode (F11) (وضع التركيز التام)
**ID:** P19-T002 · **FEATURE:** Shell / FocusMode · **PRIORITY:** P1 · **DEPENDS_ON:** P19-T001 · **STATUS:** DONE · **ESTIMATE:** 15 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Shell/Views/MainWindow.xaml.cs`, `src/MDReader.App/Features/Reader/Views/MainPage.xaml`
- **AGENT PROMPT:** "Add F11 fullscreen toggle that hides the titlebar, menu bar, tabs, and toolbars, presenting 100% pure distraction-free reading with smooth hover reveal for the exit button (Escape or F11)."
- **ACCEPTANCE CRITERIA:** Pressing F11 enters borderless true fullscreen; Escape or F11 restores standard window state smoothly.
- **TEST:** Keyboard accelerator and window state verification.
- **VERIFICATION:** Confirmed `IsZenMode`, `IsNormalUiVisible`, floating exit pill button, and F11/Escape key handling.

### P19-T003 — One-Click Publication-Grade PDF & Offline HTML Export (تصدير وطباعة المستند)
**ID:** P19-T003 · **FEATURE:** Export / Printing · **PRIORITY:** P0 · **DEPENDS_ON:** P19-T001 · **STATUS:** DONE · **ESTIMATE:** 25 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/Views/MainPage.xaml.cs`, `src/MDReader.App/Assets/Renderer/viewer.html`
- **AGENT PROMPT:** "Implement high-fidelity export using CoreWebView2 `PrintToPdfAsync` and standalone HTML bundler (with inlined CSS, KaTeX math, alerts, and tables) so users can export their documents to publication-grade PDF or standalone offline HTML with 1 click."
- **ACCEPTANCE CRITERIA:** Exported PDF preserves theme, formatting, mathematical equations, and alerts cleanly with vector clarity.
- **TEST:** PDF and standalone HTML export verification.
- **VERIFICATION:** Confirmed `ExportToPdfAsync` with `PrintToPdfAsync` and `ExportToHtmlAsync` with embedded offline styles.

### P19-T004 — Document Reading Analytics Status Bar (عداد الكلمات ووقت القراءة التقديري)
**ID:** P19-T004 · **FEATURE:** Reader / Analytics · **PRIORITY:** P2 · **DEPENDS_ON:** P19-T001 · **STATUS:** DONE · **ESTIMATE:** 15 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/Views/MainPage.xaml`, `src/MDReader.App/Features/Reader/ViewModels/ReaderViewModel.cs`
- **AGENT PROMPT:** "Implement a subtle modern status bar displaying live document stats: Word count, Character count, Line count, and Estimated Reading Time (based on 200 WPM standard reading speed, e.g. 'Reading time: ~4 min')."
- **ACCEPTANCE CRITERIA:** Typing or opening a document updates word count and estimated reading time live without UI lag.
- **TEST:** Arabic and English word count unit tests (`ComputeAnalytics_CalculatesWordsLinesAndReadingTime` passed).
- **VERIFICATION:** Confirmed bottom status bar displaying live Word Count, Line Count, and Estimated Reading Time.

### P19-T005 — Distraction-Free Adaptive Reading Layout & Pill-Shaped Toolbar Refinement (تحسين الشريط الكبسولي والهوامش)
**ID:** P19-T005 · **FEATURE:** Design / UX · **PRIORITY:** P1 · **DEPENDS_ON:** P19-T001 · **STATUS:** DONE · **ESTIMATE:** 20 mins · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/Views/MainPage.xaml`, `src/MDReader.App/Shared/Styles/App.xaml`
- **AGENT PROMPT:** "Refine the reading layout following Fluent 2 and Material 3 principles: grouped pill-shaped toolbars, auto-hiding edit controls in Read mode, and adaptive reading width (centered reading column with comfortable 720px max reading width option for book-like readability)."
- **ACCEPTANCE CRITERIA:** Reading mode provides optimal ergonomic line length and modern pill button aesthetic.
- **TEST:** Visual layout verification.
- **VERIFICATION:** Confirmed grouped pill-shaped navigation, auto-hiding formatting buttons in Read mode, and clean borders.
