using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace OmenTools.Interop.Windows.Helpers;

public class IMEHelper
{
	private unsafe static nint GameWindowHandle => ((GameWindow)((Framework)Framework.Instance()).GameWindow).WindowHandle;

	[DllImport("imm32.dll")]
	private static extern nint ImmGetContext(nint hwnd);

	[DllImport("imm32.dll")]
	private static extern bool ImmSetOpenStatus(nint himc, bool b);

	public static void Disable()
	{
		ImmSetOpenStatus(ImmGetContext(GameWindowHandle), b: false);
	}

	public static void Enable()
	{
		ImmSetOpenStatus(ImmGetContext(GameWindowHandle), b: true);
	}
}
