using System;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoCancelMountCast : ModuleBase
{
	private class Config : ModuleConfig
	{
		public bool CancelWhenJump;

		public bool CancelWhenMove;

		public bool CancelWhenUsection = true;
	}

	private Config config;

	private CancellationTokenSource? cancelSource;

	private bool isOnMountCasting;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoCancelMountCastTitle"),
		Description = LanguageManager.Get("AutoCancelMountCastDescription"),
		Category = ModuleCategory.Action,
		Author = new string[1] { "Bill" },
		ModulesPair = new string[1] { "BetterMountRoulette" }
	};

	protected override void Init()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		OmenServiceBase<UseActionManager>.Instance().RegPreUseAction(OnPreUseAction);
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPreUseAction);
		OnConditionChanged((ConditionFlag)27, value: false);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoCancelMountCast-CancelWhenUseAction")), ref config.CancelWhenUsection))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoCancelMountCast-CancelWhenMove")), ref config.CancelWhenMove))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoCancelMountCast-CancelWhenJump")), ref config.CancelWhenJump))
		{
			config.Save(this);
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Invalid comparison between Unknown and I4
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		if ((int)flag != 27)
		{
			if ((int)flag == 48 && config.CancelWhenJump && value)
			{
				ExecuteCancelCast();
			}
		}
		else if (value)
		{
			IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer == null || ((int)localPlayer.CastActionType != 13 && (localPlayer == null || (int)localPlayer.CastActionType != 5 || localPlayer.CastActionID != 9)))
			{
				return;
			}
			isOnMountCasting = true;
			cancelSource = new CancellationTokenSource();
			IFramework obj = DalamudServiceExtension.Instance<IFramework>();
			Func<Task> func = async delegate
			{
				while (config.CancelWhenMove && isOnMountCasting && !cancelSource.IsCancellationRequested)
				{
					if (OmenServiceBase<LocalPlayerState>.Instance().IsMoving)
					{
						ExecuteCancelCast();
					}
					await Task.Delay(10, cancelSource.Token);
				}
			};
			CancellationToken token = cancelSource.Token;
			obj.RunOnTick(func, default(TimeSpan), 0, token).ContinueWith(delegate(Task t)
			{
				t.Dispose();
			});
		}
		else
		{
			isOnMountCasting = false;
			cancelSource?.Cancel();
			cancelSource?.Dispose();
			cancelSource = null;
		}
	}

	private void OnPreUseAction(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID)
	{
		if (config.CancelWhenUsection && isOnMountCasting)
		{
			ExecuteCancelCast();
		}
	}

	private static void ExecuteCancelCast()
	{
		if (Throttler.Shared.Throttle("CancelMountCast-CancelCast", 100u))
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CancelCast);
		}
	}
}
