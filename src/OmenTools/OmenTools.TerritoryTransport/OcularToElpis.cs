using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class OcularToElpis : EventSimpleYesTransportBase
{
	public override string DisplayName => "观星室 → 厄尔庇斯";

	public override uint SourceZone => 844u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(961u);

	public override Vector3 EventPosition => new Vector3(-0f, 0.8f, -10.6f);

	protected override uint EventID => 131453u;
}
