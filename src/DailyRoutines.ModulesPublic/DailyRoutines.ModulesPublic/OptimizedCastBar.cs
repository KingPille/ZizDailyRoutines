using System;
using System.Collections.Frozen;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes.Simplified;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class OptimizedCastBar : ModuleBase
{
	private class Config : ModuleConfig
	{
		public Vector4 CastingTextBackgroundColor = new Vector4(0f);

		public Vector4 CastingTextColor = new Vector4(1f);

		public Vector4 CastingTextEdgeColor = new Vector4(0.56f, 0.42f, 0.05f, 1f);

		public Vector2 CastingTextPosition = new Vector2(0f, 0f);

		public byte CastingTextSize = 12;

		public Vector4 CastTimeTextBackgroundColor = new Vector4(0f);

		public Vector4 CastTimeTextColor = new Vector4(1f);

		public Vector4 CastTimeTextEdgeColor = new Vector4(0.56f, 0.42f, 0.05f, 1f);

		public Vector2 CastTimeTextPosition = new Vector2(130f, 30f);

		public byte CastTimeTextSize = 20;

		public byte IconAlpha = byte.MaxValue;

		public Vector2 IconPosition = new Vector2(0f, 3f);

		public Vector2 IconScale = new Vector2(1f);

		public Vector4 InterruptedTextBackgroundColor = new Vector4(0f);

		public Vector4 InterruptedTextColor = new Vector4(1f);

		public Vector4 InterruptedTextEdgeColor = new Vector4(0.56f, 0.42f, 0.05f, 1f);

		public Vector2 InterruptedTextPosition = new Vector2(0f, 11f);

		public byte InterruptedTextSize = 18;

		public Vector4 NameTextBackgroundColor = new Vector4(0f);

		public Vector4 NameTextColor = new Vector4(1f);

		public Vector4 NameTextEdgeColor = new Vector4(0.56f, 0.42f, 0.05f, 1f);

		public Vector2 NameTextPosition = new Vector2(48f, 0f);

		public byte NameTextSize = 12;

		public SlideCastHighlightType SlideCastHighlightType = SlideCastHighlightType.Zone;

		public int SlideCastLineHeight;

		public int SlideCastLineWidth = 3;

		public Vector4 SlideCastNotReadyColor = new Vector4(0.8f, 0.3f, 0.3f, 1f);

		public Vector4 SlideCastReadyColor = new Vector4(0.3f, 0.8f, 0.3f, 1f);

		public int SlideCastZoneAdjust = 500;
	}

	private enum SlideCastHighlightType
	{
		None,
		Zone,
		Line
	}

	private Config config;

	private SimpleNineGridNode? slideMarkerZoneNode;

	private SimpleNineGridNode? slideMarkerLineNode;

	private static readonly FrozenSet<ConditionFlag> ValidFlags = FrozenSet.Create(RuntimeHelpers.CreateSpan<ConditionFlag>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedCastBarTitle"),
		Description = LanguageManager.Get("OptimizedCastBarDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "Middo" }
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
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "_CastBar", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "_CastBar", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		slideMarkerZoneNode?.Dispose();
		slideMarkerZoneNode = null;
		slideMarkerLineNode?.Dispose();
		slideMarkerLineNode = null;
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_0939: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_095f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1050u)));
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1050u)), true);
		try
		{
			ItemWidthDisposable val2 = ImRaii.ItemWidth(250f * Globals.GlobalUIScale);
			try
			{
				IndentDisposable val3 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("TextColor")), ref config.CastingTextColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("EdgeColor")), ref config.CastingTextEdgeColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("BackgroundColor")), ref config.CastingTextBackgroundColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Position")), ref config.CastingTextPosition, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputByte(ImU8String.op_Implicit(LanguageManager.Get("FontSize")), ref config.CastingTextSize, (byte)0, (byte)0, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
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
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LuminaWrapper.GetAddonText(14051u)));
		val = ImRaii.PushId(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(14051u)), true);
		try
		{
			ItemWidthDisposable val2 = ImRaii.ItemWidth(250f * Globals.GlobalUIScale);
			try
			{
				IndentDisposable val3 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.InputByte(ImU8String.op_Implicit(LanguageManager.Get("Alpha")), ref config.IconAlpha, (byte)0, (byte)0, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Position")), ref config.IconPosition, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Scale")), ref config.IconScale, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
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
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1051u)));
		val = ImRaii.PushId(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1051u)), true);
		try
		{
			ItemWidthDisposable val2 = ImRaii.ItemWidth(250f * Globals.GlobalUIScale);
			try
			{
				IndentDisposable val3 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("TextColor")), ref config.InterruptedTextColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("EdgeColor")), ref config.InterruptedTextEdgeColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("BackgroundColor")), ref config.InterruptedTextBackgroundColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Position")), ref config.InterruptedTextPosition, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputByte(ImU8String.op_Implicit(LanguageManager.Get("FontSize")), ref config.InterruptedTextSize, (byte)0, (byte)0, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
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
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LuminaWrapper.GetAddonText(3626u)));
		val = ImRaii.PushId(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(3626u)), true);
		try
		{
			ItemWidthDisposable val2 = ImRaii.ItemWidth(250f * Globals.GlobalUIScale);
			try
			{
				IndentDisposable val3 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("TextColor")), ref config.NameTextColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("EdgeColor")), ref config.NameTextEdgeColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("BackgroundColor")), ref config.NameTextBackgroundColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Position")), ref config.NameTextPosition, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputByte(ImU8String.op_Implicit(LanguageManager.Get("FontSize")), ref config.NameTextSize, (byte)0, (byte)0, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
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
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LuminaWrapper.GetAddonText(701u)));
		val = ImRaii.PushId(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(701u)), true);
		try
		{
			ItemWidthDisposable val2 = ImRaii.ItemWidth(250f * Globals.GlobalUIScale);
			try
			{
				IndentDisposable val3 = ImRaii.PushIndent(1, true);
				try
				{
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("TextColor")), ref config.CastTimeTextColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("EdgeColor")), ref config.CastTimeTextEdgeColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("BackgroundColor")), ref config.CastTimeTextBackgroundColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputFloat2(ImU8String.op_Implicit(LanguageManager.Get("Position")), ref config.CastTimeTextPosition, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.InputByte(ImU8String.op_Implicit(LanguageManager.Get("FontSize")), ref config.CastTimeTextSize, (byte)0, (byte)0, default(ImU8String), (ImGuiInputTextFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
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
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("OptimizedCastBar-SlideCastMarker")));
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedCastBar-SlideCastMarker-Help"), 40f, (FontAwesomeIcon)61530);
		val = ImRaii.PushId(ImU8String.op_Implicit("OptimizedCastBar-SlideCastMarker"), true);
		try
		{
			ItemWidthDisposable val2 = ImRaii.ItemWidth(250f * Globals.GlobalUIScale);
			try
			{
				IndentDisposable val3 = ImRaii.PushIndent(1, true);
				try
				{
					ComboDisposable val4 = ImRaii.Combo(ImU8String.op_Implicit(LanguageManager.Get("Type")), ImU8String.op_Implicit(LanguageManager.Get($"OptimizedCastBar-SlideCastHighlightType-{config.SlideCastHighlightType}")));
					try
					{
						if (ComboDisposable.op_Implicit(val4))
						{
							SlideCastHighlightType[] values = Enum.GetValues<SlideCastHighlightType>();
							foreach (SlideCastHighlightType slideCastHighlightType in values)
							{
								if (ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get($"OptimizedCastBar-SlideCastHighlightType-{slideCastHighlightType}")), config.SlideCastHighlightType == slideCastHighlightType, (ImGuiSelectableFlags)0, default(Vector2)))
								{
									config.SlideCastHighlightType = slideCastHighlightType;
									config.Save(this);
								}
							}
						}
					}
					finally
					{
						((ComboDisposable)(ref val4)).Dispose();
					}
					if (config.SlideCastHighlightType == SlideCastHighlightType.None)
					{
						return;
					}
					if (config.SlideCastHighlightType == SlideCastHighlightType.Line)
					{
						ImGui.Spacing();
						ImGui.SliderInt(ImU8String.op_Implicit(LanguageManager.Get("Width")), ref config.SlideCastLineWidth, 1, 10, default(ImU8String), (ImGuiSliderFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
						ImGui.SliderInt(ImU8String.op_Implicit(LanguageManager.Get("Height")), ref config.SlideCastLineHeight, 0, 20, default(ImU8String), (ImGuiSliderFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
						ImGui.Spacing();
					}
					ImGui.SliderInt(ImU8String.op_Implicit(LanguageManager.Get("OptimizedCastBar-SlideCastOffsetTime")), ref config.SlideCastZoneAdjust, 0, 1000, default(ImU8String), (ImGuiSliderFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("OptimizedCastBar-SlideCastMarkerNotReadyColor")), ref config.SlideCastNotReadyColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
					ImGui.ColorEdit4(ImU8String.op_Implicit(LanguageManager.Get("OptimizedCastBar-SlideCastMarkerReadyColor")), ref config.SlideCastReadyColor, (ImGuiColorEditFlags)0);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
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

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (ValidFlags.Contains(flag))
		{
			slideMarkerZoneNode?.Dispose();
			slideMarkerZoneNode = null;
			slideMarkerLineNode?.Dispose();
			slideMarkerLineNode = null;
			UpdateOriginalAddonNodes();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				slideMarkerZoneNode = null;
				slideMarkerLineNode = null;
				UpdateOriginalAddonNodes();
			}
		}
		else
		{
			if (Addons.CastBar == null)
			{
				return;
			}
			AddonCastBar* castBar = (AddonCastBar*)Addons.CastBar;
			AtkNineGridNode* nodeById = (AtkNineGridNode*)((AtkUnitBase)Addons.CastBar).GetNodeById(11u);
			if (nodeById == null)
			{
				return;
			}
			if (Throttler.Shared.Throttle("OptimizedCastBar-PostDraw-UpdateOriginal"))
			{
				UpdateOriginalAddonNodes();
			}
			if (!Throttler.Shared.Throttle("OptimizedCastBar-PostDraw-UpdateSlideCast", 10u))
			{
				return;
			}
			float num = ((float)(((AddonCastBar)castBar).CastTime * 10) - (float)config.SlideCastZoneAdjust) / (float)(((AddonCastBar)castBar).CastTime * 10);
			float num2 = 160f * num;
			Vector4 vector = ((DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27] || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)31]) ? config.SlideCastNotReadyColor : config.SlideCastReadyColor);
			switch (config.SlideCastHighlightType)
			{
			case SlideCastHighlightType.Zone:
				if (slideMarkerLineNode != null)
				{
					slideMarkerLineNode.IsVisible = false;
				}
				if (slideMarkerZoneNode == null)
				{
					slideMarkerZoneNode = new SimpleNineGridNode
					{
						PartId = 0u,
						TexturePath = "ui/uld/parameter_gauge_hr1.tex",
						TextureCoordinates = new Vector2(0f, 0f),
						TextureSize = new Vector2(160f, 20f),
						Color = ((ByteColor)(&((AtkNineGridNode)nodeById).Color)).RGBA.ToVector4(),
						NodeFlags = ((AtkNineGridNode)nodeById).NodeFlags,
						Offsets = new Vector4(12f)
					};
					slideMarkerZoneNode.AttachNode(((AtkNineGridNode)nodeById).ParentNode);
				}
				slideMarkerZoneNode.IsVisible = true;
				slideMarkerZoneNode.Size = new Vector2(168 - (int)num2, 22f);
				slideMarkerZoneNode.Position = new Vector2(num2 - 9f, -1f);
				slideMarkerZoneNode.AddColor = vector.AsVector3();
				slideMarkerZoneNode.MultiplyColor = vector.AsVector3();
				break;
			case SlideCastHighlightType.Line:
				if (slideMarkerZoneNode != null)
				{
					slideMarkerZoneNode.IsVisible = false;
				}
				if (slideMarkerLineNode == null)
				{
					slideMarkerLineNode = new SimpleNineGridNode
					{
						TexturePath = "ui/uld/emjfacemask.tex",
						TextureCoordinates = new Vector2(28f, 28f),
						TextureSize = new Vector2(8f, 8f),
						NodeFlags = (NodeFlags)3
					};
					slideMarkerLineNode.AttachNode(((AtkNineGridNode)nodeById).ParentNode);
				}
				slideMarkerLineNode.IsVisible = true;
				slideMarkerLineNode.Size = new Vector2(config.SlideCastLineWidth, 12 + config.SlideCastLineHeight * 2);
				slideMarkerLineNode.Position = new Vector2(num2, 4 - config.SlideCastLineHeight);
				slideMarkerLineNode.Color = vector;
				break;
			}
		}
	}

	private unsafe void UpdateOriginalAddonNodes()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.CastBar != null && config != null)
		{
			AtkTextNode* textNodeById = ((AtkUnitBase)Addons.CastBar).GetTextNodeById(2u);
			if (textNodeById != null)
			{
				Unsafe.Write(&((AtkTextNode)textNodeById).TextColor, config.InterruptedTextColor.ToByteColor());
				Unsafe.Write(&((AtkTextNode)textNodeById).EdgeColor, config.InterruptedTextEdgeColor.ToByteColor());
				Unsafe.Write(&((AtkTextNode)textNodeById).BackgroundColor, config.InterruptedTextBackgroundColor.ToByteColor());
				((AtkTextNode)textNodeById).FontSize = config.InterruptedTextSize;
				((AtkTextNode)textNodeById).SetPositionFloat(config.InterruptedTextPosition.X, config.InterruptedTextPosition.Y);
			}
			AtkTextNode* textNodeById2 = ((AtkUnitBase)Addons.CastBar).GetTextNodeById(4u);
			if (textNodeById2 != null)
			{
				Unsafe.Write(&((AtkTextNode)textNodeById2).TextColor, config.NameTextColor.ToByteColor());
				Unsafe.Write(&((AtkTextNode)textNodeById2).EdgeColor, config.NameTextEdgeColor.ToByteColor());
				Unsafe.Write(&((AtkTextNode)textNodeById2).BackgroundColor, config.NameTextBackgroundColor.ToByteColor());
				((AtkTextNode)textNodeById2).FontSize = config.NameTextSize;
				((AtkTextNode)textNodeById2).SetPositionFloat(config.NameTextPosition.X, config.NameTextPosition.Y);
			}
			AtkComponentNode* nodeById = (AtkComponentNode*)((AtkUnitBase)Addons.CastBar).GetNodeById(8u);
			if (nodeById != null)
			{
				((AtkComponentNode)nodeById).SetAlpha(config.IconAlpha);
				((AtkComponentNode)nodeById).SetPositionFloat(config.IconPosition.X, config.IconPosition.Y);
				((AtkComponentNode)nodeById).SetScale(config.IconScale.X, config.IconScale.Y);
			}
			AtkTextNode* textNodeById3 = ((AtkUnitBase)Addons.CastBar).GetTextNodeById(6u);
			if (textNodeById3 != null)
			{
				Unsafe.Write(&((AtkTextNode)textNodeById3).TextColor, config.CastingTextColor.ToByteColor());
				Unsafe.Write(&((AtkTextNode)textNodeById3).EdgeColor, config.CastingTextEdgeColor.ToByteColor());
				Unsafe.Write(&((AtkTextNode)textNodeById3).BackgroundColor, config.CastingTextBackgroundColor.ToByteColor());
				((AtkTextNode)textNodeById3).FontSize = config.CastingTextSize;
				((AtkTextNode)textNodeById3).SetPositionFloat(config.CastingTextPosition.X, config.CastingTextPosition.Y);
			}
			AtkTextNode* textNodeById4 = ((AtkUnitBase)Addons.CastBar).GetTextNodeById(7u);
			if (textNodeById4 != null)
			{
				Unsafe.Write(&((AtkTextNode)textNodeById4).TextColor, config.CastTimeTextColor.ToByteColor());
				Unsafe.Write(&((AtkTextNode)textNodeById4).EdgeColor, config.CastTimeTextEdgeColor.ToByteColor());
				Unsafe.Write(&((AtkTextNode)textNodeById4).BackgroundColor, config.CastTimeTextBackgroundColor.ToByteColor());
				((AtkTextNode)textNodeById4).FontSize = config.CastTimeTextSize;
				((AtkTextNode)textNodeById4).SetPositionFloat(config.CastTimeTextPosition.X, config.CastTimeTextPosition.Y);
			}
		}
	}
}
