using System;
using System.Collections.Frozen;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class FastRetainerStore : ModuleBase
{
	private sealed class ItemMoveMenu(FastRetainerStore module) : ContextMenuEntry
	{
		public override string Identifier => "FastRetainerStore";

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			InventoryItem? targetInventoryItem = args.TargetInventoryItem;
			uint itemID;
			bool isHQ;
			bool isCollectable;
			if (targetInventoryItem.HasValue)
			{
				InventoryItem valueOrDefault = targetInventoryItem.GetValueOrDefault();
				string addonName = args.AddonName;
				if (addonName != null)
				{
					itemID = ((InventoryItem)(ref valueOrDefault)).GetBaseItemId();
					if (!LuminaGetter.TryGetRow<Item>(itemID, out Item _))
					{
						return null;
					}
					bool num = IsPlayerInventoryOpen();
					bool flag = IsRetainerInventoryOpen();
					if (!num || !flag)
					{
						return null;
					}
					isHQ = ((Enum)valueOrDefault.Flags).HasFlag((Enum)(object)(ItemFlags)1);
					isCollectable = ((Enum)valueOrDefault.Flags).HasFlag((Enum)(object)(ItemFlags)8);
					(InventoryType, int) targetSlot;
					if (PlayerAddonNames.Contains(addonName))
					{
						if (!TryFindTargetSlot(Inventories.Retainer, itemID, isHQ, isCollectable, out targetSlot))
						{
							return null;
						}
						return CreateItem(storeToRetainer: true);
					}
					if (RetainerAddonNames.Contains(addonName))
					{
						if (!TryFindTargetSlot(Inventories.Player, itemID, isHQ, isCollectable, out targetSlot))
						{
							return null;
						}
						return CreateItem(storeToRetainer: false);
					}
					return null;
				}
			}
			return null;
			ContextMenuItem CreateItem(bool storeToRetainer)
			{
				//IL_0033: Unknown result type (might be due to invalid IL or missing references)
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get(storeToRetainer ? "FastRetainerStore-ContextMenu-SaveAll" : "FastRetainerStore-ContextMenu-RetrieveAll")),
					OnClicked = delegate
					{
						module.ExecuteMoveAll(itemID, isHQ, isCollectable, storeToRetainer);
					}
				};
			}
			unsafe static bool IsPlayerInventoryOpen()
			{
				if (!(*Addons.Inventory).IsAddonAndNodesReady() && !(*Addons.InventoryLarge).IsAddonAndNodesReady())
				{
					return (*Addons.InventoryExpansion).IsAddonAndNodesReady();
				}
				return true;
			}
			unsafe static bool IsRetainerInventoryOpen()
			{
				if (!(*Addons.InventoryRetainer).IsAddonAndNodesReady())
				{
					return (*Addons.InventoryRetainerLarge).IsAddonAndNodesReady();
				}
				return true;
			}
		}
	}

	private ItemMoveMenu menuItem;

	private static readonly string[] PlayerAddonNames = new string[3] { "Inventory", "InventoryLarge", "InventoryExpansion" };

	private static readonly string[] RetainerAddonNames = new string[2] { "InventoryRetainer", "InventoryRetainerLarge" };

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastRetainerStoreTitle"),
		Description = LanguageManager.Get("FastRetainerStoreDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "YLCHEN" }
	};

	protected override void Init()
	{
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		menuItem = new ItemMoveMenu(this);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(menuItem);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(menuItem);
	}

	private unsafe void ExecuteMoveAll(uint itemID, bool isHQ, bool isCollectable, bool storeToRetainer)
	{
		if (base.TaskHelper.IsBusy)
		{
			return;
		}
		FrozenSet<InventoryType> sourceInvs = (storeToRetainer ? Inventories.Player : Inventories.Retainer);
		FrozenSet<InventoryType> targetInvs = (storeToRetainer ? Inventories.Retainer : Inventories.Player);
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			InventoryManager* ptr = InventoryManager.Instance();
			if (ptr == null)
			{
				return false;
			}
			foreach (InventoryType item in sourceInvs)
			{
				InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(item);
				if (inventoryContainer != null)
				{
					for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
					{
						InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
						if (inventorySlot != null && IsSameItem(inventorySlot, itemID, isHQ, isCollectable))
						{
							if (!TryFindTargetSlot(targetInvs, itemID, isHQ, isCollectable, out (InventoryType, int) targetSlot))
							{
								return true;
							}
							((InventoryManager)ptr).MoveItemSlot(item, (ushort)((InventoryItem)inventorySlot).Slot, targetSlot.Item1, (ushort)targetSlot.Item2, true);
						}
					}
				}
			}
			return true;
		}, storeToRetainer ? "存入雇员" : "取出到背包");
	}

	private unsafe static bool IsSameItem(InventoryItem* slot, uint itemID, bool isHQ, bool isCollectable)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		uint baseItemId = ((InventoryItem)slot).GetBaseItemId();
		if (baseItemId == 0)
		{
			return false;
		}
		bool flag = ((Enum)((InventoryItem)slot).Flags).HasFlag((Enum)(object)(ItemFlags)1);
		bool flag2 = ((Enum)((InventoryItem)slot).Flags).HasFlag((Enum)(object)(ItemFlags)8);
		if (baseItemId == itemID && flag == isHQ)
		{
			return flag2 == isCollectable;
		}
		return false;
	}

	private unsafe static bool TryFindTargetSlot(FrozenSet<InventoryType> targetInvs, uint itemID, bool isHQ, bool isCollectable, out (InventoryType Inventory, int Slot) targetSlot)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		InventoryManager* ptr = InventoryManager.Instance();
		if (!LuminaGetter.TryGetRow<Item>(itemID, out Item item))
		{
			targetSlot = (Inventory: (InventoryType)9999, Slot: -1);
			return false;
		}
		foreach (InventoryType targetInv in targetInvs)
		{
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(targetInv);
			if (inventoryContainer == null)
			{
				continue;
			}
			for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
			{
				InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
				if (inventorySlot != null && IsSameItem(inventorySlot, itemID, isHQ, isCollectable) && ((InventoryItem)inventorySlot).Quantity < ((Item)(ref item)).StackSize)
				{
					targetSlot = (Inventory: targetInv, Slot: ((InventoryItem)inventorySlot).Slot);
					return true;
				}
			}
		}
		foreach (InventoryType targetInv2 in targetInvs)
		{
			InventoryContainer* inventoryContainer2 = ((InventoryManager)ptr).GetInventoryContainer(targetInv2);
			if (inventoryContainer2 == null)
			{
				continue;
			}
			for (int j = 0; j < ((InventoryContainer)inventoryContainer2).Size; j++)
			{
				InventoryItem* inventorySlot2 = ((InventoryContainer)inventoryContainer2).GetInventorySlot(j);
				if (inventorySlot2 != null && ((InventoryItem)inventorySlot2).GetItemId() == 0)
				{
					targetSlot = (Inventory: targetInv2, Slot: ((InventoryItem)inventorySlot2).Slot);
					return true;
				}
			}
		}
		targetSlot = (Inventory: (InventoryType)9999, Slot: -1);
		return false;
	}
}
