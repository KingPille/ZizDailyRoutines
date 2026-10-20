using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class OrchestrionCommand : ExecuteCommandBase
{
	public static void SetPlaylist(uint playlistID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetOrchestrionPlaylist, playlistID);
	}

	public static void TogglePlay()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleOrchestrion);
	}

	public static void NextTrack()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.PlayNextOrchestrionTrack);
	}

	public static void PlayTrack(uint orchestrionID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.PlayOrchestrionTrack, orchestrionID);
	}
}
