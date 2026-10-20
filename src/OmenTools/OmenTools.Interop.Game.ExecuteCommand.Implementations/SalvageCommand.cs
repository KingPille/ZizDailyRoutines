using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public class SalvageCommand : ExecuteCommandBase
{
	public unsafe void Execute(InventoryType type, ushort slot)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EventFrameworkAction, 3735552u, (uint)(int)type, slot, ((InventoryItem)((InventoryManager)InventoryManager.Instance()).GetInventorySlot(type, (int)slot)).GetItemId());
	}
}
