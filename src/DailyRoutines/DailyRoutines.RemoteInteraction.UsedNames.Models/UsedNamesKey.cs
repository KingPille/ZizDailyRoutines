using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Enums;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.UsedNames.Models;

public readonly struct UsedNamesKey : IEquatable<UsedNamesKey>
{
	public ulong ContentID { get; init; }

	public WorldRegion Region { get; init; }

	public UsedNamesKey(ulong ContentID, WorldRegion Region)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.ContentID = ContentID;
		this.Region = Region;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F341644 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719CACC3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc));
		if (w2tyYWiONH(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool w2tyYWiONH(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829E4AF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
		builder.Append(ContentID.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF482B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
		builder.Append(Region.ToString());
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UsedNamesKey left, UsedNamesKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UsedNamesKey left, UsedNamesKey right)
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
		if (obj is UsedNamesKey)
		{
			return Equals((UsedNamesKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(UsedNamesKey other)
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
