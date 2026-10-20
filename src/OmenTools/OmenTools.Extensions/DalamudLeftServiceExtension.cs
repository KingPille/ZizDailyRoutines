using Dalamud.Interface;

namespace OmenTools.Extensions;

public static class DalamudLeftServiceExtension
{
	extension(IUiBuilder)
	{
		public static IUiBuilder Instance()
		{
			return DService.Instance().UIBuilder;
		}
	}
}
