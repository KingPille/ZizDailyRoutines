using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Dalamud.Utility;
using Lumina.Data;
using Lumina.Text;
using Lumina.Text.ReadOnly;
using OmenTools.OmenService;

namespace OmenTools.Extensions;

public static class NumericExtension
{
	extension<T>(T number) where T : IBinaryInteger<_0024T0>, IFormattable
	{
		public unsafe ReadOnlySeString ToChineseSeString(ushort? minusColor = null, ushort? unitColor = null)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Invalid comparison between Unknown and I4
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Expected I4, but got Unknown
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			if (!IsValidFormatLanguage(GameState.ClientLanguage) || T.IsZero(number))
			{
				return new ReadOnlySeString(number.ToString());
			}
			string text = "兆";
			string text2 = "亿";
			string text3 = "万";
			string text4 = "零";
			Language clientLanguage = GameState.ClientLanguage;
			if ((int)clientLanguage != 1)
			{
				switch (clientLanguage - 6)
				{
				case 0:
				case 2:
					text2 = "億";
					text3 = "萬";
					break;
				case 1:
					text = "조";
					text2 = "억";
					text3 = "만";
					text4 = "0";
					break;
				}
			}
			else
			{
				text2 = "億";
				text3 = "万";
				text4 = "0";
			}
			Int128 @int = Int128.CreateChecked(number);
			bool flag = @int < 0;
			Int128 int2 = Int128.Abs(@int);
			Int128 int3 = 1000000000000L;
			Int128 int4 = 100000000;
			Int128 int5 = 10000;
			Int128 int6 = 1000;
			Int128 int7 = int2 / int3;
			Int128 int8 = int2 % int3;
			Int128 int9 = int8 / int4;
			Int128 int10 = int8 % int4;
			Int128 int11 = int10 / int5;
			Int128 int12 = int10 % int5;
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
				if (flag)
				{
					if (minusColor.HasValue)
					{
						builder.PushColorType((uint)minusColor.Value);
					}
					builder.Append("-");
					if (minusColor.HasValue)
					{
						builder.PopColorType();
					}
				}
				bool flag2 = false;
				bool flag3 = false;
				if (int7 > 0)
				{
					builder.Append(int7.ToString());
					if (unitColor.HasValue)
					{
						builder.PushColorType((uint)unitColor.Value).Append(text).PopColorType();
					}
					else
					{
						builder.Append(text);
					}
					flag2 = true;
				}
				if (int9 > 0)
				{
					if (flag3 || (flag2 && int9 < int6))
					{
						builder.Append(text4);
					}
					builder.Append(int9.ToString());
					if (unitColor.HasValue)
					{
						builder.PushColorType((uint)unitColor.Value).Append(text2).PopColorType();
					}
					else
					{
						builder.Append(text2);
					}
					flag2 = true;
					flag3 = false;
				}
				else if (flag2)
				{
					flag3 = true;
				}
				if (int11 > 0)
				{
					if (flag3 || (flag2 && int11 < int6))
					{
						builder.Append(text4);
					}
					builder.Append(int11.ToString());
					if (unitColor.HasValue)
					{
						builder.PushColorType((uint)unitColor.Value).Append(text3).PopColorType();
					}
					else
					{
						builder.Append(text3);
					}
					flag2 = true;
					flag3 = false;
				}
				else if (flag2)
				{
					flag3 = true;
				}
				if (int12 > 0)
				{
					if (flag3 || (flag2 && int12 < int6))
					{
						builder.Append(text4);
					}
					builder.Append(int12.ToString());
				}
				return builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}

