using System;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoNotifyCutsceneEnd : ModuleBase
{
	private long cutsceneBeginTick;

	private long cutsceneLastWatchingTick;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyCutsceneEndTitle"),
		Description = LanguageManager.Get("AutoNotifyCutsceneEndDescription"),
		Category = ModuleCategory.Duty
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		ClearResources();
	}

	private unsafe void OnZoneChanged(uint zone)
	{
		ClearResources();
		if (GameState.ContentFinderCondition == 0 || GameState.IsInPVPArea)
		{
			return;
		}
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || LocalPlayerState.Object == null)
			{
				return false;
			}
			if (((Group)(&((GroupManager)GroupManager.Instance()).MainGroup)).MemberCount < 2)
			{
				base.TaskHelper.Abort();
				return true;
			}
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_PartyList", new AddonEventDelegate(OnAddon));
			return true;
		}, "检查是否需要开始监控");
	}

	private unsafe void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		if ((int)flag == 26 && GameState.ContentFinderCondition != 0 && !GameState.IsInPVPArea && ((Group)(&((GroupManager)GroupManager.Instance()).MainGroup)).MemberCount >= 2 && !value && !GameState.IsDutyCompleted)
		{
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_PartyList", new AddonEventDelegate(OnAddon));
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		AgentHUD* ptr = AgentHUD.Instance();
		if (ptr == null)
		{
			return;
		}
		if (GameState.ContentFinderCondition == 0 || GameState.IsInPVPArea || GameState.IsDutyCompleted || ((Group)(&((GroupManager)GroupManager.Instance()).MainGroup)).MemberCount < 2)
		{
			ClearResources();
		}
		else
		{
			if (LocalPlayerState.Object == null)
			{
				return;
			}
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
			{
				if (cutsceneBeginTick != 0L)
				{
					RelayCutsceneEnd();
				}
				DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
				return;
			}
			long tickCount = Environment.TickCount64;
			if (IsAnyPartyMemberWatchingCutscene(ptr))
			{
				if (cutsceneBeginTick == 0L)
				{
					cutsceneBeginTick = tickCount;
				}
				cutsceneLastWatchingTick = tickCount;
			}
			else if (cutsceneBeginTick != 0L && tickCount - cutsceneLastWatchingTick >= 1000)
			{
				RelayCutsceneEnd();
			}
		}
	}

	private void RelayCutsceneEnd()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan timeSpan = TimeSpan.FromMilliseconds(cutsceneLastWatchingTick - cutsceneBeginTick);
		cutsceneBeginTick = 0L;
		cutsceneLastWatchingTick = 0L;
		if (!(timeSpan < TimeSpan.FromSeconds(4L)))
		{
			OmenServiceBase<NotifyHelper>.Instance().TrayInfo(LanguageManager.Get("AutoNotifyCutsceneEnd-Notification"));
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoNotifyCutsceneEnd-Message", new Dictionary<string, object> { ["seconds"] = (int)timeSpan.TotalSeconds })), useDefaultPrefix: false);
		}
	}

	private unsafe static bool IsAnyPartyMemberWatchingCutscene(AgentHUD* agent)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (agent == null)
		{
			return false;
		}
		ref Group reference = ref ((GroupManager)GroupManager.Instance()).MainGroup;
		if (reference.MemberCount < 2)
		{
			return false;
		}
		for (int i = 0; i < reference.MemberCount; i++)
		{
			if ((((Group)(ref reference)).PartyMembers[i].Flags & 0x10) != 0)
			{
				return true;
			}
		}
		Span<HudPartyMember> partyMembers = ((AgentHUD)agent).PartyMembers;
		for (int j = 0; j < partyMembers.Length; j++)
		{
			HudPartyMember val = partyMembers[j];
			if (val.EntityId != 0 && val.ContentId != 0L)
			{
				if (val.Object == null)
				{
					return true;
				}
				if (!GameState.IsDutyStarted && !((BattleChara)val.Object).GetIsTargetable())
				{
					return true;
				}
				if (((BattleChara)val.Object).OnlineStatus == 15)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void ClearResources()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		base.TaskHelper?.Abort();
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		cutsceneBeginTick = 0L;
		cutsceneLastWatchingTick = 0L;
	}
}
