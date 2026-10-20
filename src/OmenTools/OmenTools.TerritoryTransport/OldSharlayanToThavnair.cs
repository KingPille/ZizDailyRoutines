using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class OldSharlayanToThavnair : EventSimpleYesTransportBase
{
	public override string DisplayName => "旧萨雷安 → 萨维奈岛";

	public override uint SourceZone => 962u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(957u);

	public override Vector3 EventPosition => new Vector3(-80.2f, 6.6f, -39.4f);

	protected override uint EventID => 131448u;
}
