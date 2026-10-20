using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace OmenTools.Extensions;

public static class InventoryExtension
{
	extension(IEnumerable<InventoryType> inventories)
	{
		public unsafe bool TryGetItems(Predicate<InventoryItem> predicateFunc, out List<InventoryItem> itemResult)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			itemResult = new List<InventoryItem>();
			InventoryManager* ptr = InventoryManager.Instance();
			if (ptr == null)
			{
				return false;
			}
			foreach (InventoryType inventory in inventories)
			{
				InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(inventory);
				if (inventoryContainer == null || !((InventoryContainer)inventoryContainer).IsLoaded)
				{
					return false;
				}
				for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
				{
					InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
					if (inventorySlot != null && predicateFunc(*inventorySlot))
					{
						itemResult.Add(*inventorySlot);
					}
				}
			}
			return itemResult.Count > 0;
		}

		public unsafe bool TryGetFirstItem(Predicate<InventoryItem> predicateFunc, out InventoryItem* itemResult)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			itemResult = null;
			InventoryManager* ptr = InventoryManager.Instance();
			if (ptr == null)
			{
				return false;
			}
			foreach (InventoryType inventory in inventories)
			{
				InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(inventory);
				if (inventoryContainer == null || !((InventoryContainer)inventoryContainer).IsLoaded)
				{
					return false;
				}
				for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
				{
					InventoryItem* inventorySlot = ((InventoryContainer)inventoryContainer).GetInventorySlot(i);
					if (inventorySlot != null && predicateFunc(*inventorySlot) && predicateFunc(*inventorySlot))
					{
						itemResult = inventorySlot;
						return true;
					}
				}
			}
			return false;
		}

		public unsafe bool IsFull(uint threshold = 0u)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			InventoryManager* ptr = InventoryManager.Instance();
			if (ptr == null)
			{
				return true;
			}
			uint num = 0u;
			foreach (InventoryType inventory in inventories)
			{
				InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(inventory);
				if (inventoryContainer == null || !((InventoryContainer)inventoryContainer).IsLoaded)
				{
					continue;
				}
				for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
				{
					if (((InventoryItem)((InventoryContainer)inventoryContainer).GetInventorySlot(i)).ItemId == 0)
					{
						num++;
						if (num > threshold)
						{
							return false;
						}
					}
				}
			}
			return num <= threshold;
		}
	}

	extension(InventoryType inventoryType)
	{
		public unsafe bool OpenSlotContext(ushort slot)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			AgentInventoryContext* ptr = AgentInventoryContext.Instance();
			if (ptr == null)
			{
				return false;
			}
			((AgentInventoryContext)ptr).OpenForItemSlot(inventoryType, (int)slot, 0, (*AgentInventory.Instance()).GetActiveAddonID());
			return true;
		}

		public bool TryGetItems(Predicate<InventoryItem> predicateFunc, out List<InventoryItem> itemResult)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((IEnumerable<InventoryType>)new List<InventoryType> { inventoryType }).TryGetItems(predicateFunc, out itemResult);
		}

		public unsafe bool TryGetFirstItem(Predicate<InventoryItem> predicateFunc, out InventoryItem* itemResult)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((IEnumerable<InventoryType>)new List<InventoryType> { inventoryType }).TryGetFirstItem(predicateFunc, out itemResult);
		}
	}

	extension(scoped in InventoryItem item)
	{
		public bool OpenContext()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return item.Container.OpenSlotContext((ushort)item.Slot);
		}
	}
}
