using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.OmenService;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoFCWSDeliver : ModuleBase
{
	private record WorkshopCraftItem(uint ItemID, uint ItemCount, uint Index)
	{
		public unsafe static List<WorkshopCraftItem> Parse(AtkUnitBase* addon)
		{
			List<WorkshopCraftItem> list = new List<WorkshopCraftItem>();
			if (addon == null || ((AtkUnitBase)addon).NameString != "SubmarinePartsMenu")
			{
				return list;
			}
			AgentCompanyCraftMaterial* ptr = AgentCompanyCraftMaterial.Instance();
			if (ptr == null)
			{
				return list;
			}
			if (InventoryManager.Instance() == null)
			{
				return list;
			}
			int num = ((AgentCompanyCraftMaterial)ptr).SupplyItems.ToArray().Count((uint x) => x != 0);
			if (num == 0)
			{
				return list;
			}
			for (int num2 = 0; num2 < num; num2++)
			{
				uint uInt = ((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)(12 + num2) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
				if (uInt != 0 && ((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)(132 + num2) * (nint)Unsafe.SizeOf<AtkValue>())).UInt != 1)
				{
					uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)(60 + num2) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
					if (((AtkValue)((byte*)((AtkUnitBase)addon).AtkValues + (nint)(72 + num2) * (nint)Unsafe.SizeOf<AtkValue>())).UInt >= uInt2)
					{
						list.Add(new WorkshopCraftItem(uInt, uInt2, (uint)num2));
					}
				}
			}
			return list;
		}
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoFCWSDeliverTitle"),
		Description = LanguageManager.Get("AutoFCWSDeliverDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[2] { "AutoRequestItemSubmit", "AutoCutsceneSkip" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SelectYesno", new AddonEventDelegate(OnAddonYesno));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SelectString", new AddonEventDelegate(OnAddonString));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SubmarinePartsMenu", new AddonEventDelegate(OnAddonMenu));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "SubmarinePartsMenu", new AddonEventDelegate(OnAddonMenu));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "CompanyCraftRecipeNoteBook", new AddonEventDelegate(OnAddonRecipeNote));
		if (Addons.SubmarinePartsMenu != null)
		{
			OnAddonMenu((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonRecipeNote });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonYesno });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonString });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonMenu });
	}

	protected override void ConfigUI()
	{
		ImGuiOmExtension.ConflictKeyText();
	}

	protected unsafe override void OverlayUI()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.SubmarinePartsMenu == null)
		{
			((Window)base.Overlay).IsOpen = false;
			base.TaskHelper.RemoveQueue(0);
			return;
		}
		ImGui.SetWindowPos(new Vector2(((AtkUnitBase)Addons.SubmarinePartsMenu).GetX() + 6, (float)((AtkUnitBase)Addons.SubmarinePartsMenu).GetY() - ImGui.GetWindowSize().Y + 6f), (ImGuiCond)0);
		ImGui.AlignTextToFramePadding();
		Vector4 dalamudYellow = ImGuiColors.DalamudYellow;
		ImGui.TextColored(ref dalamudYellow, ImU8String.op_Implicit(LanguageManager.Get("AutoFCWSDeliverTitle")));
		ImGui.SameLine();
		DisabledDisposable val = ImRaii.Disabled(base.TaskHelper.IsBusy);
		try
		{
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Start")), default(Vector2)))
			{
				EnqueueSubmit();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Stop")), default(Vector2)))
		{
			base.TaskHelper.Abort();
		}
	}

	private unsafe bool EnqueueSubmit()
	{
		if (base.TaskHelper.AbortByConflictKey(this))
		{
			return true;
		}
		if (!(*Addons.SubmarinePartsMenu).IsAddonAndNodesReady())
		{
			return false;
		}
		if ((*Addons.Request).IsAddonAndNodesReady())
		{
			return false;
		}
		base.TaskHelper.Abort();
		List<WorkshopCraftItem> list = WorkshopCraftItem.Parse(Addons.SubmarinePartsMenu);
		if (list.Count == 0)
		{
			return true;
		}
		using (List<WorkshopCraftItem>.Enumerator enumerator = list.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				WorkshopCraftItem item = enumerator.Current;
				base.TaskHelper.Enqueue(delegate
				{
					//IL_0033: Unknown result type (might be due to invalid IL or missing references)
					//IL_004f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0050: Unknown result type (might be due to invalid IL or missing references)
					//IL_008f: Unknown result type (might be due to invalid IL or missing references)
					if (!base.TaskHelper.AbortByConflictKey(this))
					{
						AtkValue val = default(AtkValue);
						((AtkValue)(ref val))._002Ector();
						val.Type = (AtkValueType)(item.ItemID % 500000);
						val.Int64 = SetHighDword((int)item.ItemCount);
						AtkValue val2 = val;
						((AgentId)200).SendEvent(0uL, 0, item.Index, item.ItemCount, val2);
					}
				}, $"交纳物品 {item.ItemID}x{item.ItemCount}");
				base.TaskHelper.DelayNext(500, "等待 Request");
				base.TaskHelper.Enqueue(delegate
				{
					if (base.TaskHelper.AbortByConflictKey(this))
					{
						return true;
					}
					return !(*Addons.Request).IsAddonAndNodesReady() && !(*Addons.SelectYesno).IsAddonAndNodesReady();
				}, "等待材料上交界面消失");
			}
		}
		base.TaskHelper.Enqueue((Func<bool>)EnqueueSubmit, "开始新一轮", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		return true;
	}

	private void OnAddonMenu(AddonEvent type, AddonArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Invalid comparison between Unknown and I4
		Overlay overlay = base.Overlay;
		bool isOpen = (int)type == 1 || (int)type == 6 || ((Window)base.Overlay).IsOpen;
		((Window)overlay).IsOpen = isOpen;
		if ((int)type == 1 && !base.TaskHelper.AbortByConflictKey(this))
		{
			base.TaskHelper.Enqueue((Func<bool>)EnqueueSubmit, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private unsafe void OnAddonYesno(AddonEvent type, AddonArgs args)
	{
		if (((HousingManager)HousingManager.Instance()).WorkshopTerritory != null && Addons.SubmarinePartsMenu != null && base.TaskHelper.IsBusy)
		{
			AtkUnitBase* selectYesno = Addons.SelectYesno;
			if (selectYesno != null && !string.IsNullOrWhiteSpace(((object)(*(CStringPointer*)(&((AtkValue)((AtkUnitBase)selectYesno).AtkValues).String))/*cast due to constrained. prefix*/).ToString()) && !base.TaskHelper.AbortByConflictKey(this))
			{
				AddonSelectYesnoEvent.ClickYes();
			}
		}
	}

	private unsafe void OnAddonString(AddonEvent type, AddonArgs args)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		if (!Throttler.Shared.Throttle("AutoFCWSDeliver-ClickSelectString", 3000u) || Addons.SelectString == null || ((HousingManager)HousingManager.Instance()).WorkshopTerritory == null)
		{
			return;
		}
		IGameObject target = TargetManager.Target;
		if (target == null || (int)target.ObjectKind != 7 || target.DataID != 2011588 || base.TaskHelper.AbortByConflictKey(this))
		{
			return;
		}
		base.TaskHelper.RemoveQueueTasks(0);
		AddonSelectStringEvent.Select(0);
		base.TaskHelper.Enqueue(delegate
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Invalid comparison between Unknown and I4
			if (base.TaskHelper.AbortByConflictKey(this))
			{
				return true;
			}
			if (DalamudLeftServiceExtension.Instance().CutsceneActive || !UIModuleExtension.IsScreenReady())
			{
				return false;
			}
			IGameObject target2 = TargetManager.Target;
			if (target2 == null || (int)target2.ObjectKind != 7 || target2.DataID != 2011588)
			{
				TargetManager.Target = IOmenDalamudService<IObjectTable>.Instance().FindNearest(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Position, (IGameObject x) => x != null && (int)x.ObjectKind == 7 && x.DataID == 2011588);
			}
			TargetManager.Target.Interact();
			return (*Addons.SubmarinePartsMenu).IsAddonAndNodesReady() || (*Addons.SelectString).IsAddonAndNodesReady();
		}, "尝试再次交互合建设备", 0, null, null, null, null, 1);
	}

	private void OnAddonRecipeNote(AddonEvent type, AddonArgs args)
	{
		base.TaskHelper.Abort();
	}

	public static long SetHighDword(int value)
	{
		return (long)value << 32;
	}
}
