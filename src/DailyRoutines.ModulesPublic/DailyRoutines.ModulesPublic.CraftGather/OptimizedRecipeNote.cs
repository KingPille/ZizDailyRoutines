using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Data.Parsing.Uld;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Helpers;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.ItemSource;
using OmenTools.Info.Game.ItemSource.Enums;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.KamiToolKit.Nodes;
using OmenTools.KamiToolKit.Nodes.GaugeBarCraft;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.CraftGather;

public class OptimizedRecipeNote : ModuleBase
{
	private class AddonActionsPreview(OptimizedRecipeNote inModule, TaskHelper inTaskHelper, CaculationResult result) : NativeAddon
	{
		public const float STATS_CONTAINER_HEIGHT = 70f;

		public const float STATS_CONTAINER_INNER_PADDING = 15f;

		public const float STATS_CLASS_JOB_COLUMN_WIDTH = 180f;

		public const float STATS_COLUMN_WIDTH = 120f;

		public const float STATS_COLUMN_DUMMY = 18.5f;

		public const float EXECUTION_CONTAINER_HEIGHT = 40f;

		public const float COPY_MACRO_CONTAINER_HEIGHT = 35f;

		public const float ACTION_CONTAINER_INNER_PADDING = 12.5f;

		public const float ACTION_BLOCK_SPACING = 5f;

		public const float ACTION_BLOCK_SIZE = 50f;

		public const float ITEM_INFO_SIZE = 48f;

		public const float ACTION_USED_ALPHA = 0.2f;

		public const float ACTION_NORMAL_ALPHA = 1f;

		public static AddonActionsPreview? Addon { get; set; }

		public CaculationResult Result { get; init; } = result;

		public TaskHelper TaskHelper { get; init; } = inTaskHelper;

		public OptimizedRecipeNote Module { get; init; } = inModule;

		public VerticalListNode RootContainer { get; private set; }

		public SimpleNineGridNode StatsContainerBackground { get; private set; }

		public HorizontalListNode StatsContainer { get; private set; }

		public IconImageNode ClassJobIcon { get; private set; }

		public VerticalListNode ClassJobInfoContainer { get; private set; }

		public TextNode ClassJobLable { get; private set; }

		public TextNode ClassJobName { get; private set; }

		public VerticalListNode CraftsmanshipInfoContainer { get; private set; }

		public VerticalListNode ControlInfoContainer { get; private set; }

		public VerticalListNode CraftPointInfoContainer { get; private set; }

		public HorizontalListNode ExecutionContainer { get; private set; }

		public TextButtonNode ExecuteButton { get; private set; }

		public NumericInputNode CraftCountInput { get; private set; }

		public GaugeBarCraftNode CraftProgressBar { get; private set; }

		public TextNode CraftRoundInfo { get; private set; }

		public HorizontalListNode CopyMacroContainer { get; private set; }

		public SimpleNineGridNode ActionContainerBackground { get; private set; }

		public VerticalListNode ActionContainer { get; private set; }

		public HorizontalListNode ItemInfoContainer { get; private set; }

		public ItemIconNode ItemIcon { get; private set; }

		public TextNode ItemName { get; private set; }

		public TextNode MacroStats { get; private set; }

		public List<DragDropNode> ActionBlocks { get; private set; } = new List<DragDropNode>();

		public static void OpenWithActions(OptimizedRecipeNote module, CaculationResult result)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (result.Actions.Count != 0)
			{
				Addon?.Dispose();
				Addon = new AddonActionsPreview(module, module.TaskHelper, result)
				{
					InternalName = "DRRecipeNoteActionsPreview",
					Title = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-AddonTitle")),
					Size = new Vector2(700f, 200f),
					RememberClosePosition = true
				};
				Addon.Open();
			}
		}

		private void OnRecipeNote(AddonEvent type, AddonArgs args)
		{
			foreach (DragDropNode actionBlock in ActionBlocks)
			{
				actionBlock.Alpha = 1f;
			}
		}

		private void OnCraftLogMessage(uint logMessageID, LogMessageQueueItem item)
		{
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			if (!CraftFailedLogMessages.Contains(logMessageID))
			{
				return;
			}
			OmenServiceBase<LogMessageManager>.Instance().Unreg(OnCraftLogMessage);
			TaskHelper.Abort();
			CraftRoundInfo.IsVisible = false;
			CraftProgressBar.IsVisible = false;
			foreach (DragDropNode actionBlock in ActionBlocks)
			{
				actionBlock.Alpha = 1f;
			}
			string text = LanguageManager.Get("OptimizedRecipeNote-Message-CraftFailed");
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
			OmenServiceBase<NotifyHelper>.Instance().TrayError(text);
		}

