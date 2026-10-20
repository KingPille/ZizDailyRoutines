using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Graphics.Kernel;
using FFXIVClientStructs.FFXIV.Common.Math;

namespace OmenTools.Interop.Game.Helpers;

public static class GameViewHelper
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool WorldToScreen(Vector3 worldPos, out Vector2 screenPos, out bool inView)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		Device* ptr = Device.Instance();
		ImGuiViewportPtr mainViewport = ImGuiHelpers.MainViewport;
		Vector2 pos = ((ImGuiViewportPtr)(ref mainViewport)).Pos;
		Matrix4x4 viewProjectionMatrix = ((Control)Control.Instance()).ViewProjectionMatrix;
		float x = worldPos.X;
		float y = worldPos.Y;
		float z = worldPos.Z;
		float num = x * viewProjectionMatrix.M14 + y * viewProjectionMatrix.M24 + z * viewProjectionMatrix.M34 + viewProjectionMatrix.M44;
		if ((double)num < 0.001)
		{
			screenPos = default(Vector2);
			inView = false;
			return false;
		}
		double num2 = 1.0 / (double)num;
		double num3 = (double)(x * viewProjectionMatrix.M11 + y * viewProjectionMatrix.M21 + z * viewProjectionMatrix.M31 + viewProjectionMatrix.M41) * num2;
		double num4 = (double)(x * viewProjectionMatrix.M12 + y * viewProjectionMatrix.M22 + z * viewProjectionMatrix.M32 + viewProjectionMatrix.M42) * num2;
		uint width = ((Device)ptr).Width;
		uint height = ((Device)ptr).Height;
		float num5 = (float)width * 0.5f;
		float num6 = (float)height * 0.5f;
		float num7 = (float)(num3 * (double)num5) + num5 + pos.X;
		float num8 = num6 - (float)(num4 * (double)num6) + pos.Y;
		screenPos = new Vector2(num7, num8);
		inView = num7 >= pos.X && num7 <= pos.X + (float)width && num8 >= pos.Y && num8 <= pos.Y + (float)height;
		return true;
	}
}
