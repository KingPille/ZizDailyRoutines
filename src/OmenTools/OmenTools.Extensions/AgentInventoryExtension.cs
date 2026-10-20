using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace OmenTools.Extensions;

public static class AgentInventoryExtension
{
	extension(scoped ref AgentInventory agent)
	{
		public unsafe uint GetActiveAddonID()
		{
			if (!((UIModule)UIModule.Instance()).IsInventoryOpen())
			{
				return 0u;
			}
			ushort num = (ushort)agent.AddonId;
			if (!(*((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonById(num)).IsAddonAndNodesReady())
			{
				return 0u;
			}
			return num;
		}
	}
}
