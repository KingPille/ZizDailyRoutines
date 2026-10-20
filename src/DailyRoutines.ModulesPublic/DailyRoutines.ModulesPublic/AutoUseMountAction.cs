using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoUseMountAction : ModuleBase
{
	private class Config : ModuleConfig
	{
		public Dictionary<uint, MountAction> MountActions { get; set; } = new Dictionary<uint, MountAction>();
	}

	private class MountAction : IEquatable<MountAction>
	{
		public uint MountID { get; set; }

		public uint ActionID { get; set; }

		public MountAction()
		{
		}

		public MountAction(uint mountID, uint actionID)
		{
			MountID = mountID;
			ActionID = actionID;
		}

		public bool Equals(MountAction? other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			return MountID == other.MountID;
		}

		public override bool Equals(object? obj)
		{
			if (obj is MountAction other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)MountID;
		}
	}

	private Config config;

	private string mountSearchInput = string.Empty;

	private uint selectedActionID;

	private uint selectedMountID;

	private static readonly FrozenSet<ConditionFlag> InvalidConditions = FrozenSet.Create(RuntimeHelpers.CreateSpan<ConditionFlag>((RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/));

	private static readonly LuminaSearcher<Mount> MountSearcher = new LuminaSearcher<Mount>(from x in (IEnumerable<Mount>)LuminaGetter.Get<Mount>()
		where ((Mount)(ref x)).MountAction.RowId != 0
		where ((Mount)(ref x)).Icon > 0
		where !string.IsNullOrEmpty(((object)((Mount)(ref x)).Singular/*cast due to constrained. prefix*/).ToString())
		group x by ((object)((Mount)(ref x)).Singular/*cast due to constrained. prefix*/).ToString() into x
		select x.First(), new Func<Mount, string>[1]
	{
		(Mount x) => ((object)((Mount)(ref x)).Singular/*cast due to constrained. prefix*/).ToString()
	});

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoUseMountActionTitle"),
		Description = LanguageManager.Get("AutoUseMountActionDescription"),
		Category = ModuleCategory.Action,
		Author = new string[2] { "逆光", "Bill" }
	};

	protected override void Init()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		OmenServiceBase<UseActionManager>.Instance().RegPostUseActionLocation(OnPostUseAction);
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
		{
			OnConditionChanged((ConditionFlag)4, value: true);
		}
	}

	protected override void Uninit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
		OmenServiceBase<UseActionManager>.Instance().Unreg(OnPostUseAction);
		base.TaskHelper?.Abort();
	}

	protected override void ConfigUI()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("MountActionsTable"), 3);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("坐骑"), (ImGuiTableColumnFlags)0, 200f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("动作"), (ImGuiTableColumnFlags)0, 200f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("操作"), (ImGuiTableColumnFlags)0, 80f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			if (ImGuiOm.SelectableIconCentered("AddNewAction", (FontAwesomeIcon)61543, selected: false, (ImGuiSelectableFlags)0))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("AddNewActionPopup"), (ImGuiPopupFlags)0);
			}
			PopupDisposable val2 = ImRaii.Popup(ImU8String.op_Implicit("AddNewActionPopup"));
			try
			{
				if (PopupDisposable.op_Implicit(val2))
				{
					ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
					ImU8String val3 = new ImU8String(18, 1);
					((ImU8String)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(4964u));
					((ImU8String)(ref val3)).AppendLiteral("##MountSelectCombo");
					Mount item;
					ComboDisposable val4 = ImRaii.Combo(val3, ImU8String.op_Implicit((selectedMountID != 0 && LuminaGetter.TryGetRow<Mount>(selectedMountID, out item)) ? (((object)((Mount)(ref item)).Singular/*cast due to constrained. prefix*/).ToString() ?? "") : string.Empty), (ImGuiComboFlags)8);
					try
					{
						if (ComboDisposable.op_Implicit(val4))
						{
							ImGui.SetNextItemWidth(-1f);
							if (ImGui.InputTextWithHint(ImU8String.op_Implicit("###MountSearchInput"), ImU8String.op_Implicit(LanguageManager.Get("PleaseSearch")), ref mountSearchInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
							{
								MountSearcher?.Search(mountSearchInput);
							}
							if (MountSearcher != null)
							{
								foreach (Mount item5 in MountSearcher.SearchResult)
								{
									Mount current = item5;
									if (ImageHelper.TryGetGameIcon(((Mount)(ref current)).Icon, out IDalamudTextureWrap texture) && ImGuiOm.SelectableImageWithText(texture.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()), ((object)((Mount)(ref current)).Singular/*cast due to constrained. prefix*/).ToString() ?? "", ((Mount)(ref current)).RowId == selectedMountID, (ImGuiSelectableFlags)0))
									{
										selectedMountID = ((Mount)(ref current)).RowId;
									}
								}
							}
						}
					}
					finally
					{
						((ComboDisposable)(ref val4)).Dispose();
					}
					if (selectedMountID != 0 && LuminaGetter.TryGetRow<Mount>(selectedMountID, out Mount item2))
					{
						MountAction? valueNullable = ((Mount)(ref item2)).MountAction.ValueNullable;
						if (valueNullable.HasValue)
						{
							MountAction valueOrDefault = valueNullable.GetValueOrDefault();
							Collection<RowRef<Action>> action = ((MountAction)(ref valueOrDefault)).Action;
							if (action.Count > 0)
							{
								ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
								ImU8String val5 = new ImU8String(20, 1);
								((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("Action"));
								((ImU8String)(ref val5)).AppendLiteral("###ActionSelectCombo");
								ComboDisposable val6 = ImRaii.Combo(val5, ImU8String.op_Implicit(LuminaWrapper.GetActionName(selectedActionID)), (ImGuiComboFlags)0);
								try
								{
									if (ComboDisposable.op_Implicit(val6))
									{
										Enumerator<RowRef<Action>> enumerator2 = action.GetEnumerator();
										try
										{
											while (enumerator2.MoveNext())
											{
												RowRef<Action> current2 = enumerator2.Current;
												if (current2.RowId == 0)
												{
													continue;
												}
												Action value = current2.Value;
												if (ImageHelper.TryGetGameIcon(((Action)(ref value)).Icon, out IDalamudTextureWrap texture2))
												{
													ImTextureID handle = texture2.Handle;
													Vector2 imageSize = new Vector2(ImGui.GetTextLineHeightWithSpacing());
													DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
													value = current2.Value;
													defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(((Action)(ref value)).Name);
													if (ImGuiOm.SelectableImageWithText(handle, imageSize, defaultInterpolatedStringHandler.ToStringAndClear(), current2.RowId == selectedActionID, (ImGuiSelectableFlags)0))
													{
														selectedActionID = current2.RowId;
													}
												}
											}
										}
										finally
										{
											((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
										}
									}
								}
								finally
								{
									((ComboDisposable)(ref val6)).Dispose();
								}
							}
						}
					}
					ImGui.Spacing();
					DisabledDisposable val7 = ImRaii.Disabled(selectedMountID == 0 || selectedActionID == 0);
					try
					{
						if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Add")), default(Vector2)))
						{
							MountAction mountAction = new MountAction(selectedMountID, selectedActionID);
							if (config.MountActions.TryAdd(mountAction.MountID, mountAction))
							{
								config.Save(this);
							}
							ImGui.CloseCurrentPopup();
						}
					}
					finally
					{
						((IDisposable)val7)?.Dispose();
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val2)).Dispose();
			}
			foreach (KeyValuePair<uint, MountAction> mountAction2 in config.MountActions)
			{
				MountAction value2 = mountAction2.Value;
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				if (LuminaGetter.TryGetRow<Mount>(value2.MountID, out Mount item3) && ImageHelper.TryGetGameIcon(((Mount)(ref item3)).Icon, out IDalamudTextureWrap texture3))
				{
					ImGuiOm.TextImage(((object)((Mount)(ref item3)).Singular/*cast due to constrained. prefix*/).ToString() ?? "", texture3.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
				}
				ImGui.TableNextColumn();
				if (LuminaGetter.TryGetRow<Action>(value2.ActionID, out Action item4) && ImageHelper.TryGetGameIcon(((Action)(ref item4)).Icon, out IDalamudTextureWrap texture4))
				{
					ImGuiOm.TextImage(((object)((Action)(ref item4)).Name/*cast due to constrained. prefix*/).ToString() ?? "", texture4.Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
				}
				ImGui.TableNextColumn();
				if (ImGuiOm.ButtonIcon($"{value2.MountID}_Delete", (FontAwesomeIcon)62189))
				{
					config.MountActions.Remove(value2.MountID);
					config.Save(this);
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		if (InvalidConditions.Contains(flag))
		{
			if (value)
			{
				base.TaskHelper.Abort();
			}
			else
			{
				IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
				if (localPlayer == null || !config.MountActions.ContainsKey(localPlayer.CurrentMount?.RowId ?? 0))
				{
					return;
				}
				base.TaskHelper.Enqueue((Func<bool>)UseAction, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
		}
		if ((int)flag != 4)
		{
			return;
		}
		if (value)
		{
			IPlayerCharacter localPlayer2 = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
			if (localPlayer2 != null && config.MountActions.ContainsKey(localPlayer2.CurrentMount?.RowId ?? 0))
			{
				base.TaskHelper.Enqueue((Func<bool>)UseAction, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
		}
		else
		{
			base.TaskHelper.Abort();
		}
	}

	private void OnPostUseAction(bool result, ActionType actionType, uint actionID, ulong targetID, Vector3 location, uint extraParam, byte a7)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if ((int)actionType != 1)
		{
			return;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer != null)
		{
			uint key = localPlayer.CurrentMount?.RowId ?? 0;
			if (config.MountActions.TryGetValue(key, out MountAction value) && value.ActionID == actionID)
			{
				base.TaskHelper.Enqueue((Func<bool>)UseAction, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
		}
	}

	private unsafe bool UseAction()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return true;
		}
		uint key = localPlayer.CurrentMount?.RowId ?? 0;
		if (!config.MountActions.TryGetValue(key, out MountAction value))
		{
			return true;
		}
		if (((ActionManager)ActionManager.Instance()).GetActionStatus((ActionType)1, value.ActionID, 3758096384uL, true, true, (uint*)null) != 0)
		{
			return false;
		}
		((ActionManager)ActionManager.Instance()).UseAction((ActionType)1, value.ActionID, 3758096384uL, 0u, (UseActionMode)0, 0u, (bool*)null);
		return true;
	}
}
