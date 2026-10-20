using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Common.Component.BGCollision;

namespace OmenTools.Interop.Game.Helpers;

public static class RaycastHelper
{
	[Flags]
	public enum RaycastMode
	{
		Ray = 0,
		SweepSphere = 1,
		IgnoreHorizontal = 2,
		MaterialFilter = 4
	}

	public readonly struct RaycastOptions
	{
		public ulong LayerMask { get; init; }

		public RaycastMode Mode { get; init; }

		public float MaxPlaneNormalY { get; init; }

		public ulong MaterialMask { get; init; }

		public ulong MaterialValue { get; init; }

		public ulong ObjectMaterialMask { get; init; }

		public ulong ObjectMaterialValue { get; init; }

		internal ulong GetLayerMask()
		{
			if (LayerMask != 0L)
			{
				return LayerMask;
			}
			return 1uL;
		}

		internal RaycastMode GetMode()
		{
			if (MaterialMask != 0L)
			{
				return Mode | RaycastMode.MaterialFilter;
			}
			return Mode & ~RaycastMode.MaterialFilter;
		}
	}

	private const float DEFAULT_CEILING_DISTANCE = 1000f;

	private const float DEFAULT_GROUND_ORIGIN_LIFT = 0.5f;

	private const float DEFAULT_MAX_DISTANCE = 1000f;

	private const float DEFAULT_SAFE_CLEARANCE = 1.5f;

	private const float MIN_RAY_DISTANCE = 0.001f;

	private const float MIN_RAY_DISTANCE_SQUARED = 1.0000001E-06f;

	private const ulong DEFAULT_LAYER_MASK = 1uL;

