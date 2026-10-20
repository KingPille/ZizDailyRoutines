using System;
using DailyRoutines.Common.RemoteInteraction.Enums;

namespace DailyRoutines.Common.RemoteInteraction.Models;

public readonly record struct RemoteSnapshot<T>(RemoteSnapshotStatus Status, T? Value, DateTime UpdatedAtUTC, Exception? Error)
{
	public bool HasValue
	{
		get
		{
			RemoteSnapshotStatus status = Status;
			if ((uint)(status - 2) <= 1u)
			{
				return true;
			}
			return false;
		}
	}
}
