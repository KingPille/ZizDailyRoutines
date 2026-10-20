using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class DravanianForelandsToChurningMists : EventSimpleYesTransportBase
{
	public override string DisplayName => "龙堡参天高地 → 翻云雾海";

	public override uint SourceZone => 398u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(400u);

	public override Vector3 EventPosition => new Vector3(-693.3f, 5f, -833.4f);

	protected override uint EventID => 131199u;
}
