using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent.Abstractions;

namespace OmenTools.Interop.Game.AddonEvent;

public class AddonBankEvent : AddonEventBase
{
	public unsafe static int RetainerGilAmount
	{
		get
		{
			if (!(*Addons.Bank).IsAddonAndNodesReady())
			{
				return -1;
			}
			return ((AtkValue)((byte*)((AtkUnitBase)Addons.Bank).AtkValues + (nint)6 * (nint)Unsafe.SizeOf<AtkValue>())).Int;
		}
	}

	public unsafe static void SwitchMode()
	{
		if ((*Addons.Bank).IsAddonAndNodesReady())
		{
			(*Addons.Bank).Callback(2, 0);
		}
	}

	public unsafe static void SetNumber(uint amount)
	{
		if ((*Addons.Bank).IsAddonAndNodesReady())
		{
			(*Addons.Bank).Callback(3, amount);
		}
	}

	public unsafe static void ClickConfirm()
	{
		if ((*Addons.Bank).IsAddonAndNodesReady())
		{
			(*Addons.Bank).Callback(0, 0);
		}
	}

	public unsafe static void ClickCancel()
	{
		if ((*Addons.Bank).IsAddonAndNodesReady())
		{
			(*Addons.Bank).Callback(1, 0);
		}
	}
}
