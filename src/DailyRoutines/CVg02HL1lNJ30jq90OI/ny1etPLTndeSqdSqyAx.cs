using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.RemoteInteraction.Universalis.Models.Requests;
using OmenTools.Dalamud;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace CVg02HL1lNJ30jq90OI;

internal static class ny1etPLTndeSqdSqyAx
{
	private readonly struct sWYaEwCkeUvpEkGPnY6 : IEquatable<sWYaEwCkeUvpEkGPnY6>
	{
		public bool UseOriginEndpoint { get; init; }

		public bool Succeeded { get; init; }

		public DateTimeOffset DetectedAtUTC { get; init; }

		public sWYaEwCkeUvpEkGPnY6(bool UseOriginEndpoint, bool Succeeded, DateTimeOffset DetectedAtUTC)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.UseOriginEndpoint = UseOriginEndpoint;
			this.Succeeded = Succeeded;
			this.DetectedAtUTC = DetectedAtUTC;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379937B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF1F322 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d));
			if (kBmC2V2sB9(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool kBmC2V2sB9(StringBuilder builder)
		{
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A215FA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
			builder.Append(UseOriginEndpoint.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA79497 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
			builder.Append(Succeeded.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A2158E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
			builder.Append(DetectedAtUTC.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(sWYaEwCkeUvpEkGPnY6 left, sWYaEwCkeUvpEkGPnY6 right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(sWYaEwCkeUvpEkGPnY6 left, sWYaEwCkeUvpEkGPnY6 right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (EqualityComparer<bool>.Default.GetHashCode(UseOriginEndpoint) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(Succeeded)) * -1521134295 + EqualityComparer<DateTimeOffset>.Default.GetHashCode(DetectedAtUTC);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is sWYaEwCkeUvpEkGPnY6)
			{
				return Equals((sWYaEwCkeUvpEkGPnY6)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(sWYaEwCkeUvpEkGPnY6 other)
		{
			if (EqualityComparer<bool>.Default.Equals(UseOriginEndpoint, other.UseOriginEndpoint) && EqualityComparer<bool>.Default.Equals(Succeeded, other.Succeeded))
			{
				return EqualityComparer<DateTimeOffset>.Default.Equals(DetectedAtUTC, other.DetectedAtUTC);
			}
			return false;
		}

		[CompilerGenerated]
		public void UqMCZ7qLT7(out bool UseOriginEndpoint, out bool Succeeded, out DateTimeOffset DetectedAtUTC)
		{
			UseOriginEndpoint = this.UseOriginEndpoint;
			Succeeded = this.Succeeded;
			DetectedAtUTC = this.DetectedAtUTC;
		}
	}

	private static Task<sWYaEwCkeUvpEkGPnY6>? regionDetectionTask;

	private static bool useOriginEndpoint;

	public const string BASE_URL = "hxxps://universalis.atmoomen.top/api/v2";

	public const string BASE_URL_ORIGIN = "hxxps://universalis.app/api/v2";

	private const string REGION_DETECTION_URL = "hxxps://www.cloudflare.com/cdn-cgi/trace";

	private static readonly TimeSpan RegionDetectionCacheTTL;

	private static readonly TimeSpan RegionDetectionFailureTTL;

	public static readonly TimeSpan MarketCacheTTL;

	public static readonly TimeSpan AggregatedCacheTTL;

	public static readonly TimeSpan HistoryCacheTTL;

	public static readonly TimeSpan AuxiliaryCacheTTL;

	public static readonly TimeSpan StaticDataCacheTTL;

	public static readonly TimeSpan FailureTTL;

	public static readonly TimeSpan BatchWindow;

	private const string PROXY_REASON_HEADER = "X-Universalis-Proxy-Reason";

	private const int MAX_ATTEMPT_COUNT = 5;

	private const int PROXY_LIMIT_ATTEMPT_COUNT = 2;

	public const int MAX_BATCH_ITEM_COUNT = 100;

	private static HttpClient RegionDetectionHTTPClient => OmenServiceBase<HTTPClientHelper>.Instance().Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x227FD71 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44), delegate(HttpClient client)
	{
		client.Timeout = TimeSpan.FromSeconds(5L);
	});

	private static HttpClient HTTPClient => OmenServiceBase<HTTPClientHelper>.Instance().Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702ABCAB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));

	private static string EndpointBaseURL => "https://universalis.app/api/v2";

	public static string Pd0LM0mExY(IEnumerable<uint> itemIDs)
	{
		ArgumentNullException.ThrowIfNull(itemIDs, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D3ACEE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
		uint[] array = (from id in itemIDs.Where((uint id) => id != 0).Distinct()
			orderby id
			select id).ToArray();
		if (array.Length == 0)
		{
			throw new ArgumentException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702ABBD7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4118BB93 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
		}
		return string.Join(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C32399 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0), array);
	}

	public static List<uint> oC4L4jmGZJ(string itemIDsCSV)
	{
		return (from value in itemIDsCSV.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			select uint.Parse(value)).ToList();
	}

	public static string D5GLs4B7j1(string endpointPath, UniversalisRequestParams? parameters = null)
	{
		jP4LxIOFy4();
		return zHLLvxUJgy(EndpointBaseURL + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B51614 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3) + endpointPath, parameters);
	}

	public static string KLTL8U9imE(string endpointPath, string worldOrDC, IReadOnlyCollection<uint> itemIDs, UniversalisRequestParams? parameters)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(worldOrDC, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x371F260 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e));
		jP4LxIOFy4();
		string value = string.Join(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5B9260 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a), itemIDs);
		string value2 = Uri.EscapeDataString(worldOrDC);
		string path;
		if (!string.IsNullOrEmpty(endpointPath))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
			defaultInterpolatedStringHandler.AppendFormatted(EndpointBaseURL);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E959CC3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75));
			defaultInterpolatedStringHandler.AppendFormatted(endpointPath);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5B9266 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
			defaultInterpolatedStringHandler.AppendFormatted(value2);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DD183E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
			defaultInterpolatedStringHandler.AppendFormatted(value);
			path = defaultInterpolatedStringHandler.ToStringAndClear();
		}
		else
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(2, 3);
			defaultInterpolatedStringHandler2.AppendFormatted(EndpointBaseURL);
			defaultInterpolatedStringHandler2.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200E0F73 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));
			defaultInterpolatedStringHandler2.AppendFormatted(value2);
			defaultInterpolatedStringHandler2.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768E7B37 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
			defaultInterpolatedStringHandler2.AppendFormatted(value);
			path = defaultInterpolatedStringHandler2.ToStringAndClear();
		}
		return zHLLvxUJgy(path, parameters);
	}

	private static string zHLLvxUJgy(string path, UniversalisRequestParams? parameters)
	{
		if ((object)parameters == null)
		{
			return path;
		}
		List<string> list = (from parameter in parameters.GetQueryParameters()
			select Uri.EscapeDataString(parameter.Key) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5583BDFA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d) + Uri.EscapeDataString(parameter.Value)).ToList();
		if (list.Count != 0)
		{
			return path + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2E959CE7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_6103a9bab63e42479916a134b07f1c75) + string.Join(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6117C018 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402), list);
		}
		return path;
	}

	public static async Task<string> EU4Ll0t6Wd(string url, CancellationToken cancellationToken)
	{
		HttpStatusCode? statusCode = null;
		Exception exception = null;
		for (int i = 0; i < 5; i++)
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			try
			{
				using HttpResponseMessage response = await HTTPClient.GetAsync(url, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				statusCode = response.StatusCode;
				IEnumerable<string> values;
				string text = (response.Headers.TryGetValues(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4AF76 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3), out values) ? values.FirstOrDefault() : null);
				bool flag = ((text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C59E8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32) || text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1470179789 ^ 0x39E72258 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e) || text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x22640CB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44)) ? true : false);
				int num = (flag ? 2 : 5);
				flag = i >= num - 1;
				if (!flag)
				{
					HttpStatusCode statusCode2 = response.StatusCode;
					bool flag2 = ((statusCode2 == HttpStatusCode.TooManyRequests || statusCode2 == HttpStatusCode.InternalServerError || (uint)(statusCode2 - 502) <= 2u) ? true : false);
					flag = !flag2;
				}
				if (flag)
				{
					break;
				}
				RetryConditionHeaderValue retryAfter = response.Headers.RetryAfter;
				TimeSpan? timeSpan2 = retryAfter?.Delta ?? (retryAfter?.Date - OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset);
				if (timeSpan2.HasValue)
				{
					TimeSpan timeSpan3 = ((timeSpan2.Value > TimeSpan.Zero) ? timeSpan2.Value : TimeSpan.Zero);
					int num2 = Math.Clamp((int)(timeSpan3.TotalMilliseconds / 4.0), 250, 5000);
					timeSpan = timeSpan3 + TimeSpan.FromMilliseconds(Random.Shared.Next(num2 + 1));
				}
				else
				{
					int num3 = 250 * (1 << i);
					timeSpan = TimeSpan.FromMilliseconds(Random.Shared.Next(num3 + 1));
				}
				goto IL_047d;
			}
			catch (HttpRequestException ex)
			{
				exception = ex;
				if (i >= 4)
				{
					break;
				}
				int num4 = 250 * (1 << i);
				timeSpan = TimeSpan.FromMilliseconds(Random.Shared.Next(num4 + 1));
				goto IL_047d;
			}
			IL_047d:
			if (timeSpan > TimeSpan.Zero)
			{
				await Task.Delay(timeSpan, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
		if (exception != null && !statusCode.HasValue)
		{
			throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1FF80 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83) + url, exception);
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1FF80 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
		defaultInterpolatedStringHandler.AppendFormatted(url);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B213730 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a));
		defaultInterpolatedStringHandler.AppendFormatted(statusCode);
		throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
	}

	private static void jP4LxIOFy4()
	{
	}

	private static bool ExELp849UR(Task<sWYaEwCkeUvpEkGPnY6> task)
	{
		if (!task.IsCompleted)
		{
			return true;
		}
		if (!task.IsCompletedSuccessfully)
		{
			return false;
		}
		sWYaEwCkeUvpEkGPnY6 result = task.Result;
		TimeSpan timeSpan = (result.Succeeded ? RegionDetectionCacheTTL : RegionDetectionFailureTTL);
		return OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset - result.DetectedAtUTC < timeSpan;
	}

	private static async Task bnjL3Vd4V8(TaskCompletionSource<sWYaEwCkeUvpEkGPnY6> completion)
	{
		sWYaEwCkeUvpEkGPnY6 result = await Yi3LBc0p8M().ConfigureAwait(continueOnCapturedContext: false);
		Volatile.Write(ref useOriginEndpoint, result.UseOriginEndpoint);
		completion.TrySetResult(result);
	}

	private static async Task<sWYaEwCkeUvpEkGPnY6> Yi3LBc0p8M()
	{
		DateTimeOffset detectedAtUTC = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset;
		try
		{
			_ = RegionDetectionHTTPClient;
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768FC2FF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46);
			_ = 0;
			throw null;
		}
		catch (Exception ex)
		{
			DLog.Warning(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B30518E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117), ex);
			return new sWYaEwCkeUvpEkGPnY6(Volatile.Read(in useOriginEndpoint), Succeeded: false, detectedAtUTC);
		}
	}

	private static string? KOwLm2MWSj(string trace)
	{
		SpanLineEnumerator enumerator = trace.AsSpan().EnumerateLines().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ReadOnlySpan<char> current = enumerator.Current;
				if (current.StartsWith(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719CA435 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc).AsSpan(), StringComparison.OrdinalIgnoreCase))
				{
					ReadOnlySpan<char> readOnlySpan = current.Slice(4).Trim();
					if (readOnlySpan.Length != 2 || !char.IsAsciiLetter(readOnlySpan[0]) || !char.IsAsciiLetter(readOnlySpan[1]))
					{
						return null;
					}
					string text = readOnlySpan.ToString().ToUpperInvariant();
					return (text == GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829EC77 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184)) ? null : text;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return null;
	}

	static ny1etPLTndeSqdSqyAx()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		RegionDetectionCacheTTL = TimeSpan.FromMinutes(5L);
		RegionDetectionFailureTTL = TimeSpan.FromSeconds(30L);
		MarketCacheTTL = TimeSpan.FromHours(1);
		AggregatedCacheTTL = TimeSpan.FromHours(4);
		HistoryCacheTTL = TimeSpan.FromHours(4);
		AuxiliaryCacheTTL = TimeSpan.FromHours(1);
		StaticDataCacheTTL = TimeSpan.FromHours(12);
		FailureTTL = TimeSpan.FromSeconds(30L);
		BatchWindow = TimeSpan.FromMilliseconds(30L);
	}
}
