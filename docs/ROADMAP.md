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

## Phase 4 — Windows UX and Accessibility — PLANNED
Refine native shell/title/menu, keyboard shortcuts, find within current document, single-file drag/drop, in-memory recent list, theme/language UI behavior, accessibility and high-DPI checks. Print remains deferred unless the owner approves it in the Blueprint.

## Phase 5 — Reliability and Performance — DONE (4 / 4 tasks verified: P05-T001..P05-T004)
Hardened failure paths; performance measurements recorded; Core coverage and CI policy established; verification gate passed.

## Phase 6 — MSIX Packaging — P06-T001 DONE, P06-T002 DONE (manual verification blocked by missing signing cert / no sideload env; gate P06-T003 READY)
Build, sign through a non-committed development/distribution process, install/uninstall, and launch the MSIX package. No custom updater.

## Phase 7 — Microsoft Store Readiness — PLANNED
Review capabilities and certification, privacy/legal disclosures, manifest and listing assets. Prepare a submission-ready package; actual publication requires a separate owner action.

## Global exclusions
No editing, accounts/cloud sync, database, telemetry, plugin marketplace, collaboration, browser automation, embedded HTML/script execution, custom updater, or AI provider integration. Persistent recent/session/preferences remain deferred until the owner resolves the Blueprint storage conflict.

## Planning gate — CLEARED
The owner accepted `PRODUCT_BLUEPRINT.md` and `.project/TASKS.md` on 2026-10-05 and authorized the Autonomous Developer Loop. `.project/STATE.json` records `implementation_authorized: true`. Implementation proceeds task by task from the lowest eligible READY task whose dependencies are DONE. Any future scope change still requires owner acceptance before its tasks become READY.
