using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class SkydeepCenoteInnerChamberToLivingMemory : EventSimpleYesTransportBase
{
	public override string DisplayName => "深空天坑最深处 → 活着的记忆";

	public override uint SourceZone => 1222u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1192u);

	public override Vector3 EventPosition => new Vector3(0f, 0.2f, -84.5f);

	protected override uint EventID => 131561u;
}
