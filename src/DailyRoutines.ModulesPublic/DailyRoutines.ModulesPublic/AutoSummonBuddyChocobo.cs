using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoSummonBuddyChocobo : ModuleBase
{
	private enum ChocoboStance
	{
		FreeStance = 4,
		DefenderStance,
		AttackerStance,
		HealerStance
	}

	private class Config : ModuleConfig
	{
		public bool AutoSwitchStance;

		public bool NotBattleJobUsingGysahl;

		public bool SendChat;

		public ChocoboStance Stance = ChocoboStance.FreeStance;
	}

	private Config config;

	private bool hasNotifiedInCurrentZone;

	private const uint GYSAHL_GREENS_ITEM_ID = 4868u;

	private const uint BUDDY_UNLOCK_QUEST_ID = 66698u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSummonBuddyChocoboTitle"),
		Description = LanguageManager.Get("AutoSummonBuddyChocoboDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "Veever" }
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		Cleanup();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoSummonBuddyChocobo-AutoSwitchStance")), ref config.AutoSwitchStance))
		{
			config.Save(this);
		}
		if (config.AutoSwitchStance)
		{
			IndentDisposable val = ImRaii.PushIndent(1, true);
			try
			{
				bool flag = true;
				ChocoboStance[] values = Enum.GetValues<ChocoboStance>();
				foreach (ChocoboStance chocoboStance in values)
				{
					if (LuminaGetter.TryGetRow<BuddyAction>((uint)chocoboStance, out BuddyAction item))
					{
						if (!flag)
						{
							ImGui.SameLine();
						}
						flag = false;
						if (ImGui.RadioButton(ImU8String.op_Implicit(((object)((BuddyAction)(ref item)).Name/*cast due to constrained. prefix*/).ToString()), config.Stance == chocoboStance))
						{
							config.Stance = chocoboStance;
							config.Save(this);
						}
					}
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoSummonBuddyChocobo-NotBattleJobUsingGys")), ref config.NotBattleJobUsingGysahl))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
		{
			config.Save(this);
		}
	}

	private void OnZoneChanged(uint u)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		Cleanup();
		if (IsZoneValid())
		{
			OmenServiceBase<LocalPlayerState>.Instance().PlayerMoveStateChanged += OnPlayerMoving;
			DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
			OmenServiceBase<LogMessageManager>.Instance().RegPost(OnLogMessage);
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)flag == 4)
		{
			if (!IsZoneValid())
			{
				Cleanup();
			}
			else if (!value)
			{
				OnPlayerMoving(state: false);
			}
		}
	}

	private void OnLogMessage(uint logMessageID, LogMessageQueueItem item)
	{
		if (logMessageID == 1328)
		{
			if (!IsZoneValid())
			{
				Cleanup();
			}
			else
			{
				OnPlayerMoving(state: false);
			}
		}
	}

	private unsafe void OnPlayerMoving(bool state)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (!IsZoneValid())
		{
			Cleanup();
		}
		else if (state)
		{
			base.TaskHelper.Abort();
		}
		else
		{
			if (!QuestManager.IsQuestComplete(66698u))
			{
				return;
			}
			IPlayerCharacter playerCharacter = LocalPlayerState.Object;
			if (playerCharacter == null || playerCharacter.IsDead || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) || IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
			{
				return;
			}
			if (!config.NotBattleJobUsingGysahl)
			{
				ClassJob classJobData = LocalPlayerState.ClassJobData;
				if (((ClassJob)(ref classJobData)).DohDolJobIndex != -1)
				{
					return;
				}
			}
			CompanionInfo companionInfo = ((Buddy)(&((UIState)UIState.Instance()).Buddy)).CompanionInfo;
			if (companionInfo.TimeLeft > 300f)
			{
				if (config.AutoSwitchStance && (ChocoboStance)companionInfo.ActiveCommand != config.Stance)
				{
					base.TaskHelper.Enqueue(() => OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)6, (uint)config.Stance, 3758096384uL, 0u, (UseActionMode)0));
				}
			}
			else if (config.SendChat && LocalPlayerState.GetItemCount(4868u) <= 3)
			{
				if (!hasNotifiedInCurrentZone)
				{
					hasNotifiedInCurrentZone = true;
					string text = LanguageManager.Get("AutoSummonBuddyChocobo-NotificationMessage");
					OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
					NotifyHelper.ToastError(text);
				}
			}
			else
			{
				base.TaskHelper.DelayNext(500);
				base.TaskHelper.Enqueue(delegate
				{
					OmenServiceBase<UseActionManager>.Instance().UseActionLocation((ActionType)2, 4868u, 3758096384uL, default(Vector3), 65535u, 0);
				});
			}
		}
	}

	private void Cleanup()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		OmenServiceBase<LocalPlayerState>.Instance().PlayerMoveStateChanged -= OnPlayerMoving;
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnLogMessage);
		base.TaskHelper?.Abort();
		hasNotifiedInCurrentZone = false;
	}

	private unsafe static bool IsZoneValid()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if (!GameState.IsInPVPArea && (int)GameState.TerritoryIntendedUse == 1)
		{
			return !((PlayerState)PlayerState.Instance()).IsPlayerStateFlagSet((PlayerStateFlag)2);
		}
		return false;
	}
}
