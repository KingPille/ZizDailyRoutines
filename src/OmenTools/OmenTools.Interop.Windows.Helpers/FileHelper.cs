using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OmenTools.Interop.Windows.Helpers;

public class FileHelper
{
	public static void OpenByPath(string path, bool selectFile = false)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		try
		{
			string fileName;
			string arguments;
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				fileName = "explorer.exe";
				arguments = (selectFile ? ("/select,\"" + path + "\"") : path);
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
			{
				fileName = "open";
				arguments = (selectFile ? ("-R \"" + path + "\"") : path);
			}
			else
			{
				if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
				{
					return;
				}
				fileName = "xdg-open";
				arguments = path;
			}
			using Process process = new Process();
			process.StartInfo = new ProcessStartInfo
			{
				FileName = fileName,
				Arguments = arguments,
				UseShellExecute = (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !selectFile),
				CreateNoWindow = !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
			};
			_ = 0;
		}
		catch (Exception)
		{
		}
	}

	public static async Task CopyAsync(string sourcePath, string destinationPath)
	{
		await using FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
		await using FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
		await sourceStream.CopyToAsync(destinationStream);
	}
}
