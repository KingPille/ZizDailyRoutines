using System.Numerics;
using KamiToolKit.Enums;
using KamiToolKit.Nodes.Simplified;

namespace OmenTools.KamiToolKit.Nodes;

public sealed class HorizontalDashedLineNode : SimpleImageNode
{
	public HorizontalDashedLineNode()
	{
		TexturePath = "ui/uld/Lines.tex";
		base.TextureCoordinates = new Vector2(0f, 7f);
		base.TextureSize = new Vector2(24f, 8f);
		base.WrapMode = WrapMode.Tile;
	}
}
