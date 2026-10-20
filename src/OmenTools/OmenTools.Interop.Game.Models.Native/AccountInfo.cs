using System.Runtime.InteropServices;
using FFXIVClientStructs.STD;

namespace OmenTools.Interop.Game.Models.Native;

[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct AccountInfo
{
	private unsafe delegate AccountInfo* GetInstanceDelegate();

	private static readonly CompSig GetInstanceSig = new CompSig("48 8B 05 ?? ?? ?? ?? C3 CC CC CC CC CC CC CC CC 83 39");

	private static readonly GetInstanceDelegate GetInstance = GetInstanceSig.GetDelegate<GetInstanceDelegate>();

	[FieldOffset(0)]
	public AccountInfoState State;

	[FieldOffset(8)]
	public ulong AccountID;

	[FieldOffset(16)]
	public StdVector<AccountInfoCharacter> Characters;

	[FieldOffset(40)]
	public byte IsSaving;

	[FieldOffset(41)]
	public byte SaveResult;

	public unsafe static AccountInfo* Instance()
	{
		return GetInstance();
	}
}
