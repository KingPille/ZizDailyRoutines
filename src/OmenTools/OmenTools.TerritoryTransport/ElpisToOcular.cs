using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class ElpisToOcular : EventSimpleYesTransportBase
{
	public override string DisplayName => "厄尔庇斯 → 观星室";

	public override uint SourceZone => 961u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(844u);

	public override Vector3 EventPosition => new Vector3(281.3f, 47.2f, 832.2f);

	protected override uint EventID => 131452u;
}
