using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class MateriaCommand : ExecuteCommandBase
{
	public static void EnterAttachState(uint itemID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EnterMateriaAttachState, itemID);
	}

	public static void FinishAttachState()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishMateriaAttach);
	}

	public static void LeaveAttachState()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LeaveMateriaAttachState);
	}

	public static void SendAttachRequest(uint targetEntityID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SendMateriaAttachRequest, targetEntityID);
	}

	public static void CancelAttachRequest()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CancelMateriaAttachRequestForced);
	}

	public static void Extract(InventoryType inventoryType, uint inventorySlot)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ExtractMateria, (uint)(int)inventoryType, inventorySlot);
	}

	public unsafe static void Retrieve(InventoryType inventoryType, uint inventorySlot)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EventFrameworkAction, 3735553u, (uint)(int)inventoryType, inventorySlot, ((InventoryItem)((InventoryManager)InventoryManager.Instance()).GetInventorySlot(inventoryType, (int)inventorySlot)).GetItemId());
	}
}
