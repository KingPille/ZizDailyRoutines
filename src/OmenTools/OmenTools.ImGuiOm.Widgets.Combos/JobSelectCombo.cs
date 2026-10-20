using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures.TextureWraps;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.ImGuiOm.Widgets.Combos;

public class JobSelectCombo : LuminaComboBase<ClassJob>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public unsafe JobSelectCombo(string id, IEnumerable<ClassJob> jobs = null)
		: base(id, (LuminaSearcher<ClassJob>)null)
	{
		IEnumerable<ClassJob> data = jobs ?? ((IEnumerable<ClassJob>)LuminaGetter.Get<ClassJob>()).Where((ClassJob x) => !string.IsNullOrEmpty(((object)((ClassJob)(ref x)).Name/*cast due to constrained. prefix*/).ToString()));
		base.Searcher = new LuminaSearcher<ClassJob>(data, new Func<ClassJob, string>[4]
		{
			(ClassJob x) => ((ClassJob)(ref x)).RowId.ToString(),
			(ClassJob x) => ((object)((ClassJob)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
			(ClassJob x) => ((object)((ClassJob)(ref x)).Abbreviation/*cast due to constrained. prefix*/).ToString(),
			delegate(ClassJob x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				ClassJobCategory? valueNullable = ((ClassJob)(ref x)).ClassJobCategory.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					ClassJobCategory valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((ClassJobCategory)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				if (obj == null)
				{
					obj = string.Empty;
				}
				return (string)obj;
			}
		}, (IQueryable<ClassJob> x) => x.OrderBy((Expression<Func<ClassJob, byte>>)((ClassJob d) => d.Role)).ThenBy((Expression<Func<ClassJob, sbyte>>)((ClassJob d) => d.DohDolJobIndex)).ThenBy((Expression<Func<ClassJob, uint>>)((ClassJob d) => ((RowRef<ClassJobCategory>*)d.ClassJobCategory)->RowId)), 200);
	}

	protected override string GetPreviewText(ComboSelectionMode mode)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		ClassJob val;
		ClassJobCategory valueOrDefault;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			if (((ClassJob)(ref val)).RowId != 0)
			{
				val = base.SelectedItem;
				ClassJobCategory? valueNullable = ((ClassJob)(ref val)).ClassJobCategory.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((ClassJobCategory)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
				}
				val = base.SelectedItem;
				return "[" + (string?)obj + "] " + ((object)((ClassJob)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
			}
			return string.Empty;
		}
		if (base.SelectedItems.Count != 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[");
			val = base.SelectedItems.First();
			ClassJobCategory? valueNullable = ((ClassJob)(ref val)).ClassJobCategory.ValueNullable;
			object value;
			if (!valueNullable.HasValue)
			{
				value = null;
			}
			else
			{
				valueOrDefault = valueNullable.GetValueOrDefault();
				value = ((object)((ClassJobCategory)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			defaultInterpolatedStringHandler.AppendFormatted((string?)value);
			defaultInterpolatedStringHandler.AppendLiteral("] ");
			val = base.SelectedItems.First();
			defaultInterpolatedStringHandler.AppendFormatted(((object)((ClassJob)(ref val)).Name/*cast due to constrained. prefix*/).ToString());
			defaultInterpolatedStringHandler.AppendLiteral("...");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return string.Empty;
	}

	protected override int GetTableColumnCount()
	{
		return 4;
	}

	protected override void SetupColumns(ComboSelectionMode mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Icon"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Job"), (ImGuiTableColumnFlags)4, 70f, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Category"), (ImGuiTableColumnFlags)4, 30f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(294u)));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(7542u)));
	}

	protected override bool DrawDataColumns(ClassJob job, ComboSelectionMode mode, bool isSelected)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		IDalamudTextureWrap? gameIcon = ImageHelper.GetGameIcon(62100 + ((((ClassJob)(ref job)).RowId == 0) ? 44 : ((ClassJob)(ref job)).RowId));
		ImGui.TableNextColumn();
		ImGui.Image(gameIcon.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
		ImGui.TableNextColumn();
		bool result = ImGui.Selectable(ImU8String.op_Implicit(((object)((ClassJob)(ref job)).Name/*cast due to constrained. prefix*/).ToString()), mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<ClassJob>.GetSelectableFlags(mode), default(Vector2));
		ImGui.TableNextColumn();
		ClassJobCategory? valueNullable = ((ClassJob)(ref job)).ClassJobCategory.ValueNullable;
		object obj;
		if (!valueNullable.HasValue)
		{
			obj = null;
		}
		else
		{
			ClassJobCategory valueOrDefault = valueNullable.GetValueOrDefault();
			obj = ((object)((ClassJobCategory)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		}
		if (obj == null)
		{
			obj = string.Empty;
		}
		ImGui.TextUnformatted(ImU8String.op_Implicit((string)obj));
		return result;
	}
}
