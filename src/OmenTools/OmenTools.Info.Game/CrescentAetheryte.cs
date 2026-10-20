using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Info.Game;

public class CrescentAetheryte(uint dataID, Vector3 position, CrescentAetherytePoint point) : IEquatable<CrescentAetheryte>
{
	public static CrescentAetheryte ExpeditionBaseCamp { get; } = new CrescentAetheryte(4927u, new Vector3(834f, 73f, -694.6f), CrescentAetherytePoint.ExpeditionBaseCamp);

	public static CrescentAetheryte WanderersHaven { get; } = new CrescentAetheryte(4928u, new Vector3(-169f, 6.5f, -609.1f), CrescentAetherytePoint.WanderersHaven);

	public static CrescentAetheryte CrystallizedCaverns { get; } = new CrescentAetheryte(4929u, new Vector3(-354.4f, 100f, -120.2f), CrescentAetherytePoint.CrystallizedCaverns);

	public static CrescentAetheryte Eldergrowth { get; } = new CrescentAetheryte(4930u, new Vector3(302.9f, 103f, 305.4f), CrescentAetherytePoint.Eldergrowth);

	public static CrescentAetheryte Stonemarsh { get; } = new CrescentAetheryte(4947u, new Vector3(-384.9f, 97.4f, 277.1f), CrescentAetherytePoint.Stonemarsh);

	public static CrescentAetheryte NorthHornBaseCamp { get; } = new CrescentAetheryte(5571u, new Vector3(881.1f, 258.5f, 882.2f), CrescentAetherytePoint.NorthHornBaseCamp);

	public static CrescentAetheryte CrownOfKarnak { get; } = new CrescentAetheryte(5576u, new Vector3(454f, 70f, 530.6f), CrescentAetherytePoint.CrownOfKarnak);

	public static CrescentAetheryte SinkingSanctuary { get; } = new CrescentAetheryte(5572u, new Vector3(358.2f, 45.1f, -557.3f), CrescentAetherytePoint.SinkingSanctuary);

	public static CrescentAetheryte SuspendedMasonry { get; } = new CrescentAetheryte(5573u, new Vector3(-549.2f, 67.2f, 597.2f), CrescentAetherytePoint.SuspendedMasonry);

	public static CrescentAetheryte MolderingOutskirts { get; } = new CrescentAetheryte(5574u, new Vector3(-386.6f, 39.2f, -437.6f), CrescentAetherytePoint.MolderingOutskirts);

	public static CrescentAetheryte UnhallowedHamlet { get; } = new CrescentAetheryte(5575u, new Vector3(-15.7f, 2.1f, -44.5f), CrescentAetherytePoint.UnhallowedHamlet);

	public static List<CrescentAetheryte> SouthHornAetherytes { get; }

	public static List<CrescentAetheryte> NorthHornAetherytes { get; }

	public string Name { get; } = LuminaWrapper.GetPlaceName(dataID);

	public uint DataID { get; } = dataID;

	public Vector3 Position { get; } = position;

	public CrescentAetherytePoint Point { get; } = point;

	public byte Index { get; } = (byte)(((int)point > 4) ? (point - 4) : point);

	public static bool TryGetNearestSouthHorn(Vector3 destination, [NotNullWhen(true)] out CrescentAetheryte? result)
	{
		result = null;
		float num = float.MaxValue;
		foreach (CrescentAetheryte southHornAetheryte in SouthHornAetherytes)
		{
			if (IsNeedToUseAetheryte(destination, southHornAetheryte.Position, out var aetheryteTime) && aetheryteTime < num)
			{
				result = southHornAetheryte;
				num = aetheryteTime;
			}
		}
		return num != float.MaxValue;
	}

	private static bool IsNeedToUseAetheryte(Vector3 finalPos, Vector3 aetherytePos, out float aetheryteTime)
	{
		aetheryteTime = float.MaxValue;
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return false;
		}
		if (Vector3.DistanceSquared(localPlayer.Position, finalPos) <= 10000f)
		{
			return false;
		}
		float num = Vector3.Distance(finalPos, localPlayer.Position) / 12f;
		aetheryteTime = Vector3.Distance(finalPos, aetherytePos) / 12f + 10f;
		return num > aetheryteTime;
	}

	public unsafe bool TeleportTo()
	{
		AgentTelepotTown* ptr = AgentTelepotTown.Instance();
		if (ptr == null || !((AgentTelepotTown)ptr).IsAgentActive())
		{
			return false;
		}
		((AgentTelepotTown)ptr).TeleportToAetheryte(Index);
		return true;
	}

	public bool Equals(CrescentAetheryte? other)
	{
		return DataID == other?.DataID;
	}

	public override bool Equals(object? obj)
	{
		if (obj is CrescentAetheryte other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (int)DataID;
	}

	public static bool operator ==(CrescentAetheryte left, CrescentAetheryte right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CrescentAetheryte left, CrescentAetheryte right)
	{
		return !left.Equals(right);
	}

	static CrescentAetheryte()
	{
		int num = 5;
		List<CrescentAetheryte> list = new List<CrescentAetheryte>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<CrescentAetheryte> span = CollectionsMarshal.AsSpan(list);
		span[0] = ExpeditionBaseCamp;
		span[1] = WanderersHaven;
		span[2] = CrystallizedCaverns;
		span[3] = Eldergrowth;
		span[4] = Stonemarsh;
		SouthHornAetherytes = list;
		num = 6;
		List<CrescentAetheryte> list2 = new List<CrescentAetheryte>(num);
		CollectionsMarshal.SetCount(list2, num);
		Span<CrescentAetheryte> span2 = CollectionsMarshal.AsSpan(list2);
		span2[0] = NorthHornBaseCamp;
		span2[1] = CrownOfKarnak;
		span2[2] = SinkingSanctuary;
		span2[3] = SuspendedMasonry;
		span2[4] = MolderingOutskirts;
		span2[5] = UnhallowedHamlet;
		NorthHornAetherytes = list2;
	}
}
