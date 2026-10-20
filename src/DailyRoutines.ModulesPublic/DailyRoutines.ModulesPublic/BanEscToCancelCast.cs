using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class BanEscToCancelCast : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BanEscToCancelCastTitle"),
		Description = LanguageManager.Get("BanEscToCancelCastDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(OnPreUseCommand);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnPreUseCommand);
	}

	private static void OnPreUseCommand(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (command == ExecuteCommandFlag.CancelCast)
		{
			isPrevented = true;
		}
	}
}
