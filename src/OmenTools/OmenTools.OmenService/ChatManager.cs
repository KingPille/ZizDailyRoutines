using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using FFXIVClientStructs.FFXIV.Component.Shell;
using InteropGenerator.Runtime;
using Lumina.Text.ReadOnly;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Helpers;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class ChatManager : OmenServiceBase<ChatManager>
{
	private unsafe delegate void ProcessChatBoxEntryDelegate(UIModule* module, Utf8String* message, nint a3, [MarshalAs(UnmanagedType.U1)] bool saveToHistory);

	private unsafe delegate void ExecuteCommandInnerDelegate(ShellCommandModule* module, Utf8String* message, UIModule* uiModule);

	public delegate void PreProcessChatBoxEntryDelagte(ref bool isPrevented, ref ReadOnlySeString message, ref bool saveToHistory);

	public delegate void PostProcessChatBoxEntryDelagte(ReadOnlySeString message, bool saveToHistory);

	public delegate void PreExecuteCommandInnerDelagte(ref bool isPrevented, ref ReadOnlySeString message);

	public delegate void PostExecuteCommandInnerDelagte(ReadOnlySeString message);

	public class ChatManagerConfig : OmenServiceConfig
	{
		public bool ShowProcessChatBoxEntryLog;

		public bool ShowExecuteCommandInnerLog;

		public void Save()
		{
			this.Save(DService.Instance().GetOmenService<ChatManager>());
		}
	}

	private Hook<ProcessChatBoxEntryDelegate> ProcessChatBoxEntryHook;

	private Hook<ExecuteCommandInnerDelegate> ExecuteCommandInnerHook;

	private readonly ConcurrentDictionary<Type, ImmutableList<Delegate>> methodsCollection = new ConcurrentDictionary<Type, ImmutableList<Delegate>>();

	private const int MAX_MACRO_LINES = 15;

	public ChatManagerConfig Config { get; private set; }

	protected unsafe override void Init()
	{
		Config = LoadConfig<ChatManagerConfig>() ?? new ChatManagerConfig();
		ProcessChatBoxEntryHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<ProcessChatBoxEntryDelegate>((IntPtr)DalamudReflector.GetMemberFuncByName(typeof(MemberFunctionPointers), "ProcessChatBoxEntry"), (ProcessChatBoxEntryDelegate)ProcessChatBoxEntryDetour, (HookBackend)0);
		ProcessChatBoxEntryHook.Enable();
		ExecuteCommandInnerHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromAddress<ExecuteCommandInnerDelegate>((IntPtr)DalamudReflector.GetMemberFuncByName(typeof(MemberFunctionPointers), "ExecuteCommandInner"), (ExecuteCommandInnerDelegate)ExecuteCommandInnerDetour, (HookBackend)0);
		ExecuteCommandInnerHook.Enable();
	}

	protected override void Uninit()
	{
		ProcessChatBoxEntryHook?.Dispose();
		ProcessChatBoxEntryHook = null;
		ExecuteCommandInnerHook?.Dispose();
		ExecuteCommandInnerHook = null;
		methodsCollection.Clear();
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

	public bool RegPreProcessChatBoxEntry(PreProcessChatBoxEntryDelagte method, params PreProcessChatBoxEntryDelagte[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostProcessChatBoxEntry(PostProcessChatBoxEntryDelagte method, params PostProcessChatBoxEntryDelagte[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreExecuteCommandInner(PreExecuteCommandInnerDelagte method, params PreExecuteCommandInnerDelagte[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostExecuteCommandInner(PostExecuteCommandInnerDelagte method, params PostExecuteCommandInnerDelagte[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool Unreg(params PreProcessChatBoxEntryDelagte[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostProcessChatBoxEntryDelagte[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreExecuteCommandInnerDelagte[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostExecuteCommandInnerDelagte[] methods)
	{
		return UnregisterGeneric(methods);
	}

	private unsafe void ProcessChatBoxEntryDetour(UIModule* module, Utf8String* message, nint a3, [MarshalAs(UnmanagedType.U1)] bool saveToHistory)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (message == null || !((CStringPointer)(&((Utf8String)message).StringPtr)).HasValue)
		{
			ProcessChatBoxEntryHook.Original(module, message, a3, saveToHistory);
			return;
		}
		bool isPrevented = false;
		ReadOnlySeString val = CStringExtensions.AsReadOnlySeString(((Utf8String)message).StringPtr);
		ReadOnlySeString message2 = CStringExtensions.AsReadOnlySeString(((Utf8String)message).StringPtr);
		if (Config.ShowProcessChatBoxEntryLog)
		{
			DLog.Debug($"[Chat Manager] Process Chat Box Entry\n消息: {message2}\n存储至历史记录: {saveToHistory}");
		}
		if (methodsCollection.TryGetValue(typeof(PreProcessChatBoxEntryDelagte), out ImmutableList<Delegate> value))
		{
			foreach (PreProcessChatBoxEntryDelagte item in value)
			{
				item(ref isPrevented, ref message2, ref saveToHistory);
				if (isPrevented)
				{
					return;
				}
			}
		}
		if (val != message2)
		{
			((Utf8String)message).SetString((ReadOnlySpan<byte>)SeStringExtensions.ToDalamudString(message2).EncodeWithNullTerminator());
		}
		ProcessChatBoxEntryHook.Original(module, message, a3, saveToHistory);
		if (!methodsCollection.TryGetValue(typeof(PostProcessChatBoxEntryDelagte), out ImmutableList<Delegate> value2))
		{
			return;
		}
		foreach (PostProcessChatBoxEntryDelagte item2 in value2)
		{
			item2(message2, saveToHistory);
		}
	}

	private unsafe void ExecuteCommandInnerDetour(ShellCommandModule* module, Utf8String* message, UIModule* uiModule)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		if (message == null || !((CStringPointer)(&((Utf8String)message).StringPtr)).HasValue)
		{
			ExecuteCommandInnerHook.Original(module, message, uiModule);
			return;
		}
		bool isPrevented = false;
		ReadOnlySeString val = CStringExtensions.AsReadOnlySeString(((Utf8String)message).StringPtr);
		ReadOnlySeString message2 = CStringExtensions.AsReadOnlySeString(((Utf8String)message).StringPtr);
		if (Config.ShowExecuteCommandInnerLog)
		{
			DLog.Debug($"[Chat Manager] Execute Command Inner\n消息: {message2}");
		}
		if (methodsCollection.TryGetValue(typeof(PreExecuteCommandInnerDelagte), out ImmutableList<Delegate> value))
		{
			foreach (PreExecuteCommandInnerDelagte item in value)
			{
				item(ref isPrevented, ref message2);
				if (isPrevented)
				{
					return;
				}
			}
		}
		if (val != message2)
		{
			((Utf8String)message).SetString((ReadOnlySpan<byte>)SeStringExtensions.ToDalamudString(message2).EncodeWithNullTerminator());
		}
		ExecuteCommandInnerHook.Original(module, message, uiModule);
		if (!methodsCollection.TryGetValue(typeof(PostExecuteCommandInnerDelagte), out ImmutableList<Delegate> value2))
		{
			return;
		}
		foreach (PostExecuteCommandInnerDelagte item2 in value2)
		{
			item2(message2);
		}
	}

	public void SendMessage(string message, bool saveToHistory = false)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(message);
		SendMessage(bytes, saveToHistory);
	}

	public unsafe void SendMessage(ReadOnlySpan<byte> message, bool saveToHistory = false)
	{
		if (message.Length == 0)
		{
			return;
		}
		Utf8String val = default(Utf8String);
		((Utf8String)(ref val))._002Ector(message);
		try
		{
			ProcessChatBoxEntryDetour(UIModule.Instance(), &val, (nint)(&val), saveToHistory);
		}
		finally
		{
			((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public void SendCommand(string command)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(command);
		SendCommand(bytes);
	}

	public unsafe void SendCommand(ReadOnlySpan<byte> command)
	{
		if (command.Length == 0)
		{
			return;
		}
		Utf8String val = default(Utf8String);
		((Utf8String)(ref val))._002Ector(command);
		try
		{
			ExecuteCommandInnerDetour((ShellCommandModule*)RaptureShellModule.Instance(), &val, UIModule.Instance());
		}
		finally
		{
			((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	public void ExecuteMacro(string commands)
	{
		ArgumentNullException.ThrowIfNull(commands, "commands");
		string[] array = commands.Split(new string[3] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length > 15)
		{
			throw new ArgumentOutOfRangeException("commands", $"宏命令不能超过 {15} 行");
		}
		ExecuteMacro(array);
	}

	public unsafe void ExecuteMacro(string[] commands)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		ArgumentNullException.ThrowIfNull(commands, "commands");
		if (commands.Length > 15)
		{
			throw new ArgumentOutOfRangeException("commands", $"宏命令不能超过 {15} 行");
		}
		if (commands.Length == 0 || RaptureShellModule.Instance() == null)
		{
			return;
		}
		Macro val = default(Macro);
		for (int i = 0; i < commands.Length; i++)
		{
			((Utf8String)(ref ((Macro)(ref val)).Lines[i])).SetString(commands[i]);
			if (i == 0)
			{
				((Utf8String)(ref ((Macro)(ref val)).Lines[i + 1])).SetString("\0");
			}
		}
		ExecuteMacro(&val);
	}

	public unsafe void ExecuteMacro(Macro* macro)
	{
		ArgumentNullException.ThrowIfNull(macro, "macro");
		RaptureShellModule* ptr = RaptureShellModule.Instance();
		if (ptr != null)
		{
			((RaptureShellModule)ptr).ExecuteMacro(macro);
		}
	}

	public unsafe static string SanitiseText(string text)
	{
		Utf8String val = default(Utf8String);
		((Utf8String)(ref val))._002Ector();
		try
		{
			((Utf8String)(ref val)).SetString(text);
			((Utf8String)(ref val)).SanitizeString((AllowedEntities)639, (Utf8String*)null);
			return ((object)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).ToString();
		}
		finally
		{
			((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
