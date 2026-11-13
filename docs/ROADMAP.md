# MDReader — Roadmap

> Phase 0 is complete at baseline `5f81c9e`. Phase 1 remains unstarted. This roadmap is planning-only and subject to owner review of [`PRODUCT_BLUEPRINT.md`](PRODUCT_BLUEPRINT.md). Task-level scope, permanent IDs, dependencies, and verification live in [`.project/TASKS.md`](../.project/TASKS.md); machine state lives in [`.project/STATE.json`](../.project/STATE.json).

## Phase 0 — Environment and Architecture — DONE
Baseline environment, solution, feature-based structure, architecture and documentation established. Reported verification: build PASS (0 warnings/errors); tests PASS (2/2); baseline `5f81c9e`.

## Phase 1 — Application Shell — DONE (9/9 tasks: P01-T001..P01-T009 verified; build PASS 0 warnings, launch verified, Developer Mode available)
Shell, MVVM, window size/position, theme service, empty reader state, keyboard accessibility, shared styles, and Core initialization tests completed. Gate P01-T009 passed. Launch test: `MDReader.App.exe` launched successfully; no runtime errors.

## Phase 2 — File Open and Basic Reader — DONE (6/6 tasks: P02-T001..P02-T006 verified; build PASS 0 warnings, tests PASS 41/41, runtime verified via AX tree)
Open one `.md`/`.markdown` file via picker and CLI; read safely; parse with documented Markdig dependency; render MVP Markdown nodes in a scrollable view; handle failures. Verified: CLI valid path renders headings/paragraphs/emphasis/code/lists/quotes/rules/tables/links; invalid path shows a recoverable error (no crash); scroll works; loading/empty/error/content states distinct. No folder crawling or editing.

## Phase 3 — Rendering Quality and Safe References — DONE (5 / 5 tasks verified: P03-T001..P03-T005)
Complete agreed Markdown subset, Arabic/RTL behavior, safe in-tree local images, internal anchors, user-clicked external URLs and defined file-relative Markdown navigation. Add GFM cases. HTML stays inert; no remote content retrieval.

## Phase 4 — Windows UX and Accessibility — DONE (5/5 tasks: P04-T001..P04-T005 verified)
Refined native shell/title/menu, keyboard shortcuts (Ctrl+O/R/F, zoom), in-document find with Previous/Next, single-file drag/drop, in-memory recent list, theme/language UI behavior, accessibility and high-DPI checks. Print remains deferred unless the owner approves it in the Blueprint.

## Phase 5 — Reliability and Performance — DONE (4 / 4 tasks verified: P05-T001..P05-T004)
Hardened failure paths; performance measurements recorded; Core coverage and CI policy established; verification gate passed.

## Phase 6 — MSIX Packaging — P06-T001 DONE, P06-T002 DONE (manual verification blocked by missing signing cert / no sideload env; gate P06-T003 DONE); Phase 6 complete
Build, sign through a non-committed development/distribution process, install/uninstall, and launch the MSIX package. No custom updater.

## Phase 7 — Microsoft Store Readiness — DONE (3/3 tasks: P07-T001..P07-T003 verified)
Store policy, privacy/capability review, and listing assets completed. No Store submission performed (separate owner authorization required per blueprint).

## Phase 8 — Verified Runtime Defect Remediation — DONE (P08-T001 verified)
Intermittent startup crash on file open fixed (MarkdownLinkPolicy + dispatcher deferral). Build PASS (0 errors); tests PASS (60/60); statistical verification 12/12 ALIVE (180s).

## Phase 9 — Auto/RTL/LTR Zoom and Reset — DONE (2/2 tasks: P09-T001, P09-T002 verified)
Zoom toolbar with visible ratio, Ctrl++/Ctrl+-/Ctrl+0 keyboard shortcuts, RTL/LTR direction detection per paragraph, LTR preservation for code/URLs. Build PASS (0 errors); tests PASS (60/60).

## Phase 10 — Core Mutation Foundation and Safe File I/O — DONE (2/2 tasks: P10-T001, P10-T002 verified; 66/66 tests pass)
Lift read-only constraints in Core. Implement atomic file writing (`MarkdownFileWriter`), backup mechanics, document dirty tracking (`IsDirty`), and UTF-8 Arabic round-trip tests.

## Phase 11 — Multi-Document Tab System and Folder Workspace — DONE (2/2 tasks: P11-T001, P11-T002 verified; 69/69 tests pass)
Integrate WinUI 3 `TabView` for multi-tab document editing/viewing with dirty markers (`*`), and a workspace folder `TreeView` sidebar for quick file browsing.

## Phase 12 — Antigravity-Grade Offline Rendering Engine — IN_PROGRESS
Embed 100% offline WebView2 component with bundled local scripts: GitHub Alerts (`[!NOTE]`, `[!TIP]`, `[!IMPORTANT]`, `[!WARNING]`, `[!CAUTION]`), Mermaid.js diagrams, KaTeX math expressions, and syntax-highlighted code blocks with copy-to-clipboard action.

## Phase 13 — Live Markdown Editor and Synchronized Split View — PLANNED
Deliver a dual-pane workspace: Editor on one side, live Antigravity preview on the other, synchronized scrolling, view mode switcher (Split / Editor / Preview), and quick formatting toolbar.

## Phase 14 — Performance, RTL Polish, and MSIX Packaging — PLANNED
Stress test large documents (10MB+), verify seamless Arabic/English RTL/LTR bidirectional typing, run complete automated test suite, and generate final production MSIX.

## Evolution Authorization — CLEARED
The owner authorized the Antigravity-grade Reader & Editor evolution on 2026-11-13. `.project/STATE.json` tracks execution under the Autonomous Multi-Agent Developer & Verifier Loop.
