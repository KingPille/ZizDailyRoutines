using System;
using OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

public interface INPC : ICharacter, IGameObject, IEquatable<IGameObject>
{
	new static INPC Create(nint address)
	{
		return new NPC(address);
	}
}
