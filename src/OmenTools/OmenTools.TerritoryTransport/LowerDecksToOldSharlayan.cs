using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class LowerDecksToOldSharlayan : EventSimpleYesTransportBase
{
	public override string DisplayName => "利姆萨·罗敏萨下层甲板 → 旧萨雷安";

	public override uint SourceZone => 129u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(962u);

	public override Vector3 EventPosition => new Vector3(-387.2f, 6f, 44.1f);

	protected override uint EventID => 131406u;

	public override uint Cost => 300u;
}
