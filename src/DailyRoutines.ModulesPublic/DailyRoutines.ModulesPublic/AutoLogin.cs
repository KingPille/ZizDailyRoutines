using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Widgets.Combos;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.AgentEvent;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoLogin : ModuleBase
{
	private class Config : ModuleConfig
	{
		public List<LoginInfo> LoginData = new List<LoginInfo>();

		public BehaviourMode Mode;

		public string CurrentProcessInfo = string.Empty;
	}

	private class LoginInfo(uint worldID, string characterName) : IEquatable<LoginInfo>
	{
		public uint WorldID { get; set; } = worldID;

		public string CharacterName { get; set; } = characterName;

		public bool Equals(LoginInfo? other)
		{
			if ((object)other == null || GetType() != other.GetType())
			{
				return false;
			}
			if (WorldID == other.WorldID)
			{
				return CharacterName == other.CharacterName;
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as LoginInfo);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(WorldID, CharacterName);
		}

		public static bool operator ==(LoginInfo? lhs, LoginInfo? rhs)
		{
			return lhs?.Equals(rhs) ?? ((object)rhs == null);
		}

		public static bool operator !=(LoginInfo lhs, LoginInfo rhs)
		{
			return !(lhs == rhs);
		}
	}

	private enum BehaviourMode
	{
		Once,
		Repeat
	}

	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnDialogue;
	}

	private Config config;

	private WorldSelectCombo worldSelectCombo;

	private string selectedCharaName;

	private int dropIndex = -1;

	private (string? ChracterName, int CharacterIndex, uint WorldID)? manualLoginInfo;

	private bool isNextAutoLoginHandled;

	private const string COMMAND = "/pdrlogin";

	private static readonly Dictionary<BehaviourMode, string> BehaviourModeLoc = new Dictionary<BehaviourMode, string>
	{
		[BehaviourMode.Once] = LanguageManager.Get("AutoLogin-Once"),
		[BehaviourMode.Repeat] = LanguageManager.Get("AutoLogin-Repeat")
	};

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoLoginTitle"),
		Description = LanguageManager.Get("AutoLoginDescription"),
		Category = ModuleCategory.System,
		ModulesPrerequisite = new string[1] { "InstantLogout" }
	};

	private static string CurrentProcessInfo
	{
		get
		{
			using Process process = Process.GetCurrentProcess();
			return $"{Environment.ProcessId}-{process.StartTime.ToUniversalTime().Ticks}";
		}
	}

	protected override void Init()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		worldSelectCombo = new WorldSelectCombo("World");
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 180000,
				ShowDebug = true
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "_TitleMenu", new AddonEventDelegate(OnTitleMenu));
		OnTitleMenu((AddonEvent)1, null);
		IAddonLifecycle obj2 = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj3 = _003C_003EO._003C0_003E__OnDialogue;
		if (obj3 == null)
		{
			AddonEventDelegate val = OnDialogue;
			_003C_003EO._003C0_003E__OnDialogue = val;
			obj3 = (object)val;
		}
		obj2.RegisterListener((AddonEvent)4, "Dialogue", (AddonEventDelegate)obj3);
		OmenServiceBase<CommandManager>.Instance().AddCommand("/pdrlogin", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("AutoLogin-CommandHelp")
		});
		OmenServiceBase<GameState>.Instance().Login += OnLogin;
	}

	protected override void Uninit()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		OmenServiceBase<GameState>.Instance().Login -= OnLogin;
		OmenServiceBase<CommandManager>.Instance().RemoveCommand("/pdrlogin");
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] obj2 = new AddonEventDelegate[2]
		{
			OnTitleMenu,
			default(AddonEventDelegate)
		};
		object obj3 = _003C_003EO._003C0_003E__OnDialogue;
		if (obj3 == null)
		{
			AddonEventDelegate val = OnDialogue;
			_003C_003EO._003C0_003E__OnDialogue = val;
			obj3 = (object)val;
		}
		obj2[1] = (AddonEventDelegate)obj3;
		obj.UnregisterListener((AddonEventDelegate[])(object)obj2);
	}

	protected unsafe override void ConfigUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(1, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>("/pdrlogin");
			((ImU8String)(ref val3)).AppendLiteral(" ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("AutoLogin-CommandHelp"));
			ImGui.TextWrapped(val3);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		ImGuiOmExtension.ConflictKeyText();
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val4 = default(ImU8String);
		((ImU8String)(ref val4))._002Ector(0, 1);
		((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("AutoLogin-BehaviourMode"));
		ImGui.TextColored(ref vector, val4);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.SetNextItemWidth(300f * Globals.GlobalUIScale);
			ComboDisposable val5 = ImRaii.Combo(ImU8String.op_Implicit("###BehaviourModeCombo"), ImU8String.op_Implicit(BehaviourModeLoc[config.Mode]));
			try
			{
				if (ComboDisposable.op_Implicit(val5))
				{
					foreach (KeyValuePair<BehaviourMode, string> item2 in BehaviourModeLoc)
					{
						if (ImGui.Selectable(ImU8String.op_Implicit(item2.Value), item2.Key == config.Mode, (ImGuiSelectableFlags)0, default(Vector2)))
						{
							config.Mode = item2.Key;
							config.Save(this);
						}
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val5)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		vector = KnownColor.Chocolate.ToVector4();
		ImU8String val6 = default(ImU8String);
		((ImU8String)(ref val6))._002Ector(0, 1);
		((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("AutoLogin-LoginInfos"));
		ImGui.TextColored(ref vector, val6);
		val2 = ImRaii.PushIndent(1, true);
		try
		{
			GroupDisposable val7 = ImRaii.Group();
			try
			{
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				worldSelectCombo.DrawRadio();
				ImGui.SameLine();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(13634u)));
				ImGui.SameLine();
				if (ImGui.SmallButton(ImU8String.op_Implicit(LanguageManager.Get("Current"))))
				{
					worldSelectCombo.SelectedID = GameState.CurrentWorld;
				}
				ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
				ImU8String val8 = new ImU8String(30, 1);
				((ImU8String)(ref val8)).AppendFormatted<string>(LuminaWrapper.GetAddonText(14055u));
				((ImU8String)(ref val8)).AppendLiteral("##AutoLogin-EnterCharacterName");
				ImGui.InputText(val8, ref selectedCharaName, 512, (ImGuiInputTextFlags)32, (ImGuiInputTextCallbackDelegate)null);
			}
			finally
			{
				((GroupDisposable)(ref val7)).Dispose();
			}
			ImGui.SameLine(0f, 16f * Globals.GlobalUIScale);
			if (ImGuiOm.ButtonIconWithTextVertical((FontAwesomeIcon)61543, LanguageManager.Get("Add")))
			{
				if (string.IsNullOrWhiteSpace(selectedCharaName) || worldSelectCombo.SelectedID == 0)
				{
					return;
				}
				LoginInfo item = new LoginInfo(worldSelectCombo.SelectedID, selectedCharaName);
				if (!config.LoginData.Contains(item))
				{
					config.LoginData.Add(item);
					config.Save(this);
				}
			}
			ImGuiOm.TooltipHover(LanguageManager.Get("AutoLogin-LoginInfoOrderHelp"));
			for (int i = 0; i < config.LoginData.Count; i++)
			{
				LoginInfo loginInfo = config.LoginData[i];
				string text = $"{i + 1}. {LanguageManager.Get("AutoLogin-LoginInfoDisplayText", LuminaWrapper.GetWorldName(loginInfo.WorldID), LuminaWrapper.GetWorldDCName(loginInfo.WorldID), loginInfo.CharacterName)}";
				ColorDisposable val9 = ImRaii.PushColor((ImGuiCol)0, (i % 2 == 0) ? ImGuiColors.TankBlue : ImGuiColors.DalamudWhite, true);
				try
				{
					ImGui.Selectable(ImU8String.op_Implicit(text), false, (ImGuiSelectableFlags)0, default(Vector2));
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
				DragDropSourceDisposable val10 = ImRaii.DragDropSource();
				try
				{
					if (DragDropSourceDisposable.op_Implicit(val10))
					{
						if (ImGui.SetDragDropPayload(ImU8String.op_Implicit("LoginInfoReorder"), default(ReadOnlySpan<byte>), (ImGuiCond)0))
						{
							dropIndex = i;
						}
						vector = ImGuiColors.DalamudYellow;
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(text));
					}
				}
				finally
				{
					((DragDropSourceDisposable)(ref val10)).Dispose();
				}
				DragDropTargetDisposable val11 = ImRaii.DragDropTarget();
				try
				{
					if (DragDropTargetDisposable.op_Implicit(val11) && ImGui.AcceptDragDropPayload(ImU8String.op_Implicit("LoginInfoReorder"), (ImGuiDragDropFlags)0).Handle != null)
					{
						Swap(dropIndex, i);
						dropIndex = -1;
					}
				}
				finally
				{
					((DragDropTargetDisposable)(ref val11)).Dispose();
				}
				ImU8String val12 = new ImU8String(12, 1);
				((ImU8String)(ref val12)).AppendLiteral("ContextMenu_");
				((ImU8String)(ref val12)).AppendFormatted<int>(i);
				PopupDisposable val13 = ImRaii.ContextPopupItem(val12);
				try
				{
					if (PopupDisposable.op_Implicit(val13) && ImGui.Selectable(ImU8String.op_Implicit(LanguageManager.Get("Delete")), false, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						config.LoginData.Remove(loginInfo);
						config.Save(this);
					}
				}
				finally
				{
					((PopupDisposable)(ref val13)).Dispose();
				}
				if (i != config.LoginData.Count - 1)
				{
					ImGui.Separator();
				}
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe static void OnDialogue(AddonEvent type, AddonArgs args)
	{
		AtkUnitBase* dialogue = Addons.Dialogue;
		if ((*dialogue).IsAddonAndNodesReady())
		{
			AtkComponentButton* componentButtonById = ((AtkUnitBase)dialogue).GetComponentButtonById(4u);
			if (componentButtonById != null)
			{
				(*componentButtonById).Click();
			}
		}
	}

	private void OnLogin()
	{
		base.TaskHelper.Abort();
		manualLoginInfo = null;
		isNextAutoLoginHandled = false;
		config.CurrentProcessInfo = CurrentProcessInfo;
		config.Save(this);
	}

	private unsafe void OnCommand(string command, string args)
	{
		args = args.Trim();
		if (string.IsNullOrWhiteSpace(args) || !GameState.IsLoggedIn || GameState.ContentFinderCondition != 0 || base.TaskHelper.IsBusy)
		{
			return;
		}
		string[] array = args.Split(' ');
		int num = array.Length;
		if ((uint)(num - 1) > 1u)
		{
			return;
		}
		manualLoginInfo = null;
		string item = null;
		int item2 = -1;
		uint item3 = ((AgentLobby)AgentLobby.Instance()).WorldId;
		string text = array[0];
		if (uint.TryParse(text, out var result) && result < 8)
		{
			item2 = (int)result;
		}
		else
		{
			item = text.Replace('+', ' ');
		}
		if (array.Length > 1)
		{
			string worldPart = array[1];
			if (uint.TryParse(worldPart, out var result2) && Sheets.Worlds.ContainsKey(result2))
			{
				item3 = result2;
			}
			else
			{
				KeyValuePair<uint, World> keyValuePair = Sheets.Worlds.FirstOrDefault(delegate(KeyValuePair<uint, World> x)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000a: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					World value = x.Value;
					return ((object)((World)(ref value)).Name/*cast due to constrained. prefix*/).ToString().Equals(worldPart, StringComparison.OrdinalIgnoreCase);
				});
				if (keyValuePair.Key != 0)
				{
					item3 = keyValuePair.Key;
				}
			}
		}
		manualLoginInfo = (item, item2, item3);
		base.TaskHelper.Enqueue(delegate
		{
			OmenServiceBase<ChatManager>.Instance().SendMessage("/logout");
		});
	}

	private unsafe void OnTitleMenu(AddonEvent eventType, AddonArgs? args)
	{
		if (isNextAutoLoginHandled || (config.LoginData.Count == 0 && !manualLoginInfo.HasValue) || base.TaskHelper.AbortByConflictKey(this) || (*Addons.LobbyDKT).IsAddonAndNodesReady() || GameState.IsLoggedIn || (!manualLoginInfo.HasValue && config.Mode == BehaviourMode.Once && config.CurrentProcessInfo == CurrentProcessInfo))
		{
			return;
		}
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(delegate
		{
			if ((*Addons.CharaSelectListMenu).IsAddonAndNodesReady())
			{
				return true;
			}
			if (!(*Addons.TitleMenu).IsAddonAndNodesReady())
			{
				return false;
			}
			((AgentId)0).SendEvent(0uL, 4);
			return true;
		}, "点击登录");
		base.TaskHelper.Enqueue(delegate
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_018f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Invalid comparison between Unknown and I4
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			if (base.TaskHelper.AbortByConflictKey(this))
			{
				return true;
			}
			if (!Throttler.Shared.Throttle("AutoLogin-SelectCharacter", 100u))
			{
				return false;
			}
			AgentLobby* ptr = AgentLobby.Instance();
			if (ptr == null)
			{
				return false;
			}
			if (!(*Addons.CharaSelectListMenu).IsAddonAndNodesReady())
			{
				return false;
			}
			LobbyUIClient client = ((LobbyData)(&((AgentLobby)ptr).LobbyData)).LobbyUIClient;
			(string, int, uint)? tuple = manualLoginInfo;
			if (tuple.HasValue)
			{
				(string, int, uint) valueOrDefault = tuple.GetValueOrDefault();
				string charName = valueOrDefault.Item1;
				int charIndex = valueOrDefault.Item2;
				uint worldID = valueOrDefault.Item3;
				LobbyUIClientCharacterEntry val = ((charIndex >= 0) ? ((IEnumerable<LobbyUIClientCharacterEntry>)(object)client.CurrentDataCenterCharacters).FirstOrDefault((LobbyUIClientCharacterEntry x) => x.Index == charIndex && x.CurrentWorldId == worldID) : ((IEnumerable<LobbyUIClientCharacterEntry>)(object)client.CurrentDataCenterCharacters).FirstOrDefault((LobbyUIClientCharacterEntry x) => ((LobbyUIClientCharacterEntry)(ref x)).NameString == charName && x.CurrentWorldId == worldID));
				if (val.HomeWorldId != 0)
				{
					EnqueueLogin(val.ContentId, val.CurrentWorldId, 0);
					manualLoginInfo = null;
					return true;
				}
				manualLoginInfo = null;
			}
			int num = config.LoginData.Count + 1;
			foreach (LoginInfo loginInfo in config.LoginData)
			{
				num--;
				LobbyUIClientCharacterEntry val2 = ((IEnumerable<LobbyUIClientCharacterEntry>)(object)client.CurrentDataCenterCharacters).FirstOrDefault((LobbyUIClientCharacterEntry x) => LuminaWrapper.GetWorldDC(((LobbyDataCenterWorldEntry)client.CurrentDataCenterWorlds.First).Id) == LuminaWrapper.GetWorldDC(x.CurrentWorldId) && ((LobbyUIClientCharacterEntry)(ref x)).NameString == loginInfo.CharacterName && x.HomeWorldId == loginInfo.WorldID);
				if (val2.HomeWorldId != 0)
				{
					EnqueueLogin(val2.ContentId, ((int)val2.LoginFlags == 16) ? val2.CurrentWorldId : val2.HomeWorldId, num);
					break;
				}
			}
			return true;
		}, "选择角色");
	}

	private unsafe void EnqueueLogin(ulong contentID, uint world, int weight)
	{
		AgentLobby* agent = AgentLobby.Instance();
		if (agent == null)
		{
			return;
		}
		TaskHelper? taskHelper = base.TaskHelper;
		Func<bool> task = () => AgentLobbyEvent.SelectWorldByID(world);
		int weight2 = weight;
		taskHelper.Enqueue(task, null, 0, null, null, null, null, weight2);
		TaskHelper? taskHelper2 = base.TaskHelper;
		Func<bool> task2 = () => ((AgentLobby)agent).WorldId == world;
		weight2 = weight;
		taskHelper2.Enqueue(task2, null, 0, null, null, null, null, weight2);
		TaskHelper? taskHelper3 = base.TaskHelper;
		Func<bool> task3 = () => AgentLobbyEvent.SelectCharacter((CharaSelectCharacterEntry x) => x.ContentId == contentID);
		weight2 = weight;
		taskHelper3.Enqueue(task3, null, 0, null, null, null, null, weight2);
	}

	private void Swap(int index1, int index2)
	{
		if (index1 >= 0 && index1 <= config.LoginData.Count && index2 >= 0 && index2 <= config.LoginData.Count)
		{
			List<LoginInfo> loginData = config.LoginData;
			List<LoginInfo> loginData2 = config.LoginData;
			LoginInfo value = config.LoginData[index2];
			LoginInfo value2 = config.LoginData[index1];
			loginData[index1] = value;
			loginData2[index2] = value2;
			base.TaskHelper.Abort();
			base.TaskHelper.DelayNext(500);
			base.TaskHelper.Enqueue(delegate
			{
				config.Save(this);
			});
		}
	}

	[IPCProvider("AutoLogin.MarkNextAutoLoginHandled")]
	private void MarkNextAutoLoginHandled()
	{
		isNextAutoLoginHandled = true;
	}
}
