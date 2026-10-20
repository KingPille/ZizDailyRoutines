using System;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;

namespace OmenTools.Dalamud;

public static class DLog
{
	public static void Verbose(string message)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Verbose(message, Array.Empty<object>());
	}

	public static void Verbose(string message, Exception ex)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Verbose(ex, message, Array.Empty<object>());
	}

	public static void Verbose(Exception ex)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Verbose(ex, ex.ToString(), Array.Empty<object>());
	}

	public static void Debug(string message)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Debug(message, Array.Empty<object>());
	}

	public static void Debug(string message, Exception ex)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Debug(ex, message, Array.Empty<object>());
	}

	public static void Debug(Exception ex)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Debug(ex, ex.ToString(), Array.Empty<object>());
	}

	public static void Warning(string message)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Warning(message, Array.Empty<object>());
	}

	public static void Warning(string message, Exception ex)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Warning(ex, message, Array.Empty<object>());
	}

	public static void Warning(Exception ex)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Warning(ex, ex.ToString(), Array.Empty<object>());
	}

	public static void Error(string message)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Error(message, Array.Empty<object>());
	}

	public static void Error(string message, Exception ex)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Error(ex, message, Array.Empty<object>());
	}

	public static void Error(Exception ex)
	{
		DalamudServiceExtension.Instance<IPluginLog>().Error(ex, ex.ToString(), Array.Empty<object>());
	}
}
