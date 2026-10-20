using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Enums;
using DailyRoutines.Common.RemoteInteraction.Models;
using Dalamud.Plugin.Services;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.Manager;

[ManagerInitOrder(2)]
public sealed class RemoteInteractionManager : ManagerBase<RemoteInteractionManager>
{
	private interface d9wlvuOvU7P3C3FbfQH
	{
		void Reset();
	}

	private sealed class m6s5IHOlbZ48jOZrq8e<k6yxyYOx4KAI0TbEPAU, HX57vXOpyJdYq7cpqdd, QS48VeO3p4lrhkVyaP3> : d9wlvuOvU7P3C3FbfQH where k6yxyYOx4KAI0TbEPAU : IRemoteQueryHandler<HX57vXOpyJdYq7cpqdd, QS48VeO3p4lrhkVyaP3> where HX57vXOpyJdYq7cpqdd : notnull
	{
		private sealed class tRqNyjtwmUKSPpndQ7b
		{
			[CompilerGenerated]
			private sealed class _003C_003Ec__DisplayClass24_0
			{
				public Action<RemoteSnapshot<QS48VeO3p4lrhkVyaP3>> callback;

				public RemoteSnapshot<QS48VeO3p4lrhkVyaP3> snapshot;

				public _003C_003Ec__DisplayClass24_0()
				{
					n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
					base._002Ector();
				}

				internal void fm9W1bfRjc()
				{
					callback(snapshot);
				}
			}

			private jtjpZuteT1EPJgVuadu snapshotBox;

			private Qaharnt2DjG3WMNAL5K? inflight;

			private int version;

			private long lastFailureAtUTCTicks;

			private long observerIdentity;

			public ConcurrentDictionary<long, K0twbAtIqFP0XqSmJEm> Observers { get; }

			public int Version => Volatile.Read(in version);

			public Qaharnt2DjG3WMNAL5K? CurrentInflight => Volatile.Read(in inflight);

			public RemoteSnapshot<QS48VeO3p4lrhkVyaP3> CurrentSnapshot => Volatile.Read(in snapshotBox).Snapshot;

			public DateTime LastFailureAtUTC
			{
				get
				{
					long num = Interlocked.Read(in lastFailureAtUTCTicks);
					if (num != 0L)
					{
						return new DateTime(num, DateTimeKind.Utc);
					}
					return DateTime.MinValue;
				}
			}

			public long EjItqCnZaB(K0twbAtIqFP0XqSmJEm observer)
			{
				long num = Interlocked.Increment(ref observerIdentity);
				Observers[num] = observer;
				return num;
			}

			public void nWntc2lSLT(long observerID)
			{
				Observers.TryRemove(observerID, out var _);
			}

			public void WDPtAnqotD()
			{
				Interlocked.Increment(ref version);
				Interlocked.Exchange(ref lastFailureAtUTCTicks, 0L);
				f9Lt6RqJ5l(new RemoteSnapshot<QS48VeO3p4lrhkVyaP3>(RemoteSnapshotStatus.Empty, default(QS48VeO3p4lrhkVyaP3), DateTime.MinValue, null));
			}

			public bool nZbtQUZaVp(Qaharnt2DjG3WMNAL5K next, Qaharnt2DjG3WMNAL5K? expected)
			{
				return Interlocked.CompareExchange(ref inflight, next, expected) == expected;
			}

			public void ARktJhl0Vh(Qaharnt2DjG3WMNAL5K current)
			{
				Interlocked.CompareExchange(ref inflight, null, current);
			}

			public void fPNtrpcACQ(int expectedVersion, RemoteSnapshot<QS48VeO3p4lrhkVyaP3> snapshot)
			{
				if (Version == expectedVersion)
				{
					Interlocked.Exchange(ref lastFailureAtUTCTicks, 0L);
					f9Lt6RqJ5l(snapshot);
				}
			}

			public void X70tGtHjyN(int expectedVersion, RemoteSnapshot<QS48VeO3p4lrhkVyaP3> snapshot, DateTime failedAtUTC)
			{
				if (Version == expectedVersion)
				{
					Interlocked.Exchange(ref lastFailureAtUTCTicks, failedAtUTC.Ticks);
					f9Lt6RqJ5l(snapshot);
				}
			}

