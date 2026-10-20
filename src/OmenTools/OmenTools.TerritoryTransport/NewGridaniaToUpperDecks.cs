using System.Collections.Generic;
using System.Numerics;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class NewGridaniaToUpperDecks : EventSimpleYesTransportBase
{
	public override string DisplayName => "格里达尼亚新街 → 利姆萨·罗敏萨上层甲板";

	public override uint SourceZone => 132u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(128u);

	public override Vector3 EventPosition => new Vector3(28.7f, -19f, 101f);

	protected override uint EventID => 131104u;

	public override uint Cost => 120u;
}
