using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Toast;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Controllers;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.MapOverlay;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Nodes;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface.CustomizeMapMarker;

public class CustomizeMapMarker : ModuleBase
{
	private sealed class Config : ModuleConfig
	{
		public List<MarkerRecord> Markers { get; set; } = new List<MarkerRecord>();
	}

	private sealed class MarkerRecord
	{
		public Guid ID { get; set; } = Guid.NewGuid();

		public uint TerritoryID { get; set; }

		public uint MapID { get; set; }

		public Vector2 TexturePosition { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Group { get; set; } = LanguageManager.Get("CustomizeMapMarker-DefaultGroup");

		public string Description { get; set; } = string.Empty;

		public uint IconID { get; set; } = 60561u;

		public float Scale { get; set; } = 1f;

		public bool AutoSetFlag { get; set; }

		public string ExtraCommands { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;

		public MarkerRecord Clone()
		{
			return (MarkerRecord)MemberwiseClone();
		}
	}

	private sealed class MarkerPackage
	{
		public int Version { get; set; } = 1;

		public List<MarkerRecord> Markers { get; set; } = new List<MarkerRecord>();
	}

	private sealed class MarkerDetailsAddon(CustomizeMapMarker module) : NativeAddon
	{
		private Guid markerID;

		private VerticalListNode? containerNode;

		private LabelTextNode? locationText;

		private TextInputNode? nameInput;

		private TextInputNode? groupInput;

		private TextMultiLineInputNode? descriptionInput;

		private NumericInputNode? iconInput;

		private TextInputNode? scaleInput;

		private CheckboxNode? autoSetFlagInput;

		private TextMultiLineInputNode? extraCommandsInput;

		private HoldButtonNode? deleteButton;

		public void OpenMarker(Guid id)
		{
			markerID = id;
			if (base.IsOpen)
			{
				RefreshMarker();
			}
			else
			{
				Open();
			}
		}

		public void RefreshMarker()
		{
			if (base.IsOpen)
			{
				PopulateMarker();
			}
		}

		private void PopulateMarker()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			MarkerRecord markerRecord = module.FindMarker(markerID);
			if (markerRecord == null)
			{
				if (base.IsOpen)
				{
					Close();
				}
				return;
			}
			locationText.String = ReadOnlySeString.op_Implicit(FormatMarkerLocation(markerRecord));
			nameInput.String = ReadOnlySeString.op_Implicit(markerRecord.Name);
			groupInput.String = ReadOnlySeString.op_Implicit(markerRecord.Group);
			descriptionInput.String = ReadOnlySeString.op_Implicit(markerRecord.Description);
			iconInput.Value = (int)markerRecord.IconID;
			scaleInput.String = ReadOnlySeString.op_Implicit(markerRecord.Scale.ToString("0.###", CultureInfo.InvariantCulture));
			autoSetFlagInput.IsChecked = markerRecord.AutoSetFlag;
			extraCommandsInput.String = ReadOnlySeString.op_Implicit(markerRecord.ExtraCommands);
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0379: Unknown result type (might be due to invalid IL or missing references)
			//IL_0473: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			containerNode = new VerticalListNode
			{
				Position = base.ContentStartPosition,
				Size = base.ContentSize,
				ItemSpacing = 6f
			};
			LabelTextNode labelTextNode = new LabelTextNode();
			Vector2 contentSize = base.ContentSize;
			contentSize.Y = 36f;
			labelTextNode.Size = contentSize;
			labelTextNode.String = ReadOnlySeString.op_Implicit("测试");
			labelTextNode.FontSize = 18u;
			labelTextNode.TextFlags = (TextFlags)1032;
			locationText = labelTextNode;
			AtkColors.Label.ApplyTo(locationText);
			containerNode.AddNode(locationText);
			LabelTextNode node = CreateLabel(LanguageManager.Get("Name"));
			containerNode.AddNode(node);
			TextInputNode textInputNode = new TextInputNode();
			contentSize = base.ContentSize;
			contentSize.Y = 30f;
			textInputNode.Size = contentSize;
			textInputNode.MaxCharacters = 80;
			textInputNode.PlaceholderString = LanguageManager.Get("CustomizeMapMarker-Untitled");
			nameInput = textInputNode;
			containerNode.AddNode(nameInput);
			LabelTextNode node2 = CreateLabel(LanguageManager.Get("CustomizeMapMarker-Group"));
			containerNode.AddNode(node2);
			TextInputNode textInputNode2 = new TextInputNode();
			contentSize = base.ContentSize;
			contentSize.Y = 30f;
			textInputNode2.Size = contentSize;
			textInputNode2.MaxCharacters = 40;
			textInputNode2.PlaceholderString = LanguageManager.Get("CustomizeMapMarker-DefaultGroup");
			groupInput = textInputNode2;
			containerNode.AddNode(groupInput);
			LabelTextNode node3 = CreateLabel(LanguageManager.Get("Note"));
			containerNode.AddNode(node3);
			TextMultiLineInputNode textMultiLineInputNode = new TextMultiLineInputNode();
			contentSize = base.ContentSize;
			contentSize.Y = 56f;
			textMultiLineInputNode.Size = contentSize;
			textMultiLineInputNode.MaxCharacters = 160;
			textMultiLineInputNode.MaxLines = 3u;
			textMultiLineInputNode.PlaceholderString = LanguageManager.Get("Note");
			descriptionInput = textMultiLineInputNode;
			containerNode.AddNode(descriptionInput);
			ResNode resNode = new ResNode();
			contentSize = base.ContentSize;
			contentSize.Y = 24f;
			resNode.Size = contentSize;
			ResNode resNode2 = resNode;
			CreateLabel(LanguageManager.Get("CustomizeMapMarker-IconID")).AttachNode(resNode2);
			CircleButtonNode circleButtonNode = new CircleButtonNode();
			circleButtonNode.Icon = CircleButtonIcon.MagnifyingGlass;
			circleButtonNode.Size = new Vector2(24f);
			circleButtonNode.Position = new Vector2(base.ContentSize.X - 24f, 0f);
			circleButtonNode.TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-OpenIconBrowser"));
			circleButtonNode.OnClick = delegate
			{
				OmenServiceBase<ChatManager>.Instance().SendCommand("/xldata icon");
			};
			circleButtonNode.AttachNode(resNode2);
			containerNode.AddNode(resNode2);
			NumericInputNode numericInputNode = new NumericInputNode();
			contentSize = base.ContentSize;
			contentSize.Y = 30f;
			numericInputNode.Size = contentSize;
			numericInputNode.Min = 1;
			numericInputNode.Max = int.MaxValue;
			numericInputNode.Step = 1;
			iconInput = numericInputNode;
			containerNode.AddNode(iconInput);
			LabelTextNode node4 = CreateLabel(LanguageManager.Get("Scale"));
			containerNode.AddNode(node4);
			TextInputNode textInputNode3 = new TextInputNode();
			contentSize = base.ContentSize;
			contentSize.Y = 30f;
			textInputNode3.Size = contentSize;
			textInputNode3.MaxCharacters = 12;
			textInputNode3.PlaceholderString = "1";
			scaleInput = textInputNode3;
			containerNode.AddNode(scaleInput);
			CheckboxNode obj = new CheckboxNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-AutoSetFlag"))
			};
			contentSize = base.ContentSize;
			contentSize.Y = 24f;
			obj.Size = contentSize;
			autoSetFlagInput = obj;
			containerNode.AddNode(autoSetFlagInput);
			LabelTextNode node5 = CreateLabel(LanguageManager.Get("CustomizeMapMarker-ExtraCommands"));
			containerNode.AddNode(node5);
			TextMultiLineInputNode textMultiLineInputNode2 = new TextMultiLineInputNode();
			contentSize = base.ContentSize;
			contentSize.Y = 110f;
			textMultiLineInputNode2.Size = contentSize;
			textMultiLineInputNode2.MaxCharacters = 1000;
			textMultiLineInputNode2.MaxLines = 15u;
			textMultiLineInputNode2.PlaceholderString = LanguageManager.Get("CustomizeMapMarker-ExtraCommands-Placeholder");
			extraCommandsInput = textMultiLineInputNode2;
			containerNode.AddNode(extraCommandsInput);
			HorizontalListNode horizontalListNode = new HorizontalListNode();
			contentSize = base.ContentSize;
			contentSize.Y = 30f;
			horizontalListNode.Size = contentSize;
			horizontalListNode.ItemSpacing = 6f;
			horizontalListNode.Alignment = HorizontalListAnchor.Center;
			HorizontalListNode horizontalListNode2 = horizontalListNode;
			IconButtonNode node6 = new IconButtonNode
			{
				TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-SetFlag")),
				IconId = 60561u,
				Size = new Vector2(30f),
				Position = new Vector2(0f, -2f),
				OnClick = SetFlag
			};
			horizontalListNode2.AddNode(node6);
			horizontalListNode2.AddNode(new TextButtonNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Save")),
				Size = new Vector2(140f, 30f),
				OnClick = SaveMarker
			});
			deleteButton = new HoldButtonNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Delete")),
				Size = new Vector2(140f, 30f),
				OnClick = DeleteMarker
			};
			horizontalListNode2.AddNode(deleteButton);
			horizontalListNode2.RecalculateLayout();
			containerNode.AddNode(horizontalListNode2);
			horizontalListNode2.X = base.ContentSize.X / 2f;
			containerNode.AttachNode(this);
			PopulateMarker();
		}

		protected unsafe override void OnFinalize(AtkUnitBase* addon)
		{
			containerNode = null;
			locationText = null;
			nameInput = null;
			groupInput = null;
			descriptionInput = null;
			iconInput = null;
			scaleInput = null;
			autoSetFlagInput = null;
			extraCommandsInput = null;
			deleteButton = null;
		}

		private static LabelTextNode CreateLabel(string text)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return new LabelTextNode
			{
				String = ReadOnlySeString.op_Implicit(text),
				TextFlags = (TextFlags)1,
				Size = new Vector2(100f, 20f),
				FontSize = 12u,
				AlignmentType = (AlignmentType)3
			};
		}

		private void SetFlag()
		{
			MarkerRecord markerRecord = module.FindMarker(markerID);
			if (markerRecord != null)
			{
				SetGameFlag(markerRecord);
			}
		}

		private void SaveMarker()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Expected O, but got Unknown
			MarkerRecord markerRecord = module.FindMarker(markerID);
			if (markerRecord != null)
			{
				TextInputNode? textInputNode = nameInput;
				string text = ((textInputNode != null) ? ((object)textInputNode.String/*cast due to constrained. prefix*/).ToString().Trim() : null);
				TextInputNode? textInputNode2 = groupInput;
				string text2 = ((textInputNode2 != null) ? ((object)textInputNode2.String/*cast due to constrained. prefix*/).ToString().Trim() : null);
				markerRecord.Name = (string.IsNullOrWhiteSpace(text) ? LanguageManager.Get("CustomizeMapMarker-Untitled") : text);
				markerRecord.Group = (string.IsNullOrWhiteSpace(text2) ? LanguageManager.Get("CustomizeMapMarker-DefaultGroup") : text2);
				TextMultiLineInputNode? textMultiLineInputNode = descriptionInput;
				markerRecord.Description = ((textMultiLineInputNode != null) ? ((object)textMultiLineInputNode.String/*cast due to constrained. prefix*/).ToString().Trim() : null) ?? string.Empty;
				markerRecord.IconID = (uint)Math.Max(1, iconInput?.Value ?? 60561);
				TextInputNode? textInputNode3 = scaleInput;
				string s = ((textInputNode3 != null) ? ((object)textInputNode3.String/*cast due to constrained. prefix*/).ToString().Trim() : null);
				markerRecord.Scale = ((float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var result) && float.IsFinite(result) && result > 0f) ? result : 1f);
				markerRecord.AutoSetFlag = autoSetFlagInput?.IsChecked ?? false;
				TextMultiLineInputNode? textMultiLineInputNode2 = extraCommandsInput;
				markerRecord.ExtraCommands = ((textMultiLineInputNode2 != null) ? ((object)textMultiLineInputNode2.String/*cast due to constrained. prefix*/).ToString().Trim() : null) ?? string.Empty;
				module.SaveAndRefresh();
				NotifyHelper.ToastQuest(LanguageManager.Get("CustomizeMapMarker-Saved"), new QuestToastOptions
				{
					DisplayCheckmark = true
				});
			}
		}

		private void DeleteMarker()
		{
			module.DeleteMarker(markerID);
			Close();
		}
	}

	private sealed class MarkerListAddon(CustomizeMapMarker module) : NativeAddon
	{
		private sealed class MarkerListEntry
		{
			public MarkerRecord? Marker { get; init; }

			public Action? OpenMarker { get; init; }

			public Action? SetGameFlag { get; init; }

			public Action? Export { get; init; }

			public int MarkerCount { get; init; }

			public Action? PreviousPage { get; init; }

			public Action? NextPage { get; init; }

			public int Page { get; init; }

			public int TotalPages { get; init; }

			public bool IsExport => Export != null;

			public bool IsPagination
			{
				get
				{
					if (Marker == null)
					{
						return !IsExport;
					}
					return false;
				}
			}
		}

		private sealed class MarkerTreeItemNode : TreeListItemNode<MarkerListEntry>, ITreeListItemNode
		{
			private readonly HorizontalListNode row;

			private readonly TextButtonNode markerButton;

			private readonly IconButtonNode flagButton;

			private readonly TextNode groupCountText;

			private readonly CircleButtonNode exportButton;

			private readonly TextButtonNode previousPageButton;

			private readonly TextNode pageIndicator;

			private readonly TextButtonNode nextPageButton;

			public static float ItemHeight => 32f;

			public MarkerTreeItemNode()
			{
				//IL_009b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0121: Unknown result type (might be due to invalid IL or missing references)
				row = new HorizontalListNode
				{
					Size = new Vector2(0f, ItemHeight),
					ItemSpacing = 6f
				};
				row.AttachNode(this);
				markerButton = new TextButtonNode();
				row.AddNode(markerButton);
				flagButton = new IconButtonNode
				{
					IconId = 60561u,
					Size = new Vector2(32f),
					Position = new Vector2(0f, -2f),
					TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-SetFlag"))
				};
				row.AddNode(flagButton);
				groupCountText = new TextNode
				{
					TextFlags = (TextFlags)8,
					AlignmentType = (AlignmentType)3
				};
				AtkColors.Value.ApplyTo(groupCountText);
				row.AddNode(groupCountText);
				exportButton = new CircleButtonNode
				{
					Icon = CircleButtonIcon.RightArrow,
					Size = new Vector2(28f),
					TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-Export"))
				};
				row.AddNode(exportButton);
				previousPageButton = CreatePageButton("<");
				row.AddNode(previousPageButton);
				pageIndicator = CreatePageIndicator();
				row.AddNode(pageIndicator);
				nextPageButton = CreatePageButton(">");
				row.AddNode(nextPageButton);
			}

			protected override void OnSizeChanged()
			{
				base.OnSizeChanged();
				row.Size = Size;
				markerButton.Size = new Vector2(Math.Max(0f, Width - 38f), Height);
				groupCountText.Size = new Vector2(Math.Max(0f, Width - 34f), Height);
			}

			protected override void SetNodeData(MarkerListEntry itemData)
			{
				//IL_009a: Unknown result type (might be due to invalid IL or missing references)
				//IL_016f: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				bool isExport = itemData.IsExport;
				bool isPagination = itemData.IsPagination;
				row.Alignment = HorizontalListAnchor.Left;
				markerButton.IsVisible = itemData.Marker != null;
				flagButton.IsVisible = itemData.Marker != null;
				groupCountText.IsVisible = isExport;
				exportButton.IsVisible = isExport;
				previousPageButton.IsVisible = isPagination;
				pageIndicator.IsVisible = isPagination;
				nextPageButton.IsVisible = isPagination;
				MarkerRecord marker = itemData.Marker;
				if (marker != null)
				{
					markerButton.String = ReadOnlySeString.op_Implicit(marker.Name);
					markerButton.OnClick = itemData.OpenMarker;
					flagButton.OnClick = itemData.SetGameFlag;
				}
				else if (isExport)
				{
					groupCountText.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-MarkerCount", itemData.MarkerCount));
					exportButton.OnClick = itemData.Export;
				}
				else
				{
					previousPageButton.OnClick = itemData.PreviousPage;
					nextPageButton.OnClick = itemData.NextPage;
					pageIndicator.String = ReadOnlySeString.op_Implicit($"{itemData.Page + 1} / {itemData.TotalPages}");
				}
				row.RecalculateLayout();
			}
		}

		private const int GROUPS_PER_PAGE = 5;

		private const int MARKERS_PER_PAGE = 5;

		private const float HEADER_HEIGHT = 64f;

		private readonly Dictionary<string, int> groupPages = new Dictionary<string, int>();

		private ScrollingNode<VerticalListNode>? scrollArea;

		private TreeListNode<MarkerListEntry, MarkerTreeItemNode>? markerTree;

		private LabelTextNode? mapNameText;

		private HorizontalListNode? paginationBar;

		private TextButtonNode? previousPageButton;

		private TextNode? pageIndicator;

		private TextButtonNode? nextPageButton;

		private CircleButtonNode? importButton;

		private uint currentMapID = uint.MaxValue;

		private int currentPage;

		private int totalPages;

		private bool isRebuilding;

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			mapNameText = new LabelTextNode
			{
				Position = base.ContentStartPosition + new Vector2(0f, 4f),
				Size = new Vector2(base.ContentSize.X - 36f, 36f),
				FontSize = 18u,
				TextFlags = (TextFlags)1032
			};
			AtkColors.Label.ApplyTo(mapNameText);
			mapNameText.AttachNode(this);
			importButton = new CircleButtonNode
			{
				Position = base.ContentStartPosition + new Vector2(base.ContentSize.X - 28f, 4f),
				Icon = CircleButtonIcon.Document,
				TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-Import")),
				Size = new Vector2(28f),
				OnClick = module.ImportMarkers
			};
			importButton.AttachNode(this);
			HorizontalListNode obj = new HorizontalListNode
			{
				Position = base.ContentStartPosition + new Vector2(0f, 32f)
			};
			Vector2 contentSize = base.ContentSize;
			contentSize.Y = 28f;
			obj.Size = contentSize;
			obj.Alignment = HorizontalListAnchor.Center;
			obj.ItemSpacing = 8f;
			obj.IsVisible = false;
			paginationBar = obj;
			paginationBar.AttachNode(this);
			previousPageButton = CreatePageButton("<", delegate
			{
				ShowPage(currentPage - 1);
			});
			paginationBar.AddNode(previousPageButton);
			pageIndicator = CreatePageIndicator();
			paginationBar.AddNode(pageIndicator);
			nextPageButton = CreatePageButton(">", delegate
			{
				ShowPage(currentPage + 1);
			});
			paginationBar.AddNode(nextPageButton);
			ScrollingNode<VerticalListNode> scrollingNode = new ScrollingNode<VerticalListNode>();
			scrollingNode.Position = base.ContentStartPosition + new Vector2(0f, 64f);
			scrollingNode.Size = base.ContentSize - new Vector2(0f, 64f);
			scrollingNode.ScrollSpeed = 100;
			scrollingNode.AutoHideScrollBar = true;
			scrollingNode.ContentNode.FitContents = true;
			scrollingNode.ContentNode.ItemSpacing = 4f;
			scrollArea = scrollingNode;
			scrollArea.AttachNode(this);
			markerTree = new TreeListNode<MarkerListEntry, MarkerTreeItemNode>
			{
				Size = new Vector2(base.ContentSize.X, base.ContentSize.Y - 64f),
				ItemSpacing = 4f,
				NoResultsString = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-Empty"))
			};
			scrollArea.ContentNode.AddNode(markerTree);
			BuildList();
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			uint selectedMapId = ((AgentMap)AgentMap.Instance()).SelectedMapId;
			if (selectedMapId != currentMapID)
			{
				currentMapID = selectedMapId;
				currentPage = 0;
				groupPages.Clear();
				BuildList();
			}
		}

		protected unsafe override void OnFinalize(AtkUnitBase* addon)
		{
			groupPages.Clear();
			scrollArea = null;
			markerTree = null;
			mapNameText = null;
			paginationBar = null;
			previousPageButton = null;
			pageIndicator = null;
			nextPageButton = null;
			importButton = null;
			currentMapID = uint.MaxValue;
			currentPage = 0;
			totalPages = 0;
		}

		public void RebuildList()
		{
			if (base.IsOpen)
			{
				BuildList();
			}
		}

		private unsafe void BuildList()
		{
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			if (markerTree == null || mapNameText == null || paginationBar == null || isRebuilding)
			{
				return;
			}
			isRebuilding = true;
			try
			{
				uint selectedMapId = ((AgentMap)AgentMap.Instance()).SelectedMapId;
				if (selectedMapId != currentMapID)
				{
					currentMapID = selectedMapId;
					currentPage = 0;
					groupPages.Clear();
				}
				List<IGrouping<string, MarkerRecord>> list = (from marker in module.config.Markers
					where marker.MapID == currentMapID
					orderby marker.Group, marker.Name
					group marker by marker.Group).ToList();
				totalPages = Math.Max(1, (list.Count + 5 - 1) / 5);
				currentPage = Math.Clamp(currentPage, 0, totalPages - 1);
				mapNameText.String = ReadOnlySeString.op_Implicit(FormatMapName(currentMapID));
				paginationBar.IsVisible = totalPages > 1;
				UpdatePaginationState();
				Dictionary<ReadOnlySeString, List<MarkerListEntry>> dictionary = new Dictionary<ReadOnlySeString, List<MarkerListEntry>>(5);
				foreach (IGrouping<string, MarkerRecord> group in list.Skip(currentPage * 5).Take(5))
				{
					int num = Math.Max(1, (group.Count() + 5 - 1) / 5);
					int value;
					int groupPage = (groupPages.TryGetValue(group.Key, out value) ? Math.Clamp(value, 0, num - 1) : 0);
					groupPages[group.Key] = groupPage;
					List<MarkerListEntry> list2 = new List<MarkerListEntry>
					{
						new MarkerListEntry
						{
							Export = delegate
							{
								ExportGroup(group.Key);
							},
							MarkerCount = group.Count()
						}
					};
					list2.AddRange(group.Skip(groupPage * 5).Take(5).Select(CreateMarkerEntry));
					if (num > 1)
					{
						list2.Add(new MarkerListEntry
						{
							PreviousPage = delegate
							{
								ShowGroupPage(group.Key, groupPage - 1);
							},
							NextPage = delegate
							{
								ShowGroupPage(group.Key, groupPage + 1);
							},
							Page = groupPage,
							TotalPages = num
						});
					}
					dictionary[new ReadOnlySeString(group.Key)] = list2;
				}
				markerTree.Options = dictionary;
				scrollArea?.ScrollToTop();
				scrollArea?.ContentNode.RecalculateLayout();
				scrollArea?.RecalculateSizes();
			}
			finally
			{
				isRebuilding = false;
			}
		}

		private MarkerListEntry CreateMarkerEntry(MarkerRecord marker)
		{
			return new MarkerListEntry
			{
				Marker = marker,
				OpenMarker = delegate
				{
					module.markerDetailsAddon?.OpenMarker(marker.ID);
				},
				SetGameFlag = delegate
				{
					SetGameFlag(marker);
				}
			};
		}

		private static TextButtonNode CreatePageButton(string text, Action? onClick = null)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return new TextButtonNode
			{
				String = ReadOnlySeString.op_Implicit(text),
				Size = new Vector2(36f, 24f),
				OnClick = onClick
			};
		}

		private static TextNode CreatePageIndicator()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return new TextNode
			{
				TextFlags = (TextFlags)1,
				String = ReadOnlySeString.op_Implicit("1 / 1"),
				Position = new Vector2(0f, 3f),
				AlignmentType = (AlignmentType)3
			};
		}

		private void ShowPage(int page)
		{
			currentPage = Math.Clamp(page, 0, totalPages - 1);
			BuildList();
		}

		private void ShowGroupPage(string group, int page)
		{
			groupPages[group] = page;
			BuildList();
		}

		private void UpdatePaginationState()
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			if (previousPageButton != null && pageIndicator != null && nextPageButton != null)
			{
				previousPageButton.IsEnabled = currentPage > 0;
				nextPageButton.IsEnabled = currentPage < totalPages - 1;
				pageIndicator.String = ReadOnlySeString.op_Implicit($"{currentPage + 1} / {totalPages}");
			}
		}

		private void ExportGroup(string group)
		{
			ExportMarkers(module.config.Markers.Where((MarkerRecord marker) => marker.MapID == currentMapID && marker.Group == group));
		}
	}

	private Config config;

	private readonly Dictionary<Guid, MarkerRecord> markerIndex = new Dictionary<Guid, MarkerRecord>();

	private MarkerDetailsAddon? markerDetailsAddon;

	private MarkerListAddon? markerListAddon;

	private AddonController<AddonAreaMap>? areaMapController;

	private MapOverlayController? mapOverlayController;

	private HorizontalListNode? mapButtonContainer;

	private CircleButtonNode? mapAddButton;

	private CircleButtonNode? mapListButton;

	private bool isPlacingMarker;

	private const string COMMAND = "mapmarker";

	private const uint DEFAULT_ICON_ID = 60561u;

	private const int MAX_IMPORT_COUNT = 5000;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CustomizeMapMarkerTitle"),
		Description = LanguageManager.Get("CustomizeMapMarkerDescription"),
		Category = ModuleCategory.Interface,
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/CustomizeMapMarker/preview-1.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		NormalizeConfig();
		markerDetailsAddon = new MarkerDetailsAddon(this)
		{
			InternalName = "DRCustomizeMapMarkerDetails",
			Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-DetailsTitle")),
			Size = new Vector2(420f, 640f)
		};
		markerListAddon = new MarkerListAddon(this)
		{
			InternalName = "DRCustomizeMapMarkerList",
			Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-ListTitle")),
			Size = new Vector2(620f, 520f)
		};
		mapOverlayController = new MapOverlayController
		{
			OnMapClick = AddMarkerAtMapPosition
		};
		mapOverlayController.Enable();
		areaMapController = new AddonController<AddonAreaMap>
		{
			AddonName = "AreaMap",
			OnSetup = OnAreaMapSetup,
			OnUpdate = OnAreaMapUpdate,
			OnFinalize = OnAreaMapFinalize
		};
		areaMapController.Enable();
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("mapmarker", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("CustomizeMapMarker-CommandHelp")
		});
		RebuildMapMarkers();
	}

	protected unsafe override void Uninit()
	{
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("mapmarker");
		areaMapController?.Dispose();
		areaMapController = null;
		OnAreaMapFinalize(null);
		mapOverlayController?.Dispose();
		mapOverlayController = null;
		markerDetailsAddon?.Dispose();
		markerDetailsAddon = null;
		markerListAddon?.Dispose();
		markerListAddon = null;
		markerIndex.Clear();
	}

	private void OnCommand(string command, string arguments)
	{
		markerListAddon?.Open();
	}

	private unsafe void OnAreaMapSetup(AddonAreaMap* addon)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		mapButtonContainer = new HorizontalListNode
		{
			Position = new Vector2(250f, 30f),
			Size = new Vector2(70f, 28f),
			ItemSpacing = 4f,
			Alignment = HorizontalListAnchor.Right
		};
		HorizontalListNode? horizontalListNode = mapButtonContainer;
		CircleButtonNode obj = new CircleButtonNode
		{
			Icon = CircleButtonIcon.Add,
			TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-AddMarker")),
			Size = new Vector2(28f),
			OnClick = TogglePlacementMode
		};
		CircleButtonNode node = obj;
		mapAddButton = obj;
		horizontalListNode.AddNode(node);
		HorizontalListNode? horizontalListNode2 = mapButtonContainer;
		CircleButtonNode obj2 = new CircleButtonNode
		{
			Icon = CircleButtonIcon.Document,
			TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("CustomizeMapMarker-OpenList")),
			Size = new Vector2(28f),
			OnClick = delegate
			{
				markerListAddon?.Toggle();
			}
		};
		node = obj2;
		mapListButton = obj2;
		horizontalListNode2.AddNode(node);
		mapButtonContainer.RecalculateLayout();
		mapButtonContainer.AttachNode(((AddonAreaMap)addon).LocationContainerNode);
	}

	private unsafe void OnAreaMapUpdate(AddonAreaMap* addon)
	{
		HorizontalListNode? horizontalListNode = mapButtonContainer;
		if (horizontalListNode != null)
		{
			horizontalListNode.X = ((AtkResNode)((AddonAreaMap)addon).RootNode).GetWidth() - 24;
		}
		HorizontalListNode? horizontalListNode2 = mapButtonContainer;
		if (horizontalListNode2 != null)
		{
			horizontalListNode2.Y = ((AtkResNode)((AddonAreaMap)addon).RootNode).GetHeight() - 52;
		}
	}

	private unsafe void OnAreaMapFinalize(AddonAreaMap* addon)
	{
		isPlacingMarker = false;
		mapButtonContainer?.Dispose();
		mapButtonContainer = null;
		mapAddButton = null;
		mapListButton = null;
	}

	private void TogglePlacementMode()
	{
		isPlacingMarker = !isPlacingMarker;
		UpdatePlacementButton();
		if (isPlacingMarker)
		{
			NotifyHelper.ToastQuest(LanguageManager.Get("CustomizeMapMarker-PlacementHint"));
		}
	}

	private void UpdatePlacementButton()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (mapAddButton != null)
		{
			mapAddButton.Icon = (isPlacingMarker ? CircleButtonIcon.CrossSmall : CircleButtonIcon.Add);
			mapAddButton.TextTooltip = ReadOnlySeString.op_Implicit(isPlacingMarker ? LanguageManager.Get("CustomizeMapMarker-CancelPlacement") : LanguageManager.Get("CustomizeMapMarker-AddMarker"));
		}
	}

	private void AddMarkerAtMapPosition(uint mapID, Vector2 overlayPosition)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (isPlacingMarker)
		{
			isPlacingMarker = false;
			UpdatePlacementButton();
			if (!LuminaGetter.TryGetRow<Map>(mapID, out Map item))
			{
				NotifyHelper.ToastError(LanguageManager.Get("CustomizeMapMarker-InvalidMap"));
				return;
			}
			Vector2 vector = PositionHelper.WorldToMap(overlayPosition, item);
			MarkerRecord markerRecord = new MarkerRecord();
			markerRecord.TerritoryID = ((Map)(ref item)).TerritoryType.RowId;
			markerRecord.MapID = mapID;
			markerRecord.TexturePosition = overlayPosition;
			markerRecord.Name = LanguageManager.Get("CustomizeMapMarker-DefaultName", vector.X, vector.Y);
			MarkerRecord markerRecord2 = markerRecord;
			config.Markers.Add(markerRecord2);
			markerIndex.Add(markerRecord2.ID, markerRecord2);
			SaveAndRefresh();
			markerDetailsAddon?.OpenMarker(markerRecord2.ID);
		}
	}

	private MarkerRecord? FindMarker(Guid markerID)
	{
		return markerIndex.GetValueOrDefault(markerID);
	}

	private void DeleteMarker(Guid markerID)
	{
		if (markerIndex.Remove(markerID, out MarkerRecord value))
		{
			config.Markers.Remove(value);
			SaveAndRefresh();
		}
	}

	private unsafe static void SetGameFlag(MarkerRecord marker)
	{
		(*AgentMap.Instance()).SetMapFlagAndOpen(marker.MapID, marker.TexturePosition.ToVector3(0f));
	}

	private void SaveAndRefresh()
	{
		config.Save(this);
		RebuildMapMarkers();
		markerListAddon?.RebuildList();
		markerDetailsAddon?.RefreshMarker();
	}

	private static string FormatMarkerLocation(MarkerRecord marker)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (LuminaGetter.TryGetRow<Map>(marker.MapID, out Map item))
		{
			Vector2 vector = PositionHelper.WorldToMap(marker.TexturePosition, item);
			return $"{GetMapName(item, marker.MapID)}  X: {vector.X:F1}  Y: {vector.Y:F1}";
		}
		return $"Map {marker.MapID}";
	}

	private static string FormatMapName(uint mapID)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Map>(mapID, out Map item))
		{
			return $"Map {mapID}";
		}
		return GetMapName(item, mapID);
	}

	private static string GetMapName(Map map, uint mapID)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		PlaceName? valueNullable = ((Map)(ref map)).PlaceName.ValueNullable;
		object obj;
		if (!valueNullable.HasValue)
		{
			obj = null;
		}
		else
		{
			PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
			obj = ((object)((PlaceName)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		}
		string text = (string)obj;
		if (!string.IsNullOrWhiteSpace(text))
		{
			return text;
		}
		return $"Map {mapID}";
	}

	private static void ExportMarkers(IEnumerable<MarkerRecord> markers)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		try
		{
			ImGui.SetClipboardText(ImU8String.op_Implicit(new MarkerPackage
			{
				Markers = markers.Select((MarkerRecord marker) => marker.Clone()).ToList()
			}.ToJSONBase64()));
			NotifyHelper.ToastQuest(LanguageManager.Get("CustomizeMapMarker-Exported"), new QuestToastOptions
			{
				DisplayCheckmark = true
			});
		}
		catch (Exception ex)
		{
			DLog.Error(LanguageManager.Get("CustomizeMapMarker-ExportFailed"), ex);
			NotifyHelper.ToastError(LanguageManager.Get("CustomizeMapMarker-ExportFailed"));
		}
	}

	private void ImportMarkers()
	{
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		try
		{
			MarkerPackage markerPackage = ImGui.GetClipboardText().FromJSONBase64<MarkerPackage>();
			if (markerPackage == null || markerPackage.Version != 1)
			{
				NotifyHelper.ToastError(LanguageManager.Get("CustomizeMapMarker-ImportInvalid"));
				return;
			}
			int num = 0;
			foreach (MarkerRecord item2 in markerPackage.Markers.Take(5000))
			{
				if (item2.MapID != 0 && item2.IconID <= int.MaxValue && float.IsFinite(item2.TexturePosition.X) && float.IsFinite(item2.TexturePosition.Y) && LuminaGetter.TryGetRow<Map>(item2.MapID, out Map _))
				{
					MarkerRecord markerRecord = item2.Clone();
					if (markerRecord.ID == Guid.Empty)
					{
						markerRecord.ID = Guid.NewGuid();
					}
					if (markerIndex.TryGetValue(markerRecord.ID, out MarkerRecord value))
					{
						CopyMarker(markerRecord, value);
					}
					else
					{
						config.Markers.Add(markerRecord);
						markerIndex.Add(markerRecord.ID, markerRecord);
					}
					num++;
				}
			}
			NormalizeConfig();
			SaveAndRefresh();
			NotifyHelper.ToastQuest(LanguageManager.Get("CustomizeMapMarker-Imported", num), new QuestToastOptions
			{
				DisplayCheckmark = true
			});
		}
		catch (Exception ex)
		{
			DLog.Error(LanguageManager.Get("CustomizeMapMarker-ImportFailed"), ex);
			NotifyHelper.ToastError(LanguageManager.Get("CustomizeMapMarker-ImportFailed"));
		}
	}

	private static void CopyMarker(MarkerRecord source, MarkerRecord destination)
	{
		destination.TerritoryID = source.TerritoryID;
		destination.MapID = source.MapID;
		destination.TexturePosition = source.TexturePosition;
		destination.Name = source.Name;
		destination.Group = source.Group;
		destination.Description = source.Description;
		destination.IconID = source.IconID;
		destination.Scale = source.Scale;
		destination.AutoSetFlag = source.AutoSetFlag;
		destination.ExtraCommands = source.ExtraCommands;
		destination.CreatedAt = source.CreatedAt;
	}

	private void RebuildMapMarkers()
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (mapOverlayController == null)
		{
			return;
		}
		mapOverlayController.RemoveAllMarkers();
		foreach (MarkerRecord marker in config.Markers)
		{
			if (LuminaGetter.TryGetRow<Map>(marker.MapID, out Map _))
			{
				Guid markerID = marker.ID;
				mapOverlayController.AddMarker(new MapMarkerNode
				{
					MapId = marker.MapID,
					Position = marker.TexturePosition,
					IconId = marker.IconID,
					Size = new Vector2(32f),
					MarkerScale = marker.Scale,
					TextTooltip = ReadOnlySeString.op_Implicit($"{marker.Name} [{marker.Group}]\n{marker.Description}".Trim()),
					OnClick = delegate
					{
						HandleMarkerClick(markerID);
					},
					OnRightClick = delegate
					{
						markerDetailsAddon?.OpenMarker(markerID);
					}
				});
			}
		}
	}

	private void HandleMarkerClick(Guid markerID)
	{
		MarkerRecord markerRecord = FindMarker(markerID);
		if (markerRecord != null)
		{
			if (markerRecord.AutoSetFlag)
			{
				SetGameFlag(markerRecord);
			}
			if (!string.IsNullOrWhiteSpace(markerRecord.ExtraCommands))
			{
				OmenServiceBase<ChatManager>.Instance().ExecuteMacro(markerRecord.ExtraCommands);
			}
		}
	}

	private void NormalizeConfig()
	{
		Config config = this.config;
		if (config.Markers == null)
		{
			List<MarkerRecord> list = (config.Markers = new List<MarkerRecord>());
		}
		markerIndex.Clear();
		foreach (MarkerRecord marker in this.config.Markers)
		{
			while (marker.ID == Guid.Empty || !markerIndex.TryAdd(marker.ID, marker))
			{
				marker.ID = Guid.NewGuid();
			}
			if (string.IsNullOrWhiteSpace(marker.Name))
			{
				marker.Name = LanguageManager.Get("CustomizeMapMarker-Untitled");
			}
			if (string.IsNullOrWhiteSpace(marker.Group))
			{
				marker.Group = LanguageManager.Get("CustomizeMapMarker-DefaultGroup");
			}
			uint iconID = marker.IconID;
			if ((iconID > int.MaxValue || iconID == 0) ? true : false)
			{
				marker.IconID = 60561u;
			}
			if (!float.IsFinite(marker.Scale) || marker.Scale <= 0f)
			{
				marker.Scale = 1f;
			}
			MarkerRecord markerRecord = marker;
			if (markerRecord.ExtraCommands == null)
			{
				string text = (markerRecord.ExtraCommands = string.Empty);
			}
		}
	}
}
