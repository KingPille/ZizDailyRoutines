using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace OmenTools.Info.Models;

public class Polygon
{
	private class PolarAngleComparer(Vector2 pivot) : IComparer<Vector2>
	{
		public int Compare(Vector2 p1, Vector2 p2)
		{
			float num = CrossProduct(pivot, p1, p2);
			if (Math.Abs(num) < 1E-06f)
			{
				return SquaredDistance(pivot, p1).CompareTo(SquaredDistance(pivot, p2));
			}
			if (!(num > 0f))
			{
				return 1;
			}
			return -1;
		}
	}

	private const float EPSILON = 1E-06f;

	private readonly Vector2[] vertices;

	public Polygon(IEnumerable<Vector2> vertices)
	{
		this.vertices = OptimizePolygon(vertices.ToArray());
		if (this.vertices.Length < 3)
		{
			throw new ArgumentException("无法构成一个有效的多边形");
		}
	}

	public bool IsPointInPolygon(Vector2 point)
	{
		bool flag = false;
		int num = 0;
		int num2 = vertices.Length - 1;
		while (num < vertices.Length)
		{
			if (vertices[num].Y > point.Y != vertices[num2].Y > point.Y && point.X < (vertices[num2].X - vertices[num].X) * (point.Y - vertices[num].Y) / (vertices[num2].Y - vertices[num].Y) + vertices[num].X)
			{
				flag = !flag;
			}
			num2 = num++;
		}
		return flag;
	}

	public Vector2 GenerateRandomInnerPoint()
	{
		if (vertices.Length < 3)
		{
			throw new InvalidOperationException("多边形无效，无法生成内部点");
		}
		Vector2 vector = CalculateCentroid();
		if (IsPointInPolygon(vector) && !IsPointOnBoundary(vector))
		{
			return vector;
		}
		for (int i = 0; i < 100; i++)
		{
			int num = Random.Shared.Next(vertices.Length);
			int num2 = Random.Shared.Next(vertices.Length);
			int num3 = Random.Shared.Next(vertices.Length);
			float num4 = (float)Random.Shared.NextDouble();
			float num5 = (float)Random.Shared.NextDouble() * (1f - num4);
			float num6 = 1f - num4 - num5;
			Vector2 vector2 = new Vector2(vertices[num].X * num4 + vertices[num2].X * num5 + vertices[num3].X * num6, vertices[num].Y * num4 + vertices[num2].Y * num5 + vertices[num3].Y * num6);
			if (IsPointInPolygon(vector2) && !IsPointOnBoundary(vector2))
			{
				return vector2;
			}
		}
		Vector2 vector3 = vector;
		Vector2[] array = vertices;
		foreach (Vector2 vector4 in array)
		{
			Vector2 value = vector4 - vector;
			value = Vector2.Normalize(value) * 0.1f * Vector2.Distance(vector, vector4);
			vector3 = vector + value;
			if (IsPointInPolygon(vector3) && !IsPointOnBoundary(vector3))
			{
				return vector3;
			}
		}
		throw new InvalidOperationException("无法在多边形内生成有效的点");
	}

	private bool IsPointOnBoundary(Vector2 point)
	{
		int num = 0;
		int num2 = vertices.Length - 1;
		while (num < vertices.Length)
		{
			if (IsPointOnLineSegment(point, vertices[num], vertices[num2]))
			{
				return true;
			}
			num2 = num++;
		}
		return false;
	}

	private static bool IsPointOnLineSegment(Vector2 point, Vector2 start, Vector2 end)
	{
		if (Math.Abs((point.Y - start.Y) * (end.X - start.X) - (point.X - start.X) * (end.Y - start.Y)) > 1E-06f)
		{
			return false;
		}
		float num = (point.X - start.X) * (end.X - start.X) + (point.Y - start.Y) * (end.Y - start.Y);
		if (num < 0f)
		{
			return false;
		}
		float num2 = (end.X - start.X) * (end.X - start.X) + (end.Y - start.Y) * (end.Y - start.Y);
		return !(num > num2);
	}

	private Vector2 CalculateCentroid()
	{
		Vector2 zero = Vector2.Zero;
		float num = 0f;
		for (int i = 0; i < vertices.Length; i++)
		{
			float x = vertices[i].X;
			float y = vertices[i].Y;
			float x2 = vertices[(i + 1) % vertices.Length].X;
			float y2 = vertices[(i + 1) % vertices.Length].Y;
			float num2 = x * y2 - x2 * y;
			num += num2;
			zero.X += (x + x2) * num2;
			zero.Y += (y + y2) * num2;
		}
		num *= 0.5f;
		if (Math.Abs(num) < 1E-06f)
		{
			return new Vector2(vertices.Sum((Vector2 v) => v.X) / (float)vertices.Length, vertices.Sum((Vector2 v) => v.Y) / (float)vertices.Length);
		}
		zero.X /= 6f * num;
		zero.Y /= 6f * num;
		return zero;
	}

