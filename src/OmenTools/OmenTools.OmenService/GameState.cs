using System;
using System.Collections.Frozen;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Fate;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Lumina.Data;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace OmenTools.OmenService;

public class GameState : OmenServiceBase<GameState>
{
	private delegate nint FateDirectorSetupDelegate(uint rowID, nint a2, nint a3);

	private static readonly CompSig ContentReplyManagerSig = new CompSig("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 45 33 C0 48 8D 57 ?? 41 8B CE E8 ?? ?? ?? ?? 48 8D 8F");

	private static readonly CompSig ZoneServerIDOffsetSig = new CompSig("0F 11 83 ?? ?? ?? ?? 0F 10 4F ?? 0F 11 8B ?? ?? ?? ?? 0F 10 47 ?? 0F 11 83 ?? ?? ?? ?? 0F 10 4F ?? 0F 11 8B ?? ?? ?? ?? 0F 10 47 ?? 0F 11 83 ?? ?? ?? ?? 0F 10 4F ?? 0F 11 8B ?? ?? ?? ?? 0F 10 47 ?? 0F 11 83 ?? ?? ?? ?? 0F 10 4F");

	private static readonly nint ContentReplyManagerPtr = ContentReplyManagerSig.GetStatic();

	private static readonly nint ZoneServerIDOffset = ZoneServerIDOffsetSig.GetStatic();

	private static readonly CompSig InstanceContentDirectorDutyStartedFlagSig = new CompSig("80 A7 ?? ?? ?? ?? ?? 48 8B CF E8 ?? ?? ?? ?? 0F B7 47");

	private static readonly CompSig InstanceContentDirectorDutyCompletedFlagSig = new CompSig("80 89 ?? ?? ?? ?? ?? 33 D2 48 8B D9");

	private static nint InstanceContentDirectorDutyStartedOffset;

	private static byte InstanceContentDirectorDutyStartedFlag;

	private static nint InstanceContentDirectorDutyCompletedOffset;

	private static byte InstanceContentDirectorDutyCompletedFlag;

	private static readonly CompSig FateDirectorSetupSig = new CompSig("E8 ?? ?? ?? ?? 48 39 37");

	private Hook<FateDirectorSetupDelegate>? FateDirectorSetupHook;

	private Hook<ProcessRequestResult>? ProcessRequestResultHook;

	private Hook<CompleteWarp>? CompleteWarpHook;

	private TaskHelper taskHelper;

	private uint worldID;

	public static float GameUIScale
	{
		get
		{
			uint uInt = DalamudServiceExtension.Instance<IGameConfig>().System.GetUInt("UiHighScale");
			switch (uInt)
			{
			case 0u:
				return 1f;
			case 1u:
				return 1.5f;
			case 2u:
				return 2f;
			case 3u:
				return 3f;
			default:
			{
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(uInt);
				float result = default(float);
				return result;
			}
			}
		}
	}

	public bool IsMarketListingsStuck { get; private set; }

