using System;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Runtime.Abstractions;

namespace DailyRoutines.Common.Runtime.Hosts;

public static class ManagerHost
{
	[CompilerGenerated]
	private static IManagerHost _003CCurrent_003Ek__BackingField;

	public static IManagerHost Current
	{
		get
		{
			return _003CCurrent_003Ek__BackingField ?? throw new InvalidOperationException("管理器能力提供方宿主尚未注册");
		}
		[CompilerGenerated]
		set
		{
			_003CCurrent_003Ek__BackingField = value;
		}
	}
}
