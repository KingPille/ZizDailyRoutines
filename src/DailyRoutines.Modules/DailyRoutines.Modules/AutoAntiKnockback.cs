using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoAntiKnockback : ModuleBase
{
	private unsafe delegate byte oQ100IxnW34FccYgYcf(GameObject* gameObj, float rotation, float distance, float duration, byte a5, int rowData);

	private delegate byte qxOPIOxyUSyMGxCwilY(nint a1, nint a2, nint a3, float lockTime);

	private class s1HmWVxLeIUQf8sGnxJ : ModuleConfig
	{
		public AntiKnockbackMethod AntiKnockbackMethod;

		public float DistanceAdjustMultiplier;

		public s1HmWVxLeIUQf8sGnxJ()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			DistanceAdjustMultiplier = 1f;
			base._002Ector();
		}
	}

	private enum AntiKnockbackMethod
	{
		None,
		FastInPlace,
		InstantInPlace,
		NoMove,
		Reverse,
		AdjustDistance
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass18_0
	{
		public string args;

		public _003C_003Ec__DisplayClass18_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool aBsxHc5aj3(KeyValuePair<AntiKnockbackMethod, (string Title, string Descrption)> x)
		{
			return x.Key.ToString().Contains(args.Trim(), StringComparison.OrdinalIgnoreCase);
		}
	}

	private static readonly CompSig KnockbackHandlerSig;

	private Hook<oQ100IxnW34FccYgYcf>? KnockbackHandlerHook;

	private static readonly CompSig KnockbackSpeedHandlerSig;

	private Hook<qxOPIOxyUSyMGxCwilY>? KnockbackSpeedHandlerHook;

	private s1HmWVxLeIUQf8sGnxJ config;

	private (float Rotation, float Distance) lastKnockback;

	private const string COMMAND_REPLAY = "kbreplay";

	private const string COMMAND_SWITCH = "/pdrkb";

	private static readonly FrozenDictionary<AntiKnockbackMethod, (string Title, string Descrption)> MethodsLoc;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<s1HmWVxLeIUQf8sGnxJ>(this) ?? new s1HmWVxLeIUQf8sGnxJ();
		KnockbackHandlerHook = KnockbackHandlerSig.GetHook<oQ100IxnW34FccYgYcf>(nIJ1HNYglT);
		KnockbackHandlerHook.Enable();
		KnockbackSpeedHandlerHook = KnockbackSpeedHandlerSig.GetHook<qxOPIOxyUSyMGxCwilY>(S1I1SFr9PK);
		KnockbackSpeedHandlerHook.Enable();
		OmenServiceBase<CommandManager>.Instance().AddSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1E450 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912), new CommandInfo(new HandlerDelegate(sgb1yXpYha))
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x47538E2E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c))
		});
		OmenServiceBase<CommandManager>.Instance().AddCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E58BB0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3), new CommandInfo(new HandlerDelegate(Rqx1LEOd3K))
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF205D9D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372))
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E58A50 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
		OmenServiceBase<CommandManager>.Instance().RemoveCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44270CAA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b));
		lastKnockback = (Rotation: 0f, Distance: 0f);
	}

	protected override void ConfigUI()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD3AC6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc))))
		{
			ImU8String val = new ImU8String(1, 2);
			((ImU8String)(ref val)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A2549 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FCDB4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB914B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)));
			ImGui.TextUnformatted(val);
			ImU8String val2 = new ImU8String(6, 2);
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D568B42 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
			((ImU8String)(ref val2)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D568A3E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BD9F4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE2E82 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)));
			ImGui.TextUnformatted(val2);
		}
		ImGui.NewLine();
		if (!MethodsLoc.TryGetValue(config.AntiKnockbackMethod, out (string, string) value))
		{
			return;
		}
		using (ImRaiiExtension.Heading1(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D65F20 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)) ?? ""))
		{
			ImU8String val3 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32DAD39 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
			ImU8String val4 = new ImU8String(3, 2);
			((ImU8String)(ref val4)).AppendFormatted<string>(value.Item1);
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -536589662 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
			((ImU8String)(ref val4)).AppendFormatted<AntiKnockbackMethod>(config.AntiKnockbackMethod);
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E515F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
			ComboDisposable val5 = ImRaii.Combo(val3, val4);
			try
			{
				if (ComboDisposable.op_Implicit(val5))
				{
					AntiKnockbackMethod[] values = Enum.GetValues<AntiKnockbackMethod>();
					foreach (AntiKnockbackMethod antiKnockbackMethod in values)
					{
						if (MethodsLoc.TryGetValue(antiKnockbackMethod, out (string, string) value2))
						{
							ImU8String val6 = new ImU8String(3, 2);
							((ImU8String)(ref val6)).AppendFormatted<string>(value2.Item1);
							((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FDFD9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
							((ImU8String)(ref val6)).AppendFormatted<AntiKnockbackMethod>(antiKnockbackMethod);
							((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E18A3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec));
							if (ImGui.Selectable(val6, antiKnockbackMethod == config.AntiKnockbackMethod, (ImGuiSelectableFlags)0, default(Vector2)))
							{
								config.AntiKnockbackMethod = antiKnockbackMethod;
								config.Save(this);
							}
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val5)).Dispose();
			}
			if (!string.IsNullOrEmpty(value.Item2))
			{
				ImGui.TextWrapped(ImU8String.op_Implicit(value.Item2));
			}
			if (config.AntiKnockbackMethod == AntiKnockbackMethod.AdjustDistance)
			{
				ImGui.Spacing();
				if (ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB961F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9))), ref config.DistanceAdjustMultiplier, 0f, 0f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36951EA7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)), (ImGuiInputTextFlags)0))
				{
					config.DistanceAdjustMultiplier = Math.Max(0f, config.DistanceAdjustMultiplier);
				}
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					config.Save(this);
				}
			}
		}
	}

	private unsafe void sgb1yXpYha(string command, string args)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		KnockbackHandlerHook.Original((GameObject*)IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Address, lastKnockback.Rotation, lastKnockback.Distance, IntPtr.Zero, 0, 0);
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FFB4E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414), Math.Round(lastKnockback.Distance, 1))));
	}

	private void Rqx1LEOd3K(string command, string args)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass18_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass18_0();
		CS_0024_003C_003E8__locals3.args = args;
		if (!string.IsNullOrWhiteSpace(CS_0024_003C_003E8__locals3.args))
		{
			KeyValuePair<AntiKnockbackMethod, (string, string)> keyValuePair = MethodsLoc.FirstOrDefault<KeyValuePair<AntiKnockbackMethod, (string, string)>>((KeyValuePair<AntiKnockbackMethod, (string Title, string Descrption)> x) => x.Key.ToString().Contains(CS_0024_003C_003E8__locals3.args.Trim(), StringComparison.OrdinalIgnoreCase));
			if (keyValuePair.Value.Item1 != null)
			{
				string item = MethodsLoc[config.AntiKnockbackMethod].Title;
				config.AntiKnockbackMethod = keyValuePair.Key;
				config.Save(this);
				string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4E01E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55), item, keyValuePair.Value.Item1);
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
				NotifyHelper.Toast(text);
			}
		}
	}

	private unsafe byte nIJ1HNYglT(GameObject* gameObj, float rotation, float distance, float animationTime, byte a5, int rowData)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		lastKnockback = (Rotation: rotation, Distance: distance);
		string text;
		if (MethodsLoc.TryGetValue(config.AntiKnockbackMethod, out (string, string) value))
		{
			(text, _) = value;
		}
		else
		{
			text = string.Empty;
		}
		string text2 = text;
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A2357 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92), Math.Round(distance, 1))), useDefaultPrefix: false);
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854E0FF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5), text2)));
		switch (config.AntiKnockbackMethod)
		{
		case AntiKnockbackMethod.None:
		case AntiKnockbackMethod.FastInPlace:
		case AntiKnockbackMethod.InstantInPlace:
			return KnockbackHandlerHook.Original(gameObj, rotation, distance, animationTime, a5, rowData);
		case AntiKnockbackMethod.AdjustDistance:
			return KnockbackHandlerHook.Original(gameObj, rotation, distance * config.DistanceAdjustMultiplier, animationTime, a5, rowData);
		case AntiKnockbackMethod.Reverse:
			return KnockbackHandlerHook.Original(gameObj, 0f - rotation, distance + 1f, animationTime, a5, rowData);
		case AntiKnockbackMethod.NoMove:
			return 0;
		default:
			return KnockbackHandlerHook.Original(gameObj, rotation, distance, animationTime, a5, rowData);
		}
	}

	private byte S1I1SFr9PK(nint a1, nint a2, nint a3, float lockTime)
	{
		switch (config.AntiKnockbackMethod)
		{
		case AntiKnockbackMethod.None:
		case AntiKnockbackMethod.NoMove:
		case AntiKnockbackMethod.Reverse:
		case AntiKnockbackMethod.AdjustDistance:
			if (lockTime >= 2f)
			{
				lockTime = 0.8f;
			}
			break;
		case AntiKnockbackMethod.FastInPlace:
			lockTime = 0.5f;
			break;
		case AntiKnockbackMethod.InstantInPlace:
			lockTime = 0f;
			break;
		}
		return KnockbackSpeedHandlerHook.Original(a1, a2, a3, lockTime);
	}

	[IPCProvider("DailyRoutines.Modules.AutoAntiKnockback.ReplayKnockback")]
	private void KKL1VmEjGb()
	{
		sgb1yXpYha(string.Empty, string.Empty);
	}

	[IPCProvider("DailyRoutines.Modules.AutoAntiKnockback.ChangeMethod")]
	private bool xUY19g0gUK(int methodRaw)
	{
		if (!MethodsLoc.TryGetValue((AntiKnockbackMethod)methodRaw, out (string, string) value))
		{
			return false;
		}
		config.AntiKnockbackMethod = (AntiKnockbackMethod)methodRaw;
		config.Save(ManagerBase<ModuleManager>.Instance().GetModule<AutoAntiKnockback>());
		OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4A5B4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360), value.Item1));
		return true;
	}

	[IPCProvider("DailyRoutines.Modules.AutoAntiKnockback.AdjustDistanceMultiplier")]
	private void CXL1fYMyeg(float multiplier)
	{
		config.DistanceAdjustMultiplier = Math.Max(0f, multiplier);
		config.Save(ManagerBase<ModuleManager>.Instance().GetModule<AutoAntiKnockback>());
	}

	public AutoAntiKnockback()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C353890 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE2A30 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)),
			Category = ModuleCategory.Combat
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		lastKnockback = (Rotation: 0f, Distance: 0f);
		base._002Ector();
	}

	static AutoAntiKnockback()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		KnockbackHandlerSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423EA1B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71));
		KnockbackSpeedHandlerSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1B543 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c));
		MethodsLoc = new Dictionary<AntiKnockbackMethod, (string, string)>
		{
			[AntiKnockbackMethod.None] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BDDC6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)), string.Empty),
			[AntiKnockbackMethod.FastInPlace] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FDC57 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BB626 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848))),
			[AntiKnockbackMethod.InstantInPlace] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32DB6F7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF23126 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765))),
			[AntiKnockbackMethod.NoMove] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC95154 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FD738 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af))),
			[AntiKnockbackMethod.Reverse] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4B918 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD20F2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc))),
			[AntiKnockbackMethod.AdjustDistance] = (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x501745C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -716809874 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)))
		}.ToFrozenDictionary();
	}
}
