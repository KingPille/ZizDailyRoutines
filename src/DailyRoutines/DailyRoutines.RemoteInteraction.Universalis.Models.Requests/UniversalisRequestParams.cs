using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using B2nED1Wmdu0xsDR7Tjv;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.RemoteInteraction.Universalis.Models.Requests;

public abstract record UniversalisRequestParams
{
	public List<KeyValuePair<string, string>> GetQueryParameters()
	{
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		AddQueryParameters(list);
		return list;
	}

	protected abstract void AddQueryParameters(List<KeyValuePair<string, string>> queryParameters);

	protected static void Add(List<KeyValuePair<string, string>> queryParameters, string name, string? value)
	{
		if (!string.IsNullOrWhiteSpace(value))
		{
			queryParameters.Add(new KeyValuePair<string, string>(name, value));
		}
	}

	protected static void Add(List<KeyValuePair<string, string>> queryParameters, string name, int? value)
	{
		if (value.HasValue)
		{
			queryParameters.Add(new KeyValuePair<string, string>(name, value.Value.ToString(CultureInfo.InvariantCulture)));
		}
	}

	protected static void Add(List<KeyValuePair<string, string>> queryParameters, string name, long? value)
	{
		if (value.HasValue)
		{
			queryParameters.Add(new KeyValuePair<string, string>(name, value.Value.ToString(CultureInfo.InvariantCulture)));
		}
	}

	protected static void Add(List<KeyValuePair<string, string>> queryParameters, string name, bool? value)
	{
		if (value.HasValue)
		{
			queryParameters.Add(new KeyValuePair<string, string>(name, value.Value.ToString().ToLowerInvariant()));
		}
	}

	[CompilerGenerated]
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA62B71 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
		stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1003249140 ^ -1998057559 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1));
		if (PrintMembers(stringBuilder))
		{
			stringBuilder.Append(' ');
		}
		stringBuilder.Append('}');
		return stringBuilder.ToString();
	}

	[CompilerGenerated]
	protected UniversalisRequestParams(UniversalisRequestParams original)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	protected UniversalisRequestParams()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
