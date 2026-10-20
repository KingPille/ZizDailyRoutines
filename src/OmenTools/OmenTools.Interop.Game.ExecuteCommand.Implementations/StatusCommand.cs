using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class StatusCommand : ExecuteCommandBase
{
	public static void Remove(uint statusID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveStatus, statusID, 0u, 3758096384u);
	}

	public static void Remove(uint statusID, GameObjectId sourceGameObjectID)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveStatus, statusID, 0u, (uint)GameObjectId.op_Implicit(sourceGameObjectID));
	}
}
