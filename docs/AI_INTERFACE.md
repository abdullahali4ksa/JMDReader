# AI Interface — MDReader

## Purpose

This document defines the machine-controllable command layer that allows an AI
agent (e.g., Hermes) to control MDReader operations without fragile UI automation.

## Command Model

Commands flow through three layers:

```
AI Agent
   │
   │ (transport: MCP, CLI, IPC, Named Pipes — future)
   ▼
AI Control Gateway (AI/Gateway/)
   │
   │ (security validation via AI/Security/)
   ▼
Application Commands (Core/Contracts/ICommandGateway)
   │
   ▼
Application Services (Reader, FileOpen, Theme)
```

### Request/Response Format

**CommandRequest**
| Field | Type | Description |
|---|---|---|
| Command | string | Command name (e.g., "OpenDocument") |
| Parameters | Dictionary<string, object?> | Command parameters |
| RequestId | string? | Correlation ID |

**CommandResult**
| Field | Type | Description |
|---|---|---|
| Success | bool | Whether the command succeeded |
| Data | string? | Result data (typically JSON) |
| Error | string? | Error message if failed |
| RequestId | string? | Echoed from request |

## Allowlisted Commands

| Command | Parameters | Description |
|---|---|---|
| OpenDocument | path: string | Opens a Markdown file |
| CloseDocument | — | Closes the current document |
| GetCurrentDocument | — | Returns current document content |
| GetDocumentMetadata | — | Returns file metadata |
| ChangeTheme | theme: string | Changes theme (light/dark/system) |
| NavigateTo | feature: string | Navigates to a feature |
| GetApplicationState | — | Returns app state as JSON |
| ListSupportedCommands | — | Lists all command names |

## Blocked Operations

AI must NEVER have automatic access to:

| Operation | Reason |
|---|---|
| ExecutePowerShell | Arbitrary code execution |
| DeleteFile | Data destruction |
| RunExecutable | Process execution |
| ReadCredentials | Secret access |
| WriteArbitraryFile | File system corruption |
| ModifyRegistry | System modification |
| NetworkControl | Port binding, firewall changes |

## Trust Boundary

```
┌─────────────────────────────────────┐
│         AI Agent (External)         │
│         (Hermes / MCP / CLI)        │
├─────────────────────────────────────┤
│  AI Control Gateway                 │
│  (validates against allowlist)      │
├─────────────────────────────────────┤
│  Application Commands (Core)        │
│  (provider-neutral, safe ops)       │
├─────────────────────────────────────┤
│  Application Features               │
│  (Reader, FileOpen, Settings, etc.)│
└─────────────────────────────────────┘
```

The AI Control Gateway is the only entry point for AI. It validates
every command against `AISecurityPolicy.AllowedCommands` before
delegating to `ICommandGateway`.

## Future Transport Options

When AI integration is explicitly approved, the following transports may be used:

1. **MCP (Model Context Protocol)** — Primary transport for Hermes
2. **CLI** — `mdreader.exe --command OpenDocument --param path=<path>`
3. **IPC / Named Pipes** — For local agent communication
4. **HTTP** — For remote control (with auth, not default)

## Authentication Considerations

- AI access must require explicit user opt-in
- All AI commands should be logged (audit trail)
- File open commands should be restricted to user-chosen directories
- No credentials should be exposed through the AI interface

## Current Status

**NO external AI access is implemented.** We have only created the architectural
control surface — interfaces, command registry, and security policy. See
`src/MDReader/App/AI/` for the placeholder gateway and contracts.

No LLM, no OpenAI, no OpenRouter, no Hermes SDK, no network listening, no port exposure.
