using System.Collections.Concurrent;

namespace OmenTools.Dalamud.Abstractions;

internal static class IPCErrorNotificationRegistry
{
	private static readonly ConcurrentDictionary<string, byte> NotifiedErrorIpcNames = new ConcurrentDictionary<string, byte>();

	public static bool TryMark(string ipcName)
	{
		return NotifiedErrorIpcNames.TryAdd(ipcName, 0);
	}

	public static void Clear(string ipcName)
	{
		NotifiedErrorIpcNames.TryRemove(ipcName, out var _);
	}
}
