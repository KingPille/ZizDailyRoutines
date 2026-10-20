using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoAddChatPrefixSuffix : ModuleBase
{
	private class Config : ModuleConfig
	{
		public HashSet<string> Blacklist = new HashSet<string>();

		public bool IsAddPrefix;

		public bool IsAddSuffix;

		public string PrefixString = string.Empty;

		public string SuffixString = string.Empty;
	}

	private Config? config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoAddChatPrefixSuffixTitle"),
		Description = LanguageManager.Get("AutoAddChatPrefixSuffixDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "那年雪落" }
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config
		{
			Blacklist = ((!GameState.IsCN) ? new HashSet<string>() : new HashSet<string>
			{
				".", "。", "？", "?", "！", "!", "吗", "吧", "呢", "啊",
				"呗", "呀", "阿", "哦", "嘛", "咯", "哎", "啦", "哇", "呵",
				"哈", "奥", "嗷"
			})
		};
		OmenServiceBase<ChatManager>.Instance().RegPreExecuteCommandInner(OnPreExecuteCommandInner);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ChatManager>.Instance().Unreg(OnPreExecuteCommandInner);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("Prefix")), ref config.IsAddPrefix))
		{
			config.Save(this);
		}
		if (config.IsAddPrefix)
		{
			ImGui.SameLine();
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			ImGui.InputText(ImU8String.op_Implicit("###Prefix"), ref config.PrefixString, 48, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("Suffix")), ref config.IsAddSuffix))
		{
			config.Save(this);
		}
		if (config.IsAddSuffix)
		{
			ImGui.SameLine();
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			ImGui.InputText(ImU8String.op_Implicit("###Suffix"), ref config.SuffixString, 48, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
		}
		ImGui.Spacing();
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Blacklist")));
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")))
		{
			config.Blacklist.Add(string.Empty);
			config.Save(this);
		}
		ImGui.Spacing();
		if (config.Blacklist.Count == 0)
		{
			return;
		}
		List<string> list = config.Blacklist.ToList();
		Vector2 vector2 = ImGui.GetContentRegionAvail() * 0.85f;
		vector2.Y = 0f;
		Vector2 vector3 = vector2;
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LanguageManager.Get("Blacklist")), 5, (ImGuiTableFlags)2048, vector3);
		try
		{
			if (!val)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (i % 5 == 0)
				{
					ImGui.TableNextRow();
				}
				ImGui.TableNextColumn();
				string text = list[i];
				ImU8String val2 = new ImU8String(9, 2);
				((ImU8String)(ref val2)).AppendFormatted<string>(text);
				((ImU8String)(ref val2)).AppendLiteral("_");
				((ImU8String)(ref val2)).AppendFormatted<int>(i);
				((ImU8String)(ref val2)).AppendLiteral("_Command");
				IdDisposable val3 = ImRaii.PushId(val2, true);
				try
				{
					ImU8String val4 = new ImU8String(6, 1);
					((ImU8String)(ref val4)).AppendLiteral("##Item");
					((ImU8String)(ref val4)).AppendFormatted<int>(i);
					ImGui.InputText(val4, ref text, 48, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.Blacklist.Remove(list[i]);
						config.Blacklist.Add(text);
						config.Save(this);
						list[i] = text;
					}
					ImGui.SameLine();
					if (ImGuiOm.ButtonIcon("Delete", (FontAwesomeIcon)62189, LanguageManager.Get("Delete")))
					{
						config.Blacklist.Remove(list[i]);
						config.Save(this);
						list.RemoveAt(i);
						i--;
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
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void OnPreExecuteCommandInner(ref bool isPrevented, ref ReadOnlySeString message)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)Unsafe.As<ReadOnlySeString, ReadOnlySeString>(ref message)/*cast due to constrained. prefix*/).ToString();
		bool flag = text.StartsWith('/') || text.StartsWith('／');
		bool flag2 = flag && text.StartsWith("/tell ");
		if (((!string.IsNullOrWhiteSpace(text) && !flag) || flag2) && !IsBlackListChat(text) && !IsGameItemChat(text) && AddPrefixAndSuffixIfNeeded(text, out string handledMessage, flag2))
		{
			message = new ReadOnlySeString(handledMessage);
		}
	}

	private bool IsBlackListChat(string message)
	{
		return config?.Blacklist.Any((string blackListChat) => !string.IsNullOrEmpty(blackListChat) && message.EndsWith(blackListChat)) ?? false;
	}

	private static bool IsGameItemChat(string message)
	{
		if (!message.Contains("<item>") && !message.Contains("<flag>"))
		{
			return message.Contains("<pfinder>");
		}
		return true;
	}

	private bool AddPrefixAndSuffixIfNeeded(string original, out string handledMessage, bool isTellCommand = false)
	{
		handledMessage = original;
		if (config.IsAddPrefix)
		{
			if (isTellCommand)
			{
				int num = original.IndexOf(' ');
				if (num == -1)
				{
					return false;
				}
				int num2 = original.IndexOf(' ', num + 1);
				if (num2 == -1)
				{
					return false;
				}
				handledMessage = original.Substring(0, num2) + " " + config.PrefixString + original.Substring(num2).TrimStart();
			}
			else
			{
				handledMessage = config.PrefixString + handledMessage;
			}
		}
		if (config.IsAddSuffix)
		{
			handledMessage += config.SuffixString;
		}
		return true;
	}
}
