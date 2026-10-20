using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.ImGuiOm.Widgets.Combos;

public class ActionSelectCombo : LuminaComboBase<Action>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public ActionSelectCombo(string id, IEnumerable<Action> actions = null)
		: base(id, (LuminaSearcher<Action>)null)
	{
		IEnumerable<Action> data = actions ?? Sheets.PlayerActions.Values;
		base.Searcher = new LuminaSearcher<Action>(data, new Func<Action, string>[3]
		{
			(Action x) => ((Action)(ref x)).RowId.ToString(),
			(Action x) => ((object)((Action)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
			delegate(Action x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				ClassJob? valueNullable = ((Action)(ref x)).ClassJob.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					ClassJob valueOrDefault = valueNullable.GetValueOrDefault();
					obj = ((object)((ClassJob)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Action val;
		ClassJobCategory valueOrDefault;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			if (((Action)(ref val)).RowId != 0)
			{
				val = base.SelectedItem;
				ClassJobCategory? valueNullable = ((Action)(ref val)).ClassJobCategory.ValueNullable;
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
				return "[" + (string?)obj + "] " + ((object)((Action)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
			}
			return string.Empty;
		}
		if (base.SelectedItems.Count != 0)
		{
			val = base.SelectedItems.First();
			ClassJobCategory? valueNullable = ((Action)(ref val)).ClassJobCategory.ValueNullable;
			object obj2;
			if (!valueNullable.HasValue)
			{
				obj2 = null;
			}
			else
			{
				valueOrDefault = valueNullable.GetValueOrDefault();
				obj2 = ((object)((ClassJobCategory)(ref valueOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
			}
			val = base.SelectedItems.First();
			return "[" + (string?)obj2 + "] " + ((object)((Action)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
		}
		return string.Empty;
	}

	protected override int GetTableColumnCount()
	{
		return 4;
	}

	protected override bool CanDrawItem(Action item)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup((uint)((Action)(ref item)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		return obj.TryGetFromGameIcon(ref val, ref val2);
	}

	protected override void SetupColumns(ComboSelectionMode mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Job"), (ImGuiTableColumnFlags)4, 20f, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Level"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("1234"), false, -1f).X, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Action"), (ImGuiTableColumnFlags)4, 50f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(294u)));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(335u)));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1340u)));
	}

	protected override bool DrawDataColumns(Action action, ComboSelectionMode mode, bool isSelected)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
		GameIconLookup val = new GameIconLookup((uint)((Action)(ref action)).Icon, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		obj.TryGetFromGameIcon(ref val, ref val2);
		string text = ((object)((Action)(ref action)).Name/*cast due to constrained. prefix*/).ToString();
		ClassJobCategory? valueNullable = ((Action)(ref action)).ClassJobCategory.ValueNullable;
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
		string text2 = (string)obj2;
		ImGui.TableNextColumn();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(10, 3);
		((ImU8String)(ref val3)).AppendFormatted<string>(text2);
		((ImU8String)(ref val3)).AppendLiteral("##Action_");
		((ImU8String)(ref val3)).AppendFormatted<uint>(((Action)(ref action)).RowId);
		((ImU8String)(ref val3)).AppendLiteral("_");
		((ImU8String)(ref val3)).AppendFormatted<string>(((object)((Action)(ref action)).Name/*cast due to constrained. prefix*/).ToString());
		bool result = ImGui.Selectable(val3, mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<Action>.GetSelectableFlags(mode), default(Vector2));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(((Action)(ref action)).ClassJobLevel.ToString()));
		ImGui.TableNextColumn();
		ImGuiOm.TextImage(text ?? "", val2.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
		return result;
	}
}
