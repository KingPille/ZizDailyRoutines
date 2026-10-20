using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.DutyState;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Duty;

public class AutoMovePetPosition : ModuleBase
{
	private class Config : ModuleConfig
	{
		public Dictionary<uint, List<PositionSchedule>> PositionSchedules = new Dictionary<uint, List<PositionSchedule>>();
	}

	private class PositionSchedule(string guid) : IEquatable<PositionSchedule>
	{
		public bool Enabled { get; set; } = true;

		public uint ZoneID { get; set; }

		public string Note { get; set; } = string.Empty;

		public int DelayS { get; set; }

		public Vector2 Position { get; set; }

		public string GUID { get; set; } = guid;

		public bool Equals(PositionSchedule? other)
		{
			if ((object)other == null)
			{
				return false;
			}
			if ((object)this == other)
			{
				return true;
			}
			return GUID == other.GUID;
		}

		public PositionSchedule Copy()
		{
			return new PositionSchedule(GUID)
			{
				Enabled = Enabled,
				ZoneID = ZoneID,
				Note = Note,
				DelayS = DelayS,
				Position = Position
			};
		}

		public override string ToString()
		{
			return GUID;
		}

		public override bool Equals(object? obj)
		{
			if (!(obj is PositionSchedule other))
			{
				return false;
			}
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return GUID.GetHashCode();
		}

		public static bool operator ==(PositionSchedule? left, PositionSchedule? right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(PositionSchedule? left, PositionSchedule? right)
		{
			return !object.Equals(left, right);
		}
	}

	private Config config;

	private ContentSelectCombo contentSelectCombo;

	private DateTime battleStartTime = DateTime.MinValue;

	private bool isPicking;

	private (uint territoryKey, int index)? currentPickingRow;

	private static readonly FrozenSet<uint> ValidJobs = FrozenSet.Create<uint>(26u, 27u, 28u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMovePetPositionTitle"),
		Description = LanguageManager.Get("AutoMovePetPositionDescription"),
		Category = ModuleCategory.Duty,
		Author = new string[1] { "Wotou" }
	};

	protected override void Init()
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		contentSelectCombo = new ContentSelectCombo("Content");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced += new DutyRecommencedDelegate(OnDutyRecommenced);
		DalamudServiceExtension.Instance<ICondition>().ConditionChange += new ConditionChangeDelegate(OnConditionChanged);
		base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	protected override void Uninit()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		DalamudServiceExtension.Instance<IDutyState>().DutyRecommenced -= new DutyRecommencedDelegate(OnDutyRecommenced);
		DalamudServiceExtension.Instance<ICondition>().ConditionChange -= new ConditionChangeDelegate(OnConditionChanged);
	}

