using System.Collections.Frozen;
using System.IO;

namespace OmenTools.Localization.Abstractions;

public interface ILocalizationParser
{
	FrozenDictionary<string, string> Parse(Stream stream);
}
