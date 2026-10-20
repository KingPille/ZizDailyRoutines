using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class KuganeToTiringRoom : EventSimpleYesTransportBase
{
	public override string DisplayName => "黄金港 → 剧场艇初见号道具间";

	public override uint SourceZone => 628u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(735u);

	public override Vector3 EventPosition => new Vector3(-56.7f, 79.1f, 47.5f);

	protected override uint EventID => 131281u;
}
