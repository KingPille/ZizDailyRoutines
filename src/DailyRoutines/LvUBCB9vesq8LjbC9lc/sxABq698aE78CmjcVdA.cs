using System;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Extensions;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.STD;
using FFXIVClientStructs.STD.Helper;
using GO3ln7f4pExIcYCK2q3;
using InteropGenerator.Runtime;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using P67IJbfls2Hy3Xtb45R;
using itAnDoW4mgYVlrmtMor;

namespace LvUBCB9vesq8LjbC9lc;

internal class sxABq698aE78CmjcVdA : ExINkNfv21TTK3yJ2Cx
{
	private int addonNamesSkipCount;

	public override string SectionID => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4119E183 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52);

	public override string Title => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B44C48 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3);

	protected unsafe override void DrawContent(VejoYhfM1yiPckJhcdm context)
	{
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62830, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDAFF09 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1)))
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = -1;
			int num2 = 0;
			UnmanagedArrayEnumerator<Utf8String> enumerator = ((RaptureAtkModule)RaptureAtkModule.Instance()).AddonNames.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Utf8String current = enumerator.Current;
					num++;
					if (addonNamesSkipCount <= num)
					{
						StringBuilder stringBuilder2 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(50, 2, stringBuilder2);
						handler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE8323 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
						handler.AppendFormatted<Utf8String>(current);
						handler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A2F794 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
						handler.AppendFormatted<Utf8String>(current);
						handler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47338CE1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5));
						stringBuilder2.AppendLine(ref handler);
						stringBuilder.AppendLine();
						num2++;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			ImGui.SetClipboardText(ImU8String.op_Implicit(stringBuilder.ToString()));
			NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C6F44 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
			defaultInterpolatedStringHandler.AppendFormatted(num2);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFB7D25 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
			notifyHelper.NotificationSuccess(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		ImGui.Separator();
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E94C75B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75)), 2, (ImGuiTableFlags)1985);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1317373447 ^ 0x75DF4FCF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a)), (ImGuiTableColumnFlags)4, 10f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329CBB44 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)), (ImGuiTableColumnFlags)4, 50f, 0u);
			ImGui.TableHeadersRow();
			StdVector<Utf8String> addonNames = ((RaptureAtkModule)RaptureAtkModule.Instance()).AddonNames;
			ImGuiListClipper val2 = default(ImGuiListClipper);
			((ImGuiListClipper)(ref val2)).Begin(addonNames.Count);
			while (((ImGuiListClipper)(ref val2)).Step())
			{
				for (int i = val2.DisplayStart; i < val2.DisplayEnd; i++)
				{
					Utf8String val3 = addonNames[i];
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.TextUnformatted(ImU8String.op_Implicit(i.ToString()));
					ImGui.TableNextColumn();
					if (val3.IsEmpty || !((CStringPointer)(ref val3.StringPtr)).HasValue)
					{
						ImGui.TextDisabled(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C27821 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0)));
						continue;
					}
					string? text = ((object)(*(Utf8String*)(&val3))/*cast due to constrained. prefix*/).ToString();
					ImGui.TextUnformatted(ImU8String.op_Implicit(text));
					ImGuiOmExtension.ClickToCopyAndNotify(text);
				}
			}
			((ImGuiListClipper)(ref val2)).End();
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	public sxABq698aE78CmjcVdA()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
