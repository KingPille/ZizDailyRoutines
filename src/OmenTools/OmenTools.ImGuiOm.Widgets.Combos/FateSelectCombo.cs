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
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.ImGuiOm.Widgets.Combos;

public class FateSelectCombo : LuminaComboBase<Fate>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public FateSelectCombo(string id, IEnumerable<Fate> fates = null)
		: base(id, (LuminaSearcher<Fate>)null)
	{
		IEnumerable<Fate> data = fates ?? (from x in (IEnumerable<Fate>)LuminaGetter.Get<Fate>()
			where !string.IsNullOrEmpty(((object)((Fate)(ref x)).Name/*cast due to constrained. prefix*/).ToString())
			orderby ((Fate)(ref x)).ClassJobLevel
			select x);
		base.Searcher = new LuminaSearcher<Fate>(data, new Func<Fate, string>[4]
		{
			(Fate x) => ((Fate)(ref x)).RowId.ToString(),
			(Fate x) => ((object)((Fate)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
			(Fate x) => ((object)((Fate)(ref x)).Description/*cast due to constrained. prefix*/).ToString(),
			(Fate x) => ((object)((Fate)(ref x)).Objective/*cast due to constrained. prefix*/).ToString()
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
		Fate val;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			string text;
			if (((Fate)(ref val)).RowId != 0)
			{
				val = base.SelectedItem;
				text = ((object)((Fate)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
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
			defaultInterpolatedStringHandler.AppendFormatted(((object)((Fate)(ref val)).Name/*cast due to constrained. prefix*/).ToString());
			defaultInterpolatedStringHandler.AppendLiteral("...");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return string.Empty;
	}

	protected override int GetTableColumnCount()
	{
		return 3;
	}

	protected override bool CanDrawItem(Fate item)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup(((Fate)(ref item)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		return obj.TryGetFromGameIcon(ref val, ref val2);
	}

	protected override void SetupColumns(ComboSelectionMode mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Level"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("1234"), false, -1f).X, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Fate"), (ImGuiTableColumnFlags)4, 40f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(335u)));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1340u)));
	}

	protected override bool DrawDataColumns(Fate fate, ComboSelectionMode mode, bool isSelected)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup(((Fate)(ref fate)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		obj.TryGetFromGameIcon(ref val, ref val2);
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(((Fate)(ref fate)).ClassJobLevel.ToString()));
		ImGui.TableNextColumn();
		bool result = ImGuiOm.SelectableImageWithText(val2.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()), ((object)((Fate)(ref fate)).Name/*cast due to constrained. prefix*/).ToString(), mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<Fate>.GetSelectableFlags(mode));
		ImGuiOm.TooltipHover(((object)((Fate)(ref fate)).Description/*cast due to constrained. prefix*/).ToString());
		return result;
	}
}
