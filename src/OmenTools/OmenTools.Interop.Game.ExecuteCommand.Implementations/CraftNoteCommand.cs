using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class CraftNoteCommand : ExecuteCommandBase
{
	public static void MarkSeenDivisionLevelRange(uint divisionIndex, uint levelRangeIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.MarkCraftDivisionLevelRangeSeen, divisionIndex, levelRangeIndex);
	}

	public static void LeaveQuickSynthesis()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LeaveQuickSynthesis);
	}
}
