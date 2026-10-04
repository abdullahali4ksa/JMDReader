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
7 / 9 tasks completed

### Phase 2:
PLANNED

### Phase 3:
PLANNED

### Phase 4:
PLANNED

### Phase 5:
PLANNED

### Phase 6:
PLANNED

### Phase 7:
PLANNED

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
**STATUS:** READY
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
**STATUS:** READY
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
**STATUS:** PLANNED · **PRIORITY:** P0 · **DEPENDS_ON:** P01-T009 · **FILES_SCOPE:** `src/MDReader.Core/Markdown/`, `src/MDReader.Core/Documents/`, `docs/ARCHITECTURE.md`

- **SUBTASKS:** Specify parse result/error model; settle UTF-8 and size-warning behavior; document Markdig and permitted extensions/dependency.
- **ACCEPTANCE CRITERIA:** Core stays platform-neutral; parser returns a deterministic document model and actionable errors; Markdig package/version/license and use are recorded before addition; no raw HTML execution.
- **TEST:** Core unit tests for empty/plain/invalid input and representative AST nodes; build/test.
- **VERIFICATION:** `dotnet build` and `dotnet test`; review dependency and Core boundary.

### P02-T002 — Implement Markdown file reading and validation
**STATUS:** PLANNED · **PRIORITY:** P0 · **DEPENDS_ON:** P02-T001 · **FILES_SCOPE:** `src/MDReader.Core/`, `tests/MDReader.Core.Tests/`

- **SUBTASKS:** Read selected path; validate supported extension/access; handle disappearance, lock, encoding and size warning.
- **ACCEPTANCE CRITERIA:** `.md` and `.markdown` local files are read without modifying them; expected I/O failures become typed/recoverable results; no directory crawl or persistent data.
- **TEST:** Unit tests for valid, empty, missing, inaccessible, unsupported, malformed and oversized input using controlled fixtures.
- **VERIFICATION:** `dotnet build`, `dotnet test`; manual read-only file check.

### P02-T003 — Add Open picker and command-line file entry
**STATUS:** PLANNED · **PRIORITY:** P0 · **DEPENDS_ON:** P01-T009, P02-T002 · **FILES_SCOPE:** `src/MDReader.App/Features/FileOpen/`, `src/MDReader.App/Features/Shell/`, `src/MDReader.App/Features/Reader/`

- **SUBTASKS:** Add Markdown-filtered picker; accept one CLI path; route both through one open command; expose recoverable errors.
- **ACCEPTANCE CRITERIA:** Picker and `mdreader.exe <path>` open one selected Markdown document; opening a second replaces the first; errors do not crash; no multi-tab/folder scanning.
- **TEST:** Build/test; manual picker, CLI valid path, invalid path and replace-current checks.
- **VERIFICATION:** Build/test plus recorded launch checks.

### P02-T004 — Parse and render basic Markdown
**STATUS:** PLANNED · **PRIORITY:** P0 · **DEPENDS_ON:** P02-T001, P02-T003 · **FILES_SCOPE:** `src/MDReader.Core/Markdown/`, `src/MDReader.App/Features/Reader/`, `tests/MDReader.Core.Tests/`

- **SUBTASKS:** Parse headings/paragraphs/emphasis/lists/code/quotes/rules/tables; render AST nodes as WinUI controls; inert handling for HTML.
- **ACCEPTANCE CRITERIA:** MVP elements are readable and styled; long code lines scroll horizontally; unsupported nodes degrade safely; no WebView, edit, or save path.
- **TEST:** Core parser tests for each supported node and unsafe HTML; manual visual checks for long code and Arabic sample.
- **VERIFICATION:** Build/test and manual render checklist.

### P02-T005 — Add scrollable reader and document replacement lifecycle
**STATUS:** PLANNED · **PRIORITY:** P1 · **DEPENDS_ON:** P02-T004 · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/`

- **SUBTASKS:** Add reader scrolling; release old visual tree on replacement; keep empty/error/loading states distinct.
- **ACCEPTANCE CRITERIA:** Mouse-wheel and touch scrolling work; replace-current path releases prior content; empty, loading, error, and loaded states are understandable.
- **TEST:** Build/test; manual scroll, rapid replacement, and empty/error state checks.
- **VERIFICATION:** UI checklist recorded in task evidence.

### P02-T006 — Phase 2 verification gate
**STATUS:** PLANNED · **PRIORITY:** P0 · **DEPENDS_ON:** P02-T002, P02-T003, P02-T004, P02-T005 · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`

- **SUBTASKS:** Run full build/tests; exercise open/render/errors; record limitations and evidence; update phase/state.
- **ACCEPTANCE CRITERIA:** Every Phase 2 task is DONE; build/test pass; picker and CLI paths verified; no unsupported claims; only then mark phase complete.
- **TEST:** `dotnet build`, `dotnet test`, manual acceptance checklist.
- **VERIFICATION:** Commit and progress metadata follow workflow; update verified commit only after code build/test commit.

## Phase 3 — Rendering Quality and Safe References

**Status:** PLANNED · **Depends on:** Phase 2 complete.

