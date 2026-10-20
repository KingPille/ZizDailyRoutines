using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoChangeKeyboardLayout : ModuleBase
{
	private unsafe delegate void SetTextInputTargetDelegate(AtkComponentTextInput* component, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData);

	private static class InputMethodController
	{
		private static Dictionary<ushort, KeyboardLayoutInfo>? allLayouts;

		public static nint CurrentLayout => GetKeyboardLayout(0u);

		[DllImport("user32.dll")]
		private static extern void ActivateKeyboardLayout(nint hkl, uint Flags);

		[DllImport("user32.dll")]
		private static extern nint GetKeyboardLayout(uint idThread);

		[DllImport("user32.dll")]
		private static extern int GetKeyboardLayoutList(int nBuff, nint[] lpList);

		[DllImport("user32.dll")]
		private static extern nint LoadKeyboardLayout(string pwszKLID, uint Flags);

		public static Dictionary<ushort, KeyboardLayoutInfo> GetAllKeyboardLayouts()
		{
			if (allLayouts != null)
			{
				return allLayouts;
			}
			allLayouts = new Dictionary<ushort, KeyboardLayoutInfo>();
			int keyboardLayoutList = GetKeyboardLayoutList(0, null);
			if (keyboardLayoutList == 0)
			{
				return allLayouts;
			}
			nint[] array = new nint[keyboardLayoutList];
			if (GetKeyboardLayoutList(keyboardLayoutList, array) == 0)
			{
				return allLayouts;
			}
			nint[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				nint handle = array2[i];
				ushort num = (ushort)(((IntPtr)handle).ToInt64() & 0xFFFF);
				string layoutDisplayName = GetLayoutDisplayName(num);
				KeyboardLayoutInfo value = new KeyboardLayoutInfo
				{
					Handle = handle,
					Name = layoutDisplayName,
					LangID = num
				};
				allLayouts[num] = value;
			}
			return allLayouts;
		}

		private static string GetLayoutDisplayName(ushort langID)
		{
			try
			{
				return new CultureInfo(langID).DisplayName;
			}
			catch
			{
				return string.Format($"0x{langID:X4}", default(ReadOnlySpan<object>));
			}
		}

		public static void SwitchToLayout(nint layoutHandle)
		{
			try
			{
				if (CurrentLayout != layoutHandle)
				{
					ActivateKeyboardLayout(layoutHandle, 0u);
				}
			}
			catch
			{
			}
		}

		public static nint FindKeyboardLayout(ushort langID)
		{
			int keyboardLayoutList = GetKeyboardLayoutList(0, null);
			if (keyboardLayoutList == 0)
			{
				return IntPtr.Zero;
			}
			nint[] array = new nint[keyboardLayoutList];
			if (GetKeyboardLayoutList(keyboardLayoutList, array) == 0)
			{
				return IntPtr.Zero;
			}
			nint[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				nint result = array2[i];
				if ((ushort)(((IntPtr)result).ToInt64() & 0xFFFF) == langID)
				{
					return result;
				}
			}
			return LoadKeyboardLayout($"{langID:X8}", 1u);
		}
	}

	private class Config : ModuleConfig
	{
		public ushort FocusLayoutLangID;

		public ushort UnfocusLayoutLangID;
	}

	private struct KeyboardLayoutInfo
	{
		public nint Handle;

		public string Name;

		public ushort LangID;
	}

	private static readonly CompSig SetTextInputTargetSig = new CompSig("4C 8B DC 55 53 57 41 54 41 57 49 8D AB ?? ?? ?? ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 85 ?? ?? ?? ?? 48 8B 9D ?? ?? ?? ??");

	private Hook<SetTextInputTargetDelegate>? SetTextInputTargetHook;

	private Config config;

	private Dictionary<ushort, KeyboardLayoutInfo>? cachedLayouts;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoChangeKeyboardLayoutTitle"),
		Description = LanguageManager.Get("AutoChangeKeyboardLayoutDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "JiaXX" }
	};

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		ushort num = (ushort)(((IntPtr)InputMethodController.CurrentLayout).ToInt64() & 0xFFFF);
		if (config.FocusLayoutLangID == 0)
		{
			config.FocusLayoutLangID = num;
			config.Save(this);
		}
		if (config.UnfocusLayoutLangID == 0)
		{
			config.UnfocusLayoutLangID = num;
			config.Save(this);
		}
		if (SetTextInputTargetHook == null)
		{
			SetTextInputTargetHook = SetTextInputTargetSig.GetHook<SetTextInputTargetDelegate>(ChangeKeyboardLayout);
		}
		SetTextInputTargetHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		if (Throttler.Shared.Throttle("AutoChangeKeyboardLayout-GetLayouts", 1000u))
		{
			cachedLayouts = InputMethodController.GetAllKeyboardLayouts();
		}
		if (cachedLayouts == null)
		{
			return;
		}
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Focused")));
		ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit("##FocusLayout"), ImU8String.op_Implicit(cachedLayouts.GetValueOrDefault(config.FocusLayoutLangID).Name ?? LanguageManager.Get("Unknown")));
		ushort key;
		KeyboardLayoutInfo value;
		try
		{
			if (ComboDisposable.op_Implicit(val))
			{
				foreach (KeyValuePair<ushort, KeyboardLayoutInfo> cachedLayout in cachedLayouts)
				{
					cachedLayout.Deconstruct(out key, out value);
					ushort num = key;
					KeyboardLayoutInfo keyboardLayoutInfo = value;
					bool flag = config.FocusLayoutLangID == num;
					if (ImGui.Selectable(ImU8String.op_Implicit(keyboardLayoutInfo.Name), flag, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						config.FocusLayoutLangID = num;
						config.Save(this);
					}
					if (flag)
					{
						ImGui.SetItemDefaultFocus();
					}
				}
			}
		}
		finally
		{
			((ComboDisposable)(ref val)).Dispose();
		}
		ImGui.Spacing();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Unfocused")));
		ComboDisposable val2 = ImRaii.Combo(ImU8String.op_Implicit("##UnfocusLayout"), ImU8String.op_Implicit(cachedLayouts.GetValueOrDefault(config.UnfocusLayoutLangID).Name ?? LanguageManager.Get("Unknown")));
		try
		{
			if (ComboDisposable.op_Implicit(val2))
			{
				foreach (KeyValuePair<ushort, KeyboardLayoutInfo> cachedLayout2 in cachedLayouts)
				{
					cachedLayout2.Deconstruct(out key, out value);
					ushort num2 = key;
					KeyboardLayoutInfo keyboardLayoutInfo2 = value;
					bool flag2 = config.UnfocusLayoutLangID == num2;
					if (ImGui.Selectable(ImU8String.op_Implicit(keyboardLayoutInfo2.Name), flag2, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						config.UnfocusLayoutLangID = num2;
						config.Save(this);
					}
					if (flag2)
					{
						ImGui.SetItemDefaultFocus();
					}
				}
			}
		}
		finally
		{
			((ComboDisposable)(ref val2)).Dispose();
		}
		ImGui.NewLine();
		ushort key2 = (ushort)(((IntPtr)InputMethodController.CurrentLayout).ToInt64() & 0xFFFF);
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(3, 2);
		((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Current"));
		((ImU8String)(ref val3)).AppendLiteral("\n\t\t");
		((ImU8String)(ref val3)).AppendFormatted<string>(cachedLayouts.GetValueOrDefault(key2).Name ?? LanguageManager.Get("Unknown"));
		ImGui.TextUnformatted(val3);
	}

	private unsafe void ChangeKeyboardLayout(AtkComponentTextInput* component, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		SetTextInputTargetHook.Original(component, eventType, eventParam, atkEvent, atkEventData);
		if ((int)eventType != 18)
		{
			if ((int)eventType == 19)
			{
				nint num = InputMethodController.FindKeyboardLayout(config.UnfocusLayoutLangID);
				if (num != IntPtr.Zero)
				{
					InputMethodController.SwitchToLayout(num);
				}
			}
		}
		else
		{
			DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
			{
				CheckSlashAndSwitchLayout(component);
			}, TimeSpan.FromMilliseconds(50L), 0, default(CancellationToken));
		}
	}

	private unsafe void CheckSlashAndSwitchLayout(AtkComponentTextInput* textInputEventInterface)
	{
		if (textInputEventInterface == null)
		{
			return;
		}
		AtkTextNode* atkTextNode = ((AtkComponentTextInput)textInputEventInterface).AtkTextNode;
		if (atkTextNode != null && !((object)(*(Utf8String*)(&((AtkTextNode)atkTextNode).NodeText))/*cast due to constrained. prefix*/).ToString().StartsWith('/'))
		{
			nint num = InputMethodController.FindKeyboardLayout(config.FocusLayoutLangID);
			if (num != IntPtr.Zero)
			{
				InputMethodController.SwitchToLayout(num);
			}
		}
	}
}
