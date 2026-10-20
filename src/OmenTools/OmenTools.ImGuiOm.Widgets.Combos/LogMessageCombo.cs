using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Text.Evaluator;
using Dalamud.Interface.ImGuiSeStringRenderer;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.ImGuiOm.Widgets.Combos;

public class LogMessageCombo : LuminaComboBase<LogMessage>
{
	public override uint SelectedID { get; set; }

	public override HashSet<uint> SelectedIDs { get; set; } = new HashSet<uint>();

	public LogMessageCombo(string id, IEnumerable<LogMessage> logMessages = null)
		: base(id, (LuminaSearcher<LogMessage>)null)
	{
		IEnumerable<LogMessage> data = (IEnumerable<LogMessage>)(((object)logMessages) ?? ((object)LuminaGetter.Get<LogMessage>()));
		base.Searcher = new LuminaSearcher<LogMessage>(data, new Func<LogMessage, string>[3]
		{
			(LogMessage x) => ((LogMessage)(ref x)).RowId.ToString(),
			delegate(LogMessage x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				LogKind value = ((LogMessage)(ref x)).LogKind.Value;
				ReadOnlySeString format = ((LogKind)(ref value)).Format;
				return ((ReadOnlySeString)(ref format)).ToMacroString();
			},
			delegate(LogMessage x)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				ReadOnlySeString text = ((LogMessage)(ref x)).Text;
				return ((ReadOnlySeString)(ref text)).ToMacroString();
			}
		}, null, 200);
	}

	protected override string GetPreviewText(ComboSelectionMode mode)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		LogMessage val;
		if (mode == ComboSelectionMode.Radio)
		{
			val = base.SelectedItem;
			if (((LogMessage)(ref val)).RowId != 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
				ISeStringEvaluator obj = DalamudServiceExtension.Instance<ISeStringEvaluator>();
				val = base.SelectedItem;
				defaultInterpolatedStringHandler.AppendFormatted<ReadOnlySeString>(obj.EvaluateFromLogMessage(((LogMessage)(ref val)).RowId, default(Span<SeStringParameter>), (ClientLanguage?)null));
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			return string.Empty;
		}
		if (base.SelectedItems.Count != 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 1);
			ISeStringEvaluator obj2 = DalamudServiceExtension.Instance<ISeStringEvaluator>();
			val = base.SelectedItems.First();
			defaultInterpolatedStringHandler2.AppendFormatted<ReadOnlySeString>(obj2.EvaluateFromLogMessage(((LogMessage)(ref val)).RowId, default(Span<SeStringParameter>), (ClientLanguage?)null));
			defaultInterpolatedStringHandler2.AppendLiteral("...");
			return defaultInterpolatedStringHandler2.ToStringAndClear();
		}
		return string.Empty;
	}

	protected override int GetTableColumnCount()
	{
		return 2;
	}

	protected override void SetupColumns(ComboSelectionMode mode)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit((mode == ComboSelectionMode.Radio) ? "RadioButton" : "Checkbox"), (ImGuiTableColumnFlags)8, ImGui.GetTextLineHeightWithSpacing(), 0u);
		ImGui.TableSetupColumn(ImU8String.op_Implicit("Text"), (ImGuiTableColumnFlags)4, 0f, 0u);
	}

	protected override void DrawHeaders()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextRow((ImGuiTableRowFlags)1);
		ImGui.TableNextColumn();
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(2581u)));
	}

	protected override bool DrawDataColumns(LogMessage logMessage, ComboSelectionMode mode, bool isSelected)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		Vector2 cursorPos = ImGui.GetCursorPos();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(13, 1);
		((ImU8String)(ref val)).AppendLiteral("##LogMessage_");
		((ImU8String)(ref val)).AppendFormatted<uint>(((LogMessage)(ref logMessage)).RowId);
		bool result = ImGui.Selectable(val, isSelected, LuminaComboBase<LogMessage>.GetSelectableFlags(mode), default(Vector2));
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 4);
		defaultInterpolatedStringHandler.AppendFormatted(LuminaWrapper.GetAddonText(4098u));
		defaultInterpolatedStringHandler.AppendLiteral(":\n\t");
		LogKind value = ((LogMessage)(ref logMessage)).LogKind.Value;
		ReadOnlySeString val2 = ((LogKind)(ref value)).Format;
		defaultInterpolatedStringHandler.AppendFormatted(((ReadOnlySeString)(ref val2)).ToMacroString().Trim());
		defaultInterpolatedStringHandler.AppendLiteral("\n\n");
		defaultInterpolatedStringHandler.AppendFormatted(LuminaWrapper.GetAddonText(2581u));
		defaultInterpolatedStringHandler.AppendLiteral(":\n\t");
		val2 = ((LogMessage)(ref logMessage)).Text;
		defaultInterpolatedStringHandler.AppendFormatted(((ReadOnlySeString)(ref val2)).ToMacroString());
		ImGuiOm.TooltipHover(defaultInterpolatedStringHandler.ToStringAndClear(), 40f * Globals.GlobalUIScale);
		ImGui.SameLine();
		ImGui.SetCursorPos(cursorPos);
		ReadOnlySpan<byte> readOnlySpan = ReadOnlySeString.op_Implicit(DalamudServiceExtension.Instance<ISeStringEvaluator>().EvaluateFromLogMessage(((LogMessage)(ref logMessage)).RowId, default(Span<SeStringParameter>), (ClientLanguage?)null));
		SeStringDrawParams val3 = default(SeStringDrawParams);
		ImGuiHelpers.SeStringWrapped(readOnlySpan, ref val3, default(ImGuiId), (ImGuiButtonFlags)1);
		return result;
	}
}
