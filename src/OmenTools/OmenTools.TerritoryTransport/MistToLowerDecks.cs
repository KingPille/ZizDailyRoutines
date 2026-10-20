using System.Collections.Generic;
using System.Numerics;
using OmenTools.Interop.Game.Lumina;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class MistToLowerDecks : EventSimpleStringTransportBase
{
	public override string DisplayName => "海雾村 → 利姆萨·罗敏萨下层甲板";

	public override uint SourceZone => 339u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(129u);

	public override Vector3 EventPosition => new Vector3(-65.6f, 2f, 97.5f);

	protected override uint EventID => 131161u;

	protected override string EventString => LuminaWrapper.GetAddonText(6403u);
}
