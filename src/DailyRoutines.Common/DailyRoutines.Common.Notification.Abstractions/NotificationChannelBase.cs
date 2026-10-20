using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.Common.Notification.Abstractions;

public abstract class NotificationChannelBase
{
	[CompilerGenerated]
	private static string _003CCacheDirectory_003Ek__BackingField;

	public string ConfigFilePath
	{
		get
		{
			string text = Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), "Notification");
			Directory.CreateDirectory(text);
			return Path.Join(text, GetType().Name + ".json");
		}
	}

	public bool IsDisposed { get; private set; }

	public bool IsInitialized { get; private set; }

	protected static string CacheDirectory
	{
		get
		{
			if (!string.IsNullOrEmpty(_003CCacheDirectory_003Ek__BackingField))
			{
				return _003CCacheDirectory_003Ek__BackingField;
			}
			string text = Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), "Notification", "Cache");
			Directory.CreateDirectory(text);
			_003CCacheDirectory_003Ek__BackingField = text;
			return text;
		}
	}

	public async Task PublicInitAsync()
	{
		if (IsDisposed || IsInitialized)
		{
			return;
		}
		try
		{
			await Init();
		}
		catch (Exception ex)
		{
			DLog.Error("初始化通知渠道失败", ex);
			await PublicUninitAsync();
		}
		finally
		{
			IsInitialized = true;
		}
	}

	public async Task PublicUninitAsync()
	{
		if (IsDisposed)
		{
			return;
		}
		try
		{
			await Uninit();
		}
		catch (Exception ex)
		{
			DLog.Error("卸载通知渠道失败", ex);
		}
		finally
		{
			IsDisposed = true;
		}
	}

	internal T? LoadConfig<T>() where T : NotificationChannelConfig
	{
		try
		{
			if (!File.Exists(ConfigFilePath))
			{
				return null;
			}
			return JsonConvert.DeserializeObject<T>(File.ReadAllText(ConfigFilePath), JsonSerializerSettingsExtension.GetShared());
		}
		catch (Exception ex)
		{
			DLog.Error("为通知渠道加载配置失败: " + GetType().Name, ex);
			return null;
		}
	}

	internal void SaveConfig<T>(T config) where T : NotificationChannelConfig
	{
		try
		{
			ArgumentNullException.ThrowIfNull(config, "config");
			string content = JsonConvert.SerializeObject((object)config, (Formatting)1, JsonSerializerSettingsExtension.GetShared());
			OmenServiceBase<SecureSaveHelper>.Instance().WriteAllText(ConfigFilePath, content);
		}
		catch (Exception ex)
		{
			DLog.Error("为通知渠道加载配置失败: " + GetType().Name, ex);
		}
	}

	protected virtual Task Init()
	{
		return Task.CompletedTask;
	}

	protected virtual Task Uninit()
	{
		return Task.CompletedTask;
	}

	public virtual void ConfigUI()
	{
	}
}
