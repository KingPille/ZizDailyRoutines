using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using OmenTools.Interop.Game;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.Duty;

public class AutoUnlockTellInDuty : ModuleBase
{
	private MemoryPatch publicTellPatch;

	private MemoryPatch dutyTellPatch;

	private MemoryPatch mordionPublicTellPatch;

	private MemoryPatch forayIsDutyRecorderPlayback0Patch;

	private MemoryPatch forayIsDutyRecorderPlayback1Patch;

	private MemoryPatch forayReasonPatch;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		publicTellPatch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75BDF4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1), new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		dutyTellPatch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D15CF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e), new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		mordionPublicTellPatch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35858E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def), new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		forayIsDutyRecorderPlayback0Patch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA45F46 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55), new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		forayIsDutyRecorderPlayback1Patch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194F0F0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b), new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		forayReasonPatch = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3503C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[6]
		{
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		publicTellPatch.Enable();
		dutyTellPatch.Enable();
		mordionPublicTellPatch.Enable();
		forayIsDutyRecorderPlayback0Patch.Enable();
		forayIsDutyRecorderPlayback1Patch.Enable();
		forayReasonPatch.Enable();
	}

	public AutoUnlockTellInDuty()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D789E6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454EB285 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467)),
			Category = ModuleCategory.Duty
		};
		Permission = new ModulePermission
		{
			AllDefaultEnabled = true
		};
		base._002Ector();
	}
}
