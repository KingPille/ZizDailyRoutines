using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class LimitlessTargetDashAction : ModuleBase
{
	private unsafe delegate uint Ic38kilzlKuhfhakl2Y(uint actionID, GameObject* source, GameObject* target);

	private static readonly CompSig GetActionInRangeOrLoSSig;

	private Hook<Ic38kilzlKuhfhakl2Y>? GetActionInRangeOrLoSHook;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		if (GetActionInRangeOrLoSHook == null)
		{
			GetActionInRangeOrLoSHook = GetActionInRangeOrLoSSig.GetHook<Ic38kilzlKuhfhakl2Y>(UZTTz2AXDC);
		}
		GetActionInRangeOrLoSHook.Enable();
	}

	private unsafe uint UZTTz2AXDC(uint actionID, GameObject* source, GameObject* target)
	{
		if (!LuminaGetter.TryGetRow<Action>(actionID, out Action item) || !((Action)(ref item)).AffectsPosition)
		{
			return GetActionInRangeOrLoSHook.Original(actionID, source, target);
		}
		return 0u;
	}

	public LimitlessTargetDashAction()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E0E9D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F77BC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)),
			Category = ModuleCategory.Action
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			CNPremium = true,
			TCPremium = true
		};
		base._002Ector();
	}

	static LimitlessTargetDashAction()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		GetActionInRangeOrLoSSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E589FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
	}
}
