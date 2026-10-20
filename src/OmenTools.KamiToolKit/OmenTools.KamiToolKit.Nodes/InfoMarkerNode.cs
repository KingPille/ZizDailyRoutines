using System.Numerics;
using KamiToolKit.Nodes.Simplified;

namespace OmenTools.KamiToolKit.Nodes;

public class InfoMarkerNode : SimpleNineGridNode
{
	public InfoMarkerNode()
	{
		base.TexturePath = "ui/uld/img04/CircleButtons_hr1.tex";
		base.TextureCoordinates = new Vector2(112f, 84f);
		base.TextureSize = new Vector2(28f);
	}
}
