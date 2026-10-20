using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class RoyalMenagerieToLochs : EventSimpleYesTransportBase
{
	public override string DisplayName => "空中庭园 → 基拉巴尼亚湖区";

	public override uint SourceZone => 740u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(621u);

	public override Vector3 EventPosition => new Vector3(-466.3f, 383f, -126.9f);

	protected override uint EventID => 131288u;
}
