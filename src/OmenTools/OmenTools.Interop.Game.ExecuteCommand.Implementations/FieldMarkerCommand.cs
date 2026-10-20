using System.Numerics;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class FieldMarkerCommand : ExecuteCommandBase
{
	public static void Place(FieldMarkerPoint point, Vector3 position)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.PlaceFieldMarker, (uint)point, (uint)(int)(position.X * 1000f), (uint)(int)(position.Y * 1000f), (uint)(int)(position.Z * 1000f));
	}

	public static void Remove(FieldMarkerPoint point)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveFieldMarker, (uint)point);
	}

	public static void Clear()
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.ClearFieldMarkers);
	}
}
