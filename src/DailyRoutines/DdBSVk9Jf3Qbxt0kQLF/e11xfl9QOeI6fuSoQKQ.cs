using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using GO3ln7f4pExIcYCK2q3;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using P67IJbfls2Hy3Xtb45R;
using itAnDoW4mgYVlrmtMor;

namespace DdBSVk9Jf3Qbxt0kQLF;

internal sealed class e11xfl9QOeI6fuSoQKQ : ExINkNfv21TTK3yJ2Cx
{
	private readonly struct W8GN8eKFNOv4lMETvrF : IEquatable<W8GN8eKFNOv4lMETvrF>
	{
		public uint Value { get; init; }

		public string DisplayText { get; init; }

		public string SearchText { get; init; }

		public W8GN8eKFNOv4lMETvrF(uint Value, string DisplayText, string SearchText)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.Value = Value;
			this.DisplayText = DisplayText;
			this.SearchText = SearchText;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--664844326 ^ 0x722354C2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D14E9E7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
			if (egFKqbixiN(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool egFKqbixiN(StringBuilder builder)
		{
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3701A98 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e));
			builder.Append(Value.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D24436 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
			builder.Append((object?)DisplayText);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD0731 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
			builder.Append((object?)SearchText);
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(W8GN8eKFNOv4lMETvrF left, W8GN8eKFNOv4lMETvrF right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(W8GN8eKFNOv4lMETvrF left, W8GN8eKFNOv4lMETvrF right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (EqualityComparer<uint>.Default.GetHashCode(Value) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayText)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SearchText);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is W8GN8eKFNOv4lMETvrF)
			{
				return Equals((W8GN8eKFNOv4lMETvrF)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(W8GN8eKFNOv4lMETvrF other)
		{
			if (EqualityComparer<uint>.Default.Equals(Value, other.Value) && EqualityComparer<string>.Default.Equals(DisplayText, other.DisplayText))
			{
				return EqualityComparer<string>.Default.Equals(SearchText, other.SearchText);
			}
			return false;
		}

		[CompilerGenerated]
		public void IiRKcw3tJl(out uint Value, out string DisplayText, out string SearchText)
		{
			Value = this.Value;
			DisplayText = this.DisplayText;
			SearchText = this.SearchText;
		}
	}

	private static readonly W8GN8eKFNOv4lMETvrF[] ExecuteCommandOptions;

	private static readonly FrozenDictionary<uint, W8GN8eKFNOv4lMETvrF> ExecuteCommandLookup;

	private static readonly W8GN8eKFNOv4lMETvrF[] ExecuteCommandComplexOptions;

	private static readonly FrozenDictionary<uint, W8GN8eKFNOv4lMETvrF> ExecuteCommandComplexLookup;

	private (uint commmand, uint p1, uint p2, uint p3, uint p4) executeCommandManual;

	private (uint commmand, long p1, uint p2, uint p3, uint p4, uint p5) executeCommandComplexManual;

	private (uint commmand, Vector3 location, uint p1, uint p2, uint p3, uint p4) executeCommandComplexLocationManual;

	private string executeCommandSearchInput;

	private string executeCommandComplexSearchInput;

	private string executeCommandLocationSearchInput;

	public override string SectionID => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FF685C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2);

	public override string Title => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F1884F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a);

	protected override void DrawContent(VejoYhfM1yiPckJhcdm context)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		ImGui.Checkbox(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339C775A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de)), ref OmenServiceBase<ExecuteCommandManager>.Instance().Config.ShowExecuteCommandLog);
		vx39rVChK3();
		ImGui.NewLine();
		ImGui.Checkbox(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4415A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3)), ref OmenServiceBase<ExecuteCommandManager>.Instance().Config.ShowExecuteCommandComplexLog);
		tXe9GIKwsP();
		ImGui.NewLine();
		ImGui.Checkbox(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9066F22 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3)), ref OmenServiceBase<ExecuteCommandManager>.Instance().Config.ShowExecuteCommandComplexLocationLog);
		aeV9684M5D();
	}

	private void vx39rVChK3()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CDB8CF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0)), true);
		try
		{
			Vfb9223YUY(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FF6F62 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F18F35 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a), ref executeCommandManual.commmand, ExecuteCommandOptions, ExecuteCommandLookup, ref executeCommandSearchInput);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702BEB27 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30)), ref executeCommandManual.p1, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x370A2F6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e)), ref executeCommandManual.p2, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53797B73 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09)), ref executeCommandManual.p3, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6540769A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb)), ref executeCommandManual.p4, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			if (ImGui.Button(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F18F01 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)), new Vector2(-1f, 0f)))
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand((ExecuteCommandFlag)executeCommandManual.commmand, executeCommandManual.p1, executeCommandManual.p2, executeCommandManual.p3, executeCommandManual.p4);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void tXe9GIKwsP()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BA6BAD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503)), true);
		try
		{
			Vfb9223YUY(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D2FC4A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E46E1D3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e), ref executeCommandComplexManual.commmand, ExecuteCommandComplexOptions, ExecuteCommandComplexLookup, ref executeCommandComplexSearchInput);
			ImGui.InputLong(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B44680 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3)), ref executeCommandComplexManual.p1, 0L, 0L, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5133E47A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b)), ref executeCommandComplexManual.p2, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339C7066 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de)), ref executeCommandComplexManual.p3, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28165214 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd)), ref executeCommandComplexManual.p4, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FF6FE4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2)), ref executeCommandComplexManual.p5, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			if (ImGui.Button(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5AC21A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a)), new Vector2(-1f, 0f)))
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplex((ExecuteCommandComplexFlag)executeCommandComplexManual.commmand, executeCommandComplexManual.p1, executeCommandComplexManual.p2, executeCommandComplexManual.p3, executeCommandComplexManual.p4, executeCommandComplexManual.p5);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void aeV9684M5D()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x370A278 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e)), true);
		try
		{
			Vfb9223YUY(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2828BCB9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B30B8AA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117), ref executeCommandComplexLocationManual.commmand, ExecuteCommandComplexOptions, ExecuteCommandComplexLookup, ref executeCommandLocationSearchInput);
			ImGui.InputFloat3(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E38A1B7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997)), ref executeCommandComplexLocationManual.location, 1f, 1f, ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-864210836 ^ -487981735 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75)), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F71A0D3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0)), ref executeCommandComplexLocationManual.p1, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A9FE9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1)), ref executeCommandComplexLocationManual.p2, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x61169092 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402)), ref executeCommandComplexLocationManual.p3, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			ImGui.InputUInt(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5133E44A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b)), ref executeCommandComplexLocationManual.p4, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0);
			if (ImGui.Button(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDCA691 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1)), new Vector2(-1f, 0f)))
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation((ExecuteCommandComplexFlag)executeCommandComplexLocationManual.commmand, executeCommandComplexLocationManual.location, executeCommandComplexLocationManual.p1, executeCommandComplexLocationManual.p2, executeCommandComplexLocationManual.p3, executeCommandComplexLocationManual.p4);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static W8GN8eKFNOv4lMETvrF[] fVE9kkXrv6<TEnum>() where TEnum : struct, Enum
	{
		return (from option in (from value in Enum.GetValues<TEnum>()
				select (Value: Convert.ToUInt32(value), Name: value.ToString()) into entry
				group entry by entry.Value).Select(delegate(IGrouping<uint, (uint Value, string Name)> @group)
			{
				string value = string.Join(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D34DF52 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33), @group.Select(((uint Value, string Name) entry) => entry.Name).Distinct<string>(StringComparer.Ordinal));
				uint key = @group.Key;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendFormatted(@group.Key);
				defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D150993 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
				defaultInterpolatedStringHandler.AppendFormatted(value);
				string displayText = defaultInterpolatedStringHandler.ToStringAndClear();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(1, 2);
				defaultInterpolatedStringHandler2.AppendFormatted(@group.Key);
				defaultInterpolatedStringHandler2.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x396185E8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
				defaultInterpolatedStringHandler2.AppendFormatted(value);
				return new W8GN8eKFNOv4lMETvrF(key, displayText, defaultInterpolatedStringHandler2.ToStringAndClear());
			})
			orderby option.Value
			select option).ToArray();
	}

	private static void Vfb9223YUY(string inputLabel, string comboLabel, ref uint command, W8GN8eKFNOv4lMETvrF[] options, FrozenDictionary<uint, W8GN8eKFNOv4lMETvrF> lookup, ref string searchInput)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(-1f);
		ImU8String val = ImU8String.op_Implicit(inputLabel);
		ImU8String val2 = default(ImU8String);
		ImGui.InputUInt(val, ref command, 0u, 0u, val2, (ImGuiInputTextFlags)0);
		ImGui.SetNextItemWidth(-1f);
		ComboDisposable val3 = ImRaii.Combo(ImU8String.op_Implicit(comboLabel), ImU8String.op_Implicit(Fxm9ZXcqhE(command, lookup)), (ImGuiComboFlags)8);
		try
		{
			if (ComboDisposable.op_Implicit(val3))
			{
				ImGui.SetNextItemWidth(-1f);
				ImGui.InputTextWithHint(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3922EB82 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2)), ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA77D2D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff)), ref searchInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				ImGui.Separator();
				for (int i = 0; i < options.Length; i++)
				{
					W8GN8eKFNOv4lMETvrF w8GN8eKFNOv4lMETvrF = options[i];
					if (string.IsNullOrWhiteSpace(searchInput) || w8GN8eKFNOv4lMETvrF.SearchText.Contains(searchInput, StringComparison.OrdinalIgnoreCase))
					{
						bool flag = w8GN8eKFNOv4lMETvrF.Value == command;
						if (ImGui.Selectable(ImU8String.op_Implicit(w8GN8eKFNOv4lMETvrF.DisplayText), flag, (ImGuiSelectableFlags)0, default(Vector2)))
						{
							command = w8GN8eKFNOv4lMETvrF.Value;
						}
						if (flag)
						{
							ImGui.SetItemDefaultFocus();
						}
					}
				}
			}
			if (lookup.TryGetValue(command, out var value))
			{
				val2 = new ImU8String(8, 1);
				((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC11458 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
				((ImU8String)(ref val2)).AppendFormatted<string>(value.DisplayText);
				ImGui.TextDisabled(val2);
				return;
			}
		}
		finally
		{
			((ComboDisposable)(ref val3)).Dispose();
		}
		ImU8String val4 = default(ImU8String);
		((ImU8String)(ref val4))._002Ector(15, 1);
		((ImU8String)(ref val4)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1470179789 ^ 0x7F8994EA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
		((ImU8String)(ref val4)).AppendFormatted<uint>(command);
		((ImU8String)(ref val4)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329CB18E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
		ImGui.TextDisabled(val4);
	}

	private static string Fxm9ZXcqhE(uint command, FrozenDictionary<uint, W8GN8eKFNOv4lMETvrF> lookup)
	{
		if (!lookup.TryGetValue(command, out var value))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1976305562 ^ 0x196B7A1D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
			defaultInterpolatedStringHandler.AppendFormatted(command);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6D0B04 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return value.DisplayText;
	}

	public e11xfl9QOeI6fuSoQKQ()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		executeCommandSearchInput = string.Empty;
		executeCommandComplexSearchInput = string.Empty;
		executeCommandLocationSearchInput = string.Empty;
		base._002Ector();
	}

	static e11xfl9QOeI6fuSoQKQ()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		ExecuteCommandOptions = fVE9kkXrv6<ExecuteCommandFlag>();
		ExecuteCommandLookup = ExecuteCommandOptions.ToFrozenDictionary((W8GN8eKFNOv4lMETvrF option) => option.Value);
		ExecuteCommandComplexOptions = fVE9kkXrv6<ExecuteCommandComplexFlag>();
		ExecuteCommandComplexLookup = ExecuteCommandComplexOptions.ToFrozenDictionary((W8GN8eKFNOv4lMETvrF option) => option.Value);
	}
}
