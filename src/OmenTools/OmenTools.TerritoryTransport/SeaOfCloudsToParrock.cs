using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class SeaOfCloudsToParrock : EventSimpleYesTransportBase
{
	public override string DisplayName => "阿巴拉提亚云海 → 帕洛克系留基地";

	public override uint SourceZone => 401u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(567u);

	public override Vector3 EventPosition => new Vector3(-813.9f, -88.3f, -834.3f);

	protected override uint EventID => 131223u;
}
