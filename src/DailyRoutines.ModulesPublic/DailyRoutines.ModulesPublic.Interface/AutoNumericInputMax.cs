using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Models;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoNumericInputMax : ModuleBase
{
	private unsafe delegate nint UldUpdateDelegate(AtkComponentNumericInput* component);

	private unsafe delegate void NumericSetValueDelegate(AtkComponentNumericInput* component, int value, bool a3, bool a4);

	private class Config : ModuleConfig
	{
		public bool AdjustMaximumValue = true;

		public int MaxValue = 999;
	}

	private static readonly CompSig UldUpdateSig = new CompSig("40 53 48 83 EC ?? 48 8B D9 48 83 C1 ?? E8 ?? ?? ?? ?? 80 BB ?? ?? ?? ?? ?? 74 ?? 48 8B CB");

	private Hook<UldUpdateDelegate>? UldUpdateHook;

	private static readonly CompSig NumericSetValueSig = new CompSig("E8 ?? ?? ?? ?? C7 83 ?? ?? ?? ?? ?? ?? ?? ?? 48 83 C4 ?? 5B C3 CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC CC 8B 91");

	private NumericSetValueDelegate? NumericSetValue;

	private Config config;

	private readonly Throttler<nint> throttler = new Throttler<nint>();

	private long lastInterruptTime;

	private bool isBlocked;

	private static readonly FrozenSet<string> BlacklistAddons;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNumericInputMaxTitle"),
		Description = LanguageManager.Get("AutoNumericInputMaxDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		if (config == null)
		{
			config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		}
		if (NumericSetValue == null)
		{
			NumericSetValue = Marshal.GetDelegateForFunctionPointer<NumericSetValueDelegate>(NumericSetValueSig.ScanText());
		}
		if (UldUpdateHook == null)
		{
			UldUpdateHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromSignature<UldUpdateDelegate>(UldUpdateSig.Get(), (UldUpdateDelegate)UldUpdateDetour, (HookBackend)0);
		}
		UldUpdateHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		ImGuiOmExtension.ConflictKeyText();
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoNumericInputMax-InterruptHelp"), 40f, (FontAwesomeIcon)61530);
		ImGui.Spacing();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoNumericInputMax-AdjustMax")), ref config.AdjustMaximumValue))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoNumericInputMax-AdjustMaxHelp"), 40f, (FontAwesomeIcon)61530);
		if (config.AdjustMaximumValue)
		{
			ImGui.SameLine();
			ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
			if (ImGui.InputInt(ImU8String.op_Implicit(LanguageManager.Get("AutoNumericInputMax-MaxValue")), ref config.MaxValue, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0))
			{
				config.MaxValue = Math.Clamp(config.MaxValue, 1, 9999);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoNumericInputMax-MaxValueInputHelp"), 40f, (FontAwesomeIcon)61530);
		}
	}

	private unsafe nint UldUpdateDetour(AtkComponentNumericInput* component)
	{
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		nint result = UldUpdateHook.Original(component);
		if (PluginConfig.Instance().ConflictKeyBinding.IsPressed())
		{
			lastInterruptTime = Environment.TickCount64;
		}
		if (Environment.TickCount64 - lastInterruptTime > 10000)
		{
			if (throttler.Throttle(IntPtr.Zero, 5000u))
			{
				Span<Pointer<AtkUnitBase>> entries = ((AtkUnitList)(&((AtkUnitManager)((RaptureAtkModule)RaptureAtkModule.Instance()).AtkUnitManager).FocusedUnitsList)).Entries;
				if (entries.Length == 0)
				{
					goto IL_01a5;
				}
				IEnumerable<string> source = from x in entries.ToArray()
					where x != Pointer<AtkUnitBase>.op_Implicit((AtkUnitBase*)null) && x.Value != null && !string.IsNullOrWhiteSpace(((AtkUnitBase)x.Value).NameString)
					select ((AtkUnitBase)x.Value).NameString;
				isBlocked = source.Any(BlacklistAddons.Contains);
			}
			if (!isBlocked && throttler.Throttle((nint)component, 250u))
			{
				int num = ((AtkUldComponentDataNumericInput)(&((AtkComponentNumericInput)component).Data)).Max;
				if (((AtkComponentNumericInput)component).AtkResNode != null && ((Enum)((AtkResNode)((AtkComponentNumericInput)component).AtkResNode).NodeFlags).HasFlag((Enum)(object)(NodeFlags)16) && num < 9999)
				{
					int num2 = num;
					if (num2 != 1)
					{
						if (num2 == 99 && config.AdjustMaximumValue)
						{
							num = config.MaxValue;
						}
						((AtkUldComponentDataNumericInput)(&((AtkComponentNumericInput)component).Data)).Max = num;
						NumericSetValue(component, num, a3: true, a4: false);
						((AtkComponentNumericInput)component).Value = num;
						((AtkTextNode)((AtkComponentNumericInput)component).AtkTextNode).SetNumber(num, false, false, (byte)0, false);
					}
				}
			}
		}
		goto IL_01a5;
		IL_01a5:
		return result;
	}

	static AutoNumericInputMax()
	{
		InlineArray7<string> buffer = default(InlineArray7<string>);
		buffer[0] = "RetainerSell";
		buffer[1] = "ItemSearch";
		buffer[2] = "LookingForGroupSearch";
		buffer[3] = "LookingForGroupCondition";
		buffer[4] = "CountDownSettingDialog";
		buffer[5] = "ConfigSystem";
		buffer[6] = "ConfigCharacter";
		BlacklistAddons = FrozenSet.Create<string>(buffer);
	}
}
