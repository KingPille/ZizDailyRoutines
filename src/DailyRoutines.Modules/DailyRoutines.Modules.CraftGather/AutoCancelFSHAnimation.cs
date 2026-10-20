using System.Collections.Frozen;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;
using h5qdDtTZd16pjL1WuuQ;

namespace DailyRoutines.Modules.CraftGather;

public class AutoCancelFSHAnimation : ModuleBase
{
	private class UAsVn1i5flDwxi9esTB : ModuleConfig
	{
		public bool DisableWhenPlayersAround;

		public UAsVn1i5flDwxi9esTB()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			DisableWhenPlayersAround = true;
			base._002Ector();
		}
	}

	private UAsVn1i5flDwxi9esTB config;

	private bool isAddedFish;

	private static readonly FrozenSet<string> FishingAnimations;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<UAsVn1i5flDwxi9esTB>(this) ?? new UAsVn1i5flDwxi9esTB();
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(a3xl2ji00t);
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)43])
		{
			a3xl2ji00t((ConditionFlag)43, value: true);
		}
	}

	protected override void Uninit()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(a3xl2ji00t);
		a3xl2ji00t((ConditionFlag)43, value: false);
	}

	protected override void ConfigUI()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FB24F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009))), ref config.DisableWhenPlayersAround))
		{
			config.Save(this);
		}
	}

	private void a3xl2ji00t(ConditionFlag flag, bool value)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		if ((int)flag == 43)
		{
			OmenServiceBase<FrameworkManager>.Instance().Unreg(iNClZNPW95);
			OmenServiceBase<GameResourceManager>.Instance().RemoveFromBlacklist(typeof(AutoCancelFSHAnimation), FishingAnimations);
			isAddedFish = false;
			if (value)
			{
				OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(iNClZNPW95), 1000u);
			}
		}
	}

	private void iNClZNPW95(IFramework _)
	{
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)43])
		{
			a3xl2ji00t((ConditionFlag)43, value: false);
		}
		else if (config.DisableWhenPlayersAround && ManagerBase<PlayersManager>.Instance().PlayersAroundCount > 0)
		{
			if (isAddedFish)
			{
				OmenServiceBase<GameResourceManager>.Instance().RemoveFromBlacklist(typeof(AutoCancelFSHAnimation), FishingAnimations);
				isAddedFish = false;
			}
		}
		else if (!isAddedFish)
		{
			OmenServiceBase<GameResourceManager>.Instance().AddToBlacklist(typeof(AutoCancelFSHAnimation), FishingAnimations);
			isAddedFish = true;
		}
	}

	public AutoCancelFSHAnimation()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7838C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7940F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f)),
			Category = ModuleCategory.CraftGather
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		base._002Ector();
	}

	static AutoCancelFSHAnimation()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		K2c2C8Tkd807fch57gT<string> buffer = default(K2c2C8Tkd807fch57gT<string>);
		buffer[0] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A9123 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92);
		buffer[1] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD83831 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3);
		buffer[2] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C2EA5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee);
		buffer[3] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23109D53 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d);
		buffer[4] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50179794 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf);
		buffer[5] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACACF2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09);
		buffer[6] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35F65E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def);
		buffer[7] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53515B14 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08);
		buffer[8] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75CEE6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1);
		buffer[9] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6955A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560);
		buffer[10] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F3526 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414);
		buffer[11] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C146109 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6);
		buffer[12] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F3586 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414);
		buffer[13] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBDF3B6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc);
		buffer[14] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -471672063 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c);
		buffer[15] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109CB696 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948);
		buffer[16] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD84451 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3);
		buffer[17] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39875B61 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5);
		buffer[18] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7EF9B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f);
		buffer[19] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBDF5BA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc);
		buffer[20] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F327C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414);
		buffer[21] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF12B10 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912);
		buffer[22] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D6333 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e);
		buffer[23] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5DF86 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd);
		buffer[24] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50179170 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf);
		buffer[25] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x17786741 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1);
		buffer[26] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E0062BA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa);
		buffer[27] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E7ADC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1);
		buffer[28] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7FAAA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846);
		buffer[29] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695D1E5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9);
		buffer[30] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17B61D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb);
		buffer[31] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2FB792 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7);
		buffer[32] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7F43C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846);
		buffer[33] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951DE01 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497);
		buffer[34] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5EC03 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7);
		buffer[35] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF209CB1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372);
		buffer[36] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D564A52 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1);
		buffer[37] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACA776 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09);
		buffer[38] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -1781077508 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3);
		buffer[39] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F0DCA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af);
		buffer[40] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C5D2B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee);
		buffer[41] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x231091F9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d);
		buffer[42] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E0068E8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa);
		buffer[43] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1772D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c);
		buffer[44] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469AE3C1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92);
		buffer[45] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C146D67 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6);
		buffer[46] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -887932287 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372);
		buffer[47] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75C1A4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1);
		buffer[48] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D564FCE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1);
		buffer[49] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C146CF1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6);
		buffer[50] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42236A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd);
		buffer[51] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x177871E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1);
		buffer[52] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F0301 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009);
		buffer[53] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F48DC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825);
		buffer[54] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17AF3F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb);
		buffer[55] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23108CBD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d);
		buffer[56] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F1432 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af);
		buffer[57] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB4867 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9);
		buffer[58] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39874DE3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5);
		buffer[59] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39874C0D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5);
		buffer[60] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5F595 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7);
		buffer[61] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35E546 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def);
		buffer[62] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x6B524DD6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765);
		buffer[63] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D16EDF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c);
		buffer[64] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854396D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5);
		buffer[65] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB4F45 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9);
		buffer[66] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017826A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf);
		buffer[67] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39874937 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5);
		buffer[68] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5CF82 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd);
		buffer[69] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C47A9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee);
		buffer[70] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D16D69 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c);
		buffer[71] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7EA36 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846);
		buffer[72] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5CE56 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd);
		buffer[73] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3178997 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273);
		buffer[74] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35EE32 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def);
		buffer[75] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B60C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848);
		buffer[76] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2FA700 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7);
		buffer[77] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF208CDB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372);
		buffer[78] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D565AB8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1);
		buffer[79] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3E6B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818);
		buffer[80] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5C31C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd);
		buffer[81] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x685436C9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5);
		buffer[82] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4E973 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7);
		buffer[83] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D7ACB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e);
		buffer[84] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACB02C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09);
		buffer[85] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427DB0E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b);
		buffer[86] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B6534 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848);
		buffer[87] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109CA858 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948);
		buffer[88] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D68920 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560);
		buffer[89] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE090D4F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363);
		buffer[90] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x231083DB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d);
		buffer[91] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5C72C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd);
		buffer[92] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695C6AB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9);
		buffer[93] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D68BFC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560);
		buffer[94] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D165FD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c);
		buffer[95] = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427E798 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b);
		FishingAnimations = FrozenSet.Create<string>(buffer);
	}
}
