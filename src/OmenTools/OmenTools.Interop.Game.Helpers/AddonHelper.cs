using System;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;

namespace OmenTools.Interop.Game.Helpers;

public static class AddonHelper
{
	public static bool TryGetPtrByName(string addonName, out nint addonPtr)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		addonPtr = DalamudServiceExtension.Instance<IGameGui>().GetAddonByName(addonName, 1).Address;
		return addonPtr != IntPtr.Zero;
	}

	public unsafe static bool TryGetByName(string addonName, out AtkUnitBase* addonPtr)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		nint address = DalamudServiceExtension.Instance<IGameGui>().GetAddonByName(addonName, 1).Address;
		if (address == IntPtr.Zero)
		{
			addonPtr = null;
			return false;
		}
		addonPtr = (AtkUnitBase*)address;
		return true;
	}

	public unsafe static bool TryGetByName<T>(string addonName, out T* addonPtr) where T : unmanaged
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		nint address = DalamudServiceExtension.Instance<IGameGui>().GetAddonByName(addonName, 1).Address;
		if (address == IntPtr.Zero)
		{
			addonPtr = null;
			return false;
		}
		addonPtr = (T*)address;
		return true;
	}

	public unsafe static T* GetByName<T>(string addonName) where T : unmanaged
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		nint address = DalamudServiceExtension.Instance<IGameGui>().GetAddonByName(addonName, 1).Address;
		if (address == IntPtr.Zero)
		{
			return null;
		}
		return (T*)address;
	}

	public unsafe static AtkUnitBase* GetByName(string name)
	{
		return AddonHelper.GetByName<AtkUnitBase>(name);
	}
}