		public string ToChineseString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Invalid comparison between Unknown and I4
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Expected I4, but got Unknown
			if (!IsValidFormatLanguage(GameState.ClientLanguage) || T.IsZero(number))
			{
				return ((IFormattable)number/*cast due to constrained. prefix*/).ToString("N0", (IFormatProvider?)null);
			}
			char value = '兆';
			char value2 = '亿';
			char value3 = '万';
			char value4 = '零';
			Language clientLanguage = GameState.ClientLanguage;
			if ((int)clientLanguage != 1)
			{
				switch (clientLanguage - 6)
				{
				case 0:
				case 2:
					value2 = '億';
					value3 = '萬';
					break;
				case 1:
					value = '조';
					value2 = '억';
					value3 = '만';
					value4 = '0';
					break;
				}
			}
			else
			{
				value2 = '億';
				value3 = '万';
				value4 = '0';
			}
			Int128 @int = Int128.CreateChecked(number);
			bool flag = @int < 0;
			Int128 int2 = Int128.Abs(@int);
			Int128 int3 = 1000000000000L;
			Int128 int4 = 100000000;
			Int128 int5 = 10000;
			Int128 int6 = 1000;
			Int128 int7 = int2 / int3;
			Int128 int8 = int2 % int3;
			Int128 int9 = int8 / int4;
			Int128 int10 = int8 % int4;
			Int128 int11 = int10 / int5;
			Int128 int12 = int10 % int5;
			StringBuilder stringBuilder = new StringBuilder(64);
			if (flag)
			{
				stringBuilder.Append('-');
			}
			bool flag2 = false;
			bool flag3 = false;
			if (int7 > 0)
			{
				stringBuilder.Append(int7);
				stringBuilder.Append(value);
				flag2 = true;
			}
			if (int9 > 0)
			{
				if (flag3 || (flag2 && int9 < int6))
				{
					stringBuilder.Append(value4);
				}
				stringBuilder.Append(int9);
				stringBuilder.Append(value2);
				flag2 = true;
				flag3 = false;
			}
			else if (flag2)
			{
				flag3 = true;
			}
			if (int11 > 0)
			{
				if (flag3 || (flag2 && int11 < int6))
				{
					stringBuilder.Append(value4);
				}
				stringBuilder.Append(int11);
				stringBuilder.Append(value3);
				flag2 = true;
				flag3 = false;
			}
			else if (flag2)
			{
				flag3 = true;
			}
			if (int12 > 0)
			{
				if (flag3 || (flag2 && int12 < int6))
				{
					stringBuilder.Append(value4);
				}
				stringBuilder.Append(int12);
			}
			return stringBuilder.ToString();
		}
	}

	extension<T>(T number) where T : INumber<_0024T0>, IFormattable
	{
		[SkipLocalsInit]
		public string ToMyriadString()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			Span<char> destination = stackalloc char[128];
			if (!IsValidFormatLanguage(GameState.ClientLanguage) || !number.TryFormat(destination, out var charsWritten, null, CultureInfo.InvariantCulture))
			{
				return ((IFormattable)number/*cast due to constrained. prefix*/).ToString("N0", (IFormatProvider?)null) ?? string.Empty;
			}
			ReadOnlySpan<char> span = destination.Slice(0, charsWritten);
			int num = span.IndexOf('.');
			int num2 = ((span.Length > 0 && span[0] == '-') ? 1 : 0);
			int num3 = ((num >= 0) ? num : charsWritten);
			int num4 = num3 - num2;
			int num5 = ((num4 > 0) ? ((num4 - 1) / 4) : 0);
			return string.Create(charsWritten + num5, (number, charsWritten, num2, num3), delegate(Span<char> span2, (T number, int charsWritten, int isNegative, int integerEndIndex) state)
			{
				Span<char> destination2 = stackalloc char[128];
				state.number.TryFormat(destination2, out var charsWritten2, null, CultureInfo.InvariantCulture);
				ReadOnlySpan<char> readOnlySpan = destination2.Slice(0, charsWritten2);
				int num6 = readOnlySpan.Length - 1;
				int num7 = span2.Length - 1;
				int item = state.integerEndIndex;
				int item2 = state.isNegative;
				if (item < readOnlySpan.Length)
				{
					int num8 = readOnlySpan.Length - item;
					readOnlySpan.Slice(item, num8).CopyTo(span2.Slice(num7 - num8 + 1));
					num7 -= num8;
					num6 -= num8;
				}
				int num9 = 0;
				while (num6 >= item2)
				{
					span2[num7--] = readOnlySpan[num6--];
					num9++;
					if (num9 == 4 && num6 >= item2)
					{
						span2[num7--] = ',';
						num9 = 0;
					}
				}
				if (item2 > 0)
				{
					span2[num7] = '-';
				}
			});
		}
	}

	extension(string str)
	{
		public T FromChineseString<T>() where T : IBinaryInteger<T>
		{
			if (string.IsNullOrWhiteSpace(str))
			{
				return T.Zero;
			}
			ReadOnlySpan<char> readOnlySpan = str.AsSpan().Trim();
			if (readOnlySpan.IsEmpty)
			{
				return T.Zero;
			}
			bool flag = false;
			if (readOnlySpan[0] == '-')
			{
				flag = true;
				readOnlySpan = readOnlySpan.Slice(1);
			}
			Int128 @int = 0;
			int num = readOnlySpan.IndexOfAny('兆', '조');
			if (num >= 0)
			{
				@int += ParseSegment(readOnlySpan.Slice(0, num)) * 1000000000000L;
				readOnlySpan = readOnlySpan.Slice(num + 1);
			}
			num = readOnlySpan.IndexOfAny('亿', '億', '억');
			if (num >= 0)
			{
				@int += ParseSegment(readOnlySpan.Slice(0, num)) * 100000000;
				readOnlySpan = readOnlySpan.Slice(num + 1);
			}
			num = readOnlySpan.IndexOfAny('万', '萬', '만');
			if (num >= 0)
			{
				@int += ParseSegment(readOnlySpan.Slice(0, num)) * 10000;
				readOnlySpan = readOnlySpan.Slice(num + 1);
			}
			if (!readOnlySpan.IsEmpty)
			{
				@int += ParseSegment(readOnlySpan);
			}
			if (flag)
			{
				@int = -@int;
			}
			return T.CreateChecked(@int);
			static Int128 ParseSegment(ReadOnlySpan<char> s)
			{
				if (s.IsEmpty)
				{
					return 0;
				}
				int i;
				for (i = 0; i < s.Length && s[i] == '零'; i++)
				{
				}
				if (i == s.Length)
				{
					return 0;
				}
				if (!Int128.TryParse(s.Slice(i), NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
				{
					return Int128.Zero;
				}
				return result;
			}
		}
	}

	private static bool IsValidFormatLanguage(Language language)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)language == 1 || language - 5 <= 3)
		{
			return true;
		}
		return false;
	}
}
