using KamiToolKit.Nodes;

namespace DailyRoutines.Common.Extensions;

public static class TextNodeExtension
{
	extension(TextNode textNode)
	{
		public void AutoAdjustTextSize()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			while (textNode.FontSize > 1 && textNode.GetTextDrawSize(textNode.String, considerScale: false).X > textNode.Size.X)
			{
				textNode.FontSize--;
			}
		}
	}
}
