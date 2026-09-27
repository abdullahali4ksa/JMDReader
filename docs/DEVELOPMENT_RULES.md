# DEVELOPMENT_RULES.md

> **Purpose:** This document establishes the ground rules for every AI agent and human developer working on the MDReader project. These rules exist to preserve architectural integrity, prevent scope creep, and ensure reliable, Windows-native builds. **These rules are non-negotiable.**

---

## 1. Architecture Must Never Change Silently

- Every change to the project structure, layer boundaries, or dependency direction **must be documented** in `docs/ARCHITECTURE.md`.
- `MDReader.Core` **must never** reference `WinUI`, `Windows App SDK`, or any Windows-specific API. If a Windows API is needed, it must live in `MDReader.App` or a dedicated interop layer — never in Core.
- `MDReader.Core` **must never** depend on `MDReader.App`. Dependency direction: `App → Core` only.
- Do not reorganize folders, rename projects, or move files without updating all references (`*.sln`, `*.csproj`, imports) and committing the change atomically.

## 2. No New Projects Without Justification

- The approved project structure is:
  ```
  MDReader/
    src/MDReader.App/      — WinUI 3 UI, AI control surface, shared components
    src/MDReader.Core/     — Markdown logic, document models, contracts
    tests/MDReader.Core.Tests/ — Automated tests for Core
  ```
- Do not create additional projects (e.g., a shared library, a services layer, a data layer) unless a concrete, documented requirement exists and has been reviewed.
- Do not merge `MDReader.Core` code into `MDReader.App` or vice versa.

## 3. NuGet Dependencies Require Documentation

Before adding any NuGet package:

1. Document **purpose** — what problem does it solve?
2. Document **version** — exact stable version selected.
3. Document **license** — open-source license or commercial terms.
4. Document **why built-in functionality is insufficient** — explain why the .NET/ Windows App SDK standard library cannot meet the need.
5. Record the decision in `docs/ARCHITECTURE.md` under "Dependency Decisions."

> **Default position:** Prefer the `System.Text.Markdown` APIs, `Microsoft.Windows.SDK.Contracts`, and built-in .NET libraries. Only add a third-party package when the built-in path is clearly inadequate.

## 4. Never Upgrade .NET or Windows App SDK Automatically

- Upgrades to the .NET SDK, .NET runtime, Windows App SDK, or Windows SDK **must be intentional**.
- Before upgrading: verify compatibility matrices, test the full build, and update `docs/ARCHITECTURE.md` with the new versions and rationale.
- Do not accept `dotnet restore` or `nuget` transitive upgrade suggestions unless the upgrade has been explicitly documented.

## 5. No Modifications to Unrelated Files

- Every change must be scoped to the current task.
- Do not reformat code, rename variables, or "clean up" files that are not part of the active change.
- Each commit should touch **only** the files relevant to one logical change.

## 6. Keep Changes Small and Testable

- Make the smallest change that delivers measurable value.
- Every change to `MDReader.Core` should have a corresponding unit test added or updated.
- Avoid large refactors during feature work; refactor only when tests are green and the refactor is a separate, justified step.

## 7. Build After Meaningful Code Changes

- After any code change that affects compilation, run `dotnet build` from the repository root.
- If the build fails, **fix it before doing anything else**.
- Do not commit or proceed with new work if the build is broken.

## 8. Run Relevant Tests After Changes

- After any change to `MDReader.Core`, run:
  ```bash
  dotnet test tests/MDReader.Core.Tests
  ```
- All tests must pass. If a test fails because the expected behavior changed, update the test with a clear justification.
- Do not delete or skip tests to make the suite green without explicit approval.

## 9. Verify Before Claiming Success

- Every external command (`dotnet build`, `dotnet test`, `winget install`, etc.) must be confirmed by reading its actual output.
- Do not assume a tool succeeded because no error was shown — check exit codes and output.
- Do not fabricate results. If a tool fails, report the failure honestly and propose an alternative.

## 10. Record Important Architectural Decisions

