using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class MountCommand : ExecuteCommandBase
{
	public static void Dismount(bool enqueue = false)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount, enqueue ? 1u : 0u);
	}

	public static void RidePillion(uint targetID, uint seatIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RidePillion, targetID, seatIndex);
	}

	public static void ApplyDisableMounting()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DisableMounting, 1u);
	}

	public static void RemoveDisableMounting()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DisableMounting);
	}

	public static void ToggleDisableMounting(bool isDisabled)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DisableMounting, isDisabled ? 1u : 0u);
	}
}