	public static bool IsInPVEActonZone
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (!IsInPVPArea)
			{
				ContentFinderCondition contentFinderConditionData = ContentFinderConditionData;
				if (((ContentFinderCondition)(ref contentFinderConditionData)).RowId != 0)
				{
					FrozenSet<uint> notPVE = ContentTypes.NotPVE;
					contentFinderConditionData = ContentFinderConditionData;
					return !notPVE.Contains(((ContentFinderCondition)(ref contentFinderConditionData)).ContentType.RowId);
				}
				return true;
			}
			return false;
		}
	}

	public unsafe static bool IsForeground => !((Framework)Framework.Instance()).WindowInactive;

	public unsafe static Language ClientLanguage => (Language)(ushort)(((Framework)Framework.Instance()).ClientLanguage + 1);

	public unsafe static string ClientVersion => ((Framework)Framework.Instance()).GameVersionString ?? string.Empty;

	public unsafe static bool IsGL => ((Framework)Framework.Instance()).ClientLanguage < 4;

	public unsafe static bool IsCN
	{
		get
		{
			byte clientLanguage = ((Framework)Framework.Instance()).ClientLanguage;
			if ((uint)(clientLanguage - 4) <= 1u)
			{
				return true;
			}
			return false;
		}
	}

	public unsafe static bool IsTC => ((Framework)Framework.Instance()).ClientLanguage == 7;

	public unsafe static bool IsKR => ((Framework)Framework.Instance()).ClientLanguage == 6;

	public unsafe static bool IsLoggedIn
	{
		get
		{
			AgentLobby* ptr = AgentLobby.Instance();
			if (ptr != null && ((AgentLobby)ptr).IsLoggedIn && ((AgentLobby)ptr).IsLoggedIntoZone && ((AgentLobby)ptr).LobbyUpdateStage == 1)
			{
				return ((AgentLobby)ptr).LobbyUIStage == 1;
			}
			return false;
		}
	}

	public unsafe static bool IsFlagMarkerSet => ((AgentMap)AgentMap.Instance()).FlagMarkerCount > 0;

	public unsafe static Vector2 FlagMarkerPosition
	{
		get
		{
			if (!IsFlagMarkerSet)
			{
				return default(Vector2);
			}
			return new Vector2(((AgentMap)AgentMap.Instance()).FlagMapMarkers[0].XFloat, ((AgentMap)AgentMap.Instance()).FlagMapMarkers[0].YFloat);
		}
	}

	public unsafe static FlagMapMarker FlagMarker
	{
		get
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if (!IsFlagMarkerSet)
			{
				return default(FlagMapMarker);
			}
			return ((AgentMap)AgentMap.Instance()).FlagMapMarkers[0];
		}
	}

	public unsafe static float DeltaTime => ((Framework)Framework.Instance()).FrameDeltaTime;

	public static uint CurrentDataCenter
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			World currentWorldData = CurrentWorldData;
			WorldDCGroupType? valueNullable = ((World)(ref currentWorldData)).DataCenter.ValueNullable;
			if (!valueNullable.HasValue)
			{
				return 0u;
			}
			WorldDCGroupType valueOrDefault = valueNullable.GetValueOrDefault();
			return ((WorldDCGroupType)(ref valueOrDefault)).RowId;
		}
	}

	public static WorldDCGroupType CurrentDataCenterData
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			World currentWorldData = CurrentWorldData;
			return ((World)(ref currentWorldData)).DataCenter.ValueNullable.GetValueOrDefault();
		}
	}

	public static uint HomeDataCenter
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			World homeWorldData = HomeWorldData;
			WorldDCGroupType? valueNullable = ((World)(ref homeWorldData)).DataCenter.ValueNullable;
			if (!valueNullable.HasValue)
			{
				return 0u;
			}
			WorldDCGroupType valueOrDefault = valueNullable.GetValueOrDefault();
			return ((WorldDCGroupType)(ref valueOrDefault)).RowId;
		}
	}

	public static WorldDCGroupType HomeDataCenterData
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			World homeWorldData = HomeWorldData;
			return ((World)(ref homeWorldData)).DataCenter.ValueNullable.GetValueOrDefault();
		}
	}

	public unsafe static uint HomeWorld
	{
		get
		{
			if (!DalamudServiceExtension.Instance<IClientState>().IsLoggedIn)
			{
				return 0u;
			}
			return ((LobbyData)(&((AgentLobby)AgentLobby.Instance()).LobbyData)).HomeWorldId;
		}
	}

	public static World HomeWorldData => LuminaGetter.GetRow<World>(HomeWorld).GetValueOrDefault();

	public unsafe static uint CurrentWorld
	{
		get
		{
			if (!DalamudServiceExtension.Instance<IClientState>().IsLoggedIn)
			{
				return 0u;
			}
			return ((LobbyData)(&((AgentLobby)AgentLobby.Instance()).LobbyData)).CurrentWorldId;
		}
	}

	public static World CurrentWorldData => LuminaGetter.GetRow<World>(CurrentWorld).GetValueOrDefault();

	public unsafe static bool IsChaoticRaidBonusActive
	{
		get
		{
			if (!IsInInstanceArea)
			{
				return ((InstanceContent)(&((UIState)UIState.Instance()).InstanceContent)).IsLimitedTimeBonusActive;
			}
			return false;
		}
	}

	public unsafe static float FrameRate => ((Framework)Framework.Instance()).FrameRate;

	public static DateTime ServerTime => DateTimeOffset.FromUnixTimeSeconds(ServerTimeUnix).LocalDateTime;

	public static long ServerTimeUnix => Framework.GetServerTime();

	public static bool IsInPVPInstance => GameMain.IsInPvPInstance();

	public static bool IsInPVPArea => GameMain.IsInPvPArea();

	public unsafe static bool IsInInstanceArea => ((GameMain)GameMain.Instance()).IsInInstanceArea();

	public static bool IsDutyStarted => IsInstanceContentDirectorFlagSet(InstanceContentDirectorDutyStartedOffset, InstanceContentDirectorDutyStartedFlag);

	public static bool IsDutyCompleted => IsInstanceContentDirectorFlagSet(InstanceContentDirectorDutyCompletedOffset, InstanceContentDirectorDutyCompletedFlag);

	public static bool IsInIdleCam => GameMain.IsInIdleCam();

	public static bool IsTerritoryLoaded => TerritoryLoadState == 2;

	public unsafe static uint TerritoryLoadState => ((GameMain)GameMain.Instance()).TerritoryLoadState;

	public unsafe static uint TerritoryFilterKey => ((GameMain)GameMain.Instance()).CurrentTerritoryFilterKey;

	public unsafe static uint Map
	{
		get
		{
			uint currentMapId = ((AgentMap)AgentMap.Instance()).CurrentMapId;
			if (currentMapId == 0)
			{
				currentMapId = ((GameMain)GameMain.Instance()).CurrentMapId;
			}
			return currentMapId;
		}
	}

	public static Map MapData => LuminaGetter.GetRow<Map>(Map).GetValueOrDefault();

	public unsafe static uint ZoneServerID
	{
		get
		{
			nint num = ContentReplyManagerPtr + ZoneServerIDOffset;
			ushort num2 = *(ushort*)num;
			ushort num3 = *(ushort*)(num + 4);
			return (uint)((num2 << 16) | num3);
		}
	}

	public unsafe static uint TerritoryType => ((AgentMap)AgentMap.Instance()).CurrentTerritoryId;

	public static TerritoryType TerritoryTypeData => LuminaGetter.GetRow<TerritoryType>(TerritoryType).GetValueOrDefault();

	public unsafe static TerritoryIntendedUse TerritoryIntendedUse => ((GameMain)GameMain.Instance()).CurrentTerritoryIntendedUseId;

	public static TerritoryIntendedUse TerritoryIntendedUseData => LuminaGetter.GetRow<TerritoryIntendedUse>((uint)(int)TerritoryIntendedUse).GetValueOrDefault();

	public static bool IsInFieldOperations
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Invalid comparison between Unknown and I4
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Invalid comparison between Unknown and I4
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			TerritoryIntendedUse territoryIntendedUse = TerritoryIntendedUse;
			if ((int)territoryIntendedUse == 41 || (int)territoryIntendedUse == 48 || (int)territoryIntendedUse == 61)
			{
				return true;
			}
			return false;
		}
	}

	public unsafe static uint ContentFinderCondition => ((GameMain)GameMain.Instance()).CurrentContentFinderConditionId;

	public static ContentFinderCondition ContentFinderConditionData => LuminaGetter.GetRow<ContentFinderCondition>(ContentFinderCondition).GetValueOrDefault();

	public static Weather WeatherData => LuminaGetter.GetRow<Weather>(Weather).GetValueOrDefault();

	public unsafe static uint Weather => ((WeatherManager)WeatherManager.Instance()).GetCurrentWeather();

	public event Action<int>? MarketListingsStuck;

	public event Action<uint>? EnterFate;

	public event Action? Login;

	public event Action? Logout;

	public event Action<uint>? WorldChanged;

	public event Action<WarpType>? WarpComplete;

	protected unsafe override void Init()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Expected O, but got Unknown
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		nint num = InstanceContentDirectorDutyStartedFlagSig.ScanText();
		nint num2 = InstanceContentDirectorDutyCompletedFlagSig.ScanText();
		InstanceContentDirectorDutyStartedOffset = Marshal.ReadInt32(num + 2);
		InstanceContentDirectorDutyStartedFlag = (byte)(~Marshal.ReadByte(num + 6));
		InstanceContentDirectorDutyCompletedOffset = Marshal.ReadInt32(num2 + 2);
		InstanceContentDirectorDutyCompletedFlag = Marshal.ReadByte(num2 + 6);
		taskHelper = new TaskHelper
		{
			TimeoutMS = int.MaxValue
		};
		DalamudServiceExtension.Instance<IClientState>().Logout += new LogoutDelegate(OnDalamudLogout);
		if (IsLoggedIn)
		{
			worldID = CurrentWorld;
		}
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate));
		FateDirectorSetupHook = FateDirectorSetupSig.GetHook<FateDirectorSetupDelegate>(FateDirectorSetupDetour);
		FateDirectorSetupHook.Enable();
		ProcessRequestResultHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<ProcessRequestResult>(typeof(MemberFunctionPointers), "ProcessRequestResult", new ProcessRequestResult(ProcessRequestResultDetour));
		ProcessRequestResultHook.Enable();
		CompleteWarpHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<CompleteWarp>(typeof(MemberFunctionPointers), "CompleteWarp", new CompleteWarp(CompleteWarpDetour));
		CompleteWarpHook.Enable();
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		DalamudServiceExtension.Instance<IClientState>().Logout -= new LogoutDelegate(OnDalamudLogout);
		taskHelper.Dispose();
		taskHelper = null;
		FateDirectorSetupHook?.Dispose();
		FateDirectorSetupHook = null;
		ProcessRequestResultHook?.Dispose();
		ProcessRequestResultHook = null;
		CompleteWarpHook?.Dispose();
		CompleteWarpHook = null;
	}

	private unsafe void ProcessRequestResultDetour(InfoProxyItemSearch* info, byte resultCount, int errorCode)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		ProcessRequestResultHook.Original.Invoke(info, resultCount, errorCode);
		if (resultCount == 0 && errorCode > 0 && ContentFinderCondition == 0 && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)69] && ((InfoProxyItemSearch)info).SearchItemId != 0 && LuminaGetter.TryGetRow<Item>(((InfoProxyItemSearch)info).SearchItemId, out Item item) && ((Item)(ref item)).ItemSearchCategory.RowId != 0)
		{
			DLog.Warning($"[GameState] 市场交易板数据请求被服务器拒绝，错误码：0x{errorCode:X}。");
			this.MarketListingsStuck?.Invoke(errorCode);
			IsMarketListingsStuck = true;
		}
		else
		{
			IsMarketListingsStuck = false;
		}
	}

	private void OnUpdate(IFramework framework)
	{
		if (IsLoggedIn && CurrentWorld != 0 && CurrentWorld != worldID)
		{
			worldID = CurrentWorld;
			this.WorldChanged?.Invoke(CurrentWorld);
		}
	}

	private unsafe void CompleteWarpDetour(WarpInfo* instance, int eventParam, int eventID)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		WarpType warpType = ((WarpInfo)instance).WarpType;
		CompleteWarpHook.Original.Invoke(instance, eventParam, eventID);
		if ((int)warpType != 0)
		{
			this.WarpComplete?.Invoke(((WarpInfo)instance).WarpType);
		}
		if ((int)warpType == 17)
		{
			this.Login?.Invoke();
		}
	}

	private void OnDalamudLogout(int type, int code)
	{
		this.Logout?.Invoke();
	}

	private unsafe nint FateDirectorSetupDetour(uint rowID, nint a2, nint a3)
	{
		nint result = FateDirectorSetupHook.Original(rowID, a2, a3);
		if (rowID == 102401 && ((FateManager)FateManager.Instance()).CurrentFate != null)
		{
			Action<uint>? action = this.EnterFate;
			if (action == null)
			{
				return result;
			}
			action(((FateContext)((FateManager)FateManager.Instance()).CurrentFate).FateId);
		}
		return result;
	}

	private unsafe static bool IsInstanceContentDirectorFlagSet(nint flagByteOffset, byte flag)
	{
		EventFramework* ptr = EventFramework.Instance();
		if (ptr == null)
		{
			return false;
		}
		InstanceContentDirector* instanceContentDirector = ((EventFramework)ptr).GetInstanceContentDirector();
		if (instanceContentDirector != null)
		{
			return (((byte*)instanceContentDirector)[flagByteOffset] & flag) != 0;
		}
		return false;
	}
}
