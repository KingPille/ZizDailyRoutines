using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class PhantomVillageToTuliyollal : EventSimpleYesTransportBase
{
	public override string DisplayName => "幻境村 → 图莱尤拉";

	public override uint SourceZone => 1278u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(1185u);

	public override Vector3 EventPosition => new Vector3(78f, -11.2f, 13.4f);

	protected override uint EventID => 131593u;
}
