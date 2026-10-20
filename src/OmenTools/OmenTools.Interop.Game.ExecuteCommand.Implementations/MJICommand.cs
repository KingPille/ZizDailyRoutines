using System;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using FFXIVClientStructs.STD;
using FFXIVClientStructs.STD.Helper;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class MJICommand : ExecuteCommandBase
{
	public enum Mode : uint
	{
		Free = 0u,
		Gather = 1u,
		Sow = 2u,
		Water = 3u,
		Remove = 4u,
		Feed = 6u,
		Pet = 7u,
		Beckon = 8u,
		Capture = 9u
	}

	public static void CaptureMonster(uint animalBaseID, uint animalEntityID, uint currentMode, uint currentModeItem)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CaptureMJIAnimal, animalBaseID, animalEntityID, currentMode, currentModeItem);
	}

	public static void SetMode(Mode mode)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetMJIMode, (uint)mode);
	}

	public static void SetModeParam(uint param)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetMJIModeParam, param);
	}

	public static void ClearModeParam()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetMJIModeParam);
	}

	public static void OpenSettingPanel()
	{
		SetSettingPanel(isOpen: true);
	}

	public static void CloseSettingPanel()
	{
		SetSettingPanel(isOpen: false);
	}

	public static void SetSettingPanel(bool isOpen)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ToggleMJISettingPanel, isOpen ? 1u : 0u);
	}

	public static void RequestWorkshop(uint cycleDay)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestMJIWorkshop, cycleDay);
	}

	public static void RequestWorkshopItem()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestMJIWorkshopAssignment);
	}

	public static void AddWorkshopSchedule(uint startingHour, uint craftObjectID, uint cycleDay)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AssignMJIWorkshop, BuildScheduleParam(startingHour, craftObjectID), cycleDay);
	}

	public static void RemoveWorkshopSchedule(uint startingHour, uint craftObjectID, uint cycleDay)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AssignMJIWorkshop, BuildScheduleParam(startingHour, craftObjectID), cycleDay, 0u, 1u);
	}

	public static void CancelWorkshopSchedule(uint startingHour, uint craftObjectID, uint cycleDay)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CancelMJIWorkshopAssignment, BuildScheduleParam(startingHour, craftObjectID), cycleDay);
	}

	public static void SetRestCycles(uint restDay1, uint restDay2, uint restDay3, uint restDay4)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetMJIWorkshopRest, restDay1, restDay2, restDay3, restDay4);
	}

	public static void CollectGranary(uint granaryIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CollectMJIGranary, granaryIndex);
	}

	public static void ViewGranaryDestinations(uint granaryIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ViewMJIGranaryDestination, granaryIndex);
	}

	public static void AssignGranary(uint granaryIndex, uint destinationIndex, uint explorationDays)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.AssignMJIGranary, granaryIndex, destinationIndex, explorationDays);
	}

	public static void ReleaseMinion(uint minionID, uint areaIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ReleaseMJIMinion, minionID, areaIndex);
	}

	public static void ReleaseAnimal(uint animalIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ReleaseMJIAnimal, animalIndex);
	}

	public static void CollectAnimalLeavings(uint animalIndex, uint collectFlag)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CollectMJIAnimalLeaving, animalIndex, collectFlag);
	}

	public unsafe static void CollectAllAnimalLeavings()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<StdPair<uint, int>, uint, PairKeyExtractor<uint, int>> enumerator = ((MJIPastureHandler)((MJIManager)MJIManager.Instance()).PastureHandler).AvailableMammetLeavings.GetEnumerator();
		try
		{
			uint num = default(uint);
			int num2 = default(int);
			while (enumerator.MoveNext())
			{
				StdPair<uint, int> current = enumerator.Current;
				current.Deconstruct(ref num, ref num2);
				int param = num2;
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CollectMJIAllAnimalLeaving, (uint)param);
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public static void EntrustAnimal(uint animalIndex, uint feedItemID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EntrustMJIAnimal, animalIndex, feedItemID);
	}

	public static void RecallMinion(uint minionIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RecallMJIMinion, minionIndex);
	}

	public static void EntrustFarm(uint farmIndex, uint seedItemID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EntrustMJIFarm, farmIndex, seedItemID);
	}

	public static void DismissFarm(uint farmIndex)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.DismissMJIFarmEntrust, farmIndex);
	}

	public static void CollectFarm(uint farmIndex, bool dismissAfterCollect = false)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CollectMJIFarm, farmIndex, dismissAfterCollect ? 1u : 0u);
	}

	public unsafe static void CollectAllFarms()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.CollectMJIAllFarm, (uint)(*((MJIManager)MJIManager.Instance()).GranariesState));
	}

	public static void RequestFavorState()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RequestMJIWorkshopFavor);
	}

	private static uint BuildScheduleParam(uint startingHour, uint craftObjectID)
	{
		return 8 * (startingHour | (32 * craftObjectID));
	}
}
