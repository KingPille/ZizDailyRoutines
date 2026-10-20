using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.System.String;
using OmenTools.Dalamud.Services.Game.Event.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Event;

internal class LuaActor(nint address) : ILuaActor
{
	private unsafe LuaActor* Struct => (LuaActor*)Address;

	public nint Address { get; } = address;

	public unsafe IGameObject? Object => IOmenDalamudService<IObjectTable>.Instance().CreateObjectReference((nint)((LuaActor)Struct).Object);

	public unsafe string Value => ((object)(*(Utf8String*)(&((LuaActor)Struct).LuaString))/*cast due to constrained. prefix*/).ToString();

	public unsafe LuaActor* ToStruct()
	{
		return Struct;
	}
}
