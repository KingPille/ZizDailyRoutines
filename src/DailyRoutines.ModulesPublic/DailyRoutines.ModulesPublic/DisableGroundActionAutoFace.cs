using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using OmenTools.Interop.Game;

namespace DailyRoutines.ModulesPublic;

public class DisableGroundActionAutoFace : ModuleBase
{
	private MemoryPatch groundActionAutoFacePatch;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("DisableGroundActionAutoFaceTitle"),
		Description = LanguageManager.Get("DisableGroundActionAutoFaceDescription"),
		Category = ModuleCategory.Action
	};

	protected override void Init()
	{
		groundActionAutoFacePatch = new MemoryPatch("74 ?? 48 8D 8E ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 75 ?? 48 8B 55", new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		groundActionAutoFacePatch.Enable();
	}
}
