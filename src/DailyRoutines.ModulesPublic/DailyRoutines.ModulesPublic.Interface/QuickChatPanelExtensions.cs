using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

internal static class _003CQuickChatPanel_003EF9F6B91AADA529344169EE5B602E47FE6C9C09CBC9B72240AFB138B4D9328EA52__QuickChatPanelExtensions
{
	public unsafe static QuickChatPanel.SavedMacro ToSavedMacro(this Macro macro)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new QuickChatPanel.SavedMacro
		{
			Name = ((object)(*(Utf8String*)(&macro.Name))/*cast due to constrained. prefix*/).ToString(),
			IconID = macro.IconId,
			LastUpdateTime = OmenServiceBase<StandardTimeManager>.Instance().Now
		};
	}
}
