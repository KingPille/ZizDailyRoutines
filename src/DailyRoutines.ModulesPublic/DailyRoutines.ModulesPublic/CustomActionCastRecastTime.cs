using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.UI.Arrays;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class CustomActionCastRecastTime : ModuleBase
{
	private unsafe delegate uint CastInfoUpdateTotalDelegate(CastInfo* data, uint spellActionID, float process, float processTotal);

	private class Config : ModuleConfig
	{
		public Dictionary<uint, float> CustomCastTimeSet = new Dictionary<uint, float>();

		public Dictionary<uint, float> CustomRecastTimeSet = new Dictionary<uint, float>();

		public float LongCastTimeReduction = 400f;
	}

	private Hook<GetAdjustedCastTime>? GetAdjustedCastTimeHook;

	private Hook<GetAdjustedRecastTime>? GetAdjustedRecastTimeHook;

	private static readonly CompSig CastInfoUpdateTotalSig = new CompSig("48 89 5C 24 ?? 57 48 83 EC ?? 48 8B F9 0F 29 74 24 ?? 0F B6 49");

	private Hook<CastInfoUpdateTotalDelegate>? CastInfoUpdateTotalHook;

	private Config config;

	private ActionSelectCombo castActionCombo;

	private JobSelectCombo castJobCombo;

	private ActionSelectCombo recastActionCombo;

	private JobSelectCombo recastJobCombo;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CustomActionCastRecastTimeTitle"),
		Description = LanguageManager.Get("CustomActionCastRecastTimeDescription"),
		Category = ModuleCategory.Action
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected unsafe override void Init()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		castActionCombo = new ActionSelectCombo("CastActionSelect");
		castJobCombo = new JobSelectCombo("CastJobSelect");
		recastActionCombo = new ActionSelectCombo("RecastActionSelect");
		recastJobCombo = new JobSelectCombo("RecastJobSelect");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (GetAdjustedCastTimeHook == null)
		{
			GetAdjustedCastTimeHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<GetAdjustedCastTime>(typeof(MemberFunctionPointers), "GetAdjustedCastTime", new GetAdjustedCastTime(GetAdjustedCastTimeDetour));
		}
		GetAdjustedCastTimeHook.Enable();
		if (GetAdjustedRecastTimeHook == null)
		{
			GetAdjustedRecastTimeHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<GetAdjustedRecastTime>(typeof(MemberFunctionPointers), "GetAdjustedRecastTime", new GetAdjustedRecastTime(GetAdjustedRecastTimeDetour));
		}
		GetAdjustedRecastTimeHook.Enable();
		if (CastInfoUpdateTotalHook == null)
		{
			CastInfoUpdateTotalHook = CastInfoUpdateTotalSig.GetHook<CastInfoUpdateTotalDelegate>(CastInfoUpdateTotalDetour);
		}
		CastInfoUpdateTotalHook.Enable();
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_083e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0873: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_091f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0924: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0938: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("CustomActionCastRecastTime-CustomCastTime"));
		ImGui.TextColored(ref vector, val);
		IdDisposable val2 = ImRaii.PushId(ImU8String.op_Implicit("Cast"), true);
		Action value;
		ReadOnlySeString name;
		try
		{
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				ImGui.SetNextItemWidth(150f * Globals.GlobalUIScale);
				ImU8String val4 = new ImU8String(23, 1);
				((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("CustomActionCastRecastTime-DefaultReduction"));
				((ImU8String)(ref val4)).AppendLiteral("(ms)###DefaultReduction");
				if (ImGui.InputFloat(val4, ref config.LongCastTimeReduction, 10f, 100f, ImU8String.op_Implicit("%.0f"), (ImGuiInputTextFlags)0))
				{
					config.LongCastTimeReduction = MathF.Max(0f, config.LongCastTimeReduction);
					config.Save(this);
				}
				ImGuiOm.HelpMarker(LanguageManager.Get("CustomActionCastRecastTime-DefaultReduction-Help", config.LongCastTimeReduction), 40f, (FontAwesomeIcon)61530);
				ImGui.NewLine();
				DisabledDisposable val5 = ImRaii.Disabled(castActionCombo.SelectedID == 0 || config.CustomCastTimeSet.ContainsKey(castActionCombo.SelectedID));
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")) && config.CustomCastTimeSet.TryAdd(castActionCombo.SelectedID, ActionManager.GetAdjustedCastTime((ActionType)1, castActionCombo.SelectedID, true, (CastTimeProc*)null)))
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				ImGui.SameLine();
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				castActionCombo.DrawRadio();
				val5 = ImRaii.Disabled(castJobCombo.SelectedID == 0);
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add") + "##ClassJob") && LuminaGetter.TryGetSubRowAll<ClassJobActionUI>(castJobCombo.SelectedID, out SubrowCollection<ClassJobActionUI> item))
					{
						Enumerator<ClassJobActionUI> enumerator = item.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								ClassJobActionUI current = enumerator.Current;
								if (((ClassJobActionUI)(ref current)).UpgradeAction.RowId != 0)
								{
									value = ((ClassJobActionUI)(ref current)).UpgradeAction.Value;
									name = ((Action)(ref value)).Name;
									if (!((ReadOnlySeString)(ref name)).IsEmpty)
									{
										config.CustomCastTimeSet.TryAdd(((ClassJobActionUI)(ref current)).UpgradeAction.RowId, ActionManager.GetAdjustedCastTime((ActionType)1, ((ClassJobActionUI)(ref current)).UpgradeAction.RowId, true, (CastTimeProc*)null));
									}
								}
							}
						}
						finally
						{
							((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
						}
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				ImGui.SameLine();
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				castJobCombo.DrawRadio();
				ImGui.Spacing();
				TableDisposable val6 = ImRaii.Table(ImU8String.op_Implicit("OptimizedLongCastTimeActionCastTable"), 3, (ImGuiTableFlags)1920);
				try
				{
					if (TableDisposable.op_Implicit(val6))
					{
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1340u)), (ImGuiTableColumnFlags)4, 40f, 0u);
						ImU8String val7 = new ImU8String(5, 1);
						((ImU8String)(ref val7)).AppendFormatted<string>(LuminaWrapper.GetAddonText(701u));
						((ImU8String)(ref val7)).AppendLiteral(" (ms)");
						ImGui.TableSetupColumn(val7, (ImGuiTableColumnFlags)4, 20f, 0u);
						ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Operation")), (ImGuiTableColumnFlags)8, 2f * ImGui.GetTextLineHeightWithSpacing(), 0u);
						ImGui.TableHeadersRow();
						uint num = 0u;
						ISharedImmediateTexture val9 = default(ISharedImmediateTexture);
						foreach (KeyValuePair<uint, float> item5 in config.CustomCastTimeSet)
						{
							if (LuminaGetter.TryGetRow<Action>(item5.Key, out Action item2))
							{
								ImGui.TableNextRow();
								ImGui.TableNextColumn();
								ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
								GameIconLookup val8 = new GameIconLookup((uint)((Action)(ref item2)).Icon, false, true, (ClientLanguage?)null);
								if (obj.TryGetFromGameIcon(ref val8, ref val9))
								{
									ImTextureID handle = val9.GetWrapOrEmpty().Handle;
									Vector2 imageSize = new Vector2(ImGui.GetTextLineHeightWithSpacing());
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
									name = ((Action)(ref item2)).Name;
									defaultInterpolatedStringHandler.AppendFormatted(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString());
									defaultInterpolatedStringHandler.AppendLiteral(" (");
									defaultInterpolatedStringHandler.AppendFormatted(((Action)(ref item2)).RowId);
									defaultInterpolatedStringHandler.AppendLiteral(")");
									ImGuiOm.SelectableImageWithText(handle, imageSize, defaultInterpolatedStringHandler.ToStringAndClear(), selected: false, (ImGuiSelectableFlags)0);
								}
								else
								{
									ImU8String val10 = new ImU8String(3, 2);
									name = ((Action)(ref item2)).Name;
									((ImU8String)(ref val10)).AppendFormatted<string>(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString());
									((ImU8String)(ref val10)).AppendLiteral(" (");
									((ImU8String)(ref val10)).AppendFormatted<uint>(((Action)(ref item2)).RowId);
									((ImU8String)(ref val10)).AppendLiteral(")");
									ImGui.TextUnformatted(val10);
								}
								ImGui.TableNextColumn();
								float value2 = item5.Value;
								ImGui.SetNextItemWidth(-1f);
								ImU8String val11 = new ImU8String(18, 1);
								((ImU8String)(ref val11)).AppendLiteral("###CustomCastTime_");
								((ImU8String)(ref val11)).AppendFormatted<uint>(item5.Key);
								if (ImGui.InputFloat(val11, ref value2, 10f, 100f, ImU8String.op_Implicit("%.0f"), (ImGuiInputTextFlags)0))
								{
									config.CustomCastTimeSet[item5.Key] = MathF.Max(0f, value2);
								}
								if (ImGui.IsItemDeactivatedAfterEdit())
								{
									config.Save(this);
								}
								ImGui.TableNextColumn();
								if (ImGuiOm.ButtonIcon($"###DeleteCast_{item5.Key}", (FontAwesomeIcon)61944, LanguageManager.Get("Delete")))
								{
									num = item5.Key;
								}
							}
						}
						if (num != 0)
						{
							config.CustomCastTimeSet.Remove(num);
							config.Save(this);
						}
					}
				}
				finally
				{
					((TableDisposable)(ref val6)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val12 = default(ImU8String);
		((ImU8String)(ref val12))._002Ector(0, 1);
		((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("CustomActionCastRecastTime-CustomRecastTime"));
		ImGui.TextColored(ref vector, val12);
		val2 = ImRaii.PushId(ImU8String.op_Implicit("Recast"), true);
		try
		{
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				DisabledDisposable val5 = ImRaii.Disabled(recastActionCombo.SelectedID == 0 || config.CustomRecastTimeSet.ContainsKey(recastActionCombo.SelectedID));
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add") + "##Action") && config.CustomRecastTimeSet.TryAdd(recastActionCombo.SelectedID, ActionManager.GetAdjustedRecastTime((ActionType)1, recastActionCombo.SelectedID, true)))
					{
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				ImGui.SameLine();
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				recastActionCombo.DrawRadio();
				val5 = ImRaii.Disabled(recastJobCombo.SelectedID == 0);
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add") + "##ClassJob") && LuminaGetter.TryGetSubRowAll<ClassJobActionUI>(recastJobCombo.SelectedID, out SubrowCollection<ClassJobActionUI> item3))
					{
						Enumerator<ClassJobActionUI> enumerator = item3.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								ClassJobActionUI current3 = enumerator.Current;
								if (((ClassJobActionUI)(ref current3)).UpgradeAction.RowId != 0)
								{
									value = ((ClassJobActionUI)(ref current3)).UpgradeAction.Value;
									name = ((Action)(ref value)).Name;
									if (!((ReadOnlySeString)(ref name)).IsEmpty)
									{
										config.CustomRecastTimeSet.TryAdd(((ClassJobActionUI)(ref current3)).UpgradeAction.RowId, ActionManager.GetAdjustedRecastTime((ActionType)1, ((ClassJobActionUI)(ref current3)).UpgradeAction.RowId, true));
									}
								}
							}
						}
						finally
						{
							((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
						}
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				ImGui.SameLine();
				ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
				recastJobCombo.DrawRadio();
				ImGui.Spacing();
				TableDisposable val13 = ImRaii.Table(ImU8String.op_Implicit("OptimizedLongCastTimeActionRecastTable"), 3, (ImGuiTableFlags)1920);
				try
				{
					if (!TableDisposable.op_Implicit(val13))
					{
						return;
					}
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1340u)), (ImGuiTableColumnFlags)4, 40f, 0u);
					ImU8String val14 = new ImU8String(5, 1);
					((ImU8String)(ref val14)).AppendFormatted<string>(LuminaWrapper.GetAddonText(702u));
					((ImU8String)(ref val14)).AppendLiteral(" (ms)");
					ImGui.TableSetupColumn(val14, (ImGuiTableColumnFlags)4, 20f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Operation")), (ImGuiTableColumnFlags)8, 2f * ImGui.GetTextLineHeightWithSpacing(), 0u);
					ImGui.TableHeadersRow();
					int num2 = -1;
					ISharedImmediateTexture val15 = default(ISharedImmediateTexture);
					foreach (KeyValuePair<uint, float> item6 in config.CustomRecastTimeSet)
					{
						if (LuminaGetter.TryGetRow<Action>(item6.Key, out Action item4))
						{
							ImGui.TableNextRow();
							ImGui.TableNextColumn();
							ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
							GameIconLookup val8 = new GameIconLookup((uint)((Action)(ref item4)).Icon, false, true, (ClientLanguage?)null);
							if (obj2.TryGetFromGameIcon(ref val8, ref val15))
							{
								ImTextureID handle2 = val15.GetWrapOrEmpty().Handle;
								Vector2 imageSize2 = new Vector2(ImGui.GetTextLineHeightWithSpacing());
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
								name = ((Action)(ref item4)).Name;
								defaultInterpolatedStringHandler2.AppendFormatted(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString());
								defaultInterpolatedStringHandler2.AppendLiteral(" (");
								defaultInterpolatedStringHandler2.AppendFormatted(((Action)(ref item4)).RowId);
								defaultInterpolatedStringHandler2.AppendLiteral(")");
								ImGuiOm.SelectableImageWithText(handle2, imageSize2, defaultInterpolatedStringHandler2.ToStringAndClear(), selected: false, (ImGuiSelectableFlags)0);
							}
							else
							{
								ImU8String val16 = new ImU8String(3, 2);
								name = ((Action)(ref item4)).Name;
								((ImU8String)(ref val16)).AppendFormatted<string>(((object)(*(ReadOnlySeString*)(&name))/*cast due to constrained. prefix*/).ToString());
								((ImU8String)(ref val16)).AppendLiteral(" (");
								((ImU8String)(ref val16)).AppendFormatted<uint>(((Action)(ref item4)).RowId);
								((ImU8String)(ref val16)).AppendLiteral(")");
								ImGui.TextUnformatted(val16);
							}
							ImGui.TableNextColumn();
							float value3 = item6.Value;
							ImGui.SetNextItemWidth(-1f);
							ImU8String val17 = new ImU8String(20, 1);
							((ImU8String)(ref val17)).AppendLiteral("###CustomRecastTime_");
							((ImU8String)(ref val17)).AppendFormatted<uint>(item6.Key);
							if (ImGui.InputFloat(val17, ref value3, 10f, 100f, ImU8String.op_Implicit("%.0f"), (ImGuiInputTextFlags)0))
							{
								config.CustomRecastTimeSet[item6.Key] = MathF.Max(0f, value3);
							}
							if (ImGui.IsItemDeactivatedAfterEdit())
							{
								config.Save(this);
							}
							ImGui.TableNextColumn();
							if (ImGuiOm.ButtonIcon($"###DeleteRecast_{item6.Key}", (FontAwesomeIcon)61944, LanguageManager.Get("Delete")))
							{
								num2 = (int)item6.Key;
							}
						}
					}
					if (num2 != -1)
					{
						config.CustomRecastTimeSet.Remove((uint)num2);
						config.Save(this);
					}
				}
				finally
				{
					((TableDisposable)(ref val13)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private int GetAdjustedRecastTimeDetour(ActionType actionType, uint actionID, bool applyClassMechanics)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		int result = GetAdjustedRecastTimeHook.Original.Invoke(actionType, actionID, applyClassMechanics);
		if ((int)actionType != 1)
		{
			return result;
		}
		if (config.CustomRecastTimeSet.TryGetValue(actionID, out var value))
		{
			return (int)value;
		}
		return result;
	}

	private unsafe int GetAdjustedCastTimeDetour(ActionType actionType, uint actionID, bool applyProcess, CastTimeProc* castTimeProc)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		int num = GetAdjustedCastTimeHook.Original.Invoke(actionType, actionID, applyProcess, castTimeProc);
		if ((int)actionType != 1)
		{
			return num;
		}
		if (config.CustomCastTimeSet.TryGetValue(actionID, out var value))
		{
			return (int)value;
		}
		if (ActionManager.GetAdjustedRecastTime(actionType, actionID, true) <= num)
		{
			return (int)MathF.Max(0f, num - (int)config.LongCastTimeReduction);
		}
		return num;
	}

	private unsafe uint CastInfoUpdateTotalDetour(CastInfo* data, uint spellActionID, float processTotal, float processStart)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		uint actionId = ((CastInfo)data).ActionId;
		ActionType val = (ActionType)((CastInfo)data).ActionType;
		if (actionId == spellActionID && (int)val == 1)
		{
			if (config.CustomCastTimeSet.TryGetValue(actionId, out var value))
			{
				processTotal = value / 1000f;
				((CastBarNumberArray)CastBarNumberArray.Instance()).TotalCastTime = (int)processTotal;
			}
			else if ((float)ActionManager.GetAdjustedRecastTime(val, actionId, true) <= processTotal * 1000f)
			{
				processTotal = MathF.Max(processTotal - config.LongCastTimeReduction / 1000f, 0f);
				((CastBarNumberArray)CastBarNumberArray.Instance()).TotalCastTime = (int)processTotal;
			}
		}
		return CastInfoUpdateTotalHook.Original(data, spellActionID, processTotal, processStart);
	}
}
