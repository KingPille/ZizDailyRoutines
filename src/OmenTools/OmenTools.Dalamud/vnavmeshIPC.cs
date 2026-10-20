using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;

namespace OmenTools.Dalamud;

public static class vnavmeshIPC
{
	public const string INTERNAL_NAME = "vnavmesh";

	[IPCSubscriber("vnavmesh.Nav.IsReady", DefaultValue = "false")]
	private static IPCSubscriber<bool>? NavIsReady;

	[IPCSubscriber("vnavmesh.Nav.BuildProgress", DefaultValue = "-1")]
	private static IPCSubscriber<float>? NavBuildProgress;

	[IPCSubscriber("vnavmesh.Nav.Reload")]
	private static IPCSubscriber<bool>? NavReload;

	[IPCSubscriber("vnavmesh.Nav.Rebuild")]
	private static IPCSubscriber<bool>? NavRebuild;

	[IPCSubscriber("vnavmesh.Nav.Pathfind")]
	private static IPCSubscriber<Vector3, Vector3, bool, Task<List<Vector3>>>? NavPathfind;

	[IPCSubscriber("vnavmesh.Nav.PathfindWithTolerance")]
	private static IPCSubscriber<Vector3, Vector3, bool, float, Task<List<Vector3>>>? NavPathfindWithTolerance;

	[IPCSubscriber("vnavmesh.Nav.PathfindAvoid")]
	private static IPCSubscriber<Vector3, Vector3, bool, Vector3, float, Task<List<Vector3>>>? NavPathfindAvoid;

	[IPCSubscriber("vnavmesh.Nav.PathfindCancelable")]
	private static IPCSubscriber<Vector3, Vector3, bool, CancellationToken, Task<List<Vector3>>>? NavPathfindCancelable;

	[IPCSubscriber("vnavmesh.Nav.IsAutoLoad", DefaultValue = "false")]
	private static IPCSubscriber<bool>? NavIsAutoLoad;

	[IPCSubscriber("vnavmesh.Nav.SetAutoLoad")]
	private static IPCSubscriber<bool, object>? NavSetAutoLoad;

	[IPCSubscriber("vnavmesh.Nav.PathfindNumQueued", DefaultValue = "0")]
	private static IPCSubscriber<int>? NavPathfindNumQueued;

	[IPCSubscriber("vnavmesh.Nav.BuildBitmap")]
	private static IPCSubscriber<Vector3, string, float, bool>? NavBuildBitmap;

	[IPCSubscriber("vnavmesh.Nav.BuildBitmapBounded")]
	private static IPCSubscriber<Vector3, string, float, Vector3, Vector3, bool>? NavBuildBitmapBounded;

	[IPCSubscriber("vnavmesh.Nav.BuildBitmapMulti")]
	private static IPCSubscriber<List<Vector3>, string, float, bool>? NavBuildBitmapMulti;

	[IPCSubscriber("vnavmesh.Nav.BuildBitmapMultiBounded")]
	private static IPCSubscriber<List<Vector3>, string, float, Vector3, Vector3, bool>? NavBuildBitmapMultiBounded;

	[IPCSubscriber("vnavmesh.Query.Mesh.NearestPoint")]
	private static IPCSubscriber<Vector3, float, float, Vector3?>? QueryMeshNearestPoint;

	[IPCSubscriber("vnavmesh.Query.Mesh.IsPointOnMesh")]
	private static IPCSubscriber<Vector3, float, bool, bool>? QueryMeshIsPointOnMesh;

	[IPCSubscriber("vnavmesh.Query.Mesh.NearestPointReachable")]
	private static IPCSubscriber<Vector3, float, float, Vector3?>? QueryMeshNearestPointReachable;

	[IPCSubscriber("vnavmesh.Query.Mesh.PointOnFloor")]
	private static IPCSubscriber<Vector3, bool, float, Vector3?>? QueryPointOnFloor;

