using System.Runtime.CompilerServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;

namespace DailyRoutines.ModulesPublic.Interface;

public class LargerColorantColoringPreviewComponent : ModuleBase
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static AddonEventDelegate _003C0_003E__OnAddon;
	}

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("LargerColorantColoringPreviewComponentTitle"),
		Description = LanguageManager.Get("LargerColorantColoringPreviewComponentDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		obj.RegisterListener((AddonEvent)1, "ColorantColoring", (AddonEventDelegate)obj2);
		if ((*Addons.ColorantColoring).IsAddonAndNodesReady())
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		IAddonLifecycle obj = DalamudServiceExtension.Instance<IAddonLifecycle>();
		object obj2 = _003C_003EO._003C0_003E__OnAddon;
		if (obj2 == null)
		{
			AddonEventDelegate val = OnAddon;
			_003C_003EO._003C0_003E__OnAddon = val;
			obj2 = (object)val;
		}
		obj.UnregisterListener((AddonEvent)1, "ColorantColoring", (AddonEventDelegate)obj2);
	}

	private unsafe static void OnAddon(AddonEvent type, AddonArgs args)
	{
		AtkUnitBase* colorantColoring = Addons.ColorantColoring;
		if (colorantColoring == null)
		{
			return;
		}
		((AtkComponentNode)((AtkUnitBase)colorantColoring).WindowNode).SetWidth((ushort)754);
		for (int i = 0; i < ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)colorantColoring).WindowNode).Component).UldManager)).NodeListCount; i++)
		{
			AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)((AtkUnitBase)colorantColoring).WindowNode).Component).UldManager)).NodeList[i];
			if (ptr != null)
			{
				if (((AtkResNode)ptr).Width == 654)
				{
					((AtkResNode)ptr).SetWidth((ushort)754);
				}
				if (((AtkResNode)ptr).Width == 640)
				{
					((AtkResNode)ptr).SetWidth((ushort)740);
				}
				if (((AtkResNode)ptr).Width == 649)
				{
					((AtkResNode)ptr).SetWidth((ushort)749);
				}
				if (((AtkResNode)ptr).Width == 646)
				{
					((AtkResNode)ptr).SetWidth((ushort)746);
				}
				if (((AtkResNode)ptr).X == 621f)
				{
					((AtkResNode)ptr).SetXFloat(721f);
				}
			}
		}
		AtkResNode* nodeById = ((AtkUnitBase)colorantColoring).GetNodeById(70u);
		if (nodeById != null)
		{
			((AtkResNode)nodeById).SetYFloat(56f);
		}
		AtkComponentNode* componentNodeById = ((AtkUnitBase)colorantColoring).GetComponentNodeById(71u);
		if (componentNodeById != null)
		{
			((AtkComponentNode)componentNodeById).SetWidth((ushort)330);
			((AtkComponentNode)componentNodeById).SetHeight((ushort)550);
			((AtkComponentNode)componentNodeById).SetXFloat(-6f);
			((AtkComponentNode)componentNodeById).SetYFloat(1f);
			AtkResNode* ptr2 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(3u);
			if (ptr2 != null)
			{
				((AtkResNode)ptr2).SetWidth((ushort)330);
				((AtkResNode)ptr2).SetHeight((ushort)550);
			}
			AtkResNode* ptr3 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(5u);
			if (ptr3 != null)
			{
				((AtkResNode)ptr3).SetWidth((ushort)330);
				((AtkResNode)ptr3).SetHeight((ushort)550);
			}
			AtkResNode* ptr4 = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNodeById).Component).UldManager)).SearchNodeById(4u);
			if (ptr4 != null)
			{
				((AtkResNode)ptr4).SetWidth((ushort)322);
				((AtkResNode)ptr4).SetHeight((ushort)542);
			}
		}
		AtkResNode* nodeById2 = ((AtkUnitBase)colorantColoring).GetNodeById(17u);
		if (nodeById2 != null)
		{
			((AtkResNode)nodeById2).SetWidth((ushort)386);
		}
		for (uint num = 72u; num < 81; num++)
		{
			AtkComponentNode* componentNodeById2 = ((AtkUnitBase)colorantColoring).GetComponentNodeById(num);
			if (componentNodeById2 != null)
			{
				((AtkComponentNode)componentNodeById2).SetXFloat((float)(24 + 28 * (num - 72)));
				((AtkComponentNode)componentNodeById2).SetYFloat(556f);
			}
		}
	}
}
