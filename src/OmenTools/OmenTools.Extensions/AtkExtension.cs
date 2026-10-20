using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;

namespace OmenTools.Extensions;

public static class AtkExtension
{
	extension(scoped ref AtkImageNode node)
	{
		public unsafe uint GetIconID()
		{
			fixed (AtkImageNode* ptr = &node)
			{
				if (ptr == null)
				{
					return 0u;
				}
				AtkUldPartsList* partsList = ((AtkImageNode)ptr).PartsList;
				if (partsList == null)
				{
					return 0u;
				}
				AtkUldPart* parts = ((AtkUldPartsList)partsList).Parts;
				if (parts == null)
				{
					return 0u;
				}
				AtkUldAsset* uldAsset = ((AtkUldPart)((byte*)parts + (nint)(int)((AtkImageNode)ptr).PartId * (nint)Unsafe.SizeOf<AtkUldPart>())).UldAsset;
				if (uldAsset == null)
				{
					return 0u;
				}
				AtkTextureResource* resource = ((AtkTexture)(&((AtkUldAsset)uldAsset).AtkTexture)).Resource;
				if (resource == null)
				{
					return 0u;
				}
				return ((AtkTextureResource)resource).IconId;
			}
		}
	}

	extension(scoped ref AtkTextNode node)
	{
		public unsafe Vector2 GetTextDrawSize(bool considerScale = false)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkTextNode* ptr = &node)
			{
				RentedSeStringBuilder val = default(RentedSeStringBuilder);
				((RentedSeStringBuilder)(ref val))._002Ector();
				try
				{
					ushort num = 0;
					ushort num2 = 0;
					((AtkTextNode)ptr).GetTextDrawSize(&num, &num2, CStringPointer.op_Implicit(((Utf8String)(&((AtkTextNode)ptr).NodeText)).StringPtr), 0, -1, considerScale);
					return new Vector2((int)num, (int)num2);
				}
				finally
				{
					((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
	}

	extension(scoped ref AtkCollisionNode target)
	{
		public unsafe void Click(AtkEvent* eventData)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkCollisionNode* ptr = &target)
			{
				if (ptr == null)
				{
					return;
				}
				((AtkUnitBase)((AtkCollisionNode)ptr).AtkResNode.GetOwnerAddon()).ReceiveEvent(((AtkEventState)(&((AtkEvent)eventData).State)).EventType, (int)((AtkEvent)eventData).Param, eventData, (AtkEventData*)null);
			}
		}

		public unsafe void Click()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Invalid comparison between Unknown and I4
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkCollisionNode* ptr = &target)
			{
				if (ptr == null)
				{
					return;
				}
				AtkResNode node = ((AtkCollisionNode)ptr).AtkResNode;
				AtkEvent* ptr2 = node.AtkEventManager.Event;
				while ((int)((AtkEventState)(&((AtkEvent)ptr2).State)).EventType != 9)
				{
					ptr2 = ((AtkEvent)ptr2).NextEvent;
				}
				((AtkUnitBase)node.GetOwnerAddon()).ReceiveEvent(((AtkEventState)(&((AtkEvent)ptr2).State)).EventType, (int)((AtkEvent)ptr2).Param, ptr2, (AtkEventData*)null);
			}
		}
	}
}
