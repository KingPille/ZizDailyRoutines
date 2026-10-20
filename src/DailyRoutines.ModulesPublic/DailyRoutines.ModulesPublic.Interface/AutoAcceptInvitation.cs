using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoAcceptInvitation : ModuleBase
{
	private class Config : ModuleConfig
	{
		public HashSet<string> Blacklist = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		public bool Mode = true;

		public HashSet<string> Whitelist = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
	}

	private Config config;

	private string playerNameInput = string.Empty;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoAcceptInvitationTitle"),
		Description = LanguageManager.Get("AutoAcceptInvitationDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Fragile" }
	};

	private static string Pattern { get; }

	protected override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "SelectYesno", new AddonEventDelegate(OnSelectYesno));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnSelectYesno });
	}

	protected override void ConfigUI()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Mode"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextUnformatted(val);
		ImGui.SameLine();
		uint colorU = ImGui.GetColorU32((ImGuiCol)7);
		ImGui.SameLine();
		if (ImGuiOm.ToggleButton("WorkMode", ref config.Mode, 0f, colorU, colorU))
		{
			config.Save(this);
		}
		ImGui.SameLine();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(config.Mode ? "Whitelist" : "Blacklist")));
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(1, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetAddonText(9818u));
		((ImU8String)(ref val2)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val2);
		IndentDisposable val3 = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			ImGui.InputText(ImU8String.op_Implicit("##NewPlayerInput"), ref playerNameInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			ImGuiOm.TooltipHover(LanguageManager.Get("AutoAcceptInvitationTitle-PlayerNameInputHelp"));
			ImGui.SameLine();
			DisabledDisposable val4 = ImRaii.Disabled(string.IsNullOrWhiteSpace(playerNameInput) || (config.Mode ? config.Whitelist : config.Blacklist).Contains(playerNameInput));
			try
			{
				if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")) && !string.IsNullOrWhiteSpace(playerNameInput) && (config.Mode ? config.Whitelist : config.Blacklist).Add(playerNameInput))
				{
					config.Save(this);
					playerNameInput = string.Empty;
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
			List<string> list = new List<string>();
			foreach (string item in config.Mode ? config.Whitelist : config.Blacklist)
			{
				ImU8String val5 = new ImU8String(0, 1);
				((ImU8String)(ref val5)).AppendFormatted<string>(item);
				IdDisposable val6 = ImRaii.PushId(val5, true);
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get("Delete")))
					{
						list.Add(item);
					}
					ImGui.SameLine();
					ImGui.Bullet();
					ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
					ImU8String val7 = new ImU8String(0, 1);
					((ImU8String)(ref val7)).AppendFormatted<string>(item);
					ImGui.TextUnformatted(val7);
				}
				finally
				{
					((IDisposable)val6)?.Dispose();
				}
			}
			if (list.Count > 0)
			{
				list.ForEach(delegate(string x)
				{
					(config.Mode ? config.Whitelist : config.Blacklist).Remove(x);
				});
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
	}

	private unsafe void OnSelectYesno(AddonEvent type, AddonArgs args)
	{
		AddonSelectYesno* selectYesno = (AddonSelectYesno*)Addons.SelectYesno;
		if (selectYesno == null || DalamudServiceExtension.Instance<IPartyList>().Length > 1)
		{
			return;
		}
		string text = ((object)(*(Utf8String*)(&((AtkTextNode)((AddonSelectYesno)selectYesno).PromptText).NodeText))/*cast due to constrained. prefix*/).ToString();
		if (!string.IsNullOrWhiteSpace(text))
		{
			string text2 = ExtractPlayerName(text);
			if (!string.IsNullOrWhiteSpace(text2) && (!config.Mode || config.Whitelist.Contains(text2)) && (config.Mode || !config.Blacklist.Contains(text2)))
			{
				AddonSelectYesnoEvent.ClickYes();
			}
		}
	}

	private static string ExtractPlayerName(string inputText)
	{
		Match match = Regex.Match(inputText, Pattern);
		if (match != null && match.Success)
		{
			GroupCollection groups = match.Groups;
			if (groups != null && groups.Count > 1)
			{
				return match.Groups[1].Value;
			}
		}
		return string.Empty;
	}

	private static string BuildPattern(List<Payload> payloads)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Payload payload in payloads)
		{
			TextPayload val = (TextPayload)(object)((payload is TextPayload) ? payload : null);
			if (val != null)
			{
				stringBuilder.Append(Regex.Escape(val.Text));
			}
			else
			{
				stringBuilder.Append("(.*?)");
			}
		}
		return stringBuilder.ToString();
	}

	static AutoAcceptInvitation()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Addon valueOrDefault = LuminaGetter.GetRow<Addon>(120u).GetValueOrDefault();
		Pattern = BuildPattern(SeStringExtensions.ToDalamudString(((Addon)(ref valueOrDefault)).Text).Payloads);
	}
}
