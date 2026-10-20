using System;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

internal class EventObj : GameObject, IEventObj, IGameObject, IEquatable<IGameObject>
{
	public EventObj(nint address)
		: base(address)
	{
	}
}
