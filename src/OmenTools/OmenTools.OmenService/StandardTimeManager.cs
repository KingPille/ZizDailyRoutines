using System;
using System.CodeDom.Compiler;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using System.Threading;
using System.Threading.Tasks;
using GuerrillaNtp;
using OmenTools.Dalamud;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class StandardTimeManager : OmenServiceBase<StandardTimeManager>
{
	private const string TIME_API_URL = "hxxp://vv.video.qq.com/checktime?otype=json";

	private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();

	public DateTime Today => TodayOffset.DateTime;

	public DateTimeOffset TodayOffset => new DateTimeOffset(NowOffset.Date, NowOffset.Offset);

	public DateTime Now => NowOffset.DateTime;

	public DateTimeOffset NowOffset => UTCNowOffset.ToLocalTime();

	public DateTime UTCNow => UTCNowOffset.UtcDateTime;

	public DateTimeOffset UTCNowOffset
	{
		get
		{
			NtpClock? clock = Clock;
			if ((object)clock == null)
			{
				if (!WebAPIOffset.HasValue)
				{
					return DateTimeOffset.UtcNow;
				}
				return DateTimeOffset.UtcNow + WebAPIOffset.Value;
			}
			return clock.UtcNow;
		}
	}

	public StandardTimeSource Source
	{
		get
		{
			if (Clock != null)
			{
				return StandardTimeSource.NTP;
			}
			if (WebAPIOffset.HasValue)
			{
				return StandardTimeSource.Web;
			}
			return StandardTimeSource.Local;
		}
	}

	private NtpClient NTPClient { get; } = new NtpClient("localhost\0\0\0\0\0", TimeSpan.FromSeconds(5L));

	private HttpClient HTTPClient { get; } = new HttpClient
	{
		Timeout = TimeSpan.FromSeconds(5L)
	};

	private NtpClock? Clock { get; set; }

	private TimeSpan? WebAPIOffset { get; set; }

	protected override void Init()
	{
	}

	private async Task QueryAll(CancellationToken token)
	{
		if (!WebAPIOffset.HasValue)
		{
			try
			{
				await QueryWebTimeAsync(token).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex2)
			{
				DLog.Error("尝试从 WebAPI 获取标准时间时发生错误", ex2);
			}
		}
		if (Clock == null)
		{
			try
			{
				Clock = await NTPClient.QueryAsync(token).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception ex4)
			{
				DLog.Error("尝试从 localhost\0\0\0\0\0 获取标准时间时发生错误", ex4);
				try
				{
					Clock = await NtpClient.Default.QueryAsync(token).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (OperationCanceledException)
				{
				}
				catch
				{
					DLog.Error("尝试从 pool.ntp.org 获取标准时间时发生错误", ex4);
				}
			}
		}
		DLog.Debug($"[StandardTimeManager] 请求标准时间完成, 当前 UTC 时间: {UTCNow}, 来源类型: {Source}");
	}

	private async Task QueryWebTimeAsync(CancellationToken token)
	{
		try
		{
			_ = HTTPClient;
			_ = token;
			throw null;
		}
		catch
		{
		}
	}

	protected override void Uninit()
	{
		if (!cancelSource.IsCancellationRequested)
		{
			cancelSource.Cancel();
		}
		cancelSource.Dispose();
		HTTPClient.Dispose();
	}

	[GeneratedRegex("\"t\":(\\d+)")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex QQVideoResponseRegex()
	{
		return _003CRegexGenerator_g_003EF5A919AF36546C3D797A1EDA78623F5FD69C6B1313DB9045DB2CC6C90915164F1__QQVideoResponseRegex_17.Instance;
	}
}
