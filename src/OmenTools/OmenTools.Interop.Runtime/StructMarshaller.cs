using System;
using System.Runtime.InteropServices;

namespace OmenTools.Interop.Runtime;

public static class StructMarshaller
{
	public static T FromBytes<T>(byte[] bytes) where T : struct
	{
		int num = Marshal.SizeOf<T>();
		if (bytes.Length < num)
		{
			throw new ArgumentException("字节长度不足以读取 " + typeof(T).Name + "。", "bytes");
		}
		nint num2 = Marshal.AllocHGlobal(num);
		try
		{
			Marshal.Copy(bytes, 0, num2, num);
			return Marshal.PtrToStructure<T>(num2);
		}
		finally
		{
			Marshal.FreeHGlobal(num2);
		}
	}

	public static byte[] ToBytes<T>(T value) where T : struct
	{
		int num = Marshal.SizeOf<T>();
		nint num2 = Marshal.AllocHGlobal(num);
		try
		{
			Marshal.StructureToPtr(value, num2, fDeleteOld: false);
			byte[] array = new byte[num];
			Marshal.Copy(num2, array, 0, num);
			return array;
		}
		finally
		{
			Marshal.FreeHGlobal(num2);
		}
	}
}
