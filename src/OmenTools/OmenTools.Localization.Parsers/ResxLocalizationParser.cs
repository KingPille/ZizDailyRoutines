using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using OmenTools.Localization.Abstractions;

namespace OmenTools.Localization.Parsers;

public sealed class ResxLocalizationParser : ILocalizationParser
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
		foreach (XElement item in xDocument.Root?.Elements("data") ?? Array.Empty<XElement>())
		{
			string text = item.Attribute("name")?.Value;
			if (!string.IsNullOrWhiteSpace(text))
			{
				XElement xElement = item.Element("value");
				if (xElement != null)
				{
					dictionary[text] = xElement.Value;
				}
			}
		}
		return dictionary.ToFrozenResource();
	}
}
