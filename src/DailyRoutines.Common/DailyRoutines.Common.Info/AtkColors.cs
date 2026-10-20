using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using OmenTools.Extensions;

namespace DailyRoutines.Common.Info;

public static class AtkColors
{
	public readonly struct AtkColor(uint textColor, uint edgeColor)
	{
		public readonly uint TextColor = textColor;

		public readonly uint EdgeColor = edgeColor;

		public void ApplyTo(TextNode node)
		{
			node.TextColor = GetTextColor();
			node.TextOutlineColor = GetEdgeColor();
		}

		public void ApplyTo(LabelTextNode node)
		{
			node.TextColor = GetTextColor();
			node.TextOutlineColor = GetEdgeColor();
		}

		public unsafe Vector4 GetTextColor()
		{
			return ((AtkUIColorHolder)((AtkStage)AtkStage.Instance()).AtkUIColorHolder).GetColor(true, TextColor).ToVector4();
		}

		public unsafe Vector4 GetEdgeColor()
		{
			return ((AtkUIColorHolder)((AtkStage)AtkStage.Instance()).AtkUIColorHolder).GetColor(true, EdgeColor).ToVector4();
		}
	}

	public static AtkColor ListHeader { get; } = new AtkColor(21u, 7u);

	public static AtkColor ListRow { get; } = new AtkColor(8u, 7u);

	public static AtkColor ListFooter { get; } = new AtkColor(3u, 7u);

	public static AtkColor Label { get; } = new AtkColor(8u, 7u);

	public static AtkColor Text { get; } = new AtkColor(2u, 7u);

	public static AtkColor Hint { get; } = new AtkColor(3u, 7u);

	public static AtkColor LabelLight { get; } = new AtkColor(3u, 7u);

	public static AtkColor Value { get; } = new AtkColor(50u, 32u);

	public static AtkColor ValueEmphasize { get; } = new AtkColor(64u, 65u);

	public static AtkColor TargetText { get; } = new AtkColor(1u, 23u);
}
