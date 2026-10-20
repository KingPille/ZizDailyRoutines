using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using OmenTools.Interop.Game.Models;

namespace OmenTools.Extensions;

public static class AgentHousingPortalExtension
{
	extension(scoped ref AgentHousingPortal agent)
	{
		public unsafe void TeleportToWard(int wardIndex)
		{
			NotifyCallback((AgentHousingPortal*)Unsafe.AsPointer(in agent), 3, wardIndex);
		}
	}

	private unsafe delegate void NotifyCallbackDelegate(AgentHousingPortal* agent, int action, int argument);

	private static readonly CompSig AgentHousingPortalNotifyCallbackSig = new CompSig("48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 83 79 28 00 41 8B F0 8B EA 48 8B F9");

	private static readonly NotifyCallbackDelegate NotifyCallback = AgentHousingPortalNotifyCallbackSig.GetDelegate<NotifyCallbackDelegate>();
}
