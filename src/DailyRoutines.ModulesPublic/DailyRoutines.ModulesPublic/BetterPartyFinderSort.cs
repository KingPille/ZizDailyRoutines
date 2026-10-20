using System;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class BetterPartyFinderSort : ModuleBase
{
	private delegate byte PartyFinderSortCmpDelegate(nint a1, nint a2);

	private interface ISortStrategy
	{
		byte Compare(PartyFinderListing a1, PartyFinderListing a2);
	}

	private class TimeLeftAscendingStrategy : ISortStrategy
	{
		public byte Compare(PartyFinderListing a1, PartyFinderListing a2)
		{
			return (a1.TimeLeftSeconds < a2.TimeLeftSeconds) ? ((byte)1) : ((byte)0);
		}
	}

	private class TimeLeftDescendingStrategy : ISortStrategy
	{
		public byte Compare(PartyFinderListing a1, PartyFinderListing a2)
		{
			return (a1.TimeLeftSeconds > a2.TimeLeftSeconds) ? ((byte)1) : ((byte)0);
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = 416)]
	private struct PartyFinderListing
	{
		[FieldOffset(32)]
		public ushort DutyID;

		[FieldOffset(68)]
		public uint TimeLeftSeconds;

		[FieldOffset(408)]
		public byte Unknown408;

		[FieldOffset(409)]
		public byte Unknown409;

		[FieldOffset(410)]
		public byte IsDutyUnlocked;

		[FieldOffset(411)]
		public byte IsBlacklisted;
	}

	private unsafe byte* PartyFinderSortType = null;

	private static readonly CompSig PartyFinderSortCmpSig = new CompSig("40 53 48 83 EC 20 0F B6 82 ?? ?? ?? ?? 48 8B DA 38 81 ?? ?? ?? ??");

	private Hook<PartyFinderSortCmpDelegate>? PartyFinderSortCmpHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterPartyFinderSortTitle"),
		Description = LanguageManager.Get("BetterPartyFinderSortDescription"),
		Category = ModuleCategory.Recruitment,
		Author = new string[1] { "decorwdyun" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		PartyFinderSortType = new CompSig("75 53 0F B6 05 ?? ?? ?? ??").GetStatic<byte>();
		if (PartyFinderSortCmpHook == null)
		{
			PartyFinderSortCmpHook = PartyFinderSortCmpSig.GetHook<PartyFinderSortCmpDelegate>(PartyFinderSortCmpDetour);
		}
		PartyFinderSortCmpHook.Enable();
	}

	private byte PartyFinderSortCmpDetour(nint a1, nint a2)
	{
		try
		{
			PartyFinderListing a3 = Marshal.PtrToStructure<PartyFinderListing>(a1);
			PartyFinderListing a4 = Marshal.PtrToStructure<PartyFinderListing>(a2);
			if (a3.Unknown408 != a4.Unknown408)
			{
				return (a3.Unknown408 < a4.Unknown408) ? ((byte)1) : ((byte)0);
			}
			if (a3.Unknown409 != a4.Unknown409)
			{
				return (a3.Unknown409 < a4.Unknown409) ? ((byte)1) : ((byte)0);
			}
			if (a3.IsDutyUnlocked != a4.IsDutyUnlocked)
			{
				return (a3.IsDutyUnlocked < a4.IsDutyUnlocked) ? ((byte)1) : ((byte)0);
			}
			if (a3.IsBlacklisted != a4.IsBlacklisted)
			{
				return (a3.IsBlacklisted < a4.IsBlacklisted) ? ((byte)1) : ((byte)0);
			}
			return GetSortStrategy().Compare(a3, a4);
		}
		catch (Exception)
		{
			return PartyFinderSortCmpHook.Original(a1, a2);
		}
	}

	private unsafe ISortStrategy GetSortStrategy()
	{
		return *PartyFinderSortType switch
		{
			0 => new TimeLeftAscendingStrategy(), 
			1 => new TimeLeftDescendingStrategy(), 
			_ => new TimeLeftAscendingStrategy(), 
		};
	}
}
