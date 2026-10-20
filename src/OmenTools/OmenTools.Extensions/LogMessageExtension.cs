using System;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.Interop;
using InteropGenerator.Runtime;
using Lumina.Excel.Sheets;
using Lumina.Text;
using Lumina.Text.ReadOnly;

namespace OmenTools.Extensions;

public static class LogMessageExtension
{
	extension(scoped ref LogMessageQueueItem item)
	{
		public unsafe ReadOnlySeString ToReadOnlySeString()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Invalid comparison between Unknown and I4
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Invalid comparison between Unknown and I4
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_029b: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			fixed (LogMessageQueueItem* ptr = &item)
			{
				if (ptr == null || item.LogMessageId == 0)
				{
					return default(ReadOnlySeString);
				}
				RaptureLogModule* ptr2 = RaptureLogModule.Instance();
				if ((int)item.SourceKind != 0)
				{
					byte* ptr3 = SpanExtensions.GetPointer<byte>(((LogMessageQueueItem)(ref item)).SourceName, 0);
					if (item.SourceIsPlayer)
					{
						Utf8String* pointer = SpanExtensions.GetPointer<Utf8String>(((RaptureLogModule)ptr2).TempParseMessage, 8);
						((RaptureLogModule)ptr2).FormatPlayerLink(CStringPointer.op_Implicit(ptr3), pointer, CStringPointer.op_Implicit((byte*)null), 0u, (int)item.SourceKind != 1, item.SourceHomeWorld, false, CStringPointer.op_Implicit((byte*)null), false);
						if (item.SourceHomeWorld != 0 && item.SourceHomeWorld != ((LobbyData)(&((AgentLobby)AgentLobby.Instance()).LobbyData)).HomeWorldId)
						{
							Utf8String* pointer2 = SpanExtensions.GetPointer<Utf8String>(((RaptureTextModule)((RaptureLogModule)ptr2).RaptureTextModule).UnkStrings0, 3);
							if (!((CStringPointer)(&((Utf8String)pointer2).StringPtr)).HasValue)
							{
								((RaptureTextModule)((RaptureLogModule)ptr2).RaptureTextModule).ProcessMacroCode(pointer2, "<icon(88)>\0"u8);
							}
							((Utf8String)pointer).Append(pointer2);
							World* ptr4 = default(World*);
							if (((WorldHelper)((UIModule)((RaptureLogModule)ptr2).UIModule).GetWorldHelper()).AllWorlds.TryGetValuePointer(ref item.SourceHomeWorld, ref ptr4))
							{
								((Utf8String)pointer).ConcatCStr((ReadOnlySpan<byte>)((World)ptr4).Name);
							}
						}
						ptr3 = CStringPointer.op_Implicit(((Utf8String)pointer).StringPtr);
					}
					((RaptureTextModule)((RaptureLogModule)ptr2).RaptureTextModule).SetGlobalTempEntity1(CStringPointer.op_Implicit(ptr3), (int)item.SourceSex, item.SourceObjStrId);
				}
				if ((int)item.TargetKind != 0)
				{
					byte* ptr5 = SpanExtensions.GetPointer<byte>(((LogMessageQueueItem)(ref item)).TargetName, 0);
					if (item.TargetIsPlayer)
					{
						Utf8String* pointer3 = SpanExtensions.GetPointer<Utf8String>(((RaptureLogModule)ptr2).TempParseMessage, 0);
						((RaptureLogModule)ptr2).FormatPlayerLink(CStringPointer.op_Implicit(ptr5), pointer3, CStringPointer.op_Implicit((byte*)null), 0u, (int)item.TargetKind != 1, item.TargetHomeWorld, false, CStringPointer.op_Implicit((byte*)null), false);
						if (item.TargetHomeWorld != 0 && item.TargetHomeWorld != ((LobbyData)(&((AgentLobby)AgentLobby.Instance()).LobbyData)).HomeWorldId)
						{
							Utf8String* pointer4 = SpanExtensions.GetPointer<Utf8String>(((RaptureTextModule)((RaptureLogModule)ptr2).RaptureTextModule).UnkStrings0, 3);
							if (!((CStringPointer)(&((Utf8String)pointer4).StringPtr)).HasValue)
							{
								((RaptureTextModule)((RaptureLogModule)ptr2).RaptureTextModule).ProcessMacroCode(pointer4, "<icon(88)>\0"u8);
							}
							((Utf8String)pointer3).Append(pointer4);
							World* ptr6 = default(World*);
							if (((WorldHelper)((UIModule)((RaptureLogModule)ptr2).UIModule).GetWorldHelper()).AllWorlds.TryGetValuePointer(ref item.TargetHomeWorld, ref ptr6))
							{
								((Utf8String)pointer3).ConcatCStr((ReadOnlySpan<byte>)((World)ptr6).Name);
							}
						}
						ptr5 = CStringPointer.op_Implicit(((Utf8String)pointer3).StringPtr);
					}
					((RaptureTextModule)((RaptureLogModule)ptr2).RaptureTextModule).SetGlobalTempEntity2(CStringPointer.op_Implicit(ptr5), (int)item.TargetSex, item.TargetObjStrId);
				}
				RentedSeStringBuilder val = default(RentedSeStringBuilder);
				((RentedSeStringBuilder)(ref val))._002Ector();
				try
				{
					Utf8String val2 = new Utf8String();
					try
					{
						RaptureTextModule* raptureTextModule = ((RaptureLogModule)ptr2).RaptureTextModule;
						SeStringBuilder builder = ((RentedSeStringBuilder)(ref val)).Builder;
						LogMessage value = item.LogMessageId.ToLuminaRowRef<LogMessage>().Value;
						((RaptureTextModule)raptureTextModule).FormatString(builder.Append(((LogMessage)(ref value)).Text).GetViewAsSpan(), &((LogMessageQueueItem)ptr).Parameters, &val2);
						return new ReadOnlySeString(((Utf8String)(ref val2)).AsSpan());
					}
					finally
					{
						((IDisposable)(*(Utf8String*)(&val2))/*cast due to constrained. prefix*/).Dispose();
					}
				}
				finally
				{
					((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
				}
			}
		}
	}
}
