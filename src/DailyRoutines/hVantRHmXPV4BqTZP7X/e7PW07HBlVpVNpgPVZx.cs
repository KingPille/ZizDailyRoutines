using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using DailyRoutines.Manager;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace hVantRHmXPV4BqTZP7X;

[ManagerInitOrder(16)]
internal sealed class e7PW07HBlVpVNpgPVZx : ManagerBase<e7PW07HBlVpVNpgPVZx>
{
	private class NbKMswjAp40bFfggAy9
	{
		[JsonProperty("count")]
		public uint Count { get; set; }

		[JsonProperty("date")]
		public DateTime Date { get; set; }

		public NbKMswjAp40bFfggAy9()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private static class _003C_003Eo__15
	{
		public static CallSite<Func<CallSite, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, JObject>> _003C_003Ep__1;
	}

	private Timer? dailyCheckTimer;

	private int isChecking;

	public uint Count { get; private set; }

	public DateTime Date { get; private set; }

	protected override Task Init()
	{
		KfNHi9T6uP();
		return Task.CompletedTask;
	}

	protected override async Task Stop()
	{
		if (dailyCheckTimer != null)
		{
			await dailyCheckTimer.DisposeAsync();
		}
	}

	protected override Task Uninit()
	{
		dailyCheckTimer = null;
		return Task.CompletedTask;
	}

	private void KfNHi9T6uP()
	{
	}

	private void bnUHPvHrZg()
	{
		if (!(OmenServiceBase<StandardTimeManager>.Instance().Now.Date == Date.Date) && Interlocked.CompareExchange(ref isChecking, 1, 0) == 0 && !RunBackground(async delegate(CancellationToken cancellationToken)
		{
			try
			{
				await QcqHhPfUUF(cancellationToken);
			}
			finally
			{
				Volatile.Write(ref isChecking, 0);
			}
		}))
		{
			Volatile.Write(ref isChecking, 0);
		}
	}

	private async Task QcqHhPfUUF(CancellationToken cancellationToken)
	{
		try
		{
			object arg = JsonConvert.DeserializeObject<object>(await PqtKlnSSKxdRRHSb7mo.ktqSjVMsCG(InternalAPIRequestType.GetDailyActiveUserCount, null, cancellationToken));
			if (_003C_003Eo__15._003C_003Ep__0 == null)
			{
				_003C_003Eo__15._003C_003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4A1DC0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32), typeof(e7PW07HBlVpVNpgPVZx), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
			}
			NbKMswjAp40bFfggAy9 nbKMswjAp40bFfggAy = ((JToken)(JObject)(dynamic)_003C_003Eo__15._003C_003Ep__0.Target(_003C_003Eo__15._003C_003Ep__0, arg)).ToObject<NbKMswjAp40bFfggAy9>();
			Count = nbKMswjAp40bFfggAy.Count;
			Date = nbKMswjAp40bFfggAy.Date;
		}
		catch
		{
		}
	}

	public e7PW07HBlVpVNpgPVZx()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Date = DateTime.MinValue;
		base._002Ector();
	}

	[CompilerGenerated]
	private void rOZHo8TkSS(object? _)
	{
		bnUHPvHrZg();
	}

	[CompilerGenerated]
	private async Task uXgHgAnjoR(CancellationToken cancellationToken)
	{
		try
		{
			await QcqHhPfUUF(cancellationToken);
		}
		finally
		{
			Volatile.Write(ref isChecking, 0);
		}
	}
}
