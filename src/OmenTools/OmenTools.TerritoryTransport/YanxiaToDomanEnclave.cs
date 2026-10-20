using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class YanxiaToDomanEnclave : EventSimpleYesTransportBase
{
	public override string DisplayName => "延夏 → 多玛飞地";

	public override uint SourceZone => 614u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(759u);

	public override Vector3 EventPosition => new Vector3(-491.3f, 1.1f, 541.1f);

	protected override uint EventID => 131293u;
}
