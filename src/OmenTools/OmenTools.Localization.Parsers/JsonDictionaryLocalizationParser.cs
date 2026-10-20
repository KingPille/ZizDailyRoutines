using System;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using OmenTools.Localization.Abstractions;

namespace OmenTools.Localization.Parsers;

public sealed class JsonDictionaryLocalizationParser : ILocalizationParser
{
	private static readonly FrozenDictionary<string, string> EmptyResource = new Dictionary<string, string>(StringComparer.Ordinal).ToFrozenDictionary(StringComparer.Ordinal);

	public FrozenDictionary<string, string> Parse(Stream stream)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		ArgumentNullException.ThrowIfNull(stream, "stream");
		using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, -1, leaveOpen: true);
		JsonTextReader val = new JsonTextReader((TextReader)streamReader);
		try
		{
			Dictionary<string, string> dictionary = JsonSerializer.CreateDefault().Deserialize<Dictionary<string, string>>((JsonReader)(object)val) ?? new Dictionary<string, string>();
			return (FrozenDictionary<string, string>)((dictionary.Count == 0) ? ((IDictionary)EmptyResource) : ((IDictionary)dictionary.ToFrozenDictionary(StringComparer.Ordinal)));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
