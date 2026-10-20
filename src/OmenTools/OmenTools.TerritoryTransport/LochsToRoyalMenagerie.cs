using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class LochsToRoyalMenagerie : EventSimpleYesTransportBase
{
	public override string DisplayName => "基拉巴尼亚湖区 → 空中庭园";

	public override uint SourceZone => 621u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(740u);

	public override Vector3 EventPosition => new Vector3(746f, 70f, 530.4f);

	protected override uint EventID => 131443u;
}
