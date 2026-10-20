using System;
using System.CodeDom.Compiler;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.Interop;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic.Interface;

public class FastObjectInteract : ModuleBase
{
	private sealed class Config : ModuleConfig
	{
		public bool AllowClickToTarget;

		public HashSet<string> BlacklistKeys = new HashSet<string>();

		public float FontScale = 0.6f;

		public bool LockWindow;

		public float MaxButtonWidth = 400f;

		public int MaxDisplayAmount = 5;

		public float MinButtonWidth = 300f;

		public bool OnlyDisplayInViewRange;

		public HashSet<ObjectKind> SelectedKinds = new HashSet<ObjectKind>();

		public bool WindowInvisibleWhenCombat = true;

		public bool WindowInvisibleWhenInteract = true;
	}

	private unsafe readonly struct InteractableObject(GameObject* ptr, string name, ObjectKind kind, float distSq)
	{
		public unsafe readonly GameObject* Pointer = ptr;

		public readonly string Name = name;

		public readonly ObjectKind Kind = kind;

		public readonly float DistanceSq = distSq;

		public unsafe nint ID => (nint)Pointer;
	}

	private Config config;

	private Dictionary<uint, string> dcWorlds = new Dictionary<uint, string>();

	private string blacklistKeyInput = string.Empty;

	private float windowWidth;

	private bool isOnWorldTraveling;

	private readonly List<InteractableObject> currentObjects = new List<InteractableObject>(20);

	private readonly List<AetheryteRecord> currentAethernetShards = new List<AetheryteRecord>();

	private bool forceObjectUpdate;

	private const string ENPC_TITLE_FORMAT = "[{0}] {1}";

	private static readonly FrozenSet<uint> WorldTravelValidZones = FrozenSet.Create<uint>(132u, 129u, 130u);

	private static readonly FrozenDictionary<ObjectKind, float> IncludeDistance = new Dictionary<ObjectKind, float>
	{
		[(ObjectKind)5] = 400f,
		[(ObjectKind)6] = 100f,
		[(ObjectKind)16] = 150f,
		[(ObjectKind)7] = 100f,
		[(ObjectKind)12] = 30f,
		[(ObjectKind)4] = 100f
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FastObjectInteractTitle"),
		Description = LanguageManager.Get("FastObjectInteractDescription"),
		Category = ModuleCategory.Interface,
		ModulesPrerequisite = new string[2] { "FastWorldTravel", "FastInstanceZoneChange" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	private static FrozenDictionary<uint, string> ENPCTitle { get; } = ((IEnumerable<ENpcResident>)LuminaGetter.Get<ENpcResident>()).Where((ENpcResident x) => ((ENpcResident)(ref x)).Unknown1 && !string.IsNullOrWhiteSpace(((object)((ENpcResident)(ref x)).Title/*cast due to constrained. prefix*/).ToString())).ToDictionary((ENpcResident x) => ((ENpcResident)(ref x)).RowId, (ENpcResident x) => ((object)((ENpcResident)(ref x)).Title/*cast due to constrained. prefix*/).ToString()).ToFrozenDictionary();

	private static FrozenSet<uint> ImportantENPC { get; } = (from x in (IEnumerable<ENpcResident>)LuminaGetter.Get<ENpcResident>()
		where ((ENpcResident)(ref x)).Unknown1
		select ((ENpcResident)(ref x)).RowId).ToFrozenSet();

	protected override void Init()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config
		{
			SelectedKinds = new HashSet<ObjectKind>
			{
				(ObjectKind)3,
				(ObjectKind)7,
				(ObjectKind)4,
				(ObjectKind)5,
				(ObjectKind)6
			}
		};
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000,
				ShowDebug = true
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		Overlay overlay = new Overlay(this, "##FastObjectInteract");
		((Window)overlay).Flags = (ImGuiWindowFlags)2109547;
		base.Overlay = overlay;
		UpdateWindowFlags();
		DalamudServiceExtension.Instance<IClientState>().Login += OnLogin;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnTerritoryChanged;
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 250u);
		LoadWorldData();
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		DalamudServiceExtension.Instance<IClientState>().Login -= OnLogin;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnTerritoryChanged;
		currentObjects.Clear();
	}

