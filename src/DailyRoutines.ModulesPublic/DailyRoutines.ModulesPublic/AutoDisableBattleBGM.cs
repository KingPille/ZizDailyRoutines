using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic;

public class AutoDisableBattleBGM : ModuleBase
{
	private unsafe delegate byte IsInBattleDelegate(BGMSystem* system, Scene* scene);

	private class Config : ModuleConfig
	{
		public bool EnableInDuty;
	}

	private static readonly CompSig IsInBattleStateSig = new CompSig("E8 ?? ?? ?? ?? 38 87 ?? ?? ?? ?? 75 09");

	private Hook<IsInBattleDelegate>? IsInBattleStateHook;

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisableBattleBGMTitle"),
		Description = LanguageManager.Get("AutoDisableBattleBGMDescription"),
		Category = ModuleCategory.Combat
	};

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (IsInBattleStateHook == null)
		{
			IsInBattleStateHook = IsInBattleStateSig.GetHook<IsInBattleDelegate>(IsInBattleStateDetour);
		}
		IsInBattleStateHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoDisableBattleBGM-EnableInDuty")), ref config.EnableInDuty))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoDisableBattleBGM-EnableInDutyHelp"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
	}

	private unsafe byte IsInBattleStateDetour(BGMSystem* system, Scene* scene)
	{
		if (!config.EnableInDuty && GameState.ContentFinderCondition != 0)
		{
			return IsInBattleStateHook.Original(system, scene);
		}
		return 0;
	}
}
