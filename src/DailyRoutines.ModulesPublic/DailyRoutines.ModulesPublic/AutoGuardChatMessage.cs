using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Text.ReadOnly;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;

namespace DailyRoutines.ModulesPublic;

public class AutoGuardChatMessage : ModuleBase
{
	private DalamudLinkPayload payload;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoGuardChatMessageTitle"),
		Description = LanguageManager.Get("AutoGuardChatMessageDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		payload = OmenServiceBase<LinkPayloadManager>.Instance().Reg(delegate
		{
			OmenServiceBase<ChatManager>.Instance().SendCommand("/pdr toggle AutoGuardChatMessage");
		}, out var _);
		OmenServiceBase<ChatManager>.Instance().RegPreExecuteCommandInner(OnPreExecuteCommandInner);
	}

	protected override void Uninit()
	{
		OmenServiceBase<ChatManager>.Instance().Unreg(OnPreExecuteCommandInner);
	}

	private void OnPreExecuteCommandInner(ref bool isPrevented, ref ReadOnlySeString message)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		if (!((ReadOnlySeString)(ref message)).ExtractText().StartsWith('/'))
		{
			isPrevented = true;
			if (Throttler.Shared.Throttle("AutoGuardChatMessage-Notification", 30000u))
			{
				SeStringBuilder val = new SeStringBuilder();
				val.AddText(LanguageManager.Get("AutoGuardChatMessage-Notification")).AddText("\n   " + LanguageManager.Get("Operation") + ": [").Add((Payload)(object)RawPayload.LinkTerminator)
					.Add((Payload)(object)payload)
					.AddUiForeground((ushort)32)
					.AddText(LanguageManager.Get("Disable") + "/" + LanguageManager.Get("Enable"))
					.AddUiForegroundOff()
					.Add((Payload)(object)RawPayload.LinkTerminator)
					.AddText("]");
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(val.Build().Encode()));
			}
		}
	}
}
