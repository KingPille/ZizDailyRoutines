using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Game;
using Dalamud.Game.Text;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Jeffijoe.MessageFormat;
using Lumina.Data;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace OmenTools.Extensions;

public static class SeStringExtension
{
	extension<T>(T value) where T : INumber<_0024T0>
	{
		public string ToSEHexCount(string? format = null)
		{
			string text = value.ToString(format, CultureInfo.InvariantCulture);
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return string.Create(text.Length, text, delegate(Span<char> span, string state)
			{
				for (int i = 0; i < state.Length; i++)
				{
					char c = state[i];
					if (char.IsAsciiDigit(c))
					{
						span[i] = (char)(57520 + (c - 48));
					}
					else
					{
						span[i] = c;
					}
				}
			});
		}

		public string ToSESmallCount(string? format = null)
		{
			string text = value.ToString(format, CultureInfo.InvariantCulture);
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return string.Create(text.Length, text, delegate(Span<char> span, string state)
			{
				for (int i = 0; i < state.Length; i++)
				{
					char c = state[i];
					if (char.IsAsciiDigit(c))
					{
						span[i] = (char)(57440 + (c - 48));
					}
					else
					{
						span[i] = c;
					}
				}
			});
		}

		public string ToSESquareCount(string? format = null)
		{
			string text = value.ToString(format, CultureInfo.InvariantCulture);
			if (string.IsNullOrEmpty(text))
			{
				return string.Empty;
			}
			return string.Create(text.Length, text, delegate(Span<char> span, string state)
			{
				for (int i = 0; i < state.Length; i++)
				{
					char c = state[i];
					if (char.IsAsciiDigit(c))
					{
						span[i] = (char)(57487 + (c - 48));
					}
					else
					{
						span[i] = c;
					}
				}
			});
		}
	}

	extension(SeStringBuilder b)
	{
		public SeStringBuilder AppendDalamudSeString(SeString value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return b.Append(new ReadOnlySeString(value.Encode()));
		}

		public SeStringBuilder AppendDalamudSeString(SeStringBuilder value)
		{
			return b.Append(value.Build().Encode());
		}

		public SeStringBuilder AppendDalamudSeString(Payload value)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return b.Append(new ReadOnlySeString(new SeString((Payload[])(object)new Payload[1] { value }).Encode()));
		}

