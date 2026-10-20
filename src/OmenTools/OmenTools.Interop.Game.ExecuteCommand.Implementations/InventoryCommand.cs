using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class InventoryCommand : ExecuteCommandBase
{
	public static void Request(InventoryType inventoryType)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestInventory, (uint)(int)inventoryType);
	}

	public static void Move(InventoryType sourceInventoryType, InventoryType targetInventoryType)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		//IL_0013: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.MoveItemBetweenInventory, (uint)(int)sourceInventoryType, (uint)(int)targetInventoryType);
	}

	public static void BlockOperation(InventoryType sourceInventoryType, InventoryType targetInventoryType)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		//IL_0013: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.NotifyBlockedInventoryOperation, (uint)(int)sourceInventoryType, (uint)(int)targetInventoryType);
	}

	public static void FinishOperation(InventoryType inventoryType, uint inventorySlot)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishInventoryOperation, (uint)(int)inventoryType, inventorySlot);
	}

	public static void RecoverBlockedItem(InventoryType inventoryType, uint inventorySlot)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RecoverBlockedItem, (uint)(int)inventoryType, inventorySlot);
	}

	public static void RequestSaddleBag()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestSaddleBag);
	}

	public static void Refresh()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RefreshInventory);
	}
}
