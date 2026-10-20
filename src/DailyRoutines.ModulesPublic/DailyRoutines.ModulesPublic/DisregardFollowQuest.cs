using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class DisregardFollowQuest : ModuleBase
{
	private delegate bool FollowTargetRecastDelegate(nint a1, nint a2, nint a3, nint a4, nint a5, nint a6);

	private static readonly CompSig FollowTargetRecastSig = new CompSig("48 89 5C 24 ?? 57 48 81 EC ?? ?? ?? ?? F3 41 0F 10 00");

	private Hook<FollowTargetRecastDelegate>? FollowTargetRecastHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("DisregardFollowQuestTitle"),
		Description = LanguageManager.Get("DisregardFollowQuestDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "Errer" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		if (FollowTargetRecastHook == null)
		{
			FollowTargetRecastHook = FollowTargetRecastSig.GetHook<FollowTargetRecastDelegate>(FollowTargetRecastDetour);
		}
		FollowTargetRecastHook.Enable();
	}

	private static bool FollowTargetRecastDetour(nint a1, nint a2, nint a3, nint a4, nint a5, nint a6)
	{
		return false;
	}
}
