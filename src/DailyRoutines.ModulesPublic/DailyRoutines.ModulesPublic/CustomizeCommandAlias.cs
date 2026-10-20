using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Lumina.Text.ReadOnly;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic;

public class CustomizeCommandAlias : ModuleBase
{
	private class Config : ModuleConfig
	{
		public List<AliasEntry> AliasEntries = new List<AliasEntry>();
	}

	private class AliasEntry
	{
		public bool Enabled { get; set; } = true;

		public string Alias { get; set; } = string.Empty;

		public string TargetCommand { get; set; } = string.Empty;
	}

	private readonly record struct CompiledAliasEntry(string Alias, string TargetCommand)
	{
		public readonly string Alias = Alias;

		public readonly string TargetCommand = TargetCommand;

		public readonly int AliasLength = Alias.Length;

		public readonly char FirstChar = Alias[0];

		[CompilerGenerated]
		public void Deconstruct(out string Alias, out string TargetCommand)
		{
			Alias = this.Alias;
			TargetCommand = this.TargetCommand;
		}
	}

	private Config config;

	private CompiledAliasEntry[] activeAliasEntries = Array.Empty<CompiledAliasEntry>();

	private string sourceCommandInput = string.Empty;

	private string targetCommandInput = string.Empty;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("CustomizeCommandAliasTitle"),
		Description = LanguageManager.Get("CustomizeCommandAliasDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		RefreshActiveAliasEntries();
		OmenServiceBase<ChatManager>.Instance().RegPreExecuteCommandInner(OnPreExecuteCommandInner);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ChatManager>.Instance().Unreg(OnPreExecuteCommandInner);
	}

