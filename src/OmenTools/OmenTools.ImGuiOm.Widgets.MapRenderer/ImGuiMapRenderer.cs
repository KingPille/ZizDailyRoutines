using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.ImGuiOm.Widgets.MapRenderer;

public class ImGuiMapRenderer
{
	private ISharedImmediateTexture? mapTexture;

	private Vector2 currentPan = Vector2.Zero;

	private Vector2 targetPan = Vector2.Zero;

	private float currentZoom = 1f;

	private float targetZoom = 1f;

	private bool isDragging;

	private bool isMouseDown;

	private Vector2 mouseDownPos = Vector2.Zero;

	private ImGuiMouseButton mouseDownButton;

	private readonly List<ImGuiMapMarker> markers = new List<ImGuiMapMarker>();

	private readonly Dictionary<uint, ISharedImmediateTexture> iconTextureCache = new Dictionary<uint, ISharedImmediateTexture>();

	private readonly List<ImGuiMapMarker> defaultMarkers = new List<ImGuiMapMarker>();

	private Vector2 currentViewportMin = Vector2.Zero;

	private Vector2 currentViewportSize = Vector2.Zero;

	private Vector2 currentBaseSize = Vector2.Zero;

	private float currentBaseScale = 1f;

	public bool Zoomable { get; set; } = true;

	public bool Pannable { get; set; } = true;

	public float MinZoom { get; set; } = 0.2f;

	public float MaxZoom { get; set; } = 5f;

	public float LerpSpeed { get; set; } = 15f;

	public bool EnableResizeGrip { get; set; } = true;

	public bool EnableDefaultMarkers { get; set; }

	public Func<MapMarker, bool>? DefaultMarkerFilter { get; set; }

	public uint MapID { get; private set; }

	public Map? CurrentMap { get; private set; }

	public Vector2 CustomViewportSize { get; set; } = Vector2.Zero;

	public Action<ImGuiMapRenderer, Vector3, Vector2, ImGuiMouseButton>? OnMapClicked { get; set; }

	public Action<ImGuiMapRenderer, Vector3, Vector2>? OnMapHovered { get; set; }

	public Action<Vector2>? OnViewportResized { get; set; }

	public Action<ImGuiMapRenderer, ImDrawListPtr>? OnCustomMapDraw { get; set; }

	public Action<ImGuiMapRenderer, ImDrawListPtr>? OnCustomForegroundDraw { get; set; }

