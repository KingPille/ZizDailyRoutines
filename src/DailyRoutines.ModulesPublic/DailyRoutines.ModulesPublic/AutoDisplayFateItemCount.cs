using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.UiOverlay;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic;

public class AutoDisplayFateItemCount : ModuleBase
{
	private class FateInfoNode : OverlayNode
	{
		public override OverlayLayer OverlayLayer => OverlayLayer.Foreground;

		public override bool HideWithNativeUi => false;

		private GridNode TableNode { get; set; }

		private HorizontalListNode HeaderNode { get; set; }

		private IconImageNode IconNode { get; set; }

		private TextNode NameNode { get; set; }

		private TextNode HoldLabelNode { get; set; }

		private TextNode HoldCountNode { get; set; }

		private TextNode HandInLabelNode { get; set; }

		private TextNode HandInCountNode { get; set; }

		public FateInfoNode()
		{
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			Scale = new Vector2(1.5f);
			Size = new Vector2(200f, 76f);
			HeaderNode = new HorizontalListNode
			{
				Size = new Vector2(200f, 36f),
				IsVisible = false
			};
			IconNode = new IconImageNode
			{
				Size = new Vector2(32f),
				IconId = 60498u,
				FitTexture = true,
				IsVisible = true
			};
			HeaderNode.AddNode(IconNode);
			HeaderNode.AddDummy(3f);
			NameNode = new TextNode
			{
				Size = new Vector2(160f, 64f),
				String = ReadOnlySeString.op_Implicit("测试物品"),
				FontSize = 20u,
				Position = new Vector2(2f),
				TextFlags = (TextFlags)8,
				AlignmentType = (AlignmentType)0,
				TextColor = ColorHelper.GetColor(50u),
				TextOutlineColor = ColorHelper.GetColor(30u)
			};
			HeaderNode.AddNode(NameNode);
			HeaderNode.AttachNode(this);
			TableNode = new GridNode
			{
				Position = new Vector2(0f, 40f),
				Size = new Vector2(100f, 28f) * 2f,
				GridSize = new GridSize(2, 2)
			};
			HoldLabelNode = new TextNode
			{
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(358u)),
				FontSize = 14u,
				TextFlags = (TextFlags)8,
				TextOutlineColor = ColorHelper.GetColor(37u),
				TextColor = ColorHelper.GetColor(50u)
			};
			HoldLabelNode.AttachNode(TableNode[0, 0]);
			HoldCountNode = new TextNode
			{
				String = ReadOnlySeString.op_Implicit("0"),
				FontSize = 18u,
				TextFlags = (TextFlags)8,
				FontType = (FontType)2,
				TextOutlineColor = ColorHelper.GetColor(30u),
				TextColor = ColorHelper.GetColor(50u)
			};
			HoldCountNode.AttachNode(TableNode[0, 1]);
			HandInLabelNode = new TextNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoDisplayFateItemCount-HandInCount")),
				FontSize = 14u,
				TextFlags = (TextFlags)8,
				TextOutlineColor = ColorHelper.GetColor(37u),
				TextColor = ColorHelper.GetColor(50u)
			};
			HandInLabelNode.AttachNode(TableNode[1, 0]);
			HandInCountNode = new TextNode
			{
				String = ReadOnlySeString.op_Implicit("0"),
				FontSize = 18u,
				TextFlags = (TextFlags)8,
				FontType = (FontType)2,
				TextOutlineColor = ColorHelper.GetColor(30u),
				TextColor = ColorHelper.GetColor(50u)
			};
			HandInCountNode.AttachNode(TableNode[1, 1]);
			TableNode.AttachNode(this);
		}

		protected unsafe override void OnUpdate()
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			FateContext* currentFate = ((FateManager)FateManager.Instance()).CurrentFate;
			if (currentFate != null && LuminaGetter.TryGetRow<Fate>((uint)((FateContext)currentFate).FateId, out Fate item) && ((Fate)(ref item)).EventItem.RowId != 0)
			{
				EventItem value = ((Fate)(ref item)).EventItem.Value;
				if (((EventItem)(ref value)).Icon != 0 && (*Addons.ToDoList).IsAddonAndNodesReady())
				{
					IsVisible = true;
					AtkResNode* ptr = null;
					AtkComponentNode* ptr2 = null;
					AtkComponentNode* ptr3 = null;
					for (int i = 0; i < ((AtkUldManager)(&((AtkUnitBase)Addons.ToDoList).UldManager)).NodeListCount; i++)
					{
						AtkResNode* ptr4 = ((AtkUldManager)(&((AtkUnitBase)Addons.ToDoList).UldManager)).NodeList[i];
						if (ptr4 != null)
						{
							if (((AtkResNode)ptr4).NodeId == 23001)
							{
								ptr = ptr4;
							}
							if (((AtkResNode)ptr4).NodeId == 63101)
							{
								ptr2 = (AtkComponentNode*)ptr4;
							}
							if (((AtkResNode)ptr4).NodeId == 113001)
							{
								ptr3 = (AtkComponentNode*)ptr4;
							}
						}
					}
					if (ptr != null && ptr2 != null && ptr3 != null)
					{
						NodeState nodeState = (*((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr3).Component).UldManager)).SearchNodeById(4u)).GetNodeState();
						NodeState nodeState2 = (*ptr3).GetNodeState();
						NodeState nodeState3 = (*ptr2).GetNodeState();
						NodeState nodeState4 = (*ptr).GetNodeState();
						Position = nodeState.TopLeft + new Vector2(0f, nodeState2.Height + nodeState3.Height + nodeState4.Height + 12f);
						UpdateFateItemHeader(((Fate)(ref item)).EventItem.Value);
						HoldCountNode.SetNumber((int)LocalPlayerState.GetItemCount(((Fate)(ref item)).EventItem.RowId));
						HandInCountNode.SetNumber(((FateContext)currentFate).HandInCount);
					}
					return;
				}
			}
			IsVisible = false;
		}

		private void UpdateFateItemHeader(EventItem item)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			HeaderNode.IsVisible = true;
			IconNode.IconId = ((EventItem)(ref item)).Icon;
			NameNode.String = ReadOnlySeString.op_Implicit($"{((EventItem)(ref item)).Singular}");
			while (NameNode.FontSize > 1 && NameNode.GetTextDrawSize(NameNode.String).X > NameNode.Size.X)
			{
				NameNode.FontSize--;
			}
		}
	}

	private OverlayController? controller;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayFateItemCountTitle"),
		Description = LanguageManager.Get("AutoDisplayFateItemCountDescription"),
		Category = ModuleCategory.Combat,
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/AtmoOmen/StaticAssets/main/DailyRoutines/image/AutoDisplayFateItemCount-UI.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		if (controller == null)
		{
			controller = new OverlayController();
		}
		controller.AddNode(new FateInfoNode());
	}

	protected override void Uninit()
	{
		controller?.Dispose();
		controller = null;
	}
}
