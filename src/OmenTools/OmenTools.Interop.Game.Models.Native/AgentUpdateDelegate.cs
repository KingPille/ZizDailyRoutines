using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace OmenTools.Interop.Game.Models.Native;

public unsafe delegate void AgentUpdateDelegate(AgentInterface* agent, uint frameCount);
