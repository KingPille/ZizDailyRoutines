using System;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;

namespace OmenTools.Dalamud.Services.Game.Abstractions;

public interface IStatus : IEquatable<IStatus>
{
	nint Address { get; }

	uint StatusID { get; }

	RowRef<Status> GameData { get; }

	ushort Param { get; }

	float RemainingTime { get; }

	uint SourceID { get; }

	IGameObject? SourceObject { get; }

	unsafe static IStatus Create(nint address)
	{
		return new Status((Status*)address);
	}
}
