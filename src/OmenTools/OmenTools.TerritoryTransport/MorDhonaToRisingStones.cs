using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class MorDhonaToRisingStones : EventSimpleYesTransportBase
{
	public override string DisplayName => "摩杜纳 → 石之家";

	public override uint SourceZone => 156u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(351u);

	public override Vector3 EventPosition => new Vector3(22.3f, 21.3f, -633.7f);

	protected override uint EventID => 131144u;
}
