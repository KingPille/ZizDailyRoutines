using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace OmenTools.Extensions;

public static class AtkUldManagerExtension
{
	extension(scoped ref AtkUldManager manager)
	{
		public unsafe List<nint> SearchSimpleNodesByType(NodeType type)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			if ((int)type > 1000)
			{
				throw new ArgumentOutOfRangeException("type", "不支持非 SimpleNode 类型");
			}
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return new List<nint>();
				}
				List<nint> list = new List<nint>();
				for (int i = 0; i < ((AtkUldManager)ptr).NodeListCount; i++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)ptr).NodeList[i];
					if (ptr2 != null && ((AtkResNode)ptr2).Type == type)
					{
						list.Add((nint)ptr2);
					}
				}
				return list;
			}
		}

		public unsafe T* SearchSimpleNodeByType<T>(NodeType type) where T : unmanaged
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if ((int)type > 1000)
			{
				throw new ArgumentOutOfRangeException("type", "不支持非 SimpleNode 类型");
			}
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				for (int i = 0; i < ((AtkUldManager)ptr).NodeListCount; i++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)ptr).NodeList[i];
					if (ptr2 != null && ((AtkResNode)ptr2).Type == type)
					{
						return (T*)ptr2;
					}
				}
				return null;
			}
		}

		public unsafe AtkResNode* SearchSimpleNodeByType(NodeType type)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			if ((int)type > 1000)
			{
				throw new ArgumentOutOfRangeException("type", "不支持非 SimpleNode 类型");
			}
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				for (int i = 0; i < ((AtkUldManager)ptr).NodeListCount; i++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)ptr).NodeList[i];
					if (ptr2 != null && ((AtkResNode)ptr2).Type == type)
					{
						return ptr2;
					}
				}
				return null;
			}
		}

		public unsafe List<nint> SearchComponentsByType(ComponentType type)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Invalid comparison between Unknown and I4
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				List<nint> list = new List<nint>();
				for (int i = 0; i < ((AtkUldManager)ptr).NodeListCount; i++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)ptr).NodeList[i];
					if (ptr2 != null && (int)((AtkResNode)ptr2).Type >= 1000)
					{
						AtkComponentNode* ptr3 = (AtkComponentNode*)ptr2;
						AtkUldComponentInfo* objects = (AtkUldComponentInfo*)((AtkComponentBase)((AtkComponentNode)ptr3).Component).UldManager.Objects;
						if (objects != null && ((AtkUldComponentInfo)objects).ComponentType == type)
						{
							list.Add((nint)((AtkComponentNode)ptr3).Component);
						}
					}
				}
				return list;
			}
		}

		public unsafe T* SearchComponentByType<T>(ComponentType type) where T : unmanaged
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				for (int i = 0; i < ((AtkUldManager)ptr).NodeListCount; i++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)ptr).NodeList[i];
					if (ptr2 != null && (int)((AtkResNode)ptr2).Type >= 1000)
					{
						AtkComponentNode* ptr3 = (AtkComponentNode*)ptr2;
						AtkUldComponentInfo* objects = (AtkUldComponentInfo*)((AtkComponentBase)((AtkComponentNode)ptr3).Component).UldManager.Objects;
						if (objects != null && ((AtkUldComponentInfo)objects).ComponentType == type)
						{
							return (T*)((AtkComponentNode)ptr3).Component;
						}
					}
				}
				return null;
			}
		}

		public unsafe AtkComponentBase* SearchComponentByType(ComponentType type)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				for (int i = 0; i < ((AtkUldManager)ptr).NodeListCount; i++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)ptr).NodeList[i];
					if (ptr2 != null && (int)((AtkResNode)ptr2).Type >= 1000)
					{
						AtkComponentNode* ptr3 = (AtkComponentNode*)ptr2;
						AtkUldComponentInfo* objects = (AtkUldComponentInfo*)((AtkComponentBase)((AtkComponentNode)ptr3).Component).UldManager.Objects;
						if (objects != null && ((AtkUldComponentInfo)objects).ComponentType == type)
						{
							return ((AtkComponentNode)ptr3).Component;
						}
					}
				}
				return null;
			}
		}

		public unsafe List<nint> SearchComponentNodesByType(ComponentType type)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Invalid comparison between Unknown and I4
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				List<nint> list = new List<nint>();
				for (int i = 0; i < ((AtkUldManager)ptr).NodeListCount; i++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)ptr).NodeList[i];
					if (ptr2 != null && (int)((AtkResNode)ptr2).Type >= 1000)
					{
						AtkComponentNode* ptr3 = (AtkComponentNode*)ptr2;
						AtkUldComponentInfo* objects = (AtkUldComponentInfo*)((AtkComponentBase)((AtkComponentNode)ptr3).Component).UldManager.Objects;
						if (objects != null && ((AtkUldComponentInfo)objects).ComponentType == type)
						{
							list.Add((nint)ptr3);
						}
					}
				}
				return list;
			}
		}

		public unsafe T* SearchComponentNodeByType<T>(ComponentType type) where T : unmanaged
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				for (int i = 0; i < ((AtkUldManager)ptr).NodeListCount; i++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)ptr).NodeList[i];
					if (ptr2 != null && (int)((AtkResNode)ptr2).Type >= 1000)
					{
						AtkComponentNode* ptr3 = (AtkComponentNode*)ptr2;
						AtkUldComponentInfo* objects = (AtkUldComponentInfo*)((AtkComponentBase)((AtkComponentNode)ptr3).Component).UldManager.Objects;
						if (objects != null && ((AtkUldComponentInfo)objects).ComponentType == type)
						{
							return (T*)ptr3;
						}
					}
				}
				return null;
			}
		}

		public unsafe AtkComponentNode* SearchComponentNodeByType(ComponentType type)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Invalid comparison between Unknown and I4
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				for (int i = 0; i < ((AtkUldManager)ptr).NodeListCount; i++)
				{
					AtkResNode* ptr2 = ((AtkUldManager)ptr).NodeList[i];
					if (ptr2 != null && (int)((AtkResNode)ptr2).Type >= 1000)
					{
						AtkComponentNode* ptr3 = (AtkComponentNode*)ptr2;
						AtkUldComponentInfo* objects = (AtkUldComponentInfo*)((AtkComponentBase)((AtkComponentNode)ptr3).Component).UldManager.Objects;
						if (objects != null && ((AtkUldComponentInfo)objects).ComponentType == type)
						{
							return ptr3;
						}
					}
				}
				return null;
			}
		}

		public unsafe bool IsUldManagerReady()
		{
			fixed (AtkUldManager* ptr = &manager)
			{
				if (ptr != null && manager.RootNode != null && ((AtkResNode)manager.RootNode).ChildNode != null)
				{
					return manager.NodeList != null;
				}
				return false;
			}
		}
	}
}
