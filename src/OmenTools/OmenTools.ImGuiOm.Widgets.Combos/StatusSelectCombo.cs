using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.ImGuiOm.Widgets.Combos;

public class StatusSelectCombo : LuminaComboBase<Status>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public StatusSelectCombo(string id, IEnumerable<Status> statuses = null)
		: base(id, (LuminaSearcher<Status>)null)
	{
		IEnumerable<Status> data = statuses ?? Sheets.Statuses.Values;
		base.Searcher = new LuminaSearcher<Status>(data, new Func<Status, string>[3]
		{
			(Status x) => ((Status)(ref x)).RowId.ToString(),
			(Status x) => ((object)((Status)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
			(Status x) => ((object)((Status)(ref x)).Description/*cast due to constrained. prefix*/).ToString() ?? string.Empty
		}, null, 200);
	}

	protected override string GetPreviewText(ComboSelectionMode mode)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Status val;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			string text;
			if (((Status)(ref val)).RowId != 0)
			{
				val = base.SelectedItem;
				text = ((object)((Status)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
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
			val = base.SelectedItems.First();
			defaultInterpolatedStringHandler.AppendFormatted(((object)((Status)(ref val)).Name/*cast due to constrained. prefix*/).ToString());
			defaultInterpolatedStringHandler.AppendLiteral("...");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return string.Empty;
	}

	protected override int GetTableColumnCount()
	{
		return 3;
	}

	protected override bool CanDrawItem(Status item)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup(((Status)(ref item)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		return obj.TryGetFromGameIcon(ref val, ref val2);
	}

	protected override void SetupColumns(ComboSelectionMode mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Job"), (ImGuiTableColumnFlags)4, 40f, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Status"), (ImGuiTableColumnFlags)4, 50f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(294u)));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1340u)));
	}

	protected override bool DrawDataColumns(Status status, ComboSelectionMode mode, bool isSelected)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup(((Status)(ref status)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		obj.TryGetFromGameIcon(ref val, ref val2);
		ImGui.TableNextColumn();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(10, 3);
		ClassJobCategory? valueNullable = ((Status)(ref status)).ClassJobCategory.ValueNullable;
		object obj2;
		if (!valueNullable.HasValue)
		{
			obj2 = null;
		}
		else
		{
			ClassJobCategory valueOrDefault = valueNullable.GetValueOrDefault();
			obj2 = ((object)((ClassJobCategory)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		}
		if (obj2 == null)
		{
			obj2 = string.Empty;
		}
		((ImU8String)(ref val3)).AppendFormatted<string>((string)obj2);
		((ImU8String)(ref val3)).AppendLiteral("##Status_");
		((ImU8String)(ref val3)).AppendFormatted<uint>(((Status)(ref status)).RowId);
		((ImU8String)(ref val3)).AppendLiteral("_");
		((ImU8String)(ref val3)).AppendFormatted<string>(((object)((Status)(ref status)).Name/*cast due to constrained. prefix*/).ToString());
		bool result = ImGui.Selectable(val3, mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<Status>.GetSelectableFlags(mode), default(Vector2));
		ImGuiOm.TooltipHover(((object)((Status)(ref status)).Description/*cast due to constrained. prefix*/).ToString());
		ImGui.TableNextColumn();
		ImGuiOm.TextImage(((object)((Status)(ref status)).Name/*cast due to constrained. prefix*/).ToString(), val2.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
		return result;
	}
}
