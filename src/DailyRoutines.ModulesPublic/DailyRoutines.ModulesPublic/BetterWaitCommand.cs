using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using OmenTools.Interop.Game;

namespace DailyRoutines.ModulesPublic;

public class BetterWaitCommand : ModuleBase
{
	private MemoryPatch waitSyntaxDecimalPatch;

	private MemoryPatch waitCommandDecimalPatch;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterWaitCommandTitle"),
		Description = LanguageManager.Get("BetterWaitCommandDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "Cindy-Master" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		waitSyntaxDecimalPatch = new MemoryPatch("F3 0F 58 05 ?? ?? ?? ?? F3 48 0F 2C C0 69 C8", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[24]
		{
			(byte)184,
			0,
			0,
			(byte)122,
			(byte)68,
			(byte)102,
			(byte)15,
			(byte)110,
			(byte)200,
			(byte)243,
			(byte)15,
			(byte)89,
			(byte)193,
			(byte)243,
			(byte)72,
			(byte)15,
			(byte)44,
			(byte)200,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		waitCommandDecimalPatch = new MemoryPatch("F3 0F 58 0D ?? ?? ?? ?? F3 48 0F 2C C1 69 C8", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[29]
		{
			(byte)184,
			0,
			0,
			(byte)122,
			(byte)68,
			(byte)102,
			(byte)15,
			(byte)110,
			(byte)192,
			(byte)243,
			(byte)15,
			(byte)89,
			(byte)200,
			(byte)243,
			(byte)72,
			(byte)15,
			(byte)44,
			(byte)201,
			(byte)144,
			(byte)137,
			(byte)75,
			(byte)88,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)235
		}));
		waitSyntaxDecimalPatch.Enable();
		waitCommandDecimalPatch.Enable();
	}
}
