using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class NethergateToMareLamentorum : EventSimpleYesTransportBase
{
	public override string DisplayName => "神门之间 → 叹息海";

	public override uint SourceZone => 1024u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(959u);

	public override Vector3 EventPosition => new Vector3(-0f, 5.6f, 2.1f);

	protected override uint EventID => 131462u;
}
