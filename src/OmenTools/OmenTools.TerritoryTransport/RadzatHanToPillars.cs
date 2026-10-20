using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class RadzatHanToPillars : EventSimpleYesTransportBase
{
	public override string DisplayName => "拉札罕 → 伊修加德砥柱层";

	public override uint SourceZone => 963u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(419u);

	public override Vector3 EventPosition => new Vector3(-142.3f, 28f, 229.3f);

	protected override uint EventID => 131435u;

	public override uint Cost => 500u;
}
