using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class TiringRoomToBridge : EventSimpleYesTransportBase
{
	public override string DisplayName => "剧场艇初见号道具间 → 剧场艇初见号舰桥";

	public override uint SourceZone => 735u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(736u);

	public override Vector3 EventPosition => new Vector3(36.6f, 0.4f, 0f);

	protected override uint EventID => 131283u;
}
