using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class NoviceNetworkCommand : ExecuteCommandBase
{
	public static void DismissNovice()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DissmissNoviceState);
	}

	public static void EnableNovice()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetNoviceState);
	}

	public static void ToggleMentorAutoJoin()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetAutoJoinNoviceNetworkMentor);
	}

	public static void AcceptInvitation()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AcceptNoviceNetworkInvitation);
	}

	public static void RejectInvitation()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AcceptNoviceNetworkInvitation, 1u);
	}

	public static void DismissReturner()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DismissReturnerState);
	}

	public static void Request()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RefreshNoviceNetwork);
	}

	public static void JoinAsReturner()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.JoinNoviceNetworkReturner);
	}
}
