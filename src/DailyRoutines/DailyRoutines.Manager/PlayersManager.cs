using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Info.Models;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Gui.PartyFinder.Types;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.Interop;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;
using nXyRYE1uBjK6gmU4ln;

namespace DailyRoutines.Manager;

[ManagerInitOrder(2)]
public class PlayersManager : ManagerBase<PlayersManager>
{
	public delegate void PlayerInfoReceiveDelegate(PlayerInfo data);

	public delegate void PlayersAroundUpdateDelegate(IReadOnlyList<IPlayerCharacter> characters);

	public delegate void PlayersTargetingMeUpdateDelegate(IReadOnlyList<PlayerTargetingInfo> targetingPlayersInfo);

	private unsafe delegate void Eeq0rVOCQH4QKVyf3OD(InfoProxyCommonList* infoProxy, nint a2, uint a3);

	private static readonly CompSig InfoProxyAddDataSig;

	private Hook<Eeq0rVOCQH4QKVyf3OD>? InfoProxyAddDataHook;

	private readonly List<IPlayerCharacter> playersAroundInternal;

	private readonly List<PlayerTargetingInfo> playersTargetingMeInternal;

	private readonly List<IPlayerCharacter> tempTargetingPlayers;

	private HashSet<uint> currentTargetingEntityIDs;

	private HashSet<uint> lastTargetingEntityIDs;

	private readonly Dictionary<uint, DateTime> targetingStartTimes;

	private readonly List<uint> entityIDsToRemove;

	public IReadOnlyList<IPlayerCharacter> PlayersAround => playersAroundInternal;

	public int PlayersAroundCount => playersAroundInternal.Count;

	public IReadOnlyList<PlayerTargetingInfo> PlayersTargetingMe => playersTargetingMeInternal;

	private static bool IsEnvironmentInvalid
	{
		get
		{
			if (GameState.IsLoggedIn)
			{
				ICondition obj = DalamudServiceExtension.Instance<ICondition>();
				ConditionFlag[] array = new ConditionFlag[3];
				RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				return obj.Any((ConditionFlag[])(object)array);
			}
			return true;
		}
	}

	public event PlayerInfoReceiveDelegate? ReceivePlayerInfo;

	public event PlayersAroundUpdateDelegate? ReceivePlayersAround;

	public event PlayersTargetingMeUpdateDelegate? ReceivePlayersTargetingMe;

	protected unsafe override Task Init()
	{
		if (InfoProxyAddDataHook == null)
		{
			InfoProxyAddDataHook = InfoProxyAddDataSig.GetHook<Eeq0rVOCQH4QKVyf3OD>(zYpHYHUPKd);
		}
		return Task.CompletedTask;
	}

