using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class CraftCommand : ExecuteCommandBase
{
	public static void Craft(uint recipeID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Craft, 0u, recipeID);
	}

	public static void QuickSynthesis(uint recipeID, byte count)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Craft, 1u, recipeID, count);
	}

	public static void Practice(uint recipeID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Craft, 2u, recipeID);
	}
}
