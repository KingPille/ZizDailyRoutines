using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace OmenTools.KamiToolKit.Nodes.Collasping;

public class CollaspingCategoryNode : ResNode
{
	[CompilerGenerated]
	private bool _003CIsCollapsed_003Ek__BackingField;

	private readonly List<NodeBase> children = new List<NodeBase>();

	public NineGridNode BackgroundNode { get; }

	public ResNode ChildContainer { get; }

	public ImageNode CollapseArrowNode { get; }

	public CollisionNode CollisionNode { get; }

	public TextNode LabelNode { get; }

	public IReadOnlyCollection<CollaspingHeaderNode> HeaderNodes => children.OfType<CollaspingHeaderNode>().ToList();

	public IReadOnlyCollection<NodeBase> Children => children.AsReadOnly();

	public Action<bool>? OnToggle { get; set; }

	public CollaspingNode? ParentTreeListNode { get; set; }

	public bool IsCollapsed
	{
		[CompilerGenerated]
		get
		{
			return _003CIsCollapsed_003Ek__BackingField;
		}
		set
		{
			_003CIsCollapsed_003Ek__BackingField = value;
			UpdateCollapsed();
			base.Timeline?.PlayAnimation(IsCollapsed ? 1 : 8);
		}
	}

	public float VerticalPadding { get; set; } = 4f;

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

	public IEnumerable<T> GetNodes<T>() where T : NodeBase
	{
		return children.OfType<T>();
	}

	public void RecalculateLayout()
	{
		ChildContainer.Height = 0f;
		foreach (NodeBase child in children)
		{
			if (child.IsVisible)
			{
				child.Y = ChildContainer.Height;
				child.Width = ChildContainer.Width;
				ChildContainer.Height += child.Height + VerticalPadding;
				Height = ChildContainer.Height + BackgroundNode.Height;
			}
		}
		UpdateCollapsed();
	}

