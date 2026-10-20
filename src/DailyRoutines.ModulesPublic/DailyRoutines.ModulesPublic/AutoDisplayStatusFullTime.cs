using System;
using System.Numerics;
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
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using InteropGenerator.Runtime;
using OmenTools.Extensions;
using OmenTools.Interop.Game;

namespace DailyRoutines.ModulesPublic;

public class AutoDisplayStatusFullTime : ModuleBase
{
	private class Config : ModuleConfig
	{
		public Mode Mode;
	}

	private enum Mode
	{
		FullSecond,
		FullMinute,
		Full
	}

	private Hook<FormatTimeSpan> FormatTimeSpanHook;

	private MemoryPatch updateIntervalPatch;

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayStatusFullTimeTitle"),
		Description = LanguageManager.Get("AutoDisplayStatusFullTimeDescription"),
		Category = ModuleCategory.Combat
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		updateIntervalPatch = new MemoryPatch("85 D2 74 ?? 83 FA ?? 73 ?? 41 3B D0", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[4]
		{
			(byte)176,
			(byte)1,
			(byte)195,
			(byte)144
		}));
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		updateIntervalPatch.Enable();
		FormatTimeSpanHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<FormatTimeSpan>(typeof(MemberFunctionPointers), "FormatTimeSpan", new FormatTimeSpan(FormatTimeSpanDetour));
		FormatTimeSpanHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Mode"));
		ComboDisposable val2 = ImRaii.Combo(val, ImU8String.op_Implicit(LanguageManager.Get($"AutoDisplayStatusFullTime-Mode-{config.Mode}")));
		try
		{
			if (!ComboDisposable.op_Implicit(val2))
			{
				return;
			}
			Mode[] values = Enum.GetValues<Mode>();
			foreach (Mode mode in values)
			{
				if (ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get($"AutoDisplayStatusFullTime-Mode-{mode}")), mode == config.Mode, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					config.Mode = mode;
					config.Save(this);
				}
			}
		}
		finally
		{
			((ComboDisposable)(ref val2)).Dispose();
		}
	}

	private unsafe CStringPointer FormatTimeSpanDetour(RaptureTextModule* thisPtr, uint seconds, bool alternativeMinutesGlyph)
	{
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		string text = string.Empty;
		switch (config.Mode)
		{
		case Mode.FullSecond:
			text = seconds.ToString();
			break;
		case Mode.FullMinute:
			var (num4, value3) = Math.DivRem(seconds, 60u);
			if (num4 != 0)
			{
				text = $"{num4:D2}:{value3:D2}";
				break;
			}
			goto case Mode.FullSecond;
		case Mode.Full:
		{
			uint num = seconds / 3600;
			uint num2 = seconds / 60 % 60;
			uint value = seconds % 60;
			uint value2 = num2;
			uint num3 = num;
			if (num3 != 0)
			{
				text = $"{num3:D2}:{value2:D2}:{value:D2}";
				break;
			}
			goto case Mode.FullMinute;
		}
		}
		if (string.IsNullOrEmpty(text))
		{
			return FormatTimeSpanHook.Original.Invoke(thisPtr, seconds, alternativeMinutesGlyph);
		}
		Utf8String val = default(Utf8String);
		((Utf8String)(ref val))._002Ector(text);
		try
		{
			Utf8String val2 = ((RaptureTextModule)thisPtr).UnkStrings0[0];
			((Utf8String)(ref val2)).Clear();
			((Utf8String)(ref val2)).Copy(&val);
			return val2.StringPtr;
		}
		finally
		{
			((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
