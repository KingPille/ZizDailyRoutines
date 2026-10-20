using System;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.ISPTranslation.Models;
using DailyRoutines.RemoteInteraction.ISPTranslation.Models.Responses;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace DailyRoutines.RemoteInteraction.ISPTranslation.Handlers;

public sealed class ISPTranslationQueryHandler : IRemoteQueryHandler<ISPTranslationKey, ISPTranslatorResponse>
{
	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<ISPTranslatorResponse> FetchAsync(ISPTranslationKey key, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(key.Text))
		{
			throw new ArgumentException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4733611B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B3050F4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
		}
		if (key.Text.Length > 64)
		{
			throw new ArgumentException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D21A8C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x22642F7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44));
		}
		return JsonConvert.DeserializeObject<ISPTranslatorResponse>((await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.RequestISPTranslator, new
		{
			text = key.Text,
			locale = key.LocaleName
		}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? string.Empty) ?? throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BA8CC3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
	}

	public ISPTranslationQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static ISPTranslationQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TTL = TimeSpan.FromDays(365);
		FailureTTL = TimeSpan.FromMinutes(1L);
	}
}
