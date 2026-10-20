using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class LowerDecksToKugane : EventSimpleYesTransportBase
{
	public override string DisplayName => "利姆萨·罗敏萨下层甲板 → 黄金港";

	public override uint SourceZone => 129u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(628u);

	public override Vector3 EventPosition => new Vector3(-353.4f, 8f, 46.1f);

	protected override uint EventID => 131253u;

	public override uint Cost => 300u;
}
