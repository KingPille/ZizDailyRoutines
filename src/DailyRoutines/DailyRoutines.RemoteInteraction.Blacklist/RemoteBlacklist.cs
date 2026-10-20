using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Blacklist.Handlers;
using DailyRoutines.RemoteInteraction.Blacklist.Models;
using DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;

namespace DailyRoutines.RemoteInteraction.Blacklist;

public class RemoteBlacklist : RemoteInteractionBase
{
	public static RemoteSnapshot<BlacklistContentValue> GetContentOrRequest(string blacklistID, string password, int limit = 1000, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<BlacklistContentQueryHandler, BlacklistContentKey, BlacklistContentValue>(new BlacklistContentKey(blacklistID, password, limit, offset), cancellationToken);
	}

	public static bool TryGetContent(string blacklistID, string password, out BlacklistContentValue value, int limit = 1000, int offset = 0)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().TryGet<BlacklistContentQueryHandler, BlacklistContentKey, BlacklistContentValue>(new BlacklistContentKey(blacklistID, password, limit, offset), out value);
	}

	public static ValueTask<BlacklistContentValue> GetContentAsync(string blacklistID, string password, int limit = 1000, int offset = 0, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<BlacklistContentQueryHandler, BlacklistContentKey, BlacklistContentValue>(new BlacklistContentKey(blacklistID, password, limit, offset), cancellationToken);
	}

	public static IDisposable ObserveContent(string blacklistID, string password, Action<RemoteSnapshot<BlacklistContentValue>> callback, int limit = 1000, int offset = 0, bool marshalToFrameworkThread = true, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().Observe<BlacklistContentQueryHandler, BlacklistContentKey, BlacklistContentValue>(new BlacklistContentKey(blacklistID, password, limit, offset), callback, marshalToFrameworkThread, cancellationToken);
	}

	public static RemoteSnapshot<BlacklistLogsResponse> GetLogsOrRequest(string blacklistID, string password, string? player = null, string? status = null, int limit = 50, string? startTime = null, string? endTime = null)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<BlacklistLogsQueryHandler, BlacklistLogsKey, BlacklistLogsResponse>(new BlacklistLogsKey(blacklistID, password, player, status, limit, startTime, endTime));
	}

	public static ValueTask<BlacklistLogsResponse> GetLogsAsync(string blacklistID, string password, string? player = null, string? status = null, int limit = 50, string? startTime = null, string? endTime = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetFreshAsync<BlacklistLogsQueryHandler, BlacklistLogsKey, BlacklistLogsResponse>(new BlacklistLogsKey(blacklistID, password, player, status, limit, startTime, endTime), cancellationToken);
	}

	public static ValueTask<CreateBlacklistResult> CreateAsync(string name, string password, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().ExecuteCommandAsync<CreateBlacklistCommandHandler, CreateBlacklistArgs, CreateBlacklistResult>(new CreateBlacklistArgs(name, password), cancellationToken);
	}

	public static ValueTask<CreateSharedBlacklistResult> CreateSharedAsync(string name, string password, DateTime expiresAt, IReadOnlyList<SharedBlacklistEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().ExecuteCommandAsync<CreateSharedBlacklistCommandHandler, CreateSharedBlacklistArgs, CreateSharedBlacklistResult>(new CreateSharedBlacklistArgs(name, password, expiresAt, entries), cancellationToken);
	}

	public static RemoteCommandHandle<UpdateBlacklistResult> StartUpdate(string blacklistID, string password, IReadOnlyList<BlacklistPeopleInfo> entries)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().StartCommand<UpdateBlacklistCommandHandler, UpdateBlacklistArgs, UpdateBlacklistResult>(new UpdateBlacklistArgs(blacklistID, password, entries));
	}

	public static ValueTask<UpdateBlacklistResult> UpdateAsync(string blacklistID, string password, IReadOnlyList<BlacklistPeopleInfo> entries, CancellationToken cancellationToken = default(CancellationToken))
	{
		return ManagerBase<RemoteInteractionManager>.Instance().ExecuteCommandAsync<UpdateBlacklistCommandHandler, UpdateBlacklistArgs, UpdateBlacklistResult>(new UpdateBlacklistArgs(blacklistID, password, entries), cancellationToken);
	}

	public RemoteBlacklist()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
