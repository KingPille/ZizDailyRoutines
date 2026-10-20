using System.Runtime.InteropServices;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 704)]
public struct CameraExtra
{
	[FieldOffset(320)]
	public float DirH;

	[FieldOffset(324)]
	public float DirV;

	[FieldOffset(328)]
	public float InputDeltaHAdjusted;

	[FieldOffset(332)]
	public float InputDeltaVAdjusted;

	[FieldOffset(336)]
	public float InputDeltaH;

	[FieldOffset(340)]
	public float InputDeltaV;

	[FieldOffset(344)]
	public float DirVMin;

	[FieldOffset(348)]
	public float DirVMax;
}
