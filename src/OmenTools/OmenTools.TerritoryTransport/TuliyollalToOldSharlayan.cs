using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class TuliyollalToOldSharlayan : EventSimpleYesTransportBase
{
	public override string DisplayName => "图莱尤拉 → 旧萨雷安";

	public override uint SourceZone => 1185u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(962u);

	public override Vector3 EventPosition => new Vector3(123f, -18f, 142.9f);

	protected override uint EventID => 131545u;
}
