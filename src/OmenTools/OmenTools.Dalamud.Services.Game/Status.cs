using System;
using System.Diagnostics.CodeAnalysis;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;

namespace OmenTools.Dalamud.Services.Game;

internal unsafe readonly struct Status(Status* ptr) : IStatus, IEquatable<IStatus>
{
	public unsafe nint Address => (nint)ptr;

	public unsafe uint StatusID => ((Status)ptr).StatusId;

	public unsafe RowRef<Status> GameData => ((Status)ptr).StatusId.ToLuminaRowRef<Status>();

	public unsafe ushort Param => ((Status)ptr).Param;

	public unsafe float RemainingTime => ((Status)ptr).RemainingTime;

	public unsafe uint SourceID => ((GameObjectId)(&((Status)ptr).SourceObject)).ObjectId;

	public IGameObject? SourceObject => IOmenDalamudService<IObjectTable>.Instance().SearchByID(SourceID);

	public static bool operator ==(Status x, Status y)
	{
		return x.Equals(y);
	}

	public static bool operator !=(Status x, Status y)
	{
		return !(x == y);
	}

	public bool Equals(IStatus? other)
	{
		if (StatusID == other.StatusID && SourceID == other.SourceID && Param == other.Param)
		{
			return RemainingTime == other.RemainingTime;
		}
		return false;
	}

	public override bool Equals([NotNullWhen(true)] object? obj)
	{
		if (obj is Status status)
		{
			return Equals(status);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(StatusID, SourceID, Param, RemainingTime);
	}
}
