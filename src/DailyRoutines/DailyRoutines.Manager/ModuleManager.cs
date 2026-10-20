using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Compilers;
using DailyRoutines.Common.Runtime.Hosts;
using DailyRoutines.Internal;
using DailyRoutines.Verification;
using Dalamud.Plugin.Services;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using itAnDoW4mgYVlrmtMor;
using nXyRYE1uBjK6gmU4ln;

namespace DailyRoutines.Manager;

[ManagerInitOrder(4)]
public sealed class ModuleManager : ManagerBase<ModuleManager>
{
	public delegate void OnModuleChangedDelegate(IReadOnlyList<ModuleBase> modules);

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass36_0
	{
		public IReadOnlyList<ModuleBase> modules;

		public ModuleManager _003C_003E4__this;

		public _003C_003Ec__DisplayClass36_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void JpyOOHIB1n()
		{
			foreach (ModuleBase item in modules)
			{
				if (!_003C_003E4__this.s3ISBDmHmF(item, out ModuleBase currentModule) || currentModule.IsEnabled || currentModule.Permission.IsHide(AuthState.IsAuth, AuthState.IsPremium))
				{
					continue;
				}
				try
				{
					if (!currentModule.IsDisposed)
					{
						goto IL_008a;
					}
					_003C_003E4__this.T7RSiPhtbb(currentModule);
					if (_003C_003E4__this.s3ISBDmHmF(currentModule, out currentModule))
					{
						goto IL_008a;
					}
					goto end_IL_005b;
					IL_008a:
					DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B2108BE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a) + currentModule.ModuleName);
					currentModule.PublicInit();
					if (currentModule.IsEnabled)
					{
						DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200F899F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af) + currentModule.ModuleName);
					}
					end_IL_005b:;
				}
				catch (Exception ex)
				{
					DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28168422 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd) + currentModule.ModuleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4A1782 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32), ex);
				}
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass38_0
	{
		public ModuleBase currentModule;

		public bool affectConfig;

		public string moduleName;

		public _003C_003Ec__DisplayClass38_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void SuMOKZ9EKy()
		{
			try
			{
				DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBBFC7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2) + moduleName);
				currentModule.PublicInit();
				if (currentModule.IsEnabled)
				{
					DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B210B6C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a) + moduleName);
				}
			}
			catch (Exception ex)
			{
				if (affectConfig && currentModule != null)
				{
					PluginConfig.Instance().ModuleEnabled[moduleName] = false;
				}
				DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFB90A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2) + moduleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3707422 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e), ex);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass41_0
	{
		public IReadOnlyList<ModuleBase> modules;

		public ModuleManager _003C_003E4__this;

		public _003C_003Ec__DisplayClass41_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void NkfOtlci8J()
		{
			foreach (ModuleBase item in modules)
			{
				if (_003C_003E4__this.s3ISBDmHmF(item, out ModuleBase currentModule) && currentModule.IsEnabled)
				{
					try
					{
						DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D340AA8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33) + currentModule.ModuleName);
						currentModule.PublicUninit();
						DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7DF1EE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571) + currentModule.ModuleName);
					}
					catch (Exception ex)
					{
						DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE5EFD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76) + currentModule.ModuleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D340A9C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33), ex);
					}
				}
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass43_0
	{
		public ModuleBase currentModule;

		public string moduleName;

		public _003C_003Ec__DisplayClass43_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void dBbOWFSXYe()
		{
			try
			{
				DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7AAED ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff) + moduleName);
				currentModule.PublicUninit();
				DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F71766D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0) + moduleName);
			}
			catch (Exception ex)
			{
				DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6CB36A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac) + moduleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53684566 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94), ex);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass47_0
	{
		public string moduleName;

		public _003C_003Ec__DisplayClass47_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool bREOT6vrpq(KeyValuePair<Type, ModuleBase> kvp)
		{
			return kvp.Key.Name.Equals(moduleName, StringComparison.OrdinalIgnoreCase);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass66_0
	{
		public CancellationToken token;

		public ModuleManager _003C_003E4__this;

		public Action _003C_003E9__1;

		public _003C_003Ec__DisplayClass66_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal async Task? AWpO1eYxJh()
		{
			try
			{
				await Task.Delay(1000, token);
				if (!token.IsCancellationRequested)
				{
					_003C_003E4__this.taskHelper.Enqueue(delegate
					{
						_003C_003E4__this.ReloadAllAsync(CancellationToken.None);
					}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7CB95 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		internal void bpWOMbpYGT()
		{
			_003C_003E4__this.ReloadAllAsync(CancellationToken.None);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass67_0
	{
		public ModuleManager _003C_003E4__this;

		public Type[] moduleTypes;

		public int workerCount;

		public ConcurrentDictionary<Type, ModuleBase> modulesTemp;

		public ConcurrentDictionary<Type, byte> bag;

		public Action<Type> _003C_003E9__5;

		public _003C_003Ec__DisplayClass67_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal ParallelLoopResult kkFO4ioTch()
		{
			return Parallel.ForEach(moduleTypes, new ParallelOptions
			{
				MaxDegreeOfParallelism = workerCount
			}, delegate(Type type)
			{
				try
				{
					if (Activator.CreateInstance(type) is ModuleBase value)
					{
						modulesTemp.TryAdd(type, value);
					}
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
					defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFB806 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
					defaultInterpolatedStringHandler.AppendFormatted(type);
					defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B3C15 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
					DLog.Error(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
				}
			});
		}

		internal void iglOsPco0S(Type type)
		{
			try
			{
				if (Activator.CreateInstance(type) is ModuleBase value)
				{
					modulesTemp.TryAdd(type, value);
				}
			}
			catch (Exception ex)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
				defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFB806 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
				defaultInterpolatedStringHandler.AppendFormatted(type);
				defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B3C15 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
				DLog.Error(defaultInterpolatedStringHandler.ToStringAndClear(), ex);
			}
		}

		internal async Task NVlO8Pp9C7(string path)
		{
			_ = 1;
			try
			{
				string text = Path.GetExtension(path).ToLower();
				Assembly assembly;
				if (!(text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1C02C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83)))
				{
					if (!(text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x39223F34 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2)))
					{
						return;
					}
					string code = await File.ReadAllTextAsync(path);
					assembly = _003C_003E4__this.compiler.Load(code);
				}
				else
				{
					byte[] assembly2 = await File.ReadAllBytesAsync(path);
					assembly = _003C_003E4__this.compiler.Load(assembly2);
				}
				if (!(assembly != null))
				{
					return;
				}
				JSnSEjBSW6(assembly, path);
				foreach (Type derivedType in ModuleCompiler.GetDerivedTypes<ModuleBase>(assembly))
				{
					bag.AddOrUpdate(derivedType, (Type _) => 0, (Type _, byte _) => 0);
				}
			}
			catch
			{
			}
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass71_0
	{
		public ModuleManager _003C_003E4__this;

		public Dictionary<ModuleBase, List<ModuleBase>> prerequisitesClosureCache;

		public Dictionary<ModuleBase, IReadOnlyList<ModuleBase>> directPrerequisites;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003Ec__DisplayClass71_1
	{
		public HashSet<ModuleBase> path;

		public ModuleBase module;

		public HashSet<ModuleBase> added;

		public List<ModuleBase> result;
	}

	private ConcurrentDictionary<Type, ModuleBase> modulesBag;

	private FileSystemWatcher? watcher;

	private TaskHelper? taskHelper;

	private readonly ModuleCompiler compiler;

	private readonly SemaphoreSlim loadSemaphore;

	private readonly SemaphoreSlim notifySemaphore;

	private CancellationTokenSource? fileWatchDebounceTokenSource;

	private volatile int activeModuleOperations;

	public List<ModuleBase> Modules { get; private set; }

	public int ModulesValidCount { get; private set; }

	public int ModulesEnableCount { get; private set; }

	public bool IsInitializingAllModules { get; private set; }

	public bool IsLoadingAllModules { get; private set; }

	public bool IsUnloadingAllModules { get; private set; }

	public bool IsBusy
	{
		get
		{
			if (!IsInitializingAllModules && !IsLoadingAllModules && !IsUnloadingAllModules && Interlocked.CompareExchange(ref activeModuleOperations, 0, 0) <= 0)
			{
				return taskHelper?.IsBusy ?? false;
			}
			return true;
		}
	}

	private static string[] BuiltInModulesPaths => new string[2]
	{
		Path.Join(Path.GetDirectoryName(DalamudServiceExtension.Instance().AssemblyLocation.FullName), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28176DA4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd)),
		Path.Join(Path.GetDirectoryName(DalamudServiceExtension.Instance().AssemblyLocation.FullName), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D35E402 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33))
	};

	public event OnModuleChangedDelegate? OnModuleChanged;

	public async Task ReloadAllAsync(CancellationToken ct)
	{
		await TaskExtension.WaitForConditionAsync(() => !IsInitializingAllModules && !IsUnloadingAllModules && !IsLoadingAllModules, TimeSpan.FromSeconds(180L), ct);
		await HOyS8n5HZk();
		await R7kS32ivQV();
		await hr8SMBXq7x();
	}

	public async Task ResetModuleAsync(ModuleBase module)
	{
		if (module == null || !s3ISBDmHmF(module, out ModuleBase currentModule) || !currentModule.IsEnabled)
		{
			return;
		}
		await currentModule.UnloadAsync();
		ModuleBase resetModule = GetModule(module.GetType());
		if (resetModule != null)
		{
			if (File.Exists(resetModule.ConfigFilePath))
			{
				File.Delete(resetModule.ConfigFilePath);
			}
			await resetModule.LoadAsync();
			OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D340CFE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33), resetModule.Info.Title));
		}
	}

	public bool? LoadOrUnloadModuleByName(string moduleName, bool? isLoad = null)
	{
		if (string.IsNullOrWhiteSpace(moduleName))
		{
			return null;
		}
		ModuleBase moduleByName = GetModuleByName(moduleName);
		if (moduleByName == null)
		{
			return null;
		}
		bool valueOrDefault = isLoad == true;
		if (!isLoad.HasValue)
		{
			valueOrDefault = !moduleByName.IsEnabled;
			isLoad = valueOrDefault;
		}
		if (isLoad.Value)
		{
			LoadAsync(moduleByName);
			return true;
		}
		UnloadAsync(moduleByName);
		return false;
	}

	public void InvokeOnModuleChanged()
	{
		qDuShcG7sC();
	}

	public async Task LoadAsync(ModuleBase module)
	{
		if (s3ISBDmHmF(module, out ModuleBase currentModule) && (currentModule.IsEnabled || !currentModule.Permission.IsHide(AuthState.IsAuth, AuthState.IsPremium)))
		{
			await w96Sssi5BF(currentModule, affectConfig: true);
		}
	}

	internal async Task hr8SMBXq7x()
	{
		if (IsLoadingAllModules)
		{
			return;
		}
		DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6540A6B6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb));
		IsLoadingAllModules = true;
		try
		{
			List<ModuleBase> list = new List<ModuleBase>();
			foreach (KeyValuePair<Type, ModuleBase> item in modulesBag)
			{
				item.Deconstruct(out var key, out var value);
				Type type = key;
				ModuleBase moduleBase = value;
				string name = type.Name;
				if (PluginConfig.Instance().ModuleEnabled.TryGetValue(name, out var value2) && value2 && !moduleBase.IsEnabled)
				{
					list.Add(moduleBase);
				}
			}
			foreach (ModuleBase[] item2 in list.Chunk(32))
			{
				await umSS4QPk7k(item2).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		finally
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E4631EB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
			defaultInterpolatedStringHandler.AppendFormatted(modulesBag.Count);
			DLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			IsLoadingAllModules = false;
			qDuShcG7sC();
		}
	}

	private async Task umSS4QPk7k(IReadOnlyList<ModuleBase> modules)
	{
		_003C_003Ec__DisplayClass36_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass36_0();
		CS_0024_003C_003E8__locals7.modules = modules;
		CS_0024_003C_003E8__locals7._003C_003E4__this = this;
		if (CS_0024_003C_003E8__locals7.modules.Count == 0)
		{
			return;
		}
		await DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
		{
			foreach (ModuleBase item in CS_0024_003C_003E8__locals7.modules)
			{
				if (CS_0024_003C_003E8__locals7._003C_003E4__this.s3ISBDmHmF(item, out ModuleBase currentModule) && !currentModule.IsEnabled && !currentModule.Permission.IsHide(AuthState.IsAuth, AuthState.IsPremium))
				{
					try
					{
						if (!currentModule.IsDisposed)
						{
							goto IL_008a;
						}
						CS_0024_003C_003E8__locals7._003C_003E4__this.T7RSiPhtbb(currentModule);
						if (CS_0024_003C_003E8__locals7._003C_003E4__this.s3ISBDmHmF(currentModule, out currentModule))
						{
							goto IL_008a;
						}
						goto end_IL_005b;
						IL_008a:
						DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B2108BE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a) + currentModule.ModuleName);
						currentModule.PublicInit();
						if (currentModule.IsEnabled)
						{
							DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200F899F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af) + currentModule.ModuleName);
						}
						end_IL_005b:;
					}
					catch (Exception ex)
					{
						DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28168422 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd) + currentModule.ModuleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4A1782 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32), ex);
					}
				}
			}
		}, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task LoadBatchAsync(IEnumerable<ModuleBase> modulesToLoad)
	{
		if (modulesToLoad == null)
		{
			return;
		}
		List<Task> list = new List<Task>();
		foreach (ModuleBase item in modulesToLoad)
		{
			if (item != null)
			{
				list.Add(w96Sssi5BF(item, affectConfig: true));
			}
		}
		await Task.WhenAll(list);
	}

	private async Task w96Sssi5BF(ModuleBase module, bool affectConfig)
	{
		_003C_003Ec__DisplayClass38_0 CS_0024_003C_003E8__locals27 = new _003C_003Ec__DisplayClass38_0();
		CS_0024_003C_003E8__locals27.affectConfig = affectConfig;
		if (!s3ISBDmHmF(module, out CS_0024_003C_003E8__locals27.currentModule))
		{
			return;
		}
		if (CS_0024_003C_003E8__locals27.currentModule.Permission.IsHide(AuthState.IsAuth, AuthState.IsPremium))
		{
			if (CS_0024_003C_003E8__locals27.currentModule.IsEnabled)
			{
				await CR7SlJe3NW(CS_0024_003C_003E8__locals27.currentModule, affectConfig: false);
			}
			return;
		}
		await loadSemaphore.WaitAsync();
		Interlocked.Increment(ref activeModuleOperations);
		try
		{
			if (!s3ISBDmHmF(module, out CS_0024_003C_003E8__locals27.currentModule))
			{
				return;
			}
			if (CS_0024_003C_003E8__locals27.currentModule.IsDisposed)
			{
				T7RSiPhtbb(CS_0024_003C_003E8__locals27.currentModule);
				if (!s3ISBDmHmF(module, out CS_0024_003C_003E8__locals27.currentModule))
				{
					return;
				}
			}
			if (CS_0024_003C_003E8__locals27.currentModule.Permission.IsHide(AuthState.IsAuth, AuthState.IsPremium))
			{
				return;
			}
			CS_0024_003C_003E8__locals27.moduleName = CS_0024_003C_003E8__locals27.currentModule.ModuleName;
			await DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
			{
				try
				{
					DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBBFC7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2) + CS_0024_003C_003E8__locals27.moduleName);
					CS_0024_003C_003E8__locals27.currentModule.PublicInit();
					if (CS_0024_003C_003E8__locals27.currentModule.IsEnabled)
					{
						DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B210B6C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a) + CS_0024_003C_003E8__locals27.moduleName);
					}
				}
				catch (Exception ex)
				{
					if (CS_0024_003C_003E8__locals27.affectConfig && CS_0024_003C_003E8__locals27.currentModule != null)
					{
						PluginConfig.Instance().ModuleEnabled[CS_0024_003C_003E8__locals27.moduleName] = false;
					}
					DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFB90A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2) + CS_0024_003C_003E8__locals27.moduleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3707422 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e), ex);
				}
			}, default(CancellationToken));
			if (CS_0024_003C_003E8__locals27.affectConfig)
			{
				PluginConfig.Instance().ModuleEnabled[CS_0024_003C_003E8__locals27.moduleName] = CS_0024_003C_003E8__locals27.currentModule.IsEnabled;
			}
		}
		catch
		{
			if (CS_0024_003C_003E8__locals27.affectConfig && CS_0024_003C_003E8__locals27.currentModule != null)
			{
				PluginConfig.Instance().ModuleEnabled[CS_0024_003C_003E8__locals27.currentModule.ModuleName] = false;
			}
		}
		finally
		{
			if (CS_0024_003C_003E8__locals27.affectConfig)
			{
				PluginConfig.Instance().Save();
			}
			Interlocked.Decrement(ref activeModuleOperations);
			loadSemaphore.Release();
			qDuShcG7sC();
		}
	}

	public async Task UnloadAsync(ModuleBase module)
	{
		if (s3ISBDmHmF(module, out ModuleBase currentModule) && currentModule.IsEnabled)
		{
			await CR7SlJe3NW(currentModule, affectConfig: true);
		}
	}

	private async Task HOyS8n5HZk()
	{
		if (IsUnloadingAllModules)
		{
			return;
		}
		DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A22C5C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
		IsUnloadingAllModules = true;
		try
		{
			List<ModuleBase> list = new List<ModuleBase>();
			foreach (ModuleBase value in modulesBag.Values)
			{
				if (value.IsEnabled)
				{
					list.Add(value);
				}
			}
			foreach (ModuleBase[] item in list.Chunk(32))
			{
				await NxSSvFc4Gb(item).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		finally
		{
			DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEC17F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			IsUnloadingAllModules = false;
		}
	}

	private async Task NxSSvFc4Gb(IReadOnlyList<ModuleBase> modules)
	{
		_003C_003Ec__DisplayClass41_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass41_0();
		CS_0024_003C_003E8__locals5.modules = modules;
		CS_0024_003C_003E8__locals5._003C_003E4__this = this;
		if (CS_0024_003C_003E8__locals5.modules.Count == 0)
		{
			return;
		}
		await DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
		{
			foreach (ModuleBase item in CS_0024_003C_003E8__locals5.modules)
			{
				if (CS_0024_003C_003E8__locals5._003C_003E4__this.s3ISBDmHmF(item, out ModuleBase currentModule) && currentModule.IsEnabled)
				{
					try
					{
						DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D340AA8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33) + currentModule.ModuleName);
						currentModule.PublicUninit();
						DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7DF1EE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571) + currentModule.ModuleName);
					}
					catch (Exception ex)
					{
						DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE5EFD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76) + currentModule.ModuleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D340A9C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33), ex);
					}
				}
			}
		}, default(CancellationToken)).ConfigureAwait(continueOnCapturedContext: false);
	}

	public async Task UnloadBatchAsync(IEnumerable<ModuleBase> modulesToUnload)
	{
		if (modulesToUnload != null)
		{
			await Task.WhenAll(from m in modulesToUnload
				where m != null && s3ISBDmHmF(m, out ModuleBase currentModule) && currentModule.IsEnabled
				select CR7SlJe3NW(m, affectConfig: true));
		}
	}

	private async Task CR7SlJe3NW(ModuleBase module, bool affectConfig, bool replaceInstance = true)
	{
		_003C_003Ec__DisplayClass43_0 CS_0024_003C_003E8__locals13 = new _003C_003Ec__DisplayClass43_0();
		if (!s3ISBDmHmF(module, out CS_0024_003C_003E8__locals13.currentModule) || !CS_0024_003C_003E8__locals13.currentModule.IsEnabled)
		{
			return;
		}
		await loadSemaphore.WaitAsync();
		Interlocked.Increment(ref activeModuleOperations);
		try
		{
			if (!s3ISBDmHmF(module, out CS_0024_003C_003E8__locals13.currentModule) || !CS_0024_003C_003E8__locals13.currentModule.IsEnabled)
			{
				return;
			}
			CS_0024_003C_003E8__locals13.moduleName = CS_0024_003C_003E8__locals13.currentModule.ModuleName;
			await DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
			{
				try
				{
					DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7AAED ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff) + CS_0024_003C_003E8__locals13.moduleName);
					CS_0024_003C_003E8__locals13.currentModule.PublicUninit();
					DLog.Verbose(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F71766D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0) + CS_0024_003C_003E8__locals13.moduleName);
				}
				catch (Exception ex2)
				{
					DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6CB36A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac) + CS_0024_003C_003E8__locals13.moduleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53684566 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94), ex2);
				}
			}, default(CancellationToken));
			if (replaceInstance)
			{
				T7RSiPhtbb(CS_0024_003C_003E8__locals13.currentModule);
			}
			if (affectConfig)
			{
				PluginConfig.Instance().ModuleEnabled[CS_0024_003C_003E8__locals13.moduleName] = false;
			}
		}
		catch (Exception ex)
		{
			DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A48AD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1) + CS_0024_003C_003E8__locals13.currentModule.ModuleName + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDC760F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1), ex);
		}
		finally
		{
			if (affectConfig)
			{
				PluginConfig.Instance().Save();
			}
			Interlocked.Decrement(ref activeModuleOperations);
			loadSemaphore.Release();
			qDuShcG7sC();
		}
	}

	public bool IsModuleEnabled(Type moduleType)
	{
		if (modulesBag.TryGetValue(moduleType, out ModuleBase value))
		{
			return value.IsEnabled;
		}
		return false;
	}

	public bool TryGetModuleByName(string moduleName, out ModuleBase? module)
	{
		module = GetModuleByName(moduleName);
		return module != null;
	}

	public bool? IsModuleEnabled(string moduleName)
	{
		return GetModuleByName(moduleName)?.IsEnabled;
	}

	public ModuleBase? GetModuleByName(string moduleName)
	{
		_003C_003Ec__DisplayClass47_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass47_0();
		CS_0024_003C_003E8__locals3.moduleName = moduleName;
		object obj;
		if (!string.IsNullOrEmpty(CS_0024_003C_003E8__locals3.moduleName))
		{
			obj = modulesBag.FirstOrDefault<KeyValuePair<Type, ModuleBase>>((KeyValuePair<Type, ModuleBase> kvp) => kvp.Key.Name.Equals(CS_0024_003C_003E8__locals3.moduleName, StringComparison.OrdinalIgnoreCase)).Value;
			if (obj == null)
			{
				return null;
			}
		}
		else
		{
			obj = null;
		}
		return (ModuleBase?)obj;
	}

	public bool TryGetModule<T>(out T? module) where T : ModuleBase
	{
		module = GetModule<T>();
		return module != null;
	}

	public T? GetModule<T>() where T : ModuleBase
	{
		return (T)modulesBag.GetValueOrDefault(typeof(T));
	}

	public bool TryGetModule(Type? moduleType, out ModuleBase? module)
	{
		module = null;
		if (moduleType == null || moduleType.IsAbstract)
		{
			return false;
		}
		module = GetModule(moduleType);
		return module != null;
	}

	public ModuleBase? GetModule(Type? moduleType)
	{
		if (moduleType == null || moduleType.IsAbstract)
		{
			return null;
		}
		return modulesBag.GetValueOrDefault(moduleType);
	}

	protected override Task Init()
	{
		try
		{
			Task.Run((Func<Task?>)iucSxkQqbG);
			if (taskHelper == null)
			{
				taskHelper = new TaskHelper
				{
					TimeoutMS = 180000,
					ShowDebug = true
				};
			}
			if (watcher == null)
			{
				watcher = new FileSystemWatcher(Path.GetDirectoryName(DalamudServiceExtension.Instance().AssemblyLocation.FullName))
				{
					Filter = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3719AFE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e)
				};
			}
			watcher.Created += CG9SpcXeWS;
			watcher.Changed += CG9SpcXeWS;
			watcher.EnableRaisingEvents = true;
			return Task.CompletedTask;
		}
		catch (Exception exception)
		{
			return Task.FromException(exception);
		}
	}

	protected override async Task PostInit()
	{
		await R7kS32ivQV();
		await hr8SMBXq7x();
	}

	protected override async Task Uninit()
	{
		watcher?.Dispose();
		watcher = null;
		fileWatchDebounceTokenSource?.Cancel();
		fileWatchDebounceTokenSource?.Dispose();
		fileWatchDebounceTokenSource = null;
		taskHelper?.Abort();
		taskHelper?.Dispose();
		taskHelper = null;
		await HOyS8n5HZk();
		modulesBag.Clear();
		Modules.Clear();
		compiler.UnloadAll();
		qDuShcG7sC();
		notifySemaphore?.Dispose();
		ManagerHost.Current = null;
		IsLoadingAllModules = false;
		IsInitializingAllModules = false;
		IsUnloadingAllModules = false;
	}

	private async Task iucSxkQqbG()
	{
		while (!base.IsDisposed)
		{
			try
			{
				await notifySemaphore.WaitAsync();
				if (base.IsDisposed)
				{
					break;
				}
				while (IsBusy)
				{
					await Task.Delay(100);
				}
				if (base.IsDisposed)
				{
					break;
				}
				Modules = modulesBag.Values.OrderBy((ModuleBase x) => x.Info.Title).ToList();
				ModulesValidCount = Modules.Count;
				ModulesEnableCount = Modules.Count((ModuleBase x) => x.IsEnabled);
				this.OnModuleChanged?.Invoke(Modules);
			}
			catch (ObjectDisposedException)
			{
				break;
			}
			catch (Exception ex2)
			{
				DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1C500 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83), ex2);
			}
		}
	}

	private void CG9SpcXeWS(object sender, FileSystemEventArgs e)
	{
		_003C_003Ec__DisplayClass66_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass66_0();
		CS_0024_003C_003E8__locals7._003C_003E4__this = this;
		if (!Y5SWncTBsGUDX5l2wY.IsInitialized || Y5SWncTBsGUDX5l2wY.IsUninitialized || Y5SWncTBsGUDX5l2wY.IsDisposed)
		{
			return;
		}
		CancellationTokenSource? obj = fileWatchDebounceTokenSource;
		obj?.Cancel();
		obj?.Dispose();
		fileWatchDebounceTokenSource = new CancellationTokenSource();
		CS_0024_003C_003E8__locals7.token = fileWatchDebounceTokenSource.Token;
		Task.Run(async delegate
		{
			try
			{
				await Task.Delay(1000, CS_0024_003C_003E8__locals7.token);
				if (!CS_0024_003C_003E8__locals7.token.IsCancellationRequested)
				{
					CS_0024_003C_003E8__locals7._003C_003E4__this.taskHelper.Enqueue(delegate
					{
						CS_0024_003C_003E8__locals7._003C_003E4__this.ReloadAllAsync(CancellationToken.None);
					}, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7CB95 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
				}
			}
			catch (OperationCanceledException)
			{
			}
		}, CS_0024_003C_003E8__locals7.token);
	}

	private async Task R7kS32ivQV()
	{
		_003C_003Ec__DisplayClass67_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass67_0();
		CS_0024_003C_003E8__locals15._003C_003E4__this = this;
		if (IsInitializingAllModules)
		{
			return;
		}
		DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6CB30E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
		try
		{
			IsInitializingAllModules = true;
			modulesBag.Clear();
			qDuShcG7sC();
			compiler.UnloadAll();
			if (PluginConfig.Instance().IsLastVersionDefault())
			{
				return;
			}
			List<string> paths = new List<string>();
			CS_0024_003C_003E8__locals15.bag = new ConcurrentDictionary<Type, byte>();
			paths.AddRange(from x in PluginConfig.Instance().ModuleLocal.Where<(string, bool)>(delegate((string Path, bool IsEnabled) x)
				{
					bool flag = x.IsEnabled && !string.IsNullOrWhiteSpace(x.Path);
					if (flag)
					{
						string text = Path.GetExtension(x.Path).ToLower();
						bool flag2 = ((text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F717563 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0) || text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBBFD1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2)) ? true : false);
						flag = flag2;
					}
					return flag && File.Exists(x.Path) && new FileInfo(x.Path).Length > 1024;
				})
				select x.Path);
			string[] builtInModulesPaths = BuiltInModulesPaths;
			foreach (string path in builtInModulesPaths)
			{
				await CS_0024_003C_003E8__locals15.NVlO8Pp9C7(path);
			}
			foreach (string item in paths)
			{
				await CS_0024_003C_003E8__locals15.NVlO8Pp9C7(item);
			}
			CS_0024_003C_003E8__locals15.modulesTemp = new ConcurrentDictionary<Type, ModuleBase>();
			CS_0024_003C_003E8__locals15.moduleTypes = CS_0024_003C_003E8__locals15.bag.Keys.ToArray();
			CS_0024_003C_003E8__locals15.workerCount = Math.Max(Environment.ProcessorCount / 2, 2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x55838342 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d));
			defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals15.moduleTypes.Length);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B3C87 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
			defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals15.workerCount);
			DLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
			await Task.Run(() => Parallel.ForEach(CS_0024_003C_003E8__locals15.moduleTypes, new ParallelOptions
			{
				MaxDegreeOfParallelism = CS_0024_003C_003E8__locals15.workerCount
			}, delegate(Type type3)
			{
				try
				{
					if (Activator.CreateInstance(type3) is ModuleBase value)
					{
						CS_0024_003C_003E8__locals15.modulesTemp.TryAdd(type3, value);
					}
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(17, 1);
					defaultInterpolatedStringHandler3.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFB806 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
					defaultInterpolatedStringHandler3.AppendFormatted(type3);
					defaultInterpolatedStringHandler3.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B3C15 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
					DLog.Error(defaultInterpolatedStringHandler3.ToStringAndClear(), ex);
				}
			}));
			foreach (var (type2, moduleBase2) in CS_0024_003C_003E8__locals15.modulesTemp)
			{
				PluginConfig.Instance().ModuleEnabled.TryAdd(type2.Name, PluginConfig.Instance().DefaultEnableModules && moduleBase2.Permission.IsDefaultEnabled());
			}
			modulesBag = CS_0024_003C_003E8__locals15.modulesTemp;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 1);
			defaultInterpolatedStringHandler2.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E941C01 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75));
			defaultInterpolatedStringHandler2.AppendFormatted(modulesBag.Count);
			DLog.Debug(defaultInterpolatedStringHandler2.ToStringAndClear());
			mnjSPRhfVq();
			PluginConfig.Instance().Save();
		}
		finally
		{
			IsInitializingAllModules = false;
			qDuShcG7sC();
		}
	}

	private bool s3ISBDmHmF(ModuleBase? module, out ModuleBase currentModule)
	{
		currentModule = null;
		if (module != null)
		{
			return IZFSmDDAwn(module.GetType(), out currentModule);
		}
		return false;
	}

	private bool IZFSmDDAwn(Type? moduleType, out ModuleBase currentModule)
	{
		currentModule = null;
		if (moduleType != null)
		{
			return modulesBag.TryGetValue(moduleType, out currentModule);
		}
		return false;
	}

	private void T7RSiPhtbb(ModuleBase module)
	{
		if (Activator.CreateInstance(module.GetType()) is ModuleBase value)
		{
			modulesBag[module.GetType()] = value;
			mnjSPRhfVq();
			qDuShcG7sC();
		}
	}

	private void mnjSPRhfVq()
	{
		_003C_003Ec__DisplayClass71_0 _003C_003Ec__DisplayClass71_2 = default(_003C_003Ec__DisplayClass71_0);
		_003C_003Ec__DisplayClass71_2._003C_003E4__this = this;
		_003C_003Ec__DisplayClass71_2.directPrerequisites = new Dictionary<ModuleBase, IReadOnlyList<ModuleBase>>(ReferenceEqualityComparer.Instance);
		foreach (KeyValuePair<Type, ModuleBase> item in modulesBag)
		{
			item.Deconstruct(out var key, out var value);
			Type type = key;
			ModuleBase moduleBase = value;
			moduleBase.PrecedingModules = BMpSbi1Ctj(moduleBase.Info.ModulesPrerequisite, ref _003C_003Ec__DisplayClass71_2);
			_003C_003Ec__DisplayClass71_2.directPrerequisites[moduleBase] = moduleBase.PrecedingModules;
			moduleBase.PairModules = BMpSbi1Ctj(moduleBase.Info.ModulesPair, ref _003C_003Ec__DisplayClass71_2);
			moduleBase.ConflictModules = BMpSbi1Ctj(moduleBase.Info.ModulesConflict, ref _003C_003Ec__DisplayClass71_2);
			moduleBase.ModuleIdentifier = type.FullName ?? moduleBase.ModuleName;
		}
		_003C_003Ec__DisplayClass71_2.prerequisitesClosureCache = new Dictionary<ModuleBase, List<ModuleBase>>(ReferenceEqualityComparer.Instance);
		foreach (ModuleBase value2 in modulesBag.Values)
		{
			value2.PrecedingModules = g3rSFQHCfa(value2, ref _003C_003Ec__DisplayClass71_2);
		}
		foreach (ModuleBase value3 in modulesBag.Values)
		{
			List<ModuleBase> conflictModules = value3.ConflictModules;
			if (conflictModules == null || conflictModules.Count <= 0)
			{
				continue;
			}
			foreach (ModuleBase conflictModule in value3.ConflictModules)
			{
				if (!conflictModule.ConflictModules.Contains(value3))
				{
					conflictModule.ConflictModules.Add(value3);
				}
			}
		}
	}

	private void qDuShcG7sC()
	{
		try
		{
			notifySemaphore.Release();
		}
		catch
		{
		}
	}

	public ModuleManager()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Modules = new List<ModuleBase>();
		modulesBag = new ConcurrentDictionary<Type, ModuleBase>();
		compiler = new ModuleCompiler();
		loadSemaphore = new SemaphoreSlim(4, 4);
		notifySemaphore = new SemaphoreSlim(0, 1);
		base._002Ector();
	}

	[CompilerGenerated]
	private bool wd7SoqI3as()
	{
		if (!IsInitializingAllModules && !IsUnloadingAllModules)
		{
			return !IsLoadingAllModules;
		}
		return false;
	}

	[CompilerGenerated]
	private bool gPtSgx33cW(ModuleBase m)
	{
		if (m != null && s3ISBDmHmF(m, out ModuleBase currentModule))
		{
			return currentModule.IsEnabled;
		}
		return false;
	}

	[CompilerGenerated]
	private Task qdBSwhL6lA(ModuleBase m)
	{
		return CR7SlJe3NW(m, affectConfig: true);
	}

	[CompilerGenerated]
	internal static void JSnSEjBSW6(Assembly assembly, string path)
	{
		IEnumerable<Type> types;
		try
		{
			types = assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			types = ex.Types;
		}
		foreach (Type item in types)
		{
			if (!(item == null) && !item.IsAbstract && typeof(ModuleBase).IsAssignableFrom(item) && !(item.GetConstructor(Type.EmptyTypes) != null))
			{
				DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339D488E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de) + (item.FullName ?? item.Name) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB5313 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503) + path);
			}
		}
	}

	[CompilerGenerated]
	private List<ModuleBase> BMpSbi1Ctj(string[] names, ref _003C_003Ec__DisplayClass71_0 P_1)
	{
		return (from x in names.Select(GetModuleByName)
			where x != null
			select x).OfType<ModuleBase>().ToList();
	}

	[CompilerGenerated]
	private List<ModuleBase> g3rSFQHCfa(ModuleBase module, ref _003C_003Ec__DisplayClass71_0 P_1)
	{
		_003C_003Ec__DisplayClass71_1 _003C_003Ec__DisplayClass71_2 = default(_003C_003Ec__DisplayClass71_1);
		_003C_003Ec__DisplayClass71_2.module = module;
		if (P_1.prerequisitesClosureCache.TryGetValue(_003C_003Ec__DisplayClass71_2.module, out List<ModuleBase> value))
		{
			return value;
		}
		_003C_003Ec__DisplayClass71_2.result = new List<ModuleBase>();
		_003C_003Ec__DisplayClass71_2.added = new HashSet<ModuleBase>(ReferenceEqualityComparer.Instance);
		_003C_003Ec__DisplayClass71_2.path = new HashSet<ModuleBase>(ReferenceEqualityComparer.Instance);
		SkBSqEw65w(_003C_003Ec__DisplayClass71_2.module, ref P_1, ref _003C_003Ec__DisplayClass71_2);
		P_1.prerequisitesClosureCache[_003C_003Ec__DisplayClass71_2.module] = _003C_003Ec__DisplayClass71_2.result;
		return _003C_003Ec__DisplayClass71_2.result;
	}

	[CompilerGenerated]
	private void SkBSqEw65w(ModuleBase current, ref _003C_003Ec__DisplayClass71_0 P_1, ref _003C_003Ec__DisplayClass71_1 P_2)
	{
		if (!P_2.path.Add(current))
		{
			return;
		}
		if (P_1.directPrerequisites.TryGetValue(current, out IReadOnlyList<ModuleBase> value) && value != null && value.Count > 0)
		{
			foreach (ModuleBase item in value)
			{
				if (item == P_2.module)
				{
					continue;
				}
				if (P_2.added.Add(item))
				{
					P_2.result.Add(item);
				}
				if (P_1.prerequisitesClosureCache.TryGetValue(item, out List<ModuleBase> value2))
				{
					foreach (ModuleBase item2 in value2)
					{
						if (item2 != P_2.module && P_2.added.Add(item2))
						{
							P_2.result.Add(item2);
						}
					}
				}
				else
				{
					SkBSqEw65w(item, ref P_1, ref P_2);
				}
			}
		}
		P_2.path.Remove(current);
	}
}
