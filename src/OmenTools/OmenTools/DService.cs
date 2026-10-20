using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.UI;
using OmenTools.Dalamud.Services.Game.UI.Abstractions;
using OmenTools.Extensions;
using OmenTools.Interop.Game;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace OmenTools;

public sealed class DService
{
	private List<Type> initializedServiceOrder = new List<Type>();

	public static bool IsDisposed { get; private set; }

	public static bool IsInitialized { get; private set; }

	private static DService? InternalInstance { get; set; }

	private DServiceInitOptions InitOptions { get; set; } = new DServiceInitOptions();

	private Dictionary<Type, OmenServiceBase> OmenServices { get; set; } = new Dictionary<Type, OmenServiceBase>();

	private Dictionary<Type, IOmenDalamudService> OmenDalamudServices { get; set; } = new Dictionary<Type, IOmenDalamudService>();

	private ConcurrentDictionary<TaskHelper, byte> TaskHelpers { get; set; } = new ConcurrentDictionary<TaskHelper, byte>();

	private ConcurrentDictionary<MemoryPatch, byte> MemoryPatches { get; set; } = new ConcurrentDictionary<MemoryPatch, byte>();

	private ConditionalWeakTable<AssemblyLoadContext, ConcurrentDictionary<IDalamudHook, byte>> Hooks { get; set; } = new ConditionalWeakTable<AssemblyLoadContext, ConcurrentDictionary<IDalamudHook, byte>>();

	[PluginService]
	public IAddonEventManager AddonEvent { get; private set; }

	[PluginService]
	public IAddonLifecycle AddonLifecycle { get; private set; }

	[PluginService]
	public IAgentLifecycle AgentLifecycle { get; private set; }

	[PluginService]
	public IBuddyList BuddyList { get; private set; }

	[PluginService]
	public IChatGui Chat { get; private set; }

	[PluginService]
	public IClientState ClientState { get; private set; }

	[PluginService]
	public ICommandManager Command { get; private set; }

	[PluginService]
	public ICondition Condition { get; private set; }

	[PluginService]
	public IContextMenu ContextMenu { get; private set; }

	[PluginService]
	public IDataManager Data { get; private set; }

	[PluginService]
	public IDtrBar DTRBar { get; private set; }

	[PluginService]
	public IDutyState DutyState { get; private set; }

	[PluginService]
	public IFateTable Fate { get; private set; }

	[PluginService]
	public IFlyTextGui FlyText { get; private set; }

	[PluginService]
	public IFramework Framework { get; private set; }

	[PluginService]
	public IGameConfig GameConfig { get; private set; }

	[PluginService]
	public IGameGui GameGUI { get; private set; }

	[PluginService]
	public IGameInteropProvider Hook { get; private set; }

	[PluginService]
	public IGameInventory GameInventory { get; private set; }

	[PluginService]
	public IGameLifecycle GameLifecycle { get; private set; }

	[PluginService]
	public IGamepadState Gamepad { get; private set; }

	[PluginService]
	public IJobGauges JobGauges { get; private set; }

	[PluginService]
	public IKeyState KeyState { get; private set; }

	[PluginService]
	public IMarketBoard MarketBoard { get; private set; }

	[PluginService]
	public INamePlateGui NamePlate { get; private set; }

	[PluginService]
	public INotificationManager DalamudNotification { get; private set; }

	[PluginService]
	public IPartyFinderGui PartyFinder { get; private set; }

	[PluginService]
	public IPartyList PartyList { get; private set; }

	[PluginService]
	public IPlayerState PlayerState { get; private set; }

	[PluginService]
	public IPluginLog Log { get; private set; }

	[PluginService]
	public ISeStringEvaluator SeStringEvaluator { get; private set; }

	[PluginService]
	public ISelfTestRegistry SelfTestRegistry { get; private set; }

	[PluginService]
	public ISigScanner SigScanner { get; private set; }

	[PluginService]
	public ITextureProvider Texture { get; private set; }

	[PluginService]
	public ITextureReadbackProvider TextureReadback { get; private set; }

	[PluginService]
	public ITextureSubstitutionProvider TextureSubstitution { get; private set; }

