using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Common.RemoteInteraction.Helpers;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.Blacklist;
using DailyRoutines.RemoteInteraction.Blacklist.Models;
using DailyRoutines.RemoteInteraction.Blacklist.Models.Responses;
using DailyRoutines.RemoteInteraction.PlayerInfo;
using DailyRoutines.RemoteInteraction.PlayerInfo.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class BetterBlacklist : ModuleBase
{
	private class JaqQJPpv7gKBM1Sa2ip : ContextMenuEntry
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_0
		{
			public ulong contentID;

			public JaqQJPpv7gKBM1Sa2ip _003C_003E4__this;

			public string playerName;

			public uint homeWorldID;

			public Func<W2sjNRpPqifh2MFEW32, bool> _003C_003E9__2;

			public _003C_003Ec__DisplayClass4_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal ContextMenuEntryInfo b5CPmFi1Bf(XDmpEqpiOaNG0YVioT2 list)
			{
				_003C_003Ec__DisplayClass4_1 CS_0024_003C_003E8__locals13 = new _003C_003Ec__DisplayClass4_1();
				CS_0024_003C_003E8__locals13.CS_0024_003C_003E8__locals1 = this;
				CS_0024_003C_003E8__locals13.list = list;
				return new ContextMenuEntryInfo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20B42B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372), delegate
				{
					//IL_0106: Unknown result type (might be due to invalid IL or missing references)
					bool flag = CS_0024_003C_003E8__locals13.list.Data.All((W2sjNRpPqifh2MFEW32 d) => d.PlayerContentID != CS_0024_003C_003E8__locals13.CS_0024_003C_003E8__locals1.contentID);
					return new ContextMenuItem
					{
						Name = ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE68371 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6), CS_0024_003C_003E8__locals13.list.Name) + (flag ? string.Empty : (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E4FD0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AD683E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B20B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)))),
						IsEnabled = flag,
						OnClicked = delegate
						{
							//IL_0122: Unknown result type (might be due to invalid IL or missing references)
							//IL_0128: Expected O, but got Unknown
							//IL_0136: Unknown result type (might be due to invalid IL or missing references)
							//IL_013b: Unknown result type (might be due to invalid IL or missing references)
							//IL_0141: Unknown result type (might be due to invalid IL or missing references)
							//IL_0148: Unknown result type (might be due to invalid IL or missing references)
							CS_0024_003C_003E8__locals13.list.Data.Add(new W2sjNRpPqifh2MFEW32
							{
								PlayerContentID = CS_0024_003C_003E8__locals13.CS_0024_003C_003E8__locals1.contentID,
								Remark = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C15A73B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50177ECC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf) + pHZ4qse9Kg() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23105143 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d),
								TimeUnix = GameState.ServerTimeUnix
							});
							CS_0024_003C_003E8__locals13.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.config.Save(CS_0024_003C_003E8__locals13.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP);
							CS_0024_003C_003E8__locals13.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.xao4FZlCLw();
							ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D76D9E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846), (object)new PlayerPayload(CS_0024_003C_003E8__locals13.CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals13.CS_0024_003C_003E8__locals1.homeWorldID), CS_0024_003C_003E8__locals13.list.Name);
							OmenServiceBase<NotifyHelper>.Instance().Chat(se);
							NotifyHelper.Toast(se);
						}
					};
				});
			}

			internal bool OKIPio6dsb(W2sjNRpPqifh2MFEW32 d)
			{
				return d.PlayerContentID != contentID;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_1
		{
			public XDmpEqpiOaNG0YVioT2 list;

			public _003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals1;

			public Action<ContextMenuItemClickedArgs> _003C_003E9__3;

			public _003C_003Ec__DisplayClass4_1()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal ContextMenuItem? PyOPPnRj6i(ContextMenuOpenedArgs _)
			{
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				bool flag = list.Data.All((W2sjNRpPqifh2MFEW32 d) => d.PlayerContentID != CS_0024_003C_003E8__locals1.contentID);
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE68371 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6), list.Name) + (flag ? string.Empty : (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E4FD0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AD683E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B20B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)))),
					IsEnabled = flag,
					OnClicked = delegate
					{
						//IL_0122: Unknown result type (might be due to invalid IL or missing references)
						//IL_0128: Expected O, but got Unknown
						//IL_0136: Unknown result type (might be due to invalid IL or missing references)
						//IL_013b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0141: Unknown result type (might be due to invalid IL or missing references)
						//IL_0148: Unknown result type (might be due to invalid IL or missing references)
						list.Data.Add(new W2sjNRpPqifh2MFEW32
						{
							PlayerContentID = CS_0024_003C_003E8__locals1.contentID,
							Remark = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C15A73B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50177ECC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf) + pHZ4qse9Kg() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23105143 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d),
							TimeUnix = GameState.ServerTimeUnix
						});
						CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.config.Save(CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP);
						CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.xao4FZlCLw();
						ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D76D9E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846), (object)new PlayerPayload(CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals1.homeWorldID), list.Name);
						OmenServiceBase<NotifyHelper>.Instance().Chat(se);
						NotifyHelper.Toast(se);
					}
				};
			}

			internal void JXPPh8fEfn(ContextMenuItemClickedArgs _)
			{
				//IL_0122: Unknown result type (might be due to invalid IL or missing references)
				//IL_0128: Expected O, but got Unknown
				//IL_0136: Unknown result type (might be due to invalid IL or missing references)
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Unknown result type (might be due to invalid IL or missing references)
				//IL_0148: Unknown result type (might be due to invalid IL or missing references)
				list.Data.Add(new W2sjNRpPqifh2MFEW32
				{
					PlayerContentID = CS_0024_003C_003E8__locals1.contentID,
					Remark = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C15A73B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50177ECC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf) + pHZ4qse9Kg() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23105143 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d),
					TimeUnix = GameState.ServerTimeUnix
				});
				CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.config.Save(CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP);
				CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.xao4FZlCLw();
				ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D76D9E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846), (object)new PlayerPayload(CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals1.homeWorldID), list.Name);
				OmenServiceBase<NotifyHelper>.Instance().Chat(se);
				NotifyHelper.Toast(se);
			}
		}

		public override string Identifier => LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC46E8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361);

		public JaqQJPpv7gKBM1Sa2ip(BetterBlacklist module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			_003Cmodule_003EP = module;
			base._002Ector();
		}

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass4_0();
			CS_0024_003C_003E8__locals7._003C_003E4__this = this;
			if (_003Cmodule_003EP.config.Lists.Count == 0 || !Gdl4cy7hkX(args, out CS_0024_003C_003E8__locals7.contentID, out CS_0024_003C_003E8__locals7.playerName, out CS_0024_003C_003E8__locals7.homeWorldID))
			{
				return null;
			}
			string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B314A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
			ContextMenuItem obj = new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(text)
			};
			ContextMenuSubmenu obj2 = new ContextMenuSubmenu
			{
				Title = ReadOnlySeString.op_Implicit(text)
			};
			List<ContextMenuEntry> list = new List<ContextMenuEntry>();
			list.AddRange(_003Cmodule_003EP.config.Lists.Select(delegate(XDmpEqpiOaNG0YVioT2 list2)
			{
				_003C_003Ec__DisplayClass4_1 CS_0024_003C_003E8__locals19 = new _003C_003Ec__DisplayClass4_1();
				CS_0024_003C_003E8__locals19.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals7;
				CS_0024_003C_003E8__locals19.list = list2;
				return new ContextMenuEntryInfo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20B42B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372), delegate
				{
					//IL_0106: Unknown result type (might be due to invalid IL or missing references)
					bool flag = CS_0024_003C_003E8__locals19.list.Data.All((W2sjNRpPqifh2MFEW32 d) => d.PlayerContentID != CS_0024_003C_003E8__locals19.CS_0024_003C_003E8__locals1.contentID);
					return new ContextMenuItem
					{
						Name = ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE68371 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6), CS_0024_003C_003E8__locals19.list.Name) + (flag ? string.Empty : (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E4FD0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AD683E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B20B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)))),
						IsEnabled = flag,
						OnClicked = delegate
						{
							//IL_0122: Unknown result type (might be due to invalid IL or missing references)
							//IL_0128: Expected O, but got Unknown
							//IL_0136: Unknown result type (might be due to invalid IL or missing references)
							//IL_013b: Unknown result type (might be due to invalid IL or missing references)
							//IL_0141: Unknown result type (might be due to invalid IL or missing references)
							//IL_0148: Unknown result type (might be due to invalid IL or missing references)
							CS_0024_003C_003E8__locals19.list.Data.Add(new W2sjNRpPqifh2MFEW32
							{
								PlayerContentID = CS_0024_003C_003E8__locals19.CS_0024_003C_003E8__locals1.contentID,
								Remark = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C15A73B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50177ECC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf) + pHZ4qse9Kg() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23105143 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d),
								TimeUnix = GameState.ServerTimeUnix
							});
							CS_0024_003C_003E8__locals19.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.config.Save(CS_0024_003C_003E8__locals19.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP);
							CS_0024_003C_003E8__locals19.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.xao4FZlCLw();
							ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D76D9E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846), (object)new PlayerPayload(CS_0024_003C_003E8__locals19.CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals19.CS_0024_003C_003E8__locals1.homeWorldID), CS_0024_003C_003E8__locals19.list.Name);
							OmenServiceBase<NotifyHelper>.Instance().Chat(se);
							NotifyHelper.Toast(se);
						}
					};
				});
			}));
			obj2.Entries = new global::_003C_003Ez__ReadOnlyList<ContextMenuEntry>(list);
			obj.Submenu = obj2;
			return obj;
		}
	}

	private class cxuPfCplntw3FUZYKkB : ContextMenuEntry
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_0
		{
			public ulong contentID;

			public cxuPfCplntw3FUZYKkB _003C_003E4__this;

			public string playerName;

			public uint homeWorldID;

			public Func<W2sjNRpPqifh2MFEW32, bool> _003C_003E9__2;

			public _003C_003Ec__DisplayClass4_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal ContextMenuEntryInfo kviPosVDvM(HJRFNrpmZeSERXoVlH4 group)
			{
				_003C_003Ec__DisplayClass4_1 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass4_1();
				CS_0024_003C_003E8__locals12.CS_0024_003C_003E8__locals1 = this;
				CS_0024_003C_003E8__locals12.group = group;
				return new ContextMenuEntryInfo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E5F84 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1), delegate
				{
					//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
					bool flag = CS_0024_003C_003E8__locals12.group.Data.All((W2sjNRpPqifh2MFEW32 d) => d.PlayerContentID != CS_0024_003C_003E8__locals12.CS_0024_003C_003E8__locals1.contentID);
					return new ContextMenuItem
					{
						Name = ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B2E0B0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae), CS_0024_003C_003E8__locals12.group.Name) + (flag ? string.Empty : (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E5D88 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD983EF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3CEF1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)))),
						IsEnabled = flag,
						OnClicked = delegate
						{
							_003C_003Ec__DisplayClass4_2 CS_0024_003C_003E8__locals24 = new _003C_003Ec__DisplayClass4_2();
							CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2 = CS_0024_003C_003E8__locals12;
							CS_0024_003C_003E8__locals24.newEntry = new BlacklistPeopleInfo
							{
								Player = CS_0024_003C_003E8__locals12.CS_0024_003C_003E8__locals1.contentID,
								Remark = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CDA04A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x514B0B2B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9) + pHZ4qse9Kg() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCA80C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361),
								Time = GameState.ServerTimeUnix,
								SourcePlayer = LocalPlayerState.ContentID,
								Status = 1
							};
							CS_0024_003C_003E8__locals24.cancellationToken = CS_0024_003C_003E8__locals12.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.cancelSource?.Token ?? CancellationToken.None;
							Task.Run(async delegate
							{
								_ = 1;
								try
								{
									if ((await RemoteBlacklist.UpdateAsync(CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.group.Identifier, CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(CS_0024_003C_003E8__locals24.newEntry), CS_0024_003C_003E8__locals24.cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Success)
									{
										IFramework obj = DalamudServiceExtension.Instance<IFramework>();
										Action action = delegate
										{
											//IL_005b: Unknown result type (might be due to invalid IL or missing references)
											//IL_0061: Expected O, but got Unknown
											//IL_006f: Unknown result type (might be due to invalid IL or missing references)
											//IL_0074: Unknown result type (might be due to invalid IL or missing references)
											//IL_007a: Unknown result type (might be due to invalid IL or missing references)
											//IL_0081: Unknown result type (might be due to invalid IL or missing references)
											CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.doJ4bUPgfD(CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.group);
											ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB7C01 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9), (object)new PlayerPayload(CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.homeWorldID), CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.group.Name);
											OmenServiceBase<NotifyHelper>.Instance().Chat(se);
											NotifyHelper.Toast(se);
										};
										CancellationToken cancellationToken = CS_0024_003C_003E8__locals24.cancellationToken;
										await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
									}
								}
								catch (Exception ex)
								{
									if (!CS_0024_003C_003E8__locals24.cancellationToken.IsCancellationRequested)
									{
										DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454F00B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467), ex);
									}
								}
							}, CS_0024_003C_003E8__locals24.cancellationToken);
						}
					};
				});
			}

			internal bool hTKPgFkVIg(W2sjNRpPqifh2MFEW32 d)
			{
				return d.PlayerContentID != contentID;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_1
		{
			public HJRFNrpmZeSERXoVlH4 group;

			public _003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals1;

			public Action _003C_003E9__5;

			public Action<ContextMenuItemClickedArgs> _003C_003E9__3;

			public _003C_003Ec__DisplayClass4_1()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal ContextMenuItem? y3gPwFtjWc(ContextMenuOpenedArgs openedArgs)
			{
				//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
				bool flag = group.Data.All((W2sjNRpPqifh2MFEW32 d) => d.PlayerContentID != CS_0024_003C_003E8__locals1.contentID);
				return new ContextMenuItem
				{
					Name = ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B2E0B0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae), group.Name) + (flag ? string.Empty : (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E5D88 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD983EF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3CEF1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)))),
					IsEnabled = flag,
					OnClicked = delegate
					{
						_003C_003Ec__DisplayClass4_2 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass4_2();
						CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2 = this;
						CS_0024_003C_003E8__locals15.newEntry = new BlacklistPeopleInfo
						{
							Player = CS_0024_003C_003E8__locals1.contentID,
							Remark = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CDA04A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x514B0B2B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9) + pHZ4qse9Kg() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCA80C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361),
							Time = GameState.ServerTimeUnix,
							SourcePlayer = LocalPlayerState.ContentID,
							Status = 1
						};
						CS_0024_003C_003E8__locals15.cancellationToken = CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.cancelSource?.Token ?? CancellationToken.None;
						Task.Run(async delegate
						{
							_ = 1;
							try
							{
								if ((await RemoteBlacklist.UpdateAsync(CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.group.Identifier, CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(CS_0024_003C_003E8__locals15.newEntry), CS_0024_003C_003E8__locals15.cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Success)
								{
									IFramework obj = DalamudServiceExtension.Instance<IFramework>();
									Action action = delegate
									{
										//IL_005b: Unknown result type (might be due to invalid IL or missing references)
										//IL_0061: Expected O, but got Unknown
										//IL_006f: Unknown result type (might be due to invalid IL or missing references)
										//IL_0074: Unknown result type (might be due to invalid IL or missing references)
										//IL_007a: Unknown result type (might be due to invalid IL or missing references)
										//IL_0081: Unknown result type (might be due to invalid IL or missing references)
										CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.doJ4bUPgfD(CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.group);
										ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB7C01 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9), (object)new PlayerPayload(CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.homeWorldID), CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.group.Name);
										OmenServiceBase<NotifyHelper>.Instance().Chat(se);
										NotifyHelper.Toast(se);
									};
									CancellationToken cancellationToken = CS_0024_003C_003E8__locals15.cancellationToken;
									await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
								}
							}
							catch (Exception ex)
							{
								if (!CS_0024_003C_003E8__locals15.cancellationToken.IsCancellationRequested)
								{
									DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454F00B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467), ex);
								}
							}
						}, CS_0024_003C_003E8__locals15.cancellationToken);
					}
				};
			}

			internal void zsWPEOsatV(ContextMenuItemClickedArgs arg)
			{
				_003C_003Ec__DisplayClass4_2 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass4_2();
				CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2 = this;
				CS_0024_003C_003E8__locals15.newEntry = new BlacklistPeopleInfo
				{
					Player = CS_0024_003C_003E8__locals1.contentID,
					Remark = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CDA04A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x514B0B2B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9) + pHZ4qse9Kg() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCA80C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361),
					Time = GameState.ServerTimeUnix,
					SourcePlayer = LocalPlayerState.ContentID,
					Status = 1
				};
				CS_0024_003C_003E8__locals15.cancellationToken = CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.cancelSource?.Token ?? CancellationToken.None;
				Task.Run(async delegate
				{
					_ = 1;
					try
					{
						if ((await RemoteBlacklist.UpdateAsync(CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.group.Identifier, CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(CS_0024_003C_003E8__locals15.newEntry), CS_0024_003C_003E8__locals15.cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Success)
						{
							IFramework obj = DalamudServiceExtension.Instance<IFramework>();
							Action action = delegate
							{
								//IL_005b: Unknown result type (might be due to invalid IL or missing references)
								//IL_0061: Expected O, but got Unknown
								//IL_006f: Unknown result type (might be due to invalid IL or missing references)
								//IL_0074: Unknown result type (might be due to invalid IL or missing references)
								//IL_007a: Unknown result type (might be due to invalid IL or missing references)
								//IL_0081: Unknown result type (might be due to invalid IL or missing references)
								CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.doJ4bUPgfD(CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.group);
								ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB7C01 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9), (object)new PlayerPayload(CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.homeWorldID), CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals2.group.Name);
								OmenServiceBase<NotifyHelper>.Instance().Chat(se);
								NotifyHelper.Toast(se);
							};
							CancellationToken cancellationToken = CS_0024_003C_003E8__locals15.cancellationToken;
							await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						}
					}
					catch (Exception ex)
					{
						if (!CS_0024_003C_003E8__locals15.cancellationToken.IsCancellationRequested)
						{
							DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454F00B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467), ex);
						}
					}
				}, CS_0024_003C_003E8__locals15.cancellationToken);
			}

			internal void pERPbYdmXo()
			{
				//IL_005b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Expected O, but got Unknown
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0074: Unknown result type (might be due to invalid IL or missing references)
				//IL_007a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0081: Unknown result type (might be due to invalid IL or missing references)
				CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.doJ4bUPgfD(group);
				ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB7C01 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9), (object)new PlayerPayload(CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals1.homeWorldID), group.Name);
				OmenServiceBase<NotifyHelper>.Instance().Chat(se);
				NotifyHelper.Toast(se);
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_2
		{
			public BlacklistPeopleInfo newEntry;

			public CancellationToken cancellationToken;

			public _003C_003Ec__DisplayClass4_1 CS_0024_003C_003E8__locals2;

			public _003C_003Ec__DisplayClass4_2()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal async Task? W4mPF8a80o()
			{
				_ = 1;
				try
				{
					if ((await RemoteBlacklist.UpdateAsync(CS_0024_003C_003E8__locals2.group.Identifier, CS_0024_003C_003E8__locals2.group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(newEntry), this.cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Success)
					{
						IFramework obj = DalamudServiceExtension.Instance<IFramework>();
						Action action = delegate
						{
							//IL_005b: Unknown result type (might be due to invalid IL or missing references)
							//IL_0061: Expected O, but got Unknown
							//IL_006f: Unknown result type (might be due to invalid IL or missing references)
							//IL_0074: Unknown result type (might be due to invalid IL or missing references)
							//IL_007a: Unknown result type (might be due to invalid IL or missing references)
							//IL_0081: Unknown result type (might be due to invalid IL or missing references)
							CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.doJ4bUPgfD(CS_0024_003C_003E8__locals2.group);
							ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB7C01 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9), (object)new PlayerPayload(CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.homeWorldID), CS_0024_003C_003E8__locals2.group.Name);
							OmenServiceBase<NotifyHelper>.Instance().Chat(se);
							NotifyHelper.Toast(se);
						};
						CancellationToken cancellationToken = this.cancellationToken;
						await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
				}
				catch (Exception ex)
				{
					if (!this.cancellationToken.IsCancellationRequested)
					{
						DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454F00B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467), ex);
					}
				}
			}
		}

		public override string Identifier => LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68540F7B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5);

		public cxuPfCplntw3FUZYKkB(BetterBlacklist module)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			_003Cmodule_003EP = module;
			base._002Ector();
		}

		public override ContextMenuItem? Create(ContextMenuOpenedArgs args)
		{
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			_003C_003Ec__DisplayClass4_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass4_0();
			CS_0024_003C_003E8__locals7._003C_003E4__this = this;
			if (_003Cmodule_003EP.config.Groups.Count == 0 || !Gdl4cy7hkX(args, out CS_0024_003C_003E8__locals7.contentID, out CS_0024_003C_003E8__locals7.playerName, out CS_0024_003C_003E8__locals7.homeWorldID))
			{
				return null;
			}
			string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017B4AC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
			ContextMenuItem obj = new ContextMenuItem
			{
				Name = ReadOnlySeString.op_Implicit(text)
			};
			ContextMenuSubmenu obj2 = new ContextMenuSubmenu
			{
				Title = ReadOnlySeString.op_Implicit(text)
			};
			List<ContextMenuEntry> list = new List<ContextMenuEntry>();
			list.AddRange(_003Cmodule_003EP.config.Groups.Select(delegate(HJRFNrpmZeSERXoVlH4 group)
			{
				_003C_003Ec__DisplayClass4_1 CS_0024_003C_003E8__locals18 = new _003C_003Ec__DisplayClass4_1();
				CS_0024_003C_003E8__locals18.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals7;
				CS_0024_003C_003E8__locals18.group = group;
				return new ContextMenuEntryInfo(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E5F84 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1), delegate
				{
					//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
					bool flag = CS_0024_003C_003E8__locals18.group.Data.All((W2sjNRpPqifh2MFEW32 d) => d.PlayerContentID != CS_0024_003C_003E8__locals18.CS_0024_003C_003E8__locals1.contentID);
					return new ContextMenuItem
					{
						Name = ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B2E0B0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae), CS_0024_003C_003E8__locals18.group.Name) + (flag ? string.Empty : (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E5D88 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD983EF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3CEF1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)))),
						IsEnabled = flag,
						OnClicked = delegate
						{
							_003C_003Ec__DisplayClass4_2 CS_0024_003C_003E8__locals30 = new _003C_003Ec__DisplayClass4_2();
							CS_0024_003C_003E8__locals30.CS_0024_003C_003E8__locals2 = CS_0024_003C_003E8__locals18;
							CS_0024_003C_003E8__locals30.newEntry = new BlacklistPeopleInfo
							{
								Player = CS_0024_003C_003E8__locals18.CS_0024_003C_003E8__locals1.contentID,
								Remark = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CDA04A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x514B0B2B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9) + pHZ4qse9Kg() + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCA80C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361),
								Time = GameState.ServerTimeUnix,
								SourcePlayer = LocalPlayerState.ContentID,
								Status = 1
							};
							CS_0024_003C_003E8__locals30.cancellationToken = CS_0024_003C_003E8__locals18.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.cancelSource?.Token ?? CancellationToken.None;
							Task.Run(async delegate
							{
								_ = 1;
								try
								{
									if ((await RemoteBlacklist.UpdateAsync(CS_0024_003C_003E8__locals30.CS_0024_003C_003E8__locals2.group.Identifier, CS_0024_003C_003E8__locals30.CS_0024_003C_003E8__locals2.group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(CS_0024_003C_003E8__locals30.newEntry), CS_0024_003C_003E8__locals30.cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Success)
									{
										IFramework obj3 = DalamudServiceExtension.Instance<IFramework>();
										Action action = delegate
										{
											//IL_005b: Unknown result type (might be due to invalid IL or missing references)
											//IL_0061: Expected O, but got Unknown
											//IL_006f: Unknown result type (might be due to invalid IL or missing references)
											//IL_0074: Unknown result type (might be due to invalid IL or missing references)
											//IL_007a: Unknown result type (might be due to invalid IL or missing references)
											//IL_0081: Unknown result type (might be due to invalid IL or missing references)
											CS_0024_003C_003E8__locals30.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this._003Cmodule_003EP.doJ4bUPgfD(CS_0024_003C_003E8__locals30.CS_0024_003C_003E8__locals2.group);
											ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB7C01 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9), (object)new PlayerPayload(CS_0024_003C_003E8__locals30.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.playerName, CS_0024_003C_003E8__locals30.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.homeWorldID), CS_0024_003C_003E8__locals30.CS_0024_003C_003E8__locals2.group.Name);
											OmenServiceBase<NotifyHelper>.Instance().Chat(se);
											NotifyHelper.Toast(se);
										};
										CancellationToken cancellationToken = CS_0024_003C_003E8__locals30.cancellationToken;
										await obj3.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
									}
								}
								catch (Exception ex)
								{
									if (!CS_0024_003C_003E8__locals30.cancellationToken.IsCancellationRequested)
									{
										DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454F00B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467), ex);
									}
								}
							}, CS_0024_003C_003E8__locals30.cancellationToken);
						}
					};
				});
			}));
			obj2.Entries = new global::_003C_003Ez__ReadOnlyList<ContextMenuEntry>(list);
			obj.Submenu = obj2;
			return obj;
		}
	}

	private unsafe delegate void mh9lehpx7SnAsTfLFcc(InfoProxyBlacklist* info, BlockResult* outResult, ulong accountID, ulong contentID);

	private unsafe delegate void xrQnY6ppEfhMHoQs3ot(AgentContext* agent, int eventID, uint addonTextID, bool isDisabled, bool isSubmenu);

	private class rbOc51p3JWlXYMAlsnE : ModuleConfig
	{
		public List<HJRFNrpmZeSERXoVlH4> Groups;

		public List<XDmpEqpiOaNG0YVioT2> Lists;

		public rbOc51p3JWlXYMAlsnE()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Groups = new List<HJRFNrpmZeSERXoVlH4>();
			Lists = new List<XDmpEqpiOaNG0YVioT2>();
			base._002Ector();
		}
	}

	private interface PsT8vDpBBOEatOCNQNn
	{
		string Name { get; set; }

		long ImportTimeUnix { get; set; }

		DateTime ImportTime { get; }

		List<W2sjNRpPqifh2MFEW32> Data { get; set; }

		bool IsRemote { get; }
	}

	private class HJRFNrpmZeSERXoVlH4 : PsT8vDpBBOEatOCNQNn
	{
		[CompilerGenerated]
		private DateTime _003CImportTime_003Ek__BackingField;

		public string Identifier { get; set; }

		public string Password { get; set; }

		public string Name { get; set; }

		[JsonIgnore]
		public bool IsRemote => true;

		public long ImportTimeUnix { get; set; }

		[JsonIgnore]
		public DateTime ImportTime
		{
			get
			{
				if (_003CImportTime_003Ek__BackingField != DateTime.MinValue)
				{
					return _003CImportTime_003Ek__BackingField;
				}
				if (ImportTimeUnix <= 0)
				{
					return DateTime.MinValue;
				}
				return _003CImportTime_003Ek__BackingField = ImportTimeUnix.ToUTCDateTimeFromUnixSeconds().ToLocalTime();
			}
		}

		[JsonIgnore]
		public List<W2sjNRpPqifh2MFEW32> Data { get; set; }

		public HJRFNrpmZeSERXoVlH4()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Identifier = string.Empty;
			Password = string.Empty;
			Name = string.Empty;
			_003CImportTime_003Ek__BackingField = DateTime.MinValue;
			Data = new List<W2sjNRpPqifh2MFEW32>();
			base._002Ector();
		}
	}

	private class XDmpEqpiOaNG0YVioT2 : PsT8vDpBBOEatOCNQNn
	{
		[CompilerGenerated]
		private DateTime _003CImportTime_003Ek__BackingField;

		public string Name { get; set; }

		[JsonIgnore]
		public bool IsRemote => false;

		public long ImportTimeUnix { get; set; }

		[JsonIgnore]
		public DateTime ImportTime
		{
			get
			{
				if (_003CImportTime_003Ek__BackingField != DateTime.MinValue)
				{
					return _003CImportTime_003Ek__BackingField;
				}
				if (ImportTimeUnix <= 0)
				{
					return DateTime.MinValue;
				}
				return _003CImportTime_003Ek__BackingField = ImportTimeUnix.ToUTCDateTimeFromUnixSeconds().ToLocalTime();
			}
		}

		public List<W2sjNRpPqifh2MFEW32> Data { get; set; }

		public XDmpEqpiOaNG0YVioT2()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Name = string.Empty;
			_003CImportTime_003Ek__BackingField = DateTime.MinValue;
			Data = new List<W2sjNRpPqifh2MFEW32>();
			base._002Ector();
		}
	}

	private class W2sjNRpPqifh2MFEW32
	{
		[CompilerGenerated]
		private DateTime _003CTime_003Ek__BackingField;

		[JsonProperty("time")]
		public long TimeUnix { get; set; }

		[JsonProperty("source_player")]
		public ulong SourcePlayerContentID { get; set; }

		[JsonProperty("player")]
		public ulong PlayerContentID { get; set; }

		[JsonProperty("remark")]
		public string Remark { get; set; }

		[JsonIgnore]
		public DateTime Time
		{
			get
			{
				if (_003CTime_003Ek__BackingField != DateTime.MinValue)
				{
					return _003CTime_003Ek__BackingField;
				}
				if (TimeUnix == 0L)
				{
					return DateTime.MinValue;
				}
				return _003CTime_003Ek__BackingField = TimeUnix.ToUTCDateTimeFromUnixSeconds().ToLocalTime();
			}
		}

		[JsonIgnore]
		public string PlayerName
		{
			get
			{
				if (!RUgpgfb8eR(PlayerContentID, out PlayerInfoValue value))
				{
					return string.Empty;
				}
				return value.Name;
			}
		}

		[JsonIgnore]
		public uint PlayerWorldID
		{
			get
			{
				if (!RUgpgfb8eR(PlayerContentID, out PlayerInfoValue value))
				{
					return 0u;
				}
				return value.WorldID;
			}
		}

		[JsonIgnore]
		public string PlayerWorldName
		{
			get
			{
				if (PlayerWorldID != 0)
				{
					return LuminaWrapper.GetWorldName(PlayerWorldID);
				}
				return string.Empty;
			}
		}

		[JsonIgnore]
		public string SourcePlayerName
		{
			get
			{
				if (!RUgpgfb8eR(SourcePlayerContentID, out PlayerInfoValue value))
				{
					return string.Empty;
				}
				return value.Name;
			}
		}

		[JsonIgnore]
		public uint SourcePlayerWorldID
		{
			get
			{
				if (!RUgpgfb8eR(SourcePlayerContentID, out PlayerInfoValue value))
				{
					return 0u;
				}
				return value.WorldID;
			}
		}

		[JsonIgnore]
		public string SourcePlayerWorldName
		{
			get
			{
				if (SourcePlayerWorldID != 0)
				{
					return LuminaWrapper.GetWorldName(SourcePlayerWorldID);
				}
				return string.Empty;
			}
		}

		public void OTWphlSIk5(CancellationToken cancellationToken = default(CancellationToken))
		{
			cmfpoYap1g(PlayerContentID, cancellationToken);
			cmfpoYap1g(SourcePlayerContentID, cancellationToken);
		}

		private static void cmfpoYap1g(ulong contentID, CancellationToken cancellationToken)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if (contentID != 0L)
			{
				RemotePlayerInfo.GetOrRequest(contentID, WorldRegionResolver.Resolve(GameState.ClientLanguage), cancellationToken);
			}
		}

		private static bool RUgpgfb8eR(ulong contentID, out PlayerInfoValue value)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (contentID == 0L)
			{
				value = null;
				return false;
			}
			return RemotePlayerInfo.TryGet(contentID, WorldRegionResolver.Resolve(GameState.ClientLanguage), out value);
		}

		public W2sjNRpPqifh2MFEW32()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Remark = string.Empty;
			_003CTime_003Ek__BackingField = DateTime.MinValue;
			base._002Ector();
		}
	}

	private struct G8Dg2DpwnugmrkESTYh
	{
		public string SourceName;

		public string SourceTypeLocKey;

		public string Remark;

		public long TimeUnix;
	}

	private sealed class C47amWpEw9DpQfUw7vG
	{
		public ulong ContentID;

		public ulong QueryingContentID;

		public PlayerInfoValue? Info;

		public bool IsInvalid;

		public void m42pbHPLXE()
		{
			ContentID = 0uL;
			QueryingContentID = 0uL;
			Info = null;
			IsInvalid = false;
		}

		public C47amWpEw9DpQfUw7vG()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass64_0
	{
		public BetterBlacklist _003C_003E4__this;

		public string importListID;

		public string importListPassword;

		public CancellationToken cancellationToken;

		public _003C_003Ec__DisplayClass64_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal async Task? cnepFhqTdW()
		{
			try
			{
				_003C_003Ec__DisplayClass64_1 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass64_1();
				CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1 = this;
				CS_0024_003C_003E8__locals15.result = await RemoteBlacklist.GetContentAsync(importListID, importListPassword, 1000, 0, this.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				IFramework obj = DalamudServiceExtension.Instance<IFramework>();
				Action action = delegate
				{
					//IL_0049: Unknown result type (might be due to invalid IL or missing references)
					//IL_017d: Unknown result type (might be due to invalid IL or missing references)
					if (CS_0024_003C_003E8__locals15.result.Entries.Count == 0)
					{
						string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68540E3B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
						OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
						NotifyHelper.ToastError(text);
					}
					else
					{
						XDmpEqpiOaNG0YVioT2 xDmpEqpiOaNG0YVioT = new XDmpEqpiOaNG0YVioT2
						{
							Name = CS_0024_003C_003E8__locals15.result.Info.Name,
							ImportTimeUnix = GameState.ServerTimeUnix,
							Data = CS_0024_003C_003E8__locals15.result.Entries.Select((BlacklistEntry x) => new W2sjNRpPqifh2MFEW32
							{
								PlayerContentID = x.Player,
								Remark = x.Remark,
								TimeUnix = x.Time
							}).ToList()
						};
						CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1._003C_003E4__this.config.Lists.Add(xDmpEqpiOaNG0YVioT);
						CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1._003C_003E4__this.config.Save(CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1._003C_003E4__this);
						CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1._003C_003E4__this.importListIDInput = string.Empty;
						CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1._003C_003E4__this.importListPasswordInput = string.Empty;
						CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1._003C_003E4__this.xao4FZlCLw();
						string text2 = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F2232 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), xDmpEqpiOaNG0YVioT.Name, xDmpEqpiOaNG0YVioT.Data.Count);
						OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text2));
						NotifyHelper.Toast(text2);
						CS_0024_003C_003E8__locals15.CS_0024_003C_003E8__locals1._003C_003E4__this.selectedSource = xDmpEqpiOaNG0YVioT;
					}
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				_003C_003Ec__DisplayClass64_2 CS_0024_003C_003E8__locals16 = new _003C_003Ec__DisplayClass64_2();
				CS_0024_003C_003E8__locals16.ex = ex;
				if (this.cancellationToken.IsCancellationRequested)
				{
					return;
				}
				DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CDA0D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361), CS_0024_003C_003E8__locals16.ex);
				IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
				Action action2 = delegate
				{
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6B0E8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), CS_0024_003C_003E8__locals16.ex.Message);
					OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
					NotifyHelper.ToastError(text);
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj2.RunOnTick(action2, default(TimeSpan), 0, cancellationToken);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass64_1
	{
		public BlacklistContentValue result;

		public _003C_003Ec__DisplayClass64_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass64_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void sLspqTVIG6()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			if (result.Entries.Count == 0)
			{
				string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68540E3B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
				OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
				NotifyHelper.ToastError(text);
				return;
			}
			XDmpEqpiOaNG0YVioT2 xDmpEqpiOaNG0YVioT = new XDmpEqpiOaNG0YVioT2
			{
				Name = result.Info.Name,
				ImportTimeUnix = GameState.ServerTimeUnix,
				Data = result.Entries.Select((BlacklistEntry x) => new W2sjNRpPqifh2MFEW32
				{
					PlayerContentID = x.Player,
					Remark = x.Remark,
					TimeUnix = x.Time
				}).ToList()
			};
			CS_0024_003C_003E8__locals1._003C_003E4__this.config.Lists.Add(xDmpEqpiOaNG0YVioT);
			CS_0024_003C_003E8__locals1._003C_003E4__this.config.Save(CS_0024_003C_003E8__locals1._003C_003E4__this);
			CS_0024_003C_003E8__locals1._003C_003E4__this.importListIDInput = string.Empty;
			CS_0024_003C_003E8__locals1._003C_003E4__this.importListPasswordInput = string.Empty;
			CS_0024_003C_003E8__locals1._003C_003E4__this.xao4FZlCLw();
			string text2 = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F2232 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), xDmpEqpiOaNG0YVioT.Name, xDmpEqpiOaNG0YVioT.Data.Count);
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text2));
			NotifyHelper.Toast(text2);
			CS_0024_003C_003E8__locals1._003C_003E4__this.selectedSource = xDmpEqpiOaNG0YVioT;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass64_2
	{
		public Exception ex;

		public _003C_003Ec__DisplayClass64_2()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void EWXpcbWQy1()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6B0E8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), ex.Message);
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
			NotifyHelper.ToastError(text);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass65_0
	{
		public BetterBlacklist _003C_003E4__this;

		public string name;

		public string password;

		public CancellationToken cancellationToken;

		public _003C_003Ec__DisplayClass65_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal async Task? fmcpAy4KlT()
		{
			try
			{
				_003C_003Ec__DisplayClass65_1 CS_0024_003C_003E8__locals20 = new _003C_003Ec__DisplayClass65_1();
				CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1 = this;
				CS_0024_003C_003E8__locals20.result = await RemoteBlacklist.CreateAsync(name, password, this.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				IFramework obj = DalamudServiceExtension.Instance<IFramework>();
				Action action = delegate
				{
					//IL_005e: Unknown result type (might be due to invalid IL or missing references)
					//IL_017d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0193: Unknown result type (might be due to invalid IL or missing references)
					if (!CS_0024_003C_003E8__locals20.result.Success)
					{
						string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B3688 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded), CS_0024_003C_003E8__locals20.result.Message);
						NotifyHelper.ToastError(text);
						OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
					}
					else
					{
						HJRFNrpmZeSERXoVlH4 hJRFNrpmZeSERXoVlH = new HJRFNrpmZeSERXoVlH4
						{
							Identifier = CS_0024_003C_003E8__locals20.result.BlacklistID,
							Password = CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1.password,
							Name = CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1.name,
							ImportTimeUnix = GameState.ServerTimeUnix
						};
						CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1._003C_003E4__this.config.Groups.Add(hJRFNrpmZeSERXoVlH);
						CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1._003C_003E4__this.config.Save(CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1._003C_003E4__this);
						CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1._003C_003E4__this.doJ4bUPgfD(hJRFNrpmZeSERXoVlH);
						CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1._003C_003E4__this.newGroupNameInput = string.Empty;
						CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1._003C_003E4__this.newGroupPasswordInput = string.Empty;
						OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F365006 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2), CS_0024_003C_003E8__locals20.result.BlacklistID, CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1.password) ?? ""));
						ImGui.SetClipboardText(ImU8String.op_Implicit(CS_0024_003C_003E8__locals20.result.BlacklistID));
						CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1._003C_003E4__this.selectedSource = hJRFNrpmZeSERXoVlH;
					}
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				_003C_003Ec__DisplayClass65_2 CS_0024_003C_003E8__locals21 = new _003C_003Ec__DisplayClass65_2();
				CS_0024_003C_003E8__locals21.ex = ex;
				if (this.cancellationToken.IsCancellationRequested)
				{
					return;
				}
				DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3AD68A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded), CS_0024_003C_003E8__locals21.ex);
				IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
				Action action2 = delegate
				{
					//IL_0048: Unknown result type (might be due to invalid IL or missing references)
					string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF10A56 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912), CS_0024_003C_003E8__locals21.ex.Message);
					NotifyHelper.ToastError(text);
					OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj2.RunOnTick(action2, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass65_1
	{
		public CreateBlacklistResult result;

		public _003C_003Ec__DisplayClass65_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass65_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void ehnpQrfyee()
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			if (!result.Success)
			{
				string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B3688 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded), result.Message);
				NotifyHelper.ToastError(text);
				OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
				return;
			}
			HJRFNrpmZeSERXoVlH4 hJRFNrpmZeSERXoVlH = new HJRFNrpmZeSERXoVlH4
			{
				Identifier = result.BlacklistID,
				Password = CS_0024_003C_003E8__locals1.password,
				Name = CS_0024_003C_003E8__locals1.name,
				ImportTimeUnix = GameState.ServerTimeUnix
			};
			CS_0024_003C_003E8__locals1._003C_003E4__this.config.Groups.Add(hJRFNrpmZeSERXoVlH);
			CS_0024_003C_003E8__locals1._003C_003E4__this.config.Save(CS_0024_003C_003E8__locals1._003C_003E4__this);
			CS_0024_003C_003E8__locals1._003C_003E4__this.doJ4bUPgfD(hJRFNrpmZeSERXoVlH);
			CS_0024_003C_003E8__locals1._003C_003E4__this.newGroupNameInput = string.Empty;
			CS_0024_003C_003E8__locals1._003C_003E4__this.newGroupPasswordInput = string.Empty;
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F365006 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2), result.BlacklistID, CS_0024_003C_003E8__locals1.password) ?? ""));
			ImGui.SetClipboardText(ImU8String.op_Implicit(result.BlacklistID));
			CS_0024_003C_003E8__locals1._003C_003E4__this.selectedSource = hJRFNrpmZeSERXoVlH;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass65_2
	{
		public Exception ex;

		public _003C_003Ec__DisplayClass65_2()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void OV9pJZ3Iwi()
		{
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF10A56 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912), ex.Message);
			NotifyHelper.ToastError(text);
			OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass70_0
	{
		public ulong contentID;

		public CancellationToken cancellationToken;

		public C47amWpEw9DpQfUw7vG state;

		public Action _003C_003E9__2;

		public _003C_003Ec__DisplayClass70_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal async Task? s8tprwdCjJ()
		{
			try
			{
				_003C_003Ec__DisplayClass70_1 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass70_1();
				CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals1 = this;
				CS_0024_003C_003E8__locals6.info = await RemotePlayerInfo.GetFreshAsync(contentID, WorldRegionResolver.Resolve(GameState.ClientLanguage), this.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				IFramework obj = DalamudServiceExtension.Instance<IFramework>();
				Action action = delegate
				{
					if (CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals1.state.ContentID == CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals1.contentID)
					{
						CS_0024_003C_003E8__locals6.CS_0024_003C_003E8__locals1.state.Info = CS_0024_003C_003E8__locals6.info;
					}
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception)
			{
				if (this.cancellationToken.IsCancellationRequested)
				{
					return;
				}
				IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
				Action action2 = delegate
				{
					if (state.ContentID == contentID)
					{
						state.IsInvalid = true;
					}
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj2.RunOnTick(action2, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		internal void MEIpGxm8Ip()
		{
			if (state.ContentID == contentID)
			{
				state.IsInvalid = true;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass70_1
	{
		public PlayerInfoValue info;

		public _003C_003Ec__DisplayClass70_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass70_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void uAwp6QAEMA()
		{
			if (CS_0024_003C_003E8__locals1.state.ContentID == CS_0024_003C_003E8__locals1.contentID)
			{
				CS_0024_003C_003E8__locals1.state.Info = info;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass72_0
	{
		public BetterBlacklist _003C_003E4__this;

		public HJRFNrpmZeSERXoVlH4 group;

		public _003C_003Ec__DisplayClass72_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool VCipkBkCG0(W2sjNRpPqifh2MFEW32 x)
		{
			return x.PlayerContentID != _003C_003E4__this.groupNewPlayerInput.ContentID;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass72_1
	{
		public PlayerInfoValue info;

		public BlacklistPeopleInfo newEntry;

		public CancellationToken cancellationToken;

		public _003C_003Ec__DisplayClass72_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass72_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal async Task? QPLp2ZOhfV()
		{
			_ = 1;
			try
			{
				_003C_003Ec__DisplayClass72_2 CS_0024_003C_003E8__locals9 = new _003C_003Ec__DisplayClass72_2();
				CS_0024_003C_003E8__locals9.CS_0024_003C_003E8__locals2 = this;
				CS_0024_003C_003E8__locals9.result = await RemoteBlacklist.UpdateAsync(CS_0024_003C_003E8__locals1.group.Identifier, CS_0024_003C_003E8__locals1.group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(newEntry), this.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				IFramework obj = DalamudServiceExtension.Instance<IFramework>();
				Action action = delegate
				{
					//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e2: Expected O, but got Unknown
					//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
					//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
					//IL_0105: Unknown result type (might be due to invalid IL or missing references)
					//IL_010c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0068: Unknown result type (might be due to invalid IL or missing references)
					if (!CS_0024_003C_003E8__locals9.result.Success)
					{
						string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20B345 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372), CS_0024_003C_003E8__locals9.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.group.Name);
						NotifyHelper.ToastError(text);
						OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
					}
					else
					{
						CS_0024_003C_003E8__locals9.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this.doJ4bUPgfD(CS_0024_003C_003E8__locals9.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.group);
						ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C930E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948), (object)new PlayerPayload(CS_0024_003C_003E8__locals9.CS_0024_003C_003E8__locals2.info.Name, CS_0024_003C_003E8__locals9.CS_0024_003C_003E8__locals2.info.WorldID), CS_0024_003C_003E8__locals9.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.group.Name);
						OmenServiceBase<NotifyHelper>.Instance().Chat(se);
						NotifyHelper.Toast(se);
					}
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				if (!this.cancellationToken.IsCancellationRequested)
				{
					DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AD6892 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09), ex);
				}
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass72_2
	{
		public UpdateBlacklistResult result;

		public _003C_003Ec__DisplayClass72_1 CS_0024_003C_003E8__locals2;

		public _003C_003Ec__DisplayClass72_2()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void uFhpZ83GSr()
		{
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Expected O, but got Unknown
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			if (!result.Success)
			{
				string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20B345 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372), CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.group.Name);
				NotifyHelper.ToastError(text);
				OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
			}
			else
			{
				CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this.doJ4bUPgfD(CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.group);
				ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C930E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948), (object)new PlayerPayload(CS_0024_003C_003E8__locals2.info.Name, CS_0024_003C_003E8__locals2.info.WorldID), CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.group.Name);
				OmenServiceBase<NotifyHelper>.Instance().Chat(se);
				NotifyHelper.Toast(se);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass73_0
	{
		public XDmpEqpiOaNG0YVioT2 list;

		public string password;

		public DateTime expireTime;

		public List<SharedBlacklistEntry> data;

		public CancellationToken cancellationToken;

		public _003C_003Ec__DisplayClass73_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal async Task? oBppaKjnth()
		{
			try
			{
				_003C_003Ec__DisplayClass73_1 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass73_1();
				CS_0024_003C_003E8__locals11.CS_0024_003C_003E8__locals1 = this;
				CS_0024_003C_003E8__locals11.result = await RemoteBlacklist.CreateSharedAsync(list.Name, password, expireTime, data, this.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				IFramework obj = DalamudServiceExtension.Instance<IFramework>();
				Action action = delegate
				{
					//IL_0058: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
					if (!CS_0024_003C_003E8__locals11.result.Success)
					{
						string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D40AB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e), CS_0024_003C_003E8__locals11.result.Message);
						OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
						NotifyHelper.ToastError(text);
					}
					else
					{
						OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0xD246686 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361), CS_0024_003C_003E8__locals11.result.BlacklistID, CS_0024_003C_003E8__locals11.CS_0024_003C_003E8__locals1.password, CS_0024_003C_003E8__locals11.CS_0024_003C_003E8__locals1.expireTime) ?? ""));
						ImGui.SetClipboardText(ImU8String.op_Implicit(CS_0024_003C_003E8__locals11.result.BlacklistID));
					}
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				_003C_003Ec__DisplayClass73_2 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass73_2();
				CS_0024_003C_003E8__locals12.ex = ex;
				if (this.cancellationToken.IsCancellationRequested)
				{
					return;
				}
				DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D0BF6F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c), CS_0024_003C_003E8__locals12.ex);
				IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
				Action action2 = delegate
				{
					OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D41D9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE50F1F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7) + CS_0024_003C_003E8__locals12.ex.Message);
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj2.RunOnTick(action2, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass73_1
	{
		public CreateSharedBlacklistResult result;

		public _003C_003Ec__DisplayClass73_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass73_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void xmBp0LQecQ()
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			if (!result.Success)
			{
				string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D40AB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e), result.Message);
				OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
				NotifyHelper.ToastError(text);
			}
			else
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0xD246686 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361), result.BlacklistID, CS_0024_003C_003E8__locals1.password, CS_0024_003C_003E8__locals1.expireTime) ?? ""));
				ImGui.SetClipboardText(ImU8String.op_Implicit(result.BlacklistID));
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass73_2
	{
		public Exception ex;

		public _003C_003Ec__DisplayClass73_2()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void N2Zp5LXY69()
		{
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D41D9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE50F1F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7) + ex.Message);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass75_0
	{
		public BetterBlacklist _003C_003E4__this;

		public string identifier;

		public string password;

		public CancellationToken cancellationToken;

		public Func<List<BlacklistLogEntry>> _003C_003E9__2;

		public _003C_003Ec__DisplayClass75_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal async Task? baVpRTGl59()
		{
			try
			{
				_003C_003Ec__DisplayClass75_1 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass75_1();
				CS_0024_003C_003E8__locals7.CS_0024_003C_003E8__locals1 = this;
				CS_0024_003C_003E8__locals7.response = await RemoteBlacklist.GetLogsAsync(identifier, password, null, null, 100, null, null, this.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				IFramework obj = DalamudServiceExtension.Instance<IFramework>();
				Action action = delegate
				{
					List<BlacklistLogEntry> list = (CS_0024_003C_003E8__locals7.response.Success ? (CS_0024_003C_003E8__locals7.response.Logs ?? new List<BlacklistLogEntry>()) : new List<BlacklistLogEntry>());
					foreach (BlacklistLogEntry item in list)
					{
						item.WarmupResolvedPlayersAsync(CS_0024_003C_003E8__locals7.CS_0024_003C_003E8__locals1.cancellationToken);
					}
					CS_0024_003C_003E8__locals7.CS_0024_003C_003E8__locals1._003C_003E4__this.groupLogsCache[CS_0024_003C_003E8__locals7.CS_0024_003C_003E8__locals1.identifier] = list;
				};
				CancellationToken cancellationToken = this.cancellationToken;
				await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				if (this.cancellationToken.IsCancellationRequested)
				{
					return;
				}
				DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF0EBB6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912), ex);
				IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
				Func<List<BlacklistLogEntry>> func = () => _003C_003E4__this.groupLogsCache[identifier] = new List<BlacklistLogEntry>();
				CancellationToken cancellationToken = this.cancellationToken;
				await obj2.RunOnTick<List<BlacklistLogEntry>>(func, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
		}

		internal List<BlacklistLogEntry> tFdpDBk13l()
		{
			return _003C_003E4__this.groupLogsCache[identifier] = new List<BlacklistLogEntry>();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass75_1
	{
		public BlacklistLogsResponse response;

		public _003C_003Ec__DisplayClass75_0 CS_0024_003C_003E8__locals1;

		public _003C_003Ec__DisplayClass75_1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void eH5peQMX5G()
		{
			List<BlacklistLogEntry> list = (response.Success ? (response.Logs ?? new List<BlacklistLogEntry>()) : new List<BlacklistLogEntry>());
			foreach (BlacklistLogEntry item in list)
			{
				item.WarmupResolvedPlayersAsync(CS_0024_003C_003E8__locals1.cancellationToken);
			}
			CS_0024_003C_003E8__locals1._003C_003E4__this.groupLogsCache[CS_0024_003C_003E8__locals1.identifier] = list;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass77_0
	{
		public HJRFNrpmZeSERXoVlH4 group;

		public BlacklistPeopleInfo updateEntry;

		public CancellationToken cancellationToken;

		public W2sjNRpPqifh2MFEW32 player;

		public string remark;

		public Func<string> _003C_003E9__1;

		public _003C_003Ec__DisplayClass77_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal async Task? fdkpNBrm18()
		{
			_ = 1;
			try
			{
				if ((await RemoteBlacklist.UpdateAsync(group.Identifier, group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(updateEntry), this.cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Success)
				{
					IFramework obj = DalamudServiceExtension.Instance<IFramework>();
					Func<string> func = () => player.Remark = remark;
					CancellationToken cancellationToken = this.cancellationToken;
					await obj.RunOnTick<string>(func, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch (Exception ex)
			{
				if (!this.cancellationToken.IsCancellationRequested)
				{
					DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6855E8DB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5), ex);
				}
			}
		}

		internal string xiXp7xT0cI()
		{
			return player.Remark = remark;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass78_0
	{
		public HJRFNrpmZeSERXoVlH4 group;

		public BlacklistPeopleInfo removeEntry;

		public CancellationToken cancellationToken;

		public BetterBlacklist _003C_003E4__this;

		public Action _003C_003E9__1;

		public _003C_003Ec__DisplayClass78_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal async Task? JHmpdGCo9i()
		{
			_ = 1;
			try
			{
				if ((await RemoteBlacklist.UpdateAsync(group.Identifier, group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(removeEntry), this.cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Success)
				{
					IFramework obj = DalamudServiceExtension.Instance<IFramework>();
					Action action = delegate
					{
						_003C_003E4__this.doJ4bUPgfD(group);
					};
					CancellationToken cancellationToken = this.cancellationToken;
					await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch (Exception ex)
			{
				if (!this.cancellationToken.IsCancellationRequested)
				{
					DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489EF2BA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414), ex);
				}
			}
		}

		internal void NFlpIkcmWv()
		{
			_003C_003E4__this.doJ4bUPgfD(group);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass80_0
	{
		public HJRFNrpmZeSERXoVlH4 group;

		public BetterBlacklist _003C_003E4__this;

		public Action _003C_003E9__2;

		public _003C_003Ec__DisplayClass80_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void kPXpU4uFD9(RemoteSnapshot<BlacklistContentValue> snapshot)
		{
			try
			{
				if (!snapshot.HasValue)
				{
					return;
				}
				BlacklistContentValue value = snapshot.Value;
				if ((object)value != null)
				{
					group.Data.Clear();
					group.Data.AddRange(value.Entries.Select((BlacklistEntry x) => new W2sjNRpPqifh2MFEW32
					{
						PlayerContentID = x.Player,
						Remark = x.Remark,
						TimeUnix = x.Time,
						SourcePlayerContentID = x.SourcePlayer
					}));
					_003C_003E4__this.TaskHelper.Abort();
					_003C_003E4__this.TaskHelper.Enqueue(delegate
					{
						_003C_003E4__this.config.Save(_003C_003E4__this);
					});
					_003C_003E4__this.TaskHelper.Enqueue((Action)_003C_003E4__this.xao4FZlCLw, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
				}
			}
			catch (Exception ex)
			{
				DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5FE84 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd), ex);
			}
		}

		internal void j6ppuSEFgN()
		{
			_003C_003E4__this.config.Save(_003C_003E4__this);
		}
	}

	private JaqQJPpv7gKBM1Sa2ip addToListMenu;

	private cxuPfCplntw3FUZYKkB addToGroupMenu;

	private static readonly CompSig GetBlockedResultSig;

	private Hook<mh9lehpx7SnAsTfLFcc>? GetBlockedResultHook;

	private static readonly CompSig AddContextMenuSig;

	private Hook<xrQnY6ppEfhMHoQs3ot>? AddContextMenuHook;

	private MemoryPatch contextMenuPatch0;

	private MemoryPatch contextMenuPatch1;

	private rbOc51p3JWlXYMAlsnE config;

	private CancellationTokenSource? cancelSource;

	private TextButtonNode? blacklistButtonNode;

	private readonly Dictionary<string, IDisposable> groupObservers;

	private readonly HashSet<ulong> blockedContentIDs;

	private readonly Dictionary<ulong, G8Dg2DpwnugmrkESTYh> blockedPlayerMeta;

	private PsT8vDpBBOEatOCNQNn? selectedSource;

	private string editNameInput;

	private string newGroupNameInput;

	private string newGroupPasswordInput;

	private string importGroupIDInput;

	private string importGroupPasswordInput;

	private string newListNameInput;

	private string importListIDInput;

	private string importListPasswordInput;

	private readonly C47amWpEw9DpQfUw7vG listNewPlayerInput;

	private string listNewPlayerRemarkInput;

	private string listSharePasswordInput;

	private int listShareExpireTimeDayInput;

	private readonly C47amWpEw9DpQfUw7vG groupNewPlayerInput;

	private string groupNewPlayerRemarkInput;

	private readonly Dictionary<string, bool> groupLogsVisibility;

	private readonly Dictionary<string, List<BlacklistLogEntry>> groupLogsCache;

	private string playerSearchFilter;

	private readonly Dictionary<ulong, long> lastRemarkEditTime;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Expected O, but got Unknown
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper taskHelper = (base.TaskHelper = new TaskHelper());
		}
		cancelSource = new CancellationTokenSource();
		contextMenuPatch0 = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FB3F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414), new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		contextMenuPatch1 = new MemoryPatch(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x1D602F0E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09), new global::_003C_003Ez__ReadOnlySingleElementList<byte?>((byte)235));
		addToListMenu = new JaqQJPpv7gKBM1Sa2ip(this);
		addToGroupMenu = new cxuPfCplntw3FUZYKkB(this);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(addToListMenu);
		OmenServiceBase<ContextMenuManager>.Instance().Reg(addToGroupMenu);
		rbOc51p3JWlXYMAlsnE obj = ModuleConfigExtension.Load<rbOc51p3JWlXYMAlsnE>(this);
		if (obj == null)
		{
			obj = new rbOc51p3JWlXYMAlsnE();
			rbOc51p3JWlXYMAlsnE obj2 = obj;
			int num = 1;
			List<XDmpEqpiOaNG0YVioT2> list = new List<XDmpEqpiOaNG0YVioT2>(num);
			CollectionsMarshal.SetCount(list, num);
			CollectionsMarshal.AsSpan(list)[0] = new XDmpEqpiOaNG0YVioT2
			{
				Name = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC97E14 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37))
			};
			obj2.Lists = list;
		}
		config = obj;
		if (config.Lists.Count == 0)
		{
			config.Lists.Add(new XDmpEqpiOaNG0YVioT2
			{
				Name = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8B815 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3))
			});
			config.Save(this);
		}
		GetBlockedResultHook = GetBlockedResultSig.GetHook<mh9lehpx7SnAsTfLFcc>(xNw4jk0MtZ);
		GetBlockedResultHook.Enable();
		AddContextMenuHook = AddContextMenuSig.GetHook<xrQnY6ppEfhMHoQs3ot>(x914OrO4la);
		AddContextMenuHook.Enable();
		contextMenuPatch0.Enable();
		contextMenuPatch1.Enable();
		r5f4E1f8lp();
		if (config.Lists.Count > 0)
		{
			selectedSource = config.Lists[0];
		}
		else if (config.Groups.Count > 0)
		{
			selectedSource = config.Groups[0];
		}
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += cH04ChbAEW;
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround += rnD4KkUWHp;
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)5, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4777F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7), new AddonEventDelegate(E4O4fKsNCe));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56C540 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1), new AddonEventDelegate(E4O4fKsNCe));
	}

	protected override void Uninit()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		cancelSource?.Cancel();
		cancelSource?.Dispose();
		cancelSource = null;
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { E4O4fKsNCe });
		blacklistButtonNode?.Dispose();
		blacklistButtonNode = null;
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround -= rnD4KkUWHp;
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= cH04ChbAEW;
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(addToListMenu);
		OmenServiceBase<ContextMenuManager>.Instance().Unreg(addToGroupMenu);
		foreach (IDisposable value in groupObservers.Values)
		{
			value.Dispose();
		}
		groupObservers.Clear();
		groupLogsVisibility.Clear();
		groupLogsCache.Clear();
		lastRemarkEditTime.Clear();
	}

	private unsafe void E4O4fKsNCe(AddonEvent type, AddonArgs args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if ((int)type != 5)
		{
			if ((int)type == 6)
			{
				blacklistButtonNode = null;
			}
		}
		else if (Addons.BlackList != null && blacklistButtonNode == null)
		{
			blacklistButtonNode = new TextButtonNode
			{
				Position = new Vector2(10f, 8f),
				Size = new Vector2(140f, 28f),
				IsVisible = true,
				String = ReadOnlySeString.op_Implicit(Info.Title),
				OnClick = delegate
				{
					OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x219410E4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b) + Info.Title);
				}
			};
			((AtkResNode)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)Addons.BlackList).WindowNode).Component).UldManager)).SearchNodeById(3u)).ToggleVisibility(false);
			((AtkResNode)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)Addons.BlackList).WindowNode).Component).UldManager)).SearchNodeById(4u)).ToggleVisibility(false);
			blacklistButtonNode.LabelNode.FontSize = 18u;
			blacklistButtonNode.Size = new Vector2(blacklistButtonNode.LabelNode.GetTextDrawSize(blacklistButtonNode.String).X + 56f, 36f);
			blacklistButtonNode.AttachNode(((AtkUnitBase)Addons.BlackList).RootNode);
		}
	}

	private void cH04ChbAEW(uint u)
	{
		foreach (HJRFNrpmZeSERXoVlH4 group in config.Groups)
		{
			doJ4bUPgfD(group);
		}
	}

	private unsafe void xNw4jk0MtZ(InfoProxyBlacklist* info, BlockResult* outResult, ulong accountID, ulong contentID)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		GetBlockedResultHook.Original(info, outResult, accountID, contentID);
		if ((int)((BlockResult)outResult).Type == 1)
		{
			Unsafe.Write(&((BlockResult)outResult).Type, (BlockResultType)(blockedContentIDs.Contains(contentID) ? 3 : ((int)((BlockResult)outResult).Type)));
		}
	}

	private unsafe void x914OrO4la(AgentContext* agent, int eventID, uint addonTextID, bool isDisabled, bool isSubmenu)
	{
		if (eventID != 29 && addonTextID != 59)
		{
			AddContextMenuHook.Original(agent, eventID, addonTextID, isDisabled, isSubmenu);
		}
	}

	private void rnD4KkUWHp(IReadOnlyList<IPlayerCharacter> characters)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		foreach (IPlayerCharacter character in characters)
		{
			if (blockedPlayerMeta.TryGetValue(character.ContentID, out var value))
			{
				Throttler<string> shared = Throttler.Shared;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1AB30 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
				defaultInterpolatedStringHandler.AppendFormatted(character.ContentID);
				if (shared.Throttle(defaultInterpolatedStringHandler.ToStringAndClear(), 300000u))
				{
					ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -1711208582 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1), LanguageManager.Get(value.SourceTypeLocKey), value.SourceName, (object)new PlayerPayload(character.Name, character.HomeWorld.RowId), value.Remark, value.TimeUnix.ToUTCDateTimeFromUnixSeconds().ToLocalTime().ToString(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EBDC51 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)));
					OmenServiceBase<NotifyHelper>.Instance().Chat(se);
				}
			}
		}
	}

	protected override void ConfigUI()
	{
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		if (selectedSource is XDmpEqpiOaNG0YVioT2 item && !config.Lists.Contains(item))
		{
			selectedSource = null;
		}
		else if (selectedSource is HJRFNrpmZeSERXoVlH4 item2 && !config.Groups.Contains(item2))
		{
			selectedSource = null;
		}
		if (selectedSource == null)
		{
			if (config.Lists.Count > 0)
			{
				selectedSource = config.Lists[0];
			}
			else if (config.Groups.Count > 0)
			{
				selectedSource = config.Groups[0];
			}
		}
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351DC5C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08)), 2, (ImGuiTableFlags)513);
		try
		{
			if (!(!val))
			{
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E55F48 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)), (ImGuiTableColumnFlags)8, 220f * Globals.GlobalUIScale, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17380B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)), (ImGuiTableColumnFlags)4, 0f, 0u);
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				zUs4WJbE3g();
				ImGui.TableNextColumn();
				Svl4T97uZC();
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private static bool Jbo4ts8VL3(string id, FontAwesomeIcon icon)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)21, 0u, true);
		try
		{
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)22, 1090519039u, true);
			try
			{
				ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)23, 1627389951u, true);
				try
				{
					return ImGuiOm.ButtonIcon(id, icon);
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
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void zUs4WJbE3g()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017110C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)), new Vector2(0f, 0f), true);
		try
		{
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
				{
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6127C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560))));
				}
				ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 56f * Globals.GlobalUIScale);
				if (Jbo4ts8VL3(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE6344 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a), (FontAwesomeIcon)61543))
				{
					ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x442740AE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)), (ImGuiPopupFlags)0);
				}
				if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE630C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a))));
				}
				ImGui.SameLine();
				if (Jbo4ts8VL3(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C1739C1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb), (FontAwesomeIcon)62831))
				{
					ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427401C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)), (ImGuiPopupFlags)0);
				}
				if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A69DD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92))));
				}
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
			ImGui.Separator();
			for (int i = 0; i < config.Lists.Count; i++)
			{
				XDmpEqpiOaNG0YVioT2 xDmpEqpiOaNG0YVioT = config.Lists[i];
				ImU8String val3 = new ImU8String(9, 2);
				((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017107E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
				((ImU8String)(ref val3)).AppendFormatted<string>(xDmpEqpiOaNG0YVioT.Name);
				((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FB0FC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414));
				((ImU8String)(ref val3)).AppendFormatted<int>(i);
				if (ImGui.Selectable(val3, selectedSource == xDmpEqpiOaNG0YVioT, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					selectedSource = xDmpEqpiOaNG0YVioT;
				}
			}
			q934s3XudC();
			lTD48ouIZ6();
			ImGui.Spacing();
			ImGui.Spacing();
			GroupDisposable val4 = ImRaii.Group();
			try
			{
				using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
				{
					Vector4 vector = KnownColor.Chocolate.ToVector4();
					ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E47503 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7))));
				}
				ImGui.SameLine(ImGui.GetWindowContentRegionMax().X - 56f * Globals.GlobalUIScale);
				if (Jbo4ts8VL3(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F9BD1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009), (FontAwesomeIcon)61543))
				{
					ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FBF0A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414)), (ImGuiPopupFlags)0);
				}
				if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FB05A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414))));
				}
				ImGui.SameLine();
				if (Jbo4ts8VL3(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7CF2B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6), (FontAwesomeIcon)62198))
				{
					ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8CFD1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)), (ImGuiPopupFlags)0);
				}
				if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8CF31 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3))));
				}
			}
			finally
			{
				((GroupDisposable)(ref val4)).Dispose();
			}
			ImGui.Separator();
			for (int j = 0; j < config.Groups.Count; j++)
			{
				HJRFNrpmZeSERXoVlH4 hJRFNrpmZeSERXoVlH = config.Groups[j];
				ImU8String val5 = new ImU8String(10, 2);
				((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6132A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560));
				((ImU8String)(ref val5)).AppendFormatted<string>(hJRFNrpmZeSERXoVlH.Name);
				((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD79D2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc));
				((ImU8String)(ref val5)).AppendFormatted<int>(j);
				if (ImGui.Selectable(val5, selectedSource == hJRFNrpmZeSERXoVlH, (ImGuiSelectableFlags)0, default(Vector2)))
				{
					selectedSource = hJRFNrpmZeSERXoVlH;
				}
			}
			U3I4vsgBdD();
			fFN4lU633m();
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void Svl4T97uZC()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD79E6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc)), new Vector2(0f, 0f), false);
		try
		{
			if (selectedSource == null)
			{
				ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4E0A4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360))));
			}
			else if (selectedSource is XDmpEqpiOaNG0YVioT2 list)
			{
				SSb41FkYSF(list);
			}
			else if (selectedSource is HJRFNrpmZeSERXoVlH4 hJRFNrpmZeSERXoVlH)
			{
				JWn4MC1HiO(hJRFNrpmZeSERXoVlH);
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void SSb41FkYSF(XDmpEqpiOaNG0YVioT2 list)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_054f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_0909: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Unknown result type (might be due to invalid IL or missing references)
		//IL_093e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0943: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0945: Unknown result type (might be due to invalid IL or missing references)
		//IL_097c: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			using (OmenServiceBase<FontManager>.Instance().UIFont140.Push())
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(list.Name));
			}
			ImGui.SameLine();
			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4f * Globals.GlobalUIScale);
			if (Jbo4ts8VL3(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3171899 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273), (FontAwesomeIcon)62213))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00ECD2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)), (ImGuiPopupFlags)0);
			}
			mPn4xoNYLP(list, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C173789 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb));
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FBE6C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414), list.Data.Count, list.ImportTime)));
		ImGui.Separator();
		ImGui.Spacing();
		ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)21, 0u, true);
		try
		{
			ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)22, 1090519039u, true);
			try
			{
				ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)23, 1627389951u, true);
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62004, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1FA6D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c))))
					{
						ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1A704 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)), (ImGuiPopupFlags)0);
					}
					WkL4B5iDMR(list);
					ImGui.SameLine();
					DisabledDisposable val5 = ImRaii.Disabled(list.Data.Count == 0);
					try
					{
						if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61920, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD762BF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f))))
						{
							ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2687E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765)), (ImGuiPopupFlags)0);
						}
						if (list.Data.Count == 0 && ImGui.IsItemHovered((ImGuiHoveredFlags)128))
						{
							ImGui.SetTooltip(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44274E78 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b))));
						}
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
					nVK4iURYpT(list);
					ImGui.SameLine();
					ColorDisposable val6 = ImRaii.PushColor((ImGuiCol)0, KnownColor.Red.ToVector4(), true);
					try
					{
						if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62189, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -1141732037 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c))))
						{
							ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE37603 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)), (ImGuiPopupFlags)0);
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
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
		PopupDisposable val7 = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3171B9F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273)));
		try
		{
			if (PopupDisposable.op_Implicit(val7))
			{
				ImU8String val8 = new ImU8String(1, 1);
				((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E4659 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467)));
				((ImU8String)(ref val8)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854C0EF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
				ImGui.TextUnformatted(val8);
				ImGui.Separator();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4A68A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55))), new Vector2(-1f, 0f)))
				{
					config.Lists.Remove(list);
					config.Save(this);
					xao4FZlCLw();
					selectedSource = null;
					ImGui.CloseCurrentPopup();
				}
			}
		}
		finally
		{
			((PopupDisposable)(ref val7)).Dispose();
		}
		ImGui.Spacing();
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50171D96 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753CE40 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c))), ref playerSearchFilter, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		TableDisposable val9 = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4784B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)), 4, (ImGuiTableFlags)33564608, new Vector2(0f, 0f));
		try
		{
			if (!val9)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(9818u)), (ImGuiTableColumnFlags)4, 25f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39515D13 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497))), (ImGuiTableColumnFlags)4, 40f, 0u);
			ImU8String val10 = ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE376A7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)));
			float x = ImGui.CalcTextSize(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B98B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)), false, -1f).X;
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.TableSetupColumn(val10, (ImGuiTableColumnFlags)8, x + ((ImGuiStylePtr)(ref style)).CellPadding.X * 2f, 0u);
			ImU8String val11 = ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C97BC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)));
			float num = Math.Max(ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50171CCC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf))), false, -1f).X, ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E5F99 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec))), false, -1f).X);
			style = ImGui.GetStyle();
			ImGui.TableSetupColumn(val11, (ImGuiTableColumnFlags)8, num + ((ImGuiStylePtr)(ref style)).CellPadding.X * 2f, 0u);
			ImGui.TableHeadersRow();
			for (int i = 0; i < list.Data.Count; i++)
			{
				W2sjNRpPqifh2MFEW32 w2sjNRpPqifh2MFEW = list.Data[i];
				w2sjNRpPqifh2MFEW.OTWphlSIk5(cancelSource?.Token ?? CancellationToken.None);
				if (!string.IsNullOrWhiteSpace(playerSearchFilter))
				{
					string value = playerSearchFilter;
					if (!w2sjNRpPqifh2MFEW.PlayerName.Contains(value, StringComparison.OrdinalIgnoreCase) && !w2sjNRpPqifh2MFEW.PlayerWorldName.Contains(value, StringComparison.OrdinalIgnoreCase) && !w2sjNRpPqifh2MFEW.Remark.Contains(value, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				IiM4462bSB(w2sjNRpPqifh2MFEW.PlayerName, w2sjNRpPqifh2MFEW.PlayerWorldName, w2sjNRpPqifh2MFEW.PlayerContentID);
				ImGui.TableNextColumn();
				ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
				string remark = w2sjNRpPqifh2MFEW.Remark;
				float num2 = MathF.Max(1f, remark.Count((char c) => c == '\n') + 1);
				float y = ImGui.GetFrameHeight() * num2;
				ImU8String val12 = new ImU8String(19, 1);
				((ImU8String)(ref val12)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36FE08 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
				((ImU8String)(ref val12)).AppendFormatted<int>(i);
				ImGui.InputTextMultiline(val12, ref remark, 1024, new Vector2(-1f, y), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					w2sjNRpPqifh2MFEW.Remark = remark;
					config.Save(this);
				}
				ImGuiOm.TooltipHover(remark);
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(w2sjNRpPqifh2MFEW.Time.ToString(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F9895 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009))));
				ImGui.TableNextColumn();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5CBF6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
				defaultInterpolatedStringHandler.AppendFormatted(i);
				if (Jbo4ts8VL3(defaultInterpolatedStringHandler.ToStringAndClear(), (FontAwesomeIcon)61944))
				{
					ImU8String val13 = new ImU8String(13, 1);
					((ImU8String)(ref val13)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D61F06 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560));
					((ImU8String)(ref val13)).AppendFormatted<int>(i);
					ImGui.OpenPopup(val13, (ImGuiPopupFlags)0);
				}
				ImU8String val14 = new ImU8String(13, 1);
				((ImU8String)(ref val14)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCEF86 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361));
				((ImU8String)(ref val14)).AppendFormatted<int>(i);
				PopupDisposable val15 = ImRaii.Popup(val14);
				try
				{
					if (!(!val15))
					{
						ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C9700 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb))));
						ImGui.Separator();
						if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x76DB1C5F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273))), new Vector2(-1f, 0f)))
						{
							list.Data.RemoveAt(i);
							config.Save(this);
							xao4FZlCLw();
							ImGui.CloseCurrentPopup();
							break;
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val15)).Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val9)).Dispose();
		}
	}

	private void JWn4MC1HiO(HJRFNrpmZeSERXoVlH4 group)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_071c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_0966: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a46: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aae: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			using (OmenServiceBase<FontManager>.Instance().UIFont140.Push())
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(group.Name));
			}
			ImGui.SameLine();
			ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 4f * Globals.GlobalUIScale);
			if (Jbo4ts8VL3(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8CC45 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3), (FontAwesomeIcon)62213))
			{
				ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23101147 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d)), (ImGuiPopupFlags)0);
			}
			mPn4xoNYLP(group, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39515BA1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497));
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(0, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(group.Identifier);
		ImGui.TextDisabled(val2);
		if (ImGui.IsItemHovered())
		{
			ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			ImGui.SetTooltip(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C357A80 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def))));
			if (ImGui.IsItemClicked())
			{
				ImGui.SetClipboardText(ImU8String.op_Implicit(group.Identifier));
				OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7712A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C5C0A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948) + group.Identifier);
			}
		}
		ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF268E0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765), group.Data.Count, group.ImportTime)));
		ImGui.Separator();
		ImGui.Spacing();
		bool valueOrDefault = groupLogsVisibility.GetValueOrDefault(group.Identifier, defaultValue: false);
		ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)21, 0u, true);
		try
		{
			ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)22, 1090519039u, true);
			try
			{
				ColorDisposable val5 = ImRaii.PushColor((ImGuiCol)23, 1627389951u, true);
				try
				{
					if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62004, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36955995 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9))))
					{
						ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5557C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)), (ImGuiPopupFlags)0);
					}
					Pon4muv2R9(group);
					ImGui.SameLine();
					ColorDisposable val6 = ImRaii.PushColor((ImGuiCol)0, KnownColor.GreenYellow.ToVector4(), valueOrDefault);
					try
					{
						if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61914, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351D6EC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08))))
						{
							XNK4hRff8Y(group);
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					ImGui.SameLine();
					val6 = ImRaii.PushColor((ImGuiCol)0, KnownColor.Red.ToVector4(), true);
					try
					{
						if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62197, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753C820 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c))))
						{
							ImGui.OpenPopup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32DEA27 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)), (ImGuiPopupFlags)0);
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		PopupDisposable val7 = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CDD87 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee)));
		try
		{
			if (PopupDisposable.op_Implicit(val7))
			{
				ImU8String val8 = new ImU8String(1, 1);
				((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1F6AF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)));
				((ImU8String)(ref val8)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854C0EF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
				ImGui.TextUnformatted(val8);
				ImGui.Separator();
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778EBFD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1))), new Vector2(-1f, 0f)))
				{
					dlM4oVKgOr(group);
					selectedSource = null;
					ImGui.CloseCurrentPopup();
				}
			}
		}
		finally
		{
			((PopupDisposable)(ref val7)).Dispose();
		}
		ImGui.Spacing();
		if (valueOrDefault)
		{
			HXm4PHslcl(group);
			ImGui.Spacing();
		}
		ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
		ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F8B00 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af)), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1EF7F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1))), ref playerSearchFilter, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		TableDisposable val9 = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1A3CA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)), 5, (ImGuiTableFlags)33564608, new Vector2(0f, 0f));
		try
		{
			if (!val9)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(9818u)), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753CE6E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c))), (ImGuiTableColumnFlags)4, 35f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D619F4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560))), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImU8String val10 = ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53579370 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb)));
			float x = ImGui.CalcTextSize(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -110794748 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1)), false, -1f).X;
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.TableSetupColumn(val10, (ImGuiTableColumnFlags)8, x + ((ImGuiStylePtr)(ref style)).CellPadding.X * 2f, 0u);
			ImU8String val11 = ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D61F98 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)));
			float num = Math.Max(ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F8D18 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af))), false, -1f).X, ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5CBA8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3))), false, -1f).X);
			style = ImGui.GetStyle();
			ImGui.TableSetupColumn(val11, (ImGuiTableColumnFlags)8, num + ((ImGuiStylePtr)(ref style)).CellPadding.X * 2f, 0u);
			ImGui.TableHeadersRow();
			for (int i = 0; i < group.Data.Count; i++)
			{
				W2sjNRpPqifh2MFEW32 w2sjNRpPqifh2MFEW = group.Data[i];
				w2sjNRpPqifh2MFEW.OTWphlSIk5(cancelSource?.Token ?? CancellationToken.None);
				if (!string.IsNullOrWhiteSpace(playerSearchFilter))
				{
					string value = playerSearchFilter;
					if (!w2sjNRpPqifh2MFEW.PlayerName.Contains(value, StringComparison.OrdinalIgnoreCase) && !w2sjNRpPqifh2MFEW.PlayerWorldName.Contains(value, StringComparison.OrdinalIgnoreCase) && !w2sjNRpPqifh2MFEW.Remark.Contains(value, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
				}
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				IiM4462bSB(w2sjNRpPqifh2MFEW.PlayerName, w2sjNRpPqifh2MFEW.PlayerWorldName, w2sjNRpPqifh2MFEW.PlayerContentID);
				ImGui.TableNextColumn();
				ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
				string remark = w2sjNRpPqifh2MFEW.Remark;
				int num2 = Math.Max(1, remark.Count((char c) => c == '\n') + 1);
				float y = ImGui.GetFrameHeight() * (float)num2;
				ImU8String val12 = new ImU8String(20, 1);
				((ImU8String)(ref val12)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5CD16 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
				((ImU8String)(ref val12)).AppendFormatted<int>(i);
				ImGui.InputTextMultiline(val12, ref remark, 1024, new Vector2(-1f, y), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.IsItemDeactivatedAfterEdit())
				{
					long num3 = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset.ToUnixTimeMilliseconds();
					if (!lastRemarkEditTime.TryGetValue(w2sjNRpPqifh2MFEW.PlayerContentID, out var value2) || num3 - value2 > 500)
					{
						lastRemarkEditTime[w2sjNRpPqifh2MFEW.PlayerContentID] = num3;
						OKm4gFYF0D(group, w2sjNRpPqifh2MFEW, remark);
					}
				}
				ImGuiOm.TooltipHover(remark);
				ImGui.TableNextColumn();
				IiM4462bSB(w2sjNRpPqifh2MFEW.SourcePlayerName, w2sjNRpPqifh2MFEW.SourcePlayerWorldName, w2sjNRpPqifh2MFEW.SourcePlayerContentID);
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(w2sjNRpPqifh2MFEW.Time.ToString(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE56D47 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7))));
				ImGui.TableNextColumn();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E55428 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
				defaultInterpolatedStringHandler.AppendFormatted(i);
				if (Jbo4ts8VL3(defaultInterpolatedStringHandler.ToStringAndClear(), (FontAwesomeIcon)61944))
				{
					ImU8String val13 = new ImU8String(18, 1);
					((ImU8String)(ref val13)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6197C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560));
					((ImU8String)(ref val13)).AppendFormatted<int>(i);
					ImGui.OpenPopup(val13, (ImGuiPopupFlags)0);
				}
				ImU8String val14 = new ImU8String(18, 1);
				((ImU8String)(ref val14)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A639B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
				((ImU8String)(ref val14)).AppendFormatted<int>(i);
				PopupDisposable val15 = ImRaii.Popup(val14);
				try
				{
					if (!(!val15))
					{
						ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D7544D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1))));
						ImGui.Separator();
						if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D754522 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1))), new Vector2(-1f, 0f)))
						{
							pBV4w5YMNq(group, w2sjNRpPqifh2MFEW);
							ImGui.CloseCurrentPopup();
							break;
						}
					}
				}
				finally
				{
					((PopupDisposable)(ref val15)).Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val9)).Dispose();
		}
	}

	private static void IiM4462bSB(string name, string world, ulong cid)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !string.IsNullOrEmpty(name);
		if (flag)
		{
			Vector4 vector = KnownColor.Orange.ToVector4();
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(1, 2);
			((ImU8String)(ref val)).AppendFormatted<string>(name);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B9C44 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
			((ImU8String)(ref val)).AppendFormatted<string>(world);
			ImGui.TextColored(ref vector, val);
		}
		else
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(cid.ToString()));
		}
		if (ImGui.IsItemHovered())
		{
			ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			if (flag)
			{
				ImGui.SetTooltip(ImU8String.op_Implicit(cid.ToString()));
			}
			if (ImGui.IsItemClicked())
			{
				ImGui.SetClipboardText(ImU8String.op_Implicit(cid.ToString()));
				NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
				defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x3F6C5044 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)));
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FFACD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
				defaultInterpolatedStringHandler.AppendFormatted(cid);
				notifyHelper.NotificationSuccess(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
	}

	private void q934s3XudC()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BFEF2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)));
		try
		{
			if (!val)
			{
				return;
			}
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E53DB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec))));
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val2 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CDF7F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
			ImU8String val3 = new ImU8String(3, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x231069CD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d)));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4A284 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55));
			ImGui.InputTextWithHint(val2, val3, ref newListNameInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			DisabledDisposable val4 = ImRaii.Disabled(string.IsNullOrWhiteSpace(newListNameInput));
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753C3A0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c))), new Vector2(250f * Globals.GlobalUIScale, ImGui.GetTextLineHeightWithSpacing() * 1.5f)) && config.Lists.All((XDmpEqpiOaNG0YVioT2 x) => x.Name != newListNameInput))
				{
					XDmpEqpiOaNG0YVioT2 item = new XDmpEqpiOaNG0YVioT2
					{
						Name = newListNameInput,
						ImportTimeUnix = GameState.ServerTimeUnix
					};
					config.Lists.Add(item);
					config.Save(this);
					selectedSource = item;
					newListNameInput = string.Empty;
					ImGui.CloseCurrentPopup();
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void lTD48ouIZ6()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass64_0 CS_0024_003C_003E8__locals18 = new _003C_003Ec__DisplayClass64_0();
		CS_0024_003C_003E8__locals18._003C_003E4__this = this;
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351DDAA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08)));
		try
		{
			if (!val)
			{
				return;
			}
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017106E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf))));
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x1352E356 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f)), ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FB9BA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414)), ref importListIDInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val2 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C5843 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
			ImU8String val3 = new ImU8String(3, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC90F78 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3AAF4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			ImGui.InputTextWithHint(val2, val3, ref importListPasswordInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			DisabledDisposable val4 = ImRaii.Disabled(string.IsNullOrWhiteSpace(importListIDInput) || string.IsNullOrWhiteSpace(importListPasswordInput) || importListPasswordInput.Length < 6);
			try
			{
				if (!ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44274DD4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b))), new Vector2(250f * Globals.GlobalUIScale, ImGui.GetTextLineHeightWithSpacing() * 1.5f)))
				{
					return;
				}
				CS_0024_003C_003E8__locals18.importListID = importListIDInput;
				CS_0024_003C_003E8__locals18.importListPassword = importListPasswordInput;
				CS_0024_003C_003E8__locals18.cancellationToken = cancelSource?.Token ?? CancellationToken.None;
				Task.Run(async delegate
				{
					try
					{
						_003C_003Ec__DisplayClass64_1 CS_0024_003C_003E8__locals28 = new _003C_003Ec__DisplayClass64_1();
						CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals18;
						CS_0024_003C_003E8__locals28.result = await RemoteBlacklist.GetContentAsync(CS_0024_003C_003E8__locals18.importListID, CS_0024_003C_003E8__locals18.importListPassword, 1000, 0, CS_0024_003C_003E8__locals18.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						IFramework obj = DalamudServiceExtension.Instance<IFramework>();
						Action action = delegate
						{
							//IL_0049: Unknown result type (might be due to invalid IL or missing references)
							//IL_017d: Unknown result type (might be due to invalid IL or missing references)
							if (CS_0024_003C_003E8__locals28.result.Entries.Count == 0)
							{
								string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68540E3B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
								OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
								NotifyHelper.ToastError(text);
							}
							else
							{
								XDmpEqpiOaNG0YVioT2 xDmpEqpiOaNG0YVioT = new XDmpEqpiOaNG0YVioT2
								{
									Name = CS_0024_003C_003E8__locals28.result.Info.Name,
									ImportTimeUnix = GameState.ServerTimeUnix,
									Data = CS_0024_003C_003E8__locals28.result.Entries.Select((BlacklistEntry x) => new W2sjNRpPqifh2MFEW32
									{
										PlayerContentID = x.Player,
										Remark = x.Remark,
										TimeUnix = x.Time
									}).ToList()
								};
								CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1._003C_003E4__this.config.Lists.Add(xDmpEqpiOaNG0YVioT);
								CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1._003C_003E4__this.config.Save(CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1._003C_003E4__this);
								CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1._003C_003E4__this.importListIDInput = string.Empty;
								CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1._003C_003E4__this.importListPasswordInput = string.Empty;
								CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1._003C_003E4__this.xao4FZlCLw();
								string text2 = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F2232 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), xDmpEqpiOaNG0YVioT.Name, xDmpEqpiOaNG0YVioT.Data.Count);
								OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text2));
								NotifyHelper.Toast(text2);
								CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1._003C_003E4__this.selectedSource = xDmpEqpiOaNG0YVioT;
							}
						};
						CancellationToken cancellationToken = CS_0024_003C_003E8__locals18.cancellationToken;
						await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (Exception ex)
					{
						_003C_003Ec__DisplayClass64_2 CS_0024_003C_003E8__locals29 = new _003C_003Ec__DisplayClass64_2();
						CS_0024_003C_003E8__locals29.ex = ex;
						if (CS_0024_003C_003E8__locals18.cancellationToken.IsCancellationRequested)
						{
							return;
						}
						DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CDA0D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361), CS_0024_003C_003E8__locals29.ex);
						IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
						Action action2 = delegate
						{
							//IL_0048: Unknown result type (might be due to invalid IL or missing references)
							string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6B0E8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), CS_0024_003C_003E8__locals29.ex.Message);
							OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
							NotifyHelper.ToastError(text);
						};
						CancellationToken cancellationToken = CS_0024_003C_003E8__locals18.cancellationToken;
						await obj2.RunOnTick(action2, default(TimeSpan), 0, cancellationToken);
					}
				}, CS_0024_003C_003E8__locals18.cancellationToken);
				ImGui.CloseCurrentPopup();
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void U3I4vsgBdD()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass65_0 CS_0024_003C_003E8__locals18 = new _003C_003Ec__DisplayClass65_0();
		CS_0024_003C_003E8__locals18._003C_003E4__this = this;
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F36A4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)));
		try
		{
			if (!val)
			{
				return;
			}
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C1739D1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb))));
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val2 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E00EB30 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
			ImU8String val3 = new ImU8String(3, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951232B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497)));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FD7AA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			ImGui.InputTextWithHint(val2, val3, ref newGroupNameInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val4 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7C879 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
			ImU8String val5 = new ImU8String(3, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EBD4C7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)));
			((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23101311 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d));
			ImGui.InputTextWithHint(val4, val5, ref newGroupPasswordInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			DisabledDisposable val6 = ImRaii.Disabled(string.IsNullOrWhiteSpace(newGroupNameInput) || string.IsNullOrWhiteSpace(newGroupPasswordInput) || newGroupPasswordInput.Length < 6);
			try
			{
				if (!ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1778E6D9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1))), new Vector2(250f * Globals.GlobalUIScale, ImGui.GetTextLineHeightWithSpacing() * 1.5f)))
				{
					return;
				}
				CS_0024_003C_003E8__locals18.password = newGroupPasswordInput;
				CS_0024_003C_003E8__locals18.name = newGroupNameInput;
				CS_0024_003C_003E8__locals18.cancellationToken = cancelSource?.Token ?? CancellationToken.None;
				Task.Run(async delegate
				{
					try
					{
						_003C_003Ec__DisplayClass65_1 CS_0024_003C_003E8__locals33 = new _003C_003Ec__DisplayClass65_1();
						CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals18;
						CS_0024_003C_003E8__locals33.result = await RemoteBlacklist.CreateAsync(CS_0024_003C_003E8__locals18.name, CS_0024_003C_003E8__locals18.password, CS_0024_003C_003E8__locals18.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						IFramework obj = DalamudServiceExtension.Instance<IFramework>();
						Action action = delegate
						{
							//IL_005e: Unknown result type (might be due to invalid IL or missing references)
							//IL_017d: Unknown result type (might be due to invalid IL or missing references)
							//IL_0193: Unknown result type (might be due to invalid IL or missing references)
							if (!CS_0024_003C_003E8__locals33.result.Success)
							{
								string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B3688 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded), CS_0024_003C_003E8__locals33.result.Message);
								NotifyHelper.ToastError(text);
								OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
							}
							else
							{
								HJRFNrpmZeSERXoVlH4 hJRFNrpmZeSERXoVlH = new HJRFNrpmZeSERXoVlH4
								{
									Identifier = CS_0024_003C_003E8__locals33.result.BlacklistID,
									Password = CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1.password,
									Name = CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1.name,
									ImportTimeUnix = GameState.ServerTimeUnix
								};
								CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1._003C_003E4__this.config.Groups.Add(hJRFNrpmZeSERXoVlH);
								CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1._003C_003E4__this.config.Save(CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1._003C_003E4__this);
								CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1._003C_003E4__this.doJ4bUPgfD(hJRFNrpmZeSERXoVlH);
								CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1._003C_003E4__this.newGroupNameInput = string.Empty;
								CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1._003C_003E4__this.newGroupPasswordInput = string.Empty;
								OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F365006 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2), CS_0024_003C_003E8__locals33.result.BlacklistID, CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1.password) ?? ""));
								ImGui.SetClipboardText(ImU8String.op_Implicit(CS_0024_003C_003E8__locals33.result.BlacklistID));
								CS_0024_003C_003E8__locals33.CS_0024_003C_003E8__locals1._003C_003E4__this.selectedSource = hJRFNrpmZeSERXoVlH;
							}
						};
						CancellationToken cancellationToken = CS_0024_003C_003E8__locals18.cancellationToken;
						await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (Exception ex)
					{
						_003C_003Ec__DisplayClass65_2 CS_0024_003C_003E8__locals34 = new _003C_003Ec__DisplayClass65_2();
						CS_0024_003C_003E8__locals34.ex = ex;
						if (CS_0024_003C_003E8__locals18.cancellationToken.IsCancellationRequested)
						{
							return;
						}
						DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3AD68A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded), CS_0024_003C_003E8__locals34.ex);
						IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
						Action action2 = delegate
						{
							//IL_0048: Unknown result type (might be due to invalid IL or missing references)
							string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF10A56 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912), CS_0024_003C_003E8__locals34.ex.Message);
							NotifyHelper.ToastError(text);
							OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
						};
						CancellationToken cancellationToken = CS_0024_003C_003E8__locals18.cancellationToken;
						await obj2.RunOnTick(action2, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
				}, CS_0024_003C_003E8__locals18.cancellationToken);
				ImGui.CloseCurrentPopup();
			}
			finally
			{
				((IDisposable)val6)?.Dispose();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void fFN4lU633m()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854A5E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)));
		try
		{
			if (!val)
			{
				return;
			}
			Vector4 vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1A604 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912))));
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56CFDE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1)), ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5CE0A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)), ref importGroupIDInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val2 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423EE048 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71));
			ImU8String val3 = new ImU8String(3, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D7541A0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C5887 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
			ImGui.InputTextWithHint(val2, val3, ref importGroupPasswordInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			DisabledDisposable val4 = ImRaii.Disabled(string.IsNullOrWhiteSpace(importGroupIDInput) || string.IsNullOrWhiteSpace(importGroupPasswordInput) || importGroupPasswordInput.Length < 6);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36FDB4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2))), new Vector2(250f * Globals.GlobalUIScale, ImGui.GetTextLineHeightWithSpacing() * 1.5f)))
				{
					if (config.Groups.All((HJRFNrpmZeSERXoVlH4 x) => x.Identifier != importGroupIDInput))
					{
						HJRFNrpmZeSERXoVlH4 hJRFNrpmZeSERXoVlH = new HJRFNrpmZeSERXoVlH4
						{
							Identifier = importGroupIDInput,
							Password = importGroupPasswordInput,
							Name = (importGroupIDInput ?? ""),
							ImportTimeUnix = GameState.ServerTimeUnix
						};
						config.Groups.Add(hJRFNrpmZeSERXoVlH);
						config.Save(this);
						doJ4bUPgfD(hJRFNrpmZeSERXoVlH);
						importGroupIDInput = string.Empty;
						importGroupPasswordInput = string.Empty;
						string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35791E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def), hJRFNrpmZeSERXoVlH.Name);
						OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
						NotifyHelper.Toast(text);
						selectedSource = hJRFNrpmZeSERXoVlH;
					}
					ImGui.CloseCurrentPopup();
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void mPn4xoNYLP(PsT8vDpBBOEatOCNQNn source, string popupID)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit(popupID));
		try
		{
			if (!(!val))
			{
				if (ImGui.IsWindowAppearing())
				{
					editNameInput = source.Name;
				}
				ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
				ImU8String val2 = new ImU8String(17, 1);
				((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1F5B9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c));
				((ImU8String)(ref val2)).AppendFormatted<string>(popupID);
				ImGui.InputText(val2, ref editNameInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE56C5F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7))), new Vector2(-1f, 0f)))
				{
					source.Name = editNameInput;
					config.Save(this);
					ImGui.CloseCurrentPopup();
				}
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private PlayerInfoValue? Gbv4puPUHJ(string idSuffix, C47amWpEw9DpQfUw7vG state)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(13, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(LuminaWrapper.GetAddonText(973u));
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CC175 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
		((ImU8String)(ref val)).AppendFormatted<string>(idSuffix);
		ImU8String val2 = val;
		ref ulong contentID = ref state.ContentID;
		long num = 1L;
		long num2 = 1L;
		ImU8String val3 = default(ImU8String);
		ImGui.InputULong(val2, ref contentID, (ulong)num, (ulong)num2, val3, (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit() && state.ContentID != 0L)
		{
			D0I43MpDpC(state, state.ContentID);
		}
		if (state.ContentID == 0L || state.QueryingContentID != state.ContentID)
		{
			return null;
		}
		PlayerInfoValue info = state.Info;
		if ((object)info != null)
		{
			Vector4 vector = KnownColor.Orange.ToVector4();
			((ImU8String)(ref val3))._002Ector(1, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>(info.Name);
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310134B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d));
			((ImU8String)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetWorldName(info.WorldID));
			ImGui.TextColored(ref vector, val3);
			return info;
		}
		if (state.IsInvalid)
		{
			Vector4 vector = KnownColor.Red.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14F35F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6))));
			return null;
		}
		ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351CA6A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08))));
		return null;
	}

	private void D0I43MpDpC(C47amWpEw9DpQfUw7vG state, ulong contentID)
	{
		_003C_003Ec__DisplayClass70_0 CS_0024_003C_003E8__locals24 = new _003C_003Ec__DisplayClass70_0();
		CS_0024_003C_003E8__locals24.contentID = contentID;
		CS_0024_003C_003E8__locals24.state = state;
		CS_0024_003C_003E8__locals24.state.QueryingContentID = CS_0024_003C_003E8__locals24.contentID;
		CS_0024_003C_003E8__locals24.state.Info = null;
		CS_0024_003C_003E8__locals24.state.IsInvalid = false;
		CS_0024_003C_003E8__locals24.cancellationToken = cancelSource?.Token ?? CancellationToken.None;
		Task.Run(async delegate
		{
			try
			{
				_003C_003Ec__DisplayClass70_1 CS_0024_003C_003E8__locals21 = new _003C_003Ec__DisplayClass70_1();
				CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals24;
				CS_0024_003C_003E8__locals21.info = await RemotePlayerInfo.GetFreshAsync(CS_0024_003C_003E8__locals24.contentID, WorldRegionResolver.Resolve(GameState.ClientLanguage), CS_0024_003C_003E8__locals24.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				IFramework obj = DalamudServiceExtension.Instance<IFramework>();
				Action action = delegate
				{
					if (CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1.state.ContentID == CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1.contentID)
					{
						CS_0024_003C_003E8__locals21.CS_0024_003C_003E8__locals1.state.Info = CS_0024_003C_003E8__locals21.info;
					}
				};
				CancellationToken cancellationToken = CS_0024_003C_003E8__locals24.cancellationToken;
				await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception)
			{
				if (!CS_0024_003C_003E8__locals24.cancellationToken.IsCancellationRequested)
				{
					IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
					Action action2 = delegate
					{
						if (CS_0024_003C_003E8__locals24.state.ContentID == CS_0024_003C_003E8__locals24.contentID)
						{
							CS_0024_003C_003E8__locals24.state.IsInvalid = true;
						}
					};
					CancellationToken cancellationToken = CS_0024_003C_003E8__locals24.cancellationToken;
					await obj2.RunOnTick(action2, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
		}, CS_0024_003C_003E8__locals24.cancellationToken);
	}

	private void WkL4B5iDMR(XDmpEqpiOaNG0YVioT2 list)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Expected O, but got Unknown
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCED9A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361)));
		try
		{
			if (!val)
			{
				return;
			}
			if (ImGui.IsWindowAppearing())
			{
				listNewPlayerInput.m42pbHPLXE();
				listNewPlayerRemarkInput = string.Empty;
			}
			PlayerInfoValue playerInfoValue = Gbv4puPUHJ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8D7E1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3), listNewPlayerInput);
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val2 = new ImU8String(26, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FBD94 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414)));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695493D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
			ImGui.InputTextMultiline(val2, ref listNewPlayerRemarkInput, 1024, new Vector2(250f * Globals.GlobalUIScale, 3f * ImGui.GetTextLineHeightWithSpacing()), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			DisabledDisposable val3 = ImRaii.Disabled((object)playerInfoValue == null);
			try
			{
				if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD8CDFB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3))), new Vector2(250f * Globals.GlobalUIScale, ImGui.GetTextLineHeightWithSpacing() * 1.5f)) && (object)playerInfoValue != null)
				{
					PlayerInfoValue playerInfoValue2 = playerInfoValue;
					if (list.Data.All((W2sjNRpPqifh2MFEW32 x) => x.PlayerContentID != listNewPlayerInput.ContentID))
					{
						list.Data.Add(new W2sjNRpPqifh2MFEW32
						{
							PlayerContentID = listNewPlayerInput.ContentID,
							Remark = listNewPlayerRemarkInput,
							TimeUnix = GameState.ServerTimeUnix
						});
						config.Save(this);
						xao4FZlCLw();
						ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD618A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc), (object)new PlayerPayload(playerInfoValue2.Name, playerInfoValue2.WorldID), list.Name);
						OmenServiceBase<NotifyHelper>.Instance().Chat(se);
						NotifyHelper.Toast(se);
						ImGui.CloseCurrentPopup();
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
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void Pon4muv2R9(HJRFNrpmZeSERXoVlH4 group)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass72_0 _003C_003Ec__DisplayClass72_3 = new _003C_003Ec__DisplayClass72_0();
		_003C_003Ec__DisplayClass72_3._003C_003E4__this = this;
		_003C_003Ec__DisplayClass72_3.group = group;
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x7B44434A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)));
		try
		{
			if (!val)
			{
				return;
			}
			if (ImGui.IsWindowAppearing())
			{
				groupNewPlayerInput.m42pbHPLXE();
				groupNewPlayerRemarkInput = string.Empty;
			}
			PlayerInfoValue playerInfoValue = Gbv4puPUHJ(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x535788E8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb), groupNewPlayerInput);
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val2 = new ImU8String(27, 1);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1EF7D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FC8B0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			ImGui.InputTextMultiline(val2, ref groupNewPlayerRemarkInput, 1024, new Vector2(250f * Globals.GlobalUIScale, 3f * ImGui.GetTextLineHeightWithSpacing()), (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			DisabledDisposable val3 = ImRaii.Disabled((object)playerInfoValue == null);
			try
			{
				if (!ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854A7C3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5))), new Vector2(250f * Globals.GlobalUIScale, ImGui.GetTextLineHeightWithSpacing() * 1.5f)))
				{
					return;
				}
				_003C_003Ec__DisplayClass72_1 CS_0024_003C_003E8__locals17 = new _003C_003Ec__DisplayClass72_1();
				CS_0024_003C_003E8__locals17.CS_0024_003C_003E8__locals1 = _003C_003Ec__DisplayClass72_3;
				if ((object)playerInfoValue == null)
				{
					return;
				}
				CS_0024_003C_003E8__locals17.info = playerInfoValue;
				if (!CS_0024_003C_003E8__locals17.CS_0024_003C_003E8__locals1.group.Data.All((W2sjNRpPqifh2MFEW32 x) => x.PlayerContentID != CS_0024_003C_003E8__locals17.CS_0024_003C_003E8__locals1._003C_003E4__this.groupNewPlayerInput.ContentID))
				{
					return;
				}
				CS_0024_003C_003E8__locals17.newEntry = new BlacklistPeopleInfo
				{
					Player = groupNewPlayerInput.ContentID,
					Remark = groupNewPlayerRemarkInput,
					Time = GameState.ServerTimeUnix,
					SourcePlayer = LocalPlayerState.ContentID,
					Status = 1
				};
				CS_0024_003C_003E8__locals17.cancellationToken = cancelSource?.Token ?? CancellationToken.None;
				Task.Run(async delegate
				{
					_ = 1;
					try
					{
						_003C_003Ec__DisplayClass72_2 CS_0024_003C_003E8__locals24 = new _003C_003Ec__DisplayClass72_2();
						CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2 = CS_0024_003C_003E8__locals17;
						CS_0024_003C_003E8__locals24.result = await RemoteBlacklist.UpdateAsync(CS_0024_003C_003E8__locals17.CS_0024_003C_003E8__locals1.group.Identifier, CS_0024_003C_003E8__locals17.CS_0024_003C_003E8__locals1.group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(CS_0024_003C_003E8__locals17.newEntry), CS_0024_003C_003E8__locals17.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						IFramework obj = DalamudServiceExtension.Instance<IFramework>();
						Action action = delegate
						{
							//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
							//IL_00e2: Expected O, but got Unknown
							//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
							//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
							//IL_0105: Unknown result type (might be due to invalid IL or missing references)
							//IL_010c: Unknown result type (might be due to invalid IL or missing references)
							//IL_0068: Unknown result type (might be due to invalid IL or missing references)
							if (!CS_0024_003C_003E8__locals24.result.Success)
							{
								string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20B345 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372), CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.group.Name);
								NotifyHelper.ToastError(text);
								OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
							}
							else
							{
								CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1._003C_003E4__this.doJ4bUPgfD(CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.group);
								ReadOnlySeString se = LanguageManager.GetSe(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C930E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948), (object)new PlayerPayload(CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.info.Name, CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.info.WorldID), CS_0024_003C_003E8__locals24.CS_0024_003C_003E8__locals2.CS_0024_003C_003E8__locals1.group.Name);
								OmenServiceBase<NotifyHelper>.Instance().Chat(se);
								NotifyHelper.Toast(se);
							}
						};
						CancellationToken cancellationToken = CS_0024_003C_003E8__locals17.cancellationToken;
						await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (Exception ex)
					{
						if (!CS_0024_003C_003E8__locals17.cancellationToken.IsCancellationRequested)
						{
							DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AD6892 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09), ex);
						}
					}
				}, CS_0024_003C_003E8__locals17.cancellationToken);
				ImGui.CloseCurrentPopup();
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void nVK4iURYpT(XDmpEqpiOaNG0YVioT2 list)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass73_0 CS_0024_003C_003E8__locals22 = new _003C_003Ec__DisplayClass73_0();
		CS_0024_003C_003E8__locals22.list = list;
		PopupDisposable val = ImRaii.Popup(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D1F387 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)));
		try
		{
			if (!val)
			{
				return;
			}
			if (ImGui.IsWindowAppearing())
			{
				listSharePasswordInput = string.Empty;
			}
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val2 = ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -2099770856 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			ImU8String val3 = new ImU8String(6, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1EF3BE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56D124 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1));
			ImGui.InputTextWithHint(val2, val3, ref listSharePasswordInput, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			ImGui.SetNextItemWidth(250f * Globals.GlobalUIScale);
			ImU8String val4 = new ImU8String(30, 1);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE36DCD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)));
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5D292 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
			if (ImGui.InputInt(val4, ref listShareExpireTimeDayInput, 1, 1, default(ImU8String), (ImGuiInputTextFlags)0))
			{
				listShareExpireTimeDayInput = Math.Clamp(listShareExpireTimeDayInput, 1, 365);
			}
			DisabledDisposable val5 = ImRaii.Disabled(string.IsNullOrWhiteSpace(listSharePasswordInput) || listSharePasswordInput.Length < 6);
			try
			{
				if (!ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A6437 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92))), new Vector2(250f * Globals.GlobalUIScale, ImGui.GetTextLineHeightWithSpacing() * 1.5f)))
				{
					return;
				}
				CS_0024_003C_003E8__locals22.data = CS_0024_003C_003E8__locals22.list.Data.Select((W2sjNRpPqifh2MFEW32 x) => new SharedBlacklistEntry
				{
					Player = x.PlayerContentID,
					Remark = x.Remark,
					Time = x.TimeUnix
				}).ToList();
				CS_0024_003C_003E8__locals22.password = listSharePasswordInput;
				CS_0024_003C_003E8__locals22.expireTime = OmenServiceBase<StandardTimeManager>.Instance().Now.AddDays(listShareExpireTimeDayInput);
				CS_0024_003C_003E8__locals22.cancellationToken = cancelSource?.Token ?? CancellationToken.None;
				Task.Run(async delegate
				{
					try
					{
						_003C_003Ec__DisplayClass73_1 CS_0024_003C_003E8__locals28 = new _003C_003Ec__DisplayClass73_1();
						CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals22;
						CS_0024_003C_003E8__locals28.result = await RemoteBlacklist.CreateSharedAsync(CS_0024_003C_003E8__locals22.list.Name, CS_0024_003C_003E8__locals22.password, CS_0024_003C_003E8__locals22.expireTime, CS_0024_003C_003E8__locals22.data, CS_0024_003C_003E8__locals22.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
						IFramework obj = DalamudServiceExtension.Instance<IFramework>();
						Action action = delegate
						{
							//IL_0058: Unknown result type (might be due to invalid IL or missing references)
							//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
							//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
							if (!CS_0024_003C_003E8__locals28.result.Success)
							{
								string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D40AB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e), CS_0024_003C_003E8__locals28.result.Message);
								OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
								NotifyHelper.ToastError(text);
							}
							else
							{
								OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0xD246686 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361), CS_0024_003C_003E8__locals28.result.BlacklistID, CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1.password, CS_0024_003C_003E8__locals28.CS_0024_003C_003E8__locals1.expireTime) ?? ""));
								ImGui.SetClipboardText(ImU8String.op_Implicit(CS_0024_003C_003E8__locals28.result.BlacklistID));
							}
						};
						CancellationToken cancellationToken = CS_0024_003C_003E8__locals22.cancellationToken;
						await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
					catch (Exception ex)
					{
						_003C_003Ec__DisplayClass73_2 CS_0024_003C_003E8__locals29 = new _003C_003Ec__DisplayClass73_2();
						CS_0024_003C_003E8__locals29.ex = ex;
						if (CS_0024_003C_003E8__locals22.cancellationToken.IsCancellationRequested)
						{
							return;
						}
						DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D0BF6F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c), CS_0024_003C_003E8__locals29.ex);
						IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
						Action action2 = delegate
						{
							OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D41D9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE50F1F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7) + CS_0024_003C_003E8__locals29.ex.Message);
						};
						CancellationToken cancellationToken = CS_0024_003C_003E8__locals22.cancellationToken;
						await obj2.RunOnTick(action2, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					}
				}, CS_0024_003C_003E8__locals22.cancellationToken);
				ImGui.CloseCurrentPopup();
			}
			finally
			{
				((IDisposable)val5)?.Dispose();
			}
		}
		finally
		{
			((PopupDisposable)(ref val)).Dispose();
		}
	}

	private void HXm4PHslcl(HJRFNrpmZeSERXoVlH4 group)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		if (!groupLogsCache.TryGetValue(group.Identifier, out List<BlacklistLogEntry> value) || value.Count <= 0)
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695517D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9))));
			return;
		}
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854BFDF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)), 6, (ImGuiTableFlags)33564608, new Vector2(0f, 200f * Globals.GlobalUIScale));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(9818u)), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImU8String val2 = ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F98C5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009)));
			float num = Math.Max(ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36FE36 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2))), false, -1f).X, ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39515C9F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497))), false, -1f).X);
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.TableSetupColumn(val2, (ImGuiTableColumnFlags)8, num + ((ImGuiStylePtr)(ref style)).CellPadding.X * 2f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B802E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded))), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5D23A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3))), (ImGuiTableColumnFlags)4, 20f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F94F2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af))), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImU8String val3 = ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477E479D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)));
			float x = ImGui.CalcTextSize(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489FBDD6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414)), false, -1f).X;
			style = ImGui.GetStyle();
			ImGui.TableSetupColumn(val3, (ImGuiTableColumnFlags)8, x + ((ImGuiStylePtr)(ref style)).CellPadding.X * 2f, 0u);
			ImGui.TableHeadersRow();
			foreach (BlacklistLogEntry item in value)
			{
				item.WarmupResolvedPlayersAsync(cancelSource?.Token ?? CancellationToken.None);
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				IiM4462bSB(item.PlayerName, item.PlayerWorldName, item.PlayerContentID);
				ImGui.TableNextColumn();
				string operationType = item.OperationType;
				string text = ((operationType == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489BEAFE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)) ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -1809537362 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)) : ((operationType == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14F027 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)) ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -1993114309 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467)) : ((!(operationType == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x395144EF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497))) ? (item.OperationType ?? string.Empty) : LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C357690 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)))));
				ImGui.TextUnformatted(ImU8String.op_Implicit(text));
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(item.OldRemark ?? string.Empty));
				if (!string.IsNullOrWhiteSpace(item.OldRemark) && ImGui.IsItemHovered())
				{
					ImGui.SetTooltip(ImU8String.op_Implicit(item.OldRemark));
				}
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(item.NewRemark ?? string.Empty));
				if (!string.IsNullOrWhiteSpace(item.NewRemark) && ImGui.IsItemHovered())
				{
					ImGui.SetTooltip(ImU8String.op_Implicit(item.NewRemark));
				}
				ImGui.TableNextColumn();
				IiM4462bSB(item.OperationPlayerName, item.OperationPlayerWorldName, item.OperationPlayerContentID);
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(item.ChangeDateTime.ToString(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x369552EB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9))));
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void XNK4hRff8Y(HJRFNrpmZeSERXoVlH4 group)
	{
		_003C_003Ec__DisplayClass75_0 CS_0024_003C_003E8__locals22 = new _003C_003Ec__DisplayClass75_0();
		CS_0024_003C_003E8__locals22._003C_003E4__this = this;
		bool valueOrDefault = groupLogsVisibility.GetValueOrDefault(group.Identifier, defaultValue: false);
		groupLogsVisibility[group.Identifier] = !valueOrDefault;
		if (!valueOrDefault)
		{
			if (groupLogsCache.ContainsKey(group.Identifier))
			{
				return;
			}
			CS_0024_003C_003E8__locals22.identifier = group.Identifier;
			CS_0024_003C_003E8__locals22.password = group.Password;
			CS_0024_003C_003E8__locals22.cancellationToken = cancelSource?.Token ?? CancellationToken.None;
			Task.Run(async delegate
			{
				try
				{
					_003C_003Ec__DisplayClass75_1 CS_0024_003C_003E8__locals20 = new _003C_003Ec__DisplayClass75_1();
					CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1 = CS_0024_003C_003E8__locals22;
					CS_0024_003C_003E8__locals20.response = await RemoteBlacklist.GetLogsAsync(CS_0024_003C_003E8__locals22.identifier, CS_0024_003C_003E8__locals22.password, null, null, 100, null, null, CS_0024_003C_003E8__locals22.cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
					IFramework obj = DalamudServiceExtension.Instance<IFramework>();
					Action action = delegate
					{
						List<BlacklistLogEntry> list = (CS_0024_003C_003E8__locals20.response.Success ? (CS_0024_003C_003E8__locals20.response.Logs ?? new List<BlacklistLogEntry>()) : new List<BlacklistLogEntry>());
						foreach (BlacklistLogEntry item in list)
						{
							item.WarmupResolvedPlayersAsync(CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1.cancellationToken);
						}
						CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1._003C_003E4__this.groupLogsCache[CS_0024_003C_003E8__locals20.CS_0024_003C_003E8__locals1.identifier] = list;
					};
					CancellationToken cancellationToken = CS_0024_003C_003E8__locals22.cancellationToken;
					await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				catch (Exception ex)
				{
					if (CS_0024_003C_003E8__locals22.cancellationToken.IsCancellationRequested)
					{
						return;
					}
					DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF0EBB6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912), ex);
					IFramework obj2 = DalamudServiceExtension.Instance<IFramework>();
					Func<List<BlacklistLogEntry>> func = () => CS_0024_003C_003E8__locals22._003C_003E4__this.groupLogsCache[CS_0024_003C_003E8__locals22.identifier] = new List<BlacklistLogEntry>();
					CancellationToken cancellationToken = CS_0024_003C_003E8__locals22.cancellationToken;
					await obj2.RunOnTick<List<BlacklistLogEntry>>(func, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}, CS_0024_003C_003E8__locals22.cancellationToken);
		}
		else
		{
			groupLogsCache.Remove(group.Identifier);
		}
	}

	private void dlM4oVKgOr(HJRFNrpmZeSERXoVlH4 group)
	{
		if (groupObservers.TryGetValue(group.Identifier, out IDisposable value))
		{
			value.Dispose();
			groupObservers.Remove(group.Identifier);
		}
		groupLogsVisibility.Remove(group.Identifier);
		groupLogsCache.Remove(group.Identifier);
		config.Groups.Remove(group);
		config.Save(this);
		xao4FZlCLw();
	}

	private void OKm4gFYF0D(HJRFNrpmZeSERXoVlH4 group, W2sjNRpPqifh2MFEW32 player, string remark)
	{
		_003C_003Ec__DisplayClass77_0 CS_0024_003C_003E8__locals16 = new _003C_003Ec__DisplayClass77_0();
		CS_0024_003C_003E8__locals16.group = group;
		CS_0024_003C_003E8__locals16.player = player;
		CS_0024_003C_003E8__locals16.remark = remark;
		CS_0024_003C_003E8__locals16.updateEntry = new BlacklistPeopleInfo
		{
			Player = CS_0024_003C_003E8__locals16.player.PlayerContentID,
			Remark = CS_0024_003C_003E8__locals16.remark,
			Time = GameState.ServerTimeUnix,
			SourcePlayer = LocalPlayerState.ContentID,
			Status = 1
		};
		CS_0024_003C_003E8__locals16.cancellationToken = cancelSource?.Token ?? CancellationToken.None;
		Task.Run(async delegate
		{
			_ = 1;
			try
			{
				if ((await RemoteBlacklist.UpdateAsync(CS_0024_003C_003E8__locals16.group.Identifier, CS_0024_003C_003E8__locals16.group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(CS_0024_003C_003E8__locals16.updateEntry), CS_0024_003C_003E8__locals16.cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Success)
				{
					IFramework obj = DalamudServiceExtension.Instance<IFramework>();
					Func<string> func = () => CS_0024_003C_003E8__locals16.player.Remark = CS_0024_003C_003E8__locals16.remark;
					CancellationToken cancellationToken = CS_0024_003C_003E8__locals16.cancellationToken;
					await obj.RunOnTick<string>(func, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch (Exception ex)
			{
				if (!CS_0024_003C_003E8__locals16.cancellationToken.IsCancellationRequested)
				{
					DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6855E8DB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5), ex);
				}
			}
		}, CS_0024_003C_003E8__locals16.cancellationToken);
	}

	private void pBV4w5YMNq(HJRFNrpmZeSERXoVlH4 group, W2sjNRpPqifh2MFEW32 player)
	{
		_003C_003Ec__DisplayClass78_0 CS_0024_003C_003E8__locals13 = new _003C_003Ec__DisplayClass78_0();
		CS_0024_003C_003E8__locals13.group = group;
		CS_0024_003C_003E8__locals13._003C_003E4__this = this;
		CS_0024_003C_003E8__locals13.removeEntry = new BlacklistPeopleInfo
		{
			Player = player.PlayerContentID,
			Time = GameState.ServerTimeUnix,
			SourcePlayer = LocalPlayerState.ContentID,
			Status = -1
		};
		CS_0024_003C_003E8__locals13.cancellationToken = cancelSource?.Token ?? CancellationToken.None;
		Task.Run(async delegate
		{
			_ = 1;
			try
			{
				if ((await RemoteBlacklist.UpdateAsync(CS_0024_003C_003E8__locals13.group.Identifier, CS_0024_003C_003E8__locals13.group.Password, new global::_003C_003Ez__ReadOnlySingleElementList<BlacklistPeopleInfo>(CS_0024_003C_003E8__locals13.removeEntry), CS_0024_003C_003E8__locals13.cancellationToken).ConfigureAwait(continueOnCapturedContext: false)).Success)
				{
					IFramework obj = DalamudServiceExtension.Instance<IFramework>();
					Action action = delegate
					{
						CS_0024_003C_003E8__locals13._003C_003E4__this.doJ4bUPgfD(CS_0024_003C_003E8__locals13.group);
					};
					CancellationToken cancellationToken = CS_0024_003C_003E8__locals13.cancellationToken;
					await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
			}
			catch (Exception ex)
			{
				if (!CS_0024_003C_003E8__locals13.cancellationToken.IsCancellationRequested)
				{
					DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489EF2BA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414), ex);
				}
			}
		}, CS_0024_003C_003E8__locals13.cancellationToken);
	}

	private void r5f4E1f8lp()
	{
		foreach (HJRFNrpmZeSERXoVlH4 group in config.Groups)
		{
			doJ4bUPgfD(group);
		}
	}

	private void doJ4bUPgfD(HJRFNrpmZeSERXoVlH4 group)
	{
		_003C_003Ec__DisplayClass80_0 CS_0024_003C_003E8__locals17 = new _003C_003Ec__DisplayClass80_0();
		CS_0024_003C_003E8__locals17.group = group;
		CS_0024_003C_003E8__locals17._003C_003E4__this = this;
		if (groupObservers.TryGetValue(CS_0024_003C_003E8__locals17.group.Identifier, out IDisposable value))
		{
			value.Dispose();
			groupObservers.Remove(CS_0024_003C_003E8__locals17.group.Identifier);
		}
		CancellationToken cancellationToken = cancelSource?.Token ?? CancellationToken.None;
		RemoteBlacklist.GetContentOrRequest(CS_0024_003C_003E8__locals17.group.Identifier, CS_0024_003C_003E8__locals17.group.Password, 1000, 0, cancellationToken);
		IDisposable value2 = RemoteBlacklist.ObserveContent(CS_0024_003C_003E8__locals17.group.Identifier, CS_0024_003C_003E8__locals17.group.Password, delegate(RemoteSnapshot<BlacklistContentValue> snapshot)
		{
			try
			{
				if (snapshot.HasValue)
				{
					BlacklistContentValue value3 = snapshot.Value;
					if ((object)value3 != null)
					{
						CS_0024_003C_003E8__locals17.group.Data.Clear();
						CS_0024_003C_003E8__locals17.group.Data.AddRange(value3.Entries.Select((BlacklistEntry x) => new W2sjNRpPqifh2MFEW32
						{
							PlayerContentID = x.Player,
							Remark = x.Remark,
							TimeUnix = x.Time,
							SourcePlayerContentID = x.SourcePlayer
						}));
						CS_0024_003C_003E8__locals17._003C_003E4__this.TaskHelper.Abort();
						CS_0024_003C_003E8__locals17._003C_003E4__this.TaskHelper.Enqueue(delegate
						{
							CS_0024_003C_003E8__locals17._003C_003E4__this.config.Save(CS_0024_003C_003E8__locals17._003C_003E4__this);
						});
						CS_0024_003C_003E8__locals17._003C_003E4__this.TaskHelper.Enqueue((Action)CS_0024_003C_003E8__locals17._003C_003E4__this.xao4FZlCLw, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
					}
				}
			}
			catch (Exception ex)
			{
				DLog.Error(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5FE84 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd), ex);
			}
		}, 1000, 0, marshalToFrameworkThread: true, cancellationToken);
		groupObservers[CS_0024_003C_003E8__locals17.group.Identifier] = value2;
	}

	private void xao4FZlCLw()
	{
		blockedContentIDs.Clear();
		blockedPlayerMeta.Clear();
		foreach (XDmpEqpiOaNG0YVioT2 list in config.Lists)
		{
			foreach (W2sjNRpPqifh2MFEW32 datum in list.Data)
			{
				blockedContentIDs.Add(datum.PlayerContentID);
				blockedPlayerMeta.TryAdd(datum.PlayerContentID, new G8Dg2DpwnugmrkESTYh
				{
					SourceName = list.Name,
					SourceTypeLocKey = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F387C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7),
					Remark = datum.Remark,
					TimeUnix = datum.TimeUnix
				});
			}
		}
		foreach (HJRFNrpmZeSERXoVlH4 group in config.Groups)
		{
			foreach (W2sjNRpPqifh2MFEW32 datum2 in group.Data)
			{
				blockedContentIDs.Add(datum2.PlayerContentID);
				blockedPlayerMeta.TryAdd(datum2.PlayerContentID, new G8Dg2DpwnugmrkESTYh
				{
					SourceName = group.Name,
					SourceTypeLocKey = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A69AB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92),
					Remark = datum2.Remark,
					TimeUnix = datum2.TimeUnix
				});
			}
		}
	}

	private static string pHZ4qse9Kg()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (GameState.ContentFinderCondition == 0)
		{
			return GameState.TerritoryTypeData.ExtractPlaceName();
		}
		ContentFinderCondition contentFinderConditionData = GameState.ContentFinderConditionData;
		return ((object)((ContentFinderCondition)(ref contentFinderConditionData)).Name/*cast due to constrained. prefix*/).ToString();
	}

	private unsafe static bool Gdl4cy7hkX(ContextMenuOpenedArgs args, out ulong contentID, out string? name, out uint homeWorldID)
	{
		contentID = 0uL;
		name = null;
		homeWorldID = 0u;
		if (args.TargetCharacter != null)
		{
			contentID = ((CharacterData)args.TargetCharacter).ContentId;
			name = ((CharacterData)args.TargetCharacter).NameString;
			homeWorldID = ((CharacterData)args.TargetCharacter).HomeWorld;
		}
		else if (args.TargetContentID != 0L)
		{
			contentID = args.TargetContentID;
			name = args.TargetName;
			homeWorldID = (uint)args.TargetHomeWorldID;
		}
		if (contentID != 0L && !string.IsNullOrEmpty(name) && homeWorldID != 0)
		{
			return contentID != LocalPlayerState.ContentID;
		}
		return false;
	}

	public BetterBlacklist()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F80BB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53578A54 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb)),
			Category = ModuleCategory.System
		};
		Permission = new ModulePermission
		{
			AllDefaultEnabled = true
		};
		groupObservers = new Dictionary<string, IDisposable>();
		blockedContentIDs = new HashSet<ulong>();
		blockedPlayerMeta = new Dictionary<ulong, G8Dg2DpwnugmrkESTYh>();
		editNameInput = string.Empty;
		newGroupNameInput = string.Empty;
		newGroupPasswordInput = string.Empty;
		importGroupIDInput = string.Empty;
		importGroupPasswordInput = string.Empty;
		newListNameInput = string.Empty;
		importListIDInput = string.Empty;
		importListPasswordInput = string.Empty;
		listNewPlayerInput = new C47amWpEw9DpQfUw7vG();
		listNewPlayerRemarkInput = string.Empty;
		listSharePasswordInput = string.Empty;
		listShareExpireTimeDayInput = 3;
		groupNewPlayerInput = new C47amWpEw9DpQfUw7vG();
		groupNewPlayerRemarkInput = string.Empty;
		groupLogsVisibility = new Dictionary<string, bool>();
		groupLogsCache = new Dictionary<string, List<BlacklistLogEntry>>();
		playerSearchFilter = string.Empty;
		lastRemarkEditTime = new Dictionary<ulong, long>();
		base._002Ector();
	}

	static BetterBlacklist()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		GetBlockedResultSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x3850C525 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
		AddContextMenuSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3987CB07 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
	}

	[CompilerGenerated]
	private void Ayb4AQxcFS()
	{
		OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x219410E4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b) + Info.Title);
	}

	[CompilerGenerated]
	private bool fhe4QpOFUA(XDmpEqpiOaNG0YVioT2 x)
	{
		return x.Name != newListNameInput;
	}

	[CompilerGenerated]
	private bool bb94JYx9Zr(HJRFNrpmZeSERXoVlH4 x)
	{
		return x.Identifier != importGroupIDInput;
	}

	[CompilerGenerated]
	private bool jxx4rvU2o2(W2sjNRpPqifh2MFEW32 x)
	{
		return x.PlayerContentID != listNewPlayerInput.ContentID;
	}
}
