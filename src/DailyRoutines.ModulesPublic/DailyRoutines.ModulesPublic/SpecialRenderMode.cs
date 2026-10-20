using System;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class SpecialRenderMode : ModuleBase
{
	private unsafe delegate void ToggleFadeDelegate(EnvironmentManager* manager, int a2, float fadeDuration, Vector4* fadeColor);

	private class Config : ModuleConfig
	{
		public Vector4 BackgroundColor = KnownColor.Chocolate.ToVector4();
	}

	private ToggleFadeDelegate ToggleFade;

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("SpecialRenderModeTitle"),
		Description = LanguageManager.Get("SpecialRenderModeDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		ToggleFade = new CompSig("E8 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 8F ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 8D 4C 24").GetDelegate<ToggleFadeDelegate>();
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
	}

	protected override void Uninit()
	{
		config?.Save(this);
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("SpecialRenderMode-Mode-DisableWorldRenderButAddons")));
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit("DisableWorldRenderButAddons"), true);
		try
		{
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				Vector4 backgroundColor = config.BackgroundColor;
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Enable")), default(Vector2)))
				{
					ToggleFade(((Framework)Framework.Instance()).EnvironmentManager, 1, 0.1f, &backgroundColor);
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Disable")), default(Vector2)))
				{
					ToggleFade(((Framework)Framework.Instance()).EnvironmentManager, 0, 0.1f, &backgroundColor);
				}
				ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
				ImU8String val3 = new ImU8String(1, 1);
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Color"));
				((ImU8String)(ref val3)).AppendLiteral(":");
				ImGui.TextUnformatted(val3);
				ImGui.SameLine();
				config.BackgroundColor = ImGuiComponents.ColorPickerWithPalette(1, string.Empty, config.BackgroundColor);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("SpecialRenderMode-Mode-HideAddonsButNameplate")));
		val = ImRaii.PushId(ImU8String.op_Implicit("HideAddonsButNameplate"), true);
		try
		{
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Enable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)91, false, true);
					((AtkUnitBase)Addons.DTR).IsVisible = false;
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Disable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)91, true, true);
					((AtkUnitBase)Addons.DTR).IsVisible = true;
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("SpecialRenderMode-Mode-HideAddonsButChatLog")));
		val = ImRaii.PushId(ImU8String.op_Implicit("HideAddonsButChatLog"), true);
		try
		{
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Enable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)87, false, true);
					((AtkUnitBase)Addons.DTR).IsVisible = false;
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Disable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)87, true, true);
					((AtkUnitBase)Addons.DTR).IsVisible = true;
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("SpecialRenderMode-Mode-HideChatLog")));
		val = ImRaii.PushId(ImU8String.op_Implicit("HideChatLog"), true);
		try
		{
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Enable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)8, false, true);
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Disable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)8, true, true);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("SpecialRenderMode-Mode-HideActionBars")));
		val = ImRaii.PushId(ImU8String.op_Implicit("HideActionBars"), true);
		try
		{
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Enable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)16, false, true);
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Disable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)16, true, true);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("SpecialRenderMode-Mode-HideTargetInfo")));
		val = ImRaii.PushId(ImU8String.op_Implicit("HideTargetInfo"), true);
		try
		{
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Enable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)64, false, true);
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Disable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)64, true, true);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("SpecialRenderMode-Mode-HideNameplate")));
		val = ImRaii.PushId(ImU8String.op_Implicit("HideNameplate"), true);
		try
		{
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Enable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)4, false, true);
				}
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Disable")), default(Vector2)))
				{
					((UIModule)UIModule.Instance()).ToggleUi((UiFlags)4, true, true);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
