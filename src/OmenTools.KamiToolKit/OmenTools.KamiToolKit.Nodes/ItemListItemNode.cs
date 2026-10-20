using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;

namespace OmenTools.KamiToolKit.Nodes;

public class ItemListItemNode : ListItemNode<Item>, IListItemNode
{
	protected readonly IconImageNode IconNode;

	protected readonly TextNode LabelTextNode;

	protected readonly TextNode SubLabelTextNode;

	protected readonly TextNode IDTextNode;

	public static float ItemHeight => 32f;

	public ItemListItemNode()
	{
		IconNode = new IconImageNode
		{
			FitTexture = true,
			IconId = 60072u
		};
		IconNode.AttachNode(this);
		LabelTextNode = new TextNode
		{
			TextFlags = (TextFlags)1024,
			FontSize = 14u,
			LineSpacing = 14u,
			AlignmentType = (AlignmentType)6,
			TextColor = ColorHelper.GetColor(8u)
		};
		LabelTextNode.AttachNode(this);
		SubLabelTextNode = new TextNode
		{
			TextFlags = (TextFlags)1024,
			FontSize = 12u,
			LineSpacing = 12u,
			AlignmentType = (AlignmentType)0,
			TextColor = ColorHelper.GetColor(3u)
		};
		SubLabelTextNode.AttachNode(this);
		IDTextNode = new TextNode
		{
			TextFlags = (TextFlags)32,
			FontSize = 10u,
			AlignmentType = (AlignmentType)8,
			TextColor = ColorHelper.GetColor(3u)
		};
		IDTextNode.AttachNode(this);
		base.ShowClickableCursor = true;
	}

	protected override void OnSizeChanged()
	{
		base.OnSizeChanged();
		IconNode.Size = new Vector2(Height - 4f, Height - 4f);
		IconNode.Position = new Vector2(2f, 2f);
		LabelTextNode.Size = new Vector2(Width - Height - 2f - 30f, Height / 2f);
		LabelTextNode.Position = new Vector2(Height + 2f, 0f);
		SubLabelTextNode.Size = new Vector2(Width - Height - 2f - 10f, Height / 2f);
		SubLabelTextNode.Position = new Vector2(Height + 2f + 10f, Height / 2f);
		IDTextNode.Size = new Vector2(30f, Height / 2f);
		IDTextNode.Position = new Vector2(Width - 30f, 0f);
	}

	protected override void SetNodeData(Item itemData)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (((Item)(ref itemData)).RowId != 0)
		{
			IconNode.IconId = ((Item)(ref itemData)).Icon;
			LabelTextNode.String = ReadOnlySeString.op_Implicit(((object)((Item)(ref itemData)).Name/*cast due to constrained. prefix*/).ToString());
			TextNode subLabelTextNode = SubLabelTextNode;
			ItemSearchCategory? valueNullable = ((Item)(ref itemData)).ItemSearchCategory.ValueNullable;
			object obj;
			if (!valueNullable.HasValue)
			{
				obj = null;
			}
			else
			{
				ItemSearchCategory valueOrDefault = valueNullable.GetValueOrDefault();
				obj = ((object)((ItemSearchCategory)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj == null)
			{
				obj = string.Empty;
			}
			subLabelTextNode.String = ReadOnlySeString.op_Implicit((string)obj);
			ItemTooltip = ((Item)(ref itemData)).RowId;
		}
	}
}
