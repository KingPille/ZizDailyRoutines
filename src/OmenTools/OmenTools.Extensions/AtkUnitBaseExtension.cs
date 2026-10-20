using System.Numerics;
using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Info.Models;
using OmenTools.Interop.Game.Models;

namespace OmenTools.Extensions;

public static class AtkUnitBaseExtension
{
	extension(scoped ref AtkUnitBase addon)
	{
		public unsafe T* As<T>() where T : unmanaged
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return null;
				}
				return (T*)ptr;
			}
		}

		public unsafe void CallbackNoUpdate(params object[] args)
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return;
				}
				using AtkValueArray atkValueArray = new AtkValueArray(args);
				((AtkUnitBase)ptr).FireCallback((uint)atkValueArray.Length, atkValueArray.Pointer, false);
			}
		}

		public unsafe void CallbackNoUpdate(scoped in AtkValueArray atkValues)
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return;
				}
				((AtkUnitBase)ptr).FireCallback((uint)atkValues.Length, atkValues.Pointer, false);
			}
		}

		public unsafe void Callback(params object[] args)
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return;
				}
				using AtkValueArray atkValueArray = new AtkValueArray(args);
				((AtkUnitBase)ptr).FireCallback((uint)atkValueArray.Length, atkValueArray.Pointer, true);
			}
		}

		public unsafe void Callback(scoped in AtkValueArray atkValues)
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return;
				}
				((AtkUnitBase)ptr).FireCallback((uint)atkValues.Length, atkValues.Pointer, true);
			}
		}

		public unsafe bool IsAddonAndNodesReady()
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr != null && ((AtkUnitBase)ptr).IsVisible)
				{
					return ((AtkUnitBase)ptr).IsFullyLoaded();
				}
				return false;
			}
		}

		public unsafe bool SetPosition(scoped in Vector2 position)
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return false;
				}
				bool result = false;
				if (NumericRange.ShortRange.Contains((short)position.X))
				{
					((AtkUnitBase)ptr).X = (short)position.X;
					result = true;
				}
				if (NumericRange.ShortRange.Contains((short)position.Y))
				{
					((AtkUnitBase)ptr).Y = (short)position.Y;
					result = true;
				}
				return result;
			}
		}

		public unsafe bool SetPosition(scoped in float value)
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return false;
				}
				if (NumericRange.ShortRange.Contains((short)value))
				{
					((AtkUnitBase)ptr).X = (short)value;
					((AtkUnitBase)ptr).Y = (short)value;
					return true;
				}
				return false;
			}
		}

		public unsafe bool SetPosition(scoped in float x, scoped in float y)
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return false;
				}
				bool result = false;
				if (NumericRange.ShortRange.Contains((short)x))
				{
					((AtkUnitBase)ptr).X = (short)x;
					result = true;
				}
				if (NumericRange.ShortRange.Contains((short)y))
				{
					((AtkUnitBase)ptr).Y = (short)y;
					result = true;
				}
				return result;
			}
		}

		public unsafe bool SetPositionX(scoped in float value)
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return false;
				}
				if (NumericRange.ShortRange.Contains((short)value))
				{
					((AtkUnitBase)ptr).X = (short)value;
					return true;
				}
				return false;
			}
		}

		public unsafe bool SetPositionY(scoped in float value)
		{
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return false;
				}
				if (NumericRange.ShortRange.Contains((short)value))
				{
					((AtkUnitBase)ptr).Y = (short)value;
					return true;
				}
				return false;
			}
		}

		public unsafe string GetWindowTitle(uint? windowNodeID = null, (uint MainTitleNodeID, uint SubTitleNodeID)? nodeIDs = null)
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			(uint, uint) valueOrDefault = nodeIDs.GetValueOrDefault();
			if (!nodeIDs.HasValue)
			{
				valueOrDefault = (3u, 4u);
				nodeIDs = valueOrDefault;
			}
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (!(*ptr).IsAddonAndNodesReady())
				{
					return string.Empty;
				}
				AtkComponentNode* ptr2 = (windowNodeID.HasValue ? ((AtkComponentNode*)((AtkUnitBase)ptr).GetNodeById(windowNodeID.Value)) : ((AtkUnitBase)ptr).WindowNode);
				if (ptr2 == null)
				{
					return string.Empty;
				}
				string result = string.Empty;
				AtkTextNode* ptr3 = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr2).Component).UldManager)).SearchNodeById(nodeIDs.Value.MainTitleNodeID);
				if (ptr3 != null)
				{
					result = ((object)((AtkTextNode)ptr3).GetText()/*cast due to constrained. prefix*/).ToString();
				}
				string text = string.Empty;
				AtkTextNode* ptr4 = (AtkTextNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr2).Component).UldManager)).SearchNodeById(nodeIDs.Value.SubTitleNodeID);
				if (ptr4 != null)
				{
					text = ((object)((AtkTextNode)ptr4).GetText()/*cast due to constrained. prefix*/).ToString();
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					return result;
				}
				return text;
			}
		}

		public unsafe void ClickComponent(AtkComponentNode* target, int which, AtkEventType type, ManagedAtkEvent? eventData = null, ManagedAtkInputData? inputData = null)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			if (target == null)
			{
				return;
			}
			fixed (AtkUnitBase* ptr = &addon)
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
				((AtkEventListener)(&((AtkUnitBase)ptr).AtkEventListener)).ReceiveEvent(type, which, eventData.AtkEvent, inputData.AtkEventData);
			}
		}

		public unsafe void ClickAtkStage(int which, AtkEventType type = (AtkEventType)9)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkUnitBase* ptr = &addon)
			{
				if (ptr == null)
				{
					return;
				}
				ManagedAtkEvent managedAtkEvent = ManagedAtkEvent.ForNormalTarget(AtkStage.Instance(), ptr);
				ManagedAtkInputData managedAtkInputData = ManagedAtkInputData.Empty();
				((AtkEventListener)(&((AtkUnitBase)ptr).AtkEventListener)).ReceiveEvent(type, which, managedAtkEvent.AtkEvent, managedAtkInputData.AtkEventData);
			}
		}
	}

	extension(scoped in AtkUnitBasePtr addon)
	{
		public unsafe AtkUnitBase* ToStruct()
		{
			return (AtkUnitBase*)addon.Address;
		}

		public unsafe T* ToStruct<T>() where T : unmanaged
		{
			return (T*)addon.Address;
		}
	}

	extension(nint ptr)
	{
		public unsafe AtkUnitBase* ToAtkUnitBase()
		{
			return (AtkUnitBase*)ptr;
		}
	}
}
