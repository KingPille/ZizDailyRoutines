using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using BYb1aHVl8WNHMBcTPBF;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using DailyRoutines.Common.Runtime.Hosts;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiToolKit;
using Lumina.Text.ReadOnly;
using NeerNbVPI0nYdnUeJa4;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Interop.Windows;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using gf0KuaVmiIFujFRxjxr;
using itAnDoW4mgYVlrmtMor;
using l5yB83SNcfeBgR8dU1a;
using scAmY5SuvRnwiSBZk0U;
using uDlvffVTAfCYh84Mvr7;

namespace nXyRYE1uBjK6gmU4ln;

internal sealed class Y5SWncTBsGUDX5l2wY
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass28_0
	{
		public IEnumerable<KeyValuePair<Type, ManagerBase>> managers;

		public _003C_003Ec__DisplayClass28_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal async Task TprCQgNwIC()
		{
			foreach (KeyValuePair<Type, ManagerBase> item in managers)
			{
				item.Deconstruct(out var _, out var value);
				await value.PublicUninitAsync();
			}
		}
	}

	public static bool IsDisposed { get; private set; }

	public static bool IsInitialized { get; private set; }

	public static bool IsUninitialized { get; private set; }

	private static Y5SWncTBsGUDX5l2wY? InternalInstance { get; set; }

	private Dictionary<Type, ManagerBase> Managers { get; set; }

	public static T? S36MXGucI<T>() where T : ManagerBase
	{
		Y5SWncTBsGUDX5l2wY? internalInstance = InternalInstance;
		if (internalInstance == null)
		{
			return null;
		}
		return internalInstance.lFKvQfxX7<T>();
	}

	internal static async Task giQ4vv6A4(IDalamudPluginInterface pluginInterface, IFramework framework)
	{
		if (!IsInitialized && !IsDisposed)
		{
			Y5SWncTBsGUDX5l2wY instance = p7S8tpP8V();
			try
			{
				DService.Init(pluginInterface);
				CfthEvVvHg7NlMWUcpC.lhGVxtIoo5();
				await KamiToolKitLibrary.InitializeAsync(pluginInterface);
				await instance.zDxlFncVg();
				bVepoDfix();
				await instance.DKIxXeoHJ();
				L4hg3GSeYIg8LCt4TTD.SrrS7I04uM();
				wO3bJ6VWsAShvvFiV8Z.w7XV16Dt7d();
				PluginIPC.DkvVsbimBn();
				IsInitialized = true;
			}
			catch (Exception ex)
			{
				DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F714D49 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0), ex);
				await ubxsjVN6F();
				throw;
			}
		}
	}

	internal static async Task ubxsjVN6F()
	{
		if (IsDisposed || InternalInstance == null)
		{
			return;
		}
		IsUninitialized = true;
		try
		{
			PluginIPC.T3gV89Lodq();
			CfthEvVvHg7NlMWUcpC.K0uVpi72Qd();
			L4hg3GSeYIg8LCt4TTD.vUASdW1BSU();
			wO3bJ6VWsAShvvFiV8Z.npnVMFukSB();
			TgWnIVSUWGDITG09VVO.y8RSY1mjAH();
			nhCZ7gVikRlqDFNgTmi.RWnVoJKMqc();
			await InternalInstance.xY93MNpKI();
			PluginConfig.Instance().Save();
			await DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
			{
				try
				{
					KamiToolKitLibrary.Dispose();
				}
				catch (Exception ex2)
				{
					DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEFD6D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b), ex2);
				}
			}, default(CancellationToken));
			DService.Uninit();
			InternalInstance.YuTBjRRka();
		}
		catch (Exception ex)
		{
			DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D343212 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33), ex);
			throw;
		}
		finally
		{
			IsDisposed = true;
			IsUninitialized = false;
		}
	}

	private static Y5SWncTBsGUDX5l2wY p7S8tpP8V()
	{
		return InternalInstance ?? (InternalInstance = new Y5SWncTBsGUDX5l2wY());
	}

	private T? lFKvQfxX7<T>() where T : ManagerBase
	{
		return (T)Managers.GetValueOrDefault(typeof(T));
	}

	private async Task zDxlFncVg()
	{
		foreach (var item in (from t in Assembly.GetExecutingAssembly().GetTypes().Concat(typeof(ManagerInitOrderAttribute).Assembly.GetTypes())
			where typeof(ManagerBase).IsAssignableFrom(t) && !t.IsAbstract
			select new
			{
				Type = t,
				Order = (t.GetCustomAttribute<ManagerInitOrderAttribute>()?.Order ?? int.MaxValue)
			} into t
			orderby t.Order
			select t).ToList())
		{
			if (Activator.CreateInstance(item.Type) is ManagerBase value)
			{
				Managers.TryAdd(item.Type, value);
			}
		}
		ManagerHost.Current = rOfUNRVByTWOX4reV4O.Instance;
		await DalamudServiceExtension.Instance<IFramework>().Run((Func<Task>)async delegate
		{
			foreach (KeyValuePair<Type, ManagerBase> manager in Managers)
			{
				manager.Deconstruct(out var _, out var value2);
				await value2.PublicInitAsync();
			}
		}, default(CancellationToken));
	}

	private async Task DKIxXeoHJ()
	{
		await DalamudServiceExtension.Instance<IFramework>().Run((Func<Task>)async delegate
		{
			foreach (KeyValuePair<Type, ManagerBase> manager in Managers)
			{
				manager.Deconstruct(out var _, out var value);
				await value.PublicPostInitAsync();
			}
		}, default(CancellationToken));
	}

	private static void bVepoDfix()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ReadOnlySeString? val = (PluginConfig.Instance().StripeDRPrefix ? ((ReadOnlySeString?)null) : new ReadOnlySeString?(Assets.BoxedLettersDR));
		DalamudLeftServiceExtension.Instance().DisableCutsceneUiHide = true;
		OmenServiceBase<ContextMenuManager>.Instance().DefaultPrefix = val;
		NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
		notifyHelper.ChatPrefix = val;
		notifyHelper.NotificationIcon = Assets.Icon;
		if (!RuntimeInformationExtension.IsWine())
		{
			notifyHelper.TrayNotifier = new TrayNotifier
			{
				Icon = Icon.ExtractAssociatedIcon(Path.Join(DalamudServiceExtension.Instance().AssemblyLocation.DirectoryName, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D56C677 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x466ACAC7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7))),
				MultiMessageTemplate = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CE40BFB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff))
			};
		}
		AetheryteRecordManager aetheryteRecordManager = OmenServiceBase<AetheryteRecordManager>.Instance();
		aetheryteRecordManager.HouseApartmentTemplate = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4C9985B9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1));
		aetheryteRecordManager.HouseEstateTemplate = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76981A41 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
		aetheryteRecordManager.HouseSharedName = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FFDF911 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
		aetheryteRecordManager.HouseFreeCompanyName = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E0590CD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
		aetheryteRecordManager.HousePersonalName = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x415A9ACB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
		TgWnIVSUWGDITG09VVO.MyvSX3JCpS();
		nhCZ7gVikRlqDFNgTmi.GcIVhc0oQg();
	}

	private async Task xY93MNpKI()
	{
		_003C_003Ec__DisplayClass28_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass28_0();
		CS_0024_003C_003E8__locals2.managers = Managers.Reverse();
		await DalamudServiceExtension.Instance<IFramework>().Run((Func<Task>)async delegate
		{
			foreach (KeyValuePair<Type, ManagerBase> item in CS_0024_003C_003E8__locals2.managers)
			{
				item.Deconstruct(out var _, out var value);
				await value.PublicUninitAsync();
			}
		}, default(CancellationToken));
	}

	private void YuTBjRRka()
	{
		ManagerHost.Current = null;
		Managers = new Dictionary<Type, ManagerBase>();
	}

	public Y5SWncTBsGUDX5l2wY()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Managers = new Dictionary<Type, ManagerBase>();
		base._002Ector();
	}

	[CompilerGenerated]
	private async Task iR3mPJ2TJ()
	{
		foreach (KeyValuePair<Type, ManagerBase> manager in Managers)
		{
			manager.Deconstruct(out var _, out var value);
			await value.PublicInitAsync();
		}
	}

	[CompilerGenerated]
	private async Task i7riELtH9()
	{
		foreach (KeyValuePair<Type, ManagerBase> manager in Managers)
		{
			manager.Deconstruct(out var _, out var value);
			await value.PublicPostInitAsync();
		}
	}
}
