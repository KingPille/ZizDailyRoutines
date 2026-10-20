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

public class MountSelectCombo : LuminaComboBase<Mount>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public MountSelectCombo(string id, IEnumerable<Mount> mounts = null)
		: base(id, (LuminaSearcher<Mount>)null)
	{
		IEnumerable<Mount> data = mounts ?? ((IEnumerable<Mount>)LuminaGetter.Get<Mount>()).Where((Mount x) => !string.IsNullOrEmpty(((object)((Mount)(ref x)).Singular/*cast due to constrained. prefix*/).ToString()));
		base.Searcher = new LuminaSearcher<Mount>(data, new Func<Mount, string>[3]
		{
			(Mount x) => ((Mount)(ref x)).RowId.ToString(),
			(Mount x) => ((object)((Mount)(ref x)).Singular/*cast due to constrained. prefix*/).ToString(),
			(Mount x) => (((Mount)(ref x)).ExtraSeats + 1).ToString()
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
		Mount val;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			string text;
			if (((Mount)(ref val)).RowId != 0)
			{
				val = base.SelectedItem;
				text = ((object)((Mount)(ref val)).Singular/*cast due to constrained. prefix*/).ToString();
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
			defaultInterpolatedStringHandler.AppendFormatted(((object)((Mount)(ref val)).Singular/*cast due to constrained. prefix*/).ToString());
			defaultInterpolatedStringHandler.AppendLiteral("...");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return string.Empty;
	}

	protected override int GetTableColumnCount()
	{
		return 2;
	}

	protected override void SetupColumns(ComboSelectionMode mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Mount"), (ImGuiTableColumnFlags)4, 0f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(6382u)));
	}

	protected override bool DrawDataColumns(Mount mount, ComboSelectionMode mode, bool isSelected)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup((uint)((Mount)(ref mount)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		if (!obj.TryGetFromGameIcon(ref val, ref val2))
		{
			ImU8String val3 = default(ImU8String);
			((ImU8String)(ref val3))._002Ector(8, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>(((object)((Mount)(ref mount)).Singular/*cast due to constrained. prefix*/).ToString());
			((ImU8String)(ref val3)).AppendLiteral("##Mount_");
			((ImU8String)(ref val3)).AppendFormatted<uint>(((Mount)(ref mount)).RowId);
			return ImGui.Selectable(val3, mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<Mount>.GetSelectableFlags(mode), default(Vector2));
		}
		return ImGuiOm.SelectableImageWithText(val2.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()), ((object)((Mount)(ref mount)).Singular/*cast due to constrained. prefix*/).ToString(), mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<Mount>.GetSelectableFlags(mode));
	}
}
