using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class AroundRangeCommand : ExecuteCommandBase
{
	public enum Mode : uint
	{
		Standard,
		Large,
		Maximum
	}

	public static void Set(Mode mode)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SetAroundRangeMode, (uint)mode);
	}
}
