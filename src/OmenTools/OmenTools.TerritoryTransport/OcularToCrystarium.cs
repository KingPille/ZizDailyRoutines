using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class OcularToCrystarium : EventSimpleYesTransportBase
{
	public override string DisplayName => "观星室 → 水晶都";

	public override uint SourceZone => 844u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(819u);

	public override Vector3 EventPosition => new Vector3(-0.1f, 0f, 12.4f);

	protected override uint EventID => 131311u;
}
