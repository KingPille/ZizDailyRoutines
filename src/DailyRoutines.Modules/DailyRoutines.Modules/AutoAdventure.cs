using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using DailyRoutines.Verification;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game;
using OmenTools.Info.Game.AetheryteRecord;
using OmenTools.Info.Game.Enums;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoAdventure : ModuleBase
{
	private record kWNQI8xoWf3E6SpAGDe
	{
		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass45_0
		{
			public AutoAdventure module;

			public kWNQI8xoWf3E6SpAGDe _003C_003E4__this;

			public Func<bool> _003C_003E9__7;

			public Func<bool> _003C_003E9__8;

			public Func<bool> _003C_003E9__9;

			public Func<bool> _003C_003E9__10;

			public Action _003C_003E9__11;

			public Func<bool> _003C_003E9__12;

			public Func<bool> _003C_003E9__13;

			public _003C_003Ec__DisplayClass45_0()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				base._002Ector();
			}

			internal void Rs1PKH8YGg()
			{
				TaskHelper? taskHelper = module.TaskHelper;
				Func<bool> task = delegate
				{
					if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E48142 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)))
					{
						return false;
					}
					AutoAdventure autoAdventure = module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D71388 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
					defaultInterpolatedStringHandler2.AppendFormatted(_003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753AA24 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC843BA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmote);
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmoteSpecial);
					vnavmeshIPC.StopPathfind();
					return !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
					{
						(ConditionFlag)3,
						(ConditionFlag)11
					});
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x7DDE6A59 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)_003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			}

			internal bool ROMPtPum4p()
			{
				if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E48142 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)))
				{
					return false;
				}
				AutoAdventure autoAdventure = module;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D71388 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index + 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753AA24 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
				defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC843BA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)));
				autoAdventure.message = defaultInterpolatedStringHandler.ToStringAndClear();
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmote);
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmoteSpecial);
				vnavmeshIPC.StopPathfind();
				return !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
				{
					(ConditionFlag)3,
					(ConditionFlag)11
				});
			}

			internal void fTQPWkgbVp()
			{
				TaskHelper? taskHelper = module.TaskHelper;
				Func<bool> task = delegate
				{
					if (GameState.TerritoryType == _003C_003E4__this.Zone)
					{
						return true;
					}
					if (!LuminaGetter.TryGetRow<TerritoryType>(_003C_003E4__this.Zone, out TerritoryType item))
					{
						return false;
					}
					AutoAdventure autoAdventure = module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3CA6A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
					defaultInterpolatedStringHandler2.AppendFormatted(_003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1C06E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1FBF26 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1), item.ExtractPlaceName()));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(_003C_003E4__this.Zone, _003C_003E4__this.Position);
					OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D63FF6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846) + nearestAetheryte.Name);
					return true;
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6855EFCD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)_003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			}

			internal bool n8wPT85DFG()
			{
				if (GameState.TerritoryType == _003C_003E4__this.Zone)
				{
					return true;
				}
				if (!LuminaGetter.TryGetRow<TerritoryType>(_003C_003E4__this.Zone, out TerritoryType item))
				{
					return false;
				}
				AutoAdventure autoAdventure = module;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3CA6A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index + 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1C06E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
				defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1FBF26 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1), item.ExtractPlaceName()));
				autoAdventure.message = defaultInterpolatedStringHandler.ToStringAndClear();
				AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(_003C_003E4__this.Zone, _003C_003E4__this.Position);
				OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D63FF6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846) + nearestAetheryte.Name);
				return true;
			}

			internal void r4bP1AGgVZ()
			{
				TaskHelper? taskHelper = module.TaskHelper;
				Func<bool> task = () => GameState.TerritoryType == _003C_003E4__this.Zone && UIModuleExtension.IsScreenReady();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -507444256 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)_003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			}

			internal bool LxOPM4XBfD()
			{
				if (GameState.TerritoryType == _003C_003E4__this.Zone)
				{
					return UIModuleExtension.IsScreenReady();
				}
				return false;
			}

			internal void MhjP4dwnbA()
			{
				TaskHelper? taskHelper = module.TaskHelper;
				Func<bool> task = delegate
				{
					if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA5ED06 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)))
					{
						return false;
					}
					AutoAdventure autoAdventure = module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x21946A6E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
					defaultInterpolatedStringHandler2.AppendFormatted(_003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36953753 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420DDEB4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmote);
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmoteSpecial);
					vnavmeshIPC.StopPathfind();
					return !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
					{
						(ConditionFlag)3,
						(ConditionFlag)11
					});
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420DDD06 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)_003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			}

			internal bool CVaPsABpMP()
			{
				if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA5ED06 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)))
				{
					return false;
				}
				AutoAdventure autoAdventure = module;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x21946A6E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index + 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36953753 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
				defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420DDEB4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)));
				autoAdventure.message = defaultInterpolatedStringHandler.ToStringAndClear();
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmote);
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmoteSpecial);
				vnavmeshIPC.StopPathfind();
				return !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
				{
					(ConditionFlag)3,
					(ConditionFlag)11
				});
			}

			internal void cgWP8GYaOP()
			{
				TaskHelper? taskHelper = module.TaskHelper;
				Action task = delegate
				{
					if (_003C_003E4__this.GeZxEAnBug() || !_003C_003E4__this.U0VxbZR8J1())
					{
						module.TaskHelper.RemoveQueueTasks((int)_003C_003E4__this.Index);
					}
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE685A9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)_003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			}

			internal void lLmPvHDYrY()
			{
				if (_003C_003E4__this.GeZxEAnBug() || !_003C_003E4__this.U0VxbZR8J1())
				{
					module.TaskHelper.RemoveQueueTasks((int)_003C_003E4__this.Index);
				}
			}

			internal void VcqPlbasxL()
			{
				TaskHelper? taskHelper = module.TaskHelper;
				Func<bool> task = delegate
				{
					if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C43EE7E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)))
					{
						return false;
					}
					if (GameState.TerritoryType != _003C_003E4__this.Zone)
					{
						return false;
					}
					AutoAdventure autoAdventure = module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D7521B2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
					defaultInterpolatedStringHandler2.AppendFormatted(_003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CBF8D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6855EEB1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5), _003C_003E4__this.Position.ToString(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09FA91 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363))));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					_003C_003E4__this.MAJxrGsOTw(module);
					return LocalPlayerState.DistanceTo3DSquared(_003C_003E4__this.Position) <= 9f;
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E01A702 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)_003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			}

			internal bool DfbPxytuRP()
			{
				if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C43EE7E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)))
				{
					return false;
				}
				if (GameState.TerritoryType != _003C_003E4__this.Zone)
				{
					return false;
				}
				AutoAdventure autoAdventure = module;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D7521B2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index + 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CBF8D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
				defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6855EEB1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5), _003C_003E4__this.Position.ToString(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09FA91 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363))));
				autoAdventure.message = defaultInterpolatedStringHandler.ToStringAndClear();
				_003C_003E4__this.MAJxrGsOTw(module);
				return LocalPlayerState.DistanceTo3DSquared(_003C_003E4__this.Position) <= 9f;
			}

			internal void ps3PpaXuEt()
			{
				TaskHelper? taskHelper = module.TaskHelper;
				Func<bool> task = delegate
				{
					//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
					//IL_0103: Unknown result type (might be due to invalid IL or missing references)
					//IL_0106: Unknown result type (might be due to invalid IL or missing references)
					//IL_010b: Unknown result type (might be due to invalid IL or missing references)
					if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477F172D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)))
					{
						return false;
					}
					if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy || !UIModuleExtension.IsScreenReady())
					{
						return false;
					}
					if (_003C_003E4__this.GeZxEAnBug())
					{
						return true;
					}
					AutoAdventure autoAdventure = module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D881D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
					defaultInterpolatedStringHandler2.AppendFormatted(_003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F369BDE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
					string key = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469B2DB5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92);
					object[] array = new object[1];
					Emote val = _003C_003E4__this.JiSxqLWwHF();
					array[0] = ((object)((Emote)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(key, array));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					_003C_003E4__this.MAJxrGsOTw(module);
					if (LocalPlayerState.DistanceTo3DSquared(_003C_003E4__this.Position) > 1f)
					{
						return false;
					}
					_003C_003E4__this.vpvxG3SCOS();
					return _003C_003E4__this.GeZxEAnBug();
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C343538 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)_003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			}

			internal bool MROP3rFmok()
			{
				//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
				//IL_0103: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				//IL_010b: Unknown result type (might be due to invalid IL or missing references)
				if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477F172D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)))
				{
					return false;
				}
				if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy || !UIModuleExtension.IsScreenReady())
				{
					return false;
				}
				if (_003C_003E4__this.GeZxEAnBug())
				{
					return true;
				}
				AutoAdventure autoAdventure = module;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D881D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
				defaultInterpolatedStringHandler.AppendFormatted(_003C_003E4__this.Index + 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F369BDE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
				string key = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469B2DB5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92);
				object[] array = new object[1];
				Emote val = _003C_003E4__this.JiSxqLWwHF();
				array[0] = ((object)((Emote)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
				defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(key, array));
				autoAdventure.message = defaultInterpolatedStringHandler.ToStringAndClear();
				_003C_003E4__this.MAJxrGsOTw(module);
				if (LocalPlayerState.DistanceTo3DSquared(_003C_003E4__this.Position) > 1f)
				{
					return false;
				}
				_003C_003E4__this.vpvxG3SCOS();
				return _003C_003E4__this.GeZxEAnBug();
			}
		}

		public uint Index { get; init; }

		public string Name { get; init; }

		public uint Zone { get; init; }

		public Vector3 Position { get; init; }

		public uint Emote { get; init; }

		public (uint StartTime, uint EndTime) TimePeriod { get; init; }

		public uint ExPhase { get; init; }

		public static readonly Dictionary<uint, (uint Begin, uint End)> ExPhaseInfo;

		public static readonly FrozenDictionary<uint, HashSet<uint>> WeatherRequirement;

		public static readonly Dictionary<uint, Vector3> CorrectedPosition;

		public kWNQI8xoWf3E6SpAGDe(uint Index, string Name, uint Zone, Vector3 Position, uint Emote, (uint, uint) TimePeriod, uint ExPhase)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.Index = Index;
			this.Name = Name;
			this.Zone = Zone;
			this.Position = Position;
			this.Emote = Emote;
			this.TimePeriod = TimePeriod;
			this.ExPhase = ExPhase;
			base._002Ector();
		}

		static kWNQI8xoWf3E6SpAGDe()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			ExPhaseInfo = new Dictionary<uint, (uint, uint)>();
			WeatherRequirement = new Dictionary<uint, HashSet<uint>>
			{
				[2162688u] = new HashSet<uint> { 2u, 1u },
				[2162689u] = new HashSet<uint> { 1u, 2u },
				[2162690u] = new HashSet<uint> { 7u, 8u },
				[2162691u] = new HashSet<uint> { 2u, 1u },
				[2162692u] = new HashSet<uint> { 3u },
				[2162693u] = new HashSet<uint> { 2u, 1u },
				[2162694u] = new HashSet<uint> { 4u },
				[2162695u] = new HashSet<uint> { 2u, 1u },
				[2162696u] = new HashSet<uint> { 3u },
				[2162697u] = new HashSet<uint> { 1u, 2u },
				[2162698u] = new HashSet<uint> { 2u, 1u },
				[2162699u] = new HashSet<uint> { 2u, 1u },
				[2162700u] = new HashSet<uint> { 1u, 2u },
				[2162701u] = new HashSet<uint> { 2u, 1u },
				[2162702u] = new HashSet<uint> { 3u },
				[2162703u] = new HashSet<uint> { 2u, 1u },
				[2162704u] = new HashSet<uint> { 4u },
				[2162705u] = new HashSet<uint> { 7u, 8u },
				[2162706u] = new HashSet<uint> { 3u },
				[2162707u] = new HashSet<uint> { 2u, 1u },
				[2162708u] = new HashSet<uint> { 2u, 1u },
				[2162709u] = new HashSet<uint> { 1u, 2u },
				[2162710u] = new HashSet<uint> { 7u, 8u },
				[2162711u] = new HashSet<uint> { 1u, 2u },
				[2162712u] = new HashSet<uint> { 7u, 8u },
				[2162713u] = new HashSet<uint> { 1u, 2u },
				[2162714u] = new HashSet<uint> { 6u },
				[2162715u] = new HashSet<uint> { 2u, 1u },
				[2162716u] = new HashSet<uint> { 1u, 2u },
				[2162717u] = new HashSet<uint> { 2u, 1u },
				[2162718u] = new HashSet<uint> { 1u, 2u },
				[2162719u] = new HashSet<uint> { 10u },
				[2162720u] = new HashSet<uint> { 2u, 1u },
				[2162721u] = new HashSet<uint> { 3u },
				[2162722u] = new HashSet<uint> { 1u, 2u },
				[2162723u] = new HashSet<uint> { 7u, 8u },
				[2162724u] = new HashSet<uint> { 2u, 1u },
				[2162725u] = new HashSet<uint> { 7u, 8u },
				[2162726u] = new HashSet<uint> { 7u, 8u },
				[2162727u] = new HashSet<uint> { 1u, 2u },
				[2162728u] = new HashSet<uint> { 2u, 1u },
				[2162729u] = new HashSet<uint> { 1u, 2u },
				[2162730u] = new HashSet<uint> { 9u },
				[2162731u] = new HashSet<uint> { 10u },
				[2162732u] = new HashSet<uint> { 1u, 2u },
				[2162733u] = new HashSet<uint> { 4u },
				[2162734u] = new HashSet<uint> { 2u, 1u },
				[2162735u] = new HashSet<uint> { 2u, 1u },
				[2162736u] = new HashSet<uint> { 1u, 2u },
				[2162737u] = new HashSet<uint> { 3u },
				[2162738u] = new HashSet<uint> { 1u, 2u },
				[2162739u] = new HashSet<uint> { 2u, 1u },
				[2162740u] = new HashSet<uint> { 11u },
				[2162741u] = new HashSet<uint> { 1u, 2u },
				[2162742u] = new HashSet<uint> { 1u, 2u },
				[2162743u] = new HashSet<uint> { 2u, 1u },
				[2162744u] = new HashSet<uint> { 8u },
				[2162745u] = new HashSet<uint> { 4u },
				[2162746u] = new HashSet<uint> { 2u, 1u },
				[2162747u] = new HashSet<uint> { 14u },
				[2162748u] = new HashSet<uint> { 2u, 1u },
				[2162749u] = new HashSet<uint> { 14u },
				[2162750u] = new HashSet<uint> { 1u, 2u },
				[2162751u] = new HashSet<uint> { 2u, 1u },
				[2162752u] = new HashSet<uint> { 2u, 1u },
				[2162753u] = new HashSet<uint> { 3u },
				[2162754u] = new HashSet<uint> { 4u },
				[2162755u] = new HashSet<uint> { 1u, 2u },
				[2162756u] = new HashSet<uint> { 4u },
				[2162757u] = new HashSet<uint> { 16u },
				[2162758u] = new HashSet<uint> { 2u, 1u },
				[2162759u] = new HashSet<uint> { 1u, 2u },
				[2162760u] = new HashSet<uint> { 16u, 15u },
				[2162761u] = new HashSet<uint> { 2u, 1u },
				[2162762u] = new HashSet<uint> { 2u, 1u },
				[2162763u] = new HashSet<uint> { 1u, 2u },
				[2162764u] = new HashSet<uint> { 17u },
				[2162765u] = new HashSet<uint> { 2u, 1u },
				[2162766u] = new HashSet<uint> { 1u, 2u },
				[2162767u] = new HashSet<uint> { 2u, 1u }
			}.ToFrozenDictionary();
			CorrectedPosition = new Dictionary<uint, Vector3>
			{
				[2162702u] = new Vector3(15.2f, 19.8f, -0f),
				[2162785u] = new Vector3(-736.5f, 461f, 226f),
				[2162791u] = new Vector3(867.2f, 46.9f, -31.8f),
				[2162914u] = new Vector3(588.15f, -42.7f, -384.1f),
				[2163005u] = new Vector3(189.6f, -192.4f, 286.1f)
			};
			((IEnumerable<AdventureExPhase>)LuminaGetter.Get<AdventureExPhase>()).ForEach(delegate(AdventureExPhase x)
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				ExPhaseInfo[((AdventureExPhase)(ref x)).RowId] = (((AdventureExPhase)(ref x)).AdventureBegin.RowId, ((AdventureExPhase)(ref x)).AdventureEnd.RowId);
			});
		}

		public static kWNQI8xoWf3E6SpAGDe DR8xgFVnLg(Adventure data)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			uint exPhase = 999u;
			foreach (KeyValuePair<uint, (uint, uint)> item in ExPhaseInfo)
			{
				if (((Adventure)(ref data)).RowId >= item.Value.Item1 && ((Adventure)(ref data)).RowId <= item.Value.Item2)
				{
					exPhase = item.Key;
				}
			}
			Level level = ((Adventure)(ref data)).Level.Value;
			Vector3 position = level.GetPosition();
			Vector3 vector = position;
			vector.Y = position.Y - 0.8f;
			Vector3 position2 = vector;
			if (CorrectedPosition.TryGetValue(((Adventure)(ref data)).RowId, out var value))
			{
				position2 = value;
			}
			uint index = ((Adventure)(ref data)).RowId - 2162688;
			string? name = ((object)((Adventure)(ref data)).Name/*cast due to constrained. prefix*/).ToString();
			level = ((Adventure)(ref data)).Level.Value;
			return new kWNQI8xoWf3E6SpAGDe(index, name, ((Level)(ref level)).Territory.RowId, position2, ((Adventure)(ref data)).Emote.RowId, (((Adventure)(ref data)).MinTime, (uint)(((Adventure)(ref data)).MaxTime + 1)), exPhase);
		}

		public static bool NODxw8iWRQ()
		{
			return ValidPoints.Take(20).All((kWNQI8xoWf3E6SpAGDe x) => x.GeZxEAnBug());
		}

		public unsafe bool GeZxEAnBug()
		{
			return ((PlayerState)PlayerState.Instance()).IsAdventureComplete(Index);
		}

		public bool U0VxbZR8J1()
		{
			if (go4xJEo8FA() && DUOxAY7AN0() && kHjxQYTjiK())
			{
				if (Index >= 20 && Index <= 79)
				{
					return NODxw8iWRQ();
				}
				return true;
			}
			return false;
		}

		public TerritoryType n9dxFZnlxa()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<TerritoryType>(Zone).GetValueOrDefault();
		}

		public Emote JiSxqLWwHF()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<Emote>(Emote).GetValueOrDefault();
		}

		public Adventure pe5xcNV7F6()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			return LuminaGetter.GetRow<Adventure>(Index + 2162688).GetValueOrDefault();
		}

		public bool DUOxAY7AN0()
		{
			EorzeaDate time = EorzeaDate.GetTime();
			(uint, uint) timePeriod = TimePeriod;
			if (timePeriod.Item1 == 0 && timePeriod.Item2 == 1)
			{
				return true;
			}
			uint num = TimePeriod.StartTime / 100;
			uint num2 = (TimePeriod.EndTime + 1) / 100;
			if (num2 < num)
			{
				num2 += 24;
			}
			if (time.Hour >= num)
			{
				return time.Hour < num2;
			}
			return false;
		}

		public bool kHjxQYTjiK()
		{
			if (WeatherRequirement.TryGetValue(Index + 2162688, out HashSet<uint> value))
			{
				return value.Contains(EorzeaDate.GetWeather(Zone));
			}
			return true;
		}

		public bool go4xJEo8FA()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			if (ExPhase != 999 && !LuminaGetter.TryGetRow<AdventureExPhase>(ExPhase, out AdventureExPhase item))
			{
				return QuestManager.IsQuestComplete(((AdventureExPhase)(ref item)).Quest.RowId);
			}
			return true;
		}

		public unsafe void MAJxrGsOTw(AutoAdventure module)
		{
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy || DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
			{
				(ConditionFlag)27,
				(ConditionFlag)48
			}) || IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null)
			{
				return;
			}
			TerritoryType territoryTypeData;
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && LocalPlayerState.DistanceTo2D(Position.ToVector2()) <= 5f)
			{
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(Position, new TPSmartParams
				{
					CancelAnimation = false
				});
			}
			else if (!U5YxkkVIAM(module))
			{
				if (MovementManager.TryGetExitRangeInstance(out var _))
				{
					territoryTypeData = GameState.TerritoryTypeData;
					if (((TerritoryType)(ref territoryTypeData)).Mount && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && LocalPlayerState.DistanceTo3D(Position) >= 2f)
					{
						OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
						return;
					}
				}
				OmenServiceBase<MovementManager>.Instance().TPSmart_InZone(Position, new TPSmartParams
				{
					CancelAnimation = false
				});
			}
			else
			{
				territoryTypeData = GameState.TerritoryTypeData;
				if (((TerritoryType)(ref territoryTypeData)).Mount && !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4] && LocalPlayerState.DistanceTo3D(Position) >= 2f)
				{
					OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)5, 9u, 3758096384uL, 0u, (UseActionMode)0);
				}
				else if (DalamudServiceExtension.Instance().IsPluginEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B485D4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)) && vnavmeshIPC.GetIsNavReady() && !vnavmeshIPC.GetIsPathfindRunning() && !vnavmeshIPC.GetIsPathfindInProgress() && !vnavmeshIPC.GetIsNavPathfindInProgress())
				{
					vnavmeshIPC.PathfindAndMoveTo(Position, DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4]);
				}
			}
		}

		public unsafe void vpvxG3SCOS()
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)4])
			{
				BattleChara* localPlayer = Control.GetLocalPlayer();
				if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)77])
				{
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommandComplexLocation(ExecuteCommandComplexFlag.Dismount, Vector3.op_Implicit(((BattleChara)localPlayer).Position), RotationHelper.CharaToPacket(((BattleChara)localPlayer).Rotation));
				}
				else
				{
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Dismount);
				}
			}
			else
			{
				vnavmeshIPC.StopPathfind();
				((AgentEmote)AgentEmote.Instance()).ExecuteEmote((ushort)Emote, (PlayEmoteOption*)null, false, false);
			}
		}

		public List<Action> qicx6OZXKc(AutoAdventure module)
		{
			_003C_003Ec__DisplayClass45_0 CS_0024_003C_003E8__locals54 = new _003C_003Ec__DisplayClass45_0();
			CS_0024_003C_003E8__locals54.module = module;
			CS_0024_003C_003E8__locals54._003C_003E4__this = this;
			int num = 7;
			List<Action> list = new List<Action>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<Action> span = CollectionsMarshal.AsSpan(list);
			span[0] = delegate
			{
				TaskHelper? taskHelper = CS_0024_003C_003E8__locals54.module.TaskHelper;
				Func<bool> task = delegate
				{
					if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E48142 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)))
					{
						return false;
					}
					AutoAdventure autoAdventure = CS_0024_003C_003E8__locals54.module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D71388 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
					defaultInterpolatedStringHandler2.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753AA24 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC843BA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmote);
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmoteSpecial);
					vnavmeshIPC.StopPathfind();
					return !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
					{
						(ConditionFlag)3,
						(ConditionFlag)11
					});
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x7DDE6A59 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)CS_0024_003C_003E8__locals54._003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			};
			span[1] = delegate
			{
				TaskHelper? taskHelper = CS_0024_003C_003E8__locals54.module.TaskHelper;
				Func<bool> task = delegate
				{
					if (GameState.TerritoryType == CS_0024_003C_003E8__locals54._003C_003E4__this.Zone)
					{
						return true;
					}
					if (!LuminaGetter.TryGetRow<TerritoryType>(CS_0024_003C_003E8__locals54._003C_003E4__this.Zone, out TerritoryType item))
					{
						return false;
					}
					AutoAdventure autoAdventure = CS_0024_003C_003E8__locals54.module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3CA6A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
					defaultInterpolatedStringHandler2.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1C06E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1FBF26 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1), item.ExtractPlaceName()));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					AetheryteRecord nearestAetheryte = OmenServiceBase<AetheryteRecordManager>.Instance().GetNearestAetheryte(CS_0024_003C_003E8__locals54._003C_003E4__this.Zone, CS_0024_003C_003E8__locals54._003C_003E4__this.Position);
					OmenServiceBase<ChatManager>.Instance().SendMessage(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D63FF6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846) + nearestAetheryte.Name);
					return true;
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6855EFCD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)CS_0024_003C_003E8__locals54._003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			};
			span[2] = delegate
			{
				TaskHelper? taskHelper = CS_0024_003C_003E8__locals54.module.TaskHelper;
				Func<bool> task = () => GameState.TerritoryType == CS_0024_003C_003E8__locals54._003C_003E4__this.Zone && UIModuleExtension.IsScreenReady();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -507444256 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)CS_0024_003C_003E8__locals54._003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			};
			span[3] = delegate
			{
				TaskHelper? taskHelper = CS_0024_003C_003E8__locals54.module.TaskHelper;
				Func<bool> task = delegate
				{
					if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA5ED06 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)))
					{
						return false;
					}
					AutoAdventure autoAdventure = CS_0024_003C_003E8__locals54.module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x21946A6E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
					defaultInterpolatedStringHandler2.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x36953753 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420DDEB4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmote);
					OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.InterruptEmoteSpecial);
					vnavmeshIPC.StopPathfind();
					return !DalamudServiceExtension.Instance<ICondition>().Any((ConditionFlag[])(object)new ConditionFlag[2]
					{
						(ConditionFlag)3,
						(ConditionFlag)11
					});
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420DDD06 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)CS_0024_003C_003E8__locals54._003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			};
			span[4] = delegate
			{
				TaskHelper? taskHelper = CS_0024_003C_003E8__locals54.module.TaskHelper;
				Action task = delegate
				{
					if (CS_0024_003C_003E8__locals54._003C_003E4__this.GeZxEAnBug() || !CS_0024_003C_003E8__locals54._003C_003E4__this.U0VxbZR8J1())
					{
						CS_0024_003C_003E8__locals54.module.TaskHelper.RemoveQueueTasks((int)CS_0024_003C_003E8__locals54._003C_003E4__this.Index);
					}
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE685A9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)CS_0024_003C_003E8__locals54._003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			};
			span[5] = delegate
			{
				TaskHelper? taskHelper = CS_0024_003C_003E8__locals54.module.TaskHelper;
				Func<bool> task = delegate
				{
					if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C43EE7E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)))
					{
						return false;
					}
					if (GameState.TerritoryType != CS_0024_003C_003E8__locals54._003C_003E4__this.Zone)
					{
						return false;
					}
					AutoAdventure autoAdventure = CS_0024_003C_003E8__locals54.module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D7521B2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
					defaultInterpolatedStringHandler2.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CBF8D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6855EEB1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5), CS_0024_003C_003E8__locals54._003C_003E4__this.Position.ToString(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09FA91 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363))));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					CS_0024_003C_003E8__locals54._003C_003E4__this.MAJxrGsOTw(CS_0024_003C_003E8__locals54.module);
					return LocalPlayerState.DistanceTo3DSquared(CS_0024_003C_003E8__locals54._003C_003E4__this.Position) <= 9f;
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E01A702 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)CS_0024_003C_003E8__locals54._003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			};
			span[6] = delegate
			{
				TaskHelper? taskHelper = CS_0024_003C_003E8__locals54.module.TaskHelper;
				Func<bool> task = delegate
				{
					//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
					//IL_0103: Unknown result type (might be due to invalid IL or missing references)
					//IL_0106: Unknown result type (might be due to invalid IL or missing references)
					//IL_010b: Unknown result type (might be due to invalid IL or missing references)
					if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477F172D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)))
					{
						return false;
					}
					if (OmenServiceBase<MovementManager>.Instance().IsManagerBusy || !UIModuleExtension.IsScreenReady())
					{
						return false;
					}
					if (CS_0024_003C_003E8__locals54._003C_003E4__this.GeZxEAnBug())
					{
						return true;
					}
					AutoAdventure autoAdventure = CS_0024_003C_003E8__locals54.module;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D881D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
					defaultInterpolatedStringHandler2.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F369BDE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
					string key = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469B2DB5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92);
					object[] array = new object[1];
					Emote val = CS_0024_003C_003E8__locals54._003C_003E4__this.JiSxqLWwHF();
					array[0] = ((object)((Emote)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
					defaultInterpolatedStringHandler2.AppendFormatted(LanguageManager.Get(key, array));
					autoAdventure.message = defaultInterpolatedStringHandler2.ToStringAndClear();
					CS_0024_003C_003E8__locals54._003C_003E4__this.MAJxrGsOTw(CS_0024_003C_003E8__locals54.module);
					if (LocalPlayerState.DistanceTo3DSquared(CS_0024_003C_003E8__locals54._003C_003E4__this.Position) > 1f)
					{
						return false;
					}
					CS_0024_003C_003E8__locals54._003C_003E4__this.vpvxG3SCOS();
					return CS_0024_003C_003E8__locals54._003C_003E4__this.GeZxEAnBug();
				};
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C343538 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
				defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals54._003C_003E4__this.Index);
				string name = defaultInterpolatedStringHandler.ToStringAndClear();
				int index = (int)CS_0024_003C_003E8__locals54._003C_003E4__this.Index;
				taskHelper.Enqueue(task, name, 0, null, null, null, null, index);
			};
			return list;
		}

		private static bool U5YxkkVIAM(AutoAdventure module)
		{
			if (!Sheets.SpeedDetectionZones.ContainsKey(GameState.TerritoryType) && !Control.CanFly)
			{
				return false;
			}
			if (module.config.AlwaysPathfinding)
			{
				return true;
			}
			if (AuthState.IsPremium)
			{
				return false;
			}
			if ((GameState.IsCN || GameState.IsTC) && Sheets.SpeedDetectionZones.ContainsKey(GameState.TerritoryType))
			{
				return true;
			}
			return false;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427E742 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7DDAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
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
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E004516 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
			builder.Append(Index.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC8D7E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09));
			builder.Append((object?)Name);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35D79A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
			builder.Append(Zone.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x475365FC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c));
			builder.Append(Position.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9A0E4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37));
			builder.Append(Emote.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F7892 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			builder.Append(TimePeriod.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE0932C1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363));
			builder.Append(ExPhase.ToString());
			return true;
		}

		[CompilerGenerated]
		protected kWNQI8xoWf3E6SpAGDe(kWNQI8xoWf3E6SpAGDe original)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			Index = original.Index;
			Name = original.Name;
			Zone = original.Zone;
			Position = original.Position;
			Emote = original.Emote;
			TimePeriod = original.TimePeriod;
			ExPhase = original.ExPhase;
		}

		[CompilerGenerated]
		public void QEvx2AJO1a(out uint Index, out string Name, out uint Zone, out Vector3 Position, out uint Emote, out (uint, uint) TimePeriod, out uint ExPhase)
		{
			Index = this.Index;
			Name = this.Name;
			Zone = this.Zone;
			Position = this.Position;
			Emote = this.Emote;
			TimePeriod = this.TimePeriod;
			ExPhase = this.ExPhase;
		}
	}

	private class PeTK7OxZ3CNqFlMO3GZ : ModuleConfig
	{
		public bool AlwaysPathfinding;

		public PeTK7OxZ3CNqFlMO3GZ()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			AlwaysPathfinding = true;
			base._002Ector();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass11_0
	{
		public kWNQI8xoWf3E6SpAGDe p;

		public AutoAdventure _003C_003E4__this;

		public _003C_003Ec__DisplayClass11_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal void SeJxajxpSq()
		{
			if (!p.GeZxEAnBug() && p.U0VxbZR8J1())
			{
				p.qicx6OZXKc(_003C_003E4__this).ForEach(delegate(Action x)
				{
					x();
				});
			}
		}
	}

	private PeTK7OxZ3CNqFlMO3GZ config;

	private string message;

	[CompilerGenerated]
	private static uint[] _003CVistaQuests_003Ek__BackingField;

	[CompilerGenerated]
	private static kWNQI8xoWf3E6SpAGDe[] _003CValidPoints_003Ek__BackingField;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	private static uint[] VistaQuests
	{
		get
		{
			if (_003CVistaQuests_003Ek__BackingField != null)
			{
				return _003CVistaQuests_003Ek__BackingField;
			}
			uint num = 65698u;
			uint num2 = 67643u;
			List<uint> list = (from x in (IEnumerable<AdventureExPhase>)LuminaGetter.Get<AdventureExPhase>()
				select ((AdventureExPhase)(ref x)).Quest.RowId into x
				where x != 0
				select x).ToList();
			int num3 = 2 + list.Count;
			List<uint> list2 = new List<uint>(num3);
			CollectionsMarshal.SetCount(list2, num3);
			Span<uint> span = CollectionsMarshal.AsSpan(list2);
			int num4 = 0;
			span[num4] = num;
			num4++;
			span[num4] = num2;
			num4++;
			Span<uint> span2 = CollectionsMarshal.AsSpan(list);
			span2.CopyTo(span.Slice(num4, span2.Length));
			num4 += span2.Length;
			return _003CVistaQuests_003Ek__BackingField = list2.ToArray();
		}
	}

	private static kWNQI8xoWf3E6SpAGDe[] ValidPoints
	{
		get
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (_003CValidPoints_003Ek__BackingField != null)
			{
				return _003CValidPoints_003Ek__BackingField;
			}
			List<kWNQI8xoWf3E6SpAGDe> list = new List<kWNQI8xoWf3E6SpAGDe>();
			Enumerator<Adventure> enumerator = LuminaGetter.Get<Adventure>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					kWNQI8xoWf3E6SpAGDe item = kWNQI8xoWf3E6SpAGDe.DR8xgFVnLg(enumerator.Current);
					list.Add(item);
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			return _003CValidPoints_003Ek__BackingField = list.ToArray();
		}
	}

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<PeTK7OxZ3CNqFlMO3GZ>(this) ?? new PeTK7OxZ3CNqFlMO3GZ();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = int.MaxValue
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
	}

	protected override void ConfigUI()
	{
		MSXMMjY0Lp();
		ImGui.NewLine();
		QZdM4aNj9I();
		ImGui.NewLine();
		lg0Msf35Fh();
	}

	private unsafe static void MSXMMjY0Lp()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42C1CA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd))));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			vector = KnownColor.Orange.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C1753D9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)));
			ImGui.SameLine();
			ImU8String val2 = new ImU8String(1, 1);
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A25D3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x9D62FA9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c)));
			ImGui.TextUnformatted(val2);
			if ((GameState.IsCN || GameState.IsTC) && !AuthState.IsPremium)
			{
				bool flag = DalamudServiceExtension.Instance().IsPluginEnabled(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FB45A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
				vector = (flag ? KnownColor.GreenYellow.ToVector4() : KnownColor.Red.ToVector4());
				ImGui.TextColored(ref vector, ImU8String.op_Implicit(flag ? LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39513A69 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497) : LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x17788C77 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)));
				ImGui.SameLine();
				ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE311C7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC41D4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09))));
			}
			vector = KnownColor.Chocolate.ToVector4();
			ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE0A74 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a))));
			IndentDisposable val3 = ImRaii.PushIndent(1, true);
			try
			{
				uint[] vistaQuests = VistaQuests;
				foreach (uint num in vistaQuests)
				{
					if (LuminaGetter.TryGetRow<Quest>(num, out Quest item))
					{
						Level level = ((Quest)(ref item)).IssuerLocation.Value;
						bool flag2 = QuestManager.IsQuestComplete(num);
						vector = (flag2 ? KnownColor.GreenYellow.ToVector4() : KnownColor.Red.ToVector4());
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(flag2 ? LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -1315017788 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc) : LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17537F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)));
						ImGui.SameLine();
						ImU8String val4 = new ImU8String(3, 2);
						((ImU8String)(ref val4)).AppendFormatted<string>(((object)((Quest)(ref item)).Name/*cast due to constrained. prefix*/).ToString());
						((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CCA804 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361));
						((ImU8String)(ref val4)).AppendFormatted<string>(((Level)(ref level)).Territory.Value.ExtractPlaceName());
						((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C9D71 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee));
						ImGui.TextUnformatted(val4);
						if (ImGui.IsItemHovered())
						{
							ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						}
						if (ImGui.IsItemClicked())
						{
							AgentMap* intPtr = AgentMap.Instance();
							((AgentMap)intPtr).SetFlagMapMarker(((Level)(ref level)).Territory.RowId, ((Level)(ref level)).Map.RowId, level.GetPosition(), 60561u);
							((AgentMap)intPtr).OpenMap(((Level)(ref level)).Map.RowId, ((Level)(ref level)).Territory.RowId, ((object)((Quest)(ref item)).Name/*cast due to constrained. prefix*/).ToString(), (MapType)1);
						}
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
			((IDisposable)val)?.Dispose();
		}
	}

	private void QZdM4aNj9I()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C5B62 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948))), ref config.AlwaysPathfinding))
		{
			config.Save(this);
		}
		DisabledDisposable val = ImRaii.Disabled(base.TaskHelper?.IsBusy ?? true);
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61515, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EBB2B1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9))))
			{
				ValidPoints.ForEach(delegate(kWNQI8xoWf3E6SpAGDe p)
				{
					_003C_003Ec__DisplayClass11_0 CS_0024_003C_003E8__locals8 = new _003C_003Ec__DisplayClass11_0();
					CS_0024_003C_003E8__locals8._003C_003E4__this = this;
					CS_0024_003C_003E8__locals8.p = p;
					if (!CS_0024_003C_003E8__locals8.p.GeZxEAnBug() && CS_0024_003C_003E8__locals8.p.U0VxbZR8J1())
					{
						base.TaskHelper.Enqueue(delegate
						{
							if (!CS_0024_003C_003E8__locals8.p.GeZxEAnBug() && CS_0024_003C_003E8__locals8.p.U0VxbZR8J1())
							{
								CS_0024_003C_003E8__locals8.p.qicx6OZXKc(CS_0024_003C_003E8__locals8._003C_003E4__this).ForEach(delegate(Action x)
								{
									x();
								});
							}
						}, null, 0, null, null, null, null, -2);
					}
				});
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61517, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BFAEA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded))))
		{
			base.TaskHelper.Abort();
			OmenServiceBase<MovementManager>.Instance().AbortTasks();
			message = string.Empty;
			vnavmeshIPC.StopPathfind();
		}
		ImGui.Spacing();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(0, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE0AFE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)));
		ImGui.TextColored(ref vector, val2);
		IndentDisposable val3 = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val4 = new ImU8String(0, 1);
			((ImU8String)(ref val4)).AppendFormatted<string>(message);
			ImGui.TextUnformatted(val4);
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
	}

	private unsafe static void lg0Msf35Fh()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0567: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_0867: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			EorzeaDate time = EorzeaDate.GetTime();
			ImU8String val2 = new ImU8String(2, 1);
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C175073 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb));
			((ImU8String)(ref val2)).AppendFormatted<string>(time.ToShortString());
			ImGui.TextUnformatted(val2);
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImGui.SameLine(0f, 12f * Globals.GlobalUIScale);
		GroupDisposable val3 = ImRaii.Group();
		try
		{
			if (LuminaGetter.TryGetRow<Weather>((uint)((WeatherManager)WeatherManager.Instance()).GetCurrentWeather(), out Weather item))
			{
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val4 = new GameIconLookup((uint)((Weather)(ref item)).Icon, false, true, (ClientLanguage?)null);
				ISharedImmediateTexture val5 = default(ISharedImmediateTexture);
				if (obj.TryGetFromGameIcon(ref val4, ref val5))
				{
					ImGui.Image(val5.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeight()));
					ImGui.SameLine();
					ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((Weather)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
				}
			}
		}
		finally
		{
			((GroupDisposable)(ref val3)).Dispose();
		}
		ImGui.Spacing();
		ISharedImmediateTexture val13 = default(ISharedImmediateTexture);
		ISharedImmediateTexture val15 = default(ISharedImmediateTexture);
		ISharedImmediateTexture val17 = default(ISharedImmediateTexture);
		foreach (IGrouping<uint, kWNQI8xoWf3E6SpAGDe> item3 in ValidPoints.GroupBy(delegate(kWNQI8xoWf3E6SpAGDe x)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			TerritoryType val19 = x.n9dxFZnlxa();
			return ((TerritoryType)(ref val19)).ExVersion.RowId;
		}))
		{
			ImU8String val6 = new ImU8String(13, 5);
			((ImU8String)(ref val6)).AppendFormatted<uint>(item3.Key + 2);
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427295A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EBB4D5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9));
			((ImU8String)(ref val6)).AppendFormatted<int>(item3.Count((kWNQI8xoWf3E6SpAGDe x) => x.GeZxEAnBug()));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1C050 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
			((ImU8String)(ref val6)).AppendFormatted<int>(item3.Count());
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x21946A5E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5A99E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
			((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x231073B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d)));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -131651475 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
			((ImU8String)(ref val6)).AppendFormatted<int>(item3.Count((kWNQI8xoWf3E6SpAGDe x) => !x.GeZxEAnBug() && x.U0VxbZR8J1()));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D67DD0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560));
			TreeNodeDisposable val7 = ImRaii.TreeNode(val6);
			try
			{
				if (!val7)
				{
					continue;
				}
				ImU8String val8 = new ImU8String(0, 1);
				((ImU8String)(ref val8)).AppendFormatted<uint>(item3.Key + 2);
				TableDisposable val9 = ImRaii.Table(val8, 6, (ImGuiTableFlags)705);
				try
				{
					if (!val9)
					{
						continue;
					}
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BFD78 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420CF164 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)), false, -1f).X, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42C3B0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5AFB4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)), false, -1f).X, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x17788EF3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)), (ImGuiTableColumnFlags)4, 10f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42C3D4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)), (ImGuiTableColumnFlags)4, 10f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE31347 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)), (ImGuiTableColumnFlags)4, 20f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF20ED4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765)), (ImGuiTableColumnFlags)4, 20f, 0u);
					foreach (kWNQI8xoWf3E6SpAGDe item4 in item3)
					{
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						Vector4 vector = (item4.GeZxEAnBug() ? KnownColor.GreenYellow.ToVector4() : KnownColor.Red.ToVector4());
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(item4.GeZxEAnBug() ? LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A03A7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92) : LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50177A0E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)));
						ImGui.TableNextColumn();
						ImU8String val10 = new ImU8String(0, 1);
						((ImU8String)(ref val10)).AppendFormatted<uint>(item4.Index + 1);
						ImGui.TextUnformatted(val10);
						ImGui.TableNextColumn();
						(uint, uint) timePeriod = item4.TimePeriod;
						if (timePeriod.Item1 != 0 || timePeriod.Item2 != 1)
						{
							ImU8String val11 = new ImU8String(11, 2);
							((ImU8String)(ref val11)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23107399 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d));
							((ImU8String)(ref val11)).AppendFormatted<uint>(item4.TimePeriod.StartTime / 100);
							((ImU8String)(ref val11)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42C3EC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd));
							((ImU8String)(ref val11)).AppendFormatted<uint>(item4.TimePeriod.EndTime / 100);
							((ImU8String)(ref val11)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C5A74 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948));
							ImGui.TextUnformatted(val11);
						}
						ImGui.TableNextColumn();
						GameIconLookup val4;
						if (kWNQI8xoWf3E6SpAGDe.WeatherRequirement.TryGetValue(item4.Index + 2162688, out HashSet<uint> value))
						{
							foreach (uint item5 in value)
							{
								GroupDisposable val12 = ImRaii.Group();
								try
								{
									if (LuminaGetter.TryGetRow<Weather>(item5, out Weather item2))
									{
										ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
										val4 = new GameIconLookup((uint)((Weather)(ref item2)).Icon, false, true, (ClientLanguage?)null);
										if (obj2.TryGetFromGameIcon(ref val4, ref val13))
										{
											ImGui.Image(val13.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
											ImGuiOm.TooltipHover(((object)((Weather)(ref item2)).Name/*cast due to constrained. prefix*/).ToString());
											goto IL_073e;
										}
									}
								}
								finally
								{
									((GroupDisposable)(ref val12)).Dispose();
								}
								continue;
								IL_073e:
								ImGui.SameLine();
							}
							ImGui.NewLine();
						}
						ImGui.TableNextColumn();
						ITextureProvider obj3 = DalamudServiceExtension.Instance<ITextureProvider>();
						Adventure val14 = item4.pe5xcNV7F6();
						val4 = new GameIconLookup((uint)((Adventure)(ref val14)).IconList, false, true, (ClientLanguage?)null);
						if (obj3.TryGetFromGameIcon(ref val4, ref val15))
						{
							ImGuiOm.TextImage(item4.Name, val15.GetWrapOrEmpty().Handle, new Vector2(ImGui.GetTextLineHeightWithSpacing()));
						}
						else
						{
							ImU8String val16 = new ImU8String(0, 1);
							((ImU8String)(ref val16)).AppendFormatted<string>(item4.Name);
							ImGui.TextUnformatted(val16);
						}
						if (ImGui.IsItemHovered())
						{
							ITextureProvider obj4 = DalamudServiceExtension.Instance<ITextureProvider>();
							val14 = item4.pe5xcNV7F6();
							val4 = new GameIconLookup((uint)((Adventure)(ref val14)).IconDiscovered, false, true, (ClientLanguage?)null);
							if (obj4.TryGetFromGameIcon(ref val4, ref val17))
							{
								ImGui.BeginTooltip();
								ImGui.Image(val17.GetWrapOrEmpty().Handle, val17.GetWrapOrEmpty().Size);
								ImGui.EndTooltip();
							}
						}
						ImGui.TableNextColumn();
						ImU8String val18 = new ImU8String(0, 1);
						TerritoryType row = item4.n9dxFZnlxa();
						((ImU8String)(ref val18)).AppendFormatted<string>(row.ExtractPlaceName());
						ImGui.TextUnformatted(val18);
						if (ImGui.IsItemHovered())
						{
							ImGui.SetMouseCursor((ImGuiMouseCursor)7);
						}
						if (ImGui.IsItemClicked())
						{
							AgentMap* intPtr = AgentMap.Instance();
							uint zone = item4.Zone;
							row = item4.n9dxFZnlxa();
							((AgentMap)intPtr).SetFlagMapMarker(zone, ((TerritoryType)(ref row)).Map.RowId, item4.Position, 60561u);
							row = item4.n9dxFZnlxa();
							((AgentMap)intPtr).OpenMap(((TerritoryType)(ref row)).Map.RowId, item4.Zone, item4.Name, (MapType)1);
						}
					}
				}
				finally
				{
					((TableDisposable)(ref val9)).Dispose();
				}
			}
			finally
			{
				((TreeNodeDisposable)(ref val7)).Dispose();
			}
		}
	}

	public AutoAdventure()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4753AB44 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7A8D3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)),
			Category = ModuleCategory.Script,
			ModulesPrerequisite = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951385D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497) },
			ModulesConflict = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4986E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		message = string.Empty;
		base._002Ector();
	}

	[CompilerGenerated]
	private void LG6M8eyAtG(kWNQI8xoWf3E6SpAGDe p)
	{
		_003C_003Ec__DisplayClass11_0 CS_0024_003C_003E8__locals8 = new _003C_003Ec__DisplayClass11_0();
		CS_0024_003C_003E8__locals8._003C_003E4__this = this;
		CS_0024_003C_003E8__locals8.p = p;
		if (CS_0024_003C_003E8__locals8.p.GeZxEAnBug() || !CS_0024_003C_003E8__locals8.p.U0VxbZR8J1())
		{
			return;
		}
		base.TaskHelper.Enqueue(delegate
		{
			if (!CS_0024_003C_003E8__locals8.p.GeZxEAnBug() && CS_0024_003C_003E8__locals8.p.U0VxbZR8J1())
			{
				CS_0024_003C_003E8__locals8.p.qicx6OZXKc(CS_0024_003C_003E8__locals8._003C_003E4__this).ForEach(delegate(Action x)
				{
					x();
				});
			}
		}, null, 0, null, null, null, null, -2);
	}
}
