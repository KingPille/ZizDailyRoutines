using System;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.Enums;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;

namespace OmenTools.Extensions;

public static class AtkStageExtension
{
	extension(scoped ref AtkStage atkStage)
	{
		public unsafe void ClearNodeFocus(AtkResNode* targetNode)
		{
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			if (targetNode == null)
			{
				return;
			}
			Span<FocusEntry> focusList = ((AtkInputManager)atkStage.AtkInputManager).FocusList;
			for (int i = 0; i < focusList.Length; i++)
			{
				ref FocusEntry reference = ref focusList[i];
				if (reference.AtkEventListener == null || reference.AtkEventTarget != targetNode)
				{
					continue;
				}
				reference.AtkEventTarget = null;
				reference.FocusParam = 0;
				((AtkInputManager)atkStage.AtkInputManager).FocusedNode = null;
				((AtkCollisionManager)atkStage.AtkCollisionManager).IntersectingCollisionNode = null;
				AtkUnitBase* atkEventListener = (AtkUnitBase*)reference.AtkEventListener;
				Span<Pointer<AtkResNode>> additionalFocusableNodes = ((AtkUnitBase)atkEventListener).AdditionalFocusableNodes;
				for (int j = 0; j < additionalFocusableNodes.Length; j++)
				{
					ref Pointer<AtkResNode> reference2 = ref additionalFocusableNodes[j];
					if (reference2.Value == targetNode)
					{
						reference2 = Pointer<AtkResNode>.op_Implicit((AtkResNode*)null);
					}
				}
			}
		}

		public unsafe void ShowActionTooltip(AtkResNode* node, uint actionID, string? textLabel = null)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			Utf8String val = default(Utf8String);
			((Utf8String)(ref val))._002Ector();
			try
			{
				AtkTooltipType val2 = (AtkTooltipType)4;
				AtkTooltipArgs* ptr = (AtkTooltipArgs*)stackalloc AtkTooltipArgs[1];
				((AtkTooltipArgs)ptr).Ctor();
				Unsafe.Write(&((AtkTooltipActionArgs)(&((AtkTooltipArgs)ptr).ActionArgs)).Kind, (DetailKind)30);
				((AtkTooltipActionArgs)(&((AtkTooltipArgs)ptr).ActionArgs)).Id = (int)actionID;
				((AtkTooltipActionArgs)(&((AtkTooltipArgs)ptr).ActionArgs)).Flags = 1;
				if (textLabel != null)
				{
					val2 = (AtkTooltipType)(val2 | 1);
					((Utf8String)(ref val)).SetString(textLabel);
					Unsafe.Write(&((AtkTooltipTextArgs)(&((AtkTooltipArgs)ptr).TextArgs)).Text, val.StringPtr);
				}
				AtkUnitBase* addonByNode = ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonByNode(node);
				if (addonByNode != null)
				{
					((AtkTooltipManager)(ref atkStage.TooltipManager)).ShowTooltip(val2, ((AtkUnitBase)addonByNode).Id, node, ptr, (delegate* unmanaged[Stdcall]<float*, float*, AtkResNode*, void>)null, false, true);
				}
			}
			finally
			{
				((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}

		public unsafe void ShowItemTooltip(AtkResNode* node, uint itemID)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			AtkTooltipArgs* ptr = (AtkTooltipArgs*)stackalloc AtkTooltipArgs[1];
			((AtkTooltipArgs)ptr).Ctor();
			Unsafe.Write(&((AtkTooltipItemArgs)(&((AtkTooltipArgs)ptr).ItemArgs)).Kind, (DetailKind)11);
			((AtkTooltipItemArgs)(&((AtkTooltipArgs)ptr).ItemArgs)).ItemId = (int)itemID;
			AtkUnitBase* addonByNode = ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonByNode(node);
			if (addonByNode != null)
			{
				((AtkTooltipManager)(ref atkStage.TooltipManager)).ShowTooltip((AtkTooltipType)2, ((AtkUnitBase)addonByNode).Id, node, ptr, (delegate* unmanaged[Stdcall]<float*, float*, AtkResNode*, void>)null, false, true);
			}
		}

		public unsafe void ShowInventoryItemTooltip(AtkResNode* node, InventoryType container, short slot)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			AtkTooltipArgs* ptr = (AtkTooltipArgs*)stackalloc AtkTooltipArgs[1];
			((AtkTooltipArgs)ptr).Ctor();
			Unsafe.Write(&((AtkTooltipItemArgs)(&((AtkTooltipArgs)ptr).ItemArgs)).Kind, (DetailKind)2);
			Unsafe.Write(&((AtkTooltipItemArgs)(&((AtkTooltipArgs)ptr).ItemArgs)).InventoryType, container);
			((AtkTooltipItemArgs)(&((AtkTooltipArgs)ptr).ItemArgs)).Slot = slot;
			((AtkTooltipItemArgs)(&((AtkTooltipArgs)ptr).ItemArgs)).BuyQuantity = -1;
			((AtkTooltipItemArgs)(&((AtkTooltipArgs)ptr).ItemArgs)).Flag1 = 0u;
			AtkUnitBase* addonByNode = ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonByNode(node);
			if (addonByNode != null)
			{
				((AtkTooltipManager)(ref atkStage.TooltipManager)).ShowTooltip((AtkTooltipType)2, ((AtkUnitBase)addonByNode).Id, node, ptr, (delegate* unmanaged[Stdcall]<float*, float*, AtkResNode*, void>)null, false, true);
			}
		}

		public void HideTooltip(ushort addonID)
		{
			((AtkTooltipManager)(ref atkStage.TooltipManager)).HideTooltip(addonID, false);
		}
	}
}
