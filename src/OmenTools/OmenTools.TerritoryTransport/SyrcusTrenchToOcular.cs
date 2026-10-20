using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class SyrcusTrenchToOcular : EventSimpleYesTransportBase
{
	public override string DisplayName => "希尔科斯峡谷 → 观星室";

	public override uint SourceZone => 842u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(844u);

	public override Vector3 EventPosition => new Vector3(-48.4f, 1.4f, 40.9f);

	protected override uint EventID => 131324u;
}
