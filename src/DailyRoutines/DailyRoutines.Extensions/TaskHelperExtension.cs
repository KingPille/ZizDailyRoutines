using System;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.Extensions;

public static class TaskHelperExtension
{
	extension(TaskHelper instance)
	{
		public bool AbortByConflictKey(ModuleBase module)
		{
			ArgumentNullException.ThrowIfNull(instance, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9FDDF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a));
			ArgumentNullException.ThrowIfNull(module, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C9072 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
			if (PluginConfig.Instance().ConflictKeyBinding.IsPressed())
			{
				instance.Abort();
				OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(module.Info.Title, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E255437 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25)));
				return true;
			}
			return false;
		}
	}
}
