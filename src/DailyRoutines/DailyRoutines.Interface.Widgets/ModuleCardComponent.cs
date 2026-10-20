using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Interface.Widgets;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using DailyRoutines.Verification;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Em6RvKfqQAjPMdFMp3Y;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Windows.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;
using wUQLZCVUi4RNloOxE8c;

namespace DailyRoutines.Interface.Widgets;

public sealed class ModuleCardComponent : CardComponentBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass23_0
	{
		public ModuleBase module;

		public _003C_003Ec__DisplayClass23_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void H6qKrZ9Bve()
		{
			HfWYPNVIio8M9ncZnm0 hfWYPNVIio8M9ncZnm = OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>();
			if (hfWYPNVIio8M9ncZnm != null)
			{
				hfWYPNVIio8M9ncZnm.sv49SfOr7P<HfWYPNVIio8M9ncZnm0.d1MFHNO7lI9UoBJIpx5>().SelectedCategory = module.Info.Category;
				hfWYPNVIio8M9ncZnm.SelectedTab = HfWYPNVIio8M9ncZnm0.TabPageID.模块;
			}
		}

		internal void SyuKG4weRx()
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				module.PublicConfigUI();
			}
			catch (Exception ex)
			{
				ImU8String val = default(ImU8String);
				((ImU8String)(ref val))._002Ector(3, 2);
				((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD00B3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0)));
				((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6CDCCE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
				((ImU8String)(ref val)).AppendFormatted<Exception>(ex);
				ImGui.TextUnformatted(val);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass26_0
	{
		public ModuleBase module;

		public _003C_003Ec__DisplayClass26_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void ES9K6fdN4g()
		{
		}

		internal void ADNKk3muiJ()
		{
			_ = module.Info.ReportURL;
		}

		internal void pdlK2SBXLM()
		{
			module.ToggleOverlayConfig();
		}

		internal void TVBKZGJR0d()
		{
			FileHelper.OpenByPath(module.ConfigFilePath ?? string.Empty);
		}

		internal void iPtKark9Zc()
		{
			ManagerBase<ModuleManager>.Instance().ResetModuleAsync(module);
		}

		internal void sVkK0PA6TY()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			ImGui.SetClipboardText(ImU8String.op_Implicit(module.ModuleName));
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -1491840421 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F35BD80 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862) + module.ModuleName);
		}
	}

	private static readonly ConcurrentDictionary<string, ModuleCardComponent> Cards;

	private Vector2 descriptionRegion;

	private bool isConfigExpanded;

	private bool toolbarHovered;

	private double toolbarHoverTime;

	private float toolbarParam;

	private float toolbarParamVelocity;

	private Vector2 toolbarRegion;

	protected override bool DisableContent
	{
		get
		{
			if (YbMfgryPHo(out ModuleBase module) && !module.Permission.IsSkip())
			{
				return module.Permission.IsHide(AuthState.IsAuth, AuthState.IsPremium);
			}
			return true;
		}
	}

	protected override bool DrawDisabledMask => DisableContent;

	public ModuleCardComponent(string moduleName)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		_003CmoduleName_003EP = moduleName;
		descriptionRegion = new Vector2(100f);
		toolbarRegion = new Vector2(10f);
		base._002Ector();
	}

	public static ModuleCardComponent Get(ModuleBase module)
	{
		ArgumentNullException.ThrowIfNull(module, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEE413 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
		return Cards.GetOrAdd(module.ModuleName, (string key) => new ModuleCardComponent(key));
	}

	public static void Prune(IReadOnlyList<ModuleBase> modules)
	{
		ArgumentNullException.ThrowIfNull(modules, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE7DF5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
		HashSet<string> hashSet = modules.Select((ModuleBase x) => x.ModuleName).ToHashSet<string>(StringComparer.Ordinal);
		foreach (KeyValuePair<string, ModuleCardComponent> card in Cards)
		{
			if (!hashSet.Contains(card.Key))
			{
				Cards.TryRemove(card.Key, out ModuleCardComponent _);
			}
		}
	}

	public static bool IsConfigExpanded(ModuleBase module)
	{
		if (Cards.TryGetValue(module.ModuleName, out ModuleCardComponent value))
		{
			return value.isConfigExpanded;
		}
		return false;
	}

	protected override CardDrawContext CreateContext()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImGuiStylePtr style = ImGui.GetStyle();
		return new CardDrawContext(cursorScreenPos, cursorScreenPos, ((ImGuiStylePtr)(ref style)).WindowPadding, ImGui.GetContentRegionAvail().X - 8f * Globals.GlobalUIScale);
	}

	protected override void DrawContent(CardDrawContext context, bool isHovered)
	{
		tNtf3QPNIh(isHovered);
	}

	protected override Vector2 GetTargetSize(CardDrawContext context, Vector2 contentRectSize)
	{
		Vector2 result = contentRectSize;
		result.X = context.FrameWidth;
		return result;
	}

	protected override void DrawAfterFrame(CardDrawContext context, Vector2 frameMax, bool isHovered)
	{
		if (YbMfgryPHo(out ModuleBase module) && (!PluginConfig.Instance().CompactMode || (module.WithConfigUI && isConfigExpanded)))
		{
			xRbfh4WMdR(frameMax);
		}
	}

	private void tNtf3QPNIh(bool isCardHovered)
	{
		if (YbMfgryPHo(out ModuleBase module))
		{
			if (PluginConfig.Instance().CompactMode && (!module.WithConfigUI || !isConfigExpanded))
			{
				a4kfBMfmvN(module);
			}
			else
			{
				Lc3fmug0oA(module, isCardHovered);
			}
		}
	}

	private void a4kfBMfmvN(ModuleBase module)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			bool isEnabled = module.IsEnabled;
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ImGuiStylePtr style = ImGui.GetStyle();
			float x = ((ImGuiStylePtr)(ref style)).WindowPadding.X;
			float num = cursorScreenPos.X + ImGui.GetContentRegionAvail().X;
			ImGuiOm.ScaledDummy(4f);
			ImGui.SameLine();
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)7, ImGui.GetColorU32((ImGuiCol)2), true);
				try
				{
					bool selected = isEnabled;
					if (ImGuiOm.CheckboxColored(string.Empty, ref selected))
					{
						if (selected)
						{
							module.Load(affectConfig: true);
						}
						else
						{
							module.Unload(affectConfig: true);
						}
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
			ImGui.SameLine();
			ImGuiOm.ScaledDummy(4f);
			ImGui.SameLine();
			Vector2 cursorScreenPos2 = ImGui.GetCursorScreenPos();
			bool num2 = isEnabled && module.WithConfigUI;
			float frameHeight = ImGui.GetFrameHeight();
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			if (num2)
			{
				isConfigExpanded = nAofPUFaQS(module.Info.Title, isEnabled);
				Vector2 vector = cursorScreenPos2 + new Vector2(frameHeight * 0.5f, frameHeight * 0.5f);
				uint num3 = (isEnabled ? KnownColor.Chocolate.ToUInt() : KnownColor.NavajoWhite.ToUInt());
				float num4 = frameHeight * 0.22f;
				if (isConfigExpanded)
				{
					((ImDrawListPtr)(ref windowDrawList)).AddTriangleFilled(vector + new Vector2(0f - num4, (0f - num4) * 0.5f), vector + new Vector2(num4, (0f - num4) * 0.5f), vector + new Vector2(0f, num4), num3);
				}
				else
				{
					((ImDrawListPtr)(ref windowDrawList)).AddTriangleFilled(vector + new Vector2((0f - num4) * 0.5f, 0f - num4), vector + new Vector2(num4, 0f), vector + new Vector2((0f - num4) * 0.5f, num4), num3);
				}
			}
			else
			{
				isConfigExpanded = false;
				ImGui.Dummy(new Vector2(MathF.Max(0f, num - cursorScreenPos2.X), frameHeight));
			}
			float num5 = cursorScreenPos2.X + frameHeight;
			style = ImGui.GetStyle();
			float num6 = num5 + ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			float x2 = ImGui.CalcTextSize(ImU8String.op_Implicit(module.Info.Title), false, -1f).X;
			float num7 = num6 + x2;
			style = ImGui.GetStyle();
			float num8 = num7 + ((ImGuiStylePtr)(ref style)).ItemSpacing.X + 8f * Globals.GlobalUIScale;
			float num9 = num - x;
			string text = module.Info.Description.ReplaceLineEndings(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32DFDF06 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
			if (!(num6 >= num9))
			{
				float y = cursorScreenPos2.Y + (frameHeight - ImGui.GetTextLineHeight()) / 2f;
				((ImDrawListPtr)(ref windowDrawList)).PushClipRect(new Vector2(num6, cursorScreenPos2.Y), new Vector2(num9, cursorScreenPos2.Y + frameHeight), true);
				((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num6, y), isEnabled ? KnownColor.Chocolate.ToUInt() : KnownColor.NavajoWhite.ToUInt(), ImU8String.op_Implicit(module.Info.Title));
				if (num8 < num9)
				{
					((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num8, y), ImGuiColors.DalamudGrey3.ToUInt(), ImU8String.op_Implicit(text));
				}
				((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private void Lc3fmug0oA(ModuleBase module, bool isCardHovered)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass23_0 CS_0024_003C_003E8__locals27 = new _003C_003Ec__DisplayClass23_0();
		CS_0024_003C_003E8__locals27.module = module;
		GroupDisposable val = ImRaii.Group();
		try
		{
			bool isEnabled = CS_0024_003C_003E8__locals27.module.IsEnabled;
			float num = ImGui.GetCursorScreenPos().X + ImGui.GetContentRegionAvail().X;
			ImGuiOm.ScaledDummy(4f);
			ImGui.SameLine();
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				if (isCardHovered)
				{
					float frameHeight = ImGui.GetFrameHeight();
					Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
					ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
					((ImDrawListPtr)(ref windowDrawList)).ChannelsSetCurrent(CardComponentBase.GetDecorationChannel());
					float num2 = (MathF.Sin((float)ImGui.GetTime() * 3f) + 1f) * 0.5f;
					float w = 0.22f + num2 * 0.1f;
					Vector4 color = KnownColor.Chocolate.ToVector4();
					color.W = w;
					Vector2 vector = cursorScreenPos + new Vector2(frameHeight);
					ImGuiOm.AddGlowRect(windowDrawList, cursorScreenPos, vector, color.ToUInt(), 6f, 10f, 10);
					Vector4 vector2 = color;
					vector2.W *= 0.55f;
					((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos - new Vector2(1f), vector + new Vector2(1f), ImGui.ColorConvertFloat4ToU32(vector2), 6f, (ImDrawFlags)240, 1.5f);
					((ImDrawListPtr)(ref windowDrawList)).ChannelsSetCurrent(CardComponentBase.GetContentChannel());
				}
				ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)7, ImGui.GetColorU32((ImGuiCol)2), true);
				try
				{
					bool selected = isEnabled;
					if (ImGuiOm.CheckboxColored(string.Empty, ref selected))
					{
						if (selected)
						{
							CS_0024_003C_003E8__locals27.module.Load(affectConfig: true);
						}
						else
						{
							CS_0024_003C_003E8__locals27.module.Unload(affectConfig: true);
						}
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
			ImGui.SameLine();
			ImGuiOm.ScaledDummy(4f);
			ImGui.SameLine();
			GroupDisposable val4 = ImRaii.Group();
			try
			{
				ImGuiStylePtr style;
				StyleDisposable val6;
				if (isEnabled)
				{
					if (CS_0024_003C_003E8__locals27.module.WithConfigUI)
					{
						isConfigExpanded = sEDfif25np(CS_0024_003C_003E8__locals27.module.Info.Title, isEnabled);
						if (isConfigExpanded)
						{
							GroupDisposable val5 = ImRaii.Group();
							try
							{
								ImGuiOm.ScaledDummy(4f);
								CS_0024_003C_003E8__locals27.SyuKG4weRx();
							}
							finally
							{
								((GroupDisposable)(ref val5)).Dispose();
							}
						}
					}
					else
					{
						isConfigExpanded = false;
						val6 = ImRaii.PushStyle((ImGuiStyleVar)0, 0f, true);
						try
						{
							ImGui.Button(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61536)), default(Vector2));
						}
						finally
						{
							((IDisposable)val6)?.Dispose();
						}
						style = ImGui.GetStyle();
						ImGui.SameLine(0f, ((ImGuiStylePtr)(ref style)).FramePadding.X * 1.5f);
						Vector4 vector3 = KnownColor.Chocolate.ToVector4();
						ImGui.TextColored(ref vector3, ImU8String.op_Implicit(CS_0024_003C_003E8__locals27.module.Info.Title));
					}
				}
				else
				{
					isConfigExpanded = false;
					val6 = ImRaii.PushStyle((ImGuiStyleVar)0, 0f, true);
					try
					{
						ImGui.Button(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61536)), default(Vector2));
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					style = ImGui.GetStyle();
					ImGui.SameLine(0f, ((ImGuiStylePtr)(ref style)).ItemSpacing.X);
					ImGui.TextUnformatted(ImU8String.op_Implicit(CS_0024_003C_003E8__locals27.module.Info.Title));
				}
				ImGuiOm.ScaledDummy(1f, 4f);
				bool flag = descriptionRegion.X * 1.1f + toolbarRegion.X >= ImGui.GetContentRegionAvail().X;
				val6 = ImRaii.PushStyle((ImGuiStyleVar)0, (toolbarHovered && flag) ? 0.2f : 1f, true);
				try
				{
					GroupDisposable val7 = ImRaii.Group();
					try
					{
						ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)0, ImGuiColors.DalamudGrey3, true);
						try
						{
							ImGui.TextWrapped(ImU8String.op_Implicit(CS_0024_003C_003E8__locals27.module.Info.Description));
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
						ImGuiOm.ScaledDummy(4f);
						Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816A63E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd) + CS_0024_003C_003E8__locals27.module.Info.Category), delegate
						{
							HfWYPNVIio8M9ncZnm0 hfWYPNVIio8M9ncZnm = OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>();
							if (hfWYPNVIio8M9ncZnm != null)
							{
								hfWYPNVIio8M9ncZnm.sv49SfOr7P<HfWYPNVIio8M9ncZnm0.d1MFHNO7lI9UoBJIpx5>().SelectedCategory = CS_0024_003C_003E8__locals27.module.Info.Category;
								hfWYPNVIio8M9ncZnm.SelectedTab = HfWYPNVIio8M9ncZnm0.TabPageID.模块;
							}
						});
						if (CS_0024_003C_003E8__locals27.module.Info.PreviewImageURL.Length == 1)
						{
							ImGui.SameLine();
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
							defaultInterpolatedStringHandler.AppendFormatted(FontAwesomeExtensions.ToIconChar((FontAwesomeIcon)61502));
							defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70688447 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
							defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD4CA7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0)));
							Oc1fEFQRl5(defaultInterpolatedStringHandler.ToStringAndClear());
							if (ImGui.IsItemHovered())
							{
								TooltipDisposable val8 = ImRaii.Tooltip();
								try
								{
									if (OmenServiceBase<ImageHelper>.Instance().TryGetImage(CS_0024_003C_003E8__locals27.module.Info.PreviewImageURL[0], out IDalamudTextureWrap texture))
									{
										ImGui.Image(texture.Handle, texture.Size);
									}
									else
									{
										ImU8String val9 = new ImU8String(3, 1);
										((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D004F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc)));
										((ImU8String)(ref val9)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D208D8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
										ImGui.TextDisabled(val9);
									}
								}
								finally
								{
									((TooltipDisposable)(ref val8)).Dispose();
								}
							}
						}
						ImGui.SameLine(0f, 10f * Globals.GlobalUIScale);
						if (CS_0024_003C_003E8__locals27.module.Permission.IsDefaultEnabled())
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642AB9D6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df)));
						}
						if (CS_0024_003C_003E8__locals27.module.Permission.NeedAuth)
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5583A03C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d)), delegate
							{
								OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>().SelectedTab = HfWYPNVIio8M9ncZnm0.TabPageID.验证;
							});
						}
						if (CS_0024_003C_003E8__locals27.module.Permission.CNOnly)
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE7D01 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC07AA6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E461497 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6117FF80 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402));
						}
						if (CS_0024_003C_003E8__locals27.module.Permission.GLOnly)
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4B230 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC07AA6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B212866 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFE693 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
						}
						if (CS_0024_003C_003E8__locals27.module.Permission.TCOnly)
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1675397288 ^ -306284703 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719C9BB3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FF9A68 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200E30C5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));
						}
						if (CS_0024_003C_003E8__locals27.module.Permission.KROnly)
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F35BD5A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A39206 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x282849D1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F0E003 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
						}
						if (CS_0024_003C_003E8__locals27.module.Permission.CNPremium)
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A37DA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829D3E5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x654083F2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-864210836 ^ -1573208439 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e), delegate
							{
								OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>().SelectedTab = HfWYPNVIio8M9ncZnm0.TabPageID.验证;
							});
						}
						if (CS_0024_003C_003E8__locals27.module.Permission.GLPremium)
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C454E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829D3E5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFB8339 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA61327 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff), delegate
							{
								OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>().SelectedTab = HfWYPNVIio8M9ncZnm0.TabPageID.验证;
							});
						}
						if (CS_0024_003C_003E8__locals27.module.Permission.TCPremium)
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FF9A2C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x392385B2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9069DDC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1003249140 ^ -1466584789 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff), delegate
							{
								OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>().SelectedTab = HfWYPNVIio8M9ncZnm0.TabPageID.验证;
							});
						}
						if (CS_0024_003C_003E8__locals27.module.Permission.KRPremium)
						{
							ImGui.SameLine();
							Oc1fEFQRl5(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642AB81C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x55823B9A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C9128 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9070792 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3), delegate
							{
								OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>().SelectedTab = HfWYPNVIio8M9ncZnm0.TabPageID.验证;
							});
						}
						ImGui.SameLine();
						OsIfw9e4TX(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A6A1B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1)), CS_0024_003C_003E8__locals27.module.PrecedingModules);
						ImGui.SameLine();
						OsIfw9e4TX(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A376C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a)), CS_0024_003C_003E8__locals27.module.PairModules);
						ImGui.SameLine();
						OsIfw9e4TX(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7DD2E4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571)), CS_0024_003C_003E8__locals27.module.ConflictModules);
					}
					finally
					{
						((GroupDisposable)(ref val7)).Dispose();
					}
					descriptionRegion = ImGui.GetItemRectSize();
				}
				finally
				{
					((IDisposable)val6)?.Dispose();
				}
			}
			finally
			{
				((GroupDisposable)(ref val4)).Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private static bool sEDfif25np(string title, bool isModuleEnabled)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, isModuleEnabled ? KnownColor.Chocolate.ToVector4() : KnownColor.NavajoWhite.ToVector4(), true).Push((ImGuiCol)24, 0u, true).Push((ImGuiCol)25, 0u, true)
			.Push((ImGuiCol)26, 0u, true);
		try
		{
			return ImGui.CollapsingHeader(ImU8String.op_Implicit(title), (ImGuiTreeNodeFlags)0);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static bool nAofPUFaQS(string title, bool isModuleEnabled)
	{
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)0, 0f, true);
		try
		{
			return sEDfif25np(title, isModuleEnabled);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void xRbfh4WMdR(Vector2 bottomRight)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass26_0 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass26_0();
		if (!YbMfgryPHo(out CS_0024_003C_003E8__locals12.module))
		{
			return;
		}
		ImGuiStylePtr style = ImGui.GetStyle();
		Vector2 itemSpacing = ((ImGuiStylePtr)(ref style)).ItemSpacing;
		float num = 6f * Globals.GlobalUIScale;
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61641)), false, -1f);
			float num2 = vector.X + num * 2f;
			float y = vector.Y + num * 1.5f;
			float num3 = num * 2f;
			if (CS_0024_003C_003E8__locals12.module.WithConfigUI)
			{
				num3 += num + ImGui.CalcTextSize(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)57357)), false, -1f).X;
			}
			if (CS_0024_003C_003E8__locals12.module.WithConfig)
			{
				num3 += num + ImGui.CalcTextSize(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61787)), false, -1f).X;
			}
			num3 += num + ImGui.CalcTextSize(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61470)), false, -1f).X;
			num3 += num + ImGui.CalcTextSize(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61637)), false, -1f).X;
			num3 += num + ImGui.CalcTextSize(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61445)), false, -1f).X;
			float num4 = (toolbarHovered ? 1f : 0f);
			float num5 = toolbarParam - num4;
			ImGuiIOPtr iO = ImGui.GetIO();
			float num6 = Math.Min(((ImGuiIOPtr)(ref iO)).DeltaTime, 0.05f);
			float num7 = -200f * num5 - 20f * toolbarParamVelocity;
			toolbarParamVelocity += num7 * num6;
			toolbarParam += toolbarParamVelocity * num6;
			if (Math.Abs(num5) < 0.001f && Math.Abs(toolbarParamVelocity) < 0.001f)
			{
				toolbarParam = num4;
				toolbarParamVelocity = 0f;
			}
			float x = num2 + toolbarParam * (num3 - num2);
			Vector2 vector2 = new Vector2(x, y);
			Vector2 vector3 = bottomRight - vector2 - 3f * itemSpacing;
			Vector2 mousePos = ImGui.GetMousePos();
			if ((mousePos.X >= vector3.X && mousePos.X <= vector3.X + vector2.X && mousePos.Y >= vector3.Y && mousePos.Y <= vector3.Y + vector2.Y) || ImGui.IsPopupOpen(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B304EE4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117)), (ImGuiPopupFlags)0))
			{
				double num8 = toolbarHoverTime;
				iO = ImGui.GetIO();
				toolbarHoverTime = num8 + (double)((ImGuiIOPtr)(ref iO)).DeltaTime;
				if (toolbarHoverTime > 0.20000000298023224)
				{
					toolbarHovered = true;
					toolbarHoverTime = 0.5;
				}
			}
			else
			{
				double num9 = toolbarHoverTime;
				iO = ImGui.GetIO();
				toolbarHoverTime = num9 - (double)((ImGuiIOPtr)(ref iO)).DeltaTime;
				if (toolbarHoverTime <= 0.0)
				{
					toolbarHovered = false;
					toolbarHoverTime = 0.0;
				}
			}
			toolbarRegion = vector2;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector3, vector3 + vector2, ImGui.GetColorU32((ImGuiCol)10), vector2.Y / 2f);
			ColorDisposable val;
			if (toolbarParam < 0.5f)
			{
				float num10 = 1f - toolbarParam * 2f;
				ImGui.SetCursorScreenPos(vector3 + vector2 / 2f - vector / 2f);
				val = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)0, num10), true);
				try
				{
					sY9fbhuXp2((FontAwesomeIcon)61641, string.Empty, delegate
					{
					});
					return;
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			float num11 = (toolbarParam - 0.5f) * 2f;
			ImGui.SetCursorScreenPos(vector3 + new Vector2(num, (vector2.Y - vector.Y) / 2f));
			val = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)0, num11), true);
			try
			{
				StyleDisposable val2 = ImRaii.PushStyle((ImGuiStyleVar)0, num11, true);
				try
				{
					ImGui.Dummy(Vector2.Zero);
					if (CS_0024_003C_003E8__locals12.module.WithConfigUI)
					{
						ImGui.SameLine(0f, num);
						DisabledDisposable val3 = ImRaii.Disabled(!CS_0024_003C_003E8__locals12.module.IsEnabled);
						try
						{
							sY9fbhuXp2((FontAwesomeIcon)57357, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816A400 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd)), delegate
							{
								CS_0024_003C_003E8__locals12.module.ToggleOverlayConfig();
							});
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					if (CS_0024_003C_003E8__locals12.module.WithConfig)
					{
						ImGui.SameLine(0f, num);
						sY9fbhuXp2((FontAwesomeIcon)61787, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD4E73 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0)), delegate
						{
							FileHelper.OpenByPath(CS_0024_003C_003E8__locals12.module.ConfigFilePath ?? string.Empty);
						});
					}
					ImGui.SameLine(0f, num);
					sY9fbhuXp2((FontAwesomeIcon)61470, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E461767 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e)), delegate
					{
						ManagerBase<ModuleManager>.Instance().ResetModuleAsync(CS_0024_003C_003E8__locals12.module);
					});
					ImGui.SameLine(0f, num);
					sY9fbhuXp2((FontAwesomeIcon)61637, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B212A58 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1E2AE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83) + CS_0024_003C_003E8__locals12.module.ModuleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDB9ACD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1), delegate
					{
						//IL_0012: Unknown result type (might be due to invalid IL or missing references)
						ImGui.SetClipboardText(ImU8String.op_Implicit(CS_0024_003C_003E8__locals12.module.ModuleName));
						OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -1491840421 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F35BD80 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862) + CS_0024_003C_003E8__locals12.module.ModuleName);
					});
					ImGui.SameLine(0f, num);
					bf2foxxHrl();
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

	private void bf2foxxHrl()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		if (!YbMfgryPHo(out ModuleBase module))
		{
			return;
		}
		GroupDisposable val = ImRaii.Group();
		try
		{
			StyleDisposable val2 = ImRaii.PushStyle((ImGuiStyleVar)0, 0.4f, !dSYQxEfFIB7h6WeehuR.MlOfcRFDSn(module));
			try
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61445)));
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGuiOm.TooltipHover(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE8079 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76)));
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked())
			{
				ImGui.OpenPopup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2265CE7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44)), (ImGuiPopupFlags)0);
			}
			PopupDisposable val3 = ImRaii.Popup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD4ECD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0)));
			try
			{
				if (!PopupDisposable.op_Implicit(val3))
				{
					return;
				}
				using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
				{
					ImU8String val4 = new ImU8String(1, 2);
					((ImU8String)(ref val4)).AppendFormatted<string>(dSYQxEfFIB7h6WeehuR.cA6fAjfWGn(module, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF19EAA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d)) ? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B212A28 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a) : GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D03B3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc));
					((ImU8String)(ref val4)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E058EA7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
					((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C79414A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00)));
					if (ImGui.MenuItem(val4, false, true))
					{
						dSYQxEfFIB7h6WeehuR.WBPfQPGrOH(module, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719CC14B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc));
					}
					foreach (KeyValuePair<string, HashSet<string>> moduleCustom in PluginConfig.Instance().ModuleCustoms)
					{
						ImU8String val5 = new ImU8String(1, 2);
						((ImU8String)(ref val5)).AppendFormatted<string>(dSYQxEfFIB7h6WeehuR.cA6fAjfWGn(module, moduleCustom.Key) ? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C931A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac) : GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x41191C6B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
						((ImU8String)(ref val5)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FB8192B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
						((ImU8String)(ref val5)).AppendFormatted<string>(moduleCustom.Key);
						if (ImGui.MenuItem(val5, false, true))
						{
							dSYQxEfFIB7h6WeehuR.WBPfQPGrOH(module, moduleCustom.Key);
						}
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val3)).Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private bool YbMfgryPHo(out ModuleBase module)
	{
		module = ManagerBase<ModuleManager>.Instance().GetModuleByName(_003CmoduleName_003EP);
		return module != null;
	}

	private static void OsIfw9e4TX(string prefix, List<ModuleBase> modules)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		if (modules == null || modules.Count <= 0)
		{
			return;
		}
		GroupDisposable val = ImRaii.Group();
		try
		{
			StyleDisposable val2 = ImRaii.PushStyle((ImGuiStyleVar)11, 8f, true);
			try
			{
				ImU8String val3 = new ImU8String(4, 3);
				((ImU8String)(ref val3)).AppendFormatted<string>(prefix);
				((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D14DE97 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
				((ImU8String)(ref val3)).AppendFormatted<int>(modules.Count((ModuleBase m) => m.IsEnabled));
				((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC04508 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
				((ImU8String)(ref val3)).AppendFormatted<int>(modules.Count);
				((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FE0026 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
				if (ImGui.Button(val3, default(Vector2)))
				{
					ImU8String val4 = new ImU8String(6, 1);
					((ImU8String)(ref val4)).AppendFormatted<string>(prefix);
					((ImU8String)(ref val4)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D035D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc));
					ImGui.OpenPopup(val4, (ImGuiPopupFlags)0);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImU8String val5 = new ImU8String(6, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(prefix);
			((ImU8String)(ref val5)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC502B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1));
			PopupDisposable val6 = ImRaii.Popup(val5);
			try
			{
				if (!val6)
				{
					return;
				}
				foreach (ModuleBase module in modules)
				{
					bool selected = module.IsEnabled;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 1);
					defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D154669 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
					defaultInterpolatedStringHandler.AppendFormatted(module);
					if (ImGuiOm.CheckboxColored(defaultInterpolatedStringHandler.ToStringAndClear(), ref selected))
					{
						if (selected)
						{
							module.Load(affectConfig: true);
						}
						else
						{
							module.Unload(affectConfig: true);
						}
					}
					ImGui.SameLine();
					ColorDisposable val7 = ImRaii.PushColor((ImGuiCol)0, KnownColor.Chocolate.ToVector4(), module.IsEnabled);
					try
					{
						ImGui.TextUnformatted(ImU8String.op_Implicit(module.Info.Title));
					}
					finally
					{
						((IDisposable)val7)?.Dispose();
					}
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						if (ImGui.IsMouseDoubleClicked((ImGuiMouseButton)0))
						{
							OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>()?.sv49SfOr7P<HfWYPNVIio8M9ncZnm0.OMQ0W4KSvbgxtAOQfUK>().L59KV4Y5ei(module.Info.Title);
						}
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val6)).Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private static void Oc1fEFQRl5(string tagName, Action? action = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)11, 8f, true);
		try
		{
			ImU8String val2 = new ImU8String(0, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(tagName);
			if (ImGui.Button(val2, default(Vector2)))
			{
				action?.Invoke();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	static ModuleCardComponent()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Cards = new ConcurrentDictionary<string, ModuleCardComponent>(StringComparer.Ordinal);
	}

	[CompilerGenerated]
	internal static void sY9fbhuXp2(FontAwesomeIcon icon, string tooltip, Action clickAction)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(icon)));
			ImGuiOm.TooltipHover(tooltip);
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked())
			{
				clickAction();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}
}
