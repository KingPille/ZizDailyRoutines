using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models;

public readonly struct UniversalisUserListKey : IEquatable<UniversalisUserListKey>
{
	public string ListID { get; init; }

	public UniversalisUserListKey(string ListID)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.ListID = ListID;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFD39B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A85525 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a));
		if (BHxL9nxm0Q(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool BHxL9nxm0Q(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E39FA03 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997));
		builder.Append((object?)ListID);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UniversalisUserListKey left, UniversalisUserListKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UniversalisUserListKey left, UniversalisUserListKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<string>.Default.GetHashCode(ListID);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is UniversalisUserListKey)
		{
			return Equals((UniversalisUserListKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(UniversalisUserListKey other)
	{
		return EqualityComparer<string>.Default.Equals(ListID, other.ListID);
	}

	[CompilerGenerated]
	public void Deconstruct(out string ListID)
	{
		ListID = this.ListID;
	}
}
