using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Interop.Game.Models;

namespace OmenTools.Extensions;

public static class AtkComponentExtension
{
	extension(scoped ref AtkComponentNode node)
	{
		public unsafe void OutlineNode()
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				(*(AtkResNode*)ptr).OutlineNode();
			}
		}

		public unsafe Vector2 GetPosition()
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return Vector2.Zero;
				}
				return (*(AtkResNode*)ptr).GetPosition();
			}
		}

		public unsafe Vector2 GetScale()
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return Vector2.One;
				}
				return (*(AtkResNode*)ptr).GetScale();
			}
		}

		public unsafe Vector2 GetSize()
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return Vector2.One;
				}
				return (*(AtkResNode*)ptr).GetSize();
			}
		}

		public unsafe bool GetVisibility()
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				return (*(AtkResNode*)ptr).GetVisibility();
			}
		}

		public unsafe NodeState GetNodeState()
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return default(NodeState);
				}
				return (*(AtkResNode*)ptr).GetNodeState();
			}
		}

		public unsafe bool SetSize(scoped in Vector2 size)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				return (*(AtkResNode*)ptr).SetSize(in size);
			}
		}

		public unsafe bool SetSize(scoped in float value)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				return (*(AtkResNode*)ptr).SetSize(in value);
			}
		}

		public unsafe bool SetSize(scoped in float width, scoped in float height)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				return (*(AtkResNode*)ptr).SetSize(in width, in height);
			}
		}

		public unsafe bool SetWidth(scoped in float value)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				return (*(AtkResNode*)ptr).SetWidth(in value);
			}
		}

		public unsafe bool SetHeight(scoped in float value)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				return (*(AtkResNode*)ptr).SetHeight(in value);
			}
		}

		public unsafe void SetPosition(scoped in Vector2 position)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				(*(AtkResNode*)ptr).SetPosition(in position);
			}
		}

		public unsafe void SetPosition(scoped in float value)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				(*(AtkResNode*)ptr).SetPosition(in value);
			}
		}

		public unsafe void SetPosition(scoped in float x, scoped in float y)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				(*(AtkResNode*)ptr).SetPosition(in x, in y);
			}
		}

		public unsafe void SetPositionX(scoped in float value)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				(*(AtkResNode*)ptr).SetPositionX(in value);
			}
		}

		public unsafe void SetPositionY(scoped in float value)
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				(*(AtkResNode*)ptr).SetPositionY(in value);
			}
		}

		public unsafe AtkUnitBase* GetOwnerAddon()
		{
			fixed (AtkComponentNode* ptr = &node)
			{
				if (ptr == null)
				{
					return null;
				}
				return (*(AtkResNode*)ptr).GetOwnerAddon();
			}
		}
	}

	extension(scoped ref AtkComponentTextInput textInput)
	{
		public unsafe static bool TryGetActive(out AtkComponentTextInput* component, out AtkUnitBase* addon)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Invalid comparison between Unknown and I4
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Invalid comparison between Unknown and I4
			component = null;
			addon = null;
			RaptureAtkModule* ptr = RaptureAtkModule.Instance();
			if (ptr == null)
			{
				return false;
			}
			AtkTextInputEventInterface* targetTextInputEventInterface = ((AtkTextInput)(&((RaptureAtkModule)ptr).TextInput)).TargetTextInputEventInterface;
			if (targetTextInputEventInterface == null)
			{
				return false;
			}
			AtkResNode* ownerNode = ((AtkTextInputEventInterface)targetTextInputEventInterface).GetOwnerNode();
			if (ownerNode == null || (int)((AtkResNode)ownerNode).GetNodeType() != 10000)
			{
				return false;
			}
			AtkComponentNode* ptr2 = (AtkComponentNode*)ownerNode;
			AtkComponentBase* component2 = ((AtkComponentNode)ptr2).Component;
			if (component2 == null || (int)((AtkComponentBase)component2).GetComponentType() != 7)
			{
				return false;
			}
			component = (AtkComponentTextInput*)component2;
			addon = ((AtkComponentTextInput)component).OwnerAddon;
			if (addon == null)
			{
				addon = ((AtkComponentTextInput)component).ContainingAddon2;
			}
			if (addon == null)
			{
				addon = ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonByNode((AtkResNode*)((AtkComponentTextInput)component).OwnerNode);
			}
			return addon != null;
		}
	}

	extension(scoped ref AtkComponentRadioButton target)
	{
		public unsafe void Click(int which, AtkEventType type = (AtkEventType)25)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkComponentRadioButton* ptr = &target)
			{
				if (ptr == null)
				{
					return;
				}
				(*(*((AtkComponentRadioButton)ptr).OwnerNode).GetOwnerAddon()).ClickComponent(((AtkComponentRadioButton)ptr).OwnerNode, which, type);
			}
		}

		public unsafe void Click()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkComponentRadioButton* ptr = &target)
			{
				if (ptr == null)
				{
					return;
				}
				AtkUnitBase* ownerAddon = (*((AtkComponentRadioButton)ptr).OwnerNode).GetOwnerAddon();
				AtkEvent* ptr2 = ((AtkComponentNode)((AtkComponentRadioButton)ptr).OwnerNode).AtkResNode.AtkEventManager.Event;
				((AtkUnitBase)ownerAddon).ReceiveEvent(((AtkEventState)(&((AtkEvent)ptr2).State)).EventType, (int)((AtkEvent)ptr2).Param, ptr2, (AtkEventData*)null);
			}
		}
	}

	extension(scoped ref AtkComponentCheckBox target)
	{
		public unsafe void Click(int which, AtkEventType type = (AtkEventType)25)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkComponentCheckBox* ptr = &target)
			{
				if (ptr == null)
				{
					return;
				}
				(*(*((AtkComponentCheckBox)ptr).OwnerNode).GetOwnerAddon()).ClickComponent(((AtkComponentCheckBox)ptr).OwnerNode, which, type);
			}
		}
	}

	extension(scoped ref AtkComponentDragDrop target)
	{
		public unsafe void Click(int which, AtkEventType type = (AtkEventType)60)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkComponentDragDrop* ptr = &target)
			{
				if (ptr == null)
				{
					return;
				}
				(*(*((AtkComponentDragDrop)ptr).OwnerNode).GetOwnerAddon()).ClickComponent(((AtkComponentDragDrop)ptr).OwnerNode, which, type);
			}
		}
	}

	extension(scoped ref AtkComponentButton target)
	{
		public unsafe void Click(AtkEvent* eventData)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkComponentButton* ptr = &target)
			{
				if (ptr == null)
				{
					return;
				}
				((AtkUnitBase)(*((AtkComponentButton)ptr).OwnerNode).GetOwnerAddon()).ReceiveEvent(((AtkEventState)(&((AtkEvent)eventData).State)).EventType, (int)((AtkEvent)eventData).Param, eventData, (AtkEventData*)null);
			}
		}

		public unsafe void Click()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkComponentButton* ptr = &target)
			{
				if (ptr == null)
				{
					return;
				}
				AtkComponentNode* ownerNode = ((AtkComponentButton)ptr).OwnerNode;
				AtkEvent* ptr2 = ((AtkEventManager)(&((AtkResNode)(&((AtkComponentNode)ownerNode).AtkResNode)).AtkEventManager)).Event;
				((AtkUnitBase)(*ownerNode).GetOwnerAddon()).ReceiveEvent(((AtkEventState)(&((AtkEvent)ptr2).State)).EventType, (int)((AtkEvent)ptr2).Param, ptr2, (AtkEventData*)null);
			}
		}
	}

	extension(scoped ref AtkComponentBase component)
	{
		public unsafe void Click(AtkComponentNode* target, int which, AtkEventType type, ManagedAtkEvent? eventData = null, ManagedAtkInputData? inputData = null)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkComponentBase* ptr = &component)
			{
				if (ptr == null)
				{
					return;
				}
				if (eventData == null)
				{
					eventData = ManagedAtkEvent.ForNormalTarget(target, ptr);
				}
				if (inputData == null)
				{
					inputData = ManagedAtkInputData.Empty();
				}
				((AtkEventListener)(&((AtkComponentBase)ptr).AtkEventListener)).ReceiveEvent(type, which, eventData.AtkEvent, inputData.AtkEventData);
			}
		}
	}
}
