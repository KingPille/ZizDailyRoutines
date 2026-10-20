using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.Interop.Game.Models.Native;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class LocalPlayerState : OmenServiceBase<LocalPlayerState>
{
	private unsafe delegate bool AccountInfoUpdateDelegate(AccountInfo* accountInfo, ulong accountID, byte forceReload);

	private delegate nint AccountInfoCacheLoadDelegate(ulong accountID, nint arg2, nint data, nuint dataSize, byte arg5, byte arg6);

	private delegate bool IsLocalPlayerInPartyDelegate();

	private delegate bool IsLocalPlayerPartyLeaderDelegate();

	private static readonly CompSig AccountInfoUpdateSig = new CompSig("40 56 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 8B 01");

	private Hook<AccountInfoUpdateDelegate>? AccountInfoUpdateHook;

	private static readonly CompSig AccountInfoCacheLoadSig = new CompSig("4C 8B DC 55 41 56 49 8D 6B ?? 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 45 ?? 48 8B 05");

	private Hook<AccountInfoCacheLoadDelegate>? AccountInfoCacheLoadHook;

	private static readonly IsLocalPlayerInPartyDelegate IsLocalPlayerInParty = new CompSig("E8 ?? ?? ?? ?? 84 C0 74 25 48 8B 8D ?? ?? ?? ??").GetDelegate<IsLocalPlayerInPartyDelegate>();

	private static readonly IsLocalPlayerPartyLeaderDelegate IsLocalPlayerPartyLeader = new CompSig("48 83 EC ?? E8 ?? ?? ?? ?? 84 C0 74 ?? 48 83 C4").GetDelegate<IsLocalPlayerPartyLeaderDelegate>();

	private Hook<AgentUpdateDelegate>? AgentMapUpdateHook;

	private ulong lastAccountID;

	private static bool IsOnClassJobChanging;

	public unsafe static GrandCompany GrandCompany => (GrandCompany)((PlayerState)PlayerState.Instance()).GrandCompany;

	public bool IsMoving { get; private set; }

	public static bool IsInAnyParty
	{
		get
		{
			if (!InfoProxyCrossRealm.IsCrossRealmParty())
			{
				return DalamudServiceExtension.Instance<IPartyList>().Length >= 2;
			}
			return true;
		}
	}

	public unsafe static bool IsWalking => ((Control)Control.Instance()).IsWalking;

	public unsafe static byte Sex => ((PlayerState)PlayerState.Instance()).Sex;

	public unsafe static string Name => ((PlayerState)PlayerState.Instance()).CharacterNameString;

	public unsafe static uint EntityID => ((PlayerState)PlayerState.Instance()).EntityId;

	public unsafe static ulong AccountID => AccountInfo.Instance()->AccountID;

	public static ClassJob ClassJobData => LuminaGetter.GetRow<ClassJob>(ClassJob).GetValueOrDefault();

	public unsafe static uint ClassJob => ((AgentHUD)AgentHUD.Instance()).CharacterClassJobId;

	public unsafe static ulong ContentID => ((PlayerState)PlayerState.Instance()).ContentId;

	public unsafe static ushort CurrentLevel => (ushort)((PlayerState)PlayerState.Instance()).CurrentLevel;

	public unsafe static ushort MaxLevel => ((PlayerState)PlayerState.Instance()).MaxLevel;

	public unsafe static ushort Commendations => (ushort)((PlayerState)PlayerState.Instance()).PlayerCommendations;

	public unsafe static bool IsLevelSynced => ((PlayerState)PlayerState.Instance()).IsLevelSynced;

	public static bool IsInParty => IsLocalPlayerInParty();

	public static bool IsPartyLeader => IsLocalPlayerPartyLeader();

	public static IPlayerCharacter? Object => IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;

	public event Action<bool>? PlayerMoveStateChanged;

	public event Action<ulong>? AccountIDChanged;

	protected unsafe override void Init()
	{
		if (AgentMapUpdateHook == null)
		{
			AgentMapUpdateHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<AgentUpdateDelegate>((IntPtr)(*((AgentMap)AgentMap.Instance()).VirtualTable).GetVFuncByName<AgentMapVirtualTable>("Update"), (AgentUpdateDelegate)AgentMapUpdateDetour, (HookBackend)0);
		}
		AgentMapUpdateHook.Enable();
		if (AccountInfoUpdateHook == null)
		{
			AccountInfoUpdateHook = AccountInfoUpdateSig.GetHook<AccountInfoUpdateDelegate>(AccountInfoUpdateDetour);
		}
		AccountInfoUpdateHook.Enable();
		if (AccountInfoCacheLoadHook == null)
		{
			AccountInfoCacheLoadHook = AccountInfoCacheLoadSig.GetHook<AccountInfoCacheLoadDelegate>(AccountInfoCacheLoadDetour);
		}
		AccountInfoCacheLoadHook.Enable();
	}

	protected override void Uninit()
	{
		AgentMapUpdateHook?.Dispose();
		AgentMapUpdateHook = null;
		AccountInfoUpdateHook?.Dispose();
		AccountInfoUpdateHook = null;
		AccountInfoCacheLoadHook?.Dispose();
		AccountInfoCacheLoadHook = null;
	}

	private unsafe void AgentMapUpdateDetour(AgentInterface* agent, uint frameCount)
	{
		AgentMapUpdateHook.Original(agent, frameCount);
		bool isPlayerMoving = ((AgentMap)agent).IsPlayerMoving;
		if (IsMoving != isPlayerMoving)
		{
			this.PlayerMoveStateChanged?.Invoke(isPlayerMoving);
		}
		IsMoving = isPlayerMoving;
	}

	private unsafe bool AccountInfoUpdateDetour(AccountInfo* accountInfo, ulong accountID, byte forceReload)
	{
		bool result = AccountInfoUpdateHook.Original(accountInfo, accountID, forceReload);
		NotifyAccountIDChanged();
		return result;
	}

	private nint AccountInfoCacheLoadDetour(ulong accountID, nint arg2, nint data, nuint dataSize, byte arg5, byte arg6)
	{
		nint result = AccountInfoCacheLoadHook.Original(accountID, arg2, data, dataSize, arg5, arg6);
		NotifyAccountIDChanged();
		return result;
	}

	private unsafe void NotifyAccountIDChanged()
	{
		AccountInfo* ptr = AccountInfo.Instance();
		if (ptr != null)
		{
			ulong accountID = ptr->AccountID;
			if (Interlocked.Exchange(ref lastAccountID, accountID) != accountID)
			{
				this.AccountIDChanged?.Invoke(accountID);
			}
		}
	}

	public unsafe static ushort GetClassJobLevel(uint classJobID, bool shouldGetSynced = true)
	{
		if (ClassJob != classJobID)
		{
			return ((PlayerState)PlayerState.Instance()).GetClassJobLevel((int)classJobID, shouldGetSynced);
		}
		return CurrentLevel;
	}

	public unsafe static bool TryFindClassJobGearset(uint classJobID, out byte gearsetID)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		gearsetID = 0;
		RaptureGearsetModule* ptr = RaptureGearsetModule.Instance();
		for (int i = 0; i < 100; i++)
		{
			GearsetEntry* gearset = ((RaptureGearsetModule)ptr).GetGearset(i);
			if (gearset != null && ((Enum)((GearsetEntry)gearset).Flags).HasFlag((Enum)(object)(GearsetFlag)1) && !((Enum)((GearsetEntry)gearset).Flags).HasFlag((Enum)(object)(GearsetFlag)4) && ((GearsetEntry)gearset).Id == i && ((GearsetEntry)gearset).ClassJob == classJobID)
			{
				gearsetID = ((GearsetEntry)gearset).Id;
				return true;
			}
		}
		return false;
	}

	public unsafe static bool TryFindClassJobGearsetData(uint classJobID, out GearsetEntry gearsetData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		gearsetData = default(GearsetEntry);
		RaptureGearsetModule* ptr = RaptureGearsetModule.Instance();
		for (int i = 0; i < 100; i++)
		{
			GearsetEntry* gearset = ((RaptureGearsetModule)ptr).GetGearset(i);
			if (gearset != null && ((Enum)((GearsetEntry)gearset).Flags).HasFlag((Enum)(object)(GearsetFlag)1) && !((Enum)((GearsetEntry)gearset).Flags).HasFlag((Enum)(object)(GearsetFlag)4) && ((GearsetEntry)gearset).Id == i && ((GearsetEntry)gearset).ClassJob == classJobID)
			{
				gearsetData = *gearset;
				return true;
			}
		}
		return false;
	}

	public unsafe static bool SwitchGearset(uint classJob)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<ClassJob>(classJob, out ClassJob item))
		{
			return false;
		}
		if (Object == null)
		{
			return false;
		}
		if (TryFindClassJobGearset(classJob, out var gearsetID))
		{
			OmenServiceBase<ChatManager>.Instance().SendMessage($"/gearset change {gearsetID + 1}");
			return true;
		}
		if (((IEnumerable<InventoryType>)Inventories.PlayerWithArmory).TryGetFirstItem((Predicate<InventoryItem>)delegate(InventoryItem x)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			if (LuminaGetter.TryGetRow<Item>(((InventoryItem)(ref x)).GetBaseItemId(), out Item item2) && ((Item)(ref item2)).ClassJobCategory.Value.IsClassJobIn(classJob) && ((Item)(ref item2)).LevelEquip <= GetClassJobLevel(classJob))
			{
				RowRef<EquipSlotCategory> equipSlotCategory = ((Item)(ref item2)).EquipSlotCategory;
				if (equipSlotCategory.IsValid)
				{
					EquipSlotCategory value = equipSlotCategory.Value;
					return ((EquipSlotCategory)(ref value)).MainHand == 1;
				}
				return false;
			}
			return false;
		}, out InventoryItem* itemResult))
		{
			((InventoryManager)InventoryManager.Instance()).MoveItemSlot(((InventoryItem)itemResult).GetInventoryType(), ((InventoryItem)itemResult).GetSlot(), (InventoryType)1000, (ushort)0, true);
			if (((ClassJob)(ref item)).DohDolJobIndex > -1 && ((IEnumerable<InventoryType>)Inventories.PlayerWithArmory).TryGetFirstItem((Predicate<InventoryItem>)delegate(InventoryItem x)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_005c: Unknown result type (might be due to invalid IL or missing references)
				if (LuminaGetter.TryGetRow<Item>(((InventoryItem)(ref x)).GetBaseItemId(), out Item item2) && ((Item)(ref item2)).ClassJobCategory.Value.IsClassJobIn(classJob) && ((Item)(ref item2)).LevelEquip <= GetClassJobLevel(classJob))
				{
					RowRef<EquipSlotCategory> equipSlotCategory = ((Item)(ref item2)).EquipSlotCategory;
					if (equipSlotCategory.IsValid)
					{
						EquipSlotCategory value = equipSlotCategory.Value;
						return ((EquipSlotCategory)(ref value)).OffHand == 1;
					}
					return false;
				}
				return false;
			}, out InventoryItem* offHandItem))
			{
				Task.Run(delegate
				{
					//IL_0055: Unknown result type (might be due to invalid IL or missing references)
					if (IsOnClassJobChanging)
					{
						return;
					}
					IsOnClassJobChanging = true;
					try
					{
						long num = Environment.TickCount64 + 2000;
						while (ClassJob != classJob || Environment.TickCount64 > num)
						{
							Thread.Sleep(100);
						}
						if (offHandItem != null)
						{
							((InventoryManager)InventoryManager.Instance()).MoveItemSlot(((InventoryItem)offHandItem).GetInventoryType(), ((InventoryItem)offHandItem).GetSlot(), (InventoryType)1000, (ushort)1, false);
						}
					}
					finally
					{
						IsOnClassJobChanging = false;
					}
				});
			}
			return true;
		}
		return false;
	}

	public unsafe static bool SwitchGearset(byte gearsetID)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (Object == null)
		{
			return false;
		}
		GearsetEntry* gearset = ((RaptureGearsetModule)RaptureGearsetModule.Instance()).GetGearset((int)gearsetID);
		if (gearset == null || !((Enum)((GearsetEntry)gearset).Flags).HasFlag((Enum)(object)(GearsetFlag)1) || ((Enum)((GearsetEntry)gearset).Flags).HasFlag((Enum)(object)(GearsetFlag)4))
		{
			return false;
		}
		OmenServiceBase<ChatManager>.Instance().SendMessage($"/gearset change {gearsetID + 1}");
		return true;
	}

	public unsafe static bool HasStatus(uint statusID, out int index, uint sourceID = 3758096384u)
	{
		index = -1;
		if (Object == null)
		{
			return false;
		}
		index = ((StatusManager)(&((BattleChara)Object.ToStruct()).StatusManager)).GetStatusIndex(statusID, sourceID);
		return index != -1;
	}

	public unsafe static uint GetItemCount(uint itemID)
	{
		CurrencyManager* ptr = CurrencyManager.Instance();
		if (((CurrencyManager)ptr).HasItem(itemID))
		{
			return ((CurrencyManager)ptr).GetItemCount(itemID);
		}
		InventoryManager* ptr2 = InventoryManager.Instance();
		return (uint)(((InventoryManager)ptr2).GetInventoryItemCount(itemID, false, true, true, (short)0) + ((InventoryManager)ptr2).GetInventoryItemCount(itemID, true, true, true, (short)0));
	}

	public static float DistanceTo3D(Vector3 target)
	{
		if (Object == null)
		{
			return float.MaxValue;
		}
		return Vector3.Distance(Object.Position, target);
	}

	public static float DistanceTo3DSquared(Vector3 target)
	{
		if (Object == null)
		{
			return float.MaxValue;
		}
		return Vector3.DistanceSquared(Object.Position, target);
	}

	public static float DistanceTo2D(Vector2 target)
	{
		if (Object == null)
		{
			return float.MaxValue;
		}
		return Vector2.Distance(Object.Position.ToVector2(), target);
	}

	public static float DistanceTo2DSquared(Vector2 target)
	{
		if (Object == null)
		{
			return float.MaxValue;
		}
		return Vector2.DistanceSquared(Object.Position.ToVector2(), target);
	}

	public static float DistanceToObject2D(IGameObject? target, bool ignoreRadius = true)
	{
		if (target == null || Object == null)
		{
			return float.MaxValue;
		}
		if (!ignoreRadius)
		{
			if (DistanceTo2D(target.Position.ToVector2()) <= target.HitboxRadius)
			{
				return 0f;
			}
			Vector3 vector = GetNearestPointToObject(target);
			if (vector == Vector3.Zero)
			{
				return 0f;
			}
			return DistanceTo2D(vector.ToVector2());
		}
		return DistanceTo2D(target.Position.ToVector2());
	}

	public static float DistanceToObject2DSquared(IGameObject? target, bool ignoreRadius = true)
	{
		if (target == null || Object == null)
		{
			return float.MaxValue;
		}
		if (!ignoreRadius)
		{
			if (DistanceTo2DSquared(target.Position.ToVector2()) <= target.HitboxRadius * target.HitboxRadius)
			{
				return 0f;
			}
			Vector3 vector = GetNearestPointToObject(target);
			if (vector == Vector3.Zero)
			{
				return 0f;
			}
			return DistanceTo2DSquared(vector.ToVector2());
		}
		return DistanceTo2DSquared(target.Position.ToVector2());
	}

	public static float DistanceToObject3D(IGameObject? target, bool ignoreRadius = true)
	{
		if (target == null || Object == null)
		{
			return float.MaxValue;
		}
		if (!ignoreRadius)
		{
			if (DistanceTo2D(target.Position.ToVector2()) <= target.HitboxRadius)
			{
				return 0f;
			}
			Vector3 nearestPointToObject = GetNearestPointToObject(target);
			if (nearestPointToObject == Vector3.Zero)
			{
				return 0f;
			}
			return DistanceTo3D(nearestPointToObject);
		}
		return DistanceTo3D(target.Position);
	}

	public static float DistanceToObject3DSquared(IGameObject? target, bool ignoreRadius = true)
	{
		if (target == null || Object == null)
		{
			return float.MaxValue;
		}
		if (!ignoreRadius)
		{
			if (DistanceTo2DSquared(target.Position.ToVector2()) <= target.HitboxRadius * target.HitboxRadius)
			{
				return 0f;
			}
			Vector3 nearestPointToObject = GetNearestPointToObject(target);
			if (nearestPointToObject == Vector3.Zero)
			{
				return 0f;
			}
			return DistanceTo3DSquared(nearestPointToObject);
		}
		return DistanceTo3DSquared(target.Position);
	}

	public static Vector3 GetNearestPointToObject(IGameObject? target)
	{
		if (target == null || Object == null)
		{
			return Vector3.One;
		}
		if (DistanceTo2D(target.Position.ToVector2()) <= target.HitboxRadius)
		{
			return Object.Position;
		}
		return target.Position + Vector3.Normalize(Object.Position - target.Position) * target.HitboxRadius;
	}
}
