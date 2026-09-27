# Product Specification — MDReader

## Purpose

MDReader is a Markdown viewer for Windows 11. It opens `.md` files and renders
them as formatted, readable text — not as source code. The application targets
knowledge workers, developers, and students who need to quickly preview Markdown
content without opening an editor.

## MVP Feature Set

| Feature | Description |
|---|---|
| Launch | Application starts and displays a welcome state |
| Open .md file | File → Open dialog, drag-and-drop, or `mdreader.exe <path>` CLI |
| Render Markdown | Display formatted content (headings, lists, code blocks, tables, links) |
| Arabic & English | RTL and LTR layout support, Arabic font rendering |
| Scrolling | Smooth scroll, mouse wheel + touch gesture support |
| Links | Clickable links (internal anchors + external URLs) |
| Local images | Inline images referenced by relative path (safe subset) |
| Light/Dark theme | Follows Windows system theme; manual toggle in Settings |
| Error handling | Graceful fallback for unreadable files or parse errors |

## Out of Scope (NOT MVP)

- Markdown editing (viewer only — no editor)
- Accounts or cloud sync
- Database or local storage beyond temp cache
- Collaboration or comments
- AI chatbot or LLM features
- Telemetry or analytics
- Plugin marketplace

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| UI Framework | WinUI 3 / Windows App SDK 2.5.1 |
| Target | Windows 10 20H2+ / Windows 11 |
| Build | Windows SDK BuildTools 10.0.26100 (NuGet) |
| Markdown Parsing | Markdig (planned, Phase 2) |
| Packaging | MSIX / APPX |
| Editor | VS Code + C# extension |

## Architecture Summary

- **Feature-based**: Code organized by user-facing features (Shell, Reader, FileOpen, Settings, Theme)
- **MVVM**: ViewModels separate presentation logic from UI (fully implemented when CommunityToolkit.Mvvm is added in Phase 1)
- **Core/App split**: Pure .NET logic in `MDReader.Core`; WinUI 3 UI in `MDReader.App`
- **AI Control Surface**: Provider-neutral command interface in `MDReader.App/AI/` — architectural foundation only, no AI provider integration (no LLM, no API keys, no network)

## Independent Project Policy

MDReader is an independent project under `D:\AI_PROJECTS\MDReader\`. It has its
own Git repository, solution, source, tests, documentation, and release lifecycle.
It does NOT share code with other applications (SnapDrop, DayFlow, etc.) unless
real duplication is demonstrated across multiple applications.
