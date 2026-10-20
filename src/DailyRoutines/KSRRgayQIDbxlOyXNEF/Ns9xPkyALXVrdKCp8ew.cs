using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using DailyRoutines;
using Newtonsoft.Json;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Interop.Game.Lumina;
using itAnDoW4mgYVlrmtMor;
using meYPMtyGJFBR6iuElOf;

namespace KSRRgayQIDbxlOyXNEF;

internal static class Ns9xPkyALXVrdKCp8ew
{
	private const string AuthKey = "aLYVEA5f44GQ2wJ4mP9aBaXrSu4f2NVeJ123";

	internal static string XxRyJiHmUK(string credential)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return string.Empty;
		}
		string s = JsonConvert.SerializeObject((object)new
		{
			Credential = credential,
			Time = DateTimeOffset.Now.ToUnixTimeSeconds(),
			PlayerName = localPlayer.Name + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28170D60 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd) + LuminaWrapper.GetWorldName(localPlayer.HomeWorld.RowId),
			Version = Plugin.Version.ToString()
		});
		using Aes aes = M5PUrayrrH8jqixraQZ.k1Ty6of4o5(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BB34F3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
		aes.GenerateIV();
		byte[] array;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			cryptoStream.Write(bytes, 0, bytes.Length);
			cryptoStream.FlushFinalBlock();
			array = memoryStream.ToArray();
		}
		byte[] array2 = new byte[aes.IV.Length + array.Length];
		Buffer.BlockCopy(aes.IV, 0, array2, 0, aes.IV.Length);
		Buffer.BlockCopy(array, 0, array2, aes.IV.Length, array.Length);
		return Convert.ToBase64String(array2);
	}
}
