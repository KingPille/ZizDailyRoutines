using System;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

internal class NPC : Character, INPC, ICharacter, IGameObject, IEquatable<IGameObject>
{
	public NPC(nint address)
		: base(address)
	{
	}
}
