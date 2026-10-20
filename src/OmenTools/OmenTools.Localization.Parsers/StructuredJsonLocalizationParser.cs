using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmenTools.Localization.Abstractions;

namespace OmenTools.Localization.Parsers;

public sealed class StructuredJsonLocalizationParser(string keyDelimiter = ".") : ILocalizationParser
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
				VisitObject(val2, null, dictionary);
			}
			return dictionary.ToFrozenResource();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void VisitObject(JObject obj, string? prefix, Dictionary<string, string> result)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		foreach (JProperty item in obj.Properties())
		{
			string text = (string.IsNullOrEmpty(prefix) ? item.Name : (prefix + keyDelimiter + item.Name));
			JToken value = item.Value;
			JObject val = (JObject)(object)((value is JObject) ? value : null);
			string value2;
			if (val == null)
			{
				JValue val2 = (JValue)(object)((value is JValue) ? value : null);
				if (val2 != null && (int)((JToken)val2).Type == 8)
				{
					JValue val3 = val2;
					result[text] = Extensions.Value<string>((IEnumerable<JToken>)val3) ?? string.Empty;
				}
			}
			else if (TryGetStringValue(val, out value2))
			{
				result[text] = value2;
			}
			else
			{
				JObject obj2 = val;
				VisitObject(obj2, text, result);
			}
		}
	}

	private static bool TryGetStringValue(JObject obj, out string value)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		value = string.Empty;
		JToken val = default(JToken);
		if (obj.TryGetValue("string", ref val) && (int)val.Type == 8)
		{
			value = Extensions.Value<string>((IEnumerable<JToken>)val) ?? string.Empty;
			return true;
		}
		return false;
	}
}
