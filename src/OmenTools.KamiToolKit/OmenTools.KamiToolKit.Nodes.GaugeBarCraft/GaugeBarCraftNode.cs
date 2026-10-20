using System.Numerics;

namespace OmenTools.KamiToolKit.Nodes.GaugeBarCraft;

public class GaugeBarCraftNode : GaugeBarNode
{
	private CraftGaugeType type;

	public CraftGaugeType Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
			ApplyType();
		}
	}

	public GaugeBarCraftNode()
	{
		Size = new Vector2(240f, 16f);
		base.TexturePath = "ui/uld/Synthesis.tex";
		ApplyType();
	}

	private void ApplyType()
	{
		float fillV = ((type != CraftGaugeType.Quality) ? 60f : 48f);
		base.FillV = fillV;
	}
}
