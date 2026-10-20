using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class YakTelToLowerJeuno : EventSimpleYesTransportBase
{
	public override string DisplayName => "亚克特尔树海 → 朱诺下层";

	public override uint SourceZone => 1189u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1265u);

	public override Vector3 EventPosition => new Vector3(-525f, -152.5f, 671.6f);

	protected override uint EventID => 131584u;
}
