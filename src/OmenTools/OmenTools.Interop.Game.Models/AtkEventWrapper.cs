using System;
using System.Collections.Generic;
using System.Threading;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.Models;

public class AtkEventWrapper : IDisposable
{
	public unsafe delegate void AtkEventActionDelegate(AtkEventType eventType, AtkUnitBase* addon, AtkEvent* atkEvent, AtkEventData* data);

	private readonly List<(nint AddonPtr, nint NodePtr, AtkEventType Type)> registeredData = new List<(nint, nint, AtkEventType)>();

	private readonly Lock dataLock = new Lock();

	private bool isDisposed;

	public AtkEventActionDelegate Action { get; }

	public uint ParamKey { get; }

	public AtkEventWrapper(AtkEventActionDelegate action)
	{
		Action = action;
		ParamKey = OmenServiceBase<AtkEventManager>.Instance().RegisterEvent(this);
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			RemoveRegisteredEvents();
			OmenServiceBase<AtkEventManager>.Instance().UnregisterEvent(ParamKey);
			GC.SuppressFinalize(this);
		}
	}

	public unsafe void Add(AtkUnitBase* addon, AtkResNode* node, AtkEventType eventType)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		using (dataLock.EnterScope())
		{
			if (!isDisposed)
			{
				((AtkResNode)node).AddEvent(eventType, ParamKey, (AtkEventListener*)addon, node, true);
				registeredData.Add(((nint)addon, (nint)node, eventType));
			}
		}
	}

	public unsafe void Remove(AtkUnitBase* addon, AtkResNode* node, AtkEventType eventType)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		using (dataLock.EnterScope())
		{
			if (!isDisposed)
			{
				((AtkResNode)node).RemoveEvent(eventType, ParamKey, (AtkEventListener*)addon, true);
				registeredData.Remove(((nint)addon, (nint)node, eventType));
			}
		}
	}

	internal bool IsBoundToAddon(nint addonPtr)
	{
		using (dataLock.EnterScope())
		{
			foreach (var registeredDatum in registeredData)
			{
				if (registeredDatum.AddonPtr == addonPtr)
				{
					return true;
				}
			}
			return false;
		}
	}

	private unsafe void RemoveRegisteredEvents()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		using (dataLock.EnterScope())
		{
			foreach (var registeredDatum in registeredData)
			{
				nint item = registeredDatum.AddonPtr;
				nint item2 = registeredDatum.NodePtr;
				AtkEventType item3 = registeredDatum.Type;
				AtkUnitBase* ptr = (AtkUnitBase*)item;
				AtkResNode* ptr2 = (AtkResNode*)item2;
				if (ptr != null && ptr2 != null && ((AtkUnitBase)ptr).IsFullyLoaded())
				{
					((AtkResNode)ptr2).RemoveEvent(item3, ParamKey, (AtkEventListener*)ptr, true);
				}
			}
			registeredData.Clear();
		}
	}
}
