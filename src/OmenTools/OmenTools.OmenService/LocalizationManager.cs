using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Data;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Localization;
using OmenTools.Localization.Abstractions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public sealed class LocalizationManager : OmenServiceBase<LocalizationManager>
{
	private sealed record ConfiguredState(LocalizationOptions? Options, EventHandler<LocalizationSourceChangedEventArgs>? ChangeHandler, bool IsConfigured);

	private sealed class LanguageSnapshot(Language language, FrozenDictionary<Language, string> availableLanguages, FrozenDictionary<string, string>[] resources, string loggerTag)
	{
		public static LanguageSnapshot Empty { get; } = new LanguageSnapshot((Language)0, FrozenDictionary<Language, string>.Empty, Array.Empty<FrozenDictionary<string, string>>(), "LocalizationManager");

		public Language Language { get; } = language;

		public FrozenDictionary<Language, string> AvailableLanguages { get; } = availableLanguages;

		public FrozenDictionary<string, string>[] Resources { get; } = resources;

		public string LoggerTag { get; } = loggerTag;

		public ConcurrentDictionary<string, ReadOnlySeString> PlainTextSeCache { get; } = new ConcurrentDictionary<string, ReadOnlySeString>(StringComparer.Ordinal);

		public ConcurrentDictionary<string, byte> MissingKeys { get; } = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);

		public ConcurrentDictionary<string, byte> FormatErrors { get; } = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
	}

	private static readonly FrozenDictionary<string, string> EmptyResource = new Dictionary<string, string>(StringComparer.Ordinal).ToFrozenDictionary(StringComparer.Ordinal);

	private static readonly ConfiguredState EmptyState = new ConfiguredState(null, null, IsConfigured: false);

	private ConfiguredState configuredState = EmptyState;

	private LanguageSnapshot currentSnapshot = LanguageSnapshot.Empty;

	private CancellationTokenSource? reloadCancelSource;

	public bool IsConfigured => Volatile.Read(in configuredState).IsConfigured;

	public FrozenDictionary<Language, string> SupportedLanguages => GetConfiguredState().Options.SupportedLanguages;

	public FrozenDictionary<Language, string> AvailableLanguages => GetSnapshot().AvailableLanguages;

	public Language CurrentLanguage => GetSnapshot().Language;

	protected override void Uninit()
	{
		ResetConfiguredState();
		Interlocked.Exchange(ref currentSnapshot, LanguageSnapshot.Empty);
	}

	public void Configure(LocalizationOptions options, Language initialLanguage, Language preferredLanguage)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		ArgumentNullException.ThrowIfNull(options, "options");
		ValidateOptions(options);
		ConfiguredState configuredState = new ConfiguredState(options, null, IsConfigured: true);
		ConfiguredState state = Interlocked.Exchange(ref this.configuredState, configuredState);
		try
		{
			CancelReload();
			DisposeConfiguredState(state);
			if (options != null && options.EnableHotReload)
			{
				ILocalizationSource source = options.Source;
				if (source != null && source.SupportsChangeNotifications)
				{
					EventHandler<LocalizationSourceChangedEventArgs> eventHandler = OnLocalizationSourceChanged;
					options.Source.ResourceChanged += eventHandler;
					Volatile.Write(ref this.configuredState, configuredState with
					{
						ChangeHandler = eventHandler
					});
				}
			}
			Language language = NormalizeLanguage(initialLanguage, preferredLanguage);
			LoadLanguage(language);
		}
		catch
		{
			CancelReload();
			DisposeConfiguredState(Volatile.Read(in this.configuredState));
			Volatile.Write(ref this.configuredState, EmptyState);
			Interlocked.Exchange(ref currentSnapshot, LanguageSnapshot.Empty);
			throw;
		}
	}

	public Language NormalizeLanguage(Language requestedLanguage, Language preferredLanguage)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		LocalizationOptions options = GetConfiguredState().Options;
		FrozenDictionary<Language, string> availableLanguages = EnumerateAvailableLanguages(options);
		if (IsLanguageAvailable(options, availableLanguages, requestedLanguage))
		{
			return requestedLanguage;
		}
		if (IsLanguageAvailable(options, availableLanguages, preferredLanguage))
		{
			return preferredLanguage;
		}
		foreach (Language item in EnumerateFallbackLanguages(options, requestedLanguage))
		{
			if (IsLanguageAvailable(options, availableLanguages, item))
			{
				return item;
			}
		}
		foreach (Language item2 in EnumerateFallbackLanguages(options, preferredLanguage))
		{
			if (IsLanguageAvailable(options, availableLanguages, item2))
			{
				return item2;
			}
		}
		return options.DefaultLanguage;
	}

	public void LoadLanguage(Language language)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		LocalizationOptions? options = GetConfiguredState().Options;
		if (!options.SupportedLanguages.ContainsKey(language))
		{
			throw new ArgumentOutOfRangeException("language", $"未在已配置语言列表中找到语言 {language}");
		}
		LanguageSnapshot value = BuildSnapshot(options, language);
		Interlocked.Exchange(ref currentSnapshot, value);
	}

	public string Get(string key)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key, "key");
		LanguageSnapshot snapshot = GetSnapshot();
		if (!TryResolveFormat(snapshot, key, out string format))
		{
			return LogMissingKeyAndReturnKey(snapshot, key);
		}
		return format;
	}

	public string Get(string key, params object[] args)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key, "key");
		ArgumentNullException.ThrowIfNull(args, "args");
		if (args.Length == 0)
		{
			return Get(key);
		}
		LanguageSnapshot snapshot = GetSnapshot();
		if (!TryResolveFormat(snapshot, key, out string format))
		{
			return LogMissingKeyAndReturnKey(snapshot, key);
		}
		return string.Format(CultureInfo.CurrentCulture, format, args);
	}

	public string Get(string key, IReadOnlyDictionary<string, object> args)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		ArgumentException.ThrowIfNullOrWhiteSpace(key, "key");
		ArgumentNullException.ThrowIfNull(args, "args");
		LanguageSnapshot snapshot = GetSnapshot();
		if (!TryResolveFormat(snapshot, key, out string format))
		{
			return LogMissingKeyAndReturnKey(snapshot, key);
		}
		return SeStringExtension.FormatText(format, args, snapshot.Language, delegate(string message)
		{
			LogIcuFormatError(snapshot, key, message);
		});
	}

	public ReadOnlySeString GetSe(string key, params object[] args)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		ArgumentException.ThrowIfNullOrWhiteSpace(key, "key");
		ArgumentNullException.ThrowIfNull(args, "args");
		LanguageSnapshot snapshot = GetSnapshot();
		if (!TryResolveFormat(snapshot, key, out string format))
		{
			return CreatePlainSeString(LogMissingKeyAndReturnKey(snapshot, key));
		}
		if (args.Length == 0)
		{
			return snapshot.PlainTextSeCache.GetOrAdd(key, (string _) => CreatePlainSeString(format));
		}
		return SeStringExtension.Format(format, CultureInfo.CurrentCulture, args, delegate(string token)
		{
			LogFormatError(snapshot, key, token);
		});
	}

	public ReadOnlySeString GetSe(string key, IReadOnlyDictionary<string, object> args)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		ArgumentException.ThrowIfNullOrWhiteSpace(key, "key");
		ArgumentNullException.ThrowIfNull(args, "args");
		LanguageSnapshot snapshot = GetSnapshot();
		if (!TryResolveFormat(snapshot, key, out string format))
		{
			return CreatePlainSeString(LogMissingKeyAndReturnKey(snapshot, key));
		}
		return SeStringExtension.Format(format, args, snapshot.Language, delegate(string message)
		{
			LogIcuFormatError(snapshot, key, message);
		});
	}

	private static void LogIcuFormatError(LanguageSnapshot snapshot, string key, string message)
	{
		if (snapshot.FormatErrors.TryAdd(key, 0))
		{
			DLog.Warning($"[{snapshot.LoggerTag}] 本地化键 {key} 的 ICU 格式化失败: {message}");
		}
	}

	private ConfiguredState GetConfiguredState()
	{
		ConfiguredState configuredState = Volatile.Read(in this.configuredState);
		if (configuredState.IsConfigured)
		{
			return configuredState;
		}
		throw new InvalidOperationException("本地化服务尚未配置，请先调用 Configure(...)");
	}

	private LanguageSnapshot GetSnapshot()
	{
		GetConfiguredState();
		return Volatile.Read(in currentSnapshot);
	}

	private static void ValidateOptions(LocalizationOptions options)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		ArgumentNullException.ThrowIfNull(options.SupportedLanguages, "options.SupportedLanguages");
		ArgumentNullException.ThrowIfNull(options.FileNameResolver, "options.FileNameResolver");
		ArgumentNullException.ThrowIfNull(options.Source, "options.Source");
		ArgumentNullException.ThrowIfNull(options.Parser, "options.Parser");
		ArgumentNullException.ThrowIfNull(options.FallbackResolver, "options.FallbackResolver");
		if (options.SupportedLanguages.Count == 0)
		{
			throw new ArgumentException("必须至少配置一种支持语言", "options");
		}
		if (!options.SupportedLanguages.ContainsKey(options.DefaultLanguage))
		{
			throw new ArgumentException($"默认语言 {options.DefaultLanguage} 不在支持语言列表中", "options");
		}
		if (options.ReloadDebounce < TimeSpan.Zero)
		{
			throw new ArgumentOutOfRangeException("options", "热重载防抖时间不能小于 0");
		}
		ArgumentException.ThrowIfNullOrWhiteSpace(options.LoggerTag, "options.LoggerTag");
	}

	private void ResetConfiguredState()
	{
		CancelReload();
		DisposeConfiguredState(Interlocked.Exchange(ref configuredState, EmptyState));
	}

	private void CancelReload()
	{
		CancellationTokenSource? cancellationTokenSource = Interlocked.Exchange(ref reloadCancelSource, null);
		cancellationTokenSource?.Cancel();
		cancellationTokenSource?.Dispose();
	}

	private static void DisposeConfiguredState(ConfiguredState state)
	{
		if (state.IsConfigured && state.Options != null)
		{
			if (state.ChangeHandler != null)
			{
				state.Options.Source.ResourceChanged -= state.ChangeHandler;
			}
			state.Options.Source.Dispose();
		}
	}

	private void OnLocalizationSourceChanged(object? sender, LocalizationSourceChangedEventArgs e)
	{
		ConfiguredState configuredState = Volatile.Read(in this.configuredState);
		if (!configuredState.IsConfigured || configuredState.Options == null)
		{
			return;
		}
		CancelReload();
		CancellationTokenSource cancelSource = new CancellationTokenSource();
		CancellationTokenSource? cancellationTokenSource = Interlocked.Exchange(ref reloadCancelSource, cancelSource);
		cancellationTokenSource?.Cancel();
		cancellationTokenSource?.Dispose();
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
		{
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			if (!cancelSource.IsCancellationRequested)
			{
				ConfiguredState configuredState2 = Volatile.Read(in this.configuredState);
				if (configuredState2.IsConfigured && configuredState2.Options != null)
				{
					DLog.Debug($"[{configuredState2.Options.LoggerTag}] 本地化资源 {e.ResourceName} ({e.ChangeType}) 发生变动，开始重载当前语言");
					try
					{
						LoadLanguage(CurrentLanguage);
					}
					catch (Exception ex)
					{
						DLog.Error("[" + configuredState2.Options.LoggerTag + "] 重载当前语言失败", ex);
					}
				}
			}
		}, configuredState.Options.ReloadDebounce, 0, cancelSource.Token);
	}

	private static LanguageSnapshot BuildSnapshot(LocalizationOptions options, Language language)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		List<Language> list = EnumerateResourceLanguages(options, language);
		List<FrozenDictionary<string, string>> list2 = new List<FrozenDictionary<string, string>>(list.Count);
		foreach (Language item in list)
		{
			FrozenDictionary<string, string> frozenDictionary = LoadLanguageResource(options, item);
			if (frozenDictionary.Count != 0)
			{
				list2.Add(frozenDictionary);
			}
		}
		return new LanguageSnapshot(language, EnumerateAvailableLanguages(options), list2.ToArray(), options.LoggerTag);
	}

	private static List<Language> EnumerateResourceLanguages(LocalizationOptions options, Language language)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		HashSet<Language> deduped = new HashSet<Language>();
		List<Language> ordered = new List<Language>();
		AddLanguage(language);
		foreach (Language item in EnumerateFallbackLanguages(options, language))
		{
			AddLanguage(item);
		}
		AddLanguage(options.DefaultLanguage);
		return ordered;
		void AddLanguage(Language value)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (options.SupportedLanguages.ContainsKey(value) && deduped.Add(value))
			{
				ordered.Add(value);
			}
		}
	}

	private static IEnumerable<Language> EnumerateFallbackLanguages(LocalizationOptions options, Language language)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!options.SupportedLanguages.ContainsKey(language))
		{
			yield break;
		}
		foreach (Language item in options.FallbackResolver(language))
		{
			yield return item;
		}
	}

	private static bool IsLanguageAvailable(LocalizationOptions options, FrozenDictionary<Language, string> availableLanguages, Language language)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (options.SupportedLanguages.ContainsKey(language))
		{
			return availableLanguages.ContainsKey(language);
		}
		return false;
	}

	private static bool TryResolveFormat(LanguageSnapshot snapshot, string key, out string format)
	{
		FrozenDictionary<string, string>[] resources = snapshot.Resources;
		for (int i = 0; i < resources.Length; i++)
		{
			if (resources[i].TryGetValue(key, out format))
			{
				return true;
			}
		}
		format = string.Empty;
		return false;
	}

	private static FrozenDictionary<string, string> LoadLanguageResource(LocalizationOptions options, Language language)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string text = options.FileNameResolver(language);
			if (string.IsNullOrWhiteSpace(text))
			{
				throw new InvalidOperationException($"语言 {language} 的资源文件名不能为空");
			}
			using Stream stream = options.Source.OpenRead(language, text);
			if (stream == null)
			{
				return EmptyResource;
			}
			FrozenDictionary<string, string> frozenDictionary = options.Parser.Parse(stream);
			return (FrozenDictionary<string, string>)((frozenDictionary.Count == 0) ? ((IDictionary)EmptyResource) : ((IDictionary)frozenDictionary));
		}
		catch (Exception ex)
		{
			DLog.Error($"[{options.LoggerTag}] 读取 {language} 语言数据失败", ex);
			return EmptyResource;
		}
	}

	private static FrozenDictionary<Language, string> EnumerateAvailableLanguages(LocalizationOptions options)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Language, string> dictionary = new Dictionary<Language, string>();
		foreach (KeyValuePair<Language, string> supportedLanguage in options.SupportedLanguages)
		{
			string text = options.FileNameResolver(supportedLanguage.Key);
			if (!string.IsNullOrWhiteSpace(text) && options.Source.Exists(supportedLanguage.Key, text))
			{
				dictionary[supportedLanguage.Key] = supportedLanguage.Value;
			}
		}
		if (dictionary.Count != 0)
		{
			return dictionary.ToFrozenDictionary();
		}
		return FrozenDictionary<Language, string>.Empty;
	}

	private unsafe static ReadOnlySeString CreatePlainSeString(string text)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			((RentedSeStringBuilder)(ref val)).Builder.Append(text);
			return ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString();
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private static string LogMissingKeyAndReturnKey(LanguageSnapshot snapshot, string key)
	{
		if (snapshot.MissingKeys.TryAdd(key, 0))
		{
			DLog.Error("[" + snapshot.LoggerTag + "] 未在当前语言链中找到本地化键 " + key);
		}
		return key;
	}

	private static void LogFormatError(LanguageSnapshot snapshot, string key, string token)
	{
		string key2 = key + "|" + token;
		if (snapshot.FormatErrors.TryAdd(key2, 0))
		{
			DLog.Warning($"[{snapshot.LoggerTag}] 本地化键 {key} 缺少占位符参数 {token}，已保留原始文本");
		}
	}
}
