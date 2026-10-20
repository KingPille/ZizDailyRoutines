using System;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class NoAutoClosePartyFinder : ModuleBase
{
	private unsafe delegate void LookingForGroupHideDelegate(AgentLookingForGroup* agent);

	private static readonly CompSig LookingForGroupHideSig = new CompSig("48 89 5C 24 ?? 57 48 83 EC 20 83 A1 ?? ?? ?? ?? ??");

	private Hook<LookingForGroupHideDelegate>? LookingForGroupHideHook;

	private DateTime lastPartyMemberChangeTime;

	private DateTime lastViewTime;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("NoAutoClosePartyFinderTitle"),
		Description = LanguageManager.Get("NoAutoClosePartyFinderDescription"),
		Category = ModuleCategory.Recruitment,
		Author = new string[2] { "Nyy", "YLCHEN" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		LookingForGroupHideHook = LookingForGroupHideSig.GetHook<LookingForGroupHideDelegate>(LookingForGroupHideDetour);
		LookingForGroupHideHook.Enable();
		OmenServiceBase<LogMessageManager>.Instance().RegPre(OnPreReceiveMessage);
	}

	protected override void Uninit()
	{
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnPreReceiveMessage);
	}

	private unsafe void OnPreReceiveMessage(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem values)
	{
		if (logMessageID == 947)
		{
			isPrevented = true;
			lastPartyMemberChangeTime = OmenServiceBase<StandardTimeManager>.Instance().UTCNow.AddSeconds(1.0);
			if ((*Addons.LookingForGroupDetail).IsAddonAndNodesReady())
			{
				lastViewTime = OmenServiceBase<StandardTimeManager>.Instance().UTCNow.AddSeconds(1.0);
			}
		}
	}

	private unsafe void LookingForGroupHideDetour(AgentLookingForGroup* agent)
	{
		if (OmenServiceBase<StandardTimeManager>.Instance().UTCNow < lastPartyMemberChangeTime)
		{
			if (OmenServiceBase<StandardTimeManager>.Instance().UTCNow < lastViewTime)
			{
				if ((*Addons.LookingForGroupDetail).IsAddonAndNodesReady())
				{
					((AtkUnitBase)Addons.LookingForGroupDetail).Close(true);
				}
				DalamudServiceExtension.Instance<IFramework>().RunOnTick<bool>((Func<bool>)(() => ((AgentLookingForGroup)agent).OpenListing(((Detailed)(&((AgentLookingForGroup)agent).LastViewedListing)).ListingId)), TimeSpan.FromMilliseconds(100L), 0, default(CancellationToken));
			}
		}
		else
		{
			LookingForGroupHideHook.Original(agent);
		}
	}
}
