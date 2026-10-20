using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using OmenTools.Info.Models;
using OmenTools.Interop.Game.Models;

namespace OmenTools.Interop.Game;

public class MovementInputController : IDisposable
{
	private unsafe delegate bool RMIWalkIsInputEnabledDelegate(void* self);

	private unsafe delegate void RMIWalkDelegate(void* self, float* sumLeft, float* sumForward, float* sumTurnLeft, byte* haveBackwardOrStrafe, byte* a6, byte bAdditiveUnk);

	private unsafe delegate void RMIFlyDelegate(void* self, PlayerMoveControllerFlyInput* result);

	[StructLayout(LayoutKind.Explicit, Size = 24)]
	private struct PlayerMoveControllerFlyInput
	{
		[FieldOffset(0)]
		public float Forward;

		[FieldOffset(4)]
		public float Left;

		[FieldOffset(8)]
		public float Up;
	}

	private const float DEFAULT_PRECISION = 0.01f;

	private static readonly RMIWalkIsInputEnabledDelegate rmiWalkIsInputEnabled1 = new CompSig("E8 ?? ?? ?? ?? 84 C0 75 10 38 43 3C").GetDelegate<RMIWalkIsInputEnabledDelegate>();

	private static readonly RMIWalkIsInputEnabledDelegate rmiWalkIsInputEnabled2 = new CompSig("E8 ?? ?? ?? ?? 84 C0 75 03 88 47 3F").GetDelegate<RMIWalkIsInputEnabledDelegate>();

	private readonly CompSig RMIWalkSig = new CompSig("E8 ?? ?? ?? ?? 80 7B 3E 00 48 8D 3D");

	private readonly Hook<RMIWalkDelegate>? RMIWalkHook;

	private readonly CompSig RMIFlySig = new CompSig("E8 ?? ?? ?? ?? 0F B6 0D ?? ?? ?? ?? B8");

	private readonly Hook<RMIFlyDelegate>? RMIFlyHook;

	public bool Enabled
	{
		get
		{
			return RMIWalkHook.IsEnabled;
		}
		set
		{
			if (value)
			{
				RMIWalkHook.Enable();
				RMIFlyHook.Enable();
			}
			else
			{
				RMIWalkHook.Disable();
				RMIFlyHook.Disable();
			}
		}
	}

	public bool IsAutoMove { get; set; } = true;

	public Vector3 DesiredPosition { get; set; }

	public float Precision { get; set; } = 0.01f;

	public unsafe MovementInputController()
	{
		if (RMIWalkHook == null)
		{
			RMIWalkHook = RMIWalkSig.GetHook<RMIWalkDelegate>(RMIWalkDetour);
		}
		if (RMIFlyHook == null)
		{
			RMIFlyHook = RMIFlySig.GetHook<RMIFlyDelegate>(RMIFlyDetour);
		}
	}

	public void Dispose()
	{
		RMIWalkHook?.Dispose();
		RMIFlyHook?.Dispose();
	}

	private unsafe void RMIWalkDetour(void* self, float* sumLeft, float* sumForward, float* sumTurnLeft, byte* haveBackwardOrStrafe, byte* a6, byte bAdditiveUnk)
	{
		RMIWalkHook.Original(self, sumLeft, sumForward, sumTurnLeft, haveBackwardOrStrafe, a6, bAdditiveUnk);
		if (bAdditiveUnk == 0 && rmiWalkIsInputEnabled1(self) && rmiWalkIsInputEnabled2(self) && (IsAutoMove || (*sumLeft == 0f && *sumForward == 0f)) && Angle.TryGetDirectionToDestination(DesiredPosition, allowVertical: false, out (Angle, Angle) direction, Precision))
		{
			Vector2 vector = direction.Item1.ToDirection();
			*sumLeft = vector.X;
			*sumForward = vector.Y;
		}
	}

	private unsafe void RMIFlyDetour(void* self, PlayerMoveControllerFlyInput* result)
	{
		RMIFlyHook.Original(self, result);
		if ((IsAutoMove || result->Forward != 0f || result->Left != 0f || result->Up != 0f) && Angle.TryGetDirectionToDestination(DesiredPosition, allowVertical: true, out (Angle, Angle) direction, Precision))
		{
			Vector2 vector = direction.Item1.ToDirection();
			result->Forward = vector.Y;
			result->Left = vector.X;
			result->Up = direction.Item2.Rad;
		}
	}
}
