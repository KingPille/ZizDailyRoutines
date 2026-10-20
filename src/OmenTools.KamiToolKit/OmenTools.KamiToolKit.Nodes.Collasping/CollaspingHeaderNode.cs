using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;

namespace OmenTools.KamiToolKit.Nodes.Collasping;

public class CollaspingHeaderNode : ResNode
{
	public NineGridNode DecorationNode { get; }

	public TextNode LabelNode { get; }

	public ReadOnlySeString String
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return LabelNode.String;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			LabelNode.String = value;
		}
	}

	public CollaspingHeaderNode()
	{
		DecorationNode = new SimpleNineGridNode
		{
			TexturePath = "ui/uld/journal_Separator.tex",
			TextureCoordinates = new Vector2(0f, 0f),
			TextureSize = new Vector2(424f, 24f),
			Size = new Vector2(24f, 24f),
			LeftOffset = 25f,
			RightOffset = 20f
		};
		DecorationNode.AttachNode(this);
		LabelNode = new TextNode
		{
			Position = new Vector2(22f, 1f),
			TextColor = ColorHelper.GetColor(7u),
			AlignmentType = (AlignmentType)3,
			FontSize = 12u,
			FontType = (FontType)0
		};
		LabelNode.AttachNode(this);
	}

	protected override void OnSizeChanged()
	{
		base.OnSizeChanged();
		DecorationNode.Size = Size;
		LabelNode.Size = new Vector2(Width - 22f, Height);
	}
}
