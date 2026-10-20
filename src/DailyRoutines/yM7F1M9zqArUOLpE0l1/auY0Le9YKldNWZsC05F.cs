using System.Numerics;
using B2nED1Wmdu0xsDR7Tjv;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using GO3ln7f4pExIcYCK2q3;
using OmenTools.Global;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using P67IJbfls2Hy3Xtb45R;
using itAnDoW4mgYVlrmtMor;

namespace yM7F1M9zqArUOLpE0l1;

internal sealed class auY0Le9YKldNWZsC05F : ExINkNfv21TTK3yJ2Cx
{
	private uint logMessageIDInput;

	public override string SectionID => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA0B4B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1);

	public override string Title => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1317373447 ^ 0x9B6FEE0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5);

	protected unsafe override void DrawContent(VejoYhfM1yiPckJhcdm context)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		ImGui.Checkbox(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642AB19A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df)), ref OmenServiceBase<LogMessageManager>.Instance().Config.ShowLogMessageLog);
		ImGui.SetNextItemWidth(220f * Globals.GlobalUIScale);
		ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA0A87 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1)), ref logMessageIDInput, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
		ImGui.SameLine();
		if (ImGui.Button(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1EA82 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83)), default(Vector2)))
		{
			((RaptureLogModule)RaptureLogModule.Instance()).ShowLogMessage(logMessageIDInput);
		}
		ImGui.NewLine();
		ImGui.Checkbox(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFB894D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc)), ref OmenServiceBase<LogMessageManager>.Instance().Config.ShowInstanceContentTextLog);
	}

	public auY0Le9YKldNWZsC05F()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
