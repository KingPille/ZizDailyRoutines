using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class PrsimBoxCommand : ExecuteCommandBase
{
	public static void Request()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestPrismBox);
	}

	public static void Restore(uint prismBoxItemID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RestorePrsimBoxItem, prismBoxItemID);
	}

	public static void RestoreSetItem(uint prismBoxIndex, uint slotMaskLow, uint slotMaskHigh)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RestorePrsimBoxSetItem, prismBoxIndex, slotMaskLow, slotMaskHigh);
	}
}
