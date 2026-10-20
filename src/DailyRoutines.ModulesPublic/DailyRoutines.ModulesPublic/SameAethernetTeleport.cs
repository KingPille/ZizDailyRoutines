using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using OmenTools.Interop.Game;

namespace DailyRoutines.ModulesPublic;

public class SameAethernetTeleport : ModuleBase
{
	private MemoryPatch patch0;

	private MemoryPatch patch1;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("SameAethernetTeleportTitle"),
		Description = LanguageManager.Get("SameAethernetTeleportDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true,
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		patch0 = new MemoryPatch("75 ?? 48 8B 49 ?? 48 8B 01 FF 50 ?? 48 8B C8 BA ?? ?? ?? ?? 48 83 C4 ?? 5E 5D", new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		patch1 = new MemoryPatch("75 ?? 48 8B 4E ?? 48 8B 01 FF 50 ?? 48 8B C8 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? E9 ?? ?? ?? ?? 80 7D", new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		patch0.Enable();
		patch1.Enable();
	}
}
