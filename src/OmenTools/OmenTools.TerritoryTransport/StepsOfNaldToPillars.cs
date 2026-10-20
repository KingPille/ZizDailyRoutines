using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class StepsOfNaldToPillars : EventSimpleYesTransportBase
{
	public override string DisplayName => "乌尔达哈现世回廊 → 伊修加德砥柱层";

	public override uint SourceZone => 130u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(419u);

	public override Vector3 EventPosition => new Vector3(-23.1f, 83.2f, -6f);

	protected override uint EventID => 131229u;

	public override uint Cost => 150u;
}
