using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class NethergateToGarlemald : EventSimpleYesTransportBase
{
	public override string DisplayName => "神门之间 → 加雷马";

	public override uint SourceZone => 1024u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(958u);

	public override Vector3 EventPosition => new Vector3(0.2f, 0f, 49.1f);

	protected override uint EventID => 131449u;
}
