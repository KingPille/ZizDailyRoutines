using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class ProhibitMountFlightRestrction : ModuleBase
{
	private delegate int n9lCGq3pqtBtUplp37T();

	private CompSig isFlightProhibitedSig;

	private Hook<n9lCGq3pqtBtUplp37T>? IsFlightProhibitedHook;

	private MemoryPatch aetherCurrentInterruptPatch;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		isFlightProhibitedSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42888C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd));
		aetherCurrentInterruptPatch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E61AB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[2]
		{
			(byte)144,
			(byte)233
		}), 2);
		aetherCurrentInterruptPatch.Enable();
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(hnVspB6EiR);
		if (IsFlightProhibitedHook == null)
		{
			IsFlightProhibitedHook = isFlightProhibitedSig.GetHook<n9lCGq3pqtBtUplp37T>(EtQsxQPyY8);
		}
		IsFlightProhibitedHook.Enable();
	}

	protected override void Uninit()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(hnVspB6EiR);
	}

	private int EtQsxQPyY8()
	{
		int num = IsFlightProhibitedHook.Original();
		if (num != 1)
		{
			return num;
		}
		return 0;
	}

	private static void hnVspB6EiR(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (command == ExecuteCommandFlag.DisableMounting)
		{
			if (param1 == 1)
			{
				isPrevented = true;
			}
			param1 = 0u;
		}
	}

	public ProhibitMountFlightRestrction()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E45A97 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8EF2B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}
}
