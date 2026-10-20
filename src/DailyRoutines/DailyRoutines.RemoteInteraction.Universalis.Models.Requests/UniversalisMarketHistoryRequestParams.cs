using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Requests;

public sealed record UniversalisMarketHistoryRequestParams : UniversalisRequestParams
{
	public int? EntriesToReturn { get; init; }

	public long? StatsWithin { get; init; }

	public long? EntriesWithin { get; init; }

	public long? EntriesUntil { get; init; }

	public int? MinSalePrice { get; init; }

	public int? MaxSalePrice { get; init; }

	protected override void AddQueryParameters(List<KeyValuePair<string, string>> queryParameters)
	{
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200E0ADB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af), EntriesToReturn);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768E7FBD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46), StatsWithin);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF4473 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b), EntriesWithin);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDDF23D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1), EntriesUntil);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5369C644 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94), MinSalePrice);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDDF265 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1), MaxSalePrice);
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642B1882 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3A576 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
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
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28170986 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd));
		}
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C78E00C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00));
		builder.Append(EntriesToReturn.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E47B49D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
		builder.Append(StatsWithin.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200E0A99 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));
		builder.Append(EntriesWithin.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x558202A8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d));
		builder.Append(EntriesUntil.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2FDDF12D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c6c2389b72fc4ab3828ae71fae281bf1));
		builder.Append(MinSalePrice.ToString());
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B31EE4A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
		builder.Append(MaxSalePrice.ToString());
		return true;
	}

	[CompilerGenerated]
	private UniversalisMarketHistoryRequestParams(UniversalisMarketHistoryRequestParams original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector(original);
		EntriesToReturn = original.EntriesToReturn;
		StatsWithin = original.StatsWithin;
		EntriesWithin = original.EntriesWithin;
		EntriesUntil = original.EntriesUntil;
		MinSalePrice = original.MinSalePrice;
		MaxSalePrice = original.MaxSalePrice;
	}

	public UniversalisMarketHistoryRequestParams()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
