// MIT License - see LICENSE file in the repository root.

using MDReader.Core.Contracts;

namespace MDReader.App.AI.Gateway;

/// <summary>
/// Placeholder for the AI control gateway.
/// This class will be implemented in a future session when AI integration
/// is explicitly approved. It will connect the AI command layer to the
/// application command gateway (ICommandGateway).
/// </summary>
/// <remarks>
/// The gateway will:
/// 1. Receive commands from an AI agent (via MCP, CLI, IPC, named pipes, etc.)
/// 2. Validate commands against AISecurityPolicy
/// 3. Delegate to ICommandGateway.Execute()
/// 4. Return results to the AI agent
///
/// DO NOT implement an LLM. DO NOT add OpenAI. DO NOT add Hermes SDK.
/// DO NOT add network listening. DO NOT expose a port. DO NOT add MCP yet.
/// See docs/AI_INTERFACE.md for the design and docs/DECISIONS.md for rationale.
/// </remarks>
public sealed class AIGateway
{
    // TODO: Implement in a future session with explicit approval.
    // The gateway will receive commands from an AI agent via a controlled
    // transport (MCP, CLI, IPC, Named Pipes) and delegate to ICommandGateway.
    // No AI provider integration (OpenAI, Claude, Gemini, OpenRouter) should
    // be added without explicit architectural approval.
}
