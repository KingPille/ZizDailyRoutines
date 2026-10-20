using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Requests;

public sealed record UniversalisTaxRatesRequestParams : UniversalisRequestParams
{
	public string World { get; init; }

	protected override void AddQueryParameters(List<KeyValuePair<string, string>> queryParameters)
	{
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702AB019 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30), World);
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B208A1C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1976305562 ^ 0x52773455 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
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
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C328A1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0));
		}
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D358812 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33));
		builder.Append((object?)World);
		return true;
	}

	[CompilerGenerated]
	private UniversalisTaxRatesRequestParams(UniversalisTaxRatesRequestParams original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector(original);
		World = original.World;
	}

	public UniversalisTaxRatesRequestParams()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		World = string.Empty;
		base._002Ector();
	}
}