	[PluginService]
	public ITitleScreenMenu TitleScreenMenu { get; private set; }

	[PluginService]
	public IToastGui Toast { get; private set; }

	[PluginService]
	public IUnlockState UnlockState { get; private set; }

	public IDalamudPluginInterface PI { get; private set; }

	public IUiBuilder UIBuilder { get; private set; }

	public IAetheryteList AetheryteList { get; private set; }

	public IObjectTable ObjectTable { get; private set; }

	public static void Init(IDalamudPluginInterface pluginInterface, Func<DServiceInitOptions>? optionsFunc = null)
	{
		if (IsInitialized || IsDisposed)
		{
			return;
		}
		DService dService = Instance();
		pluginInterface.Inject((object)dService, Array.Empty<object>());
		dService.ResetServiceState();
		dService.InitOptions = ((optionsFunc != null) ? optionsFunc() : new DServiceInitOptions());
		dService.PI = pluginInterface;
		dService.UIBuilder = pluginInterface.UiBuilder;
		dService.ObjectTable = new ObjectTable();
		dService.AetheryteList = new AetheryteList();
		dService.OmenDalamudServices[typeof(IObjectTable)] = dService.ObjectTable;
		dService.OmenDalamudServices[typeof(IAetheryteList)] = dService.AetheryteList;
		try
		{
			List<Type> list = dService.DiscoverEnabledServiceTypes();
			dService.InstantiateServices(list);
			foreach (Type item in list)
			{
				dService.OmenServices[item].PublicInit();
				dService.initializedServiceOrder.Add(item);
			}
			IsInitialized = true;
		}
		catch (Exception ex)
		{
			DLog.Error("[ziz] 初始化各 OmenService 时发生错误", ex);
			Uninit();
			throw;
		}
		AssemblyLoadContext loadContext = AssemblyLoadContext.GetLoadContext(typeof(DService).Assembly);
		IExposedPlugin plugin = pluginInterface.GetPlugin(loadContext);
		DLog.Debug($"[ziz] 初始化完成\tALC: {loadContext}; 持有方: {((plugin != null) ? plugin.InternalName : null) ?? "<shared>"}");
	}

	public static void Uninit()
	{
		if (IsDisposed || InternalInstance == null)
		{
			return;
		}
		try
		{
			InternalInstance.UninitOmenServices();
			InternalInstance.DisposeTrackedTaskHelpers();
			InternalInstance.DisposeTrackedMemoryPatches();
			InternalInstance.DisposeTrackedHooks();
			InternalInstance.ResetServiceState();
			Throttler.Shared.Clear();
			InternalInstance.ObjectTable = null;
			InternalInstance.AetheryteList = null;
			InternalInstance.OmenDalamudServices.Clear();
		}
		catch (Exception ex)
		{
			DLog.Error("[ziz] 卸载各 OmenService 时发生错误", ex);
			throw;
		}
		finally
		{
			IsDisposed = true;
		}
		AssemblyLoadContext loadContext = AssemblyLoadContext.GetLoadContext(typeof(DService).Assembly);
		IExposedPlugin plugin = DalamudServiceExtension.Instance().GetPlugin(loadContext);
		DLog.Debug($"[ziz] 卸载完成\tALC: {loadContext}; 持有方: {((plugin != null) ? plugin.InternalName : null) ?? "<shared>"}");
	}

	public static DService Instance()
	{
		return InternalInstance ?? (InternalInstance = new DService());
	}

	public T? GetOmenService<T>() where T : OmenServiceBase
	{
		return (T)OmenServices.GetValueOrDefault(typeof(T));
	}

	public T? GetOmenDalamudService<T>() where T : IOmenDalamudService
	{
		return (T)OmenDalamudServices.GetValueOrDefault(typeof(T));
	}

	private List<Type> DiscoverEnabledServiceTypes()
	{
		return (from t in Assembly.GetExecutingAssembly().GetTypes()
			where typeof(OmenServiceBase).IsAssignableFrom(t) && !t.IsAbstract
			where !InitOptions.IsDisabled(t)
			select t).ToList();
	}

