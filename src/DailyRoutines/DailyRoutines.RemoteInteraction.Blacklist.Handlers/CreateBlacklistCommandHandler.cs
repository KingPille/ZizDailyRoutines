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

public sealed class CreateBlacklistCommandHandler : IRemoteCommandHandler<CreateBlacklistArgs, CreateBlacklistResult>
{
	public static async ValueTask<CreateBlacklistResult> ExecuteAsync(CreateBlacklistArgs args, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(args.Name) || string.IsNullOrWhiteSpace(args.Password))
		{
			return new CreateBlacklistResult(Success: false, string.Empty, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x654096BC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb)));
		}
		CreateBlacklistResponse createBlacklistResponse = JsonConvert.DeserializeObject<CreateBlacklistResponse>((await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.CreateBlacklist, new
		{
			name = args.Name,
			password = args.Password
		}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? string.Empty);
		return (createBlacklistResponse == null) ? new CreateBlacklistResult(Success: false, string.Empty, LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C79568A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00))) : new CreateBlacklistResult(createBlacklistResponse.Success, createBlacklistResponse.BlacklistID ?? string.Empty, createBlacklistResponse.Message ?? string.Empty);
	}

	public CreateBlacklistCommandHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