	private static Vector2[] OptimizePolygon(Vector2[] vertices)
	{
		if (vertices == null || vertices.Length < 3)
		{
			return vertices ?? Array.Empty<Vector2>();
		}
		List<Vector2> list = new List<Vector2>();
		foreach (Vector2 vector in vertices)
		{
			bool flag = false;
			foreach (Vector2 item in list)
			{
				if (Vector2.Distance(item, vector) < 1E-05f)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(vector);
			}
		}
		if (list.Count < 3)
		{
			return list.ToArray();
		}
		list = FixSelfIntersection(list);
		return EnsureSimplePolygon(list.ToArray());
	}

	private static List<Vector2> FixSelfIntersection(List<Vector2> vertices)
	{
		if (vertices.Count <= 3)
		{
			return vertices;
		}
		List<Vector2> list = new List<Vector2>(vertices);
		bool flag;
		do
		{
			flag = false;
			for (int i = 0; i < list.Count; i++)
			{
				Vector2 vector = list[(i - 1 + list.Count) % list.Count];
				Vector2 vector2 = list[i];
				Vector2 vector3 = list[(i + 1) % list.Count];
				Vector2 value = Vector2.Normalize(vector2 - vector);
				Vector2 value2 = Vector2.Normalize(vector3 - vector2);
				if (Vector2.Dot(value, value2) < -0.9f)
				{
					list.RemoveAt(i);
					flag = true;
					break;
				}
			}
		}
		while (flag && list.Count > 3);
		return list;
	}

	private static Vector2[] EnsureSimplePolygon(Vector2[] vertices)
	{
		if (vertices.Length <= 3)
		{
			return vertices;
		}
		try
		{
			Vector2 centroid = new Vector2(vertices.Average((Vector2 v) => v.X), vertices.Average((Vector2 v) => v.Y));
			return (from v in vertices
				select (Point: v, Angle: Math.Atan2(v.Y - centroid.Y, v.X - centroid.X)) into item
				orderby item.Angle
				select item.Point).ToArray();
		}
		catch
		{
			return ComputeConvexHull(vertices);
		}
	}

	private static Vector2[] ComputeConvexHull(Vector2[] points)
	{
		int num = points.Length;
		if (num <= 3)
		{
			return points;
		}
		int num2 = 0;
		for (int i = 1; i < num; i++)
		{
			if (points[i].Y < points[num2].Y || (Math.Abs(points[i].Y - points[num2].Y) < 1E-06f && points[i].X < points[num2].X))
			{
				num2 = i;
			}
		}
		ref Vector2 reference = ref points[0];
		ref Vector2 reference2 = ref points[num2];
		Vector2 vector = points[num2];
		Vector2 vector2 = points[0];
		reference = vector;
		reference2 = vector2;
		Array.Sort(points, 1, num - 1, new PolarAngleComparer(points[0]));
		int num3 = 1;
		for (int j = 2; j < num; j++)
		{
			for (; j < num && CrossProduct(points[0], points[num3], points[j]) == 0f; j++)
			{
				if (SquaredDistance(points[0], points[j]) > SquaredDistance(points[0], points[num3]))
				{
					points[num3] = points[j];
				}
			}
			if (j < num)
			{
				points[++num3] = points[j];
			}
		}
		if (num3 + 1 < 3)
		{
			return points.Take(num3 + 1).ToArray();
		}
		List<Vector2> list = new List<Vector2>(num3 + 1)
		{
			points[0],
			points[1]
		};
		for (int k = 2; k <= num3; k++)
		{
			while (list.Count > 1)
			{
				if (!(CrossProduct(list[list.Count - 2], list[list.Count - 1], points[k]) <= 0f))
				{
					break;
				}
				list.RemoveAt(list.Count - 1);
			}
			list.Add(points[k]);
		}
		return list.ToArray();
	}

	private static float CrossProduct(Vector2 p0, Vector2 p1, Vector2 p2)
	{
		return (p1.X - p0.X) * (p2.Y - p0.Y) - (p2.X - p0.X) * (p1.Y - p0.Y);
	}

	private static float SquaredDistance(Vector2 p1, Vector2 p2)
	{
		float num = p2.X - p1.X;
		float num2 = p2.Y - p1.Y;
		return num * num + num2 * num2;
	}
}
