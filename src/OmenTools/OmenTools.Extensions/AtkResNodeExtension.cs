using System.Drawing;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Info.Models;
using OmenTools.Interop.Game.Models;

namespace OmenTools.Extensions;

public static class AtkResNodeExtension
{
	extension(scoped ref AtkResNode node)
	{
		public unsafe void OutlineNode()
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				Vector2 position = (*ptr).GetPosition();
				Vector2 size = (*ptr).GetSize();
				bool visibility = (*ptr).GetVisibility();
				Vector2 vector = position;
				ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
				position = vector + ((ImGuiViewportPtr)(ref mainViewport)).Pos;
				ImDrawListPtr foregroundDrawList = ImGui.GetForegroundDrawList(ImGuiHelpers.MainViewport);
				((ImDrawListPtr)(ref foregroundDrawList)).AddRect(position, position + size, visibility ? KnownColor.Lime.ToUInt() : KnownColor.Chocolate.ToUInt());
			}
		}

		public unsafe Vector2 GetPosition()
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return Vector2.Zero;
				}
				Vector2 result = new Vector2(((AtkResNode)ptr).X, ((AtkResNode)ptr).Y);
				result -= new Vector2(((AtkResNode)ptr).OriginX * (((AtkResNode)ptr).ScaleX - 1f), ((AtkResNode)ptr).OriginY * (((AtkResNode)ptr).ScaleY - 1f));
				for (AtkResNode* parentNode = ((AtkResNode)ptr).ParentNode; parentNode != null; parentNode = ((AtkResNode)parentNode).ParentNode)
				{
					result *= new Vector2(((AtkResNode)parentNode).ScaleX, ((AtkResNode)parentNode).ScaleY);
					result += new Vector2(((AtkResNode)parentNode).X, ((AtkResNode)parentNode).Y);
					result -= new Vector2(((AtkResNode)parentNode).OriginX * (((AtkResNode)parentNode).ScaleX - 1f), ((AtkResNode)parentNode).OriginY * (((AtkResNode)parentNode).ScaleY - 1f));
				}
				return result;
			}
		}

		public unsafe Vector2 GetScale()
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return Vector2.One;
				}
				AtkResNode* ptr2 = ptr;
				Vector2 result = new Vector2(((AtkResNode)ptr2).ScaleX, ((AtkResNode)ptr2).ScaleY);
				while (((AtkResNode)ptr2).ParentNode != null)
				{
					ptr2 = ((AtkResNode)ptr2).ParentNode;
					result *= new Vector2(((AtkResNode)ptr2).ScaleX, ((AtkResNode)ptr2).ScaleY);
				}
				return result;
			}
		}

		public unsafe Vector2 GetSize()
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return Vector2.One;
				}
				return new Vector2((int)((AtkResNode)ptr).Width, (int)((AtkResNode)ptr).Height) * (*ptr).GetScale();
			}
		}

		public unsafe bool GetVisibility()
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				for (AtkResNode* ptr2 = ptr; ptr2 != null; ptr2 = ((AtkResNode)ptr2).ParentNode)
				{
					if (!((AtkResNode)ptr2).IsVisible())
					{
						return false;
					}
				}
			}
			return true;
		}

		public NodeState GetNodeState()
		{
			Vector2 position = node.GetPosition();
			Vector2 size = node.GetSize();
			return new NodeState
			{
				TopLeft = position,
				Visible = node.GetVisibility(),
				Size = size
			};
		}

		public unsafe bool SetSize(scoped in Vector2 size)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				bool flag = false;
				if (NumericRange.UShortRange.Contains((ushort)size.X))
				{
					((AtkResNode)ptr).Width = (ushort)size.X;
					flag = true;
				}
				if (NumericRange.UShortRange.Contains((ushort)size.Y))
				{
					((AtkResNode)ptr).Height = (ushort)size.Y;
					flag = true;
				}
				if (flag)
				{
					((AtkResNode)ptr).DrawFlags |= 1u;
				}
				return flag;
			}
		}

		public unsafe bool SetSize(scoped in float value)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				bool flag = false;
				if (NumericRange.UShortRange.Contains((ushort)value))
				{
					((AtkResNode)ptr).Width = (ushort)value;
					flag = true;
				}
				if (NumericRange.UShortRange.Contains((ushort)value))
				{
					((AtkResNode)ptr).Height = (ushort)value;
					flag = true;
				}
				if (flag)
				{
					((AtkResNode)ptr).DrawFlags |= 1u;
				}
				return flag;
			}
		}

		public unsafe bool SetSize(scoped in float width, scoped in float height)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				bool flag = false;
				if (NumericRange.UShortRange.Contains((ushort)width))
				{
					((AtkResNode)ptr).Width = (ushort)width;
					flag = true;
				}
				if (NumericRange.UShortRange.Contains((ushort)height))
				{
					((AtkResNode)ptr).Height = (ushort)height;
					flag = true;
				}
				if (flag)
				{
					((AtkResNode)ptr).DrawFlags |= 1u;
				}
				return flag;
			}
		}

		public unsafe bool SetWidth(scoped in float value)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				bool flag = false;
				if (NumericRange.UShortRange.Contains((ushort)value))
				{
					((AtkResNode)ptr).Width = (ushort)value;
					flag = true;
				}
				if (flag)
				{
					((AtkResNode)ptr).DrawFlags |= 1u;
				}
				return flag;
			}
		}

		public unsafe bool SetHeight(scoped in float value)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return false;
				}
				bool flag = false;
				if (NumericRange.UShortRange.Contains((ushort)value))
				{
					((AtkResNode)ptr).Height = (ushort)value;
					flag = true;
				}
				if (flag)
				{
					((AtkResNode)ptr).DrawFlags |= 1u;
				}
				return flag;
			}
		}

		public unsafe void SetPosition(scoped in Vector2 position)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				((AtkResNode)ptr).X = position.X;
				((AtkResNode)ptr).Y = position.Y;
				((AtkResNode)ptr).DrawFlags |= 1u;
			}
		}

		public unsafe void SetPosition(scoped in float value)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				((AtkResNode)ptr).X = value;
				((AtkResNode)ptr).Y = value;
				((AtkResNode)ptr).DrawFlags |= 1u;
			}
		}

		public unsafe void SetPosition(scoped in float x, scoped in float y)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				((AtkResNode)ptr).X = x;
				((AtkResNode)ptr).Y = y;
				((AtkResNode)ptr).DrawFlags |= 1u;
			}
		}

		public unsafe void SetPositionX(scoped in float value)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				((AtkResNode)ptr).X = value;
				((AtkResNode)ptr).DrawFlags |= 1u;
			}
		}

		public unsafe void SetPositionY(scoped in float value)
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return;
				}
				((AtkResNode)ptr).Y = value;
				((AtkResNode)ptr).DrawFlags |= 1u;
			}
		}

		public unsafe AtkUnitBase* GetOwnerAddon()
		{
			fixed (AtkResNode* ptr = &node)
			{
				if (ptr == null)
				{
					return null;
				}
				return ((RaptureAtkUnitManager)RaptureAtkUnitManager.Instance()).GetAddonByNode(ptr);
			}
		}
	}
}
