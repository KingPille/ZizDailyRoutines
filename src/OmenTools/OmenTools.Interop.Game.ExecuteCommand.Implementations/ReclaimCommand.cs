using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class ReclaimCommand : ExecuteCommandBase
{
	public static void Clear()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ClearReclaimNotification);
	}

	public static void ReclaimItems()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ReclaimItems);
	}
}
