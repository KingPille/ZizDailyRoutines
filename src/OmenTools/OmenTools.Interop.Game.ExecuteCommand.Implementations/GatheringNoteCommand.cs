using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class GatheringNoteCommand : ExecuteCommandBase
{
	public static void RequestFishingNoteInfo(uint fishingNoteInfoID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestFishingNote, fishingNoteInfoID);
	}

	public static void RequestSpearfishNoteInfo(uint fishingNoteInfoID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestSpearfishNote, fishingNoteInfoID);
	}

	public static void RequestGatheringPoint(uint gatheringPointID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestGatheringPoint, gatheringPointID);
	}

	public static void MarkSeenGatherDivisionLevelRange(uint divisionIndex, uint levelRangeIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.MarkGatherDivisionLevelRangeSeen, divisionIndex, levelRangeIndex);
	}
}
