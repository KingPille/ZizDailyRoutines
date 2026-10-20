using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.InstanceContent;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Scheduler.Clip;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Component.Text;
using FFXIVClientStructs.STD;
using InteropGenerator.Runtime;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class LogMessageManager : OmenServiceBase<LogMessageManager>
{
	public delegate void PreLogMessageDelegate(ref bool isPrevented, ref uint logMessageID, ref LogMessageQueueItem item);

	public delegate void PostLogMessageDelegate(uint logMessageID, LogMessageQueueItem item);

	public delegate void PreInstanceContentTextDelegate(ref bool isPrevented, ref uint rowID);

	public delegate void PostInstanceContentTextDelegate(uint rowID);

	private unsafe delegate void UpdateDelegate(RaptureLogModule* module);

	public class LogMessageManagerConfig : OmenServiceConfig
	{
		public bool ShowLogMessageLog;

		public bool ShowInstanceContentTextLog;

		public void Save()
		{
			this.Save(DService.Instance().GetOmenService<LogMessageManager>());
		}
	}

	[ThreadStatic]
	private static StringBuilder? LogMessageDebugBuilder;

	private unsafe Utf8String* EmptyInstanceContentText;

	private Hook<GetInstanceContentText>? GetInstanceContentTextHook;

	private Hook<ResolveText>? ResolveTextHook;

	private Hook<UpdateDelegate>? UpdateHook;

	private readonly ConcurrentDictionary<Type, ImmutableList<Delegate>> methodsCollection = new ConcurrentDictionary<Type, ImmutableList<Delegate>>();

	private readonly HashSet<nint> seenLogMessageObjects = new HashSet<nint>();

	public LogMessageManagerConfig Config { get; private set; }

	protected unsafe override void Init()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		Config = LoadConfig<LogMessageManagerConfig>() ?? new LogMessageManagerConfig();
		if (UpdateHook == null)
		{
			UpdateHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<UpdateDelegate>(typeof(MemberFunctionPointers), "Update", UpdateDetour);
		}
		UpdateHook?.Enable();
		if (EmptyInstanceContentText == null)
		{
			EmptyInstanceContentText = Utf8String.CreateEmpty((IMemorySpace*)null);
		}
		GetInstanceContentTextHook = (*InstanceContentDirector.StaticVirtualTablePointer).HookVFuncFromName<InstanceContentDirectorVirtualTable, GetInstanceContentText>("GetInstanceContentText", new GetInstanceContentText(GetInstanceContentTextDetour));
		GetInstanceContentTextHook.Enable();
		ResolveTextHook = (*InstanceContentTextClip.StaticVirtualTablePointer).HookVFuncFromName<InstanceContentTextClipVirtualTable, ResolveText>("ResolveText", new ResolveText(ResolveInstanceContentTextClipDetour));
		ResolveTextHook.Enable();
	}

	protected unsafe override void Uninit()
	{
		UpdateHook?.Dispose();
		UpdateHook = null;
		GetInstanceContentTextHook?.Dispose();
		GetInstanceContentTextHook = null;
		ResolveTextHook?.Dispose();
		ResolveTextHook = null;
		if (EmptyInstanceContentText != null)
		{
			((Utf8String)EmptyInstanceContentText).Dtor(true);
			EmptyInstanceContentText = null;
		}
		methodsCollection.Clear();
	}

	private unsafe void UpdateDetour(RaptureLogModule* module)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<LogMessageQueueItem> enumerator = ((RaptureLogModule)module).LogMessageQueue.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ref LogMessageQueueItem current = ref enumerator.Current;
				if (!seenLogMessageObjects.Contains((nint)Unsafe.AsPointer(in current)))
				{
					ProcessLogMessage(ref current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		UpdateHook.Original(module);
		seenLogMessageObjects.Clear();
		enumerator = ((RaptureLogModule)module).LogMessageQueue.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ref LogMessageQueueItem current2 = ref enumerator.Current;
				seenLogMessageObjects.Add((nint)Unsafe.AsPointer(in current2));
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void ProcessLogMessage(ref LogMessageQueueItem item)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (item.LogMessageId != 0)
		{
			if (!OnPreReceiveLogMessage(ref item))
			{
				item.LogMessageId = 0u;
			}
			else if (item.LogMessageId != 0)
			{
				OnPostReceiveLogMessage(item);
			}
		}
	}

	private unsafe Utf8String* GetInstanceContentTextDetour(InstanceContentDirector* director, uint rowID)
	{
		if (!OnPreInstanceContentText(ref rowID))
		{
			return EmptyInstanceContentText;
		}
		Utf8String* result = GetInstanceContentTextHook.Original.Invoke(director, rowID);
		OnPostInstanceContentText(rowID);
		return result;
	}

	private unsafe bool ResolveInstanceContentTextClipDetour(InstanceContentTextClip* clip)
	{
		if (clip == null || ((InstanceContentTextClip)clip).Data == null)
		{
			return ResolveTextHook.Original.Invoke(clip);
		}
		uint rowID = ((InstanceContentTextClipData)((InstanceContentTextClip)clip).TypedData).RowId;
		if (!OnPreInstanceContentText(ref rowID))
		{
			return true;
		}
		((InstanceContentTextClipData)((InstanceContentTextClip)clip).TypedData).RowId = rowID;
		bool num = ResolveTextHook.Original.Invoke(clip);
		if (num)
		{
			OnPostInstanceContentText(rowID);
		}
		return num;
	}

	private bool OnPreInstanceContentText(ref uint rowID)
	{
		if (Config.ShowInstanceContentTextLog)
		{
			DLog.Debug("[Log Message Manager] Instance Content Text\n" + $"ID: {rowID}");
		}
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreInstanceContentTextDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreInstanceContentTextDelegate item in value)
			{
				item(ref isPrevented, ref rowID);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void OnPostInstanceContentText(uint rowID)
	{
		if (!methodsCollection.TryGetValue(typeof(PostInstanceContentTextDelegate), out ImmutableList<Delegate> value))
		{
			return;
		}
		foreach (PostInstanceContentTextDelegate item in value)
		{
			item(rowID);
		}
	}

	private unsafe bool OnPreReceiveLogMessage(ref LogMessageQueueItem item)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Expected I4, but got Unknown
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		if (Config.ShowLogMessageLog)
		{
			StringBuilder stringBuilder = RentLogMessageDebugBuilder();
			try
			{
				stringBuilder.AppendLine("[Log Message Manager] Log Message");
				stringBuilder.Append("ID: ").Append(item.LogMessageId).AppendLine();
				stringBuilder.AppendLine("预览:");
				stringBuilder.Append('\t').Append(item.ToReadOnlySeString()).AppendLine();
				if ((int)item.SourceKind != 0)
				{
					stringBuilder.AppendLine("来源:");
					stringBuilder.Append("\t分类: ").Append(item.SourceKind).AppendLine();
					stringBuilder.Append("\t名称: ").Append(((LogMessageQueueItem)(ref item)).SourceNameString).AppendLine();
					stringBuilder.Append("\t服务器: ").Append(LuminaWrapper.GetWorldName(item.SourceHomeWorld)).Append(" (")
						.Append(item.SourceHomeWorld)
						.AppendLine(")");
					stringBuilder.Append("\t玩家: ").Append(item.SourceIsPlayer).AppendLine();
					stringBuilder.Append("\t性别: ").Append(item.SourceSex).AppendLine();
					stringBuilder.Append("\tObjStrID: ").Append(item.SourceObjStrId).Append(' ')
						.Append(item.SourceObjStrId.FromObjStrID())
						.AppendLine();
				}
				if ((int)item.TargetKind != 0)
				{
					stringBuilder.AppendLine("目标:");
					stringBuilder.Append("\t分类: ").Append(item.TargetKind).AppendLine();
					stringBuilder.Append("\t名称: ").Append(((LogMessageQueueItem)(ref item)).TargetNameString).AppendLine();
					stringBuilder.Append("\t服务器: ").Append(LuminaWrapper.GetWorldName(item.TargetHomeWorld)).Append(" (")
						.Append(item.TargetHomeWorld)
						.AppendLine(")");
					stringBuilder.Append("\t玩家: ").Append(item.TargetIsPlayer).AppendLine();
					stringBuilder.Append("\t性别: ").Append(item.TargetSex).AppendLine();
					stringBuilder.Append("\tObjStrID: ").Append(item.TargetObjStrId).Append(' ')
						.Append(item.TargetObjStrId.FromObjStrID())
						.AppendLine();
				}
				if (item.Parameters.Count > 0)
				{
					stringBuilder.AppendLine("参数:");
					for (int i = 0; i < item.Parameters.Count; i++)
					{
						TextParameter val = item.Parameters[i];
						TextParameterType type = val.Type;
						switch (type - -1)
						{
						case 2:
						{
							if (val.ReferencedUtf8StringValue == null || ((ReferencedUtf8String)val.ReferencedUtf8StringValue).RefCount == 0)
							{
								break;
							}
							stringBuilder.Append("\t[").Append(i).Append("] (")
								.Append(val.Type)
								.AppendLine("):");
							for (int j = 0; j < ((ReferencedUtf8String)val.ReferencedUtf8StringValue).RefCount; j++)
							{
								ReferencedUtf8String val2 = ((ReferencedUtf8String*)val.ReferencedUtf8StringValue)[j];
								if (!val2.Utf8String.IsEmpty && ((CStringPointer)(ref val2.Utf8String.StringPtr)).HasValue)
								{
									stringBuilder.Append("\t\t[").Append(i).Append("]: ")
										.Append(CStringExtensions.ExtractText(val2.Utf8String.StringPtr))
										.AppendLine();
								}
							}
							break;
						}
						case 3:
							if (((CStringPointer)(ref val.StringValue)).HasValue)
							{
								stringBuilder.Append("\t[").Append(i).Append("] (")
									.Append(val.Type)
									.Append("): ")
									.Append(CStringExtensions.ExtractText(val.StringValue))
									.AppendLine();
							}
							break;
						default:
							stringBuilder.Append("\t[").Append(i).Append("] (")
								.Append(val.Type)
								.Append("): ")
								.Append(val.IntValue)
								.AppendLine();
							break;
						case 0:
							break;
						}
					}
				}
				TrimEndingNewLine(stringBuilder);
				DLog.Debug(stringBuilder.ToString());
			}
			finally
			{
				ReturnLogMessageDebugBuilder(stringBuilder);
			}
		}
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreLogMessageDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreLogMessageDelegate item2 in value)
			{
				item2(ref isPrevented, ref item.LogMessageId, ref item);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		return true;
	}

	private static StringBuilder RentLogMessageDebugBuilder()
	{
		StringBuilder logMessageDebugBuilder = LogMessageDebugBuilder;
		if (logMessageDebugBuilder == null)
		{
			return new StringBuilder(1024);
		}
		LogMessageDebugBuilder = null;
		logMessageDebugBuilder.Clear();
		return logMessageDebugBuilder;
	}

	private static void ReturnLogMessageDebugBuilder(StringBuilder sb)
	{
		if (sb.Capacity <= 32768)
		{
			sb.Clear();
			LogMessageDebugBuilder = sb;
		}
	}

	private static void TrimEndingNewLine(StringBuilder sb)
	{
		if (sb.Length == 0)
		{
			return;
		}
		int num = sb.Length - 1;
		if (sb[num] != '\n')
		{
			return;
		}
		sb.Length = num;
		if (sb.Length > 0)
		{
			if (sb[sb.Length - 1] == '\r')
			{
				sb.Length--;
			}
		}
	}

	private void OnPostReceiveLogMessage(LogMessageQueueItem item)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!methodsCollection.TryGetValue(typeof(PostLogMessageDelegate), out ImmutableList<Delegate> value))
		{
			return;
		}
		foreach (PostLogMessageDelegate item2 in value)
		{
			item2(item.LogMessageId, item);
		}
	}

	private bool RegisterGeneric<T>(T method, params T[] methods) where T : Delegate
	{
		Type typeFromHandle = typeof(T);
		methodsCollection.AddOrUpdate(typeFromHandle, delegate
		{
			ImmutableList<Delegate> immutableList = ImmutableList.Create((Delegate)method);
			return (methods.Length == 0) ? immutableList : immutableList.AddRange(methods);
		}, delegate(Type _, ImmutableList<Delegate> currentList)
		{
			ImmutableList<Delegate> immutableList = currentList.Add(method);
			return (methods.Length == 0) ? immutableList : immutableList.AddRange(methods);
		});
		return true;
	}

	private bool UnregisterGeneric<T>(params T[] methods) where T : Delegate
	{
		if (methods == null || methods.Length <= 0)
		{
			return false;
		}
		Type typeFromHandle = typeof(T);
		ImmutableList<Delegate> value;
		while (methodsCollection.TryGetValue(typeFromHandle, out value))
		{
			ImmutableList<Delegate> immutableList = value.RemoveRange(methods);
			if (immutableList == value)
			{
				return false;
			}
			if (immutableList.IsEmpty)
			{
				KeyValuePair<Type, ImmutableList<Delegate>> item = new KeyValuePair<Type, ImmutableList<Delegate>>(typeFromHandle, value);
				if (((ICollection<KeyValuePair<Type, ImmutableList<Delegate>>>)methodsCollection).Remove(item))
				{
					return true;
				}
			}
			else if (methodsCollection.TryUpdate(typeFromHandle, immutableList, value))
			{
				return true;
			}
		}
		return false;
	}

	public bool RegPre(PreLogMessageDelegate method, params PreLogMessageDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPost(PostLogMessageDelegate method, params PostLogMessageDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreInstanceContentText(PreInstanceContentTextDelegate method, params PreInstanceContentTextDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostInstanceContentText(PostInstanceContentTextDelegate method, params PostInstanceContentTextDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool Unreg(params PreLogMessageDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostLogMessageDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreInstanceContentTextDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostInstanceContentTextDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}
}
