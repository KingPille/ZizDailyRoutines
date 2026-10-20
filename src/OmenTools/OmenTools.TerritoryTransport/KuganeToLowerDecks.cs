using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class KuganeToLowerDecks : EventSimpleYesTransportBase
{
	public override string DisplayName => "黄金港 → 利姆萨·罗敏萨下层甲板";

	public override uint SourceZone => 628u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(129u);

	public override Vector3 EventPosition => new Vector3(-101.1f, -7f, -62.3f);

	protected override uint EventID => 131252u;

	public override uint Cost => 300u;
}