		protected unsafe override void OnFinalize(AtkUnitBase* addon)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnRecipeNote });
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0505: Unknown result type (might be due to invalid IL or missing references)
			//IL_060d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0766: Unknown result type (might be due to invalid IL or missing references)
			//IL_076b: Unknown result type (might be due to invalid IL or missing references)
			//IL_076f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0774: Unknown result type (might be due to invalid IL or missing references)
			//IL_0778: Unknown result type (might be due to invalid IL or missing references)
			//IL_077d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_07cc: Expected O, but got Unknown
			//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_08bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "RecipeNote", new AddonEventDelegate(OnRecipeNote));
			RootContainer = new VerticalListNode
			{
				FitContents = true,
				Width = base.ContentSize.X,
				Position = base.ContentStartPosition,
				ItemSpacing = 5f
			};
			RootContainer.AttachNode(this);
			SimpleNineGridNode obj = new SimpleNineGridNode
			{
				TexturePath = "ui/uld/img04/BgParts_hr1.tex",
				TextureCoordinates = new Vector2(61f, 37f),
				TextureSize = new Vector2(16f, 16f),
				Offsets = new Vector4(7f)
			};
			Vector2 contentSize = base.ContentSize;
			contentSize.Y = 70f;
			obj.Size = contentSize;
			StatsContainerBackground = obj;
			RootContainer.AddNode(StatsContainerBackground);
			StatsContainer = new HorizontalListNode
			{
				FitToContentWidth = true,
				Height = 70f,
				Position = new Vector2(15f)
			};
			StatsContainer.AttachNode((NodeBase?)StatsContainerBackground, NodePosition.AsLastChild);
			IconImageNode iconImageNode = new IconImageNode();
			ClassJob job = Result.GetJob();
			iconImageNode.IconId = job.GetIcon();
			iconImageNode.Size = new Vector2(40f, 40f);
			iconImageNode.TextureSize = new Vector2(40f, 40f);
			iconImageNode.FitTexture = true;
			ClassJobIcon = iconImageNode;
			StatsContainer.AddNode(ClassJobIcon);
			StatsContainer.AddDummy(10f);
			ClassJobInfoContainer = new VerticalListNode
			{
				FitContents = true,
				Width = 180f
			};
			StatsContainer.AddNode(ClassJobInfoContainer);
			ClassJobLable = new TextNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("ClassJob")),
				FontSize = 12u,
				TextFlags = (TextFlags)1
			};
			AtkColors.LabelLight.ApplyTo(ClassJobLable);
			ClassJobInfoContainer.AddNode(ClassJobLable);
			TextNode textNode = new TextNode();
			job = Result.GetJob();
			textNode.String = ((ClassJob)(ref job)).Name;
			textNode.FontSize = 18u;
			textNode.Size = new Vector2(180f, 24f);
			textNode.TextFlags = (TextFlags)1024;
			ClassJobName = textNode;
			AtkColors.Label.ApplyTo(ClassJobName);
			ClassJobInfoContainer.AddNode(ClassJobName);
			StatsContainer.AddNode(CreateStatsColumnSeperator());
			StatsContainer.AddDummy(18.5f);
			CraftsmanshipInfoContainer = CreateStatsColumn(3261u, Result.Craftmanship);
			StatsContainer.AddNode(CreateStatsColumnSeperator());
			StatsContainer.AddDummy(18.5f);
			ControlInfoContainer = CreateStatsColumn(3262u, Result.Control);
			StatsContainer.AddNode(CreateStatsColumnSeperator());
			StatsContainer.AddDummy(18.5f);
			CraftPointInfoContainer = CreateStatsColumn(3223u, Result.CraftPoint);
			ExecutionContainer = new HorizontalListNode
			{
				FitToContentWidth = true,
				Height = 40f,
				ItemSpacing = 10f
			};
			RootContainer.AddNode(ExecutionContainer);
			ExecuteButton = new TextButtonNode
			{
				Size = new Vector2(140f, 40f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Button-CraftMultiple", 1)),
				TextureType = ButtonTextureType.ButtonB,
				OnClick = delegate
				{
					if (TaskHelper.IsBusy)
					{
						TaskHelper.Abort();
					}
					else if ((*Addons.Synthesis).IsAddonAndNodesReady())
					{
						EnqueueActionSequence(TaskHelper, Result.Actions);
					}
					else
					{
						int totalCraftRound = CraftCountInput.Value;
						OmenServiceBase<LogMessageManager>.Instance().RegPost(OnCraftLogMessage);
						for (int i = 0; i < totalCraftRound; i++)
						{
							int currentRound = i;
							TaskHelper.Enqueue(delegate
							{
								//IL_00af: Unknown result type (might be due to invalid IL or missing references)
								int value = currentRound + 1;
								CraftProgressBar.MaxValue = totalCraftRound;
								CraftProgressBar.Value = value;
								CraftProgressBar.IsVisible = true;
								CraftRoundInfo.String = ReadOnlySeString.op_Implicit($"{value}/{totalCraftRound}");
								CraftRoundInfo.IsVisible = true;
								(*Addons.RecipeNoteAddon).Callback(8);
							});
							TaskHelper.Enqueue(() => (*Addons.Synthesis).IsAddonAndNodesReady());
							EnqueueActionSequence(TaskHelper, Result.Actions);
							TaskHelper.Enqueue(() => Addons.Synthesis == null);
							TaskHelper.Enqueue(() => DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)41]);
							TaskHelper.DelayNext(300);
						}
						TaskHelper.Enqueue(delegate
						{
							//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
							//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
							//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
							//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
							//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
							//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
							//IL_0110: Unknown result type (might be due to invalid IL or missing references)
							//IL_0115: Unknown result type (might be due to invalid IL or missing references)
							//IL_0140: Unknown result type (might be due to invalid IL or missing references)
							//IL_014f: Unknown result type (might be due to invalid IL or missing references)
							//IL_0154: Unknown result type (might be due to invalid IL or missing references)
							//IL_015a: Unknown result type (might be due to invalid IL or missing references)
							//IL_0161: Unknown result type (might be due to invalid IL or missing references)
							OmenServiceBase<LogMessageManager>.Instance().Unreg(OnCraftLogMessage);
							TaskHelper.Abort();
							CraftRoundInfo.IsVisible = false;
							CraftProgressBar.IsVisible = false;
							foreach (DragDropNode actionBlock in ActionBlocks)
							{
								actionBlock.Alpha = 1f;
							}
							Recipe recipe2 = Result.GetRecipe();
							Item value = ((Recipe)(ref recipe2)).ItemResult.Value;
							Dictionary<string, object> obj3 = new Dictionary<string, object> { ["rounds"] = totalCraftRound };
							int num5 = totalCraftRound;
							recipe2 = Result.GetRecipe();
							obj3["count"] = num5 * ((Recipe)(ref recipe2)).AmountResult;
							obj3["item"] = OmenTools.Extensions.SeStringExtension.CreateItemLink(((Item)(ref value)).RowId, false, null);
							ReadOnlySeString se = LanguageManager.GetSe("OptimizedRecipeNote-Message-CraftComplete", obj3);
							OmenServiceBase<NotifyHelper>.Instance().Chat(se);
							NotifyHelper.Toast(se);
							OmenServiceBase<NotifyHelper>.Instance().TrayInfo(((object)(*(ReadOnlySeString*)(&se))/*cast due to constrained. prefix*/).ToString());
						});
					}
				}
			};
			ExecutionContainer.AddNode(ExecuteButton);
			CraftCountInput = new NumericInputNode
			{
				Size = new Vector2(140f, 40f),
				Y = 4f,
				Min = 1,
				Max = 99999,
				Step = 1,
				Value = 1,
				OnValueUpdate = delegate(int value)
				{
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					ExecuteButton.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Button-CraftMultiple", value));
				}
			};
			ExecutionContainer.AddNode(CraftCountInput);
			ExecutionContainer.AddDummy(12f);
			CraftProgressBar = new GaugeBarCraftNode
			{
				MaxValue = 1,
				Value = 1,
				Size = new Vector2(240f, 16f),
				Y = 12f
			};
			ExecutionContainer.AddNode(CraftProgressBar);
			CraftRoundInfo = new TextNode
			{
				FontSize = 23u,
				FontType = (FontType)3,
				AlignmentType = (AlignmentType)4,
				Size = new Vector2(102f, 40f),
				String = ReadOnlySeString.op_Implicit("111/222")
			};
			AtkColors.Text.ApplyTo(CraftRoundInfo);
			ExecutionContainer.AddNode(CraftRoundInfo);
			GaugeBarCraftNode craftProgressBar = CraftProgressBar;
			bool isVisible = (CraftRoundInfo.IsVisible = false);
			craftProgressBar.IsVisible = isVisible;
			CopyMacroContainer = new HorizontalListNode
			{
				FitToContentWidth = true,
				Height = 35f,
				ItemSpacing = 5f
			};
			RootContainer.AddNode(CopyMacroContainer);
			int num = (int)Math.Ceiling((float)Result.Actions.Count / 15f);
			for (int num2 = 0; num2 < num; num2++)
			{
				int macroIndex = num2;
				TextButtonNode textButtonNode = new TextButtonNode();
				textButtonNode.Size = new Vector2(140f, 28f);
				textButtonNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Button-CopyMacro", macroIndex + 1));
				textButtonNode.OnClick = delegate
				{
					//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
					int num5 = macroIndex * 15;
					int num6 = Math.Min(num5 + 15, Result.Actions.Count);
					IEnumerable<uint> enumerable = Result.Actions.Skip(num5).Take(num6 - num5);
					StringBuilder stringBuilder = new StringBuilder();
					foreach (uint item in enumerable)
					{
						StringBuilder stringBuilder2 = stringBuilder;
						StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder2);
						handler.AppendLiteral("/ac ");
						handler.AppendFormatted(LuminaWrapper.GetActionName(item));
						handler.AppendLiteral(" <wait.3>");
						stringBuilder2.AppendLine(ref handler);
					}
					ImGui.SetClipboardText(ImU8String.op_Implicit(stringBuilder.ToString()));
					NotifyHelper.Toast(LanguageManager.Get("OptimizedRecipeNote-Message-MacroCopied", macroIndex + 1));
				};
				TextButtonNode node = textButtonNode;
				CopyMacroContainer.AddNode(node);
			}
			SimpleNineGridNode obj2 = new SimpleNineGridNode
			{
				TexturePath = "ui/uld/img04/BgParts_hr1.tex",
				TextureCoordinates = new Vector2(61f, 37f),
				TextureSize = new Vector2(16f, 16f),
				Offsets = new Vector4(7f)
			};
			contentSize = base.ContentSize;
			contentSize.Y = 70f;
			obj2.Size = contentSize;
			ActionContainerBackground = obj2;
			RootContainer.AddNode(ActionContainerBackground);
			ActionContainer = new VerticalListNode
			{
				FitContents = true,
				Width = ActionContainerBackground.Width - 25f,
				Position = new Vector2(12.5f),
				ItemSpacing = 5f
			};
			ActionContainer.AttachNode((NodeBase?)ActionContainerBackground, NodePosition.AsLastChild);
			ItemInfoContainer = new HorizontalListNode
			{
				Size = new Vector2(ActionContainer.Width, 48f),
				ItemSpacing = 5f
			};
			ActionContainer.AddNode(ItemInfoContainer);
			Recipe recipe = Result.GetRecipe();
			Item resultItem = ((Recipe)(ref recipe)).ItemResult.Value;
			ItemIcon = new ItemIconNode
			{
				IconId = ((Item)(ref resultItem)).Icon,
				Size = new Vector2(48f),
				ItemID = ((Item)(ref resultItem)).RowId,
				OnClick = (ReceiveEvent)delegate(AtkEventListener* _, AtkEventType _, int _, AtkEvent* _, AtkEventData* atkEventData)
				{
					if (AtkEventDataExtension.get_IsRightClick(in *atkEventData))
					{
						OmenServiceBase<ContextMenuManager>.Instance().OpenItem(((Item)(ref resultItem)).RowId);
					}
				}
			};
			ItemInfoContainer.AddNode(ItemIcon);
			ItemName = new TextNode
			{
				String = ((Item)(ref resultItem)).Name,
				Size = new Vector2(400f, 40f),
				FontSize = 16u,
				AlignmentType = (AlignmentType)3,
				TextFlags = (TextFlags)192
			};
			AtkColors.Hint.ApplyTo(ItemName);
			ItemInfoContainer.AddNode(ItemName);
			MacroStats = new TextNode
			{
				Size = new Vector2(0f, 40f),
				AlignmentType = (AlignmentType)5,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Text-MacroStats", Result.Actions.Count, Result.Actions.Count * 3)),
				FontSize = 14u,
				X = ActionContainer.Width
			};
			AtkColors.LabelLight.ApplyTo(MacroStats);
			MacroStats.AttachNode(ActionContainer);
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				Size = new Vector2(ActionContainer.Width, 50f),
				ItemSpacing = 5f
			};
			int num3 = 0;
			for (int num4 = 0; num4 < Result.Actions.Count; num4++)
			{
				uint actionID = Result.Actions[num4];
				uint actionIconID = LuminaWrapper.GetActionIconID(actionID);
				if (actionIconID == 0)
				{
					continue;
				}
				if (num3 > 1 && (float)num3 * 50f + (float)(num3 - 1) * 5f > ActionContainer.Width)
				{
					ActionContainer.AddNode(horizontalListNode);
					horizontalListNode = new HorizontalListNode
					{
						Size = new Vector2(ActionContainer.Width, 50f),
						ItemSpacing = 5f
					};
					num3 = 0;
				}
				ResNode resNode = new ResNode
				{
					Size = new Vector2(50f)
				};
				DragDropNode dragDropNode = new DragDropNode
				{
					Size = new Vector2(50f),
					IconId = actionIconID,
					AcceptedType = (DragDropType)1,
					IsDraggable = true,
					IsClickable = true,
					Payload = new DragDropPayload
					{
						Type = (DragDropType)((actionID > 100000) ? 12 : 7),
						Int2 = (int)actionID
					},
					OnRollOver = delegate(DragDropNode dragDropNode2)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_002f: Unknown result type (might be due to invalid IL or missing references)
						AtkTooltipArgs val = new AtkTooltipArgs
						{
							ActionArgs = 
							{
								Flags = 1
							},
							ActionArgs = 
							{
								Kind = (DetailKind)((actionID > 100000) ? 31 : 30)
							},
							ActionArgs = 
							{
								Id = (int)actionID
							}
						};
						((AtkTooltipManager)(&((AtkStage)AtkStage.Instance()).TooltipManager)).ShowTooltip((AtkTooltipType)4, ((AtkUnitBase)addon).Id, (AtkResNode*)dragDropNode2, &val, (delegate* unmanaged[Stdcall]<float*, float*, AtkResNode*, void>)null, false, true);
					},
					OnRollOut = delegate(DragDropNode dragDropNode2)
					{
						dragDropNode2.HideTooltip();
					}
				};
				dragDropNode.OnClicked = delegate
				{
					if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)40] && !TaskHelper.IsBusy)
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage("/ac " + LuminaWrapper.GetActionName(actionID));
						if (Addons.Synthesis != null)
						{
							dragDropNode.Alpha = 0.2f;
						}
					}
				};
				dragDropNode.AttachNode(resNode);
				ActionBlocks.Add(dragDropNode);
				TextNode textNode2 = new TextNode
				{
					Position = new Vector2(-4f),
					String = ReadOnlySeString.op_Implicit($"{num4 + 1}"),
					FontType = (FontType)1,
					TextFlags = (TextFlags)8
				};
				AtkColors.Value.ApplyTo(textNode2);
				textNode2.AttachNode(resNode);
				horizontalListNode.AddNode(resNode);
				num3++;
			}
			if (num3 > 0)
			{
				ActionContainer.AddNode(horizontalListNode);
			}
			ActionContainerBackground.Height = ActionContainer.Height + 25f;
			RootContainer.RecalculateLayout();
			SetWindowSize(base.Size.X, RootContainer.Height + base.ContentStartPosition.Y + 24f);
			RootContainer.Position = base.ContentStartPosition;
			VerticalListNode CreateStatsColumn(uint addonTextID, int number)
			{
				//IL_006e: Unknown result type (might be due to invalid IL or missing references)
				VerticalListNode verticalListNode = new VerticalListNode
				{
					FitContents = true,
					Width = 120f
				};
				StatsContainer.AddNode(verticalListNode);
				TextNode node2 = new TextNode
				{
					SheetType = (SheetType)0,
					TextId = addonTextID,
					FontSize = 12u,
					TextFlags = (TextFlags)1
				};
				AtkColors.LabelLight.ApplyTo(node2);
				verticalListNode.AddNode(node2);
				TextNode node3 = new TextNode
				{
					String = ReadOnlySeString.op_Implicit(number.ToString()),
					FontSize = 20u,
					Size = new Vector2(120f, 24f),
					TextFlags = (TextFlags)1032,
					FontType = (FontType)2
				};
				AtkColors.ValueEmphasize.ApplyTo(node3);
				verticalListNode.AddNode(node3);
				return verticalListNode;
			}
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (TaskHelper.IsBusy)
			{
				ExecuteButton.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Button-StopCraft"));
				return;
			}
			if ((*Addons.Synthesis).IsAddonAndNodesReady())
			{
				ExecuteButton.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Button-StartCraft"));
				return;
			}
			ExecuteButton.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Button-CraftMultiple", CraftCountInput.Value));
		}

		private static VerticalLineNode CreateStatsColumnSeperator()
		{
			return new VerticalLineNode
			{
				Height = 45f,
				Width = 4f,
				Y = -2.5f
			};
		}

		private void EnqueueActionSequence(TaskHelper th, List<uint> actions)
		{
			for (int i = 0; i < actions.Count; i++)
			{
				uint x = actions[i];
				int i2 = i;
				th.Enqueue(delegate
				{
					if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)40])
					{
						return true;
					}
					OmenServiceBase<ChatManager>.Instance().SendMessage("/ac " + LuminaWrapper.GetActionName(x));
					return false;
				});
				th.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)40]);
				th.Enqueue(delegate
				{
					ActionBlocks[i2].Alpha = 0.2f;
				});
			}
		}
	}

	private delegate int SimpleCraftGetAmountUpperLimitDelegate(nint agent, bool eventCase);

	private unsafe delegate AtkValue* RecipeNotePraticeSettingSetupDelegate(AtkEventListener* listener, AtkValue* returnValue, AtkValue* values);

	private record CaculationResult(List<uint> Actions, uint RecipeID, int CraftPoint, int Craftmanship, int Control, DalamudLinkPayload CopyLinkPayload, DalamudLinkPayload PreviewLinkPayload)
	{
		public Recipe GetRecipe()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRowOrDefault<Recipe>(RecipeID);
		}

		public ClassJob GetJob()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			Recipe recipe = GetRecipe();
			return LuminaGetter.GetRowOrDefault<ClassJob>(((Recipe)(ref recipe)).CraftType.RowId + 8);
		}
	}

	private class Config : ModuleConfig
	{
		public bool IsSearchClearButton = true;

		public bool IsCategoryButtons = true;

		public bool IsDisplayOthersButtons = true;

		public bool IsMaterialSourceButtons = true;

		public bool IsSwitchJobButton = true;

		public bool IsCaculateRecipeButton = true;

		public bool IsQuickSynthesisMore = true;

		public bool IsMorePraticeQuality = true;

		public bool IsNotifyQuickSynthesisFinish = true;
	}

	private static readonly CompSig SimpleCraftGetAmountUpperLimitSig = new CompSig("4C 8B DC 48 83 EC ?? 48 8B 81 ?? ?? ?? ?? 44 0F B6 CA");

	private Hook<SimpleCraftGetAmountUpperLimitDelegate>? SimpleCraftGetAmountUpperLimitHook;

	private static readonly CompSig SimpleCraftAmountJudgeSig = new CompSig("0F 87 ?? ?? ?? ?? 48 8B 81 ?? ?? ?? ?? 48 85 C0");

	private MemoryPatch SimpleCraftAmountJudgePatch;

	private static readonly CompSig RecipeNotePraticeSettingSetupSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? BA ?? ?? ?? ?? 49 8B F0 48 8B E9 E8 ?? ?? ?? ?? 48 8D 4E ?? 48 8B D8 E8 ?? ?? ?? ?? 48 8B D0 48 8B CB E8 ?? ?? ?? ?? 48 8D 4E");

	private Hook<RecipeNotePraticeSettingSetupDelegate>? RecipeNotePraticeSettingSetupHook;

	private Config config;

	private readonly Dictionary<uint, CaculationResult> caculationResults = new Dictionary<uint, CaculationResult>();

	private TextButtonNode? caculateRecipeButton;

	private TextButtonNode? switchJobButton;

	private TextureButtonNode? clearSearchButton;

	private TextButtonNode? displayOthersButton;

	private HorizontalListNode? displayOthersJobsLayout;

	private List<IconButtonNode> displayOthersJobButtons = new List<IconButtonNode>();

	private List<IconButtonNode> materialSourceButtons = new List<IconButtonNode>();

	private TextButtonNode? levelRecipeButton;

	private TextButtonNode? specialRecipeButton;

	private TextButtonNode? masterRecipeButton;

	private DalamudLinkPayload? installRaphaelLinkPayload;

	private Task? installRaphaelTask;

	private uint lastRecipeID;

	private int pendingQuickSynthRemaining;

	private bool pendingUseHQIngredient;

	private bool pendingAllNQResult;

	[IPCSubscriber("DailyRoutines.Modules.AutoShowItemNPCShopInfo.OpenByItemID")]
	private static IPCSubscriber<uint, bool> OpenShopListByItemIDIPC;

	private static readonly FrozenDictionary<uint, List<Recipe>> SameItemRecipes = (from x in (from x in (IEnumerable<Recipe>)LuminaGetter.Get<Recipe>()
			group x by ((Recipe)(ref x)).ItemResult.RowId).DistinctBy((IGrouping<uint, Recipe> x) => x.Key)
		where x.Key != 0 && x.Count() > 1
		select x).ToFrozenDictionary((IGrouping<uint, Recipe> x) => x.Key, (IGrouping<uint, Recipe> x) => x.DistinctBy((Recipe d) => ((Recipe)(ref d)).CraftType.RowId).ToList());

	private const int MAX_QUICK_SYNTHESIS_COUNT = 255;

	private static readonly FrozenSet<uint> CraftFailedLogMessages = FrozenSet.Create<uint>(1131u, 1133u, 1134u, 1135u, 1136u, 1137u, 1138u, 1139u, 1140u, 1141u, 1142u, 1143u, 1144u, 1145u, 1146u, 1147u, 1148u, 1149u, 1160u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedRecipeNoteTitle"),
		Description = LanguageManager.Get("OptimizedRecipeNoteDescription"),
		Category = ModuleCategory.CraftGather,
		ModulesPrerequisite = new string[1] { "AutoShowItemNPCShopInfo" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		SimpleCraftAmountJudgePatch = new MemoryPatch(SimpleCraftAmountJudgeSig.Get(), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[6]
		{
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 15000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		SimpleCraftGetAmountUpperLimitHook = SimpleCraftGetAmountUpperLimitSig.GetHook<SimpleCraftGetAmountUpperLimitDelegate>(SimpleCraftGetAmountUpperLimitDetour);
		RecipeNotePraticeSettingSetupHook = RecipeNotePraticeSettingSetupSig.GetHook<RecipeNotePraticeSettingSetupDelegate>(RecipeNotePraticeSettingSetupDetour);
		if (config.IsQuickSynthesisMore)
		{
			SimpleCraftAmountJudgePatch.Enable();
			SimpleCraftGetAmountUpperLimitHook.Enable();
			DalamudServiceExtension.Instance<IAgentLifecycle>().RegisterListener((AgentEvent)0, (AgentId)25, new AgentEventDelegate(OnAgentRecipeNote));
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)10, "SynthesisSimple", new AddonEventDelegate(OnSynthesisSimple));
		}
		if (config.IsMorePraticeQuality)
		{
			RecipeNotePraticeSettingSetupHook.Enable();
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "RecipeNote", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "RecipeNote", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "RecipeNote", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnSynthesisSimple });
		DalamudServiceExtension.Instance<IAgentLifecycle>().UnregisterListener((AgentEventDelegate[])(object)new AgentEventDelegate[1] { OnAgentRecipeNote });
		RemoveAllNodes();
		AddonActionsPreview.Addon?.Dispose();
		AddonActionsPreview.Addon = null;
		foreach (CaculationResult value in caculationResults.Values)
		{
			OmenServiceBase<LinkPayloadManager>.Instance().Unreg(value.CopyLinkPayload.CommandId);
			OmenServiceBase<LinkPayloadManager>.Instance().Unreg(value.PreviewLinkPayload.CommandId);
		}
		caculationResults.Clear();
		if (installRaphaelLinkPayload != null)
		{
			OmenServiceBase<LinkPayloadManager>.Instance().Unreg(installRaphaelLinkPayload.CommandId);
		}
		installRaphaelTask = null;
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Config-SearchClearButton")), ref config.IsSearchClearButton))
		{
			config.Save(this);
			RemoveAllNodes();
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedRecipeNote-Config-SearchClearButton-Help"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Config-CategoryButtons")), ref config.IsCategoryButtons))
		{
			config.Save(this);
			RemoveAllNodes();
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedRecipeNote-Config-CategoryButtons-Help"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Config-DisplayOthersButtons")), ref config.IsDisplayOthersButtons))
		{
			config.Save(this);
			RemoveAllNodes();
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedRecipeNote-Config-DisplayOthersButtons-Help"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Config-MaterialSourceButtons")), ref config.IsMaterialSourceButtons))
		{
			config.Save(this);
			RemoveAllNodes();
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedRecipeNote-Config-MaterialSourceButtons-Help"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Config-SwitchJobButton")), ref config.IsSwitchJobButton))
		{
			config.Save(this);
			RemoveAllNodes();
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedRecipeNote-Config-SwitchJobButton-Help"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Config-CaculateRecipeButton")), ref config.IsCaculateRecipeButton))
		{
			config.Save(this);
			RemoveAllNodes();
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedRecipeNote-Config-CaculateRecipeButton-Help"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Config-QuickSynthesisMore")), ref config.IsQuickSynthesisMore))
		{
			config.Save(this);
			SimpleCraftAmountJudgePatch.Set(config.IsQuickSynthesisMore);
			SimpleCraftGetAmountUpperLimitHook.Toggle<SimpleCraftGetAmountUpperLimitDelegate>(config.IsQuickSynthesisMore);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedRecipeNote-Config-QuickSynthesisMore-Help"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Config-MorePraticeQuality")), ref config.IsMorePraticeQuality))
		{
			config.Save(this);
			RecipeNotePraticeSettingSetupHook.Toggle<RecipeNotePraticeSettingSetupDelegate>(config.IsMorePraticeQuality);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedRecipeNote-Config-MorePraticeQuality-Help"), 40f, (FontAwesomeIcon)61530);
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Config-NotifyQuickSynthesisFinish")), ref config.IsSwitchJobButton))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("OptimizedRecipeNote-Config-NotifyQuickSynthesisFinish-Help"), 40f, (FontAwesomeIcon)61530);
	}

	public unsafe static int SimpleCraftGetAmountUpperLimitDetour(nint agentRecipeNote, bool isHQ)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		RecipeEntry* selectedRecipe = ((RecipeData)((RecipeNote)RecipeNote.Instance()).RecipeList).SelectedRecipe;
		if (selectedRecipe == null)
		{
			return 0;
		}
		int num = 9999;
		Span<RecipeIngredient> ingredients = ((RecipeEntry)selectedRecipe).Ingredients;
		for (int i = 0; i < ingredients.Length; i++)
		{
			RecipeIngredient val = ingredients[i];
			if (val.ItemId != 0)
			{
				int inventoryItemCount = ((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(val.ItemId, false, true, true, (short)0);
				int inventoryItemCount2 = ((InventoryManager)InventoryManager.Instance()).GetInventoryItemCount(val.ItemId, true, true, true, (short)0);
				int num2 = (isHQ ? (inventoryItemCount + inventoryItemCount2) : inventoryItemCount);
				if (num2 == 0)
				{
					return 0;
				}
				int num3 = num2 / val.Amount;
				if (num3 == 0)
				{
					return 0;
				}
				num = (int)MathF.Min(num3, num);
			}
		}
		return num;
	}

	private unsafe AtkValue* RecipeNotePraticeSettingSetupDetour(AtkEventListener* listener, AtkValue* returnValue, AtkValue* values)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).SetUInt(((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).UInt * 2);
		((AtkValue)values).SetUInt(((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).UInt);
		byte* num = (byte*)values + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>();
		ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
		SeStringParameter reference = SeStringParameter.op_Implicit(((AtkValue)((byte*)values + Unsafe.SizeOf<AtkValue>())).UInt);
		((AtkValue)num).SetManagedString(ReadOnlySeString.op_Implicit(obj.EvaluateFromAddon(14222u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null)));
		return RecipeNotePraticeSettingSetupHook.Original(listener, returnValue, values);
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		switch (type - 5)
		{
		case 1:
			caculateRecipeButton = null;
			switchJobButton = null;
			displayOthersButton = null;
			displayOthersJobsLayout = null;
			clearSearchButton = null;
			levelRecipeButton = null;
			specialRecipeButton = null;
			masterRecipeButton = null;
			materialSourceButtons.Clear();
			displayOthersJobButtons.Clear();
			break;
		case 3:
		{
			if (!((AgentRecipeNote)AgentRecipeNote.Instance()).RecipeSearchOpen && TryGetCurrentRecipe(out var recipeID, out var _))
			{
				lastRecipeID = recipeID;
			}
			UpdateRecipeAddonButtons();
			break;
		}
		case 0:
			if ((*Addons.RecipeNoteAddon).IsAddonAndNodesReady())
			{
				CreateCaculateRecipeButton();
				CreateSwitchJobButton();
				CreateMaterialSourceButtons();
				CreateDisplayOthersButtons();
				CreateClearSearchButton();
				CreateCategoryButtons();
				UpdateRecipeAddonButtons();
			}
			break;
		case 2:
			break;
		}
		unsafe void UpdateRecipeAddonButtons()
		{
			if (Addons.RecipeNoteAddon != null)
			{
				UpdateCaculateRecipeButton();
				UpdateSwitchJobButton();
				UpdateClearSearchButton();
				UpdateDisplayOthersButtons();
				UpdateMaterialSourceButtons();
			}
		}
	}

	private unsafe void RemoveAllNodes()
	{
		caculateRecipeButton?.Dispose();
		caculateRecipeButton = null;
		clearSearchButton?.Dispose();
		clearSearchButton = null;
		levelRecipeButton?.Dispose();
		levelRecipeButton = null;
		specialRecipeButton?.Dispose();
		specialRecipeButton = null;
		masterRecipeButton?.Dispose();
		masterRecipeButton = null;
		displayOthersButton?.Dispose();
		displayOthersButton = null;
		displayOthersJobButtons.Clear();
		displayOthersJobsLayout?.Dispose();
		displayOthersJobsLayout = null;
		foreach (IconButtonNode materialSourceButton in materialSourceButtons)
		{
			materialSourceButton.Dispose();
		}
		materialSourceButtons.Clear();
		if (Addons.RecipeNoteAddon != null)
		{
			AtkResNode* nodeById = ((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(95u);
			((AtkResNode)nodeById).Timeline = null;
			if (nodeById != null)
			{
				((AtkResNode)nodeById).SetXFloat(46f);
			}
			AtkResNode* nodeById2 = ((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(88u);
			((AtkResNode)nodeById2).Timeline = null;
			if (nodeById2 != null)
			{
				((AtkResNode)nodeById2).SetXFloat(0f);
			}
			AtkResNode* nodeById3 = ((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(84u);
			((AtkResNode)nodeById3).Timeline = null;
			if (nodeById3 != null)
			{
				((AtkResNode)nodeById3).SetXFloat(0f);
			}
		}
		switchJobButton?.Dispose();
		switchJobButton = null;
		caculateRecipeButton?.Dispose();
		caculateRecipeButton = null;
	}

	private unsafe void CreateClearSearchButton()
	{
		if (!config.IsSearchClearButton || clearSearchButton != null)
		{
			return;
		}
		clearSearchButton = new TextureButtonNode
		{
			IsVisible = true,
			Position = new Vector2(130f, 25f),
			Size = new Vector2(28f),
			TexturePath = "ui/uld/WindowA_Button_hr1.tex",
			TextureCoordinates = Vector2.Zero,
			TextureSize = new Vector2(28f),
			OnClick = delegate
			{
				if (lastRecipeID != 0)
				{
					AgentRecipeNote* ptr = AgentRecipeNote.Instance();
					if (((AgentRecipeNote)ptr).RecipeSearchOpen)
					{
						((AgentRecipeNote)ptr).OpenRecipeByRecipeId(lastRecipeID);
					}
				}
			}
		};
		clearSearchButton.AttachNode(((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(24u));
	}

	private unsafe void UpdateClearSearchButton()
	{
		if (config.IsSearchClearButton && clearSearchButton != null)
		{
			clearSearchButton.IsVisible = ((AgentRecipeNote)AgentRecipeNote.Instance()).RecipeSearchOpen && lastRecipeID != 0;
		}
	}

	private unsafe void CreateCategoryButtons()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		if (!config.IsCategoryButtons || (levelRecipeButton != null && specialRecipeButton != null && masterRecipeButton != null))
		{
			return;
		}
		if (levelRecipeButton == null)
		{
			levelRecipeButton = new TextButtonNode
			{
				IsVisible = true,
				Position = new Vector2(0f, 32f),
				Size = new Vector2(58f, 38f),
				TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(1710u)),
				OnClick = delegate
				{
					((AgentRecipeNote)AgentRecipeNote.Instance()).SelectedRecipeCategoryPage = 2;
					AtkComponentButton* button = ((AtkUnitBase)Addons.RecipeNoteAddon).GetComponentButtonById(35u);
					if (button != null)
					{
						DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
						{
							(*button).Click();
						}, default(CancellationToken));
						DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
						{
							(*button).Click();
						}, default(CancellationToken));
						DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
						{
							(*button).Click();
						}, default(CancellationToken));
						DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
						{
							(*button).Click();
						}, default(CancellationToken));
					}
				}
			};
			levelRecipeButton.BackgroundNode.IsVisible = false;
			levelRecipeButton.AttachNode(((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(32u));
		}
		if (specialRecipeButton == null)
		{
			specialRecipeButton = new TextButtonNode
			{
				IsVisible = true,
				Position = new Vector2(50f, 32f),
				Size = new Vector2(58f, 38f),
				TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(1711u)),
				OnClick = delegate
				{
					((AgentRecipeNote)AgentRecipeNote.Instance()).SelectedRecipeCategoryPage = 0;
					AtkComponentButton* button = ((AtkUnitBase)Addons.RecipeNoteAddon).GetComponentButtonById(35u);
					if (button != null)
					{
						DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
						{
							(*button).Click();
						}, default(CancellationToken));
						DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
						{
							(*button).Click();
						}, default(CancellationToken));
						DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
						{
							(*button).Click();
						}, default(CancellationToken));
						DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
						{
							(*button).Click();
						}, default(CancellationToken));
					}
				}
			};
			specialRecipeButton.BackgroundNode.IsVisible = false;
			specialRecipeButton.AttachNode(((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(32u));
		}
		if (masterRecipeButton != null)
		{
			return;
		}
		masterRecipeButton = new TextButtonNode
		{
			IsVisible = true,
			Position = new Vector2(102f, 32f),
			Size = new Vector2(58f, 38f),
			TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(14212u)),
			OnClick = delegate
			{
				((AgentRecipeNote)AgentRecipeNote.Instance()).SelectedRecipeCategoryPage = 1;
				AtkComponentButton* button = ((AtkUnitBase)Addons.RecipeNoteAddon).GetComponentButtonById(35u);
				if (button != null)
				{
					DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
					{
						(*button).Click();
					}, default(CancellationToken));
					DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
					{
						(*button).Click();
					}, default(CancellationToken));
					DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
					{
						(*button).Click();
					}, default(CancellationToken));
					DalamudServiceExtension.Instance<IFramework>().Run((Action)delegate
					{
						(*button).Click();
					}, default(CancellationToken));
				}
			}
		};
		masterRecipeButton.BackgroundNode.IsVisible = false;
		masterRecipeButton.AttachNode(((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(32u));
	}

	private unsafe void CreateDisplayOthersButtons()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		if (!config.IsDisplayOthersButtons || (displayOthersButton != null && displayOthersJobsLayout != null))
		{
			return;
		}
		if (displayOthersButton == null)
		{
			displayOthersButton = new TextButtonNode
			{
				Position = new Vector2(0f, -32f),
				Size = new Vector2(140f, 32f),
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Button-ShowOtherRecipes")),
				IsVisible = true,
				OnClick = delegate
				{
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					if (TryGetCurrentRecipe(out var _, out var recipe) && SameItemRecipes.TryGetValue(((Recipe)(ref recipe)).ItemResult.RowId, out List<Recipe> _))
					{
						((AgentRecipeNote)AgentRecipeNote.Instance()).SearchRecipeByItemId(((Recipe)(ref recipe)).ItemResult.RowId, 0u);
					}
				}
			};
			displayOthersButton.LabelNode.AutoAdjustTextSize();
			displayOthersButton.AttachNode(((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(57u));
		}
		if (displayOthersJobsLayout == null)
		{
			displayOthersJobsLayout = new HorizontalListNode
			{
				IsVisible = true,
				ItemSpacing = 5f,
				Size = new Vector2(220f, 24f),
				Position = new Vector2(142f, -30f)
			};
			for (uint num = 0u; num < 8; num++)
			{
				IconButtonNode iconButtonNode = new IconButtonNode
				{
					IconId = 62008 + num,
					Size = new Vector2(24f),
					IsVisible = true,
					TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetJobName(8 + num))
				};
				IconImageNode obj = new IconImageNode
				{
					IconId = 62008 + num,
					Size = new Vector2(24f),
					IsVisible = true,
					ImageNodeFlags = (ImageNodeFlags)128
				};
				TimelineBuilder timelineBuilder = new TimelineBuilder();
				Vector2? position = Vector2.Zero;
				byte? alpha = (byte)byte.MaxValue;
				Vector3? multiplyColor = new Vector3(100f);
				TimelineBuilder timelineBuilder2 = timelineBuilder.AddFrameSetWithFrame(1, 10, 1, position, alpha, null, multiplyColor);
				Vector2? position2 = Vector2.Zero;
				byte? alpha2 = (byte)byte.MaxValue;
				Vector3? multiplyColor2 = new Vector3(100f);
				TimelineBuilder timelineBuilder3 = timelineBuilder2.AddFrameSetWithFrame(11, 17, 11, position2, alpha2, null, multiplyColor2);
				Vector2? position3 = Vector2.Zero + new Vector2(0f, 1f);
				byte? alpha3 = (byte)byte.MaxValue;
				Vector3? multiplyColor3 = new Vector3(100f);
				TimelineBuilder timelineBuilder4 = timelineBuilder3.AddFrameSetWithFrame(18, 26, 18, position3, alpha3, null, multiplyColor3);
				Vector2? position4 = Vector2.Zero;
				byte? alpha4 = (byte)153;
				Vector3? multiplyColor4 = new Vector3(80f);
				TimelineBuilder timelineBuilder5 = timelineBuilder4.AddFrameSetWithFrame(27, 36, 27, position4, alpha4, null, multiplyColor4);
				Vector2? position5 = Vector2.Zero;
				byte? alpha5 = (byte)byte.MaxValue;
				Vector3? multiplyColor5 = new Vector3(100f);
				TimelineBuilder timelineBuilder6 = timelineBuilder5.AddFrameSetWithFrame(37, 46, 37, position5, alpha5, null, multiplyColor5);
				Vector2? position6 = Vector2.Zero;
				byte? alpha6 = (byte)byte.MaxValue;
				Vector3? multiplyColor6 = new Vector3(100f);
				obj.AddTimeline(timelineBuilder6.AddFrameSetWithFrame(47, 53, 47, position6, alpha6, null, multiplyColor6).Build());
				iconButtonNode.BackgroundNode.IsVisible = false;
				iconButtonNode.ImageNode.IsVisible = false;
				obj.AttachNode((NodeBase?)iconButtonNode, NodePosition.AsLastChild);
				displayOthersJobButtons.Add(iconButtonNode);
				displayOthersJobsLayout.AddNode(iconButtonNode);
			}
			displayOthersJobsLayout.AttachNode(((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(57u));
		}
	}

	private unsafe void UpdateDisplayOthersButtons()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!config.IsDisplayOthersButtons || displayOthersButton == null || displayOthersJobsLayout == null || displayOthersJobButtons.Count != 8)
		{
			return;
		}
		if (!TryGetCurrentRecipe(out var _, out var recipe) || !SameItemRecipes.TryGetValue(((Recipe)(ref recipe)).ItemResult.RowId, out List<Recipe> value))
		{
			displayOthersButton.IsVisible = false;
			displayOthersJobsLayout.IsVisible = false;
			return;
		}
		displayOthersButton.IsVisible = true;
		displayOthersJobsLayout.IsVisible = true;
		Dictionary<uint, uint> dictionary = value.ToDictionary((Recipe x) => ((Recipe)(ref x)).CraftType.RowId, (Recipe x) => ((Recipe)(ref x)).RowId);
		for (uint num = 0u; num < 8; num++)
		{
			IconButtonNode iconButtonNode = displayOthersJobButtons[(int)num];
			if (dictionary.TryGetValue(num, out var otherRecipeID))
			{
				iconButtonNode.Alpha = 1f;
				iconButtonNode.IsEnabled = true;
				iconButtonNode.OnClick = delegate
				{
					if (!TryGetCurrentRecipe(out var recipeID2, out var _) || recipeID2 != otherRecipeID)
					{
						((AgentRecipeNote)AgentRecipeNote.Instance()).OpenRecipeByRecipeId(otherRecipeID);
					}
				};
			}
			else
			{
				iconButtonNode.Alpha = 0.2f;
				iconButtonNode.IsEnabled = false;
				iconButtonNode.OnClick = delegate
				{
				};
			}
		}
	}

	private unsafe void CreateMaterialSourceButtons()
	{
		if (!config.IsMaterialSourceButtons || materialSourceButtons.Count != 0)
		{
			return;
		}
		for (uint num = 0u; num < 6; num++)
		{
			AtkComponentNode* componentNodeById = ((AtkUnitBase)Addons.RecipeNoteAddon).GetComponentNodeById(89 + num);
			uint index = num;
			IconButtonNode iconButtonNode = new IconButtonNode
			{
				IconId = 60412u,
				Size = new Vector2(32f),
				IsVisible = true,
				Position = new Vector2(-26f, 8f),
				OnClick = delegate
				{
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_002f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0043: Unknown result type (might be due to invalid IL or missing references)
					//IL_0047: Unknown result type (might be due to invalid IL or missing references)
					//IL_004c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0068: Unknown result type (might be due to invalid IL or missing references)
					//IL_006d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
					//IL_0120: Unknown result type (might be due to invalid IL or missing references)
					if (TryGetCurrentRecipe(out var _, out var recipe) && ((Recipe)(ref recipe)).Ingredient[(int)index].IsValid)
					{
						Item value = ((Recipe)(ref recipe)).Ingredient[(int)index].Value;
						bool flag = ItemSourceInfo.Query(((Item)(ref value)).RowId).State == ItemSourceQueryState.Ready;
						if (((Item)(ref value)).ItemSearchCategory.RowId != 0 && flag)
						{
							if (!PluginConfig.Instance().ConflictKeyBinding.IsPressed())
							{
								OpenShopListByItemIDIPC.InvokeFunc(((Item)(ref value)).RowId);
							}
							else
							{
								OmenServiceBase<ChatManager>.Instance().SendMessage($"/pdr market {((Item)(ref value)).Name}");
							}
						}
						else if (flag)
						{
							OpenShopListByItemIDIPC.InvokeFunc(((Item)(ref value)).RowId);
						}
						else if (((Item)(ref value)).ItemSearchCategory.RowId != 0)
						{
							OmenServiceBase<ChatManager>.Instance().SendMessage($"/pdr market {((Item)(ref value)).Name}");
						}
					}
				}
			};
			SimpleNineGridNode obj = (SimpleNineGridNode)iconButtonNode.BackgroundNode;
			obj.TexturePath = "ui/uld/partyfinder_hr1.tex";
			obj.TextureCoordinates = new Vector2(38f, 2f);
			obj.TextureSize = new Vector2(32f, 34f);
			obj.LeftOffset = 0f;
			obj.RightOffset = 0f;
			materialSourceButtons.Add(iconButtonNode);
			iconButtonNode.AttachNode(componentNodeById);
		}
	}

	private unsafe void UpdateMaterialSourceButtons()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		if (!config.IsMaterialSourceButtons || materialSourceButtons.Count != 6 || !TryGetCurrentRecipe(out var _, out var recipe))
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < ((Recipe)(ref recipe)).Ingredient.Count; i++)
		{
			RowRef<Item> val = ((Recipe)(ref recipe)).Ingredient[i];
			if (val.IsValid && val.RowId > 100)
			{
				byte b = ((Recipe)(ref recipe)).AmountIngredient[i];
				if (b > num)
				{
					num = b;
				}
			}
		}
		int num2 = ((num >= 10) ? 30 : 10);
		AtkResNode* nodeById = ((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(95u);
		((AtkResNode)nodeById).Timeline = null;
		if (nodeById != null)
		{
			((AtkResNode)nodeById).SetXFloat((float)(46 + num2));
		}
		AtkResNode* nodeById2 = ((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(88u);
		((AtkResNode)nodeById2).Timeline = null;
		if (nodeById2 != null)
		{
			((AtkResNode)nodeById2).SetXFloat((float)num2);
		}
		AtkResNode* nodeById3 = ((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(84u);
		((AtkResNode)nodeById3).Timeline = null;
		if (nodeById3 != null)
		{
			((AtkResNode)nodeById3).SetXFloat((float)num2);
		}
		AtkResNode* nodeById4 = ((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(2u);
		if (nodeById4 != null)
		{
			((AtkResNode)nodeById4).SetAlpha((byte)0);
		}
		for (int j = 0; j < materialSourceButtons.Count && ((Recipe)(ref recipe)).Ingredient[j].IsValid; j++)
		{
			Item value = ((Recipe)(ref recipe)).Ingredient[j].Value;
			bool flag = ItemSourceInfo.Query(((Item)(ref value)).RowId).State == ItemSourceQueryState.Ready;
			IconButtonNode iconButtonNode = materialSourceButtons[j];
			string empty = string.Empty;
			iconButtonNode.X = -6 + ((num >= 10) ? (-20) : 0);
			if (((Item)(ref value)).ItemSearchCategory.RowId != 0 && flag)
			{
				iconButtonNode.IconId = 60412u;
				empty = $"{LuminaWrapper.GetAddonText(350u)} / {LuminaWrapper.GetAddonText(548u)} [{LanguageManager.Get("ConflictKey")}]";
			}
			else if (flag)
			{
				iconButtonNode.IconId = 60412u;
				empty = LuminaWrapper.GetAddonText(350u) ?? "";
			}
			else if (((Item)(ref value)).ItemSearchCategory.RowId != 0)
			{
				iconButtonNode.IconId = 60570u;
				empty = LuminaWrapper.GetAddonText(548u) ?? "";
			}
			else
			{
				empty = string.Empty;
			}
			if (empty == string.Empty)
			{
				iconButtonNode.IsVisible = false;
				continue;
			}
			iconButtonNode.IsVisible = true;
			iconButtonNode.TextTooltip = ReadOnlySeString.op_Implicit(empty);
		}
	}

	private unsafe void CreateSwitchJobButton()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!config.IsSwitchJobButton || switchJobButton != null)
		{
			return;
		}
		switchJobButton = new TextButtonNode
		{
			Position = new Vector2(228f, 522f),
			Size = new Vector2(140f, 36f),
			String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Button-SwitchJob")),
			OnClick = delegate
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Unknown result type (might be due to invalid IL or missing references)
				if (TryGetCurrentRecipe(out var recipeID, out var recipe) && ((Recipe)(ref recipe)).CraftType.RowId != LocalPlayerState.ClassJob - 8)
				{
					if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)41])
					{
						LocalPlayerState.SwitchGearset(((Recipe)(ref recipe)).CraftType.RowId + 8);
					}
					else
					{
						base.TaskHelper.Enqueue(delegate
						{
							((AgentRecipeNote)AgentRecipeNote.Instance()).Hide();
						});
						base.TaskHelper.Enqueue(() => !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)41]);
						base.TaskHelper.Enqueue(() => LocalPlayerState.SwitchGearset(((Recipe)(ref recipe)).CraftType.RowId + 8));
						base.TaskHelper.Enqueue(delegate
						{
							((AgentRecipeNote)AgentRecipeNote.Instance()).OpenRecipeByRecipeId(recipeID);
						});
					}
				}
			}
		};
		switchJobButton.AttachNode(((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(57u));
	}

	private unsafe void UpdateSwitchJobButton()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!config.IsSwitchJobButton || switchJobButton == null)
		{
			return;
		}
		if (!TryGetCurrentRecipe(out var _, out var recipe))
		{
			switchJobButton.IsVisible = false;
			return;
		}
		AtkComponentButton* componentButtonById = ((AtkUnitBase)Addons.RecipeNoteAddon).GetComponentButtonById(104u);
		if (((Recipe)(ref recipe)).CraftType.RowId != LocalPlayerState.ClassJob - 8)
		{
			switchJobButton.IsVisible = true;
			if (componentButtonById != null)
			{
				((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).ToggleVisibility(false);
			}
			switchJobButton.TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetJobName(8 + ((Recipe)(ref recipe)).CraftType.RowId));
			for (uint num = 102u; num < 105; num++)
			{
				AtkComponentButton* componentButtonById2 = ((AtkUnitBase)Addons.RecipeNoteAddon).GetComponentButtonById(num);
				if (componentButtonById2 != null)
				{
					((AtkComponentButton)componentButtonById2).SetEnabledState(false);
				}
			}
			return;
		}
		switchJobButton.IsVisible = false;
		if (componentButtonById != null)
		{
			((AtkComponentNode)((AtkComponentButton)componentButtonById).OwnerNode).ToggleVisibility(true);
		}
		for (uint num2 = 102u; num2 < 105; num2++)
		{
			AtkComponentButton* componentButtonById3 = ((AtkUnitBase)Addons.RecipeNoteAddon).GetComponentButtonById(num2);
			if (componentButtonById3 != null)
			{
				((AtkComponentButton)componentButtonById3).SetEnabledState(true);
			}
		}
	}

	private unsafe void CreateCaculateRecipeButton()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!config.IsCaculateRecipeButton || caculateRecipeButton != null)
		{
			return;
		}
		caculateRecipeButton = new TextButtonNode
		{
			Position = new Vector2(228f, 490f),
			Size = new Vector2(140f, 32f),
			String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedRecipeNote-Button-CaculateRecipe"))
		};
		caculateRecipeButton.OnClick = delegate
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			uint recipeID;
			Recipe recipe;
			if (!DalamudServiceExtension.Instance().IsPluginEnabled("Raphael.Dalamud"))
			{
				PrintInstallRaphaelPluginMessage();
			}
			else if (TryGetCurrentRecipe(out recipeID, out recipe) && ((Recipe)(ref recipe)).CraftType.RowId == LocalPlayerState.ClassJob - 8)
			{
				int craftPoint = ((PlayerState)PlayerState.Instance()).GetAttributeByIndex((PlayerAttribute)11);
				int craftsmanship = ((PlayerState)PlayerState.Instance()).GetAttributeByIndex((PlayerAttribute)70);
				int control = ((PlayerState)PlayerState.Instance()).GetAttributeByIndex((PlayerAttribute)71);
				uint id = RaphaelIPC.StartCalculation(recipeID, new RaphaelCalculationConfig
				{
					BackloadProgress = true,
					EnsureReliability = true,
					TargetQuality = GetRecipeMaxQuality(recipe),
					MaxThreads = Environment.ProcessorCount,
					TimeoutSeconds = 90
				});
				caculateRecipeButton.IsEnabled = false;
				base.TaskHelper.Enqueue(delegate
				{
					//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
					RaphaelCaculationResponse calculationStatus = RaphaelIPC.GetCalculationStatus(id);
					switch (calculationStatus.Status)
					{
					case RaphaelCalculationStatus.Success:
					{
						caculateRecipeButton.IsEnabled = true;
						uint id2;
						DalamudLinkPayload copyLinkPayload = OmenServiceBase<LinkPayloadManager>.Instance().Reg(OnClickCopyPayload, out id2);
						DalamudLinkPayload previewLinkPayload = OmenServiceBase<LinkPayloadManager>.Instance().Reg(OnClickPreviewPayload, out id2);
						caculationResults[id] = new CaculationResult(calculationStatus.Actions, recipeID, craftPoint, craftsmanship, control, copyLinkPayload, previewLinkPayload);
						PrintActionsMessage(caculationResults[id]);
						return true;
					}
					case RaphaelCalculationStatus.Failed:
						caculateRecipeButton.IsEnabled = true;
						if (!string.IsNullOrWhiteSpace(calculationStatus.ErrorMessage))
						{
							OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(calculationStatus.ErrorMessage));
						}
						base.TaskHelper.Abort();
						return true;
					default:
						return false;
					}
				}, "请求技能数据");
			}
		};
		caculateRecipeButton.AttachNode(((AtkUnitBase)Addons.RecipeNoteAddon).GetNodeById(57u));
	}

	private void UpdateCaculateRecipeButton()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (config.IsCaculateRecipeButton && caculateRecipeButton != null)
		{
			if (!TryGetCurrentRecipe(out var _, out var recipe) || ((Recipe)(ref recipe)).CraftType.RowId != LocalPlayerState.ClassJob - 8)
			{
				caculateRecipeButton.IsVisible = false;
			}
			else
			{
				caculateRecipeButton.IsVisible = true;
			}
		}
	}

	private void OnClickInstallRaphaelPayload(uint id, SeString _)
	{
		if (DalamudServiceExtension.Instance().InstalledPlugins.Any((IExposedPlugin x) => x.InternalName == "Raphael.Dalamud"))
		{
			OmenServiceBase<ChatManager>.Instance().SendMessage("/xlenableplugin Raphael.Dalamud");
		}
		else if (installRaphaelTask == null)
		{
			installRaphaelTask = DalamudServiceExtension.Instance<IFramework>().RunOnTick<bool>((Func<Task<bool>>)(async () => await DalamudReflector.AddPlugin("hxxps://raw.githubusercontent.com/AtmoOmen/DalamudPlugins/main/pluginmaster.json", "Raphael.Dalamud")), default(TimeSpan), 0, default(CancellationToken)).ContinueWith((Task<bool> task) => installRaphaelTask = null);
		}
	}

	private void OnClickPreviewPayload(uint id, SeString _)
	{
		KeyValuePair<uint, CaculationResult> keyValuePair = caculationResults.FirstOrDefault<KeyValuePair<uint, CaculationResult>>((KeyValuePair<uint, CaculationResult> x) => x.Value.PreviewLinkPayload.CommandId == id);
		CaculationResult value = keyValuePair.Value;
		if ((object)value != null && value.RecipeID != 0)
		{
			AddonActionsPreview.OpenWithActions(this, keyValuePair.Value);
		}
	}

	private void OnClickCopyPayload(uint id, SeString _)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		KeyValuePair<uint, CaculationResult> keyValuePair = caculationResults.FirstOrDefault<KeyValuePair<uint, CaculationResult>>((KeyValuePair<uint, CaculationResult> x) => x.Value.CopyLinkPayload.CommandId == id);
		CaculationResult value = keyValuePair.Value;
		if ((object)value == null || value.RecipeID == 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (uint action in keyValuePair.Value.Actions)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(13, 1, stringBuilder2);
			handler.AppendLiteral("/ac ");
			handler.AppendFormatted(LuminaWrapper.GetActionName(action));
			handler.AppendLiteral(" <wait.3>");
			stringBuilder2.AppendLine(ref handler);
		}
		ImGui.SetClipboardText(ImU8String.op_Implicit(stringBuilder.ToString()));
		OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("CopiedToClipboard") ?? "");
	}

	private static void PrintActionsMessage(CaculationResult result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		SeStringBuilder val = new SeStringBuilder();
		SeStringBuilder obj = val.AddText(LanguageManager.Get("OptimizedRecipeNote-Message-CaculationResult") ?? "").Add((Payload)(object)NewLinePayload.Payload).AddText(LanguageManager.Get("Recipe") + ": ");
		Recipe recipe = result.GetRecipe();
		SeStringBuilder obj2 = obj.AddItemLink(((Recipe)(ref recipe)).ItemResult.RowId, (ItemKind)0, (string)null).AddText(" (").AddIcon(result.GetJob().ToBitmapFontIcon());
		ClassJob job = result.GetJob();
		obj2.AddText(((object)((ClassJob)(ref job)).Name/*cast due to constrained. prefix*/).ToString()).AddText(")").Add((Payload)(object)NewLinePayload.Payload)
			.AddText(LanguageManager.Get("Step") + ": ")
			.AddText(LanguageManager.Get("OptimizedRecipeNote-Message-StepsInfo", result.Actions.Count, result.Actions.Count * 3) ?? "")
			.Add((Payload)(object)NewLinePayload.Payload)
			.AddText(LanguageManager.Get("Operation") + ": ")
			.Add((Payload)(object)RawPayload.LinkTerminator)
			.Add((Payload)(object)result.CopyLinkPayload)
			.AddText("[")
			.AddUiForeground((ushort)35)
			.AddText(LanguageManager.Get("Copy") ?? "")
			.AddUiForegroundOff()
			.AddText("]")
			.Add((Payload)(object)RawPayload.LinkTerminator)
			.AddText(" / ")
			.Add((Payload)(object)RawPayload.LinkTerminator)
			.Add((Payload)(object)result.PreviewLinkPayload)
			.AddText("[")
			.AddUiForeground((ushort)35)
			.AddText(LanguageManager.Get("Preview") ?? "")
			.AddUiForegroundOff()
			.AddText("]")
			.Add((Payload)(object)RawPayload.LinkTerminator);
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(val.Build().Encode()));
	}

	private void PrintInstallRaphaelPluginMessage()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		if (installRaphaelLinkPayload == null)
		{
			installRaphaelLinkPayload = OmenServiceBase<LinkPayloadManager>.Instance().Reg(OnClickInstallRaphaelPayload, out var _);
		}
		SeString val = new SeStringBuilder().AddIcon((BitmapFontIcon)63).AddText(" " + LanguageManager.Get("OptimizedRecipeNote-Message-InstallRapheal")).Add((Payload)(object)NewLinePayload.Payload)
			.AddText(LanguageManager.Get("Operation") + ": ")
			.Add((Payload)(object)RawPayload.LinkTerminator)
			.Add((Payload)(object)installRaphaelLinkPayload)
			.AddText("[")
			.AddUiForeground((ushort)35)
			.AddText(LanguageManager.Get("Enable") + " / " + LanguageManager.Get("Install"))
			.AddUiForegroundOff()
			.AddText("]")
			.Add((Payload)(object)RawPayload.LinkTerminator)
			.Build();
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(val.Encode()));
	}

	private unsafe void OnAgentRecipeNote(AgentEvent type, AgentArgs args)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (pendingQuickSynthRemaining != 0)
		{
			return;
		}
		AgentReceiveEventArgs e = (AgentReceiveEventArgs)(object)((args is AgentReceiveEventArgs) ? args : null);
		if (e.EventKind != 1)
		{
			return;
		}
		AtkValue* atkValues = (AtkValue*)e.AtkValues;
		if (atkValues != null)
		{
			int num = ((AtkValue)atkValues).Int;
			if (num > 255)
			{
				pendingQuickSynthRemaining = num - 255;
				pendingUseHQIngredient = ((AtkValue)((byte*)atkValues + Unsafe.SizeOf<AtkValue>())).Bool;
				pendingAllNQResult = ((AtkValue)((byte*)atkValues + (nint)2 * (nint)Unsafe.SizeOf<AtkValue>())).Bool;
				((AtkValue)atkValues).SetInt(255);
				string text = LanguageManager.Get("OptimizedRecipeNote-Message-QuickSynthBatch", MathF.Ceiling((float)num / 255f));
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
			}
		}
	}

	private unsafe void OnSynthesisSimple(AddonEvent type, AddonArgs args)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.SynthesisSimple == null)
		{
			return;
		}
		AtkComponentButton* button = ((AtkUnitBase)Addons.SynthesisSimple).GetComponentButtonById(22u);
		if (button == null)
		{
			return;
		}
		uint uInt = ((AtkValue)((byte*)((AtkUnitBase)Addons.SynthesisSimple).AtkValues + (nint)3 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
		uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)Addons.SynthesisSimple).AtkValues + (nint)4 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
		if (uInt != uInt2)
		{
			return;
		}
		if (pendingQuickSynthRemaining == 0)
		{
			pendingQuickSynthRemaining = 0;
			pendingUseHQIngredient = false;
			pendingAllNQResult = false;
			if (config.IsNotifyQuickSynthesisFinish)
			{
				string text = LanguageManager.Get("OptimizedRecipeNote-Message-QuickSynthFinish");
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
				OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(text);
				NotifyHelper.SystemInformation();
				NotifyHelper.Speak(text);
			}
			return;
		}
		int batchSize = Math.Min(pendingQuickSynthRemaining, 255);
		pendingQuickSynthRemaining -= batchSize;
		base.TaskHelper.Enqueue(delegate
		{
			(*button).Click();
		});
		base.TaskHelper.Enqueue(() => !(*Addons.SynthesisSimple).IsAddonAndNodesReady() && (*Addons.RecipeNoteAddon).IsAddonAndNodesReady());
		base.TaskHelper.Enqueue(delegate
		{
			((AgentId)25).SendEvent(0uL, 9);
		});
		base.TaskHelper.Enqueue(() => (*Addons.SynthesisSimpleDialog).IsAddonAndNodesReady());
		base.TaskHelper.Enqueue(delegate
		{
			((AgentId)25).SendEvent(1uL, batchSize, pendingUseHQIngredient, pendingAllNQResult);
		});
	}

	private unsafe static bool TryGetCurrentRecipe(out uint recipeID, out Recipe recipe)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		recipeID = 0u;
		recipe = default(Recipe);
		RecipeData* recipeList = ((RecipeNote)(&((UIState)UIState.Instance()).RecipeNote)).RecipeList;
		if (recipeList == null)
		{
			return false;
		}
		RecipeEntry* selectedRecipe = ((RecipeData)recipeList).SelectedRecipe;
		if (selectedRecipe == null)
		{
			return false;
		}
		if (LuminaGetter.TryGetRow<Recipe>((uint)((RecipeEntry)selectedRecipe).RecipeId, out Recipe item))
		{
			Item value = ((Recipe)(ref item)).ItemResult.Value;
			if (((Item)(ref value)).RowId != 0)
			{
				recipeID = ((RecipeEntry)selectedRecipe).RecipeId;
				recipe = item;
				return true;
			}
		}
		return false;
	}

	private static int GetRecipeMaxQuality(Recipe recipe)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		RecipeLevelTable recipeLevelTable = GetRecipeLevelTable(recipe);
		return (int)((float)((RecipeLevelTable)(ref recipeLevelTable)).Quality * (float)(int)((Recipe)(ref recipe)).QualityFactor / 100f);
	}

	private static RecipeLevelTable GetRecipeLevelTable(Recipe recipe)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (((Recipe)(ref recipe)).Number != 0 || LocalPlayerState.CurrentLevel >= 100)
		{
			return ((Recipe)(ref recipe)).RecipeLevelTable.Value;
		}
		return ((IEnumerable<RecipeLevelTable>)LuminaGetter.Get<RecipeLevelTable>()).First((RecipeLevelTable x) => ((RecipeLevelTable)(ref x)).ClassJobLevel == LocalPlayerState.CurrentLevel);
	}
}
