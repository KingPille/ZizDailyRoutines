using System;

namespace DailyRoutines.Common.Manager.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ManagerInitOrderAttribute(int order) : Attribute
{
	public int Order { get; } = order;
}