	[IPCSubscriber("vnavmesh.Query.Mesh.FlagToPoint")]
	private static IPCSubscriber<Vector3?>? QueryMeshFlagToPoint;

	[IPCSubscriber("vnavmesh.Path.MoveTo")]
	private static IPCSubscriber<List<Vector3>, bool, object>? PathMoveTo;

	[IPCSubscriber("vnavmesh.Path.Stop")]
	private static IPCSubscriber<object>? PathStop;

	[IPCSubscriber("vnavmesh.Path.IsRunning", DefaultValue = "false")]
	private static IPCSubscriber<bool>? PathIsRunning;

	[IPCSubscriber("vnavmesh.Path.NumWaypoints", DefaultValue = "0")]
	private static IPCSubscriber<int>? PathNumWaypoints;

	[IPCSubscriber("vnavmesh.Path.ListWaypoints")]
	private static IPCSubscriber<List<Vector3>>? PathListWaypoints;

	[IPCSubscriber("vnavmesh.Path.GetMovementAllowed", DefaultValue = "false")]
	private static IPCSubscriber<bool>? PathGetMovementAllowed;

	[IPCSubscriber("vnavmesh.Path.SetMovementAllowed")]
	private static IPCSubscriber<bool, object>? PathSetMovementAllowed;

	[IPCSubscriber("vnavmesh.Path.GetAlignCamera", DefaultValue = "false")]
	private static IPCSubscriber<bool>? PathGetAlignCamera;

	[IPCSubscriber("vnavmesh.Path.SetAlignCamera")]
	private static IPCSubscriber<bool, object>? PathSetAlignCamera;

	[IPCSubscriber("vnavmesh.Path.GetTolerance", DefaultValue = "0")]
	private static IPCSubscriber<float>? PathGetTolerance;

	[IPCSubscriber("vnavmesh.Path.SetTolerance")]
	private static IPCSubscriber<float, object>? PathSetTolerance;

	[IPCSubscriber("vnavmesh.SimpleMove.PathfindAndMoveTo")]
	private static IPCSubscriber<Vector3, bool, bool>? SimpleMovePathfindAndMoveTo;

	[IPCSubscriber("vnavmesh.SimpleMove.PathfindAndMoveCloseTo")]
	private static IPCSubscriber<Vector3, bool, float, bool>? PathfindAndMoveToCloseTo;

	[IPCSubscriber("vnavmesh.SimpleMove.PathfindInProgress", DefaultValue = "false")]
	private static IPCSubscriber<bool>? PathfindInProgress;

	[IPCSubscriber("vnavmesh.Nav.PathfindInProgress", DefaultValue = "false")]
	private static IPCSubscriber<bool>? NavPathfindInProgress;

	[IPCSubscriber("vnavmesh.Nav.PathfindCancelAll")]
	private static IPCSubscriber<object>? PathfindCancelAll;

	[IPCSubscriber("vnavmesh.Window.IsOpen", DefaultValue = "false")]
	private static IPCSubscriber<bool>? WindowIsOpen;

	[IPCSubscriber("vnavmesh.Window.SetOpen")]
	private static IPCSubscriber<bool, object>? WindowSetOpen;

	[IPCSubscriber("vnavmesh.DTR.IsShown", DefaultValue = "false")]
	private static IPCSubscriber<bool>? DTRIsShown;

	[IPCSubscriber("vnavmesh.DTR.SetShown")]
	private static IPCSubscriber<bool, object>? DTRSetShown;

	public static bool IsPluginEnabled()
	{
		return DalamudServiceExtension.Instance().IsPluginEnabled("vnavmesh");
	}

	public static bool GetIsNavReady()
	{
		IPCSubscriber<bool> navIsReady = NavIsReady;
		if (navIsReady == null)
		{
			return false;
		}
		return navIsReady;
	}

	public static float GetNavBuildProgress()
	{
		IPCSubscriber<float> navBuildProgress = NavBuildProgress;
		if (navBuildProgress == null)
		{
			return 0f;
		}
		return navBuildProgress;
	}