	private void InstantiateServices(IEnumerable<Type> serviceTypes)
	{
		foreach (Type serviceType in serviceTypes)
		{
			if (!(Activator.CreateInstance(serviceType) is OmenServiceBase value))
			{
				throw new InvalidOperationException("初始化 OmenService 错误: " + serviceType.FullName);
			}
			OmenServices.TryAdd(serviceType, value);
		}
	}

	private void UninitOmenServices()
	{
		foreach (Type item in initializedServiceOrder.AsEnumerable().Reverse())
		{
			if (OmenServices.TryGetValue(item, out OmenServiceBase value))
			{
				value.PublicUninit();
			}
		}
	}

	private void ResetServiceState()
	{
		OmenServices = new Dictionary<Type, OmenServiceBase>();
		TaskHelpers = new ConcurrentDictionary<TaskHelper, byte>();
		MemoryPatches = new ConcurrentDictionary<MemoryPatch, byte>();
		Hooks = new ConditionalWeakTable<AssemblyLoadContext, ConcurrentDictionary<IDalamudHook, byte>>();
		initializedServiceOrder = new List<Type>();
		InitOptions = new DServiceInitOptions();
	}

	internal void RegTaskHelper(TaskHelper taskHelper)
	{
		ArgumentNullException.ThrowIfNull(taskHelper, "taskHelper");
		TaskHelpers.TryAdd(taskHelper, 0);
	}

	internal void UnregTaskHelper(TaskHelper taskHelper)
	{
		ArgumentNullException.ThrowIfNull(taskHelper, "taskHelper");
		TaskHelpers.TryRemove(taskHelper, out var _);
	}

	internal void RegMemoryPatch(MemoryPatch memoryPatch)
	{
		ArgumentNullException.ThrowIfNull(memoryPatch, "memoryPatch");
		MemoryPatches.TryAdd(memoryPatch, 0);
	}

	internal void UnregMemoryPatch(MemoryPatch memoryPatch)
	{
		ArgumentNullException.ThrowIfNull(memoryPatch, "memoryPatch");
		MemoryPatches.TryRemove(memoryPatch, out var _);
	}

	internal void RegHook(IDalamudHook hook, AssemblyLoadContext context)
	{
		ArgumentNullException.ThrowIfNull(hook, "hook");
		ArgumentNullException.ThrowIfNull(context, "context");
		ConcurrentDictionary<IDalamudHook, byte> value = Hooks.GetValue(context, (AssemblyLoadContext _) => new ConcurrentDictionary<IDalamudHook, byte>());
		if (context.IsCollectible && value.IsEmpty)
		{
			context.Unloading += delegate
			{
				UnregHooks(context);
			};
		}
		value.TryAdd(hook, 0);
	}

	internal void UnregHooks(AssemblyLoadContext context)
	{
		if (Hooks.TryGetValue(context, out ConcurrentDictionary<IDalamudHook, byte> value))
		{
			DisposeHooks(value.Keys);
			Hooks.Remove(context);
		}
	}

	private void DisposeTrackedTaskHelpers()
	{
		foreach (TaskHelper key in TaskHelpers.Keys)
		{
			if (key != null && !key.IsDisposed)
			{
				key.Dispose();
			}
		}
		TaskHelpers.Clear();
	}

	private void DisposeTrackedMemoryPatches()
	{
		foreach (MemoryPatch key in MemoryPatches.Keys)
		{
			key.Dispose();
		}
		MemoryPatches.Clear();
	}

	private void DisposeTrackedHooks()
	{
		foreach (KeyValuePair<AssemblyLoadContext, ConcurrentDictionary<IDalamudHook, byte>> item in (IEnumerable<KeyValuePair<AssemblyLoadContext, ConcurrentDictionary<IDalamudHook, byte>>>)Hooks)
		{
			item.Deconstruct(out var _, out var value);
			DisposeHooks(value.Keys);
		}
		Hooks.Clear();
	}

	private static void DisposeHooks(IEnumerable<IDalamudHook> hooks)
	{
		foreach (IDalamudHook hook in hooks)
		{
			if (hook != null && !hook.IsDisposed)
			{
				((IDisposable)hook).Dispose();
			}
		}
	}
}
