using System.Collections.Generic;
using System.Numerics;
using OmenTools.Interop.Game.Lumina;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class ShiroganeToKugane : EventSimpleStringTransportBase
{
	public override string DisplayName => "白银乡 → 黄金港";

	public override uint SourceZone => 641u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(628u);

	public override Vector3 EventPosition => new Vector3(-857f, 2f, -822.5f);

	protected override uint EventID => 131249u;

	protected override string EventString => LuminaWrapper.GetAddonText(6403u);
}
