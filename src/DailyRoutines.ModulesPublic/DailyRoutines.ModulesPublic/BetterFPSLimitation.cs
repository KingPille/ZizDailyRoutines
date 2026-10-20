using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImPlot;
using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class BetterFPSLimitation : ModuleBase
{
	private class AddonDRBetterFPSLimitation(BetterFPSLimitation module) : NativeAddon
	{
		public VerticalListNode? FPSWidget;

		private TextNode? FPSDisplayNumberNode;

		private NumericInputNode? FPSInputNode;

		private CheckboxNode? IsEnabledNode;

		protected unsafe override void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValues)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			FPSWidget = new VerticalListNode
			{
				FitContents = true,
				Position = base.ContentStartPosition
			};
			IsEnabledNode = new CheckboxNode
			{
				Size = new Vector2(200f, 20f),
				IsChecked = module.config.IsEnabled,
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("Enable")),
				OnClick = delegate(bool newState)
				{
					module.config.IsEnabled = newState;
					module.config.Save(module);
				}
			};
			FPSWidget.AddNode(IsEnabledNode);
			FPSWidget.AddDummy(8f);
			TextNode node = new TextNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterFPSLimitation-MaxFPS")),
				FontSize = 14u,
				Size = new Vector2(200f, 28f)
			};
			FPSWidget.AddNode(node);
			FPSInputNode = new NumericInputNode
			{
				Size = new Vector2(200f, 28f),
				Min = 1,
				Max = 32767,
				Step = 10,
				OnValueUpdate = delegate(int newValue)
				{
					module.config.Limitation = (short)newValue;
					module.config.Save(module);
				},
				Value = module.config.Limitation
			};
			FPSInputNode.Value = module.config.Limitation;
			FPSInputNode.ValueTextNode.SetNumber(module.config.Limitation);
			FPSWidget.AddNode(FPSInputNode);
			ResNode resNode = new ResNode
			{
				Width = 200f,
				Height = 28f
			};
			TextNode textNode = new TextNode();
			textNode.String = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterFPSLimitation-CurrentFPS"));
			textNode.FontSize = 12u;
			textNode.Size = new Vector2(20f, 25f);
			textNode.AlignmentType = (AlignmentType)3;
			textNode.AttachNode(resNode);
			FPSDisplayNumberNode = new TextNode
			{
				String = ReadOnlySeString.op_Implicit("0"),
				FontSize = 12u,
				Size = new Vector2(30f, 25f),
				X = 180f,
				AlignmentType = (AlignmentType)5,
				TextFlags = (TextFlags)1
			};
			FPSDisplayNumberNode.AttachNode(resNode);
			FPSWidget.AddDummy(8f);
			FPSWidget.AddNode(resNode);
			FPSWidget.AddDummy(8f);
			TextNode node2 = new TextNode
			{
				String = ReadOnlySeString.op_Implicit(LanguageManager.Get("BetterFPSLimitation-FastSetFPSLimitation")),
				FontSize = 14u,
				Size = new Vector2(200f, 20f),
				AlignmentType = (AlignmentType)3
			};
			FPSWidget.AddNode(node2);
			foreach (List<short> item in (from x in module.config.Thresholds.Select((short value, int index) => new { value, index })
				group x by x.index / 3 into g
				select g.Select(x => x.value).ToList()).ToList())
			{
				FPSWidget.AddDummy(8f);
				HorizontalFlexNode horizontalFlexNode = new HorizontalFlexNode
				{
					Width = 200f,
					Height = 28f
				};
				foreach (short threshold in item)
				{
					TextButtonNode node3 = new TextButtonNode
					{
						Size = new Vector2(60f, 25f),
						IsVisible = true,
						String = ReadOnlySeString.op_Implicit(threshold.ToString()),
						OnClick = delegate
						{
							module.config.Limitation = threshold;
							module.config.IsEnabled = true;
							module.config.Save(module);
							FPSInputNode.Value = module.config.Limitation;
							FPSInputNode.ValueTextNode.SetNumber(module.config.Limitation);
						}
					};
					horizontalFlexNode.AddNode(node3);
				}
				FPSWidget.AddNode(horizontalFlexNode);
			}
			FPSWidget.RecalculateLayout();
			FPSWidget.AttachNode(this);
			SetWindowSize(base.Size.X, FPSWidget.Height + base.ContentStartPosition.Y + 24f);
		}

		protected unsafe override void OnUpdate(AtkUnitBase* addon)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			if (Throttler.Shared.Throttle("BetterFPSLimitation.Addon.Update", 1000u))
			{
				TextNode? fPSDisplayNumberNode = FPSDisplayNumberNode;
				if (fPSDisplayNumberNode != null)
				{
					ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
					SeStringParameter reference = SeStringParameter.op_Implicit((int)MathF.Round(module.currentFPS));
					fPSDisplayNumberNode.String = obj.EvaluateFromAddon(4002u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
				}
				FPSDisplayNumberNode.Width = FPSDisplayNumberNode.GetTextDrawSize(considerScale: false).X;
				FPSDisplayNumberNode.X = 194f - FPSDisplayNumberNode.Width;
				CheckboxNode? isEnabledNode = IsEnabledNode;
				if (isEnabledNode != null)
				{
					isEnabledNode.IsChecked = module.config.IsEnabled;
				}
				NumericInputNode? fPSInputNode = FPSInputNode;
				if (fPSInputNode != null)
				{
					fPSInputNode.Value = module.config.Limitation;
				}
			}
		}
	}

	private class Config : ModuleConfig
	{
		public bool IsEnabled;

		public short Limitation = 60;

		public List<short> Thresholds = new List<short>();
	}

	private sealed class FramePacer : IDisposable
	{
		private readonly BetterFPSLimitation module;

		private Hook<Present>? SwapChainPresentHook;

		private long frameInterval;

		private long frameRemainder;

		private long nextTimestamp;

		private int lastTargetFPS;

		private long timerOvershoot;

		private nint timer;

		private long lastPresentTimestamp;

		private long accumulatedTicks;

		private int accumulatedFrames;

		private const int SPIN_WAIT_ITERATIONS = 256;

		private const int MAX_DRIFT_INTERVALS = 4;

		private const int COOLDOWN_MARGIN_DIVISOR = 10000;

		private const int OVERSHOOT_DECAY_DIVISOR = 16;

		private const uint CREATE_WAITABLE_TIMER_HIGH_RESOLUTION = 2u;

		private const uint TIMER_ALL_ACCESS = 2031619u;

		private const uint INFINITE = uint.MaxValue;

		public float FPS { get; private set; }

		public unsafe FramePacer(BetterFPSLimitation inModule)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			module = inModule;
			timer = CreateWaitableTimerExW(0, 0, 2u, 2031619u);
			timerOvershoot = Stopwatch.Frequency / 2000;
			SwapChainPresentHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<Present>(typeof(MemberFunctionPointers), "Present", new Present(SwapChainPresentDetour));
			SwapChainPresentHook.Enable();
		}

		public void Dispose()
		{
			if (SwapChainPresentHook != null)
			{
				SwapChainPresentHook.Dispose();
				SwapChainPresentHook = null;
			}
			if (timer != 0)
			{
				CloseHandle(timer);
				timer = 0;
			}
		}

		private unsafe void SwapChainPresentDetour(SwapChain* thisPtr)
		{
			if (module.config.IsEnabled)
			{
				Wait(module.config.Limitation);
			}
			RecordFrame();
			((Device)Device.Instance()).FrameRateLimitPreset = 0u;
			SwapChainPresentHook.Original.Invoke(thisPtr);
		}

		public void Wait(int targetFPS)
		{
			if (targetFPS <= 0)
			{
				return;
			}
			long frequency = Stopwatch.Frequency;
			if (targetFPS != lastTargetFPS)
			{
				lastTargetFPS = targetFPS;
				frameRemainder = 0L;
				nextTimestamp = 0L;
			}
			frameRemainder += frequency % targetFPS;
			frameInterval = frequency / targetFPS;
			if (frameRemainder >= targetFPS)
			{
				frameRemainder -= targetFPS;
				frameInterval++;
			}
			if (frameInterval > 0)
			{
				long timestamp = Stopwatch.GetTimestamp();
				if (nextTimestamp <= 0 || timestamp - nextTimestamp > frameInterval * 4)
				{
					nextTimestamp = timestamp;
				}
				else if (timestamp < nextTimestamp)
				{
					WaitUntil(nextTimestamp, frequency);
				}
				nextTimestamp += frameInterval;
			}
		}

		public void RecordFrame()
		{
			long timestamp = Stopwatch.GetTimestamp();
			if (lastPresentTimestamp != 0L)
			{
				accumulatedTicks += timestamp - lastPresentTimestamp;
				accumulatedFrames++;
				if (accumulatedTicks >= Stopwatch.Frequency)
				{
					FPS = (float)((double)accumulatedFrames * (double)Stopwatch.Frequency / (double)accumulatedTicks);
					accumulatedTicks = 0L;
					accumulatedFrames = 0;
				}
			}
			lastPresentTimestamp = timestamp;
		}

		private void WaitUntil(long target, long frequency)
		{
			long val = ((timer != 0) ? (timerOvershoot + frequency / 10000) : (frequency / 1000));
			val = Math.Min(val, frameInterval / 2);
			while (true)
			{
				long num = target - Stopwatch.GetTimestamp();
				if (num > 0)
				{
					if (num <= val)
					{
						Thread.SpinWait(256);
					}
					else
					{
						SleepPrecise(num - val, frequency);
					}
					continue;
				}
				break;
			}
		}

		private void SleepPrecise(long ticks, long frequency)
		{
			long num = Stopwatch.GetTimestamp() + ticks;
			if (timer == 0)
			{
				Thread.Sleep((int)(ticks * 1000 / frequency));
			}
			else
			{
				long lpDueTime = -(ticks * 10000000 / frequency);
				SetWaitableTimer(timer, ref lpDueTime, 0, 0, 0, 0);
				WaitForSingleObject(timer, uint.MaxValue);
			}
			long num2 = Stopwatch.GetTimestamp() - num;
			if (num2 > timerOvershoot)
			{
				timerOvershoot = num2;
			}
			else
			{
				timerOvershoot -= timerOvershoot / 16;
			}
		}

		[DllImport("kernel32.dll", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		private static extern nint CreateWaitableTimerExW(nint lpTimerAttributes, nint lpTimerName, uint dwFlags, uint dwDesiredAccess);

		[DllImport("kernel32.dll", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		private static extern int SetWaitableTimer(nint hTimer, ref long lpDueTime, int lPeriod, nint pfnCompletionRoutine, nint lpArgToCompletionRoutine, int fResume);

		[DllImport("kernel32.dll", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		private static extern uint WaitForSingleObject(nint hHandle, uint dwMilliseconds);

		[DllImport("kernel32.dll", SetLastError = true)]
		[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
		private static extern int CloseHandle(nint hObject);
	}

	private AddonDRBetterFPSLimitation? addon;

	private Config config;

	private IDtrBarEntry? entry;

	private FramePacer? pacer;

	private ushort newThresholdInput = 120;

	private const string COMMAND = "fps";

	private readonly float[] fpsHistory = new float[100];

	private int fpsHistoryIndex;

	private int fpsHistoryFilledCount;

	private float currentFPS;

	private float averageFPS;

	private float minFPS;

	private float maxFPS;

	private const int HISTORY_LENGTH = 100;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("BetterFPSLimitationTitle"),
		Description = LanguageManager.Get("BetterFPSLimitationDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	private void ToggleAddon()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		AddonDRBetterFPSLimitation addonDRBetterFPSLimitation = addon;
		if (addonDRBetterFPSLimitation != null && addonDRBetterFPSLimitation.IsOpen)
		{
			addon.Dispose();
			addon = null;
			return;
		}
		addon?.Dispose();
		addon = new AddonDRBetterFPSLimitation(this)
		{
			InternalName = "DRBetterFPSLimitation",
			Title = ReadOnlySeString.op_Implicit(LuminaWrapper.GetAddonText(4032u)),
			Size = new Vector2(220f, 240f),
			RememberClosePosition = true
		};
		addon.Open();
	}

	protected override void Init()
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		Config obj = ModuleConfigExtension.Load<Config>(this);
		if (obj == null)
		{
			obj = new Config();
			Config obj2 = obj;
			int num = 6;
			List<short> list = new List<short>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<short> span = CollectionsMarshal.AsSpan(list);
			span[0] = 15;
			span[1] = 30;
			span[2] = 45;
			span[3] = 60;
			span[4] = 90;
			span[5] = 120;
			obj2.Thresholds = list;
		}
		config = obj;
		pacer = new FramePacer(this);
		if (entry == null)
		{
			entry = DalamudServiceExtension.Instance<IDtrBar>().Get("DailyRoutines-BetterFPSLimitation", (SeString)null);
		}
		entry.OnClick = delegate(DtrInteractionEvent param)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I4
			MouseClickType clickType = param.ClickType;
			if ((int)clickType != 0)
			{
				if ((int)clickType == 1)
				{
					EnsureOverlay();
					Overlay? overlay = base.Overlay;
					if (overlay != null)
					{
						((Window)overlay).Toggle();
					}
				}
			}
			else
			{
				ToggleAddon();
			}
		};
		entry.Shown = true;
		entry.Text = SeString.op_Implicit(LuminaWrapper.GetAddonText(4002u));
		entry.Tooltip = SeString.op_Implicit(LanguageManager.Get("BetterFPSLimitation-DTR-Tooltip"));
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 1000u);
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("fps", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("BetterFPSLimitation-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("fps");
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		IDtrBarEntry? obj = entry;
		if (obj != null)
		{
			obj.Remove();
		}
		entry = null;
		addon?.Dispose();
		addon = null;
		pacer?.Dispose();
		pacer = null;
		ResetHistory();
	}

	protected override void ConfigUI()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LanguageManager.Get("Command")))
		{
			ImU8String val = new ImU8String(6, 2);
			((ImU8String)(ref val)).AppendLiteral("/pdr ");
			((ImU8String)(ref val)).AppendFormatted<string>("fps");
			((ImU8String)(ref val)).AppendLiteral(" ");
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("BetterFPSLimitation-CommandHelp"));
			ImGui.TextUnformatted(val);
		}
		ImGui.NewLine();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("BetterFPSLimitation-FastSetFPSLimitation")));
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			foreach (short item in config.Thresholds.ToList())
			{
				IdDisposable val3 = ImRaii.PushId((int)item, true);
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get("Delete")))
					{
						config.Thresholds.Remove(item);
						continue;
					}
					ImGui.SameLine();
					ImU8String val4 = new ImU8String(0, 1);
					((ImU8String)(ref val4)).AppendFormatted<short>(item);
					ImGui.TextUnformatted(val4);
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")) && newThresholdInput > 1 && newThresholdInput <= 32767 && !config.Thresholds.Contains((short)newThresholdInput))
			{
				config.Thresholds.Add((short)newThresholdInput);
				config.Save(this);
			}
			ImGui.SameLine();
			ImGui.SetNextItemWidth(100f * Globals.GlobalUIScale);
			if (ImGui.InputUShort(ImU8String.op_Implicit("###NewThreshold"), ref newThresholdInput, (ushort)10, (ushort)10, default(ImU8String), (ImGuiInputTextFlags)0))
			{
				newThresholdInput = (ushort)Math.Clamp((int)newThresholdInput, 1, 32767);
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void OnCommand(string command, string args)
	{
		ToggleAddon();
	}

	private unsafe void OnUpdate(IFramework _)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (entry == null)
		{
			return;
		}
		currentFPS = ((pacer.FPS >= 1f) ? pacer.FPS : 1f);
		RecordFPS(currentFPS);
		ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
		SeStringParameter reference = SeStringParameter.op_Implicit((int)MathF.Round(currentFPS));
		ReadOnlySeString val = obj.EvaluateFromAddon(4002u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
		if (config.IsEnabled)
		{
			RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val2))._002Ector();
			try
			{
				val = ((RentedSeStringBuilder)(ref val2)).Builder.PushEdgeColorType(37u).Append(val).PopEdgeColorType()
					.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		entry.Text = SeStringExtensions.ToDalamudString(val);
	}

	protected unsafe override void OverlayUI()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		if (!ImGui.IsWindowFocused((ImGuiFocusedFlags)3))
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		Vector4 fPSColor = GetFPSColor(currentFPS);
		ImGui.SetWindowFontScale(1.5f);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<float>(currentFPS, "F0");
		ImGui.TextColored(ref fPSColor, val);
		ImGui.SetWindowFontScale(1f);
		ImGui.SameLine();
		ImGui.TextColored(ref fPSColor, ImU8String.op_Implicit("FPS"));
		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.SameLine();
		TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("##FPSStatsTable"), 4, (ImGuiTableFlags)24576);
		try
		{
			if (TableDisposable.op_Implicit(val2))
			{
				DrawStatColumn("AVG", $"{averageFPS:F0}", GetFPSColor(averageFPS));
				DrawStatColumn("MIN", $"{minFPS:F0}", GetFPSColor(minFPS));
				DrawStatColumn("MAX", $"{maxFPS:F0}", GetFPSColor(maxFPS));
				DrawStatColumn("CAP", $"{config.Limitation:F0}", GetCapColor());
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
		PlotColorDisposable val3 = ImRaii.PushColor((ImPlotCol)16, new Vector4(0.05f), true);
		try
		{
			PlotColorDisposable val4 = ImRaii.PushColor((ImPlotCol)5, Vector4.Zero, true);
			try
			{
				PlotColorDisposable val5 = ImRaii.PushColor((ImPlotCol)14, new Vector4(1f, 1f, 1f, 0.05f), true);
				try
				{
					PlotStyleDisposable val6 = ImRaii.PushStyle((ImPlotStyleVar)4, 0.25f, true);
					try
					{
						PlotStyleDisposable val7 = ImRaii.PushStyle((ImPlotStyleVar)0, 2f, true);
						try
						{
							PlotDisposable val8 = ImRaii.Plot("##FPSPlot", new Vector2(-1f), (ImPlotFlags)55);
							try
							{
								if (!val8)
								{
									return;
								}
								ImPlot.SetupAxes((byte*)null, (byte*)null, (ImPlotAxisFlags)9, (ImPlotAxisFlags)9);
								float num = MathF.Max(MathF.Max(maxFPS, config.Limitation) * 1.25f, 100f);
								ImPlot.SetupAxesLimits(0.0, (double)fpsHistory.Length, 0.0, (double)num, (ImPlotCond)1);
								ImPlot.SetupAxisTicks((ImAxis)0, 0.0, (double)fpsHistory.Length, 51);
								ImPlot.SetupAxisTicks((ImAxis)3, 0.0, (double)num, 21);
								PlotColorDisposable val9 = ImRaii.PushColor((ImPlotCol)0, fPSColor, true).Push((ImPlotCol)1, fPSColor, true);
								try
								{
									ImPlot.PlotLine("##FPS", ref fpsHistory[0], fpsHistory.Length, 1.0, 0.0, (ImPlotLineFlags)16384, fpsHistoryIndex);
								}
								finally
								{
									((IDisposable)val9)?.Dispose();
								}
								if (averageFPS <= 0f)
								{
									return;
								}
								Vector4 vector = KnownColor.White.ToVector4();
								vector.W = 0.6f;
								Vector4 vector2 = vector;
								double[] array = new double[2] { 0.0, fpsHistory.Length };
								double[] array2 = new double[2] { averageFPS, averageFPS };
								val9 = ImRaii.PushColor((ImPlotCol)0, vector2, true);
								try
								{
									ImPlot.PlotLine("##FPSAvg", ref array[0], ref array2[0], 2);
								}
								finally
								{
									((IDisposable)val9)?.Dispose();
								}
							}
							finally
							{
								((PlotDisposable)(ref val8)).Dispose();
							}
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
	}

	private void EnsureOverlay()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (base.Overlay == null)
		{
			base.Overlay = new Overlay(this);
			Overlay? overlay = base.Overlay;
			((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags & -65);
			Overlay? overlay2 = base.Overlay;
			WindowSizeConstraints value = default(WindowSizeConstraints);
			((WindowSizeConstraints)(ref value))._002Ector();
			((WindowSizeConstraints)(ref value)).MinimumSize = Globals.ScaledVector2(300f, 200f);
			((Window)overlay2).SizeConstraints = value;
		}
	}

	private void ResetHistory()
	{
		Array.Clear(fpsHistory);
		fpsHistoryIndex = 0;
		fpsHistoryFilledCount = 0;
		currentFPS = 0f;
		averageFPS = 0f;
		minFPS = 0f;
		maxFPS = 0f;
	}

	private void RecordFPS(float fps)
	{
		fpsHistory[fpsHistoryIndex] = fps;
		fpsHistoryIndex = (fpsHistoryIndex + 1) % fpsHistory.Length;
		if (fpsHistoryFilledCount < fpsHistory.Length)
		{
			fpsHistoryFilledCount++;
		}
		if (fpsHistoryFilledCount == 0)
		{
			averageFPS = 0f;
			minFPS = 0f;
			maxFPS = 0f;
			return;
		}
		float num = float.MaxValue;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < fpsHistoryFilledCount; i++)
		{
			float num4 = fpsHistory[i];
			if (num4 < num)
			{
				num = num4;
			}
			if (num4 > num2)
			{
				num2 = num4;
			}
			num3 += num4;
		}
		averageFPS = num3 / (float)fpsHistoryFilledCount;
		minFPS = ((num == float.MaxValue) ? 0f : num);
		maxFPS = num2;
	}

	private Vector4 GetFPSColor(float fps)
	{
		if (config.IsEnabled)
		{
			if (config.Limitation <= 0)
			{
				return KnownColor.Gray.ToVector4();
			}
			float num = fps / (float)config.Limitation;
			if (!(num >= 0.95f))
			{
				if (num >= 0.75f)
				{
					return KnownColor.Orange.ToVector4();
				}
				return KnownColor.Red.ToVector4();
			}
			return KnownColor.SpringGreen.ToVector4();
		}
		if (!(fps >= 60f))
		{
			if (fps >= 30f)
			{
				return KnownColor.Orange.ToVector4();
			}
			return KnownColor.Red.ToVector4();
		}
		return KnownColor.SpringGreen.ToVector4();
	}

	private Vector4 GetCapColor()
	{
		if (!config.IsEnabled)
		{
			return KnownColor.Gray.ToVector4();
		}
		return KnownColor.SpringGreen.ToVector4();
	}

	private static void DrawStatColumn(string label, string value, Vector4 color)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		ImGui.Spacing();
		ImGui.TextDisabled(ImU8String.op_Implicit(label));
		ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
		using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
		{
			ImGui.TextColored(ref color, ImU8String.op_Implicit(value));
		}
	}
}
