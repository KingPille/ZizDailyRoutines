using System.Collections.Generic;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.OmenService;

public class InstancesManager : OmenServiceBase<InstancesManager>
{
	private class Config : OmenServiceConfig
	{
		public Dictionary<string, Dictionary<uint, int>> InstancesAmount = new Dictionary<string, Dictionary<uint, int>>();
	}

	private static readonly CompSig InstanceAmountSig = new CompSig("4C 8D 0D ?? ?? ?? ?? 44 0F B7 41");

	private unsafe static int* InstanceAmountPtr = InstanceAmountSig.GetStatic<int>();

	private TaskHelper? TaskHelper;

	private Config ServiceConfig;

	public unsafe static bool IsInstancedArea => ((PublicInstance)(&((UIState)UIState.Instance()).PublicInstance)).IsInstancedArea();

	public unsafe static uint CurrentInstance => ((PublicInstance)(&((UIState)UIState.Instance()).PublicInstance)).InstanceId;

	public unsafe static string CurrentVersion => ((Framework)Framework.Instance()).GameVersionString;

	public unsafe static int InstanceAmount
	{
		get
		{
			if (!IsInstancedArea || InstanceAmountPtr == null)
			{
				return 0;
			}
			return *InstanceAmountPtr;
		}
	}

	public int GetInstancesCount(uint zoneID = 0u)
	{
		if (zoneID == 0)
		{
			zoneID = GameState.TerritoryType;
		}
		if (ServiceConfig.InstancesAmount.TryAdd(CurrentVersion, new Dictionary<uint, int>()))
		{
			ServiceConfig.Save(OmenServiceBase<InstancesManager>.Instance());
		}
		return ServiceConfig.InstancesAmount[CurrentVersion].GetValueOrDefault(zoneID, 0);
	}

	protected override void Init()
	{
		ServiceConfig = OmenServiceConfigExtension.Load<Config>(this) ?? new Config();
		if (TaskHelper == null)
		{
			TaskHelper = new TaskHelper
			{
				TimeoutMS = 15000
			};
		}
		OmenServiceBase<ExecuteCommandManager>.Instance().RegPost(OnPostExecuteCommand);
		OmenServiceBase<GameState>.Instance().Login += OnLogin;
		if (DalamudServiceExtension.Instance<IClientState>().IsLoggedIn)
		{
			EnqueueInstancesCountRetrieve(GameState.TerritoryType);
		}
	}

	protected override void Uninit()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnPostExecuteCommand);
		OmenServiceBase<GameState>.Instance().Login -= OnLogin;
		TaskHelper?.Abort();
		TaskHelper?.Dispose();
		TaskHelper = null;
	}

	private unsafe void OnPostExecuteCommand(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (command == ExecuteCommandFlag.Teleport && LuminaGetter.TryGetRow<Aetheryte>(param1, out Aetheryte item) && ((UIState)UIState.Instance()).IsAetheryteUnlocked(((Aetheryte)(ref item)).RowId))
		{
			EnqueueInstancesCountRetrieve(((Aetheryte)(ref item)).Territory.RowId);
		}
	}

	private void OnLogin()
	{
		EnqueueInstancesCountRetrieve(GameState.TerritoryType);
	}

	private unsafe void EnqueueInstancesCountRetrieve(uint zoneID)
	{
		if (zoneID == 0 || ((GameMain)GameMain.Instance()).CurrentContentFinderConditionId != 0)
		{
			return;
		}
		ServiceConfig.InstancesAmount.TryAdd(CurrentVersion, new Dictionary<uint, int>());
		SaveConfig(ServiceConfig);
		if (ServiceConfig.InstancesAmount[CurrentVersion].ContainsKey(zoneID))
		{
			return;
		}
		TaskHelper.Abort();
		TaskHelper.Enqueue(() => GameState.TerritoryType == zoneID);
		TaskHelper.Enqueue(() => !IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) && IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null && UIModuleExtension.IsScreenReady());
		TaskHelper.Enqueue(delegate
		{
			if (!IsInstancedArea)
			{
				TaskHelper.Abort();
			}
		});
		TaskHelper.Enqueue(delegate
		{
			if ((*EventFramework.Instance()).TryGetAetheryteNearby(out var eventID))
			{
				new EventStartPackt(LocalPlayerState.EntityID, eventID).Send();
			}
			else
			{
				TaskHelper.Abort();
			}
		});
		TaskHelper.Enqueue(delegate
		{
			if (!(*Addons.SelectString).IsAddonAndNodesReady())
			{
				return false;
			}
			int num = *InstanceAmountSig.GetStatic<int>();
			if (num > 12)
			{
				return true;
			}
			ServiceConfig.InstancesAmount[CurrentVersion][zoneID] = num;
			SaveConfig(ServiceConfig);
			((AtkUnitBase)Addons.SelectString).Close(true);
			return true;
		});
	}
}
