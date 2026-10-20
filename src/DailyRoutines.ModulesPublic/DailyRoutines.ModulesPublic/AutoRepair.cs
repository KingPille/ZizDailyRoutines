using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.DutyState;
using Dalamud.Game.Gui.Toast;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoRepair : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AllowNPCRepair = true;

		public bool PrioritizeNPCRepair;

		public float RepairThreshold = 20f;
	}

	private Config config;

	private const uint REPAIR_EVENT_ID = 720915u;

	private static readonly FrozenSet<ConditionFlag> ValidConditions = FrozenSet.Create(RuntimeHelpers.CreateSpan<ConditionFlag>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	private static readonly FrozenSet<ExecuteCommandFlag> ValidRepairFlags = FrozenSet.Create<ExecuteCommandFlag>(ExecuteCommandFlag.RepairItemNPC, ExecuteCommandFlag.RepairAllItemsNPC, ExecuteCommandFlag.RepairEquippedItemsNPC, ExecuteCommandFlag.EventFrameworkAction);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRepairTitle"),
		Description = LanguageManager.Get("AutoRepairDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	[IPCProvider("DailyRoutines.Modules.AutoRepair.IsBusy")]
	private bool IsBusyIPC => base.TaskHelper?.IsBusy ?? false;

	[IPCProvider("DailyRoutines.Modules.AutoRepair.IsNeedToRepair")]
	private bool IsNeedToRepairIPC
	{
		get
		{
			List<InventoryItem> itemResult;
			return ((InventoryType)1000).TryGetItems((InventoryItem x) => (float)(int)x.Condition < config.RepairThreshold * 300f, out itemResult);
		}
	}

	[IPCProvider("DailyRoutines.Modules.AutoRepair.IsAbleToRepair")]
	private bool IsAbleToRepairIPC => IsAbleToRepair();

	protected override void Init()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		if (config == null)
		{
			config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		}
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				ShowDebug = true
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPost(OnExecuteCommand);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced += new DutyRecommencedDelegate(OnDutyRecommenced);
	}

	protected override void Uninit()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnExecuteCommand);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced -= new DutyRecommencedDelegate(OnDutyRecommenced);
	}

	protected override void ConfigUI()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
		ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get("AutoRepair-RepairThreshold")), ref config.RepairThreshold, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoRepair-AllowNPCRepair")), ref config.AllowNPCRepair))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoRepair-AllowNPCRepairHelp"), 100f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
		if (config.AllowNPCRepair)
		{
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoRepair-PrioritizeNPCRepair")), ref config.PrioritizeNPCRepair))
			{
				config.Save(this);
			}
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoRepair-PrioritizeNPCRepairHelp"), 100f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
		}
	}

	public unsafe void EnqueueRepair()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (base.TaskHelper.IsBusy || GameState.IsInPVPInstance)
		{
			return;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null || localPlayer.CurrentHp == 0)
		{
			return;
		}
		PlayerState* intPtr = PlayerState.Instance();
		InventoryManager* ptr = InventoryManager.Instance();
		if (intPtr == null || ptr == null || !((InventoryType)1000).TryGetItems((InventoryItem x) => (float)(int)x.Condition < this.config.RepairThreshold * 300f, out List<InventoryItem> itemResult))
		{
			return;
		}
		base.TaskHelper.Abort();
		bool flag = (*EventFramework.Instance()).IsEventIDNearby(EventId.op_Implicit(720915u));
		Config config = this.config;
		if (config != null && config.AllowNPCRepair && config.PrioritizeNPCRepair && flag)
		{
			base.TaskHelper.Enqueue((Action)NotifyStartRepair, "通知开始自动修复", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			EnqueueNPCRepairTasks();
			return;
		}
		(List<uint> ItemsUnableToRepair, bool IsDMInsufficient) tuple = AnalyzeItems(itemResult);
		List<uint> item = tuple.ItemsUnableToRepair;
		bool item2 = tuple.IsDMInsufficient;
		bool flag2 = itemResult.Count > item.Count;
		bool flag3 = this.config.AllowNPCRepair && item.Count > 0 && flag;
		if (flag2 || flag3)
		{
			base.TaskHelper.Enqueue((Action)NotifyStartRepair, "通知开始自动修复", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			if (flag2)
			{
				EnqueueSelfRepairTasks(itemResult, item, item2);
			}
			if (flag3)
			{
				EnqueueNPCRepairTasks();
			}
		}
	}

	private unsafe void EnqueueSelfRepairTasks(List<InventoryItem> items, List<uint> itemsUnableToRepair, bool isDMInsufficient)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Enqueue((Func<bool>)IsAbleToRepair, "等待可以维修状态", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		if (!isDMInsufficient)
		{
			base.TaskHelper.Enqueue(() => ((RepairManager)RepairManager.Instance()).RepairEquipped(false), "开始自动修复");
		}
		else
		{
			List<InventoryItem> list = items.ToList();
			list.RemoveAll((InventoryItem x) => itemsUnableToRepair.Contains(x.ItemId));
			foreach (InventoryItem item2 in list)
			{
				InventoryItem item = item2;
				base.TaskHelper.Enqueue(() => ((RepairManager)RepairManager.Instance()).RepairItem(item.Container, (ushort)item.Slot, false), "修理装备: " + LuminaWrapper.GetItemName(((InventoryItem)(ref item)).GetBaseItemId()));
				base.TaskHelper.DelayNext(3000, "等待 3 秒, 开始下一件单独装备修理");
			}
		}
		base.TaskHelper.DelayNext(500, "等待 500 毫秒");
	}

	private unsafe void EnqueueNPCRepairTasks()
	{
		base.TaskHelper.Enqueue((Func<bool>)IsAbleToRepair, "等待进入可以修理状态", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.Enqueue(delegate
		{
			new EventStartPackt(LocalPlayerState.EntityID, 720915u).Send();
		}, "打开 NPC 修理委托界面");
		base.TaskHelper.Enqueue(() => (*Addons.Repair).IsAddonAndNodesReady(), "等待修理界面就绪");
		base.TaskHelper.Enqueue(delegate
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RepairEquippedItemsNPC, 1000u);
		}, "发送 NPC 修理委托请求");
		base.TaskHelper.Enqueue(delegate
		{
			if ((*Addons.Repair).IsAddonAndNodesReady())
			{
				((AtkUnitBase)Addons.Repair).Close(true);
			}
		}, "关闭修理界面");
	}

	private static (List<uint> ItemsUnableToRepair, bool IsDMInsufficient) AnalyzeItems(List<InventoryItem> items)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		List<uint> list = new List<uint>();
		bool item = false;
		Dictionary<uint, uint> dictionary = ((IEnumerable<ItemRepairResource>)LuminaGetter.Get<ItemRepairResource>()).Where((ItemRepairResource x) => ((ItemRepairResource)(ref x)).Item.RowId != 0).ToDictionary((ItemRepairResource x) => ((ItemRepairResource)(ref x)).RowId, (ItemRepairResource x) => LocalPlayerState.GetItemCount(((ItemRepairResource)(ref x)).Item.RowId));
		foreach (InventoryItem item3 in items)
		{
			uint itemId = item3.ItemId;
			if (!LuminaGetter.TryGetRow<Item>(itemId, out Item item2))
			{
				continue;
			}
			uint rowId = ((Item)(ref item2)).ClassJobRepair.RowId;
			int num = Math.Max(1, Math.Max(0, ((Item)(ref item2)).LevelEquip - 10));
			uint repairDM = ((Item)(ref item2)).ItemRepair.RowId;
			uint key = dictionary.OrderBy((KeyValuePair<uint, uint> x) => x.Key).FirstOrDefault((KeyValuePair<uint, uint> x) => x.Key >= repairDM && x.Value - 1 >= 0).Key;
			if (LocalPlayerState.GetClassJobLevel(rowId) >= num && key != 0)
			{
				dictionary[key]--;
				continue;
			}
			if (key == 0)
			{
				item = true;
			}
			list.Add(itemId);
		}
		return (ItemsUnableToRepair: list, IsDMInsufficient: item);
	}

	private static void NotifyStartRepair()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		NotifyHelper.ToastQuest(LanguageManager.Get("AutoRepair-Notification-AutoStart"), new QuestToastOptions
		{
			IconId = 106u
		});
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoRepair-Notification-AutoStart")));
	}

	private unsafe static bool IsAbleToRepair()
	{
		if (UIModuleExtension.IsScreenReady() && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && !GameState.IsInPVPInstance && !IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()) && !IConditionExtension.get_IsCasting(DalamudServiceExtension.Instance<ICondition>()))
		{
			return ((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)5, 6u, 3758096384uL, true, true, (uint*)null) == 0;
		}
		return false;
	}

	private void OnDutyRecommenced(IDutyStateEventArgs args)
	{
		EnqueueRepair();
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (!value && ValidConditions.Contains(flag))
		{
			EnqueueRepair();
		}
	}

	private void OnZoneChanged(uint u)
	{
		EnqueueRepair();
	}

	private static void OnExecuteCommand(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4)
	{
		if (ValidRepairFlags.Contains(command))
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RefreshInventory);
		}
	}

	[IPCProvider("DailyRoutines.Modules.AutoRepair.EnqueueRepair")]
	private void EnqueueRepairIPC()
	{
		EnqueueRepair();
	}
}
