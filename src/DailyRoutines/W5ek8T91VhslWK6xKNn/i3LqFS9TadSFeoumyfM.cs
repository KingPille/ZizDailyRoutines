using System;
using System.Numerics;
using ACnTQP9K7F3atyiqaA4;
using B2nED1Wmdu0xsDR7Tjv;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using GO3ln7f4pExIcYCK2q3;
using J7UpOWf8MZcXOAKCJr7;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using TOm8ovfpYOVGJGuvn7b;
using itAnDoW4mgYVlrmtMor;

namespace W5ek8T91VhslWK6xKNn;

internal sealed class i3LqFS9TadSFeoumyfM : Window, IDisposable
{
	private const float SIDEBAR_WIDTH = 200f;

	private readonly tkGYLK9Ox1OpXu2BoEN registry;

	private readonly R7tvbEfsm94VLLgDZJg viewState;

	public i3LqFS9TadSFeoumyfM()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		registry = new tkGYLK9Ox1OpXu2BoEN();
		viewState = new R7tvbEfsm94VLLgDZJg();
		((Window)this)._002Ector(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -323855244 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25));
	}

	public void Dispose()
	{
	}

	public override void Draw()
	{
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			Sqx9MN3e5b();
			ImGui.SameLine();
			au594HOqGQ();
		}
	}

	private void Sqx9MN3e5b()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47338DE1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5)), new Vector2(200f * Globals.GlobalUIScale, 0f), true);
		try
		{
			if (!val)
			{
				return;
			}
			foreach (dvY7ShfxMmccDlr70u8 value in registry.Sections.Values)
			{
				at09sTriHD(value);
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void au594HOqGQ()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		dvY7ShfxMmccDlr70u8 dvY7ShfxMmccDlr70u9 = registry.iJ49tMcdcM(viewState.CurrentSectionID);
		if (dvY7ShfxMmccDlr70u9 == null)
		{
			return;
		}
		if (string.IsNullOrWhiteSpace(viewState.CurrentSectionID))
		{
			viewState.CurrentSectionID = dvY7ShfxMmccDlr70u9.SectionID;
		}
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642A4724 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df)), ImGui.GetContentRegionAvail(), false);
		try
		{
			if (!(!val))
			{
				dvY7ShfxMmccDlr70u9.Draw(new VejoYhfM1yiPckJhcdm(viewState));
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void at09sTriHD(dvY7ShfxMmccDlr70u8 section)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(section.SectionID), true);
		try
		{
			if (ImGuiOm.ButtonSelectable(section.Title + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFB7C7B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc)))
			{
				viewState.CurrentSectionID = section.SectionID;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
