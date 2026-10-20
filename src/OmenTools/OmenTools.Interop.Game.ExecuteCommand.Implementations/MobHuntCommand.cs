using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class MobHuntCommand : ExecuteCommandBase
{
	public static void AcceptBill(uint availableMarkIndex, uint markID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AcceptMobHuntBill, availableMarkIndex, markID);
	}

	public static void AbandonBill(uint availableMarkIndex, uint markID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AbandonMobHuntBill, availableMarkIndex, markID);
	}
}
