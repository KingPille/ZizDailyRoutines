using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class GlamourPlateCommand : ExecuteCommandBase
{
	public static void ApplyPlate(uint plateIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ApplyGlamourPlate, plateIndex);
	}

	public static void Apply(uint glamourIndex, InventoryType type, ushort slot)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ApplyGlamourPlate, glamourIndex, (uint)(int)type, slot);
	}

	public static void Request()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGlamourPlate);
	}

	public static void DispellItems(uint dispellItemsSelectedBitmask)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DispellGlamours, dispellItemsSelectedBitmask);
	}

	public static void Enter()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGlamourPlate, 1u, 1u);
	}

	public static void Exit()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGlamourPlate, 0u, 1u);
	}

	public static void Toggle(bool isEnter)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGlamourPlate, isEnter ? 1u : 0u, 1u);
	}
}
