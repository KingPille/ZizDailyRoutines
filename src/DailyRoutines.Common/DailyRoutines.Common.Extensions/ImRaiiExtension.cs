using DailyRoutines.Common.Interface.ImGuiDR;
using Dalamud.Interface.Utility.Raii;

namespace DailyRoutines.Common.Extensions;

public static class ImRaiiExtension
{
	extension(ImRaii)
	{
		public static Heading1 Heading1(string text, string? help = null)
		{
			return new Heading1(text, help);
		}

		public static Heading2 Heading2(string text, string? help = null)
		{
			return new Heading2(text, help);
		}
	}
}
