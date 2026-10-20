using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.LocalLifecycle;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;

namespace DailyRoutines.ModulesPublic;

public class AutoLockGameWindow : ModuleBase
{
	private static class WindowLock
	{
		private struct Rect
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;
		}

		private struct WindowPos
		{
			public nint hwnd;

			public nint hwndInsertAfter;

			public int x;

			public int y;

			public int cx;

			public int cy;

			public uint flags;
		}

		private delegate nint WndProcDelegate(nint hWnd, uint uMsg, nint wParam, nint lParam);

		private const int GWL_WNDPROC = -4;

		private const int WM_WINDOWPOSCHANGING = 70;

		private const uint SWP_NOMOVE = 2u;

		private static readonly Dictionary<nint, nint> WindowProcMap = new Dictionary<nint, nint>();

		private static readonly Dictionary<nint, WndProcDelegate> WndProcDelegates = new Dictionary<nint, WndProcDelegate>();

		[DllImport("user32.dll", SetLastError = true)]
		private static extern nint SetWindowLongPtr(nint hWnd, int nIndex, nint newProc);

		[DllImport("user32.dll")]
		private static extern nint CallWindowProc(nint lpPrevWndFunc, nint hWnd, uint uMsg, nint wParam, nint lParam);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetWindowRect(nint hWnd, out Rect lpRect);

		public static void LockWindowByHandle(nint hWnd)
		{
			if (hWnd != IntPtr.Zero)
			{
				SubclassWindow(hWnd);
			}
		}

		public static void UnlockWindow(nint hWnd)
		{
			if (hWnd != IntPtr.Zero && WindowProcMap.TryGetValue(hWnd, out var value))
			{
				SetWindowLongPtr(hWnd, -4, value);
				WindowProcMap.Remove(hWnd);
				WndProcDelegates.Remove(hWnd);
			}
		}

		private static void SubclassWindow(nint hWnd)
		{
			WndProcDelegate wndProcDelegate = NewWindowProc;
			nint functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate(wndProcDelegate);
			if (!WindowProcMap.ContainsKey(hWnd))
			{
				nint num = SetWindowLongPtr(hWnd, -4, functionPointerForDelegate);
				if (num == IntPtr.Zero && Marshal.GetLastWin32Error() != 0)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to subclass window.");
				}
				WindowProcMap[hWnd] = num;
				WndProcDelegates[hWnd] = wndProcDelegate;
			}
		}

		private static nint NewWindowProc(nint hWnd, uint uMsg, nint wParam, nint lParam)
		{
			if (uMsg == 70)
			{
				WindowPos structure = Marshal.PtrToStructure<WindowPos>(lParam);
				if ((structure.flags & 2) == 0)
				{
					GetWindowRect(hWnd, out var lpRect);
					structure.x = lpRect.Left;
					structure.y = lpRect.Top;
					structure.flags |= 2u;
					Marshal.StructureToPtr(structure, lParam, fDeleteOld: true);
				}
			}
			return CallWindowProc(WindowProcMap[hWnd], hWnd, uMsg, wParam, lParam);
		}

		public static void Cleanup()
		{
			foreach (nint item in WindowProcMap.Keys.ToList())
			{
				UnlockWindow(item);
			}
		}
	}

	private bool isLocked;

	private readonly Lock objectLock = new Lock();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoLockGameWindowTitle"),
		Description = LanguageManager.Get("AutoLockGameWindowDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "status102" }
	};

	protected override void Init()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		WindowLifetime.Start(this);
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChange);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChange);
		WindowLifetime.Stop(this, WindowLock.Cleanup);
		isLocked = false;
	}

	private void OnConditionChange(ConditionFlag flag, bool value)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		if ((int)flag != 26)
		{
			return;
		}
		WindowLifetime.Queue(delegate
		{
			using (objectLock.EnterScope())
			{
				if (value)
				{
					if (!isLocked)
					{
						WindowLock.LockWindowByHandle(Process.GetCurrentProcess().MainWindowHandle);
						isLocked = true;
					}
				}
				else if (isLocked)
				{
					WindowLock.UnlockWindow(Process.GetCurrentProcess().MainWindowHandle);
					isLocked = false;
				}
			}
		}, this);
	}
}
