using System;
using System.Collections.Generic;
using System.Numerics;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;

namespace OmenTools.Dalamud;

public static class BossModIPC
{
	public const string INTERNAL_NAME = "BossMod";

	public const string REBORN_INTERNAL_NAME = "BossModReborn";

	[IPCSubscriber("BossMod.HasModuleByDataId", DefaultValue = "false")]
	private static IPCSubscriber<uint, bool>? HasModuleByDataID;

	[IPCSubscriber("BossMod.Configuration", DefaultValue = "false")]
	private static IPCSubscriber<List<string>, bool, bool>? Configuration;

	[IPCSubscriber("BossMod.Configuration.LastModified")]
	private static IPCSubscriber<DateTime>? ConfigurationLastModified;

	[IPCSubscriber("BossMod.Configuration.DisableModule", DefaultValue = "false")]
	private static IPCSubscriber<string, bool, bool>? ConfigurationDisableModule;

	[IPCSubscriber("BossMod.Rotation.ActionQueue.HasEntries", DefaultValue = "false")]
	private static IPCSubscriber<bool>? RotationActionQueueHasEntries;

	[IPCSubscriber("BossMod.Presets.Get")]
	private static IPCSubscriber<string, string?>? PresetsGet;

	[IPCSubscriber("BossMod.Presets.Create", DefaultValue = "false")]
	private static IPCSubscriber<string, bool, bool>? PresetsCreate;

	[IPCSubscriber("BossMod.Presets.Delete", DefaultValue = "false")]
	private static IPCSubscriber<string, bool>? PresetsDelete;

	[IPCSubscriber("BossMod.Presets.GetActive")]
	private static IPCSubscriber<string?>? PresetsGetActive;

	[IPCSubscriber("BossMod.Presets.SetActive", DefaultValue = "false")]
	private static IPCSubscriber<string, bool>? PresetsSetActive;

	[IPCSubscriber("BossMod.Presets.ClearActive", DefaultValue = "false")]
	private static IPCSubscriber<bool>? PresetsClearActive;

	[IPCSubscriber("BossMod.Presets.GetForceDisabled", DefaultValue = "false")]
	private static IPCSubscriber<bool>? PresetsGetForceDisabled;

	[IPCSubscriber("BossMod.Presets.SetForceDisabled", DefaultValue = "false")]
	private static IPCSubscriber<bool>? PresetsSetForceDisabled;

	[IPCSubscriber("BossMod.Presets.Activate", DefaultValue = "false")]
	private static IPCSubscriber<string, bool>? PresetsActivate;

	[IPCSubscriber("BossMod.Presets.Deactivate", DefaultValue = "false")]
	private static IPCSubscriber<string, bool>? PresetsDeactivate;

	[IPCSubscriber("BossMod.Presets.GetActiveList")]
	private static IPCSubscriber<List<string>>? PresetsGetActiveList;

	[IPCSubscriber("BossMod.Presets.SetActiveList", DefaultValue = "false")]
	private static IPCSubscriber<List<string>, bool>? PresetsSetActiveList;

	[IPCSubscriber("BossMod.Presets.AddTransientStrategy", DefaultValue = "false")]
	private static IPCSubscriber<string, string, string, string, bool>? PresetsAddTransientStrategy;

	[IPCSubscriber("BossMod.Presets.AddTransientStrategyTargetEnemyOID", DefaultValue = "false")]
	private static IPCSubscriber<string, string, string, string, int, bool>? PresetsAddTransientStrategyTargetEnemyOID;

	[IPCSubscriber("BossMod.Presets.ClearTransientStrategy", DefaultValue = "false")]
	private static IPCSubscriber<string, string, string, bool>? PresetsClearTransientStrategy;

	[IPCSubscriber("BossMod.Presets.ClearTransientModuleStrategies", DefaultValue = "false")]
	private static IPCSubscriber<string, string, bool>? PresetsClearTransientModuleStrategies;

	[IPCSubscriber("BossMod.Presets.ClearTransientPresetStrategies", DefaultValue = "false")]
	private static IPCSubscriber<string, bool>? PresetsClearTransientPresetStrategies;

	[IPCSubscriber("BossMod.ObstacleMap.Generate", DefaultValue = "false")]
	private static IPCSubscriber<Vector3, float, bool, bool>? ObstacleMapGenerate;

	[IPCSubscriber("BossMod.ObstacleMap.GetGenerationStatus", DefaultValue = "0")]
	private static IPCSubscriber<int>? ObstacleMapGetGenerationStatus;

	[IPCSubscriber("BossMod.ObstacleMap.HasTempMap", DefaultValue = "false")]
	private static IPCSubscriber<bool>? ObstacleMapHasTempMap;

	[IPCSubscriber("BossMod.ObstacleMap.ClearTempMap", DefaultValue = "false")]
	private static IPCSubscriber<bool>? ObstacleMapClearTempMap;

	[IPCSubscriber("BossMod.ObstacleMap.EvaluateTempMapQuality", DefaultValue = "false")]
	private static IPCSubscriber<bool>? ObstacleMapEvaluateTempMapQuality;

	public static bool IsPluginEnabled()
	{
		if (!DalamudServiceExtension.Instance().IsPluginEnabled("BossMod"))
		{
			return DalamudServiceExtension.Instance().IsPluginEnabled("BossModReborn");
		}
		return true;
	}

