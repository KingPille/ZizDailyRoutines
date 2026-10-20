using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic.Interface;

public class NoUIFade : ModuleBase
{
	private unsafe delegate void FadeMiddleBackDrawDelegate(AtkUnitBase* addon);

	private delegate nint WhiteFadeInDelegate();

	private delegate nint WhiteFadeOutDelegate();

	private delegate nint EventFadeInDelegate(nint a1);

	private delegate nint EventFadeOutDelegate(nint a1, int a2, int a3);

	private static readonly CompSig FadeMiddleBackDrawSig = new CompSig("48 83 EC 28 80 B9 ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 80 B9 ?? ?? ?? ?? ??");

	private Hook<FadeMiddleBackDrawDelegate>? FadeMiddleBackDrawHook;

	private static readonly CompSig WhiteFadeInSig = new CompSig("40 53 48 83 EC ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 4C 24 ?? BA ?? ?? ?? ?? E8 ?? ?? ?? ?? C7 44 24 ?? ?? ?? ?? ?? 48 8B D8 C7 44 24 ?? ?? ?? ?? ?? 48 C7 44 24 ?? ?? ?? ?? ?? 85 C0 78 ?? 83 F8 ?? 72 ?? 33 DB 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B CB 41 B8 ?? ?? ?? ?? 4C 8B 08 8B 54 8C ?? 48 8B C8 41 FF 91 ?? ?? ?? ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 33 C0 48 8B 4C 24 ?? 48 33 CC E8 ?? ?? ?? ?? 48 83 C4 ?? 5B C3 CC CC CC CC CC CC CC 40 53");

	private Hook<WhiteFadeInDelegate>? WhiteFadeInHook;

	private static readonly CompSig WhiteFadeOutSig = new CompSig("40 53 48 83 EC ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 4C 24 ?? BA ?? ?? ?? ?? E8 ?? ?? ?? ?? C7 44 24 ?? ?? ?? ?? ?? 48 8B D8 C7 44 24 ?? ?? ?? ?? ?? 48 C7 44 24 ?? ?? ?? ?? ?? 85 C0 78 ?? 83 F8 ?? 72 ?? 33 DB 48 8B 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B CB 41 B8 ?? ?? ?? ?? 4C 8B 08 8B 54 8C ?? 48 8B C8 41 FF 91 ?? ?? ?? ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 33 C0 48 8B 4C 24 ?? 48 33 CC E8 ?? ?? ?? ?? 48 83 C4 ?? 5B C3 CC CC CC CC CC CC CC 48 89 5C 24");

	private Hook<WhiteFadeOutDelegate>? WhiteFadeOutHook;

	private static readonly CompSig EventFadeInSig = new CompSig("48 89 5C 24 ?? 57 48 83 EC ?? 48 8B 01 41 8B D8 8B FA FF 90 ?? ?? ?? ?? 48 8B C8 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 ?? 44 8B CB 44 8B C7 33 D2");

	private Hook<EventFadeInDelegate>? EventFadeInHook;

	private static readonly CompSig EventFadeOutSig = new CompSig("48 89 5C 24 ?? 57 48 83 EC ?? 48 8B 01 41 8B D8 8B FA FF 90 ?? ?? ?? ?? 48 8B C8 BA ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 ?? 44 8B CB 44 8B C7 BA");

	private Hook<EventFadeOutDelegate>? EventFadeOutHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("NoUIFadeTitle"),
		Description = LanguageManager.Get("NoUIFadeDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		if (FadeMiddleBackDrawHook == null)
		{
			FadeMiddleBackDrawHook = FadeMiddleBackDrawSig.GetHook<FadeMiddleBackDrawDelegate>(FadeMiddleBackDrawDetour);
		}
		FadeMiddleBackDrawHook.Enable();
		if (WhiteFadeInHook == null)
		{
			WhiteFadeInHook = WhiteFadeInSig.GetHook<WhiteFadeInDelegate>(WhiteFadeInDetour);
		}
		WhiteFadeInHook.Enable();
		if (WhiteFadeOutHook == null)
		{
			WhiteFadeOutHook = WhiteFadeOutSig.GetHook<WhiteFadeOutDelegate>(WhiteFadeOutDetour);
		}
		WhiteFadeOutHook.Enable();
		if (EventFadeInHook == null)
		{
			EventFadeInHook = EventFadeInSig.GetHook<EventFadeInDelegate>(EventFadeInDetour);
		}
		EventFadeInHook.Enable();
		if (EventFadeOutHook == null)
		{
			EventFadeOutHook = EventFadeOutSig.GetHook<EventFadeOutDelegate>(EventFadeOutDetour);
		}
		EventFadeOutHook.Enable();
	}

	private unsafe static void FadeMiddleBackDrawDetour(AtkUnitBase* addon)
	{
	}

	private static nint WhiteFadeInDetour()
	{
		return IntPtr.Zero;
	}

	private static nint WhiteFadeOutDetour()
	{
		return IntPtr.Zero;
	}

	private static nint EventFadeInDetour(nint a1)
	{
		return IntPtr.Zero;
	}

	private static nint EventFadeOutDetour(nint a1, int a2, int a3)
	{
		return IntPtr.Zero;
	}
}
