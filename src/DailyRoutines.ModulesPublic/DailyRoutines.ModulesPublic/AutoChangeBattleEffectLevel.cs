using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic;

public class AutoChangeBattleEffectLevel : ModuleBase
{
	private class Config : ModuleConfig
	{
		public uint AroundCountThresholdHigh = 40u;

		public uint AroundCountThresholdLow = 20u;

		public EffectSetting DutyDefault = new EffectSetting
		{
			Self = BattleEffectLevel.All,
			Party = BattleEffectLevel.All,
			Other = BattleEffectLevel.Limited,
			Enemy = BattleEffectLevel.All
		};

		public Dictionary<uint, EffectSetting> DutySpecific = new Dictionary<uint, EffectSetting>();

		public EffectSetting OverworldHigh = new EffectSetting
		{
			Self = BattleEffectLevel.All,
			Party = BattleEffectLevel.None,
			Other = BattleEffectLevel.None,
			Enemy = BattleEffectLevel.All
		};

		public EffectSetting OverworldLow = new EffectSetting
		{
			Self = BattleEffectLevel.All,
			Party = BattleEffectLevel.All,
			Other = BattleEffectLevel.All,
			Enemy = BattleEffectLevel.All
		};

		public EffectSetting OverworldMedium = new EffectSetting
		{
			Self = BattleEffectLevel.All,
			Party = BattleEffectLevel.Limited,
			Other = BattleEffectLevel.None,
			Enemy = BattleEffectLevel.All
		};
	}

	private sealed class EffectSetting : IEquatable<EffectSetting>
	{
		public bool IsEnabled { get; set; }

		public BattleEffectLevel Self { get; set; }

		public BattleEffectLevel Party { get; set; } = BattleEffectLevel.None;

		public BattleEffectLevel Other { get; set; } = BattleEffectLevel.None;

		public BattleEffectLevel Enemy { get; set; }

		public bool Equals(EffectSetting? other)
		{
			if ((object)other == null)
			{
				return false;
			}
			if (Self == other.Self && Party == other.Party && Other == other.Other)
			{
				return Enemy == other.Enemy;
			}
			return false;
		}

