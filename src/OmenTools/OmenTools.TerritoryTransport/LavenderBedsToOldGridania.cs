using System.Collections.Generic;
using System.Numerics;
using OmenTools.Interop.Game.Lumina;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class LavenderBedsToOldGridania : EventSimpleStringTransportBase
{
	public override string DisplayName => "薰衣草苗圃 → 格里达尼亚旧街";

	public override uint SourceZone => 340u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(133u);

	public override Vector3 EventPosition => new Vector3(9.7f, 2.6f, 204.7f);

	protected override uint EventID => 131159u;

	protected override string EventString => LuminaWrapper.GetAddonText(6403u);
}
