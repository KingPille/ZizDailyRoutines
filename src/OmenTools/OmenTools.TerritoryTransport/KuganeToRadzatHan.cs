using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class KuganeToRadzatHan : EventSimpleYesTransportBase
{
	public override string DisplayName => "黄金港 → 拉札罕";

	public override uint SourceZone => 628u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(963u);

	public override Vector3 EventPosition => new Vector3(-63.4f, 79.1f, 48.6f);

	protected override uint EventID => 131400u;

	public override uint Cost => 300u;
}
