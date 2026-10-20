using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public sealed class TargetManager : OmenServiceBase<TargetManager>
{
	public delegate void PreSetHardTargetDelegate(ref bool isPrevented, ref IGameObject? target, ref bool ignoreTargetModes, ref bool a4, ref int a5);

	public delegate void PostSetHardTargetDelegate(bool result, IGameObject? target, bool ignoreTargetModes, bool a4, int a5);

	public delegate void PreSetSoftTargetDelegate(ref bool isPrevented, ref IGameObject? target);

	public delegate void PostSetSoftTargetDelegate(bool result, IGameObject? target);

	public delegate void PreSetFocusTargetDelegate(ref bool isPrevented, ref GameObjectId gameObjectID);

	public delegate void PostSetFocusTargetDelegate(GameObjectId gameObjectID);

	public delegate void PreInteractWithObjectDelegate(ref bool isPrevented, ref IGameObject? target, ref bool checkLoS);

	public delegate void PostInteractWithObjectDelegate(ulong result, IGameObject? target, bool checkLoS);

	public delegate void PreOpenObjectInteractionDelegate(ref bool isPrevented, ref IGameObject? target);

	public delegate void PostOpenObjectInteractionDelegate(IGameObject? target);

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe delegate bool SetHardTargetDelegate(TargetSystem* system, GameObject* target, bool ignoreTargetModes, bool a4, int a5);

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe delegate bool SetSoftTargetDelegate(TargetSystem* system, GameObject* target);

	private unsafe delegate void SetFocusTargetDelegate(TargetSystem* system, GameObjectId gameObjectID);

	private unsafe delegate ulong InteractWithObjectDelegate(TargetSystem* system, GameObject* target, bool checkLoS);

	private unsafe delegate void OpenObjectInteractionDelegate(TargetSystem* system, GameObject* target);

	private unsafe delegate void AgentHUDUpdateTargetInfoDelegate(AgentHUD* agentHUD);

	public class TargetManagerConfig : OmenServiceConfig
	{
		public bool ShowSetHardTargetLog;

		public bool ShowSetSoftTargetLog;

		public bool ShowSetFocusTargetLog;

		public bool ShowInteractWithObjectLog;

		public bool ShowOpenObjectInteractionLog;

		public void Save()
		{
			this.Save(DService.Instance().GetOmenService<TargetManager>());
		}
	}

	private Hook<SetHardTargetDelegate>? SetHardTargetHook;

	private Hook<SetSoftTargetDelegate>? SetSoftTargetHook;

	private Hook<SetFocusTargetDelegate>? SetFocusTargetHook;

	private Hook<InteractWithObjectDelegate>? InteractWithObjectHook;

	private Hook<OpenObjectInteractionDelegate>? OpenObjectInteractionHook;

	private Hook<AgentHUDUpdateTargetInfoDelegate>? AgentHUDUpdateTargetInfoHook;

	private readonly ConcurrentDictionary<Type, ImmutableList<Delegate>> methodsCollection = new ConcurrentDictionary<Type, ImmutableList<Delegate>>();

	public TargetManagerConfig Config { get; private set; }

	public unsafe static IGameObject? Target
	{
		get
		{
			return IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((TargetSystem)ToStruct()).GetHardTarget());
		}
		set
		{
			TargetSystem* intPtr = ToStruct();
			nint? num = value?.Address;
			((TargetSystem)intPtr).SetHardTarget((GameObject*)(num.HasValue ? ((void*)num.GetValueOrDefault()) : null), false, false, 0);
		}
	}

	public unsafe static IGameObject? MouseOverTarget
	{
		get
		{
			return IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((TargetSystem)ToStruct()).MouseOverTarget);
		}
		set
		{
			TargetSystem* intPtr = ToStruct();
			nint? num = value?.Address;
			((TargetSystem)intPtr).MouseOverTarget = (GameObject*)(num.HasValue ? ((void*)num.GetValueOrDefault()) : null);
		}
	}

	public unsafe static IGameObject? FocusTarget
	{
		get
		{
			return IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((TargetSystem)ToStruct()).FocusTarget);
		}
		set
		{
			TargetSystem* intPtr = ToStruct();
			nint? num = value?.Address;
			((TargetSystem)intPtr).FocusTarget = (GameObject*)(num.HasValue ? ((void*)num.GetValueOrDefault()) : null);
		}
	}

	public unsafe static IGameObject? PreviousTarget
	{
		get
		{
			return IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((TargetSystem)ToStruct()).PreviousTarget);
		}
		set
		{
			TargetSystem* intPtr = ToStruct();
			nint? num = value?.Address;
			((TargetSystem)intPtr).PreviousTarget = (GameObject*)(num.HasValue ? ((void*)num.GetValueOrDefault()) : null);
		}
	}

	public unsafe static IGameObject? SoftTarget
	{
		get
		{
			return IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((TargetSystem)ToStruct()).GetSoftTarget());
		}
		set
		{
			TargetSystem* intPtr = ToStruct();
			nint? num = value?.Address;
			((TargetSystem)intPtr).SetSoftTarget((GameObject*)(num.HasValue ? ((void*)num.GetValueOrDefault()) : null));
		}
	}

	public unsafe static IGameObject? GPoseTarget
	{
		get
		{
			return IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((TargetSystem)ToStruct()).GPoseTarget);
		}
		set
		{
			TargetSystem* intPtr = ToStruct();
			nint? num = value?.Address;
			((TargetSystem)intPtr).GPoseTarget = (GameObject*)(num.HasValue ? ((void*)num.GetValueOrDefault()) : null);
		}
	}

	public unsafe static IGameObject? MouseOverNameplateTarget
	{
		get
		{
			return IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((TargetSystem)ToStruct()).MouseOverNameplateTarget);
		}
		set
		{
			TargetSystem* intPtr = ToStruct();
			nint? num = value?.Address;
			((TargetSystem)intPtr).MouseOverNameplateTarget = (GameObject*)(num.HasValue ? ((void*)num.GetValueOrDefault()) : null);
		}
	}

	public event Action<IGameObject?>? TargetChanged;

	protected unsafe override void Init()
	{
		Config = LoadConfig<TargetManagerConfig>() ?? new TargetManagerConfig();
		if (SetHardTargetHook == null)
		{
			SetHardTargetHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<SetHardTargetDelegate>(typeof(MemberFunctionPointers), "SetHardTarget", SetHardTargetDetour);
		}
		SetHardTargetHook.Enable();
		if (SetSoftTargetHook == null)
		{
			SetSoftTargetHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<SetSoftTargetDelegate>(typeof(MemberFunctionPointers), "SetSoftTarget", SetSoftTargetDetour);
		}
		SetSoftTargetHook.Enable();
		if (SetFocusTargetHook == null)
		{
			SetFocusTargetHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<SetFocusTargetDelegate>(typeof(MemberFunctionPointers), "SetFocusTargetByObjectId", SetFocusTargetDetour);
		}
		SetFocusTargetHook.Enable();
		if (InteractWithObjectHook == null)
		{
			InteractWithObjectHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<InteractWithObjectDelegate>(typeof(MemberFunctionPointers), "InteractWithObject", InteractWithObjectDetour);
		}
		InteractWithObjectHook.Enable();
		if (OpenObjectInteractionHook == null)
		{
			OpenObjectInteractionHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<OpenObjectInteractionDelegate>(typeof(MemberFunctionPointers), "OpenObjectInteraction", OpenObjectInteractionDetour);
		}
		OpenObjectInteractionHook.Enable();
		if (AgentHUDUpdateTargetInfoHook == null)
		{
			AgentHUDUpdateTargetInfoHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<AgentHUDUpdateTargetInfoDelegate>(typeof(MemberFunctionPointers), "UpdateTargetInfo", AgentHUDUpdateTargetInfoDetour);
		}
		AgentHUDUpdateTargetInfoHook.Enable();
	}

	protected override void Uninit()
	{
		SetHardTargetHook?.Dispose();
		SetHardTargetHook = null;
		SetSoftTargetHook?.Dispose();
		SetSoftTargetHook = null;
		SetFocusTargetHook?.Dispose();
		SetFocusTargetHook = null;
		InteractWithObjectHook?.Dispose();
		InteractWithObjectHook = null;
		OpenObjectInteractionHook?.Dispose();
		OpenObjectInteractionHook = null;
		AgentHUDUpdateTargetInfoHook?.Dispose();
		AgentHUDUpdateTargetInfoHook = null;
		methodsCollection.Clear();
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

	public bool RegPreSetHardTarget(PreSetHardTargetDelegate method, params PreSetHardTargetDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostSetHardTarget(PostSetHardTargetDelegate method, params PostSetHardTargetDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreSetSoftTarget(PreSetSoftTargetDelegate method, params PreSetSoftTargetDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostSetSoftTarget(PostSetSoftTargetDelegate method, params PostSetSoftTargetDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreSetFocusTarget(PreSetFocusTargetDelegate method, params PreSetFocusTargetDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostSetFocusTarget(PostSetFocusTargetDelegate method, params PostSetFocusTargetDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreInteractWithObject(PreInteractWithObjectDelegate method, params PreInteractWithObjectDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostInteractWithObject(PostInteractWithObjectDelegate method, params PostInteractWithObjectDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreOpenObjectInteraction(PreOpenObjectInteractionDelegate method, params PreOpenObjectInteractionDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostOpenObjectInteraction(PostOpenObjectInteractionDelegate method, params PostOpenObjectInteractionDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool Unreg(params PreSetHardTargetDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostSetHardTargetDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreSetSoftTargetDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostSetSoftTargetDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreSetFocusTargetDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostSetFocusTargetDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreInteractWithObjectDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostInteractWithObjectDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreOpenObjectInteractionDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostOpenObjectInteractionDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	private unsafe bool SetHardTargetDetour(TargetSystem* system, GameObject* target, bool ignoreTargetModes, bool a4, int a5)
	{
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)target);
		if (Config.ShowSetHardTargetLog)
		{
			DLog.Debug($"[Target Manager] Set Hard Target\n目标: {gameObject?.Name ?? "[空]"} ({gameObject?.Address:X}) | 忽略目标选择模式: {ignoreTargetModes} | a4: {a4} | a5: {a5}\nGameObjectID: {gameObject?.GameObjectID} | EntityID: {gameObject?.EntityID}");
		}
		bool isPrevented = false;
		IGameObject target2 = gameObject;
		bool ignoreTargetModes2 = ignoreTargetModes;
		bool a6 = a4;
		int a7 = a5;
		if (methodsCollection.TryGetValue(typeof(PreSetHardTargetDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreSetHardTargetDelegate item in value)
			{
				item(ref isPrevented, ref target2, ref ignoreTargetModes2, ref a6, ref a7);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		GameObject* target3 = (GameObject*)(((IntPtr?)(nint)target2?.Address) ?? ((IntPtr)(nint)0));
		bool result = SetHardTargetHook.Original(system, target3, ignoreTargetModes2, a6, a7);
		if (methodsCollection.TryGetValue(typeof(PostSetHardTargetDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostSetHardTargetDelegate item2 in value2)
			{
				item2(result, target2, ignoreTargetModes2, a6, a7);
			}
		}
		return result;
	}

	private unsafe bool SetSoftTargetDetour(TargetSystem* system, GameObject* target)
	{
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)target);
		if (Config.ShowSetSoftTargetLog)
		{
			DLog.Debug($"[Target Manager] Set Soft Target\n目标: {gameObject?.Name ?? "[空]"} ({gameObject?.Address:X})\nGameObjectID: {gameObject?.GameObjectID} | EntityID: {gameObject?.EntityID}");
		}
		bool isPrevented = false;
		IGameObject target2 = gameObject;
		if (methodsCollection.TryGetValue(typeof(PreSetSoftTargetDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreSetSoftTargetDelegate item in value)
			{
				item(ref isPrevented, ref target2);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		GameObject* target3 = (GameObject*)(((IntPtr?)(nint)target2?.Address) ?? ((IntPtr)(nint)0));
		bool result = SetSoftTargetHook.Original(system, target3);
		if (methodsCollection.TryGetValue(typeof(PostSetSoftTargetDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostSetSoftTargetDelegate item2 in value2)
			{
				item2(result, target2);
			}
		}
		return result;
	}

	private unsafe void SetFocusTargetDetour(TargetSystem* system, GameObjectId gameObjectID)
	{
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (Config.ShowSetFocusTargetLog)
		{
			IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByID(GameObjectId.op_Implicit(gameObjectID));
			DLog.Debug($"[Target Manager] Set Focus Target\n目标: {gameObject?.Name ?? "[空]"} ({gameObject?.Address:X}) | GameObjectID: {GameObjectId.op_Implicit(gameObjectID):X}\nGameObjectID: {gameObject?.GameObjectID} | EntityID: {gameObject?.EntityID}");
		}
		bool isPrevented = false;
		GameObjectId gameObjectID2 = gameObjectID;
		if (methodsCollection.TryGetValue(typeof(PreSetFocusTargetDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreSetFocusTargetDelegate item in value)
			{
				item(ref isPrevented, ref gameObjectID2);
				if (isPrevented)
				{
					return;
				}
			}
		}
		SetFocusTargetHook.Original(system, gameObjectID2);
		if (!methodsCollection.TryGetValue(typeof(PostSetFocusTargetDelegate), out ImmutableList<Delegate> value2))
		{
			return;
		}
		foreach (PostSetFocusTargetDelegate item2 in value2)
		{
			item2(gameObjectID2);
		}
	}

	private unsafe ulong InteractWithObjectDetour(TargetSystem* system, GameObject* target, bool checkLoS)
	{
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)target);
		if (Config.ShowInteractWithObjectLog)
		{
			DLog.Debug($"[Target Manager] Interact With Object\n目标: {gameObject?.Name ?? "[空]"} ({gameObject?.Address:X}) | 检查视线范围: {checkLoS}\nGameObjectID: {gameObject?.GameObjectID} | EntityID: {gameObject?.EntityID}");
		}
		bool isPrevented = false;
		IGameObject target2 = gameObject;
		bool checkLoS2 = checkLoS;
		if (methodsCollection.TryGetValue(typeof(PreInteractWithObjectDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreInteractWithObjectDelegate item in value)
			{
				item(ref isPrevented, ref target2, ref checkLoS2);
				if (isPrevented)
				{
					return 0uL;
				}
			}
		}
		GameObject* target3 = (GameObject*)(((IntPtr?)(nint)target2?.Address) ?? ((IntPtr)(nint)0));
		ulong result = InteractWithObjectHook.Original(system, target3, checkLoS2);
		if (methodsCollection.TryGetValue(typeof(PostInteractWithObjectDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostInteractWithObjectDelegate item2 in value2)
			{
				item2(result, target2, checkLoS2);
			}
		}
		return result;
	}

	private unsafe void OpenObjectInteractionDetour(TargetSystem* system, GameObject* target)
	{
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)target);
		if (Config.ShowOpenObjectInteractionLog)
		{
			DLog.Debug($"[Target Manager] Open Object Interaction\n目标: {gameObject?.Name ?? "[空]"} ({gameObject?.Address:X})\nGameObjectID: {gameObject?.GameObjectID} | EntityID: {gameObject?.EntityID}");
		}
		bool isPrevented = false;
		IGameObject target2 = gameObject;
		if (methodsCollection.TryGetValue(typeof(PreOpenObjectInteractionDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreOpenObjectInteractionDelegate item in value)
			{
				item(ref isPrevented, ref target2);
				if (isPrevented)
				{
					return;
				}
			}
		}
		GameObject* target3 = (GameObject*)(((IntPtr?)(nint)target2?.Address) ?? ((IntPtr)(nint)0));
		OpenObjectInteractionHook.Original(system, target3);
		if (!methodsCollection.TryGetValue(typeof(PostOpenObjectInteractionDelegate), out ImmutableList<Delegate> value2))
		{
			return;
		}
		foreach (PostOpenObjectInteractionDelegate item2 in value2)
		{
			item2(target2);
		}
	}

	private unsafe void AgentHUDUpdateTargetInfoDetour(AgentHUD* agentHUD)
	{
		int targetCounter = ((AgentHUD)agentHUD).TargetCounter;
		AgentHUDUpdateTargetInfoHook.Original(agentHUD);
		if (targetCounter > ((AgentHUD)agentHUD).TargetCounter)
		{
			this.TargetChanged?.Invoke(Target);
		}
	}

	public unsafe bool SetHardTarget(IGameObject? target, bool ignoreTargetModes = false, bool a4 = false, int a5 = 0)
	{
		return SetHardTargetHook.Original(TargetSystem.Instance(), (GameObject*)(((IntPtr?)(nint)target?.Address) ?? ((IntPtr)(nint)0)), ignoreTargetModes, a4, a5);
	}

	public unsafe bool SetSoftTarget(IGameObject? target)
	{
		return SetSoftTargetHook.Original(TargetSystem.Instance(), (GameObject*)(((IntPtr?)(nint)target?.Address) ?? ((IntPtr)(nint)0)));
	}

	public unsafe void SetFocusTarget(GameObjectId gameObjectID)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		SetFocusTargetHook.Original(TargetSystem.Instance(), gameObjectID);
	}

	public unsafe ulong InteractWithObject(IGameObject? target, bool checkLoS = true)
	{
		return InteractWithObjectHook.Original(TargetSystem.Instance(), (GameObject*)(((IntPtr?)(nint)target?.Address) ?? ((IntPtr)(nint)0)), checkLoS);
	}

	public unsafe void OpenObjectInteraction(IGameObject? target)
	{
		OpenObjectInteractionHook.Original(TargetSystem.Instance(), (GameObject*)(((IntPtr?)(nint)target?.Address) ?? ((IntPtr)(nint)0)));
	}

	public unsafe static TargetSystem* ToStruct()
	{
		return TargetSystem.Instance();
	}
}
