using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace OmenTools.Extensions;

public static class EnumExtension
{
	extension<T>(T value) where T : struct, Enum
	{
		public bool IsSet(T flag)
		{
			ulong num = ToUInt64(value);
			ulong num2 = ToUInt64(flag);
			return (num & num2) == num2;
		}

		public bool IsSetAny(params ReadOnlySpan<T> flags)
		{
			if (flags.IsEmpty)
			{
				return false;
			}
			ulong num = ToUInt64(value);
			ulong num2 = 0uL;
			ReadOnlySpan<T> readOnlySpan = flags;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				T origValue = readOnlySpan[i];
				num2 |= ToUInt64(origValue);
			}
			return (num & num2) != 0;
		}

		public bool IsSetAll(params ReadOnlySpan<T> flags)
		{
			if (flags.IsEmpty)
			{
				return true;
			}
			ulong num = ToUInt64(value);
			ulong num2 = 0uL;
			ReadOnlySpan<T> readOnlySpan = flags;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				T origValue = readOnlySpan[i];
				num2 |= ToUInt64(origValue);
			}
			return (num & num2) == num2;
		}

		public T Add(T flag)
		{
			return FromUInt64<T>(ToUInt64(value) | ToUInt64(flag));
		}

		public T Add(params ReadOnlySpan<T> flags)
		{
			if (flags.IsEmpty)
			{
				return value;
			}
			ulong num = ToUInt64(value);
			ReadOnlySpan<T> readOnlySpan = flags;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				T origValue = readOnlySpan[i];
				num |= ToUInt64(origValue);
			}
			return FromUInt64<T>(num);
		}

		public T Remove(T flag)
		{
			return FromUInt64<T>(ToUInt64(value) & ~ToUInt64(flag));
		}

		public T Remove(params ReadOnlySpan<T> flags)
		{
			if (flags.IsEmpty)
			{
				return value;
			}
			ulong num = ToUInt64(value);
			ulong num2 = 0uL;
			ReadOnlySpan<T> readOnlySpan = flags;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				T origValue = readOnlySpan[i];
				num2 |= ToUInt64(origValue);
			}
			return FromUInt64<T>(num & ~num2);
		}

		private static ulong ToUInt64(T origValue)
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				return Unsafe.As<T, byte>(ref origValue);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				return Unsafe.As<T, ushort>(ref origValue);
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				return Unsafe.As<T, uint>(ref origValue);
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				return Unsafe.As<T, ulong>(ref origValue);
			}
			throw new UnreachableException();
		}

		private static T FromUInt64(ulong rawValue)
		{
			if (Unsafe.SizeOf<T>() == 1)
			{
				byte source = (byte)rawValue;
				return Unsafe.As<byte, T>(ref source);
			}
			if (Unsafe.SizeOf<T>() == 2)
			{
				ushort source2 = (ushort)rawValue;
				return Unsafe.As<ushort, T>(ref source2);
			}
			if (Unsafe.SizeOf<T>() == 4)
			{
				uint source3 = (uint)rawValue;
				return Unsafe.As<uint, T>(ref source3);
			}
			if (Unsafe.SizeOf<T>() == 8)
			{
				ulong source4 = rawValue;
				return Unsafe.As<ulong, T>(ref source4);
			}
			throw new UnreachableException();
		}
	}

	private static readonly ConcurrentDictionary<Enum, string> DescriptionAttributeCache = new ConcurrentDictionary<Enum, string>();

	public static string GetDescription(this Enum value)
	{
		return DescriptionAttributeCache.GetOrAdd(value, delegate(Enum v)
		{
			FieldInfo field = v.GetType().GetField(v.ToString());
			return (!(field == null)) ? (((DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)))?.Description ?? v.ToString()) : v.ToString();
		});
	}
}