	public void AddHeader(ReadOnlySeString label)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		CollaspingHeaderNode node = new CollaspingHeaderNode
		{
			Size = new Vector2(Width, 24f),
			String = label
		};
		AddNode(node);
	}

	public void AddNode(NodeBase node)
	{
		node.Y = ChildContainer.Height;
		node.Width = ChildContainer.Width;
		ChildContainer.Height += node.Height + VerticalPadding;
		Height = ChildContainer.Height + BackgroundNode.Height;
		children.Add(node);
		node.AttachNode(ChildContainer);
		UpdateCollapsed();
	}

	public unsafe CollaspingCategoryNode()
	{
		CollisionNode = new CollisionNode
		{
			Height = 28f
		};
		CollisionNode.AttachNode(this);
		BackgroundNode = new SimpleNineGridNode
		{
			TexturePath = "ui/uld/ListItemB.tex",
			TextureSize = new Vector2(48f, 28f),
			TextureCoordinates = new Vector2(0f, 24f),
			Height = 28f,
			TopOffset = 10f,
			LeftOffset = 12f,
			RightOffset = 12f,
			BottomOffset = 12f
		};
		BackgroundNode.AttachNode(this);
		CollapseArrowNode = new ImageNode
		{
			Position = new Vector2(0f, 1f),
			Size = new Vector2(24f, 24f),
			PartId = 1u
		};
		CollapseArrowNode.AddPart(new Part
		{
			TexturePath = "ui/uld/ListItemB.tex",
			TextureCoordinates = new Vector2(0f, 0f),
			Size = new Vector2(24f, 24f),
			Id = 0u
		});
		CollapseArrowNode.AddPart(new Part
		{
			TexturePath = "ui/uld/ListItemB.tex",
			TextureCoordinates = new Vector2(24f, 0f),
			Size = new Vector2(24f, 24f),
			Id = 1u
		});
		CollapseArrowNode.AttachNode(this);
		LabelNode = new TextNode
		{
			Position = new Vector2(23f, 0f),
			FontType = (FontType)0,
			FontSize = 14u,
			Height = 28f,
			AlignmentType = (AlignmentType)3,
			TextColor = ColorHelper.GetColor(50u),
			TextOutlineColor = ColorHelper.GetColor(7u)
		};
		LabelNode.AttachNode(this);
		ChildContainer = new ResNode
		{
			Position = new Vector2(0f, 24f + VerticalPadding)
		};
		ChildContainer.AttachNode(this);
		BuildTimelines();
		CollisionNode.ShowClickableCursor = true;
		CollisionNode.AddEvent((AtkEventType)6, (Action)delegate
		{
			base.Timeline?.PlayAnimation(IsCollapsed ? 2 : 9);
		});
		CollisionNode.AddEvent((AtkEventType)7, (Action)delegate
		{
			base.Timeline?.PlayAnimation(IsCollapsed ? 1 : 8);
		});
		CollisionNode.AddEvent((AtkEventType)9, (Action)delegate
		{
			IsCollapsed = !IsCollapsed;
			UpdateCollapsed();
			OnToggle?.Invoke(!IsCollapsed);
		});
	}

	protected override void OnSizeChanged()
	{
		base.OnSizeChanged();
		BackgroundNode.Width = Width;
		CollapseArrowNode.Width = 24f;
		LabelNode.Width = Width - 23f;
		ChildContainer.Width = Width;
		CollisionNode.Width = Width;
		foreach (NodeBase child in children)
		{
			child.Width = Width;
		}
	}

	private void UpdateCollapsed()
	{
		base.Timeline?.PlayAnimation(IsCollapsed ? 1 : 8);
		ChildContainer.IsVisible = !IsCollapsed;
		Height = (IsCollapsed ? BackgroundNode.Height : (ChildContainer.Height + BackgroundNode.Height));
		ParentTreeListNode?.RefreshLayout();
	}

	private void BuildTimelines()
	{
		AddTimeline(new TimelineBuilder().BeginFrameSet(1, 119).AddLabel(1, 1, (AtkTimelineJumpBehavior)0, 0).AddLabel(9, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(10, 2, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(19, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(20, 3, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(29, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(30, 7, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(39, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(40, 6, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(49, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(50, 4, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(59, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(60, 8, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(69, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(70, 9, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(79, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(80, 10, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(89, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(90, 14, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(99, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(100, 13, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(109, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(110, 11, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(119, 0, (AtkTimelineJumpBehavior)1, 0)
			.EndFrameSet()
			.Build());
		ImageNode collapseArrowNode = CollapseArrowNode;
		FrameSetBuilder frameSetBuilder = new TimelineBuilder().BeginFrameSet(1, 9);
		byte? alpha = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder2 = frameSetBuilder.AddFrame(1, null, alpha);
		Vector3? addColor = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder3 = frameSetBuilder2.AddFrame(1, null, null, addColor, multiplyColor);
		uint? partId = 0u;
		FrameSetBuilder frameSetBuilder4 = frameSetBuilder3.AddFrame(1, null, null, null, null, null, null, null, null, partId).EndFrameSet().BeginFrameSet(10, 19);
		byte? alpha2 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder5 = frameSetBuilder4.AddFrame(10, null, alpha2);
		byte? alpha3 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder6 = frameSetBuilder5.AddFrame(12, null, alpha3);
		Vector3? addColor2 = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor2 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder7 = frameSetBuilder6.AddFrame(10, null, null, addColor2, multiplyColor2);
		Vector3? addColor3 = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor3 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder8 = frameSetBuilder7.AddFrame(12, null, null, addColor3, multiplyColor3);
		uint? partId2 = 0u;
		FrameSetBuilder frameSetBuilder9 = frameSetBuilder8.AddFrame(10, null, null, null, null, null, null, null, null, partId2);
		uint? partId3 = 0u;
		FrameSetBuilder frameSetBuilder10 = frameSetBuilder9.AddFrame(12, null, null, null, null, null, null, null, null, partId3).EndFrameSet().BeginFrameSet(20, 29);
		byte? alpha4 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder11 = frameSetBuilder10.AddFrame(20, null, alpha4);
		Vector3? addColor4 = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor4 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder12 = frameSetBuilder11.AddFrame(20, null, null, addColor4, multiplyColor4);
		uint? partId4 = 0u;
		FrameSetBuilder frameSetBuilder13 = frameSetBuilder12.AddFrame(20, null, null, null, null, null, null, null, null, partId4).EndFrameSet().BeginFrameSet(30, 39);
		byte? alpha5 = (byte)178;
		FrameSetBuilder frameSetBuilder14 = frameSetBuilder13.AddFrame(30, null, alpha5);
		Vector3? addColor5 = new Vector3(0f, 0f, 0f);
		Vector3? multiplyColor5 = new Vector3(50f, 50f, 50f);
		FrameSetBuilder frameSetBuilder15 = frameSetBuilder14.AddFrame(30, null, null, addColor5, multiplyColor5);
		uint? partId5 = 0u;
		FrameSetBuilder frameSetBuilder16 = frameSetBuilder15.AddFrame(30, null, null, null, null, null, null, null, null, partId5).EndFrameSet().BeginFrameSet(40, 49);
		byte? alpha6 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder17 = frameSetBuilder16.AddFrame(40, null, alpha6);
		Vector3? addColor6 = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor6 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder18 = frameSetBuilder17.AddFrame(40, null, null, addColor6, multiplyColor6);
		uint? partId6 = 0u;
		FrameSetBuilder frameSetBuilder19 = frameSetBuilder18.AddFrame(40, null, null, null, null, null, null, null, null, partId6).EndFrameSet().BeginFrameSet(50, 59);
		byte? alpha7 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder20 = frameSetBuilder19.AddFrame(50, null, alpha7);
		byte? alpha8 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder21 = frameSetBuilder20.AddFrame(52, null, alpha8);
		Vector3? addColor7 = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor7 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder22 = frameSetBuilder21.AddFrame(50, null, null, addColor7, multiplyColor7);
		Vector3? addColor8 = new Vector3(0f, 0f, 0f);
		Vector3? multiplyColor8 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder23 = frameSetBuilder22.AddFrame(52, null, null, addColor8, multiplyColor8);
		uint? partId7 = 0u;
		FrameSetBuilder frameSetBuilder24 = frameSetBuilder23.AddFrame(50, null, null, null, null, null, null, null, null, partId7);
		uint? partId8 = 0u;
		FrameSetBuilder frameSetBuilder25 = frameSetBuilder24.AddFrame(52, null, null, null, null, null, null, null, null, partId8).EndFrameSet().BeginFrameSet(60, 69);
		byte? alpha9 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder26 = frameSetBuilder25.AddFrame(60, null, alpha9);
		Vector3? addColor9 = new Vector3(0f, 0f, 0f);
		Vector3? multiplyColor9 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder27 = frameSetBuilder26.AddFrame(60, null, null, addColor9, multiplyColor9);
		uint? partId9 = 1u;
		FrameSetBuilder frameSetBuilder28 = frameSetBuilder27.AddFrame(60, null, null, null, null, null, null, null, null, partId9).EndFrameSet().BeginFrameSet(70, 79);
		byte? alpha10 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder29 = frameSetBuilder28.AddFrame(70, null, alpha10);
		byte? alpha11 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder30 = frameSetBuilder29.AddFrame(72, null, alpha11);
		Vector3? addColor10 = new Vector3(0f, 0f, 0f);
		Vector3? multiplyColor10 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder31 = frameSetBuilder30.AddFrame(70, null, null, addColor10, multiplyColor10);
		Vector3? addColor11 = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor11 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder32 = frameSetBuilder31.AddFrame(72, null, null, addColor11, multiplyColor11);
		uint? partId10 = 1u;
		FrameSetBuilder frameSetBuilder33 = frameSetBuilder32.AddFrame(70, null, null, null, null, null, null, null, null, partId10);
		uint? partId11 = 1u;
		FrameSetBuilder frameSetBuilder34 = frameSetBuilder33.AddFrame(72, null, null, null, null, null, null, null, null, partId11).EndFrameSet().BeginFrameSet(80, 89);
		byte? alpha12 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder35 = frameSetBuilder34.AddFrame(80, null, alpha12);
		Vector3? addColor12 = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor12 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder36 = frameSetBuilder35.AddFrame(80, null, null, addColor12, multiplyColor12);
		uint? partId12 = 0u;
		FrameSetBuilder frameSetBuilder37 = frameSetBuilder36.AddFrame(80, null, null, null, null, null, null, null, null, partId12).EndFrameSet().BeginFrameSet(90, 99);
		byte? alpha13 = (byte)178;
		FrameSetBuilder frameSetBuilder38 = frameSetBuilder37.AddFrame(90, null, alpha13);
		Vector3? addColor13 = new Vector3(0f, 0f, 0f);
		Vector3? multiplyColor13 = new Vector3(50f, 50f, 50f);
		FrameSetBuilder frameSetBuilder39 = frameSetBuilder38.AddFrame(90, null, null, addColor13, multiplyColor13);
		uint? partId13 = 1u;
		FrameSetBuilder frameSetBuilder40 = frameSetBuilder39.AddFrame(90, null, null, null, null, null, null, null, null, partId13).EndFrameSet().BeginFrameSet(100, 109);
		byte? alpha14 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder41 = frameSetBuilder40.AddFrame(100, null, alpha14);
		Vector3? addColor14 = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor14 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder42 = frameSetBuilder41.AddFrame(100, null, null, addColor14, multiplyColor14);
		uint? partId14 = 1u;
		FrameSetBuilder frameSetBuilder43 = frameSetBuilder42.AddFrame(100, null, null, null, null, null, null, null, null, partId14).EndFrameSet().BeginFrameSet(110, 119);
		byte? alpha15 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder44 = frameSetBuilder43.AddFrame(110, null, alpha15);
		byte? alpha16 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder45 = frameSetBuilder44.AddFrame(112, null, alpha16);
		Vector3? addColor15 = new Vector3(16f, 16f, 16f);
		Vector3? multiplyColor15 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder46 = frameSetBuilder45.AddFrame(110, null, null, addColor15, multiplyColor15);
		Vector3? addColor16 = new Vector3(0f, 0f, 0f);
		Vector3? multiplyColor16 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder47 = frameSetBuilder46.AddFrame(112, null, null, addColor16, multiplyColor16);
		uint? partId15 = 1u;
		FrameSetBuilder frameSetBuilder48 = frameSetBuilder47.AddFrame(110, null, null, null, null, null, null, null, null, partId15);
		uint? partId16 = 1u;
		collapseArrowNode.AddTimeline(frameSetBuilder48.AddFrame(112, null, null, null, null, null, null, null, null, partId16).EndFrameSet().Build());
		TextNode labelNode = LabelNode;
		FrameSetBuilder frameSetBuilder49 = new TimelineBuilder().BeginFrameSet(1, 9);
		alpha5 = (byte)229;
		FrameSetBuilder frameSetBuilder50 = frameSetBuilder49.AddFrame(1, null, alpha5);
		multiplyColor5 = new Vector3(0f, 0f, 0f);
		addColor5 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder51 = frameSetBuilder50.AddFrame(1, null, null, multiplyColor5, addColor5).EndFrameSet().BeginFrameSet(10, 19);
		alpha6 = (byte)229;
		FrameSetBuilder frameSetBuilder52 = frameSetBuilder51.AddFrame(10, null, alpha6);
		multiplyColor6 = new Vector3(0f, 0f, 0f);
		addColor6 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder53 = frameSetBuilder52.AddFrame(10, null, null, multiplyColor6, addColor6).EndFrameSet().BeginFrameSet(20, 29);
		alpha7 = (byte)229;
		FrameSetBuilder frameSetBuilder54 = frameSetBuilder53.AddFrame(20, null, alpha7);
		multiplyColor7 = new Vector3(0f, 0f, 0f);
		addColor7 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder55 = frameSetBuilder54.AddFrame(20, null, null, multiplyColor7, addColor7).EndFrameSet().BeginFrameSet(30, 39);
		alpha8 = (byte)153;
		FrameSetBuilder frameSetBuilder56 = frameSetBuilder55.AddFrame(30, null, alpha8);
		multiplyColor8 = new Vector3(0f, 0f, 0f);
		addColor8 = new Vector3(80f, 80f, 80f);
		FrameSetBuilder frameSetBuilder57 = frameSetBuilder56.AddFrame(30, null, null, multiplyColor8, addColor8).EndFrameSet().BeginFrameSet(40, 49);
		alpha9 = (byte)229;
		FrameSetBuilder frameSetBuilder58 = frameSetBuilder57.AddFrame(40, null, alpha9);
		multiplyColor9 = new Vector3(0f, 0f, 0f);
		addColor9 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder59 = frameSetBuilder58.AddFrame(40, null, null, multiplyColor9, addColor9).EndFrameSet().BeginFrameSet(50, 59);
		alpha10 = (byte)229;
		FrameSetBuilder frameSetBuilder60 = frameSetBuilder59.AddFrame(50, null, alpha10);
		multiplyColor10 = new Vector3(0f, 0f, 0f);
		addColor10 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder61 = frameSetBuilder60.AddFrame(50, null, null, multiplyColor10, addColor10).EndFrameSet().BeginFrameSet(60, 69);
		alpha11 = (byte)229;
		FrameSetBuilder frameSetBuilder62 = frameSetBuilder61.AddFrame(60, null, alpha11);
		multiplyColor11 = new Vector3(0f, 0f, 0f);
		addColor11 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder63 = frameSetBuilder62.AddFrame(60, null, null, multiplyColor11, addColor11).EndFrameSet().BeginFrameSet(70, 79);
		alpha12 = (byte)229;
		FrameSetBuilder frameSetBuilder64 = frameSetBuilder63.AddFrame(70, null, alpha12);
		multiplyColor12 = new Vector3(0f, 0f, 0f);
		addColor12 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder65 = frameSetBuilder64.AddFrame(70, null, null, multiplyColor12, addColor12).EndFrameSet().BeginFrameSet(80, 89);
		alpha13 = (byte)229;
		FrameSetBuilder frameSetBuilder66 = frameSetBuilder65.AddFrame(80, null, alpha13);
		multiplyColor13 = new Vector3(0f, 0f, 0f);
		addColor13 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder67 = frameSetBuilder66.AddFrame(80, null, null, multiplyColor13, addColor13).EndFrameSet().BeginFrameSet(90, 99);
		alpha14 = (byte)153;
		FrameSetBuilder frameSetBuilder68 = frameSetBuilder67.AddFrame(90, null, alpha14);
		multiplyColor14 = new Vector3(0f, 0f, 0f);
		addColor14 = new Vector3(80f, 80f, 80f);
		FrameSetBuilder frameSetBuilder69 = frameSetBuilder68.AddFrame(90, null, null, multiplyColor14, addColor14).EndFrameSet().BeginFrameSet(100, 109);
		alpha15 = (byte)229;
		FrameSetBuilder frameSetBuilder70 = frameSetBuilder69.AddFrame(100, null, alpha15);
		multiplyColor15 = new Vector3(0f, 0f, 0f);
		addColor15 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder71 = frameSetBuilder70.AddFrame(100, null, null, multiplyColor15, addColor15).EndFrameSet().BeginFrameSet(110, 119);
		alpha16 = (byte)229;
		FrameSetBuilder frameSetBuilder72 = frameSetBuilder71.AddFrame(110, null, alpha16);
		multiplyColor16 = new Vector3(0f, 0f, 0f);
		addColor16 = new Vector3(100f, 100f, 100f);
		labelNode.AddTimeline(frameSetBuilder72.AddFrame(110, null, null, multiplyColor16, addColor16).EndFrameSet().Build());
		NineGridNode backgroundNode = BackgroundNode;
		FrameSetBuilder frameSetBuilder73 = new TimelineBuilder().BeginFrameSet(1, 9);
		alpha = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder74 = frameSetBuilder73.AddFrame(1, null, alpha);
		multiplyColor = new Vector3(0f, 0f, 0f);
		addColor = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder75 = frameSetBuilder74.AddFrame(1, null, null, multiplyColor, addColor).EndFrameSet().BeginFrameSet(10, 19);
		alpha2 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder76 = frameSetBuilder75.AddFrame(10, null, alpha2);
		alpha3 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder77 = frameSetBuilder76.AddFrame(12, null, alpha3);
		multiplyColor2 = new Vector3(0f, 0f, 0f);
		addColor2 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder78 = frameSetBuilder77.AddFrame(10, null, null, multiplyColor2, addColor2);
		multiplyColor3 = new Vector3(16f, 16f, 16f);
		addColor3 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder79 = frameSetBuilder78.AddFrame(12, null, null, multiplyColor3, addColor3).EndFrameSet().BeginFrameSet(20, 29);
		alpha4 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder80 = frameSetBuilder79.AddFrame(20, null, alpha4);
		multiplyColor4 = new Vector3(16f, 16f, 16f);
		addColor4 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder81 = frameSetBuilder80.AddFrame(20, null, null, multiplyColor4, addColor4).EndFrameSet().BeginFrameSet(30, 39);
		alpha5 = (byte)178;
		FrameSetBuilder frameSetBuilder82 = frameSetBuilder81.AddFrame(30, null, alpha5);
		addColor5 = new Vector3(0f, 0f, 0f);
		multiplyColor5 = new Vector3(50f, 50f, 50f);
		FrameSetBuilder frameSetBuilder83 = frameSetBuilder82.AddFrame(30, null, null, addColor5, multiplyColor5).EndFrameSet().BeginFrameSet(40, 49);
		alpha6 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder84 = frameSetBuilder83.AddFrame(40, null, alpha6);
		addColor6 = new Vector3(16f, 16f, 16f);
		multiplyColor6 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder85 = frameSetBuilder84.AddFrame(40, null, null, addColor6, multiplyColor6).EndFrameSet().BeginFrameSet(50, 59);
		alpha7 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder86 = frameSetBuilder85.AddFrame(50, null, alpha7);
		alpha8 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder87 = frameSetBuilder86.AddFrame(52, null, alpha8);
		addColor7 = new Vector3(16f, 16f, 16f);
		multiplyColor7 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder88 = frameSetBuilder87.AddFrame(50, null, null, addColor7, multiplyColor7);
		addColor8 = new Vector3(0f, 0f, 0f);
		multiplyColor8 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder89 = frameSetBuilder88.AddFrame(52, null, null, addColor8, multiplyColor8).EndFrameSet().BeginFrameSet(60, 69);
		alpha9 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder90 = frameSetBuilder89.AddFrame(60, null, alpha9);
		addColor9 = new Vector3(0f, 0f, 0f);
		multiplyColor9 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder91 = frameSetBuilder90.AddFrame(60, null, null, addColor9, multiplyColor9).EndFrameSet().BeginFrameSet(70, 79);
		alpha10 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder92 = frameSetBuilder91.AddFrame(70, null, alpha10);
		alpha11 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder93 = frameSetBuilder92.AddFrame(72, null, alpha11);
		addColor10 = new Vector3(0f, 0f, 0f);
		multiplyColor10 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder94 = frameSetBuilder93.AddFrame(70, null, null, addColor10, multiplyColor10);
		addColor11 = new Vector3(16f, 16f, 16f);
		multiplyColor11 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder95 = frameSetBuilder94.AddFrame(72, null, null, addColor11, multiplyColor11).EndFrameSet().BeginFrameSet(80, 89);
		alpha12 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder96 = frameSetBuilder95.AddFrame(80, null, alpha12);
		addColor12 = new Vector3(16f, 16f, 16f);
		multiplyColor12 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder97 = frameSetBuilder96.AddFrame(80, null, null, addColor12, multiplyColor12).EndFrameSet().BeginFrameSet(90, 99);
		alpha13 = (byte)178;
		FrameSetBuilder frameSetBuilder98 = frameSetBuilder97.AddFrame(90, null, alpha13);
		addColor13 = new Vector3(0f, 0f, 0f);
		multiplyColor13 = new Vector3(50f, 50f, 50f);
		FrameSetBuilder frameSetBuilder99 = frameSetBuilder98.AddFrame(90, null, null, addColor13, multiplyColor13).EndFrameSet().BeginFrameSet(100, 109);
		alpha14 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder100 = frameSetBuilder99.AddFrame(100, null, alpha14);
		addColor14 = new Vector3(16f, 16f, 16f);
		multiplyColor14 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder101 = frameSetBuilder100.AddFrame(100, null, null, addColor14, multiplyColor14).EndFrameSet().BeginFrameSet(110, 119);
		alpha15 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder102 = frameSetBuilder101.AddFrame(110, null, alpha15);
		alpha16 = (byte)byte.MaxValue;
		FrameSetBuilder frameSetBuilder103 = frameSetBuilder102.AddFrame(112, null, alpha16);
		addColor15 = new Vector3(16f, 16f, 16f);
		multiplyColor15 = new Vector3(100f, 100f, 100f);
		FrameSetBuilder frameSetBuilder104 = frameSetBuilder103.AddFrame(110, null, null, addColor15, multiplyColor15);
		addColor16 = new Vector3(0f, 0f, 0f);
		multiplyColor16 = new Vector3(100f, 100f, 100f);
		backgroundNode.AddTimeline(frameSetBuilder104.AddFrame(112, null, null, addColor16, multiplyColor16).EndFrameSet().Build());
	}
}
