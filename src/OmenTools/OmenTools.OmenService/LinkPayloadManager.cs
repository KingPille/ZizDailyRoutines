using System;
using System.Collections.Concurrent;
using System.Threading;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class LinkPayloadManager : OmenServiceBase<LinkPayloadManager>
{
	private readonly ConcurrentDictionary<uint, DalamudLinkPayload> distributedPayloads = new ConcurrentDictionary<uint, DalamudLinkPayload>();

	private long lastID = -1L;

	public DalamudLinkPayload Reg(Action<uint, SeString> commandAction, out uint id)
	{
		id = GetUniqueID();
		DalamudLinkPayload val = DalamudServiceExtension.Instance<IChatGui>().AddChatLinkHandler(id, commandAction);
		distributedPayloads.TryAdd(id, val);
		return val;
	}

	public bool Unreg(uint id)
	{
		if (!distributedPayloads.TryRemove(id, out DalamudLinkPayload _))
		{
			return false;
		}
		DalamudServiceExtension.Instance<IChatGui>().RemoveChatLinkHandler(id);
		return true;
	}

	public bool TryGetPayload(uint id, out DalamudLinkPayload? payload)
	{
		return distributedPayloads.TryGetValue(id, out payload);
	}

	protected override void Init()
	{
		distributedPayloads.Clear();
		Interlocked.Exchange(ref lastID, -1L);
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IChatGui>().RemoveChatLinkHandler();
		distributedPayloads.Clear();
	}

	private uint GetUniqueID()
	{
		uint num;
		do
		{
			num = (uint)Interlocked.Increment(ref lastID);
		}
		while (distributedPayloads.ContainsKey(num));
		return num;
	}
}
