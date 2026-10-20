using System.Numerics;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;
using OmenTools.Extensions;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class PlaceFurnitureAnywhere : ModuleBase
{
	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe delegate bool RaycastFilterDelegate(BGCollisionModule* module, RaycastHit* hitInfo, Vector3* origin, Vector3* direction, float maxDistance, int layerMask, int* flags);

	private static readonly CompSig RaycastFilterSig = new CompSig("48 8B C4 48 89 58 ?? 48 89 70 ?? 57 48 81 EC ?? ?? ?? ?? 33 DB 48 8B F2");

	private Hook<RaycastFilterDelegate>? RaycastFilterHook;

	private MemoryPatch? patch0;

	private MemoryPatch? patch1;

	private MemoryPatch? patch2;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("PlaceFurnitureAnywhereTitle"),
		Description = LanguageManager.Get("PlaceFurnitureAnywhereDescription"),
		Category = ModuleCategory.System
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		nint address = (nint)DalamudServiceExtension.Instance<ISigScanner>().ScanText("C6 83 ?? ?? ?? ?? ?? 0F 29 44 24") + 6;
		patch0 = new MemoryPatch(address, new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)1));
		patch0.Enable();
		nint address2 = (nint)DalamudServiceExtension.Instance<ISigScanner>().ScanText("48 85 C0 74 ?? C6 87 ?? ?? 00 00 00") + 11;
		patch1 = new MemoryPatch(address2, new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)1));
		patch1.Enable();
		nint address3 = (nint)DalamudServiceExtension.Instance<ISigScanner>().ScanText("C6 87 83 01 00 00 00 48 83 C4 ??") + 6;
		patch2 = new MemoryPatch(address3, new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)1));
		patch2.Enable();
		if (RaycastFilterHook == null)
		{
			RaycastFilterHook = RaycastFilterSig.GetHook<RaycastFilterDelegate>(RaycastFilterDetour);
		}
		RaycastFilterHook.Enable();
	}

	private unsafe bool RaycastFilterDetour(BGCollisionModule* module, RaycastHit* hitInfo, Vector3* origin, Vector3* direction, float maxDistance, int layerMask, int* flags)
	{
		if (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)67])
		{
			return RaycastFilterHook.Original(module, hitInfo, origin, direction, maxDistance, layerMask, flags);
		}
		return false;
	}
}
