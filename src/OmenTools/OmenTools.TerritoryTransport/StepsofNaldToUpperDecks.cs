using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class StepsofNaldToUpperDecks : EventSimpleYesTransportBase
{
	public override string DisplayName => "乌尔达哈现世回廊 → 利姆萨·罗敏萨上层甲板";

	public override uint SourceZone => 130u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(128u);

	public override Vector3 EventPosition => new Vector3(-23.1f, 83.2f, -6f);

	protected override uint EventID => 131102u;

	public override uint Cost => 120u;
}
