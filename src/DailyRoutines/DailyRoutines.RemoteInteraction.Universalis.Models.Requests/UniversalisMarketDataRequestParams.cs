using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Requests;

public sealed record UniversalisMarketDataRequestParams : UniversalisRequestParams
{
	public int? Listings { get; init; }

	public int? Entries { get; init; }

	public bool? HQ { get; init; }

	public long? StatsWithin { get; init; }

	public long? EntriesWithin { get; init; }

	public string Fields { get; init; }

	protected override void AddQueryParameters(List<KeyValuePair<string, string>> queryParameters)
	{
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D14E5CF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275), Listings);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C328D3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0), Entries);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C78E27A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00), HQ);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDBA1F1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1), StatsWithin);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB3FB1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503), EntriesWithin);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F70F405 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0), Fields);
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5132B0B4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829E495 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
		if (PrintMembers(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	protected override bool PrintMembers(StringBuilder builder)
	{
		if (base.PrintMembers(builder))
		{
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F70FB79 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0));
		}
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32BCB6F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1));
		builder.Append(Listings.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CCECD1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
		builder.Append(Entries.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x65412206 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb));
		builder.Append(HQ.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC040C6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
		builder.Append(StatsWithin.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E47B4B9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
		builder.Append(EntriesWithin.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DA3F6B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
		builder.Append((object?)Fields);
		return true;
	}

	[CompilerGenerated]
	private UniversalisMarketDataRequestParams(UniversalisMarketDataRequestParams original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector(original);
		Listings = original.Listings;
		Entries = original.Entries;
		HQ = original.HQ;
		StatsWithin = original.StatsWithin;
		EntriesWithin = original.EntriesWithin;
		Fields = original.Fields;
	}

	public UniversalisMarketDataRequestParams()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Fields = string.Empty;
		base._002Ector();
	}
}
