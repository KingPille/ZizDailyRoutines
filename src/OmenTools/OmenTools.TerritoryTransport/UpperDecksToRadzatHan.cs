using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class UpperDecksToRadzatHan : EventSimpleYesTransportBase
{
	public override string DisplayName => "利姆萨·罗敏萨上层甲板 → 拉札罕";

	public override uint SourceZone => 128u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(963u);

	public override Vector3 EventPosition => new Vector3(-22.3f, 92f, -3.8f);

	protected override uint EventID => 131434u;

	public override uint Cost => 300u;
}
