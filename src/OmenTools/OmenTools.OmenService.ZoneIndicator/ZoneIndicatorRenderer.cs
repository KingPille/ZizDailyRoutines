using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Threading;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Common.Math;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Helpers;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService.ZoneIndicator;

public sealed class ZoneIndicatorRenderer : OmenServiceBase<ZoneIndicatorRenderer>
{
	internal sealed class CachedEntryState(ulong entryID, CachedDrawTarget[] targets, ZoneIndicatorSurrounding? surrounding, bool hideTextLabel)
	{
		public ulong EntryID { get; } = entryID;

		public CachedDrawTarget[] Targets { get; } = targets;

		public ZoneIndicatorSurrounding? Surrounding { get; } = surrounding;

		public bool HideTextLabel { get; } = hideTextLabel;
	}

	internal readonly struct CachedDrawTarget(Vector3 worldPosition, float distance, ZoneIndicatorText? text, Vector4 textColor, Action<ZoneIndicatorDrawContext>? onDraw)
	{
		public Vector3 WorldPosition { get; } = worldPosition;

		public float Distance { get; } = distance;

		public ZoneIndicatorText? Text { get; } = text;

		public Vector4 TextColor { get; } = textColor;

		public Action<ZoneIndicatorDrawContext>? OnDraw { get; } = onDraw;
	}

	internal struct TextDrawInfo
	{
		public Vector2 ScreenPos;

		public ReadOnlySeString? Text;

		public Vector4 Color;

		public float Scale;

		public (ulong, int) CacheKey;

		public Vector2 CachedSize;

		public ZoneIndicatorText.TextImage? Image;
	}

	private readonly ConcurrentDictionary<ulong, ZoneIndicatorEntryBase> masterStore = new ConcurrentDictionary<ulong, ZoneIndicatorEntryBase>();

	private ImmutableArray<ZoneIndicatorEntryBase> activeEntries = ImmutableArray<ZoneIndicatorEntryBase>.Empty;

	private CachedEntryState[] cachedDrawStates = Array.Empty<CachedEntryState>();

	private readonly List<CachedEntryState> stateListBuffer = new List<CachedEntryState>();

	private readonly List<CachedDrawTarget> targetListBuffer = new List<CachedDrawTarget>();

	private readonly List<TextDrawInfo> textDrawBuffer = new List<TextDrawInfo>();

	private readonly ConcurrentDictionary<(ulong EntryID, int TargetIndex), (Vector2 Size, float TextOnlyHeight)> textSizeCache = new ConcurrentDictionary<(ulong, int), (Vector2, float)>();

	private long nextID;

	private static readonly Vector2 LabelPadding = new Vector2(6f, 3f);

	private const int CIRCLE_SEGMENTS = 64;

	private static readonly Vector2[] UnitCircleOffsets = BuildUnitCircleOffsets();

	private static readonly Vector2[] UnitSquareOffsets = new Vector2[4]
	{
		new Vector2(-1f, -1f),
		new Vector2(1f, -1f),
		new Vector2(1f, 1f),
		new Vector2(-1f, 1f)
	};

	private static readonly Vector2[] UnitTriangleOffsets = new Vector2[3]
	{
		new Vector2(0f, -1f),
		new Vector2(0.8660254f, 0.5f),
		new Vector2(-0.8660254f, 0.5f)
	};

	private static readonly Vector2 FallbackTextSize = new Vector2(50f, 20f);

	public ZoneIndicatorHandle RegTemporary<T>(Func<List<T>> sourceGetter, Func<T, Vector3> posGetter, ZoneIndicatorOptions<T>? options = null)
	{
		ArgumentNullException.ThrowIfNull(sourceGetter, "sourceGetter");
		ArgumentNullException.ThrowIfNull(posGetter, "posGetter");
		return Register(new ZoneIndicatorEntry<T>(NewID(), GameState.TerritoryType, isPermanent: false, sourceGetter, posGetter, options ?? new ZoneIndicatorOptions<T>()));
	}

