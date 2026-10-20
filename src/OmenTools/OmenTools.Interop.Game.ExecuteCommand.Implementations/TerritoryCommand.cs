using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class TerritoryCommand : ExecuteCommandBase
{
	public enum TransportKind : uint
	{
		NPC = 1u,
		Boundary = 3u,
		Teleport = 4u,
		Return = 7u,
		Aethernet = 15u,
		Housing = 20u
	}

	public enum InnerTransportKind : uint
	{
		None = 0u,
		Cutscene = 1u,
		ReturnToSafeArea = 2u,
		DutyTransport = 25u,
		Diving = 26u
	}

	public static void Transport(TransportKind transportKind, InnerTransportKind innerTransportKind = InnerTransportKind.None)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartTerritoryTransport, (uint)transportKind, (uint)innerTransportKind);
	}

	public static void FinishTransport()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishTerritoryTransport);
	}
}
