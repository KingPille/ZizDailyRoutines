using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace OmenTools.Localization.Parsers;

internal static class LocalizationParserHelper
{
	public static FrozenDictionary<string, string> EmptyResource { get; } = new Dictionary<string, string>(StringComparer.Ordinal).ToFrozenDictionary(StringComparer.Ordinal);

	public static FrozenDictionary<string, string> ToFrozenResource(this Dictionary<string, string> dict)
	{
		if (dict.Count != 0)
		{
			return dict.ToFrozenDictionary(StringComparer.Ordinal);
		}
		return EmptyResource;
	}
}
