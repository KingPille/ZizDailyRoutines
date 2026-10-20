using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Models;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class SoundEffectThrottler : ModuleBase
{
	private delegate void PlaySoundEffectDelegate(uint sound, nint a2, nint a3, byte a4);

	private class Config : ModuleConfig
	{
		public uint Throttle = 1000u;
	}

	private static readonly CompSig PlaySoundEffectSig = new CompSig("E9 ?? ?? ?? ?? C6 41 28 01");

	private Hook<PlaySoundEffectDelegate>? PlaySoundEffectHook;

	private Config? config;

	private long lastPlayTick;

	private readonly Throttler<uint> soundEffectThrottler = new Throttler<uint>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("SoundEffectThrottlerTitle"),
		Description = LanguageManager.Get("SoundEffectThrottlerDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (PlaySoundEffectHook == null)
		{
			PlaySoundEffectHook = PlaySoundEffectSig.GetHook<PlaySoundEffectDelegate>(PlaySoundEffectDetour);
		}
		PlaySoundEffectHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
		ImGui.InputUInt(ImU8String.op_Implicit(LanguageManager.Get("SoundEffectThrottler-Throttle")), ref config.Throttle, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Throttle = Math.Max(100u, config.Throttle);
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("SoundEffectThrottler-ThrottleHelp", config.Throttle), 40f, (FontAwesomeIcon)61530);
	}

	private void PlaySoundEffectDetour(uint sound, nint a2, nint a3, byte a4)
	{
		uint num = sound - 36;
		if (num <= 16)
		{
			if (Environment.TickCount64 == lastPlayTick)
			{
				PlaySoundEffectHook.Original(sound, a2, a3, a4);
			}
			else if (soundEffectThrottler.Throttle(num, config.Throttle))
			{
				PlaySoundEffectHook.Original(sound, a2, a3, a4);
				lastPlayTick = Environment.TickCount64;
			}
		}
		else
		{
			PlaySoundEffectHook.Original(sound, a2, a3, a4);
		}
	}
}
