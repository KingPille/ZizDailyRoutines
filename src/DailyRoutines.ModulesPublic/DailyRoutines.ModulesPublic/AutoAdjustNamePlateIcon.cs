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
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class AutoAdjustNamePlateIcon : ModuleBase
{
	private unsafe delegate void AtkUldManagerUpdateFromParentNodeDelegate(AtkUldManager* manager, AtkResNode* node, AtkResNode* parentNode, byte onlyDirty);

	private class Config : ModuleConfig
	{
		public Vector2 Offset;

		public float Scale = 1f;
	}

	private static readonly CompSig AtkUldManagerUpdateFromParentNodeSig = new CompSig("48 89 5C 24 ?? 55 56 57 48 83 EC ?? 41 0F B6 F1");

	private AtkUldManagerUpdateFromParentNodeDelegate AtkUldManagerUpdateFromParentNode;

	private readonly Vector2[] basePositions = new Vector2[AddonNamePlate.NumNamePlateObjects];

	private readonly Vector2[] appliedPositions = new Vector2[AddonNamePlate.NumNamePlateObjects];

	private readonly bool[] applied = new bool[AddonNamePlate.NumNamePlateObjects];

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoAdjustNamePlateIconTitle"),
		Description = LanguageManager.Get("AutoAdjustNamePlateIconDescription"),
		Category = ModuleCategory.Combat,
		Author = new string[1] { "Marsh" }
	};

	protected override void Init()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		AtkUldManagerUpdateFromParentNode = AtkUldManagerUpdateFromParentNodeSig.GetDelegate<AtkUldManagerUpdateFromParentNodeDelegate>();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "NamePlate", new AddonEventDelegate(OnAddonEvent));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)3, "NamePlate", new AddonEventDelegate(OnAddonEvent));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddonEvent });
		ResetIcons();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get("Scale")), ref config.Scale, 0f, 2f, ImU8String.op_Implicit("%.2f"), (ImGuiInputTextFlags)0))
		{
			config.Save(this);
		}
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get("IconOffset"));
		if (ImGui.InputFloat2(val, ref config.Offset, -100f, 100f, ImU8String.op_Implicit("%.1f"), (ImGuiInputTextFlags)0))
		{
			config.Save(this);
		}
	}

	private unsafe void OnAddonEvent(AddonEvent type, AddonArgs? args)
	{
		AtkUnitBase* namePlate = Addons.NamePlate;
		if (namePlate == null || !(*namePlate).IsAddonAndNodesReady())
		{
			return;
		}
		AddonNamePlate* ptr = (AddonNamePlate*)namePlate;
		if (((AddonNamePlate)ptr).NamePlateObjectArray == null || ((AddonNamePlate)ptr).RootNode == null)
		{
			return;
		}
		for (int i = 0; i < AddonNamePlate.NumNamePlateObjects; i++)
		{
			AtkImageNode* markerIcon = ((NamePlateObject)((byte*)((AddonNamePlate)ptr).NamePlateObjectArray + (nint)i * (nint)Unsafe.SizeOf<NamePlateObject>())).MarkerIcon;
			if (markerIcon != null && ((AtkImageNode)markerIcon).IsVisible())
			{
				ApplyIcon(markerIcon, i, config.Scale, config.Offset);
			}
		}
		RefreshNodeTransforms(ptr);
	}

	private unsafe void ApplyIcon(AtkImageNode* icon, int index, float scale, Vector2 offset)
	{
		Vector2 vector = new Vector2(((AtkImageNode)icon).X, ((AtkImageNode)icon).Y);
		if (vector != appliedPositions[index])
		{
			basePositions[index] = vector;
		}
		Vector2 vector2 = basePositions[index] + offset;
		((AtkImageNode)icon).SetOrigin((float)(int)((AtkImageNode)icon).Width / 2f, (float)(int)((AtkImageNode)icon).Height / 2f);
		((AtkImageNode)icon).SetScale(scale, scale);
		((AtkImageNode)icon).SetPositionFloat(vector2.X, vector2.Y);
		appliedPositions[index] = vector2;
		applied[index] = true;
	}

	private unsafe void RefreshNodeTransforms(AddonNamePlate* addon)
	{
		((AtkResNode)((AddonNamePlate)addon).RootNode).DrawFlags |= 1u;
		AtkUldManagerUpdateFromParentNode(&((AddonNamePlate)addon).UldManager, ((AddonNamePlate)addon).RootNode, ((AtkResNode)((AddonNamePlate)addon).RootNode).ParentNode, 1);
	}

	private unsafe void ResetIcons()
	{
		AtkUnitBase* namePlate = Addons.NamePlate;
		if (namePlate == null)
		{
			return;
		}
		AddonNamePlate* ptr = (AddonNamePlate*)namePlate;
		if (((AddonNamePlate)ptr).NamePlateObjectArray == null || ((AddonNamePlate)ptr).RootNode == null)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < AddonNamePlate.NumNamePlateObjects; i++)
		{
			if (applied[i])
			{
				AtkImageNode* markerIcon = ((NamePlateObject)((byte*)((AddonNamePlate)ptr).NamePlateObjectArray + (nint)i * (nint)Unsafe.SizeOf<NamePlateObject>())).MarkerIcon;
				if (markerIcon != null)
				{
					((AtkImageNode)markerIcon).SetOrigin(0f, 0f);
					((AtkImageNode)markerIcon).SetScale(1f, 1f);
					((AtkImageNode)markerIcon).SetPositionFloat(basePositions[i].X, basePositions[i].Y);
					applied[i] = false;
					flag = true;
				}
			}
		}
		if (flag)
		{
			RefreshNodeTransforms(ptr);
		}
	}
}
