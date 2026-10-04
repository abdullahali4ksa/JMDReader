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
