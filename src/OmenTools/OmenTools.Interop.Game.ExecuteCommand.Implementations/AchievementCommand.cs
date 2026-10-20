using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class AchievementCommand : ExecuteCommandBase
{
	public static void Request(uint achievementRowID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestAchievement, achievementRowID);
	}

	public static void RequestCompleted()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestCompletedAchievement);
	}

	public static void RequestAll()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestAllAchievements);
	}

	public static void RequestNearCompletion()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestNearCompletedAchievement, 1u);
	}

	public static void RequestFateProgress(uint pageIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestFateProgressAchievement, pageIndex);
	}

	public static void RequestSpecial(uint achievementRowID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestAchievementSpecial, achievementRowID);
	}
}
