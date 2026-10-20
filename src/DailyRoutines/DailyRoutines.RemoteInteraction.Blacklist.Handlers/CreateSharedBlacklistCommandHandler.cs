using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Blacklist.Models;
using DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;
using Newtonsoft.Json;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace DailyRoutines.RemoteInteraction.Blacklist.Handlers;

public sealed class CreateSharedBlacklistCommandHandler : IRemoteCommandHandler<CreateSharedBlacklistArgs, CreateSharedBlacklistResult>
{
	public static async ValueTask<CreateSharedBlacklistResult> ExecuteAsync(CreateSharedBlacklistArgs args, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(args.Name) || string.IsNullOrWhiteSpace(args.Password) || args.Entries.Count == 0)
		{
			return new CreateSharedBlacklistResult(Success: false, string.Empty, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA79D47 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
		}
		CreateSharedBlacklistResponse createSharedBlacklistResponse = JsonConvert.DeserializeObject<CreateSharedBlacklistResponse>((await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.CreateSharedBlacklist, new
		{
			name = args.Name,
			password = args.Password,
			expires_at = new DateTimeOffset(args.ExpiresAt).ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture),
			entries = args.Entries
		}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? string.Empty);
		return (createSharedBlacklistResponse == null) ? new CreateSharedBlacklistResult(Success: false, string.Empty, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5583B5C0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d)) : new CreateSharedBlacklistResult(createSharedBlacklistResponse.Success, createSharedBlacklistResponse.BlacklistID ?? string.Empty, createSharedBlacklistResponse.Message ?? string.Empty);
	}

	public CreateSharedBlacklistCommandHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
