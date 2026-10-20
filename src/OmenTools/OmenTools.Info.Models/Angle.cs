using System;
using System.Numerics;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models.Native;

namespace OmenTools.Info.Models;

public readonly struct Angle(float radians)
{
	public const float RAD_TO_DEG = 180f / (float)Math.PI;

	public const float DEG_TO_RAD = (float)Math.PI / 180f;

	public float Rad { get; init; } = radians;

	public float Deg => Rad * (180f / (float)Math.PI);

	public static Angle FromDirection(Vector2 dir)
	{
		return new Angle(MathF.Atan2(dir.X, dir.Y));
	}

	public static Angle FromDirectionXZ(Vector3 dir)
	{
		return new Angle(MathF.Atan2(dir.X, dir.Z));
	}

	public unsafe static bool TryGetDirectionToDestination(Vector3 desiredPosition, bool allowVertical, out (Angle H, Angle V) direction, float precision = 0.1f)
	{
		direction = default((Angle, Angle));
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return false;
		}
		Vector3 dir = desiredPosition - localPlayer.Position;
		if (dir.LengthSquared() <= precision * precision)
		{
			return false;
		}
		Angle angle = FromDirectionXZ(dir);
		Angle item = (allowVertical ? FromDirection(new Vector2(dir.Y, new Vector2(dir.X, dir.Z).Length())) : default(Angle));
		uint num = default(uint);
		Angle angle2 = ((DalamudServiceExtension.Instance<IGameConfig>().UiControl.TryGetUInt("MoveMode", ref num) && num == 1) ? (new Angle(((CameraExtra*)((CameraManager)CameraManager.Instance()).GetActiveCamera())->DirH) + new Angle((float)Math.PI)) : new Angle(localPlayer.Rotation));
		direction = (H: angle - angle2, V: item);
		return true;
	}

	public Vector2 ToDirection()
	{
		return new Vector2(MathF.Sin(Rad), MathF.Cos(Rad));
	}

	public static Angle operator +(Angle a, Angle b)
	{
		return new Angle(a.Rad + b.Rad);
	}

	public static Angle operator -(Angle a, Angle b)
	{
		return new Angle(a.Rad - b.Rad);
	}

	public override string ToString()
	{
		return Deg.ToString("f0");
	}

	public override bool Equals(object? obj)
	{
		if (obj is Angle angle)
		{
			return Rad == angle.Rad;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Rad.GetHashCode();
	}

	public static bool operator ==(Angle l, Angle r)
	{
		return l.Rad == r.Rad;
	}

	public static bool operator !=(Angle l, Angle r)
	{
		return l.Rad != r.Rad;
	}
}
