using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models;

public readonly struct UniversalisContentKey : IEquatable<UniversalisContentKey>
{
	public string ContentID { get; init; }

	public UniversalisContentKey(string ContentID)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.ContentID = ContentID;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E24FC1F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339D281C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de));
		if (BjHLnuCvWv(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool BjHLnuCvWv(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768E73D3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
		builder.Append((object?)ContentID);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UniversalisContentKey left, UniversalisContentKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UniversalisContentKey left, UniversalisContentKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<string>.Default.GetHashCode(ContentID);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is UniversalisContentKey)
		{
			return Equals((UniversalisContentKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(UniversalisContentKey other)
	{
		return EqualityComparer<string>.Default.Equals(ContentID, other.ContentID);
	}

	[CompilerGenerated]
	public void Deconstruct(out string ContentID)
	{
		ContentID = this.ContentID;
	}
}
