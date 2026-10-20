using System;
using System.IO;
using Newtonsoft.Json;
using OmenTools.Dalamud;
using OmenTools.Extensions;

namespace OmenTools.OmenService.Abstractions;

public abstract class OmenServiceBase<T> : OmenServiceBase where T : OmenServiceBase<T>
{
	public static T Instance()
	{
		return DService.Instance().GetOmenService<T>() ?? throw new InvalidOperationException("服务 " + typeof(T).Name + " 尚未注册或初始化");
	}
}
public abstract class OmenServiceBase
{
	internal string ConfigFilePath
	{
		get
		{
			string text = Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), "OmenTools");
			Directory.CreateDirectory(text);
			string text2 = Path.Join(text, "Service");
			Directory.CreateDirectory(text2);
			return Path.Join(text2, GetType().Name + ".json");
		}
	}

	internal bool IsDisposed { get; private set; }

	internal bool IsInitialized { get; private set; }

	internal void PublicInit()
	{
		if (IsDisposed || IsInitialized)
		{
			return;
		}
		try
		{
			Init();
			IsInitialized = true;
		}
		catch (Exception ex)
		{
			DLog.Error("初始化 OmenService 时发生错误: " + GetType().Name, ex);
			PublicUninit();
		}
	}

	internal void PublicUninit()
	{
		if (IsDisposed)
		{
			return;
		}
		try
		{
			Uninit();
		}
		catch (Exception ex)
		{
			DLog.Error("卸载 OmenService 时发生错误: " + GetType().Name, ex);
		}
		finally
		{
			IsDisposed = true;
		}
	}

	protected virtual void Init()
	{
	}

	protected virtual void Uninit()
	{
	}

	internal T? LoadConfig<T>() where T : OmenServiceConfig
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
			DLog.Error("加载 OmenService 配置失败: " + GetType().Name, ex);
			return null;
		}
	}

	internal void SaveConfig<T>(T config) where T : OmenServiceConfig
	{
		try
		{
			ArgumentNullException.ThrowIfNull(config, "config");
			string content = JsonConvert.SerializeObject((object)config, (Formatting)1, JsonSerializerSettingsExtension.GetShared());
			OmenServiceBase<SecureSaveHelper>.Instance().WriteAllText(ConfigFilePath, content);
		}
		catch (Exception ex)
		{
			DLog.Error("保存 OmenService 配置失败: " + GetType().Name, ex);
		}
	}
}
