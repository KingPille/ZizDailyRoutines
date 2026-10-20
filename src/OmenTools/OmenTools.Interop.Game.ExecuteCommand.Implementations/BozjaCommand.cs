using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class BozjaCommand : ExecuteCommandBase
{
	public static void UseFromHolster(uint holsterIndex, uint slotIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AssignBozjaActionFromHolster, holsterIndex, slotIndex);
	}
}
