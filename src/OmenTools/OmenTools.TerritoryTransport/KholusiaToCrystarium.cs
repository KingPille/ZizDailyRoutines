using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class KholusiaToCrystarium : EventSimpleYesTransportBase
{
	public override string DisplayName => "珂露西亚岛 → 水晶都";

	public override uint SourceZone => 814u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(819u);

	public override Vector3 EventPosition => new Vector3(796.8f, 1.8f, 258.6f);

	protected override uint EventID => 131332u;
}
