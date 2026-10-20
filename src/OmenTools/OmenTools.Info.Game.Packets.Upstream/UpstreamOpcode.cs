using System;
using System.Runtime.InteropServices;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Game.Packets.Upstream;

public static class UpstreamOpcode
{
	private static readonly CompSig EventStartOpcodeBaseSig;

	private static readonly CompSig EventActionOpcodeBaseSig;

	private static readonly CompSig EventCompleteOpcodeBaseSig;

	private static readonly CompSig DiveStartOpcodeBaseSig;

	private static readonly CompSig PositionUpdateBaseSig;

	private static readonly CompSig PositionUpdateInstanceBaseSig;

	private static readonly CompSig TreasureOpenOpcodeBaseSig;

	private static readonly CompSig HeartbeatOpcodeBaseSig;

	private static readonly CompSig UseActionOpcodeBaseSig;

	private static readonly CompSig UseActionLocationOpcodeBaseSig;

	private static readonly CompSig MJIInteractOpcodeBaseSig;

	private static readonly CompSig ExecuteCommandOpcodeBaseSig;

	private static readonly CompSig CharaCardOpenOpcodeBaseSig;

	private static readonly CompSig HandOverItemOpcodeBaseSig;

	public static int EventStartOpcode { get; private set; }

	public static int EventActionOpcode { get; private set; }

	public static int EventCompleteOpcode { get; private set; }

	public static int DiveStartOpcode { get; private set; }

	public static int PositionUpdateOpcode { get; private set; }

	public static int PositionUpdateInstanceOpcode { get; private set; }

	public static int TreasureOpenOpcode { get; private set; }

	public static int HeartbeatOpcode { get; private set; }

	public static int UseActionOpcode { get; private set; }

	public static int UseActionLocationOpcode { get; private set; }

	public static int MJIInteractOpcode { get; private set; }

	public static int ExecuteCommandOpcode { get; private set; }

	public static int CharaCardOpenOpcode { get; private set; }

	public static int HandOverItemOpcode { get; private set; }

	static UpstreamOpcode()
	{
		EventStartOpcodeBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? 48 C7 44 24 ?? ?? ?? ?? ?? 89 5C 24 ?? 0F 85");
		EventActionOpcodeBaseSig = new CompSig("E8 ?? ?? ?? ?? EB 3A 40 84 FF");
		EventCompleteOpcodeBaseSig = new CompSig("E8 ?? ?? ?? ?? EB 10 48 8B 0D ?? ?? ?? ??");
		DiveStartOpcodeBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? 48 C7 44 24 ?? ?? ?? ?? ?? E8 ?? ?? ?? ?? B0");
		PositionUpdateBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? F7 D9");
		PositionUpdateInstanceBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? 48 8D 54 24 ?? 48 C7 44 24 ?? ?? ?? ?? ?? 0F 11 44 24");
		TreasureOpenOpcodeBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? 48 C7 44 24 ?? ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 ?? 45 33 C9");
		HeartbeatOpcodeBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? 48 F7 F1");
		UseActionOpcodeBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? 45 33 C0 48 C7 44 24 ?? ?? ?? ?? ?? 89 7C 24");
		UseActionLocationOpcodeBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? 48 C7 44 24 ?? ?? ?? ?? ?? 89 74 24 ?? 40 88 6C 24");
		MJIInteractOpcodeBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? 48 C7 44 24 ?? ?? ?? ?? ?? 0F 11 44 24 ?? E8 ?? ?? ?? ?? 48 8B AC 24 ?? ?? ?? ?? 48 8B 8C 24 ?? ?? ?? ?? 48 33 CC E8 ?? ?? ?? ?? 48 81 C4 ?? ?? ?? ?? 41 5F 41 5E 41 5D 41 5C 5F 5E 5B C3 CC CC CC CC CC CC CC CC CC CC CC CC 40 53");
		ExecuteCommandOpcodeBaseSig = new CompSig("E8 ?? ?? ?? ?? FE 43 18");
		CharaCardOpenOpcodeBaseSig = new CompSig("C7 44 24 ?? ?? ?? ?? ?? 45 33 C0 48 C7 44 24 ?? ?? ?? ?? ?? C7 44 24 ?? ?? ?? ?? ?? C6 44 24 ?? ?? E8 ?? ?? ?? ?? 48 8B 8C 24 ?? ?? ?? ?? 48 33 CC E8 ?? ?? ?? ?? 48 81 C4 ?? ?? ?? ?? C3");
		HandOverItemOpcodeBaseSig = new CompSig("E8 ?? ?? ?? ?? EB 3A 40 84 FF");
		EventStartOpcode = ReadOpcode("EventStart", EventStartOpcodeBaseSig, 4);
		EventActionOpcode = ReadOpcode("EventAction", EventActionOpcodeBaseSig, 158);
		EventCompleteOpcode = ReadOpcode("EventComplete", EventCompleteOpcodeBaseSig, 279);
		DiveStartOpcode = ReadOpcode("DiveStart", DiveStartOpcodeBaseSig, 4);
		PositionUpdateInstanceOpcode = ReadOpcode("PositionUpdateInstance", PositionUpdateInstanceBaseSig, 4);
		TreasureOpenOpcode = ReadOpcode("TreasureOpen", TreasureOpenOpcodeBaseSig, 4);
		HeartbeatOpcode = ReadOpcode("Heartbeat", HeartbeatOpcodeBaseSig, 4);
		UseActionOpcode = ReadOpcode("UseAction", UseActionOpcodeBaseSig, 4);
		UseActionLocationOpcode = ReadOpcode("UseActionLocation", UseActionLocationOpcodeBaseSig, 4);
		MJIInteractOpcode = ReadOpcode("MJIInteract", MJIInteractOpcodeBaseSig, 4);
		ExecuteCommandOpcode = ReadOpcode("ExecuteCommand", ExecuteCommandOpcodeBaseSig, 109);
		CharaCardOpenOpcode = ReadOpcode("CharaCardOpen", CharaCardOpenOpcodeBaseSig, 4);
		HandOverItemOpcode = ReadOpcode("HandOverItem", HandOverItemOpcodeBaseSig, 189);
		PositionUpdateOpcode = ReadOpcode("PositionUpdate", PositionUpdateBaseSig, 4);
	}

	private static int ReadOpcode(string name, CompSig sig, int offset)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			return Marshal.ReadInt32(sig.ScanText() + offset);
		}
		catch (Exception ex)
		{
			DLog.Error("尝试读取 OPCODE " + name + " 时发生错误", ex);
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit($"尝试读取 OPCODE {name} 发生错误, 请在 Discord 内上报\nError while reading OPCODE {name}. Please report on Discord."));
		}
		return 0;
	}

	private static int ReadOpcodes(string name, params (CompSig sig, int offset)[] data)
	{
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Exception ex = null;
		for (int i = 0; i < data.Length; i++)
		{
			var (compSig, num) = data[i];
			try
			{
				return Marshal.ReadInt32(compSig.ScanText() + num);
			}
			catch (Exception ex2)
			{
				ex = ex2;
			}
		}
		DLog.Error("尝试读取 OPCODE " + name + " 时发生错误", ex);
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit($"尝试读取 OPCODE {name} 发生错误, 请在 Discord 内上报\nError while reading OPCODE {name}. Please report on Discord."));
		return 0;
	}
}
