using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Dalamud.Interface.Windowing;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public sealed class WindowManager : OmenServiceBase<WindowManager>
{
	private readonly record struct DrawScopeRegistration(int TokenID, Func<IDisposable?> Factory);

	[CompilerGenerated]
	private WindowSystem _003CWindowSystem_003Ek__BackingField;

	private ImmutableArray<DrawScopeRegistration> drawScopeRegistrations = ImmutableArray<DrawScopeRegistration>.Empty;

	private int nextDrawScopeRegistrationID;

	public WindowSystem WindowSystem
	{
		get
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			if (_003CWindowSystem_003Ek__BackingField != null)
			{
				return _003CWindowSystem_003Ek__BackingField;
			}
			_003CWindowSystem_003Ek__BackingField = new WindowSystem(DalamudServiceExtension.Instance().InternalName);
			DalamudLeftServiceExtension.Instance().Draw += OnWindowManagerDraw;
			return _003CWindowSystem_003Ek__BackingField;
		}
	}

	public ConcurrentDictionary<Type, Window> UniqueWindows { get; } = new ConcurrentDictionary<Type, Window>();

	public event Action? PreDraw;

	public event Action? PostDraw;

	protected override void Uninit()
	{
		DalamudLeftServiceExtension.Instance().Draw -= OnWindowManagerDraw;
		this.PreDraw = null;
		this.PostDraw = null;
		drawScopeRegistrations = ImmutableArray<DrawScopeRegistration>.Empty;
		WindowSystem.RemoveAllWindows();
		foreach (Window value in UniqueWindows.Values)
		{
			if (value is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		UniqueWindows.Clear();
	}

	private void OnWindowManagerDraw()
	{
		ImmutableArray<DrawScopeRegistration> immutableArray = drawScopeRegistrations;
		IDisposable[] array = null;
		try
		{
			if (!immutableArray.IsDefaultOrEmpty)
			{
				array = ArrayPool<IDisposable>.Shared.Rent(immutableArray.Length);
				Array.Clear(array, 0, immutableArray.Length);
				for (int i = 0; i < immutableArray.Length; i++)
				{
					array[i] = immutableArray[i].Factory();
				}
			}
			this.PreDraw?.Invoke();
			WindowSystem.Draw();
			this.PostDraw?.Invoke();
		}
		finally
		{
			if (array != null)
			{
				for (int num = immutableArray.Length - 1; num >= 0; num--)
				{
					array[num]?.Dispose();
					array[num] = null;
				}
				ArrayPool<IDisposable>.Shared.Return(array);
			}
		}
	}

	public DrawScopesHandle RegDrawScopes(params Func<IDisposable?>[] factories)
	{
		ArgumentNullException.ThrowIfNull(factories, "factories");
		if (factories.Length == 0)
		{
			return default(DrawScopesHandle);
		}
		Func<IDisposable>[] array = factories;
		for (int i = 0; i < array.Length; i++)
		{
			ArgumentNullException.ThrowIfNull(array[i], "t");
		}
		DrawScopesHandle result = new DrawScopesHandle(Interlocked.Increment(ref nextDrawScopeRegistrationID));
		array = factories;
		foreach (Func<IDisposable> factory in array)
		{
			ImmutableInterlocked.Update(transformerArgument: new DrawScopeRegistration(result.ID, factory), location: ref drawScopeRegistrations, transformer: (ImmutableArray<DrawScopeRegistration> registrations, DrawScopeRegistration item) => registrations.Add(item));
		}
		return result;
	}

	public void UnregDrawScopes(DrawScopesHandle handle)
	{
		if (!handle.IsValid)
		{
			return;
		}
		ImmutableInterlocked.Update(ref drawScopeRegistrations, (ImmutableArray<DrawScopeRegistration> registrations, int targetID) => registrations.RemoveAll((DrawScopeRegistration registration) => registration.TokenID == targetID), handle.ID);
	}

	public bool AddWindow<T>(bool isForceToAdd = false) where T : Window
	{
		return AddWindow(() => Activator.CreateInstance<T>(), isForceToAdd);
	}

	public bool AddWindow<T>(Func<T> factory, bool isForceToAdd = false) where T : Window
	{
		ArgumentNullException.ThrowIfNull(factory, "factory");
		if (!isForceToAdd && UniqueWindows.ContainsKey(typeof(T)))
		{
			return false;
		}
		if (UniqueWindows.TryGetValue(typeof(T), out Window value))
		{
			WindowSystem.RemoveWindow((IWindow)(object)value);
			if (value is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		T val = factory();
		if (!AddWindow((Window)(object)val, isForceToAdd))
		{
			return false;
		}
		UniqueWindows[typeof(T)] = (Window)(object)val;
		return true;
	}

	public bool RemoveWindow<T>() where T : Window
	{
		if (!UniqueWindows.TryRemove(typeof(T), out Window value))
		{
			return false;
		}
		WindowSystem.RemoveWindow((IWindow)(object)value);
		if (value is IDisposable disposable)
		{
			disposable.Dispose();
		}
		return true;
	}

	public bool AddWindow(Window window, bool isForceToAdd = false)
	{
		ArgumentNullException.ThrowIfNull(window, "window");
		IWindow val = WindowSystem.Windows.FirstOrDefault((IWindow x) => (object)x == window || x.WindowName == window.WindowName);
		if (val != null)
		{
			if (!isForceToAdd && (object)val == window)
			{
				return false;
			}
			WindowSystem.RemoveWindow(val);
			if ((object)val != window && val is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		WindowSystem.AddWindow((IWindow)(object)window);
		return true;
	}

	public bool RemoveWindow(Window? window)
	{
		if (window == null)
		{
			return false;
		}
		if (!WindowSystem.Windows.Contains((IWindow)(object)window))
		{
			return false;
		}
		WindowSystem.RemoveWindow((IWindow)(object)window);
		if (window is IDisposable disposable)
		{
			disposable.Dispose();
		}
		return true;
	}

	public T? Get<T>() where T : Window
	{
		if (!UniqueWindows.TryGetValue(typeof(T), out Window value))
		{
			return default(T);
		}
		return (T)(object)((value is T) ? value : null);
	}
}
