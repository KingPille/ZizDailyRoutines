using System;
using OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

public interface IPlayerCharacter : IBattleChara, ICharacter, IGameObject, IEquatable<IGameObject>
{
	new static IPlayerCharacter Create(nint address)
	{
		return new PlayerCharacter(address);
	}
}
