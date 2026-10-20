using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class GangosToDomanEnclave : EventSimpleYesTransportBase
{
	public override string DisplayName => "甘戈斯 → 多玛飞地";

	public override uint SourceZone => 915u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(759u);

	public override Vector3 EventPosition => new Vector3(-50.3f, 0.2f, -36.3f);

	protected override uint EventID => 131360u;
}
