using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Agent;
using Dalamud.Game.Agent.AgentArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Timelines;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Addons;
using OmenTools.OmenService;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedFreeShop : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool IsEnabled = true;
	}

	internal class OptimizedFreeShopAddon(OptimizedFreeShop module) : AttachedAddon("FreeShop")
	{
		private readonly Dictionary<uint, IconButtonNode> jobButtons = new Dictionary<uint, IconButtonNode>();

		private uint selectedClassJob;

		protected override AttachedAddonPosition AttachPosition => AttachedAddonPosition.LeftTop;

		protected unsafe override void OnDraw(AtkUnitBase* addon)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Invalid comparison between Unknown and I4
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Invalid comparison between Unknown and I4
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			if (!(*base.HostAddon).IsAddonAndNodesReady())
			{
				return;
			}
			AtkComponentBase* componentByNodeId = ((AtkUnitBase)base.HostAddon).GetComponentByNodeId(3u);
			if (componentByNodeId == null)
			{
				return;
			}
			AtkComponentCheckBox* asAtkComponentCheckBox = ((AtkResNode)((AtkUldManager)(&((AtkComponentBase)componentByNodeId).UldManager)).SearchNodeById(2u)).GetAsAtkComponentCheckBox();
			if (asAtkComponentCheckBox == null)
			{
				return;
			}
			AtkTextNode* buttonTextNode = ((AtkComponentCheckBox)asAtkComponentCheckBox).ButtonTextNode;
			if (buttonTextNode != null)
			{
				ReadOnlySePayload val = ((IEnumerable<ReadOnlySePayload>)(object)new ReadOnlySeString(Utf8String.op_Implicit(ref ((AtkTextNode)buttonTextNode).NodeText))).First();
				ReadOnlySeExpression val2 = default(ReadOnlySeExpression);
				uint num = default(uint);
				if ((int)val.Type != 2 || (int)val.MacroCode != 18 || ((ReadOnlySePayload)(ref val)).ExpressionCount != 1 || !((ReadOnlySePayload)(ref val)).TryGetExpression(ref val2) || !((ReadOnlySeExpression)(ref val2)).TryGetUInt(ref num))
				{
					UpdateSelectedJob(0u);
					return;
				}
				ClassJob val3 = ((BitmapFontIcon)num).ToClassJob();
				UpdateSelectedJob(((ClassJob)(ref val3)).RowId);
			}
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_0277: Unknown result type (might be due to invalid IL or missing references)
			jobButtons.Clear();
			selectedClassJob = 0u;
			if (base.WindowNode is WindowNode windowNode)
			{
				windowNode.CloseButtonNode.IsVisible = false;
			}
			VerticalListNode obj = new VerticalListNode
			{
				IsVisible = true,
				ItemSpacing = 4f,
				Position = base.ContentStartPosition
			};
			Vector2 contentSize = base.ContentSize;
			contentSize.Y = 28f;
			obj.Size = contentSize;
			VerticalListNode verticalListNode = obj;
			CheckboxNode checkboxNode = new CheckboxNode();
			contentSize = base.ContentSize;
			contentSize.Y = 28f;
			checkboxNode.Size = contentSize;
			checkboxNode.IsVisible = true;
			checkboxNode.IsChecked = module.IsFastClaimEnabled;
			checkboxNode.IsEnabled = true;
			checkboxNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("OptimizedFreeShop-FastClaim"));
			checkboxNode.OnClick = delegate(bool isChecked)
			{
				module.IsFastClaimEnabled = isChecked;
			};
			CheckboxNode node = checkboxNode;
			verticalListNode.AddNode(node);
			HorizontalListNode obj2 = new HorizontalListNode
			{
				IsVisible = true
			};
			contentSize = base.ContentSize;
			contentSize.Y = 36f;
			obj2.Size = contentSize;
			obj2.Position = VectorExtensions.WithY(base.ContentStartPosition, 0f);
			HorizontalListNode horizontalListNode = obj2;
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<uint, List<(int, uint)>> claimItem in GetClaimItems())
			{
				claimItem.Deconstruct(out var key, out var value);
				uint rowID = key;
				List<(int Index, uint ID)> items = value;
				if (!LuminaGetter.TryGetRow<ClassJobCategory>(rowID, out ClassJobCategory categoryData))
				{
					continue;
				}
				ClassJob val = ((IEnumerable<ClassJob>)LuminaGetter.Get<ClassJob>()).FirstOrDefault((ClassJob x) => categoryData.IsClassJobIn(((ClassJob)(ref x)).RowId));
				if (((ClassJob)(ref val)).RowId != 0)
				{
					if (num >= 5)
					{
						verticalListNode.AddNode(horizontalListNode);
						HorizontalListNode obj3 = new HorizontalListNode
						{
							IsVisible = true
						};
						contentSize = base.ContentSize;
						contentSize.Y = 36f;
						obj3.Size = contentSize;
						obj3.Position = VectorExtensions.WithY(base.ContentStartPosition, 0f);
						horizontalListNode = obj3;
						num = 0;
						num2++;
					}
					IconButtonNode iconButtonNode = new IconButtonNode
					{
						Size = new Vector2(36f),
						IsVisible = true,
						IsEnabled = true,
						IconId = ((ClassJob)(ref val)).RowId + 62100,
						OnClick = delegate
						{
							module.BatchClaim(items);
						},
						TextTooltip = ReadOnlySeString.op_Implicit($"{LanguageManager.Get("OptimizedFreeShop-BatchClaim")}: {((ClassJob)(ref val)).Name}")
					};
					AddHighlightTimeline(iconButtonNode);
					PlayHighlight(iconButtonNode, isSelected: false);
					jobButtons[((ClassJob)(ref val)).RowId] = iconButtonNode;
					horizontalListNode.AddNode(iconButtonNode);
					horizontalListNode.AddDummy(4f);
					num++;
				}
			}
			if (num > 0)
			{
				verticalListNode.AddNode(horizontalListNode);
				num2++;
			}
			SetWindowSize(base.Size.X, 128f + (float)Math.Max(num2 - 1, 0) * 40f);
			verticalListNode.AttachNode(this);
		}

		private unsafe static Dictionary<uint, List<(int Index, uint ID)>> GetClaimItems()
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			uint uInt = ((AtkValue)((byte*)((AtkUnitBase)Addons.FreeShop).AtkValues + (nint)76 * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
			Dictionary<uint, List<(int, uint)>> dictionary = new Dictionary<uint, List<(int, uint)>>();
			for (int i = 0; i < uInt; i++)
			{
				uint uInt2 = ((AtkValue)((byte*)((AtkUnitBase)Addons.FreeShop).AtkValues + (nint)(138 + i) * (nint)Unsafe.SizeOf<AtkValue>())).UInt;
				if (LuminaGetter.TryGetRow<Item>(uInt2, out Item item))
				{
					dictionary.TryAdd(((Item)(ref item)).ClassJobCategory.RowId, new List<(int, uint)>());
					dictionary[((Item)(ref item)).ClassJobCategory.RowId].Add((i, uInt2));
				}
			}
			return dictionary;
		}

		private void UpdateSelectedJob(uint classJobID)
		{
			if (classJobID == 0)
			{
				if (selectedClassJob != 0)
				{
					foreach (KeyValuePair<uint, IconButtonNode> jobButton in jobButtons)
					{
						jobButton.Deconstruct(out var _, out var value);
						PlayHighlight(value, isSelected: true);
					}
				}
				selectedClassJob = 0u;
			}
			else if (selectedClassJob != classJobID)
			{
				if (jobButtons.TryGetValue(selectedClassJob, out IconButtonNode value2))
				{
					PlayHighlight(value2, isSelected: false);
				}
				selectedClassJob = classJobID;
				if (jobButtons.TryGetValue(selectedClassJob, out IconButtonNode value3))
				{
					PlayHighlight(value3, isSelected: true);
				}
			}
		}

		private static void AddHighlightTimeline(IconButtonNode button)
		{
			button.AddTimeline(new TimelineBuilder().BeginFrameSet(1, 220).AddLabelPair(1, 10, 1).AddLabelPair(11, 17, 2)
				.AddLabelPair(18, 26, 3)
				.AddLabelPair(27, 36, 7)
				.AddLabelPair(37, 46, 6)
				.AddLabelPair(47, 53, 4)
				.AddLabelPair(201, 210, 101)
				.AddLabelPair(211, 220, 102)
				.EndFrameSet()
				.Build());
			NineGridNode backgroundNode = button.BackgroundNode;
			TimelineBuilder timelineBuilder = new TimelineBuilder();
			Vector2? position = Vector2.Zero;
			byte? alpha = (byte)byte.MaxValue;
			Vector3? multiplyColor = new Vector3(100f);
			FrameSetBuilder frameSetBuilder = timelineBuilder.AddFrameSetWithFrame(1, 10, 1, position, alpha, null, multiplyColor).BeginFrameSet(11, 17);
			Vector2? position2 = Vector2.Zero;
			byte? alpha2 = (byte)byte.MaxValue;
			Vector3? multiplyColor2 = new Vector3(100f);
			TimelineBuilder timelineBuilder2 = frameSetBuilder.AddFrame(11, position2, alpha2, null, multiplyColor2).AddFrame(13, Vector2.Zero, (byte)byte.MaxValue, multiplyColor: new Vector3(100f), addColor: new Vector3(16f)).EndFrameSet()
				.AddFrameSetWithFrame(18, 26, 18, new Vector2(0f, 1f), (byte)byte.MaxValue, new Vector3(16f));
			Vector2? position3 = Vector2.Zero;
			byte? alpha3 = (byte)178;
			Vector3? multiplyColor3 = new Vector3(50f);
			FrameSetBuilder obj = timelineBuilder2.AddFrameSetWithFrame(27, 36, 27, position3, alpha3, null, multiplyColor3).AddFrameSetWithFrame(37, 46, 37, Vector2.Zero, (byte)byte.MaxValue, multiplyColor: new Vector3(100f), addColor: new Vector3(16f)).BeginFrameSet(47, 53)
				.AddFrame(47, Vector2.Zero, (byte)byte.MaxValue, multiplyColor: new Vector3(100f), addColor: new Vector3(16f));
			Vector2? position4 = Vector2.Zero;
			byte? alpha4 = (byte)byte.MaxValue;
			Vector3? multiplyColor4 = new Vector3(100f);
			FrameSetBuilder frameSetBuilder2 = obj.AddFrame(53, position4, alpha4, null, multiplyColor4).EndFrameSet().BeginFrameSet(201, 210);
			Vector3? addColor = Vector3.Zero;
			Vector3? multiplyColor5 = new Vector3(100f);
			FrameSetBuilder frameSetBuilder3 = frameSetBuilder2.AddFrame(201, null, null, addColor, multiplyColor5);
			Vector3? addColor2 = new Vector3(32f);
			Vector3? multiplyColor6 = new Vector3(115f);
			FrameSetBuilder frameSetBuilder4 = frameSetBuilder3.AddFrame(210, null, null, addColor2, multiplyColor6).EndFrameSet().BeginFrameSet(211, 220);
			Vector3? addColor3 = new Vector3(32f);
			Vector3? multiplyColor7 = new Vector3(115f);
			FrameSetBuilder frameSetBuilder5 = frameSetBuilder4.AddFrame(211, null, null, addColor3, multiplyColor7);
			Vector3? addColor4 = Vector3.Zero;
			Vector3? multiplyColor8 = new Vector3(100f);
			backgroundNode.AddTimeline(frameSetBuilder5.AddFrame(220, null, null, addColor4, multiplyColor8).EndFrameSet().Build());
			IconImageNode imageNode = button.ImageNode;
			TimelineBuilder timelineBuilder3 = new TimelineBuilder();
			Vector2? position5 = new Vector2(8f);
			byte? alpha5 = (byte)byte.MaxValue;
			multiplyColor2 = new Vector3(100f);
			TimelineBuilder timelineBuilder4 = timelineBuilder3.AddFrameSetWithFrame(1, 10, 1, position5, alpha5, null, multiplyColor2);
			Vector2? position6 = new Vector2(8f);
			byte? alpha6 = (byte)byte.MaxValue;
			Vector3? multiplyColor9 = new Vector3(100f);
			TimelineBuilder timelineBuilder5 = timelineBuilder4.AddFrameSetWithFrame(11, 17, 11, position6, alpha6, null, multiplyColor9);
			Vector2? position7 = new Vector2(8f, 9f);
			byte? alpha7 = (byte)byte.MaxValue;
			multiplyColor3 = new Vector3(100f);
			TimelineBuilder timelineBuilder6 = timelineBuilder5.AddFrameSetWithFrame(18, 26, 18, position7, alpha7, null, multiplyColor3);
			Vector2? position8 = new Vector2(8f);
			byte? alpha8 = (byte)153;
			Vector3? multiplyColor10 = new Vector3(80f);
			TimelineBuilder timelineBuilder7 = timelineBuilder6.AddFrameSetWithFrame(27, 36, 27, position8, alpha8, null, multiplyColor10);
			Vector2? position9 = new Vector2(8f);
			byte? alpha9 = (byte)byte.MaxValue;
			Vector3? multiplyColor11 = new Vector3(100f);
			TimelineBuilder timelineBuilder8 = timelineBuilder7.AddFrameSetWithFrame(37, 46, 37, position9, alpha9, null, multiplyColor11);
			Vector2? position10 = new Vector2(8f);
			byte? alpha10 = (byte)byte.MaxValue;
			multiplyColor4 = new Vector3(100f);
			FrameSetBuilder frameSetBuilder6 = timelineBuilder8.AddFrameSetWithFrame(47, 53, 47, position10, alpha10, null, multiplyColor4).BeginFrameSet(201, 210);
			Vector2? position11 = new Vector2(8f);
			multiplyColor5 = Vector3.Zero;
			addColor = new Vector3(100f);
			FrameSetBuilder frameSetBuilder7 = frameSetBuilder6.AddFrame(201, position11, null, multiplyColor5, addColor);
			Vector2? position12 = new Vector2(8f);
			multiplyColor6 = new Vector3(20f);
			addColor2 = new Vector3(110f);
			FrameSetBuilder frameSetBuilder8 = frameSetBuilder7.AddFrame(210, position12, null, multiplyColor6, addColor2).EndFrameSet().BeginFrameSet(211, 220);
			Vector2? position13 = new Vector2(8f);
			multiplyColor7 = new Vector3(20f);
			addColor3 = new Vector3(110f);
			FrameSetBuilder frameSetBuilder9 = frameSetBuilder8.AddFrame(211, position13, null, multiplyColor7, addColor3);
			Vector2? position14 = new Vector2(8f);
			multiplyColor8 = Vector3.Zero;
			addColor4 = new Vector3(100f);
			imageNode.AddTimeline(frameSetBuilder9.AddFrame(220, position14, null, multiplyColor8, addColor4).EndFrameSet().Build());
		}

		private static void PlayHighlight(IconButtonNode button, bool isSelected)
		{
			int labelId = (isSelected ? 101 : 102);
			button.Timeline?.PlayAnimation(labelId);
		}
	}

	private Config config;

	private OptimizedFreeShopAddon? addon;

	private TaskHelper? clickYesnoHelper;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedFreeShopTitle"),
		Description = LanguageManager.Get("OptimizedFreeShopDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[1] { "AutoClaimItemIgnoringMismatchJobAndLevel" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	internal bool IsFastClaimEnabled
	{
		get
		{
			return config.IsEnabled;
		}
		set
		{
			config.IsEnabled = value;
			config.Save(this);
		}
	}

	protected override void Init()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		if (clickYesnoHelper == null)
		{
			clickYesnoHelper = new TaskHelper();
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (addon == null)
		{
			addon = new OptimizedFreeShopAddon(this)
			{
				InternalName = "DROptimizedFreeShop",
				Title = ReadOnlySeString.op_Implicit(Info.Title),
				Size = new Vector2(220f, 128f),
				RememberClosePosition = false
			};
		}
		DalamudServiceExtension.Instance<IAgentLifecycle>().RegisterListener((AgentEvent)1, (AgentId)324, new AgentEventDelegate(OnAgent));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "FreeShop", new AddonEventDelegate(OnAddon));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAgentLifecycle>().UnregisterListener((AgentEventDelegate[])(object)new AgentEventDelegate[1] { OnAgent });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		clickYesnoHelper?.Abort();
		addon?.Dispose();
		addon = null;
		clickYesnoHelper = null;
	}

	private unsafe void OnAgent(AgentEvent type, AgentArgs args)
	{
		if (!config.IsEnabled)
		{
			return;
		}
		AgentArgs obj = ((args is AgentReceiveEventArgs) ? args : null);
		AtkValue* atkValues = (AtkValue*)((AgentReceiveEventArgs)obj).AtkValues;
		if (((AgentReceiveEventArgs)obj).EventKind == 0L && ((AtkValue)atkValues).Int == 0)
		{
			clickYesnoHelper.Abort();
			clickYesnoHelper.Enqueue(() => AddonSelectYesnoEvent.ClickYes());
		}
	}

	private void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		if ((int)type == 6)
		{
			clickYesnoHelper?.Abort();
		}
	}

	internal unsafe void BatchClaim(List<(int Index, uint ID)> itemData)
	{
		base.TaskHelper.Abort();
		bool flag = false;
		foreach (var itemDatum in itemData)
		{
			var (index, _) = itemDatum;
			if (LocalPlayerState.GetItemCount(itemDatum.ID) == 0)
			{
				flag = true;
				base.TaskHelper.Enqueue(delegate
				{
					((AgentId)324).SendEvent(0uL, 0, index);
				});
				base.TaskHelper.DelayNext(10);
			}
		}
		if (flag)
		{
			base.TaskHelper.Enqueue(delegate
			{
				BatchClaim(itemData);
			});
		}
	}
}
