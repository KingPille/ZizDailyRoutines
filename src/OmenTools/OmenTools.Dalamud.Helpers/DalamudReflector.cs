using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Dalamud.Plugin;
using OmenTools.Extensions;

namespace OmenTools.Dalamud.Helpers;

public static class DalamudReflector
{
	private const BindingFlags ALL_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	public static nint GetMemberFuncByName(Type staticType, string propertyName)
	{
		return (nint)(staticType.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public)?.GetValue(null) ?? throw new MissingMemberException(staticType.FullName, propertyName));
	}

	public static object GetPluginManager()
	{
		return ((object)DalamudServiceExtension.Instance()).GetType().Assembly.GetType("Dalamud.Service`1", throwOnError: true)?.MakeGenericType(((object)DalamudServiceExtension.Instance()).GetType().Assembly.GetType("Dalamud.Plugin.Internal.PluginManager", throwOnError: true)).GetMethod("Get")?.Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
	}

	public static async Task<List<object>> GetPluginMaster(string masterURL)
	{
		List<object> result;
		try
		{
			object service = GetService("Dalamud.Networking.Http.HappyHttpClient");
			object pluginRepository = Activator.CreateInstance(((object)DalamudServiceExtension.Instance()).GetType().Assembly.GetType("Dalamud.Plugin.Internal.Types.PluginRepository"), service, masterURL, true);
			if (pluginRepository == null)
			{
				return null;
			}
			Task<bool> reloadPluginMasterTask = TryReload(pluginRepository, "ReloadPluginMasterAsync");
			Task<bool> reloadAsyncTask = TryReload(pluginRepository, "ReloadAsync");
			Task<bool> firstCompleted = await Task.WhenAny(reloadPluginMasterTask, reloadAsyncTask);
			if (!(await firstCompleted) && !(await ((firstCompleted == reloadPluginMasterTask) ? reloadAsyncTask : reloadPluginMasterTask)))
			{
				return null;
			}
			result = (pluginRepository.GetType().GetProperty("PluginMaster").GetValue(pluginRepository) as IEnumerable)?.Cast<object>().ToList();
		}
		catch (Exception)
		{
			return null;
		}
		return result;
		static async Task<bool> TryReload(object repository, string methodName)
		{
			try
			{
				Task task = repository.Call<Task>(methodName, Array.Empty<object>());
				if (task == null)
				{
					return false;
				}
				await task;
				return true;
			}
			catch
			{
				return false;
			}
		}
	}

	public static object GetService(string serviceFullName)
	{
		return ((object)DalamudServiceExtension.Instance()).GetType().Assembly.GetType("Dalamud.Service`1", throwOnError: true)?.MakeGenericType(((object)DalamudServiceExtension.Instance()).GetType().Assembly.GetType(serviceFullName, throwOnError: true)).GetMethod("Get")?.Invoke(null, BindingFlags.Default, null, Array.Empty<object>(), null);
	}

	public static bool TryGetLocalPlugin(IDalamudPlugin instance, out object localPlugin, out AssemblyLoadContext context, out Type type)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Invalid comparison between Unknown and O
		type = null;
		context = null;
		localPlugin = null;
		try
		{
			object pluginManager = GetPluginManager();
			IList list = (IList)(pluginManager.GetType().GetProperty("InstalledPlugins")?.GetValue(pluginManager));
			if (list == null)
			{
				return false;
			}
			foreach (object item in list)
			{
				type = ((item.GetType().Name == "LocalDevPlugin") ? item.GetType().BaseType : item.GetType());
				if (!(type == null) && (object)(IDalamudPlugin)(type.GetField("instance", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(item)) == instance)
				{
					localPlugin = item;
					context = type.GetField("loader", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(item)?.GetFieldOrProperty<AssemblyLoadContext>("context");
					return true;
				}
			}
			return false;
		}
		catch
		{
		}
		return false;
	}

	public static bool HasRepo(string repoURL)
	{
		IEnumerable enumerable = (IEnumerable)GetService("Dalamud.Configuration.Internal.DalamudConfiguration").GetFieldOrProperty("ThirdRepoList");
		if (enumerable != null)
		{
			foreach (object item in enumerable)
			{
				if ((string)item.GetFieldOrProperty("Url") == repoURL)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void AddRepo(string repoURL, bool enabled)
	{
		object service = GetService("Dalamud.Configuration.Internal.DalamudConfiguration");
		IEnumerable enumerable = (IEnumerable)service.GetFieldOrProperty("ThirdRepoList");
		if (enumerable != null)
		{
			foreach (object item in enumerable)
			{
				if ((string)item.GetFieldOrProperty("Url") == repoURL)
				{
					return;
				}
			}
		}
		object obj = Activator.CreateInstance(((object)DalamudServiceExtension.Instance()).GetType().Assembly.GetType("Dalamud.Configuration.ThirdPartyRepoSettings"));
		if (obj != null)
		{
			obj.SetFieldOrProperty("Url", repoURL);
			obj.SetFieldOrProperty("IsEnabled", enabled);
			service.GetFieldOrProperty<IList>("ThirdRepoList").Add(obj);
		}
	}

	public static async Task<bool> AddPlugin(string masterURL, string pluginInternalName)
	{
		List<object> list = await GetPluginMaster(masterURL);
		if (list == null || list.Count == 0)
		{
			return false;
		}
		object obj = list.FirstOrDefault((object x) => (string)x.GetFieldOrProperty("InternalName") == pluginInternalName);
		if (obj == null)
		{
			return false;
		}
		object obj2 = null;
		string value = string.Empty;
		try
		{
			obj2 = GetPluginManager();
		}
		catch (Exception ex)
		{
			value = ex.Message;
		}
		if (obj2 == null || !string.IsNullOrWhiteSpace(value))
		{
			return false;
		}
		try
		{
			if (!HasRepo(masterURL))
			{
				AddRepo(masterURL, enabled: true);
			}
			ReloadPluginMasters();
			Task installCall = obj2.Call<Task>("InstallPluginAsync", new object[4]
			{
				obj,
				false,
				(object)(PluginLoadReason)2,
				null
			});
			await installCall;
			if ((bool?)installCall.GetFieldOrProperty("Result")?.GetFieldOrProperty("IsLoaded") == true)
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	public static void ReloadPluginMasters()
	{
		object service = GetService("Dalamud.Plugin.Internal.PluginManager");
		(service?.GetType().GetMethod("SetPluginReposFromConfigAsync", BindingFlags.Instance | BindingFlags.Public))?.Invoke(service, new object[1] { true });
	}

	public static void SaveDalamudConfig()
	{
		object service = GetService("Dalamud.Configuration.Internal.DalamudConfiguration");
		(service?.GetType().GetMethod("QueueSave", BindingFlags.Instance | BindingFlags.Public))?.Invoke(service, null);
	}

	public static void MarkCurrentThreadAsMainThread()
	{
		(((object)DalamudServiceExtension.Instance()).GetType().Assembly.GetType("Dalamud.Utility.ThreadSafety", throwOnError: true)?.GetMethod("MarkMainThread", BindingFlags.Static | BindingFlags.NonPublic))?.Invoke(null, null);
	}
}
