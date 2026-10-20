using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;

namespace DailyRoutines.ModulesPublic.Interface;

public class PlayerTargetInfoExpand : ModuleBase
{
	private class Payload(string placeholder, string description, Func<ICharacter, string> valueFunc)
	{
		public string Placeholder { get; } = placeholder;

		public string Description { get; } = description;

		public Func<ICharacter, string> ValueFunc { get; } = valueFunc;
	}

	private class Config : ModuleConfig
	{
		public string FocusTargetPattern = "/Level/级 /Name/";

		public string TargetPattern = "/Name/ [/Job/] «/FCTag/»";

		public string TargetsTargetPattern = "/Name/";
	}

	private Config config;

	private static readonly FrozenSet<Payload> Payloads;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("PlayerTargetInfoExpandTitle"),
		Description = LanguageManager.Get("PlayerTargetInfoExpandDescription"),
		Category = ModuleCategory.Interface,
		ModulesConflict = new string[1] { "LiveAnonymousMode" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		NeedAuth = true
	};

	protected override void Init()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_TargetInfo", new AddonEventDelegate(UpdateTargetInfo));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_TargetInfoMainTarget", new AddonEventDelegate(UpdateTargetInfoMainTarget));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)8, "_FocusTargetInfo", new AddonEventDelegate(UpdateFocusTargetInfo));
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { UpdateTargetInfo });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { UpdateTargetInfoMainTarget });
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { UpdateFocusTargetInfo });
	}

	protected override void ConfigUI()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 tableSize = new Vector2(ImGui.GetContentRegionAvail().X / 2f, 0f);
		GroupDisposable val = ImRaii.Group();
		try
		{
			DrawInputAndPreviewText(LanguageManager.Get("Target"), ref config.TargetPattern);
			DrawInputAndPreviewText(LanguageManager.Get("PlayerTargetInfoExpand-TargetsTarget"), ref config.TargetsTargetPattern);
			DrawInputAndPreviewText(LanguageManager.Get("PlayerTargetInfoExpand-FocusTarget"), ref config.FocusTargetPattern);
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImGui.SameLine();
		TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("PayloadDisplay"), 2, (ImGuiTableFlags)1920, tableSize / 1.5f);
		try
		{
			if (!TableDisposable.op_Implicit(val2))
			{
				return;
			}
			ImGui.TableNextRow((ImGuiTableRowFlags)1);
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("PlayerTargetInfoExpand-AvailablePayload")));
			ImGui.TableNextColumn();
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get("Description")));
			foreach (Payload payload in Payloads)
			{
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(payload.Placeholder));
				ImGui.TableNextColumn();
				ImGui.TextUnformatted(ImU8String.op_Implicit(payload.Description));
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
		void DrawInputAndPreviewText(string categoryTitle, ref string configField)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			TableDisposable val3 = ImRaii.Table(ImU8String.op_Implicit(categoryTitle), 2, (ImGuiTableFlags)1280, tableSize);
			try
			{
				if (TableDisposable.op_Implicit(val3))
				{
					ImGui.TableSetupColumn(ImU8String.op_Implicit("###Category"), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit("真得要六个字"), false, -1f).X, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit("###Content"), (ImGuiTableColumnFlags)0, 50f, 0u);
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.AlignTextToFramePadding();
					ImU8String val4 = new ImU8String(1, 1);
					((ImU8String)(ref val4)).AppendFormatted<string>(categoryTitle);
					((ImU8String)(ref val4)).AppendLiteral(":");
					ImGui.TextUnformatted(val4);
					ImGui.TableNextColumn();
					ImGui.SetNextItemWidth(-1f);
					ImU8String val5 = new ImU8String(3, 1);
					((ImU8String)(ref val5)).AppendLiteral("###");
					((ImU8String)(ref val5)).AppendFormatted<string>(categoryTitle);
					if (ImGui.InputText(val5, ref configField, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null))
					{
						config.Save(this);
					}
					ICharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
					if (localPlayer != null)
					{
						ImGui.TableNextRow();
						ImGui.TableNextColumn();
						ImGui.AlignTextToFramePadding();
						ImU8String val6 = new ImU8String(1, 1);
						((ImU8String)(ref val6)).AppendFormatted<string>(LanguageManager.Get("Example"));
						((ImU8String)(ref val6)).AppendLiteral(":");
						ImGui.TextUnformatted(val6);
						ImGui.TableNextColumn();
						ImGui.TextUnformatted(ImU8String.op_Implicit(ReplacePatterns(configField, Payloads, localPlayer)));
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val3)).Dispose();
			}
			ImGui.Spacing();
		}
	}

	private unsafe void UpdateTargetInfo(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Invalid comparison between Unknown and I4
		AtkUnitBase* address = (AtkUnitBase*)args.Addon.Address;
		if (address != null && ((AtkUnitBase)address).IsVisible)
		{
			IGameObject target = TargetManager.Target;
			AtkTextNode* textNodeById = ((AtkUnitBase)address).GetTextNodeById(16u);
			if (textNodeById != null && target is ICharacter chara && (int)target.ObjectKind == 1)
			{
				((AtkTextNode)textNodeById).SetText(ReplacePatterns(config.TargetPattern, Payloads, chara));
			}
			IGameObject gameObject = TargetManager.Target?.TargetObject;
			AtkTextNode* textNodeById2 = ((AtkUnitBase)address).GetTextNodeById(7u);
			if (textNodeById2 != null && gameObject is ICharacter chara2 && (int)gameObject.ObjectKind == 1)
			{
				((AtkTextNode)textNodeById2).SetText(ReplacePatterns(config.TargetsTargetPattern, Payloads, chara2));
			}
		}
	}

	private unsafe void UpdateTargetInfoMainTarget(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Invalid comparison between Unknown and I4
		AtkUnitBase* address = (AtkUnitBase*)args.Addon.Address;
		if (address != null && ((AtkUnitBase)address).IsVisible)
		{
			IGameObject target = TargetManager.Target;
			AtkTextNode* textNodeById = ((AtkUnitBase)address).GetTextNodeById(10u);
			if (textNodeById != null && target is ICharacter chara && (int)target.ObjectKind == 1)
			{
				((AtkTextNode)textNodeById).SetText(ReplacePatterns(config.TargetPattern, Payloads, chara));
			}
			IGameObject gameObject = TargetManager.Target?.TargetObject;
			AtkTextNode* textNodeById2 = ((AtkUnitBase)address).GetTextNodeById(7u);
			if (textNodeById2 != null && gameObject is ICharacter chara2 && (int)gameObject.ObjectKind == 1)
			{
				((AtkTextNode)textNodeById2).SetText(ReplacePatterns(config.TargetsTargetPattern, Payloads, chara2));
			}
		}
	}

	private unsafe void UpdateFocusTargetInfo(AddonEvent type, AddonArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		AtkUnitBase* address = (AtkUnitBase*)args.Addon.Address;
		if (address != null && ((AtkUnitBase)address).IsVisible)
		{
			IGameObject focusTarget = TargetManager.FocusTarget;
			AtkTextNode* textNodeById = ((AtkUnitBase)address).GetTextNodeById(10u);
			if (textNodeById != null && focusTarget is ICharacter chara && (int)focusTarget.ObjectKind == 1)
			{
				((AtkTextNode)textNodeById).SetText(ReplacePatterns(config.FocusTargetPattern, Payloads, chara));
			}
		}
	}

	private static string ReplacePatterns(string input, IEnumerable<Payload> payloads, ICharacter chara)
	{
		foreach (Payload payload in payloads)
		{
			input = input.Replace(payload.Placeholder, payload.ValueFunc(chara));
		}
		return input;
	}

	unsafe static PlayerTargetInfoExpand()
	{
		_003C_003Ey__InlineArray19<Payload> buffer = default(_003C_003Ey__InlineArray19<Payload>);
		buffer[0] = new Payload("/Name/", LuminaWrapper.GetAddonText(6382u), (ICharacter c) => c.Name.ToString());
		buffer[1] = new Payload("/Job/", LuminaWrapper.GetAddonText(294u), delegate(ICharacter c)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			ClassJob? valueNullable = c.ClassJob.ValueNullable;
			object obj;
			ClassJob val;
			if (!valueNullable.HasValue)
			{
				obj = null;
			}
			else
			{
				val = valueNullable.GetValueOrDefault();
				obj = ((object)((ClassJob)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (obj == null)
			{
				val = LuminaGetter.GetRowOrDefault<ClassJob>(0u);
				obj = ((object)((ClassJob)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
			}
			return (string)obj;
		});
		buffer[2] = new Payload("/Level/", LuminaWrapper.GetAddonText(335u), (ICharacter c) => c.Level.ToString());
		buffer[3] = new Payload("/FCTag/", LuminaWrapper.GetAddonText(297u), (ICharacter c) => c.CompanyTag.ToString());
		buffer[4] = new Payload("/Title/", LuminaWrapper.GetAddonText(14119u), delegate(ICharacter c)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			Character* ptr = c.ToStruct();
			ushort titleId = ((CharacterData)(&((Character)ptr).CharacterData)).TitleId;
			Title item;
			return (titleId == 0 || !LuminaGetter.TryGetRow<Title>((uint)titleId, out item)) ? string.Empty : ((object)((((CustomizeData)(&((DrawDataContainer)(&((Character)ptr).DrawData)).CustomizeData)).Sex == 0) ? ((Title)(ref item)).Masculine : ((Title)(ref item)).Feminine)/*cast due to constrained. prefix*/).ToString();
		});
		buffer[5] = new Payload("/OnlineStatus/", LanguageManager.Get("OnlineStatus"), delegate(ICharacter c)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			OnlineStatus? valueNullable = c.OnlineStatus.ValueNullable;
			object value;
			OnlineStatus val;
			if (!valueNullable.HasValue)
			{
				value = null;
			}
			else
			{
				val = valueNullable.GetValueOrDefault();
				value = ((object)((OnlineStatus)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
			}
			if (!string.IsNullOrEmpty((string?)value))
			{
				valueNullable = c.OnlineStatus.ValueNullable;
				if (!valueNullable.HasValue)
				{
					return (string)null;
				}
				val = valueNullable.GetValueOrDefault();
				return ((object)((OnlineStatus)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
			}
			val = LuminaGetter.GetRowOrDefault<OnlineStatus>(47u);
			return ((object)((OnlineStatus)(ref val)).Name/*cast due to constrained. prefix*/).ToString();
		});
		buffer[6] = new Payload("/Mount/", LuminaWrapper.GetAddonText(4964u), delegate(ICharacter c)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			Mount rowOrDefault = LuminaGetter.GetRowOrDefault<Mount>((uint)((MountContainer)(&((Character)c.ToStruct()).Mount)).MountId);
			return ((object)((Mount)(ref rowOrDefault)).Singular/*cast due to constrained. prefix*/).ToString();
		});
		buffer[7] = new Payload("/HomeWorld/", LuminaWrapper.GetAddonText(4728u), delegate(ICharacter c)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			World rowOrDefault = LuminaGetter.GetRowOrDefault<World>((uint)((Character)c.ToStruct()).HomeWorld);
			return ((object)((World)(ref rowOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		});
		buffer[8] = new Payload("/Emote/", LuminaWrapper.GetAddonText(780u), delegate(ICharacter c)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			Emote rowOrDefault = LuminaGetter.GetRowOrDefault<Emote>((uint)((EmoteController)(&((Character)c.ToStruct()).EmoteController)).EmoteId);
			return ((object)((Emote)(ref rowOrDefault)).Name/*cast due to constrained. prefix*/).ToString();
		});
		buffer[9] = new Payload("/TargetsTarget/", LanguageManager.Get("TargetOfTarget"), (ICharacter c) => c.TargetObject?.Name.ToString() ?? string.Empty);
		buffer[10] = new Payload("/ShieldValue/", LanguageManager.Get("Sheild"), (ICharacter c) => c.ShieldPercentage.ToString());
		buffer[11] = new Payload("/CurrentHP/", LuminaWrapper.GetAddonText(232u), (ICharacter c) => c.CurrentHp.ToString());
		buffer[12] = new Payload("/MaxHP/", LanguageManager.Get("MaxHP"), (ICharacter c) => c.MaxHp.ToString());
		buffer[13] = new Payload("/CurrentMP/", LuminaWrapper.GetAddonText(233u), (ICharacter c) => c.CurrentMp.ToString());
		buffer[14] = new Payload("/MaxMP/", LanguageManager.Get("MaxMP"), (ICharacter c) => c.MaxMp.ToString());
		buffer[15] = new Payload("/CurrentCP/", LuminaWrapper.GetAddonText(1004u), (ICharacter c) => c.CurrentCp.ToString());
		buffer[16] = new Payload("/MaxCP/", LanguageManager.Get("MaxCP"), (ICharacter c) => c.MaxCp.ToString());
		buffer[17] = new Payload("/CurrentGP/", LuminaWrapper.GetAddonText(1003u), (ICharacter c) => c.CurrentGp.ToString());
		buffer[18] = new Payload("/MaxGP/", LanguageManager.Get("MaxGP"), (ICharacter c) => c.MaxGp.ToString());
		Payloads = FrozenSet.Create<Payload>(buffer);
	}
}
