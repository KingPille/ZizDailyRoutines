using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class RepairCommand : ExecuteCommandBase
{
	public enum RepairPageCategory : uint
	{
		MainOffHands,
		HeadBodyArms,
		LegsFeet,
		EarsNeck,
		WristsRings,
		Inventory
	}

	public static void SendRequest(uint targetEntityID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SendRepairRequest, targetEntityID);
	}

	public static void CancelRequest()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CancelRepairRequest);
	}

	public static void StartRequest()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartRepairRequest);
	}

	public static void ConfirmRequest()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ConfirmRepairRequest);
	}

	public static void RepairEquippedItems()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EventFrameworkAction, 3735555u, 2u, 1000u);
	}

	public static void RepairPage(RepairPageCategory category)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EventFrameworkAction, 3735555u, 3u, (uint)category);
	}

	public unsafe static void RepairItem(InventoryType inventoryType, uint inventorySlot)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EventFrameworkAction, 3735555u, (inventorySlot << 16) | 1, (uint)(int)inventoryType, ((InventoryItem)((InventoryManager)InventoryManager.Instance()).GetInventorySlot(inventoryType, (int)inventorySlot)).GetItemId());
	}

	public static void RepairPageNPC(RepairPageCategory category)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RepairAllItemsNPC, (uint)category);
	}

	public static void RepairEquippedNPC()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RepairEquippedItemsNPC, 1000u);
	}

	public unsafe static void RepairNPC(InventoryType inventoryType, uint inventorySlot)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RepairItemNPC, (uint)(int)inventoryType, inventorySlot, ((InventoryItem)((InventoryManager)InventoryManager.Instance()).GetInventorySlot(inventoryType, (int)inventorySlot)).GetItemId());
	}
}
