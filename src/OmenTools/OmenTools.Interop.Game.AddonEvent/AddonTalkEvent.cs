using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent.Abstractions;

namespace OmenTools.Interop.Game.AddonEvent;

public class AddonTalkEvent : AddonEventBase
{
	public unsafe static bool ClickNext()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (Addons.Talk == null)
		{
			return false;
		}
		AtkEvent* ptr = stackalloc AtkEvent[1]
		{
			new AtkEvent
			{
				Listener = (AtkEventListener*)Addons.Talk,
				State = new AtkEventState
				{
					StateFlags = (AtkEventStateFlags)132
				},
				Target = &((AtkStage)AtkStage.Instance()).AtkEventTarget
			}
		};
		AtkEventData* ptr2 = (AtkEventData*)stackalloc AtkEventData[1];
		((AtkUnitBase)Addons.Talk).ReceiveEvent((AtkEventType)9, 0, ptr, ptr2);
		return true;
	}
}
