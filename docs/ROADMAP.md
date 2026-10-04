# MDReader — Roadmap

> Phase 0 is complete at baseline `5f81c9e`. Phase 1 remains unstarted. This roadmap is planning-only and subject to owner review of [`PRODUCT_BLUEPRINT.md`](PRODUCT_BLUEPRINT.md). Task-level scope, permanent IDs, dependencies, and verification live in [`.project/TASKS.md`](../.project/TASKS.md); machine state lives in [`.project/STATE.json`](../.project/STATE.json).

## Phase 0 — Environment and Architecture — DONE
Baseline environment, solution, feature-based structure, architecture and documentation established. Reported verification: build PASS (0 warnings/errors); tests PASS (2/2); baseline `5f81c9e`.

## Phase 1 — Application Shell — READY, NOT STARTED
Implement shell, MVVM foundation, window sizing/DPI, system theme, empty reader state, keyboard accessibility, and Core initialization tests. Preserve existing task IDs P01-T001 through P01-T009. Exit gate: build/test pass and launch verified or limitation recorded. No Markdown reading in this phase.

## Phase 2 — File Open and Basic Reader — PLANNED
Open one `.md`/`.markdown` file via picker and CLI; read safely; parse with documented Markdig dependency; render MVP Markdown nodes in a scrollable view; handle failures. Test Core parsing and file behavior. No folder crawling or editing.

## Phase 3 — Rendering Quality and Safe References — PLANNED
Complete agreed Markdown subset, Arabic/RTL behavior, safe in-tree local images, internal anchors, user-clicked external URLs and defined file-relative Markdown navigation. Add GFM cases. HTML stays inert; no remote content retrieval.

## Phase 4 — Windows UX and Accessibility — PLANNED
Refine native shell/title/menu, keyboard shortcuts, find within current document, single-file drag/drop, in-memory recent list, theme/language UI behavior, accessibility and high-DPI checks. Print remains deferred unless the owner approves it in the Blueprint.

## Phase 5 — Reliability and Performance — PLANNED
Malformed/unreadable and rapid-switch scenarios, large document measurements (including >=10 MB), startup measurement, memory review, Core coverage target, and CI decision. Only add complexity such as virtualization when evidence requires it.

## Phase 6 — MSIX Packaging — PLANNED
Build, sign through a non-committed development/distribution process, install/uninstall, and launch the MSIX package. No custom updater.

## Phase 7 — Microsoft Store Readiness — PLANNED
Review capabilities and certification, privacy/legal disclosures, manifest and listing assets. Prepare a submission-ready package; actual publication requires a separate owner action.

## Global exclusions
No editing, accounts/cloud sync, database, telemetry, plugin marketplace, collaboration, browser automation, embedded HTML/script execution, custom updater, or AI provider integration. Persistent recent/session/preferences remain deferred until the owner resolves the Blueprint storage conflict.

## Planning gate
The owner reviews and edits `PRODUCT_BLUEPRINT.md` and `.project/TASKS.md` before any Phase starts. Until explicit acceptance and loop authorization, `.project/STATE.json` remains `PLAN_REVIEW`, current task null, and no implementation task may be selected.
