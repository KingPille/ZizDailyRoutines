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
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoReplaceActionAnimation : ModuleBase
{
	private class Config : ModuleConfig
	{
		public Dictionary<uint, ActionConfig> ActionConfigs = new Dictionary<uint, ActionConfig>();
	}

	private class ActionConfig
	{
		public bool IsEnabled { get; set; } = true;

		public uint ReplacementActionID { get; set; }

		public EffectType EffectType { get; set; }
	}

	private enum EffectType
	{
		All,
		Self,
		Others
	}

	private Config config;

	private ActionSelectCombo inputCombo;

	private ActionSelectCombo outputCombo;

	private EffectType effectTypeInput;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoReplaceActionAnimationTitle"),
		Description = LanguageManager.Get("AutoReplaceActionAnimationDescription"),
		Category = ModuleCategory.Action
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		inputCombo = new ActionSelectCombo("Input");
		outputCombo = new ActionSelectCombo("Output");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<UseActionManager>.Instance().RegPreCharacterStartCast(OnCharacterStartCast);
		OmenServiceBase<UseActionManager>.Instance().RegPreCharacterCompleteCast(OnCharacterCompleteCast);
	}

	protected override void Uninit()
	{
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnCharacterStartCast);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnCharacterCompleteCast);
	}

	protected override void ConfigUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			IdDisposable val2 = ImRaii.PushId(ImU8String.op_Implicit("Input"), true);
			try
			{
				inputCombo.DrawRadio();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Input")));
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			val2 = ImRaii.PushId(ImU8String.op_Implicit("Output"), true);
			try
			{
				outputCombo.DrawRadio();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Output")));
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			val2 = ImRaii.PushId(ImU8String.op_Implicit("Output"), true);
			try
			{
				ComboDisposable val3 = ImRaii.Combo(ImU8String.op_Implicit("###EffectTypeCombo"), ImU8String.op_Implicit(GetEffectTypeName(effectTypeInput)));
				try
				{
					if (ComboDisposable.op_Implicit(val3))
					{
						EffectType[] values = Enum.GetValues<EffectType>();
						foreach (EffectType effectType in values)
						{
							if (ImGui.Selectable(ImU8String.op_Implicit(GetEffectTypeName(effectType)), effectType == effectTypeInput, (ImGuiSelectableFlags)0, default(Vector2)))
							{
								effectTypeInput = effectType;
							}
						}
					}
				}
				finally
				{
					((ComboDisposable)(ref val3)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGui.SameLine();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Range")));
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImGui.SameLine(0f, 10f * Globals.GlobalUIScale);
		Action val4;
		if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Confirm")), new Vector2(ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get("Confirm")), false, -1f).X * 2f, ImGui.GetItemRectSize().Y)))
		{
			val4 = inputCombo.SelectedItem;
			if (((Action)(ref val4)).RowId != 0)
			{
				val4 = outputCombo.SelectedItem;
				if (((Action)(ref val4)).RowId != 0)
				{
					ActionConfig obj = new ActionConfig
					{
						IsEnabled = true
					};
					val4 = outputCombo.SelectedItem;
					obj.ReplacementActionID = ((Action)(ref val4)).RowId;
					obj.EffectType = effectTypeInput;
					ActionConfig value = obj;
					Dictionary<uint, ActionConfig> actionConfigs = config.ActionConfigs;
					val4 = inputCombo.SelectedItem;
					actionConfigs[((Action)(ref val4)).RowId] = value;
					config.ActionConfigs = config.ActionConfigs.OrderBy<KeyValuePair<uint, ActionConfig>, uint>(delegate(KeyValuePair<uint, ActionConfig> keyValuePair)
					{
						//IL_0023: Unknown result type (might be due to invalid IL or missing references)
						//IL_0028: Unknown result type (might be due to invalid IL or missing references)
						//IL_002b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0030: Unknown result type (might be due to invalid IL or missing references)
						//IL_0050: Unknown result type (might be due to invalid IL or missing references)
						//IL_0055: Unknown result type (might be due to invalid IL or missing references)
						Action? row = LuminaGetter.GetRow<Action>(keyValuePair.Key);
						uint? num3;
						if (!row.HasValue)
						{
							num3 = null;
						}
						else
						{
							Action valueOrDefault = row.GetValueOrDefault();
							ClassJobCategory? valueNullable = ((Action)(ref valueOrDefault)).ClassJobCategory.ValueNullable;
							if (!valueNullable.HasValue)
							{
								num3 = null;
							}
							else
							{
								ClassJobCategory valueOrDefault2 = valueNullable.GetValueOrDefault();
								num3 = ((ClassJobCategory)(ref valueOrDefault2)).RowId;
							}
						}
						return (uint)(((int?)num3) ?? (-1));
					}).ThenBy((KeyValuePair<uint, ActionConfig> keyValuePair) => keyValuePair.Key).ToDictionary((KeyValuePair<uint, ActionConfig> keyValuePair) => keyValuePair.Key, (KeyValuePair<uint, ActionConfig> keyValuePair) => keyValuePair.Value);
					config.Save(this);
				}
			}
		}
		ImGui.NewLine();
		if (config.ActionConfigs.Count == 0)
		{
			return;
		}
		ImU8String val5 = ImU8String.op_Implicit("###Table");
		float x = ImGui.GetContentRegionAvail().X;
		ImGuiStylePtr style = ImGui.GetStyle();
		TableDisposable val6 = ImRaii.Table(val5, 10, (ImGuiTableFlags)0, new Vector2(x - 4f * ((ImGuiStylePtr)(ref style)).ItemSpacing.X, 0f));
		try
		{
			if (!val6)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("操作 1"), (ImGuiTableColumnFlags)8, 3f * ImGui.GetTextLineHeight(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("输入 1"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("箭头 1"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("→"), false, -1f).X * 3f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("输出 1"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImU8String val7 = ImU8String.op_Implicit("范围 1");
			ImU8String val8 = new ImU8String(2, 1);
			((ImU8String)(ref val8)).AppendLiteral("[");
			((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get("All"));
			((ImU8String)(ref val8)).AppendLiteral("]");
			ImGui.TableSetupColumn(val7, (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(val8, false, -1f).X * 1.5f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("操作 2"), (ImGuiTableColumnFlags)8, 3f * ImGui.GetTextLineHeight(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("输入 2"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("箭头 2"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("→"), false, -1f).X * 3f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("输出 2"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImU8String val9 = ImU8String.op_Implicit("范围 2");
			ImU8String val10 = new ImU8String(2, 1);
			((ImU8String)(ref val10)).AppendLiteral("[");
			((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get("All"));
			((ImU8String)(ref val10)).AppendLiteral("]");
			ImGui.TableSetupColumn(val9, (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(val10, false, -1f).X * 1.5f, 0u);
			int num = 0;
			foreach (KeyValuePair<uint, ActionConfig> actionConfig2 in config.ActionConfigs)
			{
				actionConfig2.Deconstruct(out var key, out var value2);
				uint num2 = key;
				ActionConfig actionConfig = value2;
				uint replacementActionID = actionConfig.ReplacementActionID;
				bool isEnabled = actionConfig.IsEnabled;
				if (num % 2 == 0)
				{
					ImGui.TableNextRow();
				}
				num++;
				ImU8String val11 = new ImU8String(1, 2);
				((ImU8String)(ref val11)).AppendFormatted<uint>(num2);
				((ImU8String)(ref val11)).AppendLiteral("_");
				((ImU8String)(ref val11)).AppendFormatted<uint>(replacementActionID);
				IdDisposable val12 = ImRaii.PushId(val11, true);
				try
				{
					GroupDisposable val13 = ImRaii.Group();
					try
					{
						ImGui.TableNextColumn();
						if (ImGui.Checkbox(ImU8String.op_Implicit("##Enabled"), ref isEnabled))
						{
							actionConfig.IsEnabled = isEnabled;
							config.Save(this);
						}
						ImGui.SameLine();
						if (ImGui.Button(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62189)), default(Vector2)))
						{
							config.ActionConfigs.Remove(num2);
							config.Save(this);
							continue;
						}
						ImGui.TableNextColumn();
						GroupDisposable val14 = ImRaii.Group();
						try
						{
							val4 = LuminaGetter.GetRow<Action>(num2).Value;
							IDalamudTextureWrap gameIcon = ImageHelper.GetGameIcon(((Action)(ref val4)).Icon);
							if (gameIcon != null)
							{
								ImGui.Image(gameIcon.Handle, Globals.ScaledVector2(24f));
								ImGui.SameLine();
							}
							ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetActionName(num2)));
						}
						finally
						{
							((GroupDisposable)(ref val14)).Dispose();
						}
						if (ImGui.IsItemHovered())
						{
							ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						}
						if (ImGui.IsItemClicked())
						{
							inputCombo.SelectedID = num2;
						}
						ImGui.TableNextColumn();
						ImGui.TextUnformatted(ImU8String.op_Implicit("→"));
						ImGui.TableNextColumn();
						GroupDisposable val15 = ImRaii.Group();
						try
						{
							val4 = LuminaGetter.GetRow<Action>(replacementActionID).Value;
							IDalamudTextureWrap gameIcon2 = ImageHelper.GetGameIcon(((Action)(ref val4)).Icon);
							if (gameIcon2 != null)
							{
								ImGui.Image(gameIcon2.Handle, Globals.ScaledVector2(24f));
								ImGui.SameLine();
							}
							ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetActionName(replacementActionID)));
						}
						finally
						{
							((GroupDisposable)(ref val15)).Dispose();
						}
						if (ImGui.IsItemHovered())
						{
							ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						}
						if (ImGui.IsItemClicked())
						{
							outputCombo.SelectedID = replacementActionID;
						}
						ImGui.TableNextColumn();
						Vector4 vector = KnownColor.Gray.ToVector4();
						ImU8String val16 = new ImU8String(2, 1);
						((ImU8String)(ref val16)).AppendLiteral("[");
						((ImU8String)(ref val16)).AppendFormatted<string>(GetEffectTypeName(actionConfig.EffectType));
						((ImU8String)(ref val16)).AppendLiteral("]");
						ImGui.TextColored(ref vector, val16);
						if (ImGui.IsItemHovered())
						{
							ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						}
						if (ImGui.IsItemClicked())
						{
							ImU8String val17 = new ImU8String(18, 1);
							((ImU8String)(ref val17)).AppendLiteral("ActionTargetPopup_");
							((ImU8String)(ref val17)).AppendFormatted<uint>(num2);
							ImGui.OpenPopup(val17, (ImGuiPopupFlags)0);
						}
						ImU8String val18 = new ImU8String(18, 1);
						((ImU8String)(ref val18)).AppendLiteral("ActionTargetPopup_");
						((ImU8String)(ref val18)).AppendFormatted<uint>(num2);
						PopupDisposable val19 = ImRaii.Popup(val18);
						try
						{
							if (!PopupDisposable.op_Implicit(val19))
							{
								continue;
							}
							EffectType[] values = Enum.GetValues<EffectType>();
							foreach (EffectType effectType2 in values)
							{
								bool flag = actionConfig.EffectType == effectType2;
								if (ImGui.Selectable(ImU8String.op_Implicit(GetEffectTypeName(effectType2)), flag, (ImGuiSelectableFlags)0, default(Vector2)))
								{
									actionConfig.EffectType = effectType2;
									config.Save(this);
								}
							}
						}
						finally
						{
							((PopupDisposable)(ref val19)).Dispose();
						}
					}
					finally
					{
						((GroupDisposable)(ref val13)).Dispose();
					}
				}
				finally
				{
					((IDisposable)val12)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val6)).Dispose();
		}
		static string GetEffectTypeName(EffectType target)
		{
			return target switch
			{
				EffectType.All => LanguageManager.Get("All"), 
				EffectType.Self => LanguageManager.Get("AutoReplaceActionAnimation-EffectType-Self"), 
				EffectType.Others => LanguageManager.Get("AutoReplaceActionAnimation-EffectType-Others"), 
				_ => string.Empty, 
			};
		}
	}

	private unsafe void OnCharacterStartCast(ref bool isPrevented, ref IBattleChara player, ref ActionType type, ref uint actionID, ref nint a4, ref float rotation, ref float a6)
	{
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer != null)
		{
			bool flag = player.Address == (nint)localPlayer;
			if ((int)type == 1 && config.ActionConfigs.TryGetValue(actionID, out ActionConfig value) && value.IsEnabled && value.EffectType switch
			{
				EffectType.All => true, 
				EffectType.Self => flag, 
				EffectType.Others => !flag, 
				_ => false, 
			})
			{
				actionID = value.ReplacementActionID;
			}
		}
	}

	private unsafe void OnCharacterCompleteCast(ref bool isPrevented, ref IBattleChara player, ref ActionType type, ref uint actionID, ref uint spellID, ref GameObjectId animationTargetID, ref Vector3 location, ref float rotation, ref short lastUsedActionSequence, ref int animationVariation, ref int ballistaEntityID)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		bool flag = player.Address == (nint)localPlayer;
		if ((int)type != 1 || !config.ActionConfigs.TryGetValue(actionID, out ActionConfig value) || !value.IsEnabled || !LuminaGetter.TryGetRow<Action>(value.ReplacementActionID, out Action _) || value.EffectType switch
		{
			EffectType.All => 1, 
			EffectType.Self => flag ? 1 : 0, 
			EffectType.Others => (!flag) ? 1 : 0, 
			_ => 0, 
		} == 0)
		{
			return;
		}
		if (flag)
		{
			IGameObject target = TargetManager.Target;
			if (target != null && ActionManager.CanUseActionOnTarget(value.ReplacementActionID, target.ToStruct()))
			{
				animationTargetID = GameObjectId.op_Implicit(target.GameObjectID);
			}
		}
		actionID = (spellID = value.ReplacementActionID);
	}
}
