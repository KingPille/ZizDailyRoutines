using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using BnDxeK5FguM0XdePWY;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using DailyRoutines.Manager;
using Lhk7YYyZgDg4lZPGxVj;
using LqiFr6AUanXXiJKotr;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using QyFnmXyVXpvh4aoXixb;
using Sem6j6yhpQ5uEW1MJxL;
using ThFWmWFIGyTX23kBtl;
using acXh6NyXo8Om6HO9yLt;
using hYVydiyFN238b8srlVt;
using itAnDoW4mgYVlrmtMor;
using nXyRYE1uBjK6gmU4ln;

namespace hcXTsMLIXdeaKdE7xb7;

[ManagerInitOrder(1)]
internal sealed class ILbWvcLd7xPWOTvckYC : ManagerBase<ILbWvcLd7xPWOTvckYC>
{
	private readonly struct tKqKHqjgnD1jHWQp9yj : IEquatable<tKqKHqjgnD1jHWQp9yj>
	{
		public XX0GFgy2GYwT9tTLeoM Credential { get; init; }

		public string Token { get; init; }

		public UInt128 Hash { get; init; }

		public ushort IDType { get; init; }

		public int Priority { get; init; }

		public tKqKHqjgnD1jHWQp9yj(XX0GFgy2GYwT9tTLeoM Credential, string Token, UInt128 Hash, ushort IDType, int Priority)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.Credential = Credential;
			this.Token = Token;
			this.Hash = Hash;
			this.IDType = IDType;
			this.Priority = Priority;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEF143 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DA3227 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
			if (wfJjw5bqkP(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool wfJjw5bqkP(StringBuilder builder)
		{
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6116718E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402));
			builder.Append(Credential);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2264B73 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44));
			builder.Append((object?)Token);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF04A8E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d));
			builder.Append(Hash.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C85BE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
			builder.Append(IDType.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768FCA15 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46));
			builder.Append(Priority.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(tKqKHqjgnD1jHWQp9yj left, tKqKHqjgnD1jHWQp9yj right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(tKqKHqjgnD1jHWQp9yj left, tKqKHqjgnD1jHWQp9yj right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (((EqualityComparer<XX0GFgy2GYwT9tTLeoM>.Default.GetHashCode(Credential) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Token)) * -1521134295 + EqualityComparer<UInt128>.Default.GetHashCode(Hash)) * -1521134295 + EqualityComparer<ushort>.Default.GetHashCode(IDType)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Priority);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is tKqKHqjgnD1jHWQp9yj)
			{
				return Equals((tKqKHqjgnD1jHWQp9yj)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(tKqKHqjgnD1jHWQp9yj other)
		{
			if (EqualityComparer<XX0GFgy2GYwT9tTLeoM>.Default.Equals(Credential, other.Credential) && EqualityComparer<string>.Default.Equals(Token, other.Token) && EqualityComparer<UInt128>.Default.Equals(Hash, other.Hash) && EqualityComparer<ushort>.Default.Equals(IDType, other.IDType))
			{
				return EqualityComparer<int>.Default.Equals(Priority, other.Priority);
			}
			return false;
		}

		[CompilerGenerated]
		public void TR5jE9UgbP(out XX0GFgy2GYwT9tTLeoM Credential, out string Token, out UInt128 Hash, out ushort IDType, out int Priority)
		{
			Credential = this.Credential;
			Token = this.Token;
			Hash = this.Hash;
			IDType = this.IDType;
			Priority = this.Priority;
		}
	}

	private readonly struct rqoG5fjbalSoiFRWkVx : IEquatable<rqoG5fjbalSoiFRWkVx>
	{
		[CompilerGenerated]
		private readonly bool _003CIsBlacklisted_003Ek__BackingField;

		public XX0GFgy2GYwT9tTLeoM Credential { get; init; }

		public UInt128 Hash { get; init; }

		public ushort IDType { get; init; }

		public int Priority { get; init; }

		public bool IsValid { get; init; }

		public bool IsBlacklisted
		{
			[CompilerGenerated]
			get
			{
				return false;
			}
			[CompilerGenerated]
			init
			{
			}
		}

		public bool IsPremium { get; init; }

		public DateTime? ExpiredTime { get; init; }

		public rqoG5fjbalSoiFRWkVx(XX0GFgy2GYwT9tTLeoM Credential, UInt128 Hash, ushort IDType, int Priority, bool IsValid, bool IsBlacklisted, bool IsPremium, DateTime? ExpiredTime)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.Credential = Credential;
			this.Hash = Hash;
			this.IDType = IDType;
			this.Priority = Priority;
			this.IsValid = IsValid;
			this.IsBlacklisted = IsBlacklisted;
			this.IsPremium = IsPremium;
			this.ExpiredTime = ExpiredTime;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x22648ED ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4118B31B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
			if (glFjFnQamp(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool glFjFnQamp(StringBuilder builder)
		{
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C795740 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00));
			builder.Append(Credential);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200FBE01 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));
			builder.Append(Hash.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28285D47 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
			builder.Append(IDType.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x41190AE7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
			builder.Append(Priority.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D343E28 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33));
			builder.Append(IsValid.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47336591 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5));
			builder.Append(IsBlacklisted.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBE6B9D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
			builder.Append(IsPremium.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BA8949 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
			builder.Append(ExpiredTime.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(rqoG5fjbalSoiFRWkVx left, rqoG5fjbalSoiFRWkVx right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(rqoG5fjbalSoiFRWkVx left, rqoG5fjbalSoiFRWkVx right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((((EqualityComparer<XX0GFgy2GYwT9tTLeoM>.Default.GetHashCode(Credential) * -1521134295 + EqualityComparer<UInt128>.Default.GetHashCode(Hash)) * -1521134295 + EqualityComparer<ushort>.Default.GetHashCode(IDType)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Priority)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(IsValid)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(IsBlacklisted)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(IsPremium)) * -1521134295 + EqualityComparer<DateTime?>.Default.GetHashCode(ExpiredTime);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is rqoG5fjbalSoiFRWkVx)
			{
				return Equals((rqoG5fjbalSoiFRWkVx)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(rqoG5fjbalSoiFRWkVx other)
		{
			if (EqualityComparer<XX0GFgy2GYwT9tTLeoM>.Default.Equals(Credential, other.Credential) && EqualityComparer<UInt128>.Default.Equals(Hash, other.Hash) && EqualityComparer<ushort>.Default.Equals(IDType, other.IDType) && EqualityComparer<int>.Default.Equals(Priority, other.Priority) && EqualityComparer<bool>.Default.Equals(IsValid, other.IsValid) && EqualityComparer<bool>.Default.Equals(IsBlacklisted, other.IsBlacklisted) && EqualityComparer<bool>.Default.Equals(IsPremium, other.IsPremium))
			{
				return EqualityComparer<DateTime?>.Default.Equals(ExpiredTime, other.ExpiredTime);
			}
			return false;
		}

		[CompilerGenerated]
		public void MB7jqpnA5r(out XX0GFgy2GYwT9tTLeoM Credential, out UInt128 Hash, out ushort IDType, out int Priority, out bool IsValid, out bool IsBlacklisted, out bool IsPremium, out DateTime? ExpiredTime)
		{
			Credential = this.Credential;
			Hash = this.Hash;
			IDType = this.IDType;
			Priority = this.Priority;
			IsValid = this.IsValid;
			IsBlacklisted = this.IsBlacklisted;
			IsPremium = this.IsPremium;
			ExpiredTime = this.ExpiredTime;
		}
	}

	private static readonly TimeSpan RefreshDebounceDelay;

	private static readonly SemaphoreSlim RefreshGate;

	private readonly Dictionary<Type, NRm8GoyuwLfFLeEjeU6> verifyClients;

	private readonly Dictionary<Type, XX0GFgy2GYwT9tTLeoM> credentialInstances;

	private CancellationTokenSource? refreshCancellationSource;

	private long refreshRequestID;

	internal bool IsRefreshingAuthInfo { get; private set; }

	protected override Task Init()
	{
		foreach (Type item in EdZHWnmL1S())
		{
			if (Activator.CreateInstance(item, nonPublic: true) is NRm8GoyuwLfFLeEjeU6 value)
			{
				verifyClients[item] = value;
			}
		}
		foreach (Type item2 in DrrHTks7WL())
		{
			if (Activator.CreateInstance(item2, nonPublic: true) is XX0GFgy2GYwT9tTLeoM xX0GFgy2GYwT9tTLeoM)
			{
				xX0GFgy2GYwT9tTLeoM.imUyUSD2Ep += P6aLXlbjds;
				credentialInstances[item2] = xX0GFgy2GYwT9tTLeoM;
			}
		}
		foreach (NRm8GoyuwLfFLeEjeU6 value2 in verifyClients.Values)
		{
			value2.Init();
		}
		return Task.CompletedTask;
	}

	protected override Task PostInit()
	{
		OmenServiceBase<GameState>.Instance().Login += nu8LufKx4V;
		E41Hy1O85R(useDebounce: false);
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		OmenServiceBase<GameState>.Instance().Login -= nu8LufKx4V;
		YtSHt7PFjv();
		IsRefreshingAuthInfo = false;
		return Task.CompletedTask;
	}

	protected override Task Uninit()
	{
		foreach (XX0GFgy2GYwT9tTLeoM value in credentialInstances.Values)
		{
			value.imUyUSD2Ep -= P6aLXlbjds;
			value.Dispose();
		}
		credentialInstances.Clear();
		foreach (NRm8GoyuwLfFLeEjeU6 value2 in verifyClients.Values)
		{
			value2.Uninit();
		}
		verifyClients.Clear();
		return Task.CompletedTask;
	}

	internal void WucLU7CitH()
	{
		E41Hy1O85R(useDebounce: false);
	}

	private void nu8LufKx4V()
	{
		E41Hy1O85R(useDebounce: false);
	}

	private void P6aLXlbjds(XX0GFgy2GYwT9tTLeoM _)
	{
		E41Hy1O85R(useDebounce: true);
	}

	internal T? GE5LYllDIN<T>() where T : NRm8GoyuwLfFLeEjeU6
	{
		verifyClients.TryGetValue(typeof(T), out NRm8GoyuwLfFLeEjeU6 value);
		return (T)value;
	}

	internal T eVPLzA26sb<T>() where T : XX0GFgy2GYwT9tTLeoM
	{
		if (!credentialInstances.TryGetValue(typeof(T), out XX0GFgy2GYwT9tTLeoM value))
		{
			throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C33EB5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0) + typeof(T).Name + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702AA683 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
		}
		return (T)value;
	}

	internal bool lDEHnP9BYU<T>([NotNullWhen(true)] out T? credential) where T : XX0GFgy2GYwT9tTLeoM
	{
		credential = credentialInstances.GetValueOrDefault(typeof(T)) as T;
		return credential != null;
	}

	private void E41Hy1O85R(bool useDebounce)
	{
	}

	private async Task N8BHLAeiMO(long requestID, TimeSpan debounceDelay, CancellationToken cancellationToken, bool reloadModules = true)
	{
		_ = 2;
		try
		{
			if (debounceDelay > TimeSpan.Zero)
			{
				await Task.Delay(debounceDelay, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			await RefreshGate.WaitAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			try
			{
				cancellationToken.ThrowIfCancellationRequested();
				await Hf1HHATb19(cancellationToken, reloadModules).ConfigureAwait(continueOnCapturedContext: false);
			}
			finally
			{
				RefreshGate.Release();
			}
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
		}
		catch (Exception ex2)
		{
			DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA79F41 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff), ex2);
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E4602BF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e)));
		}
		finally
		{
			if (Volatile.Read(in refreshRequestID) == requestID)
			{
				IsRefreshingAuthInfo = false;
			}
		}
	}

	private async Task Hf1HHATb19(CancellationToken cancellationToken, bool reloadModules = true)
	{
		sTVhMSySSmLd9sTlJde authClient = GE5LYllDIN<sTVhMSySSmLd9sTlJde>();
		S3DJJ3yPZjLumXepvvO dbClient = GE5LYllDIN<S3DJJ3yPZjLumXepvvO>();
		xuH6tCybRrvw1o0SxGV passAuthClient = GE5LYllDIN<xuH6tCybRrvw1o0SxGV>();
		if (authClient == null || dbClient == null || passAuthClient == null)
		{
			return;
		}
		List<tKqKHqjgnD1jHWQp9yj> snapshots = aWmH9BUsGk();
		string extraInfo = (GameState.IsLoggedIn ? UdFHCq9wEL() : string.Empty);
		ulong contentID = LocalPlayerState.ContentID;
		bool isAllowToPassAuth = await xuH6tCybRrvw1o0SxGV.lniycReZTF().ConfigureAwait(continueOnCapturedContext: false);
		cancellationToken.ThrowIfCancellationRequested();
		bool isLive = await authClient.ph0yfBchin().ConfigureAwait(continueOnCapturedContext: false);
		cancellationToken.ThrowIfCancellationRequested();
		List<rqoG5fjbalSoiFRWkVx> validationResults = new List<rqoG5fjbalSoiFRWkVx>();
		foreach (tKqKHqjgnD1jHWQp9yj item2 in snapshots)
		{
			rqoG5fjbalSoiFRWkVx item = await q45HV1mqdJ(item2, authClient, dbClient, contentID, extraInfo, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			validationResults.Add(item);
			if (item.IsBlacklisted)
			{
				break;
			}
		}
		rqoG5fjbalSoiFRWkVx rqoG5fjbalSoiFRWkVx2 = (from result in validationResults
			where result.IsValid
			orderby result.Priority
			select result).FirstOrDefault();
		bool isBlacklisted = validationResults.Any((rqoG5fjbalSoiFRWkVx result) => result.IsBlacklisted);
		bool isPremium = validationResults.Any((rqoG5fjbalSoiFRWkVx result) => result.IsPremium);
		wp5Hf6QmEh(snapshots, validationResults);
		authClient.pxOy9x9KP8(isLive, rqoG5fjbalSoiFRWkVx2.IsValid);
		dbClient.pHWyov3R0N(isBlacklisted, isPremium);
		passAuthClient.otmyqw1iYH(isAllowToPassAuth);
		if (false)
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C7954FC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00)));
			await Y5SWncTBsGUDX5l2wY.ubxsjVN6F();
		}
		else if (reloadModules)
		{
			await sZIHSSTJEM(cancellationToken);
		}
	}

	private static async Task sZIHSSTJEM(CancellationToken cancellationToken)
	{
		await TaskExtension.WaitForConditionAsync(delegate
		{
			ModuleManager moduleManager = Y5SWncTBsGUDX5l2wY.S36MXGucI<ModuleManager>();
			return moduleManager != null && moduleManager.IsInitialized && !moduleManager.IsDisposed && !moduleManager.IsInitializingAllModules && !moduleManager.IsLoadingAllModules;
		}, TimeSpan.FromSeconds(180L), cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		cancellationToken.ThrowIfCancellationRequested();
		await ManagerBase<ModuleManager>.Instance().hr8SMBXq7x().ConfigureAwait(continueOnCapturedContext: false);
		cancellationToken.ThrowIfCancellationRequested();
	}

	private static async Task<rqoG5fjbalSoiFRWkVx> q45HV1mqdJ(tKqKHqjgnD1jHWQp9yj snapshot, sTVhMSySSmLd9sTlJde authClient, S3DJJ3yPZjLumXepvvO dbClient, ulong contentID, string extraInfo, CancellationToken cancellationToken)
	{
		_ = 3;
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			bool num = await S3DJJ3yPZjLumXepvvO.AiWygRpDL0(snapshot.Token).ConfigureAwait(continueOnCapturedContext: false);
			cancellationToken.ThrowIfCancellationRequested();
			if (num)
			{
				return new rqoG5fjbalSoiFRWkVx(snapshot.Credential, snapshot.Hash, snapshot.IDType, snapshot.Priority, IsValid: false, IsBlacklisted: true, IsPremium: false, null);
			}
			bool isPremium = await dbClient.PjeywpIMOJ(snapshot.Token).ConfigureAwait(continueOnCapturedContext: false);
			cancellationToken.ThrowIfCancellationRequested();
			if (contentID != 0L)
			{
				await dbClient.ifvyE0fWeW(snapshot.Token, contentID).ConfigureAwait(continueOnCapturedContext: false);
				cancellationToken.ThrowIfCancellationRequested();
			}
			var (flag, num2) = await authClient.nkOyCxtjjT(snapshot.Hash, snapshot.IDType, extraInfo).ConfigureAwait(continueOnCapturedContext: false);
			if (!flag || num2 == 0)
			{
				return new rqoG5fjbalSoiFRWkVx(snapshot.Credential, snapshot.Hash, snapshot.IDType, snapshot.Priority, IsValid: false, IsBlacklisted: false, isPremium, null);
			}
			DateTime value = num2.ToUTCDateTimeFromUnixSeconds().ToLocalTime();
			return new rqoG5fjbalSoiFRWkVx(snapshot.Credential, snapshot.Hash, snapshot.IDType, snapshot.Priority, IsValid: true, IsBlacklisted: false, isPremium, value);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}
		catch (Exception ex2)
		{
			DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E384251 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997) + snapshot.Credential.GetType().Name, ex2);
			return new rqoG5fjbalSoiFRWkVx(snapshot.Credential, snapshot.Hash, snapshot.IDType, snapshot.Priority, IsValid: false, IsBlacklisted: false, IsPremium: false, null);
		}
	}

	private List<tKqKHqjgnD1jHWQp9yj> aWmH9BUsGk()
	{
		List<tKqKHqjgnD1jHWQp9yj> list = new List<tKqKHqjgnD1jHWQp9yj>();
		foreach (XX0GFgy2GYwT9tTLeoM item in credentialInstances.Values.OrderBy(dNoHjJqwcM).ThenBy<XX0GFgy2GYwT9tTLeoM, string>((XX0GFgy2GYwT9tTLeoM credential) => credential.GetType().FullName, StringComparer.Ordinal))
		{
			if (string.IsNullOrWhiteSpace(item.Token) || !item.qnly5HnBCD(out var hash))
			{
				item.YBuyDM9sP9(null);
				continue;
			}
			ushort num = XX0GFgy2GYwT9tTLeoM.D4yyeUGbir(item.Token[0]);
			if (num == 0)
			{
				item.YBuyDM9sP9(null);
			}
			else
			{
				list.Add(new tKqKHqjgnD1jHWQp9yj(item, item.Token, hash, num, dNoHjJqwcM(item)));
			}
		}
		return list;
	}

	private static void wp5Hf6QmEh(IReadOnlyList<tKqKHqjgnD1jHWQp9yj> snapshots, IReadOnlyList<rqoG5fjbalSoiFRWkVx> validationResults)
	{
		Dictionary<XX0GFgy2GYwT9tTLeoM, DateTime?> dictionary = validationResults.ToDictionary((rqoG5fjbalSoiFRWkVx result) => result.Credential, (rqoG5fjbalSoiFRWkVx result) => result.ExpiredTime);
		foreach (tKqKHqjgnD1jHWQp9yj snapshot in snapshots)
		{
			dictionary.TryGetValue(snapshot.Credential, out var value);
			snapshot.Credential.YBuyDM9sP9(value, value.HasValue);
		}
	}

	private static string UdFHCq9wEL()
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 4);
		defaultInterpolatedStringHandler.AppendFormatted(LocalPlayerState.Name.NullIfEmpty() ?? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFC7EB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76));
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB34C9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
		World val = GameState.HomeWorldData;
		defaultInterpolatedStringHandler.AppendFormatted(((object)((World)(ref val)).Name/*cast due to constrained. prefix*/).ToString().NullIfEmpty() ?? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5132AA34 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3923A4FC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2));
		val = GameState.CurrentWorldData;
		defaultInterpolatedStringHandler.AppendFormatted(((object)((World)(ref val)).Name/*cast due to constrained. prefix*/).ToString().NullIfEmpty() ?? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2829F2A1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702AA553 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30));
		defaultInterpolatedStringHandler.AppendFormatted(GameState.TerritoryTypeData.ExtractPlaceName().NullIfEmpty() ?? GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CCF6FB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	private static int dNoHjJqwcM(XX0GFgy2GYwT9tTLeoM credential)
	{
		return bwmHO0gt5c(credential.GetType());
	}

	private static int bwmHO0gt5c(Type credentialType)
	{
		if (!(credentialType == typeof(QVayY70lyiGAjopaKX)))
		{
			if (!(credentialType == typeof(zTMwYPctmcDQxmkmbm)))
			{
				if (!(credentialType == typeof(ksA3tkbnvUocY33ps0)))
				{
					return int.MaxValue;
				}
				return 2;
			}
			return 1;
		}
		return 0;
	}

	private CancellationTokenSource efwHKa6HuR()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		CancellationTokenSource? obj = Interlocked.Exchange(ref refreshCancellationSource, cancellationTokenSource);
		obj?.Cancel();
		obj?.Dispose();
		return cancellationTokenSource;
	}

	private void YtSHt7PFjv()
	{
		CancellationTokenSource? obj = Interlocked.Exchange(ref refreshCancellationSource, null);
		obj?.Cancel();
		obj?.Dispose();
	}

	private static IEnumerable<Type> EdZHWnmL1S()
	{
		return (from type in typeof(NRm8GoyuwLfFLeEjeU6).Assembly.GetTypes()
			where typeof(NRm8GoyuwLfFLeEjeU6).IsAssignableFrom(type) && !type.IsAbstract && type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) != null
			select type).OrderBy<Type, string>((Type type) => type.FullName, StringComparer.Ordinal);
	}

	private static IEnumerable<Type> DrrHTks7WL()
	{
		return (from type in typeof(XX0GFgy2GYwT9tTLeoM).Assembly.GetTypes()
			where typeof(XX0GFgy2GYwT9tTLeoM).IsAssignableFrom(type) && !type.IsAbstract && type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) != null
			select type).OrderBy<Type, string>((Type type) => type.FullName, StringComparer.Ordinal);
	}

	public ILbWvcLd7xPWOTvckYC()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		verifyClients = new Dictionary<Type, NRm8GoyuwLfFLeEjeU6>();
		credentialInstances = new Dictionary<Type, XX0GFgy2GYwT9tTLeoM>();
		base._002Ector();
	}

	static ILbWvcLd7xPWOTvckYC()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		RefreshDebounceDelay = TimeSpan.FromMilliseconds(500L);
		RefreshGate = new SemaphoreSlim(1, 1);
	}
}
