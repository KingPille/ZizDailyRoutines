using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit)]
public struct AgentFreeCompanyProfile
{
	private unsafe delegate void ShowForContentIDDelegate(AgentFreeCompanyProfile* agent, ulong contentID);

	private static readonly CompSig ShowForContentIDSig = new CompSig("48 89 5C 24 ?? 57 48 83 EC ?? 48 8B 01 48 8B FA 48 8B D9 FF 50 ?? 84 C0 74 ?? 48 8B 03 48 8B CB FF 50 ?? 48 89 7B");

	private static readonly ShowForContentIDDelegate ShowForContentID = ShowForContentIDSig.GetDelegate<ShowForContentIDDelegate>();

	public unsafe void ShowProfile(ulong contentID)
	{
		fixed (AgentFreeCompanyProfile* agent = &this)
		{
			ShowForContentID(agent, contentID);
		}
	}

	public unsafe static AgentFreeCompanyProfile* Instance()
	{
		return (AgentFreeCompanyProfile*)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)81);
	}
}
