using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;

namespace OmenTools.Extensions;

public static class ReflectionExtension
{
	extension<T>(scoped ref T vtable) where T : unmanaged
	{
		public unsafe nint GetVFuncByName(string fieldName)
		{
			fixed (T* ptr = &vtable)
			{
				ArgumentNullException.ThrowIfNull(ptr, "vtablePtr");
				Type typeFromHandle = typeof(T);
				FieldInfo? field = typeFromHandle.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
				if (field == null)
				{
					throw new MissingFieldException(typeFromHandle.FullName, fieldName);
				}
				int value = (field.GetCustomAttribute<FieldOffsetAttribute>() ?? throw new InvalidOperationException("字段 " + fieldName + " 无 FieldOffset 属性, 虚函数读取失败")).Value;
				return *(nint*)((byte*)ptr + value);
			}
		}

		public unsafe Hook<TDetour> HookVFuncFromName<TDetour>(string fieldName, TDetour detour) where TDetour : Delegate
		{
			fixed (T* ptr = &vtable)
			{
				ArgumentNullException.ThrowIfNull(ptr, "vtablePtr");
				return DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<TDetour>((IntPtr)GetVFuncByName(ref *ptr, fieldName), detour, (HookBackend)0);
			}
		}
	}

	extension(nint luaSetupFunctionStartAddress)
	{
		public nint GetLuaFunctionByName(string functionName, int scanSize = 8192)
		{
			if (luaSetupFunctionStartAddress == IntPtr.Zero || string.IsNullOrEmpty(functionName))
			{
				return IntPtr.Zero;
			}
			byte[] array = new byte[scanSize];
			try
			{
				Marshal.Copy(luaSetupFunctionStartAddress, array, 0, scanSize);
			}
			catch
			{
				return IntPtr.Zero;
			}
			byte[] array2 = new byte[3] { 76, 141, 5 };
			byte[] array3 = new byte[3] { 76, 141, 13 };
			int num = -1;
			for (int i = 0; i <= array.Length - 7; i++)
			{
				if (array[i] == array2[0] && array[i + 1] == array2[1] && array[i + 2] == array2[2])
				{
					int num2 = BitConverter.ToInt32(array, i + 3);
					if (Marshal.PtrToStringAnsi((nint)((long)luaSetupFunctionStartAddress + (long)i + 7 + num2)) == functionName)
					{
						num = i;
						break;
					}
				}
			}
			if (num == -1)
			{
				return IntPtr.Zero;
			}
			int num3 = Math.Max(0, num - 100);
			for (int num4 = num - 1; num4 >= num3; num4--)
			{
				if (num4 + 7 < array.Length && array[num4] == array3[0] && array[num4 + 1] == array3[1] && array[num4 + 2] == array3[2])
				{
					int num5 = BitConverter.ToInt32(array, num4 + 3);
					return (nint)((long)luaSetupFunctionStartAddress + (long)num4 + 7 + num5);
				}
			}
			return IntPtr.Zero;
		}
	}

	extension(object obj)
	{
		public object? GetFieldOrProperty(string name)
		{
			object obj2 = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(obj);
			if (obj2 == null)
			{
				PropertyInfo? property = obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if ((object)property == null)
				{
					return null;
				}
				obj2 = property.GetValue(obj);
			}
			return obj2;
		}

		public T? GetFieldOrProperty<T>(string name)
		{
			return (T)obj.GetFieldOrProperty(name);
		}

		public void SetFieldOrProperty(string name, object value)
		{
			FieldInfo field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				field.SetValue(obj, value);
			}
			else
			{
				obj.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(obj, value);
			}
		}

		public object? GetStaticFieldOrProperty(string type, string name)
		{
			object obj2 = obj.GetType().Assembly.GetType(type)?.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null);
			if (obj2 == null)
			{
				Type? type2 = obj.GetType().Assembly.GetType(type);
				if ((object)type2 == null)
				{
					return null;
				}
				PropertyInfo? property = type2.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				if ((object)property == null)
				{
					return null;
				}
				obj2 = property.GetValue(null);
			}
			return obj2;
		}

		public T? GetStaticFieldOrProperty<T>(string type, string name)
		{
			return (T)obj.GetStaticFieldOrProperty(type, name);
		}

		public void SetStaticFieldOrProperty(string type, string name, object value)
		{
			FieldInfo fieldInfo = obj.GetType().Assembly.GetType(type)?.GetField(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (fieldInfo != null)
			{
				fieldInfo.SetValue(null, value);
			}
			else
			{
				obj.GetType().Assembly.GetType(type)?.GetProperty(name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.SetValue(null, value);
			}
		}

		public object? Call(string name, object[] @params, bool matchExactArgumentTypes = false)
		{
			return ((!matchExactArgumentTypes) ? obj.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) : obj.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, @params.Select((object x) => x.GetType()).ToArray()))?.Invoke(obj, @params);
		}

		public T? Call<T>(string name, object[] @params, bool matchExactArgumentTypes = false)
		{
			return (T)obj.Call(name, @params, matchExactArgumentTypes);
		}
	}

	private const BindingFlags ALL_FLAGS = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	private const BindingFlags STATIC_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
}
