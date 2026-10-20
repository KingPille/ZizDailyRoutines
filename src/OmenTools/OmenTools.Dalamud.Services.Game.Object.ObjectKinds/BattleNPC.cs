using System;
using Dalamud.Game.ClientState.Objects.Enums;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

internal class BattleNPC : BattleChara, IBattleNPC, IBattleChara, ICharacter, IGameObject, IEquatable<IGameObject>
{
	public unsafe BattleNpcSubKind BattleNPCKind => (BattleNpcSubKind)((GameObject)(&((Character)(&((BattleChara)base.Struct).Character)).GameObject)).SubKind;

	public unsafe override ulong TargetObjectID => GameObjectId.op_Implicit(((Character)(&((BattleChara)base.Struct).Character)).TargetId);

	public BattleNPC(nint address)
		: base(address)
	{
	}
}
