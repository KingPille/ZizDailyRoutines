using System;
using System.Drawing;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Windows.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class FastResetStrikingDummy : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool IsAutoClearEnmityWhenInactive;

		public uint AutoClearEnmityInterval = 30u;
	}

	private CancellationTokenSource cancelSource;

	private Config config;

	private bool isAlreadyRequest;

	private const string COMMAND = "resetallsd";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastResetStrikingDummyTitle"),
		Description = LanguageManager.Get("FastResetStrikingDummyDescription", "resetallsd"),
		Category = ModuleCategory.Combat
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		cancelSource = new CancellationTokenSource();
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPre(OnResetStrikingDummies);
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("resetallsd", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("FastResetStrikingDummy-CommandHelp")
		});
		Task.Run((Func<Task?>)AutoClearLoop);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnResetStrikingDummies);
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("resetallsd");
		cancelSource.Cancel();
		cancelSource.Dispose();
	}

	protected override void ConfigUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(8, 2);
			((ImU8String)(ref val3)).AppendLiteral("/pdr ");
			((ImU8String)(ref val3)).AppendFormatted<string>("resetallsd");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("FastResetStrikingDummy-CommandHelp"));
			ImGui.TextUnformatted(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastResetStrikingDummy-AutoClearEnmityWhenInactive")), ref config.IsAutoClearEnmityWhenInactive))
		{
			config.Save(this);
		}
		ImGuiOm.HelpMarker(LanguageManager.Get("FastResetStrikingDummy-AutoClearEnmityWhenInactive-Help"), 40f, (FontAwesomeIcon)61530);
		if (config.IsAutoClearEnmityWhenInactive)
		{
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			if (ImGui.InputUInt(ImU8String.op_Implicit(LanguageManager.Get("FastResetStrikingDummy-AutoClearEnmityInterval")), ref config.AutoClearEnmityInterval, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)0))
			{
				config.AutoClearEnmityInterval = Math.Clamp(config.AutoClearEnmityInterval, 5u, 300u);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
		}
	}

	private void OnCommand(string command, string arguments)
	{
		ResetAllStrikingDummies();
	}

	public void OnResetStrikingDummies(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4)
	{
		if (command == ExecuteCommandFlag.ResetStrikingDummy)
		{
			isPrevented = true;
			ResetAllStrikingDummies();
		}
	}

	private void ResetAllStrikingDummies()
	{
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)FindAndResetInternal, TimeSpan.Zero, 0, cancelSource.Token);
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)FindAndResetInternal, TimeSpan.FromMilliseconds(500L), 0, cancelSource.Token);
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)FindAndResetInternal, TimeSpan.FromMilliseconds(1000L), 0, cancelSource.Token);
	}

	private unsafe static void FindAndResetInternal()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Span<HaterInfo> haters = ((Hater)(&((UIState)UIState.Instance()).Hater)).Haters;
		for (int i = 0; i < haters.Length; i++)
		{
			HaterInfo val = haters[i];
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ResetStrikingDummy, val.EntityId);
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		if ((int)flag == 26 && value)
		{
			isAlreadyRequest = false;
		}
	}

	private async Task AutoClearLoop()
	{
		try
		{
			while (!cancelSource.IsCancellationRequested)
			{
				await Task.Delay(1000, cancelSource.Token).ConfigureAwait(continueOnCapturedContext: false);
				if (!isAlreadyRequest && config.IsAutoClearEnmityWhenInactive && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26] && (LastInputInfo.GetIdleTimeTick() > config.AutoClearEnmityInterval * 1000 || !GameState.IsForeground))
				{
					isAlreadyRequest = true;
					ResetAllStrikingDummies();
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
	}
}
