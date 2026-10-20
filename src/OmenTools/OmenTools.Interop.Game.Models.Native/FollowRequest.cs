using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game.Object;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 16)]
public struct FollowRequest
{
	public enum FollowMode
	{
		Cancel = 1,
		GroundFollow,
		FlyFollow,
		Follow,
		SilentFollow
	}

	[FieldOffset(0)]
	public GameObjectId TargetGameObjectID;

	[FieldOffset(8)]
	public FollowMode Mode;

	[FieldOffset(9)]
	public byte SilentFlag;
}
