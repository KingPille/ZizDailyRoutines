using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace OmenTools.Extensions;

public static class AtkEventDataExtension
{
	extension(scoped in AtkEventData data)
	{
		public bool IsLeftClick => data.MouseData.ButtonId == 0;

		public bool IsRightClick => data.MouseData.ButtonId == 1;

		public bool IsNoModifier => (int)data.MouseData.Modifier == 0;

		public bool IsAltHeld => ((Enum)data.MouseData.Modifier).HasFlag((Enum)(object)(ModifierFlag)2);

		public bool IsControlHeld => ((Enum)data.MouseData.Modifier).HasFlag((Enum)(object)(ModifierFlag)1);

		public bool IsShiftHeld => ((Enum)data.MouseData.Modifier).HasFlag((Enum)(object)(ModifierFlag)4);

		public bool IsDragging => ((Enum)data.MouseData.Modifier).HasFlag((Enum)(object)(ModifierFlag)16);

		public bool IsScrollUp => data.MouseData.WheelDirection == 1;

		public bool IsScrollDown => data.MouseData.WheelDirection == -1;

		public Vector2 MousePosition => new Vector2(data.MouseData.PosX, data.MouseData.PosY);
	}
}
