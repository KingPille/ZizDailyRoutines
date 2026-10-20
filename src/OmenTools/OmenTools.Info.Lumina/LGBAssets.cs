using System;
using System.Collections.Generic;
using System.Linq;
using Lumina.Data.Parsing.Layer;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.DataShare.Attributes;
using OmenTools.Extensions;
using OmenTools.Info.Lumina.Enums;
using OmenTools.Info.Lumina.LGB;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Info.Lumina;

public static class LGBAssets
{
	[DataShareTag]
	private const string ZONE_LINE_EXIT_RANGES_TAG = "OmenTools.Info.Game.Data.LGBAssets.ZoneLineExitRanges";

	public static Dictionary<uint, List<ZoneLineExitRange>> ZoneLineExitRanges { get; } = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, List<ZoneLineExitRange>>>("OmenTools.Info.Game.Data.LGBAssets.ZoneLineExitRanges", (Func<Dictionary<uint, List<ZoneLineExitRange>>>)delegate
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<uint, List<ZoneLineExitRange>> dictionary = new Dictionary<uint, List<ZoneLineExitRange>>();
		Enumerator<TerritoryType> enumerator = LuminaGetter.Get<TerritoryType>().GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TerritoryType zone = enumerator.Current;
				List<InstanceObject> list = zone.GetLGB(LGBFileType.PlanMap)?.GetInstanceObjects((LayerEntryType)41);
				if (list != null && list.Count > 0)
				{
					List<ZoneLineExitRange> list2 = new List<ZoneLineExitRange>();
					foreach (InstanceObject item in list)
					{
						ExitRangeInstanceObject val = (ExitRangeInstanceObject)(object)item.Object;
						if (val.ReturnInstanceId != 0)
						{
							list2.Add(new ZoneLineExitRange(((TerritoryType)(ref zone)).RowId, val.TerritoryType, item.GetPosition()));
						}
					}
					dictionary[((TerritoryType)(ref zone)).RowId] = list2;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		return dictionary.ToDictionary();
	});
}
