using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Duty;

public class BetterMKDSupportJobList : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool AddonIsDragRealAction = true;

		public List<uint> AddonSupportJobOrder = new List<uint>();
	}

	public class AddonDRMKDJobList(BetterMKDSupportJobList module) : NativeAddon
	{
		private class SupportJobActionListNode(AddonDRMKDJobList addon) : SimpleComponentNode
		{
			public class SupportActionNode : DragDropNode
			{
				[Flags]
				public enum ActionSlotHiddenFlag : byte
				{
					Action0 = 1,
					Action1 = 2,
					Action2 = 4,
					Action3 = 8,
					Action4 = 0x10
				}

				public CrescentSupportJob Job { get; private set; }

				public SupportJobActionListNode List { get; private set; }

				public AddonDRMKDJobList Addon { get; private set; }

				public bool IsRealAction { get; private set; }

				public int ActionIndex { get; private set; }

				public uint ActionID { get; private set; }

				public bool IsDefault { get; private set; }

				public bool IsHidden { get; private set; }

				public SimpleNineGridNode DefaultIconNode { get; private set; }

				public SimpleNineGridNode HiddenIconNode { get; private set; }

				public static byte DefaultAction { get; private set; }

				public static ActionSlotHiddenFlag ActionHiddenFlags { get; private set; }

				public static HashSet<byte> HiddenActions { get; private set; } = new HashSet<byte>();

				public SupportActionNode(AddonDRMKDJobList addon, CrescentSupportJob job, SupportJobActionListNode list, uint actionID, int actionIndex, bool isRealAction = false)
				{
					Job = job;
					List = list;
					Addon = addon;
					IsRealAction = isRealAction;
					ActionIndex = actionIndex;
					ActionID = actionID;
					DefaultIconNode = new SimpleNineGridNode
					{
						TexturePath = "ui/uld/ContentsReplaySetting_hr1.tex",
						TextureCoordinates = new Vector2(36f, 44f),
						TextureSize = new Vector2(36f),
						Size = new Vector2(22f),
						Position = new Vector2(22f, 24f)
					};
					DefaultIconNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
					HiddenIconNode = new SimpleNineGridNode
					{
						TexturePath = "ui/uld/MKDWindow_hr1.tex",
						TextureCoordinates = new Vector2(64f, 82f),
						TextureSize = new Vector2(20f),
						Size = new Vector2(22f),
						Position = new Vector2(22f, 24f)
					};
					HiddenIconNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
					UpdateActionInfo();
					Toggle(IsRealAction);
				}

				public unsafe void Toggle(bool isRealAction)
				{
					IsRealAction = isRealAction;
					base.Payload = new DragDropPayload
					{
						Type = (DragDropType)(IsRealAction ? 7 : 13),
						Int2 = (IsRealAction ? ((int)ActionID) : (31 + ActionIndex))
					};
					base.OnRollOver = delegate(DragDropNode node)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_002a: Unknown result type (might be due to invalid IL or missing references)
						AtkTooltipArgs val = new AtkTooltipArgs
						{
							ActionArgs = 
							{
								Flags = 1
							},
							ActionArgs = 
							{
								Kind = (DetailKind)(isRealAction ? 30 : 32)
							},
							ActionArgs = 
							{
								Id = (IsRealAction ? ((int)ActionID) : (31 + ActionIndex))
							}
						};
						((AtkTooltipManager)(&((AtkStage)AtkStage.Instance()).TooltipManager)).ShowTooltip((AtkTooltipType)4, Addon.AddonId, (AtkResNode*)node, &val, (delegate* unmanaged[Stdcall]<float*, float*, AtkResNode*, void>)null, false, true);
					};
					base.OnRollOut = delegate(DragDropNode node)
					{
						node.HideTooltip();
					};
					base.OnClicked = delegate
					{
						UpdateActionInfo();
						if (IsDefault)
						{
							if (HiddenActions.Count != Job.Actions.Count - 1)
							{
								List<uint> list = Job.Actions.Select((KeyValuePair<uint, byte> x) => x.Key).ToList();
								for (int num = 0; num < list.Count; num++)
								{
									if (num != ActionIndex && !HiddenActions.Contains((byte)num))
									{
										ActionSlotHiddenFlag actionSlotHiddenFlag = ActionHiddenFlags | IndexToHiddenFlag(ActionIndex);
										AgentMKDSupportJob.UpdateJobSettings(Job.DataID, (byte)num, (byte)actionSlotHiddenFlag);
										UpdateActionInfo();
										break;
									}
								}
							}
						}
						else if (IsHidden)
						{
							ActionSlotHiddenFlag actionSlotHiddenFlag2 = (ActionSlotHiddenFlag)((uint)ActionHiddenFlags & (uint)(byte)(~(int)IndexToHiddenFlag(ActionIndex)));
							AgentMKDSupportJob.UpdateJobSettings(Job.DataID, DefaultAction, (byte)actionSlotHiddenFlag2);
							UpdateActionInfo();
						}
						else
						{
							AgentMKDSupportJob.UpdateJobSettings(Job.DataID, (byte)ActionIndex, (byte)ActionHiddenFlags);
							UpdateActionInfo();
						}
					};
				}

				public unsafe void UpdateActionInfo(bool updateOthers = true)
				{
					byte* ptr = stackalloc byte[1];
					byte* ptr2 = stackalloc byte[1];
					AgentMKDSupportJob.GetJobSettings(Job.DataID, ptr, ptr2);
					DefaultAction = *ptr;
					ActionHiddenFlags = (ActionSlotHiddenFlag)(*ptr2);
					HiddenActions.Clear();
					for (byte b = 0; b < 5; b++)
					{
						if (ActionHiddenFlags.HasFlag(IndexToHiddenFlag(b)))
						{
							HiddenActions.Add(b);
						}
					}
					IsDefault = DefaultAction == (byte)ActionIndex;
					IsHidden = HiddenActions.Contains((byte)ActionIndex);
					DefaultIconNode.IsVisible = IsDefault;
					HiddenIconNode.IsVisible = IsHidden;
					if (!updateOthers)
					{
						return;
					}
					foreach (SupportActionNode actionDragDropNode in List.ActionDragDropNodes)
					{
						if (actionDragDropNode.ActionID != ActionID)
						{
							actionDragDropNode.UpdateActionInfo(updateOthers: false);
						}
					}
				}

				public static ActionSlotHiddenFlag IndexToHiddenFlag(int index)
				{
					return index switch
					{
						0 => ActionSlotHiddenFlag.Action0, 
						1 => ActionSlotHiddenFlag.Action1, 
						2 => ActionSlotHiddenFlag.Action2, 
						3 => ActionSlotHiddenFlag.Action3, 
						4 => ActionSlotHiddenFlag.Action4, 
						_ => ActionSlotHiddenFlag.Action0, 
					};
				}
			}

			public SimpleNineGridNode BackgroundNode { get; private set; }

			public SimpleNineGridNode BorderNode { get; private set; }

			public SimpleNineGridNode HeaderBackgroundNode { get; private set; }

			public SimpleNineGridNode HeaderBorderNode { get; private set; }

			public VerticalListNode ActionListNode { get; private set; }

			public TextureButtonNode CloseButtonNode { get; private set; }

			public TextureButtonNode SettingButtonNode { get; private set; }

			public CheckboxNode IsRealActionNode { get; private set; }

			public List<SupportActionNode> ActionDragDropNodes { get; private set; } = new List<SupportActionNode>();

			public unsafe void LoadNodes(BetterMKDSupportJobList module, CrescentSupportJob presetJob, bool isCurrentFoucused)
			{
				//IL_0445: Unknown result type (might be due to invalid IL or missing references)
				//IL_044a: Unknown result type (might be due to invalid IL or missing references)
				//IL_045e: Unknown result type (might be due to invalid IL or missing references)
				//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
				//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
				//IL_0df5: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e00: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e2f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e79: Unknown result type (might be due to invalid IL or missing references)
				//IL_0e80: Unknown result type (might be due to invalid IL or missing references)
				//IL_06a8: Unknown result type (might be due to invalid IL or missing references)
				//IL_06ad: Unknown result type (might be due to invalid IL or missing references)
				//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
				BackgroundNode = new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(0f),
					TextureSize = new Vector2(500f, 380f),
					TexturePath = "ui/uld/MKDWallPaper_hr1.tex",
					IsVisible = true,
					Size = Size + new Vector2(50f, 0f),
					Position = new Vector2(-2f),
					Alpha = (isCurrentFoucused ? 0.9f : 0.6f)
				};
				BackgroundNode.AddTimeline(CreateFocusAlphaTimeline(153, 229));
				BackgroundNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
				SimpleNineGridNode obj = new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(110f, 0f),
					TextureSize = new Vector2(9f, 41f),
					TexturePath = "ui/uld/MKDWindow_hr1.tex",
					IsVisible = true
				};
				Vector2 size = Size;
				size.Y = 40f;
				obj.Size = size + new Vector2(54f, 0f);
				obj.Position = new Vector2(-2f);
				obj.Alpha = 1f;
				HeaderBackgroundNode = obj;
				HeaderBackgroundNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
				SimpleNineGridNode obj2 = new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(63f, 55f),
					TextureSize = new Vector2(47f, 6f),
					TexturePath = "ui/uld/MKDWindow_hr1.tex",
					IsVisible = true
				};
				size = Size;
				size.Y = 6f;
				obj2.Size = size + new Vector2(58f, 0f);
				obj2.Position = new Vector2(0f, 36f);
				obj2.Alpha = 1f;
				obj2.AddColor = new Vector3(-0.196078f);
				HeaderBorderNode = obj2;
				HeaderBorderNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
				BorderNode = new SimpleNineGridNode
				{
					TextureCoordinates = new Vector2(1f, 0f),
					TextureSize = new Vector2(60f, 70f),
					TexturePath = "ui/uld/MKDWindow_hr1.tex",
					IsVisible = true,
					Size = Size + new Vector2(64f, 14f),
					Position = new Vector2(-8.5f, -5f),
					Alpha = 0.9f,
					Offsets = new Vector4(24f),
					AddColor = (isCurrentFoucused ? new Vector3(10f / 51f) : new Vector3(-10f / 51f))
				};
				BorderNode.AddTimeline(CreateFocusAddColorTimeline(-50, 50));
				BorderNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
				ActionListNode = new VerticalListNode
				{
					Size = Size + new Vector2(50f, 0f),
					IsVisible = true,
					Position = new Vector2(10f, 30f)
				};
				ActionListNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
				ActionListNode.AddDummy(25f);
				uint key;
				byte value;
				ReadOnlySeString name;
				foreach (KeyValuePair<uint, byte> action in presetJob.Actions)
				{
					action.Deconstruct(out key, out value);
					uint rowID = key;
					byte b = value;
					if (LuminaGetter.TryGetRow<Action>(rowID, out Action item))
					{
						HorizontalListNode horizontalListNode = new HorizontalListNode
						{
							IsVisible = true,
							Size = new Vector2(40f)
						};
						SupportActionNode supportActionNode = new SupportActionNode(addon, presetJob, this, ((Action)(ref item)).RowId, ActionDragDropNodes.Count, module.config.AddonIsDragRealAction)
						{
							Size = new Vector2(40f),
							IsVisible = true,
							IconId = ((Action)(ref item)).Icon,
							AcceptedType = (DragDropType)1,
							IsDraggable = true,
							IsClickable = true
						};
						ActionDragDropNodes.Add(supportActionNode);
						horizontalListNode.AddNode(supportActionNode);
						horizontalListNode.AddDummy(10f);
						TextNode textNode = new TextNode();
						string text = b.ToSESmallCount();
						name = ((Action)(ref item)).Name;
						textNode.String = ReadOnlySeString.op_Implicit("\ue06a " + text + ": " + ((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString());
						textNode.FontSize = 14u;
						textNode.IsVisible = true;
						textNode.Size = new Vector2(Size.X - 20f, 40f);
						textNode.AlignmentType = (AlignmentType)3;
						textNode.TextColor = ColorHelper.GetColor(50u);
						textNode.TextOutlineColor = ColorHelper.GetColor((presetJob.CurrentLevel >= b) ? 32u : 4u);
						textNode.TextFlags = (TextFlags)16;
						TextNode textNode2 = textNode;
						horizontalListNode.AddNode(textNode2);
						while (textNode2.FontSize > 1 && textNode2.GetTextDrawSize(textNode2.String).X > textNode2.Size.X)
						{
							key = textNode2.FontSize--;
						}
						ActionListNode.AddNode(horizontalListNode);
						ActionListNode.AddDummy(10f);
					}
				}
				ActionListNode.AddDummy(20f);
				foreach (KeyValuePair<uint, byte> trait2 in presetJob.Traits)
				{
					trait2.Deconstruct(out key, out value);
					uint trait = key;
					byte b2 = value;
					if (!LuminaGetter.TryGetRow<MKDTrait>(trait, out MKDTrait item2))
					{
						continue;
					}
					name = ((MKDTrait)(ref item2)).Name;
					if (!((ReadOnlySeString)(ref name)).IsEmpty)
					{
						HorizontalListNode horizontalListNode2 = new HorizontalListNode
						{
							IsVisible = true,
							Size = new Vector2(44f)
						};
						DragDropNode node = new DragDropNode
						{
							Size = new Vector2(44f),
							IsVisible = true,
							IconId = (uint)((MKDTrait)(ref item2)).Icon,
							AcceptedType = (DragDropType)1,
							IsDraggable = false,
							Payload = new DragDropPayload
							{
								Int2 = (int)trait
							},
							IsClickable = false,
							OnRollOver = delegate(DragDropNode dragDropNode)
							{
								//IL_0002: Unknown result type (might be due to invalid IL or missing references)
								//IL_001e: Unknown result type (might be due to invalid IL or missing references)
								AtkTooltipArgs val = new AtkTooltipArgs
								{
									ActionArgs = 
									{
										Flags = 1
									},
									ActionArgs = 
									{
										Kind = (DetailKind)60
									},
									ActionArgs = 
									{
										Id = (int)trait
									}
								};
								((AtkTooltipManager)(&((AtkStage)AtkStage.Instance()).TooltipManager)).ShowTooltip((AtkTooltipType)4, addon.AddonId, (AtkResNode*)dragDropNode, &val, (delegate* unmanaged[Stdcall]<float*, float*, AtkResNode*, void>)null, false, true);
							},
							OnRollOut = delegate(DragDropNode dragDropNode)
							{
								dragDropNode.HideTooltip();
							}
						};
						horizontalListNode2.AddNode(node);
						horizontalListNode2.AddDummy(10f);
						TextNode textNode3 = new TextNode();
						string text2 = b2.ToSESmallCount();
						name = ((MKDTrait)(ref item2)).Name;
						textNode3.String = ReadOnlySeString.op_Implicit("\ue06a " + text2 + ": " + ((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString());
						textNode3.FontSize = 14u;
						textNode3.IsVisible = true;
						textNode3.Size = new Vector2(Size.X - 20f, 44f);
						textNode3.AlignmentType = (AlignmentType)3;
						textNode3.TextColor = ColorHelper.GetColor(50u);
						textNode3.TextOutlineColor = ColorHelper.GetColor((presetJob.CurrentLevel >= b2) ? 32u : 4u);
						textNode3.TextFlags = (TextFlags)16;
						TextNode textNode4 = textNode3;
						textNode4.AutoAdjustTextSize();
						horizontalListNode2.AddNode(textNode4);
						ActionListNode.AddNode(horizontalListNode2);
						ActionListNode.AddDummy(10f);
					}
				}
				CloseButtonNode = new TextureButtonNode
				{
					Size = new Vector2(28f),
					Position = new Vector2(210f, 8f),
					IsVisible = true,
					TexturePath = "ui/uld/WindowA_Button_hr1.tex",
					TextureCoordinates = new Vector2(0f),
					TextureSize = new Vector2(28f),
					OnClick = delegate
					{
						IsVisible = false;
						module.mkdJobListAddon.PressedButtonOnce = false;
						CollisionNode collisionNode = module.mkdJobListAddon.WindowNode.CollisionNode;
						Vector2 size2 = module.mkdJobListAddon.WindowNode.CollisionNode.Size;
						size2.X = 500f;
						collisionNode.Size = size2;
						WindowNodeBase? windowNode = module.mkdJobListAddon.WindowNode;
						size2 = module.mkdJobListAddon.WindowNode.Size;
						size2.X = 500f;
						windowNode.Size = size2;
						addon.ApplyControllerNavigation();
					}
				};
				CloseButtonNode.ImageNode.AddColor = new Vector3(-10f / 51f);
				CloseButtonNode.AddTimeline(new TimelineBuilder().BeginFrameSet(1, 20).AddLabel(1, 1, (AtkTimelineJumpBehavior)0, 0).AddLabel(10, 0, (AtkTimelineJumpBehavior)1, 1)
					.AddLabel(11, 2, (AtkTimelineJumpBehavior)0, 0)
					.AddLabel(20, 0, (AtkTimelineJumpBehavior)1, 2)
					.EndFrameSet()
					.Build());
				SimpleImageNode imageNode = CloseButtonNode.ImageNode;
				FrameSetBuilder frameSetBuilder = new TimelineBuilder().BeginFrameSet(1, 10);
				Vector3? addColor = new Vector3(0f);
				FrameSetBuilder frameSetBuilder2 = frameSetBuilder.AddFrame(1, null, null, addColor);
				Vector3? addColor2 = new Vector3(-50f);
				FrameSetBuilder frameSetBuilder3 = frameSetBuilder2.AddFrame(4, null, null, addColor2).EndFrameSet().BeginFrameSet(11, 20);
				Vector3? addColor3 = new Vector3(0f);
				FrameSetBuilder frameSetBuilder4 = frameSetBuilder3.AddFrame(11, null, null, addColor3);
				Vector3? addColor4 = new Vector3(50f);
				imageNode.AddTimeline(frameSetBuilder4.AddFrame(14, null, null, addColor4).EndFrameSet().Build());
				CloseButtonNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
				SettingButtonNode = new TextureButtonNode
				{
					Size = new Vector2(16f),
					Position = new Vector2(192f, 14f),
					IsVisible = true,
					TexturePath = "ui/uld/WindowA_Button_hr1.tex",
					TextureCoordinates = new Vector2(44f, 0f),
					TextureSize = new Vector2(16f),
					OnClick = delegate
					{
						((AgentInterface)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)483)).Show();
					}
				};
				SettingButtonNode.ImageNode.AddColor = new Vector3(1f);
				SettingButtonNode.AddTimeline(new TimelineBuilder().BeginFrameSet(1, 20).AddLabel(1, 1, (AtkTimelineJumpBehavior)0, 0).AddLabel(10, 0, (AtkTimelineJumpBehavior)1, 1)
					.AddLabel(11, 2, (AtkTimelineJumpBehavior)0, 0)
					.AddLabel(20, 0, (AtkTimelineJumpBehavior)1, 2)
					.EndFrameSet()
					.Build());
				SimpleImageNode imageNode2 = SettingButtonNode.ImageNode;
				FrameSetBuilder frameSetBuilder5 = new TimelineBuilder().BeginFrameSet(1, 10);
				addColor = new Vector3(200f);
				FrameSetBuilder frameSetBuilder6 = frameSetBuilder5.AddFrame(1, null, null, addColor);
				addColor2 = new Vector3(100f);
				FrameSetBuilder frameSetBuilder7 = frameSetBuilder6.AddFrame(4, null, null, addColor2).EndFrameSet().BeginFrameSet(11, 20);
				addColor3 = new Vector3(100f);
				FrameSetBuilder frameSetBuilder8 = frameSetBuilder7.AddFrame(11, null, null, addColor3);
				addColor4 = new Vector3(200f);
				imageNode2.AddTimeline(frameSetBuilder8.AddFrame(14, null, null, addColor4).EndFrameSet().Build());
				SettingButtonNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
				IsRealActionNode = new CheckboxNode
				{
					IsVisible = true,
					Position = new Vector2(10f, 8f),
					Size = new Vector2(Width, 28f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterMKDSupportJobList-DragRealActionIcon")),
					TextTooltip = ReadOnlySeString.op_Implicit(new SeStringBuilder().AddIcon((BitmapFontIcon)101).AddText(" " + LanguageManager.Get("BetterMKDSupportJobList-DragRealActionIcon-Help")).Build()
						.Encode()),
					IsChecked = module.config.AddonIsDragRealAction,
					IsEnabled = true,
					OnClick = delegate(bool flag)
					{
						module.config.AddonIsDragRealAction = flag;
						module.config.Save(module);
						ActionDragDropNodes.ForEach(delegate(SupportActionNode x)
						{
							x.Toggle(flag);
						});
					}
				};
				TextNode label = IsRealActionNode.Label;
				label.TextFlags = (TextFlags)(label.TextFlags | 0x28);
				IsRealActionNode.Label.TextColor = ColorHelper.GetColor(50u);
				IsRealActionNode.Label.TextOutlineColor = ColorHelper.GetColor(502u);
				IsRealActionNode.AttachNode((NodeBase?)this, NodePosition.AsLastChild);
				AddTimeline(new TimelineBuilder().BeginFrameSet(1, 19).AddLabelPair(1, 9, 17).AddLabelPair(10, 19, 18)
					.EndFrameSet()
					.Build());
				OnFocusChanged(isCurrentFoucused);
			}

			public void OnFocusChanged(bool isFocusedNow)
			{
				base.Timeline?.PlayAnimation(isFocusedNow ? 18 : 17, force: true);
			}
		}

		private const int MAX_ITEMS_PER_ROW = 5;

		private const float WINDOW_WIDTH = 500f;

		private const float ACTION_PANEL_WIDTH = 200f;

		private const float EXPANDED_WIDTH = 750f;

		private const float HEADER_HEIGHT = 70f;

		private const float ROW_HEIGHT = 72f;

		private const float ROW_SPACING = 21f;

		private const float MIN_HEIGHT = 450f;

		private const float BOTTOM_PADDING = 30f;

		private const int LABEL_UNFOCUSED = 17;

		private const int LABEL_FOCUSED = 18;

		private readonly Dictionary<uint, TextureButtonNode> supportJobButtons = new Dictionary<uint, TextureButtonNode>();

		private SimpleNineGridNode backgroundNode;

		private SimpleNineGridNode borderNode;

		private TextureButtonNode closeButtonNode;

		private SimpleNineGridNode headerBackgroundNode;

		private SimpleNineGridNode headerBorderNode;

		private bool isFocused;

		private readonly Dictionary<uint, SupportJobActionListNode> jobActionNodes = new Dictionary<uint, SupportJobActionListNode>();

		private VerticalListNode jobContainer;

		private SimpleNineGridNode moonPatternNode;

		private SimpleNineGridNode patternLeftCornerNode;

		private SimpleNineGridNode patternLeftNode;

		private SimpleNineGridNode patternRightNode;

		public bool PressedButtonOnce { get; set; }

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			List<(MKDSupportJob, CrescentSupportJob)> list = new List<(MKDSupportJob, CrescentSupportJob)>();
			foreach (CrescentSupportJob allJob in CrescentSupportJob.AllJobs)
			{
				list.Add((allJob.GetData(), allJob));
			}
			list = list.OrderBy<(MKDSupportJob, CrescentSupportJob), int>(delegate((MKDSupportJob Data, CrescentSupportJob Job) x)
			{
				int num2 = module.config.AddonSupportJobOrder.IndexOf(((MKDSupportJob)(ref x.Data)).RowId);
				return (num2 >= 0) ? num2 : int.MaxValue;
			}).ThenBy<(MKDSupportJob, CrescentSupportJob), uint>(((MKDSupportJob Data, CrescentSupportJob Job) x) => ((MKDSupportJob)(ref x.Data)).RowId).ToList();
			int num = Math.Max(1, (list.Count + 5 - 1) / 5);
			float val = 70f + 72f * (float)num + 21f * (float)Math.Max(0, num - 1) + 30f;
			float height = Math.Max(450f, val);
			PressedButtonOnce = false;
			SetWindowSize(500f, height);
			base.RootNode.Size = base.Size + new Vector2(200f, 0f);
			base.RootNode.AddTimeline(new TimelineBuilder().BeginFrameSet(1, 19).AddLabelPair(1, 9, 17).AddLabelPair(10, 19, 18)
				.EndFrameSet()
				.Build());
			WindowNode obj = (WindowNode)base.WindowNode;
			obj.CloseButtonNode.IsVisible = false;
			obj.BackgroundTextureNode.IsVisible = false;
			obj.BorderTextureNode.Alpha = 0f;
			obj.TitleNode.IsVisible = false;
			CreateWindowStyle();
			CreateJobContainer(list, num);
			CreateWindowControll();
			((AtkUnitBase)addon).FocusNode = ApplyControllerNavigation();
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			if (Addons.MKDInfo == null || DalamudServiceExtension.Instance<IKeyState>()[(VirtualKey)27])
			{
				Close();
				return;
			}
			bool focused = ((WindowNode)base.WindowNode).Focused;
			if (focused == isFocused)
			{
				return;
			}
			isFocused = focused;
			base.RootNode.Timeline?.PlayAnimation(isFocused ? 18 : 17, force: true);
			foreach (SupportJobActionListNode value in jobActionNodes.Values)
			{
				value.OnFocusChanged(isFocused);
			}
		}

		private unsafe void CreateJobContainer(List<(MKDSupportJob Data, CrescentSupportJob Job)> supportJobs, int rowCount)
		{
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0996: Unknown result type (might be due to invalid IL or missing references)
			//IL_099b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd4: Unknown result type (might be due to invalid IL or missing references)
			jobContainer = new VerticalListNode
			{
				Position = new Vector2(0f, 0f),
				Size = new Vector2(500f, base.Size.Y - 30f),
				IsVisible = true,
				FirstItemSpacing = 70f,
				ItemSpacing = 21f
			};
			List<HorizontalFlexNode> list = new List<HorizontalFlexNode>();
			for (int i = 0; i < rowCount; i++)
			{
				HorizontalFlexNode item = new HorizontalFlexNode
				{
					Position = new Vector2(0f, 0f),
					Size = new Vector2(500f, 72f),
					IsVisible = true,
					AlignmentFlags = FlexFlags.CenterHorizontally,
					ItemSpacing = 10f
				};
				list.Add(item);
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			for (int j = 0; j < supportJobs.Count; j++)
			{
				(MKDSupportJob, CrescentSupportJob) tuple = supportJobs[j];
				MKDSupportJob data = tuple.Item1;
				CrescentSupportJob presetJob = tuple.Item2;
				int index = j / 5;
				SupportJobActionListNode supportJobActionListNode = new SupportJobActionListNode(this)
				{
					Position = new Vector2(500f, 0f),
					Size = new Vector2(200f, backgroundNode.Height)
				};
				supportJobActionListNode.AttachNode(this);
				jobActionNodes[((MKDSupportJob)(ref data)).RowId] = supportJobActionListNode;
				string text = string.Empty;
				if (presetJob.UnlockType != CrescentSupportJobUnlockType.None)
				{
					text = $"{LanguageManager.Get("BetterMKDSupportJobList-SupportJobUnlockLink")}:\n{presetJob.UnlockLinkName}\n[{presetJob.UnlockTypeName}]";
				}
				TextureButtonNode textureButtonNode = new TextureButtonNode
				{
					Size = new Vector2(53f),
					IsVisible = true,
					IsEnabled = true,
					TextureCoordinates = new Vector2((int)(((MKDSupportJob)(ref data)).RowId % 5 * 28), (int)(((MKDSupportJob)(ref data)).RowId / 5 * 28)),
					TexturePath = "ui/uld/MKDSupportJobIcon_hr1.tex",
					TextureSize = new Vector2(28f, 28f),
					OnClick = delegate
					{
						if ((!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] || CrescentSupportJob.Freelancer.CurrentLevel >= 24) && !presetJob.IsThisJob() && presetJob.CurrentLevel != 0)
						{
							presetJob.ChangeTo();
							Close();
						}
					},
					TextTooltip = ReadOnlySeString.op_Implicit((!string.IsNullOrEmpty(text) && presetJob.CurrentLevel == 0) ? text : LuminaWrapper.GetMKDSupportJobDescription(presetJob.DataID))
				};
				supportJobButtons[((MKDSupportJob)(ref data)).RowId] = textureButtonNode;
				if (presetJob.IsThisJob())
				{
					textureButtonNode.AddColor = new Vector3(0.5882353f);
				}
				else
				{
					textureButtonNode.AddTimeline(new TimelineBuilder().BeginFrameSet(1, 59).AddLabelPair(1, 9, 1).AddLabelPair(10, 19, 2)
						.AddLabelPair(20, 29, 3)
						.AddLabelPair(30, 39, 7)
						.AddLabelPair(40, 49, 6)
						.AddLabelPair(50, 59, 4)
						.EndFrameSet()
						.Build());
					SimpleImageNode imageNode = textureButtonNode.ImageNode;
					TimelineBuilder timelineBuilder = new TimelineBuilder();
					Vector2? position = Vector2.Zero;
					byte? alpha = (byte)byte.MaxValue;
					Vector3? multiplyColor = new Vector3(100f);
					Vector2? scale = new Vector2(1f);
					FrameSetBuilder frameSetBuilder = timelineBuilder.AddFrameSetWithFrame(1, 9, 1, position, alpha, null, multiplyColor, null, scale).BeginFrameSet(10, 19);
					Vector2? position2 = Vector2.Zero;
					byte? alpha2 = (byte)byte.MaxValue;
					Vector3? multiplyColor2 = new Vector3(100f);
					Vector2? scale2 = new Vector2(1f);
					FrameSetBuilder frameSetBuilder2 = frameSetBuilder.AddFrame(10, position2, alpha2, null, multiplyColor2, null, scale2);
					Vector2? position3 = new Vector2(-1f);
					byte? alpha3 = (byte)byte.MaxValue;
					Vector3? vector = new Vector3(100f);
					Vector3? addColor = new Vector3(50f);
					Vector3? multiplyColor3 = vector;
					Vector2? scale3 = new Vector2(1.05f);
					TimelineBuilder timelineBuilder2 = frameSetBuilder2.AddFrame(12, position3, alpha3, addColor, multiplyColor3, null, scale3).EndFrameSet();
					Vector2? position4 = new Vector2(-1f);
					byte? alpha4 = (byte)byte.MaxValue;
					Vector3? vector2 = new Vector3(100f);
					Vector3? addColor2 = new Vector3(50f);
					Vector3? multiplyColor4 = vector2;
					Vector2? scale4 = new Vector2(1.05f);
					TimelineBuilder timelineBuilder3 = timelineBuilder2.AddFrameSetWithFrame(20, 29, 20, position4, alpha4, addColor2, multiplyColor4, null, scale4);
					Vector2? position5 = Vector2.Zero;
					byte? alpha5 = (byte)178;
					Vector3? multiplyColor5 = new Vector3(50f);
					Vector2? scale5 = new Vector2(1f);
					TimelineBuilder timelineBuilder4 = timelineBuilder3.AddFrameSetWithFrame(30, 39, 30, position5, alpha5, null, multiplyColor5, null, scale5);
					Vector2? position6 = new Vector2(-1f);
					byte? alpha6 = (byte)byte.MaxValue;
					Vector3? vector3 = new Vector3(100f);
					Vector3? addColor3 = new Vector3(50f);
					Vector3? multiplyColor6 = vector3;
					Vector2? scale6 = new Vector2(1.05f);
					FrameSetBuilder frameSetBuilder3 = timelineBuilder4.AddFrameSetWithFrame(40, 49, 40, position6, alpha6, addColor3, multiplyColor6, null, scale6).BeginFrameSet(50, 59);
					Vector2? position7 = new Vector2(-1f);
					byte? alpha7 = (byte)byte.MaxValue;
					Vector3? vector4 = new Vector3(100f);
					Vector3? addColor4 = new Vector3(50f);
					Vector3? multiplyColor7 = vector4;
					Vector2? scale7 = new Vector2(1.05f);
					FrameSetBuilder frameSetBuilder4 = frameSetBuilder3.AddFrame(50, position7, alpha7, addColor4, multiplyColor7, null, scale7);
					Vector2? position8 = Vector2.Zero;
					byte? alpha8 = (byte)byte.MaxValue;
					Vector3? multiplyColor8 = new Vector3(100f);
					Vector2? scale8 = new Vector2(1f);
					TimelineBuilder timelineBuilder5 = frameSetBuilder4.AddFrame(52, position8, alpha8, null, multiplyColor8, null, scale8).EndFrameSet();
					Vector2? position9 = new Vector2(-1f);
					byte? alpha9 = (byte)byte.MaxValue;
					Vector3? addColor5 = new Vector3(50f);
					Vector3? multiplyColor9 = new Vector3(100f);
					Vector2? scale9 = new Vector2(1.05f);
					TimelineBuilder timelineBuilder6 = timelineBuilder5.AddFrameSetWithFrame(130, 139, 130, position9, alpha9, addColor5, multiplyColor9, null, scale9);
					Vector2? position10 = Vector2.Zero;
					byte? alpha10 = (byte)byte.MaxValue;
					Vector3? multiplyColor10 = new Vector3(100f);
					Vector2? scale10 = new Vector2(1f);
					TimelineBuilder timelineBuilder7 = timelineBuilder6.AddFrameSetWithFrame(140, 149, 140, position10, alpha10, null, multiplyColor10, null, scale10);
					Vector2? position11 = Vector2.Zero;
					byte? alpha11 = (byte)byte.MaxValue;
					Vector3? multiplyColor11 = new Vector3(100f);
					Vector2? scale11 = new Vector2(1f);
					imageNode.AddTimeline(timelineBuilder7.AddFrameSetWithFrame(150, 159, 150, position11, alpha11, null, multiplyColor11, null, scale11).Build());
				}
				textureButtonNode.AddEvent((AtkEventType)6, (Action)ShowJobActionsOnHover);
				textureButtonNode.AddEvent((AtkEventType)12, (Action)ShowJobActionsOnHover);
				textureButtonNode.AddEvent((AtkEventType)23, (Action)delegate
				{
					PressedButtonOnce = true;
					ShowJobActions();
				});
				if (presetJob.CurrentLevel == 0)
				{
					textureButtonNode.Alpha = 0.5f;
				}
				textureButtonNode.ImageNode.Size = new Vector2(53f);
				string text2 = ((object)((MKDSupportJob)(ref data)).Name/*cast due to constrained. prefix*/).ToString().Replace("Phantom", string.Empty).Replace("辅助", string.Empty)
					.Replace("サポート", string.Empty)
					.Replace("서포트", string.Empty)
					.Trim();
				((RentedSeStringBuilder)(ref val))._002Ector();
				try
				{
					TextNode obj = new TextNode
					{
						String = ReadOnlySeString.op_Implicit(((RentedSeStringBuilder)(ref val)).Builder.PushEdgeColorType(32u).Append(text2).PopEdgeColorType()
							.GetViewAsSpan()),
						FontSize = 12u,
						IsVisible = true,
						Size = new Vector2(73f, 24f),
						Position = new Vector2(-10f, 48f),
						TextColor = ColorHelper.GetColor(50u),
						AlignmentType = (AlignmentType)4,
						TextFlags = (TextFlags)16
					};
					obj.AutoAdjustTextSize();
					obj.AttachNode((NodeBase?)textureButtonNode, NodePosition.AsLastChild);
					SimpleNineGridNode simpleNineGridNode = new SimpleNineGridNode();
					simpleNineGridNode.TextureCoordinates = new Vector2(64f, 62f);
					simpleNineGridNode.TexturePath = "ui/uld/MKDWindow_hr1.tex";
					simpleNineGridNode.TextureSize = new Vector2(32f, 20f);
					simpleNineGridNode.IsVisible = presetJob.CurrentLevel == presetJob.MaxLevel;
					simpleNineGridNode.Size = new Vector2(32f, 20f);
					simpleNineGridNode.Position = new Vector2(10.5f, -15f);
					simpleNineGridNode.AddColor = (presetJob.IsThisJob() ? new Vector3(-20f / 51f) : default(Vector3));
					simpleNineGridNode.AttachNode((NodeBase?)textureButtonNode, NodePosition.AsLastChild);
					string value = ((presetJob.MaxLevel == 0) ? "∞" : $"{presetJob.MaxLevel}");
					TextNode textNode = new TextNode();
					textNode.String = ReadOnlySeString.op_Implicit(new SeStringBuilder().AddUiGlow((ushort)34).Append($"{presetJob.CurrentLevel} / {value}").AddUiGlowOff()
						.Build()
						.Encode());
					textNode.FontSize = 14u;
					textNode.IsVisible = presetJob.CurrentLevel > 0 && presetJob.CurrentLevel != presetJob.MaxLevel;
					textNode.Size = new Vector2(53f, 24f);
					textNode.Position = new Vector2(0f, -19f);
					textNode.TextColor = ColorHelper.GetColor(50u);
					textNode.AlignmentType = (AlignmentType)4;
					textNode.FontType = (FontType)5;
					textNode.AttachNode((NodeBase?)textureButtonNode, NodePosition.AsLastChild);
					list[index].AddNode(textureButtonNode);
				}
				finally
				{
					((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
				}
				void ShowJobActions()
				{
					foreach (KeyValuePair<uint, SupportJobActionListNode> jobActionNode in jobActionNodes)
					{
						jobActionNode.Deconstruct(out var key, out var value2);
						uint num = key;
						SupportJobActionListNode supportJobActionListNode2 = value2;
						supportJobActionListNode2.IsVisible = num == ((MKDSupportJob)(ref data)).RowId;
						if (supportJobActionListNode2 != null && supportJobActionListNode2.IsVisible && supportJobActionListNode2.BackgroundNode == null)
						{
							supportJobActionListNode2.LoadNodes(module, presetJob, isFocused);
						}
					}
					CollisionNode collisionNode = base.WindowNode.CollisionNode;
					Vector2 size = base.WindowNode.CollisionNode.Size;
					size.X = 750f;
					collisionNode.Size = size;
					WindowNodeBase? windowNode = base.WindowNode;
					size = base.WindowNode.Size;
					size.X = 750f;
					windowNode.Size = size;
					ApplyControllerNavigation();
				}
				void ShowJobActionsOnHover()
				{
					if (!PressedButtonOnce)
					{
						ShowJobActions();
					}
				}
			}
			jobContainer.AddNode(list);
			jobContainer.AttachNode(this);
		}

		private void CreateWindowStyle()
		{
			//IL_04bf: Unknown result type (might be due to invalid IL or missing references)
			backgroundNode = new SimpleNineGridNode
			{
				TextureCoordinates = new Vector2(0f),
				TextureSize = new Vector2(500f, 490f),
				TexturePath = "ui/uld/MKDWallPaper_hr1.tex",
				IsVisible = true,
				Size = new Vector2(502f, base.Size.Y - 7f),
				Position = new Vector2(-2f),
				Alpha = 0.9f
			};
			backgroundNode.AddTimeline(CreateFocusAlphaTimeline(178, 229));
			backgroundNode.AttachNode(this);
			headerBackgroundNode = new SimpleNineGridNode
			{
				TextureCoordinates = new Vector2(110f, 0f),
				TextureSize = new Vector2(9f, 41f),
				TexturePath = "ui/uld/MKDWindow_hr1.tex",
				IsVisible = true,
				Size = new Vector2(505f, 40f),
				Position = new Vector2(-2f),
				Alpha = 1f
			};
			headerBackgroundNode.AttachNode(this);
			headerBorderNode = new SimpleNineGridNode
			{
				TextureCoordinates = new Vector2(63f, 55f),
				TextureSize = new Vector2(47f, 6f),
				TexturePath = "ui/uld/MKDWindow_hr1.tex",
				IsVisible = true,
				Size = new Vector2(520f, 6f),
				Position = new Vector2(0f, 36f),
				Alpha = 1f,
				AddColor = new Vector3(-0.196078f)
			};
			headerBorderNode.AttachNode(this);
			moonPatternNode = new SimpleNineGridNode
			{
				TextureCoordinates = new Vector2(0f),
				TextureSize = new Vector2(190f),
				TexturePath = "ui/uld/MKDWallMoon_hr1.tex",
				IsVisible = true,
				Size = new Vector2(190f),
				Position = new Vector2(310f, base.Size.Y - 205f),
				Alpha = 0.9f
			};
			moonPatternNode.AddTimeline(CreateFocusAlphaTimeline(178, 229));
			moonPatternNode.AttachNode(this);
			patternLeftNode = new SimpleNineGridNode
			{
				TextureCoordinates = new Vector2(349f, 140f),
				TextureSize = new Vector2(98f, 132f),
				TexturePath = "ui/uld/MKDWindowPattern_hr1.tex",
				IsVisible = true,
				Size = new Vector2(128f, 132f),
				Position = new Vector2(0f, 40f),
				Alpha = 0.3f
			};
			patternLeftNode.AddTimeline(CreateFocusAlphaTimeline(51, 76));
			patternLeftNode.AttachNode(this);
			patternLeftCornerNode = new SimpleNineGridNode
			{
				TextureCoordinates = new Vector2(0f, 174f),
				TextureSize = new Vector2(140f, 146f),
				TexturePath = "ui/uld/MKDWindowPattern_hr1.tex",
				IsVisible = true,
				Size = new Vector2(128f, 132f),
				Position = new Vector2(0f, base.Size.Y - 190f),
				Alpha = 0.3f
			};
			patternLeftCornerNode.AddTimeline(CreateFocusAlphaTimeline(51, 76));
			patternLeftCornerNode.AttachNode(this);
			patternRightNode = new SimpleNineGridNode
			{
				TextureCoordinates = new Vector2(0f, 45f),
				TextureSize = new Vector2(176f, 125f),
				TexturePath = "ui/uld/MKDWindowPattern_hr1.tex",
				IsVisible = true,
				Size = new Vector2(236f, 125f),
				Position = new Vector2(260f, 5f),
				Alpha = 0.3f
			};
			patternRightNode.AddTimeline(CreateFocusAlphaTimeline(51, 76));
			patternRightNode.AttachNode(this);
			TextNode textNode = new TextNode();
			textNode.LineSpacing = 23u;
			textNode.AlignmentType = (AlignmentType)3;
			textNode.FontSize = 23u;
			textNode.FontType = (FontType)3;
			textNode.NodeFlags = (NodeFlags)8243;
			textNode.TextColor = ColorHelper.GetColor(50u);
			textNode.TextOutlineColor = ColorHelper.GetColor(7u);
			textNode.Size = new Vector2(86f, 31f);
			textNode.Position = new Vector2(12f, 7f);
			textNode.IsVisible = true;
			textNode.String = base.Title;
			textNode.AttachNode(this);
			borderNode = new SimpleNineGridNode
			{
				TextureCoordinates = new Vector2(1f, 0f),
				TextureSize = new Vector2(60f, 70f),
				TexturePath = "ui/uld/MKDWindow_hr1.tex",
				IsVisible = true,
				Size = new Vector2(515f, base.Size.Y + 7f),
				Position = new Vector2(-8f, -5f),
				Alpha = 0.9f,
				Offsets = new Vector4(24f),
				AddColor = new Vector3(10f / 51f)
			};
			borderNode.AddTimeline(CreateFocusAddColorTimeline(-50, 50));
			borderNode.AttachNode(this);
		}

		private static Timeline CreateFocusAlphaTimeline(byte unfocusedAlpha, byte focusedAlpha)
		{
			FrameSetBuilder frameSetBuilder = new TimelineBuilder().BeginFrameSet(1, 9);
			byte? alpha = unfocusedAlpha;
			FrameSetBuilder frameSetBuilder2 = frameSetBuilder.AddFrame(1, null, alpha).EndFrameSet().BeginFrameSet(10, 19);
			byte? alpha2 = unfocusedAlpha;
			FrameSetBuilder frameSetBuilder3 = frameSetBuilder2.AddFrame(10, null, alpha2);
			byte? alpha3 = focusedAlpha;
			return frameSetBuilder3.AddFrame(13, null, alpha3).EndFrameSet().Build();
		}

		private static Timeline CreateFocusAddColorTimeline(int unfocusedColor, int focusedColor)
		{
			FrameSetBuilder frameSetBuilder = new TimelineBuilder().BeginFrameSet(1, 9);
			Vector3? addColor = new Vector3(unfocusedColor);
			FrameSetBuilder frameSetBuilder2 = frameSetBuilder.AddFrame(1, null, null, addColor).EndFrameSet().BeginFrameSet(10, 19);
			Vector3? addColor2 = new Vector3(unfocusedColor);
			FrameSetBuilder frameSetBuilder3 = frameSetBuilder2.AddFrame(10, null, null, addColor2);
			Vector3? addColor3 = new Vector3(focusedColor);
			return frameSetBuilder3.AddFrame(13, null, null, addColor3).EndFrameSet().Build();
		}

		private void CreateWindowControll()
		{
			closeButtonNode = new TextureButtonNode
			{
				Size = new Vector2(28f),
				Position = new Vector2(458f, 8f),
				IsVisible = true,
				TexturePath = "ui/uld/WindowA_Button_hr1.tex",
				TextureCoordinates = new Vector2(0f),
				TextureSize = new Vector2(28f),
				OnClick = base.Close
			};
			closeButtonNode.ImageNode.AddColor = new Vector3(-10f / 51f);
			closeButtonNode.AddTimeline(new TimelineBuilder().BeginFrameSet(1, 20).AddLabel(1, 1, (AtkTimelineJumpBehavior)0, 0).AddLabel(10, 0, (AtkTimelineJumpBehavior)1, 1)
				.AddLabel(11, 2, (AtkTimelineJumpBehavior)0, 0)
				.AddLabel(20, 0, (AtkTimelineJumpBehavior)1, 2)
				.EndFrameSet()
				.Build());
			SimpleImageNode imageNode = closeButtonNode.ImageNode;
			FrameSetBuilder frameSetBuilder = new TimelineBuilder().BeginFrameSet(1, 10);
			Vector3? addColor = new Vector3(0f);
			FrameSetBuilder frameSetBuilder2 = frameSetBuilder.AddFrame(1, null, null, addColor);
			Vector3? addColor2 = new Vector3(-50f);
			FrameSetBuilder frameSetBuilder3 = frameSetBuilder2.AddFrame(4, null, null, addColor2).EndFrameSet().BeginFrameSet(11, 20);
			Vector3? addColor3 = new Vector3(0f);
			FrameSetBuilder frameSetBuilder4 = frameSetBuilder3.AddFrame(11, null, null, addColor3);
			Vector3? addColor4 = new Vector3(50f);
			imageNode.AddTimeline(frameSetBuilder4.AddFrame(14, null, null, addColor4).EndFrameSet().Build());
			closeButtonNode.AttachNode(this);
		}

		private ComponentNode ApplyControllerNavigation()
		{
			List<ComponentNode> list = new List<ComponentNode>();
			foreach (HorizontalFlexNode item in jobContainer.Nodes.OfType<HorizontalFlexNode>())
			{
				List<TextureButtonNode> list2 = item.Nodes.OfType<TextureButtonNode>().ToList();
				int count = list.Count;
				list.AddRange(list2);
				for (int i = 0; i < list2.Count; i++)
				{
					list2[i].NavLeft = count + ((i == 0) ? list2.Count : i);
					list2[i].NavRight = count + ((i == list2.Count - 1) ? 1 : (i + 2));
				}
			}
			list.Add(closeButtonNode);
			foreach (SupportJobActionListNode value in jobActionNodes.Values)
			{
				if (value.BackgroundNode == null)
				{
					continue;
				}
				if (!value.IsVisible)
				{
					value.IsRealActionNode.NavIndex = 0;
					value.SettingButtonNode.NavIndex = 0;
					value.CloseButtonNode.NavIndex = 0;
					foreach (SupportJobActionListNode.SupportActionNode actionDragDropNode in value.ActionDragDropNodes)
					{
						actionDragDropNode.NavIndex = 0;
					}
					continue;
				}
				list.Add(value.IsRealActionNode);
				list.Add(value.SettingButtonNode);
				list.Add(value.CloseButtonNode);
				foreach (SupportJobActionListNode.SupportActionNode actionDragDropNode2 in value.ActionDragDropNodes)
				{
					list.Add(actionDragDropNode2);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				list[j].NavIndex = j + 1;
				list[j].NavUp = ((j == 0) ? list.Count : j);
				list[j].NavDown = ((j == list.Count - 1) ? 1 : (j + 2));
			}
			return list[0];
		}
	}

	private Config config;

	private Hook<AgentShowDelegate>? AgentMKDSupportJobShowHook;

	private TextButtonNode? jobChangeButton;

	private AddonDRMKDJobList? mkdJobListAddon;

	private int dragDropJobIndex = -1;

	private const string COMMAND = "mkdjoblist";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterMKDSupportJobListTitle"),
		Description = LanguageManager.Get("BetterMKDSupportJobListDescription"),
		Category = ModuleCategory.Duty,
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/BetterMKDSupportJobList/preview-1.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		HashSet<uint> hashSet = config.AddonSupportJobOrder.ToHashSet();
		bool flag = false;
		Enumerator<MKDSupportJob> enumerator = LuminaGetter.Get<MKDSupportJob>().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				MKDSupportJob current = enumerator.Current;
				if (!hashSet.Contains(((MKDSupportJob)(ref current)).RowId))
				{
					config.AddonSupportJobOrder.Add(((MKDSupportJob)(ref current)).RowId);
					flag = true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		if (flag)
		{
			config.Save(this);
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "MKDInfo", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "MKDInfo", new AddonEventDelegate(OnAddon));
		if (mkdJobListAddon == null)
		{
			mkdJobListAddon = new AddonDRMKDJobList(this)
			{
				InternalName = "DRMKDJobList",
				Title = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(16658u)),
				Size = new Vector2(500f, 450f),
				RememberClosePosition = true
			};
		}
		if (AgentMKDSupportJobShowHook == null)
		{
			AgentMKDSupportJobShowHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<AgentShowDelegate>((IntPtr)(*((AgentMKDSupportJob)AgentMKDSupportJob.Instance()).VirtualTable).GetVFuncByName<AgentMKDSupportJobVirtualTable>("Show"), (AgentShowDelegate)AgentMKDSupportJobShowDetour, (HookBackend)0);
		}
		AgentMKDSupportJobShowHook.Enable();
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("mkdjoblist", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("BetterMKDSupportJobList-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("mkdjoblist");
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		jobChangeButton?.Dispose();
		jobChangeButton = null;
		mkdJobListAddon?.Dispose();
		mkdJobListAddon = null;
	}

	protected override void ConfigUI()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get("Command")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(55, 1);
			((ImU8String)(ref val2)).AppendLiteral("/pdr ");
			((ImU8String)(ref val2)).AppendFormatted<string>("mkdjoblist");
			((ImU8String)(ref val2)).AppendLiteral(" → Lang.Get(\"BetterMKDSupportJobList-CommandHelp\")");
			ImGui.TextUnformatted(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.NewLine();
		if (!ImGui.CollapsingHeader(ImU8String.op_Implicit(LanguageManager.Get("BetterMKDSupportJobList-ModifySupportJobOrder")), (ImGuiTreeNodeFlags)0))
		{
			return;
		}
		if (ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get("Save"))))
		{
			config.Save(this);
		}
		ImGui.SameLine();
		if (ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get("Reset"))))
		{
			config.AddonSupportJobOrder = config.AddonSupportJobOrder.Order().ToList();
			config.Save(this);
		}
		ImGui.NewLine();
		string text = config.AddonSupportJobOrder.Select(LuminaWrapper.GetMKDSupportJobName).MaxBy((string x) => x.Length);
		for (int num = 0; num < config.AddonSupportJobOrder.Count; num++)
		{
			string mKDSupportJobName = LuminaWrapper.GetMKDSupportJobName(config.AddonSupportJobOrder[num]);
			ImGui.Button(ImU8String.op_Implicit(mKDSupportJobName), new Vector2(ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X * 2f, ImGui.GetTextLineHeightWithSpacing()));
			DragDropSourceDisposable val3 = ImRaii.DragDropSource();
			try
			{
				if (DragDropSourceDisposable.op_Implicit(val3))
				{
					if (ImGui.SetDragDropPayload(ImU8String.op_Implicit("JobReorder"), default(ReadOnlySpan<byte>), (ImGuiCond)0))
					{
						dragDropJobIndex = num;
					}
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(mKDSupportJobName));
				}
			}
			finally
			{
				((DragDropSourceDisposable)(ref val3)).Dispose();
			}
			DragDropTargetDisposable val4 = ImRaii.DragDropTarget();
			try
			{
				if (DragDropTargetDisposable.op_Implicit(val4))
				{
					ImGui.AcceptDragDropPayload(ImU8String.op_Implicit("JobReorder"), (ImGuiDragDropFlags)0);
					if (ImGui.IsMouseReleased((ImGuiMouseButton)0) && dragDropJobIndex >= 0)
					{
						List<uint> addonSupportJobOrder = config.AddonSupportJobOrder;
						int index = dragDropJobIndex;
						List<uint> addonSupportJobOrder2 = config.AddonSupportJobOrder;
						int index2 = num;
						uint value = config.AddonSupportJobOrder[num];
						uint value2 = config.AddonSupportJobOrder[dragDropJobIndex];
						addonSupportJobOrder[index] = value;
						addonSupportJobOrder2[index2] = value2;
						dragDropJobIndex = -1;
					}
				}
			}
			finally
			{
				((DragDropTargetDisposable)(ref val4)).Dispose();
			}
		}
	}

	private unsafe void AgentMKDSupportJobShowDetour(AgentInterface* agent)
	{
		if (((AgentInterface)agent).IsAgentActive())
		{
			((AgentInterface)agent).Hide();
		}
		mkdJobListAddon.Toggle();
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				jobChangeButton = null;
			}
		}
		else if (Addons.MKDInfo != null && jobChangeButton == null)
		{
			jobChangeButton = new TextButtonNode
			{
				Position = new Vector2(18f, 4f),
				Size = new Vector2(200f, 32f),
				IsVisible = true,
				TextTooltip = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(16647u)),
				OnClick = delegate
				{
					mkdJobListAddon.Toggle();
				}
			};
			jobChangeButton.BackgroundNode.IsVisible = false;
			jobChangeButton.AttachNode(((AtkUnitBase)Addons.MKDInfo).GetNodeById(29u));
		}
	}

	private void OnCommand(string command, string arguments)
	{
		mkdJobListAddon?.Toggle();
	}
}
