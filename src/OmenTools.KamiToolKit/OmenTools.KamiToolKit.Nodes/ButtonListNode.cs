using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.BaseTypes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace OmenTools.KamiToolKit.Nodes;

public abstract class ButtonListNode : SimpleComponentNode
{
}
public abstract class ButtonListNode<T> : ButtonListNode
{
	private ListButtonNode? selectedButtonNode;

	private bool isFocusSet;

	public NineGridNode BackgroundNode { get; }

	public ScrollingNode<VerticalListNode> ScrollingListNode { get; }

	public T? SelectedOption { get; set; }

	public List<T>? Options
	{
		[CompilerGenerated]
		get
		{
			return _003COptions_003Ek__BackingField;
		}
		set
		{
			_003COptions_003Ek__BackingField = value;
			RebuildNodeList();
		}
	}

	public Action<T>? OnOptionSelected { get; set; }

	public int MaxButtons
	{
		[CompilerGenerated]
		get
		{
			return _003CMaxButtons_003Ek__BackingField;
		}
		set
		{
			_003CMaxButtons_003Ek__BackingField = value;
			RebuildNodeList();
		}
	}

	public bool AutoResizeHeight { get; set; }

	protected float ButtonNodeHeight { get; set; } = 22f;

	public unsafe void Show()
	{
		IsVisible = true;
		AddDrawFlags(DrawFlags.RenderOnTop);
		if (base.ParentAddon != null)
		{
			SetFocusable(base.ParentAddon);
		}
	}

	public unsafe void Hide()
	{
		IsVisible = false;
		RemoveDrawFlags(DrawFlags.RenderOnTop);
		if (base.ParentAddon != null)
		{
			ClearFocusable(base.ParentAddon);
		}
	}

	public void Toggle(bool newState)
	{
		if (newState)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	public unsafe void SetFocusable(AtkUnitBase* addon)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Span<Pointer<AtkResNode>> additionalFocusableNodes = ((AtkUnitBase)addon).AdditionalFocusableNodes;
		for (int i = 0; i < additionalFocusableNodes.Length; i++)
		{
			ref Pointer<AtkResNode> reference = ref additionalFocusableNodes[i];
			if (reference.Value == null)
			{
				reference = Pointer<AtkResNode>.op_Implicit(ResNode);
				isFocusSet = true;
			}
		}
	}

	public unsafe void ClearFocusable(AtkUnitBase* addon)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Span<Pointer<AtkResNode>> additionalFocusableNodes = ((AtkUnitBase)addon).AdditionalFocusableNodes;
		for (int i = 0; i < additionalFocusableNodes.Length; i++)
		{
			ref Pointer<AtkResNode> reference = ref additionalFocusableNodes[i];
			if (reference.Value == ResNode)
			{
				reference = Pointer<AtkResNode>.op_Implicit((AtkResNode*)null);
				isFocusSet = false;
			}
		}
	}

	protected ButtonListNode()
	{
		BackgroundNode = new SimpleNineGridNode
		{
			TexturePath = "ui/uld/ListB.tex",
			TextureCoordinates = new Vector2(0f, 0f),
			TextureSize = new Vector2(32f, 32f),
			TopOffset = 10f,
			BottomOffset = 12f,
			LeftOffset = 10f,
			RightOffset = 10f
		};
		BackgroundNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
		ScrollingNode<VerticalListNode> scrollingNode = new ScrollingNode<VerticalListNode>();
		scrollingNode.ContentNode.FirstItemSpacing = 2f;
		scrollingNode.ContentNode.ItemSpacing = 3f;
		scrollingNode.ContentNode.FitContents = true;
		scrollingNode.AutoHideScrollBar = true;
		ScrollingListNode = scrollingNode;
		ScrollingListNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
		BuildTimelines();
	}

	protected override void OnSizeChanged()
	{
		base.OnSizeChanged();
		BackgroundNode.Size = Size;
		ScrollingListNode.Size = new Vector2(Width - 8f, Height - 14f);
		ScrollingListNode.Position = new Vector2(2f, 4f);
		ScrollingListNode.ContentNode.Position = new Vector2(4f, 0f);
		ScrollingListNode.RecalculateSizes();
		foreach (NodeBase node in ScrollingListNode.ContentNode.Nodes)
		{
			node.Width = Width - 16f;
		}
	}

	protected unsafe override void Dispose(bool disposing, bool isNativeDestructor)
	{
		if (disposing)
		{
			if (isFocusSet && !isNativeDestructor && base.ParentAddon != null)
			{
				ClearFocusable(base.ParentAddon);
			}
			base.Dispose(disposing, isNativeDestructor);
		}
	}

	protected abstract string GetLabelForOption(T option);

	protected virtual void OnOptionClick(ListButtonNode listButton, T option)
	{
		if (Options != null)
		{
			ListButtonNode? listButtonNode = selectedButtonNode;
			if (listButtonNode != null)
			{
				listButtonNode.Selected = false;
			}
			selectedButtonNode = listButton;
			selectedButtonNode.Selected = true;
			SelectedOption = option;
			OnOptionSelected?.Invoke(SelectedOption);
		}
	}

	private void RebuildNodeList()
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		ScrollingListNode.ContentNode.Clear();
		foreach (T option in Options ?? new List<T>())
		{
			ListButtonNode newButton = new ListButtonNode
			{
				Size = new Vector2(Width - 16f, ButtonNodeHeight),
				String = ReadOnlySeString.op_Implicit(GetLabelForOption(option))
			};
			newButton.OnClick = delegate
			{
				OnOptionClick(newButton, option);
			};
			ScrollingListNode.ContentNode.AddNode(newButton);
		}
		if (AutoResizeHeight)
		{
			Height = ScrollingListNode.ContentNode.Nodes.Sum((NodeBase nodes) => nodes.Height) + 24f;
		}
		else
		{
			ScrollingListNode.RecalculateSizes();
		}
	}

	private void BuildTimelines()
	{
		AddTimeline(new TimelineBuilder().BeginFrameSet(1, 29).AddLabel(1, 17, (AtkTimelineJumpBehavior)0, 0).AddLabel(9, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(10, 18, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(19, 0, (AtkTimelineJumpBehavior)1, 0)
			.AddLabel(20, 7, (AtkTimelineJumpBehavior)0, 0)
			.AddLabel(29, 0, (AtkTimelineJumpBehavior)1, 0)
			.EndFrameSet()
			.Build());
	}
}
