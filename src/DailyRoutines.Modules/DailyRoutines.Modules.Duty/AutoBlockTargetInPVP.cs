using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.Duty;

public class AutoBlockTargetInPVP : ModuleBase
{
	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(FNOvU4jxNP);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(FNOvU4jxNP);
	}

	private unsafe static void FNOvU4jxNP(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (command == ExecuteCommandFlag.Target && GameState.IsInPVPArea && param1 != 3758096384u && ((CharacterManager)CharacterManager.Instance()).LookupBattleCharaByEntityId(param1) != null)
		{
			param1 = 3758096384u;
		}
	}

	public AutoBlockTargetInPVP()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD800E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F360446 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)),
			Category = ModuleCategory.Duty
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}
}
