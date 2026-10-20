using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoGuardFishingState : ModuleBase
{
	private const uint INTERRUPT_FISHING_ACTION_ID = 299u;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoGuardFishingStateTitle"),
		Description = LanguageManager.Get("AutoGuardFishingStateDescription", LuminaWrapper.GetActionName(299u)),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(OnPreCommand);
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnPreCommand);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
	}

	protected override void ConfigUI()
	{
		ImGuiOmExtension.ConflictKeyText();
	}

	private static void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if ((int)actionType == 1 && actionID == 299)
		{
			FishingCommand.Quit();
			isPrevented = true;
		}
	}

	private static void OnPreCommand(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (command == ExecuteCommandFlag.Fishing && !PluginConfig.Instance().ConflictKeyBinding.IsPressed() && param1 == 1)
		{
			isPrevented = true;
		}
	}
}
