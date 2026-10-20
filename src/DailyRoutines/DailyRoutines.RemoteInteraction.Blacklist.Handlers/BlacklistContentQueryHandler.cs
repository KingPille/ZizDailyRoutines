using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Blacklist.Enums;
using DailyRoutines.RemoteInteraction.Blacklist.Models;
using DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace DailyRoutines.RemoteInteraction.Blacklist.Handlers;

public sealed class BlacklistContentQueryHandler : IRemoteQueryHandler<BlacklistContentKey, BlacklistContentValue>
{
	public static TimeSpan TTL { get; }

	public static TimeSpan FailureTTL { get; }

	public static async ValueTask<BlacklistContentValue> FetchAsync(BlacklistContentKey key, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(key.BlacklistID) || string.IsNullOrWhiteSpace(key.Password))
		{
			throw new ArgumentException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F35AE28 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFB9E01 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc));
		}
		BlacklistResponse blacklistResponse = JsonConvert.DeserializeObject<BlacklistResponse>((await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.GetBlacklist, new
		{
			blacklist_id = key.BlacklistID,
			password = key.Password,
			limit = key.Limit,
			offset = key.Offset
		}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? string.Empty);
		if (blacklistResponse == null || !blacklistResponse.Success)
		{
			return new BlacklistContentValue(new BlacklistInfo(), new List<BlacklistEntry>());
		}
		long result;
		BlacklistInfo info = new BlacklistInfo
		{
			Name = (blacklistResponse.BlacklistInfo?.Name ?? string.Empty),
			Type = (string.Equals(blacklistResponse.BlacklistInfo?.Type, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C83C6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac), StringComparison.OrdinalIgnoreCase) ? BlacklistType.Shared : BlacklistType.Regular),
			ExpiresAt = (long.TryParse(blacklistResponse.BlacklistInfo?.ExpiresAt, out result) ? new long?(result) : ((long?)null))
		};
		List<BlacklistEntry> entries = blacklistResponse.Data?.ConvertAll((BlacklistEntryResponse entry) => new BlacklistEntry
		{
			Time = entry.Time,
			SourcePlayer = (string.IsNullOrWhiteSpace(entry.SourcePlayer) ? 0 : ulong.Parse(entry.SourcePlayer, CultureInfo.InvariantCulture)),
			Player = ulong.Parse(entry.Player, CultureInfo.InvariantCulture),
			Remark = (entry.Remark ?? string.Empty)
		}) ?? new List<BlacklistEntry>();
		return new BlacklistContentValue(info, entries);
	}

	public BlacklistContentQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static BlacklistContentQueryHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		TTL = TimeSpan.FromMinutes(5L);
		FailureTTL = TimeSpan.FromSeconds(20L);
	}
}
