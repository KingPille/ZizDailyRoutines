using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Utility;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.UiOverlay;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoPreviewColorsInDye : ModuleBase
{
	private sealed class DyePreviewOverlayNode : OverlayNode
	{
		private readonly Func<DyeInfo?> getCurrentDye;

		private readonly Func<bool> isTooltipVisible;

		private readonly Func<bool> isConflictKeyPressed;

		private readonly Func<string> getConflictKeyString;

		private readonly Func<bool> isPinned;

		private readonly Action<DyeInfo> onPinRequest;

		private readonly SimpleNineGridNode backgroundNode = new SimpleNineGridNode
		{
			TexturePath = "ui/uld/EnemyList_hr1.tex",
			TextureCoordinates = new Vector2(96f, 80f),
			TextureSize = new Vector2(24f, 20f),
			Offsets = new Vector4(8f),
			MultiplyColor = new Vector3(0f),
			Alpha = 1f
		};

		private readonly TextNode titleNode = new TextNode
		{
			Position = new Vector2(18f, 12f),
			Size = new Vector2(560f, 34f),
			FontSize = 22u,
			TextFlags = (TextFlags)8,
			TextColor = Vector4.One,
			TextOutlineColor = VectorExtensions.WithW(Vector4.Zero, 1f),
			AlignmentType = (AlignmentType)0
		};

		private readonly TextNode countNode = new TextNode
		{
			Position = new Vector2(18f, 46f),
			Size = new Vector2(560f, 26f),
			FontSize = 16u,
			TextFlags = (TextFlags)8,
			TextColor = Vector4.One,
			TextOutlineColor = VectorExtensions.WithW(Vector4.Zero, 1f),
			AlignmentType = (AlignmentType)0
		};

		private readonly List<NodeBase> dynamicNodes = new List<NodeBase>();

		private DyeInfo? lastDye;

		private bool wasConflictKeyPressed;

		public override OverlayLayer OverlayLayer => OverlayLayer.Foreground;

		public override bool HideWithNativeUi => false;

		public override bool HideWithUiToggled => false;

		public DyePreviewOverlayNode(Func<DyeInfo?> getCurrentDye, Func<bool> isTooltipVisible, Func<bool> isConflictKeyPressed, Func<string> getConflictKeyString, Func<bool> isPinned, Action<DyeInfo> onPinRequest)
		{
			this.getCurrentDye = getCurrentDye;
			this.isTooltipVisible = isTooltipVisible;
			this.isConflictKeyPressed = isConflictKeyPressed;
			this.getConflictKeyString = getConflictKeyString;
			this.isPinned = isPinned;
			this.onPinRequest = onPinRequest;
			Size = new Vector2(600f, 160f);
			backgroundNode.Position = new Vector2(-24f, -24f);
			backgroundNode.Size = Size + new Vector2(48f);
			backgroundNode.AttachNode(this);
			titleNode.AttachNode(this);
			countNode.AttachNode(this);
		}

		protected override void OnUpdate()
		{
			DyeInfo dyeInfo = getCurrentDye();
			bool flag = isConflictKeyPressed();
			if (flag && !wasConflictKeyPressed && !isPinned() && dyeInfo != null && isTooltipVisible())
			{
				onPinRequest(dyeInfo);
			}
			wasConflictKeyPressed = flag;
			if (isPinned() || dyeInfo == null || !isTooltipVisible())
			{
				IsVisible = false;
				lastDye = null;
				ClearDynamicNodes();
				return;
			}
			IsVisible = true;
			Position = GetOverlayPosition();
			if ((object)lastDye != dyeInfo)
			{
				RebuildNormal(dyeInfo);
				lastDye = dyeInfo;
			}
		}

		private Vector2 GetOverlayPosition()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			Vector2 mousePos = ImGui.GetMousePos();
			ImGuiIOPtr iO = ImGui.GetIO();
			Vector2 displaySize = ((ImGuiIOPtr)(ref iO)).DisplaySize;
			Vector2 value = mousePos + new Vector2(36f, 24f);
			if (value.X + Size.X > displaySize.X)
			{
				value.X = mousePos.X - Size.X - 36f;
			}
			if (value.Y + Size.Y > displaySize.Y)
			{
				value.Y = displaySize.Y - Size.Y - 20f;
			}
			return Vector2.Max(value, new Vector2(20f));
		}

		private void UpdateBackgroundSize()
		{
			backgroundNode.Position = new Vector2(-24f, -24f);
			backgroundNode.Size = Size + new Vector2(48f);
		}

		private void RebuildNormal(DyeInfo dye)
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			ClearDynamicNodes();
			titleNode.Position = new Vector2(18f, 12f);
			titleNode.FontSize = 22u;
			titleNode.String = ReadOnlySeString.op_Implicit("★ " + LuminaWrapper.GetItemName(dye.ItemID));
			titleNode.TextColor = dye.HighlightColor;
			titleNode.IsVisible = true;
			countNode.Position = new Vector2(18f, 46f);
			countNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoPreviewColorsInDye-AvailableColors", dye.StainIDs.Length));
			countNode.IsVisible = true;
			int num = dye.StainIDs.Length;
			bool num2 = num > 40;
			int num3 = (num2 ? 5 : 4);
			float num4 = (num2 ? 112f : 132f);
			float num5 = (num2 ? 36f : 40f);
			int squareSize = (num2 ? 24 : 28);
			int nameSize = (num2 ? 14 : 16);
			for (int i = 0; i < num; i++)
			{
				AddDisplayStainNode(dye.StainIDs[i], i, num3, num4, num5, squareSize, nameSize);
			}
			int num6 = (int)MathF.Ceiling((float)num / (float)num3);
			float num7 = 104f + (float)num6 * num5;
			float x = (float)num3 * num4 + 36f;
			TextNode textNode = new TextNode();
			textNode.Position = new Vector2(0f, num7);
			textNode.Size = new Vector2(x, 22f);
			textNode.FontSize = 13u;
			textNode.TextFlags = (TextFlags)8;
			textNode.TextColor = ColorHelper.GetColor(7u);
			textNode.TextOutlineColor = ColorHelper.GetColor(28u);
			textNode.AlignmentType = (AlignmentType)4;
			textNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoPreviewColorsInDye-Hint", getConflictKeyString()));
			TextNode textNode2 = textNode;
			textNode2.AttachNode(this);
			dynamicNodes.Add(textNode2);
			Size = new Vector2(x, num7 + 28f);
			UpdateBackgroundSize();
		}

		private void AddDisplayStainNode(uint stainID, int index, int perRow, float cellWidth, float cellHeight, int squareSize, int nameSize)
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<Stain>(stainID, out Stain item))
			{
				float num = 18f + (float)(index % perRow) * cellWidth;
				float num2 = 84f + (float)(index / perRow) * cellHeight;
				Vector4 textColor = ((Stain)(ref item)).Color.ReverseToVector4();
				TextNode textNode = new TextNode
				{
					Position = new Vector2(num, num2),
					Size = new Vector2(squareSize + 6, squareSize + 6),
					FontSize = (byte)squareSize,
					TextColor = textColor,
					AlignmentType = (AlignmentType)0,
					String = ReadOnlySeString.op_Implicit("■")
				};
				textNode.AttachNode(this);
				dynamicNodes.Add(textNode);
				TextNode obj = new TextNode
				{
					Position = new Vector2(num + (float)squareSize + 8f, num2 + 5f),
					Size = new Vector2(cellWidth - (float)squareSize - 10f, cellHeight),
					FontSize = (byte)nameSize,
					TextFlags = (TextFlags)8,
					TextColor = Vector4.One,
					TextOutlineColor = VectorExtensions.WithW(Vector4.Zero, 1f),
					AlignmentType = (AlignmentType)0
				};
				ReadOnlySeString name = ((Stain)(ref item)).Name;
				obj.String = ReadOnlySeString.op_Implicit(((ReadOnlySeString)(ref name)).ExtractText());
				TextNode textNode2 = obj;
				textNode2.AttachNode(this);
				dynamicNodes.Add(textNode2);
			}
		}

		private void ClearDynamicNodes()
		{
			foreach (NodeBase dynamicNode in dynamicNodes)
			{
				dynamicNode.Dispose();
			}
			dynamicNodes.Clear();
		}
	}

	private sealed class DyePinnedAddon : NativeAddon
	{
		private readonly DyeInfo dye;

		private readonly Func<bool> isConflictKeyPressed;

		private readonly Action onClose;

		private readonly List<NodeBase> dynamicNodes = new List<NodeBase>();

		private int selectedStainIndex = -1;

		private int lastSelectedStainIndex = -1;

		private bool wasConflictKeyPressed = true;

		private int frameCount;

		private bool isClosing;

		public Vector2 OpenPosition { get; init; }

		public DyePinnedAddon(DyeInfo dye, Func<bool> isConflictKeyPressed, Action onClose)
		{
			this.dye = dye;
			this.isConflictKeyPressed = isConflictKeyPressed;
			this.onClose = onClose;
			base.RememberClosePosition = false;
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			RebuildGrid();
		}

		protected unsafe override void OnShow(AtkUnitBase* addon)
		{
			SetWindowPosition(OpenPosition);
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			frameCount++;
			if (frameCount < 5)
			{
				wasConflictKeyPressed = isConflictKeyPressed();
				return;
			}
			bool flag = isConflictKeyPressed();
			if (flag && !wasConflictKeyPressed)
			{
				Close();
				return;
			}
			wasConflictKeyPressed = flag;
			if (!(base.WindowNode as WindowNode).BorderTextureNode.IsVisible)
			{
				Close();
			}
			else if (lastSelectedStainIndex != selectedStainIndex)
			{
				if (selectedStainIndex >= 0)
				{
					RebuildDetail();
				}
				else
				{
					RebuildGrid();
				}
				lastSelectedStainIndex = selectedStainIndex;
			}
		}

		protected unsafe override void OnHide(AtkUnitBase* addon)
		{
			if (!isClosing)
			{
				isClosing = true;
				onClose();
			}
		}

		private void RebuildGrid()
		{
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			ClearDynamicNodes();
			base.WindowNode?.SetTitle("★ " + LuminaWrapper.GetItemName(dye.ItemID));
			int num = dye.StainIDs.Length;
			bool num2 = num > 40;
			int num3 = (num2 ? 5 : 4);
			float num4 = (num2 ? 112f : 132f);
			float num5 = (num2 ? 36f : 40f);
			int squareSize = (num2 ? 24 : 28);
			int nameSize = (num2 ? 14 : 16);
			Vector2 contentStartPosition = base.ContentStartPosition;
			float num6 = (float)num3 * num4;
			TextNode textNode = new TextNode
			{
				Position = contentStartPosition,
				Size = new Vector2(num6, 24f),
				FontSize = 16u,
				TextFlags = (TextFlags)8,
				TextColor = Vector4.One,
				TextOutlineColor = VectorExtensions.WithW(Vector4.Zero, 1f),
				AlignmentType = (AlignmentType)0,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoPreviewColorsInDye-ClickToViewDetail"))
			};
			textNode.AttachNode(this);
			dynamicNodes.Add(textNode);
			VerticalListNode verticalListNode = new VerticalListNode();
			Vector2 position = contentStartPosition;
			position.Y = contentStartPosition.Y + 28f;
			verticalListNode.Position = position;
			verticalListNode.Size = new Vector2(num6, 0f);
			verticalListNode.FitContents = true;
			verticalListNode.ItemSpacing = 4f;
			VerticalListNode verticalListNode2 = verticalListNode;
			verticalListNode2.AttachNode(this);
			dynamicNodes.Add(verticalListNode2);
			int num7 = (int)Math.Ceiling((float)num / (float)num3);
			for (int i = 0; i < num7; i++)
			{
				HorizontalListNode horizontalListNode = new HorizontalListNode
				{
					Size = new Vector2(num6, num5)
				};
				for (int j = 0; j < num3 && i * num3 + j < num; j++)
				{
					int num8 = i * num3 + j;
					AddInteractiveStainNode(horizontalListNode, dye.StainIDs[num8], num8, num4, num5, squareSize, nameSize);
				}
				verticalListNode2.AddNode(horizontalListNode);
			}
			float x = num6 + contentStartPosition.X + 12f;
			float y = contentStartPosition.Y + 36f + verticalListNode2.Height + 12f;
			SetWindowSize(new Vector2(x, y));
			contentStartPosition = (textNode.Position = base.ContentStartPosition);
			position = contentStartPosition;
			position.Y = contentStartPosition.Y + 28f;
			verticalListNode2.Position = position;
			verticalListNode2.RecalculateLayout();
		}

		private void RebuildDetail()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			ClearDynamicNodes();
			if (LuminaGetter.TryGetRow<Stain>(dye.StainIDs[selectedStainIndex], out Stain item))
			{
				ReadOnlySeString name = ((Stain)(ref item)).Name;
				string text = ((ReadOnlySeString)(ref name)).ExtractText();
				Vector4 color = ((Stain)(ref item)).Color.ReverseToVector4();
				byte value = (byte)(color.X * 255f);
				byte value2 = (byte)(color.Y * 255f);
				byte value3 = (byte)(color.Z * 255f);
				string text2 = $"({color.X:F2}, {color.Y:F2}, {color.Z:F2}, {color.W:F2})";
				uint value4 = color.ToUInt();
				string text3 = $"{value4}";
				string text4 = $"#{value:X2}{value2:X2}{value3:X2}FF";
				base.WindowNode?.SetTitle(LanguageManager.Get("AutoPreviewColorsInDye-ColorDetail"));
				Vector2 contentStartPosition = base.ContentStartPosition;
				float num = 360f - contentStartPosition.X - 12f;
				ResNode resNode = new ResNode
				{
					Position = contentStartPosition,
					Size = new Vector2(num, 0f)
				};
				resNode.AttachNode(this);
				dynamicNodes.Add(resNode);
				float num2 = 0f;
				CircleButtonNode circleButtonNode = new CircleButtonNode
				{
					Icon = CircleButtonIcon.LeftArrow,
					Size = new Vector2(24f, 24f),
					Position = new Vector2(0f, num2),
					OnClick = delegate
					{
						selectedStainIndex = -1;
					}
				};
				circleButtonNode.AttachNode(resNode);
				dynamicNodes.Add(circleButtonNode);
				TextNode textNode = new TextNode
				{
					Position = new Vector2(32f, num2),
					Size = new Vector2(num - 32f, 24f),
					FontSize = 20u,
					TextColor = color,
					TextOutlineColor = VectorExtensions.WithW(Vector4.Zero, 1f),
					AlignmentType = (AlignmentType)3,
					String = ReadOnlySeString.op_Implicit(text)
				};
				textNode.AttachNode(resNode);
				dynamicNodes.Add(textNode);
				num2 += 36f;
				ColorImageNode colorImageNode = new ColorImageNode
				{
					Color = new Vector4(0.85f, 0.85f, 0.85f, 1f),
					Position = new Vector2(0f, num2),
					Size = new Vector2(126f, 126f)
				};
				colorImageNode.AttachNode(resNode);
				dynamicNodes.Add(colorImageNode);
				ColorImageNode colorImageNode2 = new ColorImageNode
				{
					Color = color,
					Position = new Vector2(3f, num2 + 3f),
					Size = new Vector2(120f, 120f)
				};
				colorImageNode2.AttachNode(resNode);
				dynamicNodes.Add(colorImageNode2);
				num2 += 146f;
				num2 = AddDetailRow(resNode, num2, num, 30f, LanguageManager.Get("Name"), text, text);
				num2 += 6f;
				num2 = AddDetailRow(resNode, num2, num, 30f, "Vector4", text2, text2);
				num2 += 6f;
				num2 = AddDetailRow(resNode, num2, num, 30f, "UInt", text3, text3);
				num2 += 6f;
				num2 = AddDetailRow(resNode, num2, num, 30f, "HEX", text4, text4);
				float y = contentStartPosition.Y + num2 + 12f + 8f;
				SetWindowSize(new Vector2(360f, y));
				contentStartPosition = base.ContentStartPosition;
				resNode.Position = contentStartPosition;
			}
		}

		private float AddDetailRow(ResNode container, float y, float width, float height, string label, string value, string copyText)
		{
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			ResNode resNode = new ResNode
			{
				Position = new Vector2(0f, y),
				Size = new Vector2(width, height),
				NodeFlags = (NodeFlags)9008
			};
			resNode.AttachNode(container);
			dynamicNodes.Add(resNode);
			SimpleNineGridNode hoverBg = new SimpleNineGridNode
			{
				TexturePath = "ui/uld/ListItemA.tex",
				TextureCoordinates = new Vector2(0f, 22f),
				TextureSize = new Vector2(64f, 22f),
				Position = new Vector2(0f, 4f),
				TopOffset = 4f,
				BottomOffset = 4f,
				LeftOffset = 8f,
				RightOffset = 8f,
				Size = resNode.Size,
				IsVisible = false
			};
			hoverBg.AttachNode(resNode);
			dynamicNodes.Add(hoverBg);
			TextNode textNode = new TextNode
			{
				Position = new Vector2(10f, 5f),
				Size = new Vector2(90f, height),
				FontSize = 14u,
				TextColor = ColorHelper.GetColor(2u),
				AlignmentType = (AlignmentType)3,
				String = ReadOnlySeString.op_Implicit(label)
			};
			textNode.AttachNode(resNode);
			dynamicNodes.Add(textNode);
			TextNode textNode2 = new TextNode
			{
				Position = new Vector2(112f, 5f),
				Size = new Vector2(width - 122f, height),
				FontSize = 14u,
				TextFlags = (TextFlags)8,
				AlignmentType = (AlignmentType)3,
				String = ReadOnlySeString.op_Implicit(value)
			};
			AtkColors.Label.ApplyTo(textNode2);
			textNode2.AttachNode(resNode);
			dynamicNodes.Add(textNode2);
			string capturedText = copyText;
			resNode.AddEvent((AtkEventType)3, (Action)delegate
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				ImGui.SetClipboardText(ImU8String.op_Implicit(capturedText));
				OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("CopiedToClipboard") + ": " + capturedText);
			});
			resNode.AddEvent((AtkEventType)6, (Action)delegate
			{
				hoverBg.IsVisible = true;
			});
			resNode.AddEvent((AtkEventType)7, (Action)delegate
			{
				hoverBg.IsVisible = false;
			});
			resNode.AddDrawFlags(DrawFlags.ClickableCursor);
			return y + height;
		}

		private void AddInteractiveStainNode(HorizontalListNode rowList, uint stainID, int index, float cellWidth, float cellHeight, int squareSize, int nameSize)
		{
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<Stain>(stainID, out Stain item))
			{
				Vector4 textColor = ((Stain)(ref item)).Color.ReverseToVector4();
				ResNode resNode = new ResNode
				{
					Size = new Vector2(cellWidth, cellHeight),
					NodeFlags = (NodeFlags)9008
				};
				SimpleNineGridNode hoverBg = new SimpleNineGridNode
				{
					TexturePath = "ui/uld/ListItemA.tex",
					TextureCoordinates = new Vector2(0f, 22f),
					TextureSize = new Vector2(64f, 22f),
					TopOffset = 6f,
					BottomOffset = 6f,
					LeftOffset = 16f,
					RightOffset = 1f,
					Size = resNode.Size,
					IsVisible = false
				};
				hoverBg.AttachNode(resNode);
				TextNode textNode = new TextNode();
				textNode.Position = new Vector2(4f, 4f);
				textNode.Size = new Vector2(squareSize + 6, squareSize + 6);
				textNode.FontSize = (byte)squareSize;
				textNode.TextColor = textColor;
				textNode.AlignmentType = (AlignmentType)0;
				textNode.String = ReadOnlySeString.op_Implicit("■");
				textNode.AttachNode(resNode);
				TextNode obj = new TextNode
				{
					Position = new Vector2(squareSize + 12, 9f),
					Size = new Vector2(cellWidth - (float)squareSize - 14f, cellHeight),
					FontSize = (byte)nameSize,
					TextFlags = (TextFlags)8,
					TextColor = Vector4.One,
					TextOutlineColor = VectorExtensions.WithW(Vector4.Zero, 1f),
					AlignmentType = (AlignmentType)0
				};
				ReadOnlySeString name = ((Stain)(ref item)).Name;
				obj.String = ReadOnlySeString.op_Implicit(((ReadOnlySeString)(ref name)).ExtractText());
				obj.AttachNode(resNode);
				int capturedIndex = index;
				resNode.AddEvent((AtkEventType)3, (Action)delegate
				{
					selectedStainIndex = capturedIndex;
				});
				resNode.AddEvent((AtkEventType)6, (Action)delegate
				{
					hoverBg.IsVisible = true;
				});
				resNode.AddEvent((AtkEventType)7, (Action)delegate
				{
					hoverBg.IsVisible = false;
				});
				resNode.AddDrawFlags(DrawFlags.ClickableCursor);
				rowList.AddNode(resNode);
			}
		}

		private void ClearDynamicNodes()
		{
			foreach (NodeBase dynamicNode in dynamicNodes)
			{
				if (dynamicNode is LayoutListNode layoutListNode)
				{
					layoutListNode.Clear();
				}
				dynamicNode.Dispose();
			}
			dynamicNodes.Clear();
		}
	}

	private sealed record DyeInfo(uint ItemID, Vector4 HighlightColor, uint[] StainIDs);

	private DyeInfo? currentDye;

	private OverlayController? overlayController;

	private DyePinnedAddon? pinnedAddon;

	private const float OVERLAY_BG_PADDING = 24f;

	private const float OVERLAY_PADDING_X = 18f;

	private const float OVERLAY_TITLE_Y = 12f;

	private const float OVERLAY_COUNT_Y = 46f;

	private const float OVERLAY_FIRST_ROW_Y = 84f;

	private const float OVERLAY_BASE_HEIGHT = 104f;

	private const float WINDOW_PADDING = 12f;

	private const float GRID_ROW_SPACING = 4f;

	private const int COMPACT_THRESHOLD = 40;

	private const int NORMAL_PER_ROW = 4;

	private const int COMPACT_PER_ROW = 5;

	private const int NORMAL_SQUARE_SIZE = 28;

	private const int COMPACT_SQUARE_SIZE = 24;

	private const int NORMAL_NAME_SIZE = 16;

	private const int COMPACT_NAME_SIZE = 14;

	private const float NORMAL_CELL_WIDTH = 132f;

	private const float COMPACT_CELL_WIDTH = 112f;

	private const float NORMAL_CELL_HEIGHT = 40f;

	private const float COMPACT_CELL_HEIGHT = 36f;

	private const float DETAIL_WIDTH = 360f;

	private static readonly FrozenDictionary<uint, DyeInfo> Dyes = new Dictionary<uint, DyeInfo>
	{
		[52254u] = new DyeInfo(52254u, new Vector4(1f, 0.95f, 0.65f, 1f), (from x in Enumerable.Range(1, 85)
			select (uint)x).ToArray()),
		[52255u] = new DyeInfo(52255u, new Vector4(1f, 0.25f, 0.25f, 1f), new uint[9] { 86u, 87u, 88u, 89u, 90u, 91u, 92u, 93u, 94u }),
		[52256u] = new DyeInfo(52256u, new Vector4(0.25f, 0.45f, 1f, 1f), new uint[11]
		{
			95u, 96u, 97u, 98u, 99u, 100u, 121u, 122u, 123u, 124u,
			125u
		})
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoPreviewColorsInDyeTitle"),
		Description = LanguageManager.Get("AutoPreviewColorsInDyeDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "ErxCharlotte" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		OmenServiceBase<TooltipManager>.Instance().RegItem(OnItemTooltip);
		overlayController = new OverlayController();
		overlayController.AddNode(new DyePreviewOverlayNode(() => currentDye, IsCurrentTooltipVisible, () => PluginConfig.Instance().ConflictKeyBinding.IsPressed(), () => PluginConfig.Instance().ConflictKeyBinding.ToString(), () => pinnedAddon != null, OpenPinnedAddon));
	}

	protected override void Uninit()
	{
		OmenServiceBase<TooltipManager>.Instance().Unreg(OnItemTooltip);
		pinnedAddon?.Dispose();
		pinnedAddon = null;
		overlayController?.Dispose();
		overlayController = null;
		currentDye = null;
	}

	private void OnItemTooltip(ItemKind itemKind, uint itemID, ref List<TooltipItemModification> modifications)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if ((int)itemKind != 0 || !Dyes.TryGetValue(itemID, out DyeInfo value))
		{
			currentDye = null;
		}
		else
		{
			currentDye = value;
		}
	}

	private unsafe bool IsCurrentTooltipVisible()
	{
		if (currentDye != null)
		{
			return (*Addons.ItemDetail).IsAddonAndNodesReady();
		}
		return false;
	}

	private void OpenPinnedAddon(DyeInfo dye)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		pinnedAddon?.Dispose();
		int num = dye.StainIDs.Length;
		bool num2 = num > 40;
		int num3 = (num2 ? 5 : 4);
		float num4 = (num2 ? 112f : 132f);
		float num5 = (num2 ? 36f : 40f);
		int num6 = (int)MathF.Ceiling((float)num / (float)num3);
		Vector2 size = new Vector2((float)num3 * num4 + 24f, 74f + (float)num6 * num5 + (float)(num6 - 1) * 4f + 12f);
		pinnedAddon = new DyePinnedAddon(dye, () => PluginConfig.Instance().ConflictKeyBinding.IsPressed(), delegate
		{
			pinnedAddon = null;
		})
		{
			InternalName = "DRDyePreviewPinned",
			Title = ReadOnlySeString.op_Implicit("★ " + LuminaWrapper.GetItemName(dye.ItemID)),
			Size = size
		};
		pinnedAddon.Open();
	}
}
