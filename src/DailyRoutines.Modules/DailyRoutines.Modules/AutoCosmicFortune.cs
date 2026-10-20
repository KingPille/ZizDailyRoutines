using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoCosmicFortune : ModuleBase
{
	private class NU9ubtx8U5ykIh7SLEC : ModuleConfig
	{
		public bool AutoPull;

		public uint AutoPullCount;

		public uint AutoPullThreshold;

		public uint Delay;

		public NU9ubtx8U5ykIh7SLEC()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			AutoPull = true;
			AutoPullCount = 1u;
			AutoPullThreshold = 9500u;
			Delay = 50u;
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass14_0
	{
		public AutoCosmicFortune _003C_003E4__this;

		public uint lotteryType;

		public Action _003C_003E9__1;

		public _003C_003Ec__DisplayClass14_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void pdRxvEgPsn(int _)
		{
			_003C_003E4__this.TaskHelper.Enqueue(delegate
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartWKSLottery, lotteryType);
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ChooseWKSLotteryType, lotteryType);
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishWKSLottery, lotteryType);
			});
			if (_003C_003E4__this.config.Delay != 0)
			{
				_003C_003E4__this.TaskHelper.DelayNext((int)_003C_003E4__this.config.Delay);
			}
		}

		internal void DxbxliZ5n1()
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartWKSLottery, lotteryType);
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ChooseWKSLotteryType, lotteryType);
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishWKSLottery, lotteryType);
		}
	}

	private NU9ubtx8U5ykIh7SLEC config;

	private const string COMMAND = "cosfortune";

	private static readonly FrozenDictionary<uint, (uint CurrencyID, uint LotteryType)> ZoneToLotteryData;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<NU9ubtx8U5ykIh7SLEC>(this) ?? new NU9ubtx8U5ykIh7SLEC();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		Overlay? overlay3 = base.Overlay;
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags | 0x80);
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x6E26A9DA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5), new AddonEventDelegate(jHR1iGD8ed));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778BE89 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1), new AddonEventDelegate(jHR1iGD8ed));
		if ((*Addons.WKSHud).IsAddonAndNodesReady())
		{
			jHR1iGD8ed((AddonEvent)1, null);
		}
		OmenServiceBase<CommandManager>.Instance().AddSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39878067 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5), new CommandInfo(new HandlerDelegate(M7l1BI51oi))
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E0BE3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec))
		});
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPost(zXH1mCTKL0);
	}

	protected override void Uninit()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(zXH1mCTKL0);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { jHR1iGD8ed });
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7983F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
	}

	protected override void ConfigUI()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42E708 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd))));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(6, 2);
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50175C66 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
			((ImU8String)(ref val2)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCBB54 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00AE4A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854F2DF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)));
			ImGui.TextUnformatted(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(5, 1);
		((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x47539B56 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c)));
		((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1B9F1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c));
		ImU8String val4 = val3;
		ref uint delay = ref config.Delay;
		ImU8String val5 = default(ImU8String);
		if (ImGui.InputUInt(val4, ref delay, 100u, 100u, val5, (ImGuiInputTextFlags)0))
		{
			config.Delay = Math.Max(50u, config.Delay);
		}
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BA668 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)), 40f, (FontAwesomeIcon)61530);
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14BC85 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6))), ref config.AutoPull))
		{
			config.Save(this);
		}
		ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -2005832046 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd), config.AutoPullThreshold, config.AutoPullCount));
		if (config.AutoPull)
		{
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			((ImU8String)(ref val5))._002Ector(10, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x177881E1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)));
			((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FF3A7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
			ImU8String val6 = val5;
			ref uint autoPullThreshold = ref config.AutoPullThreshold;
			ImU8String val7 = default(ImU8String);
			if (ImGui.InputUInt(val6, ref autoPullThreshold, 1000u, 1000u, val7, (ImGuiInputTextFlags)0))
			{
				config.AutoPullThreshold = Math.Clamp(config.AutoPullThreshold, 1000u, 10000u);
				config.AutoPullCount = Math.Clamp(config.AutoPullCount, 1u, config.AutoPullThreshold / 1000);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			((ImU8String)(ref val7))._002Ector(10, 1);
			((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5069D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)));
			((ImU8String)(ref val7)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -243236714 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273));
			if (ImGui.InputUInt(val7, ref config.AutoPullCount, 1u, 1u, default(ImU8String), (ImGuiInputTextFlags)0))
			{
				config.AutoPullCount = Math.Clamp(config.AutoPullCount, 1u, config.AutoPullThreshold / 1000);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
		}
		ImGui.NewLine();
	}

	protected unsafe override void OverlayUI()
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.WKSHud == null)
		{
			((Window)base.Overlay).IsOpen = false;
		}
		else
		{
			if (!(*Addons.WKSHud).IsAddonAndNodesReady() || !ZoneToLotteryData.TryGetValue(GameState.TerritoryType, out (uint, uint) value))
			{
				return;
			}
			ImGui.SetWindowPos((*((AtkUnitBase)Addons.WKSHud).RootNode).GetNodeState().BottomLeft, (ImGuiCond)0);
			foreach (KeyValuePair<uint, (uint, uint)> zoneToLotteryDatum in ZoneToLotteryData)
			{
				zoneToLotteryDatum.Deconstruct(out var _, out value);
				(uint, uint) tuple = value;
				uint item = tuple.Item1;
				uint item2 = tuple.Item2;
				uint num = ((CurrencyManager)CurrencyManager.Instance()).GetItemCount(item) / 1000;
				if (num == 0 || !ImageHelper.TryGetGameIcon(LuminaWrapper.GetItemIconID(item), out IDalamudTextureWrap texture))
				{
					continue;
				}
				ImU8String val = new ImU8String(1, 2);
				((ImU8String)(ref val)).AppendFormatted<uint>(item);
				((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FB928 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
				((ImU8String)(ref val)).AppendFormatted<uint>(item2);
				IdDisposable val2 = ImRaii.PushId(val, true);
				try
				{
					ImGui.Image(texture.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
					ImGui.SameLine();
					ImU8String val3 = new ImU8String(0, 1);
					((ImU8String)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetItemName(item));
					ImGui.TextUnformatted(val3);
					if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE31CD3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818), 1)), default(Vector2)))
					{
						slC1P6VbJq(1u, item2);
					}
					if (num > 1)
					{
						ImGui.SameLine();
						if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -1891875626 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), num)), default(Vector2)))
						{
							slC1P6VbJq(num, item2);
						}
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
		}
	}

	private unsafe void M7l1BI51oi(string command, string arguments)
	{
		if (!ZoneToLotteryData.TryGetValue(GameState.TerritoryType, out (uint, uint) value))
		{
			return;
		}
		arguments = arguments.Trim();
		uint num = ((CurrencyManager)CurrencyManager.Instance()).GetItemCount(value.Item1) / 1000;
		if (num != 0)
		{
			if (!int.TryParse(arguments, out var result) || result >= num)
			{
				slC1P6VbJq(num, value.Item2);
			}
			else
			{
				slC1P6VbJq((uint)result, value.Item2);
			}
		}
	}

	private unsafe void zXH1mCTKL0(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4)
	{
		if (command != ExecuteCommandFlag.RefreshInventory || !ZoneToLotteryData.TryGetValue(GameState.TerritoryType, out (uint, uint) value) || !config.AutoPull)
		{
			return;
		}
		uint itemCount = ((CurrencyManager)CurrencyManager.Instance()).GetItemCount(value.Item1);
		if (itemCount >= config.AutoPullThreshold)
		{
			uint num = itemCount / 1000;
			if (num != 0)
			{
				slC1P6VbJq(Math.Min(config.AutoPullCount, num), value.Item2);
			}
		}
	}

	private void jHR1iGD8ed(AddonEvent type, AddonArgs args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		Overlay overlay = base.Overlay;
		bool isOpen = (int)type == 1 || ((int)type != 6 && ((Window)base.Overlay).IsOpen);
		((Window)overlay).IsOpen = isOpen;
	}

	private void slC1P6VbJq(uint times, uint lotteryType)
	{
		_003C_003Ec__DisplayClass14_0 CS_0024_003C_003E8__locals9 = new _003C_003Ec__DisplayClass14_0();
		CS_0024_003C_003E8__locals9._003C_003E4__this = this;
		CS_0024_003C_003E8__locals9.lotteryType = lotteryType;
		Enumerable.Range(0, (int)times).ForEach(delegate
		{
			CS_0024_003C_003E8__locals9._003C_003E4__this.TaskHelper.Enqueue(delegate
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartWKSLottery, CS_0024_003C_003E8__locals9.lotteryType);
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ChooseWKSLotteryType, CS_0024_003C_003E8__locals9.lotteryType);
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishWKSLottery, CS_0024_003C_003E8__locals9.lotteryType);
			});
			if (CS_0024_003C_003E8__locals9._003C_003E4__this.config.Delay != 0)
			{
				CS_0024_003C_003E8__locals9._003C_003E4__this.TaskHelper.DelayNext((int)CS_0024_003C_003E8__locals9._003C_003E4__this.config.Delay);
			}
		});
	}

	public AutoCosmicFortune()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6746A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854CCA9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39878067 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)),
			Category = ModuleCategory.General
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoCosmicFortune()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ZoneToLotteryData = new Dictionary<uint, (uint, uint)>
		{
			[1237u] = (45691u, 0u),
			[1291u] = (48146u, 1u),
			[1310u] = (48147u, 2u),
			[1319u] = (48148u, 3u)
		}.ToFrozenDictionary();
	}
}
