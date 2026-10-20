using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Dalamud.Services.Game.UI.Abstractions;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.AgentEvent;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.KamiToolKit.Addons.SelectYesno;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class FastWorldTravel : ModuleBase
{
	private class AddonDRFastWorldTravel(TaskHelper taskHelper) : NativeAddon
	{
		private static readonly Version MinDCTravelerXVersion = new Version("0.2.3.0");

		private static NodeBase TeleportWidget;

		private static bool LastOpenPluginState;

		private static bool LastForegroundState;

		private static Dictionary<uint, TextButtonNode> WorldToButtons = new Dictionary<uint, TextButtonNode>();

		public static AddonDRFastWorldTravel? Addon { get; set; }

		private static bool IsPluginEnabled => DalamudServiceExtension.Instance().IsPluginEnabled("DCTravelerX", MinDCTravelerXVersion);

		private static bool IsPluginValid
		{
			get
			{
				if (IsPluginEnabled)
				{
					return IsDCTravelerValid;
				}
				return false;
			}
		}

		public static void Open(FastWorldTravel module)
		{
			EnsureAddon(module);
			Addon.Open();
		}

		public static void Toggle(FastWorldTravel module)
		{
			EnsureAddon(module);
			Addon.Toggle();
		}

		private static void EnsureAddon(FastWorldTravel module)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (Addon == null)
			{
				Addon = new AddonDRFastWorldTravel(module.TaskHelper)
				{
					InternalName = "DRFastWorldTravel",
					Title = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(12510u)),
					Size = new Vector2(GameState.IsCN ? 710f : 180f, 480f)
				};
			}
		}

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			LastOpenPluginState = IsPluginValid;
			WorldToButtons.Clear();
			TeleportWidget = CreateTeleportWidget();
			TeleportWidget.Position = base.ContentStartPosition;
			if (GameState.IsCN)
			{
				SeString val = SeString.Empty;
				if (!IsPluginEnabled)
				{
					val = new SeStringBuilder().Append("超域旅行功能依赖 ").AddUiForeground("DCTravlerX", (ushort)32).Append($" 插件 (版本 {MinDCTravelerXVersion} 及以上)")
						.Build();
				}
				else if (!IsDCTravelerValid)
				{
					val = new SeStringBuilder().Append("无法连接至超域旅行 API, 请确认已安装并启用 ").AddUiForeground("DCTravlerX", (ushort)32).Append($" 插件 (版本 {MinDCTravelerXVersion} 及以上), 若已启用, 请从 XIVLauncherCN 重启游戏")
						.Build();
				}
				if (val != SeString.Empty)
				{
					TextNode textNode = new TextNode();
					textNode.String = ReadOnlySeString.op_Implicit(val.Encode());
					textNode.FontSize = 14u;
					textNode.IsVisible = true;
					textNode.Size = new Vector2(150f, 25f);
					textNode.AlignmentType = (AlignmentType)4;
					textNode.Position = new Vector2(305f, -22f);
					textNode.TextFlags = (TextFlags)10;
					textNode.TextColor = ColorHelper.GetColor(50u);
					textNode.TextOutlineColor = ColorHelper.GetColor(32u);
					textNode.AttachNode(this);
				}
			}
			TeleportWidget.AttachNode(this);
			UpdateWaitTimeInfo();
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			if (IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()))
			{
				Close();
			}
			else
			{
				if (!GameState.IsCN)
				{
					return;
				}
				if (Throttler.Shared.Throttle("FastWorldTravel-OnAddonUpdate") && LastOpenPluginState != IsPluginValid)
				{
					Close();
					taskHelper.Abort();
					taskHelper.DelayNext(100);
					taskHelper.Enqueue(() => !base.IsOpen, "等待界面完全关闭");
					taskHelper.Enqueue((Action)base.Open, "重新打开", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
					LastOpenPluginState = IsPluginValid;
					return;
				}
				if (LastForegroundState != GameState.IsForeground)
				{
					LastForegroundState = GameState.IsForeground;
					Throttler.Shared.Remove("FastWorldTravel-OnAddonUpdate-RequestQueueTime");
					Throttler.Shared.Remove("FastWorldTravel-OnAddonUpdate-UpdateQueueTime");
				}
				if (Throttler.Shared.Throttle("FastWorldTravel-OnAddonUpdate-RequestQueueTime", GameState.IsForeground ? 15000u : 60000u))
				{
					RequestWaitTimeInfoUpdate();
				}
				if (Throttler.Shared.Throttle("FastWorldTravel-OnAddonUpdate-UpdateQueueTime", 1000u))
				{
					UpdateWaitTimeInfo();
				}
			}
		}

		private void RequestWaitTimeInfoUpdate()
		{
			DalamudServiceExtension.Instance<IFramework>().RunOnTick((Func<Task>)async delegate
			{
				if (base.IsOpen && IsPluginValid)
				{
					Dictionary<uint, TextButtonNode> worldToButtons = WorldToButtons;
					if (worldToButtons != null && worldToButtons.Count > 0)
					{
						await RequestDCTravelInfo.InvokeFunc();
					}
				}
			}, default(TimeSpan), 0, default(CancellationToken));
		}

		private void UpdateWaitTimeInfo()
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Expected O, but got Unknown
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			if (!base.IsOpen || !IsPluginValid)
			{
				return;
			}
			Dictionary<uint, TextButtonNode> worldToButtons = WorldToButtons;
			if (worldToButtons == null || worldToButtons.Count <= 0)
			{
				return;
			}
			foreach (KeyValuePair<uint, TextButtonNode> worldToButton in WorldToButtons)
			{
				worldToButton.Deconstruct(out var key, out var value);
				uint arg = key;
				TextButtonNode textButtonNode = value;
				int num = GetDCTravelWaitTime.InvokeFunc(arg);
				if (num != -1)
				{
					SeStringBuilder val = new SeStringBuilder();
					val.AddText("超域传送状态:").Add((Payload)(object)NewLinePayload.Payload).AddText("              ");
					switch (num)
					{
					case 0:
						val.AddUiForeground("即刻完成 / 等待 1 分钟以内", (ushort)45);
						break;
					case -999:
						val.AddUiForeground("繁忙 / 无法通行", (ushort)518);
						break;
					default:
						val.AddText("至少需要等待 ").AddUiForeground(num.ToString(), (ushort)32).AddText(" 分钟");
						break;
					}
					textButtonNode.TextTooltip = ReadOnlySeString.op_Implicit(val.Build().Encode());
					Vector3 vector = ((num >= 5) ? KnownColor.Brown.ToVector4().ToVector3() : (num switch
					{
						0 => KnownColor.DarkGreen.ToVector4().ToVector3(), 
						-999 => KnownColor.DarkRed.ToVector4().ToVector3(), 
						_ => ColorHelper.GetColor(32u).ToVector3(), 
					}));
					Vector3 addColor = vector;
					textButtonNode.AddColor = addColor;
				}
			}
		}

		private HorizontalListNode CreateTeleportWidget()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			HorizontalListNode horizontalListNode = new HorizontalListNode
			{
				IsVisible = true
			};
			List<KeyValuePair<uint, World>> list = Sheets.Worlds.Where(delegate(KeyValuePair<uint, World> x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				World value2 = x.Value;
				return ((World)(ref value2)).DataCenter.RowId == GameState.CurrentDataCenter;
			}).OrderBy(delegate(KeyValuePair<uint, World> x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				World value2 = x.Value;
				return ((object)((World)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
			}).ToList();
			if (list == null || list.Count <= 0)
			{
				return horizontalListNode;
			}
			int maxWorldCount = list.Count;
			World value = list.First().Value;
			SimpleComponentNode node = CreateDataCenterColumn(((World)(ref value)).DataCenter.RowId, list, ref maxWorldCount);
			horizontalListNode.AddNode(node);
			if (!GameState.IsCN)
			{
				return horizontalListNode;
			}
			Dictionary<uint, List<KeyValuePair<uint, World>>> dictionary = Sheets.CNWorlds.Where(delegate(KeyValuePair<uint, World> kvp)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				World value2 = kvp.Value;
				return ((World)(ref value2)).DataCenter.RowId != GameState.CurrentDataCenter;
			}).OrderBy(delegate(KeyValuePair<uint, World> x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				World value2 = x.Value;
				return ((object)((World)(ref value2)).Name/*cast due to constrained. prefix*/).ToString();
			}).GroupBy(delegate(KeyValuePair<uint, World> x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				World value2 = x.Value;
				return ((World)(ref value2)).DataCenter.RowId;
			})
				.ToDictionary((IGrouping<uint, KeyValuePair<uint, World>> x) => x.Key, (IGrouping<uint, KeyValuePair<uint, World>> x) => x.ToList());
			foreach (KeyValuePair<uint, List<KeyValuePair<uint, World>>> item in dictionary)
			{
				maxWorldCount = Math.Max(maxWorldCount, item.Value.Count);
			}
			foreach (KeyValuePair<uint, List<KeyValuePair<uint, World>>> item2 in dictionary)
			{
				horizontalListNode.AddDummy(25f);
				SimpleComponentNode node2 = CreateDataCenterColumn(item2.Key, item2.Value, ref maxWorldCount);
				horizontalListNode.AddNode(node2);
			}
			return horizontalListNode;
		}

		private SimpleComponentNode CreateDataCenterColumn(uint dcID, List<KeyValuePair<uint, World>> worlds, ref int maxWorldCount)
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			string dataCenterName = LuminaWrapper.GetDataCenterName(dcID);
			float num = (float)worlds.Count * 40f + (float)Math.Max(0, worlds.Count - 1) * 5f;
			float num2 = (float)maxWorldCount * 40f + (float)Math.Max(0, maxWorldCount - 1) * 5f + 16f + 16f;
			float y = 42f + num2;
			SimpleComponentNode simpleComponentNode = new SimpleComponentNode
			{
				IsVisible = true,
				Size = new Vector2(150f, y)
			};
			TextNode textNode = new TextNode
			{
				String = ReadOnlySeString.op_Implicit(dataCenterName),
				FontSize = 20u,
				IsVisible = true,
				Position = new Vector2(0f, 8f),
				Size = new Vector2(150f, 30f),
				AlignmentType = (AlignmentType)4,
				TextFlags = (TextFlags)2
			};
			if (dcID != GameState.CurrentDataCenter)
			{
				textNode.ShowClickableCursor = true;
			}
			textNode.AttachNode((NodeBase?)simpleComponentNode, NodePosition.AsLastChild);
			HorizontalLineNode horizontalLineNode = new HorizontalLineNode();
			horizontalLineNode.IsVisible = true;
			horizontalLineNode.Size = new Vector2(118f, 4f);
			horizontalLineNode.Position = new Vector2(16f, textNode.Position.Y + 30f + 4f);
			horizontalLineNode.AttachNode((NodeBase?)simpleComponentNode, NodePosition.AsLastChild);
			SimpleComponentNode simpleComponentNode2 = new SimpleComponentNode
			{
				IsVisible = true,
				Size = new Vector2(150f, num2),
				Position = new Vector2(0f, 42f)
			};
			simpleComponentNode2.AttachNode((NodeBase?)simpleComponentNode, NodePosition.AsLastChild);
			VerticalListNode verticalListNode = new VerticalListNode
			{
				IsVisible = true,
				Size = new Vector2(140f, num),
				Position = new Vector2(5f, (num2 - num) / 2f),
				ItemSpacing = 5f
			};
			verticalListNode.AttachNode((NodeBase?)simpleComponentNode2, NodePosition.AsLastChild);
			foreach (KeyValuePair<uint, World> world in worlds)
			{
				world.Deconstruct(out var key, out var value);
				uint num3 = key;
				World worldData = value;
				SeStringBuilder val = new SeStringBuilder().Append(((object)((World)(ref worldData)).Name/*cast due to constrained. prefix*/).ToString());
				if (GameState.HomeWorld == num3)
				{
					val.Append(" ");
					val.AddIcon((BitmapFontIcon)88);
				}
				TextButtonNode textButtonNode = new TextButtonNode
				{
					Size = new Vector2(140f, 40f),
					IsVisible = true,
					String = ReadOnlySeString.op_Implicit(val.Build().Encode()),
					OnClick = delegate
					{
						//IL_001b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0020: Unknown result type (might be due to invalid IL or missing references)
						Close();
						OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr worldtravel " + ((object)((World)(ref worldData)).Name/*cast due to constrained. prefix*/).ToString());
					},
					IsEnabled = (GameState.CurrentWorld != num3 && (((World)(ref worldData)).DataCenter.RowId == GameState.CurrentDataCenter || IsPluginValid))
				};
				textButtonNode.LabelNode.TextOutlineColor = KnownColor.Black.ToVector4();
				textButtonNode.LabelNode.TextFlags = (TextFlags)8;
				verticalListNode.AddNode(textButtonNode);
				if (GameState.IsCN)
				{
					WorldToButtons.Add(num3, textButtonNode);
				}
			}
			return simpleComponentNode;
		}
	}

	private class Config : ModuleConfig
	{
		public bool AddDtrEntry = true;

		public bool AutoLeaveParty = true;

		public bool ReplaceOrigAddon = true;

		public bool SkipReturnHomeConfirmation = true;
	}

	private struct Travel
	{
		public uint CurrentWorldID;

		public uint HomeWorldID;

		public uint TargetWorldID;

		public ulong ContentID;

		public bool IsBack;

		public string Name;

		public string? Description;
	}

	private Config? config;

	private IDtrBarEntry? entry;

	private SelectYesnoAddon? selectYesnoAddon;

	[IPCSubscriber("DCTravelerX.Travel")]
	private static IPCSubscriber<int, int, ulong, bool, string, Task<Exception?>> SendDCTravel;

	[IPCSubscriber("DCTravelerX.IsValid", DefaultValue = "false")]
	private static IPCSubscriber<bool> IsDCTravelerValid;

	[IPCSubscriber("DCTravelerX.QueryAllWaitTime")]
	private static IPCSubscriber<Task> RequestDCTravelInfo;

	[IPCSubscriber("DCTravelerX.GetWaitTime", DefaultValue = "-1")]
	private static IPCSubscriber<uint, int> GetDCTravelWaitTime;

	[IPCSubscriber("AutoLogin.MarkNextAutoLoginHandled")]
	private IPCSubscriber<object> markNextAutoLoginHandledIPC;

	private const string COMMAND = "worldtravel";

	private const string MODULE_NAME_AUTO_LOGIN = "AutoLogin";

	private static readonly uint[] WorldTravelValidZones = new uint[3] { 132u, 129u, 130u };

	private static readonly ConditionFlag[] InvalidConditions;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastWorldTravelTitle"),
		Description = LanguageManager.Get("FastWorldTravelDescription", "worldtravel") + ((!GameState.IsCN) ? string.Empty : "\n支持快捷超域旅行并实时显示各服务器超域旅行拥挤度 [国服特供]"),
		Category = ModuleCategory.System,
		ModulesPair = new string[2] { "InstantReturn", "InstantTeleport" },
		ModulesPrerequisite = new string[1] { "InstantLogout" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = int.MaxValue,
				ShowDebug = true
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("worldtravel", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("FastWorldTravel-CommandHelp")
		});
		if (config.AddDtrEntry)
		{
			HandleDtrEntry(isAdd: true);
		}
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		OnConditionChanged((ConditionFlag)45, value: false);
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "WorldTravelSelect", new AddonEventDelegate(OnAddon));
		if ((*Addons.WorldTravelSelect).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		HandleDtrEntry(isAdd: false);
		AddonDRFastWorldTravel.Addon?.Dispose();
		AddonDRFastWorldTravel.Addon = null;
		selectYesnoAddon?.Dispose();
		selectYesnoAddon = null;
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("worldtravel");
	}

	protected override void ConfigUI()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LanguageManager.Get("Command")))
		{
			ImU8String val = new ImU8String(8, 2);
			((ImU8String)(ref val)).AppendLiteral("/pdr ");
			((ImU8String)(ref val)).AppendFormatted<string>("worldtravel");
			((ImU8String)(ref val)).AppendLiteral(" → ");
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("FastWorldTravel-CommandHelp"));
			ImGui.TextUnformatted(val);
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastWorldTravel-AutoLeaveParty")), ref config.AutoLeaveParty))
		{
			config.Save(this);
		}
		ImGuiOm.TooltipHover(LanguageManager.Get("FastWorldTravel-AutoLeavePartyHelp"));
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastWorldTravel-AddDtrEntry")), ref config.AddDtrEntry))
		{
			config.Save(this);
			HandleDtrEntry(config.AddDtrEntry);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastWorldTravel-ReplaceOrigAddon")), ref config.ReplaceOrigAddon))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastWorldTravel-SkipTravelBackConfirm")), ref config.SkipReturnHomeConfirmation))
		{
			config.Save(this);
		}
	}

	private unsafe void OnDTRClick(DtrInteractionEvent param)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if ((int)param.ClickType == 0)
		{
			AddonDRFastWorldTravel.Toggle(this);
		}
		else
		{
			if (CheckAndNotifyIfNotValid(GameState.HomeWorld))
			{
				return;
			}
			World homeWorldData;
			if (config.SkipReturnHomeConfirmation)
			{
				ChatManager chatManager = OmenServiceBase<ChatManager>.Instance();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
				defaultInterpolatedStringHandler.AppendLiteral("/pdr worldtravel ");
				homeWorldData = GameState.HomeWorldData;
				defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((World)(ref homeWorldData)).Name);
				chatManager.SendCommand(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			if (selectYesnoAddon != null && !AddonHelper.TryGetPtrByName("DRSelectYesno", out var _))
			{
				try
				{
					selectYesnoAddon?.Dispose();
					selectYesnoAddon = null;
				}
				catch
				{
				}
			}
			if (selectYesnoAddon != null)
			{
				return;
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
				homeWorldData = GameState.HomeWorldData;
				builder.Append(((World)(ref homeWorldData)).Name);
				if (GameState.CurrentDataCenter != GameState.HomeDataCenter)
				{
					SeStringBuilder obj2 = builder.AppendIcon(88u);
					WorldDCGroupType homeDataCenterData = GameState.HomeDataCenterData;
					obj2.Append(((WorldDCGroupType)(ref homeDataCenterData)).Name);
				}
				selectYesnoAddon = SelectYesnoAddon.Open(new SelectYesnoAddonOptions
				{
					Prompt = LanguageManager.GetSe("FastWorldTravel-Notification-TravelBackConfirm", builder),
					Callback = delegate(SelectYesnoAddon _, SelectYesnoAddonResult result)
					{
						//IL_0028: Unknown result type (might be due to invalid IL or missing references)
						//IL_002d: Unknown result type (might be due to invalid IL or missing references)
						//IL_0030: Unknown result type (might be due to invalid IL or missing references)
						selectYesnoAddon = null;
						if (result == SelectYesnoAddonResult.Yes)
						{
							ChatManager chatManager2 = OmenServiceBase<ChatManager>.Instance();
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(17, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("/pdr worldtravel ");
							World homeWorldData2 = GameState.HomeWorldData;
							defaultInterpolatedStringHandler2.AppendFormatted<ReadOnlySeString>(((World)(ref homeWorldData2)).Name);
							chatManager2.SendCommand(defaultInterpolatedStringHandler2.ToStringAndClear());
						}
					}
				});
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	private unsafe void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (entry == null)
		{
			return;
		}
		TaskHelper? taskHelper = base.TaskHelper;
		if (taskHelper == null || taskHelper.IsBusy || !InvalidConditions.Contains(flag, null))
		{
			return;
		}
		if (value)
		{
			entry.Shown = false;
			return;
		}
		entry.Shown = !DalamudServiceExtension.Instance<ICondition>().Any(InvalidConditions);
		if (!entry.Shown)
		{
			return;
		}
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
			IDtrBarEntry? obj = entry;
			SeStringBuilder val2 = builder.AppendIcon(88u);
			SeStringInterpolatedStringHandler val3 = new SeStringInterpolatedStringHandler(0, 1, val2);
			World currentWorldData = GameState.CurrentWorldData;
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(((object)((World)(ref currentWorldData)).Name/*cast due to constrained. prefix*/).ToString());
			obj.Text = SeStringExtensions.ToDalamudString(val2.Append(val3).ToReadOnlySeString());
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		if (Addons.WorldTravelSelect != null && config.ReplaceOrigAddon)
		{
			((AtkUnitBase)Addons.WorldTravelSelect).Close(true);
			AddonDRFastWorldTravel.Open(this);
		}
	}

	private void OnCommand(string command, string args)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if (!Throttler.Shared.Throttle("FastWorldTravel.OnCommand", 1000u))
		{
			return;
		}
		if (args.Length == 0)
		{
			AddonDRFastWorldTravel.Open(this);
			return;
		}
		args = args.Trim().ToLowerInvariant();
		uint num = 0u;
		if (uint.TryParse(args, out var result))
		{
			if (LuminaGetter.TryGetRow<World>(result, out World _) && Sheets.Worlds.ContainsKey(result))
			{
				num = result;
			}
		}
		else
		{
			num = Sheets.Worlds.FirstOrDefault(delegate(KeyValuePair<uint, World> x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				World value = x.Value;
				return ((object)((World)(ref value)).Name/*cast due to constrained. prefix*/).ToString().Contains(args, StringComparison.OrdinalIgnoreCase);
			}).Key;
		}
		if (!CheckAndNotifyIfNotValid(num))
		{
			string worldName = LuminaWrapper.GetWorldName(num);
			string text = LanguageManager.Get("FastWorldTravel-Notification-TravelingTo", $"{char.ToUpper(worldName[0])}{worldName.Substring(1)}");
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text), useDefaultPrefix: false);
			NotifyHelper.Toast(text);
			if (LuminaWrapper.GetWorldDC(num) != GameState.CurrentDataCenter)
			{
				EnqueueDCTravel(num);
			}
			else
			{
				EnqueueWorldTravel(num);
			}
		}
	}

	private unsafe void EnqueueWorldTravel(uint worldID)
	{
		if (!LuminaGetter.TryGetRow<World>(worldID, out World targetWorld))
		{
			return;
		}
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(delegate
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (entry != null)
			{
				entry.Text = SeString.op_Implicit("\ue06f " + ((object)((World)(ref targetWorld)).Name/*cast due to constrained. prefix*/).ToString());
			}
		}, "更新 DTR 目标服务器信息");
		if (config.AutoLeaveParty)
		{
			base.TaskHelper.Enqueue((Func<bool>)LeaveNonCrossWorldParty, "离开非跨服小队", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
		if (!WorldTravelValidZones.Contains(GameState.TerritoryType))
		{
			IAetheryteEntry nearestAetheryte = (from x in IOmenDalamudService<IAetheryteList>.Instance()
				where WorldTravelValidZones.Contains(x.TerritoryID)
				select x).MinBy((IAetheryteEntry x) => x.GilCost);
			if (nearestAetheryte == null)
			{
				return;
			}
			base.TaskHelper.Enqueue(() => OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(nearestAetheryte.TerritoryID), "传送回可跨服区域");
			base.TaskHelper.Enqueue(() => GameState.TerritoryType == nearestAetheryte.TerritoryID && UIModuleExtension.IsScreenReady(), "等待跨服完成");
		}
		base.TaskHelper.Enqueue(delegate
		{
			(*AgentWorldTravel.Instance()).TravelTo(worldID);
		}, "发起大区内跨服请求");
	}

	private void EnqueueDCTravel(uint targetWorldID)
	{
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (GameState.CurrentWorld == 0 || GameState.HomeWorld == 0 || targetWorldID == 0 || !LuminaGetter.TryGetRow<World>(targetWorldID, out World item))
		{
			return;
		}
		WorldDCGroupType value;
		Travel travel2;
		Travel travel;
		if (GameState.HomeDataCenter == GameState.CurrentDataCenter)
		{
			if (GameState.CurrentWorld != GameState.HomeWorld)
			{
				EnqueueWorldTravel(GameState.HomeWorld);
			}
			base.TaskHelper.Enqueue(() => GameState.HomeWorld == GameState.CurrentWorld && UIModuleExtension.IsScreenReady(), "等待返回原始服务器的跨服完成");
			travel = new Travel
			{
				CurrentWorldID = GameState.HomeWorld,
				TargetWorldID = targetWorldID,
				ContentID = LocalPlayerState.ContentID,
				IsBack = false,
				Name = LocalPlayerState.Name
			};
			value = ((World)(ref item)).DataCenter.Value;
			travel.Description = ((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			travel2 = travel;
			EnqueueLogout();
			base.TaskHelper.EnqueueAsync(() => EnqueueDCTravelRequest(new Travel[1] { travel2 }), "发送跨服请求");
			return;
		}
		if (targetWorldID == GameState.HomeWorld)
		{
			travel = new Travel
			{
				CurrentWorldID = GameState.CurrentWorld,
				TargetWorldID = targetWorldID,
				ContentID = LocalPlayerState.ContentID,
				IsBack = true,
				Name = LocalPlayerState.Name
			};
			value = ((World)(ref item)).DataCenter.Value;
			travel.Description = ((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			travel2 = travel;
			EnqueueLogout();
			base.TaskHelper.EnqueueAsync(() => EnqueueDCTravelRequest(new Travel[1] { travel2 }), "发送跨服请求");
			return;
		}
		if (((World)(ref item)).DataCenter.RowId == GameState.HomeDataCenter)
		{
			travel = new Travel
			{
				CurrentWorldID = GameState.CurrentWorld,
				TargetWorldID = targetWorldID,
				ContentID = LocalPlayerState.ContentID,
				IsBack = true,
				Name = LocalPlayerState.Name
			};
			value = ((World)(ref item)).DataCenter.Value;
			travel.Description = ((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			travel.HomeWorldID = GameState.HomeWorld;
			travel2 = travel;
			EnqueueLogout();
			base.TaskHelper.EnqueueAsync(() => EnqueueDCTravelRequest(new Travel[1] { travel2 }), "发送跨服请求");
			base.TaskHelper.Enqueue(delegate
			{
				if (GameState.CurrentWorld != GameState.HomeWorld || !GameState.IsLoggedIn)
				{
					return false;
				}
				EnqueueWorldTravel(targetWorldID);
				return true;
			}, "回到原始服务器, 跨服到其他服务器", 0, null, null, null, null, -1);
			return;
		}
		travel = new Travel
		{
			CurrentWorldID = GameState.CurrentWorld,
			TargetWorldID = GameState.HomeWorld,
			ContentID = LocalPlayerState.ContentID,
			IsBack = true,
			Name = LocalPlayerState.Name
		};
		value = ((World)(ref item)).DataCenter.Value;
		travel.Description = ((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
		Travel travel3 = travel;
		travel = new Travel
		{
			CurrentWorldID = GameState.HomeWorld,
			TargetWorldID = targetWorldID,
			ContentID = LocalPlayerState.ContentID,
			IsBack = false,
			Name = LocalPlayerState.Name
		};
		value = ((World)(ref item)).DataCenter.Value;
		travel.Description = ((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
		Travel travel4 = travel;
		EnqueueLogout();
		base.TaskHelper.EnqueueAsync(() => EnqueueDCTravelRequest(new Travel[2] { travel3, travel4 }), "发送跨服请求");
	}

	private unsafe void EnqueueLogout()
	{
		base.TaskHelper.Enqueue(delegate
		{
			if (ManagerBase<ModuleManager>.Instance().IsModuleEnabled("AutoLogin") == true)
			{
				markNextAutoLoginHandledIPC.InvokeAction();
			}
		}, "禁用自动登录");
		base.TaskHelper.Enqueue(delegate
		{
			if (!Throttler.Shared.Throttle("FastWorldTravel.Logout"))
			{
				return false;
			}
			if (!GameState.IsLoggedIn)
			{
				return true;
			}
			OmenServiceBase<ChatManager>.Instance().SendCommand("/logout");
			return false;
		}, "登出游戏");
		base.TaskHelper.Enqueue(() => (*Addons.TitleMenu).IsAddonAndNodesReady(), "等待标题界面");
		base.TaskHelper.DelayNext(2000, "等待 2 秒");
	}

	private unsafe async Task EnqueueDCTravelRequest(Travel[] data)
	{
		try
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationInfo("DCTravelerX 正在处理超域旅行请求, 请稍等");
			for (int i = 0; i < data.Length; i++)
			{
				Travel travelData = data[i];
				base.TaskHelper.EnqueueAsync((Func<Task>)async delegate
				{
					Exception ex2 = await SendDCTravel.InvokeFunc((int)travelData.CurrentWorldID, (int)travelData.TargetWorldID, travelData.ContentID, travelData.IsBack, travelData.Name);
					if (ex2 != null)
					{
						OmenServiceBase<NotifyHelper>.Instance().NotificationWarning("超域旅行失败: 请查看日志获取详细信息");
						DLog.Error("超域旅行失败", ex2);
						base.TaskHelper.Abort();
					}
				}, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				if (i == data.Length - 1)
				{
					base.TaskHelper.Enqueue((Action)AgentLobbyEvent.OpenCharacterSelect, "进入角色选择界面", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
					base.TaskHelper.Enqueue(() => Addons.CharaSelect != null || Addons.CharaSelectListMenu != null, "等待角色选择界面可用");
					base.TaskHelper.DelayNext(500);
					base.TaskHelper.Enqueue(() => AgentLobbyEvent.SelectWorldByID(travelData.TargetWorldID), "选择目标服务器");
					base.TaskHelper.DelayNext(500);
					base.TaskHelper.Enqueue(() => AgentLobbyEvent.SelectCharacter((CharaSelectCharacterEntry x) => x.ContentId == travelData.ContentID), "选择目标角色");
					base.TaskHelper.Enqueue(() => GameState.IsLoggedIn, "等待登录");
					break;
				}
				await Task.Delay(100);
			}
		}
		catch (Exception ex)
		{
			DLog.Debug("超域旅行失败: " + ex.Message, ex);
		}
	}

	private static bool CheckAndNotifyIfNotValid(uint targetWorld)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		if (!GameState.IsLoggedIn || LocalPlayerState.Object == null || DalamudServiceExtension.Instance<ICondition>().Any(InvalidConditions))
		{
			NotifyHelper.ToastError(DalamudServiceExtension.Instance<ISeStringEvaluator>().EvaluateFromLogMessage(9066u, default(Span<SeStringParameter>), (ClientLanguage?)null));
			return true;
		}
		if (DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)93,
			(ConditionFlag)92
		}))
		{
			NotifyHelper.ToastError(DalamudServiceExtension.Instance<ISeStringEvaluator>().EvaluateFromLogMessage(7792u, default(Span<SeStringParameter>), (ClientLanguage?)null));
			return true;
		}
		if (GameState.CurrentWorld == targetWorld)
		{
			NotifyHelper.ToastError(DalamudServiceExtension.Instance<ISeStringEvaluator>().EvaluateFromLogMessage(9407u, default(Span<SeStringParameter>), (ClientLanguage?)null));
			return true;
		}
		World item = default(World);
		if (!Sheets.Worlds.ContainsKey(targetWorld) || !LuminaGetter.TryGetRow<World>(targetWorld, out item))
		{
			string text = "???";
			if (((World)(ref item)).RowId != 0)
			{
				string text2 = ((object)((World)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
				if (!string.IsNullOrEmpty(text2))
				{
					text = text2;
				}
			}
			ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
			SeStringParameter reference = SeStringParameter.op_Implicit(text);
			NotifyHelper.ToastError(obj.EvaluateFromLogMessage(9408u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null));
			return true;
		}
		if (!GameState.IsCN && LuminaWrapper.GetWorldDC(targetWorld) != GameState.CurrentDataCenter)
		{
			NotifyHelper.ToastError(LanguageManager.Get("FastWorldTravel-Notification-CrossDCTravel"));
			return true;
		}
		return false;
	}

	private static bool LeaveNonCrossWorldParty()
	{
		if (DalamudServiceExtension.Instance<IPartyList>().Length < 2 || DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)84])
		{
			return true;
		}
		if (!Throttler.Shared.Throttle("FastWorldTravel-LeaveNonCrossWorldParty"))
		{
			return false;
		}
		OmenServiceBase<ChatManager>.Instance().SendMessage("/leave");
		return DalamudServiceExtension.Instance<IPartyList>().Length < 2;
	}

	private static (bool, uint) CheckCNDataCenterStatus(uint dcID)
	{
		foreach (uint item in (from x in Sheets.Worlds.Where(delegate(KeyValuePair<uint, World> x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				World value = x.Value;
				return ((World)(ref value)).DataCenter.RowId == dcID;
			})
			select x.Key).ToList())
		{
			if (GetDCTravelWaitTime.InvokeFunc(item) == 0)
			{
				return (true, item);
			}
		}
		return (false, 0u);
	}

	private void HandleDtrEntry(bool isAdd)
	{
		if (isAdd)
		{
			if (entry == null)
			{
				entry = DalamudServiceExtension.Instance<IDtrBar>().Get("DailyRoutines-FastWorldTravel", (SeString)null);
			}
			entry.OnClick = OnDTRClick;
			entry.Tooltip = SeString.op_Implicit(LanguageManager.Get("FastWorldTravel-DtrEntryTooltip"));
			entry.Text = SeString.op_Implicit(LuminaWrapper.GetAddonText(12510u));
			entry.Shown = true;
			DalamudServiceExtension.Instance<IGameConfig>().UiConfig.Set("InfoSettingDispWorldNameType", 0u);
		}
		else if (entry != null)
		{
			entry.Remove();
			entry = null;
		}
	}

	static FastWorldTravel()
	{
		ConditionFlag[] array = new ConditionFlag[6];
		RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
		InvalidConditions = (ConditionFlag[])(object)array;
	}
}
