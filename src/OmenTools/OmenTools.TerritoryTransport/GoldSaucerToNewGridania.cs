using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class GoldSaucerToNewGridania : EventSimpleYesTransportBase
{
	public override string DisplayName => "金碟游乐场 → 格里达尼亚新街";

	public override uint SourceZone => 144u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(132u);

	public override Vector3 EventPosition => new Vector3(-38.2f, 0f, 100.5f);

	protected override uint EventID => 131184u;

	public override uint Cost => 120u;
}
