using System.Globalization;

namespace OmenTools.ImGuiOm.Markdown.Rendering;

internal static class ListMarker
{
	public static string For(bool ordered, int itemIndex, int startNumber, bool? taskChecked)
	{
		if (taskChecked.HasValue)
		{
			if (!taskChecked.Value)
			{
				return "[ ]";
			}
			return "[x]";
		}
		if (ordered)
		{
			return (startNumber + itemIndex).ToString(CultureInfo.InvariantCulture) + ".";
		}
		return "•";
	}
}