	protected override void ConfigUI()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ImGui.GetContentRegionAvail() * 0.9f;
		vector.Y = 0f;
		Vector2 vector2 = vector;
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("PositionSchedulesTable"), 6, (ImGuiTableFlags)1984, vector2);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit("新增"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("区域"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("备注"), (ImGuiTableColumnFlags)4, 25f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("延迟"), (ImGuiTableColumnFlags)8, 50f * Globals.GlobalUIScale, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("坐标"), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("操作"), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			if (ImGuiOm.ButtonIconSelectable("AddNewPreset", (FontAwesomeIcon)61543))
			{
				if (!config.PositionSchedules.ContainsKey(1u))
				{
					config.PositionSchedules[1u] = new List<PositionSchedule>();
				}
				config.PositionSchedules[1u].Add(new PositionSchedule(Guid.NewGuid().ToString())
				{
					Enabled = true,
					ZoneID = 1u,
					DelayS = 0,
					Position = default(Vector2)
				});
				config.Save(this);
				base.TaskHelper.Abort();
				base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			}
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Zone")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Note")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61463)));
			ImGuiOm.TooltipHover(LanguageManager.Get("AutoMovePetPosition-Delay") + " (s)");
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Position")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Operation")));
			KeyValuePair<uint, List<PositionSchedule>>[] array = config.PositionSchedules.ToArray();
			Vector3 vector3 = default(Vector3);
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<uint, List<PositionSchedule>> keyValuePair = array[i];
				var (item, list2) = (KeyValuePair<uint, List<PositionSchedule>>)(ref keyValuePair);
				if (list2.Count == 0)
				{
					continue;
				}
				for (int j = 0; j < list2.Count; j++)
				{
					PositionSchedule positionSchedule = list2[j];
					IdDisposable val2 = ImRaii.PushId(ImU8String.op_Implicit(positionSchedule.GUID), true);
					try
					{
						ImGui.TableNextRow();
						bool enabled = positionSchedule.Enabled;
						ImGui.TableNextColumn();
						if (ImGui.Checkbox(ImU8String.op_Implicit("##启用"), ref enabled))
						{
							positionSchedule.Enabled = enabled;
							config.Save(this);
							base.TaskHelper.Abort();
							base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
						}
						uint zoneID = positionSchedule.ZoneID;
						if (!LuminaGetter.TryGetRow<TerritoryType>(zoneID, out TerritoryType item2))
						{
							continue;
						}
						contentSelectCombo.SelectedID = ((TerritoryType)(ref item2)).ContentFinderCondition.RowId;
						ImGui.TableNextColumn();
						ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
						if (contentSelectCombo.DrawRadio())
						{
							ContentFinderCondition selectedItem = contentSelectCombo.SelectedItem;
							zoneID = ((ContentFinderCondition)(ref selectedItem)).TerritoryType.RowId;
							PositionSchedule positionSchedule2 = positionSchedule.Copy();
							positionSchedule2.ZoneID = zoneID;
							list2.Remove(positionSchedule);
							config.PositionSchedules.TryAdd(zoneID, new List<PositionSchedule>());
							config.PositionSchedules[zoneID].Add(positionSchedule2);
							config.Save(this);
							base.TaskHelper.Abort();
							base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
							continue;
						}
						ImGui.TableNextColumn();
						string note = positionSchedule.Note;
						ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
						ImGui.InputText(ImU8String.op_Implicit("##备注"), ref note, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
						if (ImGui.IsItemDeactivatedAfterEdit() && note != positionSchedule.Note)
						{
							positionSchedule.Note = note;
							config.Save(this);
							base.TaskHelper.Abort();
							base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
						}
						ImGui.TableNextColumn();
						int delayS = positionSchedule.DelayS;
						ImGui.SetNextItemWidth(50f * Globals.GlobalUIScale);
						ImGui.InputInt(ImU8String.op_Implicit("##延迟"), ref delayS, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							delayS = Math.Max(0, delayS);
							if (delayS != positionSchedule.DelayS)
							{
								positionSchedule.DelayS = delayS;
								config.Save(this);
								base.TaskHelper.Abort();
								base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
							}
						}
						Vector2 position = positionSchedule.Position;
						ImGui.TableNextColumn();
						ImGui.SetNextItemWidth(125f * Globals.GlobalUIScale);
						ImGui.InputFloat2(ImU8String.op_Implicit("##坐标"), ref position, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
						if (ImGui.IsItemDeactivatedAfterEdit())
						{
							positionSchedule.Position = position;
							config.Save(this);
							base.TaskHelper.Abort();
							base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
						}
						ImGui.SameLine();
						if (ImGuiOm.ButtonIcon("当前坐标", (FontAwesomeIcon)61531, LanguageManager.Get("AutoMovePetPosition-GetCurrent")))
						{
							IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
							if (localPlayer != null)
							{
								positionSchedule.Position = localPlayer.Position.ToVector2();
								config.Save(this);
								base.TaskHelper.Abort();
								base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
							}
						}
						ImGui.SameLine();
						if (!isPicking)
						{
							if (ImGuiOm.ButtonIcon("鼠标位置", (FontAwesomeIcon)62021, LanguageManager.Get("AutoMovePetPosition-GetMouseHelp")))
							{
								isPicking = true;
								currentPickingRow = (item, j);
							}
						}
						else if (ImGuiOm.ButtonIcon("取消鼠标位置读取", (FontAwesomeIcon)61453, LanguageManager.Get("Cancel")))
						{
							isPicking = false;
							currentPickingRow = null;
						}
						if (!isPicking || (!ImGui.IsKeyDown((ImGuiKey)529) && !ImGui.IsKeyDown((ImGuiKey)533)) || (!ImGui.IsKeyDown((ImGuiKey)527) && !ImGui.IsKeyDown((ImGuiKey)531)) || !DalamudServiceExtension.Instance<IGameGui>().ScreenToWorld(ImGui.GetMousePos(), ref vector3, 100000f))
						{
							goto IL_0855;
						}
						uint num2 = currentPickingRow?.territoryKey ?? 0;
						int num3 = currentPickingRow?.index ?? (-1);
						if (num2 == 0 || num3 == -1)
						{
							continue;
						}
						config.PositionSchedules[num2][num3].Position = vector3.ToVector2();
						config.Save(this);
						base.TaskHelper.Abort();
						base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
						isPicking = false;
						currentPickingRow = null;
						goto IL_0855;
						IL_0855:
						ImGui.TableNextColumn();
						if (ImGuiOm.ButtonIcon("删除", (FontAwesomeIcon)62189, LanguageManager.Get("Delete") + " (Ctrl)") && ImGui.IsKeyDown((ImGuiKey)527))
						{
							list2.RemoveAt(j);
							config.Save(this);
							base.TaskHelper.Abort();
							base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
							continue;
						}
						ImGui.SameLine();
						if (ImGuiOm.ButtonIcon("导出", (FontAwesomeIcon)62830, LanguageManager.Get("Export")))
						{
							ModuleBase.ExportToClipboard(positionSchedule);
						}
						ImGui.SameLine();
						if (ImGuiOm.ButtonIcon("导入", (FontAwesomeIcon)62831, LanguageManager.Get("Import")))
						{
							PositionSchedule positionSchedule3 = ModuleBase.ImportFromClipboard<PositionSchedule>();
							if (positionSchedule3 == null)
							{
								return;
							}
							uint zoneID2 = positionSchedule3.ZoneID;
							config.PositionSchedules.TryAdd(zoneID2, new List<PositionSchedule>());
							if (!config.PositionSchedules[zoneID2].Contains(positionSchedule3))
							{
								list2.Add(positionSchedule3);
								config.Save(this);
								base.TaskHelper.Abort();
								base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
							}
						}
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void OnZoneChanged(uint u)
	{
		ResetBattleTimer();
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private void OnDutyRecommenced(IDutyStateEventArgs args)
	{
		ResetBattleTimer();
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private void OnConditionChanged(ConditionFlag flag, bool value)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if (value && (int)flag == 26)
		{
			ResetBattleTimer();
			StartBattleTimer();
			base.TaskHelper.Abort();
			base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
	}

	private void StartBattleTimer()
	{
		battleStartTime = OmenServiceBase<StandardTimeManager>.Instance().Now;
	}

	private void ResetBattleTimer()
	{
		battleStartTime = DateTime.MinValue;
	}

	private void SchedulePetMovements()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckIsEightPlayerDuty())
		{
			return;
		}
		uint territoryType = GameState.TerritoryType;
		if (!config.PositionSchedules.TryGetValue(territoryType, out List<PositionSchedule> value))
		{
			return;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer != null)
		{
			if (!ValidJobs.Contains(localPlayer.ClassJob.RowId))
			{
				return;
			}
			List<PositionSchedule> source = value.Where((PositionSchedule x) => x.Enabled).ToList();
			double elapsedTimeInSeconds = (OmenServiceBase<StandardTimeManager>.Instance().Now - battleStartTime).TotalSeconds;
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
			{
				PositionSchedule bestSchedule = (from x in source
					where (double)x.DelayS <= elapsedTimeInSeconds
					orderby x.DelayS descending
					select x).FirstOrDefault();
				if (bestSchedule != null)
				{
					base.TaskHelper.Enqueue(delegate
					{
						MovePetToLocation(bestSchedule.Position);
					});
				}
			}
			else
			{
				PositionSchedule scheduleForZero = source.FirstOrDefault((PositionSchedule x) => x.DelayS == 0);
				if (scheduleForZero != null)
				{
					base.TaskHelper.Enqueue(delegate
					{
						MovePetToLocation(scheduleForZero.Position);
					});
				}
			}
		}
		base.TaskHelper.DelayNext(1000);
		base.TaskHelper.Enqueue((Action)SchedulePetMovements, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private unsafe void MovePetToLocation(Vector2 position)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckIsEightPlayerDuty())
		{
			return;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null || !ValidJobs.Contains(localPlayer.ClassJob.RowId))
		{
			return;
		}
		BattleChara* ptr = ((CharacterManager)CharacterManager.Instance()).LookupPetByOwnerObject(localPlayer.ToStruct());
		if (ptr != null)
		{
			float y = ((Vector3)(&((BattleChara)ptr).Position)).Y;
			Vector3 location = position.ToVector3(in y);
			if (RaycastHelper.TryGetGroundPosition(position.ToVector3(in y), out var groundPosition))
			{
				location = groundPosition;
			}
			base.TaskHelper.Enqueue(delegate
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.PetAction, location, 3u);
			});
		}
	}

	private static bool CheckIsEightPlayerDuty()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		uint territoryType = GameState.TerritoryType;
		if (territoryType == 0)
		{
			return false;
		}
		TerritoryType? row = LuminaGetter.GetRow<TerritoryType>(territoryType);
		if (!row.HasValue)
		{
			return false;
		}
		TerritoryType value = row.Value;
		if (((TerritoryType)(ref value)).RowId == 0)
		{
			return false;
		}
		value = row.Value;
		ContentFinderCondition value2 = ((TerritoryType)(ref value)).ContentFinderCondition.Value;
		if (((ContentFinderCondition)(ref value2)).RowId == 0)
		{
			return false;
		}
		return ((ContentFinderCondition)(ref value2)).ContentMemberType.RowId == 3;
	}
}