		public EffectSetting Clone()
		{
			return (EffectSetting)MemberwiseClone();
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as EffectSetting);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Self, Party, Other, Enemy);
		}

		public static bool operator ==(EffectSetting? left, EffectSetting? right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(EffectSetting? left, EffectSetting? right)
		{
			return !object.Equals(left, right);
		}
	}

	private enum BattleEffectLevel : uint
	{
		All,
		Limited,
		None
	}

	private Config config;

	private EffectSetting? lastAppliedSettings;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoChangeBattleEffectLevelTitle"),
		Description = LanguageManager.Get("AutoChangeBattleEffectLevelDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "Siren" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround += OnPlayerReceived;
		if (GameState.IsLoggedIn)
		{
			OnPlayerReceived(Array.Empty<IPlayerCharacter>());
		}
	}

	protected override void Uninit()
	{
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround -= OnPlayerReceived;
	}

	protected override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit("TabBar"));
		try
		{
			if (!val)
			{
				return;
			}
			TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("OutOfDuty")));
			try
			{
				if (TabItemDisposable.op_Implicit(val2))
				{
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoChangeBattleEffectLevel-PlayerThreshold")));
					IndentDisposable val3 = ImRaii.PushIndent(1, true);
					ImU8String val8;
					try
					{
						ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
						ImU8String val4 = new ImU8String(10, 1);
						((ImU8String)(ref val4)).AppendFormatted<string>(LuminaWrapper.GetAddonText(16347u));
						((ImU8String)(ref val4)).AppendLiteral("##LimitLow");
						ImU8String val5 = val4;
						ref uint aroundCountThresholdLow = ref config.AroundCountThresholdLow;
						ImU8String val6 = default(ImU8String);
						ImGui.InputUInt(val5, ref aroundCountThresholdLow, 0u, 0u, val6, (ImGuiInputTextFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
						ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
						val6 = new ImU8String(11, 1);
						((ImU8String)(ref val6)).AppendFormatted<string>(LuminaWrapper.GetAddonText(16346u));
						((ImU8String)(ref val6)).AppendLiteral("##LimitHigh");
						ImU8String val7 = val6;
						ref uint aroundCountThresholdHigh = ref config.AroundCountThresholdHigh;
						val8 = default(ImU8String);
						ImGui.InputUInt(val7, ref aroundCountThresholdHigh, 0u, 0u, val8, (ImGuiInputTextFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							config.Save(this);
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					ImGui.NewLine();
					val8 = new ImU8String(2, 1);
					((ImU8String)(ref val8)).AppendLiteral("＜ ");
					ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
					SeStringParameter reference = SeStringParameter.op_Implicit(config.AroundCountThresholdLow);
					((ImU8String)(ref val8)).AppendFormatted<ReadOnlySeString>(obj.EvaluateFromAddon(12871u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null));
					if (ImGui.CollapsingHeader(val8, (ImGuiTreeNodeFlags)32))
					{
						DrawBattleEffectSetting("Low", config.OverworldLow);
					}
					ImU8String val9 = new ImU8String(7, 2);
					ISeStringEvaluator obj2 = DalamudServiceExtension.Instance<ISeStringEvaluator>();
					SeStringParameter reference2 = SeStringParameter.op_Implicit(config.AroundCountThresholdLow);
					((ImU8String)(ref val9)).AppendFormatted<ReadOnlySeString>(obj2.EvaluateFromAddon(12871u, new Span<SeStringParameter>(ref reference2), (ClientLanguage?)null));
					((ImU8String)(ref val9)).AppendLiteral(" ≤ X ≤ ");
					ISeStringEvaluator obj3 = DalamudServiceExtension.Instance<ISeStringEvaluator>();
					SeStringParameter reference3 = SeStringParameter.op_Implicit(config.AroundCountThresholdHigh);
					((ImU8String)(ref val9)).AppendFormatted<ReadOnlySeString>(obj3.EvaluateFromAddon(12871u, new Span<SeStringParameter>(ref reference3), (ClientLanguage?)null));
					if (ImGui.CollapsingHeader(val9, (ImGuiTreeNodeFlags)32))
					{
						DrawBattleEffectSetting("Medium", config.OverworldMedium);
					}
					ImU8String val10 = new ImU8String(2, 1);
					((ImU8String)(ref val10)).AppendLiteral("＞ ");
					ISeStringEvaluator obj4 = DalamudServiceExtension.Instance<ISeStringEvaluator>();
					SeStringParameter reference4 = SeStringParameter.op_Implicit(config.AroundCountThresholdHigh);
					((ImU8String)(ref val10)).AppendFormatted<ReadOnlySeString>(obj4.EvaluateFromAddon(12871u, new Span<SeStringParameter>(ref reference4), (ClientLanguage?)null));
					if (ImGui.CollapsingHeader(val10, (ImGuiTreeNodeFlags)32))
					{
						DrawBattleEffectSetting("High", config.OverworldHigh);
					}
				}
			}
			finally
			{
				((TabItemDisposable)(ref val2)).Dispose();
			}
			TabItemDisposable val11 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get("InDuty")));
			try
			{
				if (!TabItemDisposable.op_Implicit(val11))
				{
					return;
				}
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Default")));
				IndentDisposable val3 = ImRaii.PushIndent(1, true);
				try
				{
					DrawBattleEffectSetting("DefaultDuty", config.DutyDefault);
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
				ImGui.NewLine();
				Enumerator<ContentType> enumerator = LuminaGetter.Get<ContentType>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ContentType current = enumerator.Current;
						string text = ((object)((ContentType)(ref current)).Name/*cast due to constrained. prefix*/).ToString();
						if (string.IsNullOrEmpty(text))
						{
							continue;
						}
						if (config.DutySpecific.TryAdd(((ContentType)(ref current)).RowId, new EffectSetting()))
						{
							config.Save(this);
						}
						EffectSetting effectSetting = config.DutySpecific[((ContentType)(ref current)).RowId];
						if (ImageHelper.TryGetGameIcon(((ContentType)(ref current)).Icon, out IDalamudTextureWrap texture))
						{
							bool flag = false;
							ColorDisposable val12 = ImRaii.PushColor((ImGuiCol)0, KnownColor.LawnGreen.ToUInt(), effectSetting.IsEnabled);
							try
							{
								flag = ImGuiOm.TreeNodeImageWithText(texture.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), text ?? "", (ImGuiTreeNodeFlags)0);
							}
							finally
							{
								((IDisposable)val12)?.Dispose();
							}
							if (flag)
							{
								DrawBattleEffectSetting($"Duty_{((ContentType)(ref current)).RowId}", effectSetting);
								ImGui.TreePop();
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
			}
			finally
			{
				((TabItemDisposable)(ref val11)).Dispose();
			}
		}
		finally
		{
			((TabBarDisposable)(ref val)).Dispose();
		}
	}

	private void DrawBattleEffectSetting(string id, EffectSetting setting)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(id), true);
		try
		{
			bool value = setting.IsEnabled;
			if (ImGuiOm.ToggleButton("Enable", ref value))
			{
				setting.IsEnabled = value;
				config.Save(this);
			}
			ImGui.SameLine();
			ImGui.AlignTextToFramePadding();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Enable")));
			if (value)
			{
				ImGui.Spacing();
				ImGui.Spacing();
				BattleEffectLevel value2 = setting.Self;
				if (DrawBattleEffectLevelCombo(LuminaWrapper.GetAddonText(4087u), ref value2))
				{
					setting.Self = value2;
					config.Save(this);
				}
				ImGui.Spacing();
				BattleEffectLevel value3 = setting.Party;
				if (DrawBattleEffectLevelCombo(LuminaWrapper.GetAddonText(4088u), ref value3))
				{
					setting.Party = value3;
					config.Save(this);
				}
				ImGui.Spacing();
				BattleEffectLevel value4 = setting.Other;
				if (DrawBattleEffectLevelCombo(LuminaWrapper.GetAddonText(4089u), ref value4))
				{
					setting.Other = value4;
					config.Save(this);
				}
				ImGui.Spacing();
				BattleEffectLevel value5 = setting.Enemy;
				if (DrawBattleEffectLevelCombo(LuminaWrapper.GetAddonText(4109u), ref value5))
				{
					setting.Enemy = value5;
					config.Save(this);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static bool DrawBattleEffectLevelCombo(string label, ref BattleEffectLevel value)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(label), true);
		try
		{
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(label));
			IndentDisposable val2 = ImRaii.PushIndent(1, true);
			try
			{
				ImGui.Spacing();
				BattleEffectLevel[] values = Enum.GetValues<BattleEffectLevel>();
				foreach (BattleEffectLevel battleEffectLevel in values)
				{
					ImGui.SameLine(0f, 10f * Globals.GlobalUIScale);
					if (ImGui.RadioButton(ImU8String.op_Implicit(LuminaWrapper.GetAddonText((uint)(battleEffectLevel + 7823))), battleEffectLevel == value))
					{
						value = battleEffectLevel;
						result = true;
					}
				}
				return result;
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

	private void OnPlayerReceived(IReadOnlyList<IPlayerCharacter> characters)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EffectSetting effectSetting = null;
		if (GameState.ContentFinderCondition != 0)
		{
			Dictionary<uint, EffectSetting> dutySpecific = config.DutySpecific;
			ContentFinderCondition contentFinderConditionData = GameState.ContentFinderConditionData;
			if (dutySpecific.TryGetValue(((ContentFinderCondition)(ref contentFinderConditionData)).ContentType.RowId, out EffectSetting value) && value.IsEnabled)
			{
				effectSetting = value;
			}
			if ((object)effectSetting == null)
			{
				effectSetting = config.DutyDefault;
			}
		}
		else
		{
			int playersAroundCount = ManagerBase<PlayersManager>.Instance().PlayersAroundCount;
			effectSetting = ((playersAroundCount < config.AroundCountThresholdLow) ? config.OverworldLow : ((playersAroundCount >= config.AroundCountThresholdHigh) ? config.OverworldHigh : config.OverworldMedium));
		}
		if ((object)effectSetting != null && effectSetting.IsEnabled)
		{
			ApplySetting(effectSetting);
		}
	}

	private void ApplySetting(EffectSetting? settings)
	{
		if (settings == null || (lastAppliedSettings != null && settings == lastAppliedSettings))
		{
			return;
		}
		try
		{
			DalamudServiceExtension.Instance<IGameConfig>().UiConfig.Set("BattleEffectSelf", (uint)settings.Self);
			DalamudServiceExtension.Instance<IGameConfig>().UiConfig.Set("BattleEffectParty", (uint)settings.Party);
			DalamudServiceExtension.Instance<IGameConfig>().UiConfig.Set("BattleEffectOther", (uint)settings.Other);
			DalamudServiceExtension.Instance<IGameConfig>().UiConfig.Set("BattleEffectPvPEnemyPc", (uint)settings.Enemy);
			lastAppliedSettings = settings.Clone();
		}
		catch
		{
		}
	}
}
