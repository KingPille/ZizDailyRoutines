using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Windows.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace mbL4CiVEWd3wtCCTP72;

internal class n2hMWbVwHSf2rAJHZyL : Window, IDisposable
{
	private unsafe delegate void x03VPEO2qambORr2NSC(AtkComponentTextInput* textInput, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData);

	private record tVv0ZNOZAnSEj6gViAP
	{
		public string Command { get; init; }

		public string Description { get; init; }

		public tVv0ZNOZAnSEj6gViAP(string Command, string Description)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.Command = Command;
			this.Description = Description;
			base._002Ector();
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA247B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4118B31B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
			if (PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEC1F9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			builder.Append((object?)Command);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E3872AF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
			builder.Append((object?)Description);
			return true;
		}

		[CompilerGenerated]
		protected tVv0ZNOZAnSEj6gViAP(tVv0ZNOZAnSEj6gViAP original)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			Command = original.Command;
			Description = original.Description;
		}

		[CompilerGenerated]
		public void dvyOa7k0sf(out string Command, out string Description)
		{
			Command = this.Command;
			Description = this.Description;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass16_0
	{
		public HashSet<string> addedCommands;

		public n2hMWbVwHSf2rAJHZyL _003C_003E4__this;
	}

	private static readonly CompSig TextInputReceiveEventSig;

	private Hook<x03VPEO2qambORr2NSC>? TextInputReceiveEventHook;

	private readonly List<tVv0ZNOZAnSEj6gViAP> suggestions;

	private List<tVv0ZNOZAnSEj6gViAP> filteredSuggestions;

	private bool isNeedToCommandComplete;

	private string filterText;

	private float lastWindowHeight;

	private const ImGuiWindowFlags WINDOW_FLAGS = (ImGuiWindowFlags)528767;

	private static string MainCommand => OmenServiceBase<CommandManager>.Instance().MainCommand?.Command ?? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B57F70 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3);

