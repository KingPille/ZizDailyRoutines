using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class SkydeepCenoteInnerChamberToYakTel : EventSimpleYesTransportBase
{
	public override string DisplayName => "深空天坑最深处 → 亚克特尔树海";

	public override uint SourceZone => 1222u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1189u);

	public override Vector3 EventPosition => new Vector3(0.1f, -0f, -10.8f);

	protected override uint EventID => 131554u;
}
