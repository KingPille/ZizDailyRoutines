using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace DailyRoutines.ModulesPublic;

public class IgnoreWindowMinSizeLimit : ModuleBase
{
	private int originalMinWidth = 1024;

	private int originalMinHeight = 720;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("IgnoreWindowMinSizeLimitTitle"),
		Description = LanguageManager.Get("IgnoreWindowMinSizeLimitDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "Siren" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		originalMinWidth = ((GameWindow)GameWindow.Instance()).MinWidth;
		originalMinHeight = ((GameWindow)GameWindow.Instance()).MinHeight;
		((GameWindow)GameWindow.Instance()).MinHeight = 1;
		((GameWindow)GameWindow.Instance()).MinWidth = 1;
	}

	protected unsafe override void Uninit()
	{
		if (base.IsInitialized)
		{
			((GameWindow)GameWindow.Instance()).MinWidth = originalMinWidth;
			((GameWindow)GameWindow.Instance()).MinHeight = originalMinHeight;
		}
	}
}
