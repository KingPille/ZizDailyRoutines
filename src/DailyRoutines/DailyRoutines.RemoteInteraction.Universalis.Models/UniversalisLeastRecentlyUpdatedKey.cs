using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models;

public readonly struct UniversalisLeastRecentlyUpdatedKey : IEquatable<UniversalisLeastRecentlyUpdatedKey>
{
	public UniversalisLeastRecentlyUpdatedRequestParams? Parameters { get; init; }

	public UniversalisLeastRecentlyUpdatedKey(UniversalisLeastRecentlyUpdatedRequestParams? Parameters)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Parameters = Parameters;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F70F98D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F3411A0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
		if (SoULyRwIMy(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool SoULyRwIMy(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF1F26E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d));
		builder.Append(Parameters);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UniversalisLeastRecentlyUpdatedKey left, UniversalisLeastRecentlyUpdatedKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UniversalisLeastRecentlyUpdatedKey left, UniversalisLeastRecentlyUpdatedKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<UniversalisLeastRecentlyUpdatedRequestParams>.Default.GetHashCode(Parameters);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is UniversalisLeastRecentlyUpdatedKey)
		{
			return Equals((UniversalisLeastRecentlyUpdatedKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(UniversalisLeastRecentlyUpdatedKey other)
	{
		return EqualityComparer<UniversalisLeastRecentlyUpdatedRequestParams>.Default.Equals(Parameters, other.Parameters);
	}

	[CompilerGenerated]
	public void Deconstruct(out UniversalisLeastRecentlyUpdatedRequestParams? Parameters)
	{
		Parameters = this.Parameters;
	}
}