	public static void ReloadNav()
	{
		NavReload?.InvokeFunc();
	}

	public static void RebuildNav()
	{
		NavRebuild?.InvokeFunc();
	}

	public static Task<List<Vector3>>? Pathfind(Vector3 from, Vector3 to, bool fly = false)
	{
		return NavPathfind?.InvokeFunc(from, to, fly);
	}

	public static Task<List<Vector3>>? PathfindWithTolerance(Vector3 from, Vector3 to, bool fly, float range)
	{
		return NavPathfindWithTolerance?.InvokeFunc(from, to, fly, range);
	}

	public static Task<List<Vector3>>? PathfindAvoid(Vector3 from, Vector3 to, bool fly, Vector3 avoidCenter, float avoidRadius)
	{
		return NavPathfindAvoid?.InvokeFunc(from, to, fly, avoidCenter, avoidRadius);
	}

	public static Task<List<Vector3>>? PathfindCancelable(Vector3 from, Vector3 to, bool fly, CancellationToken cancellationToken)
	{
		return NavPathfindCancelable?.InvokeFunc(from, to, fly, cancellationToken);
	}

	public static int GetQueuedPathfindCount()
	{
		IPCSubscriber<int> navPathfindNumQueued = NavPathfindNumQueued;
		if (navPathfindNumQueued == null)
		{
			return 0;
		}
		return navPathfindNumQueued;
	}

	public static bool BuildNavBitmap(Vector3 startingPos, string filename, float pixelSize)
	{
		return NavBuildBitmap?.InvokeFunc(startingPos, filename, pixelSize) ?? false;
	}

	public static bool BuildNavBoundedBitmap(Vector3 startingPos, string filename, float pixelSize, Vector3 minBounds, Vector3 maxBounds)
	{
		return NavBuildBitmapBounded?.InvokeFunc(startingPos, filename, pixelSize, minBounds, maxBounds) ?? false;
	}

	public static bool BuildNavMultiBitmap(List<Vector3> startingPositions, string filename, float pixelSize)
	{
		return NavBuildBitmapMulti?.InvokeFunc(startingPositions, filename, pixelSize) ?? false;
	}

	public static bool BuildNavMultiBoundedBitmap(List<Vector3> startingPositions, string filename, float pixelSize, Vector3 minBounds, Vector3 maxBounds)
	{
		return NavBuildBitmapMultiBounded?.InvokeFunc(startingPositions, filename, pixelSize, minBounds, maxBounds) ?? false;
	}

	public static bool GetIsAutoLoadNav()
	{
		IPCSubscriber<bool> navIsAutoLoad = NavIsAutoLoad;
		if (navIsAutoLoad == null)
		{
			return false;
		}
		return navIsAutoLoad;
	}

	public static void SetIsAutoLoadNav(bool value)
	{
		NavSetAutoLoad?.InvokeAction(value);
	}

	public static bool GetIsNavPathfindInProgress()
	{
		IPCSubscriber<bool> navPathfindInProgress = NavPathfindInProgress;
		if (navPathfindInProgress == null)
		{
			return false;
		}
		return navPathfindInProgress;
	}

	public static Vector3? QueryNearestPointOnMesh(Vector3 pos, float halfExtentXZ, float halfExtentY)
	{
		return QueryMeshNearestPoint?.InvokeFunc(pos, halfExtentXZ, halfExtentY);
	}

	public static bool QueryIsPointOnMesh(Vector3 pos, float halfExtentY, bool allowUnreachable)
	{
		return QueryMeshIsPointOnMesh?.InvokeFunc(pos, halfExtentY, allowUnreachable) ?? false;
	}

	public static Vector3? QueryNearestPointReachable(Vector3 pos, float halfExtentXZ, float halfExtentY)
	{
		return QueryMeshNearestPointReachable?.InvokeFunc(pos, halfExtentXZ, halfExtentY);
	}

