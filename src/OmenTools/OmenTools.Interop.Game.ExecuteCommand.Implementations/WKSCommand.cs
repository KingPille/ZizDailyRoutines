using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class WKSCommand : ExecuteCommandBase
{
	public enum CurrencyKind : uint
	{
		LunarCredit,
		PhaennaCredit,
		OizysCredit
	}

	public enum WheelKind : uint
	{
		Left,
		Right
	}

	public static void ChangeMode(uint modeIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetWKSMode, modeIndex);
	}

	public static void StartMission(uint missionUnitID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AcceptWKSMission, missionUnitID);
	}

	public static void CompleteMission()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishWKSMission);
	}

	public static void AbandonMission()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AbandonWKSMission);
	}

	public static void StartLottery(CurrencyKind currencyKind)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartWKSLottery, (uint)currencyKind);
	}

	public static void ChooseLottery(CurrencyKind currencyKind, WheelKind wheelKind)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ChooseWKSLotteryType, (uint)currencyKind, (uint)wheelKind);
	}

	public static void EndLottery(CurrencyKind currencyKind)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishWKSLottery, (uint)currencyKind);
	}

	public static void RequestMecha(uint wksMechaEventDataRowID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestWKSMecha, wksMechaEventDataRowID);
	}

	public static void RequestInactiveMecha()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestWKSMecha);
	}

	public static void EndInteraction()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishWKSInteraction3401);
	}

	public static void EndInteractionAlternate()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.FinishWKSInteraction3402);
	}

	public static void RequestSuccesses()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestWKSSuccesses);
	}
}
