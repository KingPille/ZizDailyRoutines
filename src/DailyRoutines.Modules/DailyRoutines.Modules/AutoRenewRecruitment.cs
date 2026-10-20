using System;
using System.Collections.Frozen;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.LocalLifecycle;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoRenewRecruitment : ModuleBase
{
	private class UEB4ugxikY6wdYjhu8s : ModuleConfig
	{
		public int RenewTime;

		public UEB4ugxikY6wdYjhu8s()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			RenewTime = 5;
			base._002Ector();
		}
	}

	private enum RecruitmentState
	{
		Inactive,
		Scheduled,
		InProgress
	}

	private UEB4ugxikY6wdYjhu8s config;

	private RecruitmentState currentState;

	private Timer? scheduleTimer;

	private Timer? operationWatchdogTimer;

	private static readonly FrozenSet<uint> StartAgainMessages;

	private static readonly FrozenSet<uint> EndMessages;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		TimerLifetime.Begin(this);
		config = ModuleConfigExtension.Load<UEB4ugxikY6wdYjhu8s>(this) ?? new UEB4ugxikY6wdYjhu8s();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		DalamudServiceExtension.Instance<IClientState>().Logout += new LogoutDelegate(py2MfYoE27);
		OmenServiceBase<LogMessageManager>.Instance().RegPre(tvrM9fnG6m);
	}

	protected override void Uninit()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		TimerLifetime.End(this);
		OmenServiceBase<LogMessageManager>.Instance().Unreg(tvrM9fnG6m);
		DalamudServiceExtension.Instance<IClientState>().Logout -= new LogoutDelegate(py2MfYoE27);
		vG1MWq38uH();
		RUoMtEbk2D(RecruitmentState.Inactive);
	}

	protected override void ConfigUI()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		ImGui.AlignTextToFramePadding();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50177E0C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)));
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420CF6BC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
		ImGui.TextUnformatted(val);
		ImGui.SameLine();
		ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
		if (ImGui.SliderInt(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C5F1C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)), ref config.RenewTime, 5, 59, default(ImU8String), (ImGuiSliderFlags)0))
		{
			config.Save(this);
		}
		ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E2C4D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467), config.RenewTime));
		ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
		switch (currentState)
		{
		case RecruitmentState.Inactive:
		{
			Vector4 vector = KnownColor.Gray.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61830)));
			break;
		}
		case RecruitmentState.Scheduled:
		{
			Vector4 vector = KnownColor.Orange.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62034)));
			break;
		}
		case RecruitmentState.InProgress:
		{
			Vector4 vector = KnownColor.GreenYellow.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61712)));
			break;
		}
		}
	}

	private unsafe void E7PMSWDvAs(object? sender, ElapsedEventArgs e)
	{
		if (!TimerLifetime.Receive((object)this, sender, (object?)scheduleTimer, (ElapsedEventHandler)E7PMSWDvAs, e, (Func<Action, Task>)DalamudServiceExtension.Instance<IFramework>().RunOnFrameworkThread) || currentState != RecruitmentState.Scheduled)
		{
			return;
		}
		if (Control.GetLocalPlayer() != null && UIModuleExtension.IsScreenReady())
		{
			if (scheduleTimer != null)
			{
				scheduleTimer.Stop();
				scheduleTimer.Dispose();
				scheduleTimer = null;
			}
			if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)66])
			{
				base.TaskHelper.Abort();
				RUoMtEbk2D(RecruitmentState.Inactive);
				return;
			}
			RUoMtEbk2D(RecruitmentState.InProgress);
			operationWatchdogTimer = new Timer(1000.0)
			{
				AutoReset = true
			};
			operationWatchdogTimer.Elapsed += ksjMVp52M8;
			operationWatchdogTimer.Start();
			s1bMCSfLKX();
		}
		else
		{
			Timer? timer = scheduleTimer;
			if (timer != null)
			{
				timer.Interval = 1000.0;
				timer.Start();
			}
		}
	}

	private void ksjMVp52M8(object? sender, ElapsedEventArgs e)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (TimerLifetime.Receive((object)this, sender, (object?)operationWatchdogTimer, (ElapsedEventHandler)ksjMVp52M8, e, (Func<Action, Task>)DalamudServiceExtension.Instance<IFramework>().RunOnFrameworkThread) && currentState == RecruitmentState.InProgress && !base.TaskHelper.IsBusy)
		{
			s1bMCSfLKX();
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44272D10 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b))));
		}
	}

	private void tvrM9fnG6m(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem values)
	{
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (logMessageID == 978)
		{
			isPrevented = true;
		}
		else if (StartAgainMessages.Contains(logMessageID))
		{
			vG1MWq38uH();
			base.TaskHelper.Abort();
			int num = 60 - config.RenewTime;
			scheduleTimer = new Timer(num * 60000)
			{
				AutoReset = false
			};
			scheduleTimer.Elapsed += E7PMSWDvAs;
			scheduleTimer.Start();
			RUoMtEbk2D(RecruitmentState.Scheduled);
			NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
			string key = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56AA3E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1);
			object[] array = new object[1];
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
			defaultInterpolatedStringHandler.AppendFormatted(OmenServiceBase<StandardTimeManager>.Instance().Now.AddMinutes(num), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23107667 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d));
			array[0] = defaultInterpolatedStringHandler.ToStringAndClear();
			notifyHelper.Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(key, array)));
		}
		else if (EndMessages.Contains(logMessageID))
		{
			base.TaskHelper.Abort();
			vG1MWq38uH();
			RUoMtEbk2D(RecruitmentState.Inactive);
		}
	}

	private void py2MfYoE27(int type, int code)
	{
		base.TaskHelper.Abort();
		vG1MWq38uH();
		RUoMtEbk2D(RecruitmentState.Inactive);
	}

	private void s1bMCSfLKX()
	{
		base.TaskHelper.Enqueue((Func<bool>)EU2MjxgvVX, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -1229922887 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987B447 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
		base.TaskHelper.Enqueue((Func<bool>)uDyMOrGdMZ, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3176D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.DelayNext(1000, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C3DAD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
		base.TaskHelper.Enqueue((Func<bool>)qYgMKGGtaU, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -1217140514 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private unsafe static bool EU2MjxgvVX()
	{
		if (Control.GetLocalPlayer() == null)
		{
			return false;
		}
		return ((AgentLookingForGroup)AgentLookingForGroup.Instance()).OpenListingByContentId(LocalPlayerState.ContentID);
	}

	private unsafe static bool uDyMOrGdMZ()
	{
		AtkUnitBase* lookingForGroupDetail = Addons.LookingForGroupDetail;
		if (!(*lookingForGroupDetail).IsAddonAndNodesReady())
		{
			return false;
		}
		((AgentId)130).SendEvent(2uL, 0);
		((AtkUnitBase)lookingForGroupDetail).Close(true);
		return true;
	}

	private unsafe static bool qYgMKGGtaU()
	{
		AtkUnitBase* lookingForGroupCondition = Addons.LookingForGroupCondition;
		if (!(*lookingForGroupCondition).IsAddonAndNodesReady())
		{
			return false;
		}
		(*((AtkUnitBase)lookingForGroupCondition).GetComponentButtonById(113u)).Click();
		return true;
	}

	private void RUoMtEbk2D(RecruitmentState newState)
	{
		currentState = newState;
	}

	private void vG1MWq38uH()
	{
		if (scheduleTimer != null)
		{
			scheduleTimer.Stop();
			scheduleTimer.Dispose();
			scheduleTimer = null;
		}
		if (operationWatchdogTimer != null)
		{
			operationWatchdogTimer.Stop();
			operationWatchdogTimer.Dispose();
			operationWatchdogTimer = null;
		}
	}

	public AutoRenewRecruitment()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39513C35 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E96C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)),
			Category = ModuleCategory.Recruitment,
			Author = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39513E3D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoRenewRecruitment()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		StartAgainMessages = FrozenSet.Create<uint>(979u, 980u, 991u, 992u);
		EndMessages = FrozenSet.Create<uint>(981u, 982u, 983u, 984u);
	}
}
