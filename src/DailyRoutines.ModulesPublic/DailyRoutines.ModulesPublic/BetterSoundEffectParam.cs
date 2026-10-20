using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using OmenTools.Interop.Game;

namespace DailyRoutines.ModulesPublic;

public class BetterSoundEffectParam : ModuleBase
{
	private MemoryPatch channelCheckPatch;

	private MemoryPatch counterCheckPatch;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterSoundEffectParamTitle"),
		Description = LanguageManager.Get("BetterSoundEffectParamDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		channelCheckPatch = new MemoryPatch("0F 84 ?? ?? ?? ?? 48 63 4D", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[6]
		{
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		counterCheckPatch = new MemoryPatch("0F 86 ?? ?? ?? ?? 83 C1", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[6]
		{
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		channelCheckPatch.Enable();
		counterCheckPatch.Enable();
	}
}
