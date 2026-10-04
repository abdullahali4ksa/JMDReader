# Agent Workflow — MDReader

## How an Autonomous Hermes Session Should Work

> **Mandatory entry point for every autonomous session.** A new Hermes session
> must recover using ONLY the repository — no session or conversation history.

```
1. LOCATE REPOSITORY ROOT
   Identify D:\AI_PROJECTS\MDReader and confirm git HEAD.

2. READ PROJECT CONTROL LAYER
   .project/PROJECT.md
   .project/STATE.json
   .project/TASKS.md
   .project/CHANGELOG.md

3. READ PROJECT DOCS
   docs/PRODUCT_SPEC.md
   docs/ARCHITECTURE.md
   docs/FEATURE_MAP.md
   docs/UI_DESIGN.md
   docs/DEVELOPMENT_RULES.md
   docs/DECISIONS.md
   docs/ROADMAP.md

4. INSPECT GIT STATE
   git status
   git log (verify current HEAD)

5. VERIFY STATE
   Compare .project/STATE.json against actual git HEAD, build, and test status.

6. SELECT NEXT ELIGIBLE TASK
   From .project/TASKS.md: choose the next READY task whose dependencies are DONE.
   Do NOT start a task whose dependencies are not DONE.
   Do NOT skip or reorder phases.

7. IDENTIFY FEATURE OWNER
   Find the feature directory from docs/FEATURE_MAP.md
   e.g., Features/Reader/ for Markdown rendering

8. IMPLEMENT ONLY INSIDE THE TASK'S FILES_SCOPE
   Never modify unrelated features.
   e.g., implementing Reader should NOT touch FileOpen XAML

9. BUILD
   dotnet build

10. TEST
    dotnet test

11. RUNTIME VERIFY
    Verify acceptance criteria. If runtime verification is impossible
    (e.g., Developer Mode not enabled), record the limitation — do not
    falsely claim runtime verification passed.

12. MARK TASK DONE
    Only if build passes, tests pass, and acceptance criteria are met.
    Update .project/TASKS.md, .project/STATE.json, .project/CHANGELOG.md.

13. UPDATE DOCS
    Update docs/FEATURE_MAP.md, docs/ROADMAP.md, docs/ARCHITECTURE.md
    if the task introduced new decisions or scope changes.

14. REVIEW DIFF
    git diff --stat
    Check: no unintended changes, all changes within feature scope

15. COMMIT
    git add -A
    git commit -m "<task-id>: <feature description>"
    Commit message includes the task ID (e.g., P01-T001: add CommunityToolkit.Mvvm)

16. NEXT TASK
    Return to step 6
```

### Task Lifecycle States

| State | Meaning |
|---|---|
| **PLANNED** | Phase exists but tasks not yet decomposed |
| **READY** | Task is decomposed, dependencies are DONE, ready to start |
| **IN_PROGRESS** | Agent is actively implementing the task |
| **VERIFYING** | Implementation done; build/test/runtime verification in progress |
| **DONE** | All acceptance criteria verified — build, tests, and criteria pass |
| **BLOCKED** | Task cannot proceed; reason recorded in TASKS.md |
| **CANCELLED** | Task removed; ID never reused |

A task is only **DONE** when implementation, build, test, and verification all pass.

## Environment Setup

```bash
# SDK location (user-local, installed via dotnet-install.ps1)
C:/Users/ZAACO/.dotnet/dotnet.exe

# System-level SDK (installed via EXE)
C:/Program Files/dotnet/dotnet.exe

# Build from either path — global.json pins to 10.0.401
C:/Users/ZAACO/.dotnet/dotnet.exe --version
```

## Git Convention

- Each commit scoped to a single feature/phase
- Commit message: `Phase X: <feature description>`
- All changes committed together (code + tests + docs)
- No WIP commits (commit only when build + tests pass)

## Change Control

**Autonomous (no approval needed):**
- Creating files inside an approved feature
- Writing tests
- Fixing bugs
- Local refactoring
- Updating documentation

**Requires STOP and wait for approval:**
- Changing technology stack
- Changing .NET major version
- Replacing WinUI
- Adding database/cloud dependency
- Adding external AI provider (OpenAI, OpenRouter, etc.)
- Opening network ports
- Implementing AI transport (MCP, IPC, HTTP)
- Creating cross-application shared framework

## Product Plan Review Gate (Planning checkpoint)

Before any autonomous implementation session, read `docs/PRODUCT_BLUEPRINT.md` and the complete `.project/TASKS.md` backlog. The current planning state is `PLAN_REVIEW`: do not select or start a READY coding task until the project owner explicitly accepts/edits the blueprint and authorizes the Autonomous Developer Loop. Planning/documentation work is not a Phase completion and must not change code, build/test claims, `last_verified_commit`, or phase completion. After owner acceptance, update this gate/state metadata and begin only the first eligible task whose dependencies are DONE.

For each task, maintain the permanent `Pxx-Txxx` ID and record its feature, priority, dependencies, allowed file scope, subtasks, acceptance criteria, and verification command/check. A change outside the stated file scope requires revising the task/backlog first. A planning task has document-only scope and does not authorize app changes.
