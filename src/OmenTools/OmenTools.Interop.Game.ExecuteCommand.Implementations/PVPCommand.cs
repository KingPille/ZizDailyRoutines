using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class PVPCommand : ExecuteCommandBase
{
	public enum Season : uint
	{
		Current,
		Previous
	}

	public static void SelectRoleAction(uint roleActionIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SelectPVPRoleAction, roleActionIndex);
	}

	public static void SendDuel(GameObjectId targetGameObjectID)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SendDuel, (uint)GameObjectId.op_Implicit(targetGameObjectID));
	}

	public static void ConfirmDuel()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestDuel);
	}

	public static void CancelDuel()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestDuel, 1u);
	}

	public static void ForceCancelDuel()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestDuel, 4u);
	}

	public static void ConfirmDuelDirect()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ConfirmDuel);
	}

	public static void CollectTrophyCrystal(Season season = Season.Current)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CollectTrophyCrystal, (uint)season);
	}

	public static void QuickChat(uint quickChatRowID, uint param1 = 0u, uint param2 = 0u)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SendPVPQuickChat, quickChatRowID, param1, param2);
	}
}
