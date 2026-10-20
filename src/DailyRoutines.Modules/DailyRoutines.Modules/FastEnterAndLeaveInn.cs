using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Command;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using OmenTools;
using OmenTools.Dalamud.Services.Game.UI.Abstractions;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class FastEnterAndLeaveInn : ModuleBase
{
	private record QCv6323yOHSYHfxkTo2
	{
		public uint EventID { get; init; }

		public uint CompleteCategory { get; init; }

		public QCv6323yOHSYHfxkTo2(uint EventID, uint CompleteCategory)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.EventID = EventID;
			this.CompleteCategory = CompleteCategory;
			base._002Ector();
		}

		public void bBs3LrVLkX()
		{
			new EventStartPackt(LocalPlayerState.EntityID, EventID).Send();
		}

		public void YZB3HuV2EL()
		{
			new EventCompletePackt(EventID, CompleteCategory, 1u).Send();
		}

		public unsafe void oOh3SA0TUA()
		{
			FastEnterAndLeaveInn? module = ManagerBase<ModuleManager>.Instance().GetModule<FastEnterAndLeaveInn>();
			module.TaskHelper.Enqueue((Action)bBs3LrVLkX, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			module.TaskHelper.Enqueue(delegate
			{
				if (Addons.SelectString == null && Addons.SelectYesno == null)
				{
					return false;
				}
				YZB3HuV2EL();
				return true;
			});
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x9E3912E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194A448 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
			if (PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C348E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
			builder.Append(EventID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C144B33 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
			builder.Append(CompleteCategory.ToString());
			return true;
		}

		[CompilerGenerated]
		protected QCv6323yOHSYHfxkTo2(QCv6323yOHSYHfxkTo2 original)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			EventID = original.EventID;
			CompleteCategory = original.CompleteCategory;
		}

		[CompilerGenerated]
		public void MoZ3VCNF3V(out uint EventID, out uint CompleteCategory)
		{
			EventID = this.EventID;
			CompleteCategory = this.CompleteCategory;
		}

		[CompilerGenerated]
		private unsafe bool sIN39hxUMq()
		{
			if (Addons.SelectString == null && Addons.SelectYesno == null)
			{
				return false;
			}
			YZB3HuV2EL();
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass10_0
	{
		public (QCv6323yOHSYHfxkTo2 Info, Vector3 Position) info;

		public _003C_003Ec__DisplayClass10_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe bool PVO3fIRMOJ()
		{
			if (Addons.SelectString == null && Addons.SelectYesno == null)
			{
				return false;
			}
			info.Info.YZB3HuV2EL();
			return true;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass9_0
	{
		public (QCv6323yOHSYHfxkTo2 Info, Vector3 Position) data;

		public IAetheryteEntry final;

		public FastEnterAndLeaveInn _003C_003E4__this;

		public string command;

		public string args;

		public _003C_003Ec__DisplayClass9_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool hx83CMLEIr()
		{
			return OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(data.Position);
		}

		internal unsafe bool sJR3jIYFb7()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (!OmenServiceBase<MovementManager>.Instance().IsManagerBusy)
			{
				return (*EventFramework.Instance()).IsEventIDNearby(EventId.op_Implicit(data.Info.EventID));
			}
			return false;
		}

		internal void ynV3OkECXF()
		{
			data.Info.oOh3SA0TUA();
		}

		internal bool b2V3KO0ZxU()
		{
			return OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(final.TerritoryID);
		}

		internal bool lQr3tDaxyI()
		{
			if (!OmenServiceBase<MovementManager>.Instance().IsManagerBusy && GameState.TerritoryType == final.TerritoryID)
			{
				return UIModuleExtension.IsScreenReady();
			}
			return false;
		}

		internal void EnS3Wjwe10()
		{
			_003C_003E4__this.QUgsypgkmf(command, args);
		}
	}

	private static readonly FrozenDictionary<uint, (QCv6323yOHSYHfxkTo2 Info, Vector3 Position)> Infos;

	private const string COMMAND = "inn";

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		base.TaskHelper = new TaskHelper();
		OmenServiceBase<GamePacketManager>.Instance().RegPostSendPacket(fBJsLQGK2r);
		OmenServiceBase<CommandManager>.Instance().AddSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FC794 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825), new CommandInfo(new HandlerDelegate(QUgsypgkmf))
		{
			HelpMessage = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EBC47F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9))
		});
	}

	protected override void Uninit()
	{
		OmenServiceBase<GamePacketManager>.Instance().Unreg(fBJsLQGK2r);
		OmenServiceBase<CommandManager>.Instance().RemoveSubCommand(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF27FC2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765));
	}

	protected override void ConfigUI()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextColored(KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE33793 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818))));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(8, 2);
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x2ADE9AB0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
			((ImU8String)(ref val2)).AppendFormatted<string>(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x501709B4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FA702 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCFA6A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)));
			ImGui.TextUnformatted(val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe void QUgsypgkmf(string command, string args)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass9_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass9_0();
		CS_0024_003C_003E8__locals15._003C_003E4__this = this;
		CS_0024_003C_003E8__locals15.command = command;
		CS_0024_003C_003E8__locals15.args = args;
		if (GameState.ContentFinderCondition != 0)
		{
			return;
		}
		base.TaskHelper.Abort();
		if (Infos.TryGetValue(GameState.TerritoryType, out CS_0024_003C_003E8__locals15.data))
		{
			if ((*EventFramework.Instance()).IsEventIDNearby(EventId.op_Implicit(CS_0024_003C_003E8__locals15.data.Info.EventID)))
			{
				CS_0024_003C_003E8__locals15.data.Info.oOh3SA0TUA();
				return;
			}
			base.TaskHelper.Enqueue(() => OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(CS_0024_003C_003E8__locals15.data.Position));
			base.TaskHelper.Enqueue(() => !OmenServiceBase<MovementManager>.Instance().IsManagerBusy && (*EventFramework.Instance()).IsEventIDNearby(EventId.op_Implicit(CS_0024_003C_003E8__locals15.data.Info.EventID)));
			base.TaskHelper.Enqueue(delegate
			{
				CS_0024_003C_003E8__locals15.data.Info.oOh3SA0TUA();
			});
			return;
		}
		List<IAetheryteEntry> list = (from x in IOmenDalamudService<IAetheryteList>.Instance()
			where Infos.ContainsKey(x.TerritoryID)
			select x).ToList();
		if (list.Count != 0)
		{
			CS_0024_003C_003E8__locals15.final = list.MinBy((IAetheryteEntry x) => x.GilCost);
			base.TaskHelper.Enqueue(() => OmenServiceBase<MovementManager>.Instance().TPSmart_BetweenZone(CS_0024_003C_003E8__locals15.final.TerritoryID));
			base.TaskHelper.Enqueue(() => !OmenServiceBase<MovementManager>.Instance().IsManagerBusy && GameState.TerritoryType == CS_0024_003C_003E8__locals15.final.TerritoryID && UIModuleExtension.IsScreenReady());
			base.TaskHelper.Enqueue(delegate
			{
				CS_0024_003C_003E8__locals15._003C_003E4__this.QUgsypgkmf(CS_0024_003C_003E8__locals15.command, CS_0024_003C_003E8__locals15.args);
			});
		}
	}

	private unsafe void fBJsLQGK2r(int opcode, nint packet, bool isPrioritize)
	{
		_003C_003Ec__DisplayClass10_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass10_0();
		if (opcode != UpstreamOpcode.EventStartOpcode)
		{
			return;
		}
		if (!Infos.TryGetValue(GameState.TerritoryType, out CS_0024_003C_003E8__locals3.info) || ((EventStartPackt*)packet)->EventID != CS_0024_003C_003E8__locals3.info.Info.EventID)
		{
			return;
		}
		base.TaskHelper.Abort();
		base.TaskHelper.Enqueue(delegate
		{
			if (Addons.SelectString == null && Addons.SelectYesno == null)
			{
				return false;
			}
			CS_0024_003C_003E8__locals3.info.Info.YZB3HuV2EL();
			return true;
		});
	}

	public FastEnterAndLeaveInn()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753DA02 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x7B44512E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -1933616094 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			NeedAuth = true,
			AllDefaultEnabled = true
		};
		base._002Ector();
	}

	static FastEnterAndLeaveInn()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Infos = new Dictionary<uint, (QCv6323yOHSYHfxkTo2, Vector3)>
		{
			[132u] = (new QCv6323yOHSYHfxkTo2(131080u, 16777217u), new Vector3(30.1f, -20f, 73.7f)),
			[179u] = (new QCv6323yOHSYHfxkTo2(131083u, 16777216u), Vector3.Zero),
			[128u] = (new QCv6323yOHSYHfxkTo2(131079u, 16777217u), new Vector3(10.8f, 30f, 11.3f)),
			[177u] = (new QCv6323yOHSYHfxkTo2(131082u, 16777216u), Vector3.Zero),
			[130u] = (new QCv6323yOHSYHfxkTo2(131081u, 16777217u), new Vector3(29.2f, -3f, -81.7f)),
			[178u] = (new QCv6323yOHSYHfxkTo2(131084u, 16777216u), Vector3.Zero),
			[418u] = (new QCv6323yOHSYHfxkTo2(131204u, 16777217u), new Vector3(84.7f, 5f, 34.1f)),
			[429u] = (new QCv6323yOHSYHfxkTo2(131205u, 16777216u), Vector3.Zero),
			[628u] = (new QCv6323yOHSYHfxkTo2(131245u, 16777217u), new Vector3(-86.2f, 9f, -197.1f)),
			[629u] = (new QCv6323yOHSYHfxkTo2(131246u, 16777216u), Vector3.Zero),
			[819u] = (new QCv6323yOHSYHfxkTo2(131316u, 16777217u), new Vector3(49.4f, -8.3f, 230.9f)),
			[843u] = (new QCv6323yOHSYHfxkTo2(131312u, 16777216u), Vector3.Zero),
			[962u] = (new QCv6323yOHSYHfxkTo2(131401u, 16777217u), new Vector3(-100.2f, -6.1f, 3.4f)),
			[990u] = (new QCv6323yOHSYHfxkTo2(131402u, 16777216u), Vector3.Zero),
			[1185u] = (new QCv6323yOHSYHfxkTo2(131576u, 16777217u), new Vector3(-164.8f, -30.9f, 209.9f)),
			[1205u] = (new QCv6323yOHSYHfxkTo2(131578u, 16777216u), Vector3.Zero)
		}.ToFrozenDictionary();
	}
}
