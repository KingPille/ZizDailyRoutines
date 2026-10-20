using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models;

public readonly struct UniversalisTaxRatesKey : IEquatable<UniversalisTaxRatesKey>
{
	public UniversalisTaxRatesRequestParams Parameters { get; init; }

	public UniversalisTaxRatesKey(UniversalisTaxRatesRequestParams Parameters)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.Parameters = Parameters;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E95964D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1003249140 ^ -1524339996 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402));
		if (uPLLVISqWf(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool uPLLVISqWf(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D3A52A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
		builder.Append(Parameters);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UniversalisTaxRatesKey left, UniversalisTaxRatesKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UniversalisTaxRatesKey left, UniversalisTaxRatesKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<UniversalisTaxRatesRequestParams>.Default.GetHashCode(Parameters);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is UniversalisTaxRatesKey)
		{
			return Equals((UniversalisTaxRatesKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(UniversalisTaxRatesKey other)
	{
		return EqualityComparer<UniversalisTaxRatesRequestParams>.Default.Equals(Parameters, other.Parameters);
	}

	[CompilerGenerated]
	public void Deconstruct(out UniversalisTaxRatesRequestParams Parameters)
	{
		Parameters = this.Parameters;
	}
}
