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

## 12. Maintain MSIX / Microsoft Store Compatibility & Policies

- Keep the project strictly compliant with Microsoft Store and Windows App certification requirements at all times:
  - **Store Identity Alignment**:
    - **Package Name**: `birotech.JMDReader`
    - **Publisher**: `CN=4B6200B3-DE4E-4F39-A6EA-B5DC049294B2`
    - **PublisherDisplayName**: `birotech`
    - **Package Family Name (PFN)**: `birotech.JMDReader_4spa8h579ffz6`
  - **Multi-Architecture Support**:
    - Every Store release (v1.0.2, v2.0, etc.) **must build and package native binaries for both `x64` and `arm64` (AArch64)** to ensure native performance on Intel/AMD and Snapdragon / Copilot+ PC devices.
  - **MSIX Sandbox & Storage Isolation (WebView2 Protection)**:
    - Because MSIX applications execute with a read-only install directory (`C:\Program Files\WindowsApps\...`), **WebView2 must never use the default userDataFolder**.
    - All WebView2 instances must initialize with an isolated, writable user data folder in `%LocalAppData%\JMDReader\WebView2Data` via `CoreWebView2Environment.CreateWithOptionsAsync` to prevent fatal `0x8000FFFF / 0xc000027b` crashes.
  - **Capability & Privacy Strictness**:
    - Restricted capability `runFullTrust` is declared exclusively for native WinUI 3 desktop file operations and local system integration.
    - Zero telemetry, zero analytics, zero external network calls: 100% offline, local-first execution.
  - **XAML Theme Fallback**:
    - All Resource Dictionaries must define `<ResourceDictionary x:Key="Default">` fallback dictionaries alongside `Light` and `Dark` to avoid missing brushes or crash on custom Windows themes.
  - **Store Version Synchronization**:
    - Keep `Package.appxmanifest`, `setup.iss`, and git tags synchronized on 4-part semantic versioning (`1.0.2.0`, `2.0.0.0`).

---

## 13. Dual Task Registration & Tracking Policy (Linear.app & TASKS.md)

**Mandatory for all AI agents and human developers:**

1. **Every New Development Item Must Be Tracked**:
   - Any new feature, enhancement, architectural refactoring, bug fix, or Store submission step **must be recorded in both**:
     - `.project/TASKS.md` (Local project control layer)
     - **Linear.app** (`linear.app/jmdreader` workspace, team `JMDReader`)
2. **Task Synchronization Lifecycle**:
   - **Backlog / Todo**: Identified requirement or future enhancement (e.g. Phase 2 paste handling `JMD-8`).
   - **In Progress**: Actively being coded, tested, or packaged.
   - **Done / Completed**: Verified with tests (`dotnet test`), clean build (`dotnet build`), and artifacts generated.
3. **Traceability in Commits**:
   - Git commits and pull requests must reference the Linear Issue ID (e.g. `JMD-5`, `JMD-6`, `JMD-7`) and the Phase Task ID (e.g. `P20-T005`).
4. **No Ghost Features**:
   - Writing code or introducing dependencies without a prior registered task in both Linear and `TASKS.md` is strictly prohibited.

---

## 14. Never Place Secrets in Source Code

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

## 18. Project Control Lifecycle

Every change to MDReader follows the task lifecycle defined in the Project Control
Layer (`.project/`):

1. **Read control layer first** — every session starts by reading
   `.project/PROJECT.md`, `.project/STATE.json`, `.project/TASKS.md`, and
   `.project/CHANGELOG.md` before writing any code.
2. **Select only READY tasks** — start only from `.project/TASKS.md`. A task is
   READY only when all its dependencies are DONE. Do not skip or reorder phases.
3. **Do not implement Phase 1 in a Phase 0 control-layer session** — creating
   `.project/` files is itself the work; implementation tasks start only after
   the control layer is committed and marked READY.
4. **Verify before DONE** — a task is DONE only when `dotnet build`, `dotnet test`,
   and the task's acceptance criteria all pass. If runtime verification is
   impossible (e.g., Developer Mode disabled), record the limitation — do not
   falsely claim runtime verification passed.
5. **Commit coherently** — each commit maps to one verified task. Commit messages
   include the task ID (e.g., `P01-T001: add CommunityToolkit.Mvvm dependency`).
6. **Update state atomically** — after each DONE task, update `.project/TASKS.md`,
   `.project/STATE.json`, and `.project/CHANGELOG.md` together before committing.

---

## 11. Zero-Crash Architecture & Anti-Hang Guidelines (قواعد الحماية من الانهيار والتهنيج)

To guarantee 100% stability, zero process exits, and silky-smooth responsiveness under all conditions, all developers and AI agents must adhere to the following rules:

### 11.1 The Zero-Crash Mandate (منع الانهيار والإغلاق المفاجئ)
- **Global Exception Boundaries:** `App.UnhandledException`, `TaskScheduler.UnobservedTaskException`, and `AppDomain.CurrentDomain.UnhandledException` must always be trapped with `e.Handled = true` / `e.SetObserved()` and diagnostic details logged to `crash.log`. No unhandled exception may terminate the process.
- **Fail-Safe UI:** In the event of a recoverable component failure, display an in-app error notification (`ShowError` banner or InfoBar) and keep the application running. Never exit abruptly.

### 11.2 The Strict `async void` Ban (حظر async void)
- **Rule:** Never author `async void` methods. Use `async Task` for all asynchronous workflows.
- **UI Event Handler Exception:** WinUI 3 event handlers (such as `Click`, `KeyDown`, `Drop`) require a `void` return signature. Any `async void` event handler **MUST** enclose its entire method body within a comprehensive `try { ... } catch (Exception ex) { ... }` block. In .NET, an unhandled exception escaping an `async void` method bypasses all catch blocks up the call stack and immediately terminates the process (fail-fast crash).

### 11.3 Anti-Hang & Responsiveness Standards (منع التهنيج وتجمد الواجهة)
- **Debounced Rendering:** Rapid keystrokes and scroll events must be throttled/debounced using `DispatcherTimer` (e.g., 180ms delay) to avoid choking the rendering pipeline during fast user input.
- **ReDoS Protection:** All regular expression evaluations must specify an explicit execution timeout: `TimeSpan.FromSeconds(1)`. Catch `RegexMatchTimeoutException` gracefully to prevent thread starvation.
- **Asynchronous File I/O:** File reads and writes must never block the UI thread. Use `await File.ReadAllTextAsync` or background worker tasks for large payloads.

### 11.4 Out-of-Process / WebView2 Isolation (حماية محرك العرض)
- `CoreWebView2` runs in an independent child process (`msedgewebview2.exe`).
- All calls to `PostWebMessageAsJson`, `ExecuteScriptAsync`, and virtual host mappings must be guarded with `try/catch`. If WebView2 crashes, restarts, or navigates, the host WinUI 3 shell must remain completely stable.

### 11.5 Safe Atomic File Operations & Non-Locking Sharing (أمان الملفات)
- **Non-Locking Reads:** Always open files with `FileShare.ReadWrite` so concurrent tools (Git, external text editors, cloud sync) do not block or crash MDReader.
- **Atomic Writes:** Save files via temporary staging files (`.tmp`) followed by atomic replace/rename to prevent file truncation or corruption if power is interrupted.

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
