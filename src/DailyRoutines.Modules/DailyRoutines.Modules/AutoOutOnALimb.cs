using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Network;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Text;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Downstream;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoOutOnALimb : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddonSetup;

		public static Action _003C1_003E__SendInteractWithMachine;
	}

	private Hook<HandleEventYieldPacket>? HandleGoldSaucerMiniGamePacketHook;

	private int currentRound;

	private uint hitPosition;

	private long gameStartTimestamp;

	private readonly int[] tryState;

	private const uint EVENT_ID = 2359302u;

	private const uint ROUND_START_CATEGORY = 17235982u;

	private const uint DIFFICULTY_CATEGORY = 17367054u;

	private const uint NEXT_ROUND_CATEGORY = 720910u;

	private const uint CUT_TREE_CATEGORY = 17432590u;

	private const float ROUND_TIME_SECONDS = 10f;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		base.TaskHelper = new TaskHelper();
		HandleGoldSaucerMiniGamePacketHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<HandleEventYieldPacket>(typeof(MemberFunctionPointers), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD192C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc), new HandleEventYieldPacket(cTe15GWi1C));
		HandleGoldSaucerMiniGamePacketHook.Enable();
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		string text = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09FBC1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363);
		object obj2 = _003C_003EO._003C0_003E__OnAddonSetup;
		if (obj2 == null)
		{
			AddonEventDelegate val = y421REvKIg;
			_003C_003EO._003C0_003E__OnAddonSetup = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)1, text, (AddonEventDelegate)obj2);
	}

	protected unsafe override void Uninit()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		AddonEventDelegate[] array = new AddonEventDelegate[1];
		object obj2 = _003C_003EO._003C0_003E__OnAddonSetup;
		if (obj2 == null)
		{
			AddonEventDelegate val = y421REvKIg;
			_003C_003EO._003C0_003E__OnAddonSetup = val;
			obj2 = (object)val;
		}
		array[0] = (AddonEventDelegate)obj2;
		obj.UnregisterListener((AddonEventDelegate[])(object)array);
		if ((*Addons.MiniGameAimg).IsAddonAndNodesReady())
		{
			xBJ1UDLOJ9();
		}
	}

	protected override void ConfigUI()
	{
		ImGuiOmExtension.ConflictKeyText();
		ImGui.NewLine();
		DisabledDisposable val = ImRaii.Disabled(GameState.TerritoryType != 388 || base.TaskHelper.IsBusy || IConditionExtension.get_IsOccupiedInEvent(DalamudServiceExtension.Instance<ICondition>()));
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61515, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE314AD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818))))
			{
				shF1etO4Fx();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61517, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -954323845 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e))))
		{
			base.TaskHelper.Abort();
			xBJ1UDLOJ9();
		}
	}

	private unsafe void cTe15GWi1C(EventId eventID, short scene, byte yieldID, int* intParams, byte intParamsCount)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		HandleGoldSaucerMiniGamePacketHook.Original.Invoke(eventID, scene, yieldID, intParams, intParamsCount);
		if (eventID != EventId.op_Implicit(2359302u))
		{
			return;
		}
		switch (yieldID)
		{
		case 23:
			if (((OutOnALimbPacket*)intParams)->Result == OutOnALimbResult.Start && ((OutOnALimbPacket*)intParams)->Health == 10 && ((OutOnALimbPacket*)intParams)->BonusLevel == 5)
			{
				currentRound = 0;
				gameStartTimestamp = Stopwatch.GetTimestamp();
				sJx1utWwPs(currentRound, ((OutOnALimbPacket*)intParams)->Result);
				HOf1I45661(20u);
			}
			break;
		case 24:
			if (((OutOnALimbPacket*)intParams)->Health != 0)
			{
				if (((OutOnALimbPacket*)intParams)->BonusLevel != 5)
				{
					byte nextPosition = oGw1z2RUmR(((OutOnALimbPacket*)intParams)->Result);
					sJx1utWwPs(currentRound, ((OutOnALimbPacket*)intParams)->Result);
					HOf1I45661(nextPosition);
				}
				break;
			}
			currentRound++;
			sJx1utWwPs(currentRound, ((OutOnALimbPacket*)intParams)->Result);
			if (((OutOnALimbPacket*)intParams)->Result == OutOnALimbResult.Fail)
			{
				GDe1D3kElC();
			}
			else if (currentRound >= 6)
			{
				base.TaskHelper.Abort();
				if (Stopwatch.GetElapsedTime(gameStartTimestamp).TotalSeconds < 10.0)
				{
					base.TaskHelper.Enqueue(delegate
					{
						TimeSpan elapsedTime = Stopwatch.GetElapsedTime(gameStartTimestamp);
						if (elapsedTime.TotalSeconds > 10.0)
						{
							return true;
						}
						string key = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44272E18 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b);
						object[] array = new object[1];
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
						defaultInterpolatedStringHandler.AppendFormatted(MathF.Max(10f - (float)elapsedTime.TotalSeconds, 0f), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42C542 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd));
						array[0] = defaultInterpolatedStringHandler.ToStringAndClear();
						pXD1XsFv8E(LanguageManager.Get(key, array));
						return false;
					});
				}
				GDe1D3kElC();
			}
			else
			{
				OPS1YN0UPW();
				uFv1deHnGL();
				HOf1I45661(20u);
			}
			break;
		}
	}

	private static void y421REvKIg(AddonEvent type, AddonArgs args)
	{
		vvV1NTjao3();
		ITt17nFQNK();
	}

	private unsafe void GDe1D3kElC()
	{
		base.TaskHelper.Enqueue(delegate
		{
			if (Addons.MiniGameAimg != null)
			{
				base.TaskHelper.AbortByConflictKey(this);
				xBJ1UDLOJ9();
			}
		});
		base.TaskHelper.Enqueue((Action)OPS1YN0UPW, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		base.TaskHelper.Enqueue((Action)shF1etO4Fx, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private static void shF1etO4Fx()
	{
		new EventStartPackt(LocalPlayerState.EntityID, 2359302u).Send();
	}

	private static void vvV1NTjao3()
	{
		new EventActionPacket(2359302u, 17235982u).Send();
	}

	private static void ITt17nFQNK()
	{
		new EventActionPacket(2359302u, 17367054u, 2u).Send();
	}

	private static void uFv1deHnGL()
	{
		new EventActionPacket(2359302u, 720910u).Send();
	}

	private void HOf1I45661(uint nextPosition)
	{
		new EventActionPacket(2359302u, 17432590u, nextPosition).Send();
		hitPosition = nextPosition;
	}

	private static void xBJ1UDLOJ9()
	{
		new EventCompletePackt(2359302u, 14u).Send();
	}

	private static void sJx1utWwPs(int round, OutOnALimbResult hitResult)
	{
		string logMessageText = LuminaWrapper.GetLogMessageText((uint)(4785 + hitResult));
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
		defaultInterpolatedStringHandler.AppendFormatted(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42C44C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)));
		defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7AE7B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
		defaultInterpolatedStringHandler.AppendFormatted(round);
		pXD1XsFv8E(defaultInterpolatedStringHandler.ToStringAndClear() + (string.IsNullOrEmpty(logMessageText) ? string.Empty : (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EBB30D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9) + logMessageText + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -612847362 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7))));
	}

	private unsafe static void pXD1XsFv8E(string info)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (!(*Addons.SelectString).IsAddonAndNodesReady() || !(*Addons.MiniGameAimg).IsAddonAndNodesReady())
		{
			return;
		}
		AtkComponentList* componentListById = ((AtkUnitBase)Addons.SelectString).GetComponentListById(3u);
		AtkTextNode* textNodeById = ((AtkUnitBase)Addons.SelectString).GetTextNodeById(2u);
		if (componentListById == null || textNodeById == null)
		{
			return;
		}
		((AtkComponentNode)((AtkComponentList)componentListById).OwnerNode).ToggleVisibility(false);
		((AtkComponentList)componentListById).SetEnabledState(false);
		((AtkTextNode)textNodeById).FontSize = 18;
		((AtkTextNode)textNodeById).AlignmentType = (AlignmentType)4;
		RentedSeStringBuilder val = default(RentedSeStringBuilder);
		((RentedSeStringBuilder)(ref val))._002Ector();
		try
		{
			SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
			SeStringBuilder val2 = builder.PushColorType(28u);
			SeStringInterpolatedStringHandler val3 = new SeStringInterpolatedStringHandler(2, 1, val2);
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x177888AD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1));
			((SeStringInterpolatedStringHandler)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D19355 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)));
			((SeStringInterpolatedStringHandler)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4812A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360));
			val2.Append(val3).PopColorType().AppendNewLine()
				.Append(info);
			((AtkTextNode)textNodeById).SetText(builder.GetViewAsSpan());
			((AtkTextNode)textNodeById).SetPositionFloat(20f, 60f);
		}
		finally
		{
			((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void OPS1YN0UPW()
	{
		hitPosition = 20u;
		for (int i = 0; i < tryState.Length; i++)
		{
			tryState[i] = 0;
		}
	}

	private byte oGw1z2RUmR(OutOnALimbResult hitResult)
	{
		k87MnPoGq0(hitPosition, hitResult + 1);
		return (byte)nVpMyQlZmH();
	}

	private void k87MnPoGq0(uint position, OutOnALimbResult result)
	{
		for (int i = 0; i < tryState.Length; i++)
		{
			long num = Math.Abs(position - i);
			switch (result)
			{
			case OutOnALimbResult.Normal:
				if (num < 20 && tryState[i] == 0)
				{
					tryState[i] = -1;
				}
				break;
			case OutOnALimbResult.Great:
			{
				long num2 = num;
				if (num2 > 5)
				{
					if (num2 > 25 && tryState[i] == 0)
					{
						tryState[i] = -1;
					}
				}
				else
				{
					tryState[i] = 1;
				}
				break;
			}
			case OutOnALimbResult.Perfect:
			{
				long num2 = num;
				if (num2 <= 5)
				{
					if (num2 == 0L)
					{
						tryState[i] = 2;
					}
				}
				else if (tryState[i] == 0)
				{
					tryState[i] = -1;
				}
				break;
			}
			}
		}
	}

	private int nVpMyQlZmH()
	{
		int num = 0;
		int num2 = -1;
		int num3 = 0;
		for (int i = 0; i < tryState.Length; i++)
		{
			if (tryState[i] != 0)
			{
				num = i;
				continue;
			}
			int num4 = i - num;
			if (num4 > num3)
			{
				num2 = num;
				num3 = num4;
			}
		}
		int num5 = num2 + num3 / 2;
		if (num5 == hitPosition)
		{
			num5++;
		}
		if (num5 < 0)
		{
			num5 = 0;
		}
		if (num5 > 99)
		{
			num5 = 99;
		}
		return num5;
	}

	public AutoOutOnALimb()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -1329170343 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E9432 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)),
			Category = ModuleCategory.GoldSaucer,
			Author = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F369CBA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		tryState = new int[100];
		base._002Ector();
	}

	[CompilerGenerated]
	private bool QgLMLekOwD()
	{
		TimeSpan elapsedTime = Stopwatch.GetElapsedTime(gameStartTimestamp);
		if (elapsedTime.TotalSeconds > 10.0)
		{
			return true;
		}
		string key = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x44272E18 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b);
		object[] array = new object[1];
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
		defaultInterpolatedStringHandler.AppendFormatted(MathF.Max(10f - (float)elapsedTime.TotalSeconds, 0f), LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42C542 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd));
		array[0] = defaultInterpolatedStringHandler.ToStringAndClear();
		pXD1XsFv8E(LanguageManager.Get(key, array));
		return false;
	}

	[CompilerGenerated]
	private unsafe void WtbMH64kY9()
	{
		if (Addons.MiniGameAimg != null)
		{
			base.TaskHelper.AbortByConflictKey(this);
			xBJ1UDLOJ9();
		}
	}
}
