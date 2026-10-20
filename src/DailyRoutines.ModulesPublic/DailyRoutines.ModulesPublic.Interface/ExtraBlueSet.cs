using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class ExtraBlueSet : ModuleBase
{
	private class BlueMagePresetEntry
	{
		public string Name { get; set; } = string.Empty;

		public uint[] Actions { get; set; } = new uint[24];
	}

	private class Config : ModuleConfig
	{
		public List<BlueMagePresetEntry> Presets = new List<BlueMagePresetEntry>();
	}

	private Config config;

	private string newPresetNameInput = string.Empty;

	private const string COMMAND = "exblueset";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ExtraBlueSetTitle"),
		Description = LanguageManager.Get("ExtraBlueSetDescription", "exblueset"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Marsh" }
	};

	protected unsafe override void Init()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "AOZNotebook", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, "AOZNotebook", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "AOZNotebook", new AddonEventDelegate(OnAddon));
		if ((*Addons.AOZNotebook).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
		OmenServiceBase<CommandManager>.Instance().AddSubCommand("exblueset", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("ExtraBlueSet-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand("exblueset");
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
	}

	protected unsafe override void OverlayUI()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		AtkUnitBase* aOZNotebook = Addons.AOZNotebook;
		if (aOZNotebook == null)
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		AtkResNode* nodeById = ((AtkUnitBase)aOZNotebook).GetNodeById(5u);
		if (nodeById == null)
		{
			return;
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			ImGui.SetWindowPos(new Vector2((float)((AtkUnitBase)aOZNotebook).GetX() - ImGui.GetWindowSize().X + 20f * Globals.GlobalUIScale, (float)((AtkUnitBase)aOZNotebook).GetY() + 30f * Globals.GlobalUIScale), (ImGuiCond)0);
			float cursorPosY = ImGui.GetCursorPosY();
			ImGui.SetCursorPosY(cursorPosY + 2f * Globals.GlobalUIScale);
			using (OmenServiceBase<FontManager>.Instance().UIFont.Push())
			{
				Vector4 vector = KnownColor.Chocolate.ToVector4();
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(Info.Title));
			}
			ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
			ImGui.SetCursorPosY(cursorPosY - Globals.GlobalUIScale);
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61543, LanguageManager.Get("Add")))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit("NewPresetPopup"), (ImGuiPopupFlags)0);
			}
			PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit("NewPresetPopup"));
			try
			{
				if (PopupDisposable.op_Implicit(val))
				{
					ImGui.InputTextWithHint(ImU8String.op_Implicit("###NewPresetNameInput"), ImU8String.op_Implicit(LanguageManager.Get("Name")), ref newPresetNameInput, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					DisabledDisposable val2 = ImRaii.Disabled(string.IsNullOrWhiteSpace(newPresetNameInput));
					try
					{
						if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61452, LanguageManager.Get("Confirm")) && !string.IsNullOrWhiteSpace(newPresetNameInput) && config.Presets.FirstOrDefault((BlueMagePresetEntry x) => x.Name == newPresetNameInput) == null)
						{
							ActionManager* ptr = ActionManager.Instance();
							uint[] array = new uint[24];
							for (int num = 0; num < 24; num++)
							{
								array[num] = ((ActionManager)ptr).GetActiveBlueMageActionInSlot(num);
							}
							config.Presets.Add(new BlueMagePresetEntry
							{
								Name = newPresetNameInput,
								Actions = array
							});
							config.Save(this);
							ImGui.CloseCurrentPopup();
							newPresetNameInput = string.Empty;
						}
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
			}
			finally
			{
				((PopupDisposable)(ref val)).Dispose();
			}
			ImGui.Spacing();
			NodeState nodeState = (*nodeById).GetNodeState();
			ChildDisposable val3 = ImRaii.Child(ImU8String.op_Implicit("List"), nodeState.Size, true);
			try
			{
				if (!val3)
				{
					return;
				}
				ISharedImmediateTexture val9 = default(ISharedImmediateTexture);
				for (int num2 = 0; num2 < config.Presets.Count; num2++)
				{
					BlueMagePresetEntry blueMagePresetEntry = config.Presets[num2];
					IdDisposable val4 = ImRaii.PushId(num2, true);
					try
					{
						GroupDisposable val5 = ImRaii.Group();
						try
						{
							ImU8String val6 = new ImU8String(2, 2);
							((ImU8String)(ref val6)).AppendFormatted<string>(blueMagePresetEntry.Name);
							((ImU8String)(ref val6)).AppendLiteral("##");
							((ImU8String)(ref val6)).AppendFormatted<int>(num2);
							if (ImGui.Selectable(val6, false, (ImGuiSelectableFlags)0, default(Vector2)))
							{
								ApplyCustomPreset(blueMagePresetEntry);
							}
							if (ImGui.IsItemHovered())
							{
								ImGui.BeginTooltip();
								Vector4 vector = KnownColor.Chocolate.ToVector4();
								ImU8String val7 = new ImU8String(0, 1);
								((ImU8String)(ref val7)).AppendFormatted<string>(blueMagePresetEntry.Name);
								ImGui.TextColored(ref vector, val7);
								for (int num3 = 0; num3 < blueMagePresetEntry.Actions.Length; num3++)
								{
									if (!LuminaGetter.TryGetRow<Action>(blueMagePresetEntry.Actions[num3], out Action item))
									{
										continue;
									}
									ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
									GameIconLookup val8 = new GameIconLookup((uint)((Action)(ref item)).Icon, false, true, (ClientLanguage?)null);
									if (obj.TryGetFromGameIcon(ref val8, ref val9))
									{
										if (num3 != 0)
										{
											ImGui.SameLine();
										}
										ImGui.Image(val9.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
									}
								}
								ImGui.EndTooltip();
							}
							ImU8String val10 = new ImU8String(7, 1);
							((ImU8String)(ref val10)).AppendLiteral("Context");
							((ImU8String)(ref val10)).AppendFormatted<int>(num2);
							PopupDisposable val11 = ImRaii.ContextPopupItem(val10);
							try
							{
								if (PopupDisposable.op_Implicit(val11))
								{
									ImU8String val12 = new ImU8String(0, 1);
									((ImU8String)(ref val12)).AppendFormatted<string>(LanguageManager.Get("Delete"));
									if (ImGui.MenuItem(val12, false, true))
									{
										config.Presets.RemoveAt(num2);
										config.Save(this);
									}
								}
							}
							finally
							{
								((PopupDisposable)(ref val11)).Dispose();
							}
						}
						finally
						{
							((GroupDisposable)(ref val5)).Dispose();
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
			}
			finally
			{
				((ChildDisposable)(ref val3)).Dispose();
			}
		}
	}

	private void OnAddon(AddonEvent type, AddonArgs? args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		Overlay overlay = base.Overlay;
		bool isOpen = (int)type == 1 || ((int)type != 6 && ((Window)base.Overlay).IsOpen);
		((Window)overlay).IsOpen = isOpen;
	}

	private unsafe static void ApplyCustomPreset(BlueMagePresetEntry entry)
	{
		uint[] actions = entry.Actions;
		if (actions.Length != 24)
		{
			return;
		}
		ActionManager* ptr = ActionManager.Instance();
		Span<uint> span = stackalloc uint[24];
		Span<uint> span2 = stackalloc uint[24];
		for (int i = 0; i < 24; i++)
		{
			span[i] = ((ActionManager)ptr).GetActiveBlueMageActionInSlot(i);
			span2[i] = actions[i];
		}
		for (int j = 0; j < 24; j++)
		{
			if (span2[j] == 0)
			{
				continue;
			}
			for (int k = 0; k < 24; k++)
			{
				if (j != k && span2[j] == span[k])
				{
					((ActionManager)ptr).SwapBlueMageActionSlots(j, k);
					span2[j] = 0u;
					break;
				}
			}
		}
		for (int l = 0; l < 24; l++)
		{
			if (span2[l] != 0)
			{
				((ActionManager)ptr).AssignBlueMageActionToSlot(l, span2[l]);
			}
		}
		OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("ExtraBlueSet-Notification", entry.Name));
	}

	private void OnCommand(string command, string args)
	{
		args = args.Trim();
		if (!string.IsNullOrWhiteSpace(args))
		{
			BlueMagePresetEntry blueMagePresetEntry = config.Presets.FirstOrDefault((BlueMagePresetEntry x) => x.Name == args);
			if (blueMagePresetEntry != null)
			{
				ApplyCustomPreset(blueMagePresetEntry);
			}
		}
	}
}
