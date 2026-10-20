using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class OmphalosToMorDhona : EventSimpleYesTransportBase
{
	public override string DisplayName => "翁法洛斯 → 摩杜纳";

	public override uint SourceZone => 1061u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(156u);

	public override Vector3 EventPosition => new Vector3(14.5f, 0.6f, -58.2f);

	protected override uint EventID => 131473u;
}
