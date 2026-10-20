using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class AutoBlockShutdownFromLobbyError : ModuleBase
{
	private unsafe delegate bool AtkMessageBoxReceiveEventDelegate(AtkMessageBoxManager* manager, nint a2, AtkValue* values);

	private static readonly CompSig AtkMessageBoxReceiveEventSig = new CompSig("40 53 48 83 EC 30 48 8B D9 49 8B C8 E8 ?? ?? ?? ?? 8B D0");

	private Hook<AtkMessageBoxReceiveEventDelegate>? AtkMessageBoxReceiveEventHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoBlockShutdownFromLobbyErrorTitle"),
		Description = LanguageManager.Get("AutoBlockShutdownFromLobbyErrorDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		if (AtkMessageBoxReceiveEventHook == null)
		{
			AtkMessageBoxReceiveEventHook = AtkMessageBoxReceiveEventSig.GetHook<AtkMessageBoxReceiveEventDelegate>(AtkMessageBoxReceiveEventDetour);
		}
		AtkMessageBoxReceiveEventHook.Enable();
	}

	private unsafe bool AtkMessageBoxReceiveEventDetour(AtkMessageBoxManager* manager, nint a2, AtkValue* values)
	{
		((AtkValue)values).UInt = 16000u;
		return AtkMessageBoxReceiveEventHook.Original(manager, a2, values);
	}
}
