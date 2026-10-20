using System;
using System.Linq;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.UI.Abstractions;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class QueueCombatTeleport : ModuleBase
{
	private class Config : ModuleConfig
	{
		public int Delay = 500;

		public bool SendChat = true;

		public bool SendNotification = true;
	}

	private enum QueueTeleportNotifyType
	{
		Save,
		Execute,
		Clear
	}

	private static readonly CompSig CanUseTeleportSig = new CompSig("84 C0 0F 84 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? 48 89 5C 24");

	private MemoryPatch canUseTeleportPatch;

	private static readonly CompSig CanUseTeleportMapSig = new CompSig("84 C0 0F 44 CA 8B C1 48 83 C4 ?? C3 CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC");

	private MemoryPatch canUseTeleportMapPatch;

	private Config? config;

	private (uint ID, uint SubID)? queuedTeleport;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("QueueCombatTeleportTitle"),
		Description = LanguageManager.Get("QueueCombatTeleportDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		canUseTeleportPatch = new MemoryPatch(CanUseTeleportSig.Get(), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[2]
		{
			(byte)176,
			(byte)1
		}));
		canUseTeleportMapPatch = new MemoryPatch(CanUseTeleportMapSig.Get(), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[5]
		{
			(byte)8,
			(byte)192,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 60000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		canUseTeleportPatch.Enable();
		canUseTeleportMapPatch.Enable();
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(OnPreUseCommand);
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnPreUseCommand);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
		queuedTeleport = null;
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref config.SendNotification))
		{
			config.Save(this);
		}
		ImGui.SameLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
		{
			config.Save(this);
		}
		ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
		if (ImGui.InputInt(ImU8String.op_Implicit(LanguageManager.Get("Delay")), ref config.Delay, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0))
		{
			config.Delay = Math.Max(0, config.Delay);
		}
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
	}

	private unsafe static void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if ((int)actionType == 5 && actionID == 7 && ((GameMain)GameMain.Instance()).CurrentContentFinderConditionId == 0 && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && ManagerBase<ModuleManager>.Instance().IsModuleEnabled("BetterTeleport") != true)
		{
			AgentTeleport* ptr = AgentTeleport.Instance();
			if (ptr != null)
			{
				isPrevented = true;
				((AgentTeleport)ptr).Show();
			}
		}
	}

	private void OnPreUseCommand(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (!((command != ExecuteCommandFlag.Teleport) | isPrevented) && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
		{
			isPrevented = true;
			queuedTeleport = (param1, param3);
			Notify(QueueTeleportNotifyType.Save);
		}
	}

	private unsafe void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)flag != 26 || value || !queuedTeleport.HasValue)
		{
			return;
		}
		FateContext* currentFate = ((FateManager)FateManager.Instance()).CurrentFate;
		if (currentFate != null && ((FateContext)currentFate).Progress < 80)
		{
			return;
		}
		if (currentFate != null)
		{
			base.TaskHelper.Enqueue(() => ((FateManager)FateManager.Instance()).CurrentFate == null);
		}
		base.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26]);
		if (config.Delay > 0)
		{
			base.TaskHelper.Enqueue(delegate
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("QueueCombatTeleport-Notice-Waiting", config.Delay));
			});
			base.TaskHelper.DelayNext(config.Delay);
		}
		base.TaskHelper.Enqueue(delegate
		{
			((Telepo)Telepo.Instance()).Teleport(queuedTeleport.Value.ID, (byte)queuedTeleport.Value.SubID);
			Notify(QueueTeleportNotifyType.Execute);
			queuedTeleport = null;
			return true;
		});
	}

	private void Notify(QueueTeleportNotifyType type)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (!config.SendChat && !config.SendNotification)
		{
			return;
		}
		string text = string.Empty;
		Aetheryte value;
		PlaceName value2;
		switch (type)
		{
		case QueueTeleportNotifyType.Save:
			if (queuedTeleport.HasValue)
			{
				IAetheryteEntry aetheryteEntry2 = IOmenDalamudService<IAetheryteList>.Instance().FirstOrDefault((IAetheryteEntry x) => x.AetheryteID == queuedTeleport.Value.ID && x.SubIndex == queuedTeleport.Value.SubID);
				if (aetheryteEntry2 == null)
				{
					return;
				}
				object[] array2 = new object[1];
				value = aetheryteEntry2.AetheryteData.Value;
				value2 = ((Aetheryte)(ref value)).PlaceName.Value;
				array2[0] = ((object)((PlaceName)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
				text = LanguageManager.Get("QueueCombatTeleport-Notice-Saved", array2);
			}
			break;
		case QueueTeleportNotifyType.Execute:
			if (queuedTeleport.HasValue)
			{
				IAetheryteEntry aetheryteEntry = IOmenDalamudService<IAetheryteList>.Instance().FirstOrDefault((IAetheryteEntry x) => x.AetheryteID == queuedTeleport.Value.ID && x.SubIndex == queuedTeleport.Value.SubID);
				if (aetheryteEntry == null)
				{
					return;
				}
				object[] array = new object[1];
				value = aetheryteEntry.AetheryteData.Value;
				value2 = ((Aetheryte)(ref value)).PlaceName.Value;
				array[0] = ((object)((PlaceName)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
				text = LanguageManager.Get("QueueCombatTeleport-Notice-Executed", array);
			}
			break;
		case QueueTeleportNotifyType.Clear:
			text = LanguageManager.Get("QueueCombatTeleport-Notice-Cleared");
			break;
		}
		if (!string.IsNullOrWhiteSpace(text))
		{
			if (config.SendChat)
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
			}
			if (config.SendNotification)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(text);
			}
		}
	}
}
