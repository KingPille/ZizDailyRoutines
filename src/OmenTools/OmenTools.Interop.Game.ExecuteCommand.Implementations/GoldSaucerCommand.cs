using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class GoldSaucerCommand : ExecuteCommandBase
{
	public static void RequestGeneral()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGoldSaucerGeneral);
	}

	public static void RequestChocobo()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGoldSaucerChocobo);
	}

	public static void RequestLordOfVerminion()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGoldSaucerVerminion);
	}

	public static void ConfirmVerminionPalette()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ConfirmVerminionPalette);
	}

	public static void RequestMahjong()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGoldSaucerMahjong);
	}
}