	protected override Task PostInit()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		InfoProxyAddDataHook.Enable();
		DalamudServiceExtension.Instance<IPartyFinderGui>().ReceiveListing += new PartyFinderListingEventDelegate(nOUHXL8UWn);
		OmenServiceBase<GameState>.Instance().Logout += hRTSyQ44aF;
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(SoRSLjANmH), 1000u);
		return Task.CompletedTask;
	}

	protected override Task Uninit()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		OmenServiceBase<GameState>.Instance().Logout -= hRTSyQ44aF;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(SoRSLjANmH);
		InfoProxyAddDataHook?.Dispose();
		InfoProxyAddDataHook = null;
		DalamudServiceExtension.Instance<IPartyFinderGui>().ReceiveListing -= new PartyFinderListingEventDelegate(nOUHXL8UWn);
		playersAroundInternal.Clear();
		playersTargetingMeInternal.Clear();
		currentTargetingEntityIDs.Clear();
		lastTargetingEntityIDs.Clear();
		targetingStartTimes.Clear();
		tempTargetingPlayers.Clear();
		entityIDsToRemove.Clear();
		return Task.CompletedTask;
	}

	private void nOUHXL8UWn(IPartyFinderListing listing, IPartyFinderListingEventArgs args)
	{
		this.ReceivePlayerInfo?.Invoke(new PlayerInfo(listing));
	}

	private unsafe void zYpHYHUPKd(InfoProxyCommonList* infoProxy, nint a2, uint a3)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		InfoProxyAddDataHook.Original(infoProxy, a2, a3);
		if (infoProxy == null || ((InfoProxyCommonList)infoProxy).CharDataSpan.IsEmpty)
		{
			return;
		}
		ReadOnlySpan<CharacterData> charDataSpan = ((InfoProxyCommonList)infoProxy).CharDataSpan;
		for (int i = 0; i < charDataSpan.Length; i++)
		{
			CharacterData val = charDataSpan[i];
			if (val.ContentId != 0L && !string.IsNullOrWhiteSpace(((CharacterData)(ref val)).NameString) && val.HomeWorld != 0)
			{
				this.ReceivePlayerInfo?.Invoke(new PlayerInfo(val));
			}
		}
	}

	private unsafe void CgKHzWXxCe(IFramework _)
	{
		if (ManagerBase<ModuleManager>.Instance().IsBusy)
		{
			return;
		}
		if (IsEnvironmentInvalid)
		{
			if (playersAroundInternal.Count > 0)
			{
				playersAroundInternal.Clear();
				this.ReceivePlayersAround?.Invoke(playersAroundInternal);
			}
			return;
		}
		playersAroundInternal.Clear();
		Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)CharacterManager.Instance()).BattleCharas;
		for (int i = 1; i < 100; i++)
		{
			BattleChara* value = battleCharas[i].Value;
			if (value != null && c9GSHXihQW(value))
			{
				playersAroundInternal.Add(IPlayerCharacter.Create((nint)value));
			}
		}
		this.ReceivePlayersAround?.Invoke(playersAroundInternal);
	}

	private void sAJSne4Gl4(IFramework _)
	{
		if (playersAroundInternal.Count == 0)
		{
			if (lastTargetingEntityIDs.Count > 0)
			{
				lastTargetingEntityIDs.Clear();
				targetingStartTimes.Clear();
				playersTargetingMeInternal.Clear();
				this.ReceivePlayersTargetingMe?.Invoke(playersTargetingMeInternal);
			}
		}
		else
		{
			if (IsEnvironmentInvalid)
			{
				return;
			}
			tempTargetingPlayers.Clear();
			currentTargetingEntityIDs.Clear();
			foreach (IPlayerCharacter item in playersAroundInternal)
			{
				if (item.TargetObjectID == LocalPlayerState.EntityID)
				{
					tempTargetingPlayers.Add(item);
					currentTargetingEntityIDs.Add(item.EntityID);
				}
			}
			DateTime now = OmenServiceBase<StandardTimeManager>.Instance().Now;
			foreach (uint currentTargetingEntityID in currentTargetingEntityIDs)
			{
				bool exists;
				ref DateTime valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(targetingStartTimes, currentTargetingEntityID, out exists);
				if (!exists)
				{
					valueRefOrAddDefault = now;
				}
			}
			if (targetingStartTimes.Count > currentTargetingEntityIDs.Count)
			{
				entityIDsToRemove.Clear();
				foreach (KeyValuePair<uint, DateTime> targetingStartTime in targetingStartTimes)
				{
					if (!currentTargetingEntityIDs.Contains(targetingStartTime.Key))
					{
						entityIDsToRemove.Add(targetingStartTime.Key);
					}
				}
				foreach (uint item2 in entityIDsToRemove)
				{
					targetingStartTimes.Remove(item2);
				}
			}
			if (tempTargetingPlayers.Count > 1)
			{
				CollectionsMarshal.AsSpan(tempTargetingPlayers).Sort((IPlayerCharacter a, IPlayerCharacter b) => a.EntityID.CompareTo(b.EntityID));
			}
			playersTargetingMeInternal.Clear();
			Span<IPlayerCharacter> span = CollectionsMarshal.AsSpan(tempTargetingPlayers);
			for (int num = 0; num < span.Length; num++)
			{
				IPlayerCharacter playerCharacter = span[num];
				DateTime dateTime = targetingStartTimes[playerCharacter.EntityID];
				int targetingDurationSeconds = (int)(now - dateTime).TotalSeconds;
				bool isNew = !lastTargetingEntityIDs.Contains(playerCharacter.EntityID);
				playersTargetingMeInternal.Add(new PlayerTargetingInfo(playerCharacter, dateTime, targetingDurationSeconds, isNew));
			}
			HashSet<uint> hashSet = currentTargetingEntityIDs;
			HashSet<uint> hashSet2 = lastTargetingEntityIDs;
			lastTargetingEntityIDs = hashSet;
			currentTargetingEntityIDs = hashSet2;
			this.ReceivePlayersTargetingMe?.Invoke(playersTargetingMeInternal);
		}
	}

	private void hRTSyQ44aF()
	{
		playersAroundInternal.Clear();
		playersTargetingMeInternal.Clear();
		tempTargetingPlayers.Clear();
		currentTargetingEntityIDs.Clear();
		lastTargetingEntityIDs.Clear();
		targetingStartTimes.Clear();
		entityIDsToRemove.Clear();
		this.ReceivePlayersAround?.Invoke(playersAroundInternal);
		this.ReceivePlayersTargetingMe?.Invoke(playersTargetingMeInternal);
	}

	private void SoRSLjANmH(IFramework framework)
	{
		if (!Y5SWncTBsGUDX5l2wY.IsUninitialized)
		{
			CgKHzWXxCe(framework);
			sAJSne4Gl4(framework);
		}
	}

	private unsafe static bool c9GSHXihQW(BattleChara* obj)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		if ((int)((BattleChara)obj).ObjectKind == 1 && ((BattleChara)obj).EntityId != LocalPlayerState.EntityID && ((BattleChara)obj).ClassJob != 0 && ((BattleChara)obj).NameId == 0 && ((BattleChara)obj).ContentId != 0L && ((BattleChara)obj).AccountId != 0L && ((BattleChara)obj).HomeWorld != ushort.MaxValue)
		{
			return ((BattleChara)obj).CurrentWorld != ushort.MaxValue;
		}
		return false;
	}

	public PlayersManager()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		playersAroundInternal = new List<IPlayerCharacter>(100);
		playersTargetingMeInternal = new List<PlayerTargetingInfo>(20);
		tempTargetingPlayers = new List<IPlayerCharacter>(20);
		currentTargetingEntityIDs = new HashSet<uint>(20);
		lastTargetingEntityIDs = new HashSet<uint>(20);
		targetingStartTimes = new Dictionary<uint, DateTime>(20);
		entityIDsToRemove = new List<uint>(20);
		base._002Ector();
	}

	static PlayersManager()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		InfoProxyAddDataSig = new CompSig(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7C6394 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
	}
}
