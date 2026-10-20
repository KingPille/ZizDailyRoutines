using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Dalamud;
using OmenTools.Dalamud.DataShare.Attributes;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

internal class AtkEventManager : OmenServiceBase<AtkEventManager>
{
	private readonly ConcurrentDictionary<uint, byte> claimedParamKeys = DalamudServiceExtension.Instance().GetOrCreateData<ConcurrentDictionary<uint, byte>>("OmenTools.OmenService.AtkEventManager.ParamKey", (Func<ConcurrentDictionary<uint, byte>>)(() => new ConcurrentDictionary<uint, byte>()));

	private Hook<ReceiveGlobalEvent>? ReceiveGlobalEventHook;

	private readonly ConcurrentDictionary<uint, AtkEventWrapper> eventHandlers = new ConcurrentDictionary<uint, AtkEventWrapper>();

	[DataShareTag]
	private const string PARAM_KEY_TAG = "OmenTools.OmenService.AtkEventManager.ParamKey";

	private const uint PARAM_KEY_BASE = 953790464u;

	private const uint PARAM_KEY_SPACE_SIZE = 1000000u;

	private const int PARAM_KEY_MAX_ATTEMPTS = 64;

	protected unsafe override void Init()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		ReceiveGlobalEventHook = (*AtkUnitBase.StaticVirtualTablePointer).HookVFuncFromName<AtkUnitBaseVirtualTable, ReceiveGlobalEvent>("ReceiveGlobalEvent", new ReceiveGlobalEvent(ReceiveGlobalEventDetour));
		ReceiveGlobalEventHook.Enable();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, new AddonEventDelegate(OnAddonPreFinalize));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonPreFinalize });
		ReceiveGlobalEventHook?.Disable();
		foreach (KeyValuePair<uint, AtkEventWrapper> eventHandler in eventHandlers)
		{
			eventHandler.Deconstruct(out var _, out var value);
			value.Dispose();
		}
		eventHandlers.Clear();
		ReceiveGlobalEventHook?.Dispose();
	}

	internal uint RegisterEvent(AtkEventWrapper eventWrapper)
	{
		uint num = ClaimParamKey();
		if (!eventHandlers.TryAdd(num, eventWrapper))
		{
			claimedParamKeys.TryRemove(num, out var _);
			throw new Exception($"注册事件失败: {num}");
		}
		return num;
	}

	internal void UnregisterEvent(uint paramKey)
	{
		if (eventHandlers.TryRemove(paramKey, out AtkEventWrapper _))
		{
			claimedParamKeys.TryRemove(paramKey, out var _);
		}
	}

	private uint ClaimParamKey()
	{
		for (int i = 0; i < 64; i++)
		{
			uint num = (uint)(953790464 + Random.Shared.Next(1000000));
			if (claimedParamKeys.TryAdd(num, 0))
			{
				return num;
			}
		}
		throw new Exception($"获取 AtkEvent 参数 Key 失败: {953790464u:X} ~ {954790464u:X}");
	}

	private void OnAddonPreFinalize(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		nint address = args.Addon.Address;
		foreach (var (_, atkEventWrapper2) in eventHandlers)
		{
			if (atkEventWrapper2.IsBoundToAddon(address))
			{
				atkEventWrapper2.Dispose();
			}
		}
	}

	private unsafe void ReceiveGlobalEventDetour(AtkUnitBase* addon, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* data)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (addon == null || atkEvent == null || data == null)
		{
			return;
		}
		if (eventHandlers.TryGetValue((uint)eventParam, out AtkEventWrapper value))
		{
			try
			{
				value.Action(eventType, addon, atkEvent, data);
				((AtkEvent)atkEvent).SetEventIsHandled(false);
				return;
			}
			catch (Exception ex)
			{
				DLog.Error($"尝试触发自定义 AtkEvent 时发生错误, ID: {eventParam}", ex);
			}
		}
		Hook<ReceiveGlobalEvent>? receiveGlobalEventHook = ReceiveGlobalEventHook;
		if (receiveGlobalEventHook != null)
		{
			receiveGlobalEventHook.OriginalDisposeSafe.Invoke(addon, eventType, eventParam, atkEvent, data);
		}
	}
}
