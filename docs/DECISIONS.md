# Architecture Decisions — MDReader

## ADR-001: Feature-Based Architecture

**Status:** Accepted
**Date:** 2026-09-27

**Context:** The application needs to evolve from a layer-first structure to
a feature-based structure to support independent development and maintainability.

**Decision:** Adopt feature-based architecture with subfolders per feature
(Shell, Reader, FileOpen, Settings, Theme), each containing its own
Views/, ViewModels/, Services/, and Commands/ as needed.

**Consequences:** Each feature is independently developable. Cross-feature
communication happens through contracts and commands, not direct references.

## ADR-002: Core/App Separation

**Status:** Accepted
**Date:** 2026-09-27

**Context:** The application needs testable logic that doesn't depend on WinUI.

**Decision:** Maintain a strict Core/App split. `MDReader.Core` contains all
logic (documents, markdown processing, contracts). `MDReader.App` depends
on Core. Core NEVER depends on App.

**Consequences:** Core types are unit-testable. App types can reference
Core types but not vice versa.

## ADR-003: AI Control Surface (No AI Integration)

**Status:** Accepted
**Date:** 2026-09-27

**Context:** Future AI agent control is desired, but immediate AI integration
(LLM, API keys, network) is not wanted.

**Decision:** Create the architectural control surface (ICommandGateway,
CommandRequest/CommandResult, AICommandSet, AISecurityPolicy) but do NOT
implement any AI provider or network transport. The AI gateway remains a placeholder.

**Rationale:** Allows future AI integration without fragile UI automation.
Prevents architecting the app into a corner. No external dependencies added.

## ADR-004: NuGet vs System Windows SDK

**Status:** Accepted
**Date:** 2026-09-27

**Context:** winget install of Windows SDK 10.0.26100 fails with "stdin is not a tty"
in background mode. The SDK is needed for compilation.

**Decision:** Use `Microsoft.Windows.SDK.BuildTools` NuGet package (version 10.0.26100.9169)
instead of system-installed Windows SDK. The NuGet package provides all required
Windows headers and libraries for compilation.

**Consequences:** No system Windows SDK required. Builds work on any machine
with the NuGet package restore. No UAC elevation needed.

## ADR-005: .NET SDK Installation Method

**Status:** Accepted
**Date:** 2026-09-27

**Context:** winget install of .NET 10 SDK fails with "stdin is not a tty"
in background mode. The EXE installer hangs on "Launching elevated engine process".

**Decision:** Use `dotnet-install.ps1` script with `-InstallDir` parameter to
install the SDK to a user-local directory (`C:\Users\ZAACO\.dotnet`) without
elevation. A later EXE install succeeded system-wide at `C:\Program Files\dotnet`.

**Consequences:** Both user-local and system-level SDKs are available.
`global.json` pins SDK version to 10.0.401.

## ADR-006: MSIX Package Identity

**Status:** Accepted
**Date:** 2026-09-27

**Context:** The template-generated Package.appxmanifest uses a default GUID
for the package identity, and DisplayName="MDReader.App" for the application.

**Decision:** Change Identity Name to "MDReader.MDReader" (domain-style,
required for MSIX). Update DisplayName to "MDReader". Use generated GUIDs
for PhoneIdentity ProductId.

**Consequences:** Package is uniquely identifiable. Display name is consistent
with the product name.

## ADR-007: WinUI Code-Behind Namespace Mapping

**Status:** Accepted
**Date:** 2026-09-27

**Context:** The WinUI template generated code in the root `MDReader.App` namespace.
Feature-based architecture requires per-feature namespaces.

**Decision:** Map each XAML file's x:Class and code-behind namespace to its feature directory:
- MainWindow → `MDReader.App.Features.Shell.Views`
- MainPage → `MDReader.App.Features.Reader.Views`
- App.xaml → stays at root `MDReader.App` (application entry point)

**Consequences:** XAML and code-behind types are in the correct feature namespace.
The App.xaml.cs root namespace remains unchanged for application entry point.
