using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class OldSharlayanToLowerDecks : EventSimpleYesTransportBase
{
	public override string DisplayName => "旧萨雷安 → 利姆萨·罗敏萨下层甲板";

	public override uint SourceZone => 962u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(129u);

	public override Vector3 EventPosition => new Vector3(155.2f, -16.1f, 180.5f);

	protected override uint EventID => 131405u;

	public override uint Cost => 300u;
}