### P03-T001 — Complete agreed GFM rendering
**FEATURE:** Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P02-T006 · **FILES_SCOPE:** `src/MDReader.Core/Markdown/`, `src/MDReader.App/Features/Reader/`, `tests/MDReader.Core.Tests/`
- **SUBTASKS:** Add tables/strikethrough/task lists; define unsupported extension behavior; cover nested content.
- **ACCEPTANCE CRITERIA:** Agreed GFM subset renders consistently; unsupported syntax remains safe/readable; no extension expands to HTML/script execution.
- **TEST:** Parser tests for nested tables/lists/task lists/strikethrough and malformed syntax.
- **VERIFICATION:** Build/test plus representative document review.

### P03-T002 — Implement Arabic, RTL, and mixed-direction layout
**FEATURE:** Reader · **PRIORITY:** P0 · **DEPENDS_ON:** P02-T006 · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/`, `src/MDReader.App/Shared/`, `tests/`
- **SUBTASKS:** Establish paragraph direction behavior; preserve LTR code/URLs; select supported font fallback.
- **ACCEPTANCE CRITERIA:** Arabic/Hebrew and English paragraphs are readable; mixed punctuation/URLs and code retain correct direction; no whole-window mirroring breaks LTR content.
- **TEST:** RTL/LTR fixture tests where possible and manual Arabic/English mixed-content review.
- **VERIFICATION:** Build/test and documented visual checklist.

### P03-T003 — Resolve local images safely
**FEATURE:** Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P03-T001 · **FILES_SCOPE:** `src/MDReader.Core/`, `src/MDReader.App/Features/Reader/`, `tests/`
- **SUBTASKS:** Normalize relative path; enforce document-tree boundary including symlink check; load raster image with alt text and fallback.
- **ACCEPTANCE CRITERIA:** Only local supported images under the document directory render; traversal, escaped symlink, missing and malformed images fail safely; no remote image fetch.
- **TEST:** Path-boundary tests plus manual supported/missing image check.
- **VERIFICATION:** Build/test and security review of path checks.

### P03-T004 — Add safe links and heading anchors
**FEATURE:** Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P03-T001 · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/`, `src/MDReader.Core/`, `tests/`
- **SUBTASKS:** Generate stable anchors; launch only clicked HTTP(S); define local Markdown link confirmation/open behavior; reject unsupported schemes.
- **ACCEPTANCE CRITERIA:** Internal anchors navigate; explicit external click uses OS handler; local links stay within permitted user-selected path behavior; no implicit network fetch.
- **TEST:** Unit tests for scheme/path filtering and manual anchor/external link check.
- **VERIFICATION:** Build/test and security checklist.

