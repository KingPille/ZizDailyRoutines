using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KamiToolKit.Nodes;

namespace OmenTools.KamiToolKit.Nodes.Collasping;

public class CollaspingNode : ResNode
{
	private readonly ResNode childContainer;

	private readonly List<CollaspingCategoryNode> children = new List<CollaspingCategoryNode>();

	public float CategoryVerticalSpacing { get; set; } = 4f;

	public Action<float>? OnLayoutUpdate { get; set; }

	public ReadOnlyCollection<CollaspingCategoryNode> CategoryNodes => children.AsReadOnly();

	public void AddCategoryNode(CollaspingCategoryNode node)
	{
		RefreshLayout();
		children.Add(node);
		node.Width = childContainer.Width;
		node.Y = childContainer.Height;
		node.AttachNode(childContainer);
		node.ParentTreeListNode = this;
		childContainer.Height += node.Height + CategoryVerticalSpacing;
	}

	public void RefreshLayout()
	{
		childContainer.Height = 0f;
		foreach (CollaspingCategoryNode child in children)
		{
			if (child.IsVisible)
			{
				child.Y = childContainer.Height;
				childContainer.Height += child.Height + CategoryVerticalSpacing;
			}
		}
		OnLayoutUpdate?.Invoke(childContainer.Height);
	}

	public CollaspingNode()
	{
		childContainer = new ResNode();
		childContainer.AttachNode(this);
	}

	protected override void OnSizeChanged()
	{
		base.OnSizeChanged();
		childContainer.Width = Width;
	}
}
