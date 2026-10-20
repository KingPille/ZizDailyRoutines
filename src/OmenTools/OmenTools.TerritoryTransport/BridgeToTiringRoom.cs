using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class BridgeToTiringRoom : EventSimpleYesTransportBase
{
	public override string DisplayName => "剧场艇初见号舰桥 → 剧场艇初见号道具间";

	public override uint SourceZone => 736u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(735u);

	public override Vector3 EventPosition => new Vector3(7.3f, -0f, 0.1f);

	protected override uint EventID => 131284u;
}
