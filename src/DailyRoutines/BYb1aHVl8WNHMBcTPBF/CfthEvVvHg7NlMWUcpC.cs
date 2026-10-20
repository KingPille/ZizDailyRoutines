using System.Runtime.CompilerServices;
using DailyRoutines.Internal;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using itAnDoW4mgYVlrmtMor;

namespace BYb1aHVl8WNHMBcTPBF;

internal static class CfthEvVvHg7NlMWUcpC
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddon;
	}

	public static void lhGVxtIoo5()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		string text = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1976305562 ^ 0x4CEFDB1A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2);
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = ilQV3fMTPE;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)2, text, (AddonEventDelegate)obj2);
	}

	public static void K0uVpi72Qd()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = ilQV3fMTPE;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
	}

	private unsafe static void ilQV3fMTPE(AddonEvent type, AddonArgs args)
	{
		if (PluginConfig.Instance().SkipLogo && Addons.Logo != null)
		{
			int* ptr = (int*)Addons.Logo + 142;
			*ptr = 4;
		}
	}
}
