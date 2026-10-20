using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace OmenTools.Extensions;

public static class AgentPointMenuExtension
{
	extension(scoped ref AgentPointMenu agent)
	{
		public unsafe int FindFirstUncompleteEntry()
		{
			fixed (AgentPointMenu* ptr = &agent)
			{
				if (ptr == null)
				{
					return -1;
				}
				int count = ((PointMenuContext)((AgentPointMenu)ptr).Context).Entries.Count;
				int pointMenuId = ((PointMenuContext)((AgentPointMenu)ptr).Context).PointMenuId;
				int num = 0;
				if (((AgentPointMenu)ptr).CompletionData != null)
				{
					((AgentPointMenu)ptr).CompletionData->TryGetValue(ref pointMenuId, ref num, false);
				}
				if ((count <= 0 || count > 32) ? true : false)
				{
					return -1;
				}
				for (int i = 0; i < count; i++)
				{
					if ((num & (1 << i)) == 0)
					{
						return i;
					}
				}
				return -1;
			}
		}
	}
}
