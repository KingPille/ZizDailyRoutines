using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class AutoUnlockMapDiscoverZone : ModuleBase
{
	private unsafe delegate void AgentMapUpdateDelegate(AgentMap* agent, uint updateCount);

	private static readonly CompSig AgentMapUpdateSig = new CompSig("48 89 5C 24 ?? 55 56 57 41 54 41 55 41 56 41 57 48 83 EC ?? 48 8B E9 E8");

	private Hook<AgentMapUpdateDelegate>? AgentMapUpdateHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoUnlockMapDiscoverZoneTitle"),
		Description = LanguageManager.Get("AutoUnlockMapDiscoverZoneDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		if (AgentMapUpdateHook == null)
		{
			AgentMapUpdateHook = AgentMapUpdateSig.GetHook<AgentMapUpdateDelegate>(AgentMapUpdateDetour);
		}
		AgentMapUpdateHook.Enable();
	}

	private unsafe void AgentMapUpdateDetour(AgentMap* agent, uint updateCount)
	{
		((AgentMap)agent).CurrentMapDiscoveryFlag = 0u;
		((AgentMap)agent).SelectedMapDiscoveryFlag = 0u;
		AgentMapUpdateHook.Original(agent, updateCount);
		((AgentMap)agent).CurrentMapDiscoveryFlag = 0u;
		((AgentMap)agent).SelectedMapDiscoveryFlag = 0u;
	}
}
