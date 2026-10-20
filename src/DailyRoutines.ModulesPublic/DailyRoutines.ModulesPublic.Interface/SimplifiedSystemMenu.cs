using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic.Interface;

public class SimplifiedSystemMenu : ModuleBase
{
	private unsafe delegate byte GenerateSystemMenuDelegate(AgentHUD* agent, AtkValue* atkValueArgs, int valueCount, ulong eventKind);

	private Hook<OpenSystemMenu> OpenSystemMenuHook;

	private static readonly CompSig GenerateSystemMenuSig = new CompSig("40 53 55 56 57 48 81 EC ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ?? 48 33 C4 48 89 84 24 ?? ?? ?? ?? 49 8B D9");

	private Hook<GenerateSystemMenuDelegate> GenerateSystemMenuHook;

	private const int MAX_ENTRIES = 20;

	private const int START_INDEX = 5;

	private static readonly FrozenSet<uint> BlockedCommandID = FrozenSet.Create<uint>(30u, 32u, 37u, 68u, 90u);

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("SimplifiedSystemMenuTitle"),
		Description = LanguageManager.Get("SimplifiedSystemMenuDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		OpenSystemMenuHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<OpenSystemMenu>(typeof(MemberFunctionPointers), "OpenSystemMenu", new OpenSystemMenu(OpenSystemMenuDetour));
		OpenSystemMenuHook.Enable();
		GenerateSystemMenuHook = GenerateSystemMenuSig.GetHook<GenerateSystemMenuDelegate>(GenerateSystemMenuDetour);
		GenerateSystemMenuHook.Enable();
	}

	private unsafe byte GenerateSystemMenuDetour(AgentHUD* agent, AtkValue* atkValueArgs, int valueCount, ulong eventKind)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if (eventKind == 102 || atkValueArgs == null)
		{
			return GenerateSystemMenuHook.Original(agent, atkValueArgs, valueCount, eventKind);
		}
		if (((AtkValue)atkValueArgs).Int != 1)
		{
			return GenerateSystemMenuHook.Original(agent, atkValueArgs, valueCount, eventKind);
		}
		if ((((AtkValue)((byte*)atkValueArgs + (nint)4 * (nint)Unsafe.SizeOf<AtkValue>())).Int & 0x100) != 0)
		{
			return GenerateSystemMenuHook.Original(agent, atkValueArgs, valueCount, eventKind);
		}
		List<int> list = new List<int>();
		for (int i = 5; i < valueCount; i++)
		{
			AtkValue val = ((AtkValue*)atkValueArgs)[i];
			if ((int)val.Type == 0)
			{
				break;
			}
			if (!BlockedCommandID.Contains((uint)val.Int))
			{
				list.Add(val.Int);
			}
		}
		RentedAtkValues val2 = default(RentedAtkValues);
		((RentedAtkValues)(ref val2))._002Ector(valueCount);
		try
		{
			for (int j = 0; j < 5; j++)
			{
				((AtkValue)(ref ((RentedAtkValues)(ref val2))[j])).Copy((AtkValue*)((byte*)atkValueArgs + (nint)j * (nint)Unsafe.SizeOf<AtkValue>()));
			}
			for (int k = 0; k < list.Count; k++)
			{
				((AtkValue)(ref ((RentedAtkValues)(ref val2))[k + 5])).SetInt(list[k]);
			}
			((AtkValue)(ref ((RentedAtkValues)(ref val2))[4])).SetInt(list.Count);
			return GenerateSystemMenuHook.Original(agent, RentedAtkValues.op_Implicit(val2), valueCount, eventKind);
		}
		finally
		{
			((RentedAtkValues)(ref val2)).Dispose();
		}
	}

	private unsafe void OpenSystemMenuDetour(AgentHUD* agent, AtkValue* atkValueArgs, uint menuSize)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (menuSize > 20)
		{
			OpenSystemMenuHook.Original.Invoke(agent, atkValueArgs, menuSize);
			return;
		}
		RentedAtkValues val = default(RentedAtkValues);
		((RentedAtkValues)(ref val))._002Ector(45);
		try
		{
			for (int i = 0; i < 5; i++)
			{
				((AtkValue)(ref ((RentedAtkValues)(ref val))[i])).Copy((AtkValue*)((byte*)atkValueArgs + (nint)i * (nint)Unsafe.SizeOf<AtkValue>()));
			}
			int num = 0;
			for (int j = 0; j < menuSize; j++)
			{
				int num2 = 5 + j;
				if (!BlockedCommandID.Contains((uint)((AtkValue)((byte*)atkValueArgs + (nint)num2 * (nint)Unsafe.SizeOf<AtkValue>())).Int))
				{
					int num3 = 5 + num;
					((AtkValue)(ref ((RentedAtkValues)(ref val))[num3])).Copy((AtkValue*)((byte*)atkValueArgs + (nint)num2 * (nint)Unsafe.SizeOf<AtkValue>()));
					((AtkValue)(ref ((RentedAtkValues)(ref val))[num3 + 20])).Copy((AtkValue*)((byte*)atkValueArgs + (nint)(num2 + 20) * (nint)Unsafe.SizeOf<AtkValue>()));
					num++;
				}
			}
			((AtkValue)(ref ((RentedAtkValues)(ref val))[3])).SetInt(num);
			OpenSystemMenuHook.Original.Invoke(agent, RentedAtkValues.op_Implicit(val), (uint)num);
		}
		finally
		{
			((RentedAtkValues)(ref val)).Dispose();
		}
	}
}
