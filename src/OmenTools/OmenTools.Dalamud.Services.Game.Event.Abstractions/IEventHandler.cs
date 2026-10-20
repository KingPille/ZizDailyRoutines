using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Common.Lua;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Event.Abstractions;

public interface IEventHandler
{
	nint Address { get; }

	EventId EventID { get; }

	uint IconID { get; }

	short Scene { get; }

	IGameObject? SceneGameObject { get; }

	SceneFlag SceneFlags { get; }

	LuaStatus LuaStatus { get; }

	bool HasTimer { get; }

	uint EventItemID { get; }

	int RecommendedLevel { get; }

	int GetTimeRemaining(int currentTimestamp);

	unsafe EventHandler* ToStruct();

	static IEventHandler Create(nint address)
	{
		return new EventHandler(address);
	}
}
