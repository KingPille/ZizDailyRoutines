using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;

namespace DailyRoutines.ModulesPublic;

public class AutoAllowMultipleGames : ModuleBase
{
	private struct ProcessHandleTableEntryInfo
	{
		public ulong HandleValue;

		public ulong HandleCount;

		public ulong PointerCount;

		public uint GrantedAccess;

		public uint ObjectTypeIndex;

		public uint HandleAttributes;

		public uint Reserved;
	}

	private struct ProcessHandleSnapshotInformation
	{
		public ulong NumberOfHandles;

		public ulong Reserved;
	}

	private struct UnicodeString
	{
		public ushort Length;

		public ushort MaximumLength;

		public unsafe byte* Buffer;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoAllowMultipleGamesTitle"),
		Description = LanguageManager.Get("AutoAllowMultipleGamesDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "Fragile" }
	};

	protected override void Init()
	{
		foreach (ulong item in EnumHandles())
		{
			if (ObjectNameOrTypeName(item, typeName: true) == "Mutant" && ObjectNameOrTypeName(item, typeName: false).Contains("6AA83AB5-BAC4-4a36-9F66-A309770760CB_ffxiv_game0", StringComparison.Ordinal))
			{
				CloseHandle(item);
			}
		}
	}

	private unsafe static List<ulong> EnumHandles()
	{
		List<ulong> list = new List<ulong>();
		uint num = 32768u;
		while (true)
		{
			byte[] array = new byte[num];
			fixed (byte* ptr = &array[0])
			{
				ProcessHandleSnapshotInformation* ptr2 = (ProcessHandleSnapshotInformation*)ptr;
				ptr2->NumberOfHandles = 0uL;
				uint num2 = 0u;
				int num3 = NtQueryInformationProcess(ulong.MaxValue, 51, ptr, num, &num2);
				if (num3 == -1073741820)
				{
					num = num2;
					continue;
				}
				if (num3 >= 0)
				{
					ProcessHandleTableEntryInfo* ptr3 = (ProcessHandleTableEntryInfo*)(ptr2 + 1);
					for (ulong num4 = 0uL; num4 < ptr2->NumberOfHandles; num4++)
					{
						list.Add(ptr3[num4].HandleValue);
					}
				}
			}
			break;
		}
		return list;
	}

	private unsafe static string ObjectNameOrTypeName(ulong handle, bool typeName)
	{
		fixed (byte* ptr = &(new byte[1024])[0])
		{
			uint num = 0u;
			if (NtQueryObject(handle, (!typeName) ? 1 : 2, ptr, 1024u, &num) >= 0)
			{
				UnicodeString* ptr2 = (UnicodeString*)ptr;
				if (ptr2->Buffer != null)
				{
					return Encoding.Unicode.GetString(ptr2->Buffer, ptr2->Length);
				}
			}
		}
		return string.Empty;
	}

	[DllImport("ntdll.dll")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	private unsafe static extern int NtQueryInformationProcess(ulong ProcessHandle, int ProcessInformationClass, void* ProcessInformation, uint ProcessInformationLength, uint* ReturnLength);

	[DllImport("ntdll.dll")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	private unsafe static extern int NtQueryObject(ulong Handle, int ObjectInformationClass, void* ObjectInformation, uint ObjectInformationLength, uint* ReturnLength);

	[DllImport("kernel32.dll")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool CloseHandle(ulong Handle);
}