	public void SetMap(uint mapID)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (MapID != mapID || !CurrentMap.HasValue)
		{
			MapID = mapID;
			Map? row = LuminaGetter.GetRow<Map>(mapID);
			if (row.HasValue)
			{
				CurrentMap = row.Value;
				string texturePath = CurrentMap.Value.GetTexturePath();
				mapTexture = DalamudServiceExtension.Instance<ITextureProvider>().GetFromGame(texturePath);
			}
			else
			{
				CurrentMap = null;
				mapTexture = null;
			}
			RefreshDefaultMarkers();
			ResetView();
		}
	}

	public void RefreshDefaultMarkers()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		defaultMarkers.Clear();
		if (!CurrentMap.HasValue)
		{
			return;
		}
		foreach (MapMarker mapMarker in CurrentMap.Value.GetMapMarkers())
		{
			MapMarker marker = mapMarker;
			if (((MapMarker)(ref marker)).Icon != 0 && (DefaultMarkerFilter == null || DefaultMarkerFilter(marker)))
			{
				Vector2 vector = PositionHelper.TextureToWorld(new Vector2(((MapMarker)(ref marker)).X, ((MapMarker)(ref marker)).Y), CurrentMap.Value);
				defaultMarkers.Add(new ImGuiMapMarker
				{
					ID = $"__default_{((MapMarker)(ref marker)).RowId}_{((MapMarker)(ref marker)).SubrowId}",
					Position = vector.ToVector3(0f),
					Size = Globals.ScaledVector2(24f),
					IconID = ((MapMarker)(ref marker)).Icon,
					Name = marker.GetMarkerLabel(),
					ShowLabel = false,
					ShowTooltip = true
				});
			}
		}
	}

	public void ResetView()
	{
		currentPan = Vector2.Zero;
		targetPan = Vector2.Zero;
		currentZoom = 1f;
		targetZoom = 1f;
	}

	public void AddMarker(ImGuiMapMarker marker)
	{
		if (string.IsNullOrEmpty(marker.ID))
		{
			marker.ID = Guid.NewGuid().ToString();
		}
		markers.RemoveAll((ImGuiMapMarker x) => x.ID == marker.ID);
		markers.Add(marker);
	}

	public bool RemoveMarker(string id)
	{
		return markers.RemoveAll((ImGuiMapMarker x) => x.ID == id) > 0;
	}

	public void ClearMarkers()
	{
		markers.Clear();
	}

	public ImGuiMapMarker? GetMarker(string id)
	{
		return markers.FirstOrDefault((ImGuiMapMarker x) => x.ID == id);
	}

	public Vector2 WorldToScreen(Vector3 worldPos, Vector2 viewportMin, Vector2 viewportSize, Vector2 baseSize, float baseScale)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!CurrentMap.HasValue)
		{
			return Vector2.Zero;
		}
		Vector2 vector = PositionHelper.WorldToTexture(worldPos, CurrentMap.Value);
		return viewportMin + (viewportSize - baseSize * currentZoom) / 2f + currentPan + vector * baseScale * currentZoom;
	}

	public Vector2 WorldToScreen(Vector3 worldPos)
	{
		return WorldToScreen(worldPos, currentViewportMin, currentViewportSize, currentBaseSize, currentBaseScale);
	}

	public Vector3 ScreenToWorld(Vector2 screenPos, Vector2 viewportMin, Vector2 viewportSize, Vector2 baseSize, float baseScale)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if (!CurrentMap.HasValue)
		{
			return Vector3.Zero;
		}
		Vector2 vector = viewportMin + (viewportSize - baseSize * currentZoom) / 2f + currentPan;
		return PositionHelper.TextureToWorld((screenPos - vector) / (baseScale * currentZoom), CurrentMap.Value).ToVector3(0f);
	}

	public Vector3 ScreenToWorld(Vector2 screenPos)
	{
		return ScreenToWorld(screenPos, currentViewportMin, currentViewportSize, currentBaseSize, currentBaseScale);
	}

	public void Draw(Vector2 drawSize)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Invalid comparison between Unknown and I4
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d56: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e52: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
		//IL_1087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fde: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe4: Invalid comparison between Unknown and I4
		//IL_1016: Unknown result type (might be due to invalid IL or missing references)
		if (!CurrentMap.HasValue || mapTexture == null)
		{
			return;
		}
		IDalamudTextureWrap wrapOrEmpty = mapTexture.GetWrapOrEmpty();
		if (wrapOrEmpty.Handle == (IntPtr)IntPtr.Zero || wrapOrEmpty.Width < 64 || wrapOrEmpty.Height < 64)
		{
			return;
		}
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		Vector2 vector = drawSize;
		if (vector.X <= 0f)
		{
			vector.X = ImGui.GetContentRegionAvail().X;
		}
		if (vector.Y <= 0f)
		{
			vector.Y = ImGui.GetContentRegionAvail().Y;
		}
		Vector2 customViewportSize = CustomViewportSize;
		if (customViewportSize.X > 50f && customViewportSize.Y > 50f)
		{
			vector = CustomViewportSize;
		}
		float num = (EnableResizeGrip ? (14f * ImGuiHelpers.GlobalScale) : 0f);
		Vector2 vector2 = vector - new Vector2(num, num);
		ImGui.InvisibleButton(ImU8String.op_Implicit("##MapCanvas"), vector2, (ImGuiButtonFlags)0);
		Vector2 vector3 = cursorScreenPos + vector;
		bool flag = ImGui.IsItemHovered((ImGuiHoveredFlags)32);
		bool flag2 = false;
		if (EnableResizeGrip)
		{
			Vector2 vector4 = vector3 - new Vector2(num, num);
			Vector2 mousePos = ImGui.GetMousePos();
			flag2 = mousePos.X >= vector4.X && mousePos.X <= vector3.X && mousePos.Y >= vector4.Y && mousePos.Y <= vector3.Y;
		}
		if (flag && !flag2)
		{
			for (ImGuiMouseButton val = (ImGuiMouseButton)0; (int)val <= 1; val = (ImGuiMouseButton)(val + 1))
			{
				if (ImGui.IsMouseClicked(val))
				{
					mouseDownPos = ImGui.GetMousePos();
					isMouseDown = true;
					mouseDownButton = val;
					isDragging = false;
					break;
				}
			}
		}
		float num2 = Math.Min(vector.X / (float)wrapOrEmpty.Width, vector.Y / (float)wrapOrEmpty.Height);
		Vector2 vector5 = new Vector2((float)wrapOrEmpty.Width * num2, (float)wrapOrEmpty.Height * num2);
		currentViewportMin = cursorScreenPos;
		currentViewportSize = vector;
		currentBaseSize = vector5;
		currentBaseScale = num2;
		ImGuiIOPtr iO = ImGui.GetIO();
		float num3 = Math.Min(((ImGuiIOPtr)(ref iO)).DeltaTime * LerpSpeed, 1f);
		float num4 = currentZoom;
		currentZoom += (targetZoom - currentZoom) * num3;
		if (Zoomable && flag)
		{
			iO = ImGui.GetIO();
			if (((ImGuiIOPtr)(ref iO)).MouseWheel != 0f)
			{
				iO = ImGui.GetIO();
				float mouseWheel = ((ImGuiIOPtr)(ref iO)).MouseWheel;
				float num5 = 1f + mouseWheel * 0.12f;
				float num6 = Math.Clamp(targetZoom * num5, MinZoom, MaxZoom);
				Vector2 vector6 = ImGui.GetMousePos() - cursorScreenPos;
				Vector2 vector7 = (vector6 - (vector - vector5 * targetZoom) / 2f - targetPan) / num4;
				targetPan = vector6 - (vector - vector5 * num6) / 2f - vector7 * num6;
				targetZoom = num6;
			}
		}
		currentPan += (targetPan - currentPan) * num3;
		if (Pannable)
		{
			if (isMouseDown)
			{
				if (ImGui.IsMouseDown(mouseDownButton))
				{
					Vector2 vector8 = ImGui.GetMousePos() - mouseDownPos;
					if (isDragging || vector8.Length() > 16f)
					{
						isDragging = true;
						iO = ImGui.GetIO();
						Vector2 mouseDelta = ((ImGuiIOPtr)(ref iO)).MouseDelta;
						if (mouseDelta != Vector2.Zero)
						{
							targetPan += mouseDelta;
							currentPan += mouseDelta;
						}
					}
				}
				else if (!ImGui.IsMouseDown(mouseDownButton) && !ImGui.IsMouseReleased(mouseDownButton))
				{
					isMouseDown = false;
				}
			}
			if (flag && ImGui.IsMouseDoubleClicked((ImGuiMouseButton)0))
			{
				ResetView();
				isMouseDown = false;
				isDragging = false;
			}
		}
		((ImDrawListPtr)(ref windowDrawList)).PushClipRect(cursorScreenPos, vector3, true);
		Vector2 vector9 = cursorScreenPos + (vector - vector5 * currentZoom) / 2f + currentPan;
		Vector2 vector10 = vector9 + vector5 * currentZoom;
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector3, 436207616u, 4f);
		((ImDrawListPtr)(ref windowDrawList)).AddImage(wrapOrEmpty.Handle, vector9, vector10);
		OnCustomMapDraw?.Invoke(this, windowDrawList);
		List<(ImGuiMapMarker, Vector2)> list = new List<(ImGuiMapMarker, Vector2)>();
		Vector2 mousePos2 = ImGui.GetMousePos();
		ImGuiMapMarker imGuiMapMarker = null;
		if (EnableDefaultMarkers)
		{
			foreach (ImGuiMapMarker defaultMarker in defaultMarkers)
			{
				Vector2 vector11 = WorldToScreen(defaultMarker.Position);
				if (vector11.X < cursorScreenPos.X - 50f || vector11.X > vector3.X + 50f || vector11.Y < cursorScreenPos.Y - 50f || vector11.Y > vector3.Y + 50f)
				{
					continue;
				}
				Vector2 vector12 = defaultMarker.Size / 2f;
				Vector2 vector13 = vector11 - vector12;
				Vector2 vector14 = vector11 + vector12;
				int num7;
				if (flag && mousePos2.X >= vector13.X && mousePos2.X <= vector14.X && mousePos2.Y >= vector13.Y)
				{
					num7 = ((mousePos2.Y <= vector14.Y) ? 1 : 0);
					if (num7 != 0)
					{
						imGuiMapMarker = defaultMarker;
						defaultMarker.OnHover?.Invoke(defaultMarker);
					}
				}
				else
				{
					num7 = 0;
				}
				ImTextureID val2 = default(ImTextureID);
				if (defaultMarker.IconID.HasValue)
				{
					if (!iconTextureCache.TryGetValue(defaultMarker.IconID.Value, out ISharedImmediateTexture value))
					{
						ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
						GameIconLookup val3 = new GameIconLookup(defaultMarker.IconID.Value, false, true, (ClientLanguage?)null);
						value = obj.GetFromGameIcon(ref val3);
						iconTextureCache[defaultMarker.IconID.Value] = value;
					}
					IDalamudTextureWrap wrapOrEmpty2 = value.GetWrapOrEmpty();
					if (wrapOrEmpty2.Handle != (IntPtr)IntPtr.Zero)
					{
						val2 = wrapOrEmpty2.Handle;
					}
				}
				Vector2 vector15 = vector13;
				Vector2 vector16 = vector14;
				if (num7 != 0)
				{
					vector15 -= new Vector2(2f, 2f);
					vector16 += new Vector2(2f, 2f);
					ImGui.SetMouseCursor((ImGuiMouseCursor)7);
				}
				if (val2 != (IntPtr)IntPtr.Zero)
				{
					((ImDrawListPtr)(ref windowDrawList)).AddImage(val2, vector15, vector16, Vector2.Zero, Vector2.One, defaultMarker.Color);
				}
				else
				{
					((ImDrawListPtr)(ref windowDrawList)).AddCircleFilled(vector11, Math.Min(defaultMarker.Size.X, defaultMarker.Size.Y) / 2f, defaultMarker.Color);
					((ImDrawListPtr)(ref windowDrawList)).AddCircle(vector11, Math.Min(defaultMarker.Size.X, defaultMarker.Size.Y) / 2f, uint.MaxValue, 16, 1.5f);
				}
				if (defaultMarker.ShowLabel && !string.IsNullOrEmpty(defaultMarker.Label))
				{
					list.Add((defaultMarker, vector11));
				}
			}
		}
		foreach (ImGuiMapMarker marker in markers)
		{
			Vector2 vector17 = WorldToScreen(marker.Position);
			if (vector17.X < cursorScreenPos.X - 50f || vector17.X > vector3.X + 50f || vector17.Y < cursorScreenPos.Y - 50f || vector17.Y > vector3.Y + 50f)
			{
				continue;
			}
			Vector2 vector18 = marker.Size / 2f;
			Vector2 vector19 = vector17 - vector18;
			Vector2 vector20 = vector17 + vector18;
			int num8;
			if (flag && mousePos2.X >= vector19.X && mousePos2.X <= vector20.X && mousePos2.Y >= vector19.Y)
			{
				num8 = ((mousePos2.Y <= vector20.Y) ? 1 : 0);
				if (num8 != 0)
				{
					imGuiMapMarker = marker;
					marker.OnHover?.Invoke(marker);
				}
			}
			else
			{
				num8 = 0;
			}
			if (marker.PulseEffect)
			{
				float num9 = (float)ImGui.GetTime();
				float num10 = (float)Math.Sin(num9 * 8f) * 0.15f + 1f;
				float num11 = marker.Size.X * 0.7f * num10;
				float num12 = num9 % 1.2f;
				float num13 = 1f - num12 / 1.2f;
				float num14 = marker.Size.X * 0.5f + num12 * 25f;
				if (num13 > 0f)
				{
					uint pulseColor = marker.PulseColor;
					uint num15 = (uint)(num13 * (float)((pulseColor >> 24) & 0xFF));
					pulseColor = (pulseColor & 0xFFFFFF) | (num15 << 24);
					((ImDrawListPtr)(ref windowDrawList)).AddCircle(vector17, num14, pulseColor, 32, 2f);
				}
				((ImDrawListPtr)(ref windowDrawList)).AddCircleFilled(vector17, num11, (marker.PulseColor & 0xFFFFFF) | 0x40000000);
			}
			ImTextureID val4 = default(ImTextureID);
			if (marker.Texture != null)
			{
				IDalamudTextureWrap wrapOrEmpty3 = marker.Texture.GetWrapOrEmpty();
				if (wrapOrEmpty3.Handle != (IntPtr)IntPtr.Zero)
				{
					val4 = wrapOrEmpty3.Handle;
				}
			}
			else if (marker.IconID.HasValue)
			{
				if (!iconTextureCache.TryGetValue(marker.IconID.Value, out ISharedImmediateTexture value2))
				{
					ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val3 = new GameIconLookup(marker.IconID.Value, false, true, (ClientLanguage?)null);
					value2 = obj2.GetFromGameIcon(ref val3);
					iconTextureCache[marker.IconID.Value] = value2;
				}
				IDalamudTextureWrap wrapOrEmpty4 = value2.GetWrapOrEmpty();
				if (wrapOrEmpty4.Handle != (IntPtr)IntPtr.Zero)
				{
					val4 = wrapOrEmpty4.Handle;
				}
			}
			Vector2 vector21 = vector19;
			Vector2 vector22 = vector20;
			if (num8 != 0)
			{
				vector21 -= new Vector2(2f, 2f);
				vector22 += new Vector2(2f, 2f);
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (marker.OnCustomDraw != null)
			{
				marker.OnCustomDraw(marker, vector17, windowDrawList);
			}
			else if (val4 != (IntPtr)IntPtr.Zero)
			{
				((ImDrawListPtr)(ref windowDrawList)).AddImage(val4, vector21, vector22, Vector2.Zero, Vector2.One, marker.Color);
			}
			else
			{
				((ImDrawListPtr)(ref windowDrawList)).AddCircleFilled(vector17, Math.Min(marker.Size.X, marker.Size.Y) / 2f, marker.Color);
				((ImDrawListPtr)(ref windowDrawList)).AddCircle(vector17, Math.Min(marker.Size.X, marker.Size.Y) / 2f, uint.MaxValue, 16, 1.5f);
			}
			if (marker.ShowLabel && !string.IsNullOrEmpty(marker.Label))
			{
				list.Add((marker, vector17));
			}
		}
		foreach (var item3 in list)
		{
			ImGuiMapMarker item = item3.Item1;
			Vector2 item2 = item3.Item2;
			string label = item.Label;
			Vector2 vector23 = ImGui.CalcTextSize(ImU8String.op_Implicit(label), false, -1f);
			Vector2 vector24 = new Vector2(3f, 2f);
			Vector2 vector25 = item2 - new Vector2(vector23.X / 2f, item.Size.Y / 2f + vector23.Y + 4f);
			Vector2 vector26 = cursorScreenPos + vector24;
			Vector2 vector27 = vector3 - vector24 - vector23;
			if (vector26.X < vector27.X)
			{
				vector25.X = Math.Clamp(vector25.X, vector26.X, vector27.X);
			}
			if (vector26.Y < vector27.Y)
			{
				vector25.Y = Math.Clamp(vector25.Y, vector26.Y, vector27.Y);
			}
			Vector2 vector28 = vector25 - vector24;
			Vector2 vector29 = vector25 + vector23 + vector24;
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector28, vector29, 3422552064u, 3f);
			((ImDrawListPtr)(ref windowDrawList)).AddRect(vector28, vector29, 1090519039u, 3f, (ImDrawFlags)0, 1f);
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector25, item.LabelColor, ImU8String.op_Implicit(label));
			if (!string.IsNullOrEmpty(item.Hint))
			{
				string hint = item.Hint;
				Vector2 vector30 = ImGui.CalcTextSize(ImU8String.op_Implicit(hint), false, -1f);
				Vector2 vector31 = new Vector2(3f, 1f);
				Vector2 vector32 = new Vector2(vector29.X + 2f, vector28.Y + (vector29.Y - vector28.Y - vector30.Y) / 2f);
				if (vector32.X + vector30.X + vector31.X < vector3.X)
				{
					Vector2 vector33 = vector32 - vector31;
					Vector2 vector34 = vector32 + vector30 + vector31;
					((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector33, vector34, 3861259814u, 3f);
					((ImDrawListPtr)(ref windowDrawList)).AddRect(vector33, vector34, 822083583u, 3f, (ImDrawFlags)0, 1f);
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector32, item.HintColor, ImU8String.op_Implicit(hint));
				}
			}
		}
		if (imGuiMapMarker != null && imGuiMapMarker.ShowTooltip)
		{
			string text = (string.IsNullOrEmpty(imGuiMapMarker.TooltipText) ? imGuiMapMarker.Name : imGuiMapMarker.TooltipText);
			if (!string.IsNullOrEmpty(text))
			{
				ImGui.BeginTooltip();
				ImGui.TextUnformatted(ImU8String.op_Implicit(text));
				if (!string.IsNullOrEmpty(imGuiMapMarker.Description))
				{
					ImGui.TextDisabled(ImU8String.op_Implicit(imGuiMapMarker.Description));
				}
				ImGui.EndTooltip();
			}
		}
		OnCustomForegroundDraw?.Invoke(this, windowDrawList);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector3, ImGui.GetColorU32((ImGuiCol)5), 4f, (ImDrawFlags)0, 2f);
		if (flag && !isDragging)
		{
			Vector3 arg = ScreenToWorld(mousePos2, cursorScreenPos, vector, vector5, num2);
			Vector2 arg2 = (mousePos2 - vector9) / (num2 * currentZoom);
			OnMapHovered?.Invoke(this, arg, arg2);
		}
		if (isMouseDown && ImGui.IsMouseReleased(mouseDownButton))
		{
			if (!isDragging)
			{
				Vector2 mousePos3 = ImGui.GetMousePos();
				Vector3 arg3 = ScreenToWorld(mousePos3, cursorScreenPos, vector, vector5, num2);
				Vector2 arg4 = (mousePos3 - vector9) / (num2 * currentZoom);
				bool flag3 = false;
				if (imGuiMapMarker != null)
				{
					if ((int)mouseDownButton == 0 && imGuiMapMarker.OnClick != null)
					{
						imGuiMapMarker.OnClick(imGuiMapMarker);
						flag3 = true;
					}
					else if ((int)mouseDownButton == 1 && imGuiMapMarker.OnRightClick != null)
					{
						imGuiMapMarker.OnRightClick(imGuiMapMarker);
						flag3 = true;
					}
				}
				if (!flag3)
				{
					OnMapClicked?.Invoke(this, arg3, arg4, mouseDownButton);
				}
			}
			isMouseDown = false;
			isDragging = false;
		}
		if (EnableResizeGrip)
		{
			Vector2 cursorScreenPos2 = vector3 - new Vector2(num, num);
			((ImDrawListPtr)(ref windowDrawList)).AddTriangleFilled(vector3, vector3 - new Vector2(num, 0f), vector3 - new Vector2(0f, num), 2868903935u);
			ImGui.SetCursorScreenPos(cursorScreenPos2);
			ImGui.InvisibleButton(ImU8String.op_Implicit("##MapRendererResizeGrip"), new Vector2(num, num), (ImGuiButtonFlags)0);
			if (ImGui.IsItemActive() && ImGui.IsMouseDragging((ImGuiMouseButton)0))
			{
				iO = ImGui.GetIO();
				Vector2 mouseDelta2 = ((ImGuiIOPtr)(ref iO)).MouseDelta;
				Vector2 vector35 = ((CustomViewportSize.X > 50f) ? CustomViewportSize : vector);
				CustomViewportSize = new Vector2(Math.Max(100f, vector35.X + mouseDelta2.X), Math.Max(100f, vector35.Y + mouseDelta2.Y));
				OnViewportResized?.Invoke(CustomViewportSize);
			}
		}
		((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
	}
}