	public static Vector3? QueryFlagToPoint()
	{
		return QueryMeshFlagToPoint?.InvokeFunc();
	}

	public static Vector3? QueryMeshPointOnFloor(Vector3 pos, bool allowUnlandable, float halfExtentXZ)
	{
		return QueryPointOnFloor?.InvokeFunc(pos, allowUnlandable, halfExtentXZ);
	}

	public static void PathfindWithPath(List<Vector3> waypoints, bool fly)
	{
		PathMoveTo?.InvokeAction(waypoints, fly);
	}

	public static void StopPathfind()
	{
		PathStop?.InvokeAction();
	}

	public static bool GetIsPathfindRunning()
	{
		IPCSubscriber<bool> pathIsRunning = PathIsRunning;
		if (pathIsRunning == null)
		{
			return false;
		}
		return pathIsRunning;
	}

	public static int GetPathWayPointCount()
	{
		IPCSubscriber<int> pathNumWaypoints = PathNumWaypoints;
		if (pathNumWaypoints == null)
		{
			return 0;
		}
		return pathNumWaypoints;
	}

	public static List<Vector3> GetPathfindWayPoints()
	{
		return PathListWaypoints?.InvokeFunc() ?? new List<Vector3>();
	}

	public static bool GetIsPathfindMovementAllowed()
	{
		IPCSubscriber<bool> pathGetMovementAllowed = PathGetMovementAllowed;
		if (pathGetMovementAllowed == null)
		{
			return false;
		}
		return pathGetMovementAllowed;
	}

	public static void SetIsPathfindMovementAllowed(bool value)
	{
		PathSetMovementAllowed?.InvokeAction(value);
	}

	public static bool GetIsPathfindAlignCamera()
	{
		IPCSubscriber<bool> pathGetAlignCamera = PathGetAlignCamera;
		if (pathGetAlignCamera == null)
		{
			return false;
		}
		return pathGetAlignCamera;
	}

	public static void SetIsPathfindAlignCamera(bool value)
	{
		PathSetAlignCamera?.InvokeAction(value);
	}

	public static float GetPathfindTolerance()
	{
		IPCSubscriber<float> pathGetTolerance = PathGetTolerance;
		if (pathGetTolerance == null)
		{
			return 0f;
		}
		return pathGetTolerance;
	}

	public static void SetPathfindTolerance(float tolerance)
	{
		PathSetTolerance?.InvokeAction(tolerance);
	}

	public static bool PathfindAndMoveTo(Vector3 pos, bool fly)
	{
		return SimpleMovePathfindAndMoveTo?.InvokeFunc(pos, fly) ?? false;
	}

	public static bool PathfindAndMoveToClosely(Vector3 pos, bool fly, float range)
	{
		return PathfindAndMoveToCloseTo?.InvokeFunc(pos, fly, range) ?? false;
	}

	public static bool GetIsPathfindInProgress()
	{
		IPCSubscriber<bool> pathfindInProgress = PathfindInProgress;
		if (pathfindInProgress == null)
		{
			return false;
		}
		return pathfindInProgress;
	}

	public static void CancelAllQueries()
	{
		PathfindCancelAll?.InvokeAction();
	}

	public static bool GetIsWindowOpen()
	{
		IPCSubscriber<bool> windowIsOpen = WindowIsOpen;
		if (windowIsOpen == null)
		{
			return false;
		}
		return windowIsOpen;
	}

	public static void SetIsWindowOpen(bool value)
	{
		WindowSetOpen?.InvokeAction(value);
	}

	public static bool GetIsDTRShown()
	{
		IPCSubscriber<bool> dTRIsShown = DTRIsShown;
		if (dTRIsShown == null)
		{
			return false;
		}
		return dTRIsShown;
	}

	public static void SetIsDTRShown(bool value)
	{
		DTRSetShown?.InvokeAction(value);
	}
}
