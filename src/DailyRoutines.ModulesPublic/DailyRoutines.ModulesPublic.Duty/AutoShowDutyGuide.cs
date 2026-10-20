using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.LocalData;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoShowDutyGuide : ModuleBase
{
	private class Config : ModuleConfig
	{
		public float FontScale = 1f;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CGetDutyGuide_003Ed__13 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public uint dutyID;

		public AutoShowDutyGuide _003C_003E4__this;

		private TaskAwaiter<string> _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			AutoShowDutyGuide autoShowDutyGuide = _003C_003E4__this;
			try
			{
				try
				{
					TaskAwaiter<string> awaiter;
					if (num != 0)
					{
						awaiter = OmenServiceBase<HTTPClientHelper>.Instance().Get().GetStringAsync($"https://raw.githubusercontent.com/thewakingsands/novice-network/7d4fdab9d9a4aa1ade5941b504985d9800c88ac1/docs/duty/{dutyID}.md")
							.GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter<string>);
						num = (_003C_003E1__state = -1);
					}
					string text = awaiter.GetResult().SanitizeMarkdown();
					if (!string.IsNullOrWhiteSpace(text))
					{
						autoShowDutyGuide.guideData = text.Split('\n').ToList();
						((Window)autoShowDutyGuide.Overlay).IsOpen = true;
					}
				}
				catch
				{
				}
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult();
		}

		void IAsyncStateMachine.MoveNext()
		{
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.SetStateMachine(stateMachine);
		}
	}

	private Config config;

	private List<string> guideData = new List<string>();

	private bool isOnDebug;

	private const string FF14_ORG_LINK_BASE = "hxxps://gh.atmoomen.top/raw.githubusercontent.com/thewakingsands/novice-network/refs/heads/master/docs/duty/{0}.md";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = "Auto Show Duty Guide",
		Description = "Automatically display a bundled English duty guide in an overlay when entering a duty.",
		Category = ModuleCategory.Duty
	};

	protected override void Init()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		HuntSupport.NormalizeScale(ref config.FontScale);
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 60000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		Overlay? overlay3 = base.Overlay;
		((Window)overlay3).Flags = (ImGuiWindowFlags)(((Window)overlay3).Flags & -2);
		Overlay? overlay4 = base.Overlay;
		((Window)overlay4).Flags = (ImGuiWindowFlags)(((Window)overlay4).Flags & -65);
		Overlay? overlay5 = base.Overlay;
		((Window)overlay5).Flags = (ImGuiWindowFlags)(((Window)overlay5).Flags | 0x42000);
		((Window)base.Overlay).ShowCloseButton = false;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChange;
		OnZoneChange(0u);
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChange;
		guideData.Clear();
	}

	protected override void ConfigUI()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		HuntSupport.NormalizeScale(ref config.FontScale);
		ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
		ImGui.InputFloat(ImU8String.op_Implicit("Font Scale"), ref config.FontScale, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0);
		HuntSupport.NormalizeScale(ref config.FontScale);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		DisabledDisposable val = ImRaii.Disabled(IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()));
		try
		{
			if (ImGui.Checkbox(ImU8String.op_Implicit("Preview Mode"), ref isOnDebug))
			{
				base.TaskHelper.Abort();
				guideData.Clear();
				((Window)base.Overlay).IsOpen = false;
				if (isOnDebug)
				{
					base.TaskHelper.EnqueueAsync(() => GetDutyGuide(1u));
				}
			}
			ImGuiOm.TooltipHover("Preview a bundled English duty guide. No download is required.");
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	protected override void OverlayOnOpen()
	{
		ImGui.SetScrollHereY();
	}

	protected override void OverlayPreDraw()
	{
		if (!isOnDebug && (!IConditionExtension.get_IsBoundByDuty(DalamudServiceExtension.Instance<ICondition>()) || guideData.Count <= 0))
		{
			((Window)base.Overlay).IsOpen = false;
			guideData.Clear();
			base.TaskHelper.Abort();
		}
		else if (guideData.Count > 0)
		{
			((Window)base.Overlay).WindowName = guideData[0] + "###AutoShowDutyGuide-GuideWindow";
		}
	}

	protected override void OverlayUI()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().GetUIFont(HuntSupport.ClampScale(config.FontScale)).Push())
		{
			ImGui.Spacing();
			ImGui.Separator();
			ImGui.Spacing();
			for (int i = 1; i < guideData.Count; i++)
			{
				string text = guideData[i];
				ImU8String val = new ImU8String(14, 1);
				((ImU8String)(ref val)).AppendLiteral("DutyGuideLine-");
				((ImU8String)(ref val)).AppendFormatted<int>(i);
				IdDisposable val2 = ImRaii.PushId(val, true);
				try
				{
					ImGui.TextWrapped(ImU8String.op_Implicit(text));
					if (ImGui.IsItemHovered())
					{
						ImGui.SetMouseCursor((ImGuiMouseCursor)7);
					}
					if (ImGui.IsItemClicked())
					{
						ImGui.SetClipboardText(ImU8String.op_Implicit(text));
						OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit("Copied this guide section to the clipboard."));
					}
					ImGui.NewLine();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
		}
	}

	private void OnZoneChange(uint u)
	{
		base.TaskHelper.Abort();
		guideData.Clear();
		((Window)base.Overlay).IsOpen = false;
		if (GameState.ContentFinderCondition != 0)
		{
			base.TaskHelper.EnqueueAsync(() => GetDutyGuide(GameState.ContentFinderCondition));
		}
	}

	[AsyncStateMachine(typeof(_003CGetDutyGuide_003Ed__13))]
	private Task GetDutyGuide(uint dutyID)
	{
		guideData = HuntSupport.ReadEnglishGuide(dutyID);
		Overlay? overlay = base.Overlay;
		if (overlay != null)
		{
			((Window)overlay).IsOpen = true;
		}
		return HuntSupport.Completed();
	}
}
