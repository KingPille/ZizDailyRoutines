using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class DomanEnclaveToGangos : EventSimpleYesTransportBase
{
	public override string DisplayName => "多玛飞地 → 甘戈斯";

	public override uint SourceZone => 759u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(915u);

	public override Vector3 EventPosition => new Vector3(123.2f, -4.2f, 98.4f);

	protected override uint EventID => 131359u;
}
