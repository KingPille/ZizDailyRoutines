using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Bindings.ImGui;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.ImGuiOm.Widgets.Combos;

public class WorldSelectCombo : LuminaComboBase<World>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public unsafe WorldSelectCombo(string id, IEnumerable<World> worlds = null)
		: base(id, (LuminaSearcher<World>)null)
	{
		IEnumerable<World> data = worlds ?? Sheets.Worlds.Values;
		base.Searcher = new LuminaSearcher<World>(data, new Func<World, string>[3]
		{
			(World x) => ((World)(ref x)).RowId.ToString(),
			(World x) => ((object)((World)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
			delegate(World x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				WorldDCGroupType value = ((World)(ref x)).DataCenter.Value;
				return ((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			}
		}, (IQueryable<World> x) => x.OrderBy((Expression<Func<World, byte>>)((World w) => w.UserType)).ThenBy((Expression<Func<World, uint>>)((World w) => ((RowRef<WorldDCGroupType>*)w.DataCenter)->RowId)), 200);
	}

	protected override string GetPreviewText(ComboSelectionMode mode)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		World val;
		WorldDCGroupType value;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			if (((World)(ref val)).RowId != 0)
			{
				val = base.SelectedItem;
				value = ((World)(ref val)).DataCenter.Value;
				string? text = ((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
				val = base.SelectedItem;
				return "[" + text + "] " + ((object)((World)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
			}
			return string.Empty;
		}
		if (base.SelectedItems.Count != 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[");
			val = base.SelectedItems.First();
			value = ((World)(ref val)).DataCenter.Value;
			defaultInterpolatedStringHandler.AppendFormatted(((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString());
			defaultInterpolatedStringHandler.AppendLiteral("] ");
			val = base.SelectedItems.First();
			defaultInterpolatedStringHandler.AppendFormatted(((object)((World)(ref val)).Name/*cast due to constrained. prefix*/).ToString());
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
		ImGui.TableSetupColumn(ImU8String.op_Implicit("World"), (ImGuiTableColumnFlags)4, 50f, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("DataCenter"), (ImGuiTableColumnFlags)4, 40f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(16222u)));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetLobbyText(802u)));
	}

	protected override bool DrawDataColumns(World world, ComboSelectionMode mode, bool isSelected)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)((World)(ref world)).Name/*cast due to constrained. prefix*/).ToString();
		ImGui.TableNextColumn();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(8, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		((ImU8String)(ref val)).AppendLiteral("##World_");
		((ImU8String)(ref val)).AppendFormatted<uint>(((World)(ref world)).RowId);
		bool result = ImGui.Selectable(val, isSelected, LuminaComboBase<World>.GetSelectableFlags(mode), default(Vector2));
		ImGuiOm.TooltipHover(text);
		ImGui.TableNextColumn();
		WorldDCGroupType value = ((World)(ref world)).DataCenter.Value;
		ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((WorldDCGroupType)(ref value)).Name/*cast due to constrained. prefix*/).ToString()));
		return result;
	}
}
