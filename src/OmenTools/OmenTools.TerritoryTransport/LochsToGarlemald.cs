using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class LochsToGarlemald : EventSimpleYesTransportBase
{
	public override string DisplayName => "基拉巴尼亚湖区 → 加雷马";

	public override uint SourceZone => 621u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(958u);

	public override Vector3 EventPosition => new Vector3(744.5f, 70f, 536f);

	protected override uint EventID => 131420u;
}
