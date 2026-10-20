using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Dalamud.DataShare.Attributes;

namespace OmenTools.Extensions;

public static class ColorExtension
{
	extension(ImGuiCol imguiCol)
	{
		public Vector4 ToVector4()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return ImGuiColToVector4.GetOrAdd(imguiCol, (ImGuiCol _) => ImGui.GetColorU32(imguiCol).ToVector4());
		}

		public uint ToUInt()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			return ImGuiColToUInt.GetOrAdd(imguiCol, (ImGuiCol _) => ImGui.GetColorU32(imguiCol));
		}
	}

	extension(KnownColor knownColor)
	{
		public Vector4 ToVector4()
		{
			switch (knownColor)
			{
			case KnownColor.Brown:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.Chocolate:
				return new Vector4(0.7137255f, 41f / 85f, 16f / 51f, 1f);
			case KnownColor.Crimson:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.DarkGoldenrod:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.DarkGreen:
				return new Vector4(0.70980394f, 0.5568628f, 33f / 85f, 1f);
			case KnownColor.DarkOrange:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.DarkRed:
				return new Vector4(31f / 85f, 0.18039216f, 14f / 85f, 1f);
			case KnownColor.DarkSlateBlue:
				return new Vector4(46f / 85f, 0.44313726f, 31f / 51f, 1f);
			case KnownColor.DarkSlateGray:
				return new Vector4(46f / 85f, 0.44313726f, 31f / 51f, 1f);
			case KnownColor.DeepPink:
				return new Vector4(46f / 85f, 0.44313726f, 31f / 51f, 1f);
			case KnownColor.DimGray:
				return new Vector4(31f / 51f, 16f / 51f, 0.23921569f, 1f);
			case KnownColor.Firebrick:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.ForestGreen:
				return new Vector4(0.70980394f, 0.5568628f, 33f / 85f, 1f);
			case KnownColor.Gold:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.Goldenrod:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.GreenYellow:
				return new Vector4(0.70980394f, 0.5568628f, 33f / 85f, 1f);
			case KnownColor.HotPink:
				return new Vector4(46f / 85f, 0.44313726f, 31f / 51f, 1f);
			case KnownColor.IndianRed:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.Khaki:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.LawnGreen:
				return new Vector4(67f / 85f, 49f / 85f, 19f / 51f, 1f);
			case KnownColor.LightCoral:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.LightGreen:
				return new Vector4(0.70980394f, 0.5568628f, 33f / 85f, 1f);
			case KnownColor.LightPink:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.LightSalmon:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.LightYellow:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.Lime:
				return new Vector4(0.70980394f, 0.5568628f, 33f / 85f, 1f);
			case KnownColor.LimeGreen:
				return new Vector4(0.70980394f, 0.5568628f, 33f / 85f, 1f);
			case KnownColor.Magenta:
				return new Vector4(41f / 85f, 21f / 85f, 0.21960784f, 1f);
			case KnownColor.Maroon:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.MediumOrchid:
				return new Vector4(32f / 51f, 46f / 85f, 0.6901961f, 1f);
			case KnownColor.MediumPurple:
				return new Vector4(39f / 85f, 33f / 85f, 0.5137255f, 1f);
			case KnownColor.MediumVioletRed:
				return new Vector4(46f / 85f, 0.44313726f, 31f / 51f, 1f);
			case KnownColor.NavajoWhite:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.Orange:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.OrangeRed:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.PaleGreen:
				return new Vector4(0.70980394f, 0.5568628f, 33f / 85f, 1f);
			case KnownColor.PaleVioletRed:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.Pink:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.Plum:
				return new Vector4(46f / 85f, 0.44313726f, 31f / 51f, 1f);
			case KnownColor.Red:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.SaddleBrown:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.Salmon:
				return new Vector4(0.6784314f, 1f / 3f, 14f / 51f, 1f);
			case KnownColor.SlateBlue:
				return new Vector4(46f / 85f, 0.44313726f, 31f / 51f, 1f);
			case KnownColor.SpringGreen:
				return new Vector4(0.70980394f, 0.5568628f, 33f / 85f, 1f);
			case KnownColor.Teal:
				return new Vector4(46f / 85f, 0.44313726f, 31f / 51f, 1f);
			case KnownColor.White:
				return new Vector4(72f / 85f, 40f / 51f, 63f / 85f, 1f);
			case KnownColor.Yellow:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			case KnownColor.YellowGreen:
				return new Vector4(0.75686276f, 0.54509807f, 0.34509805f, 1f);
			default:
			{
				KnownColor knownColor2 = knownColor;
				return KnownColorToVector4.GetOrAdd(knownColor2, (KnownColor _) => ColorHelpers.Vector(knownColor2));
			}
			}
		}

		public uint ToUInt()
		{
			switch (knownColor)
			{
			case KnownColor.Brown:
				return 4282799533u;
			case KnownColor.Chocolate:
				return 4283464630u;
			case KnownColor.Crimson:
				return 4282799533u;
			case KnownColor.DarkGoldenrod:
				return 4283993025u;
			case KnownColor.DarkGreen:
				return 4284714677u;
			case KnownColor.DarkOrange:
				return 4283993025u;
			case KnownColor.DarkRed:
				return 4280954461u;
			case KnownColor.DarkSlateBlue:
				return 4288377226u;
			case KnownColor.DarkSlateGray:
				return 4288377226u;
			case KnownColor.DeepPink:
				return 4288377226u;
			case KnownColor.DimGray:
				return 4282208411u;
			case KnownColor.Firebrick:
				return 4282799533u;
			case KnownColor.ForestGreen:
				return 4284714677u;
			case KnownColor.Gold:
				return 4283993025u;
			case KnownColor.Goldenrod:
				return 4283993025u;
			case KnownColor.GreenYellow:
				return 4284714677u;
			case KnownColor.HotPink:
				return 4288377226u;
			case KnownColor.IndianRed:
				return 4282799533u;
			case KnownColor.Khaki:
				return 4283993025u;
			case KnownColor.LawnGreen:
				return 4284453833u;
			case KnownColor.LightCoral:
				return 4282799533u;
			case KnownColor.LightGreen:
				return 4284714677u;
			case KnownColor.LightPink:
				return 4282799533u;
			case KnownColor.LightSalmon:
				return 4282799533u;
			case KnownColor.LightYellow:
				return 4283993025u;
			case KnownColor.Lime:
				return 4284714677u;
			case KnownColor.LimeGreen:
				return 4284714677u;
			case KnownColor.Magenta:
				return 4281876347u;
			case KnownColor.Maroon:
				return 4282799533u;
			case KnownColor.MediumOrchid:
				return 4289759904u;
			case KnownColor.MediumPurple:
				return 4286800757u;
			case KnownColor.MediumVioletRed:
				return 4288377226u;
			case KnownColor.NavajoWhite:
				return 4283993025u;
			case KnownColor.Orange:
				return 4283993025u;
			case KnownColor.OrangeRed:
				return 4282799533u;
			case KnownColor.PaleGreen:
				return 4284714677u;
			case KnownColor.PaleVioletRed:
				return 4282799533u;
			case KnownColor.Pink:
				return 4282799533u;
			case KnownColor.Plum:
				return 4288377226u;
			case KnownColor.Red:
				return 4282799533u;
			case KnownColor.SaddleBrown:
				return 4283993025u;
			case KnownColor.Salmon:
				return 4282799533u;
			case KnownColor.SlateBlue:
				return 4288377226u;
			case KnownColor.SpringGreen:
				return 4284714677u;
			case KnownColor.Teal:
				return 4288377226u;
			case KnownColor.White:
				return 4290627800u;
			case KnownColor.Yellow:
				return 4283993025u;
			case KnownColor.YellowGreen:
				return 4283993025u;
			default:
			{
				KnownColor knownColor2 = knownColor;
				return KnownColorToUInt.GetOrAdd(knownColor2, (KnownColor _) => knownColor2.ToVector4().ToUInt());
			}
			}
		}
	}

	extension(uint color)
	{
		public Vector4 ToVector4()
		{
			return UIntToVector4.GetOrAdd(color, (uint _) => ImGui.ColorConvertU32ToFloat4(color));
		}

		public Vector4 ReverseToVector4()
		{
			return ReverseUIntToVector4.GetOrAdd(color, delegate
			{
				Vector4 vector = ImGui.ColorConvertU32ToFloat4(color);
				return new Vector4(vector.Z, vector.Y, vector.X, 1f);
			});
		}

		public unsafe Vector4 GetVector4UIColor()
		{
			return ((AtkUIColorHolder)((AtkStage)AtkStage.Instance()).AtkUIColorHolder).GetColor(true, color).ToVector4();
		}

		public unsafe uint GetUIntUIColor()
		{
			return ((AtkUIColorHolder)((AtkStage)AtkStage.Instance()).AtkUIColorHolder).GetColor(true, color);
		}
	}

	extension(scoped in Vector4 color)
	{
		public Vector3 ToVector3()
		{
			return new Vector3(color.X, color.Y, color.Z);
		}

		public Vector4 Invert()
		{
			Vector4 result = color;
			result.X = 1f - color.X;
			result.Y = 1f - color.Y;
			result.Z = 1f - color.Z;
			return result;
		}

		public ByteColor ToByteColor()
		{
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			byte r = (byte)Math.Round(Math.Clamp(color.X, 0f, 1f) * 255f, MidpointRounding.AwayFromZero);
			byte g = (byte)Math.Round(Math.Clamp(color.Y, 0f, 1f) * 255f, MidpointRounding.AwayFromZero);
			byte b = (byte)Math.Round(Math.Clamp(color.Z, 0f, 1f) * 255f, MidpointRounding.AwayFromZero);
			byte a = (byte)Math.Round(Math.Clamp(color.W, 0f, 1f) * 255f, MidpointRounding.AwayFromZero);
			return new ByteColor
			{
				R = r,
				G = g,
				B = b,
				A = a
			};
		}

		public uint ToUInt()
		{
			return ImGui.ColorConvertFloat4ToU32(color);
		}
	}

	extension(ByteColor color)
	{
		public Vector4 ToVector4()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			float x = (float)(int)color.R / 255f;
			float y = (float)(int)color.G / 255f;
			float z = (float)(int)color.B / 255f;
			float w = (float)(int)color.A / 255f;
			return new Vector4(x, y, z, w);
		}
	}

	[DataShareTag]
	private const string IMGUI_COL_TO_VECTOR4_TAG = "OmenTools.Extensions.ColorExtension.ImGuiColToVector4";

	private static readonly Dictionary<ImGuiCol, Vector4> ImGuiColToVector4 = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<ImGuiCol, Vector4>>("OmenTools.Extensions.ColorExtension.ImGuiColToVector4", (Func<Dictionary<ImGuiCol, Vector4>>)(() => new Dictionary<ImGuiCol, Vector4>()));

	[DataShareTag]
	private const string IMGUI_COL_TO_UINT_TAG = "OmenTools.Extensions.ColorExtension.ImGuiColToUInt";

	private static readonly Dictionary<ImGuiCol, uint> ImGuiColToUInt = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<ImGuiCol, uint>>("OmenTools.Extensions.ColorExtension.ImGuiColToUInt", (Func<Dictionary<ImGuiCol, uint>>)(() => new Dictionary<ImGuiCol, uint>()));

	[DataShareTag]
	private const string KNOWN_COLOR_TO_VECTOR4_TAG = "OmenTools.Extensions.ColorExtension.KnownColorToVector4";

	private static readonly Dictionary<KnownColor, Vector4> KnownColorToVector4 = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<KnownColor, Vector4>>("OmenTools.Extensions.ColorExtension.KnownColorToVector4", (Func<Dictionary<KnownColor, Vector4>>)(() => new Dictionary<KnownColor, Vector4>()));

	[DataShareTag]
	private const string KNOWN_COLOR_TO_UINT_TAG = "OmenTools.Extensions.ColorExtension.KnownColorToUInt";

	private static readonly Dictionary<KnownColor, uint> KnownColorToUInt = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<KnownColor, uint>>("OmenTools.Extensions.ColorExtension.KnownColorToUInt", (Func<Dictionary<KnownColor, uint>>)(() => new Dictionary<KnownColor, uint>()));

	[DataShareTag]
	private const string UINT_TO_VECTOR4_TAG = "OmenTools.Extensions.ColorExtension.UIntToVector4";

	private static readonly Dictionary<uint, Vector4> UIntToVector4 = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Vector4>>("OmenTools.Extensions.ColorExtension.UIntToVector4", (Func<Dictionary<uint, Vector4>>)(() => new Dictionary<uint, Vector4>()));

	[DataShareTag]
	private const string REVERSE_UINT_TO_VECTOR4_TAG = "OmenTools.Extensions.ColorExtension.ReverseUIntToVector4";

	private static readonly Dictionary<uint, Vector4> ReverseUIntToVector4 = DalamudServiceExtension.Instance().GetOrCreateData<Dictionary<uint, Vector4>>("OmenTools.Extensions.ColorExtension.ReverseUIntToVector4", (Func<Dictionary<uint, Vector4>>)(() => new Dictionary<uint, Vector4>()));
}
