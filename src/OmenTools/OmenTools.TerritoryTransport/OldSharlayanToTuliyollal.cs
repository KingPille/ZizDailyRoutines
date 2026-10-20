using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class OldSharlayanToTuliyollal : EventSimpleYesTransportBase
{
	public override string DisplayName => "旧萨雷安 → 图莱尤拉";

	public override uint SourceZone => 962u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1185u);

	public override Vector3 EventPosition => new Vector3(141.1f, -16.1f, 221.8f);

	protected override uint EventID => 131543u;
}
