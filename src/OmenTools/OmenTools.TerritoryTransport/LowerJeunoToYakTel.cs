using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class LowerJeunoToYakTel : EventSimpleYesTransportBase
{
	public override string DisplayName => "朱诺下层 → 亚克特尔树海";

	public override uint SourceZone => 1265u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1189u);

	public override Vector3 EventPosition => new Vector3(-0.1f, 42f, 11.8f);

	protected override uint EventID => 131583u;
}
