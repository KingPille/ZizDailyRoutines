using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class PillarsToUpperDecks : EventSimpleYesTransportBase
{
	public override string DisplayName => "伊修加德砥柱层 → 利姆萨·罗敏萨上层甲板";

	public override uint SourceZone => 419u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(128u);

	public override Vector3 EventPosition => new Vector3(153.7f, -12.6f, -8.6f);

	protected override uint EventID => 131234u;

	public override uint Cost => 150u;
}
