using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmenTools.Localization.Abstractions;

namespace OmenTools.Localization.Parsers;

public sealed class KeyValueJsonLocalizationParser(string keyDelimiter = ".") : ILocalizationParser
{
	private readonly string keyDelimiter = keyDelimiter;

	public FrozenDictionary<string, string> Parse(Stream stream)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		ArgumentNullException.ThrowIfNull(stream, "stream");
		using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, -1, leaveOpen: true);
		JsonTextReader val = new JsonTextReader((TextReader)streamReader);
		try
		{
			JToken obj = JToken.ReadFrom((JsonReader)(object)val);
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
			JObject val2 = (JObject)(object)((obj is JObject) ? obj : null);
			if (val2 != null)
			{
				VisitToken((JToken)(object)val2, null, dictionary);
			}
			return dictionary.ToFrozenResource();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void VisitToken(JToken token, string? prefix, Dictionary<string, string> result)
	{
		JObject val = (JObject)(object)((token is JObject) ? token : null);
		if (val == null)
		{
			JArray val2 = (JArray)(object)((token is JArray) ? token : null);
			if (val2 == null)
			{
				JValue val3 = (JValue)(object)((token is JValue) ? token : null);
				if (val3 != null && prefix != null)
				{
					result[prefix] = ConvertToString(val3);
				}
				return;
			}
			for (int i = 0; i < ((JContainer)val2).Count; i++)
			{
				VisitToken(val2[i], $"{prefix}[{i}]", result);
			}
			return;
		}
		foreach (JProperty item in val.Properties())
		{
			string prefix2 = (string.IsNullOrEmpty(prefix) ? item.Name : (prefix + keyDelimiter + item.Name));
			VisitToken(item.Value, prefix2, result);
		}
	}

	private static string ConvertToString(JValue value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected I4, but got Unknown
		JTokenType type = ((JToken)value).Type;
		return (type - 8) switch
		{
			2 => string.Empty, 
			0 => Extensions.Value<string>((IEnumerable<JToken>)value) ?? string.Empty, 
			1 => Extensions.Value<bool>((IEnumerable<JToken>)value) ? "true" : "false", 
			_ => Convert.ToString(value.Value, CultureInfo.InvariantCulture) ?? string.Empty, 
		};
	}
}
