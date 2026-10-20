using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Lumina.Excel.Sheets;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic;

public class AutoConvertMapLink : ModuleBase
{
	private delegate nint MessageParseDelegate(nint a, nint b);

	private class PreMapLinkPayload(uint zoneID, uint mapID, int rawX, int rawY) : Payload
	{
		private const int RAW_Z = -30000;

		public override PayloadType Type => (PayloadType)8;

		protected override byte[] EncodeImpl()
		{
			byte[] array = Payload.MakeInteger(zoneID);
			byte[] array2 = Payload.MakeInteger(mapID);
			byte[] array3 = Payload.MakeInteger((uint)rawX);
			byte[] array4 = Payload.MakeInteger((uint)rawY);
			byte[] array5 = Payload.MakeInteger(4294937296u);
			int num = 4 + array.Length + array2.Length + array3.Length + array4.Length + array5.Length;
			List<byte> list = new List<byte>();
			list.Add(2);
			list.Add(46);
			list.Add((byte)num);
			list.Add(201);
			list.Add(4);
			list.AddRange(array);
			list.AddRange(array2);
			list.AddRange(array3);
			list.AddRange(array4);
			list.AddRange(array5);
			list.Add(1);
			list.Add(3);
			return list.ToArray();
		}

		protected override void DecodeImpl(BinaryReader reader, long endOfStream)
		{
			throw new NotImplementedException();
		}
	}

	private static readonly CompSig MessageParseSig = new CompSig("E8 ?? ?? ?? ?? 48 8B D0 48 8D 4D D0 E8 ?? ?? ?? ?? 49 8B 07");

	private Hook<MessageParseDelegate>? MessageParseHook;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoConvertMapLinkTitle"),
		Description = LanguageManager.Get("AutoConvertMapLinkDescription"),
		Category = ModuleCategory.System,
		Author = new string[1] { "KirisameVanilla" }
	};

	protected override void Init()
	{
		if (MessageParseHook == null)
		{
			MessageParseHook = MessageParseSig.GetHook<MessageParseDelegate>(ParseMessageDetour);
		}
		MessageParseHook.Enable();
	}

	private nint ParseMessageDetour(nint a, nint b)
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Expected O, but got Unknown
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Expected O, but got Unknown
		nint num = MessageParseHook.Original(a, b);
		try
		{
			nint num2 = Marshal.ReadIntPtr(num);
			int i;
			for (i = 0; Marshal.ReadByte(num2, i) != 0; i++)
			{
			}
			byte[] array = new byte[i];
			Marshal.Copy(num2, array, 0, i);
			SeString val = SeString.Parse(array);
			foreach (Payload payload in val.Payloads)
			{
				AutoTranslatePayload val2 = (AutoTranslatePayload)(object)((payload is AutoTranslatePayload) ? payload : null);
				if (val2 != null && ((Payload)val2).Encode(false)[3] == 201 && ((Payload)val2).Encode(false)[4] == 4)
				{
					return num;
				}
			}
			for (int j = 0; j < val.Payloads.Count; j++)
			{
				Payload obj = val.Payloads[j];
				TextPayload val3 = (TextPayload)(object)((obj is TextPayload) ? obj : null);
				if (val3 == null)
				{
					continue;
				}
				Match match = MapLinkRegex().Match(val3.Text);
				if (!match.Success)
				{
					continue;
				}
				string mapName = match.Groups["map"].Value;
				TerritoryType val4 = Sheets.Zones.Values.FirstOrDefault(delegate(TerritoryType x)
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000a: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					PlaceName value = ((TerritoryType)(ref x)).PlaceName.Value;
					return ((object)((PlaceName)(ref value)).Name/*cast due to constrained. prefix*/).ToString() == mapName;
				});
				if (((TerritoryType)(ref val4)).RowId == 0)
				{
					continue;
				}
				uint rowId = ((TerritoryType)(ref val4)).RowId;
				uint num3 = ((TerritoryType)(ref val4)).Map.RowId;
				uint zoneID = rowId;
				if (LuminaGetter.TryGetRow<Map>(num3, out Map item))
				{
					int rawX = GenerateRawPosition(float.Parse(match.Groups["x"].Value, CultureInfo.InvariantCulture), ((Map)(ref item)).OffsetX, ((Map)(ref item)).SizeFactor);
					int rawY = GenerateRawPosition(float.Parse(match.Groups["y"].Value, CultureInfo.InvariantCulture), ((Map)(ref item)).OffsetY, ((Map)(ref item)).SizeFactor);
					if (match.Groups["instance"].Value != "")
					{
						num3 |= (uint)(match.Groups["instance"].Value[0] - 57520 << 16);
					}
					List<Payload> list = new List<Payload>();
					if (match.Index > 0)
					{
						list.Add((Payload)new TextPayload(val3.Text.Substring(0, match.Index)));
					}
					list.Add((Payload)(object)new PreMapLinkPayload(zoneID, num3, rawX, rawY));
					if (match.Index + match.Length < val3.Text.Length)
					{
						list.Add((Payload)new TextPayload(val3.Text.Substring(match.Index + match.Length)));
					}
					val.Payloads.RemoveAt(j);
					val.Payloads.InsertRange(j, list);
					byte[] array2 = val.Encode();
					long num4 = Marshal.ReadInt64(num + 8);
					if (array2.Length + 1 > num4)
					{
						return num;
					}
					Marshal.WriteInt64(num + 16, array2.Length + 1);
					Marshal.Copy(array2, 0, num2, array2.Length);
					Marshal.WriteByte(num2, array2.Length, 0);
					break;
				}
			}
		}
		catch
		{
		}
		return num;
	}

	private static int GenerateRawPosition(float visibleCoordinate, short offset, ushort factor)
	{
		visibleCoordinate += (float)Random.Shared.NextDouble() * 0.07f;
		float num = (float)(int)factor / 100f;
		return (int)Math.Ceiling(((visibleCoordinate - 1f) * num / 41f * 2048f - 1024f) / num - (float)offset) * 1000;
	}

	[GeneratedRegex("\\uE0BB(?<map>.+?)(?<instance>[\\ue0b1-\\ue0b9])? \\( (?<x>\\d{1,2}\\.\\d)  , (?<y>\\d{1,2}\\.\\d) \\)", RegexOptions.Compiled)]
	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	private static Regex MapLinkRegex()
	{
		return _003CRegexGenerator_g_003EFDD7C65F98B3009022F7AC561E347A370A49D71D176A01AA07C89E66F21EBD08F__MapLinkRegex_3.Instance;
	}
}
