using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class LowerDecksToEasternLaNoscea : EventSimpleYesTransportBase
{
	public override string DisplayName => "利姆萨·罗敏萨下层甲板 → 东拉诺西亚";

	public override uint SourceZone => 129u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(137u);

	public override Vector3 EventPosition => new Vector3(-189.1f, 1f, 208.7f);

	protected override uint EventID => 131111u;

	public override uint Cost => 40u;
}
