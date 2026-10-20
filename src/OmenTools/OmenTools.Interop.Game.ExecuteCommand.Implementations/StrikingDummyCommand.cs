using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class StrikingDummyCommand : ExecuteCommandBase
{
	public static void Reset(uint strikingDummyobjectID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ResetStrikingDummy, strikingDummyobjectID);
	}
}
