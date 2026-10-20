using System.Collections.Generic;
using System.Numerics;
using OmenTools.Interop.Game.Lumina;
using OmenTools.TerritoryTransport.Abstractions;

namespace OmenTools.TerritoryTransport;

public sealed class EmpyreumToFoundation : EventSimpleStringTransportBase
{
	public override string DisplayName => "穹顶皓天 → 伊修加德基础层";

	public override uint SourceZone => 979u;

	public override IReadOnlyList<uint> TargetZones => new _003C_003Ez__ReadOnlySingleElementList<uint>(418u);

	public override Vector3 EventPosition => new Vector3(-882f, -15.2f, -693.5f);

	protected override uint EventID => 131396u;

	protected override string EventString => LuminaWrapper.GetAddonText(6403u);
}
