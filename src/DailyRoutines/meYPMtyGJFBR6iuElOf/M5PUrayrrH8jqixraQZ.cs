using System;
using System.Security.Cryptography;
using System.Text;

namespace meYPMtyGJFBR6iuElOf;

internal static class M5PUrayrrH8jqixraQZ
{
	internal static Aes k1Ty6of4o5(string key)
	{
		Aes aes = Aes.Create();
		aes.Key = tXmykiopAP(key);
		aes.Padding = PaddingMode.PKCS7;
		return aes;
	}

	private static byte[] tXmykiopAP(string key)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] array = new byte[32];
		Buffer.BlockCopy(bytes, 0, array, 0, Math.Min(bytes.Length, 32));
		return array;
	}
}
