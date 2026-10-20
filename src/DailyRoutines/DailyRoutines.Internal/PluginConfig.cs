using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Info;
using DailyRoutines.Manager;
using Dalamud.Configuration;
using Lumina.Data;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.Internal;

[Serializable]
public class PluginConfig : IPluginConfiguration
{
	private static PluginConfig? InstanceInternal;

	[CompilerGenerated]
	private Language _003CUILanguage_003Ek__BackingField;

	public ConflictKey ConflictKeyBinding { get; set; }

	public Language UILanguage
	{
		[CompilerGenerated]
		get
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return _003CUILanguage_003Ek__BackingField;
		}
		set
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (!OmenServiceBase<LocalizationManager>.Instance().IsConfigured)
			{
				_003CUILanguage_003Ek__BackingField = value;
			}
			else
			{
				_003CUILanguage_003Ek__BackingField = LanguageManager.orJSOvTK6K(value);
			}
		}
	}

	public float LeftTabWidth { get; set; }

	public int DefaultHomePage { get; set; }

	public bool SendOnlineAuthInfo { get; set; }

	public bool ShowTrayOverlay { get; set; }

	public float TrayOverlaySize { get; set; }

	public string PremiumCredential { get; set; }

	public int InterfaceStyleV2Index { get; set; }

	public Dictionary<string, bool> ModuleEnabled { get; set; }

	public HashSet<string> ModuleFavorites { get; set; }

	public string LastVersion { get; set; }

	public bool DefaultEnableModules { get; set; }

	public bool CompactMode { get; set; }

	public List<(string Path, bool IsEnabled)> ModuleLocal { get; set; }

	public Dictionary<string, HashSet<string>> ModuleCustoms { get; set; }

	public int Version { get; set; }

	public bool CorrectEnvironment { get; set; }

	public bool SkipLogo { get; set; }

	public bool PreventAFK { get; set; }

	public bool StripeDRPrefix { get; set; }

	public static PluginConfig Instance()
	{
		if (InstanceInternal != null)
		{
			return InstanceInternal;
		}
		Reload();
		return InstanceInternal;
	}

	public bool IsLastVersionDefault()
	{
		return LastVersion == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F347C5C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862);
	}

	internal static void Reload()
	{
		InstanceInternal = (DalamudServiceExtension.Instance().GetPluginConfig() as PluginConfig) ?? new PluginConfig();
		InstanceInternal.Save();
	}

	internal void Save()
	{
		DalamudServiceExtension.Instance().SavePluginConfig((IPluginConfiguration)(object)this);
	}

	public PluginConfig()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		ConflictKeyBinding = new ConflictKey();
		LeftTabWidth = 300f;
		DefaultHomePage = 3;
		SendOnlineAuthInfo = true;
		ShowTrayOverlay = true;
		TrayOverlaySize = 48f;
		PremiumCredential = string.Empty;
		ModuleEnabled = new Dictionary<string, bool>();
		ModuleFavorites = new HashSet<string>();
		LastVersion = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C7883DA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00);
		DefaultEnableModules = false;
		ModuleLocal = new List<(string, bool)>();
		ModuleCustoms = new Dictionary<string, HashSet<string>>();
		CorrectEnvironment = true;
		SkipLogo = true;
		PreventAFK = true;
		base._002Ector();
	}
}
