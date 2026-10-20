using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoResumeQuestRedo : ModuleBase
{
	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoResumeQuestRedoTitle"),
		Description = LanguageManager.Get("AutoResumeQuestRedoDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		OmenServiceBase<GameState>.Instance().Login += OnLogin;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<GameState>.Instance().Login -= OnLogin;
	}

	private static void OnZoneChanged(uint zone)
	{
		if (GameState.ContentFinderCondition != 0)
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ContinueQuestRedo);
		}
	}

	private static void OnLogin()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ContinueQuestRedo);
	}
}
