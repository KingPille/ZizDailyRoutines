using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using OmenTools.Extensions;

namespace OmenTools.Interop.Windows.Helpers;

public static class KeyEmulationHelper
{
	private enum WindowMessage : uint
	{
		WmKeydown = 256u,
		WmKeyup = 257u,
		WmXbuttondown = 523u,
		WmXbuttonup = 524u
	}

	private unsafe static nint GameWindowHandle => ((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle;

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	private static extern nint SendMessage(nint hWnd, WindowMessage msg, nint wParam, nint lParam);

	public static bool SendKeypress(Keys key)
	{
		return SendKeypress((int)key);
	}

	public static bool SendMousepress(Keys key)
	{
		return SendKeypress((int)key);
	}

	public static bool SendKeypress(int key)
	{
		SendMessage(GameWindowHandle, WindowMessage.WmKeydown, key, 0);
		SendMessage(GameWindowHandle, WindowMessage.WmKeyup, key, 0);
		return true;
	}

	public static void SendMousepress(int key)
	{
		switch (key)
		{
		case 5:
		{
			int num2 = MakeWParam(0, 1);
			SendMessage(GameWindowHandle, WindowMessage.WmXbuttondown, num2, 0);
			SendMessage(GameWindowHandle, WindowMessage.WmXbuttonup, num2, 0);
			break;
		}
		case 6:
		{
			int num = MakeWParam(0, 2);
			SendMessage(GameWindowHandle, WindowMessage.WmXbuttondown, num, 0);
			SendMessage(GameWindowHandle, WindowMessage.WmXbuttonup, num, 0);
			break;
		}
		default:
			DalamudServiceExtension.Instance<IPluginLog>().Error($"尝试发送非法按键 {key}, 已阻止", Array.Empty<object>());
			break;
		}
	}

	public static void SendKeyDown(Keys key)
	{
		SendKeyDown((int)key);
	}

	public static void SendKeyUp(Keys key)
	{
		SendKeyUp((int)key);
	}

	public static void SendKeyDown(int key)
	{
		SendMessage(GameWindowHandle, WindowMessage.WmKeydown, key, 0);
	}

	public static void SendKeyUp(int key)
	{
		SendMessage(GameWindowHandle, WindowMessage.WmKeyup, key, 0);
	}

	public static async Task<bool> SendKeypressLongPressAsync(Keys key, int durationMilliseconds)
	{
		SendMessage(GameWindowHandle, WindowMessage.WmKeydown, (nint)key, 0);
		await Task.Delay(durationMilliseconds);
		SendMessage(GameWindowHandle, WindowMessage.WmKeyup, (nint)key, 0);
		return true;
	}

	private static int MakeWParam(int l, int h)
	{
		return (l & 0xFFFF) | (h << 16);
	}
}
