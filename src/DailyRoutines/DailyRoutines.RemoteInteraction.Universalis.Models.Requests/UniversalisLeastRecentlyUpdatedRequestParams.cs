using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Requests;

public sealed record UniversalisLeastRecentlyUpdatedRequestParams : UniversalisRequestParams
{
	public string World { get; init; }

	public string DCName { get; init; }

	public int? Entries { get; init; }

	protected override void AddQueryParameters(List<KeyValuePair<string, string>> queryParameters)
	{
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F70FBED ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0), World);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E47BAEF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e), DCName);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E95978F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75), Entries);
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768E706D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D3A466 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
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
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C78EDC6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00));
		}
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829E88D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
		builder.Append((object?)World);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1675397288 ^ -1147648097 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
		builder.Append((object?)DCName);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB3FC3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
		builder.Append(Entries.ToString());
		return true;
	}

	[CompilerGenerated]
	private UniversalisLeastRecentlyUpdatedRequestParams(UniversalisLeastRecentlyUpdatedRequestParams original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector(original);
		World = original.World;
		DCName = original.DCName;
		Entries = original.Entries;
	}

	public UniversalisLeastRecentlyUpdatedRequestParams()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		World = string.Empty;
		DCName = string.Empty;
		base._002Ector();
	}
}