	public ZoneIndicatorHandle RegPermanent<T>(uint territoryType, Func<List<T>> sourceGetter, Func<T, Vector3> posGetter, ZoneIndicatorOptions<T>? options = null)
	{
		ArgumentNullException.ThrowIfNull(sourceGetter, "sourceGetter");
		ArgumentNullException.ThrowIfNull(posGetter, "posGetter");
		return Register(new ZoneIndicatorEntry<T>(NewID(), territoryType, isPermanent: true, sourceGetter, posGetter, options ?? new ZoneIndicatorOptions<T>()));
	}

	protected override void Init()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += OnZoneChanged;
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 100u);
		OmenServiceBase<WindowManager>.Instance().PostDraw += OnDraw;
	}

	protected override void Uninit()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= OnZoneChanged;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		OmenServiceBase<WindowManager>.Instance().PostDraw -= OnDraw;
		masterStore.Clear();
		textSizeCache.Clear();
		activeEntries = ImmutableArray<ZoneIndicatorEntryBase>.Empty;
		cachedDrawStates = Array.Empty<CachedEntryState>();
	}

	private void OnZoneChanged(uint territoryType)
	{
		foreach (KeyValuePair<ulong, ZoneIndicatorEntryBase> item in masterStore)
		{
			item.Deconstruct(out var key, out var value);
			ulong key2 = key;
			if (!value.IsPermanent)
			{
				masterStore.TryRemove(key2, out value);
			}
		}
		textSizeCache.Clear();
		RebuildActiveEntries();
	}

	private void OnUpdate(IFramework framework)
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		ImmutableArray<ZoneIndicatorEntryBase> immutableArray = activeEntries;
		if (immutableArray.IsDefaultOrEmpty)
		{
			cachedDrawStates = Array.Empty<CachedEntryState>();
			return;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			cachedDrawStates = Array.Empty<CachedEntryState>();
			return;
		}
		Vector3 position = localPlayer.Position;
		Vector3? vector = null;
		stateListBuffer.Clear();
		ImmutableArray<ZoneIndicatorEntryBase>.Enumerator enumerator = immutableArray.GetEnumerator();
		while (enumerator.MoveNext())
		{
			ZoneIndicatorEntryBase current = enumerator.Current;
			targetListBuffer.Clear();
			foreach (ResolvedTarget item in current.ResolveTargets())
			{
				float num = Vector3.DistanceSquared(position, item.Position);
				float? renderRadiusSquared = current.RenderRadiusSquared;
				if (renderRadiusSquared.HasValue)
				{
					float valueOrDefault = renderRadiusSquared.GetValueOrDefault();
					if (num > valueOrDefault)
					{
						continue;
					}
				}
				if (current.HiddenWhenBlocked)
				{
					Vector3 valueOrDefault2 = vector.GetValueOrDefault();
					if (!vector.HasValue)
					{
						valueOrDefault2 = (TryGetCameraPosition(out var position2) ? position2 : position);
						vector = valueOrDefault2;
					}
					if (!RaycastHelper.HasLineOfSight(vector.Value, item.Position))
					{
						continue;
					}
				}
				ZoneIndicatorText text = item.Text;
				if (text == null)
				{
					goto IL_019d;
				}
				ReadOnlySeString? text2 = text.Text;
				Vector4 vector2;
				if (text2.HasValue)
				{
					ReadOnlySeString valueOrDefault3 = text2.GetValueOrDefault();
					vector2 = item.Text.TextColor ?? GetStableColor(valueOrDefault3);
				}
				else
				{
					if (text.Image == null)
					{
						goto IL_019d;
					}
					vector2 = item.Text.TextColor ?? new Vector4(1f);
				}
				goto IL_01a4;
				IL_019d:
				vector2 = Vector4.Zero;
				goto IL_01a4;
				IL_01a4:
				Vector4 textColor = vector2;
				targetListBuffer.Add(new CachedDrawTarget(item.Position, MathF.Sqrt(num), item.Text, textColor, item.OnDraw));
			}
			if (targetListBuffer.Count > 0)
			{
				stateListBuffer.Add(new CachedEntryState(current.ID, targetListBuffer.ToArray(), current.Surrounding, current.HideTextLabel));
			}
		}
		cachedDrawStates = stateListBuffer.ToArray();
	}

	private void OnDraw()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		CachedEntryState[] array = cachedDrawStates;
		if (array.Length == 0)
		{
			return;
		}
		ImDrawListPtr backgroundDrawList = ImGui.GetBackgroundDrawList();
		ImDrawListPtr foregroundDrawList = ImGui.GetForegroundDrawList();
		textDrawBuffer.Clear();
		CachedEntryState[] array2 = array;
		Vector2 vector = default(Vector2);
		foreach (CachedEntryState cachedEntryState in array2)
		{
			for (int j = 0; j < cachedEntryState.Targets.Length; j++)
			{
				CachedDrawTarget cachedDrawTarget = cachedEntryState.Targets[j];
				bool flag = DalamudServiceExtension.Instance<IGameGui>().WorldToScreen(cachedDrawTarget.WorldPosition, ref vector);
				try
				{
					Vector2 textSize = FallbackTextSize;
					(ulong, int) tuple = (cachedEntryState.EntryID, j);
					ZoneIndicatorText text = cachedDrawTarget.Text;
					if (text != null && flag && (text.Text.HasValue || text.Image != null))
					{
						Vector3? textOffset = text.TextOffset;
						Vector2 vector2;
						if (textOffset.HasValue)
						{
							Vector3 valueOrDefault = textOffset.GetValueOrDefault();
							vector2 = vector + new Vector2(valueOrDefault.X, valueOrDefault.Y);
						}
						else
						{
							vector2 = vector;
						}
						Vector2 vector3 = vector2;
						(Vector2, float) value;
						Vector2 vector4 = (textSizeCache.TryGetValue(tuple, out value) ? value.Item1 : FallbackTextSize);
						if (!cachedEntryState.HideTextLabel)
						{
							DrawTextBackground(backgroundDrawList, vector3, cachedDrawTarget.TextColor, vector4);
						}
						textDrawBuffer.Add(new TextDrawInfo
						{
							ScreenPos = vector3,
							Text = text.Text,
							Color = cachedDrawTarget.TextColor,
							Scale = (text.TextScale ?? 1f),
							CacheKey = tuple,
							CachedSize = vector4,
							Image = text.Image
						});
						textSize = vector4;
					}
					ZoneIndicatorSurrounding surrounding = cachedEntryState.Surrounding;
					if (surrounding != null)
					{
						DrawSurrounding(backgroundDrawList, cachedDrawTarget.WorldPosition, surrounding);
					}
					Action<ZoneIndicatorDrawContext> onDraw = cachedDrawTarget.OnDraw;
					if (onDraw != null)
					{
						ZoneIndicatorDrawContext obj = new ZoneIndicatorDrawContext(cachedDrawTarget.WorldPosition, vector, flag, cachedDrawTarget.Distance, foregroundDrawList, textSize);
						onDraw(obj);
					}
				}
				catch (Exception ex)
				{
					DLog.Error("绘制区域物体标记时发生错误", ex);
				}
			}
		}
		if (textDrawBuffer.Count > 0)
		{
			RenderSeStringBatch();
		}
	}

	private ZoneIndicatorHandle Register(ZoneIndicatorEntryBase entry)
	{
		masterStore[entry.ID] = entry;
		RebuildActiveEntries();
		return new ZoneIndicatorHandle
		{
			ID = entry.ID
		};
	}

	private ulong NewID()
	{
		return (ulong)Interlocked.Increment(ref nextID);
	}

	internal bool UnregisterByID(ulong id)
	{
		if (!masterStore.TryRemove(id, out ZoneIndicatorEntryBase _))
		{
			return false;
		}
		foreach (var key in textSizeCache.Keys)
		{
			if (key.EntryID == id)
			{
				textSizeCache.TryRemove(key, out (Vector2, float) _);
			}
		}
		RebuildActiveEntries();
		return true;
	}

	internal bool UpdateByID(ulong id, Action<IZoneIndicatorMutable> mutator)
	{
		if (!masterStore.TryGetValue(id, out ZoneIndicatorEntryBase value))
		{
			return false;
		}
		mutator(value);
		return true;
	}

	private void RebuildActiveEntries()
	{
		uint territoryType = GameState.TerritoryType;
		ImmutableArray<ZoneIndicatorEntryBase>.Builder builder = ImmutableArray.CreateBuilder<ZoneIndicatorEntryBase>();
		foreach (ZoneIndicatorEntryBase value in masterStore.Values)
		{
			if (value.TerritoryType == territoryType)
			{
				builder.Add(value);
			}
		}
		activeEntries = builder.ToImmutable();
	}

	private static void DrawTextBackground(ImDrawListPtr drawList, Vector2 screenPosition, Vector4 textColor, Vector2 cachedSize)
	{
		Vector2 vector = cachedSize * 0.5f;
		Vector2 vector2 = screenPosition - vector - LabelPadding;
		Vector2 vector3 = screenPosition - vector + cachedSize + LabelPadding;
		Vector4 color = new Vector4(textColor.X * 0.08f, textColor.Y * 0.08f, textColor.Z * 0.08f, 0.9f);
		Vector4 vector4 = textColor;
		vector4.W = 0.8f;
		Vector4 color2 = vector4;
		((ImDrawListPtr)(ref drawList)).AddRectFilled(vector2, vector3, color.ToUInt(), 5f);
		((ImDrawListPtr)(ref drawList)).AddRect(vector2, vector3, color2.ToUInt(), 5f, (ImDrawFlags)0, 1f);
	}

	private void RenderSeStringBatch()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		ImGuiIOPtr iO = ImGui.GetIO();
		ImGui.SetNextWindowPos(Vector2.Zero, (ImGuiCond)1);
		ImGui.SetNextWindowSize(((ImGuiIOPtr)(ref iO)).DisplaySize, (ImGuiCond)1);
		ImGui.PushStyleVar((ImGuiStyleVar)1, Vector2.Zero);
		ImGui.PushStyleVar((ImGuiStyleVar)3, 0f);
		if (ImGui.Begin(ImU8String.op_Implicit("##zone_texts"), (ImGuiWindowFlags)2896815))
		{
			foreach (TextDrawInfo item in textDrawBuffer)
			{
				RenderTextItem(item);
			}
			ImGui.End();
		}
		ImGui.PopStyleVar(2);
	}

	private void RenderTextItem(TextDrawInfo d)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetCursorScreenPos(d.ScreenPos - d.CachedSize * 0.5f);
		using (OmenServiceBase<FontManager>.Instance().GetUIFont(d.Scale).Push())
		{
			ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, d.Color.ToUInt(), true);
			try
			{
				float item = 0f;
				GroupDisposable val2 = ImRaii.Group();
				try
				{
					ZoneIndicatorText.TextImage image = d.Image;
					ReadOnlySeString? text = d.Text;
					if (image != null)
					{
						if (text.HasValue)
						{
							ReadOnlySeString valueOrDefault = text.GetValueOrDefault();
							float num;
							if (!textSizeCache.TryGetValue(d.CacheKey, out (Vector2, float) value))
							{
								_ = FallbackTextSize;
								num = 0f;
							}
							else
							{
								(Vector2, float) tuple = value;
								num = tuple.Item2;
							}
							float cursorPosY = ImGui.GetCursorPosY();
							Vector2 vector = image.SizeGetter();
							float num2 = ((num > 0f) ? num : ImGui.GetTextLineHeight());
							float num3 = Math.Max(vector.Y, num2);
							IDalamudTextureWrap wrapOrDefault = image.Texture.GetWrapOrDefault((IDalamudTextureWrap)null);
							if (wrapOrDefault != null && wrapOrDefault.Handle != (IntPtr)IntPtr.Zero)
							{
								ImGui.SetCursorPosY(cursorPosY + (num3 - vector.Y) * 0.5f);
								ImGui.Image(wrapOrDefault.Handle, vector);
								ImGui.SameLine();
							}
							ImGui.SetCursorPosY(cursorPosY + (num3 - num2) * 0.5f);
							ReadOnlySpan<byte> readOnlySpan = ReadOnlySeString.op_Implicit(valueOrDefault);
							SeStringDrawParams val3 = default(SeStringDrawParams);
							ImGuiHelpers.SeStringWrapped(readOnlySpan, ref val3, default(ImGuiId), (ImGuiButtonFlags)1);
							item = ImGui.GetItemRectSize().Y;
						}
						else
						{
							ZoneIndicatorText.TextImage textImage = image;
							IDalamudTextureWrap wrapOrDefault2 = textImage.Texture.GetWrapOrDefault((IDalamudTextureWrap)null);
							if (wrapOrDefault2 != null && wrapOrDefault2.Handle != (IntPtr)IntPtr.Zero)
							{
								ImGui.Image(wrapOrDefault2.Handle, textImage.SizeGetter());
							}
						}
					}
					else if (text.HasValue)
					{
						ReadOnlySeString valueOrDefault = text.GetValueOrDefault();
						ReadOnlySpan<byte> readOnlySpan2 = ReadOnlySeString.op_Implicit(valueOrDefault);
						SeStringDrawParams val3 = default(SeStringDrawParams);
						ImGuiHelpers.SeStringWrapped(readOnlySpan2, ref val3, default(ImGuiId), (ImGuiButtonFlags)1);
						item = ImGui.GetItemRectSize().Y;
					}
				}
				finally
				{
					((GroupDisposable)(ref val2)).Dispose();
				}
				Vector2 itemRectSize = ImGui.GetItemRectSize();
				if (itemRectSize.X > 0f && itemRectSize.Y > 0f)
				{
					textSizeCache[d.CacheKey] = (itemRectSize, item);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	private static void DrawSurrounding(ImDrawListPtr drawList, Vector3 worldPos, ZoneIndicatorSurrounding s)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		IGameGui gui = DalamudServiceExtension.Instance<IGameGui>();
		uint color = s.Color.ToUInt();
		Vector2[] array = s.Type switch
		{
			ZoneIndicatorSurrounding.Shape.Circle => UnitCircleOffsets, 
			ZoneIndicatorSurrounding.Shape.Square => UnitSquareOffsets, 
			ZoneIndicatorSurrounding.Shape.Triangle => UnitTriangleOffsets, 
			_ => null, 
		};
		if (array != null)
		{
			DrawPolyline(drawList, gui, worldPos, array, s.Radius, color, s.Thickness);
		}
	}

	private static void DrawPolyline(ImDrawListPtr drawList, IGameGui gui, Vector3 center, ReadOnlySpan<Vector2> unitOffsets, float radius, uint color, float thickness)
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		int length = unitOffsets.Length;
		Span<Vector3> span = stackalloc Vector3[length];
		for (int i = 0; i < length; i++)
		{
			span[i] = new Vector3(center.X + unitOffsets[i].X * radius, center.Y, center.Z + unitOffsets[i].Y * radius);
		}
		Span<Vector2?> span2 = stackalloc Vector2?[length];
		Vector2 value = default(Vector2);
		for (int j = 0; j < length; j++)
		{
			if (gui.WorldToScreen(span[j], ref value))
			{
				span2[j] = value;
			}
		}
		for (int k = 0; k < length; k++)
		{
			int index = (k + 1) % length;
			DrawClippedEdge(drawList, gui, span[k], span[index], span2[k], span2[index], color, thickness);
		}
	}

	private static void DrawClippedEdge(ImDrawListPtr drawList, IGameGui gui, Vector3 worldA, Vector3 worldB, Vector2? screenA, Vector2? screenB, uint color, float thickness)
	{
		Vector2? vector = screenA;
		if (vector.HasValue)
		{
			Vector2 valueOrDefault = vector.GetValueOrDefault();
			if (screenB.HasValue)
			{
				Vector2 valueOrDefault2 = screenB.GetValueOrDefault();
				((ImDrawListPtr)(ref drawList)).AddLine(valueOrDefault, valueOrDefault2, color, thickness);
				return;
			}
		}
		else if (!screenB.HasValue)
		{
			return;
		}
		Vector3 visible = (screenA.HasValue ? worldA : worldB);
		Vector3 behind = ((!screenA.HasValue) ? worldA : worldB);
		Vector2 vector2 = screenA ?? screenB.Value;
		if (TryClipToNearPlane(gui, visible, behind, out var clipScreen))
		{
			((ImDrawListPtr)(ref drawList)).AddLine(vector2, clipScreen, color, thickness);
		}
	}

	private static bool TryClipToNearPlane(IGameGui gui, Vector3 visible, Vector3 behind, out Vector2 clipScreen)
	{
		clipScreen = default(Vector2);
		float num = 0f;
		float num2 = 1f;
		Vector2 vector = default(Vector2);
		for (int i = 0; i < 8; i++)
		{
			float num3 = (num + num2) * 0.5f;
			if (gui.WorldToScreen(Vector3.Lerp(visible, behind, num3), ref vector))
			{
				num = num3;
			}
			else
			{
				num2 = num3;
			}
		}
		return gui.WorldToScreen(Vector3.Lerp(visible, behind, num), ref clipScreen);
	}

	private static Vector4 GetStableColor(ReadOnlySeString text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return HslToRgb((float)((GetStableHash(text) & 0x7FFFFFFF) % 360) / 360f, 0.8f, 0.6f);
	}

	private unsafe static int GetStableHash(ReadOnlySeString text)
	{
		string text2 = ((object)(*(ReadOnlySeString*)(&text))/*cast due to constrained. prefix*/).ToString();
		int num = 352654597;
		int num2 = num;
		for (int i = 0; i < text2.Length; i += 2)
		{
			num = ((num << 5) + num) ^ text2[i];
			if (i + 1 < text2.Length)
			{
				num2 = ((num2 << 5) + num2) ^ text2[i + 1];
			}
		}
		return num + num2 * 1566083941;
	}

	private static Vector4 HslToRgb(float h, float s, float l)
	{
		if (s == 0f)
		{
			return new Vector4(l, l, l, 1f);
		}
		float num = ((l < 0.5f) ? (l * (1f + s)) : (l + s - l * s));
		float p = 2f * l - num;
		return new Vector4(HueToRgb(p, num, h + 1f / 3f), HueToRgb(p, num, h), HueToRgb(p, num, h - 1f / 3f), 1f);
	}

	private static float HueToRgb(float p, float q, float t)
	{
		if (t < 0f)
		{
			t += 1f;
		}
		if (t > 1f)
		{
			t -= 1f;
		}
		if (!(t < 1f / 6f))
		{
			if (!(t < 0.5f))
			{
				if (t < 2f / 3f)
				{
					return p + (q - p) * (2f / 3f - t) * 6f;
				}
				return p;
			}
			return q;
		}
		return p + (q - p) * 6f * t;
	}

	private unsafe static bool TryGetCameraPosition(out Vector3 position)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		position = default(Vector3);
		CameraManager* ptr = CameraManager.Instance();
		if (ptr == null)
		{
			return false;
		}
		Camera* camera = ((CameraManager)ptr).Camera;
		if (camera == null)
		{
			return false;
		}
		position = Vector3.op_Implicit(((Camera)(&((CameraBase)(&((Camera)camera).CameraBase)).SceneCamera)).Position);
		return true;
	}

	private static Vector2[] BuildUnitCircleOffsets()
	{
		Vector2[] array = new Vector2[64];
		for (int i = 0; i < 64; i++)
		{
			float x = (float)Math.PI * 2f * (float)i / 64f;
			array[i] = new Vector2(MathF.Cos(x), MathF.Sin(x));
		}
		return array;
	}
}
