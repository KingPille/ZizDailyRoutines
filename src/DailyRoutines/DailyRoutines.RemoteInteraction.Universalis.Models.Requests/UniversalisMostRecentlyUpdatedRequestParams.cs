using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Requests;

public sealed record UniversalisMostRecentlyUpdatedRequestParams : UniversalisRequestParams
{
	public string World { get; init; }

	public string DCName { get; init; }

	public int? Entries { get; init; }

	protected override void AddQueryParameters(List<KeyValuePair<string, string>> queryParameters)
	{
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829E7E1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184), World);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4628E625 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7), DCName);
		UniversalisRequestParams.Add(queryParameters, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF1F0B0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d), Entries);
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9073E2E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329DE82C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
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
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F341240 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
		}
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DA3E3F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
		builder.Append((object?)World);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC04EDE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
		builder.Append((object?)DCName);
		builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329DE420 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
		builder.Append(Entries.ToString());
		return true;
	}

	[CompilerGenerated]
	private UniversalisMostRecentlyUpdatedRequestParams(UniversalisMostRecentlyUpdatedRequestParams original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector(original);
		World = original.World;
		DCName = original.DCName;
		Entries = original.Entries;
	}

	public UniversalisMostRecentlyUpdatedRequestParams()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		World = string.Empty;
		DCName = string.Empty;
		base._002Ector();
	}
}
