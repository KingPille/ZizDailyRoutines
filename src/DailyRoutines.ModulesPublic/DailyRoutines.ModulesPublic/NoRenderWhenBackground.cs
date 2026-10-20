using System;
using System.Runtime.InteropServices;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic;

public class NoRenderWhenBackground : ModuleBase
{
	private unsafe delegate void DeviceDX11PostTickDelegate(Device* device);

	private class Config : ModuleConfig
	{
		public bool OnlyProhibitedInIconic;
	}

	private static readonly CompSig DeviceDX11PostTickSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 41 54 41 55 41 56 41 57 B8 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 2B E0 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 8B 15");

	private Hook<DeviceDX11PostTickDelegate>? DeviceDX11PostTickHook;

	private Config config;

	private bool isOnNoRender;

	private long nextRenderTick;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("NoRenderWhenBackgroundTitle"),
		Description = LanguageManager.Get("NoRenderWhenBackgroundDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "Siren" }
	};

	protected unsafe override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)4, "NamePlate", new AddonEventDelegate(OnAddon));
		DeviceDX11PostTickHook = DeviceDX11PostTickSig.GetHook<DeviceDX11PostTickDelegate>(DeviceDX11PostTickDetour);
		DeviceDX11PostTickHook.Enable();
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
	}

	protected override void ConfigUI()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("NoRenderWhenBackground-OnlyProhibitedInIconic", LuminaWrapper.GetAddonText(4024u))), ref config.OnlyProhibitedInIconic))
		{
			config.Save(this);
		}
	}

	private void OnAddon(AddonEvent type, AddonArgs args)
	{
		if (isOnNoRender)
		{
			args.PreventOriginal();
		}
	}

	private unsafe void DeviceDX11PostTickDetour(Device* device)
	{
		if (GameState.IsForeground || !GameState.IsLoggedIn)
		{
			DeviceDX11PostTickHook.Original(device);
			isOnNoRender = false;
			return;
		}
		if (config.OnlyProhibitedInIconic && !IsIconic(((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle))
		{
			DeviceDX11PostTickHook.Original(device);
			isOnNoRender = false;
			return;
		}
		isOnNoRender = true;
		long tickCount = Environment.TickCount64;
		if (tickCount - nextRenderTick > 0)
		{
			nextRenderTick = tickCount + 5000;
			DeviceDX11PostTickHook.Original(device);
		}
		else if (((UIModule)UIModule.Instance()).ShouldLimitFps())
		{
			Thread.Sleep(50);
		}
	}

	[DllImport("user32.dll")]
	private static extern bool IsIconic(nint hWnd);
}
