using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit)]
public struct AgentFreeCompany
{
	private unsafe delegate void ProcessMemberListEventDelegate(AgentFreeCompany* agent, AtkValue* values);

	private unsafe delegate void ConfirmMemberActionDelegate(AgentFreeCompany* agent, AtkValue* values);

	public enum MemberActionType : byte
	{
		Dismiss = 0,
		Promote = 4
	}

	private static readonly CompSig ProcessMemberListEventSig = new CompSig("48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 41 56 48 83 EC ?? 48 8B F1 48 8B DA");

	private static readonly ProcessMemberListEventDelegate ProcessMemberListEvent = ProcessMemberListEventSig.GetDelegate<ProcessMemberListEventDelegate>();

	private static readonly CompSig ConfirmMemberActionSig = new CompSig("48 89 5C 24 ?? 56 48 83 EC ?? 48 8B D9 48 8B CA E8 ?? ?? ?? ?? C7 43");

	private static readonly ConfirmMemberActionDelegate ConfirmMemberAction = ConfirmMemberActionSig.GetDelegate<ConfirmMemberActionDelegate>();

	[FieldOffset(80)]
	public unsafe InfoProxyFreeCompanyMember* InfoProxyFreeCompanyMember;

	[FieldOffset(94)]
	public byte CurrentMemberPageIndex;

	[FieldOffset(104)]
	public MemberActionType PendingAction;

	[FieldOffset(120)]
	public ulong TargetContentID;

	[FieldOffset(128)]
	public byte TargetRank;

	private const int MEMBER_STRING_SIZE = 5;

	private const int MEMBER_STRING_LOCATION = 1;

	public unsafe void SwitchMemberPage(int page)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		AtkValue* ptr = (AtkValue*)stackalloc AtkValue[2];
		Unsafe.Write(&((AtkValue)ptr).Type, (AtkValueType)3);
		((AtkValue)ptr).Int = 1;
		Unsafe.Write(&((AtkValue)((byte*)ptr + Unsafe.SizeOf<AtkValue>())).Type, (AtkValueType)5);
		((AtkValue)((byte*)ptr + Unsafe.SizeOf<AtkValue>())).UInt = (uint)page;
		fixed (AgentFreeCompany* agent = &this)
		{
			ProcessMemberListEvent(agent, ptr);
		}
	}

	public unsafe void ExecuteMemberAction(ulong contentID, MemberActionType action, byte rank)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		TargetContentID = contentID;
		TargetRank = rank;
		PendingAction = action;
		AtkValue* ptr = (AtkValue*)stackalloc AtkValue[1];
		Unsafe.Write(&((AtkValue)ptr).Type, (AtkValueType)3);
		((AtkValue)ptr).Int = 0;
		fixed (AgentFreeCompany* agent = &this)
		{
			ConfirmMemberAction(agent, ptr);
		}
	}

	public unsafe readonly string GetMemberLocationText(int memberIndex)
	{
		StringArrayData* stringArrayData = ((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)37);
		if (stringArrayData != null)
		{
			return SeString.Parse(((CStringPointer)((byte*)((StringArrayData)stringArrayData).StringArray + (nint)(1 + 5 * memberIndex) * (nint)Unsafe.SizeOf<CStringPointer>())).Value).TextValue;
		}
		return string.Empty;
	}

	public unsafe static AgentFreeCompany* Instance()
	{
		return (AgentFreeCompany*)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)79);
	}
}
