using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models;

public readonly struct UniversalisMostRecentlyUpdatedKey : IEquatable<UniversalisMostRecentlyUpdatedKey>
{
	public UniversalisMostRecentlyUpdatedRequestParams? Parameters { get; init; }

	public UniversalisMostRecentlyUpdatedKey(UniversalisMostRecentlyUpdatedRequestParams? Parameters)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Parameters = Parameters;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DA303D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB33CF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
		if (v8fLSD6hpv(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool v8fLSD6hpv(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1675397288 ^ -1526676266 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
		builder.Append(Parameters);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UniversalisMostRecentlyUpdatedKey left, UniversalisMostRecentlyUpdatedKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UniversalisMostRecentlyUpdatedKey left, UniversalisMostRecentlyUpdatedKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<UniversalisMostRecentlyUpdatedRequestParams>.Default.GetHashCode(Parameters);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is UniversalisMostRecentlyUpdatedKey)
		{
			return Equals((UniversalisMostRecentlyUpdatedKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(UniversalisMostRecentlyUpdatedKey other)
	{
		return EqualityComparer<UniversalisMostRecentlyUpdatedRequestParams>.Default.Equals(Parameters, other.Parameters);
	}

	[CompilerGenerated]
	public void Deconstruct(out UniversalisMostRecentlyUpdatedRequestParams? Parameters)
	{
		Parameters = this.Parameters;
	}
}
