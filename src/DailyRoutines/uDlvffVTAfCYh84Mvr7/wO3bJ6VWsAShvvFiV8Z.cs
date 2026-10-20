using System.Numerics;
using DailyRoutines.Internal;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace uDlvffVTAfCYh84Mvr7;

internal static class wO3bJ6VWsAShvvFiV8Z
{
	public static void w7XV16Dt7d()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPreComplexLocation(xeqV4B8VtJ);
	}

	public static void npnVMFukSB()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(xeqV4B8VtJ);
	}

	private static void xeqV4B8VtJ(ref bool isPrevented, ref ExecuteCommandComplexFlag command, ref Vector3 location, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (PluginConfig.Instance().CorrectEnvironment && command == ExecuteCommandComplexFlag.SetDivingState)
		{
			if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || !UIModuleExtension.IsScreenReady())
			{
				isPrevented = true;
				return;
			}
			command = ExecuteCommandComplexFlag.DiveEnd;
			param1 = RotationHelper.CharaToPacket(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer.Rotation);
			param2 = (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] ? 1u : 0u);
			DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3C898 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
		}
	}
}
