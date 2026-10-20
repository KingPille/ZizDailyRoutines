using System;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace OmenTools.Extensions;

public static class DalamudServiceExtension
{
	extension<TService>(TService) where TService : IDalamudService
	{
		public static TService Instance()
		{
			return (TService)((IServiceProvider)Instance()).GetService(typeof(TService));
		}
	}

	extension(IDalamudPluginInterface)
	{
		public static IDalamudPluginInterface Instance()
		{
			return DService.Instance().PI;
		}
	}
}
