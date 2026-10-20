using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class LowerDecksToWesternLaNoscea : EventSimpleYesTransportBase
{
	public override string DisplayName => "利姆萨·罗敏萨下层甲板 → 西拉诺西亚";

	public override uint SourceZone => 129u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(138u);

	public override Vector3 EventPosition => new Vector3(-189.1f, 1f, 208.7f);

	protected override uint EventID => 131109u;

	public override uint Cost => 40u;
}
