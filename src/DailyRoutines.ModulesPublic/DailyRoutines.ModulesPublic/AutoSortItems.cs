using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoSortItems : ModuleBase
{
	private class Config : ModuleConfig
	{
		public int ArmouryCategory;

		public int ArmouryChestID;

		public int ArmouryItemLevel;

		public int InventoryCategory;

		public int InventoryHQ;

		public int InventoryID;

		public int InventoryItemLevel;

		public int InventoryTab;

		public bool SendChat;

		public bool SendNotification = true;
	}

	private readonly string[] sortOptions = new string[2]
	{
		LanguageManager.Get("Descending"),
		LanguageManager.Get("Ascending")
	};

	private readonly string[] tabOptions = new string[2]
	{
		LanguageManager.Get("AutoSortItems-Splited"),
		LanguageManager.Get("AutoSortItems-Merged")
	};

	private Config config;

	private static readonly string[] SortOptionsCommand = new string[2] { "des", "asc" };

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSortItemsTitle"),
		Description = LanguageManager.Get("AutoSortItemsDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "那年雪落" }
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 15000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OnZoneChanged(0u);
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Button(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1389u)), default(Vector2)))
		{
			base.TaskHelper.Enqueue((Func<bool>)CheckCanSort, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendChat")), ref config.SendChat))
		{
			config.Save(this);
		}
		ImGui.SameLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("SendNotification")), ref config.SendNotification))
		{
			config.Save(this);
		}
		ImGui.Spacing();
		Vector2 vector = ImGui.GetContentRegionAvail() * 0.75f;
		vector.Y = 0f;
		Vector2 vector2 = vector;
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LanguageManager.Get("Sort")), 2, (ImGuiTableFlags)1984, vector2);
		try
		{
			if (!(!val))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit("名称"), (ImGuiTableColumnFlags)4, 30f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit("方法"), (ImGuiTableColumnFlags)4, 30f, 0u);
				ImGui.TableNextRow((ImGuiTableRowFlags)1);
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(12210u)));
				string addonText = LuminaWrapper.GetAddonText(9448u);
				DrawTableRow("兵装库 ID", "ID", ref config.ArmouryChestID, sortOptions);
				DrawTableRow("兵装库等级", LanguageManager.Get("Level"), ref config.ArmouryItemLevel, sortOptions);
				DrawTableRow("兵装库类型", addonText, ref config.ArmouryCategory, sortOptions, LanguageManager.Get("AutoSortItems-ArmouryCategoryDesc"));
				ImGui.TableNextRow((ImGuiTableRowFlags)1);
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(12209u)));
				DrawTableRow("背包 HQ", "HQ", ref config.InventoryHQ, sortOptions);
				DrawTableRow("背包 ID", "ID", ref config.InventoryID, sortOptions);
				DrawTableRow("背包等级", LanguageManager.Get("Level"), ref config.InventoryItemLevel, sortOptions);
				DrawTableRow("背包类型", addonText, ref config.InventoryCategory, sortOptions, LanguageManager.Get("AutoSortItems-InventoryCategoryDesc"));
				DrawTableRow("背包分栏", LanguageManager.Get("AutoSortItems-Splited"), ref config.InventoryTab, tabOptions, LanguageManager.Get("AutoSortItems-InventoryTabDesc"));
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void DrawTableRow(string id, string label, ref int value, string[] options, string note = "")
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(label);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(id);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			ImGui.TableNextRow();
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(label));
			if (!string.IsNullOrWhiteSpace(note))
			{
				ImGuiOm.HelpMarker(note, 40f, (FontAwesomeIcon)61530);
			}
			int num = value;
			ImGui.TableNextColumn();
			ImGui.SetNextItemWidth(-1f);
			ImU8String val3 = new ImU8String(2, 1);
			((ImU8String)(ref val3)).AppendLiteral("##");
			((ImU8String)(ref val3)).AppendFormatted<string>(label);
			if (ImGui.Combo(val3, ref value, (ReadOnlySpan<string>)options, options.Length) && value != num)
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void OnZoneChanged(uint u)
	{
		base.TaskHelper.Abort();
		if (GameState.TerritoryType != 0)
		{
			base.TaskHelper.DelayNext(2000);
			base.TaskHelper.Enqueue((Func<bool>)CheckCanSort, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private bool CheckCanSort()
	{
		if (!GameState.IsLoggedIn || !UIModuleExtension.IsScreenReady() || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		if (!IConditionExtension.get_IsIdle(DalamudServiceExtension.Instance<ICondition>()) || !IsInValidZone())
		{
			base.TaskHelper.Abort();
			return true;
		}
		base.TaskHelper.Enqueue((Func<bool>)SendSortCommand, "SendSortCommand", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		return true;
	}

	private static bool IsInValidZone()
	{
		if (GameState.Map != 0 && GameState.TerritoryType != 0 && !GameState.IsInPVPArea)
		{
			return GameState.ContentFinderCondition == 0;
		}
		return false;
	}

	private bool SendSortCommand()
	{
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		SendSortCondition("armourychest", "id", config.ArmouryChestID);
		SendSortCondition("armourychest", "itemlevel", config.ArmouryItemLevel);
		SendSortCondition("armourychest", "category", config.ArmouryCategory);
		OmenServiceBase<ChatManager>.Instance().SendMessage("/itemsort execute armourychest");
		SendSortCondition("inventory", "hq", config.InventoryHQ);
		SendSortCondition("inventory", "id", config.InventoryID);
		SendSortCondition("inventory", "itemlevel", config.InventoryItemLevel);
		SendSortCondition("inventory", "category", config.InventoryCategory);
		if (config.InventoryTab == 0)
		{
			OmenServiceBase<ChatManager>.Instance().SendMessage("/itemsort condition inventory tab");
		}
		OmenServiceBase<ChatManager>.Instance().SendMessage("/itemsort execute inventory");
		if (config.SendNotification)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("AutoSortItems-SortMessage"));
		}
		if (config.SendChat)
		{
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoSortItems-SortMessage")));
		}
		return true;
		static void SendSortCondition(string target, string condition, int setting)
		{
			OmenServiceBase<ChatManager>.Instance().SendMessage($"/itemsort condition {target} {condition} {SortOptionsCommand[setting]}");
		}
	}
}
