using Dalamud.Game;
using Dalamud.Plugin.Services;
using Lumina.Excel;
using OmenTools.Extensions;

namespace OmenTools.Interop.Game.Lumina;

public static class LuminaGetter
{
	public static ExcelSheet<T> Get<T>() where T : struct, IExcelRow<T>
	{
		return DalamudServiceExtension.Instance<IDataManager>().GetExcelSheet<T>((ClientLanguage?)null, (string)null);
	}

	public static SubrowExcelSheet<T> GetSub<T>() where T : struct, IExcelSubrow<T>
	{
		return DalamudServiceExtension.Instance<IDataManager>().GetSubrowExcelSheet<T>((ClientLanguage?)null, (string)null);
	}

	public static bool TryGet<T>(out ExcelSheet<T> sheet) where T : struct, IExcelRow<T>
	{
		sheet = Get<T>();
		return true;
	}

	public static bool TryGetSub<T>(out SubrowExcelSheet<T> sheet) where T : struct, IExcelSubrow<T>
	{
		sheet = GetSub<T>();
		return true;
	}

	public static T? GetRow<T>(uint rowID) where T : struct, IExcelRow<T>
	{
		ExcelSheet<T> sheet;
		return (TryGet<T>(out sheet) ? sheet : null)?.GetRowOrDefault(rowID);
	}

	public static T? GetSubRow<T>(uint rowID, int index = 0) where T : struct, IExcelSubrow<T>
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		SubrowExcelSheet<T> sheet;
		return (TryGetSub<T>(out sheet) ? sheet : null)?.GetRowOrDefault(rowID)?[index];
	}

	public static SubrowCollection<T>? GetSubRowAll<T>(uint rowID) where T : struct, IExcelSubrow<T>
	{
		SubrowExcelSheet<T> sheet;
		return (TryGetSub<T>(out sheet) ? sheet : null)?.GetRowOrDefault(rowID);
	}

	public static T GetRowOrDefault<T>(uint rowID) where T : struct, IExcelRow<T>
	{
		ExcelSheet<T> sheet;
		return (TryGet<T>(out sheet) ? sheet : null).GetRowOrDefault(rowID).GetValueOrDefault();
	}

	public static T GetSubRowOrDefault<T>(uint rowID, int index = 0) where T : struct, IExcelSubrow<T>
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		SubrowExcelSheet<T> sheet;
		return (TryGetSub<T>(out sheet) ? sheet : null).GetRowOrDefault(rowID).GetValueOrDefault()[index];
	}

	public static bool TryGetRow<T>(uint rowID, out T item) where T : struct, IExcelRow<T>
	{
		T? row = GetRow<T>(rowID);
		item = row.GetValueOrDefault();
		return row.HasValue;
	}

	public static bool TryGetSubRow<T>(uint rowID, out T item, int index = 0) where T : struct, IExcelSubrow<T>
	{
		T? subRow = GetSubRow<T>(rowID, index);
		item = subRow.GetValueOrDefault();
		return subRow.HasValue;
	}

	public static bool TryGetSubRowAll<T>(uint rowID, out SubrowCollection<T> item) where T : struct, IExcelSubrow<T>
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		SubrowCollection<T>? subRowAll = GetSubRowAll<T>(rowID);
		item = subRowAll.GetValueOrDefault();
		return subRowAll.HasValue;
	}
}
