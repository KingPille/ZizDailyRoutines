using DailyRoutines.Common.Module.Abstractions;

namespace DailyRoutines.Extensions;

public static class ModuleConfigExtension
{
	extension<T>(T config) where T : ModuleConfig
	{
		public static T? Load(ModuleBase instance)
		{
			return instance.LoadConfig<T>();
		}

		public void Save(ModuleBase instance)
		{
			instance.SaveConfig(config);
		}
	}
}
