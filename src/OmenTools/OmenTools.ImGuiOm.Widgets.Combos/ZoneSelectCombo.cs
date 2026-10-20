using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.ImGuiOm.Widgets.Combos;

public class ZoneSelectCombo : LuminaComboBase<TerritoryType>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public ZoneSelectCombo(string id, IEnumerable<TerritoryType> zones = null)
		: base(id, (LuminaSearcher<TerritoryType>)null)
	{
		IEnumerable<TerritoryType> data = zones ?? ((IEnumerable<TerritoryType>)LuminaGetter.Get<TerritoryType>()).Where((TerritoryType x) => !string.IsNullOrEmpty(((object)((TerritoryType)(ref x)).Name/*cast due to constrained. prefix*/).ToString()));
		base.Searcher = new LuminaSearcher<TerritoryType>(data, new Func<TerritoryType, string>[5]
		{
			(TerritoryType x) => ((TerritoryType)(ref x)).RowId.ToString(),
			(TerritoryType x) => ((object)((TerritoryType)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
			delegate(TerritoryType x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				PlaceName? valueNullable = ((TerritoryType)(ref x)).PlaceName.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((PlaceName)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj == null)
				{
					obj = string.Empty;
				}
				return (string)obj;
			},
			delegate(TerritoryType x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				PlaceName? valueNullable = ((TerritoryType)(ref x)).PlaceNameZone.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((PlaceName)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj == null)
				{
					obj = string.Empty;
				}
				return (string)obj;
			},
			delegate(TerritoryType x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				PlaceName? valueNullable = ((TerritoryType)(ref x)).PlaceNameRegion.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((PlaceName)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj == null)
				{
					obj = string.Empty;
				}
				return (string)obj;
			}
		}, null, 200);
	}

	protected override string GetPreviewText(ComboSelectionMode mode)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		TerritoryType row;
		if (mode == ComboSelectionMode.Radio)
		{
			row = base.SelectedItem;
			string text;
			if (((TerritoryType)(ref row)).RowId != 0)
			{
				row = base.SelectedItem;
				text = row.ExtractPlaceName();
				if (text == null)
				{
					return "";
				}
			}
			else
			{
				text = string.Empty;
			}
			return text;
		}
		if (base.SelectedItems.Count != 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[");
			defaultInterpolatedStringHandler.AppendFormatted(base.SelectedItems.Count);
			defaultInterpolatedStringHandler.AppendLiteral("] ");
			row = base.SelectedItems.First();
			defaultInterpolatedStringHandler.AppendFormatted(row.ExtractPlaceName());
			defaultInterpolatedStringHandler.AppendLiteral("...");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return string.Empty;
	}

	protected override int GetTableColumnCount()
	{
		return 3;
	}

	protected override void SetupColumns(ComboSelectionMode mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Region"), (ImGuiTableColumnFlags)4, 40f, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Zone"), (ImGuiTableColumnFlags)4, 50f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(870u)));
	}

	protected override bool DrawDataColumns(TerritoryType zone, ComboSelectionMode mode, bool isSelected)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		string text = zone.ExtractPlaceName();
		PlaceName? valueNullable = ((TerritoryType)(ref zone)).PlaceNameRegion.ValueNullable;
		object obj;
		if (!valueNullable.HasValue)
		{
			obj = null;
		}
		else
		{
			PlaceName valueOrDefault = valueNullable.GetValueOrDefault();
			obj = ((object)((PlaceName)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		}
		if (obj == null)
		{
			obj = string.Empty;
		}
		if (mode == ComboSelectionMode.Radio && ((TerritoryType)(ref zone)).ContentFinderCondition.RowId != 0)
		{
			ContentFinderCondition value = ((TerritoryType)(ref zone)).ContentFinderCondition.Value;
			string text2 = ((object)((ContentFinderCondition)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + " (" + text2 + ")";
			}
		}
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit((string)obj));
		ImGui.TableNextColumn();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(7, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("##Zone_");
		((ImU8String)(ref val)).AppendFormatted<uint>(((TerritoryType)(ref zone)).RowId);
		bool result = ImGui.Selectable(val, mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<TerritoryType>.GetSelectableFlags(mode), default(Vector2));
		if (mode == ComboSelectionMode.Radio)
		{
			ImGuiOm.TooltipHover(text);
		}
		return result;
	}
}
