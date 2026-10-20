using System;

namespace OmenTools;

public interface IOmenDalamudService<T> : IOmenDalamudService where T : IOmenDalamudService<T>
{
	static T Instance()
	{
		T omenDalamudService = DService.Instance().GetOmenDalamudService<T>();
		if (omenDalamudService == null)
		{
			throw new InvalidOperationException("服务 " + typeof(T).Name + " 尚未注册或初始化");
		}
		return omenDalamudService;
	}
}
public interface IOmenDalamudService
{
}
