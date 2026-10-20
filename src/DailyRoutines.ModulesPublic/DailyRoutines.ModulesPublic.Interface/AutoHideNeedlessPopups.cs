using System.Collections.Frozen;
using System.Collections.Generic;
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

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoHideNeedlessPopups : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddon;
	}

	private static readonly FrozenSet<string> AddonNames;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoHideNeedlessPopupsTitle"),
		Description = LanguageManager.Get("AutoHideNeedlessPopupsDescription"),
		Category = ModuleCategory.Interface
	};

	protected override void Init()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		FrozenSet<string> addonNames = AddonNames;
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)17, (IEnumerable<string>)addonNames, (AddonEventDelegate)obj2);
		IAddonLifecycle obj3 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		FrozenSet<string> addonNames2 = AddonNames;
		object obj4 = _003C_003EO._003C0_003E__OnAddon;
		if (obj4 == null)
		{
			AddonEventDelegate val2 = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val2;
			obj4 = (object)val2;
		}
		obj3.RegisterListener((AddonEvent)4, (IEnumerable<string>)addonNames2, (AddonEventDelegate)obj4);
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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		AtkUnitBase* address = (AtkUnitBase*)args.Addon.Address;
		if (address != null)
		{
			if ((int)type == 17)
			{
				args.PreventOriginal();
			}
			else
			{
				((AtkUnitBase)address).Hide(true, true, 1u);
			}
		}
	}

	static AutoHideNeedlessPopups()
	{
		InlineArray10<string> buffer = default(InlineArray10<string>);
		buffer[0] = "_NotificationCircleBook";
		buffer[1] = "_NotificationAchieveLogIn";
		buffer[2] = "_NotificationAchieveZoneIn";
		buffer[3] = "AchievementInfo";
		buffer[4] = "RecommendList";
		buffer[5] = "PlayGuide";
		buffer[6] = "HowTo";
		buffer[7] = "WebLauncher";
		buffer[8] = "LicenseViewer";
		buffer[9] = "WKSEnterInfo";
		AddonNames = FrozenSet.Create<string>(buffer);
	}
}
