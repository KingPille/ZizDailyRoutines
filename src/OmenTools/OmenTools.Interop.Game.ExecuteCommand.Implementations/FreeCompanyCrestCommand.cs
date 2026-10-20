using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class FreeCompanyCrestCommand : ExecuteCommandBase
{
	public enum BatchTarget : uint
	{
		Armory = 5u,
		Inventory
	}

	public static void Apply(InventoryType inventoryType, uint inventorySlot, bool isAttach)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleFreeCompanyCrestDecal, (uint)(int)inventoryType, inventorySlot, isAttach ? 1u : 0u);
	}

	public static void ApplyBatchEquipped(bool isAttach)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleFreeCompanyCrestDecalBatchEquipped, isAttach ? 1u : 0u);
	}

	public static void ApplyBatch(BatchTarget target, bool isAttach)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleFreeCompanyCrestDecalBatch, (uint)target, isAttach ? 1u : 0u);
	}
}
