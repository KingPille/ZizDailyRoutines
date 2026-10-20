using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using DailyRoutines.Common.Notification.Abstractions;

namespace YHhMxZSAQwJ3GdIZ8fj;

[ManagerInitOrder(2)]
internal sealed class iFwL5MScsGWxnn8dLt0 : ManagerBase<iFwL5MScsGWxnn8dLt0>
{
	public ConcurrentDictionary<Type, NotificationChannelBase> Channels { get; private set; }

	protected override async Task Init()
	{
		IEnumerable<Type> enumerable = from t in Assembly.GetExecutingAssembly().GetTypes()
			where typeof(NotificationChannelBase).IsAssignableFrom(t) && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null
			select t;
		foreach (Type item in enumerable)
		{
			if (Activator.CreateInstance(item) is NotificationChannelBase notificationChannelBase && Channels.TryAdd(item, notificationChannelBase))
			{
				await notificationChannelBase.PublicInitAsync();
			}
		}
	}

	internal T? KUGSQapY2H<T>() where T : NotificationChannelBase
	{
		if (!Channels.TryGetValue(typeof(T), out NotificationChannelBase value))
		{
			return null;
		}
		return value as T;
	}

	protected override async Task Uninit()
	{
		foreach (NotificationChannelBase value in Channels.Values)
		{
			await value.PublicUninitAsync();
		}
		Channels.Clear();
	}

	public iFwL5MScsGWxnn8dLt0()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Channels = new ConcurrentDictionary<Type, NotificationChannelBase>();
		base._002Ector();
	}
}
