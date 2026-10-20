using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace OmenTools.OmenService;

public class AchievementManager : OmenServiceBase<AchievementManager>
{
	private unsafe delegate void ReceiveAchievementProgressDelegate(Achievement* achievement, uint id, uint current, uint max);

	private Hook<ReceiveAchievementProgressDelegate>? ReceiveAchievementProgressHook;

	public Dictionary<uint, AchievementInfo> Infos { get; private set; } = new Dictionary<uint, AchievementInfo>();

	public unsafe bool TryGetAchievement(uint id, [NotNullWhen(true)] out AchievementInfo? achievementInfo)
	{
		achievementInfo = null;
		if (!GameState.IsLoggedIn)
		{
			return false;
		}
		if (!LuminaGetter.TryGetRow<Achievement>(id, out Achievement _))
		{
			return false;
		}
		if (((Achievement)(&((UIState)UIState.Instance()).Achievement)).IsComplete((int)id) && !Infos.TryAdd(id, new AchievementInfo(id, 1u, 1u)))
		{
			AchievementInfo achievementInfo2 = Infos[id];
			if (!achievementInfo2.IsFinished)
			{
				Infos[id] = new AchievementInfo(id, achievementInfo2.Max, achievementInfo2.Max);
			}
		}
		SendRequest(id);
		if (!Infos.TryGetValue(id, out AchievementInfo value))
		{
			return false;
		}
		achievementInfo = value;
		return true;
	}

	protected unsafe override void Init()
	{
		OmenServiceBase<GameState>.Instance().Logout += OnLogout;
		if (ReceiveAchievementProgressHook == null)
		{
			ReceiveAchievementProgressHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<ReceiveAchievementProgressDelegate>(typeof(MemberFunctionPointers), "ReceiveAchievementProgress", ReceiveAchievementProgressDetour);
		}
		ReceiveAchievementProgressHook.Enable();
	}

	protected override void Uninit()
	{
		OmenServiceBase<GameState>.Instance().Logout -= OnLogout;
		ReceiveAchievementProgressHook?.Dispose();
		ReceiveAchievementProgressHook = null;
		Infos = new Dictionary<uint, AchievementInfo>();
	}

	private void OnLogout()
	{
		Infos.Clear();
	}

	private static void SendRequest(uint id)
	{
		if (Throttler.Shared.Throttle($"AchievementManager-Request-{id}", 10000u))
		{
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestAchievement, id);
		}
	}

	private unsafe void ReceiveAchievementProgressDetour(Achievement* achievement, uint id, uint current, uint max)
	{
		ReceiveAchievementProgressHook.Original(achievement, id, current, max);
		Infos[id] = new AchievementInfo(id, current, max);
	}
}
