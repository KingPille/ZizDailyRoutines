using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Extensions;
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
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.CraftGather;

public class AutoEliminateFishAwareness : ModuleBase
{
	private class Config : ModuleConfig
	{
		public HashSet<uint> BlacklistZones = new HashSet<uint>();

		public string ExtraCommands = string.Empty;

		public bool LogoutWhenGlobalWarning;
	}

	private Config config;

	private ZoneSelectCombo zoneSelectCombo;

	private bool isQuestUnlocked;

	private const uint TARGET_CONTENT = 195u;

	private const uint QUEST_ID = 65973u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoEliminateFishAwarenessTitle"),
		Description = LanguageManager.Get("AutoEliminateFishAwarenessDescription"),
		Category = ModuleCategory.CraftGather,
		ModulesPrerequisite = new string[3] { "FieldEntryCommand", "AutoCommenceDuty", "InstantLogout" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		zoneSelectCombo = new ZoneSelectCombo("BlacklistZone");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000,
				ShowDebug = true
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		zoneSelectCombo.SelectedIDs = config.BlacklistZones;
		OmenServiceBase<LogMessageManager>.Instance().RegPost(OnPost);
	}

	protected override void Uninit()
	{
		OmenServiceBase<LogMessageManager>.Instance().Unreg(OnPost);
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		Quest rowOrDefault = LuminaGetter.GetRowOrDefault<Quest>(65973u);
		if (Throttler.Shared.Throttle("AutoEliminateFishAwareness.UpdateQuest"))
		{
			isQuestUnlocked = DalamudServiceExtension.Instance<IUnlockState>().IsQuestCompleted(rowOrDefault);
		}
		using (ImRaiiExtension.Heading1(LanguageManager.Get("PreCondition")))
		{
			ImGui.Bullet();
			ImGui.SameLine();
			GroupDisposable val = ImRaii.Group();
			try
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("AutoEliminateFishAwareness-PreCondition-Quest", new Dictionary<string, object> { ["quest"] = ((Quest)(ref rowOrDefault)).Name })));
				ImGui.SameLine();
				ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)0, KnownColor.LawnGreen.ToUInt(), isQuestUnlocked).Push((ImGuiCol)0, KnownColor.OrangeRed.ToUInt(), !isQuestUnlocked);
				try
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(isQuestUnlocked ? "√" : "X"));
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((GroupDisposable)(ref val)).Dispose();
			}
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked())
			{
				Level level = ((Quest)(ref rowOrDefault)).IssuerLocation.Value;
				(*AgentMap.Instance()).SetMapFlagAndOpen(((Level)(ref level)).Map.RowId, level.GetPosition(), ((object)((Quest)(ref rowOrDefault)).Name/*cast due to constrained. prefix*/).ToString());
			}
		}
		ImGui.NewLine();
		using (ImRaiiExtension.Heading1(LanguageManager.Get("BlacklistZones")))
		{
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			if (zoneSelectCombo.DrawCheckbox())
			{
				config.BlacklistZones = zoneSelectCombo.SelectedIDs;
				config.Save(this);
			}
		}
		ImGui.NewLine();
		using (ImRaiiExtension.Heading1(LanguageManager.Get("AutoEliminateFishAwareness-ExtraCommands"), LanguageManager.Get("AutoEliminateFishAwareness-ExtraCommands-Help")))
		{
			ImGui.InputTextMultiline(ImU8String.op_Implicit("###ExtraCommandsInput"), ref config.ExtraCommands, 2048, default(Vector2), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.ExtraCommands = string.Join("\n", config.ExtraCommands.Split(new string[3] { "\r\n", "\n", "\r" }, StringSplitOptions.None).Take(15));
				config.Save(this);
			}
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoEliminateFishAwareness-LogoutWhenGlobalWarning")), ref config.LogoutWhenGlobalWarning))
		{
			config.Save(this);
		}
	}

	private unsafe void OnPost(uint logMessageID, LogMessageQueueItem item)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (config.BlacklistZones.Contains(GameState.TerritoryType))
		{
			return;
		}
		switch (logMessageID)
		{
		case 5518u:
		{
			string text = LanguageManager.Get("AutoEliminateFishAwareness-Notification-GlobalWarning");
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
			NotifyHelper.ToastError(text);
			OmenServiceBase<NotifyHelper>.Instance().TrayError(text);
			if (config.LogoutWhenGlobalWarning)
			{
				OmenServiceBase<ChatManager>.Instance().SendCommand("/logout");
			}
			break;
		}
		case 3516u:
		case 5517u:
			base.TaskHelper.Abort();
			if (GameState.TerritoryType == 939)
			{
				Vector3 currentPosition = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Position;
				float currentRotation = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Rotation;
				base.TaskHelper.Enqueue((Func<bool>)ExitFishing, "离开钓鱼状态", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				base.TaskHelper.DelayNext(5000, "等待 5 秒");
				base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()), "等待不在钓鱼状态");
				base.TaskHelper.Enqueue(() => ExitDuty(753u), "离开副本");
				base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()) && UIModuleExtension.IsScreenReady() && GameState.TerritoryType != 939, "等待离开副本");
				base.TaskHelper.Enqueue(delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage("/pdrfe diadem");
				}, "发送进入指令");
				base.TaskHelper.Enqueue(() => GameState.TerritoryType == 939 && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null, "等待进入");
				base.TaskHelper.Enqueue(() => OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(currentPosition), $"传送到原始位置 {currentPosition}");
				base.TaskHelper.DelayNext(500, "等待 500 毫秒");
				base.TaskHelper.Enqueue(() => !OmenServiceBase<MovementManager>.Instance().IsManagerBusy, "等待传送完毕");
				base.TaskHelper.Enqueue(delegate
				{
					((BattleChara)IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.ToStruct()).SetRotation(currentRotation);
				}, "设置面向");
			}
			else
			{
				if (IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()))
				{
					break;
				}
				base.TaskHelper.Enqueue((Func<bool>)ExitFishing, "离开钓鱼状态", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				base.TaskHelper.DelayNext(5000, "等待 5 秒");
				base.TaskHelper.Enqueue(() => !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()), "等待离开忙碌状态");
				base.TaskHelper.Enqueue(() => ContentsFinderHelper.RequestDutyNormal(195u, ContentsFinderHelper.DefaultOption), "申请目标副本");
				base.TaskHelper.Enqueue(() => ExitDuty(195u), "离开目标副本");
			}
			base.TaskHelper.Enqueue(() => ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)1, 289u, 3758096384uL, true, true, (uint*)null) == 0, "等待技能抛竿可用");
			base.TaskHelper.Enqueue(delegate
			{
				if (!string.IsNullOrWhiteSpace(config.ExtraCommands))
				{
					OmenServiceBase<ChatManager>.Instance().ExecuteMacro(config.ExtraCommands);
				}
			}, "执行文本指令");
			break;
		}
	}

	private static bool ExitFishing()
	{
		if (!Throttler.Shared.Throttle("AutoEliminateFishAwareness-ExitFishing"))
		{
			return false;
		}
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Fishing, 1u);
		return !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)43];
	}

	private static bool ExitDuty(uint targetContent)
	{
		if (!Throttler.Shared.Throttle("AutoEliminateFishAwareness-ExitDuty"))
		{
			return false;
		}
		if (GameState.ContentFinderCondition != targetContent)
		{
			return false;
		}
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishTerritoryTransport);
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LeaveDuty);
		return true;
	}
}
