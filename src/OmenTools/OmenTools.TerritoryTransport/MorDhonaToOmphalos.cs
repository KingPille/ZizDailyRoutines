using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class MorDhonaToOmphalos : EventSimpleYesTransportBase
{
	public override string DisplayName => "摩杜纳 → 翁法洛斯";

	public override uint SourceZone => 156u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1061u);

	public override Vector3 EventPosition => new Vector3(155.4f, -26.9f, -440.6f);

	protected override uint EventID => 131472u;
}