	public static bool GetHasModuleByDataID(uint dataID)
	{
		return HasModuleByDataID?.InvokeFunc(dataID) ?? false;
	}

	public static bool InvokeConfiguration(List<string> args, bool save)
	{
		return Configuration?.InvokeFunc(args, save) ?? false;
	}

	public static DateTime GetConfigurationLastModified()
	{
		return ConfigurationLastModified?.InvokeFunc() ?? DateTime.MinValue;
	}

	public static bool DisableModule(string name, bool disable)
	{
		return ConfigurationDisableModule?.InvokeFunc(name, disable) ?? false;
	}

	public static bool GetHasRotationActionQueueEntries()
	{
		IPCSubscriber<bool> rotationActionQueueHasEntries = RotationActionQueueHasEntries;
		if (rotationActionQueueHasEntries == null)
		{
			return false;
		}
		return rotationActionQueueHasEntries;
	}

	public static string? GetPreset(string name)
	{
		return PresetsGet?.InvokeFunc(name);
	}

	public static bool CreatePreset(string presetSerialized, bool overwrite)
	{
		return PresetsCreate?.InvokeFunc(presetSerialized, overwrite) ?? false;
	}

	public static bool DeletePreset(string name)
	{
		return PresetsDelete?.InvokeFunc(name) ?? false;
	}

	public static string? GetActivePreset()
	{
		return PresetsGetActive?.InvokeFunc();
	}

	public static bool SetActivePreset(string name)
	{
		return PresetsSetActive?.InvokeFunc(name) ?? false;
	}

	public static bool ClearActivePresets()
	{
		IPCSubscriber<bool> presetsClearActive = PresetsClearActive;
		if (presetsClearActive == null)
		{
			return false;
		}
		return presetsClearActive;
	}

	public static bool GetIsForceDisabled()
	{
		IPCSubscriber<bool> presetsGetForceDisabled = PresetsGetForceDisabled;
		if (presetsGetForceDisabled == null)
		{
			return false;
		}
		return presetsGetForceDisabled;
	}

	public static bool SetForceDisabled()
	{
		IPCSubscriber<bool> presetsSetForceDisabled = PresetsSetForceDisabled;
		if (presetsSetForceDisabled == null)
		{
			return false;
		}
		return presetsSetForceDisabled;
	}

	public static bool ActivatePreset(string name)
	{
		return PresetsActivate?.InvokeFunc(name) ?? false;
	}

	public static bool DeactivatePreset(string name)
	{
		return PresetsDeactivate?.InvokeFunc(name) ?? false;
	}

	public static List<string> GetActivePresetList()
	{
		return PresetsGetActiveList?.InvokeFunc() ?? new List<string>();
	}

	public static bool SetActivePresetList(List<string> names)
	{
		return PresetsSetActiveList?.InvokeFunc(names) ?? false;
	}

	public static bool AddTransientStrategy(string presetName, string moduleTypeName, string trackName, string value)
	{
		return PresetsAddTransientStrategy?.InvokeFunc(presetName, moduleTypeName, trackName, value) ?? false;
	}

	public static bool AddTransientStrategyTargetEnemyOID(string presetName, string moduleTypeName, string trackName, string value, int oid)
	{
		return PresetsAddTransientStrategyTargetEnemyOID?.InvokeFunc(presetName, moduleTypeName, trackName, value, oid) ?? false;
	}

	public static bool ClearTransientStrategy(string presetName, string moduleTypeName, string trackName)
	{
		return PresetsClearTransientStrategy?.InvokeFunc(presetName, moduleTypeName, trackName) ?? false;
	}

	public static bool ClearTransientModuleStrategies(string presetName, string moduleTypeName)
	{
		return PresetsClearTransientModuleStrategies?.InvokeFunc(presetName, moduleTypeName) ?? false;
	}

	public static bool ClearTransientPresetStrategies(string presetName)
	{
		return PresetsClearTransientPresetStrategies?.InvokeFunc(presetName) ?? false;
	}

	public static bool GenerateObstacleMap(Vector3 centerWorld, float radius, bool writeToFile)
	{
		return ObstacleMapGenerate?.InvokeFunc(centerWorld, radius, writeToFile) ?? false;
	}

	public static int GetObstacleMapGenerationStatus()
	{
		IPCSubscriber<int> obstacleMapGetGenerationStatus = ObstacleMapGetGenerationStatus;
		if (obstacleMapGetGenerationStatus == null)
		{
			return 0;
		}
		return obstacleMapGetGenerationStatus;
	}

	public static bool GetHasTempObstacleMap()
	{
		IPCSubscriber<bool> obstacleMapHasTempMap = ObstacleMapHasTempMap;
		if (obstacleMapHasTempMap == null)
		{
			return false;
		}
		return obstacleMapHasTempMap;
	}

	public static bool ClearTempObstacleMap()
	{
		IPCSubscriber<bool> obstacleMapClearTempMap = ObstacleMapClearTempMap;
		if (obstacleMapClearTempMap == null)
		{
			return false;
		}
		return obstacleMapClearTempMap;
	}

	public static bool EvaluateTempObstacleMapQuality()
	{
		IPCSubscriber<bool> obstacleMapEvaluateTempMapQuality = ObstacleMapEvaluateTempMapQuality;
		if (obstacleMapEvaluateTempMapQuality == null)
		{
			return false;
		}
		return obstacleMapEvaluateTempMapQuality;
	}
}
