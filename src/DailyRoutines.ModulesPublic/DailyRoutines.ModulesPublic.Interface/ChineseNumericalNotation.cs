using System;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic.Interface;

public class ChineseNumericalNotation : ModuleBase
{
	private unsafe delegate Utf8String* FormatNumberDelegate(Utf8String* outNumberString, int number, int baseNumber, int mode, void* seperator);

	private unsafe delegate void AtkCounterNodeSetNumberDelegate(AtkCounterNode* node, CStringPointer number);

	private class Config : ModuleConfig
	{
		public bool ColoringUnit;

		public ushort ColorMinus = 17;

		public ushort ColorUnit = 25;

		public bool NoChineseUnit;
	}

	private static readonly CompSig FormatNumberSig = new CompSig("E8 ?? ?? ?? ?? 44 3B F7");

	private Hook<FormatNumberDelegate>? FormatNumberHook;

	private static readonly CompSig AtkCounterNodeSetNumberSig = new CompSig("40 53 48 83 EC ?? 48 8B C2 48 8B D9 48 85 C0");

	private Hook<AtkCounterNodeSetNumberDelegate>? AtkCounterNodeSetNumberHook;

	private MemoryPatch AtkTextNodeSetNumberCommaPatch;

	private Config config;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("ChineseNumericalNotationTitle"),
		Description = LanguageManager.Get("ChineseNumericalNotationDescription"),
		Category = ModuleCategory.Interface
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		CNDefaultEnabled = true,
		TCDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		AtkTextNodeSetNumberCommaPatch = new MemoryPatch("B8 ?? ?? ?? ?? F7 E1 D1 EA 8D 04 52 2B C8 83 F9 ?? 75 ?? 41 0F B6 D0 48 8D 8F", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[14]
		{
			(byte)131,
			(byte)225,
			(byte)3,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144,
			(byte)144
		}));
		AtkTextNodeSetNumberCommaPatch.Enable();
		if (AtkCounterNodeSetNumberHook == null)
		{
			AtkCounterNodeSetNumberHook = AtkCounterNodeSetNumberSig.GetHook<AtkCounterNodeSetNumberDelegate>(AtkCounterNodeSetNumberDetour);
		}
		AtkCounterNodeSetNumberHook.Enable();
		if (FormatNumberHook == null)
		{
			FormatNumberHook = FormatNumberSig.GetHook<FormatNumberDelegate>(FormatNumberDetour);
		}
		FormatNumberHook.Enable();
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("ChineseNumericalNotation-NoChineseUnit")), ref config.NoChineseUnit))
		{
			config.Save(this);
		}
		if (config.NoChineseUnit)
		{
			return;
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("Dye")), ref config.ColoringUnit))
		{
			config.Save(this);
		}
		if (!config.ColoringUnit)
		{
			return;
		}
		GroupDisposable val = ImRaii.Group();
		ImU8String val4;
		try
		{
			if (!LuminaGetter.TryGetRow<UIColor>((uint)config.ColorMinus, out UIColor item))
			{
				config.ColorMinus = 17;
				config.Save(this);
				return;
			}
			ImU8String val2 = ImU8String.op_Implicit("###ColorButtonMinus");
			Vector4 vector = item.ToVector4();
			ImGui.ColorButton(val2, ref vector, (ImGuiColorEditFlags)0, default(Vector2));
			ImGui.SameLine();
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			ImU8String val3 = ImU8String.op_Implicit(LanguageManager.Get("ChineseNumericalNotation-ColorMinus"));
			ref ushort colorMinus = ref config.ColorMinus;
			val4 = default(ImU8String);
			if (ImGui.InputUShort(val3, ref colorMinus, (ushort)1, (ushort)1, val4, (ImGuiInputTextFlags)0))
			{
				config.Save(this);
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImGui.SameLine();
		ImGui.TextDisabled(ImU8String.op_Implicit("|"));
		ImGui.SameLine();
		GroupDisposable val5 = ImRaii.Group();
		try
		{
			if (!LuminaGetter.TryGetRow<UIColor>((uint)config.ColorUnit, out UIColor item2))
			{
				config.ColorUnit = 17;
				config.Save(this);
				return;
			}
			ImU8String val6 = ImU8String.op_Implicit("###ColorButtonUnit");
			Vector4 vector = item2.ToVector4();
			ImGui.ColorButton(val6, ref vector, (ImGuiColorEditFlags)0, default(Vector2));
			ImGui.SameLine();
			ImGui.SetNextItemWidth(200f * Globals.GlobalUIScale);
			ImU8String val7 = ImU8String.op_Implicit(LanguageManager.Get("ChineseNumericalNotation-ColorUnit"));
			ref ushort colorUnit = ref config.ColorUnit;
			val4 = default(ImU8String);
			if (ImGui.InputUShort(val7, ref colorUnit, (ushort)1, (ushort)1, val4, (ImGuiInputTextFlags)0))
			{
				config.Save(this);
			}
		}
		finally
		{
			((GroupDisposable)(ref val5)).Dispose();
		}
		ExcelSheet<UIColor> val8 = LuminaGetter.Get<UIColor>();
		TreeNodeDisposable val9 = ImRaii.TreeNode(ImU8String.op_Implicit(LanguageManager.Get("ChineseNumericalNotation-ColorTable")));
		try
		{
			if (!TreeNodeDisposable.op_Implicit(val9))
			{
				return;
			}
			TableDisposable val10 = ImRaii.Table(ImU8String.op_Implicit("###ColorTable"), 6);
			try
			{
				if (!val10)
				{
					return;
				}
				int num = 0;
				Enumerator<UIColor> enumerator = val8.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						UIColor color = enumerator.Current;
						if (((UIColor)(ref color)).RowId != 0 && ((UIColor)(ref color)).Dark != 0)
						{
							if (num % 5 == 0)
							{
								ImGui.TableNextRow();
							}
							ImGui.TableNextColumn();
							num++;
							GroupDisposable val11 = ImRaii.Group();
							try
							{
								val4 = new ImU8String(19, 1);
								((ImU8String)(ref val4)).AppendLiteral("###ColorButtonTable");
								((ImU8String)(ref val4)).AppendFormatted<uint>(((UIColor)(ref color)).RowId);
								ImU8String val12 = val4;
								Vector4 vector = color.ToVector4();
								ImGui.ColorButton(val12, ref vector, (ImGuiColorEditFlags)0, default(Vector2));
								ImGui.SameLine();
								ImU8String val13 = new ImU8String(0, 1);
								((ImU8String)(ref val13)).AppendFormatted<uint>(((UIColor)(ref color)).RowId);
								ImGui.TextUnformatted(val13);
							}
							finally
							{
								((GroupDisposable)(ref val11)).Dispose();
							}
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
			}
			finally
			{
				((TableDisposable)(ref val10)).Dispose();
			}
		}
		finally
		{
			((TreeNodeDisposable)(ref val9)).Dispose();
		}
	}

	private unsafe Utf8String* FormatNumberDetour(Utf8String* outNumberString, int number, int baseNumber, int mode, void* seperator)
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		Utf8String* result = FormatNumberHook.Original(outNumberString, number, baseNumber, mode, seperator);
		if (baseNumber % 10 == 0)
		{
			switch (mode)
			{
			case 1:
			{
				ushort? minusColor = (config.ColoringUnit ? new ushort?(config.ColorMinus) : ((ushort?)null));
				ushort? unitColor = (config.ColoringUnit ? new ushort?(config.ColorUnit) : ((ushort?)null));
				ReadOnlySeString val = ((!config.NoChineseUnit) ? number.ToChineseSeString(minusColor, unitColor) : ReadOnlySeString.op_Implicit(number.ToMyriadString()));
				((Utf8String)outNumberString).SetString((ReadOnlySpan<byte>)SeStringExtensions.ToDalamudString(val).EncodeWithNullTerminator());
				return outNumberString;
			}
			default:
			{
				string text = number.ToMyriadString();
				((Utf8String)outNumberString).SetString((ReadOnlySpan<byte>)new SeString((Payload[])(object)new Payload[1] { (Payload)new TextPayload(text) }).EncodeWithNullTerminator());
				return outNumberString;
			}
			case 2:
			case 3:
			case 4:
			case 5:
				break;
			}
		}
		return result;
	}

	private unsafe void AtkCounterNodeSetNumberDetour(AtkCounterNode* node, CStringPointer number)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!config.NoChineseUnit && ((CStringPointer)(ref number)).HasValue)
		{
			string text = CStringExtensions.ExtractText(number);
			if (text.IsAnyChinese())
			{
				((AtkCounterNode)node).SetText(text.FromChineseString<int>().ToMyriadString());
				((AtkCounterNode)node).UpdateWidth();
				return;
			}
		}
		AtkCounterNodeSetNumberHook.Original(node, number);
	}
}
