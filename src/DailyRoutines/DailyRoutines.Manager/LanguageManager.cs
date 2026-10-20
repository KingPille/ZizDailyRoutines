using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using DailyRoutines.Common.Manager.Extensions;
using DailyRoutines.Internal;
using Dalamud.Plugin.Services;
using Lumina.Data;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Localization;
using OmenTools.Localization.Parsers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.Manager;

[ManagerInitOrder(0)]
public sealed class LanguageManager : ManagerBase<LanguageManager>
{
	public class Config : ManagerConfig
	{
		public DateTimeOffset LastLocalizationUpdate;

		public Config()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			LastLocalizationUpdate = DateTimeOffset.MinValue;
			base._002Ector();
		}
	}

	private Config config;

	private int isUpdating;

	private const string REMOTE_LOCALIZATION_REPO_OWNER = "Dalamud-DailyRoutines";

	private const string REMOTE_LOCALIZATION_REPO_NAME = "DailyRoutines.Localizations";

	private const string REMOTE_LOCALIZATION_BRANCH = "main";

	private const string REMOTE_LOCALIZATION_BASE_RAW = "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines.Localizations/main/";

	private static readonly TimeSpan RemoteUpdateInterval;

	private static readonly JsonDictionaryLocalizationParser DictionaryParser;

	internal static FrozenDictionary<Language, string> LanguageInfo { get; }

	internal static FrozenDictionary<Language, string> AvailableLanguages => OmenServiceBase<LocalizationManager>.Instance().AvailableLanguages;

	private static string LanguageDirectory { get; }

	internal static Language orJSOvTK6K(Language language)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return OmenServiceBase<LocalizationManager>.Instance().NormalizeLanguage(language, GameState.ClientLanguage);
	}

	public static void LoadLanguage(Language language)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		OmenServiceBase<LocalizationManager>.Instance().LoadLanguage(language);
	}

	public static string Get(string key)
	{
		return OmenServiceBase<LocalizationManager>.Instance().Get(key);
	}

	public static string Get(string key, params object[] args)
	{
		return OmenServiceBase<LocalizationManager>.Instance().Get(key, args);
	}

	public static string Get(string key, IReadOnlyDictionary<string, object> args)
	{
		return OmenServiceBase<LocalizationManager>.Instance().Get(key, args);
	}

	public static ReadOnlySeString GetSe(string key, params object[] args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return OmenServiceBase<LocalizationManager>.Instance().GetSe(key, args);
	}

	public static ReadOnlySeString GetSe(string key, IReadOnlyDictionary<string, object> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return OmenServiceBase<LocalizationManager>.Instance().GetSe(key, args);
	}

	protected override Task Init()
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		Directory.CreateDirectory(LanguageDirectory);
		config = ManagerConfigExtension.Load<Config>(this) ?? new Config();
		OmenServiceBase<LocalizationManager>.Instance().Configure(new LocalizationOptions
		{
			SupportedLanguages = LanguageInfo,
			DefaultLanguage = (Language)2,
			FileNameResolver = delegate(Language language)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
				defaultInterpolatedStringHandler.AppendFormatted<Language>(language);
				defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A22688 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
				return defaultInterpolatedStringHandler.ToStringAndClear();
			},
			Source = new FileLocalizationSource(LanguageDirectory),
			Parser = DictionaryParser,
			FallbackResolver = delegate(Language language)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Invalid comparison between Unknown and I4
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Invalid comparison between Unknown and I4
				if ((int)language == 5)
				{
					return new global::_003C_003Ez__ReadOnlySingleElementList<Language>((Language)5);
				}
				return ((int)language == 8) ? ((IEnumerable<Language>)new global::_003C_003Ez__ReadOnlySingleElementList<Language>((Language)5)) : ((IEnumerable<Language>)Array.Empty<Language>());
			},
			EnableHotReload = true,
			ReloadDebounce = TimeSpan.FromSeconds(3L),
			LoggerTag = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -431024363 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52)
		}, GameState.ClientLanguage, GameState.ClientLanguage);
		PluginConfig pluginConfig = PluginConfig.Instance();
		pluginConfig.UILanguage = orJSOvTK6K(pluginConfig.UILanguage);
		pluginConfig.Save();
		LoadLanguage(pluginConfig.UILanguage);
		return Task.CompletedTask;
	}

	protected override Task PostInit()
	{
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(TxoSKxjHUS);
		return Task.CompletedTask;
	}

	private void TxoSKxjHUS(IFramework _)
	{
	}

	private async Task vCQStMJUOV(CancellationToken token)
	{
		HttpClient httpClient = OmenServiceBase<HTTPClientHelper>.Instance().Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9D6C9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a), delegate(HttpClient client)
		{
			client.Timeout = TimeSpan.FromSeconds(30L);
		});
		ImmutableArray<Language>.Enumerator enumerator = LanguageInfo.Keys.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Language current = enumerator.Current;
			token.ThrowIfCancellationRequested();
			await qgZSWhZwxS(httpClient, current, token);
		}
		config.LastLocalizationUpdate = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset;
		config.Save(this);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 2);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5B8AD0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
		defaultInterpolatedStringHandler.AppendFormatted(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9075686 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3));
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339CA43E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de));
		defaultInterpolatedStringHandler.AppendFormatted(config.LastLocalizationUpdate + RemoteUpdateInterval);
		DLog.Debug(defaultInterpolatedStringHandler.ToStringAndClear());
	}

	private static async Task qgZSWhZwxS(HttpClient httpClient, Language language, CancellationToken token)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 1);
		defaultInterpolatedStringHandler.AppendFormatted<Language>(language);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBB1D9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
		string fileName = defaultInterpolatedStringHandler.ToStringAndClear();
		Path.Join(LanguageDirectory, fileName);
		_ = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642A990C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df) + fileName;
		try
		{
			_ = httpClient;
			_ = token;
			throw null;
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379AEB3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09) + fileName, ex2);
		}
	}

	private static Dictionary<string, string>? BEXSTXdUIN(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}
		try
		{
			using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous);
			return DictionaryParser.Parse(stream).ToDictionary<KeyValuePair<string, string>, string, string>((KeyValuePair<string, string> kv) => kv.Key, (KeyValuePair<string, string> kv) => kv.Value, StringComparer.Ordinal);
		}
		catch (Exception ex)
		{
			DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B20E3C0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a) + Path.GetFileName(path), ex);
			return null;
		}
	}

	public LanguageManager()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static LanguageManager()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		LanguageInfo = new Dictionary<Language, string>
		{
			[(Language)2] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D148F39 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275),
			[(Language)5] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -1664078284 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e),
			[(Language)8] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C788930 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00),
			[(Language)1] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5BFDE6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a),
			[(Language)7] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D3C318 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1),
			[(Language)4] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDBCA3F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1),
			[(Language)3] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A83295 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a)
		}.ToFrozenDictionary();
		LanguageDirectory = Path.Join(Path.GetDirectoryName(DalamudServiceExtension.Instance().AssemblyLocation.FullName), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x55C0237A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFB62F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
		RemoteUpdateInterval = TimeSpan.FromHours(12);
		DictionaryParser = new JsonDictionaryLocalizationParser();
	}

	[CompilerGenerated]
	private async Task bTLS167ukE(CancellationToken cancellationToken)
	{
		try
		{
			await vCQStMJUOV(cancellationToken);
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768FF05B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46), ex2);
		}
		finally
		{
			Volatile.Write(ref isUpdating, 0);
		}
	}
}
