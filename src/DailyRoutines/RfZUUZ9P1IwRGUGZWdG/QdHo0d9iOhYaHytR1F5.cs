using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Extensions;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility.Raii;
using GO3ln7f4pExIcYCK2q3;
using OmenTools.Global;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using P67IJbfls2Hy3Xtb45R;
using itAnDoW4mgYVlrmtMor;

namespace RfZUUZ9P1IwRGUGZWdG;

internal sealed class QdHo0d9iOhYaHytR1F5 : ExINkNfv21TTK3yJ2Cx
{
	private readonly struct mDojuMKhjVwRLPWcJkg : IEquatable<mDojuMKhjVwRLPWcJkg>
	{
		public string Command { get; init; }

		public string FullCommand { get; init; }

		public string HelpMessage { get; init; }

		public bool ShowInHelp { get; init; }

		public string DisplayOrderText { get; init; }

		public string HandlerTypeName { get; init; }

		public string HandlerMethodName { get; init; }

		public string SearchText { get; init; }

		public mDojuMKhjVwRLPWcJkg(string Command, string FullCommand, string HelpMessage, bool ShowInHelp, string DisplayOrderText, string HandlerTypeName, string HandlerMethodName, string SearchText)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.Command = Command;
			this.FullCommand = FullCommand;
			this.HelpMessage = HelpMessage;
			this.ShowInHelp = ShowInHelp;
			this.DisplayOrderText = DisplayOrderText;
			this.HandlerTypeName = HandlerTypeName;
			this.HandlerMethodName = HandlerMethodName;
			this.SearchText = SearchText;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816EC96 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B31E0F4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
			if (DHsKohe9iu(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool DHsKohe9iu(StringBuilder builder)
		{
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F71716D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0));
			builder.Append((object?)Command);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7D9802 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
			builder.Append((object?)FullCommand);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE36C7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
			builder.Append((object?)HelpMessage);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53682CA0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94));
			builder.Append(ShowInHelp.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFBC973 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
			builder.Append((object?)DisplayOrderText);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B300774 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
			builder.Append((object?)HandlerTypeName);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A7D64 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
			builder.Append((object?)HandlerMethodName);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B300718 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
			builder.Append((object?)SearchText);
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(mDojuMKhjVwRLPWcJkg left, mDojuMKhjVwRLPWcJkg right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(mDojuMKhjVwRLPWcJkg left, mDojuMKhjVwRLPWcJkg right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((((EqualityComparer<string>.Default.GetHashCode(Command) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FullCommand)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HelpMessage)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(ShowInHelp)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayOrderText)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HandlerTypeName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(HandlerMethodName)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SearchText);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is mDojuMKhjVwRLPWcJkg)
			{
				return Equals((mDojuMKhjVwRLPWcJkg)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(mDojuMKhjVwRLPWcJkg other)
		{
			if (EqualityComparer<string>.Default.Equals(Command, other.Command) && EqualityComparer<string>.Default.Equals(FullCommand, other.FullCommand) && EqualityComparer<string>.Default.Equals(HelpMessage, other.HelpMessage) && EqualityComparer<bool>.Default.Equals(ShowInHelp, other.ShowInHelp) && EqualityComparer<string>.Default.Equals(DisplayOrderText, other.DisplayOrderText) && EqualityComparer<string>.Default.Equals(HandlerTypeName, other.HandlerTypeName) && EqualityComparer<string>.Default.Equals(HandlerMethodName, other.HandlerMethodName))
			{
				return EqualityComparer<string>.Default.Equals(SearchText, other.SearchText);
			}
			return false;
		}

		[CompilerGenerated]
		public void RPPKgZ7HlO(out string Command, out string FullCommand, out string HelpMessage, out bool ShowInHelp, out string DisplayOrderText, out string HandlerTypeName, out string HandlerMethodName, out string SearchText)
		{
			Command = this.Command;
			FullCommand = this.FullCommand;
			HelpMessage = this.HelpMessage;
			ShowInHelp = this.ShowInHelp;
			DisplayOrderText = this.DisplayOrderText;
			HandlerTypeName = this.HandlerTypeName;
			HandlerMethodName = this.HandlerMethodName;
			SearchText = this.SearchText;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public MainCommandInfo mainCommand;

		public _003C_003Ec__DisplayClass7_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool ye6KwSfmf9(KeyValuePair<string, CommandInfo> entry)
		{
			return !string.Equals(entry.Key, mainCommand?.Command, StringComparison.OrdinalIgnoreCase);
		}

		internal mDojuMKhjVwRLPWcJkg cuJKEXpYnk(KeyValuePair<string, CommandInfo> entry)
		{
			return slN9qyvhvF(entry.Key, entry.Value, mainCommand?.Command);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_0
	{
		public string normalizedSearchInput;

		public _003C_003Ec__DisplayClass8_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool VYnKbiFO28(mDojuMKhjVwRLPWcJkg row)
		{
			return gsy9FapgRY(row, normalizedSearchInput);
		}
	}

	private const ImGuiTableFlags TABLE_FLAGS = (ImGuiTableFlags)50358213;

	private string commandSearchInput;

	private string subCommandSearchInput;

	public override string SectionID => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E38A5CB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997);

	public override string Title => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDAF03D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1);

	protected override void DrawContent(VejoYhfM1yiPckJhcdm context)
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass7_0();
		CommandManager commandManager = OmenServiceBase<CommandManager>.Instance();
		CS_0024_003C_003E8__locals3.mainCommand = commandManager.MainCommand;
		mDojuMKhjVwRLPWcJkg[] array = (from entry in commandManager.Commands
			where !string.Equals(entry.Key, CS_0024_003C_003E8__locals3.mainCommand?.Command, StringComparison.OrdinalIgnoreCase)
			select slN9qyvhvF(entry.Key, entry.Value, null)).OrderBy<mDojuMKhjVwRLPWcJkg, string>((mDojuMKhjVwRLPWcJkg row) => row.Command, StringComparer.OrdinalIgnoreCase).ToArray();
		mDojuMKhjVwRLPWcJkg[] array2 = commandManager.SubCommands.Select<KeyValuePair<string, CommandInfo>, mDojuMKhjVwRLPWcJkg>((KeyValuePair<string, CommandInfo> entry) => slN9qyvhvF(entry.Key, entry.Value, CS_0024_003C_003E8__locals3.mainCommand?.Command)).OrderBy<mDojuMKhjVwRLPWcJkg, string>((mDojuMKhjVwRLPWcJkg row) => row.Command, StringComparer.OrdinalIgnoreCase).ToArray();
		TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -311655704 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc)));
		try
		{
			if (!val)
			{
				return;
			}
			ImU8String val2 = new ImU8String(5, 1);
			((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EE15B9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			((ImU8String)(ref val2)).AppendFormatted<int>(array.Length);
			((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3921E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
			TabItemDisposable val3 = ImRaii.TabItem(val2);
			try
			{
				if (TabItemDisposable.op_Implicit(val3))
				{
					baF9h07Ipg(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E94C985 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x370A714 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B21D88A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a), ref commandSearchInput, array, includeFullCommand: false);
				}
			}
			finally
			{
				((TabItemDisposable)(ref val3)).Dispose();
			}
			ImU8String val4 = new ImU8String(6, 1);
			((ImU8String)(ref val4)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C276B9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0));
			((ImU8String)(ref val4)).AppendFormatted<int>(array2.Length);
			((ImU8String)(ref val4)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA61327 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
			TabItemDisposable val5 = ImRaii.TabItem(val4);
			try
			{
				if (TabItemDisposable.op_Implicit(val5))
				{
					baF9h07Ipg(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EE15FB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1976305562 ^ 0x26A4918A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x558352FC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d), ref subCommandSearchInput, array2, includeFullCommand: true);
				}
			}
			finally
			{
				((TabItemDisposable)(ref val5)).Dispose();
			}
		}
		finally
		{
			((TabBarDisposable)(ref val)).Dispose();
		}
	}

	private static void baF9h07Ipg(string title, string hint, string emptyText, ref string searchInput, IReadOnlyList<mDojuMKhjVwRLPWcJkg> rows, bool includeFullCommand)
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass8_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass8_0();
		g4R9oFI26X(hint, ref searchInput, rows.Count);
		ImGui.Spacing();
		CS_0024_003C_003E8__locals3.normalizedSearchInput = searchInput.Trim();
		mDojuMKhjVwRLPWcJkg[] array = rows.Where((mDojuMKhjVwRLPWcJkg row) => gsy9FapgRY(row, CS_0024_003C_003E8__locals3.normalizedSearchInput)).ToArray();
		if (array.Length == 0)
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(string.IsNullOrWhiteSpace(CS_0024_003C_003E8__locals3.normalizedSearchInput) ? emptyText : (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE8FC3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76) + title)));
			return;
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(5, 1);
		((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A90B13 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a));
		((ImU8String)(ref val)).AppendFormatted<string>(title);
		TableDisposable val2 = ImRaii.Table(val, 6, (ImGuiTableFlags)50358213);
		try
		{
			if (!(!val2))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit(includeFullCommand ? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D2F990 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1) : GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--367535148 ^ 0x7ED89940 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117)), (ImGuiTableColumnFlags)8, 160f * Globals.GlobalUIScale, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC113F2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83)), (ImGuiTableColumnFlags)4, 2.4f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x370A4E4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e)), (ImGuiTableColumnFlags)8, 110f * Globals.GlobalUIScale, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4629BB07 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7)), (ImGuiTableColumnFlags)8, 90f * Globals.GlobalUIScale, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C79B05A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00)), (ImGuiTableColumnFlags)4, 1.8f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BA6DA5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503)), (ImGuiTableColumnFlags)8, 180f * Globals.GlobalUIScale, 0u);
				ImGui.TableHeadersRow();
				mDojuMKhjVwRLPWcJkg[] array2 = array;
				for (int num = 0; num < array2.Length; num++)
				{
					mDojuMKhjVwRLPWcJkg mDojuMKhjVwRLPWcJkg2 = array2[num];
					ImGui.TableNextRow();
					Chd9g8RKse(mDojuMKhjVwRLPWcJkg2.Command);
					gyx9wDbSrk(mDojuMKhjVwRLPWcJkg2.HelpMessage);
					COE9EASY70(mDojuMKhjVwRLPWcJkg2.ShowInHelp);
					SEl9bahxCY(mDojuMKhjVwRLPWcJkg2.DisplayOrderText);
					SEl9bahxCY(mDojuMKhjVwRLPWcJkg2.HandlerTypeName);
					SEl9bahxCY(mDojuMKhjVwRLPWcJkg2.HandlerMethodName);
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
	}

	private static void g4R9oFI26X(string hint, ref string searchInput, int totalCount)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		float x = ImGui.GetContentRegionAvail().X;
		ImGui.SetNextItemWidth(MathF.Max(220f * Globals.GlobalUIScale, x - 150f * Globals.GlobalUIScale));
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(2, 1);
		((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E38A62B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
		((ImU8String)(ref val)).AppendFormatted<string>(hint);
		ImGui.InputTextWithHint(val, ImU8String.op_Implicit(hint), ref searchInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(4, 1);
		((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200F593D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));
		((ImU8String)(ref val2)).AppendFormatted<int>(totalCount);
		((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -2004130993 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1));
		ImGui.TextDisabled(val2);
	}

	private static void Chd9g8RKse(string? value)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		if (string.IsNullOrWhiteSpace(value))
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--367535148 ^ 0x60063F41 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b)));
			return;
		}
		ImGui.TextUnformatted(ImU8String.op_Implicit(value));
		ImGuiOmExtension.ClickToCopyAndNotify(value);
	}

	private static void gyx9wDbSrk(string? value)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		if (string.IsNullOrWhiteSpace(value))
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDCACD5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1)));
		}
		else
		{
			ImGui.TextWrapped(ImU8String.op_Implicit(value));
		}
	}

	private static void COE9EASY70(bool value)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		if (value)
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B21DBDE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a)));
		}
		else
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5AC43E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a)));
		}
	}

	private static void SEl9bahxCY(string? value)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		if (string.IsNullOrWhiteSpace(value))
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F1840B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)));
		}
		else
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(value));
		}
	}

	private static bool gsy9FapgRY(mDojuMKhjVwRLPWcJkg row, string searchInput)
	{
		if (!string.IsNullOrWhiteSpace(searchInput))
		{
			return row.SearchText.Contains(searchInput, StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static mDojuMKhjVwRLPWcJkg slN9qyvhvF(string command, CommandInfo commandInfo, string? fullCommand)
	{
		HandlerDelegate handler = commandInfo.Handler;
		string text = ((Delegate)(object)handler).Target?.GetType().FullName ?? ((Delegate)(object)handler).Method.DeclaringType?.FullName ?? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--367535148 ^ 0x633348B7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2);
		string name = ((Delegate)(object)handler).Method.Name;
		string text2 = (string.IsNullOrWhiteSpace(fullCommand) ? command : (string.Equals(command, fullCommand, StringComparison.OrdinalIgnoreCase) ? fullCommand : (fullCommand + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x265C5DD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44) + command)));
		string text3 = commandInfo.HelpMessage ?? string.Empty;
		bool showInHelp = commandInfo.ShowInHelp;
		string displayOrderText = commandInfo.DisplayOrder.ToString();
		InlineArray5<string> buffer = default(InlineArray5<string>);
		buffer[0] = command;
		buffer[1] = text2;
		buffer[2] = text3;
		buffer[3] = text;
		buffer[4] = name;
		return new mDojuMKhjVwRLPWcJkg(command, text2, text3, showInHelp, displayOrderText, text, name, string.Join('\n', (ReadOnlySpan<string?>)buffer));
	}

	public QdHo0d9iOhYaHytR1F5()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		commandSearchInput = string.Empty;
		subCommandSearchInput = string.Empty;
		base._002Ector();
	}
}
