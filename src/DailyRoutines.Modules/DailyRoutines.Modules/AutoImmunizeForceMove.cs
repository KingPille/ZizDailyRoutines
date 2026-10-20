using System;
using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoImmunizeForceMove : ModuleBase
{
	private unsafe delegate void njc5LlxOPcxiwlvbSZH(Character* chara, float destX, float destY, float destZ, int rotationLow, nint a6);

	private static readonly CompSig ForceMarchSig;

	private Hook<njc5LlxOPcxiwlvbSZH>? ForceMarchHook;

	private static readonly FrozenSet<ConditionFlag> StatusFlags;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (ForceMarchHook == null)
		{
			ForceMarchHook = ForceMarchSig.GetHook<njc5LlxOPcxiwlvbSZH>(NJn1TYLbSS);
		}
		ForceMarchHook.Enable();
	}

	private unsafe void NJn1TYLbSS(Character* chara, float destX, float destY, float destZ, int rotationLow, nint a6)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (((Character)chara).EntityId != IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.EntityID)
		{
			ForceMarchHook.Original(chara, destX, destY, destZ, rotationLow, a6);
		}
		foreach (ConditionFlag statusFlag in StatusFlags)
		{
			MemoryHelper.Write<bool>((IntPtr)((nint)DalamudServiceExtension.Instance<ICondition>().Address + (nint)statusFlag), false);
		}
	}

	public AutoImmunizeForceMove()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -798915648 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E501B6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)),
			Category = ModuleCategory.Combat
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		base._002Ector();
	}

	static AutoImmunizeForceMove()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ForceMarchSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE53E81 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
		StatusFlags = FrozenSet.Create(RuntimeHelpers.CreateSpan<ConditionFlag>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));
	}
}
