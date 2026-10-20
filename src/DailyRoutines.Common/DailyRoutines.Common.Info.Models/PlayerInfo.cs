using System;
using System.Linq;
using DailyRoutines.Common.Info.Enums;
using Dalamud.Game.Gui.PartyFinder.Types;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;

namespace DailyRoutines.Common.Info.Models;

public sealed class PlayerInfo
{
	public string Name { get; init; }

	public ulong ContentID { get; init; }

	public byte Job { get; init; }

	public ClassJob JobData { get; init; }

	public ushort CurrentWorld { get; init; }

	public World CurrentWorldData { get; init; }

	public ushort HomeWorld { get; init; }

	public World HomeWorldData { get; init; }

	public string FreeCompanyName { get; init; } = string.Empty;

	public PlayerInfoSource Source { get; init; }

	public Type SourceType { get; init; }

	public object RawData { get; init; }

	public PlayerInfo(CharacterData data)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		Source = PlayerInfoSource.InfoProxyCommonList;
		SourceType = typeof(CharacterData);
		RawData = data;
		Name = ((CharacterData)(ref data)).NameString;
		ContentID = data.ContentId;
		Job = data.Job;
		JobData = LuminaGetter.GetRow<ClassJob>((uint)Job).GetValueOrDefault();
		CurrentWorld = data.CurrentWorld;
		CurrentWorldData = LuminaGetter.GetRow<World>((uint)data.CurrentWorld).GetValueOrDefault();
		HomeWorld = data.HomeWorld;
		HomeWorldData = LuminaGetter.GetRow<World>((uint)HomeWorld).GetValueOrDefault();
		FreeCompanyName = ((CharacterData)(ref data)).FCTagString;
	}

	public PlayerInfo(IPartyFinderListing data)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Source = PlayerInfoSource.PartyFinder;
		SourceType = typeof(IPartyFinderListing);
		RawData = data;
		Name = ((object)data.Name).ToString();
		ContentID = data.ContentId;
		Job = data.RawJobsPresent.First();
		JobData = LuminaGetter.GetRow<ClassJob>((uint)Job).GetValueOrDefault();
		CurrentWorld = (ushort)data.CurrentWorld.RowId;
		CurrentWorldData = data.CurrentWorld.Value;
		HomeWorld = (ushort)data.HomeWorld.RowId;
		HomeWorldData = data.HomeWorld.Value;
	}

	public T ConvertFrom<T>()
	{
		object rawData = RawData;
		if (rawData is T)
		{
			return (T)rawData;
		}
		throw new InvalidCastException("无法将原始数据 " + RawData.GetType().Name + " 转换为目标类型 " + typeof(T).Name);
	}
}
