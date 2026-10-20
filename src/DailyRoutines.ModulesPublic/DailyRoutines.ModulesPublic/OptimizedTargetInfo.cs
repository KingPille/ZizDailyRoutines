using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.LocalFixes;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Data;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class OptimizedTargetInfo : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AlignLeft;

		public bool CastBarAlignLeft;

		public Vector4 CastBarCustomColor = new Vector4(1f, 1f, 1f, 0f);

		public byte CastBarFontSize = 14;

		public bool CastBarIsEnabled = true;

		public Vector4 CastBarOutlineColor = new Vector4(0.7137255f, 41f / 85f, 16f / 51f, 1f);

		public Vector2 CastBarPosition = new Vector2(0f);

		public bool ClearFocusIsEnabled = true;

		public Vector2 ClearFocusPosition = new Vector2(0f);

		public Vector4 CustomColor = new Vector4(1f, 1f, 1f, 0f);

		public DisplayFormat DisplayFormat = DisplayFormat.ChineseOnePrecision;

		public string DisplayFormatString = "{0} / {1}";

		public bool FocusAlignLeft;

		public bool FocusCastBarAlignLeft;

		public Vector4 FocusCastBarCustomColor = new Vector4(1f, 1f, 1f, 0f);

		public byte FocusCastBarFontSize = 14;

		public bool FocusCastBarIsEnabled = true;

		public Vector4 FocusCastBarOutlineColor = new Vector4(0.7137255f, 41f / 85f, 16f / 51f, 1f);

		public Vector2 FocusCastBarPosition = new Vector2(0f);

		public Vector4 FocusCustomColor = new Vector4(1f, 1f, 1f, 0f);

		public byte FocusFontSize = 14;

		public bool FocusIsEnabled = true;

		public Vector4 FocusOutlineColor = new Vector4(0.7137255f, 41f / 85f, 16f / 51f, 0f);

		public Vector2 FocusPosition = new Vector2(0f);

		public byte FontSize = 14;

		public bool HideAutoAttack = true;

		public bool IsEnabled = true;

		public Vector4 OutlineColor = new Vector4(0.7137255f, 41f / 85f, 16f / 51f, 0f);

		public Vector2 Position = new Vector2(0f);

		public bool StatusIsEnabled = true;

		public float StatusScale = 1.4f;
	}

	private enum DisplayFormat
	{
		FullNumber,
		FullNumberSeparators,
		ChineseFull,
		ChineseZeroPrecision,
		ChineseOnePrecision,
		ChineseTwoPrecision,
		ZeroPrecision,
		OnePrecision,
		TwoPrecision
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddonCastBarEnemy;
	}

	private Config config;

	private TextNode? targetHPTextNode;

	private TextNode? focusTargetHPTextNode;

	private TextNode? mainTargetSplitHPTextNode;

	private TextNode? targetCastBarTextNode;

	private TextNode? targetSplitCastBarTextNode;

	private TextNode? focusTargetCastBarTextNode;

	private TextButtonNode? clearFocusButtonNode;

	private int numberPreview = 12345678;

	private int currentSecondRowOffset = 41;

	private static readonly Throttler<nint> AddonThrottler = new Throttler<nint>();

	private static readonly FrozenDictionary<DisplayFormat, string> DisplayFormatLoc = new Dictionary<DisplayFormat, string>
	{
		[DisplayFormat.FullNumber] = LanguageManager.Get("OptimizedTargetInfo-FullNumber"),
		[DisplayFormat.FullNumberSeparators] = LanguageManager.Get("OptimizedTargetInfo-FullNumberSeparators"),
		[DisplayFormat.ChineseFull] = LanguageManager.Get("OptimizedTargetInfo-ChineseFull"),
		[DisplayFormat.ChineseZeroPrecision] = LanguageManager.Get("OptimizedTargetInfo-ChineseZeroPrecision"),
		[DisplayFormat.ChineseOnePrecision] = LanguageManager.Get("OptimizedTargetInfo-ChineseOnePrecision"),
		[DisplayFormat.ChineseTwoPrecision] = LanguageManager.Get("OptimizedTargetInfo-ChineseTwoPrecision"),
		[DisplayFormat.ZeroPrecision] = LanguageManager.Get("OptimizedTargetInfo-ZeroPrecision"),
		[DisplayFormat.OnePrecision] = LanguageManager.Get("OptimizedTargetInfo-OnePrecision"),
		[DisplayFormat.TwoPrecision] = LanguageManager.Get("OptimizedTargetInfo-TwoPrecision")
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedTargetInfoTitle"),
		Description = LanguageManager.Get("OptimizedTargetInfoDescription"),
		Category = ModuleCategory.Combat
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Expected O, but got Unknown
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected O, but got Unknown
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_TargetInfo", new AddonEventDelegate(OnAddonTargetInfo));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_TargetInfo", new AddonEventDelegate(OnAddonTargetInfo));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_TargetInfo", new AddonEventDelegate(OnAddonTargetInfo));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_TargetInfoMainTarget", new AddonEventDelegate(OnAddonTargetInfoSplitTarget));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_TargetInfoMainTarget", new AddonEventDelegate(OnAddonTargetInfoSplitTarget));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_TargetInfoMainTarget", new AddonEventDelegate(OnAddonTargetInfoSplitTarget));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_FocusTargetInfo", new AddonEventDelegate(OnAddonFocusTargetInfo));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_FocusTargetInfo", new AddonEventDelegate(OnAddonFocusTargetInfo));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_FocusTargetInfo", new AddonEventDelegate(OnAddonFocusTargetInfo));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_TargetInfoCastBar", new AddonEventDelegate(OnAddonTargetInfoCastBar));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_TargetInfoCastBar", new AddonEventDelegate(OnAddonTargetInfoCastBar));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_TargetInfoCastBar", new AddonEventDelegate(OnAddonTargetInfoCastBar));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_TargetInfoBuffDebuff", new AddonEventDelegate(OnAddonTargetInfoBuffDebuff));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_TargetInfoBuffDebuff", new AddonEventDelegate(OnAddonTargetInfoBuffDebuff));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_TargetInfoBuffDebuff", new AddonEventDelegate(OnAddonTargetInfoBuffDebuff));
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnAddonCastBarEnemy;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddonCastBarEnemy;
			_003C_003EO._003C0_003E__OnAddonCastBarEnemy = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)8, "CastBarEnemy", (AddonEventDelegate)obj2);
		IAddonLifecycle obj3 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj4 = _003C_003EO._003C0_003E__OnAddonCastBarEnemy;
		if (obj4 == null)
		{
			AddonEventDelegate val2 = OnAddonCastBarEnemy;
			_003C_003EO._003C0_003E__OnAddonCastBarEnemy = val2;
			obj4 = (object)val2;
		}
		obj3.RegisterListener((AddonEvent)5, "CastBarEnemy", (AddonEventDelegate)obj4);
		IAddonLifecycle obj5 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj6 = _003C_003EO._003C0_003E__OnAddonCastBarEnemy;
		if (obj6 == null)
		{
			AddonEventDelegate val3 = OnAddonCastBarEnemy;
			_003C_003EO._003C0_003E__OnAddonCastBarEnemy = val3;
			obj6 = (object)val3;
		}
		obj5.RegisterListener((AddonEvent)6, "CastBarEnemy", (AddonEventDelegate)obj6);
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonTargetInfo });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonTargetInfoSplitTarget });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonFocusTargetInfo });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonTargetInfoCastBar });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonTargetInfoBuffDebuff });
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnAddonCastBarEnemy;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddonCastBarEnemy;
			_003C_003EO._003C0_003E__OnAddonCastBarEnemy = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
		targetHPTextNode?.Dispose();
		targetHPTextNode = null;
		targetCastBarTextNode?.Dispose();
		targetCastBarTextNode = null;
		mainTargetSplitHPTextNode?.Dispose();
		mainTargetSplitHPTextNode = null;
		focusTargetHPTextNode?.Dispose();
		focusTargetHPTextNode = null;
		focusTargetCastBarTextNode?.Dispose();
		focusTargetCastBarTextNode = null;
		clearFocusButtonNode?.Dispose();
		clearFocusButtonNode = null;
		targetSplitCastBarTextNode?.Dispose();
		targetSplitCastBarTextNode = null;
	}

	protected override void ConfigUI()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("OptimizedTargetInfo-DisplayFormat"));
		ImGui.TextColored(ref vector, val);
		ImGui.SetNextItemWidth(400f * Globals.GlobalUIScale);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = ImU8String.op_Implicit("###DisplayFormatCombo");
			ImU8String val4 = new ImU8String(3, 2);
			((ImU8String)(ref val4)).AppendFormatted<string>(DisplayFormatLoc.GetValueOrDefault(config.DisplayFormat, LanguageManager.Get("OptimizedTargetInfo-UnknownDisplayFormat")));
			((ImU8String)(ref val4)).AppendLiteral(" ");
			((ImU8String)(ref val4)).AppendLiteral("(");
			((ImU8String)(ref val4)).AppendFormatted<string>(FormatNumber((uint)numberPreview, config.DisplayFormat));
			((ImU8String)(ref val4)).AppendLiteral(")");
			ComboDisposable val5 = ImRaii.Combo(val3, val4, (ImGuiComboFlags)8);
			try
			{
				if (ComboDisposable.op_Implicit(val5))
				{
					ImGui.AlignTextToFramePadding();
					ImU8String val6 = new ImU8String(1, 1);
					((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("OptimizedTargetInfo-NumberPreview"));
					((ImU8String)(ref val6)).AppendLiteral(":");
					ImGui.TextUnformatted(val6);
					ImGui.SameLine();
					ImGui.SetNextItemWidth(-1f);
					ImU8String val7 = ImU8String.op_Implicit("###PreviewNumberInput");
					ref int reference = ref numberPreview;
					ImU8String val8 = default(ImU8String);
					if (ImGui.InputInt(val7, ref reference, 0, 0, val8, (ImGuiInputTextFlags)0))
					{
						numberPreview = (int)Math.Clamp(numberPreview, 0L, 4294967295L);
					}
					ImGui.Separator();
					ImGui.Spacing();
					DisplayFormat[] values = Enum.GetValues<DisplayFormat>();
					foreach (DisplayFormat displayFormat in values)
					{
						val8 = new ImU8String(17, 2);
						((ImU8String)(ref val8)).AppendFormatted<string>(DisplayFormatLoc.GetValueOrDefault(displayFormat, LanguageManager.Get("OptimizedTargetInfo-UnknownDisplayFormat")));
						((ImU8String)(ref val8)).AppendLiteral(" ");
						((ImU8String)(ref val8)).AppendLiteral("(");
						((ImU8String)(ref val8)).AppendFormatted<string>(FormatNumber((uint)numberPreview, displayFormat));
						((ImU8String)(ref val8)).AppendLiteral(")##FormatSelect");
						if (ImGui.Selectable(val8, config.DisplayFormat == displayFormat, (ImGuiSelectableFlags)0, default(Vector2)))
						{
							config.DisplayFormat = displayFormat;
							config.Save(this);
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val5)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val9 = default(ImU8String);
		((ImU8String)(ref val9))._002Ector(0, 1);
		((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get("OptimizedTargetInfo-DisplayStringFormat"));
		ImGui.TextColored(ref vector, val9);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.SetNextItemWidth(400f * Globals.GlobalUIScale);
			ImGui.InputText(ImU8String.op_Implicit("###DisplayStringFormatInput"), ref config.DisplayFormatString, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedTargetInfo-DisplayStringFormatHelp"), 40f, (FontAwesomeIcon)61530);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		DrawTargetConfigSection(LuminaWrapper.GetAddonText(1030u), "Target", ref config.AlignLeft, ref config.Position, ref config.CustomColor, ref config.OutlineColor, ref config.FontSize, ref config.HideAutoAttack, showHideAutoAttack: true, ref config.IsEnabled);
		ImGui.NewLine();
		DrawTargetConfigSection(LuminaWrapper.GetAddonText(1110u), "Focus", ref config.FocusAlignLeft, ref config.FocusPosition, ref config.FocusCustomColor, ref config.FocusOutlineColor, ref config.FocusFontSize, ref config.HideAutoAttack, showHideAutoAttack: false, ref config.FocusIsEnabled);
		ImGui.NewLine();
		DrawTargetConfigSection(LuminaWrapper.GetAddonText(1032u), "CastBar", ref config.CastBarAlignLeft, ref config.CastBarPosition, ref config.CastBarCustomColor, ref config.CastBarOutlineColor, ref config.CastBarFontSize, ref config.HideAutoAttack, showHideAutoAttack: false, ref config.CastBarIsEnabled);
		ImGui.NewLine();
		DrawTargetConfigSection(LuminaWrapper.GetAddonText(1110u) + " " + LuminaWrapper.GetAddonText(1032u), "FocusCastBar", ref config.FocusCastBarAlignLeft, ref config.FocusCastBarPosition, ref config.FocusCastBarCustomColor, ref config.FocusCastBarOutlineColor, ref config.FocusCastBarFontSize, ref config.HideAutoAttack, showHideAutoAttack: false, ref config.FocusCastBarIsEnabled);
		ImGui.NewLine();
		IdDisposable val10 = ImRaii.PushId(ImU8String.op_Implicit("Status"), true);
		try
		{
			ImGui.AlignTextToFramePadding();
			vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LuminaWrapper.GetAddonText(215u)));
			ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
			ImU8String val11 = new ImU8String(0, 1);
			((ImU8String)(ref val11)).AppendFormatted<string>(LanguageManager.Get("Enable"));
			if (ImGui.Checkbox(val11, ref config.StatusIsEnabled))
			{
				config.Save(this);
				if (!config.StatusIsEnabled)
				{
					targetSplitCastBarTextNode?.Dispose();
					targetSplitCastBarTextNode = null;
					OnAddonTargetInfoBuffDebuff((AddonEvent)6, null);
				}
			}
			if (!config.StatusIsEnabled)
			{
				return;
			}
			ImGui.Spacing();
			val2 = ImRaii.PushIndent(1, true);
			try
			{
				ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
				ImU8String val12 = new ImU8String(0, 1);
				((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("Scale"));
				if (ImGui.InputFloat(val12, ref config.StatusScale, 0.1f, 0.1f, ImU8String.op_Implicit("%.2f"), (ImGuiInputTextFlags)0))
				{
					config.StatusScale = Math.Clamp(config.StatusScale, 0.1f, 10f);
				}
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
					targetSplitCastBarTextNode?.Dispose();
					targetSplitCastBarTextNode = null;
					OnAddonTargetInfoBuffDebuff((AddonEvent)6, null);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val10)?.Dispose();
		}
		ImGui.NewLine();
		val10 = ImRaii.PushId(ImU8String.op_Implicit("ClearFocus"), true);
		try
		{
			ImGui.AlignTextToFramePadding();
			vector = KnownColor.Chocolate.ToVector4();
			ImU8String val13 = new ImU8String(0, 1);
			((ImU8String)(ref val13)).AppendFormatted<string>(LanguageManager.Get("OptimizedTargetInfo-ClearFocusTarget"));
			ImGui.TextColored(ref vector, val13);
			ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
			ImU8String val14 = new ImU8String(0, 1);
			((ImU8String)(ref val14)).AppendFormatted<string>(LanguageManager.Get("Enable"));
			if (ImGui.Checkbox(val14, ref config.ClearFocusIsEnabled))
			{
				config.Save(this);
			}
			if (!config.ClearFocusIsEnabled)
			{
				return;
			}
			ImGui.Spacing();
			val2 = ImRaii.PushIndent(1, true);
			try
			{
				ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
				ImU8String val15 = new ImU8String(0, 1);
				((ImU8String)(ref val15)).AppendFormatted<string>(LanguageManager.Get("OptimizedTargetInfo-PosOffset"));
				ImGui.InputFloat2(val15, ref config.ClearFocusPosition, 0f, 0f, ImU8String.op_Implicit("%.2f"), (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val10)?.Dispose();
		}
	}

	private void DrawTargetConfigSection(string sectionTitle, string prefix, ref bool alignLeft, ref Vector2 position, ref Vector4 customColor, ref Vector4 outlineColor, ref byte fontSize, ref bool hideAutoAttack, bool showHideAutoAttack, ref bool isEnabled)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(prefix);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(sectionTitle);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			ImGui.AlignTextToFramePadding();
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(sectionTitle));
			ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
			ImU8String val3 = new ImU8String(0, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Enable"));
			if (ImGui.Checkbox(val3, ref isEnabled))
			{
				config.Save(this);
			}
			if (!isEnabled)
			{
				return;
			}
			ImGui.Spacing();
			IndentDisposable val4 = ImRaii.PushIndent(1, true);
			try
			{
				ImU8String val5 = new ImU8String(12, 1);
				((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("OptimizedTargetInfo-AlignLeft"));
				((ImU8String)(ref val5)).AppendLiteral("###AlignLeft");
				if (ImGui.Checkbox(val5, ref alignLeft))
				{
					config.Save(this);
				}
				ImU8String val6 = new ImU8String(20, 1);
				((ImU8String)(ref val6)).AppendLiteral("###");
				((ImU8String)(ref val6)).AppendFormatted<string>(prefix);
				((ImU8String)(ref val6)).AppendLiteral("CustomColorButton");
				if (ImGui.ColorButton(val6, ref customColor, (ImGuiColorEditFlags)0, default(Vector2)))
				{
					ImU8String val7 = new ImU8String(16, 1);
					((ImU8String)(ref val7)).AppendFormatted<string>(prefix);
					((ImU8String)(ref val7)).AppendLiteral("CustomColorPopup");
					ImGui.OpenPopup(val7, (ImGuiPopupFlags)0);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get("OptimizedTargetInfo-ZeroAlphaHelp"));
				ImGui.SameLine();
				ImU8String val8 = new ImU8String(0, 1);
				((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("OptimizedTargetInfo-CustomColor"));
				ImGui.TextUnformatted(val8);
				ImU8String val9 = new ImU8String(16, 1);
				((ImU8String)(ref val9)).AppendFormatted<string>(prefix);
				((ImU8String)(ref val9)).AppendLiteral("CustomColorPopup");
				PopupDisposable val10 = ImRaii.Popup(val9);
				try
				{
					if (PopupDisposable.op_Implicit(val10))
					{
						ImU8String val11 = new ImU8String(14, 1);
						((ImU8String)(ref val11)).AppendLiteral("###");
						((ImU8String)(ref val11)).AppendFormatted<string>(prefix);
						((ImU8String)(ref val11)).AppendLiteral("CustomColor");
						ImGui.ColorPicker4(val11, ref customColor, (ImGuiColorEditFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val10)).Dispose();
				}
				ImU8String val12 = new ImU8String(21, 1);
				((ImU8String)(ref val12)).AppendLiteral("###");
				((ImU8String)(ref val12)).AppendFormatted<string>(prefix);
				((ImU8String)(ref val12)).AppendLiteral("OutlineColorButton");
				if (ImGui.ColorButton(val12, ref outlineColor, (ImGuiColorEditFlags)0, default(Vector2)))
				{
					ImU8String val13 = new ImU8String(17, 1);
					((ImU8String)(ref val13)).AppendFormatted<string>(prefix);
					((ImU8String)(ref val13)).AppendLiteral("OutlineColorPopup");
					ImGui.OpenPopup(val13, (ImGuiPopupFlags)0);
				}
				ImGuiOm.TooltipHover(LanguageManager.Get("OptimizedTargetInfo-ZeroAlphaHelp"));
				ImGui.SameLine();
				ImU8String val14 = new ImU8String(0, 1);
				((ImU8String)(ref val14)).AppendFormatted<string>(LanguageManager.Get("EdgeColor"));
				ImGui.TextUnformatted(val14);
				ImU8String val15 = new ImU8String(17, 1);
				((ImU8String)(ref val15)).AppendFormatted<string>(prefix);
				((ImU8String)(ref val15)).AppendLiteral("OutlineColorPopup");
				PopupDisposable val16 = ImRaii.Popup(val15);
				try
				{
					if (PopupDisposable.op_Implicit(val16))
					{
						ImU8String val17 = new ImU8String(15, 1);
						((ImU8String)(ref val17)).AppendLiteral("###");
						((ImU8String)(ref val17)).AppendFormatted<string>(prefix);
						((ImU8String)(ref val17)).AppendLiteral("OutlineColor");
						ImGui.ColorPicker4(val17, ref outlineColor, (ImGuiColorEditFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val16)).Dispose();
				}
				if (showHideAutoAttack)
				{
					ImU8String val18 = new ImU8String(21, 2);
					((ImU8String)(ref val18)).AppendFormatted<string>(LanguageManager.Get("OptimizedTargetInfo-HideAutoAttackIcon"));
					((ImU8String)(ref val18)).AppendLiteral("###");
					((ImU8String)(ref val18)).AppendFormatted<string>(prefix);
					((ImU8String)(ref val18)).AppendLiteral("HideAutoAttackIcon");
					if (ImGui.Checkbox(val18, ref hideAutoAttack))
					{
						config.Save(this);
					}
				}
				ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
				ImU8String val19 = new ImU8String(11, 1);
				((ImU8String)(ref val19)).AppendFormatted<string>(LanguageManager.Get("OptimizedTargetInfo-PosOffset"));
				((ImU8String)(ref val19)).AppendLiteral("###Position");
				ImGui.InputFloat2(val19, ref position, 0f, 0f, ImU8String.op_Implicit("%.2f"), (ImGuiInputTextFlags)0);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
				int num = fontSize;
				ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
				ImU8String val20 = new ImU8String(11, 1);
				((ImU8String)(ref val20)).AppendFormatted<string>(LanguageManager.Get("FontScale"));
				((ImU8String)(ref val20)).AppendLiteral("###FontSize");
				if (ImGui.SliderInt(val20, ref num, 1, 32, default(ImU8String), (ImGuiSliderFlags)0))
				{
					fontSize = (byte)num;
				}
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private static void OnAddonCastBarEnemy(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		HandleAddonEventCastBarEnemy(type);
	}

	private unsafe void OnAddonTargetInfo(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		HandleAddonEventTargetInfo(type, config.IsEnabled, config.HideAutoAttack, 18u, Addons.TargetInfo, ref targetHPTextNode, 16u, 19u, config.Position, config.AlignLeft, config.FontSize, config.CustomColor, config.OutlineColor, () => (TargetManager.SoftTarget ?? TargetManager.Target) as IBattleChara, (uint width, uint height) => new Vector2(width - 5, height + 2));
		HandleAddonEventCastBar(type, config.CastBarIsEnabled, Addons.TargetInfo, ref targetCastBarTextNode, 10u, 12u, config.CastBarPosition, config.CastBarAlignLeft, config.CastBarFontSize, config.CastBarCustomColor, config.CastBarOutlineColor, 12u, () => (TargetManager.SoftTarget ?? TargetManager.Target) as IBattleChara, (uint width, uint height) => new Vector2(width - 5, height));
		HandleAddonEventTargetStatus(type, Addons.TargetInfo, 32);
	}

	private unsafe void OnAddonTargetInfoSplitTarget(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		HandleAddonEventTargetInfo(type, config.IsEnabled, config.HideAutoAttack, 12u, Addons.TargetInfoMainTarget, ref mainTargetSplitHPTextNode, 10u, 13u, config.Position, config.AlignLeft, config.FontSize, config.CustomColor, config.OutlineColor, () => (TargetManager.SoftTarget ?? TargetManager.Target) as IBattleChara, (uint width, uint height) => new Vector2(width - 5, height + 2));
	}

	private unsafe void OnAddonFocusTargetInfo(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		HandleAddonEventTargetInfo(type, config.FocusIsEnabled, isHideAutoAttack: false, 0u, Addons.FocusTargetInfo, ref focusTargetHPTextNode, 10u, 18u, config.FocusPosition, config.FocusAlignLeft, config.FocusFontSize, config.FocusCustomColor, config.FocusOutlineColor, () => TargetManager.FocusTarget as IBattleChara, (uint width, uint height) => new Vector2(width - 5, height + 2));
		HandleAddonEventCastBar(type, config.FocusCastBarIsEnabled, Addons.FocusTargetInfo, ref focusTargetCastBarTextNode, 3u, 5u, config.FocusCastBarPosition, config.FocusCastBarAlignLeft, config.FocusCastBarFontSize, config.FocusCastBarCustomColor, config.FocusCastBarOutlineColor, 5u, () => TargetManager.FocusTarget as IBattleChara, (uint width, uint height) => new Vector2(width - 5, height));
		HandleAddonEventFocusTargetControl(type);
	}

	private unsafe void OnAddonTargetInfoCastBar(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		HandleAddonEventCastBar(type, config.CastBarIsEnabled, Addons.TargetInfoCastBar, ref targetSplitCastBarTextNode, 2u, 4u, config.CastBarPosition, config.CastBarAlignLeft, config.CastBarFontSize, config.CastBarCustomColor, config.CastBarOutlineColor, 4u, () => (TargetManager.SoftTarget ?? TargetManager.Target) as IBattleChara, (uint width, uint height) => new Vector2(width - 5, height));
	}

	private unsafe void OnAddonTargetInfoBuffDebuff(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		HandleAddonEventTargetStatus(type, Addons.TargetInfoBuffDebuff, 31);
	}

	private unsafe void HandleAddonEventFocusTargetControl(AddonEvent type)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 6)
		{
			if ((int)type != 8 || !(*Addons.FocusTargetInfo).IsAddonAndNodesReady())
			{
				return;
			}
			if (clearFocusButtonNode == null)
			{
				clearFocusButtonNode = new TextButtonNode
				{
					IsVisible = true,
					Size = new Vector2(32f),
					Position = new Vector2(-13f, 12f),
					String = ReadOnlySeString.op_Implicit("\ue04c"),
					TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedTargetInfo-ClearFocusTarget")),
					OnClick = delegate
					{
						//IL_000b: Unknown result type (might be due to invalid IL or missing references)
						((TargetSystem)TargetSystem.Instance()).SetFocusTargetByObjectId(GameObjectId.op_Implicit(3758096384uL));
					}
				};
				clearFocusButtonNode.BackgroundNode.IsVisible = false;
				clearFocusButtonNode.AttachNode(((AtkUnitBase)Addons.FocusTargetInfo).RootNode);
			}
			clearFocusButtonNode.IsVisible = config.ClearFocusIsEnabled;
			clearFocusButtonNode.Position = new Vector2(-13f, 12f) + config.ClearFocusPosition;
		}
		else
		{
			clearFocusButtonNode = null;
		}
	}

	private unsafe void HandleAddonEventTargetStatus(AddonEvent type, AtkUnitBase* addon, int statusNodeStartIndex)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected I4, but got Unknown
		if (!(*addon).IsAddonAndNodesReady())
		{
			return;
		}
		switch (type - 5)
		{
		case 1:
		{
			for (int j = 0; j < 15; j++)
			{
				AtkResNode* intPtr = ((AtkUldManager)(&((AtkUnitBase)addon).UldManager)).NodeList[statusNodeStartIndex - j];
				((AtkResNode)intPtr).ScaleX = 1f;
				((AtkResNode)intPtr).ScaleY = 1f;
				((AtkResNode)intPtr).X = j * 25;
				((AtkResNode)intPtr).Y = 0f;
				((AtkResNode)intPtr).DrawFlags |= 1u;
			}
			for (int num7 = statusNodeStartIndex - 14; num7 >= statusNodeStartIndex - 29; num7--)
			{
				((AtkResNode)((AtkUldManager)(&((AtkUnitBase)addon).UldManager)).NodeList[num7]).Y = 41f;
				((AtkResNode)((AtkUldManager)(&((AtkUnitBase)addon).UldManager)).NodeList[num7]).DrawFlags |= 1u;
			}
			((AtkResNode)((AtkUldManager)(&((AtkUnitBase)addon).UldManager)).NodeList[statusNodeStartIndex - 30]).DrawFlags |= 4u;
			currentSecondRowOffset = 41;
			break;
		}
		case 0:
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && AddonThrottler.Throttle((nint)addon, 10u))
			{
				HandleAddonEventTargetStatus((AddonEvent)8, addon, statusNodeStartIndex);
			}
			break;
		case 3:
		{
			if (!config.StatusIsEnabled || !(TargetManager.Target is IBattleChara battleChara))
			{
				break;
			}
			int num = 0;
			foreach (IStatus status in battleChara.StatusList)
			{
				if (status.SourceID == LocalPlayerState.EntityID)
				{
					num++;
				}
			}
			int num2 = -(int)((double)(41f * (config.StatusScale - 1f)) / 4.5);
			int num3 = (int)((config.StatusScale - 1f) * 25f);
			int num4 = 0;
			for (int i = 0; i < 15; i++)
			{
				AtkResNode* ptr = ((AtkUldManager)(&((AtkUnitBase)addon).UldManager)).NodeList[statusNodeStartIndex - i];
				if (!((AtkResNode)ptr).IsVisible())
				{
					return;
				}
				((AtkResNode)ptr).X = i * 25 + num4;
				if (i < num)
				{
					((AtkResNode)ptr).ScaleX = config.StatusScale;
					((AtkResNode)ptr).ScaleY = config.StatusScale;
					((AtkResNode)ptr).Y = num2;
					num4 += num3;
				}
				else
				{
					((AtkResNode)ptr).ScaleX = 1f;
					((AtkResNode)ptr).ScaleY = 1f;
					((AtkResNode)ptr).Y = 0f;
				}
				((AtkResNode)ptr).DrawFlags |= 1u;
			}
			int num5 = ((num > 0) ? ((int)(config.StatusScale * 41f)) : 41);
			if (num5 != currentSecondRowOffset)
			{
				for (int num6 = statusNodeStartIndex - 15; num6 >= statusNodeStartIndex - 29; num6--)
				{
					((AtkResNode)((AtkUldManager)(&((AtkUnitBase)addon).UldManager)).NodeList[num6]).Y = num5;
					((AtkResNode)((AtkUldManager)(&((AtkUnitBase)addon).UldManager)).NodeList[num6]).DrawFlags |= 1u;
				}
				currentSecondRowOffset = num5;
			}
			((AtkResNode)((AtkUldManager)(&((AtkUnitBase)addon).UldManager)).NodeList[statusNodeStartIndex - 30]).DrawFlags |= 4u;
			((AtkResNode)((AtkUldManager)(&((AtkUnitBase)addon).UldManager)).NodeList[statusNodeStartIndex - 30]).DrawFlags |= 1u;
			break;
		}
		case 2:
			break;
		}
	}

	private unsafe static void HandleAddonEventCastBarEnemy(AddonEvent type)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected I4, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		if (!(*Addons.CastBarEnemy).IsAddonAndNodesReady())
		{
			return;
		}
		switch (type - 5)
		{
		case 1:
		{
			if (Addons.CastBarEnemy == null)
			{
				break;
			}
			for (int num3 = 10; num3 > 0; num3--)
			{
				AtkComponentNode* ptr2 = (AtkComponentNode*)((AtkUldManager)(&((AtkUnitBase)Addons.CastBarEnemy).UldManager)).NodeList[num3];
				if (ptr2 != null)
				{
					AtkTextNode* textNodeById2 = ((AtkComponentBase)((AtkComponentNode)ptr2).Component).GetTextNodeById(4u);
					if (textNodeById2 != null)
					{
						((AtkTextNode)textNodeById2).SetText(LuminaWrapper.GetAddonText(16482u));
						((AtkTextNode)textNodeById2).FontSize = 12;
					}
				}
			}
			break;
		}
		case 0:
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && Throttler.Shared.Throttle("OptimizedTargetInfo-CastBarEnemy", 10u))
			{
				HandleAddonEventCastBarEnemy((AddonEvent)8);
			}
			break;
		case 3:
		{
			if (Addons.CastBarEnemy == null)
			{
				break;
			}
			AddonCastBarEnemy* castBarEnemy = (AddonCastBarEnemy*)Addons.CastBarEnemy;
			int num = ((NumberArrayData)((AtkStage)AtkStage.Instance()).GetNumberArrayData((NumberArrayType)6)).IntArray[1];
			int num2 = 0;
			Span<CastBarNodeStruct> castBarNodes = ((AddonCastBarEnemy)castBarEnemy).CastBarNodes;
			for (int i = 0; i < castBarNodes.Length; i++)
			{
				CastBarNodeStruct val = castBarNodes[i];
				AtkComponentNode* castBarNode = (AtkComponentNode*)val.CastBarNode;
				if (!((AtkComponentNode)castBarNode).IsVisible())
				{
					continue;
				}
				BattleChara* ptr = ((CharacterManager)CharacterManager.Instance()).LookupBattleCharaByEntityId(val.ObjectId.ObjectId);
				if (ptr == null)
				{
					continue;
				}
				CastInfo* castInfo = ((BattleChara)ptr).GetCastInfo();
				if (castInfo != null && !(((CastInfo)castInfo).CurrentCastTime <= 0f))
				{
					num2++;
					AtkTextNode* textNodeById = ((AtkComponentBase)((AtkComponentNode)castBarNode).Component).GetTextNodeById(4u);
					float value = ((CastInfo)castInfo).TotalCastTime - ((CastInfo)castInfo).CurrentCastTime;
					((AtkTextNode)textNodeById).SetText($"{value:F2}");
					((AtkTextNode)textNodeById).FontSize = 16;
					if (num2 >= num)
					{
						break;
					}
				}
			}
			break;
		}
		case 2:
			break;
		}
	}

	private unsafe void HandleAddonEventTargetInfo(AddonEvent type, bool isEnabled, bool isHideAutoAttack, uint autoAttackNodeID, AtkUnitBase* addon, ref TextNode? textNode, uint textNodeID, uint gaugeNodeID, Vector2 position, bool alignLeft, byte fontSize, Vector4 customColor, Vector4 outlineColor, Func<IGameObject?> getTarget, Func<uint, uint, Vector2> getSizeFunc)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Invalid comparison between Unknown and I4
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		switch (type - 5)
		{
		case 1:
			textNode = null;
			break;
		case 0:
			if ((*addon).IsAddonAndNodesReady() && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && AddonThrottler.Throttle((nint)addon, 10u))
			{
				HandleAddonEventTargetInfo((AddonEvent)8, isEnabled, isHideAutoAttack, autoAttackNodeID, addon, ref textNode, textNodeID, gaugeNodeID, position, alignLeft, fontSize, customColor, outlineColor, getTarget, getSizeFunc);
			}
			break;
		case 3:
		{
			if (!(*addon).IsAddonAndNodesReady())
			{
				break;
			}
			if (textNode == null)
			{
				AtkTextNode* textNodeById = ((AtkUnitBase)addon).GetTextNodeById(textNodeID);
				if (textNodeById == null)
				{
					break;
				}
				AtkComponentBase* componentByNodeId = ((AtkUnitBase)addon).GetComponentByNodeId(gaugeNodeID);
				if (componentByNodeId == null)
				{
					break;
				}
				textNode = new TextNode
				{
					IsVisible = isEnabled,
					Position = position,
					AlignmentType = (AlignmentType)(alignLeft ? 6 : 8),
					FontSize = fontSize,
					TextFlags = (TextFlags)10,
					TextColor = ((customColor.W != 0f) ? customColor : ((AtkTextNode)textNodeById).TextColor.ToVector4()),
					TextOutlineColor = ((outlineColor.W == 0f) ? ((AtkTextNode)textNodeById).EdgeColor.ToVector4() : outlineColor)
				};
				textNode.AttachNode(((AtkComponentBase)componentByNodeId).OwnerNode);
			}
			if (autoAttackNodeID != 0 && isHideAutoAttack)
			{
				AtkImageNode* imageNodeById = ((AtkUnitBase)addon).GetImageNodeById(autoAttackNodeID);
				if (imageNodeById != null && ((AtkImageNode)imageNodeById).IsVisible())
				{
					((AtkImageNode)imageNodeById).ToggleVisibility(false);
				}
			}
			textNode.IsVisible = isEnabled && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)6];
			if (!isEnabled)
			{
				break;
			}
			IGameObject gameObject = getTarget();
			if (!(gameObject is IBattleChara battleChara) || (int)gameObject.ObjectKind == 6)
			{
				break;
			}
			AtkTextNode* textNodeById2 = ((AtkUnitBase)addon).GetTextNodeById(textNodeID);
			if (textNodeById2 != null)
			{
				AtkComponentBase* componentByNodeId2 = ((AtkUnitBase)addon).GetComponentByNodeId(gaugeNodeID);
				if (componentByNodeId2 != null)
				{
					textNode.Position = position;
					textNode.Size = getSizeFunc(((AtkComponentNode)((AtkComponentBase)componentByNodeId2).OwnerNode).Width, ((AtkComponentNode)((AtkComponentBase)componentByNodeId2).OwnerNode).Height);
					textNode.AlignmentType = (AlignmentType)(alignLeft ? 6 : 8);
					textNode.FontSize = fontSize;
					textNode.TextColor = ((customColor.W != 0f) ? customColor : ((AtkTextNode)textNodeById2).TextColor.ToVector4());
					textNode.TextOutlineColor = ((outlineColor.W == 0f) ? ((AtkTextNode)textNodeById2).EdgeColor.ToVector4() : outlineColor);
					textNode.String = ReadOnlySeString.op_Implicit(SafeFormatting.TargetHP(config.DisplayFormatString, FormatNumber(battleChara.MaxHp), FormatNumber(battleChara.CurrentHp)));
				}
			}
			break;
		}
		case 2:
			break;
		}
	}

	private unsafe static void HandleAddonEventCastBar(AddonEvent type, bool isEnabled, AtkUnitBase* addon, ref TextNode? textNode, uint nodeIDToAttach, uint textNodeID, Vector2 position, bool alignLeft, byte fontSize, Vector4 customColor, Vector4 outlineColor, uint actionNameTextNodeID, Func<IGameObject?> getTarget, Func<uint, uint, Vector2> getSizeFunc)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Invalid comparison between Unknown and I4
		switch (type - 5)
		{
		case 1:
			textNode = null;
			break;
		case 0:
			if ((*addon).IsAddonAndNodesReady() && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && AddonThrottler.Throttle((nint)addon, 10u))
			{
				HandleAddonEventCastBar((AddonEvent)8, isEnabled, addon, ref textNode, nodeIDToAttach, textNodeID, position, alignLeft, fontSize, customColor, outlineColor, actionNameTextNodeID, getTarget, getSizeFunc);
			}
			break;
		case 3:
		{
			if (!(*addon).IsAddonAndNodesReady())
			{
				break;
			}
			if (textNode == null)
			{
				AtkTextNode* textNodeById = ((AtkUnitBase)addon).GetTextNodeById(textNodeID);
				if (textNodeById == null)
				{
					break;
				}
				textNode = new TextNode
				{
					IsVisible = isEnabled,
					Position = position + new Vector2(4f, -12f),
					AlignmentType = (AlignmentType)((!alignLeft) ? 2 : 0),
					FontSize = fontSize,
					TextFlags = (TextFlags)10,
					TextColor = ((customColor.W != 0f) ? customColor : ((AtkTextNode)textNodeById).TextColor.ToVector4()),
					TextOutlineColor = ((outlineColor.W == 0f) ? ((AtkTextNode)textNodeById).EdgeColor.ToVector4() : outlineColor),
					FontType = (FontType)2
				};
				textNode.AttachNode(((AtkUnitBase)addon).GetNodeById(nodeIDToAttach));
			}
			textNode.IsVisible = isEnabled && getTarget() != null;
			if (!textNode.IsVisible || !(getTarget() is IBattleChara battleChara))
			{
				break;
			}
			AtkTextNode* textNodeById2 = ((AtkUnitBase)addon).GetTextNodeById(textNodeID);
			if (textNodeById2 == null)
			{
				break;
			}
			AtkTextNode* textNodeById3 = ((AtkUnitBase)addon).GetTextNodeById(actionNameTextNodeID);
			if (textNodeById3 == null)
			{
				break;
			}
			AtkImageNode* imageNodeById = ((AtkUnitBase)addon).GetImageNodeById(actionNameTextNodeID + 3);
			if (imageNodeById == null)
			{
				break;
			}
			float num = battleChara.TotalCastTime - battleChara.CurrentCastTime;
			textNode.IsVisible = battleChara.CurrentCastTime > 0f && num > 0f;
			((AtkTextNode)textNodeById3).ToggleVisibility(textNode.IsVisible);
			((AtkImageNode)imageNodeById).ToggleVisibility(textNode.IsVisible);
			if (textNode.IsVisible)
			{
				textNode.Position = position + new Vector2(4f, -12f);
				textNode.Size = getSizeFunc(((AtkTextNode)textNodeById2).Width, ((AtkTextNode)textNodeById2).Height);
				textNode.AlignmentType = (AlignmentType)((!alignLeft) ? 2 : 0);
				textNode.FontSize = fontSize;
				textNode.TextColor = ((customColor.W != 0f) ? customColor : ((AtkTextNode)textNodeById2).TextColor.ToVector4());
				textNode.TextOutlineColor = ((outlineColor.W == 0f) ? ((AtkTextNode)textNodeById2).EdgeColor.ToVector4() : outlineColor);
				textNode.String = ReadOnlySeString.op_Implicit($"{num:F2}");
				if ((int)battleChara.CastActionType == 1)
				{
					((AtkTextNode)textNodeById3).SetText(LuminaWrapper.GetActionName(battleChara.CastActionID));
				}
			}
			break;
		}
		case 2:
			break;
		}
	}

	private string FormatNumber(uint num, DisplayFormat? displayFormat = null)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Invalid comparison between Unknown and I4
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Invalid comparison between Unknown and I4
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Invalid comparison between Unknown and I4
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Invalid comparison between Unknown and I4
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Invalid comparison between Unknown and I4
		DisplayFormat valueOrDefault = displayFormat.GetValueOrDefault();
		if (!displayFormat.HasValue)
		{
			valueOrDefault = config.DisplayFormat;
			displayFormat = valueOrDefault;
		}
		string text = default(string);
		switch (displayFormat)
		{
		case DisplayFormat.FullNumber:
			return num.ToString();
		case DisplayFormat.FullNumberSeparators:
			return num.ToString("N0");
		case DisplayFormat.ChineseFull:
			return num.ToChineseString();
		case DisplayFormat.ChineseZeroPrecision:
		case DisplayFormat.ChineseOnePrecision:
		case DisplayFormat.ChineseTwoPrecision:
		{
			(float, string) tuple;
			if (num < 100000000)
			{
				if (num >= 10000)
				{
					Language clientLanguage = GameState.ClientLanguage;
					bool flag = (((int)clientLanguage == 6 || (int)clientLanguage == 8) ? true : false);
					tuple = (10000f, flag ? "萬" : "万");
				}
				else
				{
					tuple = (1f, string.Empty);
				}
			}
			else
			{
				Language clientLanguage = GameState.ClientLanguage;
				bool flag = (((int)clientLanguage == 1 || (int)clientLanguage == 6 || (int)clientLanguage == 8) ? true : false);
				tuple = (100000000f, flag ? "億" : "亿");
			}
			(float, string) tuple2 = tuple;
			float item = tuple2.Item1;
			string item2 = tuple2.Item2;
			float num2 = (float)num / item;
			switch (displayFormat)
			{
			case DisplayFormat.ChineseOnePrecision:
				text = "F1";
				break;
			case DisplayFormat.ChineseTwoPrecision:
				text = "F2";
				break;
			case DisplayFormat.ChineseZeroPrecision:
				text = "F0";
				break;
			default:
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(displayFormat);
				break;
			}
			string text3 = text;
			return num2.ToString(text3).TrimEnd('0').TrimEnd('.') + item2;
		}
		case DisplayFormat.ZeroPrecision:
		case DisplayFormat.OnePrecision:
		case DisplayFormat.TwoPrecision:
		{
			switch (displayFormat)
			{
			case DisplayFormat.OnePrecision:
				text = "F1";
				break;
			case DisplayFormat.TwoPrecision:
				text = "F2";
				break;
			case DisplayFormat.ZeroPrecision:
				text = "F0";
				break;
			default:
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(displayFormat);
				break;
			}
			string text2 = text;
			if (num < 1000000)
			{
				if (num < 1000)
				{
					return $"{num}";
				}
				return ((float)num / 1000f).ToString(text2) + "K";
			}
			return ((float)num / 1000000f).ToString(text2) + "M";
		}
		default:
			return num.ToString("N0");
		}
	}
}
