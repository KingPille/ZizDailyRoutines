using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class FashionCheckCommand : ExecuteCommandBase
{
	public static void GetEntryReward()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CliamFashionCheckEntryReward);
	}

	public static void GetBonusReward()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ClaimFashionCheckBonusReward);
	}

	public static void AddEntryAndBonusReward()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ClaimFashionCheckNewGearReward);
	}
}
