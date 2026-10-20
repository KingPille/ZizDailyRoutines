using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using OmenTools.Localization.Abstractions;

namespace OmenTools.Localization.Parsers;

public sealed class JavaXmlPropertiesLocalizationParser : ILocalizationParser
{
	public FrozenDictionary<string, string> Parse(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		using XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings
		{
			CloseInput = false,
			DtdProcessing = DtdProcessing.Ignore,
			IgnoreComments = true,
			IgnoreWhitespace = true,
			XmlResolver = null
		});
		XDocument xDocument = XDocument.Load(reader);
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (XElement item in xDocument.Root?.Elements("entry") ?? Array.Empty<XElement>())
		{
			string text = item.Attribute("key")?.Value;
			if (!string.IsNullOrWhiteSpace(text))
			{
				dictionary[text] = item.Value;
			}
		}
		return dictionary.ToFrozenResource();
	}
}