		public SeStringBuilder AppendRentedSeStringBuilder(RentedSeStringBuilder value)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return b.Append(((RentedSeStringBuilder)(ref value)).Builder.ToReadOnlySeString());
		}

		public SeStringBuilder AppendIcon(BitmapFontIcon icon)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected I4, but got Unknown
			return b.AppendIcon((uint)(int)icon);
		}

		public SeStringBuilder AppendIcon(SeIconChar icon)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return b.Append(SeIconCharExtensions.ToIconString(icon));
		}

		public SeStringBuilder AppendFormattable(IFormattable value, IFormatProvider provider)
		{
			return b.Append(value.ToString(null, provider) ?? string.Empty);
		}
	}

	extension(SeStringBuilder b)
	{
		public SeStringBuilder AddRange(IEnumerable<Payload> payloads)
		{
			foreach (Payload payload in payloads)
			{
				b = b.Add(payload);
			}
			return b;
		}
	}

	extension(ReadOnlySeString)
	{
		public unsafe static ReadOnlySeString CreateAchievementLink(uint achievementID, ReadOnlySeString? displayNameOverride = null)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			if (!LuminaGetter.TryGetRow<Achievement>(achievementID, out Achievement item))
			{
				throw new InvalidDataException("无效的 Achievement。");
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				ReadOnlySeString val2 = (ReadOnlySeString)(((_003F?)displayNameOverride) ?? ((Achievement)(ref item)).Name);
				ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				SeStringParameter reference = SeStringParameter.op_Implicit(val2);
				val2 = obj.EvaluateFromAddon(371u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
				((RentedSeStringBuilder)(ref val)).PushLinkAchievement(achievementID).Append(val2).PopLink();
				return ((RentedSeStringBuilder)(ref val)).ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}

		public unsafe static ReadOnlySeString CreateMapLink(Vector3 worldPosition, uint territoryType = 0u, uint map = 0u, ReadOnlySeString? displayNameOverride = null)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			if (territoryType == 0)
			{
				territoryType = GameState.TerritoryType;
			}
			if (!LuminaGetter.TryGetRow<TerritoryType>(territoryType, out TerritoryType item))
			{
				throw new InvalidDataException("无效的 TerritoryType。");
			}
			if (map == 0)
			{
				map = ((territoryType == GameState.TerritoryType) ? GameState.Map : ((TerritoryType)(ref item)).Map.RowId);
			}
			if (!LuminaGetter.TryGetRow<Map>(map, out Map item2))
			{
				throw new InvalidDataException("无效的 Map。");
			}
			if (((Map)(ref item2)).TerritoryType.RowId != territoryType)
			{
				throw new InvalidDataException("Map 对应的 TerritoryType 与传入的 TerritoryType 不一致。");
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				Vector2 vector = PositionHelper.WorldToMap(worldPosition, item2);
				ReadOnlySeString val2;
				if (displayNameOverride.HasValue)
				{
					val2 = displayNameOverride.GetValueOrDefault();
				}
				else
				{
					val2 = ReadOnlySeString.op_Implicit(item.ExtractPlaceName());
					if (territoryType == GameState.TerritoryType && InstancesManager.IsInstancedArea)
					{
						SeStringBuilder val3 = ((RentedSeStringBuilder)(ref val)).Append(val2).Append(InstancesManager.CurrentInstance.ToSEHexCount());
						SeStringInterpolatedStringHandler val4 = new SeStringInterpolatedStringHandler(9, 2, val3);
						((SeStringInterpolatedStringHandler)(ref val4)).AppendLiteral(" ( ");
						((SeStringInterpolatedStringHandler)(ref val4)).AppendFormatted(vector.X, "F1");
						((SeStringInterpolatedStringHandler)(ref val4)).AppendLiteral("  , ");
						((SeStringInterpolatedStringHandler)(ref val4)).AppendFormatted(vector.Y, "F1");
						((SeStringInterpolatedStringHandler)(ref val4)).AppendLiteral(" )");
						val3.Append(val4);
						val2 = ((RentedSeStringBuilder)(ref val)).ToReadOnlySeString();
						((RentedSeStringBuilder)(ref val)).Clear(false);
					}
				}
				ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				SeStringParameter reference = SeStringParameter.op_Implicit(val2);
				val2 = obj.EvaluateFromAddon(371u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
				((RentedSeStringBuilder)(ref val)).PushLinkMapPosition(territoryType, map, (int)(worldPosition.X * 1000f), (int)(worldPosition.Z * 1000f)).Append(val2).PopLink();
				return ((RentedSeStringBuilder)(ref val)).ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}

		public static ReadOnlySeString CreateItemLink(uint itemID, bool isHQ = false, ReadOnlySeString? displayNameOverride = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return CreateItemLink(itemID, (ItemKind)(isHQ ? 1000000 : 0), displayNameOverride);
		}

		public unsafe static ReadOnlySeString CreateItemLink(uint itemID, ItemKind kind = (ItemKind)0u, ReadOnlySeString? displayNameOverride = null)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			uint rawId = ItemUtil.GetRawId(itemID, kind);
			ReadOnlySeString val = (ReadOnlySeString)(((_003F?)displayNameOverride) ?? ItemUtil.GetItemName(rawId, true, (ClientLanguage?)null));
			if (((ReadOnlySeString)(ref val)).IsEmpty)
			{
				throw new Exception("无法确定物品名称。");
			}
			uint itemRarityColorType = ItemUtil.GetItemRarityColorType(rawId, false);
			uint num = itemRarityColorType + 1;
			RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val2))._002Ector();
			try
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val2)).Builder;
				ReadOnlySeString val3 = ((RentedSeStringBuilder)(ref val2)).Builder.PushColorType(itemRarityColorType).PushEdgeColorType(num).Append(val)
					.PopEdgeColorType()
					.PopColorType()
					.ToReadOnlySeString();
				ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				SeStringParameter reference = SeStringParameter.op_Implicit(val3);
				val3 = obj.EvaluateFromAddon(371u, new Span<SeStringParameter>(ref reference), (ClientLanguage?)null);
				builder.Clear(false);
				builder.PushLinkItem(itemID).Append(val3).PopLink();
				return builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
			}
		}

		public static ReadOnlySeString CreateItemName(uint itemID, bool isHQ = false, ReadOnlySeString? displayNameOverride = null)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return CreateItemName(itemID, (ItemKind)(isHQ ? 1000000 : 0), displayNameOverride);
		}

		public unsafe static ReadOnlySeString CreateItemName(uint itemID, ItemKind kind = (ItemKind)0u, ReadOnlySeString? displayNameOverride = null)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			uint rawId = ItemUtil.GetRawId(itemID, kind);
			ReadOnlySeString val = (ReadOnlySeString)(((_003F?)displayNameOverride) ?? ItemUtil.GetItemName(rawId, true, (ClientLanguage?)null));
			if (((ReadOnlySeString)(ref val)).IsEmpty)
			{
				throw new Exception("无法确定物品名称。");
			}
			uint itemRarityColorType = ItemUtil.GetItemRarityColorType(rawId, false);
			uint num = itemRarityColorType + 1;
			RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val2))._002Ector();
			try
			{
				return ((RentedSeStringBuilder)(ref val2)).Builder.PushColorType(itemRarityColorType).PushEdgeColorType(num).Append(val)
					.PopEdgeColorType()
					.PopColorType()
					.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
			}
		}

		public static ReadOnlySeString Format(string value, params object[] args)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return Format(value, CultureInfo.CurrentCulture, args, null);
		}

		public unsafe static ReadOnlySeString Format(string value, IReadOnlyDictionary<string, object> args, Language language, Action<string>? onFormatError = null)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			ArgumentNullException.ThrowIfNull(args, "args");
			CultureInfo cultureInfo = ResolveCulture(language);
			Dictionary<string, object> dictionary = null;
			Dictionary<string, object> dictionary2 = null;
			string key;
			object value2;
			foreach (KeyValuePair<string, object> arg in args)
			{
				arg.Deconstruct(out key, out value2);
				string key2 = key;
				object value3 = value2;
				if (IsSeStringArgument(value3))
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<string, object>(StringComparer.Ordinal);
					}
					if (dictionary2 == null)
					{
						dictionary2 = new Dictionary<string, object>(StringComparer.Ordinal);
					}
					string text = $"<se:{dictionary.Count}>";
					dictionary[text] = value3;
					dictionary2[key2] = text;
				}
			}
			if (dictionary2 != null)
			{
				foreach (KeyValuePair<string, object> arg2 in args)
				{
					arg2.Deconstruct(out key, out value2);
					string key3 = key;
					object value4 = value2;
					dictionary2.TryAdd(key3, value4);
				}
			}
			string text2;
			try
			{
				IReadOnlyDictionary<string, object> readOnlyDictionary = dictionary2;
				text2 = MessageFormatter.Format(value, readOnlyDictionary ?? args, cultureInfo);
			}
			catch (Exception ex)
			{
				onFormatError?.Invoke(ex.Message);
				return ReadOnlySeString.op_Implicit(value);
			}
			if (dictionary == null)
			{
				return ReadOnlySeString.op_Implicit(text2);
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				AppendWithSeStringArguments(((RentedSeStringBuilder)(ref val)).Builder, text2, dictionary, cultureInfo);
				return ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}

		public static string FormatText(string value, IReadOnlyDictionary<string, object> args, Language language, Action<string>? onFormatError = null)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			ReadOnlySeString val = Format(value, args, language, onFormatError);
			return ((ReadOnlySeString)(ref val)).ExtractText();
		}

		public static ReadOnlySeString Format(string value, IFormatProvider? provider, params object[] args)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return Format(value, provider ?? CultureInfo.CurrentCulture, args, null);
		}

		internal unsafe static ReadOnlySeString Format(string value, IFormatProvider provider, object[] args, Action<string>? onFormatError)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			ArgumentNullException.ThrowIfNull(args, "args");
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				AppendFormattedText(((RentedSeStringBuilder)(ref val)).Builder, value, args, provider, onFormatError);
				return ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	extension(string input)
	{
		public string SanitizeSEIcon()
		{
			return input.AsSpan().SanitizeSEIcon();
		}
	}

	extension(ReadOnlySpan<char> input)
	{
		public string SanitizeSEIcon()
		{
			if (input.IsEmpty)
			{
				return string.Empty;
			}
			(int Start, int End, ulong[] Bitmap) value = SEIconBitmap.Value;
			int item = value.Start;
			int item2 = value.End;
			ulong[] item3 = value.Bitmap;
			Span<char> span = stackalloc char[input.Length];
			int length = 0;
			ReadOnlySpan<char> readOnlySpan = input;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				if (c < item || c > item2 || !c.IsSEIcon(item, item3))
				{
					span[length++] = c;
				}
			}
			return new string(span.Slice(0, length));
		}
	}

	extension(ReadOnlySeStringSpan span)
	{
		public unsafe ReadOnlySeString PraseAutoTranslate()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Invalid comparison between Unknown and I4
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Invalid comparison between Unknown and I4
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Invalid comparison between Unknown and I4
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
				int num = -1;
				Enumerator enumerator = ((ReadOnlySeStringSpan)(ref span)).GetEnumerator();
				RentedSeStringBuilder val2 = default(RentedSeStringBuilder);
				ReadOnlySeExpressionSpan val3 = default(ReadOnlySeExpressionSpan);
				ReadOnlySeExpressionSpan val4 = default(ReadOnlySeExpressionSpan);
				uint num2 = default(uint);
				uint rowID = default(uint);
				while (((Enumerator)(ref enumerator)).MoveNext())
				{
					ReadOnlySePayloadSpan current = ((Enumerator)(ref enumerator)).Current;
					num++;
					((RentedSeStringBuilder)(ref val2))._002Ector();
					try
					{
						if ((int)current.Type != 2 || (int)current.MacroCode != 46 || ((ReadOnlySePayloadSpan)(ref current)).ExpressionCount != 2 || !((ReadOnlySePayloadSpan)(ref current)).TryGetExpression(ref val3, ref val4) || !((ReadOnlySeExpressionSpan)(ref val3)).TryGetUInt(ref num2) || !((ReadOnlySeExpressionSpan)(ref val4)).TryGetUInt(ref rowID) || !LuminaGetter.TryGetRow<Completion>(rowID, out Completion item) || ((Completion)(ref item)).Group != num2 + 1)
						{
							if (num != 0 || (int)current.Type != 1 || !string.IsNullOrEmpty(((object)(*(ReadOnlySePayloadSpan*)(&current))/*cast due to constrained. prefix*/).ToString().Trim()))
							{
								builder.Append(((RentedSeStringBuilder)(ref val2)).Builder.Append(current).ToReadOnlySeString());
							}
						}
						else
						{
							builder.Append(current);
						}
					}
					finally
					{
						((IDisposable)(*(RentedSeStringBuilder*)(&val2))/*cast due to constrained. prefix*/).Dispose();
					}
				}
				return builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	extension(char c)
	{
		private bool IsSEIcon(int start, ulong[] bitmap)
		{
			int num = c - start;
			int num2 = num >> 6;
			int num3 = num & 0x3F;
			if (num2 < bitmap.Length)
			{
				return (bitmap[num2] & (ulong)(1L << num3)) != 0;
			}
			return false;
		}
	}

	private const char SE_SQUARE_COUNT_BASE_CHAR = '\ue08f';

	private const char SE_SMALL_COUNT_BASE_CHAR = '\ue060';

	private const char SE_HEX_COUNT_BASE_CHAR = '\ue0b0';

	private static readonly Lazy<(int Start, int End, ulong[] Bitmap)> SEIconBitmap = new Lazy<(int, int, ulong[])>(delegate
	{
		List<int> list = (from i in Enum.GetValues<SeIconChar>()
			select (int)i).ToList();
		int num = list.Min();
		int num2 = list.Max();
		ulong[] array = new ulong[(num2 - num + 1 + 63) / 64];
		foreach (int item in list)
		{
			int num3 = item - num;
			int num4 = num3 >> 6;
			int num5 = num3 & 0x3F;
			array[num4] |= (ulong)(1L << num5);
		}
		return (Start: num, End: num2, Bitmap: array);
	});

	private static bool IsSeStringArgument(object? value)
	{
		if (value is SeString || value is SeStringBuilder || value is Payload || value is ReadOnlySeString || value is ReadOnlySePayload || value is RentedSeStringBuilder)
		{
			return true;
		}
		return false;
	}

	private static CultureInfo ResolveCulture(Language language)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected I4, but got Unknown
		return (language - 1) switch
		{
			0 => CultureInfo.GetCultureInfo("ja-JP"), 
			1 => CultureInfo.GetCultureInfo("en-US"), 
			2 => CultureInfo.GetCultureInfo("de-DE"), 
			3 => CultureInfo.GetCultureInfo("fr-FR"), 
			4 => CultureInfo.GetCultureInfo("zh-CN"), 
			7 => CultureInfo.GetCultureInfo("zh-TW"), 
			6 => CultureInfo.GetCultureInfo("ko-KR"), 
			_ => CultureInfo.InvariantCulture, 
		};
	}

	private static void AppendWithSeStringArguments(SeStringBuilder builder, string message, Dictionary<string, object> restored, IFormatProvider provider)
	{
		int num = 0;
		while (num < message.Length)
		{
			int num2 = message.IndexOf("<se:", num, StringComparison.Ordinal);
			if (num2 < 0)
			{
				builder.Append(message.AsSpan(num));
				break;
			}
			int num3 = message.IndexOf('>', num2);
			if (num3 < 0)
			{
				builder.Append(message.AsSpan(num));
				break;
			}
			int num4 = num2;
			string key = message.Substring(num4, num3 + 1 - num4);
			if (!restored.TryGetValue(key, out object value))
			{
				builder.Append(message.AsSpan(num, num3 + 1 - num));
				num = num3 + 1;
			}
			else
			{
				builder.Append(message.AsSpan(num, num2 - num));
				AppendFormattedArgument(builder, value, provider);
				num = num3 + 1;
			}
		}
	}

	private static void AppendFormattedText(SeStringBuilder builder, string text, object[] args, IFormatProvider provider, Action<string>? onFormatError)
	{
		int num = 0;
		while (num < text.Length)
		{
			char c = text[num];
			switch (c)
			{
			case '{':
				if (num + 1 < text.Length && text[num + 1] == '{')
				{
					builder.Append('{');
					num += 2;
					continue;
				}
				break;
			case '}':
				if (num + 1 < text.Length && text[num + 1] == '}')
				{
					builder.Append('}');
					num += 2;
					continue;
				}
				break;
			}
			if (c != '{' || !TryReadPlaceholder(text, num, out var nextIndex, out var argumentIndex))
			{
				builder.Append(c.ToString());
				num++;
				continue;
			}
			int num2 = num;
			string text2 = text.Substring(num2, nextIndex - num2);
			if ((uint)argumentIndex < (uint)args.Length)
			{
				AppendFormattedArgument(builder, args[argumentIndex], provider);
			}
			else
			{
				onFormatError?.Invoke(text2);
				builder.Append(text2);
			}
			num = nextIndex;
		}
	}

	internal static void AppendFormattedArgument(SeStringBuilder builder, object? value, IFormatProvider provider)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (value == null)
		{
			return;
		}
		SeString val = (SeString)((value is SeString) ? value : null);
		if (val == null)
		{
			SeStringBuilder val2 = (SeStringBuilder)((value is SeStringBuilder) ? value : null);
			if (val2 == null)
			{
				Payload val3 = (Payload)((value is Payload) ? value : null);
				if (val3 == null)
				{
					if (!(value is ReadOnlySeString val4))
					{
						if (!(value is ReadOnlySePayload val5))
						{
							if (!(value is RentedSeStringBuilder value2))
							{
								SeStringBuilder val6 = (SeStringBuilder)((value is SeStringBuilder) ? value : null);
								if (val6 == null)
								{
									if (!(value is BitmapFontIcon icon))
									{
										if (!(value is SeIconChar icon2))
										{
											if (value is IFormattable value3)
											{
												builder.AppendFormattable(value3, provider);
											}
											else
											{
												builder.Append(value.ToString() ?? string.Empty);
											}
										}
										else
										{
											builder.AppendIcon(icon2);
										}
									}
									else
									{
										builder.AppendIcon(icon);
									}
								}
								else
								{
									builder.Append(val6.ToReadOnlySeString());
								}
							}
							else
							{
								builder.AppendRentedSeStringBuilder(value2);
							}
						}
						else
						{
							builder.Append(val5);
						}
					}
					else
					{
						builder.Append(val4);
					}
				}
				else
				{
					builder.AppendDalamudSeString(val3);
				}
			}
			else
			{
				builder.AppendDalamudSeString(val2);
			}
		}
		else
		{
			builder.AppendDalamudSeString(val);
		}
	}

	private static bool TryReadPlaceholder(string format, int startIndex, out int nextIndex, out int argumentIndex)
	{
		nextIndex = startIndex;
		argumentIndex = -1;
		int i = startIndex + 1;
		if (i >= format.Length || !char.IsAsciiDigit(format[i]))
		{
			return false;
		}
		int num = 0;
		for (; i < format.Length && char.IsAsciiDigit(format[i]); i++)
		{
			num = num * 10 + format[i] - 48;
		}
		if (i >= format.Length || format[i] != '}')
		{
			return false;
		}
		argumentIndex = num;
		nextIndex = i + 1;
		return true;
	}
}
