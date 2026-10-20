using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Addons;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.BetterPartyFilter;

public class BetterPartyFinderFilter : ModuleBase
{
	private class BetterPartyFinderFilterAddon(BetterPartyFinderFilter module) : AttachedAddon("LookingForGroup")
	{
		private class RegexRow
		{
			public HorizontalListNode Row { get; set; }

			public CheckboxNode Checkbox { get; set; }

			public TextInputNode TextInput { get; set; }

			public TextButtonNode DeleteButton { get; set; }
		}

		private readonly List<RegexRow> regexRows = new List<RegexRow>();

		private TabBarNode tabBar1;

		private TabBarNode tabBar2;

		private static readonly FieldInfo RadioButtonsField = typeof(TabBarNode).GetField("radioButtons", BindingFlags.Instance | BindingFlags.NonPublic);

		private VerticalListNode generalPanel;

		private VerticalListNode highEndPanel;

		private VerticalListNode descriptionPanel;

		private CheckboxNode ascCheckbox;

		private CheckboxNode desCheckbox;

		private CheckboxNode blacklistedCheckbox;

		private CheckboxNode lockedCheckbox;

		private VerticalListNode notifyLayout;

		private CheckboxNode notifyCheckbox;

		private NumericInputNode notifyIntervalInput;

		private CheckboxNode noNotifyWhenZeroCheckbox;

		private CheckboxNode autoModeCheckbox;

		private CheckboxNode manualModeCheckbox;

		private HorizontalListNode modeRow;

		private VerticalListNode numLayout;

		private CheckboxNode blacklistCheckbox;

		private CheckboxNode whitelistCheckbox;

		private VerticalListNode listContainer;

		private TextButtonNode prevPageBtn;

		private TextButtonNode nextPageBtn;

		private TextNode pageLabel;

		private int currentPageIndex;

		private int currentActiveTab;

		protected override AttachedAddonPosition AttachPosition => AttachedAddonPosition.LeftTop;

		protected override bool CanOpenAddon => module.isNeedToOpenAddon;

		private static void ClearTabBarSelection(TabBarNode bar)
		{
			if (!(RadioButtonsField.GetValue(bar) is List<TabBarRadioButtonNode> list))
			{
				return;
			}
			foreach (TabBarRadioButtonNode item in list)
			{
				item.IsChecked = false;
				item.IsSelected = false;
			}
		}

		protected unsafe override bool CanCloseHostAddon(AtkUnitBase* hostAddon)
		{
			return module.isNeedToOpenAddon;
		}

		protected unsafe override void OnDraw(AtkUnitBase* addon)
		{
			if ((*base.HostAddon).IsAddonAndNodesReady() && generalPanel.IsVisible)
			{
				ascCheckbox.IsChecked = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[4] == 1;
				desCheckbox.IsChecked = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[4] == 3;
				blacklistedCheckbox.IsChecked = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[12] == 1;
				lockedCheckbox.IsChecked = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[7] == 0;
				notifyCheckbox.IsChecked = NotifyNewRecruitment == 1;
				notifyIntervalInput.Value = (int)((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[5];
				noNotifyWhenZeroCheckbox.IsChecked = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[6] == 1;
				bool isChecked = notifyCheckbox.IsChecked;
				if (notifyLayout.IsVisible != isChecked)
				{
					notifyLayout.IsVisible = isChecked;
					notifyLayout.RecalculateLayout();
					RecalculatePanel(generalPanel);
				}
			}
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			currentPageIndex = 0;
			currentActiveTab = 0;
			if (base.WindowNode is WindowNode windowNode)
			{
				windowNode.CloseButtonNode.IsVisible = false;
			}
			SetupTabBars();
			SetupGeneralPanel();
			SetupHighEndPanel();
			SetupDescriptionPanel();
			SwitchTab(0);
			module.TaskHelper.Enqueue(delegate
			{
				ClearTabBarSelection(tabBar2);
			});
		}

		private void SetupTabBars()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			TabBarNode obj = new TabBarNode
			{
				Position = base.ContentStartPosition
			};
			Vector2 contentSize = base.ContentSize;
			contentSize.Y = 28f;
			obj.Size = contentSize;
			obj.IsVisible = true;
			tabBar1 = obj;
			tabBar1.AddTab(ReadOnlySeString.op_Implicit(LanguageManager.Get("General")), delegate
			{
				SwitchTab(0);
			});
			tabBar1.AddTab(ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(10822u)), delegate
			{
				SwitchTab(1);
			});
			tabBar1.AddTab(ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-Category-Description")), delegate
			{
				SwitchTab(2);
			});
			tabBar1.AttachNode(this);
			TabBarNode obj2 = new TabBarNode
			{
				Position = base.ContentStartPosition + new Vector2(0f, 28f)
			};
			contentSize = base.ContentSize;
			contentSize.Y = 28f;
			obj2.Size = contentSize;
			obj2.IsVisible = true;
			tabBar2 = obj2;
			tabBar2.AddTab(ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(11070u)), delegate
			{
				OnActionTabClicked(3);
			});
			tabBar2.AddTab(ReadOnlySeString.op_Implicit(LanguageManager.Get("Search")), delegate
			{
				OnActionTabClicked(4);
			});
			tabBar2.AddTab(ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-Category-SearchByName")), delegate
			{
				OnActionTabClicked(5);
			});
			tabBar2.AttachNode(this);
			UpdateTabButtons(tabBar1);
			UpdateTabButtons(tabBar2);
			static void UpdateTabButtons(TabBarNode tab)
			{
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				if (!(RadioButtonsField.GetValue(tab) is List<TabBarRadioButtonNode> list))
				{
					return;
				}
				foreach (TabBarRadioButtonNode item in list)
				{
					item.TextTooltip = item.String;
					TextNode labelNode = item.LabelNode;
					labelNode.TextFlags = (TextFlags)(labelNode.TextFlags | 0x400);
				}
			}
		}

