using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Common.Runtime.Abstractions;
using DailyRoutines.Common.Runtime.Hosts;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface.Windowing;
using Newtonsoft.Json;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Interop.Game;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.Common.Module.Abstractions;

public abstract class ModuleBase : IEquatable<ModuleBase>
{
	private Action<ModuleBase>? cleanupAction;

	public string ModuleName { get; private set; }

	public List<ModuleBase> PrecedingModules { get; set; }

	public List<ModuleBase> PairModules { get; set; }

	public List<ModuleBase> ConflictModules { get; set; }

	public bool WithConfigUI { get; private set; }

	public bool WithConfig { get; private set; }

	public string ModuleIdentifier { get; set; }

	public string ModuleGUID { get; init; }

	public virtual ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = ManagerHost.Current.GetLoc("DevModuleTitle"),
		Description = ManagerHost.Current.GetLoc("DevModuleDescription"),
		Category = ModuleCategory.General
	};

	public virtual ModulePermission Permission { get; } = new ModulePermission();

	public bool IsInitialized { get; private set; }

	public bool IsDisposed { get; private set; }

	public bool IsEnabled
	{
		get
		{
			if (IsInitialized)
			{
				return !IsDisposed;
			}
			return false;
		}
	}

	public string ConfigFilePath => Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), ModuleName + ".json");

	public string ConfigDirectoryPath
	{
		get
		{
			string text = Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), "Module", ModuleName ?? "");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			return text;
		}
	}

	protected TaskHelper? TaskHelper { get; set; }

	protected Overlay? Overlay { get; set; }

	protected OverlayConfig? OverlayConfig { get; set; }

	protected ModuleBase()
	{
		ModuleGUID = Guid.Empty.ToString();
		ModuleName = GetType().Name;
		WithConfig = File.Exists(ConfigFilePath);
		WithConfigUI = GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic).Any((MethodInfo m) => m.Name == "ConfigUI" && m.DeclaringType != typeof(ModuleBase));
	}

	public void Load(bool affectConfig = false)
	{
		LoadAsync(affectConfig);
	}

	public Task LoadAsync(bool affectConfig = false)
	{
		return ManagerHost.Current.LoadAsync(this, affectConfig);
	}

	public void Unload(bool affectConfig = false)
	{
		UnloadAsync(affectConfig);
	}

	public Task UnloadAsync(bool affectConfig = false)
	{
		return ManagerHost.Current.UnloadAsync(this, affectConfig);
	}

	public void ToggleOverlayConfig(bool? isEnabled = null)
	{
		if (WithConfigUI)
		{
			if (OverlayConfig == null)
			{
				OverlayConfig overlayConfig = (OverlayConfig = new OverlayConfig(this));
			}
			bool valueOrDefault = isEnabled == true;
			if (!isEnabled.HasValue)
			{
				valueOrDefault = !((Window)OverlayConfig).IsOpen;
				isEnabled = valueOrDefault;
			}
			((Window)OverlayConfig).IsOpen = isEnabled.Value;
		}
	}

	public override string ToString()
	{
		return ModuleIdentifier;
	}

	public bool Equals(ModuleBase? other)
	{
		if (other == null)
		{
			return false;
		}
		return ModuleGUID == other.ModuleGUID;
	}

	public override bool Equals(object? obj)
	{
		return Equals(obj as ModuleBase);
	}

	public override int GetHashCode()
	{
		return ModuleGUID.GetHashCode();
	}

	public void PublicInit()
	{
		if (IsEnabled)
		{
			return;
		}
		try
		{
			IsDisposed = false;
			BaseInit();
			Init();
			IsInitialized = true;
		}
		catch (Exception ex)
		{
			DLog.Error("初始化模块时发生错误", ex);
			PublicUninit();
		}
	}

	public void PublicUninit()
	{
		if (IsDisposed)
		{
			return;
		}
		try
		{
			try
			{
				Uninit();
			}
			catch (Exception ex)
			{
				DLog.Error("卸载模块时发生错误", ex);
			}
			try
			{
				BaseUninit();
			}
			catch (Exception ex2)
			{
				DLog.Error("基础卸载模块时发生错误", ex2);
			}
		}
		finally
		{
			IsDisposed = true;
		}
	}

	public void PublicConfigUI()
	{
		ConfigUI();
	}

	public void PublicOverlayUI()
	{
		OverlayUI();
	}

	public void PublicOverlayOnOpen()
	{
		OverlayOnOpen();
	}

	public void PublicOverlayOnClose()
	{
		OverlayOnClose();
	}

	public void PublicOverlayPreDraw()
	{
		OverlayPreDraw();
	}

	public void PublicOverlayPostDraw()
	{
		OverlayPostDraw();
	}

	public void PublicOverlayUpdate()
	{
		OverlayUpdate();
	}

	public void PublicOverlayPreOpenCheck()
	{
		OverlayPreOpenCheck();
	}

	protected virtual void Init()
	{
	}

	protected virtual void ConfigUI()
	{
	}

	protected virtual void OverlayUI()
	{
	}

	protected virtual void OverlayOnOpen()
	{
	}

	protected virtual void OverlayOnClose()
	{
	}

	protected virtual void OverlayPreDraw()
	{
	}

	protected virtual void OverlayPostDraw()
	{
	}

	protected virtual void OverlayUpdate()
	{
	}

	protected virtual void OverlayPreOpenCheck()
	{
	}

	protected virtual void Uninit()
	{
	}

	private void BaseInit()
	{
		cleanupAction = GenerateCleanupDelegate(GetType());
		IPCAttributeRegistry.RegObjectIPCs(this);
	}

	private void BaseUninit()
	{
		if (Overlay != null)
		{
			ManagerHost.Current.RemoveWindow((Window)(object)Overlay);
		}
		Overlay = null;
		if (OverlayConfig != null)
		{
			ManagerHost.Current.RemoveWindow((Window)(object)OverlayConfig);
		}
		OverlayConfig = null;
		IPCAttributeRegistry.UnregObjectIPCs(this);
		TaskHelper?.Abort();
		TaskHelper?.Dispose();
		TaskHelper = null;
		cleanupAction?.Invoke(this);
		cleanupAction = null;
	}

	public T? LoadConfig<T>() where T : ModuleConfig
	{
		try
		{
			T val = null;
			if (File.Exists(ConfigFilePath))
			{
				val = JsonConvert.DeserializeObject<T>(File.ReadAllText(ConfigFilePath), JsonSerializerSettingsExtension.GetShared());
			}
			if (val == null)
			{
				string text = Activator.CreateInstance<T>()?.PreviousModuleName;
				if (!string.IsNullOrWhiteSpace(text))
				{
					string path = Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), text + ".json");
					if (File.Exists(path))
					{
						val = JsonConvert.DeserializeObject<T>(File.ReadAllText(path), JsonSerializerSettingsExtension.GetShared());
						if (val != null)
						{
							SaveConfig(val);
						}
					}
				}
			}
			return val;
		}
		catch (Exception ex)
		{
			DLog.Error("加载模块配置失败: " + GetType().Name, ex);
			return null;
		}
	}

	public void SaveConfig<T>(T config) where T : ModuleConfig
	{
		try
		{
			ArgumentNullException.ThrowIfNull(config, "config");
			string content = JsonConvert.SerializeObject((object)config, (Formatting)1, JsonSerializerSettingsExtension.GetShared());
			OmenServiceBase<SecureSaveHelper>.Instance().WriteAllText(ConfigFilePath, content);
		}
		catch (Exception ex)
		{
			DLog.Error("加载模块配置失败: " + GetType().Name, ex);
		}
	}

	protected static void ExportToClipboard<T>(T config) where T : class
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		IManagerHost current = ManagerHost.Current;
		try
		{
			ArgumentNullException.ThrowIfNull(config, "config");
			ImGui.SetClipboardText(ImU8String.op_Implicit(config.ToJSONBase64()));
			OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(current.GetLoc("DailyModuleBase-Exported"));
		}
		catch (Exception ex)
		{
			string loc = current.GetLoc("DailyModuleBase-ExportError");
			DLog.Error(loc, ex);
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(loc);
		}
	}

	protected static T? ImportFromClipboard<T>() where T : class
	{
		IManagerHost current = ManagerHost.Current;
		try
		{
			T? val = ImGui.GetClipboardText().FromJSONBase64<T>();
			if (val != null)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(current.GetLoc("DailyModuleBase-Exported"));
			}
			return val;
		}
		catch (Exception ex)
		{
			string loc = current.GetLoc("DailyModuleBase-ImportError");
			DLog.Error(loc, ex);
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(loc);
		}
		return null;
	}

	private static Action<ModuleBase>? GenerateCleanupDelegate(Type type)
	{
		List<FieldInfo> list = (from f in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
			where (f.FieldType.IsGenericType && f.FieldType.GetGenericTypeDefinition() == typeof(Hook<>)) || typeof(MemoryPatch).IsAssignableFrom(f.FieldType)
			select f).ToList();
		if (list.Count == 0)
		{
			return null;
		}
		ParameterExpression parameterExpression = Expression.Parameter(typeof(ModuleBase), "module");
		UnaryExpression unaryExpression = Expression.Convert(parameterExpression, type);
		List<Expression> list2 = new List<Expression>();
		foreach (FieldInfo item in list)
		{
			MemberExpression memberExpression = Expression.Field(item.IsStatic ? null : unaryExpression, item);
			BinaryExpression test = Expression.NotEqual(memberExpression, Expression.Constant(null));
			MethodCallExpression methodCallExpression = Expression.Call(Expression.Convert(memberExpression, typeof(IDisposable)), typeof(IDisposable).GetMethod("Dispose"));
			Expression ifTrue;
			if (!item.IsInitOnly)
			{
				BinaryExpression arg = Expression.Assign(memberExpression, Expression.Convert(Expression.Constant(null), item.FieldType));
				ifTrue = Expression.Block(methodCallExpression, arg);
			}
			else
			{
				ifTrue = methodCallExpression;
			}
			list2.Add(Expression.IfThen(test, ifTrue));
		}
		return Expression.Lambda<Action<ModuleBase>>(Expression.Block(list2), new ParameterExpression[1] { parameterExpression }).Compile();
	}
}
