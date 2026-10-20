using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class CrystariumToOcular : EventSimpleYesTransportBase
{
	public override string DisplayName => "水晶都 → 观星室";

	public override uint SourceZone => 819u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(844u);

	public override Vector3 EventPosition => new Vector3(115f, 14.6f, 7.2f);

	protected override uint EventID => 131370u;
}
