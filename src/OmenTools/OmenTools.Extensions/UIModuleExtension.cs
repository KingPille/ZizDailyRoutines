using FFXIVClientStructs.FFXIV.Client.UI;
using OmenTools.Info.Game.Data;

namespace OmenTools.Extensions;

public static class UIModuleExtension
{
	extension(scoped ref UIModule module)
	{
		public unsafe static bool IsScreenReady()
		{
			if ((*Addons.NowLoading).IsAddonAndNodesReady())
			{
				return false;
			}
			if ((*Addons.FadeMiddle).IsAddonAndNodesReady())
			{
				return false;
			}
			if ((*Addons.FadeBack).IsAddonAndNodesReady())
			{
				return false;
			}
			return true;
		}
	}
}
