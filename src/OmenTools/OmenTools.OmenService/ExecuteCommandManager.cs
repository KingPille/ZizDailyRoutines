using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class ExecuteCommandManager : OmenServiceBase<ExecuteCommandManager>
{
	[return: MarshalAs(UnmanagedType.U1)]
	private delegate bool ExecuteCommandDelegate(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4);

	[return: MarshalAs(UnmanagedType.U1)]
	private delegate bool ExecuteCommandComplexDelegate(ExecuteCommandComplexFlag command, long target, uint param1, uint param2, uint param3, uint param4);

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe delegate bool ExecuteCommandComplexLocationDelegate(ExecuteCommandComplexFlag command, Vector3* location, uint param1, uint param2, uint param3, uint param4);

	public delegate void PreExecuteCommandDelegate(ref bool isPrevented, ref ExecuteCommandFlag command, ref uint param1, ref uint param2, ref uint param3, ref uint param4);

	public delegate void PostExecuteCommandDelegate(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4);

	public delegate void PreExecuteCommandComplexDelegate(ref bool isPrevented, ref ExecuteCommandComplexFlag command, ref long target, ref uint param1, ref uint param2, ref uint param3, ref uint param4);

	public delegate void PostExecuteCommandComplexDelegate(ExecuteCommandComplexFlag command, long target, uint param1, uint param2, uint param3, uint param4);

	public delegate void PreExecuteCommandComplexLocationDelegate(ref bool isPrevented, ref ExecuteCommandComplexFlag command, ref Vector3 location, ref uint param1, ref uint param2, ref uint param3, ref uint param4);

	public delegate void PostExecuteCommandComplexLocationDelegate(ExecuteCommandComplexFlag command, Vector3 location, uint param1, uint param2, uint param3, uint param4);

	public class ExecuteCommandManagerConfig : OmenServiceConfig
	{
		public bool ShowExecuteCommandLog;

		public bool ShowExecuteCommandComplexLog;

		public bool ShowExecuteCommandComplexLocationLog;

		public void Save()
		{
			this.Save(DService.Instance().GetOmenService<ExecuteCommandManager>());
		}
	}

	private Hook<ExecuteCommandDelegate>? ExecuteCommandHook;

	private static readonly CompSig ExecuteCommandComplexSig = new CompSig("E8 ?? ?? ?? ?? 80 7D ?? ?? 74 ?? 41 0F B6 45");

	private Hook<ExecuteCommandComplexDelegate>? ExecuteCommandComplexHook;

	private Hook<ExecuteCommandComplexLocationDelegate>? ExecuteCommandComplexLocationHook;

	private readonly ConcurrentDictionary<Type, ImmutableList<Delegate>> methodsCollection = new ConcurrentDictionary<Type, ImmutableList<Delegate>>();

	public ExecuteCommandManagerConfig Config { get; private set; }

	protected unsafe override void Init()
	{
		Config = LoadConfig<ExecuteCommandManagerConfig>() ?? new ExecuteCommandManagerConfig();
		if (ExecuteCommandHook == null)
		{
			ExecuteCommandHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<ExecuteCommandDelegate>(typeof(MemberFunctionPointers), "ExecuteCommand", ExecuteCommandDetour);
		}
		ExecuteCommandHook?.Enable();
		if (ExecuteCommandComplexHook == null)
		{
			ExecuteCommandComplexHook = ExecuteCommandComplexSig.GetHook<ExecuteCommandComplexDelegate>(ExecuteCommandComplexDetour);
		}
		ExecuteCommandComplexHook?.Enable();
		if (ExecuteCommandComplexLocationHook == null)
		{
			ExecuteCommandComplexLocationHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<ExecuteCommandComplexLocationDelegate>(typeof(MemberFunctionPointers), "ExecuteLocationCommand", ExecuteCommandComplexLocationDetour);
		}
		ExecuteCommandComplexLocationHook?.Enable();
	}

	protected override void Uninit()
	{
		ExecuteCommandHook?.Dispose();
		ExecuteCommandHook = null;
		ExecuteCommandComplexHook?.Dispose();
		ExecuteCommandComplexHook = null;
		ExecuteCommandComplexLocationHook?.Dispose();
		ExecuteCommandComplexLocationHook = null;
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

	public bool RegPre(PreExecuteCommandDelegate method, params PreExecuteCommandDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPost(PostExecuteCommandDelegate method, params PostExecuteCommandDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreComplex(PreExecuteCommandComplexDelegate method, params PreExecuteCommandComplexDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostComplex(PostExecuteCommandComplexDelegate method, params PostExecuteCommandComplexDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreComplexLocation(PreExecuteCommandComplexLocationDelegate method, params PreExecuteCommandComplexLocationDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostComplexLocation(PostExecuteCommandComplexLocationDelegate method, params PostExecuteCommandComplexLocationDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool Unreg(params PreExecuteCommandDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostExecuteCommandDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreExecuteCommandComplexDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostExecuteCommandComplexDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreExecuteCommandComplexLocationDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostExecuteCommandComplexLocationDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool ExecuteCommandDetour(ExecuteCommandFlag command, uint param1, uint param2, uint param3, uint param4)
	{
		if (Config.ShowExecuteCommandLog)
		{
			DLog.Debug($"[Execute Command Manager] Execute Command\n命令:{command}({(uint)command}) | p1:{param1} | p2:{param2} | p3:{param3} | p4:{param4}");
		}
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreExecuteCommandDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreExecuteCommandDelegate item in value)
			{
				item(ref isPrevented, ref command, ref param1, ref param2, ref param3, ref param4);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		bool result = ExecuteCommandHook.Original(command, param1, param2, param3, param4);
		if (methodsCollection.TryGetValue(typeof(PostExecuteCommandDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostExecuteCommandDelegate item2 in value2)
			{
				item2(command, param1, param2, param3, param4);
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private bool ExecuteCommandComplexDetour(ExecuteCommandComplexFlag command, long target, uint param1, uint param2, uint param3, uint param4)
	{
		if (Config.ShowExecuteCommandComplexLog)
		{
			DLog.Debug($"[Execute Command Manager] Execute Command Complex\n命令:{command}({(uint)command}) | 目标:{target:X} | p1:{param1} | p2:{param2} | p3:{param3} | p4:{param4}");
		}
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreExecuteCommandComplexDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreExecuteCommandComplexDelegate item in value)
			{
				item(ref isPrevented, ref command, ref target, ref param1, ref param2, ref param3, ref param4);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		bool result = ExecuteCommandComplexHook.Original(command, target, param1, param2, param3, param4);
		if (methodsCollection.TryGetValue(typeof(PostExecuteCommandComplexDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostExecuteCommandComplexDelegate item2 in value2)
			{
				item2(command, target, param1, param2, param3, param4);
			}
		}
		return result;
	}

	[return: MarshalAs(UnmanagedType.U1)]
	private unsafe bool ExecuteCommandComplexLocationDetour(ExecuteCommandComplexFlag command, Vector3* location, uint param1, uint param2, uint param3, uint param4)
	{
		Vector3 location2 = *location;
		if (Config.ShowExecuteCommandComplexLog)
		{
			DLog.Debug($"[Execute Command Manager] Execute Command Complex Location\n命令:{command}({(uint)command}) | 地点:{location2} | p1:{param1} | p2:{param2} | p3:{param3} | p4:{param4}");
		}
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreExecuteCommandComplexLocationDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreExecuteCommandComplexLocationDelegate item in value)
			{
				item(ref isPrevented, ref command, ref location2, ref param1, ref param2, ref param3, ref param4);
				if (isPrevented)
				{
					return false;
				}
			}
		}
		bool result = ExecuteCommandComplexLocationHook.Original(command, &location2, param1, param2, param3, param4);
		if (methodsCollection.TryGetValue(typeof(PostExecuteCommandComplexLocationDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostExecuteCommandComplexLocationDelegate item2 in value2)
			{
				item2(command, location2, param1, param2, param3, param4);
			}
		}
		return result;
	}

	public void ExecuteCommand(ExecuteCommandFlag command, uint param1 = 0u, uint param2 = 0u, uint param3 = 0u, uint param4 = 0u)
	{
		ExecuteCommandHook.Original(command, param1, param2, param3, param4);
		if (!methodsCollection.TryGetValue(typeof(PostExecuteCommandDelegate), out ImmutableList<Delegate> value))
		{
			return;
		}
		foreach (PostExecuteCommandDelegate item in value)
		{
			item(command, param1, param2, param3, param4);
		}
	}

	public void ExecuteCommandComplex(ExecuteCommandComplexFlag command, long target = 3758096384L, uint param1 = 0u, uint param2 = 0u, uint param3 = 0u, uint param4 = 0u)
	{
		ExecuteCommandComplexHook.Original(command, target, param1, param2, param3, param4);
		if (!methodsCollection.TryGetValue(typeof(PostExecuteCommandComplexDelegate), out ImmutableList<Delegate> value))
		{
			return;
		}
		foreach (PostExecuteCommandComplexDelegate item in value)
		{
			item(command, target, param1, param2, param3, param4);
		}
	}

	public unsafe void ExecuteCommandComplexLocation(ExecuteCommandComplexFlag command, Vector3 location = default(Vector3), uint param1 = 0u, uint param2 = 0u, uint param3 = 0u, uint param4 = 0u)
	{
		ExecuteCommandComplexLocationHook.Original(command, &location, param1, param2, param3, param4);
		if (!methodsCollection.TryGetValue(typeof(PostExecuteCommandComplexLocationDelegate), out ImmutableList<Delegate> value))
		{
			return;
		}
		foreach (PostExecuteCommandComplexLocationDelegate item in value)
		{
			item(command, location, param1, param2, param3, param4);
		}
	}
}
