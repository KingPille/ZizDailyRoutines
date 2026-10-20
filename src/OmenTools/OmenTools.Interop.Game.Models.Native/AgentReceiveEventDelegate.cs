using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace OmenTools.Interop.Game.Models.Native;

public unsafe delegate AtkValue* AgentReceiveEventDelegate(AgentInterface* agent, AtkValue* returnValues, AtkValue* values, uint valueCount, ulong eventKind);
