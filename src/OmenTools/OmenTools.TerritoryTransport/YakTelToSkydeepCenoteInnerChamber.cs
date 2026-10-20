using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class YakTelToSkydeepCenoteInnerChamber : EventSimpleYesTransportBase
{
	public override string DisplayName => "亚克特尔树海 → 深空天坑最深处";

	public override uint SourceZone => 1189u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1222u);

	public override Vector3 EventPosition => new Vector3(-826.2f, -297.9f, 874.5f);

	protected override uint EventID => 131553u;
}
