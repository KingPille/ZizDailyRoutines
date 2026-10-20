using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class WesternThanalanToLowerDecks : EventSimpleYesTransportBase
{
	public override string DisplayName => "西萨纳兰 → 利姆萨·罗敏萨下层甲板";

	public override uint SourceZone => 140u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(129u);

	public override Vector3 EventPosition => new Vector3(-487.8f, 24f, -330.7f);

	protected override uint EventID => 131107u;

	public override uint Cost => 80u;
}
