using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Common.Lua;
using OmenTools.Dalamud.Services.Game.Event.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Event;

internal class EventHandler(nint address) : IEventHandler
{
	private unsafe EventHandler* Struct => (EventHandler*)Address;

	public nint Address { get; } = address;

	public unsafe EventId EventID => ((EventHandler)Struct).GetEventId();

	public unsafe uint IconID => ((EventHandler)Struct).IconId;

	public unsafe short Scene => ((EventHandler)Struct).Scene;

	public unsafe IGameObject? SceneGameObject => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((EventHandler)Struct).SceneGameObject);

	public unsafe SceneFlag SceneFlags => ((EventHandler)Struct).SceneFlags;

	public unsafe LuaStatus LuaStatus => ((EventHandler)Struct).LuaStatus;

	public unsafe bool HasTimer => ((EventHandler)Struct).HasTimer();

	public unsafe uint EventItemID => ((EventHandler)Struct).GetEventItemId();

	public unsafe int RecommendedLevel => ((EventHandler)Struct).GetRecommendedLevel();

	public unsafe int GetTimeRemaining(int currentTimestamp)
	{
		return ((EventHandler)Struct).GetTimeRemaining(currentTimestamp);
	}

	public unsafe EventHandler* ToStruct()
	{
		return Struct;
	}
}
