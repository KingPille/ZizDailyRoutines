using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoMateriaRetrive : ModuleBase
{
	private Hook<MaterializeItem>? RetriveMateriaHook;

	private ItemSelectCombo itemSelectCombo;

	private const uint EVENT_ID = 3735553u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMateriaRetriveTitle"),
		Description = LanguageManager.Get("AutoMateriaRetriveDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		itemSelectCombo = new ItemSelectCombo("Item", (from x in (IEnumerable<Item>)LuminaGetter.Get<Item>()
			where ((Item)(ref x)).MateriaSlotCount > 0 && !string.IsNullOrEmpty(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString())
			group x by ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString() into x
			select x.First()).ToList());
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		RetriveMateriaHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<MaterializeItem>(typeof(MemberFunctionPointers), "MaterializeItem", new MaterializeItem(RetriveMateriaDetour));
		RetriveMateriaHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		ImGuiOmExtension.ConflictKeyText();
		ImGui.NewLine();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoMateriaRetrive-ManuallySelect"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			itemSelectCombo.DrawRadio();
			ImGui.SameLine();
			DisabledDisposable val3 = ImRaii.Disabled(itemSelectCombo.SelectedID == 0);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Start")), default(Vector2)))
				{
					base.TaskHelper.Abort();
					EnqueueRetriveTaskByItemID(itemSelectCombo.SelectedID);
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe void EnqueueRetriveTaskByItemID(uint itemID)
	{
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			if (base.TaskHelper.AbortByConflictKey(this))
			{
				base.TaskHelper.Abort();
			}
			else
			{
				InventoryManager* ptr = InventoryManager.Instance();
				foreach (InventoryType item in Inventories.PlayerWithArmory)
				{
					InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(item);
					if (inventoryContainer != null && ((InventoryContainer)inventoryContainer).IsLoaded)
					{
						for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
						{
							InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
							if (inventorySlot != null && ((InventoryItem)inventorySlot).ItemId != 0 && ((InventoryItem)inventorySlot).ItemId == itemID && !((InventoryItem)inventorySlot).Materia.ToArray().All((ushort x) => x == 0))
							{
								EnqueueRetriveTask(item, (short)i);
								return;
							}
						}
					}
				}
				base.TaskHelper.Abort();
				OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(LanguageManager.Get("AutoMateriaRetrive-NoItemFound"));
			}
		});
		base.TaskHelper.Enqueue(delegate
		{
			if (base.TaskHelper.AbortByConflictKey(this))
			{
				base.TaskHelper.Abort();
			}
			else
			{
				EnqueueRetriveTaskByItemID(itemID);
			}
		});
	}

	private unsafe void EnqueueRetriveTask(InventoryType inventoryType, short inventorySlot)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(delegate
		{
			if (base.TaskHelper.AbortByConflictKey(this))
			{
				base.TaskHelper.Abort();
				return true;
			}
			return !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>());
		}, "WaitEventEndBefore", 0, null, null, null, null, 1);
		base.TaskHelper.Enqueue(delegate
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			if (base.TaskHelper.AbortByConflictKey(this) || Inventories.Player.IsFull())
			{
				base.TaskHelper.Abort();
			}
			else
			{
				Retrive(inventoryType, inventorySlot);
			}
		}, "RetriveWork", 0, null, null, null, null, 1);
		base.TaskHelper.Enqueue(delegate
		{
			if (base.TaskHelper.AbortByConflictKey(this))
			{
				base.TaskHelper.Abort();
				return true;
			}
			return !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>());
		}, "WaitEventEndAfter", 0, null, null, null, null, 1);
		base.TaskHelper.Enqueue(delegate
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			if (base.TaskHelper.AbortByConflictKey(this))
			{
				base.TaskHelper.Abort();
			}
			else
			{
				InventoryItem* inventorySlot2 = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(inventoryType, (int)inventorySlot);
				if (inventorySlot2 != null && ((InventoryItem)inventorySlot2).ItemId != 0 && !((InventoryItem)inventorySlot2).Materia.ToArray().All((ushort x) => x == 0))
				{
					EnqueueRetriveTask(inventoryType, inventorySlot);
				}
			}
		}, "EnqueueNewRound_SingleSlot", 0, null, null, null, null, 1);
	}

	private unsafe void Retrive(InventoryType type, short slot)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		RetriveMateriaHook.Original.Invoke(EventFramework.Instance(), EventId.op_Implicit(3735553u), type, slot, 0);
	}

	private unsafe void RetriveMateriaDetour(EventFramework* framework, EventId eventID, InventoryType inventoryType, short inventorySlot, int extraParam)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		RetriveMateriaHook.Original.Invoke(framework, eventID, inventoryType, inventorySlot, extraParam);
		if (eventID == EventId.op_Implicit(3735553u) && !base.TaskHelper.IsBusy)
		{
			EnqueueRetriveTask(inventoryType, inventorySlot);
		}
	}
}
