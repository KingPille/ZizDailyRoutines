using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoSellCards : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddonDialog;
	}

	private HorizontalListNode? layoutNode;

	private TextNode? titleNode;

	private TextButtonNode? startButton;

	private TextButtonNode? stopButton;

	private const string COMMAND = "scards";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoSellCardsTitle"),
		Description = LanguageManager.Get("AutoSellCardsDescription"),
		Category = ModuleCategory.GoldSaucer,
		ModulesPrerequisite = new string[2] { "InstantLeaveDuty", "ContentFinderCommand" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000,
				ShowDebug = true
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		IAddonLifecycle obj2 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj3 = _003C_003EO._003C0_003E__OnAddonDialog;
		if (obj3 == null)
		{
			AddonEventDelegate val = OnAddonDialog;
			_003C_003EO._003C0_003E__OnAddonDialog = val;
			obj3 = (object)val;
		}
		obj2.RegisterListener((AddonEvent)1, "ShopCardDialog", (AddonEventDelegate)obj3);
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "TripleTriadCoinExchange", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "TripleTriadCoinExchange", new AddonEventDelegate(OnAddon));
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("scards", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("AutoSellCards-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("scards");
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		layoutNode?.Dispose();
		layoutNode = null;
		startButton?.Dispose();
		startButton = null;
		stopButton?.Dispose();
		stopButton = null;
		titleNode?.Dispose();
		titleNode = null;
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnAddonDialog;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddonDialog;
			_003C_003EO._003C0_003E__OnAddonDialog = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
	}

	protected override void ConfigUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextColored(ref vector, val);
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(8, 2);
		((ImU8String)(ref val2)).AppendLiteral("/pdr ");
		((ImU8String)(ref val2)).AppendFormatted<string>("scards");
		((ImU8String)(ref val2)).AppendLiteral(" → ");
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoSellCards-CommandHelp"));
		ImGui.TextUnformatted(val2);
	}

	private unsafe void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				layoutNode = null;
				startButton = null;
				stopButton = null;
				titleNode = null;
				base.TaskHelper?.Abort();
			}
		}
		else
		{
			if (Addons.TripleTriadCoinExchange == null)
			{
				return;
			}
			if (layoutNode == null)
			{
				titleNode = new TextNode
				{
					LineSpacing = 23u,
					AlignmentType = (AlignmentType)3,
					FontSize = 23u,
					FontType = (FontType)3,
					TextColor = ColorHelper.GetColor(2u),
					TextOutlineColor = ColorHelper.GetColor(7u),
					BackgroundColor = Vector4.Zero,
					Size = new Vector2(86f, 31f),
					Position = new Vector2(15f, 465f),
					String = ReadOnlySeString.op_Implicit(Info.Title)
				};
				titleNode.AttachNode(((AtkUnitBase)Addons.TripleTriadCoinExchange).RootNode);
				layoutNode = new HorizontalListNode
				{
					IsVisible = true,
					Position = new Vector2(15f, 495f)
				};
				layoutNode.AttachNode(((AtkUnitBase)Addons.TripleTriadCoinExchange).RootNode);
				startButton = new TextButtonNode
				{
					IsVisible = true,
					Size = new Vector2(260f, 35f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Start")),
					OnClick = delegate
					{
						base.TaskHelper.Abort();
						StartHandOver();
					}
				};
				layoutNode.AddNode(startButton);
				stopButton = new TextButtonNode
				{
					IsVisible = true,
					Size = new Vector2(260f, 35f),
					String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Stop")),
					OnClick = delegate
					{
						base.TaskHelper.Abort();
					}
				};
				layoutNode.AddNode(stopButton);
				((AtkResNode)((AtkUnitBase)Addons.TripleTriadCoinExchange).RootNode).SetHeight((ushort)546);
				((AtkComponentNode)((AtkUnitBase)Addons.TripleTriadCoinExchange).WindowNode).SetHeight((ushort)546);
				for (int num = 0; num < ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)Addons.TripleTriadCoinExchange).WindowNode).Component).UldManager)).NodeListCount; num++)
				{
					AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)Addons.TripleTriadCoinExchange).WindowNode).Component).UldManager)).NodeList[num];
					if (ptr != null)
					{
						if (((AtkResNode)ptr).Height == 486)
						{
							((AtkResNode)ptr).SetHeight((ushort)546);
						}
						if (((AtkResNode)ptr).Height == 470)
						{
							((AtkResNode)ptr).SetHeight((ushort)530);
						}
					}
				}
			}
			if (base.TaskHelper.IsBusy)
			{
				startButton.IsEnabled = false;
				stopButton.IsEnabled = true;
			}
			else
			{
				startButton.IsEnabled = true;
				stopButton.IsEnabled = false;
			}
		}
	}

	private unsafe static void OnAddonDialog(AddonEvent type, AddonArgs args)
	{
		if (Addons.ShopCardDialog != null)
		{
			(*Addons.ShopCardDialog).Callback(0, ((AtkValue)((byte*)((AtkUnitBase)Addons.ShopCardDialog).AtkValues + (nint)6 * (nint)Unsafe.SizeOf<AtkValue>())).UInt);
			((AtkUnitBase)Addons.ShopCardDialog).FireCloseCallback();
			((AtkUnitBase)Addons.ShopCardDialog).Close(true);
		}
	}

	private unsafe void OnCommand(string command, string args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if ((*Addons.TripleTriadCoinExchange).IsAddonAndNodesReady())
		{
			StartHandOver();
			return;
		}
		if (!(*EventFramework.Instance()).IsEventIDNearby(EventId.op_Implicit(721135u)))
		{
			base.TaskHelper.Enqueue(delegate
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage("/pdrduty n 195");
			}, "发送九宫幻卡对局室参加申请");
			base.TaskHelper.Enqueue(() => GameState.TerritoryType == 579 && UIModuleExtension.IsScreenReady(), "等待进入九宫幻卡对局室");
		}
		base.TaskHelper.Enqueue(delegate
		{
			new EventStartPackt(LocalPlayerState.EntityID, 721135u).Send();
		}, "发包打开幻卡交换页面");
		base.TaskHelper.Enqueue((Func<bool>)StartHandOver, "开始交换", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.Enqueue(delegate
		{
			if ((*Addons.TripleTriadCoinExchange).IsAddonAndNodesReady())
			{
				(*Addons.TripleTriadCoinExchange).Callback(-1);
			}
		}, "交换完毕, 关闭界面");
		base.TaskHelper.Enqueue(delegate
		{
			OmenServiceBase<ChatManager>.Instance().SendMessage("/pdr leaveduty");
		}, "离开幻卡对局室");
	}

	private unsafe bool StartHandOver()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!Throttler.Shared.Throttle("AutoSellCards-HandOver"))
		{
			return false;
		}
		if ((*Addons.ShopCardDialog).IsAddonAndNodesReady())
		{
			base.TaskHelper.RemoveQueueTasks(2);
			return true;
		}
		if (!(*Addons.TripleTriadCoinExchange).IsAddonAndNodesReady())
		{
			return false;
		}
		if (((AtkValue)((byte*)((AtkUnitBase)Addons.TripleTriadCoinExchange).AtkValues + Unsafe.SizeOf<AtkValue>())).Int == 0)
		{
			base.TaskHelper.RemoveQueueTasks(2);
			return true;
		}
		if (!Convert.ToBoolean(((AtkValue)((byte*)((AtkUnitBase)Addons.TripleTriadCoinExchange).AtkValues + (nint)204 * (nint)Unsafe.SizeOf<AtkValue>())).Byte))
		{
			string text = LanguageManager.Get("AutoSellCards-CurrentCardNotInDeckMessage");
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
			OmenServiceBase<NotifyHelper>.Instance().NotificationWarning(text);
			base.TaskHelper.RemoveQueueTasks(2);
			return true;
		}
		base.TaskHelper.Enqueue(delegate
		{
			(*Addons.TripleTriadCoinExchange).Callback(0, 0, 0);
		}, "点击交换幻卡", 0, null, null, null, null, 2);
		base.TaskHelper.DelayNext(100, "等待 100 毫秒", 2);
		base.TaskHelper.Enqueue((Func<bool>)StartHandOver, "开始新一轮检测交换", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 2);
		return true;
	}
}
