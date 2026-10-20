using System;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class DutyCommand : ExecuteCommandBase
{
	public enum Difficulty : uint
	{
		Normal,
		Easy,
		VeryEasy
	}

	public enum LeaveDutyKind : uint
	{
		Normal,
		Inactive
	}

	public static void Leave(LeaveDutyKind kind = LeaveDutyKind.Normal)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LeaveDuty, (uint)kind);
	}

	public static void DutySupport(uint dawnStoryID, ReadOnlySpan<byte> memberUIParamIDs)
	{
		uint param = Pack(memberUIParamIDs, 0, 4);
		uint param2 = Pack(memberUIParamIDs, 4, 3);
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SendDutySupport, dawnStoryID, param, param2);
	}

	public static void SoloQuestBattle(Difficulty difficulty = Difficulty.Normal)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartSoloQuestBattle, (uint)difficulty);
	}

	private static uint Pack(ReadOnlySpan<byte> values, int start, int count)
	{
		uint num = 0u;
		int num2 = Math.Min(values.Length, start + count);
		for (int i = start; i < num2; i++)
		{
			num |= (uint)(values[i] << (i - start) * 8);
		}
		return num;
	}
}
