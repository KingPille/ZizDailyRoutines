using System.Numerics;
using System.Runtime.InteropServices;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 320)]
public struct PlayerMoveControllerWalk
{
	private unsafe delegate float GetTargetSpeedPtrDelegate(PlayerMoveControllerWalk* controller);

	private unsafe delegate bool IsAbleToSetMisdirectionStateDelegate(PlayerMoveControllerWalk* controller);

	[FieldOffset(16)]
	public Vector3 MovementDirection;

	[FieldOffset(32)]
	public unsafe PlayerController* Parent;

	[FieldOffset(56)]
	public float SpeedMultiplier;

	[FieldOffset(60)]
	public bool IsAutoRunForward;

	[FieldOffset(61)]
	public bool IsAutoRunStrafe;

	[FieldOffset(62)]
	public bool IsCameraRelativeMode;

	[FieldOffset(63)]
	private bool UnknownFlag63;

	[FieldOffset(64)]
	public bool IsJumping;

	[FieldOffset(68)]
	public float CurrentSpeed;

	[FieldOffset(88)]
	public float BaseMovementSpeed;

	[FieldOffset(136)]
	public bool IsMovementInput;

	[FieldOffset(140)]
	private float Unknown140;

	[FieldOffset(144)]
	public float MovementDirRelToCharacterFacing;

	[FieldOffset(148)]
	public byte IsForced;

	[FieldOffset(160)]
	public Vector3 MovementDirectionWorld;

	[FieldOffset(176)]
	public float RotationDirection;

	[FieldOffset(184)]
	private unsafe void* InternalVtable1;

	[FieldOffset(192)]
	private unsafe void* InternalVtable2;

	[FieldOffset(200)]
	public unsafe fixed float SentinelFloat[5];

	[FieldOffset(264)]
	private unsafe void* InternalVtable3;

	[FieldOffset(272)]
	public uint MovementState;

	[FieldOffset(276)]
	public float MovementLeft;

	[FieldOffset(280)]
	public float MovementForward;

	[FieldOffset(292)]
	public bool IsMovementInputLocked;

	[FieldOffset(298)]
	public bool CameraTiltToggle;

	private static readonly GetTargetSpeedPtrDelegate GetTargetSpeedPtr = new CompSig("E8 ?? ?? ?? ?? 0F 28 F0 E9 ?? ?? ?? ?? 8B 81").GetDelegate<GetTargetSpeedPtrDelegate>();

	private static readonly IsAbleToSetMisdirectionStateDelegate IsAbleToSetMisdirectionStatePtr = new CompSig("40 53 48 83 EC 20 48 8B D9 48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 75 78").GetDelegate<IsAbleToSetMisdirectionStateDelegate>();

	private static readonly CompSig InstanceSig = new CompSig("48 8D 0D ?? ?? ?? ?? 45 0F 28 D4");

	private unsafe static PlayerMoveControllerWalk* instance;

	public unsafe float GetTargetSpeed()
	{
		fixed (PlayerMoveControllerWalk* controller = &this)
		{
			return GetTargetSpeedPtr(controller);
		}
	}

	public unsafe bool IsAbleToSetMisdirectionState()
	{
		fixed (PlayerMoveControllerWalk* controller = &this)
		{
			return IsAbleToSetMisdirectionStatePtr(controller);
		}
	}

	public unsafe static PlayerMoveControllerWalk* Instance()
	{
		if (instance == null)
		{
			instance = InstanceSig.GetStatic<PlayerMoveControllerWalk>();
		}
		return instance;
	}
}
