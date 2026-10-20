using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class CustomActionQueueTime : ModuleBase
{
	private class Config : ModuleConfig
	{
		public DefaultQueueMode DefaultQueueMode;

		public float DefaultQueueTime = 500f;

		public bool DisplayQueueActionOverlay = true;

		public Vector4 OverlayFontColor = new Vector4(0f, 0f, 0f, 1f);

		public float OverlayFontScale = 1f;

		public Dictionary<uint, float> QueueTime = new Dictionary<uint, float>();
	}

	private enum DefaultQueueMode
	{
		None,
		Fixed,
		BasedOnFrameRate
	}

	private Config config;

	private ActionSelectCombo actionSelectCombo;

	private float queueTimeMSInput = 500f;

	private static readonly FrozenDictionary<DefaultQueueMode, string> DefaultQueueModesLoc = new Dictionary<DefaultQueueMode, string>
	{
		[DefaultQueueMode.None] = LanguageManager.Get("CustomActionQueueTime-DefaultQueueMode-None"),
		[DefaultQueueMode.Fixed] = LanguageManager.Get("CustomActionQueueTime-DefaultQueueMode-Fixed"),
		[DefaultQueueMode.BasedOnFrameRate] = LanguageManager.Get("CustomActionQueueTime-DefaultQueueMode-BasedOnFrameRate")
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CustomActionQueueTimeTitle"),
		Description = LanguageManager.Get("CustomActionQueueTimeDescription"),
		Category = ModuleCategory.Action
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		actionSelectCombo = new ActionSelectCombo("Action");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		base.Overlay = new Overlay(this);
		Overlay? overlay = base.Overlay;
		((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags | 0x84);
		OmenServiceBase<UseActionManager>.Instance().RegPreIsActionOffCooldown(OnPreIsActionOffCooldown);
		if (config.DisplayQueueActionOverlay)
		{
			((Window)base.Overlay).IsOpen = true;
		}
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreIsActionOffCooldown);
	}

	protected override void ConfigUI()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_082c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("CustomActionQueueTime-DisplayOverlay"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit("###DisplayQueueActionOverlay"), ref config.DisplayQueueActionOverlay))
		{
			((Window)base.Overlay).IsOpen = config.DisplayQueueActionOverlay;
			config.Save(this);
		}
		DisabledDisposable val3;
		if (config.DisplayQueueActionOverlay)
		{
			ImGui.AlignTextToFramePadding();
			vector = KnownColor.Chocolate.ToVector4();
			ImU8String val2 = default(ImU8String);
			((ImU8String)(ref val2))._002Ector(1, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("FontScale"));
			((ImU8String)(ref val2)).AppendLiteral(":");
			ImGui.TextColored(ref vector, val2);
			for (float num = 0.6f; num < 1.8f; num += 0.2f)
			{
				float num2 = (float)Math.Round(num, 1);
				ImGui.SameLine();
				val3 = ImRaii.Disabled(config.OverlayFontScale == num2);
				try
				{
					ImU8String val4 = new ImU8String(0, 1);
					((ImU8String)(ref val4)).AppendFormatted<float>(num2);
					if (ImGui.Button(val4, default(Vector2)))
					{
						config.OverlayFontScale = num2;
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			ImGui.AlignTextToFramePadding();
			vector = KnownColor.Chocolate.ToVector4();
			ImU8String val5 = default(ImU8String);
			((ImU8String)(ref val5))._002Ector(1, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("CustomActionQueueTime-UnlockOverlay"));
			((ImU8String)(ref val5)).AppendLiteral(":");
			ImGui.TextColored(ref vector, val5);
			bool flag = !((Enum)((Window)base.Overlay).Flags).HasFlag((Enum)(object)(ImGuiWindowFlags)4);
			ImGui.SameLine();
			if (ImGui.Checkbox(ImU8String.op_Implicit("###UnlockOverlay"), ref flag))
			{
				if (((Enum)((Window)base.Overlay).Flags).HasFlag((Enum)(object)(ImGuiWindowFlags)4))
				{
					Overlay? overlay = base.Overlay;
					((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags & -5);
				}
				else
				{
					Overlay? overlay2 = base.Overlay;
					((Window)overlay2).Flags = (ImGuiWindowFlags)(((Window)overlay2).Flags | 4);
				}
			}
			ImGui.AlignTextToFramePadding();
			vector = KnownColor.Chocolate.ToVector4();
			ImU8String val6 = default(ImU8String);
			((ImU8String)(ref val6))._002Ector(1, 1);
			((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("CustomActionQueueTime-OverlayTextColor"));
			((ImU8String)(ref val6)).AppendLiteral(":");
			ImGui.TextColored(ref vector, val6);
			ImGui.SameLine();
			if (ImGui.ColorButton(ImU8String.op_Implicit(LanguageManager.Get("CustomActionQueueTime-OverlayTextColor")), ref config.OverlayFontColor, (ImGuiColorEditFlags)131072, default(Vector2)))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("OverlayColorPickerPopup"), (ImGuiPopupFlags)0);
			}
			PopupDisposable val7 = ImRaii.Popup(ImU8String.op_Implicit("OverlayColorPickerPopup"));
			try
			{
				if (PopupDisposable.op_Implicit(val7) && ImGui.ColorPicker4(ImU8String.op_Implicit(LanguageManager.Get("CustomActionQueueTime-OverlayTextColor")), ref config.OverlayFontColor, (ImGuiColorEditFlags)131072))
				{
					config.Save(this);
				}
			}
			finally
			{
				((PopupDisposable)(ref val7)).Dispose();
			}
		}
		ImGui.Spacing();
		ImGui.AlignTextToFramePadding();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val8 = default(ImU8String);
		((ImU8String)(ref val8))._002Ector(1, 1);
		((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("CustomActionQueueTime-DefaultQueueMode"));
		((ImU8String)(ref val8)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val8);
		ImGui.SameLine();
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ComboDisposable val9 = ImRaii.Combo(ImU8String.op_Implicit("###DefaultQueueTimeCombo"), ImU8String.op_Implicit(DefaultQueueModesLoc[config.DefaultQueueMode]), (ImGuiComboFlags)8);
		try
		{
			if (ComboDisposable.op_Implicit(val9))
			{
				DefaultQueueMode[] values = Enum.GetValues<DefaultQueueMode>();
				foreach (DefaultQueueMode defaultQueueMode in values)
				{
					if (ImGui.Selectable(ImU8String.op_Implicit(DefaultQueueModesLoc[defaultQueueMode]), config.DefaultQueueMode == defaultQueueMode, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						config.DefaultQueueMode = defaultQueueMode;
						config.Save(this);
					}
				}
			}
		}
		finally
		{
			((ComboDisposable)(ref val9)).Dispose();
		}
		ImGui.SameLine();
		ImGui.TextDisabled(ImU8String.op_Implicit("|"));
		ImGui.SameLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val10 = default(ImU8String);
		((ImU8String)(ref val10))._002Ector(1, 1);
		((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("CustomActionQueueTime-DefaultQueueTime"));
		((ImU8String)(ref val10)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val10);
		ImGui.SameLine();
		if (config.DefaultQueueMode == DefaultQueueMode.Fixed)
		{
			ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
			if (ImGui.InputFloat(ImU8String.op_Implicit("(ms)###FixedDefaultQueueTimeInput"), ref config.DefaultQueueTime, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0))
			{
				config.DefaultQueueTime = Math.Max(1f, config.DefaultQueueTime);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
		}
		else
		{
			ImU8String val11 = default(ImU8String);
			((ImU8String)(ref val11))._002Ector(5, 1);
			((ImU8String)(ref val11)).AppendFormatted<float>(GetDefaultQueueTime(), "F1");
			((ImU8String)(ref val11)).AppendLiteral(" (ms)");
			ImGui.TextUnformatted(val11);
		}
		ImGui.Spacing();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("CustomActionQueueTime-CustomDefaultQueueTime")));
		val3 = ImRaii.Disabled(actionSelectCombo.SelectedID == 0 || config.QueueTime.ContainsKey(actionSelectCombo.SelectedID));
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")) && actionSelectCombo.SelectedID != 0)
			{
				config.QueueTime.TryAdd(actionSelectCombo.SelectedID, 500f);
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		ImGui.SameLine();
		ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
		actionSelectCombo.DrawRadio();
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		float x = ImGui.GetContentRegionAvail().X * 0.75f;
		Vector2 vector2 = new Vector2(x, 0f);
		TableDisposable val12 = ImRaii.Table(ImU8String.op_Implicit("CustomActionQueueTimeTable"), 3, (ImGuiTableFlags)1920, vector2);
		try
		{
			if (!val12)
			{
				return;
			}
			Addon? row = LuminaGetter.GetRow<Addon>(1340u);
			object obj;
			Addon valueOrDefault;
			if (!row.HasValue)
			{
				obj = null;
			}
			else
			{
				valueOrDefault = row.GetValueOrDefault();
				obj = ((object)((Addon)(ref valueOrDefault)).Text/*cast due to constrained. prefix*/).ToString();
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit((string)obj), (ImGuiTableColumnFlags)4, 20f, 0u);
			row = LuminaGetter.GetRow<Addon>(702u);
			object obj2;
			if (!row.HasValue)
			{
				obj2 = null;
			}
			else
			{
				valueOrDefault = row.GetValueOrDefault();
				obj2 = ((object)((Addon)(ref valueOrDefault)).Text/*cast due to constrained. prefix*/).ToString();
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit((string)obj2), (ImGuiTableColumnFlags)4, 10f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("CustomActionQueueTime-QueueTime")), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableHeadersRow();
			List<uint> list = new List<uint>();
			foreach (KeyValuePair<uint, float> item2 in config.QueueTime)
			{
				if (!LuminaGetter.TryGetRow<Action>(item2.Key, out Action item))
				{
					continue;
				}
				ITextureProvider obj3 = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val13 = new GameIconLookup((uint)((Action)(ref item)).Icon, false, true, (ClientLanguage?)null);
				IDalamudTextureWrap wrapOrDefault = obj3.GetFromGameIcon(ref val13).GetWrapOrDefault((IDalamudTextureWrap)null);
				if (wrapOrDefault == null)
				{
					continue;
				}
				IdDisposable val14 = ImRaii.PushId(ImU8String.op_Implicit(((Action)(ref item)).RowId.ToString()), true);
				try
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGuiOm.SelectableImageWithText(wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Action)(ref item)).Name/*cast due to constrained. prefix*/).ToString(), selected: false, (ImGuiSelectableFlags)0);
					PopupDisposable val15 = ImRaii.ContextPopupItem(ImU8String.op_Implicit("ActionContext"));
					try
					{
						if (PopupDisposable.op_Implicit(val15) && ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("Delete")), false, true))
						{
							list.Add(((Action)(ref item)).RowId);
						}
					}
					finally
					{
						((PopupDisposable)(ref val15)).Dispose();
					}
					int adjustedRecastTime = ActionManager.GetAdjustedRecastTime((ActionType)1, item2.Key, true);
					ImGui.TableNextColumn();
					ColorDisposable val16 = ImRaii.PushColor((ImGuiCol)0, KnownColor.OrangeRed.ToVector4(), ((Action)(ref item)).ClassJob.RowId != 0 && localPlayer.ClassJob.RowId != ((Action)(ref item)).ClassJob.RowId);
					try
					{
						ImGuiOm.Text($"{adjustedRecastTime} ms");
					}
					finally
					{
						((IDisposable)val16)?.Dispose();
					}
					float value = item2.Value;
					ImGui.TableNextColumn();
					ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
					if (ImGui.InputFloat(ImU8String.op_Implicit("(ms)"), ref value, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0))
					{
						value = Math.Clamp(value, 0f, adjustedRecastTime);
					}
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						config.QueueTime[item2.Key] = value;
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val14)?.Dispose();
				}
			}
			if (list.Count > 0)
			{
				list.ForEach(delegate(uint key)
				{
					config.QueueTime.Remove(key);
				});
				config.Save(this);
			}
		}
		finally
		{
			((TableDisposable)(ref val12)).Dispose();
		}
	}

	protected unsafe override void OverlayUI()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Invalid comparison between Unknown and I4
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
		{
			return;
		}
		ActionManager* ptr = ActionManager.Instance();
		if (ptr == null)
		{
			return;
		}
		using (OmenServiceBase<FontManager>.Instance().GetUIFont(config.OverlayFontScale).Push())
		{
			ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, config.OverlayFontColor, true);
			try
			{
				uint queuedActionId = ((ActionManager)ptr).QueuedActionId;
				ActionType queuedActionType = ((ActionManager)ptr).QueuedActionType;
				GroupDisposable val2 = ImRaii.Group();
				try
				{
					if (queuedActionId == 0)
					{
						ImU8String val3 = new ImU8String(2, 1);
						((ImU8String)(ref val3)).AppendLiteral("(");
						((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("CustomActionQueueTime-NoActionInQueue"));
						((ImU8String)(ref val3)).AppendLiteral(")");
						ImGui.TextUnformatted(val3);
					}
					else if ((int)queuedActionType != 1)
					{
						ImU8String val4 = new ImU8String(2, 1);
						((ImU8String)(ref val4)).AppendLiteral("(");
						((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("CustomActionQueueTime-NonePlayerAction"));
						((ImU8String)(ref val4)).AppendLiteral(")");
						ImGui.TextUnformatted(val4);
					}
					else
					{
						if (!LuminaGetter.TryGetRow<Action>(queuedActionId, out Action item))
						{
							return;
						}
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						GameIconLookup val5 = new GameIconLookup((uint)((Action)(ref item)).Icon, false, true, (ClientLanguage?)null);
						IDalamudTextureWrap wrapOrDefault = obj.GetFromGameIcon(ref val5).GetWrapOrDefault((IDalamudTextureWrap)null);
						if (wrapOrDefault == null)
						{
							return;
						}
						ImGuiOm.TextImage(((object)((Action)(ref item)).Name/*cast due to constrained. prefix*/).ToString() ?? "", wrapOrDefault.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
					}
				}
				finally
				{
					((GroupDisposable)(ref val2)).Dispose();
				}
				if (ImGui.IsItemHovered())
				{
					ImGui.SetMouseCursor((ImGuiMouseCursor)7);
				}
				if (ImGui.IsItemClicked())
				{
					((ActionManager)ptr).QueuedActionId = 0u;
					Unsafe.Write(&((ActionManager)ptr).QueuedActionType, (ActionType)0);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	private void OnPreIsActionOffCooldown(ref bool isPrevented, ActionType actionType, uint actionID, ref float queueTimeSecond)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)actionType == 1)
		{
			float value;
			float num = (config.QueueTime.TryGetValue(actionID, out value) ? value : GetDefaultQueueTime());
			queueTimeSecond = num / 1000f;
		}
	}

	private unsafe float GetDefaultQueueTime()
	{
		return config.DefaultQueueMode switch
		{
			DefaultQueueMode.None => 500f, 
			DefaultQueueMode.Fixed => config.DefaultQueueTime, 
			DefaultQueueMode.BasedOnFrameRate => Math.Clamp(500f + (90f - ((Framework)Framework.Instance()).FrameRate) / 5f * 20f, 300f, 800f), 
			_ => 500f, 
		};
	}
}
