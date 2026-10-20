using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoBroadcastActionHitInfo : ModuleBase
{
	private unsafe delegate void ProcessPacketActionEffectDelegate(uint sourceID, nint sourceCharacter, nint pos, Header* effectHeader, Effect* effectArray, ulong* effectTrail);

	private class Config : ModuleConfig
	{
		public HashSet<uint> BlacklistActions = new HashSet<uint>();

		public string CriticalHitPattern = "技能 {0} 触发了暴击";

		public Dictionary<uint, string> CustomActionName = new Dictionary<uint, string>();

		public string DirectCriticalHitPattern = "技能 {0} 触发了直暴";

		public string DirectHitPattern = "技能 {0} 触发了直击";

		public bool UseTTS;

		public HashSet<uint> WhitelistActions = new HashSet<uint>();

		public bool WorkMode;
	}

	private static readonly CompSig ProcessPacketActionEffectSig = new CompSig("E8 ?? ?? ?? ?? 48 8B 8D F0 03 00 00");

	private Hook<ProcessPacketActionEffectDelegate> ProcessPacketActionEffectHook;

	private Config config;

	private ActionSelectCombo whitelistCombo;

	private ActionSelectCombo blacklistCombo;

	private ActionSelectCombo selectedCombo;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoBroadcastActionHitInfoTitle"),
		Description = LanguageManager.Get("AutoBroadcastActionHitInfoDescription"),
		Category = ModuleCategory.Action,
		Author = new string[1] { "Xww" }
	};

	protected unsafe override void Init()
	{
		whitelistCombo = new ActionSelectCombo("Whitelist");
		blacklistCombo = new ActionSelectCombo("Blacklist");
		selectedCombo = new ActionSelectCombo("Selected");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		whitelistCombo.SelectedIDs = config.WhitelistActions;
		blacklistCombo.SelectedIDs = config.BlacklistActions;
		if (ProcessPacketActionEffectHook == null)
		{
			ProcessPacketActionEffectHook = ProcessPacketActionEffectSig.GetHook<ProcessPacketActionEffectDelegate>(ProcessPacketActionEffectDetour);
		}
		ProcessPacketActionEffectHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoBroadcastActionHitInfo-DHHint"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
		ImGui.InputText(ImU8String.op_Implicit("###DirectHitMessage"), ref config.DirectHitPattern, 512, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(1, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoBroadcastActionHitInfo-CHHint"));
		((ImU8String)(ref val2)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val2);
		ImGui.SameLine();
		ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
		ImGui.InputText(ImU8String.op_Implicit("###CriticalHitMessage"), ref config.CriticalHitPattern, 512, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(1, 1);
		((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoBroadcastActionHitInfo-DCHHint"));
		((ImU8String)(ref val3)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val3);
		ImGui.SameLine();
		ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
		ImGui.InputText(ImU8String.op_Implicit("###DirectCriticalHitMessage"), ref config.DirectCriticalHitPattern, 512, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGuiOm.ScaledDummy(5f);
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val4 = default(ImU8String);
		((ImU8String)(ref val4))._002Ector(0, 1);
		((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("AutoBroadcastActionHitInfo-UseTTS"));
		ImGui.TextColored(ref vector, val4);
		ImGui.SameLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit("###UseTTS"), ref config.UseTTS))
		{
			config.Save(this);
		}
		ImGuiOm.ScaledDummy(5f);
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val5 = default(ImU8String);
		((ImU8String)(ref val5))._002Ector(1, 1);
		((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("WorkMode"));
		((ImU8String)(ref val5)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val5);
		ImGui.SameLine();
		uint colorU = ImGui.GetColorU32((ImGuiCol)7);
		ImGui.SameLine();
		if (ImGuiOm.ToggleButton("WorkMode", ref config.WorkMode, 0f, colorU, colorU))
		{
			config.Save(this);
		}
		ImGui.SameLine();
		ImGui.TextUnformatted(ImU8String.op_Implicit(config.WorkMode ? LanguageManager.Get("Whitelist") : LanguageManager.Get("Blacklist")));
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val6 = default(ImU8String);
		((ImU8String)(ref val6))._002Ector(1, 1);
		((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Action"));
		((ImU8String)(ref val6)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val6);
		ImGui.SameLine();
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		if (config.WorkMode ? whitelistCombo.DrawCheckbox() : blacklistCombo.DrawCheckbox())
		{
			config.BlacklistActions = blacklistCombo.SelectedIDs;
			config.WhitelistActions = blacklistCombo.SelectedIDs;
			config.Save(this);
		}
		ImGuiOm.ScaledDummy(5f);
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val7 = default(ImU8String);
		((ImU8String)(ref val7))._002Ector(1, 1);
		((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("AutoBroadcastActionHitInfo-CustomActionAlias"));
		((ImU8String)(ref val7)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val7);
		ImGui.SameLine();
		ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
		IdDisposable val8 = ImRaii.PushId(ImU8String.op_Implicit("AddCustomActionSelect"), true);
		try
		{
			selectedCombo.DrawRadio();
		}
		finally
		{
			((IDisposable)val8)?.Dispose();
		}
		ImGui.SameLine();
		DisabledDisposable val9 = ImRaii.Disabled(selectedCombo.SelectedID == 0 || config.CustomActionName.ContainsKey(selectedCombo.SelectedID));
		try
		{
			if (ImGuiOm.ButtonIcon("##新增", (FontAwesomeIcon)61543) && selectedCombo.SelectedID != 0)
			{
				config.CustomActionName.TryAdd(selectedCombo.SelectedID, string.Empty);
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val9)?.Dispose();
		}
		ImGui.Spacing();
		if (config.CustomActionName.Count < 1)
		{
			return;
		}
		ImU8String val10 = default(ImU8String);
		((ImU8String)(ref val10))._002Ector(21, 1);
		((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("AutoBroadcastActionHitInfo-CustomActionAliasCount", config.CustomActionName.Count));
		((ImU8String)(ref val10)).AppendLiteral("###CustomActionsCombo");
		if (!ImGui.CollapsingHeader(val10, (ImGuiTreeNodeFlags)0))
		{
			return;
		}
		int num = 1;
		foreach (KeyValuePair<uint, string> item2 in config.CustomActionName)
		{
			ImU8String val11 = new ImU8String(17, 1);
			((ImU8String)(ref val11)).AppendLiteral("ActionCustomName_");
			((ImU8String)(ref val11)).AppendFormatted<uint>(item2.Key);
			IdDisposable val12 = ImRaii.PushId(val11, true);
			try
			{
				if (!LuminaGetter.TryGetRow<Action>(item2.Key, out Action item))
				{
					continue;
				}
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val13 = new GameIconLookup((uint)((Action)(ref item)).Icon, false, true, (ClientLanguage?)null);
				IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val13).GetWrapOrDefault((IDalamudTextureWrap)null);
				if (wrapOrDefault == null)
				{
					continue;
				}
				GroupDisposable val14 = ImRaii.Group();
				try
				{
					ImGui.AlignTextToFramePadding();
					ImU8String val15 = new ImU8String(1, 1);
					((ImU8String)(ref val15)).AppendFormatted<int>(num);
					((ImU8String)(ref val15)).AppendLiteral(".");
					ImGui.TextUnformatted(val15);
					ImGui.SameLine();
					ImGui.Image(wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
					ImGui.SameLine();
					ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((Action)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
					ImGui.SameLine();
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get("Delete")))
					{
						config.CustomActionName.Remove(item2.Key);
						config.Save(this);
						continue;
					}
					IndentDisposable val16 = ImRaii.PushIndent(1, true);
					try
					{
						string value = item2.Value;
						ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
						if (ImGui.InputText(ImU8String.op_Implicit("###ActionCustomNameInput"), ref value, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
						{
							config.CustomActionName[item2.Key] = value;
						}
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
					}
					finally
					{
						((IDisposable)val16)?.Dispose();
					}
					num++;
				}
				finally
				{
					((GroupDisposable)(ref val14)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val12)?.Dispose();
			}
		}
	}

	private unsafe void ProcessPacketActionEffectDetour(uint sourceID, nint sourceCharacter, nint pos, Header* effectHeader, Effect* effectArray, ulong* effectTrail)
	{
		ProcessPacketActionEffectHook.Original(sourceID, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
		Parse(sourceID, effectHeader, effectArray);
	}

	private unsafe void Parse(uint sourceEntityID, Header* effectHeader, Effect* effectArray)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (((Header)effectHeader).NumTargets < 1)
			{
				return;
			}
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer == null || localPlayer.EntityID != sourceEntityID)
			{
				return;
			}
			uint actionId = ((Header)effectHeader).ActionId;
			Action? row = LuminaGetter.GetRow<Action>(actionId);
			if (!row.HasValue)
			{
				return;
			}
			Action value = row.Value;
			if (((Action)(ref value)).ActionCategory.RowId == 1)
			{
				return;
			}
			if (!config.WorkMode)
			{
				if (config.BlacklistActions.Contains(actionId))
				{
					return;
				}
			}
			else if (!config.WhitelistActions.Contains(actionId))
			{
				return;
			}
			string text;
			if (!config.CustomActionName.TryGetValue(actionId, out string value2) || string.IsNullOrWhiteSpace(value2))
			{
				value = row.Value;
				text = ((object)((Action)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			}
			else
			{
				text = value2;
			}
			string arg = text;
			string text2 = ((Effect)effectArray).Param0 switch
			{
				64 => string.Format(config.DirectHitPattern, arg), 
				32 => string.Format(config.CriticalHitPattern, arg), 
				96 => string.Format(config.DirectCriticalHitPattern, arg), 
				_ => string.Empty, 
			};
			if (string.IsNullOrWhiteSpace(text2))
			{
				return;
			}
			switch (((Effect)effectArray).Param0)
			{
			case 32:
			case 64:
				OmenServiceBase<NotifyHelper>.Instance().ContentHintBlue(text2, TimeSpan.FromSeconds(1L));
				if (config.UseTTS)
				{
					NotifyHelper.Speak(text2);
				}
				break;
			case 96:
				OmenServiceBase<NotifyHelper>.Instance().ContentHintRed(text2, TimeSpan.FromSeconds(1L));
				if (config.UseTTS)
				{
					NotifyHelper.Speak(text2);
				}
				break;
			}
		}
		catch
		{
		}
	}
}
