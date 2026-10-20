using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Enums;
using KamiToolKit.Extensions;
using KamiToolKit.Nodes.Simplified;

namespace OmenTools.KamiToolKit.Nodes;

public class GaugeBarNode : ComponentNode<AtkComponentGaugeBar, AtkUldComponentDataGaugeBar>
{
	public SimpleNineGridNode BackdropNode { get; }

	public SimpleNineGridNode FillNode { get; }

	public SimpleNineGridNode IncreaseNode { get; }

	public SimpleNineGridNode DecreaseNode { get; }

	public SimpleNineGridNode BorderNode { get; }

	public unsafe int Value
	{
		get
		{
			return ((AtkComponentGaugeBar)base.Component).Values[0].ValueInt;
		}
		set
		{
			((AtkComponentGaugeBar)base.Component).SetGaugeValue(value, 0, false);
		}
	}

	public unsafe int MinValue
	{
		get
		{
			return ((AtkComponentGaugeBar)base.Component).MinValue;
		}
		set
		{
			if (value <= ((AtkComponentGaugeBar)base.Component).MaxValue)
			{
				int minValue = ((AtkComponentGaugeBar)base.Component).MinValue;
				((AtkComponentGaugeBar)base.Component).MinValue = value;
				if (value > minValue && Value < value)
				{
					SetValueInstant(value);
				}
			}
		}
	}

	public unsafe int MaxValue
	{
		get
		{
			return ((AtkComponentGaugeBar)base.Component).MaxValue;
		}
		set
		{
			if (value >= ((AtkComponentGaugeBar)base.Component).MinValue)
			{
				int maxValue = ((AtkComponentGaugeBar)base.Component).MaxValue;
				((AtkComponentGaugeBar)base.Component).MaxValue = value;
				if (value < maxValue && Value > value)
				{
					SetValueInstant(value);
				}
			}
		}
	}

	public virtual Vector4 BackgroundColor
	{
		get
		{
			return BackdropNode.Color;
		}
		set
		{
			BackdropNode.Color = value;
		}
	}

	public Vector4 BarColor
	{
		get
		{
			return FillNode.Color;
		}
		set
		{
			FillNode.Color = value;
			IncreaseNode.Color = value;
			DecreaseNode.Color = value;
		}
	}

	public string TexturePath
	{
		set
		{
			BackdropNode.TexturePath = value;
			FillNode.TexturePath = value;
			IncreaseNode.TexturePath = value;
			DecreaseNode.TexturePath = value;
			BorderNode.TexturePath = value;
		}
	}

	public float FillV
	{
		set
		{
			FillNode.TextureCoordinates = new Vector2(0f, value);
		}
	}

	public unsafe GaugeBarNode()
	{
		SetInternalComponentType((ComponentType)5);
		AddDrawFlags(DrawFlags.ClickableCursor);
		BackdropNode = CreateNineGridNode(0f, 0f, 64f, 16f, 8f, 8f, 2f, 3f, 32f);
		IncreaseNode = CreateNineGridNode(0f, 2f, 64f, 12f, 8f, 8f, 2f, 3f, 96f);
		DecreaseNode = CreateNineGridNode(0f, 2f, 64f, 12f, 8f, 8f, 2f, 3f, 84f);
		FillNode = CreateNineGridNode(0f, 2f, 64f, 12f, 8f, 8f, 2f, 3f, 60f);
		BorderNode = CreateNineGridNode(0f, 2f, 64f, 12f, 8f, 8f, 2f, 3f, 108f);
		((AtkUldComponentDataGaugeBar)base.Data).Nodes[0] = FillNode.NodeId;
		((AtkUldComponentDataGaugeBar)base.Data).Nodes[1] = BackdropNode.NodeId;
		((AtkUldComponentDataGaugeBar)base.Data).Nodes[3] = IncreaseNode.NodeId;
		((AtkUldComponentDataGaugeBar)base.Data).Nodes[4] = DecreaseNode.NodeId;
		((AtkUldComponentDataGaugeBar)base.Data).Nodes[5] = BorderNode.NodeId;
		((AtkUldComponentDataGaugeBar)base.Data).Min = 0;
		((AtkUldComponentDataGaugeBar)base.Data).Max = 0;
		((AtkUldComponentDataGaugeBar)base.Data).Value = 0;
		ApplyFillNodes();
		InitializeComponentEvents();
		ApplyFillNodes();
		((AtkComponentGaugeBar)base.Component).MinValue = 0;
		((AtkComponentGaugeBar)base.Component).MaxValue = 100;
		SetValueInstant(0);
	}

	public unsafe void SetValueInstant(int newValue)
	{
		((AtkComponentGaugeBar)base.Component).SetGaugeValue(newValue, 0, true);
	}

	protected unsafe override void OnSizeChanged()
	{
		base.OnSizeChanged();
		BackdropNode.Size = new Vector2(Width, 16f);
		BorderNode.Size = new Vector2(Width, 12f);
		FillNode.Size = new Vector2(Width, 12f);
		if (Width <= (float)(int)(ushort)((AtkComponentGaugeBar)base.Component).MarginX)
		{
			((AtkComponentGaugeBar)base.Component).MaxFillPositionX = 0;
		}
		else
		{
			((AtkComponentGaugeBar)base.Component).MaxFillPositionX = (short)(Width - (float)(int)(ushort)((AtkComponentGaugeBar)base.Component).MarginX);
		}
	}

	private unsafe SimpleNineGridNode CreateNineGridNode(float positionX, float positionY, float width, float height, float leftOffset, float rightOffset, float topOffset, float bottomOffset, float textureV)
	{
		SimpleNineGridNode simpleNineGridNode = new SimpleNineGridNode
		{
			Position = new Vector2(positionX, positionY),
			TextureCoordinates = new Vector2(0f, textureV),
			TextureSize = new Vector2(width, height),
			Offsets = new Vector4(topOffset, bottomOffset, leftOffset, rightOffset)
		};
		simpleNineGridNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
		((AtkComponentBase)ComponentBase).UldManager.AddNodeToObjectList(simpleNineGridNode);
		return simpleNineGridNode;
	}

	private unsafe void ApplyFillNodes()
	{
		((AtkComponentGaugeBar)base.Component).BackdropImageNode = (AtkImageNode*)BackdropNode.ResNode;
		((GaugeFill)(&((AtkComponentGaugeBar)base.Component).PrimaryFill)).MainFillNode = FillNode;
		((GaugeFill)(&((AtkComponentGaugeBar)base.Component).PrimaryFill)).IncreaseFillNode = IncreaseNode;
		((GaugeFill)(&((AtkComponentGaugeBar)base.Component).PrimaryFill)).DecreaseFillNode = DecreaseNode;
		((AtkComponentGaugeBar)base.Component).BorderNineGridNode = BorderNode;
	}
}
