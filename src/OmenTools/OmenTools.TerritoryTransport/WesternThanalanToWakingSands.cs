using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class WesternThanalanToWakingSands : EventSimpleYesTransportBase
{
	public override string DisplayName => "西萨纳兰 → 沙之家";

	public override uint SourceZone => 140u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(212u);

	public override Vector3 EventPosition => new Vector3(-482.2f, 17f, -386.9f);

	protected override uint EventID => 131088u;
}