	public unsafe n2hMWbVwHSf2rAJHZyL()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		suggestions = new List<tVv0ZNOZAnSEj6gViAP>();
		filteredSuggestions = new List<tVv0ZNOZAnSEj6gViAP>();
		filterText = string.Empty;
		lastWindowHeight = 100f;
		((Window)this)._002Ector(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2279C89 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44), (ImGuiWindowFlags)528767, false);
		tcGVqXt2n1();
		TextInputReceiveEventHook = TextInputReceiveEventSig.GetHook<x03VPEO2qambORr2NSC>(HhsVb5GIvU);
		TextInputReceiveEventHook.Enable();
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(fIsVFTZ0C2), 100u);
	}

	public void Dispose()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		filterText = string.Empty;
		suggestions.Clear();
		filteredSuggestions.Clear();
		OmenServiceBase<FrameworkManager>.Instance().Unreg(fIsVFTZ0C2);
		TextInputReceiveEventHook?.Dispose();
		TextInputReceiveEventHook = null;
	}

	public unsafe override void PreDraw()
	{
		if ((*Addons.ChatLog).IsAddonAndNodesReady())
		{
			Vector2 vector = new Vector2(((AtkUnitBase)Addons.ChatLog).X, ((AtkUnitBase)Addons.ChatLog).Y);
			Vector2 nextWindowPos = new Vector2(vector.X + 2f, vector.Y - lastWindowHeight);
			if (nextWindowPos.Y < 0f)
			{
				nextWindowPos.Y = 0f;
			}
			ImGui.SetNextWindowPos(nextWindowPos);
			ImGui.SetNextWindowSize((filteredSuggestions.Count == 0) ? Vector2.Zero : new Vector2(((AtkUnitBase)Addons.ChatLog).GetScaledWidth(true), ((AtkUnitBase)Addons.ChatLog).GetScaledHeight(true)));
		}
	}

	public override void Draw()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)1, Vector2.One * 6f, true);
			try
			{
				StyleDisposable val2 = ImRaii.PushStyle((ImGuiStyleVar)13, Vector2.One * 3f, true);
				try
				{
					StyleDisposable val3 = ImRaii.PushStyle((ImGuiStyleVar)3, 1f, true);
					try
					{
						ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)2, 3758096384u, true);
						try
						{
							ColorDisposable val5 = ImRaii.PushColor((ImGuiCol)5, 4282400832u, true);
							try
							{
								lastWindowHeight = ImGui.GetContentRegionMax().Y;
								ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1675397288 ^ -1467958122 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079))));
								ImGui.Spacing();
								ImGui.Separator();
								ImGui.Spacing();
								if (filteredSuggestions.Count == 0)
								{
									ColorDisposable val6 = ImRaii.PushColor((ImGuiCol)0, 4286611584u, true);
									try
									{
										ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6117A624 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402))));
										return;
									}
									finally
									{
										((IDisposable)val6)?.Dispose();
									}
								}
								for (int i = 0; i < filteredSuggestions.Count; i++)
								{
									tVv0ZNOZAnSEj6gViAP tVv0ZNOZAnSEj6gViAP2 = filteredSuggestions[i];
									string mainCommand = MainCommand;
									string text = (tVv0ZNOZAnSEj6gViAP2.Command.StartsWith(mainCommand) ? tVv0ZNOZAnSEj6gViAP2.Command : (mainCommand + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D77B320 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33) + tVv0ZNOZAnSEj6gViAP2.Command)) ?? "";
									string text2 = tVv0ZNOZAnSEj6gViAP2.Description ?? "";
									Vector2 cursorPos = ImGui.GetCursorPos();
									if (ImGui.GetCursorScreenPos().Y > ImGui.GetWindowPos().Y + ImGui.GetWindowSize().Y - 50f)
									{
										continue;
									}
									GroupDisposable val7 = ImRaii.Group();
									try
									{
										ColorDisposable val6 = ImRaii.PushColor((ImGuiCol)0, KnownColor.LawnGreen.ToVector4(), true);
										try
										{
											ImGui.TextUnformatted(ImU8String.op_Implicit(text));
										}
										finally
										{
											((IDisposable)val6)?.Dispose();
										}
										IndentDisposable val8 = ImRaii.PushIndent(1, true);
										try
										{
											val6 = ImRaii.PushColor((ImGuiCol)0, KnownColor.Chocolate.ToVector4(), true);
											try
											{
												ImGui.TextWrapped(ImU8String.op_Implicit(text2));
											}
											finally
											{
												((IDisposable)val6)?.Dispose();
											}
										}
										finally
										{
											((IDisposable)val8)?.Dispose();
										}
									}
									finally
									{
										((GroupDisposable)(ref val7)).Dispose();
									}
									Vector2 itemRectSize = ImGui.GetItemRectSize();
									ImGui.SetCursorPos(cursorPos);
									ImU8String val9 = new ImU8String(9, 1);
									((ImU8String)(ref val9)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B20EA52 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a));
									((ImU8String)(ref val9)).AppendFormatted<int>(i);
									ImU8String val10 = val9;
									Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
									contentRegionAvail.Y = itemRectSize.Y + 4f;
									ImGui.InvisibleButton(val10, contentRegionAvail, (ImGuiButtonFlags)0);
									if (ImGui.IsItemClicked((ImGuiMouseButton)0))
									{
										XF7VANWp5q(tVv0ZNOZAnSEj6gViAP2.Command, justSend: false);
									}
									if (ImGui.IsItemClicked((ImGuiMouseButton)1))
									{
										XF7VANWp5q(tVv0ZNOZAnSEj6gViAP2.Command, justSend: true);
									}
									if (ImGui.IsItemHovered())
									{
										ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
										Vector2 itemRectMin = ImGui.GetItemRectMin();
										Vector2 itemRectMax = ImGui.GetItemRectMax();
										((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(itemRectMin, itemRectMax, 1090519039u);
									}
									ImGui.Separator();
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

	private unsafe void HhsVb5GIvU(AtkComponentTextInput* textInput, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Invalid comparison between Unknown and I4
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		TextInputReceiveEventHook.Original(textInput, eventType, eventParam, atkEvent, atkEventData);
		if (textInput == null)
		{
			return;
		}
		AtkUnitBase* ptr = ((AtkComponentTextInput)textInput).OwnerAddon;
		if (ptr == null)
		{
			ptr = ((AtkComponentTextInput)textInput).ContainingAddon2;
		}
		if (ptr == null)
		{
			ptr = ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonByNode((AtkResNode*)((AtkComponentTextInput)textInput).OwnerNode);
		}
		if (((AtkUnitBase)ptr).NameString != GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D35EB38 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33))
		{
			return;
		}
		if ((int)eventType != 18)
		{
			if ((int)eventType == 19)
			{
				isNeedToCommandComplete = false;
				((Window)this).IsOpen = false;
			}
		}
		else
		{
			isNeedToCommandComplete = true;
		}
	}

	private unsafe void fIsVFTZ0C2(IFramework framework)
	{
		if (!isNeedToCommandComplete)
		{
			return;
		}
		if (!AtkComponentExtension.TryGetActive(out var component, out var addon) || ((AtkUnitBase)addon).NameString != GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C34473 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0))
		{
			if (((Window)this).IsOpen)
			{
				((Window)this).IsOpen = false;
			}
			return;
		}
		string text = ((object)(*(Utf8String*)(&((AtkComponentTextInput)component).EvaluatedString))/*cast due to constrained. prefix*/).ToString();
		string mainCommand = MainCommand;
		if (text.StartsWith(mainCommand, StringComparison.OrdinalIgnoreCase))
		{
			if (text.Length >= mainCommand.Length)
			{
				TLiVcBiXqH(text);
				((Window)this).IsOpen = true;
			}
		}
		else if (((Window)this).IsOpen)
		{
			((Window)this).IsOpen = false;
		}
	}

	private void tcGVqXt2n1()
	{
		_003C_003Ec__DisplayClass16_0 _003C_003Ec__DisplayClass16_1 = default(_003C_003Ec__DisplayClass16_0);
		_003C_003Ec__DisplayClass16_1._003C_003E4__this = this;
		suggestions.Clear();
		_003C_003Ec__DisplayClass16_1.addedCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (KeyValuePair<string, CommandInfo> command in OmenServiceBase<CommandManager>.Instance().Commands)
		{
			if (!(command.Key == MainCommand))
			{
				dxyVQJswlZ(command.Key, command.Value.HelpMessage, ref _003C_003Ec__DisplayClass16_1);
			}
		}
		foreach (KeyValuePair<string, CommandInfo> subCommand in OmenServiceBase<CommandManager>.Instance().SubCommands)
		{
			dxyVQJswlZ(subCommand.Key, subCommand.Value.HelpMessage, ref _003C_003Ec__DisplayClass16_1);
		}
	}

	private void TLiVcBiXqH(string input)
	{
		string mainCommand = MainCommand;
		if (input.StartsWith(mainCommand + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F76268A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862), StringComparison.OrdinalIgnoreCase))
		{
			filterText = input.Substring(mainCommand.Length + 1).Trim();
		}
		else if (input.StartsWith(mainCommand, StringComparison.OrdinalIgnoreCase))
		{
			filterText = input.Substring(mainCommand.Length).Trim();
		}
		else
		{
			filterText = string.Empty;
		}
		tcGVqXt2n1();
		filteredSuggestions = (from s in suggestions
			where string.IsNullOrEmpty(filterText) || s.Command.Contains(filterText, StringComparison.OrdinalIgnoreCase) || s.Description.Contains(filterText, StringComparison.OrdinalIgnoreCase)
			orderby (!s.Command.StartsWith(filterText, StringComparison.OrdinalIgnoreCase)) ? 1 : 0, s.Command.Length, s.Command
			select s).Take(12).ToList();
	}

	private unsafe void XF7VANWp5q(string command, bool justSend)
	{
		try
		{
			if (!AtkComponentExtension.TryGetActive(out var component, out var addon) || ((AtkUnitBase)addon).NameString != GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D35EB38 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33))
			{
				return;
			}
			string mainCommand = MainCommand;
			string text = (string.IsNullOrEmpty(command) ? mainCommand : (command.StartsWith(mainCommand) ? command : (mainCommand + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x33DF1F36 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de) + command)));
			string text2 = ((object)(*(Utf8String*)(&((AtkTextNode)((AtkComponentTextInput)component).AtkTextNode).NodeText))/*cast due to constrained. prefix*/).ToString();
			((AtkComponentTextInput)component).SetText(string.Empty);
			((Utf8String)(&((AtkComponentTextInput)component).EvaluatedString)).Clear();
			((Utf8String)(&((AtkComponentTextInput)component).RawString)).Clear();
			((Utf8String)(&((AtkComponentTextInput)component).AvailableLines)).Clear();
			((Utf8String)(&((AtkComponentTextInput)component).HighlightedAutoTranslateOptionColorPrefix)).Clear();
			((Utf8String)(&((AtkComponentTextInput)component).HighlightedAutoTranslateOptionColorSuffix)).Clear();
			((Utf8String)(&((AtkTextNode)((AtkComponentTextInput)component).AtkTextNode).NodeText)).Clear();
			((AtkStage)AtkStage.Instance()).ClearFocus();
			if (text == text2 || justSend)
			{
				((Utf8String)(&((AtkTextNode)((AtkComponentTextInput)component).AtkTextNode).NodeText)).Clear();
				OmenServiceBase<ChatManager>.Instance().SendMessage(text, saveToHistory: true);
			}
			else
			{
				((AtkComponentTextInput)component).SetText(text);
				KeyEmulationHelper.SendKeypress(Keys.Return);
			}
		}
		catch
		{
		}
		((Window)this).IsOpen = false;
	}

	public override bool DrawConditions()
	{
		if (((Window)this).IsOpen)
		{
			return filteredSuggestions.Count >= 0;
		}
		return false;
	}

	static n2hMWbVwHSf2rAJHZyL()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TextInputReceiveEventSig = new CompSig(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4118DC3F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
	}

	[CompilerGenerated]
	private void dxyVQJswlZ(string command, string description, ref _003C_003Ec__DisplayClass16_0 P_2)
	{
		if (P_2.addedCommands.Add(command) && !string.IsNullOrWhiteSpace(description))
		{
			suggestions.Add(new tVv0ZNOZAnSEj6gViAP(command, description));
		}
	}

	[CompilerGenerated]
	private bool lrAVJbZ8Iv(tVv0ZNOZAnSEj6gViAP s)
	{
		if (!string.IsNullOrEmpty(filterText) && !s.Command.Contains(filterText, StringComparison.OrdinalIgnoreCase))
		{
			return s.Description.Contains(filterText, StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	[CompilerGenerated]
	private int ubmVrQUHcR(tVv0ZNOZAnSEj6gViAP s)
	{
		return (!s.Command.StartsWith(filterText, StringComparison.OrdinalIgnoreCase)) ? 1 : 0;
	}
}
