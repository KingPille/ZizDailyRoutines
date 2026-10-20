using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine.Layer;
using OmenTools.OmenService;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 256)]
public struct PopRangeManager
{
	[FieldOffset(0)]
	public int State;

	[FieldOffset(4)]
	public float Time;

	[FieldOffset(16)]
	public unsafe ExitRangeLayoutInstance* ExitLayoutInstance;

	[FieldOffset(32)]
	public Vector3 Position;

	[FieldOffset(48)]
	public Vector3 RecoveredPosition;

	private static readonly CompSig PopRangManagerSig = new CompSig("83 3D ?? ?? ?? ?? ?? 77 ??");

	private unsafe static PopRangeManager* PopRangeManagerPtr;

	public unsafe static PopRangeManager* Instance()
	{
		if (PopRangeManagerPtr != null)
		{
			return PopRangeManagerPtr;
		}
		PopRangeManagerPtr = PopRangManagerSig.GetStatic<PopRangeManager>();
		return PopRangeManagerPtr;
	}

	public unsafe void PopRange(ILayoutInstance* exit, Vector3? recoveredPosition = null)
	{
		PopRangeLayoutInstance* returnInstance = ((ExitRangeLayoutInstance*)exit)->ReturnInstance;
		if (!recoveredPosition.HasValue)
		{
			Position = LocalPlayerState.Object?.Position ?? default(Vector3);
			RecoveredPosition = *((TriggerBoxLayoutInstance)(&returnInstance->Base)).GetTranslationImpl();
		}
		else
		{
			Position = new Vector3(float.MaxValue);
			RecoveredPosition = recoveredPosition.Value;
		}
		ExitLayoutInstance = (ExitRangeLayoutInstance*)exit;
		State = 2;
	}
}
