using System;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Manager;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Attributes;

namespace DailyRoutines.Internal;

public static class PluginIPC
{
	[IPCProvider("DailyRoutines.IsModuleEnabled")]
	public static bool? IsModuleEnabled(string moduleName)
	{
		return ManagerBase<ModuleManager>.Instance().IsModuleEnabled(moduleName);
	}

	[IPCProvider("DailyRoutines.LoadModule")]
	public static bool LoadModule(string moduleName, bool affectConfig)
	{
		ModuleBase moduleByName = ManagerBase<ModuleManager>.Instance().GetModuleByName(moduleName);
		if (moduleByName == null)
		{
			return false;
		}
		moduleByName.Load(affectConfig);
		return true;
	}

	[IPCProvider("DailyRoutines.UnloadModule")]
	public static bool UnloadModule(string moduleName, bool affectConfig, bool forceUnload)
	{
		ModuleBase moduleByName = ManagerBase<ModuleManager>.Instance().GetModuleByName(moduleName);
		if (moduleByName == null)
		{
			return false;
		}
		moduleByName.Unload(affectConfig);
		return true;
	}

	[IPCProvider("DailyRoutines.Version")]
	public static Version GetVersion()
	{
		return Plugin.Version;
	}

	internal static void DkvVsbimBn()
	{
		IPCAttributeRegistry.RegStaticIPCs(typeof(PluginIPC));
	}

	internal static void T3gV89Lodq()
	{
		IPCAttributeRegistry.UnregStaticIPCs(typeof(PluginIPC));
	}
}
