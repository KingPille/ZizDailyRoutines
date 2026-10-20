using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class SyrcusTrenchToMorDhona : EventSimpleYesTransportBase
{
	public override string DisplayName => "希尔科斯峡谷 → 摩杜纳";

	public override uint SourceZone => 842u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(156u);

	public override Vector3 EventPosition => new Vector3(-70f, 4.9f, 77.8f);

	protected override uint EventID => 131323u;
}
