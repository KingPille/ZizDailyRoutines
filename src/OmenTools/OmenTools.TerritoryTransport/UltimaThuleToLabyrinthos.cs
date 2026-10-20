using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class UltimaThuleToLabyrinthos : EventSimpleYesTransportBase
{
	public override string DisplayName => "天外天垓 → 迷津";

	public override uint SourceZone => 960u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(956u);

	public override Vector3 EventPosition => new Vector3(-375.4f, 80.7f, 613.9f);

	protected override uint EventID => 131418u;
}
