using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DailyRoutines.LocalData;

public static class BundledData
{
	private static readonly Lazy<string> Hunting = new Lazy<string>(() => Read("DailyRoutines.LocalData.HuntingLog.json"));

	private static readonly Lazy<string> Marks = new Lazy<string>(() => Read("DailyRoutines.LocalData.MarksFinder.json"));

	public static Task<string> ReadHuntingAsync(HttpClient? unusedClient, string unusedUrl)
	{
		return Task.FromResult(Hunting.Value);
	}

	public static Task<string> ReadMarksAsync(HttpClient? unusedClient, string unusedUrl)
	{
		return Task.FromResult(Marks.Value);
	}

	private static string Read(string resource)
	{
		using Stream stream = typeof(BundledData).Assembly.GetManifestResourceStream(resource) ?? throw new InvalidDataException("Missing bundled module data: " + resource);
		using StreamReader streamReader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
		return streamReader.ReadToEnd();
	}
}
