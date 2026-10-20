using System;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class AutoTalkSkip : ModuleBase
{
	private unsafe delegate AtkValue* ReceiveDialogueEventDelegate(AtkEventInterface* eventInterface, AtkValue* returnValue, AtkValue* values, uint valueCount);

	private unsafe delegate void HandleDialogueDelegate(DialogueCallback* callback, int valueCount, byte** text);

	private unsafe delegate void ShortTalkDelegate(EventSceneModule* eventSceneModule, nint target, nint text, nint speakerName, float duration, int unknown0, int unknown1, byte withLineVoice);

	private unsafe delegate void SchedulerTalkDelegate(SchedulerControl* control, nint line, nint text, nint target, nint result);

	private unsafe delegate void SchedulerCommandDelegate(AtkEventInterface* control, int command, AtkValue* values, uint valueCount, uint unknown);

	private unsafe delegate ushort OpenTalkDelegate(AgentCutscene* agentCutscene, uint valueCount, AtkValue* values, AtkEventInterface* eventInterface, ulong eventKind);

	[StructLayout(LayoutKind.Explicit)]
	private struct DialogueCallback
	{
		[FieldOffset(80)]
		public int Remaining;

		[FieldOffset(103)]
		public byte Finished;

		[FieldOffset(104)]
		public nint EventImpl0;

		[FieldOffset(112)]
		public nint EventImpl1;

		[FieldOffset(120)]
		public nint SelectedEventImpl;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct SchedulerControl
	{
		[FieldOffset(64)]
		public ushort Flags;

		[FieldOffset(112)]
		public nint TextFinishedCallback;

		[FieldOffset(120)]
		public nint TextFinishedContext;
	}

	private static readonly CompSig HandleDialogueSig = new CompSig("40 57 41 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 4D 8B F0");

	private static readonly CompSig ShortTalkSig = new CompSig("48 89 5C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 4C 89 74 24 ?? 55 48 8B EC 48 83 EC ?? 33 F6");

	private static readonly CompSig SchedulerTalkSig = new CompSig("40 55 57 41 54 41 55 41 56 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 4C 8B A4 24");

	private static readonly CompSig SchedulerCommandSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC ?? 48 8B D9 48 63 FA");

	private static readonly CompSig OpenTalkSig = new CompSig("40 53 55 56 57 41 54 41 56 41 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 4C 8B BC 24 ?? ?? ?? ?? 48 8B D9 48 8B 49 ?? 4D 8B F1 49 8B E8 8B F2");

	private static readonly CompSig ReceiveDialogueEventSig = new CompSig("40 53 48 83 EC ?? 4D 8B D0 48 8B DA 4C 8B C1");

	private Hook<HandleDialogueDelegate>? HandleDialogueHook;

	private Hook<ShortTalkDelegate>? ShortTalkHook;

	private Hook<SchedulerTalkDelegate>? SchedulerTalkHook;

	private Hook<SchedulerCommandDelegate>? SchedulerCommandHook;

	private Hook<OpenTalkDelegate>? OpenTalkHook;

	private Hook<OpenAddon>? OpenDialogueAddonHook;

	private ReceiveDialogueEventDelegate ReceiveDialogueEvent;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoTalkSkipTitle"),
		Description = LanguageManager.Get("AutoTalkSkipDescription"),
		Category = ModuleCategory.System
	};

	protected unsafe override void Init()
	{
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		ReceiveDialogueEvent = ReceiveDialogueEventSig.GetDelegate<ReceiveDialogueEventDelegate>();
		HandleDialogueHook = HandleDialogueSig.GetHook<HandleDialogueDelegate>(HandleDialogueDetour);
		HandleDialogueHook.Enable();
		ShortTalkHook = ShortTalkSig.GetHook<ShortTalkDelegate>(ShortTalkDetour);
		ShortTalkHook.Enable();
		SchedulerTalkHook = SchedulerTalkSig.GetHook<SchedulerTalkDelegate>(SchedulerTalkDetour);
		SchedulerTalkHook.Enable();
		SchedulerCommandHook = SchedulerCommandSig.GetHook<SchedulerCommandDelegate>(SchedulerCommandDetour);
		SchedulerCommandHook.Enable();
		OpenTalkHook = OpenTalkSig.GetHook<OpenTalkDelegate>(OpenTalkDetour);
		OpenTalkHook.Enable();
		OpenDialogueAddonHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<OpenAddon>((IntPtr)(nint)MemberFunctionPointers.OpenAddon, new OpenAddon(OpenDialogueAddonDetour), (HookBackend)0);
		OpenDialogueAddonHook.Enable();
	}

	private unsafe void HandleDialogueDetour(DialogueCallback* callback, int valueCount, byte** text)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (callback->Remaining <= 0)
		{
			callback->SelectedEventImpl = (nint)(&callback->EventImpl0);
		}
		else
		{
			callback->SelectedEventImpl = (nint)(&callback->EventImpl1);
			callback->Remaining--;
		}
		callback->Finished = 1;
		AtkValue val = default(AtkValue);
		ReceiveDialogueEvent((AtkEventInterface*)callback, &val, null, 0u);
	}

	private unsafe static void ShortTalkDetour(EventSceneModule* eventSceneModule, nint target, nint text, nint speakerName, float duration, int unknown0, int unknown1, byte withLineVoice)
	{
	}

	private unsafe static void SchedulerTalkDetour(SchedulerControl* control, nint line, nint text, nint target, nint result)
	{
		FinishSchedulerText(control);
		*(sbyte*)(result + 4) = 0;
	}

	private unsafe void SchedulerCommandDetour(AtkEventInterface* control, int command, AtkValue* values, uint valueCount, uint unknown)
	{
		if (command != 0)
		{
			SchedulerCommandHook.Original(control, command, values, valueCount, unknown);
		}
		else
		{
			FinishSchedulerText((SchedulerControl*)control);
		}
	}

	private unsafe static ushort OpenTalkDetour(AgentCutscene* agentCutscene, uint valueCount, AtkValue* values, AtkEventInterface* eventInterface, ulong eventKind)
	{
		return 0;
	}

	private unsafe ushort OpenDialogueAddonDetour(RaptureAtkModule* module, uint addonNameID, uint valueCount, AtkValue* values, AtkEventInterface* eventInterface, ulong eventKind, ushort parentAddonID, int depthLayer)
	{
		Span<Utf8String> span = ((RaptureAtkModule)module).AddonNames.AsSpan();
		if (addonNameID < span.Length)
		{
			ReadOnlySpan<byte> span2 = ((Utf8String)(ref span[(int)addonNameID])).AsSpan();
			if (span2.SequenceEqual("Talk"u8) || span2.SequenceEqual("TalkSubtitle"u8))
			{
				return 0;
			}
		}
		return OpenDialogueAddonHook.Original.Invoke(module, addonNameID, valueCount, values, eventInterface, eventKind, parentAddonID, depthLayer);
	}

	private unsafe static void FinishSchedulerText(SchedulerControl* control)
	{
		control->Flags = (ushort)((control->Flags & 0xFFF9) | 4);
		nint textFinishedCallback = control->TextFinishedCallback;
		if (textFinishedCallback != 0)
		{
			((delegate* unmanaged<nint, void>)textFinishedCallback)(control->TextFinishedContext);
		}
	}
}
