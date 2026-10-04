# Project Identity

Name:
MDReader

Project Root:
D:\AI_PROJECTS\MDReader

Project Type:
Independent Windows Application

Repository:
Independent Git repository

Platform:
Windows

Architecture:
Feature-Based + MVVM

UI:
WinUI 3

Language:
C#

Runtime:
.NET

Packaging Target:
MSIX

Distribution Target:
Microsoft Store

AI Control Surface:
Architectural boundary exists.
External AI integration is NOT enabled.

---

## Project Independence Policy

MDReader is one independent application. It has its own Git repository, solution file,
source code, tests, documentation, and release lifecycle under `D:\AI_PROJECTS\MDReader\`.

Future applications such as:

- SnapDrop
- DayFlow
- and other utilities

must use separate repositories and projects. Do not place unrelated applications
inside the MDReader repository. Each application gets its own root directory under
`D:\AI_PROJECTS\` with its own solution, `.gitignore`, docs, and `.project/` control layer.

---

## Project Control Hierarchy

The project control system provides operational memory across autonomous development
sessions. Work flows through this hierarchy:

```
PROJECT
  ↓
PHASE
  ↓
TASK
  ↓
SUBTASK
  ↓
IMPLEMENTATION
  ↓
BUILD
  ↓
TEST
  ↓
VERIFY
  ↓
COMMIT
  ↓
PROGRESS
```

- **PROJECT** — The entire MDReader application. Defined by this document.
- **PHASE** — A major milestone with a set of related deliverables (e.g., Phase 1: Application Shell).
- **TASK** — A single unit of executable work within a phase, identified by a permanent ID.
- **SUBTASK** — A finer-grained step within a task (optional, used when a task has distinct sub-steps).
- **IMPLEMENTATION** — The actual code/test/doc changes made for a task.
- **BUILD** — `dotnet build` must succeed with 0 errors.
- **TEST** — `dotnet test` must pass all tests.
- **VERIFY** — Acceptance criteria for the task are confirmed (including runtime verification when possible).
- **COMMIT** — A single coherent git commit capturing the verified task.
- **PROGRESS** — Update STATE.json, TASKS.md, CHANGELOG.md, and ROADMAP.md.

A task is only **DONE** when implementation, build, test, and verification all pass.

---

## Operational Sources of Truth

The project maintains several sources of truth, each with clearly separated responsibilities.
No file may silently replace another file's responsibility.

| Source | File / Location | Responsibility |
|---|---|---|
| **PRODUCT_SPEC** | `docs/PRODUCT_SPEC.md` | **WHAT** the product is — purpose, MVP features, tech stack, out-of-scope items. |
| **ARCHITECTURE** | `docs/ARCHITECTURE.md` | **HOW** the software is architected — technology choices, project boundaries, dependency direction, design decisions. |
| **FEATURE_MAP** | `docs/FEATURE_MAP.md` | **WHERE** capabilities/features live — which directory owns each feature and the communication mechanism between features. |
| **ROADMAP** | `docs/ROADMAP.md` | **WHICH** major phases exist — high-level phased delivery plan with goals and deliverables per phase. |
| **TASKS** | `.project/TASKS.md` | **WHAT** executable work exists — the detailed backlog of tasks organized by phase, with acceptance criteria and verification steps. |
| **STATE** | `.project/STATE.json` | **WHERE** autonomous development currently is — machine-readable pointer to the current phase, task, and status. |
| **PROJECT** | `.project/PROJECT.md` | **PROJECT** identity, technology, and control rules — this file. |
| **PRODUCT_BLUEPRINT** | `docs/PRODUCT_BLUEPRINT.md` | **WHAT** the product is — expanded definition, user targets, scope boundaries, deferred items, owner review items. Adopted by user authorization. |
| **CHANGELOG** | `.project/CHANGELOG.md` | **VERIFIED** project-level milestones — only meaningful, verified outcomes. |
| **Git** | `.git/` repository | **WHAT** actually changed — full history and rollback capability. |
| **Tests/Build** | `dotnet build` / `dotnet test` | **WHETHER** implementation is technically verified — compilation and test pass/fail. |

### Source-of-truth precedence

When sources conflict, the precedence is:

1. **Tests/Build** — The ultimate arbiter. If code does not build or tests fail, no claim of correctness stands.
2. **Git** — Provides immutable history and rollback.
3. **ARCHITECTURE.md** — Cannot be changed silently; every architectural change must be documented here.
4. **PRODUCT_SPEC.md** — Defines MVP scope; no scope change without updating this file.
5. **ROADMAP.md** — Defines phase boundaries and deliverables.
6. **TASKS.md** — Defines the executable work backlog.
7. **STATE.json** — Machine-readable current position.
8. **CHANGELOG.md** — Human-readable verified milestones.
9. **PROJECT.md** — Control rules and project identity.

---

## Commit Consistency Convention

To avoid creating a permanently dirty repository merely because `STATE.json` records
the current commit, the following convention is used:

**`last_verified_commit`** in `STATE.json` represents the last **code/build verification**
commit — i.e., the most recent commit where `dotnet build` and `dotnet test` were verified
to pass on the actual source code.

The Project Control Layer metadata files (`.project/`) are committed as a single
**metadata checkpoint** that does not alter `last_verified_commit` because metadata
files contain no application code. Future Phase 1 implementation tasks will produce
code commits; after each code commit is verified (build + test), `last_verified_commit`
is updated to that commit's hash.

This convention is deterministic: `last_verified_commit` only moves forward on commits
that pass build + test verification of application code.

---

## Agent Workflow Entry Point

Every new autonomous Hermes session MUST start by reading this control layer:

1. Read `.project/PROJECT.md` and `.project/STATE.json`.
2. Read the documentation suite (`docs/*.md`).
3. Read `.project/TASKS.md` and `.project/CHANGELOG.md`.
4. Inspect `git status` and `git log` to verify STATE against actual repository.
5. Select only the next eligible **READY** task (all dependencies DONE).
6. Follow the implementation lifecycle: IN_PROGRESS → VERIFYING → DONE.

See `docs/AGENT_WORKFLOW.md` for the full workflow.

---

## Recovery After New Session

A completely new Hermes session must be able to recover using ONLY the repository.
The session reads `.project/STATE.json` to find the current phase and task, reads
`.project/TASKS.md` for the work backlog, checks `git status` to confirm the repository
state, and continues from the next eligible task. No session or conversation history
is relied upon.

## Product Blueprint and Owner Review Gate

`docs/PRODUCT_BLUEPRINT.md` is the detailed, owner-reviewable product boundary. Read it before interpreting MVP scope or decomposing implementation. `docs/PRODUCT_SPEC.md` remains the WHAT source; update both together when the owner accepts a scope change. Until the owner explicitly accepts the Blueprint and starts the Autonomous Developer Loop, `.project/STATE.json` is `PLAN_REVIEW`; no implementation task may start. A metadata-only planning checkpoint does not change the verified code commit, Phase 0 result, or build/test claims. Conflicts called out in the Blueprint are review items; do not silently resolve them by adding features or persistent storage.
