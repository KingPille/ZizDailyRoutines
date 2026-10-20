using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Sound;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using InteropGenerator.Runtime;
using OmenTools.Extensions;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoIgnoreLoginLock : ModuleBase
{
	private unsafe delegate byte TimerDelegate(void* timer, int intervalSecond, int retryCount);

	private Hook<Update> AgentLobbyUpdateHook;

	private Hook<OpenLoginWaitDialog> OpenLoginWaitDialogHook;

	private static readonly CompSig Timer0Sig = new CompSig("40 57 41 57 48 83 EC ?? 48 8B 05 ?? ?? ?? ?? 45 8B F8");

	private Hook<TimerDelegate> Timer0Hook;

	private static readonly CompSig Timer1Sig = new CompSig("40 53 57 48 83 EC ?? 48 8B F9 41 8B D8");

	private Hook<TimerDelegate> Timer1Hook;

	private Hook<PlaySystemSound> PlaySystemSoundHook;

	private MemoryPatch loginFallbackPatch;

	private MemoryPatch queueSelectOkNoFilterPatch;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoIgnoreLoginLockTitle"),
		Description = LanguageManager.Get("AutoIgnoreLoginLockDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		loginFallbackPatch = new MemoryPatch("48 81 BE ?? ?? ?? ?? ?? ?? ?? ?? 76", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[13]
		{
			(byte)72,
			(byte)129,
			(byte)190,
			(byte)80,
			(byte)18,
			0,
			0,
			(byte)232,
			(byte)3,
			0,
			0,
			(byte)118,
			(byte)22
		}));
		queueSelectOkNoFilterPatch = new MemoryPatch("01 4C 8B CE 44 88 64 24 40 4C 8B C3 44 88 64 24 38 48 8B D0 C7 44 24 30 0A 00 00 00 48 8B CF 66 44 89 64 24 28 48 C7 44 24 20 1E 00 00 00 E8 ?? ?? ?? ?? 4C 8B BC 24", new global::_003C_003Ez__ReadOnlySingleElementList<byte?>(0));
		AgentLobbyUpdateHook = (*((AgentLobby)AgentLobby.Instance()).VirtualTable).HookVFuncFromName<AgentLobbyVirtualTable, Update>("Update", new Update(AgentLobbyUpdateDetour));
		AgentLobbyUpdateHook.Enable();
		OpenLoginWaitDialogHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<OpenLoginWaitDialog>(typeof(MemberFunctionPointers), "OpenLoginWaitDialog", new OpenLoginWaitDialog(OpenLoginWaitDialogDetour));
		OpenLoginWaitDialogHook.Enable();
		Timer0Hook = Timer0Sig.GetHook<TimerDelegate>(Timer0Detour);
		Timer1Hook = Timer1Sig.GetHook<TimerDelegate>(Timer1Detour);
		PlaySystemSoundHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<PlaySystemSound>(typeof(MemberFunctionPointers), "PlaySystemSound", new PlaySystemSound(PlaySystemSoundDetour));
		Timer0Hook.Enable();
		Timer1Hook.Enable();
		PlaySystemSoundHook.Enable();
		loginFallbackPatch.Enable();
		queueSelectOkNoFilterPatch.Enable();
	}

	private unsafe void AgentLobbyUpdateDetour(AgentLobby* agent, uint deltaTime)
	{
		((AgentLobby)agent).TemporaryLocked = false;
		AgentLobbyUpdateHook.Original.Invoke(agent, deltaTime);
		((AgentLobby)agent).TemporaryLocked = false;
	}

	private unsafe void OpenLoginWaitDialogDetour(AgentLobby* thisPtr, int position)
	{
		Throttler.Shared.Throttle("AutoIgnoreLoginLock.SystemSound", 1500u, reThrottle: true);
		OpenLoginWaitDialogHook.Original.Invoke(thisPtr, position);
	}

	private unsafe byte Timer0Detour(void* timer, int intervalSecond, int retryCount)
	{
		return Timer0Hook.Original(timer, 1, retryCount);
	}

	private unsafe byte Timer1Detour(void* timer, int intervalSecond, int retryCount)
	{
		return Timer1Hook.Original(timer, 1, retryCount);
	}

	private unsafe SoundData* PlaySystemSoundDetour(SoundManager* soundManager, CStringPointer path, float volume, uint soundNumber, uint fadeInDuration, bool autoRelease, SoundVolumeCategory volumeCategory)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (Throttler.Shared.Check("AutoIgnoreLoginLock.SystemSound"))
		{
			return PlaySystemSoundHook.Original.Invoke(soundManager, path, volume, soundNumber, fadeInDuration, autoRelease, volumeCategory);
		}
		return null;
	}
}
