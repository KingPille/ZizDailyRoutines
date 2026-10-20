using System;
using Dalamud.Game.ClientState.Objects.Enums;
using OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

public interface IBattleNPC : IBattleChara, ICharacter, IGameObject, IEquatable<IGameObject>
{
	BattleNpcSubKind BattleNPCKind { get; }

	new static IBattleNPC Create(nint address)
	{
		return new BattleNPC(address);
	}
}
