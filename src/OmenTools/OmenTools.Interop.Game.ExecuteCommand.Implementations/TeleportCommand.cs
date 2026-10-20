using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class TeleportCommand : ExecuteCommandBase
{
	public static void Teleport(uint aetheryteID, uint aetheryteSubID, bool useTicket)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Teleport, aetheryteID, useTicket ? 1u : 0u, aetheryteSubID);
	}

	public static void AcceptOffer()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AcceptTeleportOffer);
	}

	public static void Cancel()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CancelTeleport);
	}
}
