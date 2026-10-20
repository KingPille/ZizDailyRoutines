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

public class CompanionCombo : LuminaComboBase<Companion>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public CompanionCombo(string id, IEnumerable<Companion> companions = null)
		: base(id, (LuminaSearcher<Companion>)null)
	{
		IEnumerable<Companion> data = companions ?? ((IEnumerable<Companion>)LuminaGetter.Get<Companion>()).Where((Companion x) => !string.IsNullOrEmpty(((object)((Companion)(ref x)).Singular/*cast due to constrained. prefix*/).ToString()));
		base.Searcher = new LuminaSearcher<Companion>(data, new Func<Companion, string>[3]
		{
			(Companion x) => ((Companion)(ref x)).RowId.ToString(),
			(Companion x) => ((object)((Companion)(ref x)).Singular/*cast due to constrained. prefix*/).ToString(),
			GetDescription
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
		Companion val;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			string text;
			if (((Companion)(ref val)).RowId != 0)
			{
				val = base.SelectedItem;
				text = ((object)((Companion)(ref val)).Singular/*cast due to constrained. prefix*/).ToString();
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
			defaultInterpolatedStringHandler.AppendFormatted(((object)((Companion)(ref val)).Singular/*cast due to constrained. prefix*/).ToString());
			defaultInterpolatedStringHandler.AppendLiteral("...");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return string.Empty;
	}

	protected override int GetTableColumnCount()
	{
		return 2;
	}

	protected override bool CanDrawItem(Companion item)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup((uint)((Companion)(ref item)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		return obj.TryGetFromGameIcon(ref val, ref val2);
	}

	protected override void SetupColumns(ComboSelectionMode mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Companion"), (ImGuiTableColumnFlags)4, 50f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(6382u)));
	}

	protected override bool DrawDataColumns(Companion companion, ComboSelectionMode mode, bool isSelected)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)((Companion)(ref companion)).Singular/*cast due to constrained. prefix*/).ToString();
		string description = GetDescription(companion);
		string text2 = text ?? "";
		ImGui.TableNextColumn();
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup((uint)((Companion)(ref companion)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		bool result;
		if (!obj.TryGetFromGameIcon(ref val, ref val2))
		{
			ImU8String val3 = default(ImU8String);
			((ImU8String)(ref val3))._002Ector(13, 3);
			((ImU8String)(ref val3)).AppendFormatted<string>(text2);
			((ImU8String)(ref val3)).AppendLiteral("##Companion_");
			((ImU8String)(ref val3)).AppendFormatted<uint>(((Companion)(ref companion)).RowId);
			((ImU8String)(ref val3)).AppendLiteral("_");
			((ImU8String)(ref val3)).AppendFormatted<string>(text);
			result = ImGui.Selectable(val3, mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<Companion>.GetSelectableFlags(mode), default(Vector2));
		}
		else
		{
			result = ImGuiOm.SelectableImageWithText(val2.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), text2, mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<Companion>.GetSelectableFlags(mode));
		}
		if (!string.IsNullOrWhiteSpace(description))
		{
			ImGuiOm.TooltipHover(description);
		}
		return result;
	}

	private static string GetDescription(Companion companion)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<CompanionTransient>(((Companion)(ref companion)).RowId, out CompanionTransient item))
		{
			return string.Empty;
		}
		return ((object)((CompanionTransient)(ref item)).Description/*cast due to constrained. prefix*/).ToString() ?? string.Empty;
	}
}
