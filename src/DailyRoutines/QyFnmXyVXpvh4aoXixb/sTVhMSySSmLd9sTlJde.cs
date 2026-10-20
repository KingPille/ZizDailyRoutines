using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using OmenTools.Dalamud;
using OmenTools.Interop.Runtime;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OtpNet;
using QNYjbDEJoidQFQmMPb;
using RSASharp.Algorithms;
using RSASharp.Cryptography;
using RSASharp.Keys;
using acXh6NyXo8Om6HO9yLt;
using iMuCq4goTTvj7QQIVg;
using itAnDoW4mgYVlrmtMor;
using wlcw3qhrMGJyAEyXVG;

namespace QyFnmXyVXpvh4aoXixb;

internal class sTVhMSySSmLd9sTlJde : NRm8GoyuwLfFLeEjeU6
{
	private const string OVERSEAS_ENDPOINT = "hxxps://auth.n4ko.com/";

	private const string DOMESTIC_ENDPOINT = "hxxps://auth.n3ko.fun/";

	private const string ULTIMATE_ENDPOINT = "hxxps://auth-test.n4ko.com/";

	private const string QUERY_PATH = "api/v1/query";

	private static readonly TimeSpan ProbeFailureCooldown;

	private static readonly string[] AuthEndpoints;

	private readonly SemaphoreSlim endpointProbeSemaphore;

	private Hotp hotpUtil;

	private HttpClient httpClient;

	private RSAKeyMaterial verificationKey;

	private string? activeEndpoint;

	private DateTime lastProbeFailureAt;

	internal bool IsLive { get; private set; }

	internal bool IsValid { get; private set; }

