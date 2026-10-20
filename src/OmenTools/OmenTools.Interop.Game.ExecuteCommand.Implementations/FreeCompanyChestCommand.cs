using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class FreeCompanyChestCommand : ExecuteCommandBase
{
	public static void DepositGil(uint amount)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DepositFreeCompanyChestGil, amount);
	}

	public static void WithdrawGil(uint amount)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.WithdrawFreeCompanyChestGil, amount);
	}

	public static void RequestLog()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestFreeCompanyChestLog);
	}
}
