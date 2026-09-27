# Agent Workflow — MDReader

## How an Autonomous Hermes Session Should Work

```
1. READ PROJECT DOCS
   docs/*.md (PRODUCT_SPEC, ARCHITECTURE, FEATURE_MAP, AI_INTERFACE, DEVELOPMENT_RULES)

2. SELECT TASK
   Choose from docs/ROADMAP.md (Phase 1, 2, 3...)

3. IDENTIFY FEATURE OWNER
   Find the feature directory from docs/FEATURE_MAP.md
   e.g., Features/Reader/ for Markdown rendering

4. PLAN
   What files to create/modify, what tests to write
   Ensure the plan stays within the feature boundary

5. UPDATE DESIGN IF NECESSARY
   Add new features or commands to FEATURE_MAP.md and AI_INTERFACE.md

6. IMPLEMENT ONLY INSIDE RELEVANT BOUNDARIES
   Never modify unrelated features
   e.g., implementing Reader should NOT touch FileOpen XAML

7. BUILD
   C:/Users/ZAACO/.dotnet/dotnet.exe build MDReader.sln --nologo

8. TEST
   C:/Users/ZAACO/.dotnet/dotnet.exe test MDReader.sln --nologo --no-build

9. REVIEW DIFF
   git diff --stat
   Check: no unintended changes, all changes within feature scope

10. FIX
    Address warnings, errors, or unintended changes

11. UPDATE DOCS
    Update FEATURE_MAP.md if new commands/features added
    Update ROADMAP.md to reflect progress

12. COMMIT
    git add -A
    git commit -m "Phase X: <feature description>"

13. NEXT TASK
    Return to step 2
```

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
