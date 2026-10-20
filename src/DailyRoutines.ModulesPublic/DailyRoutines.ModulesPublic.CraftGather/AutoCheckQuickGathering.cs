using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;

namespace DailyRoutines.ModulesPublic.CraftGather;

public class AutoCheckQuickGathering : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddon;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCheckQuickGatheringTitle"),
		Description = LanguageManager.Get("AutoCheckQuickGatheringDescription"),
		Category = ModuleCategory.CraftGather
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)1, "Gathering", (AddonEventDelegate)obj2);
	}

	protected override void Uninit()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
	}

	private unsafe static void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* ptr = args.Addon.ToStruct();
		if (ptr != null)
		{
			(*ptr).Callback(130, true);
		}
	}
}
