using System;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using DailyRoutines.Common.Info.Models;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.Gui.NamePlate;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.FFXIV.Component.Text;
using FFXIVClientStructs.STD;
using InteropGenerator.Runtime;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Lumina;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class LiveAnonymousMode : ModuleBase
{
	private delegate CStringPointer qopi00xxgFf3SkhDyeU(nint data);

	private class ujGwogxpHdoYSSCmLV1 : ModuleConfig
	{
		public bool ObfuscateDamageNumber;

		public ObfuscationMethod ObfuscationMethod;

		public string PlayerName;

		public ujGwogxpHdoYSSCmLV1()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			PlayerName = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09F015 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363);
			base._002Ector();
		}
	}

	private enum ObfuscationMethod
	{
		RandomLetters,
		RandomNumbers,
		None,
		RandomNPCName
	}

	private unsafe delegate void* LiywPjx3hQmVeYgOCpF(AddonFlyText* addon, int kind, int val1, int val2, byte* text2, uint color, uint icon, uint damageTypeIcon, byte* text1, float yOffset);

	private unsafe delegate void sErX2OxBpQj4UDTB1RL(AtkTextNode* textNode, CStringPointer text);

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass34_0
	{
		public LiveAnonymousMode _003C_003E4__this;

		public string input;

		public _003C_003Ec__DisplayClass34_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal string EOYxmqEUMf(string _)
		{
			_003C_003E4__this.QyW12WPU7A();
			return _003C_003E4__this.iN51rHJaky(input);
		}
	}

	private static readonly CompSig BannerPartySplitNameSig;

	private Hook<qopi00xxgFf3SkhDyeU> BannerPartySplitNameHook;

	private ujGwogxpHdoYSSCmLV1 config;

	private readonly RandomLuminaSelector<BNpcName, string> randomNPCNameSelector;

	private static readonly CompSig CreateFlyTextSig;

	private Hook<LiywPjx3hQmVeYgOCpF>? CreateFlyTextHook;

	private static readonly FrozenDictionary<ObfuscationMethod, string> ObfuscationMethodLoc;

	private static readonly FrozenSet<ObjectKind> ValidTitleKinds;

	private static readonly char[] CharPool;

	private static readonly FrozenSet<string> AddonTexts;

	private static readonly FrozenSet<string> ClassJobTexts;

	private static readonly CompSig TextNodeSetStringSig;

	private Hook<sErX2OxBpQj4UDTB1RL>? TextNodeSetStringHook;

	private ConcurrentDictionary<string, string> obfuscationCache;

	private ConcurrentDictionary<string, byte> permanentKeys;

	private int obfuscationRevision;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	private unsafe CStringPointer Hvi1gCUSBM(nint data)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Utf8String* ptr = (Utf8String*)(data + 24);
		string text = UtO1JGvQ9d(((object)(*(CStringPointer*)(&((Utf8String)ptr).StringPtr))/*cast due to constrained. prefix*/).ToString());
		Utf8String val = default(Utf8String);
		((Utf8String)(ref val))._002Ector(text);
		try
		{
			((Utf8String)ptr).Copy(&val);
			return BannerPartySplitNameHook.Original(data);
		}
		finally
		{
			((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	protected unsafe override void Init()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<ujGwogxpHdoYSSCmLV1>(this) ?? new ujGwogxpHdoYSSCmLV1();
		DalamudServiceExtension.Instance<INamePlateGui>().OnNamePlateUpdate += new OnPlateUpdateDelegate(bgZ1FOd83f);
		DalamudServiceExtension.Instance<IClientState>().Login += kOx1ErWOHT;
		if (GameState.IsLoggedIn)
		{
			Refresh();
		}
		if (TextNodeSetStringHook == null)
		{
			TextNodeSetStringHook = TextNodeSetStringSig.GetHook<sErX2OxBpQj4UDTB1RL>(Qxn1aKFoUy);
		}
		TextNodeSetStringHook.Enable();
		ManagerBase<PlayersManager>.Instance().ReceivePlayerInfo += HD21btt2b7;
		CreateFlyTextHook = CreateFlyTextSig.GetHook<LiywPjx3hQmVeYgOCpF>(NyA1wL2aJm);
		CreateFlyTextHook.Enable();
		BannerPartySplitNameHook = BannerPartySplitNameSig.GetHook<qopi00xxgFf3SkhDyeU>(Hvi1gCUSBM);
		BannerPartySplitNameHook.Enable();
	}

	protected override void Uninit()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		ManagerBase<PlayersManager>.Instance().ReceivePlayerInfo -= HD21btt2b7;
		DalamudServiceExtension.Instance<INamePlateGui>().OnNamePlateUpdate -= new OnPlateUpdateDelegate(bgZ1FOd83f);
		DalamudServiceExtension.Instance<IClientState>().Login -= kOx1ErWOHT;
	}

	protected override void ConfigUI()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(0, 1);
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -1365399650 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)));
		ImGui.TextColored(ref vector, val);
		IndentDisposable val2 = ImRaii.PushIndent(1, true);
		try
		{
			GroupDisposable val3 = ImRaii.Group();
			try
			{
				vector = KnownColor.Chocolate.ToVector4();
				ImU8String val4 = new ImU8String(0, 1);
				((ImU8String)(ref val4)).AppendFormatted<string>(LuminaWrapper.GetAddonText(7641u));
				ImGui.TextColored(ref vector, val4);
				ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14816D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)), 40f, (FontAwesomeIcon)61530);
			}
			finally
			{
				((GroupDisposable)(ref val3)).Dispose();
			}
			GroupDisposable val5 = ImRaii.Group();
			try
			{
				vector = KnownColor.Chocolate.ToVector4();
				ImU8String val6 = new ImU8String(0, 1);
				((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D752D30 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)));
				ImGui.TextColored(ref vector, val6);
				ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x79DD30F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)), 40f, (FontAwesomeIcon)61530);
			}
			finally
			{
				((GroupDisposable)(ref val5)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		ImGui.NewLine();
		GroupDisposable val7 = ImRaii.Group();
		try
		{
			vector = KnownColor.Chocolate.ToVector4();
			ImU8String val8 = new ImU8String(0, 1);
			((ImU8String)(ref val8)).AppendFormatted<string>(LuminaWrapper.GetAddonText(9818u));
			ImGui.TextColored(ref vector, val8);
			val2 = ImRaii.PushIndent(1, true);
			try
			{
				ItemWidthDisposable val9 = ImRaii.ItemWidth(300f * Globals.GlobalUIScale);
				try
				{
					ImGui.InputText(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E24CF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467)), ref config.PlayerName, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6767E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)));
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						if (config.PlayerName.Split(' ').Length > 2)
						{
							config.PlayerName = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E53AA0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd);
						}
						config.Save(this);
					}
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImGui.NewLine();
			vector = KnownColor.Chocolate.ToVector4();
			ImU8String val10 = new ImU8String(0, 1);
			((ImU8String)(ref val10)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E411BB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)));
			ImGui.TextColored(ref vector, val10);
			val2 = ImRaii.PushIndent(1, true);
			try
			{
				ItemWidthDisposable val9 = ImRaii.ItemWidth(300f * Globals.GlobalUIScale);
				try
				{
					ComboDisposable val11 = ImRaii.Combo(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E8C92 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71)), ImU8String.op_Implicit(ObfuscationMethodLoc.GetValueOrDefault(config.ObfuscationMethod, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x6AD594A4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)))));
					try
					{
						if (ComboDisposable.op_Implicit(val11))
						{
							foreach (KeyValuePair<ObfuscationMethod, string> item in ObfuscationMethodLoc)
							{
								ImU8String val12 = new ImU8String(3, 2);
								((ImU8String)(ref val12)).AppendFormatted<string>(item.Value);
								((ImU8String)(ref val12)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D73E4E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
								((ImU8String)(ref val12)).AppendFormatted<ObfuscationMethod>(item.Key);
								if (ImGui.Selectable(val12, item.Key == config.ObfuscationMethod, (ImGuiSelectableFlags)0, default(Vector2)))
								{
									config.ObfuscationMethod = item.Key;
									config.Save(this);
								}
							}
						}
					}
					finally
					{
						((ComboDisposable)(ref val11)).Dispose();
					}
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val7)).Dispose();
		}
		ImGui.NewLine();
		ImU8String val13 = default(ImU8String);
		((ImU8String)(ref val13))._002Ector(23, 1);
		((ImU8String)(ref val13)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC4F98 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)));
		((ImU8String)(ref val13)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E9930 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1));
		if (ImGui.Checkbox(val13, ref config.ObfuscateDamageNumber))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		ImU8String val14 = default(ImU8String);
		((ImU8String)(ref val14))._002Ector(1, 2);
		((ImU8String)(ref val14)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61561));
		((ImU8String)(ref val14)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE337BD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818));
		((ImU8String)(ref val14)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FE26E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af)));
		if (ImGui.Button(val14, default(Vector2)))
		{
			Refresh();
		}
	}

	private unsafe void* NyA1wL2aJm(AddonFlyText* addon, int kind, int val1, int val2, byte* text2, uint color, uint icon, uint damageTypeIcon, byte* text1, float yoffset)
	{
		if (config.ObfuscateDamageNumber)
		{
			val1 = w9m10ysE2S(val1);
		}
		return CreateFlyTextHook.Original(addon, kind, val1, val2, text2, color, icon, damageTypeIcon, text1, yoffset);
	}

	private void kOx1ErWOHT()
	{
		Refresh();
	}

	private void HD21btt2b7(PlayerInfo data)
	{
		UtO1JGvQ9d(data.Name);
		if (!string.IsNullOrEmpty(data.FreeCompanyName))
		{
			UtO1JGvQ9d(data.FreeCompanyName);
		}
	}

	private void bgZ1FOd83f(INamePlateUpdateContext context, IReadOnlyList<INamePlateUpdateHandler> handlers)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected I4, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Invalid comparison between Unknown and I4
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		foreach (INamePlateUpdateHandler handler in handlers)
		{
			NamePlateKind namePlateKind = handler.NamePlateKind;
			switch ((int)namePlateKind)
			{
			case 1:
			case 4:
				if (handler.GameObject != null && ValidTitleKinds.Contains(handler.GameObject.ObjectKind))
				{
					handler.RemoveTitle();
				}
				break;
			case 0:
			{
				handler.RemoveFreeCompanyTag();
				handler.RemoveTitle();
				IPlayerCharacter playerCharacter = IPlayerCharacter.Create(handler.GameObject.Address);
				if (playerCharacter != null && (int)playerCharacter.ObjectKind == 1 && playerCharacter.ClassJob.RowId != 0)
				{
					UtO1JGvQ9d(playerCharacter.FreeCompanyName);
					UtO1JGvQ9d(playerCharacter.Name);
					ClassJob value = playerCharacter.ClassJob.Value;
					string text = ((object)((ClassJob)(ref value)).Name/*cast due to constrained. prefix*/).ToString();
					if (((object)handler.Name).ToString() != text)
					{
						handler.Name = SeString.op_Implicit(text);
					}
				}
				break;
			}
			}
		}
	}

	private void Refresh()
	{
		ConcurrentDictionary<string, string> concurrentDictionary = obfuscationCache;
		ConcurrentDictionary<string, string> concurrentDictionary2 = new ConcurrentDictionary<string, string>();
		foreach (string key in permanentKeys.Keys)
		{
			if (concurrentDictionary.TryGetValue(key, out var value))
			{
				concurrentDictionary2.TryAdd(key, value);
			}
		}
		Volatile.Write(ref obfuscationCache, concurrentDictionary2);
		QyW12WPU7A();
		Ww31q43KPk();
		wQH1c39iWr();
		PLd1AIONmA();
	}

	private unsafe void Ww31q43KPk()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		RaptureTextModule* ptr = RaptureTextModule.Instance();
		if (ptr == null)
		{
			return;
		}
		StdDeque<TextParameter> globalParameters = ((RaptureTextModule)ptr).GlobalParameters;
		if (((RaptureTextModule)ptr).GetGlobalParameters() == null || globalParameters.Count <= 54)
		{
			return;
		}
		string text = ((object)(*(Utf8String*)(&((ReferencedUtf8String)globalParameters[0].ReferencedUtf8StringValue).Utf8String))/*cast due to constrained. prefix*/).ToString();
		if (!string.IsNullOrWhiteSpace(config.PlayerName) && !string.IsNullOrWhiteSpace(text))
		{
			string[] array = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			string[] array2 = config.PlayerName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			Wvv1QCJ8nE(text, config.PlayerName);
			if (array2.Length == 2 && array.Length == 2)
			{
				Wvv1QCJ8nE(array[0], array2[0]);
				Wvv1QCJ8nE(array[1], array2[1]);
			}
		}
	}

	private unsafe void wQH1c39iWr()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		RaptureTextModule* ptr = RaptureTextModule.Instance();
		if (ptr == null)
		{
			return;
		}
		StdDeque<TextParameter> globalParameters = ((RaptureTextModule)ptr).GlobalParameters;
		if (((RaptureTextModule)ptr).GetGlobalParameters() != null && globalParameters.Count > 54)
		{
			string text = ((object)(*(Utf8String*)(&((ReferencedUtf8String)globalParameters[54].ReferencedUtf8StringValue).Utf8String))/*cast due to constrained. prefix*/).ToString();
			if (!string.IsNullOrWhiteSpace(text))
			{
				UtO1JGvQ9d(text, addToHistory: true, isPermanent: true);
			}
		}
	}

	private void PLd1AIONmA()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<uint, World> world in Sheets.Worlds)
		{
			World value = world.Value;
			UtO1JGvQ9d(((object)((World)(ref value)).Name/*cast due to constrained. prefix*/).ToString(), addToHistory: true, isPermanent: true);
		}
	}

	private void Wvv1QCJ8nE(string key, string value)
	{
		if (!string.IsNullOrWhiteSpace(key) && obfuscationCache.TryAdd(key, value))
		{
			permanentKeys?.TryAdd(key, 0);
			QyW12WPU7A();
		}
	}

	private string UtO1JGvQ9d(string input, bool addToHistory = true, bool isPermanent = false)
	{
		_003C_003Ec__DisplayClass34_0 CS_0024_003C_003E8__locals13 = new _003C_003Ec__DisplayClass34_0();
		CS_0024_003C_003E8__locals13._003C_003E4__this = this;
		CS_0024_003C_003E8__locals13.input = input;
		if (config.ObfuscationMethod == ObfuscationMethod.None || string.IsNullOrEmpty(CS_0024_003C_003E8__locals13.input) || CS_0024_003C_003E8__locals13.input.Length == 1 || AddonTexts.Contains(CS_0024_003C_003E8__locals13.input) || ClassJobTexts.Contains(CS_0024_003C_003E8__locals13.input))
		{
			return CS_0024_003C_003E8__locals13.input;
		}
		if (!addToHistory)
		{
			return iN51rHJaky(CS_0024_003C_003E8__locals13.input);
		}
		string orAdd = obfuscationCache.GetOrAdd(CS_0024_003C_003E8__locals13.input, delegate
		{
			CS_0024_003C_003E8__locals13._003C_003E4__this.QyW12WPU7A();
			return CS_0024_003C_003E8__locals13._003C_003E4__this.iN51rHJaky(CS_0024_003C_003E8__locals13.input);
		});
		if (isPermanent)
		{
			ConcurrentDictionary<string, byte> concurrentDictionary = permanentKeys;
			if (concurrentDictionary == null)
			{
				return orAdd;
			}
			concurrentDictionary.TryAdd(CS_0024_003C_003E8__locals13.input, 0);
		}
		return orAdd;
	}

	private string iN51rHJaky(string input)
	{
		return config.ObfuscationMethod switch
		{
			ObfuscationMethod.RandomLetters => Yvq1GA6qX8(input.Length), 
			ObfuscationMethod.RandomNumbers => gLi16vBMgT(input.Length), 
			ObfuscationMethod.RandomNPCName => aA41ZFdoKw(input, randomNPCNameSelector?.Get() ?? input), 
			_ => input, 
		};
	}

	private static string Yvq1GA6qX8(int length)
	{
		return string.Create(length, 0, delegate(Span<char> span, int _)
		{
			for (int i = 0; i < span.Length; i++)
			{
				span[i] = CharPool[Random.Shared.Next(CharPool.Length)];
			}
		});
	}

	private static string gLi16vBMgT(int length)
	{
		return string.Create(length, 0, delegate(Span<char> span, int _)
		{
			for (int i = 0; i < span.Length; i++)
			{
				span[i] = (char)(48 + Random.Shared.Next(10));
			}
		});
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool kJY1krlsy7(SeString origText, out byte[]? modifiedText)
	{
		modifiedText = null;
		KeyValuePair<string, string>[] array = obfuscationCache.ToArray();
		if (array.Length == 0)
		{
			return false;
		}
		bool flag = false;
		for (int i = 0; i < origText.Payloads.Count; i++)
		{
			Payload obj = origText.Payloads[i];
			TextPayload val = (TextPayload)(object)((obj is TextPayload) ? obj : null);
			if (val == null)
			{
				continue;
			}
			string text = val.Text;
			KeyValuePair<string, string>[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				KeyValuePair<string, string> keyValuePair = array2[j];
				var (target, replacement) = (KeyValuePair<string, string>)(ref keyValuePair);
				if (text.TryReplaceIgnoreCase(target, replacement, out string result))
				{
					text = result;
					flag = true;
				}
			}
			if (!(text == val.Text))
			{
				val.Text = text;
				origText.Payloads[i] = (Payload)(object)val;
			}
		}
		if (flag)
		{
			modifiedText = origText.Encode();
		}
		return flag;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void QyW12WPU7A()
	{
		Interlocked.Increment(ref obfuscationRevision);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string aA41ZFdoKw(string source, string replacement)
	{
		if (source.Length == 0 || replacement.Length == 0 || replacement.Length == source.Length)
		{
			return replacement;
		}
		int length = source.Length;
		int length2 = replacement.Length;
		if (length2 > length)
		{
			int num = Math.Max(length + 4, (int)Math.Ceiling((double)length * 1.5));
			if (length2 > num)
			{
				return replacement.Substring(0, num);
			}
			return replacement;
		}
		if (length2 + 2 >= length)
		{
			return replacement;
		}
		return string.Create(length, replacement, delegate(Span<char> span, string rep)
		{
			ReadOnlySpan<char> readOnlySpan = rep.AsSpan();
			readOnlySpan.CopyTo(span);
			int num2;
			for (int i = readOnlySpan.Length; i < span.Length; i += num2)
			{
				num2 = Math.Min(readOnlySpan.Length, span.Length - i);
				readOnlySpan.Slice(0, num2).CopyTo(span.Slice(i));
			}
		});
	}

	private unsafe void Qxn1aKFoUy(AtkTextNode* textNode, CStringPointer text)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		if (textNode == null || !((CStringPointer)(ref text)).HasValue)
		{
			TextNodeSetStringHook.Original(textNode, text);
			return;
		}
		SeString val = SeString.Parse(text.Value);
		if (string.IsNullOrEmpty(val.TextValue))
		{
			TextNodeSetStringHook.Original(textNode, text);
			return;
		}
		if (kJY1krlsy7(val, out byte[] modifiedText))
		{
			if (((Utf8String)(&((AtkTextNode)textNode).NodeText)).IsEmpty || !((CStringPointer)(&((Utf8String)(&((AtkTextNode)textNode).NodeText)).StringPtr)).HasValue || string.IsNullOrEmpty(((object)(*(CStringPointer*)(&((Utf8String)(&((AtkTextNode)textNode).NodeText)).StringPtr))/*cast due to constrained. prefix*/).ToString()))
			{
				TextNodeSetStringHook.Original(textNode, text);
			}
			Utf8String val2 = default(Utf8String);
			((Utf8String)(ref val2))._002Ector((ReadOnlySpan<byte>)modifiedText);
			try
			{
				((Utf8String)(&((AtkTextNode)textNode).NodeText)).Copy(&val2);
				TextNodeSetStringHook.Original(textNode, ((Utf8String)(&((AtkTextNode)textNode).NodeText)).StringPtr);
				return;
			}
			finally
			{
				((IDisposable)(*(Utf8String*)(&val2))/*cast due to constrained. prefix*/).Dispose();
			}
		}
		TextNodeSetStringHook.Original(textNode, text);
	}

	public LiveAnonymousMode()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C1487E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D19EF3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)),
			Category = ModuleCategory.General,
			ModulesConflict = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC96546 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		randomNPCNameSelector = new RandomLuminaSelector<BNpcName, string>(((IEnumerable<BNpcName>)LuminaGetter.Get<BNpcName>()).Where(delegate(BNpcName x)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			ReadOnlySeString singular = ((BNpcName)(ref x)).Singular;
			return !((ReadOnlySeString)(ref singular)).IsEmpty;
		}).ToArray(), (BNpcName x) => ((object)((BNpcName)(ref x)).Singular/*cast due to constrained. prefix*/).ToString());
		obfuscationCache = new ConcurrentDictionary<string, string>();
		permanentKeys = new ConcurrentDictionary<string, byte>();
		base._002Ector();
	}

	unsafe static LiveAnonymousMode()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		BannerPartySplitNameSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1317373447 ^ 0x62608534 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
		CreateFlyTextSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420CF968 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
		ObfuscationMethodLoc = new Dictionary<ObfuscationMethod, string>
		{
			[ObfuscationMethod.RandomLetters] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C53BC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)),
			[ObfuscationMethod.RandomNumbers] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C175845 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb)),
			[ObfuscationMethod.None] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9CB65F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee)),
			[ObfuscationMethod.RandomNPCName] = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E415A3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7))
		}.ToFrozenDictionary();
		ValidTitleKinds = FrozenSet.Create(new ReadOnlySpan<ObjectKind>(Unsafe.AsPointer(ref global::_003CPrivateImplementationDetails_003E.F9535535516E208E35E9C0E78EFC9F998672699F00C34688D01678BBE58B95A6), 2));
		CharPool = Enumerable.Range(0, 26).SelectMany((int i) => new char[2]
		{
			(char)(65 + i),
			(char)(97 + i)
		}).ToArray();
		AddonTexts = (from x in (IEnumerable<Addon>)LuminaGetter.Get<Addon>()
			select ((object)((Addon)(ref x)).Text/*cast due to constrained. prefix*/).ToString() into x
			where !string.IsNullOrWhiteSpace(x)
			select x).Distinct().ToHashSet<string>(StringComparer.OrdinalIgnoreCase).ToFrozenSet();
		ClassJobTexts = (from x in (IEnumerable<ClassJob>)LuminaGetter.Get<ClassJob>()
			select ((object)((ClassJob)(ref x)).Name/*cast due to constrained. prefix*/).ToString() into x
			where !string.IsNullOrWhiteSpace(x)
			select x).Distinct().ToHashSet<string>(StringComparer.OrdinalIgnoreCase).ToFrozenSet();
		TextNodeSetStringSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258FBE5A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
	}

	[CompilerGenerated]
	internal static int w9m10ysE2S(int value)
	{
		uint num = (uint)value;
		num = ((num >> 16) ^ num) * 73244475;
		num = ((num >> 16) ^ num) * 73244475;
		num = (num >> 16) ^ num;
		return (int)(num % 1000000 + 1);
	}
}
