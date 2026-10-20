using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models;

public readonly struct UniversalisMarketDataKey : IEquatable<UniversalisMarketDataKey>
{
	public string ItemIDsCSV { get; init; }

	public string WorldOrDC { get; init; }

	public UniversalisMarketDataRequestParams? Parameters { get; init; }

	public UniversalisMarketDataKey(string ItemIDsCSV, string WorldOrDC, UniversalisMarketDataRequestParams? Parameters)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.ItemIDsCSV = ItemIDsCSV;
		this.WorldOrDC = WorldOrDC;
		this.Parameters = Parameters;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB32A5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4732DFDF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5));
		if (q8HLLbguyb(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool q8HLLbguyb(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB3377 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
		builder.Append((object?)ItemIDsCSV);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4628E5E1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7));
		builder.Append((object?)WorldOrDC);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32BC615 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1));
		builder.Append(Parameters);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UniversalisMarketDataKey left, UniversalisMarketDataKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UniversalisMarketDataKey left, UniversalisMarketDataKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return (EqualityComparer<string>.Default.GetHashCode(ItemIDsCSV) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WorldOrDC)) * -1521134295 + EqualityComparer<UniversalisMarketDataRequestParams>.Default.GetHashCode(Parameters);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is UniversalisMarketDataKey)
		{
			return Equals((UniversalisMarketDataKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(UniversalisMarketDataKey other)
	{
		if (EqualityComparer<string>.Default.Equals(ItemIDsCSV, other.ItemIDsCSV) && EqualityComparer<string>.Default.Equals(WorldOrDC, other.WorldOrDC))
		{
			return EqualityComparer<UniversalisMarketDataRequestParams>.Default.Equals(Parameters, other.Parameters);
		}
		return false;
	}

	[CompilerGenerated]
	public void Deconstruct(out string ItemIDsCSV, out string WorldOrDC, out UniversalisMarketDataRequestParams? Parameters)
	{
		ItemIDsCSV = this.ItemIDsCSV;
		WorldOrDC = this.WorldOrDC;
		Parameters = this.Parameters;
	}
}
