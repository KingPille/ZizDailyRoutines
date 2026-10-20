using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class AutoSkipBuddyFeedScene : ModuleBase
{
	private unsafe delegate void PlayFeedBuddySceneDelegate(HousingManager* manager);

	private static readonly CompSig PlayFeedBuddySceneSig = new CompSig("E8 ?? ?? ?? ?? 48 8B 5C 24 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 33 C0 48 83 C4 ?? C3 CC CC CC CC CC CC CC CC CC CC CC 48 83 EC");

	private Hook<PlayFeedBuddySceneDelegate>? PlayFeedBuddySceneHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSkipBuddyFeedSceneTitle"),
		Description = LanguageManager.Get("AutoSkipBuddyFeedSceneDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		if (PlayFeedBuddySceneHook == null)
		{
			PlayFeedBuddySceneHook = PlayFeedBuddySceneSig.GetHook<PlayFeedBuddySceneDelegate>(PlayFeedBuddySceneDetour);
		}
		PlayFeedBuddySceneHook.Enable();
	}

	private unsafe static void PlayFeedBuddySceneDetour(HousingManager* manager)
	{
	}
}
