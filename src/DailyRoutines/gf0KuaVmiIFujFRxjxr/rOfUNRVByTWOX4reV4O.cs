using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Runtime.Abstractions;
using DailyRoutines.Manager;
using Dalamud.Interface.Windowing;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using nXyRYE1uBjK6gmU4ln;

namespace gf0KuaVmiIFujFRxjxr;

internal sealed class rOfUNRVByTWOX4reV4O : IManagerHost
{
	internal static rOfUNRVByTWOX4reV4O Instance { get; }

	private rOfUNRVByTWOX4reV4O()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	public T? Get<T>() where T : ManagerBase
	{
		return Y5SWncTBsGUDX5l2wY.S36MXGucI<T>();
	}

	public Task LoadAsync(ModuleBase module, bool affectConfig)
	{
		return ManagerBase<ModuleManager>.Instance().LoadAsync(module);
	}

	public Task UnloadAsync(ModuleBase module, bool affectConfig)
	{
		return ManagerBase<ModuleManager>.Instance().UnloadAsync(module);
	}

	public bool AddWindow(Window window)
	{
		return OmenServiceBase<WindowManager>.Instance().AddWindow(window);
	}

	public bool RemoveWindow(Window window)
	{
		return OmenServiceBase<WindowManager>.Instance().RemoveWindow(window);
	}

	public string GetLoc(string key, params object[] args)
	{
		return LanguageManager.Get(key, args);
	}

	static rOfUNRVByTWOX4reV4O()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Instance = new rOfUNRVByTWOX4reV4O();
	}
}
