using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class AmhAraengToCrystarium : EventSimpleYesTransportBase
{
	public override string DisplayName => "安穆·艾兰 → 水晶都";

	public override uint SourceZone => 815u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(819u);

	public override Vector3 EventPosition => new Vector3(693.5f, -38.9f, -660.1f);

	protected override uint EventID => 131336u;
}
