using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OmenTools.OmenService;

namespace OmenTools.Extensions;

public static class AgentWorldTravelExtension
{
	extension(scoped ref AgentWorldTravel agent)
	{
		public void TravelTo(uint destinationWorldID)
		{
			agent.TravelTo((ushort)destinationWorldID);
		}

		public unsafe void TravelTo(ushort destinationWorldID)
		{
			fixed (AgentWorldTravel* ptr = &agent)
			{
				if (ptr == null)
				{
					return;
				}
				((AgentWorldTravel)ptr).DestinationWorldId = destinationWorldID;
				((AgentWorldTravel)ptr).SetupWorldTravelInfo((ushort)GameState.CurrentWorld, destinationWorldID);
				((AgentId)335).SendEvent(1uL, 0);
			}
		}
	}
}
