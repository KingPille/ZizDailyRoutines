using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.NativeWrapper;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoCommenceDuty : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddonSetup;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCommenceDutyTitle"),
		Description = LanguageManager.Get("AutoCommenceDutyDescription"),
		Category = ModuleCategory.Duty,
		Author = new string[1] { "Cindy-Master" }
	};

	protected override void Init()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnAddonSetup;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddonSetup;
			_003C_003EO._003C0_003E__OnAddonSetup = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)1, "ContentsFinderConfirm", (AddonEventDelegate)obj2);
		IAddonLifecycle obj3 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj4 = _003C_003EO._003C0_003E__OnAddonSetup;
		if (obj4 == null)
		{
			AddonEventDelegate val2 = OnAddonSetup;
			_003C_003EO._003C0_003E__OnAddonSetup = val2;
			obj4 = (object)val2;
		}
		obj3.RegisterListener((AddonEvent)4, "ContentsFinderConfirm", (AddonEventDelegate)obj4);
	}

	protected override void Uninit()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnAddonSetup;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddonSetup;
			_003C_003EO._003C0_003E__OnAddonSetup = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
	}

	private unsafe static void OnAddonSetup(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.Addon == AtkUnitBasePtr.op_Implicit((IntPtr)IntPtr.Zero)))
		{
			AtkUnitBase* ptr = args.Addon.ToStruct();
			if (((AtkValue)((byte*)((AtkUnitBase)ptr).AtkValues + (nint)7 * (nint)Unsafe.SizeOf<AtkValue>())).UInt == 0)
			{
				(*((AddonContentsFinderConfirm)ptr).CommenceButton).Click();
			}
		}
	}
}