	internal override void Init()
	{
		SocketsHttpHandler handler = new SocketsHttpHandler
		{
			UseProxy = false,
			ConnectTimeout = TimeSpan.FromSeconds(3L),
			ConnectCallback = UQSysAPy8s
		};
		httpClient = OmenServiceBase<HTTPClientHelper>.Instance().Get(handler, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x227C2BD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44));
		httpClient.Timeout = TimeSpan.FromSeconds(10L);
		activeEndpoint = null;
		verificationKey = RSAKeyMaterial.ImportPEM(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642B2510 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df));
		hotpUtil = new Hotp(Base32Encoding.ToBytes(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F70FD17 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0)));
	}

	internal override void Uninit()
	{
		Interlocked.Exchange(ref activeEndpoint, null);
		pxOy9x9KP8(isLive: false, isValid: false);
	}

	internal void pxOy9x9KP8(bool isLive, bool isValid)
	{
		IsLive = isLive;
		IsValid = isValid;
	}

	internal async Task<bool> ph0yfBchin()
	{
		try
		{
			m6YIovwNAdUyKnOtqU req = ATryOjhoje();
			string text = await od9yMnD1SX(Convert.ToBase64String(StructMarshaller.ToBytes(req))).ConfigureAwait(continueOnCapturedContext: false);
			return text != null && RyLytIQPl2(text, (ushort)req.Nonce);
		}
		catch
		{
		}
		return false;
	}

	internal async Task<(bool, uint)> nkOyCxtjjT(UInt128 credential, ushort idType, string msg = "")
	{
		try
		{
			vnxJxSoewtw7WmO2uI vnxJxSoewtw7WmO2uI2 = await Wy8yjK3ks0(credential, idType, msg);
			uint showExpiredTime = vnxJxSoewtw7WmO2uI2.ShowExpiredTime;
			if (vnxJxSoewtw7WmO2uI2.Nonce == 0 || vnxJxSoewtw7WmO2uI2.ExpiredTime == 0 || vnxJxSoewtw7WmO2uI2.IDHash != credential || vnxJxSoewtw7WmO2uI2.PluginID != 1011)
			{
				return (false, showExpiredTime);
			}
			int num = (int)credential;
			bool num2 = ~(num & ~num) + -2024 + ~(num ^ 0x3F3) - 11 * num - 11121 + 11 * (num ^ 0x3F3) + ~(num & -1012) - 2 * ~(num | 0x3F3) + 5 * ~(num | -1012) + 7 * (num & -1012) + 29 * (num & 0x3F3) == 13 * (~(num & ~num) & -2024) + 10 * (~(num & ~num) & 0x7E7) - 11 * ~(num & ~num) - 2 * ~(~(num & ~num) | -2024) + ~(~(num & ~num) | 0x7E7) + 4046 + ~(num ^ 0x3F3) - 11 * num - 11121 + 11 * (num ^ 0x3F3) + ~(num & -1012) - 2 * ~(num | 0x3F3) + 5 * ~(num | -1012) + 7 * (num & -1012) + 29 * (num & 0x3F3);
			bool flag = ~(num & ~num) + -2024 + ~(num ^ 0x3F3) - 11 * num - 11121 + 11 * (num ^ 0x3F3) + ~(num & -1012) - 2 * ~(num | 0x3F3) + 5 * ~(num | -1012) + 7 * (num & -1012) + 29 * (num & 0x3F3) == 13 * (~(num & ~num) & -2024) + 10 * (~(num & ~num) & 0x7E7) - 11 * ~(num & ~num) - 2 * ~(~(num & ~num) | -2024) + ~(~(num & ~num) | 0x7E7) + 4046 + ~(num ^ 0x3F3) - 11 * num - 11121 + 11 * (num ^ 0x3F3) + ~(num & -1012) - 2 * ~(num | 0x3F3) + 5 * ~(num | -1012) + 7 * (num & -1012) + 29 * (num & 0x3F3);
			bool flag2 = ~(num & ~num) + -2024 + ~(num ^ 0x3F3) - 11 * num - 11121 + 11 * (num ^ 0x3F3) + ~(num & -1012) - 2 * ~(num | 0x3F3) + 5 * ~(num | -1012) + 7 * (num & -1012) + 29 * (num & 0x3F3) != 13 * (~(num & ~num) & -2024) + 10 * (~(num & ~num) & 0x7E7) - 11 * ~(num & ~num) - 2 * ~(~(num & ~num) | -2024) + ~(~(num & ~num) | 0x7E7) + 4046 + ~(num ^ 0x3F3) - 11 * num - 11121 + 11 * (num ^ 0x3F3) + ~(num & -1012) - 2 * ~(num | 0x3F3) + 5 * ~(num | -1012) + 7 * (num & -1012) + 29 * (num & 0x3F3);
			bool flag3 = ~(num & ~num) + -2024 + ~(num ^ 0x3F3) - 11 * num - 11121 + 11 * (num ^ 0x3F3) + ~(num & -1012) - 2 * ~(num | 0x3F3) + 5 * ~(num | -1012) + 7 * (num & -1012) + 29 * (num & 0x3F3) == 13 * (~(num & ~num) & -2024) + 10 * (~(num & ~num) & 0x7E7) - 11 * ~(num & ~num) - 2 * ~(~(num & ~num) | -2024) + ~(~(num & ~num) | 0x7E7) + 4046 + ~(num ^ 0x3F3) - 11 * num - 11121 + 11 * (num ^ 0x3F3) + ~(num & -1012) - 2 * ~(num | 0x3F3) + 5 * ~(num | -1012) + 7 * (num & -1012) + 29 * (num & 0x3F3);
			return (num2 && flag && !flag2 && flag3, showExpiredTime);
		}
		catch (Exception)
		{
		}
		return (false, 0u);
	}

	private async Task<vnxJxSoewtw7WmO2uI> Wy8yjK3ks0(UInt128 credential, ushort idType, string message = "")
	{
		try
		{
			m6YIovwNAdUyKnOtqU req = wI7yK2M7ae(credential, idType, message);
			string text = await od9yMnD1SX(Convert.ToBase64String(StructMarshaller.ToBytes(req))).ConfigureAwait(continueOnCapturedContext: false);
			if (text == null)
			{
				return default(vnxJxSoewtw7WmO2uI);
			}
			vnxJxSoewtw7WmO2uI vnxJxSoewtw7WmO2uI2 = StructMarshaller.FromBytes<vnxJxSoewtw7WmO2uI>(RSACryptography.DecryptFromBase64(verificationKey, RSAEncryptionAlgorithm.PKCS1, text));
			string hotp = vnxJxSoewtw7WmO2uI2.Nonce.ToString().PadLeft(6, '0');
			return (!hotpUtil.VerifyHotp(hotp, req.Nonce)) ? default(vnxJxSoewtw7WmO2uI) : vnxJxSoewtw7WmO2uI2;
		}
		catch (Exception)
		{
		}
		return default(vnxJxSoewtw7WmO2uI);
	}

	private static m6YIovwNAdUyKnOtqU ATryOjhoje()
	{
		return new m6YIovwNAdUyKnOtqU
		{
			Nonce = (ushort)Random.Shared.Next(4096, 65535),
			FunctionCode = 0,
			PluginID = 0,
			IDHash = (byte)0,
			NekoVersion = 2,
			Message = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339D2DCE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de)
		};
	}

	private static m6YIovwNAdUyKnOtqU wI7yK2M7ae(UInt128 credential, ushort idType, string message)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected I4, but got Unknown
		return new m6YIovwNAdUyKnOtqU
		{
			Nonce = (ushort)Random.Shared.Next(4096, 65535),
			FunctionCode = 2,
			PluginID = 1011,
			IDHash = credential,
			NekoVersion = 2,
			IDType = idType,
			Message = message,
			GameLanguage = (ushort)(int)GameState.ClientLanguage
		};
	}

	private bool RyLytIQPl2(string encryptedResponse, ushort nonce)
	{
		try
		{
			string hotp = StructMarshaller.FromBytes<EuPQmJP1SPkEdXb5ZI>(RSACryptography.DecryptFromBase64(verificationKey, RSAEncryptionAlgorithm.PKCS1, encryptedResponse)).Nonce.ToString().PadLeft(6, '0');
			return hotpUtil.VerifyHotp(hotp, nonce);
		}
		catch
		{
			return false;
		}
	}

	private async Task<string?> HGTyWICHVP()
	{
		string text = Volatile.Read(in activeEndpoint);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		if (OmenServiceBase<StandardTimeManager>.Instance().UTCNow - lastProbeFailureAt < ProbeFailureCooldown)
		{
			return null;
		}
		return await B5OyTMFjvm().ConfigureAwait(continueOnCapturedContext: false);
	}

	private async Task<string?> B5OyTMFjvm()
	{
		await endpointProbeSemaphore.WaitAsync().ConfigureAwait(continueOnCapturedContext: false);
		try
		{
			string text = Volatile.Read(in activeEndpoint);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			List<Task<(string Endpoint, bool IsAvailable)>> pendingTasks = AuthEndpoints.Select(ynRy1tQYN4).ToList();
			while (pendingTasks.Count > 0)
			{
				Task<(string, bool)> task = await Task.WhenAny(pendingTasks).ConfigureAwait(continueOnCapturedContext: false);
				pendingTasks.Remove(task);
				(string, bool) tuple = await task.ConfigureAwait(continueOnCapturedContext: false);
				if (tuple.Item2)
				{
					U75y3JHJu1(tuple.Item1);
					return tuple.Item1;
				}
			}
			Interlocked.Exchange(ref activeEndpoint, null);
			lastProbeFailureAt = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
			DLog.Warning(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2D343248 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_27ab9f3154924788ac627b1ee5fccf33));
			return null;
		}
		finally
		{
			endpointProbeSemaphore.Release();
		}
	}

	private async Task<(string, bool)> ynRy1tQYN4(string endpoint)
	{
		m6YIovwNAdUyKnOtqU req = ATryOjhoje();
		string text = await oi3y4TfK9a(endpoint, Convert.ToBase64String(StructMarshaller.ToBytes(req))).ConfigureAwait(continueOnCapturedContext: false);
		return (endpoint, text != null && RyLytIQPl2(text, (ushort)req.Nonce));
	}

	private async Task<string?> od9yMnD1SX(string payload)
	{
		string currentEndpoint = await HGTyWICHVP().ConfigureAwait(continueOnCapturedContext: false);
		if (string.IsNullOrEmpty(currentEndpoint))
		{
			return null;
		}
		string text = await oi3y4TfK9a(currentEndpoint, payload).ConfigureAwait(continueOnCapturedContext: false);
		if (text != null)
		{
			return text;
		}
		string alternateEndpoint = AlHyBPVIfh(currentEndpoint);
		Interlocked.CompareExchange(ref activeEndpoint, null, currentEndpoint);
		text = await oi3y4TfK9a(alternateEndpoint, payload).ConfigureAwait(continueOnCapturedContext: false);
		if (text != null)
		{
			U75y3JHJu1(alternateEndpoint, currentEndpoint);
			return text;
		}
		Interlocked.Exchange(ref activeEndpoint, null);
		lastProbeFailureAt = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
		DLog.Warning(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339C9E5E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de));
		return null;
	}

	private async Task<string?> oi3y4TfK9a(string endpoint, string payload)
	{
		_ = 1;
		try
		{
			using StringContent httpContent = new StringContent(payload);
			_ = httpClient;
			ceAymuhSJE(endpoint);
			_ = httpContent;
			throw null;
		}
		catch
		{
			return null;
		}
	}

	private static async ValueTask<Stream> UQSysAPy8s(SocketsHttpConnectionContext context, CancellationToken cancellationToken)
	{
		DnsEndPoint endPoint = context.DnsEndPoint;
		if (IPAddress.TryParse(endPoint.Host, out IPAddress address))
		{
			return await PJ0yp8eSnO(address, endPoint.Port, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		_ = endPoint.Host;
		_ = cancellationToken;
		throw null;
	}

	private static IPAddress[] tbAy8sS4WU(IPAddress[] addresses)
	{
		IPAddress[] array = addresses.Where((IPAddress x) => x.AddressFamily == AddressFamily.InterNetworkV6).ToArray();
		IPAddress[] array2 = addresses.Where((IPAddress x) => x.AddressFamily == AddressFamily.InterNetwork).ToArray();
		int num = Math.Min(array.Length, array2.Length);
		IPAddress[] array3 = new IPAddress[addresses.Length];
		for (int num2 = 0; num2 < num; num2++)
		{
			array3[num2 * 2] = array[num2];
			array3[num2 * 2 + 1] = array2[num2];
		}
		if (array.Length > num)
		{
			array.AsSpan(num).CopyTo(array3.AsSpan(num * 2));
		}
		else
		{
			array2.AsSpan(num).CopyTo(array3.AsSpan(num * 2));
		}
		return array3;
	}

	private static async Task<Stream> BKWyvbHOYD(IPAddress[] addresses, int port, CancellationToken cancellationToken)
	{
		TimeSpan attemptDelay = TimeSpan.FromMilliseconds(250L);
		using CancellationTokenSource raceSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		List<Task<Stream>> attempts = new List<Task<Stream>>();
		List<Exception> failures = new List<Exception>();
		int nextIndex = 0;
		while (true)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (nextIndex < addresses.Length)
			{
				attempts.Add(PJ0yp8eSnO(addresses[nextIndex], port, raceSource.Token));
			}
			nextIndex++;
			if (attempts.Count == 0)
			{
				break;
			}
			Task<Stream> completed = await dRIylZgANO(attempts, nextIndex < addresses.Length, attemptDelay, raceSource.Token).ConfigureAwait(continueOnCapturedContext: false);
			if (completed != null)
			{
				if (completed.IsCompletedSuccessfully)
				{
					await raceSource.CancelAsync().ConfigureAwait(continueOnCapturedContext: false);
					await xbpyxNkeG4(attempts, completed).ConfigureAwait(continueOnCapturedContext: false);
					return completed.Result;
				}
				attempts.Remove(completed);
				if (completed.Exception != null)
				{
					failures.AddRange(completed.Exception.InnerExceptions);
				}
			}
		}
		throw (failures.Count == 1) ? failures[0] : new AggregateException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C8AE0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac), failures);
	}

	private static async Task<Task<Stream>?> dRIylZgANO(List<Task<Stream>> attempts, bool hasPendingAddress, TimeSpan attemptDelay, CancellationToken cancellationToken)
	{
		Task<Task<Stream>> anyAttempt = Task.WhenAny(attempts);
		if (!hasPendingAddress)
		{
			return await anyAttempt.ConfigureAwait(continueOnCapturedContext: false);
		}
		Task task = Task.Delay(attemptDelay, cancellationToken);
		return (await Task.WhenAny(anyAttempt, task).ConfigureAwait(continueOnCapturedContext: false) != anyAttempt) ? null : (await anyAttempt.ConfigureAwait(continueOnCapturedContext: false));
	}

	private static async Task xbpyxNkeG4(List<Task<Stream>> attempts, Task<Stream> winner)
	{
		foreach (Task<Stream> attempt in attempts)
		{
			if (attempt != winner)
			{
				try
				{
					(await attempt.ConfigureAwait(continueOnCapturedContext: false)).Dispose();
				}
				catch
				{
				}
			}
		}
	}

	private static async Task<Stream> PJ0yp8eSnO(IPAddress address, int port, CancellationToken cancellationToken)
	{
		_ = address.AddressFamily;
		_ = 1;
		_ = 6;
		throw null;
	}

	private void U75y3JHJu1(string endpoint, string? failedEndpoint = null)
	{
		if (!string.Equals(Interlocked.Exchange(ref activeEndpoint, endpoint), endpoint, StringComparison.Ordinal))
		{
			if (string.IsNullOrEmpty(failedEndpoint))
			{
				DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x642B1086 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f11a41e9f664db18364eb0afc92a5df) + kneyiqNFB6(endpoint));
			}
			else
			{
				DLog.Warning(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6117CEEC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402) + kneyiqNFB6(failedEndpoint) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768E75A1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46) + kneyiqNFB6(endpoint));
			}
		}
	}

	private static string AlHyBPVIfh(string endpoint)
	{
		if (!string.Equals(endpoint, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A85355 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a), StringComparison.Ordinal))
		{
			return GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D3A216 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1);
		}
		return GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4FBFD77B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_881eb9df907345dea66a751ad6d65e76);
	}

	private static string ceAymuhSJE(string endpoint)
	{
		return endpoint + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E39FE63 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997);
	}

	private static string kneyiqNFB6(string endpoint)
	{
		return new Uri(endpoint).Host;
	}

	public sTVhMSySSmLd9sTlJde()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		endpointProbeSemaphore = new SemaphoreSlim(1, 1);
		lastProbeFailureAt = DateTime.MinValue;
		base._002Ector();
	}

	static sTVhMSySSmLd9sTlJde()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		ProbeFailureCooldown = TimeSpan.FromSeconds(30L);
		AuthEndpoints = new string[3]
		{
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53782579 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09),
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28170CE6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd),
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5B9C44 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a)
		};
	}
}
