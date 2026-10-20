using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Hooking;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.Text;
using OmenTools.Global;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Models;

namespace DailyRoutines.ModulesPublic.Interface;

public class OptimizedChatBubble : ModuleBase
{
	private unsafe delegate ulong ChatBubbleDelegate(ChatBubbleStruct* chatBubbleStruct);

	private delegate byte SetupChatBubbleDelegate(nint unk, nint newBubble, nint a3);

	private unsafe delegate uint GetStringSizeDelegate(TextChecker* textChecker, Utf8String* str);

	[StructLayout(LayoutKind.Explicit)]
	private struct ChatBubbleStruct
	{
		[FieldOffset(140)]
		public byte LineCount;
	}

	[StructLayout(LayoutKind.Explicit)]
	private struct ChatBubbleEntry
	{
		[FieldOffset(0)]
		public Utf8String String;

		[FieldOffset(440)]
		public long Timestamp;
	}

	private class Config : ModuleConfig
	{
		public uint AdditionalDuration;

		public uint Duration = 4000u;

		public bool IsShowInCombat = true;

		public int MaxLines = 2;
	}

	private static readonly CompSig ChatBubbleSig = new CompSig("E8 ?? ?? ?? ?? 0F B6 E8 48 8D 5F 18 40 0A 6C 24 ?? BE");

	private Hook<ChatBubbleDelegate> ChatBubbleHook;

	private static readonly CompSig SetupChatBubbleSig = new CompSig("E8 ?? ?? ?? ?? 49 FF 46 60");

	private Hook<SetupChatBubbleDelegate> SetupChatBubbleHook;

	private static readonly CompSig GetStringSizeSig = new CompSig("E8 ?? ?? ?? ?? 49 8D 56 40");

	private GetStringSizeDelegate getStringSize;

	private MemoryPatch showMiniTalkPlayerPatch;

	private Config moduleConfig;

	private readonly HashSet<nint> newBubbles = new HashSet<nint>();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("OptimizedChatBubbleTitle"),
		Description = LanguageManager.Get("OptimizedChatBubbleDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[2] { "Middo", "Xww" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		getStringSize = GetStringSizeSig.GetDelegate<GetStringSizeDelegate>();
		showMiniTalkPlayerPatch = new MemoryPatch("0F 84 ?? ?? ?? ?? ?? ?? ?? 48 8B CF 49 89 46", new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[2]
		{
			(byte)144,
			(byte)233
		}));
		moduleConfig = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		ChatBubbleHook = ChatBubbleSig.GetHook<ChatBubbleDelegate>(ChatBubbleDetour);
		ChatBubbleHook.Enable();
		SetupChatBubbleHook = SetupChatBubbleSig.GetHook<SetupChatBubbleDelegate>(SetupChatBubbleDetour);
		SetupChatBubbleHook.Enable();
		showMiniTalkPlayerPatch.Set(moduleConfig.IsShowInCombat);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("OptimizedChatBubble-ShowInCombat")), ref moduleConfig.IsShowInCombat))
		{
			moduleConfig.Save(this);
			showMiniTalkPlayerPatch.Set(moduleConfig.IsShowInCombat);
		}
		ItemWidthDisposable val = ImRaii.ItemWidth(150f * Globals.GlobalUIScale);
		try
		{
			ImU8String val2 = ImU8String.op_Implicit(LanguageManager.Get("OptimizedChatBubble-MaxLine"));
			ref int maxLines = ref moduleConfig.MaxLines;
			ImU8String val3 = default(ImU8String);
			if (ImGui.InputInt(val2, ref maxLines, 1, 0, val3, (ImGuiInputTextFlags)0))
			{
				moduleConfig.MaxLines = Math.Clamp(moduleConfig.MaxLines, 1, 7);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				moduleConfig.Save(this);
			}
			val3 = new ImU8String(5, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("OptimizedChatBubble-BaseDuration"));
			((ImU8String)(ref val3)).AppendLiteral(" (ms)");
			ImU8String val4 = val3;
			ref uint duration = ref moduleConfig.Duration;
			ImU8String val5 = default(ImU8String);
			if (ImGui.InputUInt(val4, ref duration, 500u, 1000u, val5, (ImGuiInputTextFlags)0))
			{
				moduleConfig.Duration = Math.Clamp(moduleConfig.Duration, 1000u, 60000u);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				moduleConfig.Save(this);
			}
			val5 = new ImU8String(5, 1);
			((ImU8String)(ref val5)).AppendFormatted<string>(LanguageManager.Get("OptimizedChatBubble-AdditionalDuration"));
			((ImU8String)(ref val5)).AppendLiteral(" (ms)");
			if (ImGui.InputUInt(val5, ref moduleConfig.AdditionalDuration, 1u, 10u, default(ImU8String), (ImGuiInputTextFlags)0))
			{
				moduleConfig.AdditionalDuration = Math.Clamp(moduleConfig.AdditionalDuration, 0u, 10000u);
			}
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				moduleConfig.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private unsafe ulong ChatBubbleDetour(ChatBubbleStruct* chatBubbleStruct)
	{
		try
		{
			return ChatBubbleHook.Original(chatBubbleStruct);
		}
		finally
		{
			chatBubbleStruct->LineCount = (byte)Math.Clamp(moduleConfig.MaxLines, 1, 7);
			newBubbles.RemoveWhere(delegate(nint b)
			{
				if (((ChatBubbleEntry*)b)->Timestamp < 200)
				{
					if (((ChatBubbleEntry*)b)->Timestamp >= 0)
					{
						((ChatBubbleEntry*)b)->Timestamp++;
					}
					return false;
				}
				((ChatBubbleEntry*)b)->Timestamp += moduleConfig.Duration - 4000;
				if (moduleConfig.AdditionalDuration != 0)
				{
					uint num = getStringSize(&((RaptureTextModule)RaptureTextModule.Instance()).TextChecker, &((ChatBubbleEntry*)b)->String);
					long num2 = moduleConfig.AdditionalDuration * Math.Clamp(num, 0L, 194 * moduleConfig.MaxLines);
					((ChatBubbleEntry*)b)->Timestamp += num2;
				}
				return true;
			});
		}
	}

	private byte SetupChatBubbleDetour(nint unk, nint newBubble, nint a3)
	{
		try
		{
			if (moduleConfig.Duration != 4000 || moduleConfig.AdditionalDuration != 0)
			{
				newBubbles.Add(newBubble);
			}
			return SetupChatBubbleHook.Original(unk, newBubble, a3);
		}
		catch
		{
			return 0;
		}
	}
}
