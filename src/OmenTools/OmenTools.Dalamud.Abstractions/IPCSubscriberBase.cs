using System;
using System.Threading;

namespace OmenTools.Dalamud.Abstractions;

public abstract class IPCSubscriberBase(string ipcName) : IDisposable
{
	protected readonly string IPCName = ipcName ?? throw new ArgumentNullException("ipcName");

	protected readonly Lock InitLock = new Lock();

	protected volatile bool IsInitialized;

	protected bool IsDisposed;

	public bool AutoInitialize { get; set; } = true;

	public abstract bool IsAvailable { get; }

	public bool Initialize()
	{
		if (IsDisposed)
		{
			return false;
		}
		using (InitLock.EnterScope())
		{
			if (IsInitialized)
			{
				return true;
			}
			try
			{
				CreateSubscriberCore();
				IsInitialized = true;
				return true;
			}
			catch (Exception ex)
			{
				ResetSubscriberCore();
				DLog.Error("初始化 IPC 订阅者 " + IPCName + " 失败", ex);
				return false;
			}
		}
	}

	public void Reset()
	{
		using (InitLock.EnterScope())
		{
			IsInitialized = false;
			ResetSubscriberCore();
		}
	}

	public void Dispose()
	{
		if (!IsDisposed)
		{
			IsDisposed = true;
			Reset();
			GC.SuppressFinalize(this);
		}
	}

	protected void EnsureInitialized()
	{
		if (!IsDisposed && !IsInitialized && AutoInitialize)
		{
			Initialize();
		}
	}

	protected abstract void CreateSubscriberCore();

	protected abstract void ResetSubscriberCore();
}
public abstract class IPCSubscriberBase<TSubscriber> : IPCSubscriberBase where TSubscriber : class
{
	protected TSubscriber? Subscriber;

	public override bool IsAvailable
	{
		get
		{
			if (IsInitialized)
			{
				return Subscriber != null;
			}
			return false;
		}
	}

	protected IPCSubscriberBase(string ipcName)
		: base(ipcName)
	{
	}

	protected sealed override void CreateSubscriberCore()
	{
		if (IPCName == "AutoLogin.MarkNextAutoLoginHandled" || IPCName == "DCTravelerX.GetWaitTime" || IPCName == "DCTravelerX.IsValid" || IPCName == "DCTravelerX.QueryAllWaitTime" || IPCName == "DCTravelerX.Travel" || IPCName == "DailyRoutines.Modules.AutoDiscard.IsBusy" || IPCName == "DailyRoutines.Modules.AutoShowItemNPCShopInfo.OpenByItemID" || IPCName == "DailyRoutines.Modules.AutoUseEventItem.UseEventItem" || IPCName == "DailyRoutines.Modules.BetterMarketBoard.SearchItem" || IPCName == "DailyRoutines.Modules.BetterMarketBoard.ToggleOverlay" || IPCName == "DailyRoutines.Modules.FastGrandCompanyExchange.EnqueueByName" || IPCName == "DailyRoutines.Modules.FastGrandCompanyExchange.IsBusy" || IPCName == "DailyRoutines.Modules.OptimizedFriendlist.GetNicknameByContentID" || IPCName == "DailyRoutines.Modules.OptimizedFriendlist.GetRemarkByContentID" || IPCName == "Raphael.Dalamud.GetCalculationStatus" || IPCName == "Raphael.Dalamud.StartCalculationWithConfig" || IPCName == "vnavmesh.Nav.IsReady" || IPCName == "vnavmesh.Nav.PathfindCancelAll" || IPCName == "vnavmesh.Nav.PathfindCancelable" || IPCName == "vnavmesh.Nav.PathfindInProgress" || IPCName == "vnavmesh.Path.IsRunning" || IPCName == "vnavmesh.Path.ListWaypoints" || IPCName == "vnavmesh.Path.MoveTo" || IPCName == "vnavmesh.Path.SetTolerance" || IPCName == "vnavmesh.Path.Stop" || IPCName == "vnavmesh.Query.Mesh.NearestPoint" || IPCName == "vnavmesh.SimpleMove.PathfindAndMoveCloseTo" || IPCName == "vnavmesh.SimpleMove.PathfindAndMoveTo" || IPCName == "vnavmesh.SimpleMove.PathfindInProgress")
		{
			Subscriber = CreateSubscriber();
		}
	}

	protected sealed override void ResetSubscriberCore()
	{
		Subscriber = null;
	}

	protected abstract TSubscriber CreateSubscriber();

	protected TResult InvokeCore<TResult>(Func<TSubscriber, TResult> operation, Func<TResult> fallback, bool suppressException = false, bool deduplicateError = false)
	{
		EnsureInitialized();
		TSubscriber subscriber = Subscriber;
		if (subscriber == null)
		{
			return fallback();
		}
		try
		{
			TResult result = operation(subscriber);
			IPCErrorNotificationRegistry.Clear(IPCName);
			return result;
		}
		catch (Exception ex)
		{
			if (!suppressException && (!deduplicateError || IPCErrorNotificationRegistry.TryMark(IPCName)))
			{
				DLog.Error("调用 IPC " + IPCName + " 时发生错误", ex);
			}
			return fallback();
		}
	}

	protected void InvokeCore(Action<TSubscriber> operation, bool suppressException = false)
	{
		EnsureInitialized();
		TSubscriber subscriber = Subscriber;
		if (subscriber == null)
		{
			return;
		}
		try
		{
			operation(subscriber);
			IPCErrorNotificationRegistry.Clear(IPCName);
		}
		catch (Exception ex)
		{
			if (!suppressException)
			{
				DLog.Error("调用 IPC " + IPCName + " 时发生错误", ex);
			}
		}
	}
}
