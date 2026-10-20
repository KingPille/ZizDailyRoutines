using System;
using System.Threading;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class AutoConstantlyClick : ModuleBase
{
	private delegate void CheckHotbarClickedDelegate(nint a1, byte a2);

	private delegate void CheckCrossHotbarClickedDelegate(nint a1, int a2);

	private class HeldInfo
	{
		public SimpleTimer LastPress { get; } = new SimpleTimer();

		public SimpleTimer WaitPress { get; } = new SimpleTimer();

		public bool LastFramePressed { get; set; }

		public bool LastFrameHeld { get; set; }

		public bool IsWaiting(AutoConstantlyClick module)
		{
			if (WaitPress.IsRunning)
			{
				return WaitPress.ElapsedMilliseconds < module.config.WaitTime;
			}
			return false;
		}

		public bool GetIsReady(AutoConstantlyClick module)
		{
			if (LastPress.IsRunning)
			{
				return LastPress.ElapsedMilliseconds >= module.config.RepeatInterval;
			}
			return false;
		}

		public void WaitLastPress(AutoConstantlyClick module)
		{
			if (!WaitPress.IsRunning)
			{
				Interlocked.Increment(ref module.runningTimersCount);
			}
			WaitPress.Restart();
			if (!LastPress.IsRunning)
			{
				Interlocked.Increment(ref module.runningTimersCount);
			}
			LastPress.Restart();
		}

		public void RestartLastPress(AutoConstantlyClick module)
		{
			if (!LastPress.IsRunning)
			{
				Interlocked.Increment(ref module.runningTimersCount);
			}
			LastPress.Restart();
		}

		public void ResetLastPress(AutoConstantlyClick module)
		{
			if (LastPress.IsRunning)
			{
				Interlocked.Decrement(ref module.runningTimersCount);
			}
			LastPress.Reset();
			if (WaitPress.IsRunning)
			{
				Interlocked.Decrement(ref module.runningTimersCount);
			}
			WaitPress.Reset();
		}
	}

	private class SimpleTimer
	{
		private long startTime;

		public bool IsRunning { get; private set; }

		public long ElapsedMilliseconds
		{
			get
			{
				if (!IsRunning)
				{
					return 0L;
				}
				return Environment.TickCount64 - startTime;
			}
		}

		public void Restart()
		{
			startTime = Environment.TickCount64;
			IsRunning = true;
		}

		public void Reset()
		{
			startTime = 0L;
			IsRunning = false;
		}
	}

	private class Config : ModuleConfig
	{
		public bool GamepadMode = true;

		public bool MouseMode = true;

		public uint RepeatInterval = 200u;

		public uint WaitTime;
	}

	private static readonly CompSig CheckHotbarClickedSig = new CompSig("E8 ?? ?? ?? ?? 48 8B 4F ?? 48 8B 01 FF 50 ?? 48 8B C8 E8 ?? ?? ?? ?? 84 C0 74");

	private Hook<CheckHotbarClickedDelegate>? CheckHotbarClickedHook;

	private static readonly CompSig CheckCrossHotbarClickedSig = new CompSig("89 54 24 ?? 48 89 4C 24 ?? 56 41 55");

	private Hook<CheckCrossHotbarClickedDelegate>? CheckCrossHotbarClickedHook;

	private Config config;

	private readonly HeldInfo[] inputIDInfos = new HeldInfo[513];

	private int runningTimersCount;

	private bool isHandlingHotbarClick;

	private const int MAX_KEY = 512;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoConstantlyClickTitle"),
		Description = LanguageManager.Get("AutoConstantlyClickDescription"),
		Category = ModuleCategory.System,
		Author = new string[2] { "ziz", "KirisameVanilla" }
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		for (int i = 0; i <= 512; i++)
		{
			inputIDInfos[i] = new HeldInfo();
		}
		if (CheckHotbarClickedHook == null)
		{
			CheckHotbarClickedHook = CheckHotbarClickedSig.GetHook<CheckHotbarClickedDelegate>(CheckHotbarClickedDetour);
		}
		if (CheckCrossHotbarClickedHook == null)
		{
			CheckCrossHotbarClickedHook = CheckCrossHotbarClickedSig.GetHook<CheckCrossHotbarClickedDelegate>(CheckCrossHotbarClickedDetour);
		}
		InputIDManager inputIDManager = OmenServiceBase<InputIDManager>.Instance();
		inputIDManager.RegPrePressed(OnPrePressed);
		inputIDManager.RegPreHeld(OnPrePressed);
		UpdateHookState();
	}

	protected override void Uninit()
	{
		InputIDManager inputIDManager = OmenServiceBase<InputIDManager>.Instance();
		inputIDManager.UnregPrePressed(OnPrePressed);
		inputIDManager.UnregPreHeld(OnPrePressed);
	}

	protected override void ConfigUI()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(19, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Interval"));
		((ImU8String)(ref val)).AppendLiteral("（ms）##Throttle Time");
		ImU8String val2 = val;
		ref uint repeatInterval = ref config.RepeatInterval;
		ImU8String val3 = default(ImU8String);
		ImGui.InputUInt(val2, ref repeatInterval, 100u, 1000u, val3, (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		((ImU8String)(ref val3))._002Ector(14, 1);
		((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoConstantlyClick-WaitTime"));
		((ImU8String)(ref val3)).AppendLiteral("（ms）##WaitTime");
		ImGui.InputUInt(val3, ref config.WaitTime, 100u, 1000u, default(ImU8String), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("AutoConstantlyClick-WaitTime-Help"), 40f, (FontAwesomeIcon)61530);
		ImGui.NewLine();
		if ((0u | (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoConstantlyClick-MouseMode")), ref config.MouseMode) ? 1u : 0u) | (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoConstantlyClick-GamepadMode")), ref config.GamepadMode) ? 1u : 0u)) != 0)
		{
			config.Save(this);
			UpdateHookState();
		}
	}

	private void UpdateHookState()
	{
		if (config.MouseMode || config.GamepadMode)
		{
			CheckHotbarClickedHook.Enable();
			CheckCrossHotbarClickedHook.Enable();
		}
		else
		{
			CheckHotbarClickedHook.Disable();
			CheckCrossHotbarClickedHook.Disable();
		}
	}

	private void OnPrePressed(ref bool? overrideResult, ref InputId key)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Invalid comparison between Unknown and I4
		if (!isHandlingHotbarClick)
		{
			return;
		}
		InputId val = key;
		if ((int)val >= 210)
		{
			if ((int)val > 226)
			{
				goto IL_002b;
			}
		}
		else if ((int)val < 45 || (int)val > 190)
		{
			goto IL_002b;
		}
		bool flag = false;
		goto IL_0033;
		IL_0033:
		if (flag)
		{
			return;
		}
		HeldInfo heldInfo = inputIDInfos[(int)key];
		bool flag2 = OmenServiceBase<InputIDManager>.Instance().IsInputIDPressed(key);
		bool flag3 = OmenServiceBase<InputIDManager>.Instance().IsInputIDDown(key);
		if (!heldInfo.IsWaiting(this) && flag3 && !heldInfo.LastFrameHeld)
		{
			if (config.WaitTime != 0)
			{
				heldInfo.WaitLastPress(this);
			}
			else
			{
				heldInfo.RestartLastPress(this);
			}
		}
		overrideResult = ((heldInfo.IsWaiting(this) || !heldInfo.GetIsReady(this)) ? flag2 : flag3);
		if (overrideResult.Value)
		{
			heldInfo.RestartLastPress(this);
		}
		else if (flag3 != heldInfo.LastFrameHeld)
		{
			if (flag3 && runningTimersCount > 0)
			{
				heldInfo.RestartLastPress(this);
			}
			else
			{
				heldInfo.ResetLastPress(this);
			}
		}
		heldInfo.LastFrameHeld = flag3;
		heldInfo.LastFramePressed = flag2;
		return;
		IL_002b:
		flag = true;
		goto IL_0033;
	}

	private void CheckHotbarClickedDetour(nint a1, byte a2)
	{
		isHandlingHotbarClick = true;
		try
		{
			CheckHotbarClickedHook.Original(a1, a2);
		}
		finally
		{
			isHandlingHotbarClick = false;
		}
	}

	private void CheckCrossHotbarClickedDetour(nint a1, int a2)
	{
		isHandlingHotbarClick = true;
		try
		{
			CheckCrossHotbarClickedHook.Original(a1, a2);
		}
		finally
		{
			isHandlingHotbarClick = false;
		}
	}
}
