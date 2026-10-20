using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class AetheryteCommand : ExecuteCommandBase
{
	public static void RemoveFavorite(uint aetheryteID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveFavoriteAetheryte, aetheryteID);
	}

	public static void RemoveFree()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveFreeAetheryte);
	}

	public static void RemovePSPlusFree()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveFreeAetherytePSPlus);
	}

	public static void RemoveNSOFree()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveFreeAetheryteNSO);
	}
}