	protected unsafe override void ConfigUI()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		ItemWidthDisposable val = ImRaii.ItemWidth(300f * Globals.GlobalUIScale);
		try
		{
			flag |= ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastObjectInteract-WindowInvisibleWhenInteract")), ref config.WindowInvisibleWhenInteract);
			flag |= ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastObjectInteract-WindowInvisibleWhenCombat")), ref config.WindowInvisibleWhenCombat);
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastObjectInteract-LockWindow")), ref config.LockWindow))
			{
				flag = true;
				UpdateWindowFlags();
			}
			if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastObjectInteract-OnlyDisplayInViewRange")), ref config.OnlyDisplayInViewRange))
			{
				flag = true;
				forceObjectUpdate = true;
			}
			flag |= ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("FastObjectInteract-AllowClickToTarget")), ref config.AllowClickToTarget);
			ImGui.NewLine();
			ImU8String val2 = new ImU8String(16, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("FontScale"));
			((ImU8String)(ref val2)).AppendLiteral("##FontScaleInput");
			ImGui.InputFloat(val2, ref config.FontScale, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				flag = true;
				config.FontScale = Math.Max(0.1f, config.FontScale);
			}
			ImU8String val3 = new ImU8String(21, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("FastObjectInteract-MinButtonWidth"));
			((ImU8String)(ref val3)).AppendLiteral("##MinButtonWidthInput");
			ImGui.InputFloat(val3, ref config.MinButtonWidth, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				flag = true;
				ValidateButtonWidthSettings();
			}
			ImU8String val4 = new ImU8String(21, 1);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("FastObjectInteract-MaxButtonWidth"));
			((ImU8String)(ref val4)).AppendLiteral("##MaxButtonWidthInput");
			ImGui.InputFloat(val4, ref config.MaxButtonWidth, 0f, 0f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				flag = true;
				ValidateButtonWidthSettings();
			}
			ImU8String val5 = new ImU8String(23, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("FastObjectInteract-MaxDisplayAmount"));
			((ImU8String)(ref val5)).AppendLiteral("##MaxDisplayAmountInput");
			ImU8String val6 = val5;
			ref int maxDisplayAmount = ref config.MaxDisplayAmount;
			ImU8String val7 = default(ImU8String);
			ImGui.InputInt(val6, ref maxDisplayAmount, 0, 0, val7, (ImGuiInputTextFlags)0);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				flag = true;
				config.MaxDisplayAmount = Math.Max(1, config.MaxDisplayAmount);
			}
			val7 = new ImU8String(22, 1);
			((ImU8String)(ref val7)).AppendFormatted<string>(LanguageManager.Get("FastObjectInteract-SelectedObjectKinds"));
			((ImU8String)(ref val7)).AppendLiteral("##ObjectKindsSelection");
			ComboDisposable val8 = ImRaii.Combo(val7, ImU8String.op_Implicit(LanguageManager.Get("FastObjectInteract-SelectedObjectKindsAmount", config.SelectedKinds.Count)), (ImGuiComboFlags)8);
			try
			{
				if (ComboDisposable.op_Implicit(val8))
				{
					ObjectKind[] values = Enum.GetValues<ObjectKind>();
					for (int i = 0; i < values.Length; i++)
					{
						ObjectKind item = values[i];
						bool flag2 = config.SelectedKinds.Contains(item);
						if (ImGui.Checkbox(ImU8String.op_Implicit(((object)(*(ObjectKind*)(&item))/*cast due to constrained. prefix*/).ToString()), ref flag2))
						{
							flag = true;
							if (flag2)
							{
								config.SelectedKinds.Add(item);
							}
							else
							{
								config.SelectedKinds.Remove(item);
							}
							forceObjectUpdate = true;
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val8)).Dispose();
			}
			ImU8String val9 = new ImU8String(27, 1);
			((ImU8String)(ref val9)).AppendFormatted<string>(LanguageManager.Get("FastObjectInteract-BlacklistKeysList"));
			((ImU8String)(ref val9)).AppendLiteral("##BlacklistObjectsSelection");
			ComboDisposable val10 = ImRaii.Combo(val9, ImU8String.op_Implicit(LanguageManager.Get("FastObjectInteract-BlacklistKeysListAmount", config.BlacklistKeys.Count)), (ImGuiComboFlags)8);
			try
			{
				if (ComboDisposable.op_Implicit(val10))
				{
					if (ImGuiOm.ButtonIcon("###BlacklistKeyInputAdd", (FontAwesomeIcon)61543, LanguageManager.Get("Add")) && config.BlacklistKeys.Add(blacklistKeyInput))
					{
						config.Save(this);
						forceObjectUpdate = true;
					}
					ImGui.SameLine();
					ImGui.SetNextItemWidth(-1f);
					ImU8String val11 = ImU8String.op_Implicit("###BlacklistKeyInput");
					ImU8String val12 = new ImU8String(0, 1);
					((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("FastObjectInteract-BlacklistKeysListInputHelp"));
					ImGui.InputTextWithHint(val11, val12, ref blacklistKeyInput, 100, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					ImGui.Spacing();
					ImGui.Separator();
					ImGui.Spacing();
					List<string> list = new List<string>();
					foreach (string item2 in config.BlacklistKeys.ToList())
					{
						if (ImGuiOm.ButtonIcon(item2, (FontAwesomeIcon)62189, LanguageManager.Get("Delete")))
						{
							flag = true;
							list.Add(item2);
							forceObjectUpdate = true;
						}
						ImGui.SameLine();
						ImGui.TextUnformatted(ImU8String.op_Implicit(item2));
					}
					if (list.Count > 0)
					{
						config.BlacklistKeys.RemoveRange(list);
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val10)).Dispose();
			}
			if (flag)
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	protected override void OverlayUI()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().GetUIFont(config.FontScale).Push())
		{
			InteractableObject? instanceChangeObject;
			InteractableObject? worldTravelObject;
			uint currentAetheryteID;
			bool flag = RenderObjectButtons(out instanceChangeObject, out worldTravelObject, out currentAetheryteID);
			if (instanceChangeObject.HasValue || worldTravelObject.HasValue || (currentAetheryteID != 0 && currentAethernetShards.Count > 0))
			{
				if (flag)
				{
					ImGui.SameLine();
				}
				GroupDisposable val = ImRaii.Group();
				try
				{
					if (instanceChangeObject.HasValue)
					{
						RenderInstanceZoneChangeButtons();
					}
					if (worldTravelObject.HasValue)
					{
						RenderWorldChangeButtons();
					}
					if (currentAetheryteID != 0 && currentAethernetShards.Count > 0)
					{
						RenderAethernetShardButtons(currentAetheryteID);
					}
				}
				finally
				{
					((GroupDisposable)(ref val)).Dispose();
				}
			}
			windowWidth = Math.Clamp(ImGui.GetItemRectSize().X, config.MinButtonWidth, config.MaxButtonWidth);
		}
	}

	private void ValidateButtonWidthSettings()
	{
		if (config.MinButtonWidth >= config.MaxButtonWidth)
		{
			config.MinButtonWidth = 300f;
			config.MaxButtonWidth = 350f;
		}
		config.MinButtonWidth = Math.Max(1f, config.MinButtonWidth);
		config.MaxButtonWidth = Math.Max(1f, config.MaxButtonWidth);
	}

	private unsafe bool RenderObjectButtons(out InteractableObject? instanceChangeObject, out InteractableObject? worldTravelObject, out uint currentAetheryteID)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		instanceChangeObject = null;
		worldTravelObject = null;
		currentAetheryteID = 0u;
		GroupDisposable val = ImRaii.Group();
		try
		{
			foreach (InteractableObject currentObject in currentObjects)
			{
				if (currentObject.Pointer == null)
				{
					continue;
				}
				if ((int)currentObject.Kind == 5)
				{
					currentAetheryteID = ((GameObject)currentObject.Pointer).BaseId;
					Aetheryte? row = LuminaGetter.GetRow<Aetheryte>(((GameObject)currentObject.Pointer).BaseId);
					if (!row.HasValue)
					{
						continue;
					}
					Aetheryte valueOrDefault = row.GetValueOrDefault();
					if (!((Aetheryte)(ref valueOrDefault)).IsAetheryte)
					{
						continue;
					}
					if (InstancesManager.IsInstancedArea)
					{
						instanceChangeObject = currentObject;
					}
					if (!isOnWorldTraveling && WorldTravelValidZones.Contains(GameState.TerritoryType))
					{
						worldTravelObject = currentObject;
					}
				}
				result = true;
				RenderSingleObjectButton(currentObject);
			}
			return result;
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void RenderSingleObjectButton(InteractableObject obj)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (*obj.Pointer).IsReachable();
		bool allowClickToTarget = config.AllowClickToTarget;
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)0, 0.5f, !flag);
		try
		{
			if (allowClickToTarget)
			{
				ImGuiStylePtr style = ImGui.GetStyle();
				ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)23, ((ImGuiStylePtr)(ref style)).Colors[26], !flag);
				try
				{
					style = ImGui.GetStyle();
					ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)22, ((ImGuiStylePtr)(ref style)).Colors[25], !flag);
					try
					{
						if (ButtonCenterText(((IntPtr)obj.ID).ToString(), obj.Name))
						{
							if (flag)
							{
								InteractWithObject(obj.Pointer, obj.Kind);
							}
							else
							{
								((TargetSystem)TargetSystem.Instance()).Target = obj.Pointer;
							}
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
			else
			{
				DisabledDisposable val4 = ImRaii.Disabled(!flag);
				try
				{
					if (ButtonCenterText(((IntPtr)obj.ID).ToString(), obj.Name) && flag)
					{
						InteractWithObject(obj.Pointer, obj.Kind);
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
			ImU8String val5 = new ImU8String(1, 2);
			((ImU8String)(ref val5)).AppendFormatted<nint>(obj.ID);
			((ImU8String)(ref val5)).AppendLiteral("_");
			((ImU8String)(ref val5)).AppendFormatted<string>(obj.Name);
			PopupDisposable val6 = ImRaii.ContextPopupItem(val5);
			try
			{
				if (!(!val6) && ImGui.MenuItem(ImU8String.op_Implicit(LanguageManager.Get("FastObjectInteract-AddToBlacklist")), false, true))
				{
					string item = FastObjectInteractTitleRegex().Replace(obj.Name, string.Empty).Trim();
					if (config.BlacklistKeys.Add(item))
					{
						config.Save(this);
						forceObjectUpdate = true;
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val6)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void RenderInstanceZoneChangeButtons()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		float cursorPosX = ImGui.GetCursorPosX();
		if (ImGui.CollapsingHeader(ImU8String.op_Implicit(LanguageManager.Get("InstancedArea")), (ImGuiTreeNodeFlags)0))
		{
			for (int i = 1; i <= OmenServiceBase<InstancesManager>.Instance().GetInstancesCount(); i++)
			{
				if (i != InstancesManager.CurrentInstance && ButtonCenterText($"InstanceChangeWidget_{i}", LanguageManager.Get("FastObjectInteract-InstanceAreaChange", i)))
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage($"/pdr insc {i}");
				}
			}
		}
		ImGui.SameLine();
		ImGui.SetCursorPosX(cursorPosX);
		ImGui.InvisibleButton(ImU8String.op_Implicit("副本区"), new Vector2(Math.Max(windowWidth, 1f), ImGui.GetFrameHeight()), (ImGuiButtonFlags)0);
	}

	private void RenderWorldChangeButtons()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		DisabledDisposable val = ImRaii.Disabled(isOnWorldTraveling);
		try
		{
			float cursorPosX = ImGui.GetCursorPosX();
			if (ImGui.CollapsingHeader(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(12510u)), (ImGuiTreeNodeFlags)0))
			{
				foreach (KeyValuePair<uint, string> dcWorld in dcWorlds)
				{
					if (dcWorld.Key != GameState.CurrentWorld && ButtonCenterText($"WorldTravelWidget_{dcWorld.Key}", dcWorld.Value + ((dcWorld.Key == GameState.HomeWorld) ? " (★)" : "")))
					{
						OmenServiceBase<ChatManager>.Instance().SendMessage($"/pdr worldtravel {dcWorld.Key}");
					}
				}
			}
			ImGui.SameLine();
			ImGui.SetCursorPosX(cursorPosX);
			ImGui.InvisibleButton(ImU8String.op_Implicit("跨界传送"), new Vector2(Math.Max(windowWidth, 1f), ImGui.GetFrameHeight()), (ImGuiButtonFlags)0);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void RenderAethernetShardButtons(uint aetheryteID)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		float cursorPosX = ImGui.GetCursorPosX();
		if (ImGui.CollapsingHeader(ImU8String.op_Implicit(LuminaWrapper.GetEObjName(2000151u)), (ImGuiTreeNodeFlags)0))
		{
			foreach (AetheryteRecord currentAethernetShard in currentAethernetShards)
			{
				if (currentAethernetShard.RowID != aetheryteID && ButtonCenterText($"AethernetShard_{currentAethernetShard.RowID}_{currentAethernetShard.SubIndex}", currentAethernetShard.Name))
				{
					currentAethernetShard.TeleportTo();
				}
			}
		}
		ImGui.SameLine();
		ImGui.SetCursorPosX(cursorPosX);
		ImGui.InvisibleButton(ImU8String.op_Implicit("城内以太之晶"), new Vector2(Math.Max(windowWidth, 1f), ImGui.GetFrameHeight()), (ImGuiButtonFlags)0);
	}

	public bool ButtonCenterText(string id, string text)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(id);
		((ImU8String)(ref val)).AppendLiteral("_");
		((ImU8String)(ref val)).AppendFormatted<string>(text);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ImGuiStylePtr style = ImGui.GetStyle();
			Vector2 framePadding = ((ImGuiStylePtr)(ref style)).FramePadding;
			float num = Math.Clamp(vector.X + framePadding.X * 2f, windowWidth, config.MaxButtonWidth);
			bool result = ImGui.Button(ImU8String.op_Implicit(string.Empty), new Vector2(num, vector.Y + framePadding.Y * 2f));
			ImGuiOm.TooltipHover(text);
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(cursorScreenPos.X + (num - vector.X) / 2f, cursorScreenPos.Y + framePadding.Y), ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(text));
			return result;
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe void OnUpdate(IFramework framework)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		BattleChara* localPlayer = Control.GetLocalPlayer();
		if (IConditionExtension.get_IsBetweenAreas(DalamudServiceExtension.Instance<ICondition>()) || localPlayer == null)
		{
			if (((Window)base.Overlay).IsOpen)
			{
				currentObjects.Clear();
				windowWidth = 0f;
				((Window)base.Overlay).IsOpen = false;
			}
			return;
		}
		if (forceObjectUpdate || Throttler.Shared.Throttle("FastObjectInteract-Monitor"))
		{
			forceObjectUpdate = false;
			UpdateObjectsList((GameObject*)localPlayer);
		}
		if (currentAethernetShards.Count == 0 && (int)GameState.TerritoryIntendedUse == 0)
		{
			TryPopulateAethernetShards();
		}
		bool flag = currentObjects.Count > 0 && IsWindowShouldBeOpen();
		if (base.Overlay != null)
		{
			((Window)base.Overlay).IsOpen = flag;
			if (!flag)
			{
				windowWidth = 0f;
			}
		}
	}

	private void OnLogin()
	{
		LoadWorldData();
	}

	private void OnTerritoryChanged(uint u)
	{
		forceObjectUpdate = true;
		currentAethernetShards.Clear();
	}

	private unsafe void UpdateObjectsList(GameObject* localPlayer)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Invalid comparison between Unknown and I4
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Invalid comparison between Unknown and I4
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		currentObjects.Clear();
		GameObjectManager* ptr = GameObjectManager.Instance();
		if (ptr == null)
		{
			return;
		}
		isOnWorldTraveling = DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)92,
			(ConditionFlag)93
		});
		Vector3 vector = ((GameObject)localPlayer).Position;
		int maxDisplayAmount = config.MaxDisplayAmount;
		for (int i = 200; i < ((ObjectArrays)(&((GameObjectManager)ptr).Objects)).IndexSorted.Length; i++)
		{
			Pointer<GameObject> val = ((ObjectArrays)(&((GameObjectManager)ptr).Objects)).IndexSorted[i];
			if (val == Pointer<GameObject>.op_Implicit((GameObject*)null))
			{
				continue;
			}
			GameObject* value = val.Value;
			if (value == null || !((GameObject)value).GetIsTargetable())
			{
				continue;
			}
			ObjectKind objectKind = (ObjectKind)((GameObject)value).ObjectKind;
			if (!config.SelectedKinds.Contains(objectKind))
			{
				continue;
			}
			float num = Vector2.DistanceSquared(vector.ToVector2(), ((GameObject)value).Position.ToVector2());
			float num2 = 400f;
			if (IncludeDistance.TryGetValue(objectKind, out var value2))
			{
				num2 = value2;
			}
			if (num > num2 || (!DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77] && MathF.Abs(((Vector3)(&((GameObject)value).Position)).Y - vector.Y) >= 4f))
			{
				continue;
			}
			if ((int)objectKind == 4)
			{
				Treasure* ptr2 = (Treasure*)value;
				if (((Treasure)ptr2).Flags.IsSetAny<TreasureFlags>(new ReadOnlySpan<TreasureFlags>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E._25DFD29C09617DCC9852281C030E5B3037A338A4712A42A21C907F259C6412A0), 2)))
				{
					continue;
				}
			}
			string text = ((GameObject)value).NameString;
			if (string.IsNullOrEmpty(text) || config.BlacklistKeys.Contains(text))
			{
				continue;
			}
			if ((int)objectKind == 3)
			{
				if (!ImportantENPC.Contains(((GameObject)value).BaseId) && ((GameObject)value).NamePlateIconId == 0)
				{
					continue;
				}
				if (ImportantENPC.Contains(((GameObject)value).BaseId) && ENPCTitle.TryGetValue(((GameObject)value).BaseId, out string value3))
				{
					text = $"[{value3}] {text}";
				}
			}
			if (!config.OnlyDisplayInViewRange || (GameViewHelper.WorldToScreen(Vector3.op_Implicit(((GameObject)value).Position), out var _, out var inView) && inView))
			{
				currentObjects.Add(new InteractableObject(value, text, objectKind, num));
			}
		}
		currentObjects.Sort(InteractableObjectComparison);
		if (currentObjects.Count > maxDisplayAmount)
		{
			currentObjects.RemoveRange(maxDisplayAmount, currentObjects.Count - maxDisplayAmount);
		}
	}

	private void UpdateWindowFlags()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (base.Overlay != null)
		{
			if (config.LockWindow)
			{
				Overlay? overlay = base.Overlay;
				((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags | 4);
			}
			else
			{
				Overlay? overlay2 = base.Overlay;
				((Window)overlay2).Flags = (ImGuiWindowFlags)(((Window)overlay2).Flags & -5);
			}
		}
	}

	private bool IsWindowShouldBeOpen()
	{
		if (currentObjects.Count == 0 && currentAethernetShards.Count == 0)
		{
			return false;
		}
		if (config.WindowInvisibleWhenInteract && IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()))
		{
			return false;
		}
		if (config.WindowInvisibleWhenCombat && DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)26])
		{
			return false;
		}
		return true;
	}

	private unsafe void InteractWithObject(GameObject* obj, ObjectKind kind)
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Invalid comparison between Unknown and I4
		base.TaskHelper.RemoveQueueTasks(2);
		if (IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()))
		{
			base.TaskHelper.Enqueue(delegate
			{
				OmenServiceBase<MovementManager>.Instance().Dismount();
			}, "DismountInteract", 0, null, null, null, null, 2);
			base.TaskHelper.DelayNext(500, null, 2);
		}
		base.TaskHelper.Enqueue((Func<bool>)IsAbleToInteract, "等待可交互状态", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 2);
		base.TaskHelper.Enqueue(delegate
		{
			if (!IsAbleToInteract())
			{
				return false;
			}
			((TargetSystem)TargetSystem.Instance()).Target = obj;
			return ((TargetSystem)TargetSystem.Instance()).InteractWithObject(obj, false) != 0;
		}, "Interact", 0, null, null, null, null, 2);
		if ((int)kind == 7)
		{
			base.TaskHelper.Enqueue(delegate
			{
				((TargetSystem)TargetSystem.Instance()).OpenObjectInteraction(obj);
			}, "OpenInteraction", 0, null, null, null, null, 2);
		}
		static bool IsAbleToInteract()
		{
			if (!IConditionExtension.get_IsOnMount(DalamudServiceExtension.Instance<ICondition>()) && !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
			{
				(ConditionFlag)48,
				(ConditionFlag)77
			}))
			{
				return !OmenServiceBase<MovementManager>.Instance().IsManagerBusy;
			}
			return false;
		}
	}

	private void LoadWorldData()
	{
		if (GameState.IsLoggedIn)
		{
			dcWorlds = (from x in Sheets.Worlds.Where(delegate(KeyValuePair<uint, World> x)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000a: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					World value = x.Value;
					return ((World)(ref value)).DataCenter.RowId == GameState.CurrentDataCenter;
				})
				orderby x.Key == GameState.HomeWorld
				select x).ThenBy(delegate(KeyValuePair<uint, World> x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				World value = x.Value;
				return ((object)((World)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			}).ToDictionary((KeyValuePair<uint, World> x) => x.Key, delegate(KeyValuePair<uint, World> x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				World value = x.Value;
				return ((object)((World)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
			});
		}
	}

	private unsafe void TryPopulateAethernetShards()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Invalid comparison between Unknown and I4
		foreach (InteractableObject currentObject in currentObjects)
		{
			if ((int)currentObject.Kind == 5 && currentObject.Pointer != null)
			{
				PopulateAethernetShards(((GameObject)currentObject.Pointer).BaseId);
				break;
			}
		}
	}

	private void PopulateAethernetShards(uint baseID)
	{
		AetheryteRecordManager aetheryteRecordManager = OmenServiceBase<AetheryteRecordManager>.Instance();
		AetheryteRecord aetheryteRecord = aetheryteRecordManager.AllRecords.FirstOrDefault((AetheryteRecord x) => x.RowID == baseID);
		if (aetheryteRecord == null || aetheryteRecord.Group == 0)
		{
			return;
		}
		foreach (AetheryteRecord allRecord in aetheryteRecordManager.AllRecords)
		{
			if (allRecord.Group == aetheryteRecord.Group && allRecord.IsUnlocked())
			{
				currentAethernetShards.Add(allRecord);
			}
		}
	}

	[GeneratedRegex("\\[.*?\\]")]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex FastObjectInteractTitleRegex()
	{
		return _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__FastObjectInteractTitleRegex_1.Instance;
	}

	private static int InteractableObjectComparison(InteractableObject x, InteractableObject y)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		int num = x.DistanceSq.CompareTo(y.DistanceSq);
		if (num != 0)
		{
			return num;
		}
		return GetKindPriority(x.Kind).CompareTo(GetKindPriority(y.Kind));
	}

	private static int GetKindPriority(ObjectKind kind)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected I4, but got Unknown
		return (kind - 3) switch
		{
			2 => 1, 
			0 => 2, 
			4 => 3, 
			1 => 4, 
			3 => 5, 
			_ => 10, 
		};
	}
}
