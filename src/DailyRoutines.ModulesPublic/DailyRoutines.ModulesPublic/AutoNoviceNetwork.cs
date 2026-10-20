using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Timers;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Windows.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoNoviceNetwork : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool IsTryJoinWhenInactive;
	}

	private Config config;

	private Timer? afkTimer;

	private int tryTimes;

	private bool isJoined;

	private bool isMentor;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoNoviceNetworkTitle"),
		Description = LanguageManager.Get("AutoNoviceNetworkDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
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
		if (afkTimer == null)
		{
			afkTimer = new Timer(10000.0);
		}
		afkTimer.Elapsed += OnAfkStateCheck;
		afkTimer.AutoReset = true;
		afkTimer.Enabled = true;
	}

	protected override void Uninit()
	{
		if (afkTimer != null)
		{
			afkTimer?.Stop();
			afkTimer.Elapsed -= OnAfkStateCheck;
			afkTimer?.Dispose();
			afkTimer = null;
		}
		tryTimes = 0;
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		if (Throttler.Shared.Throttle("AutoNoviceNetwork-UpdateInfo", 1000u))
		{
			isMentor = ((PlayerState)PlayerState.Instance()).IsMentor();
			isJoined = IsInNoviceNetwork();
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoNoviceNetwork-JoinState"));
		((ImU8String)(ref val)).AppendLiteral(":");
		ImGui.TextUnformatted(val);
		ImGui.SameLine();
		Vector4 vector = (isJoined ? ImGuiColors.HealerGreen : ImGuiColors.DPSRed);
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(isJoined ? "√" : "×"));
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(1, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoNoviceNetwork-AttemptedTimes"));
		((ImU8String)(ref val2)).AppendLiteral(":");
		ImGui.TextUnformatted(val2);
		ImGui.SameLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(0, 1);
		((ImU8String)(ref val3)).AppendFormatted<int>(tryTimes);
		ImGui.TextColored(ref vector, val3);
		ImGui.NewLine();
		DisabledDisposable val4 = ImRaii.Disabled(base.TaskHelper.IsBusy || !isMentor);
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61515, LanguageManager.Get("Start")))
			{
				tryTimes = 0;
				base.TaskHelper.Enqueue((Action)EnqueueARound, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61517, LanguageManager.Get("Stop")))
		{
			base.TaskHelper.Abort();
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoNoviceNetwork-TryJoinWhenInactive")), ref config.IsTryJoinWhenInactive))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoNoviceNetwork-TryJoinWhenInactiveHelp"), 20f * Globals.GlobalUIScale, (FontAwesomeIcon)61530);
	}

	private unsafe void EnqueueARound()
	{
		if (!(isMentor = ((PlayerState)PlayerState.Instance()).IsMentor()))
		{
			return;
		}
		base.TaskHelper.Enqueue(delegate
		{
			if (!((PlayerState)PlayerState.Instance()).IsPlayerStateFlagSet((PlayerStateFlag)8))
			{
				OmenServiceBase<ChatManager>.Instance().SendMessage("/beginnerchannel on");
			}
		});
		base.TaskHelper.Enqueue((Action)TryJoin, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.DelayNext(250);
		base.TaskHelper.Enqueue(delegate
		{
			tryTimes++;
		});
		base.TaskHelper.Enqueue(delegate
		{
			if (IsInNoviceNetwork())
			{
				base.TaskHelper.Abort();
			}
			else
			{
				EnqueueARound();
			}
		});
	}

	private unsafe static void TryJoin()
	{
		((InfoProxyNoviceNetwork)InfoProxyNoviceNetwork.Instance()).SendJoinRequest();
	}

	private unsafe static bool IsInNoviceNetwork()
	{
		return ((int)((InfoProxyInterface)((byte*)((InfoModule)InfoModule.Instance()).GetInfoProxyById((InfoProxyId)21) + Unsafe.SizeOf<InfoProxyInterface>())).VirtualTable & 1) != 0;
	}

	private unsafe void OnAfkStateCheck(object? sender, ElapsedEventArgs e)
	{
		if (isMentor = ((PlayerState)PlayerState.Instance()).IsMentor())
		{
			isJoined = IsInNoviceNetwork();
			if (!isJoined && config.IsTryJoinWhenInactive && !base.TaskHelper.IsBusy && !IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()) && !IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()) && (LastInputInfo.GetIdleTimeTick() > 10000 || !GameState.IsForeground))
			{
				TryJoin();
			}
		}
	}
}
