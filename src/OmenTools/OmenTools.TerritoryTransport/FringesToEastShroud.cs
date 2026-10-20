using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class FringesToEastShroud : EventSimpleYesTransportBase
{
	public override string DisplayName => "基拉巴尼亚边区 → 黑衣森林东部林区";

	public override uint SourceZone => 612u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(152u);

	public override Vector3 EventPosition => new Vector3(-662f, 130f, -532.3f);

	protected override uint EventID => 131250u;
}
