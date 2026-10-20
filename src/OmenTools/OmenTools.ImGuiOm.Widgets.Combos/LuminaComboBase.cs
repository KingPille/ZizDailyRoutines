using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel;
using OmenTools.Global;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.ImGuiOm.Widgets.Combos;

public abstract class LuminaComboBase<T>(string id, LuminaSearcher<T> searcher) where T : struct, IExcelRow<T>
{
	protected enum ComboSelectionMode
	{
		Radio,
		Checkbox
	}

	private static readonly Vector2 PopupSize = Globals.ScaledVector2(600f, 400f);

	protected string SearchWord = string.Empty;

	private bool IsSelectedOnly;

	protected LuminaSearcher<T> Searcher { get; init; } = searcher;

	protected string ID { get; init; } = id;

	public abstract uint SelectedID { get; set; }

	public abstract HashSet<uint> SelectedIDs { get; set; }

	public T SelectedItem => LuminaGetter.GetRow<T>(SelectedID).GetValueOrDefault();

	public List<T> SelectedItems => (from x in SelectedIDs
		select LuminaGetter.GetRow<T>(x).GetValueOrDefault() into x
		where ((IExcelRow<T>)x).RowId != 0
		select x).ToList();

	public bool DrawRadio()
	{
		return DrawCore(ComboSelectionMode.Radio);
	}

	public bool DrawCheckbox()
	{
		return DrawCore(ComboSelectionMode.Checkbox);
	}

	protected abstract string GetPreviewText(ComboSelectionMode mode);

	protected abstract int GetTableColumnCount();

	protected virtual ImGuiTableFlags GetTableFlags()
	{
		return (ImGuiTableFlags)33556352;
	}

	protected virtual bool CanDrawItem(T item)
	{
		return true;
	}

	protected abstract void SetupColumns(ComboSelectionMode mode);

	protected abstract void DrawHeaders();

	protected abstract bool DrawDataColumns(T item, ComboSelectionMode mode, bool isSelected);

	protected static ImGuiSelectableFlags GetSelectableFlags(ComboSelectionMode mode)
	{
		if (mode == ComboSelectionMode.Radio)
		{
			return (ImGuiSelectableFlags)2;
		}
		return (ImGuiSelectableFlags)3;
	}

	private bool DrawCore(ComboSelectionMode mode)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(ID), true);
		try
		{
			if (ImGui.BeginCombo(ImU8String.op_Implicit("###Combo"), ImU8String.op_Implicit(GetPreviewText(mode)), (ImGuiComboFlags)8))
			{
				ImGui.EndCombo();
			}
			if (ImGui.IsItemClicked())
			{
				ImU8String val2 = new ImU8String(9, 1);
				((ImU8String)(ref val2)).AppendLiteral("###Popup_");
				((ImU8String)(ref val2)).AppendFormatted<string>(ID);
				ImGui.OpenPopup(val2, (ImGuiPopupFlags)0);
			}
			ImGui.SetNextWindowSize(PopupSize);
			ImU8String val3 = new ImU8String(9, 1);
			((ImU8String)(ref val3)).AppendLiteral("###Popup_");
			((ImU8String)(ref val3)).AppendFormatted<string>(ID);
			PopupDisposable val4 = ImRaii.Popup(val3);
			try
			{
				if (!val4)
				{
					return false;
				}
				ImGui.Checkbox(ImU8String.op_Implicit("##SelectedOnly_" + ID), ref IsSelectedOnly);
				ImGui.SameLine();
				ImGui.SetNextItemWidth(-1f);
				if (ImGui.InputTextWithHint(ImU8String.op_Implicit("###Search"), ImU8String.op_Implicit(LuminaWrapper.GetAddonText(8128u)), ref SearchWord, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
				{
					Searcher.Search(SearchWord);
				}
				ImGui.Separator();
				Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
				TableDisposable val5 = ImRaii.Table(ImU8String.op_Implicit("###Table"), GetTableColumnCount(), GetTableFlags(), contentRegionAvail);
				try
				{
					if (!val5)
					{
						return false;
					}
					SetupColumns(mode);
					ImGui.TableSetupScrollFreeze(0, 1);
					DrawHeaders();
					List<T> list = BuildVisibleItems(mode);
					if (list.Count == 0)
					{
						return false;
					}
					ImGuiListClipper val6 = default(ImGuiListClipper);
					bool flag = false;
					((ImGuiListClipper)(ref val6)).Begin(list.Count);
					while (((ImGuiListClipper)(ref val6)).Step())
					{
						for (int i = val6.DisplayStart; i < val6.DisplayEnd; i++)
						{
							T item = list[i];
							bool isSelected = IsSelected(item, mode);
							ImU8String val7 = new ImU8String(4, 1);
							((ImU8String)(ref val7)).AppendLiteral("Row_");
							((ImU8String)(ref val7)).AppendFormatted<uint>(((IExcelRow<T>)item).RowId);
							IdDisposable val8 = ImRaii.PushId(val7, true);
							try
							{
								ImGui.TableNextRow();
								ImGui.TableNextColumn();
								if (DrawSelector(mode, isSelected) || DrawDataColumns(item, mode, isSelected))
								{
									flag |= ApplySelection(((IExcelRow<T>)item).RowId, mode);
								}
							}
							finally
							{
								((IDisposable)val8)?.Dispose();
							}
						}
					}
					((ImGuiListClipper)(ref val6)).End();
					return flag;
				}
				finally
				{
					((TableDisposable)(ref val5)).Dispose();
				}
			}
			finally
			{
				((PopupDisposable)(ref val4)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private List<T> BuildVisibleItems(ComboSelectionMode mode)
	{
		List<T> result = new List<T>();
		HashSet<uint> seen = new HashSet<uint>();
		if (!IsSelectedOnly)
		{
			foreach (T item in Searcher.SearchResult)
			{
				AppendVisibleItem(item, seen, result);
			}
			return result;
		}
		if (mode == ComboSelectionMode.Radio)
		{
			AppendVisibleItem(SelectedItem, seen, result);
			return result;
		}
		foreach (T selectedItem in SelectedItems)
		{
			AppendVisibleItem(selectedItem, seen, result);
		}
		return result;
	}

	private void AppendVisibleItem(T item, HashSet<uint> seen, List<T> result)
	{
		if (((IExcelRow<T>)item).RowId != 0 && CanDrawItem(item) && seen.Add(((IExcelRow<T>)item).RowId))
		{
			result.Add(item);
		}
	}

	private bool IsSelected(T item, ComboSelectionMode mode)
	{
		if (mode != ComboSelectionMode.Radio)
		{
			return SelectedIDs.Contains(((IExcelRow<T>)item).RowId);
		}
		return SelectedID == ((IExcelRow<T>)item).RowId;
	}

	private static bool DrawSelector(ComboSelectionMode mode, bool isSelected)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (mode == ComboSelectionMode.Radio)
		{
			return ImGui.RadioButton(ImU8String.op_Implicit(string.Empty), isSelected);
		}
		return ImGui.Checkbox(ImU8String.op_Implicit(string.Empty), ref isSelected);
	}

	private bool ApplySelection(uint rowID, ComboSelectionMode mode)
	{
		if (mode == ComboSelectionMode.Radio)
		{
			SelectedID = ((SelectedID != rowID) ? rowID : 0u);
			return true;
		}
		if (!SelectedIDs.Remove(rowID))
		{
			SelectedIDs.Add(rowID);
		}
		return true;
	}
}
