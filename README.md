# MDReader

A small, fast, and reliable native Windows application for reading Markdown (`.md`) files.

Built with **WinUI 3** and the **Windows App SDK** on **.NET 10**, using **Visual Studio Code** as the development environment.

## Features

- Open and read Markdown (`.md`) files
- Render Markdown clearly with GitHub Flavored Markdown support
- Basic scrolling
- Light/Dark theme consistent with Windows
- Graceful handling of invalid/missing/unreadable files
- MSIX-packaged for native Windows deployment

*See [docs/ROADMAP.md](docs/ROADMAP.md) for the complete phased feature plan.*

## Prerequisites

| Tool | Minimum Version | Install Command |
|------|-----------------|-----------------|
| Windows | 10 version 1809 (build 17763) | N/A |
| .NET SDK | 10.0.401 | `dotnet-install.ps1 -Channel 10.0 -InstallDir $env:USERPROFILE\.dotnet` |
| Windows SDK | 10.0.26100 (Optional) | `winget install Microsoft.WindowsSDK.10.0.26100` — or rely on `Microsoft.Windows.SDK.BuildTools` NuGet package |
| Git | 2.40+ | `winget install Git.Git` |
| VS Code | 1.80+ | `winget install Microsoft.VisualStudioCode` |
| C# Extension | Latest | `code --install-extension ms-dotnettools.csharp` |
| Developer Mode | Enabled | `ms-settings:developers` |

> The Windows App SDK runtime (versions 1.x and 2.x) is provided via NuGet package references and does not require a separate runtime install on development machines. It is distributed automatically with the app package or via the Microsoft Store.

## Build and Run

```powershell
# Clone the repository
git clone <repository-url>
cd MDReader

# Restore NuGet packages
dotnet restore

# Build the solution
dotnet build

# Run the application
dotnet run --project src/MDReader.App
```

## Test

```bash
dotnet test
```

## Project Structure

```
MDReader/
├── src/
│   ├── MDReader.App/              — WinUI 3 UI, Windows integration
│   └── MDReader.Core/             — Markdown logic, document models, business logic
├── tests/
│   └── MDReader.Core.Tests/       — Automated tests for Core
├── docs/
│   ├── ARCHITECTURE.md            — Technology choices and design decisions
│   ├── DEVELOPMENT_RULES.md       — Rules for all future agents
│   └── ROADMAP.md                 — Phased delivery plan
├── .vscode/                       — VS Code config (settings, tasks, launch)
├── global.json                    — SDK version pinning
├── .gitignore
└── MDReader.sln
```

## Development Rules

All developers and AI agents working on this project **must** follow the rules in [docs/DEVELOPMENT_RULES.md](docs/DEVELOPMENT_RULES.md). Key points:

1. Never change the architecture silently
2. Never add a NuGet dependency without documenting its purpose, version, license, and justification
3. Never upgrade .NET or Windows App SDK automatically
4. Build after meaningful code changes
5. Run tests after changes
6. Keep each change small and testable
7. Never place secrets in source code
8. Maintain MSIX / Microsoft Store compatibility

## Licensing

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

## Contributing

See [docs/DEVELOPMENT_RULES.md](docs/DEVELOPMENT_RULES.md) for contribution guidelines.
