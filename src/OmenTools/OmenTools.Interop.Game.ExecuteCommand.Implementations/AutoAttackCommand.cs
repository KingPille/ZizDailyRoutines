using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class AutoAttackCommand : ExecuteCommandBase
{
	public static void Enable(uint targetID, bool isManual = false)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleAutoAttack, 1u, targetID, isManual ? 1u : 0u);
	}

	public static void Disable(bool isManual = false)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleAutoAttack, 0u, 3758096384u, isManual ? 1u : 0u);
	}

	public static void Set(bool isEnabled, uint targetObjectID, bool isManual = false)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleAutoAttack, isEnabled ? 1u : 0u, targetObjectID, isManual ? 1u : 0u);
	}
}
