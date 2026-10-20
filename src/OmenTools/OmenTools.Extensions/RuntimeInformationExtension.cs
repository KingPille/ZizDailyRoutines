using System;
using System.Runtime.InteropServices;

namespace OmenTools.Extensions;

public static class RuntimeInformationExtension
{
	extension(RuntimeInformation)
	{
		public static bool IsWine()
		{
			return GetNtdllProcAddress("wine_get_version") != IntPtr.Zero;
		}

		public static string? GetWineVersion()
		{
			return GetWineStringExport("wine_get_version");
		}

		public static string? GetWineBuildID()
		{
			return GetWineStringExport("wine_get_build_id");
		}
	}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	private delegate nint WineStringExportDelegate();

	private static nint GetNtdllProcAddress(string procName)
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return IntPtr.Zero;
		}
		nint moduleHandle = GetModuleHandle("ntdll.dll");
		if (moduleHandle != IntPtr.Zero)
		{
			return GetProcAddress(moduleHandle, procName);
		}
		return IntPtr.Zero;
	}

	private static string? GetWineStringExport(string exportName)
	{
		nint ntdllProcAddress = GetNtdllProcAddress(exportName);
		if (ntdllProcAddress == IntPtr.Zero)
		{
			return null;
		}
		return Marshal.PtrToStringAnsi(Marshal.GetDelegateForFunctionPointer<WineStringExportDelegate>(ntdllProcAddress)());
	}

	[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
	private static extern nint GetModuleHandle(string moduleName);

	[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
	private static extern nint GetProcAddress(nint moduleHandle, string procName);
}
