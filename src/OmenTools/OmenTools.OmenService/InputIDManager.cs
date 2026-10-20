using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.UI;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class InputIDManager : OmenServiceBase<InputIDManager>
{
	public delegate void PrePressedDelegate(ref bool? overrideResult, ref InputId id);

	public delegate void PostPressedDelegate(bool result, InputId id);

	public delegate void PreHeldDelegate(ref bool? overrideResult, ref InputId id);

	public delegate void PostHeldDelegate(bool result, InputId id);

	public delegate void PreDownDelegate(ref bool? overrideResult, ref InputId id);

	public delegate void PostDownDelegate(bool result, InputId id);

	public delegate void PreReleasedDelegate(ref bool? overrideResult, ref InputId id);

	public delegate void PostReleasedDelegate(bool result, InputId id);

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe delegate bool IsInputIDDelegate(InputData* data, InputId id);

	private Hook<IsInputIDDelegate>? IsInputIDPressedHook;

	private Hook<IsInputIDDelegate>? IsInputIDHeldHook;

	private Hook<IsInputIDDelegate>? IsInputIDDownHook;

	private Hook<IsInputIDDelegate>? IsInputIDReleasedHook;

	private static readonly CompSig CrossHotbarSlotPressedSig = new CompSig("E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? 48 8B 03 48 8B CB FF 50 ?? B9");

	private Hook<IsInputIDDelegate>? CrossHotbarSlotPressedHook;

	private readonly ConcurrentDictionary<Type, ImmutableList<Delegate>> methodsCollection = new ConcurrentDictionary<Type, ImmutableList<Delegate>>();

	protected unsafe override void Init()
	{
		if (IsInputIDDownHook == null)
		{
			IsInputIDDownHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<IsInputIDDelegate>(typeof(MemberFunctionPointers), "IsInputIdDown", IsInputIDDownDetour);
		}
		if (IsInputIDHeldHook == null)
		{
			IsInputIDHeldHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<IsInputIDDelegate>(typeof(MemberFunctionPointers), "IsInputIdHeld", IsInputIDHeldDetour);
		}
		if (IsInputIDPressedHook == null)
		{
			IsInputIDPressedHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<IsInputIDDelegate>(typeof(MemberFunctionPointers), "IsInputIdPressed", IsInputIDPressedDetour);
		}
		if (IsInputIDReleasedHook == null)
		{
			IsInputIDReleasedHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<IsInputIDDelegate>(typeof(MemberFunctionPointers), "IsInputIdReleased", IsInputIDReleasedDetour);
		}
		if (CrossHotbarSlotPressedHook == null)
		{
			CrossHotbarSlotPressedHook = CrossHotbarSlotPressedSig.GetHook<IsInputIDDelegate>(IsInputIDCrossHotbarSlotDetour);
		}
		IsInputIDDownHook?.Enable();
		IsInputIDHeldHook?.Enable();
		IsInputIDPressedHook?.Enable();
		IsInputIDReleasedHook?.Enable();
		CrossHotbarSlotPressedHook?.Enable();
	}

	protected override void Uninit()
	{
		IsInputIDPressedHook?.Dispose();
		IsInputIDPressedHook = null;
		IsInputIDHeldHook?.Dispose();
		IsInputIDHeldHook = null;
		IsInputIDDownHook?.Dispose();
		IsInputIDDownHook = null;
		IsInputIDReleasedHook?.Dispose();
		IsInputIDReleasedHook = null;
		CrossHotbarSlotPressedHook?.Dispose();
		CrossHotbarSlotPressedHook = null;
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

	public bool RegPrePressed(PrePressedDelegate method, params PrePressedDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostPressed(PostPressedDelegate method, params PostPressedDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreHeld(PreHeldDelegate method, params PreHeldDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostHeld(PostHeldDelegate method, params PostHeldDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreDown(PreDownDelegate method, params PreDownDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostDown(PostDownDelegate method, params PostDownDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreReleased(PreReleasedDelegate method, params PreReleasedDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostReleased(PostReleasedDelegate method, params PostReleasedDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool UnregPrePressed(params PrePressedDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool UnregPostPressed(params PostPressedDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool UnregPreHeld(params PreHeldDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool UnregPostHeld(params PostHeldDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool UnregPreDown(params PreDownDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool UnregPostDown(params PostDownDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool UnregPreReleased(params PreReleasedDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool UnregPostReleased(params PostReleasedDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsInputIDPressedDetour(InputData* data, InputId id)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		InputId id2 = id;
		return RunDetour(data, ref id2, typeof(PrePressedDelegate), typeof(PostPressedDelegate), IsInputIDPressedHook);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsInputIDHeldDetour(InputData* data, InputId id)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		InputId id2 = id;
		return RunDetour(data, ref id2, typeof(PreHeldDelegate), typeof(PostHeldDelegate), IsInputIDHeldHook);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsInputIDDownDetour(InputData* data, InputId id)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		InputId id2 = id;
		return RunDetour(data, ref id2, typeof(PreDownDelegate), typeof(PostDownDelegate), IsInputIDDownHook);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsInputIDReleasedDetour(InputData* data, InputId id)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		InputId id2 = id;
		return RunDetour(data, ref id2, typeof(PreReleasedDelegate), typeof(PostReleasedDelegate), IsInputIDReleasedHook);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool IsInputIDCrossHotbarSlotDetour(InputData* data, InputId id)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		InputId id2 = id;
		return RunDetour(data, ref id2, typeof(PrePressedDelegate), typeof(PostPressedDelegate), CrossHotbarSlotPressedHook);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool RunDetour(InputData* data, ref InputId id, Type preType, Type postType, Hook<IsInputIDDelegate>? hook)
	{
		bool? overrideResult = null;
		if (methodsCollection.TryGetValue(preType, out ImmutableList<Delegate> value))
		{
			foreach (Delegate item in value)
			{
				InvokePre(preType, item, ref overrideResult, ref id);
				if (overrideResult.HasValue)
				{
					return overrideResult.Value;
				}
			}
		}
		bool result = hook.Original(data, id);
		if (methodsCollection.TryGetValue(postType, out ImmutableList<Delegate> value2))
		{
			foreach (Delegate item2 in value2)
			{
				InvokePost(postType, item2, result, id);
			}
		}
		return result;
	}

	private static void InvokePre(Type preType, Delegate method, ref bool? overrideResult, ref InputId id)
	{
		if (preType == typeof(PrePressedDelegate))
		{
			((PrePressedDelegate)method)(ref overrideResult, ref id);
		}
		else if (preType == typeof(PreHeldDelegate))
		{
			((PreHeldDelegate)method)(ref overrideResult, ref id);
		}
		else if (preType == typeof(PreDownDelegate))
		{
			((PreDownDelegate)method)(ref overrideResult, ref id);
		}
		else
		{
			((PreReleasedDelegate)method)(ref overrideResult, ref id);
		}
	}

	private static void InvokePost(Type postType, Delegate method, bool result, InputId id)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (postType == typeof(PostPressedDelegate))
		{
			((PostPressedDelegate)method)(result, id);
		}
		else if (postType == typeof(PostHeldDelegate))
		{
			((PostHeldDelegate)method)(result, id);
		}
		else if (postType == typeof(PostDownDelegate))
		{
			((PostDownDelegate)method)(result, id);
		}
		else
		{
			((PostReleasedDelegate)method)(result, id);
		}
	}

	public unsafe bool IsInputIDPressed(InputId id)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return IsInputIDPressedHook.Original(ToStruct(), id);
	}

	public unsafe bool IsInputIDHeld(InputId id)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return IsInputIDHeldHook.Original(ToStruct(), id);
	}

	public unsafe bool IsInputIDDown(InputId id)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return IsInputIDDownHook.Original(ToStruct(), id);
	}

	public unsafe bool IsInputIDReleased(InputId id)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return IsInputIDReleasedHook.Original(ToStruct(), id);
	}

	public unsafe static InputData* ToStruct()
	{
		return &((UIInputData)UIInputData.Instance()).InputData;
	}
}
