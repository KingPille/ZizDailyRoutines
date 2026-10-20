using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class EulmoreToCrystarium : EventSimpleYesTransportBase
{
	public override string DisplayName => "游末邦 → 水晶都";

	public override uint SourceZone => 820u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(819u);

	public override Vector3 EventPosition => new Vector3(0f, 84.8f, -76.9f);

	protected override uint EventID => 131355u;
}
