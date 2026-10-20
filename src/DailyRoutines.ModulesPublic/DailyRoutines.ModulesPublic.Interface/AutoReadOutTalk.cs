using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.LocalFixes;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoReadOutTalk : ModuleBase
{
	private unsafe delegate void ShowBattleTalkDelegate(UIModule* module, CStringPointer name, CStringPointer text, float duration, byte style);

	private unsafe delegate void ShowBattleTalkImageDelegate(UIModule* module, CStringPointer name, CStringPointer text, float duration, uint image, byte style, int sound, uint entityID);

	private class Config : ModuleConfig
	{
		public string Format = "{0}: {1}";
	}

	private Config config;

	private CancellationTokenSource? speechCancellationSource;

	private Hook<ShowBattleTalkDelegate>? ShowBattleTalkHook;

	private Hook<ShowBattleTalkImageDelegate>? ShowBattleTalkImageHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoReadOutTalkTitle"),
		Description = LanguageManager.Get("AutoReadOutTalkDescription"),
		Category = ModuleCategory.Interface,
		ModulesConflict = new string[1] { "AutoTalkSkip" }
	};

	protected unsafe override void Init()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)10, "Talk", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "Talk", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)19, "Talk", new AddonEventDelegate(OnAddon));
		ShowBattleTalkHook = (*((UIModule)UIModule.Instance()).VirtualTable).HookVFuncFromName<UIModuleVirtualTable, ShowBattleTalkDelegate>("ShowBattleTalk", (ShowBattleTalkDelegate)ShowBattleTalkDetour);
		ShowBattleTalkHook.Enable();
		ShowBattleTalkImageHook = (*((UIModule)UIModule.Instance()).VirtualTable).HookVFuncFromName<UIModuleVirtualTable, ShowBattleTalkImageDelegate>("ShowBattleTalkImage", (ShowBattleTalkImageDelegate)ShowBattleTalkImageDetour);
		ShowBattleTalkImageHook.Enable();
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		CancelBefore();
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Format")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(13, 0);
			((ImU8String)(ref val2)).AppendLiteral("##FormatInput");
			ImGui.InputText(val2, ref config.Format, 512, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void ShowBattleTalkDetour(UIModule* module, CStringPointer name, CStringPointer text, float duration, byte style)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		ShowBattleTalkHook.Original(module, name, text, duration, style);
		string text2 = (((CStringPointer)(ref name)).HasValue ? CStringExtensions.ExtractText(name) : string.Empty);
		string text3 = (((CStringPointer)(ref text)).HasValue ? CStringExtensions.ExtractText(text) : string.Empty);
		if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text2) && !(duration < 3f))
		{
			SpeakText(SafeFormatting.Speech(config.Format, text2, text3));
		}
	}

	private unsafe void ShowBattleTalkImageDetour(UIModule* module, CStringPointer name, CStringPointer text, float duration, uint image, byte style, int sound, uint entityID)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ShowBattleTalkImageHook.Original(module, name, text, duration, image, style, sound, entityID);
		if (sound <= -1)
		{
			string text2 = (((CStringPointer)(ref name)).HasValue ? CStringExtensions.ExtractText(name) : string.Empty);
			string text3 = (((CStringPointer)(ref text)).HasValue ? CStringExtensions.ExtractText(text) : string.Empty);
			if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text2) && !(duration < 3f))
			{
				SpeakText(SafeFormatting.Speech(config.Format, text2, text3));
			}
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Invalid comparison between Unknown and I4
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Invalid comparison between Unknown and I4
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 6)
		{
			if ((int)type == 10)
			{
				string text = null;
				string name = null;
				if (Addons.Talk == null || (int)((AtkValue)((AtkUnitBase)Addons.Talk).AtkValues).Type != 40 || !((CStringPointer)(&((AtkValue)((AtkUnitBase)Addons.Talk).AtkValues).String)).HasValue)
				{
					return;
				}
				if ((int)((AtkValue)((byte*)((AtkUnitBase)Addons.Talk).AtkValues + Unsafe.SizeOf<AtkValue>())).Type == 40 && ((CStringPointer)(&((AtkValue)((byte*)((AtkUnitBase)Addons.Talk).AtkValues + Unsafe.SizeOf<AtkValue>())).String)).HasValue)
				{
					name = CStringExtensions.ExtractText(((AtkValue)((byte*)((AtkUnitBase)Addons.Talk).AtkValues + Unsafe.SizeOf<AtkValue>())).String);
				}
				if ((int)((AtkValue)((byte*)((AtkUnitBase)Addons.Talk).AtkValues + (nint)3 * (nint)Unsafe.SizeOf<AtkValue>())).Type == 5 && ((AtkValue)((byte*)((AtkUnitBase)Addons.Talk).AtkValues + (nint)3 * (nint)Unsafe.SizeOf<AtkValue>())).UInt == 0)
				{
					text = CStringExtensions.ExtractText(((AtkValue)((AtkUnitBase)Addons.Talk).AtkValues).String);
					if (!string.IsNullOrEmpty(text))
					{
						SpeakText(SafeFormatting.Speech(config.Format, name, text));
					}
				}
				return;
			}
			if ((int)type != 19)
			{
				return;
			}
		}
		CancelBefore();
	}

	private void SpeakText(string text)
	{
		CancelBefore();
		speechCancellationSource = new CancellationTokenSource();
		NotifyHelper.SpeakAsync(text, speechCancellationSource.Token);
	}

	private void CancelBefore()
	{
		speechCancellationSource?.Cancel();
		speechCancellationSource?.Dispose();
		speechCancellationSource = null;
	}
}
