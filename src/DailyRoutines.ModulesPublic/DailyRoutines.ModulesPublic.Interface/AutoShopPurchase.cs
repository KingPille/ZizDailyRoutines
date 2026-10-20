using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.NativeWrapper;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Helpers;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoShopPurchase : ModuleBase
{
	private class Config : ModuleConfig
	{
		public List<ShopPurchasePreset> Presets = new List<ShopPurchasePreset>();
	}

	public class AddonWithListInfo(string addonName, HashSet<uint> listNodeIDs) : IEquatable<AddonWithListInfo>
	{
		public string AddonName { get; } = addonName;

		public HashSet<uint> ListNodeIDs { get; } = listNodeIDs;

		public bool Equals(AddonWithListInfo? other)
		{
			if ((object)other == null)
			{
				return false;
			}
			if ((object)this == other)
			{
				return true;
			}
			return AddonName == other.AddonName;
		}

		public unsafe AtkUnitBase* GetAddon()
		{
			return AddonHelper.GetByName(AddonName);
		}

		public unsafe AtkComponentList* GetListByID(uint nodeID)
		{
			if (!ListNodeIDs.Contains(nodeID))
			{
				return null;
			}
			AtkUnitBase* addon = GetAddon();
			if (!(*addon).IsAddonAndNodesReady())
			{
				return null;
			}
			return ((AtkUnitBase)addon).GetComponentListById(nodeID);
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((AddonWithListInfo)obj);
		}

		public override int GetHashCode()
		{
			return AddonName.GetHashCode();
		}

		public static bool operator ==(AddonWithListInfo left, AddonWithListInfo right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(AddonWithListInfo left, AddonWithListInfo right)
		{
			return !object.Equals(left, right);
		}

		public unsafe static List<AddonWithListInfo> ScanAddons(AtkUnitList managerList)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			HashSet<AddonWithListInfo> hashSet = new HashSet<AddonWithListInfo>();
			Span<Pointer<AtkUnitBase>> entries = ((AtkUnitList)(ref managerList)).Entries;
			if (entries.Length == 0 || entries.IsEmpty || entries.Length == 0)
			{
				return hashSet.ToList();
			}
			Span<Pointer<AtkUnitBase>> span = entries;
			for (int i = 0; i < span.Length; i++)
			{
				Pointer<AtkUnitBase> val = span[i];
				AtkUnitBase* value = val.Value;
				if ((*value).IsAddonAndNodesReady())
				{
					AddonWithListInfo info = new AddonWithListInfo(((AtkUnitBase)value).NameString, new HashSet<uint>());
					((AtkUnitBase)value).UldManager.SearchComponentsByType((ComponentType)9).ForEach(delegate(nint x)
					{
						info.ListNodeIDs.Add(((AtkComponentNode)((AtkComponentList)x).OwnerNode).NodeId);
					});
					((AtkUnitBase)value).UldManager.SearchComponentsByType((ComponentType)12).ForEach(delegate(nint x)
					{
						info.ListNodeIDs.Add(((AtkComponentNode)((AtkComponentTreeList)x).OwnerNode).NodeId);
					});
					if (info.ListNodeIDs.Count > 0)
					{
						hashSet.Add(info);
					}
				}
			}
			return hashSet.ToList();
		}
	}

	public class ShopPurchasePreset(string addonName) : IEquatable<ShopPurchasePreset>
	{
		public string Name { get; set; } = string.Empty;

		public string AddonName { get; set; } = addonName;

		public string TargetName { get; set; } = string.Empty;

		public KeyValuePair<uint, int> ClickRoute { get; set; }

		public KeyValuePair<bool, int> NumberRoute { get; set; }

		public bool Equals(ShopPurchasePreset? other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			if (AddonName == other.AddonName && ClickRoute.Equals(other.ClickRoute))
			{
				return NumberRoute.Equals(other.NumberRoute);
			}
			return false;
		}

		public unsafe AtkUnitBase* GetAddon()
		{
			return AddonHelper.GetByName(AddonName);
		}

		public unsafe bool IsAddonValid()
		{
			return (*GetAddon()).IsAddonAndNodesReady();
		}

		public unsafe AtkComponentList* GetListNode()
		{
			if (IsAddonValid())
			{
				return ((AtkUnitBase)GetAddon()).GetComponentListById(ClickRoute.Key);
			}
			return null;
		}

		public unsafe AtkComponentNumericInput* GetNumberNode()
		{
			if (!NumberRoute.Key)
			{
				return null;
			}
			AtkComponentList* listNode = GetListNode();
			if (listNode == null || ((AtkComponentList)listNode).ListLength < ClickRoute.Value)
			{
				return null;
			}
			return ((AtkComponentListItemRenderer)((ListItem)((byte*)((AtkComponentList)listNode).ItemRendererList + (nint)ClickRoute.Value * (nint)Unsafe.SizeOf<ListItem>())).AtkComponentListItemRenderer).UldManager.SearchComponentByType<AtkComponentNumericInput>((ComponentType)8);
		}

		public unsafe bool IsNodeValid()
		{
			if (GetListNode() != null)
			{
				if (NumberRoute.Key)
				{
					if (NumberRoute.Key)
					{
						return GetNumberNode() != null;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public bool IsTargetValid()
		{
			if (!string.IsNullOrWhiteSpace(TargetName))
			{
				if (!string.IsNullOrWhiteSpace(TargetName))
				{
					return (TargetManager.Target?.Name ?? string.Empty) == TargetName;
				}
				return false;
			}
			return true;
		}

		public unsafe List<Func<bool>> GetTasks()
		{
			try
			{
				List<Func<bool>> list = new List<Func<bool>>();
				if (!IsTargetValid())
				{
					throw new Exception(LanguageManager.Get("AutoShopPurchase-Exception-PresetTargetInvalid"));
				}
				if (!IsAddonValid())
				{
					throw new Exception(LanguageManager.Get("AutoShopPurchase-Exception-PresetAddonInvalid"));
				}
				if (!IsNodeValid())
				{
					throw new Exception(LanguageManager.Get("AutoShopPurchase-Exception-PresetNodeInvalid"));
				}
				if (NumberRoute.Key)
				{
					list.Add(delegate
					{
						AtkComponentNumericInput* numberNode = GetNumberNode();
						if (numberNode == null)
						{
							return false;
						}
						((AtkComponentNumericInput)numberNode).SetValue(NumberRoute.Value);
						return true;
					});
				}
				list.Add(delegate
				{
					AtkComponentList* listNode = GetListNode();
					if (listNode == null)
					{
						return false;
					}
					((AtkComponentList)listNode).DispatchItemEvent(ClickRoute.Value, (AtkEventType)35);
					return true;
				});
				return list;
			}
			catch (Exception ex)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get("Error") + ": " + ex.Message);
				return new List<Func<bool>>();
			}
		}

		public override bool Equals(object? obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((ShopPurchasePreset)obj);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(AddonName, ClickRoute, NumberRoute);
		}

		public override string ToString()
		{
			return $"{Name}_{AddonName}_Click:{ClickRoute.Key}-{ClickRoute.Value}_Number:{NumberRoute.Key}-{NumberRoute.Value}";
		}
	}

	private class ShopPresetDisplayTable : IDisposable
	{
		private class PresetRunTimesInputComponent(ShopPurchasePreset preset)
		{
			private static readonly Dictionary<ShopPurchasePreset, PresetRunTimesInputComponent> Cache = new Dictionary<ShopPurchasePreset, PresetRunTimesInputComponent>();

			private int timesInput = 1;

			public static PresetRunTimesInputComponent Using(ShopPurchasePreset preset)
			{
				if (Cache.TryGetValue(preset, out PresetRunTimesInputComponent value))
				{
					return value;
				}
				value = new PresetRunTimesInputComponent(preset);
				Cache[preset] = value;
				return value;
			}

			public void Draw()
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
				DisabledDisposable val = ImRaii.Disabled(ShopPresetExecutor.IsRunning);
				try
				{
					GroupDisposable val2 = ImRaii.Group();
					try
					{
						if (ImGuiOm.ButtonIcon($"RunPreset_{preset}", (FontAwesomeIcon)61515, LanguageManager.Get("Run")))
						{
							DalamudServiceExtension.Instance<IFramework>().RunOnTick<bool>((Func<Task<bool>>)(async () => await ShopPresetExecutor.TryExecuteAsync(preset, timesInput).ConfigureAwait(continueOnCapturedContext: false)), default(TimeSpan), 0, default(CancellationToken));
						}
						ImGui.SameLine(0f, 2f * Globals.GlobalUIScale);
						ImGui.SetNextItemWidth(50f * Globals.GlobalUIScale);
						ImU8String val3 = new ImU8String(22, 1);
						((ImU8String)(ref val3)).AppendLiteral("###RunPresetTimesInput");
						((ImU8String)(ref val3)).AppendFormatted<ShopPurchasePreset>(preset);
						if (ImGui.InputInt(val3, ref timesInput, 0, 0, default(ImU8String), (ImGuiInputTextFlags)0))
						{
							timesInput = Math.Max(1, timesInput);
						}
					}
					finally
					{
						((GroupDisposable)(ref val2)).Dispose();
					}
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}

		private readonly AutoShopPurchase module;

		private string nameInput = string.Empty;

		private string targetNameInput = LanguageManager.Get("AutoShopPurchase-UI-UnknownTarget");

		private string addonNameInput = string.Empty;

		private uint listComponentNodeIDInput;

		private int clickIndexInput;

		private bool isSetNumberInput;

		private int setNumberInput;

		private bool isAddNewPresetWindowOpen;

		private unsafe static AtkUnitList FocusedList => ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).FocusedUnitsList;

		public ShopPresetDisplayTable(AutoShopPurchase module)
		{
			this.module = module;
			OmenServiceBase<WindowManager>.Instance().PostDraw += WindowRenderAddNewPreset;
		}

		public void Dispose()
		{
			OmenServiceBase<WindowManager>.Instance().PostDraw -= WindowRenderAddNewPreset;
		}

		public void Draw()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
			contentRegionAvail.Y = 0f;
			Vector2 vector = contentRegionAvail;
			TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("ShopPresetDisplayTable"), 7, (ImGuiTableFlags)1920, vector);
			try
			{
				if (!(!val))
				{
					TableSetupColumns();
					TableRenderHeaderRow();
					for (int i = 0; i < module.config.Presets.Count; i++)
					{
						TableRenderRow(i, module.config.Presets[i]);
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val)).Dispose();
			}
		}

		private static void TableSetupColumns()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			ImGui.TableSetupColumn(ImU8String.op_Implicit("序号"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("1234"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("名称"), (ImGuiTableColumnFlags)0, 25f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("对象"), (ImGuiTableColumnFlags)0, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("界面"), (ImGuiTableColumnFlags)0, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("路径"), (ImGuiTableColumnFlags)0, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("数量"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("1234"), false, -1f).X, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit("操作"), (ImGuiTableColumnFlags)0, 30f, 0u);
		}

		private void TableRenderHeaderRow()
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			if (ImGuiOm.ButtonIconSelectable("OpenAddNewPresetWindow", (FontAwesomeIcon)61543))
			{
				module.scannedData = AddonWithListInfo.ScanAddons(FocusedList);
				isAddNewPresetWindowOpen = !isAddNewPresetWindowOpen;
			}
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Name")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Target")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Addon")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Route")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Amount")));
			ImGui.TableNextColumn();
		}

		private void TableRenderRow(int counter, ShopPurchasePreset preset)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(preset.ToString()), true);
			try
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImU8String val2 = new ImU8String(0, 1);
				((ImU8String)(ref val2)).AppendFormatted<int>(counter + 1);
				ImGui.TextUnformatted(val2);
				ImGui.TableNextColumn();
				ImU8String val3 = new ImU8String(0, 1);
				((ImU8String)(ref val3)).AppendFormatted<string>(preset.Name);
				ImGui.TextUnformatted(val3);
				ImGui.TableNextColumn();
				ImU8String val4 = new ImU8String(0, 1);
				((ImU8String)(ref val4)).AppendFormatted<string>(preset.TargetName);
				ImGui.TextUnformatted(val4);
				ImGui.TableNextColumn();
				ImU8String val5 = new ImU8String(0, 1);
				((ImU8String)(ref val5)).AppendFormatted<string>(preset.AddonName);
				ImGui.TextUnformatted(val5);
				ImGui.TableNextColumn();
				ImU8String val6 = new ImU8String(4, 2);
				((ImU8String)(ref val6)).AppendFormatted<uint>(preset.ClickRoute.Key);
				((ImU8String)(ref val6)).AppendLiteral(" -> ");
				((ImU8String)(ref val6)).AppendFormatted<int>(preset.ClickRoute.Value);
				ImGui.TextUnformatted(val6);
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(preset.NumberRoute.Key ? $"{preset.NumberRoute.Value}" : ("(" + LanguageManager.Get("None") + ")")));
				ImGui.TableNextColumn();
				PresetRunTimesInputComponent.Using(preset).Draw();
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon($"Pause_{preset}", (FontAwesomeIcon)61517, LanguageManager.Get("Stop")))
				{
					ShopPresetExecutor.CancelAndDispose();
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon($"Export_{preset}", (FontAwesomeIcon)62830, LanguageManager.Get("Export")))
				{
					ModuleBase.ExportToClipboard(preset);
				}
				ImGui.SameLine();
				if (ImGuiOm.ButtonIcon($"DeletePreset_{preset}", (FontAwesomeIcon)61944, LanguageManager.Get("Delete") + " (Ctrl)") && ImGui.IsKeyDown((ImGuiKey)527))
				{
					module.config.Presets.Remove(preset);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		private void WindowRenderAddNewPreset()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			if (!isAddNewPresetWindowOpen)
			{
				return;
			}
			RefreshWindowInfo();
			using (OmenServiceBase<FontManager>.Instance().UIFont.Push())
			{
				ImU8String val = new ImU8String(32, 1);
				((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("AutoShopPurchase-UI-AddNewPreset"));
				((ImU8String)(ref val)).AppendLiteral("###AutoShopPurchase-AddNewPreset");
				if (!ImGui.Begin(val, ref isAddNewPresetWindowOpen, (ImGuiWindowFlags)0))
				{
					return;
				}
				GroupDisposable val2 = ImRaii.Group();
				try
				{
					WindowRenderPresetInfoInput();
				}
				finally
				{
					((GroupDisposable)(ref val2)).Dispose();
				}
				ImGui.SameLine();
				ImGuiOm.ScaledDummy(8f);
				ImGui.SameLine();
				GroupDisposable val3 = ImRaii.Group();
				try
				{
					foreach (AddonWithListInfo item in module.scannedData.ToList())
					{
						WindowRenderAddonInfo(item);
					}
				}
				finally
				{
					((GroupDisposable)(ref val3)).Dispose();
				}
				ImGui.End();
			}
		}

		private void RefreshWindowInfo()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (!Throttler.Shared.Throttle("AutoShopPurchase-RefreshFocusedAddonsInfo", 2000u))
			{
				return;
			}
			AddonWithListInfo.ScanAddons(FocusedList).ForEach(delegate(AddonWithListInfo x)
			{
				if (!module.scannedData.Contains(x))
				{
					module.scannedData.Add(x);
				}
			});
			if (!string.IsNullOrWhiteSpace(targetNameInput))
			{
				IGameObject target = TargetManager.Target;
				if (target != null)
				{
					targetNameInput = target.Name;
				}
			}
		}

		private void WindowRenderPresetInfoInput()
		{
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			ImGuiOm.CompLabelLeft(LanguageManager.Get("Name") + ":", 200f * Globals.GlobalUIScale, () => ImGui.InputText(ImU8String.op_Implicit("###NameInput"), ref nameInput, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null));
			ImGuiOm.CompLabelLeft(LanguageManager.Get("Target") + ":", 200f * Globals.GlobalUIScale, () => ImGui.InputText(ImU8String.op_Implicit("###TargetNameInput"), ref targetNameInput, 256, (ImGuiInputTextFlags)16384, (ImGuiInputTextCallbackDelegate)null));
			ImGuiOm.TooltipHover(LanguageManager.Get("AutoShopPurchase-Tooltip-EmptyTargetInput"), 30f);
			ImGuiOm.CompLabelLeft(LanguageManager.Get("Addon") + ":", 200f * Globals.GlobalUIScale, () => ImGui.InputText(ImU8String.op_Implicit("###AddonNameInput"), ref addonNameInput, 128, (ImGuiInputTextFlags)16384, (ImGuiInputTextCallbackDelegate)null));
			ImGuiOm.CompLabelLeft(LanguageManager.Get("List") + ":", 200f * Globals.GlobalUIScale, () => ImGui.InputUInt(ImU8String.op_Implicit("###ListComponentNodeIDInput"), ref listComponentNodeIDInput, 0u, 0u, default(ImU8String), (ImGuiInputTextFlags)16384));
			ImGuiOm.CompLabelLeft(LanguageManager.Get("Click") + ":", 200f * Globals.GlobalUIScale, delegate
			{
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				bool num = ImGui.InputInt(ImU8String.op_Implicit("###ClickIndexInput"), ref clickIndexInput, 0, 0, default(ImU8String), (ImGuiInputTextFlags)16384);
				if (num)
				{
					clickIndexInput = Math.Max(1, clickIndexInput);
				}
				return num;
			});
			ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoShopPurchase-UI-IsSetNumber")), ref isSetNumberInput);
			if (isSetNumberInput)
			{
				ImGuiOm.CompLabelLeft(LanguageManager.Get("Number") + ":", 200f * Globals.GlobalUIScale, delegate
				{
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001b: Unknown result type (might be due to invalid IL or missing references)
					bool num = ImGui.InputInt(ImU8String.op_Implicit("###SetNumberInput"), ref setNumberInput, 0, 100, default(ImU8String), (ImGuiInputTextFlags)0);
					if (num)
					{
						setNumberInput = Math.Max(1, setNumberInput);
					}
					return num;
				});
			}
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")) && !string.IsNullOrWhiteSpace(addonNameInput) && !string.IsNullOrWhiteSpace(nameInput))
			{
				ShopPurchasePreset item = new ShopPurchasePreset(addonNameInput)
				{
					ClickRoute = new KeyValuePair<uint, int>(listComponentNodeIDInput, clickIndexInput),
					NumberRoute = new KeyValuePair<bool, int>(isSetNumberInput, setNumberInput),
					Name = nameInput,
					TargetName = targetNameInput
				};
				if (!module.config.Presets.Contains(item))
				{
					module.config.Presets.Add(item);
					OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("AutoShopPurchase-Tooltip-AddPresetSuccess", nameInput));
				}
			}
		}

		private unsafe void WindowRenderAddonInfo(AddonWithListInfo data)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (!(*data.GetAddon()).IsAddonAndNodesReady())
			{
				module.scannedData.Remove(data);
			}
			else
			{
				if (!ImGui.CollapsingHeader(ImU8String.op_Implicit(LanguageManager.Get("AutoShopPurchase-UI-AddonInfoHeader", data.AddonName, data.ListNodeIDs.Count)), (ImGuiTreeNodeFlags)32))
				{
					return;
				}
				foreach (uint listNodeID in data.ListNodeIDs)
				{
					WindowRenderListComponentNodeInfo(data, listNodeID);
				}
			}
		}

		private unsafe void WindowRenderListComponentNodeInfo(AddonWithListInfo data, uint nodeID)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			AtkComponentList* listByID = data.GetListByID(nodeID);
			if (listByID == null)
			{
				return;
			}
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(1, 2);
			((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("List"));
			((ImU8String)(ref val)).AppendLiteral(" ");
			((ImU8String)(ref val)).AppendFormatted<uint>(nodeID);
			TreeNodeDisposable val2 = ImRaii.TreeNode(val, (ImGuiTreeNodeFlags)32);
			try
			{
				if (ImGui.IsItemHovered())
				{
					(*(AtkResNode*)((AtkComponentList)listByID).OwnerNode).OutlineNode();
				}
				if (TreeNodeDisposable.op_Implicit(val2))
				{
					for (int i = 0; i < ((AtkComponentList)listByID).ListLength; i++)
					{
						if (i % 11 != 0)
						{
							ImGui.SameLine();
						}
						ImU8String val3 = new ImU8String(3, 3);
						((ImU8String)(ref val3)).AppendFormatted<int>(i, "D3");
						((ImU8String)(ref val3)).AppendLiteral("##");
						((ImU8String)(ref val3)).AppendFormatted<string>(data.AddonName);
						((ImU8String)(ref val3)).AppendLiteral("-");
						((ImU8String)(ref val3)).AppendFormatted<uint>(nodeID);
						if (ImGui.Button(val3, default(Vector2)))
						{
							addonNameInput = data.AddonName;
							listComponentNodeIDInput = nodeID;
							clickIndexInput = i;
							((AtkComponentList)listByID).DispatchItemEvent(i, (AtkEventType)35);
						}
						if (((AtkComponentList)listByID).ItemRendererList == null)
						{
							continue;
						}
						bool num = ImGui.IsItemHovered();
						ListItem val4 = ((ListItem*)((AtkComponentList)listByID).ItemRendererList)[i];
						if (num)
						{
							if (!val4.IsHighlighted)
							{
								((AtkComponentList)listByID).SetItemHighlightedState(i, true, true);
							}
						}
						else if (val4.IsHighlighted)
						{
							((AtkComponentList)listByID).SetItemHighlightedState(i, false, true);
						}
					}
				}
			}
			finally
			{
				((TreeNodeDisposable)(ref val2)).Dispose();
			}
			if (!((AtkComponentNode)((AtkComponentList)listByID).OwnerNode).IsVisible())
			{
				ImGui.SameLine();
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoShopPurchase-UI-InvisibleList")));
			}
		}
	}

	public class ShopPresetExecutor : IDisposable
	{
		private static ShopPresetExecutor? Instance;

		private static readonly Lock Lock = new Lock();

		private readonly CancellationTokenSource CancelSource = new CancellationTokenSource();

		private readonly TaskCompletionSource<bool> CompletionSource = new TaskCompletionSource<bool>();

		private int currentLoopCount;

		private bool IsWaitingRefresh;

		private TaskHelper TaskHelper { get; init; } = new TaskHelper
		{
			TimeoutMS = 10000
		};

		private ShopPurchasePreset Preset { get; init; }

		private int LoopCount { get; init; }

		public static bool IsRunning => Instance != null;

		public static string CurrentPresetName => Instance?.Preset.Name ?? string.Empty;

		private ShopPresetExecutor(ShopPurchasePreset preset, int loopCount)
		{
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			Preset = preset;
			LoopCount = loopCount;
			DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, (IEnumerable<string>)new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { "SelectYesno", "ShopExchangeItemDialog" }), new AddonEventDelegate(OnAddonYesno));
			OmenServiceBase<ExecuteCommandManager>.Instance().RegPost(OnReceiveCommand);
		}

		public void Dispose()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			OmenServiceBase<ExecuteCommandManager>.Instance().Unreg(OnReceiveCommand);
			DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonYesno });
			TaskHelper.Dispose();
			IsWaitingRefresh = false;
			CancelSource.Cancel();
			CancelSource.Dispose();
		}

		public static async Task<bool> TryExecuteAsync(ShopPurchasePreset preset, int loopCount)
		{
			using (Lock.EnterScope())
			{
				if (Instance != null)
				{
					return false;
				}
				Instance = new ShopPresetExecutor(preset, loopCount);
			}
			try
			{
				await Instance.ExecuteAsync();
				return true;
			}
			catch (OperationCanceledException)
			{
				return false;
			}
			finally
			{
				using (Lock.EnterScope())
				{
					Instance?.Dispose();
					Instance = null;
				}
			}
		}

		public static bool CancelAndDispose()
		{
			using (Lock.EnterScope())
			{
				if (Instance == null)
				{
					return false;
				}
				Instance.CancelSource.Cancel();
				Instance.Dispose();
				Instance = null;
				return true;
			}
		}

		private async Task ExecuteAsync()
		{
			try
			{
				Execute();
				await CompletionSource.Task;
			}
			catch (OperationCanceledException)
			{
				CompletionSource.TrySetCanceled();
				throw;
			}
		}

		private unsafe void Execute()
		{
			CancelSource.Token.ThrowIfCancellationRequested();
			List<Func<bool>> tasks = Preset.GetTasks();
			if (currentLoopCount == LoopCount || tasks.Count <= 0)
			{
				CompletionSource.TrySetResult(result: true);
				return;
			}
			tasks.ForEach(delegate(Func<bool> x)
			{
				TaskHelper.Enqueue(delegate
				{
					CancelSource.Token.ThrowIfCancellationRequested();
					if ((*Addons.Request).IsAddonAndNodesReady())
					{
						return false;
					}
					IsWaitingRefresh = true;
					x();
					return true;
				}, null, 0, null, null, null, null, 2);
			});
			TaskHelper.DelayNext(1000, "防止卡住", 1);
			TaskHelper.Enqueue(delegate
			{
				OnReceiveCommand(ExecuteCommandFlag.RefreshInventory, 0u, 0u, 0u, 0u);
			});
		}

		private unsafe void OnAddonYesno(AddonEvent type, AddonArgs args)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			if ((TaskHelper.IsBusy || IsWaitingRefresh) && !(args.Addon == AtkUnitBasePtr.op_Implicit((IntPtr)IntPtr.Zero)))
			{
				(*args.Addon.ToStruct()).Callback(0);
			}
		}

		private void OnReceiveCommand(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4)
		{
			if (IsWaitingRefresh && command == ExecuteCommandFlag.RefreshInventory)
			{
				IsWaitingRefresh = false;
				TaskHelper.RemoveQueue(1);
				TaskHelper.Enqueue(delegate
				{
					currentLoopCount++;
					Execute();
				}, null, 0, null, null, null, null, 2);
			}
		}
	}

	private Config config;

	private ShopPresetDisplayTable? presetDisplayTable;

	private List<AddonWithListInfo> scannedData = new List<AddonWithListInfo>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoShopPurchaseTitle"),
		Description = LanguageManager.Get("AutoShopPurchaseDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (presetDisplayTable == null)
		{
			presetDisplayTable = new ShopPresetDisplayTable(this);
		}
	}

	protected override void Uninit()
	{
		config?.Save(this);
		presetDisplayTable?.Dispose();
		presetDisplayTable = null;
		ShopPresetExecutor.CancelAndDispose();
	}

	protected override void ConfigUI()
	{
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62831, LanguageManager.Get("Import")))
		{
			ShopPurchasePreset shopPurchasePreset = ModuleBase.ImportFromClipboard<ShopPurchasePreset>();
			if (shopPurchasePreset != null)
			{
				config.Presets.Add(shopPurchasePreset);
				config.Save(this);
			}
		}
		presetDisplayTable.Draw();
	}
}