			public void f9Lt6RqJ5l(RemoteSnapshot<QS48VeO3p4lrhkVyaP3> snapshot)
			{
				jtjpZuteT1EPJgVuadu value = new jtjpZuteT1EPJgVuadu(snapshot);
				jtjpZuteT1EPJgVuadu jtjpZuteT1EPJgVuadu2 = Interlocked.Exchange(ref snapshotBox, value);
				if (EqualityComparer<RemoteSnapshot<QS48VeO3p4lrhkVyaP3>>.Default.Equals(jtjpZuteT1EPJgVuadu2.Snapshot, snapshot))
				{
					return;
				}
				foreach (K0twbAtIqFP0XqSmJEm value2 in Observers.Values)
				{
					sSFtkHLaxA(snapshot, value2.Callback, value2.MarshalToFrameworkThread, value2.CancellationToken);
				}
			}

			public static void sSFtkHLaxA(RemoteSnapshot<QS48VeO3p4lrhkVyaP3> snapshot, Action<RemoteSnapshot<QS48VeO3p4lrhkVyaP3>> callback, bool marshalToFrameworkThread, CancellationToken cancellationToken)
			{
				_003C_003Ec__DisplayClass24_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass24_0();
				CS_0024_003C_003E8__locals6.callback = callback;
				CS_0024_003C_003E8__locals6.snapshot = snapshot;
				try
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
					if (!marshalToFrameworkThread)
					{
						CS_0024_003C_003E8__locals6.callback(CS_0024_003C_003E8__locals6.snapshot);
						return;
					}
					DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
					{
						CS_0024_003C_003E8__locals6.callback(CS_0024_003C_003E8__locals6.snapshot);
					}, default(TimeSpan), 0, default(CancellationToken));
				}
				catch (Exception ex)
				{
					DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x39225D32 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2), ex);
				}
			}

			public tRqNyjtwmUKSPpndQ7b()
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				snapshotBox = new jtjpZuteT1EPJgVuadu(new RemoteSnapshot<QS48VeO3p4lrhkVyaP3>(RemoteSnapshotStatus.Empty, default(QS48VeO3p4lrhkVyaP3), DateTime.MinValue, null));
				Observers = new ConcurrentDictionary<long, K0twbAtIqFP0XqSmJEm>();
				base._002Ector();
			}
		}

		private sealed class Qaharnt2DjG3WMNAL5K
		{
			private readonly TaskCompletionSource<QS48VeO3p4lrhkVyaP3> completionSource;

			public int Version { get; }

			public Task<QS48VeO3p4lrhkVyaP3> Task => completionSource.Task;

			public bool IsCompleted => completionSource.Task.IsCompleted;

			public Qaharnt2DjG3WMNAL5K(int version)
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				completionSource = new TaskCompletionSource<QS48VeO3p4lrhkVyaP3>(TaskCreationOptions.RunContinuationsAsynchronously);
				Version = version;
				base._002Ector();
			}

			public void XCFt5ght8R(QS48VeO3p4lrhkVyaP3 value)
			{
				completionSource.TrySetResult(value);
			}

			public void cETtR15ATf(CancellationToken cancellationToken)
			{
				completionSource.TrySetCanceled(cancellationToken);
			}

			public void AhQtD1aSed(Exception exception)
			{
				completionSource.TrySetException(exception);
			}
		}

		private sealed class jtjpZuteT1EPJgVuadu
		{
			public RemoteSnapshot<QS48VeO3p4lrhkVyaP3> Snapshot { get; }

			public jtjpZuteT1EPJgVuadu(RemoteSnapshot<QS48VeO3p4lrhkVyaP3> snapshot)
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				Snapshot = snapshot;
				base._002Ector();
			}
		}

		private readonly struct K0twbAtIqFP0XqSmJEm : IEquatable<K0twbAtIqFP0XqSmJEm>
		{
			public Action<RemoteSnapshot<QS48VeO3p4lrhkVyaP3>> Callback { get; init; }

			public bool MarshalToFrameworkThread { get; init; }

			public CancellationToken CancellationToken { get; init; }

			public K0twbAtIqFP0XqSmJEm(Action<RemoteSnapshot<QS48VeO3p4lrhkVyaP3>> Callback, bool MarshalToFrameworkThread, CancellationToken CancellationToken)
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				this.Callback = Callback;
				this.MarshalToFrameworkThread = MarshalToFrameworkThread;
				this.CancellationToken = CancellationToken;
			}

			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE39E9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
				stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4B9914 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32));
				if (VwmtY1jmms(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			private bool VwmtY1jmms(StringBuilder builder)
			{
				builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5583E4EC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d));
				builder.Append(Callback);
				builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-224923271 ^ -983893628 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
				builder.Append(MarshalToFrameworkThread.ToString());
				builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA45CB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1));
				builder.Append(CancellationToken.ToString());
				return true;
			}

			[CompilerGenerated]
			public static bool operator !=(K0twbAtIqFP0XqSmJEm left, K0twbAtIqFP0XqSmJEm right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(K0twbAtIqFP0XqSmJEm left, K0twbAtIqFP0XqSmJEm right)
			{
				return left.Equals(right);
			}

			[CompilerGenerated]
			public override int GetHashCode()
			{
				return (EqualityComparer<Action<RemoteSnapshot<QS48VeO3p4lrhkVyaP3>>>.Default.GetHashCode(Callback) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(MarshalToFrameworkThread)) * -1521134295 + EqualityComparer<CancellationToken>.Default.GetHashCode(CancellationToken);
			}

			[CompilerGenerated]
			public override bool Equals(object obj)
			{
				if (obj is K0twbAtIqFP0XqSmJEm)
				{
					return Equals((K0twbAtIqFP0XqSmJEm)obj);
				}
				return false;
			}

			[CompilerGenerated]
			public bool Equals(K0twbAtIqFP0XqSmJEm other)
			{
				if (EqualityComparer<Action<RemoteSnapshot<QS48VeO3p4lrhkVyaP3>>>.Default.Equals(Callback, other.Callback) && EqualityComparer<bool>.Default.Equals(MarshalToFrameworkThread, other.MarshalToFrameworkThread))
				{
					return EqualityComparer<CancellationToken>.Default.Equals(CancellationToken, other.CancellationToken);
				}
				return false;
			}

			[CompilerGenerated]
			public void RAYtzwEyou(out Action<RemoteSnapshot<QS48VeO3p4lrhkVyaP3>> Callback, out bool MarshalToFrameworkThread, out CancellationToken CancellationToken)
			{
				Callback = this.Callback;
				MarshalToFrameworkThread = this.MarshalToFrameworkThread;
				CancellationToken = this.CancellationToken;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass9_0
		{
			public tRqNyjtwmUKSPpndQ7b entry;

			public long observerID;

			public _003C_003Ec__DisplayClass9_0()
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				base._002Ector();
			}

			internal void U2KWnqZRWP()
			{
				entry.nWntc2lSLT(observerID);
			}
		}

		private readonly ConcurrentDictionary<HX57vXOpyJdYq7cpqdd, tRqNyjtwmUKSPpndQ7b> entries;

		public m6s5IHOlbZ48jOZrq8e(RemoteInteractionManager manager)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			_003Cmanager_003EP = manager;
			entries = new ConcurrentDictionary<HX57vXOpyJdYq7cpqdd, tRqNyjtwmUKSPpndQ7b>();
			base._002Ector();
		}

		public void Reset()
		{
			entries.Clear();
		}

		public RemoteSnapshot<QS48VeO3p4lrhkVyaP3> IdcOBHYZrq(HX57vXOpyJdYq7cpqdd key, CancellationToken cancellationToken)
		{
			tRqNyjtwmUKSPpndQ7b orAdd = entries.GetOrAdd(key, (HX57vXOpyJdYq7cpqdd _, RemoteInteractionManager currentOwner) => new tRqNyjtwmUKSPpndQ7b(), _003Cmanager_003EP);
			RemoteSnapshot<QS48VeO3p4lrhkVyaP3> currentSnapshot = orAdd.CurrentSnapshot;
			DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
			if (currentSnapshot.Status == RemoteSnapshotStatus.Ready && gfTOgigCVd(currentSnapshot, uTCNow))
			{
				return currentSnapshot;
			}
			if (currentSnapshot.Status == RemoteSnapshotStatus.Failed && !gs7OwFo834(orAdd, uTCNow))
			{
				return currentSnapshot;
			}
			if (currentSnapshot.HasValue)
			{
				RemoteSnapshot<QS48VeO3p4lrhkVyaP3> remoteSnapshot = ((currentSnapshot.Status == RemoteSnapshotStatus.Refreshing) ? currentSnapshot : currentSnapshot with
				{
					Status = RemoteSnapshotStatus.Refreshing
				});
				if (currentSnapshot.Status != RemoteSnapshotStatus.Refreshing)
				{
					orAdd.f9Lt6RqJ5l(remoteSnapshot);
				}
				OjCS0Ers48(tvnOE1yPkZ(orAdd, key, cancellationToken));
				return remoteSnapshot;
			}
			RemoteSnapshot<QS48VeO3p4lrhkVyaP3> remoteSnapshot2 = ((currentSnapshot.Status == RemoteSnapshotStatus.Loading) ? currentSnapshot : new RemoteSnapshot<QS48VeO3p4lrhkVyaP3>(RemoteSnapshotStatus.Loading, currentSnapshot.Value, currentSnapshot.UpdatedAtUTC, currentSnapshot.Error));
			if (currentSnapshot.Status != RemoteSnapshotStatus.Loading)
			{
				orAdd.f9Lt6RqJ5l(remoteSnapshot2);
			}
			OjCS0Ers48(tvnOE1yPkZ(orAdd, key, cancellationToken));
			return remoteSnapshot2;
		}

		public bool zArOm0D5kE(HX57vXOpyJdYq7cpqdd key, out QS48VeO3p4lrhkVyaP3 value)
		{
			if (entries.TryGetValue(key, out tRqNyjtwmUKSPpndQ7b value2))
			{
				RemoteSnapshot<QS48VeO3p4lrhkVyaP3> currentSnapshot = value2.CurrentSnapshot;
				if (currentSnapshot.HasValue)
				{
					value = currentSnapshot.Value;
					return true;
				}
			}
			value = default(QS48VeO3p4lrhkVyaP3);
			return false;
		}

		public async ValueTask<QS48VeO3p4lrhkVyaP3> qlVOi7DjXK(HX57vXOpyJdYq7cpqdd key, CancellationToken cancellationToken)
		{
			tRqNyjtwmUKSPpndQ7b orAdd = entries.GetOrAdd(key, (HX57vXOpyJdYq7cpqdd _, RemoteInteractionManager currentOwner) => new tRqNyjtwmUKSPpndQ7b(), _003Cmanager_003EP);
			RemoteSnapshot<QS48VeO3p4lrhkVyaP3> currentSnapshot = orAdd.CurrentSnapshot;
			DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
			if (currentSnapshot.Status == RemoteSnapshotStatus.Ready && gfTOgigCVd(currentSnapshot, uTCNow))
			{
				return currentSnapshot.Value;
			}
			if (currentSnapshot.Status == RemoteSnapshotStatus.Failed && !gs7OwFo834(orAdd, uTCNow))
			{
				if (currentSnapshot.HasValue)
				{
					return currentSnapshot.Value;
				}
				throw currentSnapshot.Error ?? new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F13FD5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
			}
			return await tvnOE1yPkZ(orAdd, key, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}

		public void rauOPyUGOL(HX57vXOpyJdYq7cpqdd key)
		{
			if (entries.TryGetValue(key, out tRqNyjtwmUKSPpndQ7b value))
			{
				value.WDPtAnqotD();
			}
		}

		public void N4POhESIy6(Func<HX57vXOpyJdYq7cpqdd, bool> predicate)
		{
			foreach (var (arg, tRqNyjtwmUKSPpndQ7b3) in entries)
			{
				if (predicate(arg))
				{
					tRqNyjtwmUKSPpndQ7b3.WDPtAnqotD();
				}
			}
		}

		public IDisposable hr2OoeUcUU(HX57vXOpyJdYq7cpqdd key, Action<RemoteSnapshot<QS48VeO3p4lrhkVyaP3>> callback, bool marshalToFrameworkThread, CancellationToken cancellationToken)
		{
			_003C_003Ec__DisplayClass9_0 obj = new _003C_003Ec__DisplayClass9_0();
			ArgumentNullException.ThrowIfNull(callback, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E463027 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e));
			obj.entry = entries.GetOrAdd(key, (HX57vXOpyJdYq7cpqdd _, RemoteInteractionManager currentOwner) => new tRqNyjtwmUKSPpndQ7b(), _003Cmanager_003EP);
			obj.observerID = obj.entry.EjItqCnZaB(new K0twbAtIqFP0XqSmJEm(callback, marshalToFrameworkThread, cancellationToken));
			tRqNyjtwmUKSPpndQ7b.sSFtkHLaxA(obj.entry.CurrentSnapshot, callback, marshalToFrameworkThread, cancellationToken);
			return new gOyYxkOJqGLVUqLxF1B(delegate
			{
				obj.entry.nWntc2lSLT(obj.observerID);
			});
		}

		private static bool gfTOgigCVd(RemoteSnapshot<QS48VeO3p4lrhkVyaP3> snapshot, DateTime nowUTC)
		{
			if (snapshot.UpdatedAtUTC > DateTime.MinValue)
			{
				return nowUTC - snapshot.UpdatedAtUTC <= k6yxyYOx4KAI0TbEPAU.TTL;
			}
			return false;
		}

		private static bool gs7OwFo834(tRqNyjtwmUKSPpndQ7b entry, DateTime nowUTC)
		{
			if (!(entry.LastFailureAtUTC == DateTime.MinValue))
			{
				return nowUTC - entry.LastFailureAtUTC > k6yxyYOx4KAI0TbEPAU.FailureTTL;
			}
			return true;
		}

		private Task<QS48VeO3p4lrhkVyaP3> tvnOE1yPkZ(tRqNyjtwmUKSPpndQ7b entry, HX57vXOpyJdYq7cpqdd key, CancellationToken cancellationToken)
		{
			Qaharnt2DjG3WMNAL5K currentInflight;
			Qaharnt2DjG3WMNAL5K qaharnt2DjG3WMNAL5K;
			do
			{
				int version = entry.Version;
				currentInflight = entry.CurrentInflight;
				if (currentInflight != null && !currentInflight.IsCompleted && currentInflight.Version == version)
				{
					return currentInflight.Task;
				}
				qaharnt2DjG3WMNAL5K = new Qaharnt2DjG3WMNAL5K(version);
			}
			while (!entry.nZbtQUZaVp(qaharnt2DjG3WMNAL5K, currentInflight));
			OjCS0Ers48(bIdObk6u3R(entry, qaharnt2DjG3WMNAL5K, key, cancellationToken));
			return qaharnt2DjG3WMNAL5K.Task;
		}

		private async Task bIdObk6u3R(tRqNyjtwmUKSPpndQ7b entry, Qaharnt2DjG3WMNAL5K inflight, HX57vXOpyJdYq7cpqdd key, CancellationToken cancellationToken)
		{
			CancellationTokenSource linkedSource = null;
			try
			{
				linkedSource = _003Cmanager_003EP.TJZSkRYSlV(cancellationToken);
				QS48VeO3p4lrhkVyaP3 value = await k6yxyYOx4KAI0TbEPAU.FetchAsync(key, linkedSource.Token).ConfigureAwait(continueOnCapturedContext: false);
				entry.fPNtrpcACQ(snapshot: new RemoteSnapshot<QS48VeO3p4lrhkVyaP3>(RemoteSnapshotStatus.Ready, value, OmenServiceBase<StandardTimeManager>.Instance().UTCNow, null), expectedVersion: inflight.Version);
				inflight.XCFt5ght8R(value);
			}
			catch (OperationCanceledException ex)
			{
				inflight.cETtR15ATf(ex.CancellationToken.CanBeCanceled ? ex.CancellationToken : cancellationToken);
			}
			catch (Exception ex2)
			{
				RemoteSnapshot<QS48VeO3p4lrhkVyaP3> currentSnapshot = entry.CurrentSnapshot;
				RemoteSnapshot<QS48VeO3p4lrhkVyaP3> snapshot = (currentSnapshot.HasValue ? currentSnapshot with
				{
					Status = RemoteSnapshotStatus.Failed,
					Error = ex2
				} : new RemoteSnapshot<QS48VeO3p4lrhkVyaP3>(RemoteSnapshotStatus.Failed, currentSnapshot.Value, OmenServiceBase<StandardTimeManager>.Instance().UTCNow, ex2));
				entry.X70tGtHjyN(inflight.Version, snapshot, OmenServiceBase<StandardTimeManager>.Instance().UTCNow);
				inflight.AhQtD1aSed(ex2);
			}
			finally
			{
				linkedSource?.Dispose();
				entry.ARktJhl0Vh(inflight);
			}
		}
	}

	private sealed class NoXwIZOFsdAD8r3XGLF<Iuw37LOqyoTp67BrCDo> : d9wlvuOvU7P3C3FbfQH
	{
		private sealed class Om77vSWyIygqrAUucID
		{
			private wj4xwSWS0RYawTw3Wxk snapshotBox;

			public RemoteCommandSnapshot<Iuw37LOqyoTp67BrCDo> CurrentSnapshot => Volatile.Read(in snapshotBox).Snapshot;

			public Om77vSWyIygqrAUucID(DateTime startedAtUTC)
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				snapshotBox = new wj4xwSWS0RYawTw3Wxk(new RemoteCommandSnapshot<Iuw37LOqyoTp67BrCDo>(IsCompleted: false, IsSucceeded: false, default(Iuw37LOqyoTp67BrCDo), null, startedAtUTC, null));
				base._002Ector();
			}

			public void gUAWHN1XlP(RemoteCommandSnapshot<Iuw37LOqyoTp67BrCDo> snapshot)
			{
				Interlocked.Exchange(ref snapshotBox, new wj4xwSWS0RYawTw3Wxk(snapshot));
			}
		}

		private sealed class wj4xwSWS0RYawTw3Wxk
		{
			public RemoteCommandSnapshot<Iuw37LOqyoTp67BrCDo> Snapshot { get; }

			public wj4xwSWS0RYawTw3Wxk(RemoteCommandSnapshot<Iuw37LOqyoTp67BrCDo> snapshot)
			{
				n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
				Snapshot = snapshot;
				base._002Ector();
			}
		}

		private readonly ConcurrentDictionary<Guid, Om77vSWyIygqrAUucID> entries;

		public NoXwIZOFsdAD8r3XGLF(RemoteInteractionManager manager)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			_003Cmanager_003EP = manager;
			entries = new ConcurrentDictionary<Guid, Om77vSWyIygqrAUucID>();
			base._002Ector();
		}

		public void Reset()
		{
			entries.Clear();
		}

		public RemoteCommandHandle<Iuw37LOqyoTp67BrCDo> ikPOcDDtVs<THandler, TArgs>(TArgs args) where THandler : IRemoteCommandHandler<TArgs, Iuw37LOqyoTp67BrCDo>
		{
			RemoteCommandHandle<Iuw37LOqyoTp67BrCDo> result = new RemoteCommandHandle<Iuw37LOqyoTp67BrCDo>(Guid.NewGuid());
			Om77vSWyIygqrAUucID om77vSWyIygqrAUucID = new Om77vSWyIygqrAUucID(OmenServiceBase<StandardTimeManager>.Instance().UTCNow);
			entries[result.ID] = om77vSWyIygqrAUucID;
			OjCS0Ers48(oD5OQPeUVg<THandler, TArgs>(om77vSWyIygqrAUucID, args));
			return result;
		}

		public bool OUPOAmruDC(RemoteCommandHandle<Iuw37LOqyoTp67BrCDo> handle, out RemoteCommandSnapshot<Iuw37LOqyoTp67BrCDo> snapshot)
		{
			if (entries.TryGetValue(handle.ID, out Om77vSWyIygqrAUucID value))
			{
				snapshot = value.CurrentSnapshot;
				return true;
			}
			snapshot = default(RemoteCommandSnapshot<Iuw37LOqyoTp67BrCDo>);
			return false;
		}

		private async Task oD5OQPeUVg<THandler, TArgs>(Om77vSWyIygqrAUucID entry, TArgs args) where THandler : IRemoteCommandHandler<TArgs, Iuw37LOqyoTp67BrCDo>
		{
			try
			{
				entry.gUAWHN1XlP(new RemoteCommandSnapshot<Iuw37LOqyoTp67BrCDo>(IsCompleted: true, IsSucceeded: true, await _003Cmanager_003EP.MiLS64ExLv<THandler, TArgs, Iuw37LOqyoTp67BrCDo>(args, CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false), null, entry.CurrentSnapshot.StartedAtUTC, OmenServiceBase<StandardTimeManager>.Instance().UTCNow));
			}
			catch (Exception ex) when (!(ex is OperationCanceledException))
			{
				entry.gUAWHN1XlP(new RemoteCommandSnapshot<Iuw37LOqyoTp67BrCDo>(IsCompleted: true, IsSucceeded: false, default(Iuw37LOqyoTp67BrCDo), ex, entry.CurrentSnapshot.StartedAtUTC, OmenServiceBase<StandardTimeManager>.Instance().UTCNow));
			}
		}
	}

	private sealed class gOyYxkOJqGLVUqLxF1B : IDisposable
	{
		private Action? dispose;

		public gOyYxkOJqGLVUqLxF1B(Action? dispose)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.dispose = dispose;
			base._002Ector();
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref dispose, null)?.Invoke();
		}
	}

	private readonly ConcurrentDictionary<Type, d9wlvuOvU7P3C3FbfQH> queryStores;

	private readonly ConcurrentDictionary<Type, d9wlvuOvU7P3C3FbfQH> commandStores;

	private CancellationTokenSource cancellationSource;

	public RemoteSnapshot<TValue> GetOrRequest<THandler, TKey, TValue>(TKey key, CancellationToken cancellationToken = default(CancellationToken)) where THandler : IRemoteQueryHandler<TKey, TValue> where TKey : notnull
	{
		return Uw9SrV2Pr7<THandler, TKey, TValue>().IdcOBHYZrq(key, cancellationToken);
	}

	public bool TryGet<THandler, TKey, TValue>(TKey key, out TValue value) where THandler : IRemoteQueryHandler<TKey, TValue> where TKey : notnull
	{
		return Uw9SrV2Pr7<THandler, TKey, TValue>().zArOm0D5kE(key, out value);
	}

	public ValueTask<TValue> GetFreshAsync<THandler, TKey, TValue>(TKey key, CancellationToken cancellationToken = default(CancellationToken)) where THandler : IRemoteQueryHandler<TKey, TValue> where TKey : notnull
	{
		return Uw9SrV2Pr7<THandler, TKey, TValue>().qlVOi7DjXK(key, cancellationToken);
	}

	public void Invalidate<THandler, TKey, TValue>(TKey key) where THandler : IRemoteQueryHandler<TKey, TValue> where TKey : notnull
	{
		Uw9SrV2Pr7<THandler, TKey, TValue>().rauOPyUGOL(key);
	}

	public IDisposable Observe<THandler, TKey, TValue>(TKey key, Action<RemoteSnapshot<TValue>> callback, bool marshalToFrameworkThread = true, CancellationToken cancellationToken = default(CancellationToken)) where THandler : IRemoteQueryHandler<TKey, TValue> where TKey : notnull
	{
		if (!cancellationToken.IsCancellationRequested)
		{
			return Uw9SrV2Pr7<THandler, TKey, TValue>().hr2OoeUcUU(key, callback, marshalToFrameworkThread, cancellationToken);
		}
		return new gOyYxkOJqGLVUqLxF1B(null);
	}

	public RemoteCommandHandle<TResult> StartCommand<THandler, TArgs, TResult>(TArgs args) where THandler : IRemoteCommandHandler<TArgs, TResult>
	{
		return UkASGWG2p7<TResult>().ikPOcDDtVs<THandler, TArgs>(args);
	}

	public bool TryGetCommandSnapshot<TResult>(RemoteCommandHandle<TResult> handle, out RemoteCommandSnapshot<TResult> snapshot)
	{
		return UkASGWG2p7<TResult>().OUPOAmruDC(handle, out snapshot);
	}

	public ValueTask<TResult> ExecuteCommandAsync<THandler, TArgs, TResult>(TArgs args, CancellationToken cancellationToken = default(CancellationToken)) where THandler : IRemoteCommandHandler<TArgs, TResult>
	{
		return MiLS64ExLv<THandler, TArgs, TResult>(args, cancellationToken);
	}

	internal void eYKSJ0Hiyf<THandler, TKey, TValue>(Func<TKey, bool> predicate) where THandler : IRemoteQueryHandler<TKey, TValue> where TKey : notnull
	{
		Uw9SrV2Pr7<THandler, TKey, TValue>().N4POhESIy6(predicate);
	}

	protected override Task Init()
	{
		exKS2F04so();
		return Task.CompletedTask;
	}

	protected override Task Uninit()
	{
		exKS2F04so();
		return Task.CompletedTask;
	}

	private m6s5IHOlbZ48jOZrq8e<THandler, TKey, TValue> Uw9SrV2Pr7<THandler, TKey, TValue>() where THandler : IRemoteQueryHandler<TKey, TValue> where TKey : notnull
	{
		return (m6s5IHOlbZ48jOZrq8e<THandler, TKey, TValue>)queryStores.GetOrAdd(typeof(m6s5IHOlbZ48jOZrq8e<THandler, TKey, TValue>), (Type _, RemoteInteractionManager manager) => new m6s5IHOlbZ48jOZrq8e<THandler, TKey, TValue>(manager), this);
	}

	private NoXwIZOFsdAD8r3XGLF<TResult> UkASGWG2p7<TResult>()
	{
		return (NoXwIZOFsdAD8r3XGLF<TResult>)commandStores.GetOrAdd(typeof(NoXwIZOFsdAD8r3XGLF<TResult>), (Type _, RemoteInteractionManager manager) => new NoXwIZOFsdAD8r3XGLF<TResult>(manager), this);
	}

	private async ValueTask<TResult> MiLS64ExLv<THandler, TArgs, TResult>(TArgs args, CancellationToken cancellationToken) where THandler : IRemoteCommandHandler<TArgs, TResult>
	{
		using CancellationTokenSource linkedSource = TJZSkRYSlV(cancellationToken);
		return await THandler.ExecuteAsync(args, linkedSource.Token).ConfigureAwait(continueOnCapturedContext: false);
	}

	private CancellationTokenSource TJZSkRYSlV(CancellationToken cancellationToken)
	{
		if (!cancellationToken.CanBeCanceled)
		{
			return CancellationTokenSource.CreateLinkedTokenSource(Volatile.Read(in cancellationSource).Token);
		}
		return CancellationTokenSource.CreateLinkedTokenSource(Volatile.Read(in cancellationSource).Token, cancellationToken);
	}

	private void exKS2F04so()
	{
		g6jSZP12Fh();
		h9sSajjOkt(queryStores);
		h9sSajjOkt(commandStores);
	}

	private void g6jSZP12Fh()
	{
		CancellationTokenSource value = new CancellationTokenSource();
		CancellationTokenSource cancellationTokenSource = Interlocked.Exchange(ref cancellationSource, value);
		try
		{
			cancellationTokenSource.Cancel();
		}
		catch (ObjectDisposedException)
		{
		}
		finally
		{
			cancellationTokenSource.Dispose();
		}
	}

	private static void h9sSajjOkt(ConcurrentDictionary<Type, d9wlvuOvU7P3C3FbfQH> stores)
	{
		foreach (d9wlvuOvU7P3C3FbfQH value in stores.Values)
		{
			value.Reset();
		}
		stores.Clear();
	}

	private static void OjCS0Ers48(Task task)
	{
		task.ContinueWith(delegate(Task completedTask)
		{
			_ = completedTask.Exception;
		}, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
	}

	public RemoteInteractionManager()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		queryStores = new ConcurrentDictionary<Type, d9wlvuOvU7P3C3FbfQH>();
		commandStores = new ConcurrentDictionary<Type, d9wlvuOvU7P3C3FbfQH>();
		cancellationSource = new CancellationTokenSource();
		base._002Ector();
	}
}
