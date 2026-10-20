using System;
using System.Diagnostics;
using System.IO;

namespace DailyRoutines.LocalLifecycle;

public static class LocalShutdown
{
	public static ProcessStartInfo BuildStartInfo(ProcessStartInfo request)
	{
		ArgumentNullException.ThrowIfNull(request, "request");
		if (request.FileName != "shutdown" || request.Arguments != "/s /t 0" || request.UseShellExecute || !request.CreateNoWindow)
		{
			throw new ArgumentException("Only the module's fixed local shutdown operation is permitted.", "request");
		}
		return new ProcessStartInfo
		{
			FileName = Path.Combine(Environment.SystemDirectory, "shutdown.exe"),
			WorkingDirectory = Environment.SystemDirectory,
			Arguments = "/s /t 0",
			UseShellExecute = false,
			CreateNoWindow = true,
			WindowStyle = ProcessWindowStyle.Hidden
		};
	}

	public static void Start(ProcessStartInfo request)
	{
		using (Process.Start(BuildStartInfo(request)))
		{
		}
	}
}
