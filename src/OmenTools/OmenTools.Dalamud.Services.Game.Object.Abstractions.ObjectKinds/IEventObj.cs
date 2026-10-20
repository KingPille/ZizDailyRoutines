using System;
using OmenTools.Dalamud.Services.Game.Object.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

public interface IEventObj : IGameObject, IEquatable<IGameObject>
{
	new static IEventObj Create(nint address)
	{
		return new EventObj(address);
	}
}
