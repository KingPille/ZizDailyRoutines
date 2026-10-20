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

public class ItemSelectCombo : LuminaComboBase<Item>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public ItemSelectCombo(string id, IEnumerable<Item> items = null)
		: base(id, (LuminaSearcher<Item>)null)
	{
		IEnumerable<Item> data = items ?? ((IEnumerable<Item>)LuminaGetter.Get<Item>()).Where((Item x) => !string.IsNullOrEmpty(((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString()));
		base.Searcher = new LuminaSearcher<Item>(data, new Func<Item, string>[6]
		{
			(Item x) => ((Item)(ref x)).RowId.ToString(),
			(Item x) => ((object)((Item)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
			(Item x) => ((object)((Item)(ref x)).Description/*cast due to constrained. prefix*/).ToString(),
			(Item x) => ((Item)(ref x)).LevelEquip.ToString(),
			(Item x) => ((Item)(ref x)).LevelItem.RowId.ToString(),
			delegate(Item x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				ItemUICategory? valueNullable = ((Item)(ref x)).ItemUICategory.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					ItemUICategory valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((ItemUICategory)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
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
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Item val;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			if (((Item)(ref val)).RowId != 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[");
				val = base.SelectedItem;
				defaultInterpolatedStringHandler.AppendFormatted(((Item)(ref val)).LevelItem.RowId);
				defaultInterpolatedStringHandler.AppendLiteral("] ");
				val = base.SelectedItem;
				defaultInterpolatedStringHandler.AppendFormatted(((object)((Item)(ref val)).Name/*cast due to constrained. prefix*/).ToString());
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			return string.Empty;
		}
		if (base.SelectedItems.Count != 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(6, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[");
			val = base.SelectedItems.First();
			defaultInterpolatedStringHandler2.AppendFormatted(((Item)(ref val)).LevelItem.RowId);
			defaultInterpolatedStringHandler2.AppendLiteral("] ");
			val = base.SelectedItems.First();
			defaultInterpolatedStringHandler2.AppendFormatted(((object)((Item)(ref val)).Name/*cast due to constrained. prefix*/).ToString());
			defaultInterpolatedStringHandler2.AppendLiteral("...");
			return defaultInterpolatedStringHandler2.ToStringAndClear();
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
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImU8String val = ImU8String.op_Implicit("ItemLevel");
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(2, 1);
		((ImU8String)(ref val2)).AppendLiteral("14");
		((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetAddonText(7873u));
		ImGui.TableSetupColumn(val, (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(val2, false, -1f).X, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Item"), (ImGuiTableColumnFlags)4, 80f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(7873u)));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(520u)));
	}

	protected override bool DrawDataColumns(Item item, ComboSelectionMode mode, bool isSelected)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
		string text2 = ((object)((Item)(ref item)).Description/*cast due to constrained. prefix*/).ToString();
		string text3 = text ?? "";
		string text4 = $"##Item_{((Item)(ref item)).RowId}_{text}";
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(((Item)(ref item)).LevelItem.RowId.ToString()));
		ImGui.TableNextColumn();
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup((uint)((Item)(ref item)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		bool result;
		if (!obj.TryGetFromGameIcon(ref val, ref val2))
		{
			ImU8String val3 = default(ImU8String);
			((ImU8String)(ref val3))._002Ector(0, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>(text3);
			((ImU8String)(ref val3)).AppendFormatted<string>(text4);
			result = ImGui.Selectable(val3, isSelected, LuminaComboBase<Item>.GetSelectableFlags(mode), default(Vector2));
		}
		else
		{
			result = ImGuiOm.SelectableImageWithText(val2.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), text3, isSelected, LuminaComboBase<Item>.GetSelectableFlags(mode));
		}
		if (!string.IsNullOrWhiteSpace(text2))
		{
			ImGuiOm.TooltipHover(text2);
		}
		return result;
	}
}
