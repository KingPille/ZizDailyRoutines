using System;
using System.Runtime.InteropServices;

namespace OmenTools.Interop.Windows.Models;

public struct LastInputInfo
{
	public uint Size;

	public uint LastInputTickCount;

	[DllImport("User32.dll")]
	public static extern bool GetLastInputInfo(ref LastInputInfo info);

	public static TimeSpan GetIdleTime()
	{
		LastInputInfo info = new LastInputInfo
		{
			Size = (uint)Marshal.SizeOf<LastInputInfo>()
		};
		GetLastInputInfo(ref info);
		return TimeSpan.FromMilliseconds(Environment.TickCount - (int)info.LastInputTickCount);
	}

	public static long GetIdleTimeTick()
	{
		LastInputInfo info = new LastInputInfo
		{
			Size = (uint)Marshal.SizeOf<LastInputInfo>()
		};
		GetLastInputInfo(ref info);
		return Environment.TickCount64 - info.LastInputTickCount;
	}
}
