using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class GoldSaucerToStepsOfNald : EventSimpleYesTransportBase
{
	public override string DisplayName => "金碟游乐场 → 乌尔达哈现世回廊";

	public override uint SourceZone => 144u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(130u);

	public override Vector3 EventPosition => new Vector3(-38.2f, 0f, 100.5f);

	protected override uint EventID => 131183u;

	public override uint Cost => 0u;
}
