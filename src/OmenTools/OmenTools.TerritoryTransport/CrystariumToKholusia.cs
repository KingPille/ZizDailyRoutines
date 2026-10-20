using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class CrystariumToKholusia : EventSimpleYesTransportBase
{
	public override string DisplayName => "水晶都 → 珂露西亚岛";

	public override uint SourceZone => 819u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(814u);

	public override Vector3 EventPosition => new Vector3(55.9f, 36.2f, -168.4f);

	protected override uint EventID => 131329u;
}
