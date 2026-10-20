using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace OmenTools.Interop.Game.Helpers;

public static class RotationHelper
{
	private const float PACKET_SCALE = 10430.22f;

	private const float INV_PACKET_SCALE = 9.587526E-05f;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float WorldDirHToChara(Vector2 direction)
	{
		return MathF.Atan2(direction.X, direction.Y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CharaSymmetricTransform(float rotation)
	{
		return rotation - MathF.CopySign((float)Math.PI, rotation);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float CameraDirHToChara(float cameraDirH)
	{
		float num = (cameraDirH + (float)Math.PI) % ((float)Math.PI * 2f);
		if (!(num < 0f))
		{
			return num;
		}
		return num + (float)Math.PI * 2f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsChanged(float target, float current, float tolerance = 0.1f)
	{
		float num = MathF.Abs(current - target);
		if (num > (float)Math.PI)
		{
			num = (float)Math.PI * 2f - num;
		}
		return num > tolerance;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ushort CharaToPacket(float rotation)
	{
		return (ushort)Math.Clamp((rotation + (float)Math.PI) * 10430.22f, 0f, 65535f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float PacketToChara(ushort rotation)
	{
		return (float)(int)rotation * 9.587526E-05f - (float)Math.PI;
	}
}
