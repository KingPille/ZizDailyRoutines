using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class WakingSandsToWesternThanalan : EventSimpleYesTransportBase
{
	public override string DisplayName => "沙之家 → 西萨纳兰";

	public override uint SourceZone => 212u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(140u);

	public override Vector3 EventPosition => new Vector3(-13.5f, 0f, 0.2f);

	protected override uint EventID => 131089u;
}
