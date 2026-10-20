using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class EastShroudToFringes : EventSimpleYesTransportBase
{
	public override string DisplayName => "黑衣森林东部林区 → 基拉巴尼亚边区";

	public override uint SourceZone => 152u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(612u);

	public override Vector3 EventPosition => new Vector3(25f, 5.8f, 403.9f);

	protected override uint EventID => 131251u;
}
