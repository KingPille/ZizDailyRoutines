using System;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class OptimizedBorderlessWindow : ModuleBase
{
	private delegate nint WindowProcessDelegate(ulong hWnd, uint uMsg, ulong wParam, long lParam);

	private unsafe delegate void SetMainWindowBorderlessDelegate(GameWindow* self, bool borderless);

	private static class WinAPI
	{
		public struct Rect
		{
			public int Left;

			public int Top;

			public int Right;

			public int Bottom;
		}

		public struct MonitorInfo
		{
			public int CbSize;

			public Rect RCMonitor;

			public Rect RCWork;

			public uint DwFlags;
		}

		public struct WindowPos
		{
			public nint Hwnd;

			public nint HwndInsertAfter;

			public int X;

			public int Y;

			public int CX;

			public int CY;

			public uint Flags;
		}

		public const int GWLP_STYLE = -16;

		public const int SW_SHOW_MAXIMIZED = 3;

		public const int SW_RESTORE = 9;

		public const uint SWP_NO_SIZE = 1u;

		public const uint SWP_NO_MOVE = 2u;

		public const uint SWP_NO_Z_ORDER = 4u;

		public const uint SWP_FRAME_CHANGED = 32u;

		public const uint WM_WINDOW_POS_CHANGING = 70u;

		public const uint WM_NC_CALC_SIZE = 131u;

		public const uint MONITOR_DEFAULT_TO_PRIMARY = 1u;

		[DllImport("user32.dll", ExactSpelling = true)]
		public unsafe static extern bool GetWindowRect(nint hWnd, Rect* lpRect);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern ulong SetWindowLongPtrW(nint hWnd, int nIndex, ulong dwNewLong);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll", ExactSpelling = true)]
		public static extern bool ShowWindow(nint hWnd, int nCmdShow);

		[DllImport("user32.dll", ExactSpelling = true)]
		public unsafe static extern ulong MonitorFromRect(Rect* lprc, uint dwFlags);

		[DllImport("user32.dll", ExactSpelling = true)]
		public unsafe static extern bool GetMonitorInfoW(ulong hmonitor, MonitorInfo* lpmi);
	}

	private static readonly CompSig WindowProcessSig = new CompSig("40 55 53 56 57 41 54 41 56 48 8D 6C 24 ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 E0");

	private Hook<WindowProcessDelegate> WindowProcessHook;

	private static readonly CompSig SetMainWindowBorderlessSig = new CompSig("40 53 48 83 EC 60 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 44 24 ?? 48 8B D9 48 8B 49 18");

	private Hook<SetMainWindowBorderlessDelegate> SetMainWindowBorderlessHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedBorderlessWindowTitle"),
		Description = LanguageManager.Get("OptimizedBorderlessWindowDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		if (WindowProcessHook == null)
		{
			WindowProcessHook = WindowProcessSig.GetHook<WindowProcessDelegate>(WindowProcessDetour);
		}
		if (SetMainWindowBorderlessHook == null)
		{
			SetMainWindowBorderlessHook = SetMainWindowBorderlessSig.GetHook<SetMainWindowBorderlessDelegate>(SetMainWindowBorderlessDetour);
		}
		WindowProcessHook.Enable();
		SetMainWindowBorderlessHook.Enable();
		if (((GameWindow)((Framework)Framework.Instance()).GameWindow).Borderless)
		{
			MakeBorderless();
		}
	}

	protected unsafe override void Uninit()
	{
		if (base.IsEnabled && ((GameWindow)((Framework)Framework.Instance()).GameWindow).Borderless)
		{
			IntPtr windowHandle = ((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle;
			WinAPI.SetWindowLongPtrW(windowHandle, -16, 2147483648uL);
			WinAPI.ShowWindow(windowHandle, 3);
			WinAPI.SetWindowPos(windowHandle, 0, 0, 0, 0, 0, 39u);
		}
	}

	private unsafe nint WindowProcessDetour(ulong hWnd, uint uMsg, ulong wParam, long lParam)
	{
		switch (uMsg)
		{
		case 70u:
			if (((GameWindow)((Framework)Framework.Instance()).GameWindow).Borderless)
			{
				WinAPI.WindowPos* ptr = (WinAPI.WindowPos*)lParam;
				if ((ptr->Flags & 1) == 0)
				{
					WinAPI.Rect rect = new WinAPI.Rect
					{
						Left = ptr->X,
						Top = ptr->Y,
						Right = ptr->X + ptr->CX,
						Bottom = ptr->Y + ptr->CY
					};
					ConvertToBorderlessRect(ref rect);
					ptr->X = rect.Left;
					ptr->Y = rect.Top;
					ptr->CX = rect.Right - ptr->X;
					ptr->CY = rect.Bottom - ptr->Y;
					SyncSwapchainResolution(ptr->CX, ptr->CY);
				}
				return 1;
			}
			break;
		case 131u:
			if (wParam != 0L && ((GameWindow)((Framework)Framework.Instance()).GameWindow).Borderless)
			{
				return 0;
			}
			break;
		}
		return WindowProcessHook.Original(hWnd, uMsg, wParam, lParam);
	}

	private unsafe void SetMainWindowBorderlessDetour(GameWindow* self, bool borderless)
	{
		if (borderless)
		{
			((GameWindow)self).Borderless = true;
			MakeBorderless();
		}
		else
		{
			SetMainWindowBorderlessHook.Original(self, borderless);
		}
	}

	private unsafe static void MakeBorderless()
	{
		IntPtr windowHandle = ((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle;
		WinAPI.Rect rect = default(WinAPI.Rect);
		WinAPI.GetWindowRect(windowHandle, &rect);
		ConvertToBorderlessRect(ref rect);
		WinAPI.SetWindowLongPtrW(windowHandle, -16, 2160984064uL);
		WinAPI.ShowWindow(windowHandle, 9);
		WinAPI.SetWindowPos(windowHandle, 0, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, 36u);
	}

	private unsafe static void SyncSwapchainResolution(int width, int height)
	{
		if (width > 0 && height > 0)
		{
			Device* ptr = Device.Instance();
			if (((Device)ptr).NewWidth != width || ((Device)ptr).NewHeight != height)
			{
				((Device)ptr).NewWidth = (uint)width;
				((Device)ptr).NewHeight = (uint)height;
				((Device)ptr).RequestResolutionChange = 1;
			}
		}
	}

	private unsafe static void ConvertToBorderlessRect(ref WinAPI.Rect rect)
	{
		WinAPI.Rect rect2 = rect;
		ulong num = WinAPI.MonitorFromRect(&rect2, 1u);
		if (num != 0L)
		{
			WinAPI.MonitorInfo monitorInfo = new WinAPI.MonitorInfo
			{
				CbSize = sizeof(WinAPI.MonitorInfo)
			};
			if (WinAPI.GetMonitorInfoW(num, &monitorInfo))
			{
				rect = monitorInfo.RCMonitor;
			}
		}
	}
}
