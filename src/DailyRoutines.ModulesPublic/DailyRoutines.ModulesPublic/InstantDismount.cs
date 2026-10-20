using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class InstantDismount : ModuleBase
{
	private unsafe delegate bool DismountDelegate(nint a1, Vector3* location);

	private static readonly CompSig DismountSig = new CompSig("E8 ?? ?? ?? ?? 84 C0 75 ?? 4D 85 F6 0F 84 ?? ?? ?? ?? 49 8B 06");

	private Hook<DismountDelegate>? DismountHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("InstantDismountTitle"),
		Description = LanguageManager.Get("InstantDismountDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		if (DismountHook == null)
		{
			DismountHook = DismountSig.GetHook<DismountDelegate>(DismountDetour);
		}
		DismountHook.Enable();
	}

	private unsafe static bool DismountDetour(nint a1, Vector3* location)
	{
		OmenServiceBase<MovementManager>.Instance().Dismount();
		return false;
	}
}
