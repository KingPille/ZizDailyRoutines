using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.ImGuiOm.Widgets.Combos;

public class ContentSelectCombo : LuminaComboBase<ContentFinderCondition>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public ContentSelectCombo(string id, IEnumerable<ContentFinderCondition> contents = null)
		: base(id, (LuminaSearcher<ContentFinderCondition>)null)
	{
		IEnumerable<ContentFinderCondition> data = contents ?? Sheets.Contents.Values;
		base.Searcher = new LuminaSearcher<ContentFinderCondition>(data, new Func<ContentFinderCondition, string>[3]
		{
			(ContentFinderCondition x) => ((ContentFinderCondition)(ref x)).RowId.ToString(),
			(ContentFinderCondition x) => ((object)((ContentFinderCondition)(ref x)).Name/*cast due to constrained. prefix*/).ToString(),
			delegate(ContentFinderCondition x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0023: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0048: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				TerritoryType? valueNullable = ((ContentFinderCondition)(ref x)).TerritoryType.ValueNullable;
				object obj;
				if (!valueNullable.HasValue)
				{
					obj = null;
				}
				else
				{
					TerritoryType valueOrDefault = valueNullable.GetValueOrDefault();
					PlaceName? valueNullable2 = ((TerritoryType)(ref valueOrDefault)).PlaceName.ValueNullable;
					if (!valueNullable2.HasValue)
					{
						obj = null;
					}
					else
					{
						PlaceName valueOrDefault2 = valueNullable2.GetValueOrDefault();
						obj = ((object)((PlaceName)(ref valueOrDefault2)).Name/*cast due to constrained. prefix*/).ToString();
					}
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
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		ContentFinderCondition val;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			string text;
			if (((ContentFinderCondition)(ref val)).RowId != 0)
			{
				val = base.SelectedItem;
				text = ((object)((ContentFinderCondition)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
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
			defaultInterpolatedStringHandler.AppendFormatted(((object)((ContentFinderCondition)(ref val)).Name/*cast due to constrained. prefix*/).ToString());
			defaultInterpolatedStringHandler.AppendLiteral("...");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return string.Empty;
	}

	protected override int GetTableColumnCount()
	{
		return 5;
	}

	protected override void SetupColumns(ComboSelectionMode mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, (mode == ComboSelectionMode.Radio) ? (20f * Globals.GlobalUIScale) : ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Icon"), (ImGuiTableColumnFlags)8, 20f * Globals.GlobalUIScale, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Level"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(335u)), false, -1f).X, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("DutyName"), (ImGuiTableColumnFlags)4, 40f, 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("PlaceName"), (ImGuiTableColumnFlags)4, 40f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(335u)));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(14098u)));
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(870u)));
	}

	protected override bool DrawDataColumns(ContentFinderCondition content, ComboSelectionMode mode, bool isSelected)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		string text = ((object)((ContentFinderCondition)(ref content)).Name/*cast due to constrained. prefix*/).ToString();
		TerritoryType? valueNullable = ((ContentFinderCondition)(ref content)).TerritoryType.ValueNullable;
		object obj;
		if (!valueNullable.HasValue)
		{
			obj = null;
		}
		else
		{
			TerritoryType valueOrDefault = valueNullable.GetValueOrDefault();
			PlaceName? valueNullable2 = ((TerritoryType)(ref valueOrDefault)).PlaceName.ValueNullable;
			if (!valueNullable2.HasValue)
			{
				obj = null;
			}
			else
			{
				PlaceName valueOrDefault2 = valueNullable2.GetValueOrDefault();
				obj = ((object)((PlaceName)(ref valueOrDefault2)).Name/*cast due to constrained. prefix*/).ToString();
			}
		}
		if (obj == null)
		{
			obj = string.Empty;
		}
		string text2 = (string)obj;
		ImGui.TableNextColumn();
		ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
		ContentType? valueNullable3 = ((ContentFinderCondition)(ref content)).ContentType.ValueNullable;
		int num;
		if (!valueNullable3.HasValue)
		{
			num = 0;
		}
		else
		{
			ContentType valueOrDefault3 = valueNullable3.GetValueOrDefault();
			num = (int)((ContentType)(ref valueOrDefault3)).Icon;
		}
		GameIconLookup val = new GameIconLookup((uint)num, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		if (obj2.TryGetFromGameIcon(ref val, ref val2))
		{
			ImGui.Image(val2.GetWrapOrEmpty().Handle, Globals.ScaledVector2(20f));
		}
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(((ContentFinderCondition)(ref content)).ClassJobLevelRequired.ToString()));
		ImGui.TableNextColumn();
		ImU8String val3 = default(ImU8String);
		((ImU8String)(ref val3))._002Ector(10, 2);
		((ImU8String)(ref val3)).AppendFormatted<string>(text);
		((ImU8String)(ref val3)).AppendLiteral("##Content_");
		((ImU8String)(ref val3)).AppendFormatted<uint>(((ContentFinderCondition)(ref content)).RowId);
		bool result = ImGui.Selectable(val3, mode == ComboSelectionMode.Checkbox && isSelected, LuminaComboBase<ContentFinderCondition>.GetSelectableFlags(mode), default(Vector2));
		ITextureProvider obj3 = DalamudServiceExtension.Instance<ITextureProvider>();
		val = new GameIconLookup(((ContentFinderCondition)(ref content)).Image, false, true, (ClientLanguage?)null);
		ISharedImmediateTexture val4 = default(ISharedImmediateTexture);
		if (obj3.TryGetFromGameIcon(ref val, ref val4) && ImGui.IsItemHovered())
		{
			TooltipDisposable val5 = ImRaii.Tooltip();
			try
			{
				ImGui.Image(val4.GetWrapOrEmpty().Handle, val4.GetWrapOrEmpty().Size / 2f);
			}
			finally
			{
				((TooltipDisposable)(ref val5)).Dispose();
			}
		}
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(text2));
		return result;
	}
}
