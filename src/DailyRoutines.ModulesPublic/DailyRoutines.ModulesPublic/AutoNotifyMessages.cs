using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Chat;
using Dalamud.Game.Text;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoNotifyMessages : ModuleBase
{
	private class Config : ModuleConfig
	{
		public HashSet<XivChatType> ValidChatTypes = new HashSet<XivChatType>();

		public List<string> ValidKeywords = new List<string>();
	}

	private Config config;

	private string searchChatTypesContent = string.Empty;

	private string keywordInput = string.Empty;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNotifyMessagesTitle"),
		Description = LanguageManager.Get("AutoNotifyMessagesDescription"),
		Category = ModuleCategory.Notification
	};

	protected override void Init()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IChatGui>().ChatMessage += new OnHandleableChatMessageDelegate(OnChatMessage);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IChatGui>().ChatMessage -= new OnHandleableChatMessageDelegate(OnChatMessage);
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		ItemWidthDisposable val = ImRaii.ItemWidth(300f * Globals.GlobalUIScale);
		try
		{
			ComboDisposable val2 = ImRaii.Combo(ImU8String.op_Implicit("###SelectChatTypesCombo"), ImU8String.op_Implicit(LanguageManager.Get("AutoNotifyMessages-SelectedTypesAmount", config.ValidChatTypes.Count)), (ImGuiComboFlags)8);
			try
			{
				if (ComboDisposable.op_Implicit(val2))
				{
					ImGui.SetNextItemWidth(-1f);
					ImU8String val3 = ImU8String.op_Implicit("###ChatTypeSelectInput");
					ImU8String val4 = new ImU8String(3, 1);
					((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("PleaseSearch"));
					((ImU8String)(ref val4)).AppendLiteral("...");
					ImGui.InputTextWithHint(val3, val4, ref searchChatTypesContent, 50, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					ImGui.Separator();
					ImGui.Spacing();
					foreach (var (val6, text2) in XIVChatTypes.ChatTypeToAddonText)
					{
						if (!string.IsNullOrEmpty(searchChatTypesContent) && !((object)(*(XivChatType*)(&val6))/*cast due to constrained. prefix*/).ToString().Contains(searchChatTypesContent, StringComparison.OrdinalIgnoreCase) && !text2.Contains(searchChatTypesContent, StringComparison.OrdinalIgnoreCase))
						{
							continue;
						}
						bool flag = config.ValidChatTypes.Contains(val6);
						ImU8String val7 = new ImU8String(2, 2);
						((ImU8String)(ref val7)).AppendFormatted<string>(text2);
						((ImU8String)(ref val7)).AppendLiteral("##");
						((ImU8String)(ref val7)).AppendFormatted<XivChatType>(val6);
						if (ImGui.Checkbox(val7, ref flag))
						{
							if (!config.ValidChatTypes.Remove(val6))
							{
								config.ValidChatTypes.Add(val6);
							}
							config.Save(this);
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val2)).Dispose();
			}
			ComboDisposable val8 = ImRaii.Combo(ImU8String.op_Implicit("###ExistedKeywordsCombo"), ImU8String.op_Implicit(LanguageManager.Get("AutoNotifyMessages-ExistedKeywords", config.ValidKeywords.Count)), (ImGuiComboFlags)8);
			try
			{
				if (!ComboDisposable.op_Implicit(val8))
				{
					return;
				}
				ImGui.AlignTextToFramePadding();
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImU8String val9 = new ImU8String(0, 1);
				((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get("Keyword"));
				ImGui.TextColored(ref vector, val9);
				ImGui.SameLine();
				if (ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get("Add"))) && !string.IsNullOrWhiteSpace(keywordInput) && !config.ValidKeywords.Contains(keywordInput))
				{
					config.ValidKeywords.Add(keywordInput);
					config.Save(this);
					keywordInput = string.Empty;
				}
				ImGui.SetNextItemWidth(-1f);
				ImGui.InputText(ImU8String.op_Implicit("###KeywordInput"), ref keywordInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (config.ValidKeywords.Count == 0)
				{
					return;
				}
				ImGui.Separator();
				ImGui.Spacing();
				string[] array = config.ValidKeywords.ToArray();
				foreach (string text3 in array)
				{
					IdDisposable val10 = ImRaii.PushId(ImU8String.op_Implicit(text3), true);
					try
					{
						ImGui.Selectable(ImU8String.op_Implicit(text3), false, (ImGuiSelectableFlags)0, default(Vector2));
						ImU8String val11 = new ImU8String(0, 1);
						((ImU8String)(ref val11)).AppendFormatted<string>(text3);
						PopupDisposable val12 = ImRaii.ContextPopupItem(val11);
						try
						{
							if (!(!val12) && ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("Delete")), false, true))
							{
								config.ValidKeywords.Remove(text3);
								config.Save(this);
							}
						}
						finally
						{
							((PopupDisposable)(ref val12)).Dispose();
						}
					}
					finally
					{
						((IDisposable)val10)?.Dispose();
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val8)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void OnChatMessage(IHandleableChatMessage message)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (XIVChatTypes.ChatTypeToAddonText.ContainsKey(((IChatMessage)message).LogKind) && (config.ValidChatTypes.Count != 0 || config.ValidKeywords.Count != 0))
		{
			string messageContent = ((object)((IMutableChatMessage)message).Message).ToString();
			bool num = config.ValidChatTypes.Count > 0 && config.ValidChatTypes.Contains(((IChatMessage)message).LogKind);
			bool flag = config.ValidKeywords.Count > 0 && config.ValidKeywords.FirstOrDefault((string x) => messageContent.Contains(x, StringComparison.OrdinalIgnoreCase)) != null;
			if (num || flag)
			{
				OmenServiceBase<NotifyHelper>.Instance().TrayInfo(LanguageManager.Get("AutoNotifyMessages-Notification", new Dictionary<string, object>
				{
					["player"] = ((IMutableChatMessage)message).Sender,
					["message"] = messageContent
				}));
			}
		}
	}
}
