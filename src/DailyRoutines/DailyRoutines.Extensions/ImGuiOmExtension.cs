using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Info;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace DailyRoutines.Extensions;

public static class ImGuiOmExtension
{
	extension(ImGuiOm)
	{
		public static void ConflictKeyText()
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(2, 2);
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5368645E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94)));
			((ImU8String)(ref val)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DB9E07 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2));
			((ImU8String)(ref val)).AppendFormatted<ConflictKey>(PluginConfig.Instance().ConflictKeyBinding);
			ImGui.TextUnformatted(val);
		}

		public static void ClickToCopyAndNotify(string copiedText)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (ImGui.IsItemHovered())
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (ImGui.IsItemClicked())
			{
				ImGui.SetClipboardText(ImU8String.op_Implicit(copiedText));
				OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2265D3F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44)));
			}
		}

		public static bool MultiSelectCombo<T>(string id, IDictionary<uint, T> sourceData, ref HashSet<uint> selectedItems, ref string searchInput, (string Header, ImGuiTableColumnFlags Flags, float Weight)[] headerFuncs, Func<T, Action>[] displayFuncs, Func<T, string>[] searchFuncs, bool disableCheckbox = false) where T : struct, IExcelRow<T>
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0190: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0262: Unknown result type (might be due to invalid IL or missing references)
			//IL_0325: Unknown result type (might be due to invalid IL or missing references)
			//IL_032a: Unknown result type (might be due to invalid IL or missing references)
			//IL_035d: Unknown result type (might be due to invalid IL or missing references)
			IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(id), true);
			try
			{
				bool result = false;
				ComboDisposable val2 = ImRaii.Combo(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D032D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc)), ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD48D3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), selectedItems.Count)), (ImGuiComboFlags)16);
				try
				{
				}
				finally
				{
					((ComboDisposable)(ref val2)).Dispose();
				}
				if (ImGui.IsItemClicked())
				{
					ImGui.OpenPopup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x200FAF83 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af)), (ImGuiPopupFlags)0);
				}
				ImGui.SetNextWindowSize(Globals.ScaledVector2(600f, 400f));
				PopupDisposable val3 = ImRaii.Popup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9FD0B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a)));
				try
				{
					if (PopupDisposable.op_Implicit(val3))
					{
						ImGui.SetNextItemWidth(-1f);
						ImGui.InputTextWithHint(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FF9F00 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2)), ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816A208 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd))), ref searchInput, 32, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						ImGui.Separator();
						Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X, 0f);
						TableDisposable val4 = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B212DD4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a)), headerFuncs.Length + 1, (ImGuiTableFlags)1920, vector);
						try
						{
							if (TableDisposable.op_Implicit(val4))
							{
								_003C_003Ec__DisplayClass3_0<T> CS_0024_003C_003E8__locals8 = new _003C_003Ec__DisplayClass3_0<T>();
								ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6C94C6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac)), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
								(string, ImGuiTableColumnFlags, float)[] array = headerFuncs;
								for (int i = 0; i < array.Length; i++)
								{
									(string, ImGuiTableColumnFlags, float) tuple = array[i];
									ImGui.TableSetupColumn(ImU8String.op_Implicit(tuple.Item1), tuple.Item2, tuple.Item3, 0u);
								}
								ImGui.TableNextRow((ImGuiTableRowFlags)1);
								ImGui.TableNextColumn();
								array = headerFuncs;
								for (int i = 0; i < array.Length; i++)
								{
									(string, ImGuiTableColumnFlags, float) tuple2 = array[i];
									ImGui.TableNextColumn();
									ImGui.TextUnformatted(ImU8String.op_Implicit(tuple2.Item1));
								}
								CS_0024_003C_003E8__locals8.searchInputCopy = searchInput;
								CS_0024_003C_003E8__locals8.selectedItemsCopy = selectedItems;
								using IEnumerator<KeyValuePair<uint, T>> enumerator = sourceData.OrderByDescending((KeyValuePair<uint, T> x) => CS_0024_003C_003E8__locals8.selectedItemsCopy.Contains(x.Key)).GetEnumerator();
								while (enumerator.MoveNext())
								{
									_003C_003Ec__DisplayClass3_1<T> CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass3_1<T>();
									CS_0024_003C_003E8__locals10.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals8;
									uint num2;
									(num2, CS_0024_003C_003E8__locals10.item) = (KeyValuePair<uint, T>)(ref enumerator.Current);
									if (!string.IsNullOrWhiteSpace(searchInput) && !searchFuncs.Any((Func<T, string> func) => func(CS_0024_003C_003E8__locals10.item).Contains(CS_0024_003C_003E8__locals10.CS_0024_003C_003E8__locals1.searchInputCopy, StringComparison.OrdinalIgnoreCase)))
									{
										continue;
									}
									bool flag = selectedItems.Contains(num2);
									ImGui.TableNextRow();
									ImGui.TableNextColumn();
									DisabledDisposable val5 = ImRaii.Disabled(disableCheckbox);
									try
									{
										ImU8String val6 = new ImU8String(2, 1);
										((ImU8String)(ref val6)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DC4E78 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f));
										((ImU8String)(ref val6)).AppendFormatted<uint>(num2);
										if (ImGui.Checkbox(val6, ref flag))
										{
											if (!selectedItems.Remove(num2))
											{
												selectedItems.Add(num2);
											}
											result = true;
										}
									}
									finally
									{
										((IDisposable)val5)?.Dispose();
									}
									foreach (Func<T, Action> obj in displayFuncs)
									{
										ImGui.TableNextColumn();
										obj(CS_0024_003C_003E8__locals10.item)();
									}
								}
							}
						}
						finally
						{
							((TableDisposable)(ref val4)).Dispose();
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val3)).Dispose();
				}
				return result;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		public static bool SingleSelectCombo<T>(string id, IDictionary<uint, T> sourceData, ref uint selectedItem, ref string searchInput, Func<T, string> previewFunc, (string Header, ImGuiTableColumnFlags Flags, float Weight)[] headerFuncs, Func<T, Action>[] displayFuncs, Func<T, string>[] searchFuncs, bool disableCheckbox = false) where T : struct, IExcelRow<T>
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(id), true);
			try
			{
				bool result = false;
				string text = ((selectedItem != 0) ? previewFunc(sourceData[selectedItem]) : (LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E38506D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x41191FA5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52)));
				ComboDisposable val2 = ImRaii.Combo(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F35BE4E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862)), ImU8String.op_Implicit(text), (ImGuiComboFlags)16);
				try
				{
				}
				finally
				{
					((ComboDisposable)(ref val2)).Dispose();
				}
				if (ImGui.IsItemClicked())
				{
					ImGui.OpenPopup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x611660C6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402)), (ImGuiPopupFlags)0);
				}
				ImGui.SetNextWindowSize(Globals.ScaledVector2(600f, 400f));
				PopupDisposable val3 = ImRaii.Popup(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E4611A3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e)));
				try
				{
					if (PopupDisposable.op_Implicit(val3))
					{
						_003C_003Ec__DisplayClass4_0<T> CS_0024_003C_003E8__locals8 = new _003C_003Ec__DisplayClass4_0<T>();
						ImGui.SetNextItemWidth(-1f);
						ImGui.InputTextWithHint(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A6FE5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1)), ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF05B4C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d))), ref searchInput, 32, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						ImGui.Separator();
						Vector2 vector = new Vector2(ImGui.GetContentRegionAvail().X, 0f);
						TableDisposable val4 = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2265A7D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44)), headerFuncs.Length + 1, (ImGuiTableFlags)1920, vector);
						try
						{
							if (!val4)
							{
								return result;
							}
							ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D15414D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275)), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
							(string, ImGuiTableColumnFlags, float)[] array = headerFuncs;
							for (int i = 0; i < array.Length; i++)
							{
								(string, ImGuiTableColumnFlags, float) tuple = array[i];
								ImGui.TableSetupColumn(ImU8String.op_Implicit(tuple.Item1), tuple.Item2, tuple.Item3, 0u);
							}
							ImGui.TableNextRow((ImGuiTableRowFlags)1);
							ImGui.TableNextColumn();
							array = headerFuncs;
							for (int i = 0; i < array.Length; i++)
							{
								(string, ImGuiTableColumnFlags, float) tuple2 = array[i];
								ImGui.TableNextColumn();
								ImGui.TextUnformatted(ImU8String.op_Implicit(tuple2.Item1));
							}
							CS_0024_003C_003E8__locals8.searchInputCopy = searchInput;
							CS_0024_003C_003E8__locals8.selectedItemCopy = selectedItem;
							using IEnumerator<KeyValuePair<uint, T>> enumerator = sourceData.OrderByDescending((KeyValuePair<uint, T> x) => x.Key == CS_0024_003C_003E8__locals8.selectedItemCopy).GetEnumerator();
							while (enumerator.MoveNext())
							{
								_003C_003Ec__DisplayClass4_1<T> CS_0024_003C_003E8__locals10 = new _003C_003Ec__DisplayClass4_1<T>();
								CS_0024_003C_003E8__locals10.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals8;
								uint num2;
								(num2, CS_0024_003C_003E8__locals10.item) = (KeyValuePair<uint, T>)(ref enumerator.Current);
								if (!string.IsNullOrWhiteSpace(searchInput) && !searchFuncs.Any((Func<T, string> func) => func(CS_0024_003C_003E8__locals10.item).Contains(CS_0024_003C_003E8__locals10.CS_0024_003C_003E8__locals1.searchInputCopy, StringComparison.OrdinalIgnoreCase)))
								{
									continue;
								}
								ImGui.TableNextRow();
								bool flag = selectedItem == num2;
								ImGui.TableNextColumn();
								DisabledDisposable val5 = ImRaii.Disabled(disableCheckbox);
								try
								{
									if (ImGui.Checkbox(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CDBE45 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0) + num2), ref flag))
									{
										selectedItem = num2;
										result = true;
									}
								}
								finally
								{
									((IDisposable)val5)?.Dispose();
								}
								foreach (Func<T, Action> obj in displayFuncs)
								{
									ImGui.TableNextColumn();
									obj(CS_0024_003C_003E8__locals10.item)();
								}
							}
						}
						finally
						{
							((TableDisposable)(ref val4)).Dispose();
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val3)).Dispose();
				}
				return result;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}
}
