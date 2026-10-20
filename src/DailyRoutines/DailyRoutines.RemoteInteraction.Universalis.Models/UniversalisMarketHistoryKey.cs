using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models;

public readonly struct UniversalisMarketHistoryKey : IEquatable<UniversalisMarketHistoryKey>
{
	public string ItemIDsCSV { get; init; }

	public string WorldOrDC { get; init; }

	public UniversalisMarketHistoryRequestParams? Parameters { get; init; }

	public UniversalisMarketHistoryKey(string ItemIDsCSV, string WorldOrDC, UniversalisMarketHistoryRequestParams? Parameters)
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
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702AB2BD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B20855E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a));
		if (fyjLHF4sVT(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool fyjLHF4sVT(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719CAC7B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc));
		builder.Append((object?)ItemIDsCSV);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A85583 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a));
		builder.Append((object?)WorldOrDC);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3923B37C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
		builder.Append(Parameters);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UniversalisMarketHistoryKey left, UniversalisMarketHistoryKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UniversalisMarketHistoryKey left, UniversalisMarketHistoryKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return (EqualityComparer<string>.Default.GetHashCode(ItemIDsCSV) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WorldOrDC)) * -1521134295 + EqualityComparer<UniversalisMarketHistoryRequestParams>.Default.GetHashCode(Parameters);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is UniversalisMarketHistoryKey)
		{
			return Equals((UniversalisMarketHistoryKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(UniversalisMarketHistoryKey other)
	{
		if (EqualityComparer<string>.Default.Equals(ItemIDsCSV, other.ItemIDsCSV) && EqualityComparer<string>.Default.Equals(WorldOrDC, other.WorldOrDC))
		{
			return EqualityComparer<UniversalisMarketHistoryRequestParams>.Default.Equals(Parameters, other.Parameters);
		}
		return false;
	}

	[CompilerGenerated]
	public void Deconstruct(out string ItemIDsCSV, out string WorldOrDC, out UniversalisMarketHistoryRequestParams? Parameters)
	{
		ItemIDsCSV = this.ItemIDsCSV;
		WorldOrDC = this.WorldOrDC;
		Parameters = this.Parameters;
	}
}
