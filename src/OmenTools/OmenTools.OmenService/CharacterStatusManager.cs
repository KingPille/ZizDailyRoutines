using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Dalamud.Hooking;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class CharacterStatusManager : OmenServiceBase<CharacterStatusManager>
{
	public delegate void GainStatusDelegate(IBattleChara player, ushort id, ushort param, ushort stackCount, TimeSpan remainingTime, ulong sourceID);

	public delegate void LoseStatusDelegate(IBattleChara player, ushort id, ushort param, ushort stackCount, ulong sourceID);

	private unsafe delegate void OnGainStatusDelegate(BattleChara** player, ushort statusID, float remainingTime, ushort statusParam, ulong sourceID, ushort stackCount);

	private unsafe delegate void OnLoseStatusDelegate(BattleChara** player, ushort statusID, ushort statusParam, ulong sourceID, ushort stackCount);

	public class PlayerStatusManagerConfig : OmenServiceConfig
	{
		public bool ShowGainStatusLog;

		public bool ShowLoseStatusLog;

		public void Save()
		{
			this.Save(DService.Instance().GetOmenService<GameResourceManager>());
		}
	}

	private static readonly CompSig OnGainStatusSig = new CompSig("48 8B C4 55 57 41 54 41 56");

	private Hook<OnGainStatusDelegate>? OnGainStatusHook;

	private static readonly CompSig OnLoseStatusSig = new CompSig("E8 ?? ?? ?? ?? FF C6 48 83 C3 10 83 FE 3C");

	private Hook<OnLoseStatusDelegate>? OnLoseStatusHook;

	private readonly ConcurrentDictionary<Type, ImmutableList<Delegate>> methodsCollection = new ConcurrentDictionary<Type, ImmutableList<Delegate>>();

	public PlayerStatusManagerConfig Config { get; private set; }

	protected unsafe override void Init()
	{
		Config = LoadConfig<PlayerStatusManagerConfig>() ?? new PlayerStatusManagerConfig();
		if (OnGainStatusHook == null)
		{
			OnGainStatusHook = OnGainStatusSig.GetHook<OnGainStatusDelegate>(OnGainStatusDetour);
		}
		OnGainStatusHook.Enable();
		if (OnLoseStatusHook == null)
		{
			OnLoseStatusHook = OnLoseStatusSig.GetHook<OnLoseStatusDelegate>(OnLoseStatusDetour);
		}
		OnLoseStatusHook.Enable();
	}

	protected override void Uninit()
	{
		OnGainStatusHook?.Dispose();
		OnGainStatusHook = null;
		OnLoseStatusHook?.Dispose();
		OnLoseStatusHook = null;
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

	public bool RegGain(GainStatusDelegate method, params GainStatusDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool RegLose(LoseStatusDelegate method, params LoseStatusDelegate[] methods)
	{
		return RegisterGeneric(method, methods);
	}

	public bool Unreg(params GainStatusDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	public bool Unreg(params LoseStatusDelegate[] methods)
	{
		return UnregisterGeneric(methods);
	}

	private unsafe void OnGainStatusDetour(BattleChara** player, ushort statusID, float remainingTime, ushort statusParam, ulong sourceID, ushort stackCount)
	{
		OnGainStatusHook.Original(player, statusID, remainingTime, statusParam, sourceID, stackCount);
		if (statusID == 0 || player == null || *player == null)
		{
			return;
		}
		IBattleChara battleChara = IBattleChara.Create((nint)(*player));
		if (battleChara == null)
		{
			return;
		}
		if (Config.ShowGainStatusLog)
		{
			string value = ((sourceID == 3758096384u) ? "本地对象" : (IOmenDalamudService<IObjectTable>.Instance().SearchByEntityID((uint)sourceID)?.Name.ToString() ?? "未知对象"));
			DLog.Debug($"[Character Status Manager] Gain Status\n玩家: {battleChara.Name} ({battleChara.EntityID})\n状态: {LuminaWrapper.GetStatusName(statusID)} ({statusID})\n剩余时间: {remainingTime:F1} 秒 | 参数: {statusParam} | 层数: {stackCount}\n来源: {value} ({sourceID})");
		}
		if (!methodsCollection.TryGetValue(typeof(GainStatusDelegate), out ImmutableList<Delegate> value2))
		{
			return;
		}
		foreach (GainStatusDelegate item in value2)
		{
			item(battleChara, statusID, statusParam, stackCount, TimeSpan.FromSeconds(remainingTime), sourceID);
		}
	}

	private unsafe void OnLoseStatusDetour(BattleChara** player, ushort statusID, ushort statusParam, ulong sourceID, ushort stackCount)
	{
		OnLoseStatusHook.Original(player, statusID, statusParam, sourceID, stackCount);
		if (statusID == 0 || player == null || *player == null)
		{
			return;
		}
		IBattleChara battleChara = IBattleChara.Create((nint)(*player));
		if (battleChara == null)
		{
			return;
		}
		if (Config.ShowLoseStatusLog)
		{
			string value = ((sourceID == 3758096384u) ? "本地对象" : (IOmenDalamudService<IObjectTable>.Instance().SearchByEntityID((uint)sourceID)?.Name.ToString() ?? "未知对象"));
			DLog.Debug($"[Character Status Manager] Lose Status\n玩家: {battleChara.Name} ({battleChara.EntityID})\n状态: {LuminaWrapper.GetStatusName(statusID)} ({statusID})\n参数: {statusParam} | 层数: {stackCount}\n来源: {value} ({sourceID})");
		}
		if (!methodsCollection.TryGetValue(typeof(LoseStatusDelegate), out ImmutableList<Delegate> value2))
		{
			return;
		}
		foreach (LoseStatusDelegate item in value2)
		{
			item(battleChara, statusID, statusParam, stackCount, sourceID);
		}
	}
}
