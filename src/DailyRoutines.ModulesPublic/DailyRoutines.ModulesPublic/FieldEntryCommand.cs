using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility.Raii;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class FieldEntryCommand : ModuleBase
{
	private readonly FrozenDictionary<string, (Action EnqueueAction, uint Content)> commandArgs = new Dictionary<string, (Action, uint)>
	{
		["bozja"] = (EnqueueBozja, 735u),
		["zadnor"] = (EnqueueZadonor, 778u),
		["anemos"] = (EnqueueAnemos, 283u),
		["pagos"] = (EnqueuePagos, 581u),
		["pyros"] = (EnqueuePyros, 598u),
		["hydatos"] = (EnqueueHydatos, 639u),
		["diadem"] = (EnqueueDiadem, 753u),
		["island"] = (EnqueueIsland, 1u),
		["ardorum"] = (EnqueueArdorum, 2u),
		["phaenna"] = (EnqueuePhaenna, 3u),
		["oizys"] = (EnqueueOizys, 4u),
		["auxesia"] = (EnqueueAuxesia, 5u),
		["ocs"] = (EnqueueSouthHorn, 1018u),
		["ocn"] = (EnqueueNorthHorn, 1093u)
	}.ToFrozenDictionary();

	private uint redirectTargetZoneInMoon;

	private const string COMMAND = "/pdrfe";

	private static readonly FrozenDictionary<uint, string> ContentToPlaceName = new Dictionary<uint, string>
	{
		[1u] = LuminaWrapper.GetPlaceName(2566u),
		[2u] = LuminaWrapper.GetPlaceName(5219u),
		[3u] = LuminaWrapper.GetPlaceName(5301u),
		[4u] = LuminaWrapper.GetPlaceName(5406u),
		[5u] = LuminaWrapper.GetPlaceName(5551u)
	}.ToFrozenDictionary();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("FieldEntryCommandTitle"),
		Description = LanguageManager.Get("FieldEntryCommandDescription", "/pdrfe"),
		Category = ModuleCategory.Assist,
		ModulesPrerequisite = new string[1] { "AutoTalkSkip" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		OmenServiceBase<GamePacketManager>.Instance().RegPreSendPacket(OnPreSendPacket);
		OmenServiceBase<CommandManager>.Instance().AddCommand("/pdrfe", new CommandInfo(new HandlerDelegate(OnCommand))
		{
			HelpMessage = LanguageManager.Get("FieldEntryCommand-CommandHelp")
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<GamePacketManager>.Instance().Unreg(OnPreSendPacket);
		OmenServiceBase<CommandManager>.Instance().RemoveCommand("/pdrfe");
		redirectTargetZoneInMoon = 0u;
	}

	protected override void ConfigUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("Command"));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val3 = new ImU8String(3, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>("/pdrfe");
			((ImU8String)(ref val3)).AppendLiteral(" → ");
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("FieldEntryCommand-CommandHelp"));
			ImGui.TextUnformatted(val3);
			ImGui.Spacing();
			ImU8String val4 = ImU8String.op_Implicit("ArgsTable");
			Vector2 vector2 = ImGui.GetContentRegionAvail() / 2f;
			vector2.Y = 0f;
			TableDisposable val5 = ImRaii.Table(val4, 2, (ImGuiTableFlags)1920, vector2);
			try
			{
				if (!val5)
				{
					return;
				}
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Argument")), (ImGuiTableColumnFlags)4, 10f, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(14098u)), (ImGuiTableColumnFlags)4, 20f, 0u);
				ImGui.TableHeadersRow();
				foreach (KeyValuePair<string, (Action, uint)> commandArg in commandArgs)
				{
					if (LuminaGetter.TryGetRow<ContentFinderCondition>(commandArg.Value.Item2, out ContentFinderCondition item))
					{
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						ImGui.TextUnformatted(ImU8String.op_Implicit(commandArg.Key));
						if (ImGui.IsItemHovered())
						{
							ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						}
						if (ImGui.IsItemClicked())
						{
							ImU8String clipboardText = new ImU8String(0, 1);
							((ImU8String)(ref clipboardText)).AppendFormatted<string>(commandArg.Key);
							ImGui.SetClipboardText(clipboardText);
							OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get("CopiedToClipboard") + ": " + commandArg.Key);
						}
						ImGui.TableNextColumn();
						ImGui.TextUnformatted(ImU8String.op_Implicit(ContentToPlaceName.TryGetValue(commandArg.Value.Item2, out string value) ? value : ((object)((ContentFinderCondition)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val5)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void OnCommand(string command, string args)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		base.TaskHelper.Abort();
		args = args.Trim().ToLowerInvariant();
		foreach (KeyValuePair<string, (Action, uint)> commandArg in commandArgs)
		{
			if (LuminaGetter.TryGetRow<ContentFinderCondition>(commandArg.Value.Item2, out ContentFinderCondition item))
			{
				string value;
				string text = (ContentToPlaceName.TryGetValue(commandArg.Value.Item2, out value) ? value : ((object)((ContentFinderCondition)(ref item)).Name/*cast due to constrained. prefix*/).ToString());
				if (args == commandArg.Key || text.Contains(args, StringComparison.OrdinalIgnoreCase))
				{
					commandArg.Value.Item1();
					OmenServiceBase<NotifyHelper>.Instance().NotificationInfo(LanguageManager.Get("FieldEntryCommand-Notification", text));
					break;
				}
			}
		}
	}

	public unsafe void OnPreSendPacket(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize)
	{
		if (redirectTargetZoneInMoon == 0 || GameState.TerritoryType != 959 || opcode != UpstreamOpcode.EventCompleteOpcode)
		{
			return;
		}
		EventCompletePackt* ptr = (EventCompletePackt*)packet;
		if (ptr->EventID == 327855)
		{
			if (ptr->Category == 33554432)
			{
				ptr->Param1 = redirectTargetZoneInMoon;
			}
			if (ptr->Category == 16777316)
			{
				ptr->Param0 = redirectTargetZoneInMoon;
				redirectTargetZoneInMoon = 0u;
			}
		}
	}

	private static void EnqueueIsland()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(1055u, new Vector3(-269.4f, 40f, 227.8f));
	}

	private static void EnqueueDiadem()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(939u);
	}

	private static void EnqueueAnemos()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(732u);
	}

	private static void EnqueuePagos()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(763u);
	}

	private static void EnqueuePyros()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(795u);
	}

	private static void EnqueueHydatos()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(827u);
	}

	private static void EnqueueBozja()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(920u);
	}

	private static void EnqueueZadonor()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(975u);
	}

	private static void EnqueueSouthHorn()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(1252u);
	}

	private static void EnqueueNorthHorn()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(1346u);
	}

	private static void EnqueueArdorum()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(1237u);
	}

	private static void EnqueuePhaenna()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(1291u);
	}

	private static void EnqueueOizys()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(1310u);
	}

	private static void EnqueueAuxesia()
	{
		OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(1319u);
	}
}
