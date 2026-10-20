using System;
using System.Collections.Generic;

namespace OmenTools.Threading.TaskHelper.Models;

internal class TaskHelperQueue(int weight) : IEquatable<TaskHelperQueue>, IComparable<TaskHelperQueue>
{
	public int Weight { get; } = weight;

	public List<TaskHelperTask> Tasks { get; } = new List<TaskHelperTask>();

	public bool Equals(TaskHelperQueue? other)
	{
		if ((object)other != null)
		{
			return Weight == other.Weight;
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is TaskHelperQueue other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Weight;
	}

	public int CompareTo(TaskHelperQueue? other)
	{
		return other?.Weight.CompareTo(Weight) ?? 1;
	}

	public static bool operator ==(TaskHelperQueue? left, TaskHelperQueue? right)
	{
		return object.Equals(left, right);
	}

	public static bool operator !=(TaskHelperQueue? left, TaskHelperQueue? right)
	{
		return !object.Equals(left, right);
	}

	public static bool operator <(TaskHelperQueue left, TaskHelperQueue right)
	{
		return left.CompareTo(right) < 0;
	}

	public static bool operator <=(TaskHelperQueue left, TaskHelperQueue right)
	{
		return left.CompareTo(right) <= 0;
	}

	public static bool operator >(TaskHelperQueue left, TaskHelperQueue right)
	{
		return left.CompareTo(right) > 0;
	}

	public static bool operator >=(TaskHelperQueue left, TaskHelperQueue right)
	{
		return left.CompareTo(right) >= 0;
	}
}
