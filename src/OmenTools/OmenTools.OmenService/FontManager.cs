using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.GameFonts;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class FontManager : OmenServiceBase<FontManager>
{
	public sealed class FontManagerConfig : OmenServiceConfig
	{
		public float FontSize = 20f;

		public string FontFileName = string.Empty;

		public void Save()
		{
			this.Save(OmenServiceBase<FontManager>.Instance());
		}
	}

	private static readonly ushort[] DefaultFontRange;

	private readonly SemaphoreSlim semaphore = new SemaphoreSlim(Environment.ProcessorCount, Environment.ProcessorCount);

	private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();

	private readonly ConcurrentDictionary<float, Task<IFontHandle>> fontTasks = new ConcurrentDictionary<float, Task<IFontHandle>>();

	private volatile int activeFontBuilds;

	private readonly Lazy<IFontHandle> axisFont360Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)5)));

	private readonly Lazy<IFontHandle> axisFont180Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)4)));

	private readonly Lazy<IFontHandle> axisFont120Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)2)));

	private readonly Lazy<IFontHandle> axisFont140Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)3)));

	private readonly Lazy<IFontHandle> axisFont96Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)1)));

	private readonly Lazy<IFontHandle> jupiterFont160Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)6)));

	private readonly Lazy<IFontHandle> jupiterFont200Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)7)));

	private readonly Lazy<IFontHandle> jupiterFont230Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)8)));

	private readonly Lazy<IFontHandle> jupiterFont450Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)9)));

	private readonly Lazy<IFontHandle> jupiterFont460Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)10)));

	private readonly Lazy<IFontHandle> jupiterFont900Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)11)));

	private readonly Lazy<IFontHandle> meidingerFont160Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)12)));

	private readonly Lazy<IFontHandle> meidingerFont200Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)13)));

	private readonly Lazy<IFontHandle> meidingerFont400Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)14)));

	private readonly Lazy<IFontHandle> miedingerMidFont100Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)15)));

	private readonly Lazy<IFontHandle> miedingerMidFont120Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)16)));

	private readonly Lazy<IFontHandle> miedingerMidFont140Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)17)));

	private readonly Lazy<IFontHandle> miedingerMidFont180Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)18)));

	private readonly Lazy<IFontHandle> miedingerMidFont360Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)19)));

	private readonly Lazy<IFontHandle> trumpGothicFont184Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)20)));

	private readonly Lazy<IFontHandle> trumpGothicFont230Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)21)));

	private readonly Lazy<IFontHandle> trumpGothicFont340Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)22)));

	private readonly Lazy<IFontHandle> trumpGothicFont680Lazy = new Lazy<IFontHandle>(() => FontAtlasGame.NewGameFontHandle(new GameFontStyle((GameFontFamilyAndSize)23)));

	private static readonly float[] PreBuildFontSizes;

	private static readonly string[] FontExtensions;

	private static IFontAtlas FontAtlasGame => DalamudLeftServiceExtension.Instance().FontAtlas;

	public FontManagerConfig Config { get; private set; }

	public ConcurrentDictionary<string, string> InstalledFonts { get; private set; } = new ConcurrentDictionary<string, string>();

	public IFontHandle AxisFont360 => axisFont360Lazy.Value;

	public IFontHandle AxisFont180 => axisFont180Lazy.Value;

	public IFontHandle AxisFont120 => axisFont120Lazy.Value;

	public IFontHandle AxisFont140 => axisFont140Lazy.Value;

	public IFontHandle AxisFont96 => axisFont96Lazy.Value;

	public IFontHandle JupiterFont160 => jupiterFont160Lazy.Value;

	public IFontHandle JupiterFont200 => jupiterFont200Lazy.Value;

	public IFontHandle JupiterFont230 => jupiterFont230Lazy.Value;

	public IFontHandle JupiterFont450 => jupiterFont450Lazy.Value;

	public IFontHandle JupiterFont460 => jupiterFont460Lazy.Value;

	public IFontHandle JupiterFont900 => jupiterFont900Lazy.Value;

	public IFontHandle MeidingerFont160 => meidingerFont160Lazy.Value;

	public IFontHandle MeidingerFont200 => meidingerFont200Lazy.Value;

	public IFontHandle MeidingerFont400 => meidingerFont400Lazy.Value;

	public IFontHandle MiedingerMidFont100 => miedingerMidFont100Lazy.Value;

	public IFontHandle MiedingerMidFont120 => miedingerMidFont120Lazy.Value;

	public IFontHandle MiedingerMidFont140 => miedingerMidFont140Lazy.Value;

	public IFontHandle MiedingerMidFont180 => miedingerMidFont180Lazy.Value;

	public IFontHandle MiedingerMidFont360 => miedingerMidFont360Lazy.Value;

	public IFontHandle TrumpGothicFont184 => trumpGothicFont184Lazy.Value;

	public IFontHandle TrumpGothicFont230 => trumpGothicFont230Lazy.Value;

	public IFontHandle TrumpGothicFont340 => trumpGothicFont340Lazy.Value;

	public IFontHandle TrumpGothicFont680 => trumpGothicFont680Lazy.Value;

	public IFontHandle UIFont => GetUIFont(1f);

	public IFontHandle UIFont160 => GetUIFont(1.6f);

	public IFontHandle UIFont140 => GetUIFont(1.4f);

	public IFontHandle UIFont120 => GetUIFont(1.2f);

	public IFontHandle UIFont90 => GetUIFont(0.9f);

	public IFontHandle UIFont80 => GetUIFont(0.8f);

	public IFontHandle UIFont60 => GetUIFont(0.6f);

	public float GlobalFontScale => Config.FontSize / 12f * ImGuiHelpers.GlobalScale;

	public bool IsFontBuilding => activeFontBuilds > 0;

	private IFontAtlas? FontAtlas { get; set; }

	private unsafe static ushort[] BuildRange(ushort[]? extraRanges, params ushort*[] nativeRanges)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		ImFontGlyphRangesBuilderPtr val = default(ImFontGlyphRangesBuilderPtr);
		((ImFontGlyphRangesBuilderPtr)(ref val))._002Ector(ImGuiNative.ImFontGlyphRangesBuilder());
		try
		{
			foreach (ushort* ptr in nativeRanges)
			{
				((ImFontGlyphRangesBuilderPtr)(ref val)).AddRanges(ptr);
			}
			if (extraRanges != null && extraRanges.Length > 0)
			{
				fixed (ushort* ptr2 = extraRanges)
				{
					((ImFontGlyphRangesBuilderPtr)(ref val)).AddRanges(ptr2);
				}
			}
			((ImFontGlyphRangesBuilderPtr)(ref val)).AddText(ImU8String.op_Implicit("ΑαΒβΓγΔδΕεΖζΗηΘθΙιΚκΛλΜμΝνΞξΟοΠπΡρΣσΤτΥυΦφΧχΨψΩω"));
			((ImFontGlyphRangesBuilderPtr)(ref val)).AddText(ImU8String.op_Implicit("←→↑↓《》■※☀★★☆♥♡ヅツッシ☀☁☂℃℉°♀♂♠♣♦♣♧®©™€$£♯♭♪✓√◎◆◇♦■□〇●△▽▼▲‹›≤≥≈<«─＼～⅓½¼⅔¾✓✗"));
			((ImFontGlyphRangesBuilderPtr)(ref val)).AddText(ImU8String.op_Implicit("ŒœĂăÂâÎîȘșȚț"));
			((ImFontGlyphRangesBuilderPtr)(ref val)).AddChar((ushort)9450);
			InlineArray5<ushort> buffer = default(InlineArray5<ushort>);
			buffer[0] = 8226;
			buffer[1] = 8226;
			buffer[2] = 9312;
			buffer[3] = 9397;
			buffer[4] = 0;
			Span<ushort> span = buffer;
			fixed (ushort* ptr3 = span)
			{
				((ImFontGlyphRangesBuilderPtr)(ref val)).AddRanges(ptr3);
			}
			return ImGuiHelpers.BuildRangesToArray(val, true, true);
		}
		finally
		{
			((ImFontGlyphRangesBuilderPtr)(ref val)).Destroy();
		}
	}

	public IFontHandle GetUIFont(float scale)
	{
		return GetFont(GetActualFontSize(scale));
	}

	public float GetActualFontSize(float scale)
	{
		return MathF.Round(Config.FontSize * scale * ImGuiHelpers.GlobalScale, 1);
	}

	public IFontHandle GetFont(float size)
	{
		Task<IFontHandle> orAdd = fontTasks.GetOrAdd(size, CreateFontHandleAsync);
		if (orAdd.IsCompletedSuccessfully)
		{
			return orAdd.Result;
		}
		if (orAdd.IsFaulted)
		{
			fontTasks.TryRemove(size, out Task<IFontHandle> _);
			OmenServiceBase<NotifyHelper>.Instance().NotificationError($"字体 (大小: {size}) 构建失败");
			DLog.Error($"字体 (大小: {size}) 构建失败", orAdd.Exception);
		}
		return AxisFont180;
	}

	public async Task RebuildUIFontsAsync(bool clearOld = true)
	{
		if (clearOld)
		{
			ClearFontHandles();
		}
		float[] sizes = PreBuildFontSizes.Select(GetActualFontSize).Distinct().ToArray();
		await semaphore.WaitAsync();
		Interlocked.Increment(ref activeFontBuilds);
		try
		{
			float[] array = sizes;
			foreach (float num in array)
			{
				if (!fontTasks.ContainsKey(num))
				{
					try
					{
						IFontHandle result = CreateFontHandleDefinition(num);
						fontTasks.TryAdd(num, Task.FromResult<IFontHandle>(result));
					}
					catch (Exception ex)
					{
						OmenServiceBase<NotifyHelper>.Instance().NotificationError($"字体 (大小: {num}) 预构建失败");
						DLog.Error($"字体 (大小: {num}) 预构建失败", ex);
					}
				}
			}
			await FontAtlas.BuildFontsAsync();
		}
		finally
		{
			Interlocked.Decrement(ref activeFontBuilds);
			semaphore.Release();
		}
	}

	public Task RegenerateInstalledFontsAsync()
	{
		return Task.Run((Action)RegenerateInstalledFonts, cancelSource.Token);
	}

	protected override void Init()
	{
		Config = LoadConfig<FontManagerConfig>() ?? new FontManagerConfig();
		Task.Run(async delegate
		{
			InlineArray2<Task> buffer = default(InlineArray2<Task>);
			buffer[0] = RebuildUIFontsAsync();
			buffer[1] = Task.Run((Action)RegenerateInstalledFonts, cancelSource.Token);
			await Task.WhenAll(buffer);
		}, cancelSource.Token).ConfigureAwait(continueOnCapturedContext: false);
	}

	protected override void Uninit()
	{
		cancelSource.Cancel();
		cancelSource.Dispose();
		fontTasks.Clear();
		((IDisposable)FontAtlas)?.Dispose();
	}

	private IFontHandle CreateFontHandleDefinition(float size)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		string fontPath = Config.FontFileName;
		if (!File.Exists(fontPath))
		{
			return FontAtlas.NewDelegateFontHandle((FontAtlasBuildStepDelegate)delegate(IFontAtlasBuildToolkit e)
			{
				FontAtlasBuildToolkitUtilities.OnPreBuild(e, (Action<IFontAtlasBuildToolkitPreBuild>)delegate(IFontAtlasBuildToolkitPreBuild tk)
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_005e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0066: Unknown result type (might be due to invalid IL or missing references)
					ImFontPtr mergeFont = tk.AddDalamudDefaultFont(size, DefaultFontRange);
					SafeFontConfig val = default(SafeFontConfig);
					((SafeFontConfig)(ref val))._002Ector();
					((SafeFontConfig)(ref val)).SizePx = size;
					((SafeFontConfig)(ref val)).PixelSnapH = true;
					((SafeFontConfig)(ref val)).MergeFont = mergeFont;
					ImFontPtr mergeFont2 = tk.AddGameSymbol(ref val);
					((SafeFontConfig)(ref val))._002Ector();
					((SafeFontConfig)(ref val)).SizePx = size;
					((SafeFontConfig)(ref val)).PixelSnapH = true;
					((SafeFontConfig)(ref val)).MergeFont = mergeFont2;
					tk.AddFontAwesomeIconFont(ref val);
				});
			});
		}
		return FontAtlas.NewDelegateFontHandle((FontAtlasBuildStepDelegate)delegate(IFontAtlasBuildToolkit e)
		{
			FontAtlasBuildToolkitUtilities.OnPreBuild(e, (Action<IFontAtlasBuildToolkitPreBuild>)delegate(IFontAtlasBuildToolkitPreBuild tk)
			{
				//IL_0039: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_005e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0093: Unknown result type (might be due to invalid IL or missing references)
				string text = fontPath;
				SafeFontConfig val = default(SafeFontConfig);
				((SafeFontConfig)(ref val))._002Ector();
				((SafeFontConfig)(ref val)).SizePx = size;
				((SafeFontConfig)(ref val)).PixelSnapH = true;
				((SafeFontConfig)(ref val)).GlyphRanges = DefaultFontRange;
				((SafeFontConfig)(ref val)).FontNo = 0;
				ImFontPtr mergeFont = tk.AddFontFromFile(text, ref val);
				((SafeFontConfig)(ref val))._002Ector();
				((SafeFontConfig)(ref val)).SizePx = size;
				((SafeFontConfig)(ref val)).PixelSnapH = true;
				((SafeFontConfig)(ref val)).MergeFont = mergeFont;
				ImFontPtr mergeFont2 = tk.AddGameSymbol(ref val);
				((SafeFontConfig)(ref val))._002Ector();
				((SafeFontConfig)(ref val)).SizePx = size;
				((SafeFontConfig)(ref val)).PixelSnapH = true;
				((SafeFontConfig)(ref val)).MergeFont = mergeFont2;
				tk.AddFontAwesomeIconFont(ref val);
			});
		});
	}

	private async Task<IFontHandle> CreateFontHandleAsync(float size)
	{
		await semaphore.WaitAsync();
		Interlocked.Increment(ref activeFontBuilds);
		try
		{
			IFontHandle handle = CreateFontHandleDefinition(size);
			await FontAtlas.BuildFontsAsync();
			return handle;
		}
		catch (Exception ex)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError($"字体 (大小: {size}) 构建失败");
			DLog.Error($"构建字体 (大小: {size}) 失败", ex);
			throw;
		}
		finally
		{
			Interlocked.Decrement(ref activeFontBuilds);
			semaphore.Release();
		}
	}

	private void RegenerateInstalledFonts()
	{
		string[] source = new string[2]
		{
			"C:\\Windows\\Fonts",
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft\\Windows\\Fonts")
		};
		FrozenSet<string> fontExtensions = FontExtensions.ToFrozenSet<string>(StringComparer.OrdinalIgnoreCase);
		ConcurrentDictionary<string, string> localInstalledFonts = new ConcurrentDictionary<string, string>();
		ConcurrentBag<Exception> errors = new ConcurrentBag<Exception>();
		int errorCounter = 0;
		try
		{
			Parallel.ForEach(from f in source.Where(Directory.Exists).SelectMany((string dir) => Directory.EnumerateFiles(dir, "*.*", new EnumerationOptions
				{
					RecurseSubdirectories = true,
					IgnoreInaccessible = true
				}))
				where fontExtensions.Contains(Path.GetExtension(f))
				select f, new ParallelOptions
			{
				MaxDegreeOfParallelism = Environment.ProcessorCount,
				CancellationToken = cancelSource.Token
			}, () => new PrivateFontCollection(), delegate(string file, ParallelLoopState loopState, long _, PrivateFontCollection threadLocalPfc)
			{
				if (Volatile.Read(in errorCounter) >= 100)
				{
					loopState.Stop();
					return threadLocalPfc;
				}
				try
				{
					threadLocalPfc.AddFontFile(file);
					if (threadLocalPfc.Families.Length != 0)
					{
						localInstalledFonts.TryAdd(file, threadLocalPfc.Families[0].Name);
					}
					threadLocalPfc.Dispose();
					return new PrivateFontCollection();
				}
				catch (Exception ex3) when (((ex3 is IOException || ex3 is SecurityException || ex3 is UnauthorizedAccessException || ex3 is ExternalException) ? 1 : 0) != 0)
				{
					if (Interlocked.Increment(ref errorCounter) <= 100)
					{
						errors.Add(ex3);
					}
					threadLocalPfc.Dispose();
					return new PrivateFontCollection();
				}
			}, delegate(PrivateFontCollection threadLocalPfc)
			{
				threadLocalPfc.Dispose();
			});
		}
		catch (OperationCanceledException)
		{
			DLog.Warning("已取消获取本机安装字体");
		}
		catch (Exception ex2)
		{
			DLog.Error("尝试获取本机安装字体时发生错误", ex2);
		}
		finally
		{
			InstalledFonts = localInstalledFonts;
			foreach (Exception item in errors)
			{
				DLog.Error("尝试获取本机安装字体时出错", item);
			}
		}
	}

	private void ClearFontHandles()
	{
		fontTasks.Clear();
		((IDisposable)FontAtlas)?.Dispose();
		FontAtlas = DalamudLeftServiceExtension.Instance().CreateFontAtlas((FontAtlasAutoRebuildMode)0, true, (string)null);
	}

	unsafe static FontManager()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ushort*[] array = new ushort*[3];
		ImGuiIOPtr iO = ImGui.GetIO();
		array[0] = ((ImFontAtlasPtr)(ref ((ImGuiIOPtr)(ref iO)).Fonts)).GetGlyphRangesDefault();
		iO = ImGui.GetIO();
		array[1] = ((ImFontAtlasPtr)(ref ((ImGuiIOPtr)(ref iO)).Fonts)).GetGlyphRangesChineseFull();
		iO = ImGui.GetIO();
		array[2] = ((ImFontAtlasPtr)(ref ((ImGuiIOPtr)(ref iO)).Fonts)).GetGlyphRangesKorean();
		DefaultFontRange = BuildRange(null, array);
		PreBuildFontSizes = new float[7] { 0.6f, 0.8f, 0.9f, 1f, 1.2f, 1.4f, 1.6f };
		FontExtensions = new string[3] { ".ttf", ".otf", ".ttc" };
	}
}
