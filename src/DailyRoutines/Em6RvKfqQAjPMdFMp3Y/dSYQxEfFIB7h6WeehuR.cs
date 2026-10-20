using System;
using System.Collections.Generic;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using itAnDoW4mgYVlrmtMor;

namespace Em6RvKfqQAjPMdFMp3Y;

internal static class dSYQxEfFIB7h6WeehuR
{
	public static bool MlOfcRFDSn(ModuleBase module)
	{
		ArgumentNullException.ThrowIfNull(module, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -1781823936 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
		if (PluginConfig.Instance().ModuleFavorites.Contains(module.ModuleName))
		{
			return true;
		}
		foreach (HashSet<string> value in PluginConfig.Instance().ModuleCustoms.Values)
		{
			if (value.Contains(module.ModuleName))
			{
				return true;
			}
		}
		return false;
	}

	public static bool cA6fAjfWGn(ModuleBase module, string groupName)
	{
		ArgumentNullException.ThrowIfNull(module, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C4432 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
		ArgumentException.ThrowIfNullOrWhiteSpace(groupName, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F35BE18 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
		if (!(groupName == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329D85A4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)))
		{
			if (PluginConfig.Instance().ModuleCustoms.TryGetValue(groupName, out HashSet<string> value))
			{
				return value.Contains(module.ModuleName);
			}
			return false;
		}
		return PluginConfig.Instance().ModuleFavorites.Contains(module.ModuleName);
	}

	public static void WBPfQPGrOH(ModuleBase module, string groupName = "default")
	{
		ArgumentNullException.ThrowIfNull(module, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E461593 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
		ArgumentException.ThrowIfNullOrWhiteSpace(groupName, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816A5DE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd));
		bool num;
		if (!(groupName == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9075D72 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3)))
		{
			if (!PluginConfig.Instance().ModuleCustoms.TryGetValue(groupName, out HashSet<string> value))
			{
				return;
			}
			num = lWqfJwwa4V(value, module.ModuleName);
		}
		else
		{
			num = lWqfJwwa4V(PluginConfig.Instance().ModuleFavorites, module.ModuleName);
		}
		if (num)
		{
			PluginConfig.Instance().Save();
			ManagerBase<ModuleManager>.Instance().InvokeOnModuleChanged();
		}
	}

	private static bool lWqfJwwa4V(HashSet<string> modules, string moduleName)
	{
		if (modules.Remove(moduleName))
		{
			return true;
		}
		modules.Add(moduleName);
		return true;
	}
}
