using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class BetterCommandInput : ModuleBase
{
	public class Config : ModuleConfig
	{
		public bool IsAvoidingSpace = true;

		public List<string> Whitelist = new List<string>();
	}

	private Config config;

	private DateTime lastChatTime = DateTime.MinValue;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterCommandInputTitle"),
		Description = LanguageManager.Get("BetterCommandInputDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<ChatManager>.Instance().RegPreExecuteCommandInner(OnPreExecuteCommandInner);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ChatManager>.Instance().Unreg(OnPreExecuteCommandInner);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("BetterCommandInput-DeleteSpaceBeforeCommand")), ref config.IsAvoidingSpace))
		{
			config.Save(this);
		}
		ImGui.Spacing();
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Whitelist")));
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")))
		{
			config.Whitelist.Add(string.Empty);
			config.Save(this);
		}
		ImGui.Spacing();
		ImU8String val = default(ImU8String);
		for (int i = 0; i < config.Whitelist.Count; i++)
		{
			string text = config.Whitelist[i];
			string value = text;
			((ImU8String)(ref val))._002Ector(9, 2);
			((ImU8String)(ref val)).AppendFormatted<string>(text);
			((ImU8String)(ref val)).AppendLiteral("_");
			((ImU8String)(ref val)).AppendFormatted<int>(i);
			((ImU8String)(ref val)).AppendLiteral("_Command");
			IdDisposable val2 = ImRaii.PushId(val, true);
			try
			{
				ImGui.AlignTextToFramePadding();
				ImU8String val3 = new ImU8String(11, 2);
				((ImU8String)(ref val3)).AppendLiteral("###Command");
				((ImU8String)(ref val3)).AppendFormatted<string>(text);
				((ImU8String)(ref val3)).AppendLiteral("-");
				((ImU8String)(ref val3)).AppendFormatted<int>(i);
				ImGui.InputText(val3, ref value, 48, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Whitelist[i] = value;
					config.Save(this);
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon("Delete", (FontAwesomeIcon)62189, LanguageManager.Get("Delete") ?? ""))
				{
					config.Whitelist.Remove(text);
					config.Save(this);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
	}

	private void OnPreExecuteCommandInner(ref bool isPrevented, ref ReadOnlySeString message)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)Unsafe.As<ReadOnlySeString, ReadOnlySeString>(ref message)/*cast due to constrained. prefix*/).ToString();
		bool flag = CommandRegex().IsMatch(text);
		bool flag2 = text.StartsWith('/') || text.StartsWith('／');
		bool flag3 = (config.IsAvoidingSpace ? flag : flag2);
		if (!string.IsNullOrWhiteSpace(text) && flag3 && HandleSlashCommand(text, out string handledMessage))
		{
			message = new ReadOnlySeString(handledMessage);
		}
	}

	private bool HandleSlashCommand(string command, out string handledMessage)
	{
		handledMessage = string.Empty;
		if (!IsValid(command.AsSpan()))
		{
			return false;
		}
		if (config.IsAvoidingSpace)
		{
			command = command.TrimStart(new char[2] { ' ', '\u3000' });
		}
		int num = command.IndexOf(' ');
		if (num == -1)
		{
			string text = command.ToLowerAndHalfWidth();
			foreach (string item in config.Whitelist)
			{
				if (text.Equals(item, StringComparison.CurrentCultureIgnoreCase))
				{
					text = item;
				}
			}
			handledMessage = text;
		}
		else
		{
			string text2 = command.Substring(0, num).ToLowerAndHalfWidth();
			foreach (string item2 in config.Whitelist)
			{
				if (text2.Equals(item2, StringComparison.CurrentCultureIgnoreCase))
				{
					text2 = item2;
				}
			}
			handledMessage = text2 + command.Substring(num);
		}
		lastChatTime = OmenServiceBase<StandardTimeManager>.Instance().Now;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool IsValid(ReadOnlySpan<char> chars)
	{
		if ((OmenServiceBase<StandardTimeManager>.Instance().Now - lastChatTime).TotalMilliseconds >= 500.0)
		{
			if (!ContainsUppercase(chars) && !ContainsFullWidth(chars))
			{
				return ContainsSpace(chars);
			}
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool ContainsUppercase(ReadOnlySpan<char> chars)
	{
		ReadOnlySpan<char> readOnlySpan = chars;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			if (char.IsUpper(readOnlySpan.get_Item(i)))
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool ContainsSpace(ReadOnlySpan<char> chars)
	{
		ReadOnlySpan<char> readOnlySpan = chars;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = readOnlySpan.get_Item(i);
			if ((c == ' ' || c == '\u3000') ? true : false)
			{
				return true;
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool ContainsFullWidth(ReadOnlySpan<char> chars)
	{
		ReadOnlySpan<char> readOnlySpan = chars;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			if (readOnlySpan.get_Item(i).IsFullWidth())
			{
				return true;
			}
		}
		return false;
	}

	[GeneratedRegex("^[ \u3000]*[/／]")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex CommandRegex()
	{
		return _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__CommandRegex_5.Instance;
	}
}
