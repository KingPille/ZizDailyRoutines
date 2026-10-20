using FFXIVClientStructs.FFXIV.Client.Game.Event;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Event.Abstractions;

public interface ILuaActor
{
	nint Address { get; }

	IGameObject? Object { get; }

	string Value { get; }

	unsafe LuaActor* ToStruct();

	static ILuaActor Create(nint address)
	{
		return new LuaActor(address);
	}
}