		private unsafe void SetupGeneralPanel()
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0389: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_050e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0587: Unknown result type (might be due to invalid IL or missing references)
			generalPanel = new VerticalListNode
			{
				IsVisible = true,
				ItemSpacing = 8f,
				FirstItemSpacing = 16f,
				FitContents = true,
				FitWidth = true,
				Size = base.ContentSize
			};
			LabelTextNode node = new LabelTextNode
			{
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(11127u)),
				TextColor = ColorHelper.GetColor(2u)
			};
			generalPanel.AddNode(node);
			generalPanel.AddDummy();
			VerticalListNode verticalListNode = new VerticalListNode
			{
				FitContents = true,
				FitWidth = true,
				Position = new Vector2(20f, 0f)
			};
			CheckboxNode node2 = new CheckboxNode
			{
				Size = new Vector2(280f, 24f),
				IsVisible = true,
				IsChecked = module.config.FilterSameDescription,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-FilterDuplicate")),
				OnClick = delegate(bool isChecked)
				{
					module.config.FilterSameDescription = isChecked;
					module.config.Save(module);
				}
			};
			verticalListNode.AddNode(node2);
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				IsVisible = true,
				Size = new Vector2(280f, 24f)
			};
			ascCheckbox = new CheckboxNode
			{
				Size = new Vector2(135f, 24f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(10127u)),
				OnClick = delegate(bool isChecked)
				{
					if (isChecked)
					{
						((AgentId)130).SendEvent(1uL, 24, 0, 0);
						desCheckbox.IsChecked = false;
					}
					else
					{
						ascCheckbox.IsChecked = true;
					}
				}
			};
			horizontalListNode.AddNode(ascCheckbox);
			horizontalListNode.AddDummy(10f);
			desCheckbox = new CheckboxNode
			{
				Size = new Vector2(135f, 24f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(10128u)),
				OnClick = delegate(bool isChecked)
				{
					if (isChecked)
					{
						((AgentId)130).SendEvent(1uL, 24, 1, 0);
						ascCheckbox.IsChecked = false;
					}
					else
					{
						desCheckbox.IsChecked = true;
					}
				}
			};
			horizontalListNode.AddNode(desCheckbox);
			verticalListNode.AddNode(horizontalListNode);
			blacklistedCheckbox = new CheckboxNode
			{
				Size = new Vector2(280f, 24f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(11124u)),
				OnClick = delegate(bool isChecked)
				{
					bool value = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[7] == 0;
					RefreshDisplaySettings(isChecked, value);
				}
			};
			verticalListNode.AddNode(blacklistedCheckbox);
			lockedCheckbox = new CheckboxNode
			{
				Size = new Vector2(280f, 24f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(11128u)),
				OnClick = delegate(bool isChecked)
				{
					RefreshDisplaySettings(((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[12] == 1, isChecked);
				}
			};
			verticalListNode.AddNode(lockedCheckbox);
			generalPanel.AddNode(verticalListNode);
			LabelTextNode node3 = new LabelTextNode
			{
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(11116u)),
				TextColor = ColorHelper.GetColor(2u)
			};
			generalPanel.AddDummy(16f);
			generalPanel.AddNode(node3);
			VerticalListNode verticalListNode2 = new VerticalListNode
			{
				Position = new Vector2(20f, 0f),
				FitContents = true,
				FitWidth = true,
				FirstItemSpacing = 8f
			};
			notifyCheckbox = new CheckboxNode
			{
				Size = new Vector2(280f, 24f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(11119u)),
				OnClick = delegate(bool isChecked)
				{
					bool? notifyRecruitment = isChecked;
					RefreshDisplaySettings(null, null, notifyRecruitment);
					bool isVisible = NotifyNewRecruitment == 1;
					notifyLayout.IsVisible = isVisible;
					notifyLayout.RecalculateLayout();
					RecalculatePanel(generalPanel);
				}
			};
			verticalListNode2.AddNode(notifyCheckbox);
			notifyLayout = new VerticalListNode
			{
				Position = new Vector2(20f, 0f),
				FitContents = true,
				IsVisible = (NotifyNewRecruitment == 1)
			};
			LabelTextNode node4 = new LabelTextNode
			{
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(11117u)),
				Size = new Vector2(198f, 20f),
				Position = new Vector2(0f, 3f),
				TextColor = ColorHelper.GetColor(31u)
			};
			notifyIntervalInput = new NumericInputNode
			{
				Size = new Vector2(220f, 24f),
				Position = new Vector2(20f, 0f),
				Min = 1,
				Max = 10,
				Value = (int)((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[5],
				OnValueUpdate = delegate(int val)
				{
					uint? notifyInterval = (uint)val;
					RefreshDisplaySettings(null, null, null, notifyInterval);
				}
			};
			notifyLayout.AddNode(node4);
			notifyLayout.AddNode(notifyIntervalInput);
			noNotifyWhenZeroCheckbox = new CheckboxNode
			{
				Size = new Vector2(260f, 24f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(11118u)),
				OnClick = delegate(bool isChecked)
				{
					bool? noNotifyWhenZero = isChecked;
					RefreshDisplaySettings(null, null, null, null, noNotifyWhenZero);
				}
			};
			notifyLayout.AddDummy(12f);
			notifyLayout.AddNode(noNotifyWhenZeroCheckbox);
			verticalListNode2.AddNode(notifyLayout);
			LabelTextNode node5 = new LabelTextNode
			{
				TextFlags = (TextFlags)1,
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(11171u)),
				Position = new Vector2(0f, 3f),
				TextColor = ColorHelper.GetColor(3u),
				FontSize = 12u
			};
			verticalListNode2.AddDummy(12f);
			verticalListNode2.AddNode(node5);
			verticalListNode2.AddDummy(12f);
			generalPanel.AddNode(verticalListNode2);
			generalPanel.AttachNode(this);
		}

		private void SetupHighEndPanel()
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			highEndPanel = new VerticalListNode
			{
				IsVisible = false,
				ItemSpacing = 4f,
				FitContents = true,
				FitWidth = true,
				Size = base.ContentSize
			};
			CheckboxNode node = new CheckboxNode
			{
				Size = new Vector2(280f, 24f),
				IsVisible = true,
				IsChecked = module.config.HighEndFilterSameJob,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-HighEndFilter-SameJob")),
				OnClick = delegate(bool isChecked)
				{
					module.config.HighEndFilterSameJob = isChecked;
					module.config.Save(module);
				}
			};
			highEndPanel.AddNode(node);
			CheckboxNode node2 = new CheckboxNode
			{
				Size = new Vector2(280f, 24f),
				IsVisible = true,
				IsChecked = module.config.HighEndFilterRoleCount,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-HighEndFilter-RoleCount")),
				TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-HighEndFilter-RoleCount-Help")),
				OnClick = delegate(bool isChecked)
				{
					module.config.HighEndFilterRoleCount = isChecked;
					module.config.Save(module);
					UpdateHighEndContainerVisibility();
				}
			};
			highEndPanel.AddNode(node2);
			VerticalListNode verticalListNode = new VerticalListNode
			{
				Position = new Vector2(20f, 0f),
				FitContents = true,
				FitWidth = true
			};
			autoModeCheckbox = new CheckboxNode
			{
				Size = new Vector2(135f, 24f),
				IsVisible = module.config.HighEndFilterRoleCount,
				IsChecked = !module.manualMode,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("AutoMode")),
				TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-HighEndFilter-RoleCount-AutoMode-Help")),
				OnClick = delegate(bool isChecked)
				{
					if (isChecked)
					{
						module.manualMode = false;
						manualModeCheckbox.IsChecked = false;
					}
					else
					{
						autoModeCheckbox.IsChecked = true;
					}
				}
			};
			manualModeCheckbox = new CheckboxNode
			{
				Size = new Vector2(135f, 24f),
				IsVisible = module.config.HighEndFilterRoleCount,
				IsChecked = module.manualMode,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("ManualMode")),
				TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-HighEndFilter-RoleCount-ManualMode-Help")),
				OnClick = delegate(bool isChecked)
				{
					if (isChecked)
					{
						module.manualMode = true;
						autoModeCheckbox.IsChecked = false;
					}
					else
					{
						manualModeCheckbox.IsChecked = true;
					}
				}
			};
			modeRow = new HorizontalListNode
			{
				IsVisible = module.config.HighEndFilterRoleCount,
				Size = new Vector2(280f, 24f)
			};
			modeRow.AddNode(autoModeCheckbox);
			modeRow.AddDummy(10f);
			modeRow.AddNode(manualModeCheckbox);
			verticalListNode.AddNode(modeRow);
			numLayout = new VerticalListNode
			{
				IsVisible = module.config.HighEndFilterRoleCount,
				ItemSpacing = 4f,
				FitContents = true,
				FitWidth = true,
				Size = base.ContentSize
			};
			numLayout.AddNode(CreateRoleCountNumericInput(1082u, module.config.FilterJobTypeCountData.Tank, delegate(int val)
			{
				module.config.FilterJobTypeCountData.Tank = val;
				module.config.Save(module);
			}));
			numLayout.AddNode(CreateRoleCountNumericInput(11300u, module.config.FilterJobTypeCountData.PureHealer, delegate(int val)
			{
				module.config.FilterJobTypeCountData.PureHealer = val;
				module.config.Save(module);
			}));
			numLayout.AddNode(CreateRoleCountNumericInput(11301u, module.config.FilterJobTypeCountData.ShieldHealer, delegate(int val)
			{
				module.config.FilterJobTypeCountData.ShieldHealer = val;
				module.config.Save(module);
			}));
			numLayout.AddNode(CreateRoleCountNumericInput(1084u, module.config.FilterJobTypeCountData.Melee, delegate(int val)
			{
				module.config.FilterJobTypeCountData.Melee = val;
				module.config.Save(module);
			}));
			numLayout.AddNode(CreateRoleCountNumericInput(1085u, module.config.FilterJobTypeCountData.PhysicalRanged, delegate(int val)
			{
				module.config.FilterJobTypeCountData.PhysicalRanged = val;
				module.config.Save(module);
			}));
			numLayout.AddNode(CreateRoleCountNumericInput(1086u, module.config.FilterJobTypeCountData.MagicalRanged, delegate(int val)
			{
				module.config.FilterJobTypeCountData.MagicalRanged = val;
				module.config.Save(module);
			}));
			verticalListNode.AddDummy(4f);
			verticalListNode.AddNode(numLayout);
			highEndPanel.AddNode(verticalListNode);
			highEndPanel.AttachNode(this);
		}

		private void SetupDescriptionPanel()
		{
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_046f: Unknown result type (might be due to invalid IL or missing references)
			//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
			regexRows.Clear();
			descriptionPanel = new VerticalListNode
			{
				IsVisible = false,
				ItemSpacing = 8f,
				FirstItemSpacing = 8f,
				FitContents = true,
				FitWidth = true,
				Size = base.ContentSize
			};
			LabelTextNode node = new LabelTextNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Mode"))
			};
			descriptionPanel.AddNode(node);
			blacklistCheckbox = new CheckboxNode
			{
				Size = new Vector2(135f, 24f),
				IsVisible = true,
				IsChecked = !module.config.IsWhiteList,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Blacklist")),
				TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-Description-Blacklist-Help")),
				OnClick = delegate(bool isChecked)
				{
					if (isChecked)
					{
						module.config.IsWhiteList = false;
						module.config.Save(module);
						whitelistCheckbox.IsChecked = false;
					}
					else
					{
						blacklistCheckbox.IsChecked = true;
					}
				}
			};
			whitelistCheckbox = new CheckboxNode
			{
				Size = new Vector2(135f, 24f),
				IsVisible = true,
				IsChecked = module.config.IsWhiteList,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Whitelist")),
				TextTooltip = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-Description-Whitelist-Help")),
				OnClick = delegate(bool isChecked)
				{
					if (isChecked)
					{
						module.config.IsWhiteList = true;
						module.config.Save(module);
						blacklistCheckbox.IsChecked = false;
					}
					else
					{
						whitelistCheckbox.IsChecked = true;
					}
				}
			};
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				IsVisible = true,
				Size = new Vector2(280f, 24f),
				Position = new Vector2(16f, 0f)
			};
			horizontalListNode.AddNode(blacklistCheckbox);
			horizontalListNode.AddDummy(10f);
			horizontalListNode.AddNode(whitelistCheckbox);
			descriptionPanel.AddNode(horizontalListNode);
			TextButtonNode node2 = new TextButtonNode
			{
				Size = new Vector2(280f, 28f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Add") + " (" + LanguageManager.Get("Regex") + ")"),
				OnClick = delegate
				{
					module.config.BlackList.Add(new KeyValuePair<bool, string>(key: true, string.Empty));
					module.config.Save(module);
					int num2 = Math.Max(1, (int)Math.Ceiling((double)module.config.BlackList.Count / 10.0));
					currentPageIndex = num2 - 1;
					RebuildRegexList();
				}
			};
			descriptionPanel.AddNode(node2);
			listContainer = new VerticalListNode
			{
				IsVisible = true,
				ItemSpacing = 4f,
				FitContents = true,
				FitWidth = true,
				Size = base.ContentSize
			};
			descriptionPanel.AddNode(listContainer);
			for (int num = 0; num < 10; num++)
			{
				HorizontalListNode horizontalListNode2 = new HorizontalListNode
				{
					IsVisible = false,
					Size = new Vector2(280f, 32f)
				};
				CheckboxNode checkboxNode = new CheckboxNode
				{
					Size = new Vector2(28f, 28f),
					IsVisible = true,
					String = ReadOnlySeString.op_Implicit(string.Empty)
				};
				TextInputNode textInputNode = new TextInputNode
				{
					Size = new Vector2(290f, 32f),
					PlaceholderString = LanguageManager.Get("Regex")
				};
				TextButtonNode textButtonNode = new TextButtonNode
				{
					Size = new Vector2(42f, 28f),
					Position = new Vector2(0f, 3f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Delete"))
				};
				horizontalListNode2.AddNode(checkboxNode);
				horizontalListNode2.AddDummy(4f);
				horizontalListNode2.AddNode(textInputNode);
				horizontalListNode2.AddDummy(4f);
				horizontalListNode2.AddNode(textButtonNode);
				listContainer.AddNode(horizontalListNode2);
				regexRows.Add(new RegexRow
				{
					Row = horizontalListNode2,
					Checkbox = checkboxNode,
					TextInput = textInputNode,
					DeleteButton = textButtonNode
				});
			}
			HorizontalFlexNode obj = new HorizontalFlexNode
			{
				IsVisible = true
			};
			Vector2 contentSize = base.ContentSize;
			contentSize.Y = 28f;
			obj.Size = contentSize;
			obj.AlignmentFlags = FlexFlags.CenterHorizontally;
			obj.Position = new Vector2(0f, 6f);
			HorizontalFlexNode horizontalFlexNode = obj;
			prevPageBtn = new TextButtonNode
			{
				Size = new Vector2(40f, 24f),
				String = ReadOnlySeString.op_Implicit("<"),
				OnClick = delegate
				{
					if (currentPageIndex > 0)
					{
						currentPageIndex--;
						RebuildRegexList();
					}
				}
			};
			pageLabel = new TextNode
			{
				TextFlags = (TextFlags)1,
				String = ReadOnlySeString.op_Implicit("1 / 1"),
				AlignmentType = (AlignmentType)3,
				Position = new Vector2(0f, 3f)
			};
			nextPageBtn = new TextButtonNode
			{
				Size = new Vector2(40f, 24f),
				String = ReadOnlySeString.op_Implicit(">"),
				OnClick = delegate
				{
					int num2 = Math.Max(1, (int)Math.Ceiling((double)module.config.BlackList.Count / 10.0));
					if (currentPageIndex < num2 - 1)
					{
						currentPageIndex++;
						RebuildRegexList();
					}
				}
			};
			horizontalFlexNode.AddNode(prevPageBtn);
			horizontalFlexNode.AddNode(pageLabel);
			horizontalFlexNode.AddNode(nextPageBtn);
			descriptionPanel.AddNode(horizontalFlexNode);
			descriptionPanel.AttachNode(this);
		}

		private static HorizontalListNode CreateRoleCountNumericInput(uint addonTextID, int initialVal, Action<int> onValueUpdate)
		{
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				IsVisible = true,
				Size = new Vector2(310f, 28f)
			};
			SimpleNineGridNode simpleNineGridNode = addonTextID switch
			{
				1082u => new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(0f, 80f),
					TextureSize = new Vector2(28f),
					Size = new Vector2(28f),
					TexturePath = "ui/uld/img04/LFG_hr1.tex"
				}, 
				11300u => new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(0f, 56f),
					TextureSize = new Vector2(28f),
					Size = new Vector2(28f),
					TexturePath = "ui/uld/LFGSelectRole_hr1.tex"
				}, 
				11301u => new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(28f, 56f),
					TextureSize = new Vector2(28f),
					Size = new Vector2(28f),
					TexturePath = "ui/uld/LFGSelectRole_hr1.tex"
				}, 
				1084u => new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(0f),
					TextureSize = new Vector2(28f),
					Size = new Vector2(28f),
					TexturePath = "ui/uld/LFGSelectRole_hr1.tex"
				}, 
				1085u => new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(28f, 0f),
					TextureSize = new Vector2(28f),
					Size = new Vector2(28f),
					TexturePath = "ui/uld/LFGSelectRole_hr1.tex"
				}, 
				1086u => new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(56f, 0f),
					TextureSize = new Vector2(28f),
					Size = new Vector2(28f),
					TexturePath = "ui/uld/LFGSelectRole_hr1.tex"
				}, 
				_ => null, 
			};
			ArgumentNullException.ThrowIfNull((object?)simpleNineGridNode, "(object?)icon");
			horizontalListNode.AddNode(simpleNineGridNode);
			horizontalListNode.AddDummy(10f);
			LabelTextNode node = new LabelTextNode
			{
				String = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(addonTextID)),
				Size = new Vector2(208f, 20f),
				Position = new Vector2(0f, 3f)
			};
			NumericInputNode node2 = new NumericInputNode
			{
				Size = new Vector2(100f, 24f),
				Min = -1,
				Max = 8,
				Value = initialVal,
				OnValueUpdate = onValueUpdate
			};
			horizontalListNode.AddNode(node);
			horizontalListNode.AddDummy(10f);
			horizontalListNode.AddNode(node2);
			return horizontalListNode;
		}

		private void RecalculatePanel(LayoutListNode panel)
		{
			if (panel != null && panel.IsVisible)
			{
				panel.RecalculateLayout();
				SetWindowSize(400f, base.ContentStartPosition.Y + tabBar1.Height + tabBar2.Height + panel.Height + 24f);
				panel.Position = base.ContentStartPosition + new Vector2(0f, 62f);
				tabBar1.Position = base.ContentStartPosition;
				tabBar2.Position = base.ContentStartPosition + new Vector2(0f, 28f);
			}
		}

		private void SwitchTab(int tabIndex)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			currentActiveTab = tabIndex;
			switch (tabIndex)
			{
			case 0:
				tabBar1.SelectTab(ReadOnlySeString.op_Implicit(LanguageManager.Get("General")));
				break;
			case 1:
				tabBar1.SelectTab(ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(10822u)));
				break;
			case 2:
				tabBar1.SelectTab(ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterPartyFinderFilter-Category-Description")));
				break;
			}
			ClearTabBarSelection(tabBar2);
			generalPanel.IsVisible = tabIndex == 0;
			highEndPanel.IsVisible = tabIndex == 1;
			descriptionPanel.IsVisible = tabIndex == 2;
			switch (tabIndex)
			{
			case 0:
				RecalculatePanel(generalPanel);
				break;
			case 1:
				UpdateHighEndContainerVisibility();
				break;
			case 2:
				RebuildRegexList();
				break;
			}
		}

		private unsafe void OnActionTabClicked(int actionIndex)
		{
			switch (actionIndex)
			{
			case 3:
				if (Addons.LFGFilterSettings != null)
				{
					((AtkUnitBase)Addons.LFGFilterSettings).Close(true);
					break;
				}
				((AgentId)130).SendEvent(1uL, 25);
				break;
			case 4:
				if (Addons.LookingForGroupSearch != null)
				{
					((AtkUnitBase)Addons.LookingForGroupSearch).Close(true);
					break;
				}
				((AgentId)130).SendEvent(1uL, 15);
				break;
			case 5:
				if (Addons.LookingForGroupNameSearch != null)
				{
					((AtkUnitBase)Addons.LookingForGroupNameSearch).Close(true);
					break;
				}
				((AgentId)130).SendEvent(1uL, 19);
				((AgentId)130).SendEvent(10uL, 16);
				break;
			}
			SwitchTab(currentActiveTab);
			module.TaskHelper.Enqueue(delegate
			{
				ClearTabBarSelection(tabBar2);
			});
		}

		private void UpdateHighEndContainerVisibility()
		{
			bool highEndFilterRoleCount = module.config.HighEndFilterRoleCount;
			autoModeCheckbox.IsVisible = highEndFilterRoleCount;
			manualModeCheckbox.IsVisible = highEndFilterRoleCount;
			modeRow.IsVisible = highEndFilterRoleCount;
			numLayout.IsVisible = highEndFilterRoleCount;
			numLayout.RecalculateLayout();
			RecalculatePanel(highEndPanel);
		}

		private unsafe void RebuildRegexList()
		{
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			int count = module.config.BlackList.Count;
			int num = Math.Max(1, (int)Math.Ceiling((double)count / 10.0));
			if (currentPageIndex >= num)
			{
				currentPageIndex = num - 1;
			}
			if (currentPageIndex < 0)
			{
				currentPageIndex = 0;
			}
			List<KeyValuePair<bool, string>> list = module.config.BlackList.Skip(currentPageIndex * 10).Take(10).ToList();
			for (int i = 0; i < 10; i++)
			{
				RegexRow regexRow = regexRows[i];
				if (i < list.Count)
				{
					KeyValuePair<bool, string> localItem = list[i];
					int globalIndex = currentPageIndex * 10 + i;
					regexRow.Row.IsVisible = true;
					regexRow.Checkbox.IsChecked = localItem.Key;
					regexRow.Checkbox.OnClick = delegate(bool isChecked)
					{
						module.config.BlackList[globalIndex] = new KeyValuePair<bool, string>(isChecked, localItem.Value);
						module.config.Save(module);
					};
					regexRow.TextInput.String = ReadOnlySeString.op_Implicit(localItem.Value);
					regexRow.TextInput.OnInputComplete = delegate(ReadOnlySeString text)
					{
						module.HandleRegexUpdate(globalIndex, module.config.BlackList[globalIndex].Key, ((object)(*(ReadOnlySeString*)(&text))/*cast due to constrained. prefix*/).ToString());
					};
					regexRow.DeleteButton.OnClick = delegate
					{
						module.config.BlackList.RemoveAt(globalIndex);
						module.config.Save(module);
						int num2 = Math.Max(1, (int)Math.Ceiling((double)module.config.BlackList.Count / 10.0));
						if (currentPageIndex >= num2)
						{
							currentPageIndex = num2 - 1;
						}
						RebuildRegexList();
					};
				}
				else
				{
					regexRow.Row.IsVisible = false;
				}
			}
			prevPageBtn.IsEnabled = currentPageIndex > 0;
			nextPageBtn.IsEnabled = currentPageIndex < num - 1;
			pageLabel.String = ReadOnlySeString.op_Implicit($"{currentPageIndex + 1} / {num}");
			listContainer.RecalculateLayout();
			RecalculatePanel(descriptionPanel);
		}

		protected unsafe override void OnAttachedAddonUpdate(AtkUnitBase* addon, AtkUnitBase* hostAddon)
		{
			if (tabBar1 != null)
			{
				tabBar1.Position = base.ContentStartPosition;
				tabBar1.Width = base.ContentSize.X;
			}
			if (tabBar2 != null)
			{
				tabBar2.Position = base.ContentStartPosition + new Vector2(0f, 28f);
				tabBar2.Width = base.ContentSize.X;
			}
			switch (currentActiveTab)
			{
			case 0:
				if (generalPanel != null)
				{
					RecalculatePanel(generalPanel);
				}
				break;
			case 1:
				if (highEndPanel != null)
				{
					RecalculatePanel(highEndPanel);
				}
				break;
			case 2:
				if (descriptionPanel != null)
				{
					RecalculatePanel(descriptionPanel);
				}
				break;
			}
		}
	}

	private class Config : ModuleConfig
	{
		public List<KeyValuePair<bool, string>> BlackList = new List<KeyValuePair<bool, string>>();

		public (int Tank, int PureHealer, int ShieldHealer, int Melee, int PhysicalRanged, int MagicalRanged) FilterJobTypeCountData = (Tank: 2, PureHealer: 1, ShieldHealer: 1, Melee: 2, PhysicalRanged: 1, MagicalRanged: 2);

		public bool FilterSameDescription = true;

		public bool HighEndFilterRoleCount = true;

		public bool HighEndFilterSameJob = true;

		public bool IsWhiteList;

		public override string PreviousModuleName => "PartyFinderFilter";
	}

	private TextButtonNode? buttonNode;

	private bool isNeedToOpenAddon;

	private BetterPartyFinderFilterAddon? addon;

	private Config config;

	private int batchIndex;

	private bool isSecret;

	private bool isRaid;

	private bool manualMode;

	private readonly HashSet<(ushort, string)> descriptionSet = new HashSet<(ushort, string)>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterPartyFinderFilterTitle"),
		Description = LanguageManager.Get("BetterPartyFinderFilterDescription"),
		Category = ModuleCategory.Recruitment,
		Author = new string[1] { "status102" },
		PreviewImageURL = new string[3] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/BetterPartyFinderFilter/preview-1.png", "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/BetterPartyFinderFilter/preview-2.png", "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/BetterPartyFinderFilter/preview-3.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	private static uint NotifyNewRecruitment
	{
		get
		{
			return DalamudServiceExtension.Instance<IGameConfig>().UiConfig.GetUInt("PartyFinderNewArrivalDisp");
		}
		set
		{
			DalamudServiceExtension.Instance<IGameConfig>().UiConfig.Set("PartyFinderNewArrivalDisp", value);
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				buttonNode = null;
				isNeedToOpenAddon = false;
			}
		}
		else
		{
			if (Addons.LookingForGroup == null || buttonNode != null)
			{
				return;
			}
			buttonNode = new TextButtonNode
			{
				Size = new Vector2(154f, 32f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Filter")),
				Position = new Vector2(736f, 72f),
				OnClick = delegate
				{
					isNeedToOpenAddon = !isNeedToOpenAddon;
					if (!isNeedToOpenAddon)
					{
						addon.Close();
					}
				}
			};
			buttonNode.LabelNode.AutoAdjustTextSize();
			buttonNode.AttachNode(((AtkUnitBase)Addons.LookingForGroup).RootNode);
			for (uint num = 24u; num < 29; num++)
			{
				AtkResNode* nodeById = ((AtkUnitBase)Addons.LookingForGroup).GetNodeById(num);
				if (nodeById != null)
				{
					((AtkResNode)nodeById).ToggleVisibility(false);
				}
			}
		}
	}

	protected override void Init()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		DalamudServiceExtension.Instance<IPartyFinderGui>().ReceiveListing += new PartyFinderListingEventDelegate(OnReceiveListing);
		if (addon == null)
		{
			addon = new BetterPartyFinderFilterAddon(this)
			{
				InternalName = "DRBetterPartyFinderFilter",
				Title = ReadOnlySeString.op_Implicit(Info.Title),
				Size = new Vector2(400f, 220f),
				RememberClosePosition = false
			};
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "LookingForGroup", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "LookingForGroup", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IPartyFinderGui>().ReceiveListing -= new PartyFinderListingEventDelegate(OnReceiveListing);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		addon?.Dispose();
		addon = null;
		buttonNode?.Dispose();
		buttonNode = null;
		isNeedToOpenAddon = false;
	}

	private unsafe static void RefreshDisplaySettings(bool? displayBlacklisted = null, bool? displayLocked = null, bool? notifyRecruitment = null, uint? notifyInterval = null, bool? noNotifyWhenZero = null)
	{
		bool valueOrDefault = displayBlacklisted == true;
		if (!displayBlacklisted.HasValue)
		{
			valueOrDefault = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[12] == 1;
			displayBlacklisted = valueOrDefault;
		}
		valueOrDefault = displayLocked == true;
		if (!displayLocked.HasValue)
		{
			valueOrDefault = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[7] == 0;
			displayLocked = valueOrDefault;
		}
		uint valueOrDefault2 = notifyInterval.GetValueOrDefault();
		if (!notifyInterval.HasValue)
		{
			valueOrDefault2 = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[5];
			notifyInterval = valueOrDefault2;
		}
		valueOrDefault = noNotifyWhenZero == true;
		if (!noNotifyWhenZero.HasValue)
		{
			valueOrDefault = ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[6] == 1;
			noNotifyWhenZero = valueOrDefault;
		}
		valueOrDefault = notifyRecruitment == true;
		if (!notifyRecruitment.HasValue)
		{
			valueOrDefault = NotifyNewRecruitment == 1;
			notifyRecruitment = valueOrDefault;
		}
		int num = ((!displayBlacklisted.Value) ? 131072 : 0);
		int num2 = ((!displayLocked.Value) ? 65536 : 0);
		int num3 = (noNotifyWhenZero.Value ? 65536 : 0);
		int num4 = (notifyRecruitment.Value ? 1 : 0);
		int num5 = (displayBlacklisted.Value ? 1 : 0);
		int num6 = ((!displayLocked.Value) ? 1 : 0);
		int num7 = (noNotifyWhenZero.Value ? 1 : 0);
		int num8 = (notifyRecruitment.Value ? 1 : 0);
		if (((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[12] != num5 || ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[7] != num6 || ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[5] != notifyInterval.Value || ((FlagStatusModule)FlagStatusModule.Instance()).UIFlags[6] != num7 || NotifyNewRecruitment != num8)
		{
			((AgentId)130).SendEvent(13uL, 0, (uint)(num3 + notifyInterval.Value), (uint)(num + num2 + num4));
			NotifyNewRecruitment = (uint)num8;
		}
	}

	private void HandleRegexUpdate(int index, bool key, string value)
	{
		try
		{
			new Regex(value);
			config.BlackList[index] = new KeyValuePair<bool, string>(key, value);
			config.Save(this);
		}
		catch (ArgumentException)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(LanguageManager.Get("BetterPartyFinderFilter-RegexError"));
			config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		}
	}

	private void OnReceiveListing(IPartyFinderListing listing, IPartyFinderListingEventArgs args)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		if (batchIndex != args.BatchNumber)
		{
			isSecret = ((Enum)listing.SearchArea).HasFlag((Enum)(object)(SearchAreaFlags)2);
			isRaid = (int)listing.Category == 64;
			batchIndex = args.BatchNumber;
			descriptionSet.Clear();
		}
		if (!isSecret)
		{
			args.Visible &= FilterBySameDescription(listing);
			args.Visible &= FilterByRegexList(listing);
			args.Visible &= FilterByHighEndSameJob(listing);
			args.Visible &= FilterByHighEndSameRole(listing);
		}
	}

	private bool FilterBySameDescription(IPartyFinderListing listing)
	{
		if (!config.FilterSameDescription)
		{
			return true;
		}
		string text = ((object)listing.Description).ToString();
		if (string.IsNullOrWhiteSpace(text))
		{
			return true;
		}
		return descriptionSet.Add((listing.RawDuty, text));
	}

	private bool FilterByRegexList(IPartyFinderListing listing)
	{
		string description = ((object)listing.Description).ToString();
		if (string.IsNullOrEmpty(description))
		{
			return true;
		}
		bool flag = config.BlackList.Where<KeyValuePair<bool, string>>((KeyValuePair<bool, string> i) => i.Key).Any((KeyValuePair<bool, string> item) => Regex.IsMatch(((object)listing.Name).ToString(), item.Value) || Regex.IsMatch(description, item.Value));
		if (!config.IsWhiteList)
		{
			return !flag;
		}
		return flag;
	}

	private bool FilterByHighEndSameJob(IPartyFinderListing listing)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (!config.HighEndFilterSameJob)
		{
			return true;
		}
		if (!isRaid)
		{
			return true;
		}
		ClassJob classJobData = LocalPlayerState.ClassJobData;
		if (((ClassJob)(ref classJobData)).DohDolJobIndex != -1 || ((ClassJob)(ref classJobData)).ClassJobParent.RowId == ((ClassJob)(ref classJobData)).RowId)
		{
			return true;
		}
		foreach (RowRef<ClassJob> item in listing.JobsPresent)
		{
			if (item.RowId == LocalPlayerState.ClassJob)
			{
				return false;
			}
		}
		return true;
	}

	private bool FilterByHighEndSameRole(IPartyFinderListing listing)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		if (!config.HighEndFilterRoleCount)
		{
			return true;
		}
		if (!isRaid)
		{
			return true;
		}
		ClassJob classJobData = LocalPlayerState.ClassJobData;
		if (manualMode)
		{
			bool num = JobTypeCounter(1, config.FilterJobTypeCountData.Tank, classJobData);
			bool flag = JobTypeCounter(2, config.FilterJobTypeCountData.PureHealer, classJobData);
			bool flag2 = JobTypeCounter(6, config.FilterJobTypeCountData.ShieldHealer, classJobData);
			bool flag3 = JobTypeCounter(3, config.FilterJobTypeCountData.Melee, classJobData);
			bool flag4 = JobTypeCounter(4, config.FilterJobTypeCountData.PhysicalRanged, classJobData);
			bool flag5 = JobTypeCounter(5, config.FilterJobTypeCountData.MagicalRanged, classJobData);
			return num && flag && flag2 && flag3 && flag4 && flag5;
		}
		return ((ClassJob)(ref classJobData)).JobType switch
		{
			0 => true, 
			1 => JobTypeCounter(1, config.FilterJobTypeCountData.Tank, classJobData), 
			2 => JobTypeCounter(2, config.FilterJobTypeCountData.PureHealer, classJobData), 
			3 => JobTypeCounter(3, config.FilterJobTypeCountData.Melee, classJobData), 
			4 => JobTypeCounter(4, config.FilterJobTypeCountData.PhysicalRanged, classJobData), 
			5 => JobTypeCounter(5, config.FilterJobTypeCountData.MagicalRanged, classJobData), 
			6 => JobTypeCounter(6, config.FilterJobTypeCountData.ShieldHealer, classJobData), 
			_ => true, 
		};
		bool JobTypeCounter(int jobType, int maxCount, ClassJob currentJob)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			if (maxCount == -1)
			{
				return true;
			}
			int num2 = 0;
			bool flag6 = false;
			List<PartyFinderSlot> list = listing.Slots.ToList();
			List<RowRef<ClassJob>> list2 = listing.JobsPresent.ToList();
			foreach (int item in Enumerable.Range(0, 8))
			{
				if (list.Count <= item || list2.Count <= item || num2 >= maxCount)
				{
					break;
				}
				ClassJob value = list2.ElementAt(item).Value;
				if (((ClassJob)(ref value)).RowId != 0)
				{
					value = list2.ElementAt(item).Value;
					if (((ClassJob)(ref value)).JobType == jobType)
					{
						num2++;
					}
				}
				else if (!flag6)
				{
					JobFlags result2;
					if (manualMode)
					{
						foreach (ClassJob item2 in ((IEnumerable<ClassJob>)LuminaGetter.Get<ClassJob>()).Where((ClassJob j) => ((ClassJob)(ref j)).RowId != 0 && ((ClassJob)(ref j)).JobType == jobType))
						{
							ClassJob current2 = item2;
							if (Enum.TryParse<JobFlags>(((object)((ClassJob)(ref current2)).NameEnglish/*cast due to constrained. prefix*/).ToString().Replace(" ", string.Empty), out JobFlags result) && list.ElementAt(item)[result])
							{
								flag6 = true;
								break;
							}
						}
					}
					else if (Enum.TryParse<JobFlags>(((object)((ClassJob)(ref currentJob)).NameEnglish/*cast due to constrained. prefix*/).ToString().Replace(" ", string.Empty), out result2) && list.ElementAt(item)[result2])
					{
						flag6 = true;
					}
				}
			}
			return num2 < maxCount && flag6;
		}
	}
}
