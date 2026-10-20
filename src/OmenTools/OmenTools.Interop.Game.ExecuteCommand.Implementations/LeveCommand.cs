using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class LeveCommand : ExecuteCommandBase
{
	public static void Abandon(uint leveID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AbandonLeveQuest, leveID);
	}

	public static void Start(uint levequestID, uint levelToIncrease)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartLeveQuest, levequestID, levelToIncrease);
	}

	public static void Refresh()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RefreshLeveQuest);
	}

	public static void MarkReadyToAccept(uint leveID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.MarkLeveReadyToAccept, leveID);
	}
}