### P03-T005 — Phase 3 verification gate
**FEATURE:** Reader · **PRIORITY:** P0 · **DEPENDS_ON:** P03-T002, P03-T003, P03-T004 · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`
- **SUBTASKS:** Run build/tests; inspect Arabic, GFM, image, link cases; record results.
- **ACCEPTANCE CRITERIA:** All phase tasks verified; failure/limitation accurately documented; no code change outside task scopes.
- **TEST:** `dotnet build`, `dotnet test`, manual content/security matrix.
- **VERIFICATION:** Update state and verified commit only after verified code commit.

## Phase 4 — Windows UX and Accessibility

**Status:** PLANNED · **Depends on:** Phase 3 complete.

### P04-T001 — Refine native shell and File/View/Settings commands
**FEATURE:** Shell · **PRIORITY:** P1 · **DEPENDS_ON:** P03-T005 · **FILES_SCOPE:** `src/MDReader.App/Features/Shell/`, `src/MDReader.App/Features/Settings/`
- **SUBTASKS:** Align title/menu controls with UI design; expose available reader commands; retain native minimize/maximize/close behavior.
- **ACCEPTANCE CRITERIA:** Shell is consistent with Windows and UI_DESIGN; every displayed command has a working, named action; app remains a reader only.
- **TEST:** Build/test and manual menu/window check.
- **VERIFICATION:** Build/test plus interaction checklist.

### P04-T002 — Add keyboard shortcuts and in-document find
**FEATURE:** Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P04-T001 · **FILES_SCOPE:** `src/MDReader.App/Features/Reader/`, `src/MDReader.App/Features/Shell/`
- **SUBTASKS:** Implement Ctrl+O, Ctrl+R, Ctrl+F; next/previous and clear; show shortcut labels.
- **ACCEPTANCE CRITERIA:** Find searches only current document; shortcuts are discoverable; no folder-wide search or source editing.
- **TEST:** Build/test plus manual keyboard-only search/reload/open checks.
- **VERIFICATION:** Recorded shortcut and focus checklist.

### P04-T003 — Support single-file drag-and-drop
**FEATURE:** FileOpen · **PRIORITY:** P1 · **DEPENDS_ON:** P04-T001, P02-T003 · **FILES_SCOPE:** `src/MDReader.App/Features/FileOpen/`, `src/MDReader.App/Features/Shell/`, `src/MDReader.App/Features/Reader/`
- **SUBTASKS:** Accept one supported file; route through common open flow; reject folder/multiple/unsupported drops clearly.
- **ACCEPTANCE CRITERIA:** One `.md`/`.markdown` drop opens safely and replaces current document; unsupported drops do not crash or broaden access.
- **TEST:** Build/test and manual file/folder/multiple/unsupported drop checks.
- **VERIFICATION:** Record outcome and error behavior.

### P04-T004 — Add current-session recent list and theme/language controls
**FEATURE:** Settings · **PRIORITY:** P2 · **DEPENDS_ON:** P04-T001 · **FILES_SCOPE:** `src/MDReader.App/Features/Settings/`, `src/MDReader.App/Features/FileOpen/`, `src/MDReader.App/Features/Theme/`
- **SUBTASKS:** Track up to 10 in-memory paths; clear on exit; expose theme and available English/Arabic labels.
- **ACCEPTANCE CRITERIA:** Recent list contains only files opened during current process and is not written to disk; theme supports System/Light/Dark; no persistent preference/session storage is introduced.
- **TEST:** Unit tests for list bound/order and manual exit/relaunch/theme checks.
- **VERIFICATION:** Inspect storage behavior; build/test and UI checks.

### P04-T005 — Accessibility and DPI verification gate
**FEATURE:** Shell/Reader · **PRIORITY:** P1 · **DEPENDS_ON:** P04-T002, P04-T003, P04-T004 · **FILES_SCOPE:** `src/MDReader.App/`, `docs/UI_DESIGN.md`, `.project/`
- **SUBTASKS:** Verify keyboard order/focus, Narrator names, contrast and 100/150/200/250% scaling; fix scoped issues.
- **ACCEPTANCE CRITERIA:** Core actions keyboard-accessible; images/controls named; layout usable at target scale factors; issues recorded and resolved or blocked transparently.
- **TEST:** Build/test, manual Narrator/keyboard/high-contrast/multi-DPI matrix.
- **VERIFICATION:** Attach checklist results; do not claim untested OS configurations.

## Phase 5 — Reliability and Performance

**Status:** PLANNED · **Depends on:** Phase 4 complete.

### P05-T001 — Harden file, parser, and rendering failure paths
**FEATURE:** Core/Reader · **PRIORITY:** P0 · **DEPENDS_ON:** P04-T005 · **FILES_SCOPE:** `src/MDReader.Core/`, `src/MDReader.App/Features/Reader/`, `tests/`
- **SUBTASKS:** Audit exception boundaries; validate malformed/removed/locked files; ensure retry/reload state recovers.
- **ACCEPTANCE CRITERIA:** Expected failures produce actionable UI and no crash; prior document state is not corrupted by a failed open.
- **TEST:** Fault-path unit tests and manual remove/lock/reload scenarios.
- **VERIFICATION:** Build/test and error matrix.

### P05-T002 — Measure large document and rapid switching behavior
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
**FEATURE:** Reliability · **PRIORITY:** P0 · **DEPENDS_ON:** P05-T001, P05-T002, P05-T003 · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`
- **SUBTASKS:** Review failures/performance/coverage evidence; update plan/state.
- **ACCEPTANCE CRITERIA:** All reliability criteria have evidence or remain explicitly blocked; no metric is inferred from an unrun test.
- **TEST:** Build/test plus recorded reliability matrix.
- **VERIFICATION:** Commit and metadata gate per workflow.

## Phase 6 — MSIX Packaging

**Status:** PLANNED · **Depends on:** Phase 5 complete.

### P06-T001 — Configure reproducible MSIX package identity and manifest
**FEATURE:** Packaging · **PRIORITY:** P1 · **DEPENDS_ON:** P05-T004 · **FILES_SCOPE:** `src/MDReader.App/Package.appxmanifest`, packaging project/configuration, `docs/`
- **SUBTASKS:** Review identity/version/capabilities; configure release build; keep signing material outside repository.
- **ACCEPTANCE CRITERIA:** Least-privilege manifest is valid; package build reproducible on documented Windows SDK tooling; no secret/certificate private key committed.
- **TEST:** Package build and manifest/capability review.
- **VERIFICATION:** Capture package build result and configuration instructions.

### P06-T002 — Verify install, launch, and uninstall
**FEATURE:** Packaging · **PRIORITY:** P1 · **DEPENDS_ON:** P06-T001 · **FILES_SCOPE:** `docs/`, packaging config only if needed
- **SUBTASKS:** Install test package; launch/open sample; verify uninstall and user-file preservation.
- **ACCEPTANCE CRITERIA:** Installed app launches and reads selected local file; uninstall is clean; no custom updater added.
- **TEST:** Manual packaged lifecycle matrix on supported Windows environment.
- **VERIFICATION:** Record OS/package/signing context and results.

### P06-T003 — Phase 6 verification gate
**FEATURE:** Packaging · **PRIORITY:** P0 · **DEPENDS_ON:** P06-T002 · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`
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
**FEATURE:** Distribution · **PRIORITY:** P0 · **DEPENDS_ON:** P07-T002 · **FILES_SCOPE:** `.project/STATE.json`, `.project/TASKS.md`, `.project/CHANGELOG.md`, `docs/ROADMAP.md`
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