- Maintain `docs/ARCHITECTURE.md` as the single source of truth for technology choices, project boundaries, dependency direction, and design decisions.
- Maintain `docs/ROADMAP.md` with phased delivery plans.
- When a critical decision is made (e.g., choosing a Markdown parser library), record:
  - **What** was decided.
  - **Why** it was chosen over alternatives.
  - **Trade-offs** accepted.
  - **Future impact** on MSIX packaging, Store distribution, or maintenance.

## 11. Preserve Windows-Native Behavior

- The application **must** respect Windows conventions: light/dark theme, accent colors, high-DPI scaling, accessibility (Narrator, keyboard navigation), and RTL layout.
- Do not override system theme behavior without a documented, tested reason.
- Do not bundle a custom window frame that breaks Windows snap, taskbar thumbnail previews, or system title bar behavior.

## 12. Maintain MSIX / Microsoft Store Compatibility

- Keep the project compatible with MSIX packaging at all times.
- Ensure the app manifest declares supported architectures (x64) and supported OSes (Windows 10 1809+).
- Keep the minimum OS target at or above Windows 10 version 1809 (build 17763) as required by the Windows App SDK.
- Do not use Win32 APIs or patterns that are blocked by the Microsoft Store certification process (e.g., `LoadLibrary` for arbitrary code, registry writes to HKLM, processes with `SeDebugPrivilege`).

## 13. Never Place Secrets in Source Code

- No API keys, tokens, passwords, or connection strings in source files.
- If a configuration secret is needed, use the `appsettings.json` + user-secrets pattern or environment variables — and **never commit** secret values.
- This project currently has **no cloud dependencies**, so no secrets are expected.

## 14. Never Delete User Files

- The application must never delete, overwrite, or move the user's `.md` files without explicit confirmation.
- The application must handle missing, locked, or unreadable files gracefully — show an error message, do not crash.

## 15. Logical Git Commit Boundaries

- Each commit must represent a single, coherent logical change.
- Commit messages should follow a clear convention:
  ```
  <type>: <brief summary>

  <detailed explanation if needed>
  ```
  Where `<type>` is one of: `feat`, `fix`, `docs`, `build`, `chore`, `refactor`, `test`.
- Do not commit generated files (`bin/`, `obj/`, `packages/`). Verify `.gitignore` covers these.

## 16. Feature Isolation

- Each new screen, major operation, or independent workflow must have a clear
  **feature owner** directory under `Features/`.
- Features should be internally cohesive — Reader should not directly manipulate
  Settings UI.
- Communication between features happens through contracts, commands, shared
  services, or clearly defined state.
- Only create subdirectories that will contain files. Do not create empty
  placeholder directories.

## 17. AI Control Surface Restrictions

- The `AI/` directory is the architectural control surface for future AI agent
  integration.
- **DO NOT** implement an LLM, OpenAI, OpenRouter, or any AI provider.
- **DO NOT** add network listening, expose ports, or add MCP.
- **DO NOT** automate the graphical UI by clicking buttons — design explicit
  machine-controllable command interfaces instead.
- All AI commands must be validated against `AISecurityPolicy` before execution.
- No AI provider keys, tokens, or credentials should be stored in source.

---

## Developer Setup Checklist

Before writing code, ensure:

- [x] .NET 10 SDK installed (`dotnet --version` returns `10.x`) — installed to `C:\Users\ZAACO\.dotnet` via `dotnet-install.ps1`
- [x] Windows SDK 10.0.26100 provided via `Microsoft.Windows.SDK.BuildTools` NuGet (no system install needed)
- [x] Windows App SDK 2.5.1 (NuGet) resolved via project references
- [x] VS Code C# extension (`ms-dotnettools.csharp`) installed
- [ ] Developer Mode enabled (optional — required only for `dotnet run` debugging; build/test work without it)
- [x] Project builds: `dotnet build`
- [x] Tests pass: `dotnet test`

---

## Review Process

Before opening a PR or committing significant changes:

1. Run `dotnet build` — must succeed with zero errors.
2. Run `dotnet test` — all tests must pass.
3. Review `docs/ARCHITECTURE.md` for any decisions that need updating.
4. Review `.gitignore` coverage for new file types.

---

*These rules were created as part of Phase 0 — Environment and Architecture. They supersede any conflicting instructions in individual files or session history.*