	protected override void ConfigUI()
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		DisabledDisposable val = ImRaii.Disabled(string.IsNullOrEmpty(sourceCommandInput) || string.IsNullOrEmpty(targetCommandInput) || !sourceCommandInput.StartsWith('/') || !targetCommandInput.StartsWith('/'));
		try
		{
			if (ImGuiOm.ButtonIconWithTextVertical((FontAwesomeIcon)61543, LanguageManager.Get("Add")) && !HasDuplicateAlias(sourceCommandInput.AsSpan()))
			{
				AliasEntry item = new AliasEntry
				{
					Enabled = true,
					Alias = sourceCommandInput,
					TargetCommand = targetCommandInput
				};
				config.AliasEntries.Add(item);
				SaveAndRefresh(this);
				sourceCommandInput = string.Empty;
				targetCommandInput = string.Empty;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		GroupDisposable val2 = ImRaii.Group();
		try
		{
			ImU8String val3 = new ImU8String(12, 1);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get("Alias"));
			((ImU8String)(ref val3)).AppendLiteral("##AliasInput");
			ImGui.InputText(val3, ref sourceCommandInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			ImU8String val4 = new ImU8String(20, 1);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get("Target"));
			((ImU8String)(ref val4)).AppendLiteral("##TargetCommandInput");
			ImGui.InputText(val4, ref targetCommandInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		}
		finally
		{
			((GroupDisposable)(ref val2)).Dispose();
		}
		if (config.AliasEntries.Count == 0)
		{
			return;
		}
		ImGui.NewLine();
		TableDisposable val5 = ImRaii.Table(ImU8String.op_Implicit("##CustomizeCommandAliasTable"), 4, (ImGuiTableFlags)26560);
		try
		{
			if (!val5)
			{
				return;
			}
			ImU8String val6 = new ImU8String(9, 0);
			((ImU8String)(ref val6)).AppendLiteral("##Enabled");
			ImGui.TableSetupColumn(val6, (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Alias")), (ImGuiTableColumnFlags)4, 0.28f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Target")), (ImGuiTableColumnFlags)4, 0.42f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get("Operation")), (ImGuiTableColumnFlags)4, 0.18f, 0u);
			ImGui.TableHeadersRow();
			for (int i = 0; i < config.AliasEntries.Count; i++)
			{
				AliasEntry aliasEntry = config.AliasEntries[i];
				ImU8String val7 = new ImU8String(22, 1);
				((ImU8String)(ref val7)).AppendLiteral("CustomizeCommandAlias_");
				((ImU8String)(ref val7)).AppendFormatted<int>(i);
				IdDisposable val8 = ImRaii.PushId(val7, true);
				try
				{
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					bool enabled = aliasEntry.Enabled;
					if (ImGui.Checkbox(ImU8String.op_Implicit("##Enabled"), ref enabled))
					{
						aliasEntry.Enabled = enabled;
						SaveAndRefresh(this);
					}
					ImGui.TableNextColumn();
					string alias = aliasEntry.Alias;
					ImGui.SetNextItemWidth(-1f);
					ImGui.InputText(ImU8String.op_Implicit("##Alias"), ref alias, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					if (ImGui.IsItemDeactivatedAfterEdit() && !string.IsNullOrEmpty(alias) && !HasDuplicateAlias(alias.AsSpan(), i))
					{
						aliasEntry.Alias = alias;
						SaveAndRefresh(this);
					}
					ImGui.TableNextColumn();
					string targetCommand = aliasEntry.TargetCommand;
					ImGui.SetNextItemWidth(-1f);
					ImGui.InputText(ImU8String.op_Implicit("##Target"), ref targetCommand, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
					if (ImGui.IsItemDeactivatedAfterEdit() && !string.IsNullOrEmpty(targetCommand))
					{
						aliasEntry.TargetCommand = targetCommand;
						SaveAndRefresh(this);
					}
					if (ImGui.IsItemHovered())
					{
						ImGui.SetTooltip(ImU8String.op_Implicit(targetCommand));
					}
					ImGui.TableNextColumn();
					if (ImGuiOm.ButtonIcon("Delete", (FontAwesomeIcon)62189, LanguageManager.Get("Delete")))
					{
						config.AliasEntries.RemoveAt(i);
						SaveAndRefresh(this);
						i--;
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val5)).Dispose();
		}
	}

	private void OnPreExecuteCommandInner(ref bool isPrevented, ref ReadOnlySeString message)
	{
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		Span<CompiledAliasEntry> span = activeAliasEntries.AsSpan();
		if (span.IsEmpty)
		{
			return;
		}
		string text = ((object)Unsafe.As<ReadOnlySeString, ReadOnlySeString>(ref message)/*cast due to constrained. prefix*/).ToString();
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		ReadOnlySpan<char> span2 = text.AsSpan();
		char c = span2.get_Item(0);
		if (!c.Equals('/'))
		{
			return;
		}
		Span<CompiledAliasEntry> span3 = span;
		for (int i = 0; i < span3.Length; i++)
		{
			ref CompiledAliasEntry reference = ref span3[i];
			if (c == reference.FirstChar && span2.Length >= reference.AliasLength && span2.StartsWith(reference.Alias.AsSpan(), StringComparison.Ordinal))
			{
				message = ((span2.Length == reference.AliasLength) ? new ReadOnlySeString(reference.TargetCommand) : new ReadOnlySeString(string.Concat(reference.TargetCommand.AsSpan(), span2.Slice(reference.AliasLength))));
				break;
			}
		}
	}

	private void SaveAndRefresh(ModuleBase module)
	{
		config.Save(module);
		RefreshActiveAliasEntries();
	}

	private void RefreshActiveAliasEntries()
	{
		Span<AliasEntry> span = CollectionsMarshal.AsSpan(config.AliasEntries);
		int num = 0;
		Span<AliasEntry> span2 = span;
		for (int i = 0; i < span2.Length; i++)
		{
			ref AliasEntry reference = ref span2[i];
			if (reference.Enabled && !string.IsNullOrEmpty(reference.Alias) && !string.IsNullOrEmpty(reference.TargetCommand))
			{
				num++;
			}
		}
		if (num == 0)
		{
			activeAliasEntries = Array.Empty<CompiledAliasEntry>();
			return;
		}
		CompiledAliasEntry[] array = GC.AllocateUninitializedArray<CompiledAliasEntry>(num);
		int num2 = 0;
		Span<AliasEntry> span3 = span;
		for (int i = 0; i < span3.Length; i++)
		{
			ref AliasEntry reference2 = ref span3[i];
			if (reference2.Enabled && !string.IsNullOrEmpty(reference2.Alias) && !string.IsNullOrEmpty(reference2.TargetCommand))
			{
				array[num2++] = new CompiledAliasEntry(reference2.Alias, reference2.TargetCommand);
			}
		}
		activeAliasEntries = array;
	}

	private bool HasDuplicateAlias(ReadOnlySpan<char> alias, int excludedIndex = -1)
	{
		Span<AliasEntry> span = CollectionsMarshal.AsSpan(config.AliasEntries);
		for (int i = 0; i < span.Length; i++)
		{
			if (i != excludedIndex && alias.SequenceEqual(span[i].Alias.AsSpan()))
			{
				return true;
			}
		}
		return false;
	}
}
