using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class UseActionManager : OmenServiceBase<UseActionManager>
{
	public delegate void PreUseActionDelegate(ref bool isPrevented, ref ActionType actionType, ref uint actionID, ref ulong targetID, ref uint extraParam, ref UseActionMode queueState, ref uint comboRouteID);

	public delegate void PostUseActionDelegate(bool result, ActionType actionType, uint actionID, ulong targetID, uint extraParam, UseActionMode queueState, uint comboRouteID);

	public delegate void PreUseActionLocationDelegate(ref bool isPrevented, ref ActionType type, ref uint actionID, ref ulong targetID, ref Vector3 location, ref uint extraParam, ref byte a7);

	public delegate void PostUseActionLocationDelegate(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7);

	public delegate void PreIsActionOffCooldownDelegate(ref bool isPrevented, ActionType actionType, uint actionID, ref float queueTime);

	public delegate void PreCharacterCompleteCastDelegate(ref bool isPrevented, ref IBattleChara player, ref ActionType type, ref uint actionID, ref uint spellID, ref GameObjectId animationTargetID, ref Vector3 location, ref float rotation, ref short lastUsedActionSequence, ref int animationVariation, ref int ballistaEntityID);

	public delegate void PostCharacterCompleteCastDelegate(bool result, IBattleChara player, ActionType type, uint actionID, uint spellID, GameObjectId animationTargetID, Vector3 location, float rotation, short lastUsedActionSequence, int animationVariation, int ballistaEntityID);

	public delegate void PreCharacterStartCastDelegate(ref bool isPrevented, ref IBattleChara player, ref ActionType type, ref uint actionID, ref nint a4, ref float rotation, ref float a6);

	public delegate void PostCharacterStartCastDelegate(bool result, IBattleChara player, ActionType type, uint actionID, nint a4, float rotation, float a6);

	private unsafe delegate bool UseActionDelegate(ActionManager* actionManager, ActionType actionType, uint actionID, ulong targetID, uint extraParam, UseActionMode queueState, uint comboRouteID, bool* outOptAreaTargeted);

	private unsafe delegate bool UseActionLocationDelegate(ActionManager* manager, ActionType type, uint actionID, ulong targetID, Vector3* location, uint extraParam, byte a7);

	private unsafe delegate bool IsActionOffCooldownDelegate(ActionManager* manager, ActionType actionType, uint actionID);

	private unsafe delegate void* CharacterCompleteCastDelegate(BattleChara* player, uint type, uint actionID, uint spellID, GameObjectId animationTargetID, Vector3* location, float rotation, short lastUsedActionSequence, int animationVariation, int ballistaEntityID);

	private unsafe delegate void* CharacterStartCastDelegate(BattleChara* player, ActionType type, uint actionID, nint a4, float rotation, float a6);

	private unsafe delegate bool CanPlayerUseActionDelegate(ActionManager* manager, uint actionID);

	public class UseActionManagerConfig : OmenServiceConfig
	{
		public bool ShowUseActionLog;

		public bool ShowUseActionLocationLog;

		public bool ShowCharacterCompleteCastLog;

		public bool ShowCharacterStartCastLog;

		public void Save()
		{
			this.Save(DService.Instance().GetOmenService<UseActionManager>());
		}
	}

	private Hook<UseActionDelegate>? UseActionHook;

	private Hook<UseActionLocationDelegate>? UseActionLocationHook;

	private Hook<IsActionOffCooldownDelegate>? IsActionOffCooldownHook;

	private static readonly CompSig CharacterCompleteCastSig = new CompSig("E8 ?? ?? ?? ?? 41 0F B6 56 ?? 44 0F 28 8C 24 ?? ?? ?? ??");

	private Hook<CharacterCompleteCastDelegate>? CharacterCompleteCastHook;

	private static readonly CompSig CharacterStartCastSig = new CompSig("E8 ?? ?? ?? ?? 80 7E 22 11");

	private Hook<CharacterStartCastDelegate>? CharacterStartCastHook;

	private static readonly CompSig CanPlayerUseActionSig = new CompSig("48 89 5C 24 ?? 57 48 83 EC ?? 8B DA 48 8D 0D");

	private readonly CanPlayerUseActionDelegate? CanPlayerUseAction = CanPlayerUseActionSig.GetDelegate<CanPlayerUseActionDelegate>();

	private readonly ConcurrentDictionary<Type, ImmutableList<Delegate>> methodsCollection = new ConcurrentDictionary<Type, ImmutableList<Delegate>>();

	public UseActionManagerConfig Config { get; private set; }

	protected unsafe override void Init()
	{
		Config = LoadConfig<UseActionManagerConfig>() ?? new UseActionManagerConfig();
		if (UseActionHook == null)
		{
			UseActionHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<UseActionDelegate>(typeof(MemberFunctionPointers), "UseAction", UseActionDetour);
		}
		if (UseActionLocationHook == null)
		{
			UseActionLocationHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<UseActionLocationDelegate>(typeof(MemberFunctionPointers), "UseActionLocation", UseActionLocationDetour);
		}
		if (IsActionOffCooldownHook == null)
		{
			IsActionOffCooldownHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<IsActionOffCooldownDelegate>(typeof(MemberFunctionPointers), "IsActionOffCooldown", IsActionOffCooldownDetour);
		}
		if (CharacterCompleteCastHook == null)
		{
			CharacterCompleteCastHook = CharacterCompleteCastSig.GetHook<CharacterCompleteCastDelegate>(CharacterCompleteCastDetour);
		}
		if (CharacterStartCastHook == null)
		{
			CharacterStartCastHook = CharacterStartCastSig.GetHook<CharacterStartCastDelegate>(CharacterStartCastDetour);
		}
		UseActionHook?.Enable();
		UseActionLocationHook?.Enable();
		IsActionOffCooldownHook?.Enable();
		CharacterCompleteCastHook?.Enable();
		CharacterStartCastHook?.Enable();
	}

	private bool RegisterGeneric<T>(T method, params T[] methods) where T : Delegate
	{
		Type typeFromHandle = typeof(T);
		methodsCollection.AddOrUpdate(typeFromHandle, delegate
		{
			ImmutableList<Delegate> immutableList = ImmutableList.Create((Delegate)method);
			return (methods.Length == 0) ? immutableList : immutableList.AddRange(methods);
		}, delegate(Type _, ImmutableList<Delegate> currentList)
		{
			ImmutableList<Delegate> immutableList = currentList.Add(method);
			return (methods.Length == 0) ? immutableList : immutableList.AddRange(methods);
		});
		return true;
	}

	private bool UnregisterGeneric<T>(params T[] methods) where T : Delegate
	{
		if (methods == null || methods.Length <= 0)
		{
			return false;
		}
		Type typeFromHandle = typeof(T);
		ImmutableList<Delegate> value;
		while (methodsCollection.TryGetValue(typeFromHandle, out value))
		{
			ImmutableList<Delegate> immutableList = value.RemoveRange(methods);
			if (immutableList == value)
			{
				return false;
			}
			if (immutableList.IsEmpty)
			{
				KeyValuePair<Type, ImmutableList<Delegate>> item = new KeyValuePair<Type, ImmutableList<Delegate>>(typeFromHandle, value);
				if (((ICollection<KeyValuePair<Type, ImmutableList<Delegate>>>)methodsCollection).Remove(item))
				{
					return true;
				}
			}
			else if (methodsCollection.TryUpdate(typeFromHandle, immutableList, value))
			{
				return true;
			}
		}
		return false;
	}

	public bool RegPreUseAction(PreUseActionDelegate method, params PreUseActionDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostUseAction(PostUseActionDelegate method, params PostUseActionDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreUseActionLocation(PreUseActionLocationDelegate method, params PreUseActionLocationDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostUseActionLocation(PostUseActionLocationDelegate method, params PostUseActionLocationDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreIsActionOffCooldown(PreIsActionOffCooldownDelegate method, params PreIsActionOffCooldownDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreCharacterCompleteCast(PreCharacterCompleteCastDelegate method, params PreCharacterCompleteCastDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostCharacterCompleteCast(PostCharacterCompleteCastDelegate method, params PostCharacterCompleteCastDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreCharacterStartCast(PreCharacterStartCastDelegate method, params PreCharacterStartCastDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostCharacterStartCast(PostCharacterStartCastDelegate method, params PostCharacterStartCastDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool Unreg(params PreUseActionDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostUseActionDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreUseActionLocationDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostUseActionLocationDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreIsActionOffCooldownDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreCharacterCompleteCastDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostCharacterCompleteCastDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreCharacterStartCastDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostCharacterStartCastDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	private unsafe bool UseActionDetour(ActionManager* actionManager, ActionType actionType, uint actionID, ulong targetID, uint extraParam, UseActionMode queueState, uint comboRouteID, bool* outOptAreaTargeted)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		if (Config.ShowUseActionLog)
		{
			DLog.Debug($"[Use Action Manager] 一般类技能\n类型:{actionType} | ID:{actionID} | 目标ID: {targetID}\n额外参数: {extraParam} | 队列状态: {queueState} | 连击路径ID: {comboRouteID}");
		}
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreUseActionDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreUseActionDelegate item in value)
			{
				item(ref isPrevented, ref actionType, ref actionID, ref targetID, ref extraParam, ref queueState, ref comboRouteID);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		bool result = UseActionHook.Original(actionManager, actionType, actionID, targetID, extraParam, queueState, comboRouteID, outOptAreaTargeted);
		if (methodsCollection.TryGetValue(typeof(PostUseActionDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostUseActionDelegate item2 in value2)
			{
				item2(result, actionType, actionID, targetID, extraParam, queueState, comboRouteID);
			}
		}
		return result;
	}

	private unsafe bool UseActionLocationDetour(ActionManager* manager, ActionType type, uint actionID, ulong targetID, Vector3* location, uint extraParam, byte a7)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		Vector3 location2 = ((location != null) ? (*location) : Vector3.Zero);
		if (Config.ShowUseActionLocationLog)
		{
			DLog.Debug($"[Use Action Manager] 地面类技能\n类型:{type} | ID:{actionID} | 目标ID: {targetID} | 地点:{location2} | 额外参数:{extraParam}");
		}
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreUseActionLocationDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreUseActionLocationDelegate item in value)
			{
				item(ref isPrevented, ref type, ref actionID, ref targetID, ref location2, ref extraParam, ref a7);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		bool result = UseActionLocationHook.Original(manager, type, actionID, targetID, &location2, extraParam, a7);
		if (methodsCollection.TryGetValue(typeof(PostUseActionLocationDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostUseActionLocationDelegate item2 in value2)
			{
				item2(result, type, actionID, targetID, location2, extraParam, a7);
			}
		}
		return result;
	}

	private unsafe bool IsActionOffCooldownDetour(ActionManager* manager, ActionType actionType, uint actionID)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected I4, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (manager == null)
		{
			return false;
		}
		bool isPrevented = false;
		float queueTime = 0.5f;
		if (methodsCollection.TryGetValue(typeof(PreIsActionOffCooldownDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreIsActionOffCooldownDelegate item in value)
			{
				item(ref isPrevented, actionType, actionID, ref queueTime);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		RecastDetail* recastGroupDetail = ((ActionManager)manager).GetRecastGroupDetail(((ActionManager)manager).GetRecastGroup((int)actionType, actionID));
		if (recastGroupDetail == null)
		{
			return false;
		}
		int additionalRecastGroup = ((ActionManager)manager).GetAdditionalRecastGroup(actionType, actionID);
		RecastDetail* recastGroupDetail2 = ((ActionManager)manager).GetRecastGroupDetail(additionalRecastGroup);
		if (recastGroupDetail2 != null && ((RecastDetail)recastGroupDetail2).IsActive && ((RecastDetail)recastGroupDetail2).Total - ((RecastDetail)recastGroupDetail2).Elapsed > queueTime)
		{
			return false;
		}
		ushort maxCharges = ActionManager.GetMaxCharges(ActionManager.GetSpellIdForAction(actionType, actionID), 100u);
		bool num = CanPlayerUseAction(manager, ((RecastDetail)recastGroupDetail).ActionId);
		float total = ((RecastDetail)recastGroupDetail).Total;
		float elapsed = ((RecastDetail)recastGroupDetail).Elapsed;
		if (num)
		{
			return !(total / (float)(int)maxCharges - elapsed > queueTime);
		}
		return !(total - elapsed > queueTime);
	}

	private unsafe void* CharacterCompleteCastDetour(BattleChara* player, uint type, uint actionID, uint spellID, GameObjectId animationTargetID, Vector3* locationPtr, float rotation, short lastUsedActionSequence, int animationVariation, int ballistaEntityID)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected I4, but got Unknown
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		ActionType type2 = (ActionType)type;
		IBattleChara player2 = IBattleChara.Create((nint)player);
		Vector3 location = ((locationPtr != null) ? (*locationPtr) : Vector3.Zero);
		if (Config.ShowCharacterCompleteCastLog)
		{
			DLog.Debug($"[Use Action Manager] 技能释放完成\n对象: {player2.Name} ({player2.GameObjectID}) | 类型: {type} | 技能 ID: {actionID} | 施法 ID: {spellID}\n动画目标 ID: {GameObjectId.op_Implicit(animationTargetID)} | 地点: {location} | 面向: {rotation}\n技能序列: {lastUsedActionSequence} | 动画变化: {animationVariation} | 弩炮实体 ID: {ballistaEntityID}");
		}
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreCharacterCompleteCastDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreCharacterCompleteCastDelegate item in value)
			{
				item(ref isPrevented, ref player2, ref type2, ref actionID, ref spellID, ref animationTargetID, ref location, ref rotation, ref lastUsedActionSequence, ref animationVariation, ref ballistaEntityID);
				if (isPrevented)
				{
					return null;
				}
			}
		}
		void* ptr = CharacterCompleteCastHook.Original(player2.ToStruct(), (uint)(int)type2, actionID, spellID, animationTargetID, &location, rotation, lastUsedActionSequence, animationVariation, ballistaEntityID);
		if (methodsCollection.TryGetValue(typeof(PostCharacterCompleteCastDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostCharacterCompleteCastDelegate item2 in value2)
			{
				item2(ptr != null, player2, type2, actionID, spellID, animationTargetID, location, rotation, lastUsedActionSequence, animationVariation, ballistaEntityID);
			}
		}
		return ptr;
	}

	private unsafe void* CharacterStartCastDetour(BattleChara* player, ActionType type, uint actionID, nint a4, float rotation, float a6)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		IBattleChara player2 = IBattleChara.Create((nint)player);
		if (Config.ShowCharacterStartCastLog)
		{
			DLog.Debug($"[Use Action Manager] 技能开始释放\n对象: {player2.Name} ({player2.GameObjectID}) | 类型: {type} | ID: {actionID} | 面向: {rotation}\nP4: {a4} | P6: {a6}");
		}
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreCharacterStartCastDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreCharacterStartCastDelegate item in value)
			{
				item(ref isPrevented, ref player2, ref type, ref actionID, ref a4, ref rotation, ref a6);
				if (isPrevented)
				{
					return null;
				}
			}
		}
		void* ptr = CharacterStartCastHook.Original(player2.ToStruct(), type, actionID, a4, rotation, a6);
		if (methodsCollection.TryGetValue(typeof(PostCharacterStartCastDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostCharacterStartCastDelegate item2 in value2)
			{
				item2(ptr != null, player2, type, actionID, a4, rotation, a6);
			}
		}
		return ptr;
	}

	public unsafe bool UseAction(ActionType type, uint actionID, ulong targetID = 3758096384uL, uint extraParam = 0u, UseActionMode queueState = (UseActionMode)0, uint comboRouteID = 0u)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return UseActionHook.Original(ActionManager.Instance(), type, actionID, targetID, extraParam, queueState, comboRouteID, null);
	}

	public unsafe bool UseActionLocation(ActionType type, uint actionID, ulong targetID = 3758096384uL, Vector3 location = default(Vector3), uint extraParam = 0u, byte a7 = 0)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return UseActionLocationHook.Original(ActionManager.Instance(), type, actionID, targetID, &location, extraParam, a7);
	}

	public unsafe bool IsActionOffCooldown(ActionType actionType, uint actionID)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return IsActionOffCooldownHook.Original(ActionManager.Instance(), actionType, actionID);
	}

	protected override void Uninit()
	{
		UseActionHook?.Dispose();
		UseActionHook = null;
		UseActionLocationHook?.Dispose();
		UseActionLocationHook = null;
		IsActionOffCooldownHook?.Dispose();
		IsActionOffCooldownHook = null;
		CharacterCompleteCastHook?.Dispose();
		CharacterCompleteCastHook = null;
		CharacterStartCastHook?.Dispose();
		CharacterStartCastHook = null;
		methodsCollection.Clear();
	}
}
