using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class BlueMagicCommand : ExecuteCommandBase
{
	public static void Apply(uint slotIndex, uint actionID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetBlueAction, 0u, slotIndex, actionID);
	}

	public static void Swap(uint slotIndex, uint targetSlotIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetBlueAction, 1u, slotIndex, targetSlotIndex);
	}
}
