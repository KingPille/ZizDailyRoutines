using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Enums;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.PlayerInfo.Models;

public readonly struct PlayerInfoKey : IEquatable<PlayerInfoKey>
{
	public ulong ContentID { get; init; }

	public WorldRegion Region { get; init; }

	public PlayerInfoKey(ulong ContentID, WorldRegion Region)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.ContentID = ContentID;
		this.Region = Region;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702ABAEF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF480D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
		if (LCaLrmsopF(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool LCaLrmsopF(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C78EE1C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00));
		builder.Append(ContentID.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DA3201 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
		builder.Append(Region.ToString());
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(PlayerInfoKey left, PlayerInfoKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(PlayerInfoKey left, PlayerInfoKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<ulong>.Default.GetHashCode(ContentID) * -1521134295 + EqualityComparer<WorldRegion>.Default.GetHashCode(Region);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is PlayerInfoKey)
		{
			return Equals((PlayerInfoKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(PlayerInfoKey other)
	{
		if (EqualityComparer<ulong>.Default.Equals(ContentID, other.ContentID))
		{
			return EqualityComparer<WorldRegion>.Default.Equals(Region, other.Region);
		}
		return false;
	}

	[CompilerGenerated]
	public void Deconstruct(out ulong ContentID, out WorldRegion Region)
	{
		ContentID = this.ContentID;
		Region = this.Region;
	}
}
