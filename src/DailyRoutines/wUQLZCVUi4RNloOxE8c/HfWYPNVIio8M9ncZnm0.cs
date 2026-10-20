using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using BnDxeK5FguM0XdePWY;
using DailyRoutines;
using DailyRoutines.AnimationLockOptions;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Interface.Widgets;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Extensions;
using DailyRoutines.Interface.Widgets;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using DailyRoutines.Verification;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using KSRRgayQIDbxlOyXNEF;
using Lhk7YYyZgDg4lZPGxVj;
using LqiFr6AUanXXiJKotr;
using Lumina.Data;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using ThFWmWFIGyTX23kBtl;
using hVantRHmXPV4BqTZP7X;
using hcXTsMLIXdeaKdE7xb7;
using iGxwSFV6Y2uamybGI4O;
using itAnDoW4mgYVlrmtMor;

namespace wUQLZCVUi4RNloOxE8c;

internal class HfWYPNVIio8M9ncZnm0 : Window, IDisposable
{
	internal class PsFBitORrb7AIRB0KYJ : swio5KOzdbVemDXeuDN
	{
		private sealed class ufva4NWfDLl3j6vobx2 : CardComponentBase
		{
			private Vector2 contentRegionAvail;

			private static readonly Vector2 Padding;

			protected override Vector2 InitialSize => new Vector2(450f, 400f);

			public ufva4NWfDLl3j6vobx2(PsFBitORrb7AIRB0KYJ auth)
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				_003Cauth_003EP = auth;
				base._002Ector();
			}

			protected override CardDrawContext CreateContext()
			{
				contentRegionAvail = ImGui.GetContentRegionAvail();
				if (contentRegionAvail.Y > 2000f)
				{
					contentRegionAvail.Y = ImGui.GetWindowHeight() - ImGui.GetCursorPosY();
				}
				Vector2 vector = (contentRegionAvail - base.CurrentSize - Globals.ScaledVector2(0f, 50f)) / 2f;
				vector = new Vector2(MathF.Round(vector.X), MathF.Round(vector.Y));
				vector.X = Math.Max(vector.X, 0f);
				vector.Y = Math.Max(vector.Y, 0f);
				Vector2 vector2 = ImGui.GetCursorScreenPos() + vector;
				return new CardDrawContext(vector2, vector2 + Padding, Padding);
			}

			protected override void DrawContent(CardDrawContext context, bool isHovered)
			{
				float x = 500f * Globals.GlobalUIScale;
				float y = contentRegionAvail.X - Padding.X * 2f - 20f;
				_003Cauth_003EP.fLjODLSmMh(MathF.Min(x, y));
			}

			protected override Vector2 GetTargetSize(CardDrawContext context, Vector2 contentRectSize)
			{
				return contentRectSize + Padding * 2f;
			}

