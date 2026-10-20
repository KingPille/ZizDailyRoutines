using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models;

public readonly struct UniversalisAggregatedMarketDataKey : IEquatable<UniversalisAggregatedMarketDataKey>
{
	public string ItemIDsCSV { get; init; }

	public string WorldOrDC { get; init; }

	public UniversalisAggregatedMarketDataKey(string ItemIDsCSV, string WorldOrDC)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		this.ItemIDsCSV = ItemIDsCSV;
		this.WorldOrDC = WorldOrDC;
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -78978599 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5132BC38 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
		if (soMyzr3qKY(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	private bool soMyzr3qKY(StringBuilder builder)
	{
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CCE065 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
		builder.Append((object?)ItemIDsCSV);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F341106 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
		builder.Append((object?)WorldOrDC);
		return true;
	}

	[CompilerGenerated]
	public static bool operator !=(UniversalisAggregatedMarketDataKey left, UniversalisAggregatedMarketDataKey right)
	{
		return !(left == right);
	}

	[CompilerGenerated]
	public static bool operator ==(UniversalisAggregatedMarketDataKey left, UniversalisAggregatedMarketDataKey right)
	{
		return left.Equals(right);
	}

	[CompilerGenerated]
	public override int GetHashCode()
	{
		return EqualityComparer<string>.Default.GetHashCode(ItemIDsCSV) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WorldOrDC);
	}

	[CompilerGenerated]
	public override bool Equals(object obj)
	{
		if (obj is UniversalisAggregatedMarketDataKey)
		{
			return Equals((UniversalisAggregatedMarketDataKey)obj);
		}
		return false;
	}

	[CompilerGenerated]
	public bool Equals(UniversalisAggregatedMarketDataKey other)
	{
		if (EqualityComparer<string>.Default.Equals(ItemIDsCSV, other.ItemIDsCSV))
		{
			return EqualityComparer<string>.Default.Equals(WorldOrDC, other.WorldOrDC);
		}
		return false;
	}

	[CompilerGenerated]
	public void Deconstruct(out string ItemIDsCSV, out string WorldOrDC)
	{
		ItemIDsCSV = this.ItemIDsCSV;
		WorldOrDC = this.WorldOrDC;
	}
}
