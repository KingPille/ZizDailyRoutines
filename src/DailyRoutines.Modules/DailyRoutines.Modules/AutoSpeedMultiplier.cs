using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoSpeedMultiplier : ModuleBase
{
	private delegate float P1LBMXp1EFjxuXrMPHi(nint a1);

	private class etyoGPpMcd4bUtVuu6j : ModuleConfig
	{
		public wMDG5Mp4sAVXERQqjbd DutyConfig;

		public wMDG5Mp4sAVXERQqjbd NormalConfig;

		public Dictionary<uint, wMDG5Mp4sAVXERQqjbd> ZoneConfigs;

		public etyoGPpMcd4bUtVuu6j()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			DutyConfig = new wMDG5Mp4sAVXERQqjbd
			{
				WithPlayers = -1f
			};
			NormalConfig = new wMDG5Mp4sAVXERQqjbd();
			ZoneConfigs = new Dictionary<uint, wMDG5Mp4sAVXERQqjbd>();
			base._002Ector();
		}
	}

	private class wMDG5Mp4sAVXERQqjbd
	{
		public float InCombat;

		public float Normal;

		public float WithPlayers;

		public float MblpsHCdxm()
		{
			if (InCombat > 0f && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
			{
				return InCombat;
			}
			if (WithPlayers > 0f && ManagerBase<PlayersManager>.Instance().PlayersAroundCount > 0)
			{
				return WithPlayers;
			}
			if (Normal > 0f)
			{
				return Normal;
			}
			return 1f;
		}

		public wMDG5Mp4sAVXERQqjbd()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			InCombat = 1.12f;
			Normal = 1f;
			WithPlayers = 1f;
			base._002Ector();
		}
	}

	private static readonly CompSig SpeedMultiplierCalculateSig;

	private Hook<P1LBMXp1EFjxuXrMPHi>? SpeedMultiplierCalculateHook;

	private etyoGPpMcd4bUtVuu6j config;

	private ZoneSelectCombo zoneSelectCombo;

	private bool isLastZoneSpeedLimited;

	private float lastSpeedMultiplier;

	private float? tempMultiplier;

	private const string COMMAND = "/pdrspeed";

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		zoneSelectCombo = new ZoneSelectCombo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E8306 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1));
		config = ModuleConfigExtension.Load<etyoGPpMcd4bUtVuu6j>(this) ?? new etyoGPpMcd4bUtVuu6j();
		if (SpeedMultiplierCalculateHook == null)
		{
			SpeedMultiplierCalculateHook = SpeedMultiplierCalculateSig.GetHook<P1LBMXp1EFjxuXrMPHi>(lO34yyEnTQ);
		}
		SpeedMultiplierCalculateHook.Enable();
		OmenServiceBase<CommandManager>.Instance().AddCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42D20C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd), new CommandInfo(new HandlerDelegate(LmjMz1d5kC))
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B876C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848))
		});
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += eQe4nIag0C;
		eQe4nIag0C(0u);
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= eQe4nIag0C;
		OmenServiceBase<CommandManager>.Instance().RemoveCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56BE6E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
		isLastZoneSpeedLimited = false;
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4E740 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55))))
		{
			ImU8String val = new ImU8String(3, 2);
			((ImU8String)(ref val)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC9A9E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361));
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A1091 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357E76A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb)));
			ImGui.TextUnformatted(val);
		}
		ImGui.NewLine();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FC9D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414))));
		TabBarDisposable val2 = ImRaii.TabBar(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17419D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)));
		try
		{
			if (!val2)
			{
				return;
			}
			IdDisposable val3 = ImRaii.PushId(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC97EEE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)), true);
			try
			{
				TabItemDisposable val4 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E2BCF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec))));
				try
				{
					if (TabItemDisposable.op_Implicit(val4))
					{
						yqvMYf4JyA(ref config.DutyConfig, this);
					}
				}
				finally
				{
					((TabItemDisposable)(ref val4)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			val3 = ImRaii.PushId(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4D34A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)), true);
			try
			{
				TabItemDisposable val5 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E2B11 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec))));
				try
				{
					if (TabItemDisposable.op_Implicit(val5))
					{
						yqvMYf4JyA(ref config.NormalConfig, this);
					}
				}
				finally
				{
					((TabItemDisposable)(ref val5)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			val3 = ImRaii.PushId(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -1075658333 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)), true);
			try
			{
				TabItemDisposable val6 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017681C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf))));
				try
				{
					if (!TabItemDisposable.op_Implicit(val6))
					{
						return;
					}
					ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
					zoneSelectCombo.DrawRadio();
					ImGui.SameLine();
					DisabledDisposable val7 = ImRaii.Disabled(zoneSelectCombo.SelectedID == 0);
					try
					{
						GroupDisposable val8 = ImRaii.Group();
						try
						{
							DisabledDisposable val9 = ImRaii.Disabled(config.ZoneConfigs.ContainsKey(zoneSelectCombo.SelectedID));
							try
							{
								if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1EFD5A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1))))
								{
									config.ZoneConfigs.TryAdd(zoneSelectCombo.SelectedID, new wMDG5Mp4sAVXERQqjbd());
									config.Save(this);
								}
							}
							finally
							{
								((IDisposable)val9)?.Dispose();
							}
							ImGui.SameLine();
							val9 = ImRaii.Disabled(!config.ZoneConfigs.ContainsKey(zoneSelectCombo.SelectedID));
							try
							{
								if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695599B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9))))
								{
									config.ZoneConfigs.Remove(zoneSelectCombo.SelectedID);
									config.Save(this);
								}
							}
							finally
							{
								((IDisposable)val9)?.Dispose();
							}
						}
						finally
						{
							((GroupDisposable)(ref val8)).Dispose();
						}
					}
					finally
					{
						((IDisposable)val7)?.Dispose();
					}
					if (zoneSelectCombo.SelectedID != 0 && config.ZoneConfigs.TryGetValue(zoneSelectCombo.SelectedID, out wMDG5Mp4sAVXERQqjbd value))
					{
						yqvMYf4JyA(ref value, this);
					}
				}
				finally
				{
					((TabItemDisposable)(ref val6)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((TabBarDisposable)(ref val2)).Dispose();
		}
	}

	private void yqvMYf4JyA(ref wMDG5Mp4sAVXERQqjbd multiplierConfig, ModuleBase instance)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x501717FC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf))));
		ImGui.NewLine();
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14E3E1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6))), ref multiplierConfig.InCombat, 0.1f, 1f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E546D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(instance);
		}
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35765E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def))), ref multiplierConfig.WithPlayers, 0.1f, 1f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987DFF1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(instance);
		}
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09ECFF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363))), ref multiplierConfig.Normal, 0.1f, 1f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36955961 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(instance);
		}
	}

	private void LmjMz1d5kC(string command, string args)
	{
		if (float.TryParse(args, out var result))
		{
			d3h4H4I0Ob(result);
		}
	}

	private void eQe4nIag0C(uint u)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		tempMultiplier = null;
		if (!Sheets.SpeedDetectionZones.ContainsKey(GameState.TerritoryType))
		{
			if (isLastZoneSpeedLimited)
			{
				string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1AFB0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text), useDefaultPrefix: false);
				NotifyHelper.Toast(text);
			}
			isLastZoneSpeedLimited = false;
			return;
		}
		TerritoryType territoryTypeData = GameState.TerritoryTypeData;
		if (((TerritoryType)(ref territoryTypeData)).RowId != 0)
		{
			if (!isLastZoneSpeedLimited)
			{
				string text2 = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35771C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text2), useDefaultPrefix: false);
				NotifyHelper.Toast(text2);
			}
			isLastZoneSpeedLimited = true;
		}
	}

	private float lO34yyEnTQ(nint a1)
	{
		return lastSpeedMultiplier = SpeedMultiplierCalculateHook.Original(a1) * koq4LqnkQe();
	}

	private float koq4LqnkQe()
	{
		float? num = tempMultiplier;
		if (num.HasValue && num.GetValueOrDefault() > 0f)
		{
			return tempMultiplier.Value;
		}
		if (config.ZoneConfigs.TryGetValue(GameState.TerritoryType, out wMDG5Mp4sAVXERQqjbd value))
		{
			return value.MblpsHCdxm();
		}
		if (GameState.ContentFinderCondition != 0)
		{
			return config.DutyConfig.MblpsHCdxm();
		}
		return config.NormalConfig.MblpsHCdxm();
	}

	private void d3h4H4I0Ob(float multiplier)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		if (multiplier <= 0f)
		{
			tempMultiplier = null;
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4EA68 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360))));
			return;
		}
		string text = tempMultiplier?.ToString() ?? null;
		tempMultiplier = Math.Clamp(multiplier, 0f, 10f);
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36955BE9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), text ?? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36BAA8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)), tempMultiplier)));
	}

	[IPCProvider("DailyRoutines.Modules.AutoSpeedMultiplier.ChangeMultiplier")]
	private void UbJ4SpZXFy(float multiplier)
	{
		d3h4H4I0Ob(multiplier);
	}

	[IPCProvider("DailyRoutines.Modules.AutoSpeedMultiplier.GetMultiplier")]
	private float poV4VrsvuY()
	{
		return lastSpeedMultiplier;
	}

	public AutoSpeedMultiplier()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36F788 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32DE42F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD0FC2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoSpeedMultiplier()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		SpeedMultiplierCalculateSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1ADAC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
	}
}
