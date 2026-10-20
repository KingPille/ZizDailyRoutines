using System.Numerics;
using System.Runtime.CompilerServices;
using B2nED1Wmdu0xsDR7Tjv;
using Dalamud.Bindings.ImGui;
using GO3ln7f4pExIcYCK2q3;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using P67IJbfls2Hy3Xtb45R;
using itAnDoW4mgYVlrmtMor;

namespace YhDutv93SVCTHF2m7p1;

internal sealed class y1e9LQ9pwGSZpjyTOxI : ExINkNfv21TTK3yJ2Cx
{
	private string chatMessageInput;

	private string commandMessageInput;

	public override string SectionID => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CDBCC7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0);

	public override string Title => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642A4978 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df);

	protected override void DrawContent(VejoYhfM1yiPckJhcdm context)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		ImGui.Checkbox(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702BEF5F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30)), ref OmenServiceBase<ChatManager>.Instance().Config.ShowProcessChatBoxEntryLog);
		if (ImGui.Button(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E46E5C9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e)), default(Vector2)))
		{
			O579BmC9ot();
		}
		ImGui.SameLine();
		ImGui.SetNextItemWidth(-1f);
		if (ImGui.InputText(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E94C88D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75)), ref chatMessageInput, 512, (ImGuiInputTextFlags)32, (ImGuiInputTextCallbackDelegate)null))
		{
			O579BmC9ot();
		}
		ImGui.NewLine();
		ImGui.Checkbox(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F354D08 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862)), ref OmenServiceBase<ChatManager>.Instance().Config.ShowExecuteCommandInnerLog);
		ImGui.AlignTextToFramePadding();
		ImGui.TextUnformatted(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A2F986 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079)));
		if (ImGui.Button(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816577E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd)), default(Vector2)))
		{
			ItY9mhpOVZ();
		}
		ImGui.SameLine();
		ImGui.SetNextItemWidth(-1f);
		if (ImGui.InputText(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x611695C2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402)), ref commandMessageInput, 512, (ImGuiInputTextFlags)32, (ImGuiInputTextCallbackDelegate)null))
		{
			ItY9mhpOVZ();
		}
	}

	public y1e9LQ9pwGSZpjyTOxI()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		chatMessageInput = string.Empty;
		commandMessageInput = string.Empty;
		base._002Ector();
	}

	[CompilerGenerated]
	private void O579BmC9ot()
	{
		string text = chatMessageInput.Trim();
		if (!string.IsNullOrWhiteSpace(text))
		{
			OmenServiceBase<ChatManager>.Instance().SendMessage(text);
			chatMessageInput = string.Empty;
		}
	}

	[CompilerGenerated]
	private void ItY9mhpOVZ()
	{
		string text = commandMessageInput.Trim();
		if (!string.IsNullOrWhiteSpace(text))
		{
			OmenServiceBase<ChatManager>.Instance().SendCommand(text);
			commandMessageInput = string.Empty;
		}
	}
}
