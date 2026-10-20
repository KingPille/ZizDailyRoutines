using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Common.Math;
using OmenTools.Global;

namespace OmenTools.Extensions;

public static class VectorExtension
{
	extension(scoped in Vector3 vector3)
	{
		public Vector2 ToVector2()
		{
			return new Vector2(vector3.X, vector3.Z);
		}
	}

	extension(IEnumerable<Vector3> points)
	{
		public float GetTotalDistance()
		{
			ArgumentNullException.ThrowIfNull(points, "points");
			float num = 0f;
			Vector3? vector = null;
			foreach (Vector3 point in points)
			{
				if (vector.HasValue)
				{
					num += Vector3.Distance(vector.Value, point);
				}
				vector = point;
			}
			return num;
		}

		public float GetTotalDistanceSquared()
		{
			ArgumentNullException.ThrowIfNull(points, "points");
			float num = 0f;
			Vector3? vector = null;
			foreach (Vector3 point in points)
			{
				if (vector.HasValue)
				{
					num += Vector3.DistanceSquared(vector.Value, point);
				}
				vector = point;
			}
			return num;
		}
	}

	extension(scoped in Vector3 vector3)
	{
		public Vector2 ToVector2()
		{
			return new Vector2(vector3.X, vector3.Z);
		}
	}

	extension(scoped in Vector2 vector2)
	{
		public Vector3 ToVector3(scoped in float y)
		{
			return new Vector3(vector2.X, y, vector2.Y);
		}

		public Vector2 Scale()
		{
			return vector2 * Globals.GlobalUIScale;
		}
	}
}
