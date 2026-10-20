using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using OmenTools.Extensions;

namespace OmenTools.KamiToolKit.Nodes;

public sealed class PaginationNode : SimpleComponentNode
{
	[CompilerGenerated]
	private bool _003CIsDisplayIndicatorText_003Ek__BackingField;

	private int? lastMeasuredWidth;

	private bool isTrackingTextWidth;

	private const float BUTTON_WIDTH = 36f;

	private const float BUTTON_HEIGHT = 28f;

	private const float BUTTON_SPACING = 9f;

	public TextureButtonNode PreviousPageButtonNode { get; }

	public TextNode IndicatorTextNode { get; }

	public TextureButtonNode NextPageButtonNode { get; }

	public HorizontalListNode LayoutNode { get; }

	public Action? OnPreviousPage { get; set; }

	public Action? OnNextPage { get; set; }

	public bool IsDisplayIndicatorText
	{
		[CompilerGenerated]
		get
		{
			return _003CIsDisplayIndicatorText_003Ek__BackingField;
		}
		set
		{
			if (_003CIsDisplayIndicatorText_003Ek__BackingField != value)
			{
				_003CIsDisplayIndicatorText_003Ek__BackingField = value;
				IndicatorTextNode.IsVisible = value;
				UpdateLayout();
			}
		}
	}

	public PaginationNode()
	{
		PreviousPageButtonNode = new TextureButtonNode
		{
			TexturePath = "ui/uld/ItemSearch.tex",
			TextureCoordinates = new Vector2(0f, 36f),
			TextureSize = new Vector2(36f, 28f),
			Size = new Vector2(36f, 28f)
		};
		PreviousPageButtonNode.OnClick = delegate
		{
			OnPreviousPage?.Invoke();
		};
		IndicatorTextNode = new TextNode
		{
			FontType = (FontType)1,
			FontSize = 14u,
			LineSpacing = 0u,
			CharSpacing = 0u,
			AlignmentType = (AlignmentType)4,
			TextColor = ColorHelper.GetColor(64u),
			TextOutlineColor = ColorHelper.GetColor(65u),
			TextFlags = (TextFlags)16,
			Height = 28f,
			OnStringUpdated = UpdateLayout,
			IsVisible = false
		};
		NextPageButtonNode = new TextureButtonNode
		{
			TexturePath = "ui/uld/ItemSearch.tex",
			TextureCoordinates = new Vector2(0f, 36f),
			TextureSize = new Vector2(36f, 28f),
			Size = new Vector2(36f, 28f)
		};
		NextPageButtonNode.ImageNode.ImageNodeFlags = (ImageNodeFlags)1;
		NextPageButtonNode.OnClick = delegate
		{
			OnNextPage?.Invoke();
		};
		LayoutNode = new HorizontalListNode
		{
			ItemSpacing = 9f,
			Height = 28f,
			FitToContentWidth = true
		};
		LayoutNode.AddNode(new global::_003C_003Ez__ReadOnlyArray<NodeBase>(new NodeBase[3] { PreviousPageButtonNode, IndicatorTextNode, NextPageButtonNode }));
		LayoutNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
		UpdateLayout();
	}

	protected override void Dispose(bool disposing, bool isNativeDestructor)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		if (disposing)
		{
			if (isTrackingTextWidth && !DalamudServiceExtension.Instance<IFramework>().IsFrameworkUnloading)
			{
				DalamudServiceExtension.Instance<IFramework>().Update -= new OnUpdateDelegate(OnFrameworkUpdate);
				isTrackingTextWidth = false;
			}
			base.Dispose(disposing, isNativeDestructor);
		}
	}

	private void UpdateLayout()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		ApplyTextWidth();
		if (!isTrackingTextWidth)
		{
			isTrackingTextWidth = true;
			DalamudServiceExtension.Instance<IFramework>().Update += new OnUpdateDelegate(OnFrameworkUpdate);
		}
	}

	private void OnFrameworkUpdate(IFramework framework)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		if (!ApplyTextWidth())
		{
			DalamudServiceExtension.Instance<IFramework>().Update -= new OnUpdateDelegate(OnFrameworkUpdate);
			isTrackingTextWidth = false;
		}
	}

	private bool ApplyTextWidth()
	{
		int num = (int)IndicatorTextNode.GetTextDrawSize().X;
		if (num == lastMeasuredWidth)
		{
			return false;
		}
		lastMeasuredWidth = num;
		IndicatorTextNode.Width = num;
		LayoutNode.RecalculateLayout();
		Size = LayoutNode.Size;
		return true;
	}
}
