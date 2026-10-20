using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Addons;
using OmenTools.KamiToolKit.Nodes;
using OmenTools.KamiToolKit.Nodes.Collasping;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class QuickChatPanel : ModuleBase
{
	private class Config : ModuleConfig
	{
		public MacroDisplayMode OverlayMacroDisplayMode = MacroDisplayMode.Buttons;

		public Vector2 OverlayOffset = new Vector2(0f);

		public QuickChatTab SelectedTab;

		public List<SavedMacro> SavedMacros = new List<SavedMacro>();

		public List<string> SavedMessages = new List<string>();

		public Dictionary<uint, string> SoundEffectNotes = new Dictionary<uint, string>();
	}

	private class QuickChatPanelAddon : AttachedAddon
	{
		private readonly Dictionary<QuickChatTab, ListButtonNode> tabButtons = new Dictionary<QuickChatTab, ListButtonNode>();

		private readonly Dictionary<QuickChatTab, ResNode> tabRoots = new Dictionary<QuickChatTab, ResNode>();

		private readonly Dictionary<QuickChatTab, ScrollingNode<VerticalListNode>> tabContentLists = new Dictionary<QuickChatTab, ScrollingNode<VerticalListNode>>();

		private readonly LuminaSearcher<Item> searcher;

		private readonly QuickChatPanel instance;

		private readonly TaskHelper taskHelper = new TaskHelper();

		private QuickChatTab selectedTab;

		private SimpleComponentNode? contentPanel;

		private SimpleNineGridNode? contentPanelBackground;

		private Vector2 tabContentSize;

		private string itemSearchInput = string.Empty;

		protected override AttachedAddonPosition AttachPosition => AttachedAddonPosition.RightBottom;

		protected override Vector2 PositionOffset => instance.config.OverlayOffset + new Vector2(0f, -28f);

		protected override bool CanOpenAddon => false;

		public QuickChatPanelAddon(QuickChatPanel instance)
			: base("ChatLog")
		{
			this.instance = instance;
			selectedTab = (Enum.IsDefined(instance.config.SelectedTab) ? instance.config.SelectedTab : QuickChatTab.Messages);
			searcher = new LuminaSearcher<Item>((IEnumerable<Item>)LuminaGetter.Get<Item>(), new Func<Item, string>[3]
			{
				(Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
				(Item x) => ((object)((Item)(ref x)).Description/*cast due to constrained. prefix*/).ToString(),
				(Item x) => ((Item)(ref x)).RowId.ToString()
			}, null, 50);
		}

		public void TogglePanel()
		{
			if (base.IsOpen)
			{
				Close();
			}
			else
			{
				Open();
			}
		}

		protected unsafe override bool CanCloseHostAddon(AtkUnitBase* hostAddon)
		{
			return false;
		}

		protected unsafe override void OnAttachedAddonUpdate(AtkUnitBase* addon, AtkUnitBase* hostAddon)
		{
			if (DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)27])
			{
				Close();
			}
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			tabButtons.Clear();
			tabRoots.Clear();
			tabContentLists.Clear();
			itemSearchInput = string.Empty;
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				IsVisible = true,
				Position = base.ContentStartPosition,
				Size = base.ContentSize,
				ItemSpacing = 8f,
				FitHeight = true
			};
			VerticalListNode verticalListNode = new VerticalListNode
			{
				IsVisible = true,
				Size = new Vector2(108f, horizontalListNode.Height),
				ItemSpacing = 5f,
				FitWidth = true
			};
			verticalListNode.AddNode(CreateNavButton(QuickChatTab.Messages, LanguageManager.Get("QuickChatPanel-Messages")));
			verticalListNode.AddNode(CreateNavButton(QuickChatTab.Macros, LanguageManager.Get("QuickChatPanel-Macro")));
			verticalListNode.AddNode(CreateNavButton(QuickChatTab.SystemSounds, LanguageManager.Get("QuickChatPanel-SystemSound")));
			verticalListNode.AddNode(CreateNavButton(QuickChatTab.GameItems, LanguageManager.Get("QuickChatPanel-GameItems")));
			verticalListNode.AddNode(CreateNavButton(QuickChatTab.SpecialIconChars, LanguageManager.Get("QuickChatPanel-SpecialIconChar")));
			verticalListNode.AddDummy(8f);
			verticalListNode.AddNode(CreateNavButton(QuickChatTab.Settings, LanguageManager.Get("Settings")));
			Vector2 vector = (tabContentSize = new Vector2(base.ContentSize.X - verticalListNode.Width - 16f, horizontalListNode.Height - 16f));
			contentPanel = new SimpleComponentNode
			{
				IsVisible = true,
				Size = vector
			};
			contentPanelBackground = new SimpleNineGridNode
			{
				IsVisible = true,
				TexturePath = "ui/uld/ToolTipS.tex",
				TextureCoordinates = Vector2.Zero,
				TextureSize = new Vector2(32f, 24f),
				TopOffset = 10f,
				BottomOffset = 10f,
				LeftOffset = 12f,
				RightOffset = 12f,
				Size = vector + new Vector2(12f),
				Alpha = 0.42f
			};
			contentPanelBackground.AttachNode((NodeBase?)contentPanel, NodePosition.AsLastChild);
			horizontalListNode.AddNode(verticalListNode);
			horizontalListNode.AddNode(contentPanel);
			RefreshTabButtons();
			horizontalListNode.AttachNode(this);
			ShowTab(selectedTab);
		}

		public override void Dispose()
		{
			taskHelper.Dispose();
			base.Dispose();
		}

		private void SelectTab(QuickChatTab tab)
		{
			if (selectedTab != tab)
			{
				selectedTab = tab;
				instance.config.SelectedTab = tab;
				instance.config.Save(instance);
				RefreshTabButtons();
				ShowTab(tab);
			}
		}

		private void RefreshTabButtons()
		{
			foreach (KeyValuePair<QuickChatTab, ListButtonNode> tabButton in tabButtons)
			{
				tabButton.Deconstruct(out var key, out var value);
				QuickChatTab quickChatTab = key;
				value.Selected = quickChatTab == selectedTab;
			}
		}

		private void EnsureTabBuilt(QuickChatTab tab)
		{
			if (!tabRoots.ContainsKey(tab) && contentPanel != null)
			{
				ResNode resNode = new ResNode
				{
					IsVisible = false,
					Size = tabContentSize
				};
				resNode.AttachNode((NodeBase?)contentPanel, NodePosition.AsLastChild);
				ScrollingNode<VerticalListNode> scrollingNode = new ScrollingNode<VerticalListNode>
				{
					Position = new Vector2(9f),
					Size = tabContentSize - new Vector2(8f, 12f),
					ScrollSpeed = 36,
					AutoHideScrollBar = true
				};
				scrollingNode.ContentNode.ItemSpacing = 5f;
				scrollingNode.ContentNode.FitWidth = true;
				scrollingNode.ContentNode.FitContents = true;
				scrollingNode.AttachNode(resNode);
				tabRoots[tab] = resNode;
				tabContentLists[tab] = scrollingNode;
				BuildTab(tab, scrollingNode);
				scrollingNode.RecalculateSizes();
			}
		}

		private void BuildTab(QuickChatTab tab, ScrollingNode<VerticalListNode> contentList)
		{
			switch (tab)
			{
			case QuickChatTab.Messages:
				BuildMessagesTab(contentList);
				break;
			case QuickChatTab.Macros:
				BuildMacrosTab(contentList);
				break;
			case QuickChatTab.SystemSounds:
				BuildSystemSoundsTab(contentList);
				break;
			case QuickChatTab.GameItems:
				BuildGameItemsTab(contentList);
				break;
			case QuickChatTab.SpecialIconChars:
				BuildSpecialIconCharsTab(contentList);
				break;
			case QuickChatTab.Settings:
				BuildSettingsTab(contentList);
				break;
			}
		}

		private void ShowTab(QuickChatTab tab)
		{
			taskHelper.Enqueue(delegate
			{
				EnsureTabBuilt(tab);
				foreach (KeyValuePair<QuickChatTab, ResNode> tabRoot in tabRoots)
				{
					tabRoot.Deconstruct(out var key, out var value);
					QuickChatTab quickChatTab = key;
					value.IsVisible = quickChatTab == tab;
				}
			});
		}

		private ListButtonNode CreateNavButton(QuickChatTab tab, string text)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			ListButtonNode listButtonNode = new ListButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(116f, 34f),
				String = ReadOnlySeString.op_Implicit(text),
				TextTooltip = ReadOnlySeString.op_Implicit(text),
				OnClick = delegate
				{
					SelectTab(tab);
				}
			};
			listButtonNode.LabelNode.FontSize = 15u;
			listButtonNode.LabelNode.AutoAdjustTextSize();
			tabButtons[tab] = listButtonNode;
			return listButtonNode;
		}

		private unsafe void BuildMessagesTab(ScrollingNode<VerticalListNode> contentList)
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Expected O, but got Unknown
			if (instance.config.SavedMessages.Count == 0)
			{
				AddEmptyState(contentList, LanguageManager.Get("QuickChatPanel-SavedMessagesAmountText", 0), LanguageManager.Get("QuickChatPanel-SendMessageHelp"));
				return;
			}
			foreach (string message in instance.config.SavedMessages)
			{
				contentList.ContentNode.AddNode(CreateTextActionRow(contentList, message, LanguageManager.Get("QuickChatPanel-SendMessageHelp"), delegate
				{
					CopyText(message);
				}, (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* data)
				{
					if (AtkEventDataExtension.get_IsRightClick(in *data))
					{
						instance.SendSavedMessage(message);
					}
					else if (AtkEventDataExtension.get_IsLeftClick(in *data))
					{
						CopyText(message);
					}
				}));
			}
		}

		private void BuildMacrosTab(ScrollingNode<VerticalListNode> contentList)
		{
			if (instance.config.SavedMacros.Count == 0)
			{
				AddEmptyState(contentList, LanguageManager.Get("QuickChatPanel-SavedMacrosAmountText", 0), LanguageManager.Get("QuickChatPanelTitle-DragHelp"));
			}
			else if (instance.config.OverlayMacroDisplayMode == MacroDisplayMode.List)
			{
				BuildMacroList(contentList);
			}
			else
			{
				BuildMacroButtonGrid(contentList);
			}
		}

		private void BuildMacroList(ScrollingNode<VerticalListNode> contentList)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Expected O, but got Unknown
			foreach (SavedMacro macro in instance.config.SavedMacros)
			{
				if (!string.IsNullOrWhiteSpace(macro.Name))
				{
					contentList.ContentNode.AddNode(CreateMacroListButton(macro.IconID, macro.Name, (ReceiveEvent)delegate
					{
						instance.ExecuteMacro(macro);
					}));
				}
			}
			TextButtonNode CreateMacroListButton(uint iconID, string title, ReceiveEvent? onMouseClick = null)
			{
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				//IL_010d: Unknown result type (might be due to invalid IL or missing references)
				TextButtonNode textButtonNode = new TextButtonNode
				{
					IsVisible = true,
					IsEnabled = true,
					Size = new Vector2(contentList.ContentNode.Width, 52f),
					String = ReadOnlySeString.op_Implicit(string.Empty),
					TextTooltip = ReadOnlySeString.op_Implicit(title)
				};
				textButtonNode.LabelNode.IsVisible = false;
				IconImageNode iconImageNode = new IconImageNode
				{
					IsVisible = true,
					Size = new Vector2(28f),
					TextureSize = new Vector2(28f),
					IconId = iconID,
					FitTexture = true
				};
				iconImageNode.Position = new Vector2(12f, (textButtonNode.Size.Y - iconImageNode.Size.Y) / 2f - 2f);
				iconImageNode.AttachNode((NodeBase?)textButtonNode, NodePosition.AsLastChild);
				TextNode textNode = new TextNode
				{
					IsVisible = true,
					Position = new Vector2(iconImageNode.Position.X + iconImageNode.Size.X + 6f, 0f),
					String = ReadOnlySeString.op_Implicit(title),
					AlignmentType = (AlignmentType)3,
					FontSize = 14u,
					TextFlags = (TextFlags)2
				};
				textNode.Size = new Vector2(textButtonNode.Size.X - textNode.Position.X, 46f);
				textNode.AttachNode((NodeBase?)textButtonNode, NodePosition.AsLastChild);
				if (onMouseClick != null)
				{
					textButtonNode.AddEvent((AtkEventType)9, onMouseClick);
				}
				return textButtonNode;
			}
		}

		private void BuildMacroButtonGrid(ScrollingNode<VerticalListNode> contentList)
		{
			HorizontalListNode horizontalListNode = CreateCardRow(contentList);
			float num = 0f;
			foreach (SavedMacro macro in instance.config.SavedMacros)
			{
				if (!string.IsNullOrWhiteSpace(macro.Name))
				{
					TextButtonNode textButtonNode = CreateMacroCardButton(macro.IconID, macro.Name, delegate
					{
						instance.ExecuteMacro(macro);
					});
					if (num + textButtonNode.Width > contentList.ContentNode.Width)
					{
						contentList.ContentNode.AddNode(horizontalListNode);
						horizontalListNode = CreateCardRow(contentList);
						num = 0f;
					}
					horizontalListNode.AddNode(textButtonNode);
					num += textButtonNode.Width;
				}
			}
			if (horizontalListNode.Nodes.Count > 0)
			{
				contentList.ContentNode.AddNode(horizontalListNode);
			}
			static TextButtonNode CreateMacroCardButton(uint iconID, string text, Action onClick)
			{
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
				TextButtonNode textButtonNode2 = new TextButtonNode
				{
					IsVisible = true,
					IsEnabled = true,
					Size = new Vector2(110f, 110f),
					String = ReadOnlySeString.op_Implicit(string.Empty),
					TextTooltip = ReadOnlySeString.op_Implicit(text),
					OnClick = onClick
				};
				textButtonNode2.LabelNode.IsVisible = false;
				IconImageNode iconImageNode = new IconImageNode
				{
					IsVisible = true,
					Size = new Vector2(50f),
					IconId = iconID,
					FitTexture = true
				};
				iconImageNode.Position = new Vector2((textButtonNode2.Size.X - iconImageNode.Size.X) / 2f, 14f);
				iconImageNode.AttachNode((NodeBase?)textButtonNode2, NodePosition.AsLastChild);
				TextNode textNode = new TextNode();
				textNode.IsVisible = true;
				textNode.Position = new Vector2(0f, 64f);
				textNode.Size = new Vector2(textButtonNode2.Width, 24f);
				textNode.String = ReadOnlySeString.op_Implicit(text);
				textNode.AlignmentType = (AlignmentType)4;
				textNode.FontSize = 12u;
				textNode.TextFlags = (TextFlags)2;
				textNode.AttachNode((NodeBase?)textButtonNode2, NodePosition.AsLastChild);
				return textButtonNode2;
			}
		}

		private unsafe void BuildSystemSoundsTab(ScrollingNode<VerticalListNode> contentList)
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Expected O, but got Unknown
			HorizontalListNode horizontalListNode = CreateCompactRow(contentList);
			float num = 0f;
			foreach (KeyValuePair<uint, string> item in instance.config.SoundEffectNotes.OrderBy<KeyValuePair<uint, string>, uint>((KeyValuePair<uint, string> x) => x.Key))
			{
				item.Deconstruct(out var key, out var value);
				uint key2 = key;
				TextButtonNode textButtonNode = CreateCompactTextButton(value, LanguageManager.Get("QuickChatPanel-SystemSoundHelp"), delegate
				{
					UIGlobals.PlayChatSoundEffect(key2);
				}, (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* data)
				{
					if (((AtkMouseData)(&((AtkEventData)data).MouseData)).ButtonId == 1)
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage($"<se.{key2}><se.{key2}>");
					}
					else
					{
						UIGlobals.PlayChatSoundEffect(key2);
					}
				});
				if (num + textButtonNode.Width > contentList.ContentNode.Width)
				{
					contentList.ContentNode.AddNode(horizontalListNode);
					horizontalListNode = CreateCompactRow(contentList);
					num = 0f;
				}
				horizontalListNode.AddNode(textButtonNode);
				num += textButtonNode.Width;
			}
			if (horizontalListNode.Nodes.Count > 0)
			{
				contentList.ContentNode.AddNode(horizontalListNode);
			}
		}

		private unsafe void BuildGameItemsTab(ScrollingNode<VerticalListNode> contentList)
		{
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			ListNode<Item, ItemListItemNode> listNode = new ListNode<Item, ItemListItemNode>
			{
				IsVisible = true,
				ItemSpacing = 4f,
				Size = new Vector2(contentList.ContentNode.Width, contentList.Size.Y - 48f),
				OptionsList = GetGameItemResults(),
				OnItemSelected = delegate(Item item)
				{
					//IL_0009: Unknown result type (might be due to invalid IL or missing references)
					if (((Item)(ref item)).RowId != 0)
					{
						SendGameItemLink(item);
					}
				}
			};
			TextInputNode textInputNode = new TextInputNode
			{
				IsVisible = true,
				Size = new Vector2(contentList.ContentNode.Width, 36f),
				String = ReadOnlySeString.op_Implicit(itemSearchInput),
				MaxCharacters = 128,
				OnInputReceived = delegate(ReadOnlySeString text)
				{
					UpdateGameItemList(listNode, ((object)(*(ReadOnlySeString*)(&text))/*cast due to constrained. prefix*/).ToString());
				},
				OnInputComplete = delegate(ReadOnlySeString text)
				{
					UpdateGameItemList(listNode, ((object)(*(ReadOnlySeString*)(&text))/*cast due to constrained. prefix*/).ToString());
				}
			};
			textInputNode.CurrentTextNode.FontSize = 14u;
			textInputNode.CurrentTextNode.Position += new Vector2(0f, 3f);
			contentList.ContentNode.AddNode(textInputNode);
			contentList.ContentNode.AddNode(listNode);
			List<Item> GetGameItemResults()
			{
				if (!string.IsNullOrWhiteSpace(itemSearchInput))
				{
					return searcher.SearchResult;
				}
				return new List<Item>();
			}
			void UpdateGameItemList(ListNode<Item, ItemListItemNode> node, string searchString)
			{
				itemSearchInput = searchString;
				searcher.Search(itemSearchInput);
				node.OptionsList = GetGameItemResults();
				node.ResetScroll();
			}
		}

		private static void BuildSpecialIconCharsTab(ScrollingNode<VerticalListNode> contentList)
		{
			HorizontalListNode horizontalListNode = CreateGlyphRow();
			float num = 0f;
			char[] seIconChars = SeIconChars;
			for (int i = 0; i < seIconChars.Length; i++)
			{
				char value = seIconChars[i];
				string text = value.ToString();
				TextButtonNode textButtonNode = CreateGlyphButton(text, $"0x{value:X4}", delegate
				{
					CopyText(text);
				});
				if (num + horizontalListNode.ItemSpacing + textButtonNode.Width > contentList.ContentNode.Width)
				{
					contentList.ContentNode.AddNode(horizontalListNode);
					horizontalListNode = CreateGlyphRow();
					num = 0f;
				}
				horizontalListNode.AddNode(textButtonNode);
				num += horizontalListNode.ItemSpacing + textButtonNode.Width;
			}
			if (horizontalListNode.Nodes.Count > 0)
			{
				contentList.ContentNode.AddNode(horizontalListNode);
			}
			static TextButtonNode CreateGlyphButton(string text2, string tooltip, Action onClick)
			{
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				TextButtonNode textButtonNode2 = new TextButtonNode();
				textButtonNode2.IsVisible = true;
				textButtonNode2.IsEnabled = true;
				textButtonNode2.Size = new Vector2(48f, 32f);
				textButtonNode2.String = ReadOnlySeString.op_Implicit(text2);
				textButtonNode2.TextTooltip = ReadOnlySeString.op_Implicit(tooltip);
				textButtonNode2.OnClick = onClick;
				textButtonNode2.LabelNode.FontSize = 18u;
				return textButtonNode2;
			}
			HorizontalListNode CreateGlyphRow()
			{
				return new HorizontalListNode
				{
					IsVisible = true,
					Size = new Vector2(contentList.ContentNode.Width, 36f),
					ItemSpacing = 4f,
					FirstItemSpacing = 0f,
					FitToContentHeight = true
				};
			}
		}

		private unsafe void BuildSettingsTab(ScrollingNode<VerticalListNode> contentList)
		{
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0317: Unknown result type (might be due to invalid IL or missing references)
			//IL_040f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0460: Unknown result type (might be due to invalid IL or missing references)
			//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_059e: Unknown result type (might be due to invalid IL or missing references)
			//IL_069d: Unknown result type (might be due to invalid IL or missing references)
			//IL_083e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0894: Unknown result type (might be due to invalid IL or missing references)
			//IL_0977: Unknown result type (might be due to invalid IL or missing references)
			float contentWidth = contentList.ContentNode.Width - 12f;
			CollaspingNode tree = null;
			tree = new CollaspingNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 0f),
				CategoryVerticalSpacing = 5f
			};
			tree.OnLayoutUpdate = delegate(float height)
			{
				if (tree != null)
				{
					tree.Height = height;
					contentList.RecalculateSizes();
				}
			};
			VerticalListNode verticalListNode = new VerticalListNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 0f),
				FitContents = true,
				FitWidth = true,
				ItemSpacing = 5f,
				FirstItemSpacing = 10f
			};
			verticalListNode.AddNode(new TextNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 22f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Offset")),
				FontSize = 13u,
				AlignmentType = (AlignmentType)3
			});
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 34f),
				ItemSpacing = 6f,
				FirstItemSpacing = 0f,
				FitToContentHeight = true
			};
			horizontalListNode.AddNode(OffsetInput("X", (int)instance.config.OverlayOffset.X, delegate(int num)
			{
				Config config = instance.config;
				Vector2 overlayOffset = instance.config.OverlayOffset;
				overlayOffset.X = num;
				config.OverlayOffset = overlayOffset;
			}));
			horizontalListNode.AddNode(OffsetInput("Y", (int)instance.config.OverlayOffset.Y, delegate(int num)
			{
				Config config = instance.config;
				Vector2 overlayOffset = instance.config.OverlayOffset;
				overlayOffset.Y = num;
				config.OverlayOffset = overlayOffset;
			}));
			verticalListNode.AddNode(horizontalListNode);
			tree.AddCategoryNode(new CollaspingCategoryNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 28f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("General"))
			});
			ReadOnlyCollection<CollaspingCategoryNode> categoryNodes = tree.CategoryNodes;
			categoryNodes[categoryNodes.Count - 1].AddNode(verticalListNode);
			VerticalListNode verticalListNode2 = new VerticalListNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 0f),
				FitContents = true,
				FitWidth = true,
				ItemSpacing = 5f
			};
			HorizontalListNode horizontalListNode2 = new HorizontalListNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 36f),
				ItemSpacing = 6f,
				FirstItemSpacing = 0f,
				FitToContentHeight = true
			};
			TextInputNode messageInputNode = new TextInputNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth - 84f, 32f),
				MaxCharacters = 1000
			};
			horizontalListNode2.AddNode(messageInputNode);
			TextButtonNode textButtonNode = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(78f, 32f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Add")),
				OnClick = delegate
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					string text2 = ((object)messageInputNode.String/*cast due to constrained. prefix*/).ToString();
					if (!string.IsNullOrWhiteSpace(text2) && !instance.config.SavedMessages.Contains(text2))
					{
						instance.config.SavedMessages.Add(text2);
						instance.config.Save(instance);
						((AtkStage)AtkStage.Instance()).ClearFocus();
					}
				}
			};
			textButtonNode.LabelNode.AutoAdjustTextSize();
			horizontalListNode2.AddNode(textButtonNode);
			verticalListNode2.AddNode(horizontalListNode2);
			foreach (string message in instance.config.SavedMessages.ToList())
			{
				HorizontalListNode horizontalListNode3 = new HorizontalListNode
				{
					IsVisible = true,
					Size = new Vector2(contentWidth, 34f),
					ItemSpacing = 6f,
					FirstItemSpacing = 0f,
					FitToContentHeight = true
				};
				horizontalListNode3.AddNode(new TextNode
				{
					IsVisible = true,
					Size = new Vector2(contentWidth - 86f, 30f),
					String = ReadOnlySeString.op_Implicit(message),
					AlignmentType = (AlignmentType)3,
					FontSize = 14u
				});
				TextButtonNode textButtonNode2 = new TextButtonNode
				{
					IsVisible = true,
					IsEnabled = true,
					Size = new Vector2(80f, 30f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Delete")),
					OnClick = delegate
					{
						instance.config.SavedMessages.Remove(message);
						instance.config.Save(instance);
					}
				};
				textButtonNode2.LabelNode.AutoAdjustTextSize();
				horizontalListNode3.AddNode(textButtonNode2);
				verticalListNode2.AddNode(horizontalListNode3);
			}
			tree.AddCategoryNode(new CollaspingCategoryNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 28f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("QuickChatPanel-Messages")),
				IsCollapsed = true
			});
			ReadOnlyCollection<CollaspingCategoryNode> categoryNodes2 = tree.CategoryNodes;
			categoryNodes2[categoryNodes2.Count - 1].AddNode(verticalListNode2);
			VerticalListNode macrosSection = new VerticalListNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 0f),
				FitContents = true,
				FitWidth = true,
				ItemSpacing = 5f,
				FirstItemSpacing = 10f
			};
			macrosSection.AddNode(new TextNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 22f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("QuickChatPanel-MacroButton-DisplayType")),
				FontSize = 13u,
				AlignmentType = (AlignmentType)3
			});
			StringDropDownNode stringDropDownNode = new StringDropDownNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 30f),
				MaxListOptions = MacroDisplayModeLoc.Count,
				Options = MacroDisplayModeLoc.Values.ToList()
			};
			stringDropDownNode.SelectedOption = MacroDisplayModeLoc[instance.config.OverlayMacroDisplayMode];
			stringDropDownNode.OnOptionSelected = delegate(string text2)
			{
				MacroDisplayMode key3 = MacroDisplayModeLoc.FirstOrDefault<KeyValuePair<MacroDisplayMode, string>>((KeyValuePair<MacroDisplayMode, string> x) => x.Value == text2).Key;
				if (key3 != instance.config.OverlayMacroDisplayMode)
				{
					instance.config.OverlayMacroDisplayMode = key3;
					instance.config.Save(instance);
				}
			};
			macrosSection.AddNode(stringDropDownNode);
			macrosSection.AddDummy(5f);
			AddMacroSection(isIndividual: true);
			AddMacroSection(isIndividual: false);
			tree.AddCategoryNode(new CollaspingCategoryNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 28f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("QuickChatPanel-Macro")),
				IsCollapsed = true
			});
			ReadOnlyCollection<CollaspingCategoryNode> categoryNodes3 = tree.CategoryNodes;
			categoryNodes3[categoryNodes3.Count - 1].AddNode(macrosSection);
			VerticalListNode verticalListNode3 = new VerticalListNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 0f),
				FitContents = true,
				FitWidth = true,
				ItemSpacing = 5f
			};
			foreach (KeyValuePair<uint, string> item in instance.config.SoundEffectNotes.OrderBy<KeyValuePair<uint, string>, uint>((KeyValuePair<uint, string> x) => x.Key))
			{
				item.Deconstruct(out var key, out var value);
				uint key2 = key;
				string text = value;
				HorizontalListNode horizontalListNode4 = new HorizontalListNode
				{
					IsVisible = true,
					Size = new Vector2(contentWidth, 34f),
					ItemSpacing = 6f,
					FirstItemSpacing = 0f,
					FitToContentHeight = true
				};
				horizontalListNode4.AddNode(new TextNode
				{
					IsVisible = true,
					Size = new Vector2(58f, 30f),
					String = ReadOnlySeString.op_Implicit($"<se.{key2}>"),
					AlignmentType = (AlignmentType)3,
					FontSize = 14u
				});
				TextInputNode input = new TextInputNode
				{
					IsVisible = true,
					Size = new Vector2(contentWidth - 64f, 30f),
					String = ReadOnlySeString.op_Implicit(text),
					MaxCharacters = 32
				};
				input.OnInputReceived = delegate(ReadOnlySeString val)
				{
					instance.config.SoundEffectNotes[key2] = ((object)(*(ReadOnlySeString*)(&val))/*cast due to constrained. prefix*/).ToString();
				};
				input.OnInputComplete = delegate(ReadOnlySeString val)
				{
					SaveSoundEffectNote(key2, ((object)(*(ReadOnlySeString*)(&val))/*cast due to constrained. prefix*/).ToString());
				};
				input.OnFocusLost = delegate
				{
					//IL_001c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					SaveSoundEffectNote(key2, ((object)input.String/*cast due to constrained. prefix*/).ToString());
				};
				input.CurrentTextNode.FontSize = 14u;
				horizontalListNode4.AddNode(input);
				verticalListNode3.AddNode(horizontalListNode4);
			}
			tree.AddCategoryNode(new CollaspingCategoryNode
			{
				IsVisible = true,
				Size = new Vector2(contentWidth, 28f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("QuickChatPanel-SystemSound")),
				IsCollapsed = true
			});
			ReadOnlyCollection<CollaspingCategoryNode> categoryNodes4 = tree.CategoryNodes;
			categoryNodes4[categoryNodes4.Count - 1].AddNode(verticalListNode3);
			contentList.ContentNode.AddNode(tree);
			tree.RefreshLayout();
			contentList.RecalculateSizes();
			unsafe void AddMacroSection(bool isIndividual)
			{
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_009d: Unknown result type (might be due to invalid IL or missing references)
				RaptureMacroModule* ptr = RaptureMacroModule.Instance();
				macrosSection.AddNode(new TextNode
				{
					IsVisible = true,
					Size = new Vector2(contentWidth, 22f),
					String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(isIndividual ? 17337u : 17338u)),
					FontSize = 13u,
					AlignmentType = (AlignmentType)3
				});
				Span<Macro> span = (isIndividual ? ((RaptureMacroModule)ptr).Individual : ((RaptureMacroModule)ptr).Shared);
				for (int i = 0; i < span.Length; i++)
				{
					Macro* pointer = SpanExtensions.GetPointer<Macro>(span, i);
					if (pointer != null && !string.IsNullOrEmpty(((object)(*(Utf8String*)(&((Macro)pointer).Name))/*cast due to constrained. prefix*/).ToString()))
					{
						SavedMacro savedMacro = (*pointer).ToSavedMacro();
						savedMacro.Position = i;
						savedMacro.Category = ((!isIndividual) ? 1u : 0u);
						macrosSection.AddNode(MacroRow(savedMacro));
					}
				}
			}
			HorizontalListNode MacroRow(SavedMacro macro)
			{
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_0297: Unknown result type (might be due to invalid IL or missing references)
				//IL_0227: Unknown result type (might be due to invalid IL or missing references)
				bool flag = instance.config.SavedMacros.Contains(macro);
				HorizontalListNode horizontalListNode5 = new HorizontalListNode
				{
					IsVisible = true,
					Size = new Vector2(contentWidth, 42f),
					ItemSpacing = 6f,
					FirstItemSpacing = 0f,
					FitToContentHeight = true
				};
				SimpleComponentNode simpleComponentNode = new SimpleComponentNode
				{
					IsVisible = true,
					Size = new Vector2(contentWidth - (flag ? 172f : 86f), 38f)
				};
				IconImageNode iconImageNode = new IconImageNode();
				iconImageNode.IsVisible = true;
				iconImageNode.Position = new Vector2(4f, 4f);
				iconImageNode.Size = new Vector2(28f);
				iconImageNode.IconId = macro.IconID;
				iconImageNode.FitTexture = true;
				iconImageNode.AttachNode((NodeBase?)simpleComponentNode, NodePosition.AsLastChild);
				TextNode textNode = new TextNode();
				textNode.IsVisible = true;
				textNode.Position = new Vector2(38f, 0f);
				textNode.Size = new Vector2(simpleComponentNode.Width - 42f, 36f);
				textNode.String = ReadOnlySeString.op_Implicit(macro.Name);
				textNode.AlignmentType = (AlignmentType)3;
				textNode.FontSize = 14u;
				textNode.TextFlags = (TextFlags)2;
				textNode.AttachNode((NodeBase?)simpleComponentNode, NodePosition.AsLastChild);
				horizontalListNode5.AddNode(simpleComponentNode);
				if (flag)
				{
					TextButtonNode textButtonNode3 = new TextButtonNode
					{
						IsVisible = true,
						IsEnabled = true,
						Size = new Vector2(80f, 34f),
						String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Refresh")),
						TextTooltip = ReadOnlySeString.op_Implicit($"{LanguageManager.Get("QuickChatPanel-LastUpdateTime")}: {instance.config.SavedMacros.Find((SavedMacro x) => x.Equals(macro))?.LastUpdateTime}")
					};
					textButtonNode3.OnClick = delegate
					{
						int num = instance.config.SavedMacros.IndexOf(macro);
						if (num >= 0)
						{
							instance.config.SavedMacros[num] = macro;
							instance.config.Save(instance);
						}
					};
					textButtonNode3.LabelNode.AutoAdjustTextSize();
					horizontalListNode5.AddNode(textButtonNode3);
				}
				TextButtonNode textButtonNode4 = new TextButtonNode
				{
					IsVisible = true,
					IsEnabled = true,
					Size = new Vector2(80f, 34f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get(flag ? "Delete" : "Add"))
				};
				textButtonNode4.OnClick = delegate
				{
					if (!instance.config.SavedMacros.Remove(macro))
					{
						instance.config.SavedMacros.Add(macro);
					}
					instance.config.Save(instance);
				};
				textButtonNode4.LabelNode.AutoAdjustTextSize();
				horizontalListNode5.AddNode(textButtonNode4);
				return horizontalListNode5;
			}
			HorizontalListNode OffsetInput(string label, int num, Action<int> updateValue)
			{
				//IL_0092: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
				HorizontalListNode horizontalListNode5 = new HorizontalListNode();
				horizontalListNode5.IsVisible = true;
				horizontalListNode5.Size = new Vector2(154f, 32f);
				horizontalListNode5.ItemSpacing = 4f;
				horizontalListNode5.FirstItemSpacing = 0f;
				horizontalListNode5.FitToContentHeight = true;
				horizontalListNode5.AddNode(new TextNode
				{
					IsVisible = true,
					Position = new Vector2(0f, 2f),
					Size = new Vector2(18f, 20f),
					TextFlags = (TextFlags)1,
					String = ReadOnlySeString.op_Implicit(label),
					AlignmentType = (AlignmentType)3,
					FontSize = 14u
				});
				TextInputNode input2 = new TextInputNode
				{
					IsVisible = true,
					Size = new Vector2(132f, 30f),
					Position = new Vector2(0f, -4f),
					String = ReadOnlySeString.op_Implicit(num.ToString())
				};
				input2.CurrentTextNode.FontSize = 14u;
				input2.OnFocusLost = delegate
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					if (int.TryParse(ReadOnlySeString.op_Implicit(input2.String), out var result))
					{
						updateValue(result);
						instance.config.Save(instance);
					}
				};
				horizontalListNode5.AddNode(input2);
				return horizontalListNode5;
			}
			void SaveSoundEffectNote(uint key3, string text2)
			{
				if (!(instance.config.SoundEffectNotes.GetValueOrDefault(key3) == text2))
				{
					instance.config.SoundEffectNotes[key3] = text2;
					instance.config.Save(instance);
				}
			}
		}

		private static void AddEmptyState(ScrollingNode<VerticalListNode> contentList, string text, string? detail = null)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			VerticalListNode verticalListNode = new VerticalListNode
			{
				IsVisible = true,
				Size = new Vector2(contentList.ContentNode.Width, 72f),
				ItemSpacing = 2f,
				FitWidth = true
			};
			verticalListNode.AddNode(new TextNode
			{
				IsVisible = true,
				Size = new Vector2(contentList.ContentNode.Width, 30f),
				String = ReadOnlySeString.op_Implicit(text),
				FontSize = 16u,
				TextColor = ColorHelper.GetColor(3u),
				AlignmentType = (AlignmentType)4
			});
			if (!string.IsNullOrWhiteSpace(detail))
			{
				verticalListNode.AddNode(new TextNode
				{
					IsVisible = true,
					Size = new Vector2(contentList.ContentNode.Width, 24f),
					String = ReadOnlySeString.op_Implicit(detail),
					TextColor = ColorHelper.GetColor(3u),
					AlignmentType = (AlignmentType)4
				});
			}
			contentList.ContentNode.AddNode(verticalListNode);
		}

		private static HorizontalListNode CreateCardRow(ScrollingNode<VerticalListNode> contentList)
		{
			return new HorizontalListNode
			{
				IsVisible = true,
				Size = new Vector2(contentList.ContentNode.Width, 82f),
				ItemSpacing = 8f,
				FirstItemSpacing = 0f,
				FitToContentHeight = true
			};
		}

		private static HorizontalListNode CreateCompactRow(ScrollingNode<VerticalListNode> contentList)
		{
			return new HorizontalListNode
			{
				IsVisible = true,
				Size = new Vector2(contentList.ContentNode.Width, 38f),
				ItemSpacing = 6f,
				FirstItemSpacing = 0f,
				FitToContentHeight = true
			};
		}

		private static TextButtonNode CreateTextActionRow(ScrollingNode<VerticalListNode> contentList, string text, string tooltip, Action onClick, ReceiveEvent? onMouseClick = null)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			TextButtonNode textButtonNode = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(contentList.ContentNode.Width, 32f),
				String = ReadOnlySeString.op_Implicit(text),
				TextTooltip = ReadOnlySeString.op_Implicit(tooltip),
				OnClick = onClick
			};
			textButtonNode.LabelNode.AlignmentType = (AlignmentType)3;
			textButtonNode.LabelNode.FontSize = 14u;
			if (onMouseClick != null)
			{
				textButtonNode.AddEvent((AtkEventType)9, onMouseClick);
			}
			return textButtonNode;
		}

		private static TextButtonNode CreateCompactTextButton(string text, string tooltip, Action onClick, ReceiveEvent? onMouseClick = null)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			TextButtonNode textButtonNode = new TextButtonNode
			{
				IsVisible = true,
				IsEnabled = true,
				Size = new Vector2(116f, 34f),
				String = ReadOnlySeString.op_Implicit(text),
				TextTooltip = ReadOnlySeString.op_Implicit(tooltip),
				OnClick = onClick
			};
			textButtonNode.LabelNode.AutoAdjustTextSize();
			if (onMouseClick != null)
			{
				textButtonNode.AddEvent((AtkEventType)9, onMouseClick);
			}
			return textButtonNode;
		}
	}

	public class SavedMacro : IEquatable<SavedMacro>
	{
		public uint Category { get; set; }

		public int Position { get; set; }

		public string Name { get; set; } = string.Empty;

		public uint IconID { get; set; }

		public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;

		public bool Equals(SavedMacro? other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (Category == other.Category)
			{
				return Position == other.Position;
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() == GetType())
			{
				return Equals((SavedMacro)obj);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Category, Position);
		}
	}

	private enum MacroDisplayMode
	{
		List,
		Buttons
	}

	private enum QuickChatTab
	{
		Messages,
		Macros,
		SystemSounds,
		GameItems,
		SpecialIconChars,
		Settings
	}

	private Config config;

	private int dropMacroIndex = -1;

	private int dropMessageIndex = -1;

	private TextButtonNode? sendButton;

	private QuickChatPanelAddon? chatPanelAddon;

	private static readonly FrozenDictionary<MacroDisplayMode, string> MacroDisplayModeLoc = new Dictionary<MacroDisplayMode, string>
	{
		[MacroDisplayMode.List] = LanguageManager.Get("QuickChatPanel-List"),
		[MacroDisplayMode.Buttons] = LanguageManager.Get("QuickChatPanel-Buttons")
	}.ToFrozenDictionary();

	private static readonly char[] SeIconChars = (from x in Enum.GetValues<SeIconChar>()
		select (char)x).ToArray();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("QuickChatPanelTitle"),
		Description = LanguageManager.Get("QuickChatPanelDescription"),
		Category = ModuleCategory.Interface
	};

	protected override void Init()
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (config.SoundEffectNotes.Count <= 0)
		{
			for (uint num = 1u; num < 17; num++)
			{
				config.SoundEffectNotes[num] = $"<se.{num}>";
			}
		}
		chatPanelAddon = new QuickChatPanelAddon(this)
		{
			InternalName = "DRQuickChatPanel",
			Title = ReadOnlySeString.op_Implicit(Info.Title),
			Size = new Vector2(542f, 400f),
			RememberClosePosition = false,
			DisableClose = true
		};
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "ChatLog", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "ChatLog", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "ChatLog", new AddonEventDelegate(OnAddon));
	}

	protected unsafe override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		sendButton?.Dispose();
		sendButton = null;
		chatPanelAddon?.Dispose();
		chatPanelAddon = null;
		if (Addons.ChatLog == null)
		{
			return;
		}
		AtkComponentNode* componentNodeById = ((AtkUnitBase)Addons.ChatLog).GetComponentNodeById(5u);
		if (componentNodeById == null)
		{
			return;
		}
		AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(17u);
		if (ptr == null)
		{
			return;
		}
		AtkResNode* ptr2 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(16u);
		if (ptr2 != null)
		{
			AtkResNode* rootNode = ((AtkUnitBase)Addons.ChatLog).RootNode;
			if (rootNode != null)
			{
				ushort width = (ushort)(((AtkResNode)rootNode).Width - 38);
				((AtkResNode)ptr).SetWidth(width);
				((AtkResNode)ptr2).SetWidth(width);
				((AtkComponentNode)componentNodeById).SetWidth(width);
			}
		}
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("QuickChatPanel-Messages")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			DrawMessageOrder();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("QuickChatPanel-Macro")));
		val = ImRaii.PushIndent(1, true);
		try
		{
			DrawMacroOrder();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		void DrawMacroOrder()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			if (config.SavedMacros.Count == 0)
			{
				ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get("QuickChatPanel-SavedMacrosAmountText", 0)));
			}
			else
			{
				ImU8String val2 = default(ImU8String);
				for (int i = 0; i < config.SavedMacros.Count; i++)
				{
					SavedMacro savedMacro = config.SavedMacros[i];
					((ImU8String)(ref val2))._002Ector(28, 3);
					((ImU8String)(ref val2)).AppendFormatted<int>(i + 1);
					((ImU8String)(ref val2)).AppendLiteral(". ");
					((ImU8String)(ref val2)).AppendFormatted<string>(savedMacro.Name);
					((ImU8String)(ref val2)).AppendLiteral("##QuickChatPanelMacroOrder");
					((ImU8String)(ref val2)).AppendFormatted<int>(i);
					ImGui.Button(val2, new Vector2(320f * Globals.GlobalUIScale, 0f));
					DragDropSourceDisposable val3 = ImRaii.DragDropSource();
					try
					{
						if (DragDropSourceDisposable.op_Implicit(val3))
						{
							if (ImGui.SetDragDropPayload(ImU8String.op_Implicit("QuickChatPanelMacroReorder"), default(ReadOnlySpan<byte>), (ImGuiCond)0))
							{
								dropMacroIndex = i;
							}
							Vector4 vector2 = KnownColor.Chocolate.ToVector4();
							ImGui.TextColored(ref vector2, ImU8String.op_Implicit(savedMacro.Name));
						}
					}
					finally
					{
						((DragDropSourceDisposable)(ref val3)).Dispose();
					}
					DragDropTargetDisposable val4 = ImRaii.DragDropTarget();
					try
					{
						if (!(!val4))
						{
							ImGui.AcceptDragDropPayload(ImU8String.op_Implicit("QuickChatPanelMacroReorder"), (ImGuiDragDropFlags)0);
							if (ImGui.IsMouseReleased((ImGuiMouseButton)0) && dropMacroIndex >= 0)
							{
								List<SavedMacro> savedMacros = config.SavedMacros;
								int index = dropMacroIndex;
								List<SavedMacro> savedMacros2 = config.SavedMacros;
								int index2 = i;
								SavedMacro value = config.SavedMacros[i];
								SavedMacro value2 = config.SavedMacros[dropMacroIndex];
								savedMacros[index] = value;
								savedMacros2[index2] = value2;
								dropMacroIndex = -1;
								config.Save(this);
							}
						}
					}
					finally
					{
						((DragDropTargetDisposable)(ref val4)).Dispose();
					}
				}
			}
		}
		void DrawMessageOrder()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			if (config.SavedMessages.Count == 0)
			{
				ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get("QuickChatPanel-SavedMessagesAmountText", 0)));
			}
			else
			{
				ImU8String val2 = default(ImU8String);
				for (int i = 0; i < config.SavedMessages.Count; i++)
				{
					string text = config.SavedMessages[i];
					((ImU8String)(ref val2))._002Ector(30, 3);
					((ImU8String)(ref val2)).AppendFormatted<int>(i + 1);
					((ImU8String)(ref val2)).AppendLiteral(". ");
					((ImU8String)(ref val2)).AppendFormatted<string>(text);
					((ImU8String)(ref val2)).AppendLiteral("##QuickChatPanelMessageOrder");
					((ImU8String)(ref val2)).AppendFormatted<int>(i);
					ImGui.Button(val2, new Vector2(320f * Globals.GlobalUIScale, 0f));
					DragDropSourceDisposable val3 = ImRaii.DragDropSource();
					try
					{
						if (DragDropSourceDisposable.op_Implicit(val3))
						{
							if (ImGui.SetDragDropPayload(ImU8String.op_Implicit("QuickChatPanelMessageReorder"), default(ReadOnlySpan<byte>), (ImGuiCond)0))
							{
								dropMessageIndex = i;
							}
							Vector4 vector2 = KnownColor.Chocolate.ToVector4();
							ImGui.TextColored(ref vector2, ImU8String.op_Implicit(text));
						}
					}
					finally
					{
						((DragDropSourceDisposable)(ref val3)).Dispose();
					}
					DragDropTargetDisposable val4 = ImRaii.DragDropTarget();
					try
					{
						if (!(!val4))
						{
							ImGui.AcceptDragDropPayload(ImU8String.op_Implicit("QuickChatPanelMessageReorder"), (ImGuiDragDropFlags)0);
							if (ImGui.IsMouseReleased((ImGuiMouseButton)0) && dropMessageIndex >= 0)
							{
								List<string> savedMessages = config.SavedMessages;
								int index = dropMessageIndex;
								List<string> savedMessages2 = config.SavedMessages;
								int index2 = i;
								string value = config.SavedMessages[i];
								string value2 = config.SavedMessages[dropMessageIndex];
								savedMessages[index] = value;
								savedMessages2[index2] = value2;
								dropMessageIndex = -1;
								config.Save(this);
							}
						}
					}
					finally
					{
						((DragDropTargetDisposable)(ref val4)).Dispose();
					}
				}
			}
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 1 && (int)type != 5)
		{
			if ((int)type == 6)
			{
				sendButton = null;
			}
		}
		else
		{
			if (Addons.ChatLog == null)
			{
				return;
			}
			AtkComponentNode* componentNodeById = ((AtkUnitBase)Addons.ChatLog).GetComponentNodeById(5u);
			if (componentNodeById == null)
			{
				return;
			}
			AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(17u);
			if (ptr == null)
			{
				return;
			}
			AtkResNode* ptr2 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(16u);
			if (ptr2 == null)
			{
				return;
			}
			AtkResNode* rootNode = ((AtkUnitBase)Addons.ChatLog).RootNode;
			if (rootNode == null)
			{
				return;
			}
			if (sendButton == null)
			{
				sendButton = new TextButtonNode
				{
					Size = new Vector2(64f, ((AtkComponentNode)componentNodeById).Height + 4),
					IsVisible = true,
					IsEnabled = true,
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Send")),
					TextTooltip = ReadOnlySeString.op_Implicit(Info.Title)
				};
				sendButton.AddEvent((AtkEventType)9, (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* data)
				{
					if (AtkEventDataExtension.get_IsRightClick(in *data))
					{
						chatPanelAddon.TogglePanel();
					}
					else if (AtkEventDataExtension.get_IsLeftClick(in *data) && !SendChatboxMessage())
					{
						chatPanelAddon.TogglePanel();
					}
				});
				sendButton.Position = new Vector2((float)(int)((AtkResNode)rootNode).Width - sendButton.Width - 40f, 0f);
				sendButton.LabelNode.AutoAdjustTextSize();
				sendButton?.AttachNode(componentNodeById);
			}
			((AtkResNode)ptr).SetWidth((ushort)((float)(int)((AtkResNode)rootNode).Width - sendButton.Width - 40f));
			((AtkResNode)ptr2).SetWidth((ushort)((float)(int)((AtkResNode)rootNode).Width - sendButton.Width - 40f));
			((AtkComponentNode)componentNodeById).SetWidth((ushort)((float)(int)((AtkResNode)rootNode).Width - sendButton.Width - 40f));
			sendButton.X = (float)(int)((AtkResNode)rootNode).Width - sendButton.Width - 40f;
			TextButtonNode? textButtonNode = sendButton;
			Vector2 size = sendButton.Size;
			size.Y = ((AtkComponentNode)componentNodeById).Height + 4;
			textButtonNode.Size = size;
			sendButton.String = ReadOnlySeString.op_Implicit(LanguageManager.Get(IsAnyTextInBlock() ? "Send" : "Open"));
		}
	}

	private unsafe static bool IsAnyTextInBlock()
	{
		if (Addons.ChatLog == null || !(*Addons.ChatLog).IsAddonAndNodesReady())
		{
			return false;
		}
		AtkComponentNode* nodeById = (AtkComponentNode*)((AtkUnitBase)Addons.ChatLog).GetNodeById(5u);
		if (nodeById == null)
		{
			return false;
		}
		AtkTextNode* asAtkTextNode = ((AtkResNode)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)nodeById).Component).UldManager)).SearchNodeById(16u)).GetAsAtkTextNode();
		if (asAtkTextNode == null)
		{
			return false;
		}
		if (string.IsNullOrWhiteSpace(((object)(*(Utf8String*)(&((AtkTextNode)asAtkTextNode).NodeText))/*cast due to constrained. prefix*/).ToString()))
		{
			return false;
		}
		return true;
	}

	private unsafe bool SendChatboxMessage()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.ChatLog == null || !(*Addons.ChatLog).IsAddonAndNodesReady())
		{
			return false;
		}
		AtkComponentNode* nodeById = (AtkComponentNode*)((AtkUnitBase)Addons.ChatLog).GetNodeById(5u);
		if (nodeById == null)
		{
			return false;
		}
		AtkTextNode* asAtkTextNode = ((AtkResNode)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)nodeById).Component).UldManager)).SearchNodeById(16u)).GetAsAtkTextNode();
		if (asAtkTextNode == null)
		{
			return false;
		}
		ReadOnlySeString val = default(ReadOnlySeString);
		((ReadOnlySeString)(ref val))._002Ector(Utf8String.op_Implicit(ref ((AtkTextNode)asAtkTextNode).NodeText));
		if (string.IsNullOrWhiteSpace(((object)(*(ReadOnlySeString*)(&val))/*cast due to constrained. prefix*/).ToString()))
		{
			return false;
		}
		OmenServiceBase<ChatManager>.Instance().SendMessage(ReadOnlySeString.op_Implicit(val));
		AtkComponentTextInput* component = (AtkComponentTextInput*)((AtkComponentNode)nodeById).Component;
		((Utf8String)(&((AtkComponentTextInput)component).EvaluatedString)).Clear();
		((Utf8String)(&((AtkComponentTextInput)component).RawString)).Clear();
		((Utf8String)(&((AtkComponentTextInput)component).AvailableLines)).Clear();
		((Utf8String)(&((AtkComponentTextInput)component).HighlightedAutoTranslateOptionColorPrefix)).Clear();
		((Utf8String)(&((AtkComponentTextInput)component).HighlightedAutoTranslateOptionColorSuffix)).Clear();
		((Utf8String)(&((AtkTextNode)asAtkTextNode).NodeText)).Clear();
		chatPanelAddon?.Close();
		return true;
	}

	private static void CopyText(string text)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetClipboardText(ImU8String.op_Implicit(text));
		OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("CopiedToClipboard") + ": " + text);
	}

	private void SendSavedMessage(string message)
	{
		OmenServiceBase<ChatManager>.Instance().SendMessage(message);
		chatPanelAddon?.Close();
	}

	private unsafe void ExecuteMacro(SavedMacro macro)
	{
		Macro* macro2 = ((RaptureMacroModule)RaptureMacroModule.Instance()).GetMacro(macro.Category, (uint)macro.Position);
		((RaptureShellModule)RaptureShellModule.Instance()).ExecuteMacro(macro2);
		chatPanelAddon?.Close();
	}

	private static void SendGameItemLink(Item item)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(new SeStringBuilder().AddItemLink(((Item)(ref item)).RowId, PluginConfig.Instance().ConflictKeyBinding.IsPressed(), (string)null).Build().Encode()));
	}
}