			static ufva4NWfDLl3j6vobx2()
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				Padding = Globals.ScaledVector2(24f);
			}
		}

		private readonly ufva4NWfDLl3j6vobx2 authCard;

		private string premiumCredentialInput;

		public override TabPageID PageID => TabPageID.验证;

		public PsFBitORrb7AIRB0KYJ(HfWYPNVIio8M9ncZnm0 window)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			premiumCredentialInput = PluginConfig.Instance().PremiumCredential;
			base._002Ector(window);
			authCard = new ufva4NWfDLl3j6vobx2(this);
		}

		public override void Dispose()
		{
		}

		public override void hiNTxC7Py0()
		{
			if (swio5KOzdbVemDXeuDN.zoWKHnZvLt("No Spyware", base.SelectedTab == PageID))
			{
				base.SelectedTab = PageID;
				base.hiNTxC7Py0();
			}
		}

		public override void Draw()
		{
			authCard.Draw();
		}

		private void fLjODLSmMh(float contentWidth)
		{
			//Discarded unreachable code: IL_007d, IL_008c, IL_0099
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0320: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_037e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0394: Unknown result type (might be due to invalid IL or missing references)
			//IL_0399: Unknown result type (might be due to invalid IL or missing references)
			//IL_039b: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
			ILbWvcLd7xPWOTvckYC lbWvcLd7xPWOTvckYC = ManagerBase<ILbWvcLd7xPWOTvckYC>.Instance();
			bool isConnected = AuthState.IsConnected;
			Vector2 cursorPos = ImGui.GetCursorPos();
			ImGui.SetCursorPos(cursorPos);
			using (OmenServiceBase<FontManager>.Instance().UIFont160.Push())
			{
				float x = ImGui.CalcTextSize(ImU8String.op_Implicit("ziz - No Spyware"), false, -1f).X;
				ImGui.SetCursorPosX(cursorPos.X + (contentWidth - x) / 2f);
				ImGui.TextUnformatted(ImU8String.op_Implicit("ziz - No Spyware"));
			}
			ImGui.Spacing();
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit("edited by ziz"), false, -1f);
				ImGuiStylePtr style = ImGui.GetStyle();
				float x2 = ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
				float num = vector.X + x2;
				ImGui.SetCursorPosX(cursorPos.X + (contentWidth - num) / 2f);
				uint num2 = ((!isConnected) ? KnownColor.Red.ToUInt() : 4283464630u);
				ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, num2, true);
				try
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit("edited by ziz"));
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			ImGui.Spacing();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddLine(cursorScreenPos, cursorScreenPos + new Vector2(contentWidth, 0f), ImGui.GetColorU32((ImGuiCol)27));
			ImGui.Dummy(new Vector2(contentWidth, 1f));
			ImGui.Spacing();
			if (!lbWvcLd7xPWOTvckYC.lDEHnP9BYU<QVayY70lyiGAjopaKX>(out QVayY70lyiGAjopaKX credential) || !lbWvcLd7xPWOTvckYC.lDEHnP9BYU<zTMwYPctmcDQxmkmbm>(out zTMwYPctmcDQxmkmbm credential2) || !lbWvcLd7xPWOTvckYC.lDEHnP9BYU<ksA3tkbnvUocY33ps0>(out ksA3tkbnvUocY33ps0 credential3))
			{
				string text = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7DD396 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
				float x3 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X;
				ImGui.SetCursorPosX(cursorPos.X + (contentWidth - x3) / 2f);
				ImGui.TextDisabled(ImU8String.op_Implicit(text));
				return;
			}
			DisabledDisposable val2 = ImRaii.Disabled(!isConnected);
			try
			{
				TableDisposable val3 = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BAA729 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503)), 2, (ImGuiTableFlags)24576, new Vector2(contentWidth, 0f));
				try
				{
					if (!(!val3))
					{
						ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC7569 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1)), (ImGuiTableColumnFlags)8, 0f, 0u);
						ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F154EB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)), (ImGuiTableColumnFlags)4, 0f, 0u);
						QMjOe8Acf4(credential, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-864210836 ^ -829656523 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)), LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E386DF1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997)), new string[2]
						{
							LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA3823 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1)),
							LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE4413 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76))
						});
						QMjOe8Acf4(credential2, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1317373447 ^ 0x62445D81 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83)), LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFBB863 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc)), new string[1] { LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFBBBE9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc)) });
						QMjOe8Acf4(credential3, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F141B3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)), LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF065C8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d)), new string[1] { LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x282873B5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184)) });
						if (AuthState.IsAuth)
						{
							NHdONX96dm();
						}
					}
				}
				finally
				{
					((TableDisposable)(ref val3)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}

		private static void QMjOe8Acf4(XX0GFgy2GYwT9tTLeoM credential, string name, string usageHint = null, string[]? emptyHints = null)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_040a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0376: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0431: Unknown result type (might be due to invalid IL or missing references)
			//IL_0436: Unknown result type (might be due to invalid IL or missing references)
			//IL_0445: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(0, 1);
			((ImU8String)(ref val)).AppendFormatted<string>(name);
			IdDisposable val2 = ImRaii.PushId(val, true);
			try
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.AlignTextToFramePadding();
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(name));
				if (!string.IsNullOrEmpty(usageHint))
				{
					ImGuiOm.HelpMarker(usageHint, 40f, (FontAwesomeIcon)61530);
				}
				ImGui.TableNextColumn();
				float x = ImGui.GetContentRegionAvail().X;
				if (!AuthState.IsConnected)
				{
					uint num = KnownColor.OrangeRed.ToUInt();
					ImU8String val3 = new ImU8String(2, 1);
					((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719CBCAB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc));
					((ImU8String)(ref val3)).AppendFormatted<string>("No spyware, your data is safe and in your hands");
					((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51331AD0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
					ImGui.TextColored(num, val3);
					return;
				}
				if (credential.IsEmpty)
				{
					ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)0, KnownColor.OrangeRed.ToUInt(), true);
					try
					{
						ImU8String val5 = new ImU8String(2, 1);
						((ImU8String)(ref val5)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E39E8DB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
						((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-864210836 ^ -829656869 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)));
						((ImU8String)(ref val5)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F17183 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
						ImGui.Button(val5, new Vector2(x, 0f));
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
					if (ImGui.IsItemHovered())
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768FE3F1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46)));
						foreach (string value in emptyHints)
						{
							StringBuilder stringBuilder2 = stringBuilder;
							StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
							handler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642A8550 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df));
							handler.AppendFormatted(value);
							stringBuilder2.Append(ref handler);
						}
						ImU8String tooltip = new ImU8String(0, 1);
						((ImU8String)(ref tooltip)).AppendFormatted<StringBuilder>(stringBuilder);
						ImGui.SetTooltip(tooltip);
					}
					ImU8String val6 = new ImU8String(1, 1);
					((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x41192341 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52)));
					((ImU8String)(ref val6)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7DEF78 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
					ImGui.TextDisabled(val6);
					return;
				}
				GroupDisposable val7 = ImRaii.Group();
				try
				{
					ImU8String val8 = new ImU8String(2, 2);
					((ImU8String)(ref val8)).AppendFormatted<string>(credential.Token);
					((ImU8String)(ref val8)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53797D91 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09));
					((ImU8String)(ref val8)).AppendFormatted<string>(name);
					if (ImGui.Button(val8, new Vector2(x, 0f)))
					{
						string text = Ns9xPkyALXVrdKCp8ew.XxRyJiHmUK(credential.Token);
						if (!string.IsNullOrEmpty(text))
						{
							ImGui.SetClipboardText(ImU8String.op_Implicit(text));
							OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BA9C07 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503)));
						}
					}
					ImU8String val9 = new ImU8String(1, 1);
					((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A2352C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079)));
					((ImU8String)(ref val9)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3706AD6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e));
					ImGui.TextDisabled(val9);
					ImGui.SameLine();
					uint num2 = (credential.IsActivated ? KnownColor.LawnGreen : KnownColor.DarkGray).ToUInt();
					ImU8String val10 = new ImU8String(0, 1);
					((ImU8String)(ref val10)).AppendFormatted<DateTime?>(credential.ExpiredTime);
					ImGui.TextColored(num2, val10);
				}
				finally
				{
					((GroupDisposable)(ref val7)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}

		private void NHdONX96dm()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E38682B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997)), true);
			try
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.AlignTextToFramePadding();
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6540BE60 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb))));
				ImGuiOm.HelpMarker(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379B3D3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09)), 40f, (FontAwesomeIcon)61530);
				ImGui.TableNextColumn();
				float x = ImGui.GetContentRegionAvail().X;
				if (AuthState.IsPremium)
				{
					ImGui.Button(ImU8String.op_Implicit(premiumCredentialInput), new Vector2(x, 0f));
					ImGuiOmExtension.ClickToCopyAndNotify(premiumCredentialInput);
					return;
				}
				ImGui.SetNextItemWidth(x);
				ImGui.InputText(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFA65E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2)), ref premiumCredentialInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					PluginConfig.Instance().PremiumCredential = premiumCredentialInput;
					PluginConfig.Instance().Save();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	internal class d1MFHNO7lI9UoBJIpx5 : swio5KOzdbVemDXeuDN
	{
		public ModuleCategory SelectedCategory;

		private Dictionary<ModuleCategory, List<ModuleBase>> categorized;

		public override TabPageID PageID => TabPageID.模块;

		public d1MFHNO7lI9UoBJIpx5(HfWYPNVIio8M9ncZnm0 window)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			categorized = new Dictionary<ModuleCategory, List<ModuleBase>>();
			base._002Ector(window);
			ManagerBase<ModuleManager>.Instance().OnModuleChanged += GUkOdpRXjO;
		}

		public override void hiNTxC7Py0()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			if (base.SelectedTab != PageID)
			{
				SelectedCategory = ModuleCategory.None;
			}
			using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
			{
				GroupDisposable val = ImRaii.Group();
				try
				{
					ModuleCategory[] values = Enum.GetValues<ModuleCategory>();
					foreach (ModuleCategory moduleCategory in values)
					{
						if (moduleCategory == ModuleCategory.None)
						{
							continue;
						}
						if (moduleCategory == ModuleCategory.Duty)
						{
							ImGuiOm.ScaledDummy(4f);
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 1);
						defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D342852 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33));
						defaultInterpolatedStringHandler.AppendFormatted(moduleCategory);
						if (swio5KOzdbVemDXeuDN.zoWKHnZvLt(LanguageManager.Get(defaultInterpolatedStringHandler.ToStringAndClear()), base.SelectedTab == PageID && SelectedCategory == moduleCategory))
						{
							base.hiNTxC7Py0();
							Window.isNeedToScrollToTop = true;
							SelectedCategory = moduleCategory;
							base.SelectedTab = PageID;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 1);
						defaultInterpolatedStringHandler2.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D342852 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33));
						defaultInterpolatedStringHandler2.AppendFormatted(moduleCategory);
						defaultInterpolatedStringHandler2.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE40DD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
						ImGuiOm.TooltipHover(LanguageManager.Get(defaultInterpolatedStringHandler2.ToStringAndClear()), 20f * Globals.GlobalUIScale);
						DisabledDisposable val2 = ImRaii.Disabled(ManagerBase<ModuleManager>.Instance().IsBusy);
						try
						{
							ImU8String val3 = new ImU8String(19, 1);
							((ImU8String)(ref val3)).AppendFormatted<ModuleCategory>(moduleCategory);
							((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51332D7A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
							PopupDisposable val4 = ImRaii.ContextPopupItem(val3);
							try
							{
								if (PopupDisposable.op_Implicit(val4))
								{
									if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7B5D3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff))), false, true) && categorized.TryGetValue(SelectedCategory, out List<ModuleBase> value))
									{
										ManagerBase<ModuleManager>.Instance().LoadBatchAsync(value);
									}
									if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B307172 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117))), false, true) && categorized.TryGetValue(SelectedCategory, out List<ModuleBase> value2))
									{
										ManagerBase<ModuleManager>.Instance().UnloadBatchAsync(value2);
									}
								}
							}
							finally
							{
								((PopupDisposable)(ref val4)).Dispose();
							}
						}
						finally
						{
							((IDisposable)val2)?.Dispose();
						}
					}
				}
				finally
				{
					((GroupDisposable)(ref val)).Dispose();
				}
			}
		}

		public override void Draw()
		{
			if (categorized.TryGetValue(SelectedCategory, out List<ModuleBase> value))
			{
				P5KKnMnhkq(value);
			}
		}

		public override void Dispose()
		{
			ManagerBase<ModuleManager>.Instance().OnModuleChanged -= GUkOdpRXjO;
		}

		private void GUkOdpRXjO(IReadOnlyList<ModuleBase> modules)
		{
			categorized = (from x in modules
				group x by x.Info.Category).ToDictionary((IGrouping<ModuleCategory, ModuleBase> x) => x.Key, (IGrouping<ModuleCategory, ModuleBase> x) => x.ToList());
		}
	}

	internal class L19rdTOIWaZP57ZxwlW : swio5KOzdbVemDXeuDN
	{
		private List<ModuleBase> enabled;

		public override TabPageID PageID => TabPageID.启用;

		public L19rdTOIWaZP57ZxwlW(HfWYPNVIio8M9ncZnm0 window)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			enabled = new List<ModuleBase>();
			base._002Ector(window);
			ManagerBase<ModuleManager>.Instance().OnModuleChanged += AAXOUoXR7m;
		}

		public override void hiNTxC7Py0()
		{
			if (swio5KOzdbVemDXeuDN.zoWKHnZvLt(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D15B851 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275)), base.SelectedTab == PageID))
			{
				base.SelectedTab = PageID;
				base.hiNTxC7Py0();
			}
		}

		public override void Draw()
		{
			P5KKnMnhkq(enabled);
		}

		private void AAXOUoXR7m(IReadOnlyList<ModuleBase> modules)
		{
			enabled = modules.Where((ModuleBase x) => x.IsEnabled).ToList();
		}

		public override void Dispose()
		{
			ManagerBase<ModuleManager>.Instance().OnModuleChanged -= AAXOUoXR7m;
		}
	}

	internal class Ilo1xvOu4IfQAk4pJC7 : swio5KOzdbVemDXeuDN
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass8_0
		{
			public KeyValuePair<string, HashSet<string>> customPair;

			public _003C_003Ec__DisplayClass8_0()
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				base._002Ector();
			}

			internal bool B1cWCnuNrb(ModuleBase x)
			{
				return customPair.Value.Contains(x.ModuleName);
			}
		}

		private List<ModuleBase> favorite;

		private Dictionary<string, List<ModuleBase>> custom;

		private string newFavoriteInput;

		public override TabPageID PageID => TabPageID.收藏;

		public Ilo1xvOu4IfQAk4pJC7(HfWYPNVIio8M9ncZnm0 window)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			favorite = new List<ModuleBase>();
			custom = new Dictionary<string, List<ModuleBase>>();
			newFavoriteInput = string.Empty;
			base._002Ector(window);
			ManagerBase<ModuleManager>.Instance().OnModuleChanged += q2YOXn2n97;
		}

		public override void hiNTxC7Py0()
		{
			if (swio5KOzdbVemDXeuDN.zoWKHnZvLt(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B30B176 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117)), base.SelectedTab == PageID))
			{
				base.SelectedTab = PageID;
				base.hiNTxC7Py0();
			}
		}

		public override void Draw()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_034e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0373: Unknown result type (might be due to invalid IL or missing references)
			//IL_0378: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
			TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DC8142 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f)), (ImGuiTabBarFlags)9);
			try
			{
				if (!val)
				{
					return;
				}
				if (ImGui.TabItemButton(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61543)), (ImGuiTabItemFlags)96))
				{
					ImGui.OpenPopup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFA69C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2)), (ImGuiPopupFlags)0);
				}
				PopupDisposable val2 = ImRaii.Popup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x39222310 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2)));
				try
				{
					if (PopupDisposable.op_Implicit(val2))
					{
						ImGui.InputTextWithHint(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6540BF1E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb)), ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28169868 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd))), ref newFavoriteInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						DisabledDisposable val3 = ImRaii.Disabled(string.IsNullOrWhiteSpace(newFavoriteInput) || PluginConfig.Instance().ModuleCustoms.ContainsKey(newFavoriteInput));
						try
						{
							if (ImGuiOm.ButtonSelectable(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F34FE06 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862))))
							{
								PluginConfig.Instance().ModuleCustoms.Add(newFavoriteInput, new HashSet<string>());
								newFavoriteInput = string.Empty;
								ManagerBase<ModuleManager>.Instance().InvokeOnModuleChanged();
								ImGui.CloseCurrentPopup();
							}
							if (ImGuiOm.ButtonSelectable(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x906A2A8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3))))
							{
								try
								{
									List<string> list = ImGui.GetClipboardText().FromJSONBase64<List<string>>();
									if (list != null)
									{
										OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF06160 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d)));
									}
									PluginConfig.Instance().ModuleCustoms.Add(newFavoriteInput, list.ToHashSet());
									newFavoriteInput = string.Empty;
									ManagerBase<ModuleManager>.Instance().InvokeOnModuleChanged();
									ImGui.CloseCurrentPopup();
								}
								catch (Exception ex)
								{
									DLog.Error(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1DF40 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83)), ex);
									OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7DED8C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571)));
								}
							}
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val2)).Dispose();
				}
				TabItemDisposable val4 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F7157F5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0))), (ImGuiTabItemFlags)96);
				try
				{
					if (TabItemDisposable.op_Implicit(val4))
					{
						P5KKnMnhkq(favorite);
					}
				}
				finally
				{
					((TabItemDisposable)(ref val4)).Dispose();
				}
				foreach (KeyValuePair<string, List<ModuleBase>> item in custom.ToList())
				{
					TabItemDisposable val5 = ImRaii.TabItem(ImU8String.op_Implicit(item.Key));
					try
					{
						if (TabItemDisposable.op_Implicit(val5))
						{
							if (ImGui.IsItemHovered() && ImGui.IsMouseClicked((ImGuiMouseButton)1))
							{
								ImU8String val6 = new ImU8String(5, 1);
								((ImU8String)(ref val6)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF061F2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d));
								((ImU8String)(ref val6)).AppendFormatted<string>(item.Key);
								ImGui.OpenPopup(val6, (ImGuiPopupFlags)0);
							}
							cPaOYp2eWT(item);
							P5KKnMnhkq(item.Value);
						}
					}
					finally
					{
						((TabItemDisposable)(ref val5)).Dispose();
					}
					if (ImGui.IsItemClicked((ImGuiMouseButton)1))
					{
						ImU8String val7 = new ImU8String(5, 1);
						((ImU8String)(ref val7)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFBBCD1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
						((ImU8String)(ref val7)).AppendFormatted<string>(item.Key);
						ImGui.OpenPopup(val7, (ImGuiPopupFlags)0);
					}
					cPaOYp2eWT(item);
				}
			}
			finally
			{
				((TabBarDisposable)(ref val)).Dispose();
			}
		}

		private void q2YOXn2n97(IReadOnlyList<ModuleBase> modules)
		{
			favorite = modules.Where((ModuleBase x) => PluginConfig.Instance().ModuleFavorites.Contains(x.ModuleName)).ToList();
			custom.Clear();
			using Dictionary<string, HashSet<string>>.Enumerator enumerator = PluginConfig.Instance().ModuleCustoms.GetEnumerator();
			while (enumerator.MoveNext())
			{
				_003C_003Ec__DisplayClass8_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass8_0();
				CS_0024_003C_003E8__locals4.customPair = enumerator.Current;
				custom.TryAdd(CS_0024_003C_003E8__locals4.customPair.Key, new List<ModuleBase>());
				custom[CS_0024_003C_003E8__locals4.customPair.Key].AddRange(modules.Where((ModuleBase x) => CS_0024_003C_003E8__locals4.customPair.Value.Contains(x.ModuleName)));
			}
		}

		public override void Dispose()
		{
			ManagerBase<ModuleManager>.Instance().OnModuleChanged -= q2YOXn2n97;
		}

		[CompilerGenerated]
		internal static void cPaOYp2eWT(KeyValuePair<string, List<ModuleBase>> moduleCustom)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(5, 1);
			((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D3E13 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc));
			((ImU8String)(ref val)).AppendFormatted<string>(moduleCustom.Key);
			PopupDisposable val2 = ImRaii.Popup(val);
			try
			{
				if (!val2)
				{
					return;
				}
				if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E386A7D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997))), false, true))
				{
					PluginConfig.Instance().ModuleCustoms.Remove(moduleCustom.Key);
					PluginConfig.Instance().Save();
					ManagerBase<ModuleManager>.Instance().InvokeOnModuleChanged();
				}
				if (ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D157B19 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275))), false, true))
				{
					ImGui.SetClipboardText(ImU8String.op_Implicit(PluginConfig.Instance().ModuleCustoms[moduleCustom.Key].ToList().ToJSONBase64()));
					OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F715751 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A6B8B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1) + moduleCustom.Key);
				}
				if (ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F716905 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0))), false, (ImGuiSelectableFlags)1, default(Vector2)))
				{
					moduleCustom.Value.ForEach(delegate(ModuleBase x)
					{
						x.Load(affectConfig: true);
					});
					ImGui.CloseCurrentPopup();
				}
				if (ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x55839D6C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d))), false, (ImGuiSelectableFlags)1, default(Vector2)))
				{
					moduleCustom.Value.ForEach(delegate(ModuleBase x)
					{
						x.Unload(affectConfig: true);
					});
					ImGui.CloseCurrentPopup();
				}
			}
			finally
			{
				((PopupDisposable)(ref val2)).Dispose();
			}
		}
	}

	internal abstract class swio5KOzdbVemDXeuDN : IDisposable
	{
		protected readonly HfWYPNVIio8M9ncZnm0 Window;

		public abstract TabPageID PageID { get; }

		public virtual bool IsHiddenInSidebar => false;

		protected TabPageID SelectedTab
		{
			get
			{
				return Window.SelectedTab;
			}
			set
			{
				Window.SelectedTab = value;
			}
		}

		protected swio5KOzdbVemDXeuDN(HfWYPNVIio8M9ncZnm0 window)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			Window = window;
			base._002Ector();
		}

		public abstract void Dispose();

		public virtual void hiNTxC7Py0()
		{
			Window.sv49SfOr7P<OMQ0W4KSvbgxtAOQfUK>().iuIK9HKYfw();
		}

		public abstract void Draw();

		protected void P5KKnMnhkq(IReadOnlyList<ModuleBase> modules)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			fO1KycVpPH();
			ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x55839E04 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d)), ImGui.GetContentRegionAvail(), false, (ImGuiWindowFlags)128);
			try
			{
				if (!val)
				{
					return;
				}
				if (Window.isNeedToScrollToTop)
				{
					ImGui.SetScrollY(0f);
					Window.isNeedToScrollToTop = false;
				}
				using (OmenServiceBase<FontManager>.Instance().GetUIFont(0.9f).Push())
				{
					ModuleCardComponent.Prune(ManagerBase<ModuleManager>.Instance().Modules);
					foreach (ModuleBase module in modules)
					{
						if (module != null)
						{
							BrAKLhpN0b(module);
						}
					}
				}
			}
			finally
			{
				((ChildDisposable)(ref val)).Dispose();
			}
		}

		private static void fO1KycVpPH()
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			bool value = PluginConfig.Instance().CompactMode;
			TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D3FC7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc)), 2, (ImGuiTableFlags)12591104);
			try
			{
				if (!(!val))
				{
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4A0A4E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32)), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBA16D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2)), (ImGuiTableColumnFlags)8, 0f, 0u);
					ImGui.TableNextRow();
					ImGui.TableSetColumnIndex(1);
					uint colorU = ImGui.GetColorU32((ImGuiCol)7);
					if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -1830596660 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3), ref value, ImGui.GetFrameHeight() / 2f, colorU, colorU))
					{
						PluginConfig.Instance().CompactMode = value;
						PluginConfig.Instance().Save();
					}
					ImGuiOm.TooltipHover(LanguageManager.Get((!value) ? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEDBC7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b) : GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x411920A3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52)));
				}
			}
			finally
			{
				((TableDisposable)(ref val)).Dispose();
			}
		}

		private static void BrAKLhpN0b(ModuleBase? module)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (module == null)
			{
				return;
			}
			IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(module.ToString()), true);
			try
			{
				GroupDisposable val2 = ImRaii.Group();
				try
				{
					DisabledDisposable val3 = ImRaii.Disabled(module.Permission.NeedAuth && !AuthState.IsAuth);
					try
					{
						ModuleCardComponent.Get(module).Draw();
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					ImGuiOm.ScaledDummy(8f);
				}
				finally
				{
					((GroupDisposable)(ref val2)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		protected static bool zoWKHnZvLt(string text, bool condition)
		{
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X - 30f * Globals.GlobalUIScale, ImGui.GetTextLineHeightWithSpacing());
			bool flag = false;
			ColorDisposable val = ImRaii.PushColor((ImGuiCol)21, ImGui.GetColorU32((ImGuiCol)2), !condition);
			try
			{
				ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 10f * Globals.GlobalUIScale);
				return ImGui.Button(ImU8String.op_Implicit(text), vector);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	internal enum TabPageID
	{
		主页,
		设置,
		搜索,
		收藏,
		启用,
		验证,
		模块
	}

	internal class OMQ0W4KSvbgxtAOQfUK : swio5KOzdbVemDXeuDN
	{
		private ModuleSearcher searcher;

		public string SearchInputBuffer { get; set; }

		public override TabPageID PageID => TabPageID.搜索;

		public override bool IsHiddenInSidebar => true;

		public OMQ0W4KSvbgxtAOQfUK(HfWYPNVIio8M9ncZnm0 window)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			SearchInputBuffer = string.Empty;
			searcher = x9sKf7OKXe(Array.Empty<ModuleBase>());
			base._002Ector(window);
			searcher = x9sKf7OKXe(ManagerBase<ModuleManager>.Instance().Modules);
			ManagerBase<ModuleManager>.Instance().OnModuleChanged += eDfKCHkugm;
		}

		public override void hiNTxC7Py0()
		{
		}

		public override void Draw()
		{
			P5KKnMnhkq(searcher.Search(SearchInputBuffer));
		}

		internal void L59KV4Y5ei(string text)
		{
			SearchInputBuffer = (string.IsNullOrWhiteSpace(text) ? string.Empty : text);
		}

		internal void iuIK9HKYfw()
		{
			SearchInputBuffer = string.Empty;
		}

		private static ModuleSearcher x9sKf7OKXe(IEnumerable<ModuleBase> modules)
		{
			return new ModuleSearcher(modules);
		}

		private void eDfKCHkugm(IReadOnlyList<ModuleBase> modules)
		{
			searcher = x9sKf7OKXe(modules);
		}

		public override void Dispose()
		{
			ManagerBase<ModuleManager>.Instance().OnModuleChanged -= eDfKCHkugm;
		}
	}

	internal class sBs7DvKjKAhrVE3ke5W : swio5KOzdbVemDXeuDN
	{
		private sealed class qDk5NYWjnU7fy2POekt : CardComponentBase
		{
			private string title;

			private Action content;

			public void FEXWOgIwc1(string titleParam, Action contentParam)
			{
				title = titleParam;
				content = contentParam;
				Draw();
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
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				if (!string.IsNullOrEmpty(title))
				{
					ImGuiOm.ScaledDummy(0f, 4f * Globals.GlobalUIScale);
					using (OmenServiceBase<FontManager>.Instance().UIFont140.Push())
					{
						Vector4 vector = KnownColor.Chocolate.ToVector4();
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(title));
					}
					ImGuiOm.ScaledDummy(0f, 8f * Globals.GlobalUIScale);
					ImGui.Spacing();
				}
				content();
			}

			protected override Vector2 GetTargetSize(CardDrawContext context, Vector2 contentRectSize)
			{
				Vector2 result = contentRectSize;
				result.X = context.FrameWidth;
				return result;
			}

			public qDk5NYWjnU7fy2POekt()
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				title = string.Empty;
				content = delegate
				{
				};
				base._002Ector();
			}
		}

		private class C31hNPWKPCVRnNb4dq5
		{
			public float Scale;

			public float Velocity;

			public C31hNPWKPCVRnNb4dq5()
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				Scale = 1f;
				base._002Ector();
			}
		}

		private class J9qG02WtyTlDkRr0bob
		{
			public float Scale;

			public float Velocity;

			public J9qG02WtyTlDkRr0bob()
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				Scale = 1f;
				base._002Ector();
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass30_0
		{
			public string label;

			public Action onClick;

			public FontAwesomeIcon? icon;

			public Action tooltip;

			public Action control;

			public string help;

			public Action postDraw;

			public _003C_003Ec__DisplayClass30_0()
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				base._002Ector();
			}

			internal void MIEWWGPVDd()
			{
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_018d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0197: Unknown result type (might be due to invalid IL or missing references)
				//IL_0235: Unknown result type (might be due to invalid IL or missing references)
				//IL_023a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0275: Unknown result type (might be due to invalid IL or missing references)
				//IL_02be: Unknown result type (might be due to invalid IL or missing references)
				float num = ImGui.GetFrameHeight() + 8f * Globals.GlobalUIScale;
				float num2 = (num - ImGui.GetFrameHeight()) / 2f;
				ImU8String val = default(ImU8String);
				((ImU8String)(ref val))._002Ector(4, 1);
				((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D151CA1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
				((ImU8String)(ref val)).AppendFormatted<string>(label);
				TableDisposable val2 = ImRaii.Table(val, 2);
				try
				{
					if (!val2)
					{
						return;
					}
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C2A19D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0)), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFBD84 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2)), (ImGuiTableColumnFlags)8, 0f, 0u);
					ImGui.TableNextRow((ImGuiTableRowFlags)0, num);
					ImGui.TableNextColumn();
					if (onClick != null)
					{
						Vector2 cursorPos = ImGui.GetCursorPos();
						ImU8String val3 = new ImU8String(6, 1);
						((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D25114 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
						((ImU8String)(ref val3)).AppendFormatted<string>(label);
						if (ImGui.Selectable(val3, false, (ImGuiSelectableFlags)18, new Vector2(0f, num)))
						{
							onClick();
						}
						ImGui.SetCursorPos(cursorPos);
					}
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() + num2);
					ImGui.AlignTextToFramePadding();
					if (icon.HasValue)
					{
						ImGui.TextUnformatted(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(icon.Value)));
						ImGui.SameLine(0f, 6f * Globals.GlobalUIScale);
					}
					ImGui.AlignTextToFramePadding();
					ImGui.TextUnformatted(ImU8String.op_Implicit(label));
					if (tooltip != null)
					{
						ImGui.SameLine(0f, 4f * Globals.GlobalUIScale);
						tooltip();
					}
					ImGui.TableNextColumn();
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() + num2);
					if (control != null)
					{
						control();
					}
					else if (onClick != null)
					{
						ImGui.AlignTextToFramePadding();
						ImU8String val4 = new ImU8String(2, 1);
						((ImU8String)(ref val4)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61524));
						((ImU8String)(ref val4)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F347A78 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
						ImGui.TextUnformatted(val4);
					}
				}
				finally
				{
					((TableDisposable)(ref val2)).Dispose();
				}
				if (help != null)
				{
					ImGuiOm.ScaledDummy(0f, 2f * Globals.GlobalUIScale);
					ColorDisposable val5 = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), true);
					try
					{
						ImGui.TextWrapped(ImU8String.op_Implicit(help));
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
				}
				postDraw?.Invoke();
			}
		}

		private string conflictKeySearchString;

		private string fontSearchString;

		private readonly Dictionary<uint, qDk5NYWjnU7fy2POekt> cardStates;

		private readonly Dictionary<string, C31hNPWKPCVRnNb4dq5> linkStates;

		private readonly Dictionary<string, J9qG02WtyTlDkRr0bob> authorStates;

		private List<string> allAuthors;

		private static readonly List<string> AllModerators;

		private static readonly List<string> AllCredits;

		public static float ControlWidth => 200f * Globals.GlobalUIScale;

		public override TabPageID PageID => TabPageID.设置;

		public override bool IsHiddenInSidebar => true;

		public float InputBoxHeight { get; internal set; }

		public Vector2 ButtonSize { get; internal set; }

		public sBs7DvKjKAhrVE3ke5W(HfWYPNVIio8M9ncZnm0 window)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			InputBoxHeight = 36f;
			ButtonSize = new Vector2(36f);
			conflictKeySearchString = string.Empty;
			fontSearchString = string.Empty;
			cardStates = new Dictionary<uint, qDk5NYWjnU7fy2POekt>();
			linkStates = new Dictionary<string, C31hNPWKPCVRnNb4dq5>();
			authorStates = new Dictionary<string, J9qG02WtyTlDkRr0bob>();
			allAuthors = new List<string>();
			base._002Ector(window);
			ManagerBase<ModuleManager>.Instance().OnModuleChanged += NIGKOM3onE;
		}

		private void NIGKOM3onE(IReadOnlyList<ModuleBase> modules)
		{
			allAuthors = (from x in (from x in modules.Select((ModuleBase x) => x.Info.Author).SelectMany((string[] x) => x)
					group x by x into x
					select new
					{
						Name = x.Key,
						Count = x.Count()
					}).DistinctBy(x => x.Name)
				orderby x.Count descending, x.Name
				select x.Name).ToList();
		}

		public override void hiNTxC7Py0()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)11, 18f, true);
			try
			{
				ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)21, ImGui.GetColorU32((ImGuiCol)2), true);
				try
				{
					ImU8String val3 = new ImU8String(11, 1);
					((ImU8String)(ref val3)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61459));
					((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFBB213 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
					if (ImGui.Button(val3, new Vector2(InputBoxHeight)))
					{
						base.SelectedTab = TabPageID.设置;
						base.hiNTxC7Py0();
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
			ImGuiOm.TooltipHover(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C6DF2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac)));
			ButtonSize = ImGui.GetItemRectSize();
		}

		public override void Draw()
		{
			//Discarded unreachable code: IL_01e8
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-864210836 ^ -1341039202 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac)));
			try
			{
				if (!val)
				{
					return;
				}
				TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816965A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd))));
				try
				{
					if (TabItemDisposable.op_Implicit(val2))
					{
						J6aKKKgPar();
					}
				}
				finally
				{
					((TabItemDisposable)(ref val2)).Dispose();
				}
				TabItemDisposable val3 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC639B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1))));
				try
				{
					if (TabItemDisposable.op_Implicit(val3))
					{
						Nx0KtHauDb();
					}
				}
				finally
				{
					((TabItemDisposable)(ref val3)).Dispose();
				}
				TabItemDisposable val4 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47334385 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5))));
				try
				{
					if (TabItemDisposable.op_Implicit(val4))
					{
						PMZKWEU1IR();
					}
				}
				finally
				{
					((TabItemDisposable)(ref val4)).Dispose();
				}
				TabItemDisposable val5 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A06F8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a))));
				try
				{
					if (TabItemDisposable.op_Implicit(val5))
					{
						TYUKTUOdP7();
					}
				}
				finally
				{
					((TabItemDisposable)(ref val5)).Dispose();
				}
				TabItemDisposable val6 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E4625FD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e))));
				try
				{
					if (TabItemDisposable.op_Implicit(val6))
					{
						B8oK1omiD0();
					}
				}
				finally
				{
					((TabItemDisposable)(ref val6)).Dispose();
				}
			}
			finally
			{
				((TabBarDisposable)(ref val)).Dispose();
			}
		}

		private unsafe void J6aKKKgPar()
		{
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A5B17 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1)), delegate
			{
				R2BKlnA1JZ(ControlWidth, delegate
				{
					ManagerBase<ModuleManager>.Instance().ReloadAllAsync(CancellationToken.None);
				});
			}, delegate
			{
			}, (FontAwesomeIcon)61612);
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BAAF6F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503)), delegate
			{
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				//IL_0073: Unknown result type (might be due to invalid IL or missing references)
				string valueOrDefault = TabPagesLoc.GetValueOrDefault((TabPageID)PluginConfig.Instance().DefaultHomePage, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E386387 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
				ImGui.SetNextItemWidth(ControlWidth);
				ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F35F944 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862)), ImU8String.op_Implicit(valueOrDefault));
				try
				{
					if (ComboDisposable.op_Implicit(val))
					{
						foreach (KeyValuePair<TabPageID, string> item in TabPagesLoc)
						{
							if (ImGuiOm.Selectable(item.Value, selected: false, (ImGuiSelectableFlags)0))
							{
								PluginConfig.Instance().DefaultHomePage = (int)item.Key;
								PluginConfig.Instance().Save();
							}
						}
						return;
					}
				}
				finally
				{
					((ComboDisposable)(ref val)).Dispose();
				}
			}, null, (FontAwesomeIcon)61461, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642A899A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df)));
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E25618B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25)), delegate
			{
				bool value = PluginConfig.Instance().SendOnlineAuthInfo;
				if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A739A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a), ref value))
				{
					PluginConfig.Instance().SendOnlineAuthInfo = value;
					PluginConfig.Instance().Save();
				}
			}, null, (FontAwesomeIcon)62637, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFBB395 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc)));
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFAA8E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2)), delegate
			{
				bool value = PluginConfig.Instance().ShowTrayOverlay;
				if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBDB57 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2), ref value))
				{
					PluginConfig.Instance().ShowTrayOverlay = value;
					PluginConfig.Instance().Save();
				}
			}, delegate
			{
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_009f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0115: Unknown result type (might be due to invalid IL or missing references)
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0194: Unknown result type (might be due to invalid IL or missing references)
				if (!PluginConfig.Instance().ShowTrayOverlay)
				{
					return;
				}
				StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)11, 18f, true);
				try
				{
					ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)21, Vector4.Zero, true);
					try
					{
						ImU8String val3 = new ImU8String(13, 1);
						((ImU8String)(ref val3)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61459));
						((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53682260 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94));
						if (ImGui.Button(val3, default(Vector2)))
						{
							ImGui.OpenPopup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C0192 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)), (ImGuiPopupFlags)0);
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
				PopupDisposable val4 = ImRaii.Popup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F711127 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0)));
				try
				{
					if (PopupDisposable.op_Implicit(val4))
					{
						float trayOverlaySize = PluginConfig.Instance().TrayOverlaySize;
						ImGui.SetNextItemWidth(ControlWidth);
						ImU8String val5 = new ImU8String(25, 1);
						((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A2E67 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1)));
						((ImU8String)(ref val5)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x906D902 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3));
						if (ImGui.InputFloat(val5, ref trayOverlaySize, 0f, 0f, ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFB7729 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc)), (ImGuiInputTextFlags)0))
						{
							PluginConfig.Instance().TrayOverlaySize = MathF.Max(1f, trayOverlaySize);
						}
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							PluginConfig.Instance().Save();
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val4)).Dispose();
				}
			}, (FontAwesomeIcon)62134, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x473341D3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5)));
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339C87B2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642A8B3C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBAC5F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE4F97 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76), delegate
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_0120: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0156: Unknown result type (might be due to invalid IL or missing references)
				//IL_0137: Unknown result type (might be due to invalid IL or missing references)
				//IL_017b: Unknown result type (might be due to invalid IL or missing references)
				ImGui.SetNextItemWidth(ControlWidth);
				ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x513358DA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b)), ImU8String.op_Implicit(((object)PluginConfig.Instance().ConflictKeyBinding.Keyboard/*cast due to constrained. prefix*/).ToString()), (ImGuiComboFlags)8);
				try
				{
					if (!val)
					{
						return;
					}
					ImGui.SetNextItemWidth(-1f);
					ImU8String val2 = ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC1A87 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1));
					ImU8String val3 = new ImU8String(3, 1);
					((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA78C21 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff)));
					((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x39221E7C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
					ImGui.InputTextWithHint(val2, val3, ref conflictKeySearchString, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					ImGui.Separator();
					ImGui.Spacing();
					foreach (VirtualKey validVirtualKey in DalamudServiceExtension.Instance<IKeyState>().GetValidVirtualKeys())
					{
						if ((string.IsNullOrWhiteSpace(conflictKeySearchString) || VirtualKeyExtensions.GetFancyName(validVirtualKey).Contains(conflictKeySearchString, StringComparison.OrdinalIgnoreCase)) && ImGui.Selectable(ImU8String.op_Implicit(VirtualKeyExtensions.GetFancyName(validVirtualKey)), false, (ImGuiSelectableFlags)0, default(Vector2)))
						{
							PluginConfig.Instance().ConflictKeyBinding.Keyboard = validVirtualKey;
							PluginConfig.Instance().Save();
						}
					}
				}
				finally
				{
					((ComboDisposable)(ref val)).Dispose();
				}
			}, null, (FontAwesomeIcon)61724, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7BA2D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff)));
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F715737 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B48088 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47334107 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC61D9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1), delegate
			{
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0095: Unknown result type (might be due to invalid IL or missing references)
				//IL_009c: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_0125: Unknown result type (might be due to invalid IL or missing references)
				//IL_0155: Unknown result type (might be due to invalid IL or missing references)
				//IL_0166: Unknown result type (might be due to invalid IL or missing references)
				//IL_016b: Unknown result type (might be due to invalid IL or missing references)
				//IL_017a: Unknown result type (might be due to invalid IL or missing references)
				//IL_017f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_0183: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
				//IL_01da: Unknown result type (might be due to invalid IL or missing references)
				ImGui.SetNextItemWidth(ControlWidth);
				ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BAD68D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503)), ImU8String.op_Implicit(((object)PluginConfig.Instance().ConflictKeyBinding.Gamepad/*cast due to constrained. prefix*/).ToString()), (ImGuiComboFlags)8);
				try
				{
					if (!(!val))
					{
						ImGui.SetNextItemWidth(-1f);
						ImU8String val2 = ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3701F34 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e));
						ImU8String val3 = new ImU8String(3, 1);
						((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C4042 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)));
						((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816A6D8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd));
						ImGui.InputTextWithHint(val2, val3, ref conflictKeySearchString, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						ImGui.Separator();
						ImGui.Spacing();
						GamepadButtons[] values = Enum.GetValues<GamepadButtons>();
						for (int i = 0; i < values.Length; i++)
						{
							GamepadButtons val4 = values[i];
							if ((string.IsNullOrWhiteSpace(conflictKeySearchString) || ((object)(*(GamepadButtons*)(&val4))/*cast due to constrained. prefix*/).ToString().Contains(conflictKeySearchString, StringComparison.OrdinalIgnoreCase)) && (int)val4 != 0)
							{
								GamepadButtons gamepad = PluginConfig.Instance().ConflictKeyBinding.Gamepad;
								if (ImGui.Selectable(ImU8String.op_Implicit(((object)(*(GamepadButtons*)(&val4))/*cast due to constrained. prefix*/).ToString()), gamepad.IsSetAny<GamepadButtons>(new ReadOnlySpan<GamepadButtons>((GamepadButtons)val4)), (ImGuiSelectableFlags)0, default(Vector2)))
								{
									gamepad = (GamepadButtons)((!gamepad.IsSetAny<GamepadButtons>(new ReadOnlySpan<GamepadButtons>((GamepadButtons)val4))) ? (gamepad | val4) : (gamepad & (ushort)(~val4)));
									PluginConfig.Instance().ConflictKeyBinding.Gamepad = gamepad;
									PluginConfig.Instance().Save();
								}
							}
						}
					}
				}
				finally
				{
					((ComboDisposable)(ref val)).Dispose();
				}
			}, null, (FontAwesomeIcon)61723, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2266C95 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44)));
			NV7KM5IXqi(string.Empty, delegate
			{
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, KnownColor.Orange.ToUInt(), true);
				try
				{
					ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C790428 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00))));
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F149A1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
		}

		private void Nx0KtHauDb()
		{
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF06DD0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d)), delegate
			{
				bool value = OmenServiceBase<MovementManager>.Instance().AlwaysMinGilTransport;
				if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200FEDF9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af), ref value))
				{
					OmenServiceBase<MovementManager>.Instance().AlwaysMinGilTransport = value;
					OmenServiceBase<MovementManager>.Instance().SaveConfig();
				}
			}, null, (FontAwesomeIcon)61554, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A0594 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a)));
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F716709 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0)), delegate
			{
				bool value = PluginConfig.Instance().StripeDRPrefix;
				if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD0A45 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), ref value))
				{
					PluginConfig.Instance().StripeDRPrefix = value;
					PluginConfig.Instance().Save();
				}
			}, null, (FontAwesomeIcon)61693, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3706568 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e)));
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E3860A1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997)), delegate
			{
				bool value = PluginConfig.Instance().CorrectEnvironment;
				if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -1056230437 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de), ref value))
				{
					PluginConfig.Instance().CorrectEnvironment = value;
					PluginConfig.Instance().Save();
				}
			}, null, (FontAwesomeIcon)61528, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D23C30 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1)));
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B211DDA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a)), delegate
			{
				bool value = PluginConfig.Instance().SkipLogo;
				if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBD8EB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2), ref value))
				{
					PluginConfig.Instance().SkipLogo = value;
					PluginConfig.Instance().Save();
				}
			}, null, (FontAwesomeIcon)61518, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC6761 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1)));
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F716187 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0)), delegate
			{
				bool value = PluginConfig.Instance().PreventAFK;
				if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A2D5D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1), ref value))
				{
					PluginConfig.Instance().PreventAFK = value;
					PluginConfig.Instance().Save();
				}
			}, null, (FontAwesomeIcon)62093, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A23C54 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079)));
		}

		private void PMZKWEU1IR()
		{
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x370632A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e)), delegate
			{
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_0063: Unknown result type (might be due to invalid IL or missing references)
				float num = PluginConfig.Instance().LeftTabWidth;
				ImGui.SetNextItemWidth(ControlWidth);
				if (ImGui.InputFloat(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x46290E33 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7)), ref num, 0f, 0f, ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F71A1B1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0)), (ImGuiInputTextFlags)0))
				{
					num = Math.Clamp(num, 100f, ImGui.GetWindowWidth() - 50f);
				}
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					PluginConfig.Instance().LeftTabWidth = num;
					PluginConfig.Instance().Save();
				}
			}, null, (FontAwesomeIcon)61493);
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1976305562 ^ 0x58F8096C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33)), delegate
			{
				lpIKp1xWfw(ControlWidth);
			}, null, (FontAwesomeIcon)61489);
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD7971 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0)), delegate
			{
				CPiKxGRoiq(ControlWidth, ref fontSearchString, 300f * Globals.GlobalUIScale, isNoBackgroundAndScrollbar: true);
			}, delegate
			{
				//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
				if (OmenServiceBase<FontManager>.Instance().IsFontBuilding)
				{
					ImGui.SameLine();
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImU8String val = default(ImU8String);
					((ImU8String)(ref val))._002Ector(2, 1);
					((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x65413E80 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb));
					((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B300B74 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117)));
					((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FF91A6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
					ImGui.TextColored(ref vector, val);
					return;
				}
				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + Globals.GlobalUIScale);
				StyleDisposable val2 = ImRaii.PushStyle((ImGuiStyleVar)11, 18f, true);
				try
				{
					ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)21, Vector4.Zero, true);
					try
					{
						if (ImGuiOm.ButtonIcon(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5583E748 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d), (FontAwesomeIcon)61741, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A24EB0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079))))
						{
							OmenServiceBase<FontManager>.Instance().Config.FontFileName = string.Empty;
							OmenServiceBase<FontManager>.Instance().Config.Save();
							OmenServiceBase<FontManager>.Instance().RebuildUIFontsAsync();
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
			}, (FontAwesomeIcon)61491);
		}

		private void TYUKTUOdP7()
		{
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x55838766 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d)), delegate
			{
				bool value = PluginConfig.Instance().DefaultEnableModules;
				if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE25FD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76), ref value))
				{
					PluginConfig.Instance().DefaultEnableModules = value;
					PluginConfig.Instance().Save();
				}
			}, null, (FontAwesomeIcon)61726, null, null, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B2AB5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30)));
			KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768FE9E7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46)), null, delegate
			{
				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + Globals.GlobalUIScale);
				StyleDisposable val2 = ImRaii.PushStyle((ImGuiStyleVar)11, 18f, true);
				try
				{
					ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)21, Vector4.Zero, true);
					try
					{
						ImGuiOm.ButtonIcon(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B3014BC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117), (FontAwesomeIcon)61637, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA592B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1)));
						ImGuiOmExtension.ClickToCopyAndNotify(DalamudServiceExtension.Instance().ConfigDirectory.ToString());
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
				ImGuiOm.TooltipHover(DalamudServiceExtension.Instance().ConfigDirectory.ToString());
			}, (FontAwesomeIcon)61564, delegate
			{
				Runtime.OpenConfigDirectory(DalamudServiceExtension.Instance().ConfigDirectory.FullName);
			});
			DisabledDisposable val = ImRaii.Disabled(ManagerBase<ModuleManager>.Instance().IsLoadingAllModules);
			try
			{
				KxBK4gJekg(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642A8F54 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df)), null, null, (FontAwesomeIcon)61473, delegate
				{
					if (!ManagerBase<ModuleManager>.Instance().IsLoadingAllModules)
					{
						ManagerBase<ModuleManager>.Instance().ReloadAllAsync(CancellationToken.None);
					}
				});
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		private void B8oK1omiD0()
		{
			NV7KM5IXqi(string.Empty, delegate
			{
				//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00de: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				//IL_010f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0114: Unknown result type (might be due to invalid IL or missing references)
				//IL_0154: Unknown result type (might be due to invalid IL or missing references)
				if (ImGuiOm.ButtonSelectable(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61543) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x415A8431 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E250863 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25))))
				{
					PluginConfig.Instance().ModuleLocal.Add((string.Empty, true));
					PluginConfig.Instance().Save();
				}
				ImGui.Spacing();
				for (int i = 0; i < PluginConfig.Instance().ModuleLocal.Count; i++)
				{
					(string, bool) tuple = PluginConfig.Instance().ModuleLocal[i];
					IdDisposable val = ImRaii.PushId(i, true);
					try
					{
						ImGui.AlignTextToFramePadding();
						ImU8String val2 = new ImU8String(1, 1);
						((ImU8String)(ref val2)).AppendFormatted<int>(i + 1);
						((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FE2170 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
						ImGui.TextDisabled(val2);
						ImGui.SameLine();
						float num = ImGui.GetFrameHeight() * 2f;
						ImGuiStylePtr style = ImGui.GetStyle();
						float num2 = num + ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
						float frameHeight = ImGui.GetFrameHeight();
						style = ImGui.GetStyle();
						float num3 = frameHeight + ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
						ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X - num2 - num3);
						if (ImGui.InputText(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9A191 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a)), ref tuple.Item1, 1024, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
						{
							PluginConfig.Instance().ModuleLocal[i] = tuple;
						}
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							PluginConfig.Instance().Save();
						}
						string text;
						if (!string.IsNullOrWhiteSpace(tuple.Item1))
						{
							(text, _) = tuple;
						}
						else
						{
							text = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5133489C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
						}
						ImGuiOm.TooltipHover(text);
						ImGui.SameLine();
						if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D582D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc), ref tuple.Item2))
						{
							PluginConfig.Instance().ModuleLocal[i] = tuple;
							PluginConfig.Instance().Save();
						}
						ImGui.SameLine();
						if (ImGuiOm.ButtonIcon(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFBDB15 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc), (FontAwesomeIcon)62189))
						{
							PluginConfig.Instance().ModuleLocal.RemoveAt(i);
							PluginConfig.Instance().Save();
							i--;
						}
					}
					finally
					{
						((IDisposable)val)?.Dispose();
					}
				}
			}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFAB3E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
		}

		private void NV7KM5IXqi(string title, Action content, string? stateID = null)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			uint iD = ImGui.GetID(ImU8String.op_Implicit(stateID ?? title));
			if (!cardStates.TryGetValue(iD, out qDk5NYWjnU7fy2POekt value))
			{
				value = new qDk5NYWjnU7fy2POekt();
				cardStates[iD] = value;
			}
			value.FEXWOgIwc1(title, content);
			ImGuiOm.ScaledDummy(8f);
		}

		private void KxBK4gJekg(string label, Action? control = null, Action? tooltip = null, FontAwesomeIcon? icon = null, Action? onClick = null, Action? postDraw = null, string? help = null)
		{
			_003C_003Ec__DisplayClass30_0 CS_0024_003C_003E8__locals23 = new _003C_003Ec__DisplayClass30_0();
			CS_0024_003C_003E8__locals23.label = label;
			CS_0024_003C_003E8__locals23.onClick = onClick;
			CS_0024_003C_003E8__locals23.icon = icon;
			CS_0024_003C_003E8__locals23.tooltip = tooltip;
			CS_0024_003C_003E8__locals23.control = control;
			CS_0024_003C_003E8__locals23.help = help;
			CS_0024_003C_003E8__locals23.postDraw = postDraw;
			NV7KM5IXqi(string.Empty, delegate
			{
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				//IL_006d: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_0100: Unknown result type (might be due to invalid IL or missing references)
				//IL_0105: Unknown result type (might be due to invalid IL or missing references)
				//IL_013c: Unknown result type (might be due to invalid IL or missing references)
				//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
				//IL_018d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0197: Unknown result type (might be due to invalid IL or missing references)
				//IL_0235: Unknown result type (might be due to invalid IL or missing references)
				//IL_023a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0275: Unknown result type (might be due to invalid IL or missing references)
				//IL_02be: Unknown result type (might be due to invalid IL or missing references)
				float num = ImGui.GetFrameHeight() + 8f * Globals.GlobalUIScale;
				float num2 = (num - ImGui.GetFrameHeight()) / 2f;
				ImU8String val = default(ImU8String);
				((ImU8String)(ref val))._002Ector(4, 1);
				((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D151CA1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
				((ImU8String)(ref val)).AppendFormatted<string>(CS_0024_003C_003E8__locals23.label);
				TableDisposable val2 = ImRaii.Table(val, 2);
				try
				{
					if (!val2)
					{
						return;
					}
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C2A19D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0)), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFBD84 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2)), (ImGuiTableColumnFlags)8, 0f, 0u);
					ImGui.TableNextRow((ImGuiTableRowFlags)0, num);
					ImGui.TableNextColumn();
					if (CS_0024_003C_003E8__locals23.onClick != null)
					{
						Vector2 cursorPos = ImGui.GetCursorPos();
						ImU8String val3 = new ImU8String(6, 1);
						((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D25114 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
						((ImU8String)(ref val3)).AppendFormatted<string>(CS_0024_003C_003E8__locals23.label);
						if (ImGui.Selectable(val3, false, (ImGuiSelectableFlags)18, new Vector2(0f, num)))
						{
							CS_0024_003C_003E8__locals23.onClick();
						}
						ImGui.SetCursorPos(cursorPos);
					}
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() + num2);
					ImGui.AlignTextToFramePadding();
					if (CS_0024_003C_003E8__locals23.icon.HasValue)
					{
						ImGui.TextUnformatted(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(CS_0024_003C_003E8__locals23.icon.Value)));
						ImGui.SameLine(0f, 6f * Globals.GlobalUIScale);
					}
					ImGui.AlignTextToFramePadding();
					ImGui.TextUnformatted(ImU8String.op_Implicit(CS_0024_003C_003E8__locals23.label));
					if (CS_0024_003C_003E8__locals23.tooltip != null)
					{
						ImGui.SameLine(0f, 4f * Globals.GlobalUIScale);
						CS_0024_003C_003E8__locals23.tooltip();
					}
					ImGui.TableNextColumn();
					ImGui.SetCursorPosY(ImGui.GetCursorPosY() + num2);
					if (CS_0024_003C_003E8__locals23.control != null)
					{
						CS_0024_003C_003E8__locals23.control();
					}
					else if (CS_0024_003C_003E8__locals23.onClick != null)
					{
						ImGui.AlignTextToFramePadding();
						ImU8String val4 = new ImU8String(2, 1);
						((ImU8String)(ref val4)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61524));
						((ImU8String)(ref val4)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F347A78 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
						ImGui.TextUnformatted(val4);
					}
				}
				finally
				{
					((TableDisposable)(ref val2)).Dispose();
				}
				if (CS_0024_003C_003E8__locals23.help != null)
				{
					ImGuiOm.ScaledDummy(0f, 2f * Globals.GlobalUIScale);
					ColorDisposable val5 = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), true);
					try
					{
						ImGui.TextWrapped(ImU8String.op_Implicit(CS_0024_003C_003E8__locals23.help));
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
				}
				CS_0024_003C_003E8__locals23.postDraw?.Invoke();
			}, CS_0024_003C_003E8__locals23.label);
		}

		private void CXtKsxFvcq()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			ImGui.TextUnformatted(ImU8String.op_Implicit("ziz"));
			ImGui.TextUnformatted(ImU8String.op_Implicit("1.0"));
		}

		private void O9EK8bs56A(string label, string url, FontAwesomeIcon icon)
		{
		}

		private void zI7Kvcr1sv(string author, float width)
		{
		}

		public override void Dispose()
		{
			ManagerBase<ModuleManager>.Instance().OnModuleChanged -= NIGKOM3onE;
		}

		internal static void R2BKlnA1JZ(float width, Action? onLanguageChanged = null)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			ImGui.SetNextItemWidth(width);
			ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x392229D0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2)), ImU8String.op_Implicit(LanguageManager.LanguageInfo.GetValueOrDefault(PluginConfig.Instance().UILanguage, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C7318 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32))));
			try
			{
				if (!val)
				{
					return;
				}
				foreach (KeyValuePair<Language, string> availableLanguage in LanguageManager.AvailableLanguages)
				{
					ImU8String val2 = new ImU8String(3, 2);
					((ImU8String)(ref val2)).AppendFormatted<string>(availableLanguage.Value);
					((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C93E2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
					((ImU8String)(ref val2)).AppendFormatted<Language>(availableLanguage.Key);
					if (ImGui.Selectable(val2, PluginConfig.Instance().UILanguage == availableLanguage.Key, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						PluginConfig.Instance().UILanguage = availableLanguage.Key;
						PluginConfig.Instance().Save();
						LanguageManager.LoadLanguage(availableLanguage.Key);
						onLanguageChanged?.Invoke();
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val)).Dispose();
			}
		}

		internal static void CPiKxGRoiq(float width, ref string searchString, float childHeight = 300f, bool isNoBackgroundAndScrollbar = false)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			string valueOrDefault = OmenServiceBase<FontManager>.Instance().InstalledFonts.GetValueOrDefault(OmenServiceBase<FontManager>.Instance().Config.FontFileName, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBA901 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2)));
			ImGui.SetNextItemWidth(width);
			ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E940F6F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75)), ImU8String.op_Implicit(valueOrDefault), (ImGuiComboFlags)8);
			try
			{
				if (!val)
				{
					return;
				}
				using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
				{
					ImGui.InputTextWithHint(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD7B57 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0)), ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x41191B75 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52))), ref searchString, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					float x = ImGui.GetItemRectSize().X;
					ImGuiWindowFlags val2 = (ImGuiWindowFlags)(isNoBackgroundAndScrollbar ? 136 : 0);
					ChildDisposable val3 = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379B8A1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09)), new Vector2(x, childHeight), !isNoBackgroundAndScrollbar, val2);
					try
					{
						if (!ChildDisposable.op_Implicit(val3))
						{
							return;
						}
						foreach (KeyValuePair<string, string> installedFont in OmenServiceBase<FontManager>.Instance().InstalledFonts)
						{
							if (string.IsNullOrWhiteSpace(searchString) || installedFont.Key.Contains(searchString, StringComparison.OrdinalIgnoreCase) || installedFont.Value.Contains(searchString, StringComparison.OrdinalIgnoreCase))
							{
								bool flag = OmenServiceBase<FontManager>.Instance().Config.FontFileName == installedFont.Key;
								if (ImGui.IsWindowAppearing() && flag)
								{
									ImGui.SetScrollHereY();
								}
								ImU8String val4 = new ImU8String(2, 2);
								((ImU8String)(ref val4)).AppendFormatted<string>(installedFont.Value);
								((ImU8String)(ref val4)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702BEDF5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
								((ImU8String)(ref val4)).AppendFormatted<string>(installedFont.Key);
								if (ImGui.Selectable(val4, flag, (ImGuiSelectableFlags)0, default(Vector2)))
								{
									OmenServiceBase<FontManager>.Instance().Config.FontFileName = installedFont.Key;
									OmenServiceBase<FontManager>.Instance().Config.Save();
									OmenServiceBase<FontManager>.Instance().RebuildUIFontsAsync();
								}
								ImGuiOm.TooltipHover(installedFont.Key);
							}
						}
					}
					finally
					{
						((ChildDisposable)(ref val3)).Dispose();
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val)).Dispose();
			}
		}

		internal static void lpIKp1xWfw(float width)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			float num = OmenServiceBase<FontManager>.Instance().Config.FontSize;
			ImGui.SetNextItemWidth(width);
			if (ImGui.InputFloat(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x46297E99 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7)), ref num, 0f, 0f, ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E38A19B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997)), (ImGuiInputTextFlags)0))
			{
				num = Math.Clamp(num, 8f, 48f);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				OmenServiceBase<FontManager>.Instance().Config.FontSize = num;
				OmenServiceBase<FontManager>.Instance().Config.Save();
			}
		}

		static sBs7DvKjKAhrVE3ke5W()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			int num = 5;
			List<string> list = new List<string>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<string> span = CollectionsMarshal.AsSpan(list);
			span[0] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EED3E1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b);
			span[1] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C2B0A7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0);
			span[2] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4119570D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52);
			span[3] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200FE3AF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af);
			span[4] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F13351 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a);
			AllModerators = list;
			num = 10;
			List<string> list2 = new List<string>(num);
			CollectionsMarshal.SetCount(list2, num);
			Span<string> span2 = CollectionsMarshal.AsSpan(list2);
			span2[0] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379C72B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09);
			span2[1] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F13327 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a);
			span2[2] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D48A1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc);
			span2[3] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A241E8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079);
			span2[4] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7C0C5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff);
			span2[5] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F133ED ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a);
			span2[6] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6540CA5C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb);
			span2[7] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A7E1A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a);
			span2[8] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBD6F9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2);
			span2[9] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-864210836 ^ -1470643748 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df);
			AllCredits = list2;
		}

		[CompilerGenerated]
		private void Y8SK3U3mbA()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			ImGui.SetNextItemWidth(ControlWidth);
			ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x513358DA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b)), ImU8String.op_Implicit(((object)PluginConfig.Instance().ConflictKeyBinding.Keyboard/*cast due to constrained. prefix*/).ToString()), (ImGuiComboFlags)8);
			try
			{
				if (!val)
				{
					return;
				}
				ImGui.SetNextItemWidth(-1f);
				ImU8String val2 = ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC1A87 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1));
				ImU8String val3 = new ImU8String(3, 1);
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA78C21 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff)));
				((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x39221E7C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
				ImGui.InputTextWithHint(val2, val3, ref conflictKeySearchString, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				ImGui.Separator();
				ImGui.Spacing();
				foreach (VirtualKey validVirtualKey in DalamudServiceExtension.Instance<IKeyState>().GetValidVirtualKeys())
				{
					if ((string.IsNullOrWhiteSpace(conflictKeySearchString) || VirtualKeyExtensions.GetFancyName(validVirtualKey).Contains(conflictKeySearchString, StringComparison.OrdinalIgnoreCase)) && ImGui.Selectable(ImU8String.op_Implicit(VirtualKeyExtensions.GetFancyName(validVirtualKey)), false, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						PluginConfig.Instance().ConflictKeyBinding.Keyboard = validVirtualKey;
						PluginConfig.Instance().Save();
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val)).Dispose();
			}
		}

		[CompilerGenerated]
		private unsafe void OGSKBfbrTP()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			ImGui.SetNextItemWidth(ControlWidth);
			ComboDisposable val = ImRaii.Combo(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BAD68D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503)), ImU8String.op_Implicit(((object)PluginConfig.Instance().ConflictKeyBinding.Gamepad/*cast due to constrained. prefix*/).ToString()), (ImGuiComboFlags)8);
			try
			{
				if (!val)
				{
					return;
				}
				ImGui.SetNextItemWidth(-1f);
				ImU8String val2 = ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3701F34 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e));
				ImU8String val3 = new ImU8String(3, 1);
				((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C4042 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)));
				((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816A6D8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd));
				ImGui.InputTextWithHint(val2, val3, ref conflictKeySearchString, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				ImGui.Separator();
				ImGui.Spacing();
				GamepadButtons[] values = Enum.GetValues<GamepadButtons>();
				for (int i = 0; i < values.Length; i++)
				{
					GamepadButtons val4 = values[i];
					if ((string.IsNullOrWhiteSpace(conflictKeySearchString) || ((object)(*(GamepadButtons*)(&val4))/*cast due to constrained. prefix*/).ToString().Contains(conflictKeySearchString, StringComparison.OrdinalIgnoreCase)) && (int)val4 != 0)
					{
						GamepadButtons gamepad = PluginConfig.Instance().ConflictKeyBinding.Gamepad;
						if (ImGui.Selectable(ImU8String.op_Implicit(((object)(*(GamepadButtons*)(&val4))/*cast due to constrained. prefix*/).ToString()), gamepad.IsSetAny<GamepadButtons>(new ReadOnlySpan<GamepadButtons>((GamepadButtons)val4)), (ImGuiSelectableFlags)0, default(Vector2)))
						{
							gamepad = (GamepadButtons)((!gamepad.IsSetAny<GamepadButtons>(new ReadOnlySpan<GamepadButtons>((GamepadButtons)val4))) ? (gamepad | val4) : (gamepad & (ushort)(~val4)));
							PluginConfig.Instance().ConflictKeyBinding.Gamepad = gamepad;
							PluginConfig.Instance().Save();
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val)).Dispose();
			}
		}

		[CompilerGenerated]
		private void zNlKm3Kg0W()
		{
			CPiKxGRoiq(ControlWidth, ref fontSearchString, 300f * Globals.GlobalUIScale, isNoBackgroundAndScrollbar: true);
		}

		[CompilerGenerated]
		private void OB9KiUhd9l()
		{
			StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)13, new Vector2(8f * Globals.GlobalUIScale, 8f * Globals.GlobalUIScale), true);
			try
			{
				O9EK8bs56A(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642A45C6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C2CE89 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0), (FontAwesomeIcon)61734);
				ImGui.SameLine();
				O9EK8bs56A(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B57584 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CC8BDD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), (FontAwesomeIcon)61574);
				ImGui.SameLine();
				O9EK8bs56A(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D2425A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1)), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5583EAA6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d), (FontAwesomeIcon)61867);
				ImGui.SameLine();
				O9EK8bs56A(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -1618735726 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1)), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE80CF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76), (FontAwesomeIcon)61529);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		[CompilerGenerated]
		private void YgjKPgrq2e()
		{
		}
	}

	private Vector2 leftTabComponentSize;

	private Vector2 logoComponentSize;

	private bool isLeftTabExpanded;

	private float leftTabCurrentWidth;

	private Vector2 upperTabComponentSize;

	private float searchFocusProgress;

	private float searchFocusVelocity;

	private float searchBarCurrentWidth;

	private float searchBarWidthVelocity;

	private bool isSearchFocused;

	private Vector2 rightTabComponentSize;

	private Vector2 contactComponentSize;

	private bool isNeedToScrollToTop;

	private bool isDraggingButton;

	private Vector2 dragStartPos;

	private double trayFlashStartTime;

	private Task? openFirstTimeInstructionTask;

	private static readonly string WindowTitle;

	private readonly Dictionary<Type, swio5KOzdbVemDXeuDN> tabPages;

	private static readonly FrozenDictionary<TabPageID, Type> TabPageIDToType;

	private static readonly FrozenDictionary<TabPageID, string> TabPagesLoc;

	internal TabPageID SelectedTab { get; set; }

	public HfWYPNVIio8M9ncZnm0()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		isLeftTabExpanded = true;
		leftTabCurrentWidth = PluginConfig.Instance().LeftTabWidth;
		((Window)this)._002Ector(WindowTitle + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B30B7DA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
		WindowSizeConstraints value = default(WindowSizeConstraints);
		((WindowSizeConstraints)(ref value))._002Ector();
		((WindowSizeConstraints)(ref value)).MinimumSize = new Vector2(325f, 200f);
		((Window)this).SizeConstraints = value;
		SelectedTab = (TabPageID)PluginConfig.Instance().DefaultHomePage;
		OmenServiceBase<WindowManager>.Instance().PostDraw += s7NVuTOelw;
		tabPages = new Dictionary<Type, swio5KOzdbVemDXeuDN>
		{
			[typeof(sBs7DvKjKAhrVE3ke5W)] = new sBs7DvKjKAhrVE3ke5W(this),
			[typeof(OMQ0W4KSvbgxtAOQfUK)] = new OMQ0W4KSvbgxtAOQfUK(this),
			[typeof(Ilo1xvOu4IfQAk4pJC7)] = new Ilo1xvOu4IfQAk4pJC7(this),
			[typeof(L19rdTOIWaZP57ZxwlW)] = new L19rdTOIWaZP57ZxwlW(this),
			[typeof(PsFBitORrb7AIRB0KYJ)] = new PsFBitORrb7AIRB0KYJ(this),
			[typeof(d1MFHNO7lI9UoBJIpx5)] = new d1MFHNO7lI9UoBJIpx5(this)
		};
	}

	public void Dispose()
	{
		OmenServiceBase<WindowManager>.Instance().PostDraw -= s7NVuTOelw;
		tabPages.Values.ForEach(delegate(swio5KOzdbVemDXeuDN x)
		{
			x.Dispose();
		});
	}

	public override bool DrawConditions()
	{
		return openFirstTimeInstructionTask == null;
	}

	public override void OnOpen()
	{
		if (openFirstTimeInstructionTask != null)
		{
			return;
		}
		if (!Version.TryParse(PluginConfig.Instance().LastVersion, out Version result))
		{
			result = new Version(0, 0, 0, 0);
		}
		if (((object)result != null && result.Major == 0 && result.Minor == 0 && result.Build == 0 && result.Revision == 0) || Plugin.Version != result)
		{
			XOoANOVG4jDKQ7WvCb9 xOoANOVG4jDKQ7WvCb = OmenServiceBase<WindowManager>.Instance().Get<XOoANOVG4jDKQ7WvCb9>();
			if (xOoANOVG4jDKQ7WvCb != null)
			{
				xOoANOVG4jDKQ7WvCb.LastVersion = result;
			}
			openFirstTimeInstructionTask = DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
			{
				XOoANOVG4jDKQ7WvCb9 xOoANOVG4jDKQ7WvCb2 = OmenServiceBase<WindowManager>.Instance().Get<XOoANOVG4jDKQ7WvCb9>();
				if (xOoANOVG4jDKQ7WvCb2 != null)
				{
					((Window)this).IsOpen = false;
					((Window)xOoANOVG4jDKQ7WvCb2).IsOpen = true;
				}
			}, default(TimeSpan), 0, default(CancellationToken)).ContinueWith((Task _) => openFirstTimeInstructionTask = null);
		}
		else
		{
			PluginConfig.Reload();
			((Window)this).Collapsed = false;
		}
	}

	public override void OnClose()
	{
		PluginConfig.Instance().Save();
		trayFlashStartTime = ImGui.GetTime();
	}

	public override void Draw()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		((Window)this).WindowName = WindowTitle + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200F5083 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af);
		((Window)this).Collapsed = null;
		((Window)this).CollapsedCondition = (ImGuiCond)0;
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		GWyVzRYEdK();
		ImGui.SameLine(0f, 12f * Globals.GlobalUIScale);
		GroupDisposable val = ImRaii.Group();
		try
		{
			HaUVXvfO8U();
			SxBVY0xy20();
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		xoW9nENUiO(cursorScreenPos);
	}

	private void s7NVuTOelw()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		if (!PluginConfig.Instance().ShowTrayOverlay || DalamudLeftServiceExtension.Instance().CutsceneActive || ((Window)this).IsOpen || !ImGui.Begin(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E25ABA5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25)), (ImGuiWindowFlags)2105579))
		{
			return;
		}
		Vector2 windowPos = ImGui.GetWindowPos();
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)21, 0u, true);
		try
		{
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)22, 0u, true);
			try
			{
				ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)23, 0u, true);
				try
				{
					float w = 1f;
					double num = ImGui.GetTime() - trayFlashStartTime;
					if (num < 2.5)
					{
						w = 0.3f + 0.7f * (float)Math.Abs(Math.Cos(num * 4.0));
					}
					if (ImGui.ImageButton(Assets.Icon.GetWrapOrEmpty().Handle, Globals.ScaledVector2(PluginConfig.Instance().TrayOverlaySize), Vector2.Zero, Vector2.One, -1, Vector4.Zero, new Vector4(1f, 1f, 1f, w)))
					{
						((Window)this).IsOpen = !((Window)this).IsOpen;
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
		if (ImGui.IsItemHovered() && ImGui.IsMouseClicked((ImGuiMouseButton)1))
		{
			isDraggingButton = true;
			dragStartPos = ImGui.GetMousePos();
		}
		if (isDraggingButton)
		{
			Vector2 mousePos = ImGui.GetMousePos();
			Vector2 vector = mousePos - dragStartPos;
			if (vector.X != 0f || vector.Y != 0f)
			{
				Vector2 vector2 = new Vector2(windowPos.X + vector.X, windowPos.Y + vector.Y);
				ImGui.SetWindowPos(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1675397288 ^ -1527675838 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1)), vector2, (ImGuiCond)0);
				dragStartPos = mousePos;
			}
			if (ImGui.IsMouseReleased((ImGuiMouseButton)1))
			{
				isDraggingButton = false;
			}
		}
		ImGui.End();
	}

	private void HaUVXvfO8U()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
		{
			upperTabComponentSize.X = ImGui.GetContentRegionAvail().X;
			ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F354634 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862)), upperTabComponentSize, false, (ImGuiWindowFlags)152);
			try
			{
				if (!val)
				{
					return;
				}
				GroupDisposable val2 = ImRaii.Group();
				try
				{
					ImGuiOm.ScaledDummy(4f);
					Vector2 cursorPos = ImGui.GetCursorPos() + new Vector2(4f * Globals.GlobalUIScale, 0f);
					ImGuiStylePtr style = ImGui.GetStyle();
					Vector2 itemSpacing = ((ImGuiStylePtr)(ref style)).ItemSpacing;
					Vector2 buttonSize = sv49SfOr7P<sBs7DvKjKAhrVE3ke5W>().ButtonSize;
					float num = ImGui.GetContentRegionAvail().X - buttonSize.X - 3f * itemSpacing.X;
					ImGuiIOPtr iO = ImGui.GetIO();
					float num2 = Math.Min(((ImGuiIOPtr)(ref iO)).DeltaTime, 0.05f);
					if (searchBarCurrentWidth == 0f)
					{
						searchBarCurrentWidth = num;
					}
					float num3 = searchBarCurrentWidth - num;
					float num4 = -150f * num3 - 10f * searchBarWidthVelocity;
					searchBarWidthVelocity += num4 * num2;
					searchBarCurrentWidth += searchBarWidthVelocity * num2;
					float num5 = (isSearchFocused ? 1f : 0f);
					float num6 = searchFocusProgress - num5;
					float num7 = -180f * num6 - 12f * searchFocusVelocity;
					searchFocusVelocity += num7 * num2;
					searchFocusProgress += searchFocusVelocity * num2;
					if (searchFocusProgress < 0f)
					{
						searchFocusProgress = 0f;
					}
					if (searchFocusProgress > 1f)
					{
						searchFocusProgress = 1f;
					}
					ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
					float num8 = 8f * Globals.GlobalUIScale;
					float num9 = ImGui.GetFrameHeight() + num8;
					float num10 = 8f * Globals.GlobalUIScale;
					Vector2 vector = ImGui.GetCursorScreenPos() + new Vector2(4f * Globals.GlobalUIScale, 0f);
					Vector2 vector2 = vector + new Vector2(searchBarCurrentWidth, num9);
					Vector4 vector3 = KnownColor.Chocolate.ToVector4();
					if (searchFocusProgress > 0.01f)
					{
						float num11 = searchFocusProgress;
						float num12 = num11 * num11 * (3f - 2f * num11);
						Vector4 color = vector3;
						color.W = 0.55f * num12;
						float glowSize = 10f * Globals.GlobalUIScale * (0.35f + 0.65f * num12);
						ImGuiOm.AddGlowRect(windowDrawList, vector, vector2, color.ToUInt(), num10, glowSize, 10, 0.3f);
						Vector4 vector4 = vector3;
						vector4.W = 0.18f * num12;
						((ImDrawListPtr)(ref windowDrawList)).AddRect(vector + new Vector2(0.5f), vector2 - new Vector2(0.5f), ImGui.ColorConvertFloat4ToU32(vector4), num10, (ImDrawFlags)0, 1f);
					}
					uint colorU = ImGui.GetColorU32((ImGuiCol)7);
					((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector, vector2, colorU, num10);
					style = ImGui.GetStyle();
					uint num13 = ImGui.ColorConvertFloat4ToU32(Vector4.Lerp(((ImGuiStylePtr)(ref style)).Colors[5], vector3, searchFocusProgress));
					((ImDrawListPtr)(ref windowDrawList)).AddRect(vector, vector2, num13, num10, (ImDrawFlags)0, 1f + searchFocusProgress);
					Vector2 vector5 = vector + new Vector2(8f * Globals.GlobalUIScale, (num9 - ImGui.GetTextLineHeight()) / 2f);
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector5, ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61442)));
					ImGui.SetCursorPos(cursorPos);
					ImGui.SetNextItemWidth(num);
					float x = vector5.X + 2f * Globals.GlobalUIScale - vector.X + ImGui.GetTextLineHeight() + 4f * Globals.GlobalUIScale;
					style = ImGui.GetStyle();
					float y = ((ImGuiStylePtr)(ref style)).FramePadding.Y + num8 / 2f;
					StyleDisposable val3 = ImRaii.PushStyle((ImGuiStyleVar)10, new Vector2(x, y), true);
					try
					{
						StyleDisposable val4 = ImRaii.PushStyle((ImGuiStyleVar)11, num10, true);
						try
						{
							ColorDisposable val5 = ImRaii.PushColor((ImGuiCol)7, 0u, true);
							try
							{
								ColorDisposable val6 = ImRaii.PushColor((ImGuiCol)5, 0u, true);
								try
								{
									string searchInputBuffer = sv49SfOr7P<OMQ0W4KSvbgxtAOQfUK>().SearchInputBuffer;
									if (ImGui.InputText(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5ACD08 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a)), ref searchInputBuffer, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
									{
										sv49SfOr7P<OMQ0W4KSvbgxtAOQfUK>().SearchInputBuffer = searchInputBuffer;
									}
									isSearchFocused = ImGui.IsItemActive() || ImGui.IsItemFocused();
									sv49SfOr7P<sBs7DvKjKAhrVE3ke5W>().InputBoxHeight = ImGui.GetItemRectSize().Y;
								}
								finally
								{
									((IDisposable)val6)?.Dispose();
								}
							}
							finally
							{
								((IDisposable)val5)?.Dispose();
							}
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					ImGui.SameLine();
					ImGui.SetCursorPosX(cursorPos.X + num + itemSpacing.X);
					sv49SfOr7P<sBs7DvKjKAhrVE3ke5W>().hiNTxC7Py0();
					ImGuiOm.ScaledDummy(4f);
				}
				finally
				{
					((GroupDisposable)(ref val2)).Dispose();
				}
				upperTabComponentSize.Y = ImGui.GetItemRectSize().Y;
			}
			finally
			{
				((ChildDisposable)(ref val)).Dispose();
			}
		}
	}

	private void SxBVY0xy20()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		rightTabComponentSize = ImGui.GetContentRegionAvail();
		float x = 12f * Globals.GlobalUIScale;
		ImGuiStylePtr style = ImGui.GetStyle();
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)1, new Vector2(x, ((ImGuiStylePtr)(ref style)).WindowPadding.Y), true);
		try
		{
			ChildDisposable val2 = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA773E9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff)), rightTabComponentSize, false, (ImGuiWindowFlags)(((SelectedTab == TabPageID.主页) ? 16 : 0) | 0x80));
			try
			{
				if (!val2)
				{
					return;
				}
				IDalamudTextureWrap val3 = default(IDalamudTextureWrap);
				Exception ex = default(Exception);
				if (Assets.BackgroundTexture.TryGetWrap(ref val3, ref ex))
				{
					ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
					Vector2 windowPos = ImGui.GetWindowPos();
					Vector2 windowSize = ImGui.GetWindowSize();
					Vector2 size = val3.Size;
					float num = windowSize.X / windowSize.Y;
					float num2 = size.X / size.Y;
					Vector2 vector;
					if (num > num2)
					{
						float y = windowSize.Y;
						float x2 = y * num2;
						vector = new Vector2(x2, y);
					}
					else
					{
						float x3 = windowSize.X;
						float y2 = x3 / num2;
						vector = new Vector2(x3, y2);
					}
					vector *= 0.6f;
					Vector2 vector2 = (windowSize - vector) / 2f;
					vector2.X -= 16f * Globals.GlobalUIScale;
					Vector2 vector3 = windowPos + vector2;
					Vector2 vector4 = vector3 + vector;
					((ImDrawListPtr)(ref windowDrawList)).AddImage(val3.Handle, vector3, vector4, Vector2.Zero, Vector2.One, VectorExtensions.WithW(Vector4.One, 0.07f).ToUInt());
				}
				if (TabPageIDToType.TryGetValue(SelectedTab, out Type value) && tabPages.TryGetValue(value, out swio5KOzdbVemDXeuDN value2))
				{
					if (!string.IsNullOrWhiteSpace(sv49SfOr7P<OMQ0W4KSvbgxtAOQfUK>().SearchInputBuffer))
					{
						sv49SfOr7P<OMQ0W4KSvbgxtAOQfUK>().Draw();
					}
					else
					{
						value2.Draw();
					}
				}
			}
			finally
			{
				((ChildDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void GWyVzRYEdK()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		leftTabComponentSize.Y = ImGui.GetContentRegionAvail().Y;
		float value = (isLeftTabExpanded ? PluginConfig.Instance().LeftTabWidth : 24f);
		float x = Vector2.Lerp(new Vector2(leftTabCurrentWidth), new Vector2(value), 0.1f).X;
		leftTabCurrentWidth = ((Math.Abs(24f - x) < 1.5f) ? 24f : ((Math.Abs(PluginConfig.Instance().LeftTabWidth - x) < 1.5f) ? PluginConfig.Instance().LeftTabWidth : x));
		ImU8String val = ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FF60B8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
		Vector2 vector = leftTabComponentSize;
		vector.X = leftTabCurrentWidth;
		ChildDisposable val2 = ImRaii.Child(val, vector, false, (ImGuiWindowFlags)136);
		try
		{
			if (ChildDisposable.op_Implicit(val2) && (isLeftTabExpanded || Math.Abs(leftTabCurrentWidth - 24f) > 24f))
			{
				ImGuiOm.ScaledDummy(1f, 16f);
				Fws9yOuKvP();
				ImGuiOm.ScaledDummy(1f, 4f);
				mm19L0MFAb();
				ImGuiOm.ScaledDummy(1f, 12f);
				StyleDisposable val3 = ImRaii.PushStyle((ImGuiStyleVar)1, Vector2.Zero, true);
				try
				{
					ChildDisposable val4 = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2828B497 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184)), new Vector2(0f, ImGui.GetContentRegionAvail().Y), false, (ImGuiWindowFlags)136);
					try
					{
						if (ChildDisposable.op_Implicit(val4))
						{
							omw9HYU6ev();
						}
					}
					finally
					{
						((ChildDisposable)(ref val4)).Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
		}
		finally
		{
			((ChildDisposable)(ref val2)).Dispose();
		}
		leftTabComponentSize.X = leftTabCurrentWidth;
	}

	private void xoW9nENUiO(Vector2 screenStartPos)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = Globals.ScaledVector2(24f);
		ImGui.SetCursorScreenPos(screenStartPos + new Vector2(leftTabCurrentWidth - vector.X * 0.7f, (leftTabComponentSize.Y - vector.Y) / 2f));
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)1, Vector2.Zero, true);
		try
		{
			ChildDisposable val2 = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329CB808 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)), vector, false, (ImGuiWindowFlags)136);
			try
			{
				if (!ChildDisposable.op_Implicit(val2))
				{
					return;
				}
				uint colorU = ImGui.GetColorU32((ImGuiCol)2);
				FontAwesomeIcon val3 = (FontAwesomeIcon)(isLeftTabExpanded ? 61696 : 61697);
				ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)21, colorU, true).Push((ImGuiCol)22, colorU, true).Push((ImGuiCol)23, colorU, true);
				try
				{
					if (ImGui.Button(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString(val3)), vector))
					{
						isLeftTabExpanded = !isLeftTabExpanded;
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
			finally
			{
				((ChildDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void Fws9yOuKvP()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		ImGuiHelpers.CenterCursorFor(logoComponentSize.X);
		GroupDisposable val = ImRaii.Group();
		try
		{
			ImGuiHelpers.CenterCursorFor(96f * Globals.GlobalUIScale);
			ImGui.Image(Assets.Icon.GetWrapOrEmpty().Handle, Globals.ScaledVector2(96f));
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		logoComponentSize = ImGui.GetItemRectSize();
		ImGuiOm.ScaledDummy(1f, 10f);
		ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)0, KnownColor.LawnGreen.ToVector4(), AuthState.IsAuth);
		try
		{
			ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)0, KnownColor.OrangeRed.ToVector4(), !AuthState.IsAuth);
			try
			{
				using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 3);
					defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B50EA2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3));
					defaultInterpolatedStringHandler.AppendLiteral("1.0");
					defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5ACAC6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
					defaultInterpolatedStringHandler.AppendFormatted(ManagerBase<ModuleManager>.Instance().ModulesEnableCount);
					defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DD183E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
					defaultInterpolatedStringHandler.AppendFormatted(ManagerBase<ModuleManager>.Instance().ModulesValidCount);
					defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-864210836 ^ -475989839 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1));
					brx9jtRo9m(defaultInterpolatedStringHandler.ToStringAndClear(), 4f);
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
					}
					if (ImGui.IsItemClicked())
					{
						if (openFirstTimeInstructionTask != null)
						{
							return;
						}
						if (!Version.TryParse(PluginConfig.Instance().LastVersion, out Version result))
						{
							result = new Version(0, 0, 0, 0);
						}
						XOoANOVG4jDKQ7WvCb9 xOoANOVG4jDKQ7WvCb = OmenServiceBase<WindowManager>.Instance().Get<XOoANOVG4jDKQ7WvCb9>();
						if (xOoANOVG4jDKQ7WvCb != null)
						{
							xOoANOVG4jDKQ7WvCb.LastVersion = result;
						}
						openFirstTimeInstructionTask = DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
						{
							XOoANOVG4jDKQ7WvCb9 xOoANOVG4jDKQ7WvCb2 = OmenServiceBase<WindowManager>.Instance().Get<XOoANOVG4jDKQ7WvCb9>();
							if (xOoANOVG4jDKQ7WvCb2 != null)
							{
								((Window)this).IsOpen = false;
								((Window)xOoANOVG4jDKQ7WvCb2).IsOpen = true;
							}
						}, default(TimeSpan), 0, default(CancellationToken)).ContinueWith((Task _) => openFirstTimeInstructionTask = null);
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
			((IDisposable)val2)?.Dispose();
		}
		uint count = ManagerBase<e7PW07HBlVpVNpgPVZx>.Instance().Count;
		if (count == 0)
		{
			return;
		}
		DateTime date = ManagerBase<e7PW07HBlVpVNpgPVZx>.Instance().Date;
		val2 = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), true);
		try
		{
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				brx9jtRo9m(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x65407E80 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb), count), 4f);
				if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip(ImU8String.op_Implicit(date.ToShortDateString()));
				}
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void mm19L0MFAb()
	{
	}

	private void omw9HYU6ev()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
		{
			GroupDisposable val = ImRaii.Group();
			try
			{
				foreach (var (type2, swio5KOzdbVemDXeuDN3) in tabPages)
				{
					if (!swio5KOzdbVemDXeuDN3.IsHiddenInSidebar)
					{
						if (type2 == typeof(d1MFHNO7lI9UoBJIpx5))
						{
							ImGuiOm.ScaledDummy(12f);
						}
						swio5KOzdbVemDXeuDN3.hiNTxC7Py0();
					}
				}
			}
			finally
			{
				((GroupDisposable)(ref val)).Dispose();
			}
		}
	}

	internal T sv49SfOr7P<T>() where T : swio5KOzdbVemDXeuDN
	{
		return tabPages.GetValueOrDefault(typeof(T)) as T;
	}

	static HfWYPNVIio8M9ncZnm0()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		WindowTitle = "ziz";
		TabPageIDToType = new Dictionary<TabPageID, Type>
		{
			[TabPageID.设置] = typeof(sBs7DvKjKAhrVE3ke5W),
			[TabPageID.搜索] = typeof(OMQ0W4KSvbgxtAOQfUK),
			[TabPageID.收藏] = typeof(Ilo1xvOu4IfQAk4pJC7),
			[TabPageID.启用] = typeof(L19rdTOIWaZP57ZxwlW),
			[TabPageID.验证] = typeof(PsFBitORrb7AIRB0KYJ),
			[TabPageID.模块] = typeof(d1MFHNO7lI9UoBJIpx5)
		}.ToFrozenDictionary();
		TabPagesLoc = new Dictionary<TabPageID, string>
		{
			[TabPageID.设置] = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x55835D74 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d)),
			[TabPageID.收藏] = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6116996A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402)),
			[TabPageID.启用] = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-864210836 ^ -410152200 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d)),
			[TabPageID.验证] = "No Spyware"
		}.ToFrozenDictionary();
	}

	[CompilerGenerated]
	private void ELX9VpNdVo()
	{
		XOoANOVG4jDKQ7WvCb9 xOoANOVG4jDKQ7WvCb = OmenServiceBase<WindowManager>.Instance().Get<XOoANOVG4jDKQ7WvCb9>();
		if (xOoANOVG4jDKQ7WvCb != null)
		{
			((Window)this).IsOpen = false;
			((Window)xOoANOVG4jDKQ7WvCb).IsOpen = true;
		}
	}

	[CompilerGenerated]
	private Task nOE99yit6x(Task _)
	{
		return openFirstTimeInstructionTask = null;
	}

	[CompilerGenerated]
	private void Ccm9fwolV8()
	{
		XOoANOVG4jDKQ7WvCb9 xOoANOVG4jDKQ7WvCb = OmenServiceBase<WindowManager>.Instance().Get<XOoANOVG4jDKQ7WvCb9>();
		if (xOoANOVG4jDKQ7WvCb != null)
		{
			((Window)this).IsOpen = false;
			((Window)xOoANOVG4jDKQ7WvCb).IsOpen = true;
		}
	}

	[CompilerGenerated]
	private Task EKc9C93D9b(Task _)
	{
		return openFirstTimeInstructionTask = null;
	}

	[CompilerGenerated]
	internal static void brx9jtRo9m(string text, float offsetY = 0f, bool isDisabled = false)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetCursorPosY(ImGui.GetCursorPosY() - (float)Math.Round(offsetY * Globals.GlobalUIScale));
		ImGuiHelpers.CenterCursorForText(ImU8String.op_Implicit(text));
		if (isDisabled)
		{
			ImGuiOm.TextDisabledWrapped(text);
		}
		else
		{
			ImGuiOm.Text(text);
		}
	}
}
