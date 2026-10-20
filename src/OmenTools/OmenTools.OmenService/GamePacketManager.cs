using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Application.Network;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Network;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Info.Game.Packets.Abstractions;
using OmenTools.Info.Game.Packets.Upstream;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class GamePacketManager : OmenServiceBase<GamePacketManager>
{
	public delegate void PreSendPacketDelegate(ref bool isPrevented, int opcode, ref nint packet, ref bool isPrioritize);

	public delegate void PostSendPacketDelegate(int opcode, nint packet, bool isPrioritize);

	public delegate void PreReceivePacketDelegate(ref bool isPrevented, int opcode, ref nint packet);

	public delegate void PostReceivePacketDelegate(int opcode, nint packet);

	private unsafe delegate void* SendPacketDelegate(NetworkModuleProxy* module, byte* packet, uint a3, uint a4);

	private unsafe delegate void ReceivePacketInternalDelegate(PacketDispatcher* dispatcher, uint targetID, nint packet);

	private unsafe delegate void PacketLogger(byte* packet);

	private class PacketHandler(Func<bool> shouldLog, PacketLogger log)
	{
		public Func<bool> ShouldLog { get; } = shouldLog;

		public PacketLogger Log { get; } = log;
	}

	public class GamePacketManagerConfig : OmenServiceConfig
	{
		public bool ShowGamePacketOpcodeLog;

		public bool ShowGamePacketEventStartLog;

		public bool ShowGamePacketEventActionLog;

		public bool ShowGamePacketEventCompleteLog;

		public bool ShowGamePacketDiveStartLog;

		public bool ShowGamePacketPositionUpdateLog;

		public bool ShowGamePacketTreasureOpenLog;

		public bool ShowGamePacketHeartbeatLog;

		public bool ShowGamePacketUseActionLog;

		public bool ShowGamePacketUseActionLocationLog;

		public bool ShowGamePacketMJIInteractLog;

		public bool ShowGamePacketExecuteCommandLog;

		public bool ShowGamePacketCharaCardOpenLog;

		public bool ShowGamePacketHandOverItemLog;

		public bool ShowReceivedPacketOpcodeLog;

		public void Save()
		{
			this.Save(DService.Instance().GetOmenService<GamePacketManager>());
		}
	}

	private static readonly SendPacketDelegate? SendPacket = new CompSig("E8 ?? ?? ?? ?? 48 8B D6 48 8B CF E8 ?? ?? ?? ?? 48 8B 8C 24").GetDelegate<SendPacketDelegate>();

	private Hook<SendPacket>? SendPacketInternalHook;

	private Hook<ReceivePacketInternalDelegate>? ReceivePacketInternalHook;

	private readonly ConcurrentDictionary<Type, ImmutableList<Delegate>> methodsCollection = new ConcurrentDictionary<Type, ImmutableList<Delegate>>();

	private unsafe static readonly Dictionary<int, PacketHandler> PacketHandlers = new Dictionary<int, PacketHandler>
	{
		[UpstreamOpcode.EventCompleteOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketEventCompleteLog, LogPacket<EventCompletePackt>),
		[UpstreamOpcode.EventStartOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketEventStartLog, LogPacket<EventStartPackt>),
		[UpstreamOpcode.EventActionOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketEventActionLog, LogPacket<EventActionPacket>),
		[UpstreamOpcode.DiveStartOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketDiveStartLog, LogPacket<DiveStartPacket>),
		[UpstreamOpcode.PositionUpdateOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketPositionUpdateLog, LogPacket<PositionUpdatePacket>),
		[UpstreamOpcode.PositionUpdateInstanceOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketPositionUpdateLog, LogPacket<PositionUpdateInstancePacket>),
		[UpstreamOpcode.TreasureOpenOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketTreasureOpenLog, LogPacket<TreasureOpenPacket>),
		[UpstreamOpcode.HeartbeatOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketHeartbeatLog, LogPacket<HeartbeatPacket>),
		[UpstreamOpcode.UseActionOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketUseActionLog, LogPacket<UseActionPacket>),
		[UpstreamOpcode.UseActionLocationOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketUseActionLocationLog, LogPacket<UseActionLocationPacket>),
		[UpstreamOpcode.MJIInteractOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketMJIInteractLog, LogPacket<MJIInteractPacket>),
		[UpstreamOpcode.ExecuteCommandOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketExecuteCommandLog, LogPacket<ExecuteCommandPacket>),
		[UpstreamOpcode.CharaCardOpenOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketCharaCardOpenLog, LogPacket<CharaCardOpenPacket>),
		[UpstreamOpcode.HandOverItemOpcode] = new PacketHandler(() => OmenServiceBase<GamePacketManager>.Instance().Config.ShowGamePacketHandOverItemLog, LogPacket<HandOverItemPacket>)
	};

	public GamePacketManagerConfig Config { get; private set; }

	public unsafe void SendPackt<T>(T data) where T : unmanaged, IUpstreamPacket
	{
		byte* ptr = (byte*)(&data);
		if (ptr != null && *(int*)ptr != 0)
		{
			SendPacket(((Framework)Framework.Instance()).NetworkModuleProxy, ptr, 0u, 159868227u);
		}
	}

	public bool RegPreSendPacket(PreSendPacketDelegate method, params PreSendPacketDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostSendPacket(PostSendPacketDelegate method, params PostSendPacketDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPreReceivePacket(PreReceivePacketDelegate method, params PreReceivePacketDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegPostReceivePacket(PostReceivePacketDelegate method, params PostReceivePacketDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool Unreg(params PreSendPacketDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostSendPacketDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PreReceivePacketDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params PostReceivePacketDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	protected unsafe override void Init()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		Config = LoadConfig<GamePacketManagerConfig>() ?? new GamePacketManagerConfig();
		SendPacketInternalHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<SendPacket>(typeof(MemberFunctionPointers), "SendPacket", new SendPacket(SendPacketInternalDetour));
		SendPacketInternalHook.Enable();
		ReceivePacketInternalHook = (*PacketDispatcher.StaticVirtualTablePointer).HookVFuncFromName<PacketDispatcherVirtualTable, ReceivePacketInternalDelegate>("OnReceivePacket", (ReceivePacketInternalDelegate)ReceivePacketInternalDetour);
		ReceivePacketInternalHook.Enable();
	}

	protected override void Uninit()
	{
		SendPacketInternalHook?.Dispose();
		SendPacketInternalHook = null;
		ReceivePacketInternalHook?.Dispose();
		ReceivePacketInternalHook = null;
		methodsCollection.Clear();
	}

	private unsafe bool SendPacketInternalDetour(ZoneClient* zoneClient, nint packet, uint a3, uint a4, bool isPrioritize)
	{
		ushort opcode = *(ushort*)packet;
		LogKnownGamePacket(*(ushort*)packet, (byte*)packet, isPrioritize);
		bool isPrevented = false;
		if (a4 != 159868227 && methodsCollection.TryGetValue(typeof(PreSendPacketDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreSendPacketDelegate item in value)
			{
				item(ref isPrevented, opcode, ref packet, ref isPrioritize);
				if (isPrevented)
				{
					if (Config.ShowReceivedPacketOpcodeLog)
					{
						DLog.Debug("[Game Packet Manager] 上方该包已被阻止发送");
					}
					return false;
				}
			}
		}
		if (a4 == 159868227)
		{
			if (!isPrioritize)
			{
				isPrioritize = true;
			}
		}
		else
		{
			HandlePacketPriority(ref isPrioritize);
		}
		bool result = SendPacketInternalHook.Original.Invoke(zoneClient, (IntPtr)packet, a3, a4, isPrioritize);
		if (a4 != 159868227 && methodsCollection.TryGetValue(typeof(PostSendPacketDelegate), out ImmutableList<Delegate> value2))
		{
			foreach (PostSendPacketDelegate item2 in value2)
			{
				item2(*(ushort*)packet, packet, isPrioritize);
			}
		}
		return result;
	}

	private unsafe void ReceivePacketInternalDetour(PacketDispatcher* dispatcher, uint targetID, nint packet)
	{
		packet -= 16;
		short opcode = Marshal.ReadInt16(packet, 18);
		LogKnownReceivedPacket(opcode, targetID);
		bool isPrevented = false;
		if (methodsCollection.TryGetValue(typeof(PreReceivePacketDelegate), out ImmutableList<Delegate> value))
		{
			foreach (PreReceivePacketDelegate item in value)
			{
				item(ref isPrevented, opcode, ref packet);
				if (isPrevented)
				{
					return;
				}
			}
		}
		ReceivePacketInternalHook.Original(dispatcher, targetID, packet + 16);
		if (!methodsCollection.TryGetValue(typeof(PostReceivePacketDelegate), out ImmutableList<Delegate> value2))
		{
			return;
		}
		foreach (PostReceivePacketDelegate item2 in value2)
		{
			item2(opcode, packet);
		}
	}

	private void LogKnownReceivedPacket(int opcode, uint targetID)
	{
		if (Config.ShowReceivedPacketOpcodeLog)
		{
			DLog.Debug($"[Game Packet Manager] [下行] 操作码: {opcode} / 目标: 0x{targetID:X8}");
		}
	}

	private unsafe void LogKnownGamePacket(int opcode, byte* packet, bool isPrioritize)
	{
		if (Config.ShowGamePacketOpcodeLog)
		{
			DLog.Debug($"[Game Packet Manager] [上行] 操作码: {opcode} / 长度: {((uint*)packet)[2]} / 优先级: {isPrioritize}");
		}
		if (PacketHandlers.TryGetValue(opcode, out PacketHandler value) && value.ShouldLog())
		{
			value.Log(packet);
		}
	}

	private unsafe static void LogPacket<T>(byte* packet) where T : unmanaged, IUpstreamPacket
	{
		DLog.Debug("[Game Packet Manager] " + ((T*)packet)->Log());
	}

	private unsafe static void HandlePacketPriority(ref bool isPrioritize)
	{
		if (!isPrioritize && !((Conditions)Conditions.Instance()).Gathering && Addons.FreeCompanyChest == null)
		{
			isPrioritize = true;
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
}
