using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class RetainerCommand : ExecuteCommandBase
{
	public static void SetMarketPrice(uint slot, uint price)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetRetainerMarketPrice, slot, price);
	}

	public static void RequestTime()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestRetainerVentureTime);
	}

	public static void Return()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Retainer, 0u, 3u);
	}

	public static void Refresh()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Retainer, 0u, 4u);
	}

	public static void AssignVenture(uint retainerTaskID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Retainer, 0u, 5u, retainerTaskID);
	}

	public static void CancelVenture()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Retainer, 0u, 6u);
	}

	public static void CastGlamour(InventoryType targetInventoryType, uint targetInventorySlot, InventoryType sourceInventoryType, uint sourceInventorySlot)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		//IL_0013: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CastRetainerGlamour, (uint)(int)targetInventoryType, targetInventorySlot, (uint)(int)sourceInventoryType, sourceInventorySlot);
	}
}
