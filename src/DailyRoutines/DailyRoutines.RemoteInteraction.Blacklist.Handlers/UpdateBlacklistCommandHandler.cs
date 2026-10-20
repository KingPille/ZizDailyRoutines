using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Blacklist.Models;
using DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;
using Newtonsoft.Json;
using ycTZ7OSV787Z3a0g7JT;

namespace DailyRoutines.RemoteInteraction.Blacklist.Handlers;

public sealed class UpdateBlacklistCommandHandler : IRemoteCommandHandler<UpdateBlacklistArgs, UpdateBlacklistResult>
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass0_0
	{
		public UpdateBlacklistArgs args;

		public _003C_003Ec__DisplayClass0_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal bool KMkjhZV221(BlacklistContentKey key)
		{
			if (key.BlacklistID == args.BlacklistID)
			{
				return key.Password == args.Password;
			}
			return false;
		}

		internal bool zkBjoedG7s(BlacklistLogsKey key)
		{
			if (key.BlacklistID == args.BlacklistID)
			{
				return key.Password == args.Password;
			}
			return false;
		}
	}

	public static async ValueTask<UpdateBlacklistResult> ExecuteAsync(UpdateBlacklistArgs args, CancellationToken cancellationToken)
	{
		_003C_003Ec__DisplayClass0_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass0_0();
		CS_0024_003C_003E8__locals11.args = args;
		if (string.IsNullOrWhiteSpace(CS_0024_003C_003E8__locals11.args.BlacklistID) || string.IsNullOrWhiteSpace(CS_0024_003C_003E8__locals11.args.Password) || CS_0024_003C_003E8__locals11.args.Entries.Count == 0)
		{
			return new UpdateBlacklistResult(Success: false);
		}
		bool num = JsonConvert.DeserializeObject<UpdateBlacklistResponse>((await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.UpdateBlacklist, new
		{
			blacklist_id = CS_0024_003C_003E8__locals11.args.BlacklistID,
			password = CS_0024_003C_003E8__locals11.args.Password,
			entries = CS_0024_003C_003E8__locals11.args.Entries
		}, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)) ?? string.Empty)?.Success ?? false;
		if (num)
		{
			ManagerBase<RemoteInteractionManager>.Instance().eYKSJ0Hiyf<BlacklistContentQueryHandler, BlacklistContentKey, BlacklistContentValue>((BlacklistContentKey key) => key.BlacklistID == CS_0024_003C_003E8__locals11.args.BlacklistID && key.Password == CS_0024_003C_003E8__locals11.args.Password);
			ManagerBase<RemoteInteractionManager>.Instance().eYKSJ0Hiyf<BlacklistLogsQueryHandler, BlacklistLogsKey, BlacklistLogsResponse>((BlacklistLogsKey key) => key.BlacklistID == CS_0024_003C_003E8__locals11.args.BlacklistID && key.Password == CS_0024_003C_003E8__locals11.args.Password);
		}
		return new UpdateBlacklistResult(num);
	}

	public UpdateBlacklistCommandHandler()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
