# Feature Map — MDReader

This is the authoritative map of application capabilities. Each feature has a
clear owner directory under `src/MDReader/App/Features/` or `src/MDReader/App/AI/`.

## Core Library (`src/MDReader.Core/`)

| Feature | Directory | Purpose | Responsible |
|---|---|---|---|
| MarkdownDocument | `Markdown/` | Document model (source text, metadata) | MDReader.Core |
| DocumentInfo | `Documents/` | Document metadata (file path, size, modified) | MDReader.Core |
| ICommandGateway | `Contracts/` | Provider-neutral command processing | MDReader.Core |
| CommandRequest | `Contracts/` | Request model (command name, parameters, requestId) | MDReader.Core |
| CommandResult | `Contracts/` | Result model (success, data, error) | MDReader.Core |

## Application Features (`src/MDReader.App/Features/`)

| Feature | Directory | Purpose | Owner |
|---|---|---|---|
| Shell | `Features/Shell/` | Application window, title bar, navigation frame | MDReader.App |
| Reader | `Features/Reader/` | Markdown rendering, scrolling, links | MDReader.App |
| FileOpen | `Features/FileOpen/` | File dialog, path validation, open command | MDReader.App |
| Settings | `Features/Settings/` | Theme, language, preferences | MDReader.App |
| Theme | `Features/Theme/` | Light/dark/system theme management | MDReader.App |

## AI Control Surface (`src/MDReader.App/AI/`)

| Component | Directory | Purpose | Owner |
|---|---|---|---|
| IAICommandHandler | `AI/Contracts/` | AI-facing command handler interface | MDReader.App |
| CommandMetadata | `AI/Contracts/` | Metadata for each AI command | MDReader.App |
| AICommandSet | `AI/Commands/` | Registry of available commands | MDReader.App |
| AIGateway | `AI/Gateway/` | Future AI transport adapter (placeholder) | MDReader.App |
| AISecurityPolicy | `AI/Security/` | Security policy (allowlist/blocklist) | MDReader.App |

## Shared Components (`src/MDReader.App/Shared/`)

| Component | Directory | Purpose | Owner |
|---|---|---|---|
| Styles | `Shared/Styles/` | Resource dictionaries, theme resources | MDReader.App |
| Converters | `Shared/Converters/` | XAML value converters | MDReader.App |
| Helpers | `Shared/Helpers/` | Utility classes (PathHelper, etc.) | MDReader.App |
| Controls | `Shared/Controls/` | Custom XAML controls (future) | MDReader.App |

## Cross-Feature Communication

Features communicate through:
1. **Contracts** — Shared interfaces defined in `MDReader.Core.Contracts`
2. **Commands** — Application command layer (`ICommandGateway`)
3. **State** — Clearly defined application state passed through commands
4. **Events** — (Future) Event aggregation for decoupled notifications

Features do NOT directly reference each other's XAML or ViewModels.
For example, `FileOpen` communicates with `Reader` through commands, not
by directly manipulating `Reader`'s XAML.
