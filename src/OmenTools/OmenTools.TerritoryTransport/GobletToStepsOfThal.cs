using System.Collections.Generic;
using System.Numerics;
using OmenTools.Interop.Game.Lumina;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class GobletToStepsOfThal : EventSimpleStringTransportBase
{
	public override string DisplayName => "高脚孤丘 → 乌尔达哈来生回廊";

	public override uint SourceZone => 341u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(131u);

	public override Vector3 EventPosition => new Vector3(30.8f, -11.1f, -214.3f);

	protected override uint EventID => 131163u;

	protected override string EventString => LuminaWrapper.GetAddonText(6403u);
}
