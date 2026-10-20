using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using OmenTools.Interop.Game;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedPVPProfileAction : ModuleBase
{
	private MemoryPatch dragDropPatch;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedPVPProfileActionTitle"),
		Description = LanguageManager.Get("OptimizedPVPProfileActionDescription"),
		Category = ModuleCategory.Interface,
		ModulesPair = new string[1] { "BetterPVPACCommand" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		dragDropPatch = new MemoryPatch("83 E7 ?? E8 ?? ?? ?? ?? C1 E7", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[3]
		{
			(byte)106,
			(byte)1,
			(byte)95
		}));
		dragDropPatch.Enable();
	}
}
