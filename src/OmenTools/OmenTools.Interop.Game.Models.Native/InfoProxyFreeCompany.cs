using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.UI.Info;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit)]
public struct InfoProxyFreeCompany
{
	private unsafe delegate byte* GetRankNameDelegate(InfoProxyFreeCompany* proxy, RankData* rank);

	[StructLayout(LayoutKind.Explicit, Size = 88)]
	public struct RankData
	{
		[FieldOffset(32)]
		public ushort MemberCount;

		[FieldOffset(34)]
		public byte RankNumber;

		[FieldOffset(35)]
		public unsafe fixed byte Name[16];
	}

	private static readonly CompSig GetRankNameSig = new CompSig("48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 48 85 D2 75");

	private static readonly GetRankNameDelegate GetRankName = GetRankNameSig.GetDelegate<GetRankNameDelegate>();

	[FieldOffset(122)]
	public ushort TotalMembers;

	[FieldOffset(320)]
	public RankData Ranks;

	private const int RANK_COUNT = 16;

	public unsafe RankData* GetRankData(byte rankIndex)
	{
		if (rankIndex >= 16)
		{
			return null;
		}
		fixed (InfoProxyFreeCompany* ptr = &this)
		{
			return &ptr->Ranks + (int)rankIndex;
		}
	}

	public unsafe bool IsRankAssignable(byte rankIndex)
	{
		if (rankIndex >= 15)
		{
			return false;
		}
		RankData* rankData = GetRankData(rankIndex);
		if (rankData != null)
		{
			return *rankData->Name != 0;
		}
		return false;
	}

	public unsafe string GetRankNameText(byte rankIndex)
	{
		RankData* rankData = GetRankData(rankIndex);
		if (rankData == null)
		{
			return string.Empty;
		}
		fixed (InfoProxyFreeCompany* proxy = &this)
		{
			return MemoryHelper.ReadStringNullTerminated((IntPtr)(nint)GetRankName(proxy, rankData));
		}
	}

	public string GetMemberRankNameText(uint memberExtraFlags)
	{
		return GetRankNameText((byte)(memberExtraFlags >> 12));
	}

	public List<(byte RankIndex, string Name)> GetAssignableRanks()
	{
		List<(byte, string)> list = new List<(byte, string)>();
		for (byte b = 0; b < 16; b++)
		{
			if (IsRankAssignable(b))
			{
				string rankNameText = GetRankNameText(b);
				if (!string.IsNullOrWhiteSpace(rankNameText))
				{
					list.Add((b, rankNameText));
				}
			}
		}
		return list;
	}

	public unsafe static InfoProxyFreeCompany* Instance()
	{
		return (InfoProxyFreeCompany*)((InfoModule)InfoModule.Instance()).GetInfoProxyById((InfoProxyId)13);
	}
}
