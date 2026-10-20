using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class WeaponCommand : ExecuteCommandBase
{
	public static void Draw()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleWeapon, 1u, 1u);
	}

	public static void Sheathe()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleWeapon, 0u, 1u);
	}

	public static void Toggle(bool isDraw)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleWeapon, isDraw ? 1u : 0u, 1u);
	}
}
