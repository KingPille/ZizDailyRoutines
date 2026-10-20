using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class MorDhonaToSyrcusTrench : EventSimpleYesTransportBase
{
	public override string DisplayName => "摩杜纳 → 希尔科斯峡谷";

	public override uint SourceZone => 156u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(842u);

	public override Vector3 EventPosition => new Vector3(353.6f, -25.4f, -374.1f);

	protected override uint EventID => 131322u;
}
