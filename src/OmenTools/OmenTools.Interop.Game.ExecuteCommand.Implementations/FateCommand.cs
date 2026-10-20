using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Abstractions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Interop.Game.ExecuteCommand.Implementations;

public sealed class FateCommand : ExecuteCommandBase
{
	public static void Enter(uint fateID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.EnterFate, fateID);
	}

	public static void SyncLevel(uint fateID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SyncToFateLevel, fateID, 1u);
	}

	public static void UnsyncLevel(uint fateID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SyncToFateLevel, fateID);
	}

	public static void ToggleLevelSync(uint fateID, bool isSync)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.SyncToFateLevelAuto, fateID, isSync ? 1u : 0u);
	}

	public static void Load(uint fateID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LoadFate, fateID);
	}

	public static void Spawn(uint entityID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.LoadFateMob, entityID);
	}

	public static void Start(uint fateID, uint targetObjectID)
	{
		OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.StartFate, fateID, targetObjectID);
	}
}
