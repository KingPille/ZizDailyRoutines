using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class GoldSaucerToUpperDecks : EventSimpleYesTransportBase
{
	public override string DisplayName => "金碟游乐场 → 利姆萨·罗敏萨上层甲板";

	public override uint SourceZone => 144u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(128u);

	public override Vector3 EventPosition => new Vector3(-38.2f, 0f, 100.5f);

	protected override uint EventID => 131182u;

	public override uint Cost => 120u;
}
