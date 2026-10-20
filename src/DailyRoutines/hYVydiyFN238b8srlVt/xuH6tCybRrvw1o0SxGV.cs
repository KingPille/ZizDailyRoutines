using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using Newtonsoft.Json;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using acXh6NyXo8Om6HO9yLt;
using itAnDoW4mgYVlrmtMor;
using meYPMtyGJFBR6iuElOf;

namespace hYVydiyFN238b8srlVt;

internal class xuH6tCybRrvw1o0SxGV : NRm8GoyuwLfFLeEjeU6
{
	internal static class sPOxEPCGs3C1ZYyiIBp
	{
		private const string PASS_AUTH_KEY = "k7PdSgUkRzCv4aQn9qWXcYbZ8yF6jMhN2pTlLrEeBtHvOxKs0";

		internal static bool FucC658LfR(string encryptedToken)
		{
			try
			{
				byte[] array = Convert.FromBase64String(encryptedToken);
				using Aes aes = M5PUrayrrH8jqixraQZ.k1Ty6of4o5(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B305644 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
				byte[] array2 = new byte[aes.BlockSize / 8];
				byte[] array3 = new byte[array.Length - array2.Length];
				Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
				Buffer.BlockCopy(array, array2.Length, array3, 0, array3.Length);
				aes.IV = array2;
				string text;
				using (MemoryStream stream = new MemoryStream(array3))
				{
					using CryptoStream stream2 = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Read);
					using StreamReader streamReader = new StreamReader(stream2, Encoding.UTF8);
					text = streamReader.ReadToEnd();
				}
				var anon = JsonConvert.DeserializeAnonymousType(text, new
				{
					StartTime = 0L,
					Duration = 0L
				});
				if (anon == null)
				{
					return false;
				}
				DateTime dateTime = new DateTime(anon.StartTime, DateTimeKind.Utc);
				TimeSpan timeSpan = TimeSpan.FromMilliseconds(anon.Duration);
				DateTime dateTime2 = dateTime + timeSpan;
				DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
				return uTCNow >= dateTime && uTCNow <= dateTime2;
			}
			catch
			{
				return false;
			}
		}
	}

	private const string URL = "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines.Announcement/refs/heads/main/PassAuthInfo.txt";

	private readonly CancellationTokenSource cancelSource;

	internal bool IsAllowToPassAuth { get; private set; }

	internal override void Init()
	{
	}

	internal void otmyqw1iYH(bool isAllowToPassAuth)
	{
		IsAllowToPassAuth = isAllowToPassAuth;
	}

	internal static async Task<bool> lniycReZTF()
	{
		try
		{
			OmenServiceBase<HTTPClientHelper>.Instance().Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CC8D55 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
			GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BA84D9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503);
			throw null;
		}
		catch
		{
			return false;
		}
	}

	internal override void Uninit()
	{
		otmyqw1iYH(isAllowToPassAuth: false);
		cancelSource.Cancel();
		cancelSource.Dispose();
	}

	public xuH6tCybRrvw1o0SxGV()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		cancelSource = new CancellationTokenSource();
		base._002Ector();
	}
}
