using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DailyRoutines.LocalData;

public static class HuntSupport
{
	private static readonly ConditionalWeakTable<object, HashSet<ulong>> CompletedAreas = new ConditionalWeakTable<object, HashSet<ulong>>();

	private static readonly Lazy<Dictionary<uint, string>> Guides = new Lazy<Dictionary<uint, string>>(delegate
	{
		using Stream utf8Json = typeof(HuntSupport).Assembly.GetManifestResourceStream("DailyRoutines.LocalData.EnglishDutyGuides.json") ?? throw new InvalidDataException("The bundled English duty guides are missing.");
		return JsonSerializer.Deserialize<Dictionary<uint, string>>(utf8Json) ?? throw new InvalidDataException("The bundled English duty guides are invalid.");
	});

	public static List<string> ReadEnglishGuide(uint dutyID)
	{
		if (Guides.Value.TryGetValue(dutyID, out string value) && !string.IsNullOrWhiteSpace(value))
		{
			return (from line in value.Replace("\r\n", "\n").Split('\n')
				select Regex.Replace(line, "^\\s{0,3}#{1,6}\\s+", string.Empty, RegexOptions.None, TimeSpan.FromSeconds(1L)).Replace("**", string.Empty)).ToList();
		}
		return new List<string>
		{
			$"Duty Guide ({dutyID})",
			"No guide is available for this duty in the bundled English snapshot."
		};
	}

	public static float ClampScale(float scale)
	{
		if (!float.IsFinite(scale))
		{
			return 1f;
		}
		return Math.Clamp(scale, 0.5f, 3f);
	}

	public static void NormalizeScale(ref float scale)
	{
		scale = ClampScale(scale);
	}

	public static Task Completed()
	{
		return Task.CompletedTask;
	}

	public static Task RunLocal(Func<Task> work)
	{
		return work();
	}

	public static int ClampIndex(int index, int count)
	{
		if (count <= 0)
		{
			return 0;
		}
		return Math.Clamp(index, 0, count - 1);
	}

	public static bool RememberArea(bool completed, object scanner, uint zone, uint instance)
	{
		HashSet<ulong> value = CompletedAreas.GetValue(scanner, (object _) => new HashSet<ulong>());
		ulong item = ((ulong)zone << 32) | instance;
		if (completed)
		{
			value.Add(item);
		}
		if (!completed)
		{
			return value.Contains(item);
		}
		return true;
	}

	public static string FormatMacro(string? pattern, object? position, object? name, object? instance)
	{
		if (pattern != null)
		{
			try
			{
				return string.Format(pattern, position, name, instance);
			}
			catch (FormatException)
			{
			}
		}
		return string.Format("{1} at {0} {2}", position, name, instance);
	}
}