	public static bool TryGetGroundHeight(Vector3 position, out float groundY, float maxDistance = 1000f)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetGroundHit(position, out var hitInfo, maxDistance))
		{
			groundY = hitInfo.Point.Y;
			return true;
		}
		groundY = position.Y;
		return false;
	}

	public static bool TryGetGroundHit(Vector3 position, out RaycastHit hitInfo, float maxDistance = 1000f, float originLift = 0.5f, RaycastOptions options = default(RaycastOptions))
	{
		Vector3 origin = position;
		origin.Y = position.Y + originLift;
		return TryRaycast(origin, -Vector3.UnitY, out hitInfo, maxDistance, options);
	}

	public static bool TryGetGroundPosition(Vector3 position, out Vector3 groundPosition, float maxDistance = 1000f, float originLift = 0.5f, RaycastOptions options = default(RaycastOptions))
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetGroundHit(position, out var hitInfo, maxDistance, originLift, options))
		{
			groundPosition = hitInfo.Point;
			return true;
		}
		groundPosition = position;
		return false;
	}

	public static bool HasLineOfSight(Vector3 from, Vector3 to)
	{
		if (!IsFinite(from) || !IsFinite(to))
		{
			return false;
		}
		if (!TryGetRay(from, to, out var direction, out var distance))
		{
			return true;
		}
		return !Intersects(from, direction, distance);
	}

	public static bool HasLineOfSight(Vector3 from, Vector3 to, RaycastOptions options)
	{
		if (!IsFinite(from) || !IsFinite(to))
		{
			return false;
		}
		if (!TryGetRay(from, to, out var direction, out var distance))
		{
			return true;
		}
		return !Intersects(from, direction, distance, options);
	}

	public static bool TryGetCeilingHeight(Vector3 position, out float ceilingY, float maxDistance = 1000f)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetCeilingHit(position, out var hitInfo, maxDistance))
		{
			ceilingY = hitInfo.Point.Y;
			return true;
		}
		ceilingY = position.Y + maxDistance;
		return false;
	}

	public static bool TryGetCeilingHit(Vector3 position, out RaycastHit hitInfo, float maxDistance = 1000f, RaycastOptions options = default(RaycastOptions))
	{
		return TryRaycast(position, Vector3.UnitY, out hitInfo, maxDistance, options);
	}

	public static bool TryGetCeilingPosition(Vector3 position, out Vector3 ceilingPosition, float maxDistance = 1000f, RaycastOptions options = default(RaycastOptions))
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetCeilingHit(position, out var hitInfo, maxDistance, options))
		{
			ceilingPosition = hitInfo.Point;
			return true;
		}
		Vector3 vector = position;
		vector.Y = position.Y + maxDistance;
		ceilingPosition = vector;
		return false;
	}

	public static bool TryGetNearestVerticalHit(Vector3 position, out RaycastHit hitInfo, float downDistance = 1000f, float upDistance = 1000f, RaycastOptions options = default(RaycastOptions))
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit hitInfo2;
		bool flag = TryRaycast(position + new Vector3(0f, 0.5f, 0f), -Vector3.UnitY, out hitInfo2, downDistance, options);
		RaycastHit hitInfo3;
		bool flag2 = TryRaycast(position, Vector3.UnitY, out hitInfo3, upDistance, options);
		if (flag && flag2)
		{
			hitInfo = ((hitInfo2.Distance <= hitInfo3.Distance) ? hitInfo2 : hitInfo3);
			return true;
		}
		if (flag)
		{
			hitInfo = hitInfo2;
			return true;
		}
		hitInfo = hitInfo3;
		return flag2;
	}

	public static Vector3 GetSafePosition(Vector3 from, Vector3 to)
	{
		if (!TryGetSafePosition(from, to, out var safePosition))
		{
			return to;
		}
		return safePosition;
	}

	public static bool TryGetSafePosition(Vector3 from, Vector3 to, out Vector3 safePosition, float clearance = 1.5f, RaycastOptions options = default(RaycastOptions))
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		safePosition = to;
		if (!TryGetRay(from, to, out var direction, out var distance))
		{
			return false;
		}
		if (!TryRaycast(from, direction, out var hitInfo, distance, options))
		{
			return false;
		}
		float num = MathF.Max(0f, hitInfo.Distance - MathF.Max(0f, clearance));
		safePosition = from + direction * num;
		Vector3 normal = hitInfo.Normal;
		normal.Y = 0f;
		if (TryNormalize(normal, out var normalized))
		{
			safePosition += normalized * clearance;
		}
		return true;
	}

	public static bool TryRaycastBetween(Vector3 from, Vector3 to, out RaycastHit hitInfo, RaycastOptions options = default(RaycastOptions))
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		hitInfo = default(RaycastHit);
		if (TryGetRay(from, to, out var direction, out var distance))
		{
			return TryRaycast(from, direction, out hitInfo, distance, options);
		}
		return false;
	}

	public static bool TryRaycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance = 1000f, RaycastOptions options = default(RaycastOptions))
	{
		return RaycastScene(origin, direction, maxDistance, out hitInfo, options, findClosest: true, options.GetMode());
	}

	public static bool TrySweepSphere(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance = 1000f, RaycastOptions options = default(RaycastOptions))
	{
		return RaycastScene(origin, direction, maxDistance, out hitInfo, options, findClosest: true, options.GetMode() | RaycastMode.SweepSphere);
	}

	public static bool Intersects(Vector3 origin, Vector3 direction, float maxDistance = 1000f, RaycastOptions options = default(RaycastOptions))
	{
		RaycastHit hitInfo;
		return RaycastScene(origin, direction, maxDistance, out hitInfo, options, findClosest: false, options.GetMode());
	}

	public unsafe static bool IsInsideCollision(Vector3 position, RaycastOptions options = default(RaycastOptions))
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetSceneManager(out var sceneManager) || !IsFinite(position))
		{
			return false;
		}
		NodeEnumerator<SceneWrapper> enumerator = ((SceneManager)sceneManager).Scenes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SceneWrapper* current = enumerator.Current;
			ColliderList val = default(ColliderList);
			if (((SceneWrapper)current).FindContainingCollidersCheckLayer(&val, options.GetLayerMask(), &position))
			{
				return true;
			}
		}
		return false;
	}

	public static bool TryGetCollisionDistance(Vector3 origin, Vector3 direction, out float distance, float maxDistance = 1000f, RaycastOptions options = default(RaycastOptions))
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (TryRaycast(origin, direction, out var hitInfo, maxDistance, options))
		{
			distance = hitInfo.Distance;
			return true;
		}
		distance = maxDistance;
		return false;
	}

	private unsafe static bool RaycastScene(Vector3 origin, Vector3 direction, float maxDistance, out RaycastHit hitInfo, RaycastOptions options, bool findClosest, RaycastMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		hitInfo = default(RaycastHit);
		if (!TryGetSceneManager(out var sceneManager) || !TryNormalize(direction, out var normalized) || !IsFinite(origin) || !float.IsFinite(maxDistance) || maxDistance <= 0.001f)
		{
			return false;
		}
		Vector4 vector = new Vector4(origin, 1f);
		float num = maxDistance;
		RaycastMaterialFilter val = new RaycastMaterialFilter
		{
			Mask = options.MaterialMask,
			Value = options.MaterialValue
		};
		RaycastParams val2 = new RaycastParams
		{
			Algorithm = (int)mode,
			Origin = &vector,
			Direction = &normalized,
			MaxDistance = &num,
			MaxPlaneNormalY = options.MaxPlaneNormalY,
			MaterialFilter = (((mode & RaycastMode.MaterialFilter) == 0) ? null : (&val)),
			ObjectMaterialMask = options.ObjectMaterialMask,
			ObjectMaterialValue = options.ObjectMaterialValue
		};
		ulong layerMask = options.GetLayerMask();
		float num2 = float.PositiveInfinity;
		bool result = false;
		NodeEnumerator<SceneWrapper> enumerator = ((SceneManager)sceneManager).Scenes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SceneWrapper* current = enumerator.Current;
			RaycastHit val3 = default(RaycastHit);
			if (((SceneWrapper)current).Raycast(&val3, layerMask, &val2))
			{
				if (!findClosest)
				{
					hitInfo = val3;
					return true;
				}
				if (!(val3.Distance >= num2))
				{
					num2 = val3.Distance;
					hitInfo = val3;
					result = true;
				}
			}
		}
		return result;
	}

	private unsafe static bool TryGetSceneManager(out SceneManager* sceneManager)
	{
		sceneManager = null;
		Framework* ptr = Framework.Instance();
		if (ptr == null)
		{
			return false;
		}
		BGCollisionModule* bGCollisionModule = ((Framework)ptr).BGCollisionModule;
		if (bGCollisionModule == null || ((BGCollisionModule)bGCollisionModule).SceneManager == null || ((SceneManager)((BGCollisionModule)bGCollisionModule).SceneManager).NumScenes == 0)
		{
			return false;
		}
		sceneManager = ((BGCollisionModule)bGCollisionModule).SceneManager;
		return true;
	}

	private static bool TryGetRay(Vector3 from, Vector3 to, out Vector3 direction, out float distance)
	{
		direction = default(Vector3);
		distance = 0f;
		if (!IsFinite(from) || !IsFinite(to))
		{
			return false;
		}
		Vector3 vector = to - from;
		float num = vector.LengthSquared();
		if (num <= 1.0000001E-06f || !float.IsFinite(num))
		{
			return false;
		}
		distance = MathF.Sqrt(num);
		direction = vector / distance;
		return true;
	}

	private static bool TryNormalize(Vector3 vector, out Vector3 normalized)
	{
		normalized = default(Vector3);
		float num = vector.LengthSquared();
		if (num <= 1.0000001E-06f || !float.IsFinite(num))
		{
			return false;
		}
		normalized = vector / MathF.Sqrt(num);
		return true;
	}

	private static bool IsFinite(Vector3 vector)
	{
		if (float.IsFinite(vector.X) && float.IsFinite(vector.Y))
		{
			return float.IsFinite(vector.Z);
		}
		return false;
	}
}
