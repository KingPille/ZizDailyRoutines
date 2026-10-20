using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class EasternLaNosceaToLowerDecks : EventSimpleYesTransportBase
{
	public override string DisplayName => "东拉诺西亚 → 利姆萨·罗敏萨下层甲板";

	public override uint SourceZone => 137u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(129u);

	public override Vector3 EventPosition => new Vector3(612.3f, 11.6f, 394.3f);

	protected override uint EventID => 131112u;

	public override uint Cost => 40u;
}
