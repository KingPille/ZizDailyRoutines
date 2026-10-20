using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class GarlemaldToNethergate : EventSimpleYesTransportBase
{
	public override string DisplayName => "加雷马 → 神门之间";

	public override uint SourceZone => 958u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1024u);

	public override Vector3 EventPosition => new Vector3(-480.2f, 10.9f, -673f);

	protected override uint EventID => 131450u;
}
